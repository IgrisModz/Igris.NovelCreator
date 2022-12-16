using Igris.NovelCreator.Models;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Igris.NovelCreator.Databases
{
    public static class Database
    {
        public static byte[] BitmapImageToByteArray(BitmapImage bitmap)
        {
            byte[] data;
            PngBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (MemoryStream ms = new())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        public static BitmapImage GetThumbnails(object image)
        {
            BitmapImage bitmap = new();
            if (image is not byte[] imageBytes)
            {
                return bitmap;
            }
            bitmap.BeginInit();
            bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = new MemoryStream(imageBytes);
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        public static void CreateNovelTable()
        {
            SqlCommand command = new("SELECT name FROM sysobjects WHERE name='Novels'", Connection.Instance);
            Connection.Instance.Open();
            object name = command.ExecuteScalar();
            if (name != null && name.ToString() == "Novels")
            {
                command.Dispose();
                Connection.Instance.Close();
                return;
            }
            command.CommandText = "CREATE TABLE Novels (Id INT NOT NULL PRIMARY KEY IDENTITY(1,1), Title VARCHAR(MAX), AlternativeTitle VARCHAR(MAX), Author VARCHAR(MAX), Synopsy TEXT, Genres VARCHAR(MAX), OnGoing BIT, Cover IMAGE, CreationDate DATETIME)";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE TABLE Volumes (Id INT, Title VARCHAR(MAX), Type VARCHAR(MAX), NovelTitle VARCHAR(MAX))";
            command.ExecuteNonQuery();
            command.CommandText = "CREATE TABLE Chapters (Id REAL, Title VARCHAR(MAX), Text TEXT, AuthorDescription VARCHAR(MAX), CreationDate DATETIME, Type VARCHAR(MAX), NovelTitle VARCHAR(MAX), VolumeTitle VARCHAR(MAX))";
            command.ExecuteNonQuery();
            command.Dispose();
            Connection.Instance.Close();
        }

        public static ObservableCollection<Novel> GetNovels()
        {
            ObservableCollection<Novel> novels = new();
            SqlCommand command = new("SELECT * FROM Novels", Connection.Instance);
            Connection.Instance.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string genre = string.IsNullOrEmpty(reader["Genres"].ToString()) ? null : reader["Genres"].ToString();
                string[] genres = string.IsNullOrEmpty(genre) ? null : genre.Split(',');

                novels.Add(new()
                {
                    Title = reader["Title"].ToString(),
                    AlternativeTitle = reader["AlternativeTitle"].ToString(),
                    Author = reader["Author"].ToString(),
                    Synopsy = reader["Synopsy"].ToString(),
                    Volumes = new(),
                    Genres = genres == null ? new() : new(genres),
                    OnGoing = Convert.ToBoolean(reader["OnGoing"]),
                    Cover = GetThumbnails(reader["Cover"]),
                    CreationDate = string.IsNullOrEmpty(reader["CreationDate"].ToString()) ? DateTime.Now : DateTime.Parse(reader["CreationDate"].ToString())
                });
            }
            reader.DisposeAsync();
            command.CommandText = "SELECT * FROM Volumes ORDER BY Id DESC";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                for (int i = 0; i < novels.Count; i++)
                {
                    if (novels[i].Title == reader["NovelTitle"].ToString())
                    {
                        novels[i].Volumes.Add(new()
                        {
                            Id = i,
                            Title = reader["Title"].ToString(),
                            Type = (VolumeType)Enum.Parse(typeof(VolumeType), reader["Type"].ToString()),
                            Chapters = new()
                        });
                    }
                }
            }
            reader.DisposeAsync();
            command.CommandText = "SELECT * FROM Chapters ORDER BY Id DESC";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                for (int x = 0; x < novels.Count; x++)
                {
                    if (novels[x].Title == reader["NovelTitle"].ToString())
                    {
                        for (int y = 0; y < novels[x].Volumes.Count; y++)
                        {
                            if (novels[x].Volumes[y].Title == reader["VolumeTitle"].ToString())
                            {
                                novels[x].Volumes[y].Chapters.Add(new()
                                {
                                    Id = Convert.ToSingle(reader["Id"].ToString()),
                                    Title = reader["Title"].ToString(),
                                    Text = reader["Text"].ToString(),
                                    AuthorDescription = reader["AuthorDescription"].ToString(),
                                    CreationDate = DateTime.Parse(reader["CreationDate"].ToString()),
                                    Type = (ChapterType)Enum.Parse(typeof(ChapterType), reader["Type"].ToString()),
                                });
                            }
                        }
                    }
                }
            }
            command.Dispose();
            Connection.Instance.Close();
            return novels;
        }

        public static void AddNovel(Novel novel)
        {
            string genre = "";
            foreach (string genres in novel.Genres)
            {
                genre += genres + ",";
            }
            genre = genre.Length > 0 && genre[^1..] == "," ? genre[0..^1] : genre;
            SqlCommand command = new("INSERT INTO Novels (Title, AlternativeTitle, Author, Synopsy, Genres, OnGoing, Cover, CreationDate) VALUES (@Title, @AlternativeTitle, @Author, @Synopsy, @Genres, @OnGoing, @Cover, @CreationDate)", Connection.Instance);
            command.Parameters.AddWithValue("@Title", novel.Title);
            command.Parameters.AddWithValue("@AlternativeTitle", novel.AlternativeTitle);
            command.Parameters.AddWithValue("@Author", novel.Author);
            command.Parameters.AddWithValue("@Synopsy", novel.Synopsy);
            command.Parameters.AddWithValue("@Genres", genre);
            command.Parameters.AddWithValue("@OnGoing", novel.OnGoing);
            command.Parameters.AddWithValue("@Cover", BitmapImageToByteArray(novel.Cover));
            command.Parameters.AddWithValue("@CreationDate", novel.CreationDate);
            Connection.Instance.Open();
            command.ExecuteNonQuery();
            command.Dispose();
            Connection.Instance.Close();
        }

        public static void AddVolume(Volume volume, string novelTitle)
        {
            SqlCommand command = new("INSERT INTO Volumes (Id, Title, Type, NovelTitle) VALUES (@Id, @Title, @Type, @NovelTitle)", Connection.Instance);
            command.Parameters.AddWithValue("@Id", volume.Id);
            command.Parameters.AddWithValue("@Title", volume.Title);
            command.Parameters.AddWithValue("@Type", Enum.GetName(volume.Type));
            command.Parameters.AddWithValue("@NovelTitle", novelTitle);
            Connection.Instance.Open();
            command.ExecuteNonQuery();
            command.Dispose();
            Connection.Instance.Close();
        }

        public static void AddChapter(Chapter chapter, string novelTitle, string volumeTitle)
        {
            SqlCommand command = new("INSERT INTO Chapters (Id, Title, Text, AuthorDescription, CreationDate, Type, NovelTitle, VolumeTitle) VALUES (@Id, @Title, @Text, @AuthorDescription, @CreationDate, @Type, @NovelTitle, @VolumeTitle)", Connection.Instance);
            command.Parameters.AddWithValue("@Id", chapter.Id);
            command.Parameters.AddWithValue("@Title", chapter.Title);
            command.Parameters.AddWithValue("@Text", chapter.Text);
            command.Parameters.AddWithValue("@AuthorDescription", chapter.AuthorDescription);
            command.Parameters.AddWithValue("@CreationDate", chapter.CreationDate);
            command.Parameters.AddWithValue("@Type", Enum.GetName(chapter.Type));
            command.Parameters.AddWithValue("@NovelTitle", novelTitle);
            command.Parameters.AddWithValue("@VolumeTitle", volumeTitle);
            Connection.Instance.Open();
            command.ExecuteNonQuery();
            command.Dispose();
            Connection.Instance.Close();
        }

        public static void UpdateNovel(Novel novel, string oldTitle)
        {
            string genre = "";
            foreach (string genres in novel.Genres)
            {
                genre += genres + ",";
            }
            genre = genre.Length > 0 && genre[^1..] == "," ? genre[0..^1] : genre;
            SqlCommand command = new("UPDATE Novels SET Title=@Title, AlternativeTitle=@AlternativeTitle, Author=@Author, Synopsy=@Synopsy, Genres=@Genres, OnGoing=@OnGoing, Cover=@Cover, CreationDate=@CreationDate WHERE Title=@OldTitle", Connection.Instance);
            command.Parameters.AddWithValue("@Title", novel.Title);
            command.Parameters.AddWithValue("@AlternativeTitle", novel.AlternativeTitle);
            command.Parameters.AddWithValue("@Author", novel.Author);
            command.Parameters.AddWithValue("@Synopsy", novel.Synopsy);
            command.Parameters.AddWithValue("@Genres", genre);
            command.Parameters.AddWithValue("@OnGoing", novel.OnGoing);
            command.Parameters.AddWithValue("@Cover", BitmapImageToByteArray(novel.Cover));
            command.Parameters.AddWithValue("@CreationDate", novel.CreationDate);
            command.Parameters.AddWithValue("@OldTitle", oldTitle);
            Connection.Instance.Open();
            command.ExecuteNonQuery();
            command.Parameters.Clear();
            command.CommandText = "UPDATE Volumes SET NovelTitle=@NovelTitle WHERE NovelTitle=@OldTitle";
            command.Parameters.AddWithValue("@NovelTitle", novel.Title);
            command.Parameters.AddWithValue("@OldTitle", oldTitle);
            command.ExecuteNonQuery();
            command.Parameters.Clear();
            command.CommandText = "UPDATE Chapters SET NovelTitle=@NovelTitle WHERE NovelTitle=@OldTitle";
            command.Parameters.AddWithValue("@NovelTitle", novel.Title);
            command.Parameters.AddWithValue("@OldTitle", oldTitle);
            command.ExecuteNonQuery();
            command.Dispose();
            Connection.Instance.Close();
        }

        public static void UpdateVolume(Volume volume, string oldTitle, string novelTitle)
        {
            SqlCommand command = new("UPDATE Volumes SET Title=@Title, Type=@Type, NovelTitle=@NovelTitle WHERE Id=@Id AND Title=@OldTitle AND NovelTitle=@NovelTitle", Connection.Instance);
            command.Parameters.AddWithValue("@Title", volume.Title);
            command.Parameters.AddWithValue("@Type", Enum.GetName(volume.Type));
            command.Parameters.AddWithValue("@Id", volume.Id);
            command.Parameters.AddWithValue("@OldTitle", oldTitle);
            command.Parameters.AddWithValue("@NovelTitle", novelTitle);
            Connection.Instance.Open();
            command.ExecuteNonQuery();
            command.Parameters.Clear();
            command.CommandText = "UPDATE Chapters SET VolumeTitle=@VolumeTitle WHERE NovelTitle=@NovelTitle AND VolumeTitle=@OldTitle";
            command.Parameters.AddWithValue("@NovelTitle", novelTitle);
            command.Parameters.AddWithValue("@VolumeTitle", volume.Title);
            command.Parameters.AddWithValue("@OldTitle", oldTitle);
            command.ExecuteNonQuery();
            command.Dispose();
            Connection.Instance.Close();
        }

        public static void UpdateChapter(Chapter chapter, float oldId, string oldTitle, string novelTitle, string volumeTitle)
        {
            SqlCommand command = new("UPDATE Chapters SET Id=@Id, Title=@Title, Text=@Text, AuthorDescription=@AuthorDescription, CreationDate=@CreationDate, Type=@Type WHERE Id=@OldId AND Title=@OldTitle AND NovelTitle=@NovelTitle AND VolumeTitle=@VolumeTitle", Connection.Instance);
            command.Parameters.AddWithValue("@Id", chapter.Id);
            command.Parameters.AddWithValue("@Title", chapter.Title);
            command.Parameters.AddWithValue("@Text", chapter.Text);
            command.Parameters.AddWithValue("@AuthorDescription", chapter.AuthorDescription);
            command.Parameters.AddWithValue("@CreationDate", chapter.CreationDate);
            command.Parameters.AddWithValue("@Type", Enum.GetName(chapter.Type));
            command.Parameters.AddWithValue("@OldId", oldId);
            command.Parameters.AddWithValue("@OldTitle", oldTitle);
            command.Parameters.AddWithValue("@NovelTitle", novelTitle);
            command.Parameters.AddWithValue("@VolumeTitle", volumeTitle);
            Connection.Instance.Open();
            command.ExecuteNonQuery();
            command.Dispose();
            Connection.Instance.Close();
        }

        public static void DeleteNovel(Novel novel)
        {
            SqlCommand command = new("DELETE FROM Novels WHERE Title=@Title", Connection.Instance);
            command.Parameters.AddWithValue("@Title", novel.Title);
            Connection.Instance.Open();
            command.ExecuteNonQuery();
            command.Parameters.Clear();
            command.CommandText = "DELETE FROM Volumes WHERE NovelTitle=@NovelTitle";
            command.Parameters.AddWithValue("@NovelTitle", novel.Title);
            command.ExecuteNonQuery();
            command.Parameters.Clear();
            command.CommandText = "DELETE FROM Chapters WHERE NovelTitle=@NovelTitle";
            command.Parameters.AddWithValue("@NovelTitle", novel.Title);
            command.ExecuteNonQuery();
            command.Dispose();
            Connection.Instance.Close();
        }

        public static void DeleteVolume(Volume volume, string novelTitle)
        {
            SqlCommand command = new("DELETE FROM Volumes WHERE Id=@Id AND Title=@Title AND NovelTitle=@NovelTitle", Connection.Instance);
            command.Parameters.AddWithValue("@Id", volume.Id);
            command.Parameters.AddWithValue("@Title", volume.Title);
            command.Parameters.AddWithValue("@NovelTitle", novelTitle);
            Connection.Instance.Open();
            command.ExecuteNonQuery();
            command.Parameters.Clear();
            command.CommandText = "DELETE FROM Chapters WHERE NovelTitle=@NovelTitle AND VolumeTitle=@VolumeTitle";
            command.Parameters.AddWithValue("@NovelTitle", novelTitle);
            command.Parameters.AddWithValue("@VolumeTitle", volume.Title);
            command.ExecuteNonQuery();
            command.Dispose();
            Connection.Instance.Close();
        }

        public static void DeleteChapter(Chapter chapter, string novelTitle, string volumeTitle)
        {
            SqlCommand command = new("DELETE FROM Chapters WHERE Id=@Id AND Title=@Title AND NovelTitle=@NovelTitle AND VolumeTitle=@VolumeTitle", Connection.Instance);
            command.Parameters.AddWithValue("@Id", chapter.Id);
            command.Parameters.AddWithValue("@Title", chapter.Title);
            command.Parameters.AddWithValue("@NovelTitle", novelTitle);
            command.Parameters.AddWithValue("@VolumeTitle", volumeTitle);
            Connection.Instance.Open();
            command.ExecuteNonQuery();
            command.Dispose();
            Connection.Instance.Close();
        }

        public static void DeleteAllChapter()
        {
            SqlCommand command = new("DELETE FROM Chapters", Connection.Instance);
            Connection.Instance.Open();
            command.ExecuteNonQuery();
            command.Dispose();
            Connection.Instance.Close();
        }

        public static void AddIdColumnToVolume()
        {
            SqlCommand command = new("ALTER TABLE Volumes ADD Id INT", Connection.Instance);
            Connection.Instance.Open();
            command.ExecuteNonQuery();
            command.Dispose();
            Connection.Instance.Close();
        }
    }
}
