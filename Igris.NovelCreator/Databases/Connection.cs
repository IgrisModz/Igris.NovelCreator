using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace Igris.NovelCreator.Databases
{
    public sealed class Connection
    {
        private static SqlConnection _instance;
        private static readonly object _lock = new();

        private  static void CreateDataBase(string path, string name)
        {
            string databaseName = Path.GetFileNameWithoutExtension(name);
            using SqlConnection connection = new("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;");
            connection.Open();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = $"CREATE DATABASE {databaseName} ON PRIMARY (NAME={databaseName}, FILENAME='{path}\\{name}')";
                command.ExecuteNonQuery();

                command.CommandText = $"EXEC sp_detach_db '{databaseName}', 'true'";
                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        public static SqlConnection Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        string fileName = "Igris_NovelCreator.mdf";
                        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        if (!File.Exists($@"{path}\{fileName}"))
                        {
                            CreateDataBase(path, fileName);
                        }
                        _instance = new SqlConnection(@$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={path}\{fileName};Integrated Security=True");
                    }
                    return _instance;
                }
            }
        }

        private Connection()
        {
        }
    }
}
