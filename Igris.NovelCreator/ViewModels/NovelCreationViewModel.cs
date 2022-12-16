using Igris.Mvvm;
using Igris.NovelCreator.Databases;
using Igris.NovelCreator.Models;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Igris.NovelCreator.ViewModels
{
    public class NovelCreationViewModel : ViewModelBase
    {
        private readonly IDialogCoordinator dialog;
        private readonly bool modify;
        private readonly string oldTitle;
        private readonly int novelIndex;

        public CreationViewModel CreationViewModel { get; }

        public string IsModify { get => GetProperty(() => IsModify); set => SetProperty(() => IsModify, value); }
        public Novel Novel { get => GetProperty(() => Novel); set => SetProperty(() => Novel, value); }

        public ICommand AddModifyCoverCommand { get; }
        public ICommand AddGenresCommand { get; }
        public ICommand DeleteAllGenresCommand { get; }
        public ICommand AddModifyNovelCommand { get; }
        public ICommand CancelCommand { get; }

        public NovelCreationViewModel(CreationViewModel vm, Novel novel = null)
        {
            dialog = DialogCoordinator.Instance;
            CreationViewModel = vm ?? throw new ArgumentNullException(nameof(vm));
            IsModify = novel == null ? "Ajouter" : "Modifier";

            AddModifyCoverCommand = new DelegateCommand(AddModifyCover);
            AddGenresCommand = new DelegateCommand<string>(g => AddGenres(g));
            DeleteAllGenresCommand = new DelegateCommand(DeleteAllGenres);
            AddModifyNovelCommand = new DelegateCommand(AddModifyNovel, AllowAddModify);
            CancelCommand = new DelegateCommand(Cancel);

            if (novel != null)
            {
                novelIndex = vm.Novels.IndexOf(novel);
                modify = true;
                oldTitle = novel.Title;
                Novel = new()
                {
                    Title = novel.Title,
                    AlternativeTitle = novel.AlternativeTitle,
                    Author = novel.Author,
                    Synopsy = novel.Synopsy,
                    Cover = novel.Cover,
                    Genres = novel.Genres,
                    OnGoing = novel.OnGoing,
                    Volumes = novel.Volumes,
                    CreationDate = novel.CreationDate
                };
            }
            else
            {
                Novel = new Novel() { Cover = new BitmapImage(new Uri("pack://application:,,,/Igris.NovelCreator;Component/Images/ImageNA.png")), Genres = new() };
            }
        }

        private NovelCreationViewModel() { }

        private bool AllowAddModify()
        {
            return !string.IsNullOrEmpty(Novel.Title) && !string.IsNullOrEmpty(Novel.AlternativeTitle) && !string.IsNullOrEmpty(Novel.Author) && !string.IsNullOrEmpty(Novel.Synopsy);
        }

        private void AddModifyCover()
        {
            OpenFileDialog dialog = new()
            {
                Title = "Sélectionner un fichier",
                Filter = "Fichiers JPEG (*.JPG;*.JPEG;*.JPE;*.JFIF)|*.jpg;*.jpeg;*.jpe;*.jfif|Fichiers PNG (*.PNG)|*.png|Tous les Fichiers (*.*)|*.*",
                Multiselect = false
            };
            if ((bool)dialog.ShowDialog())
            {
                BitmapImage bitmapImage = new();
                bitmapImage.BeginInit();
                bitmapImage.DecodePixelWidth = 220;
                bitmapImage.DecodePixelHeight = 280;
                bitmapImage.UriSource = new Uri(dialog.FileName);
                bitmapImage.EndInit();
                Novel.Cover = bitmapImage;
            }
        }

        private void AddGenres(string genre)
        {
            if (!Novel.Genres.Any(prop => prop == genre))
            {
                Novel.Genres.Add(genre);
                Novel.Genres = new ObservableCollection<string>(Novel.Genres.OrderBy(q => q).ToList());
            }
        }

        private void DeleteAllGenres()
        {
            Novel.Genres.Clear();
        }

        private async void AddModifyNovel()
        {
            if (oldTitle != Novel.Title && CreationViewModel.Novels.Any(n => n.Title == Novel.Title))
            {
                await dialog.ShowMessageAsync(this, "Erreur", "Ce nom de roman existe déjà!");
                return;
            }
            if (modify)
            {
                if (novelIndex <= -1)
                {
                    return;
                }
                CreationViewModel.Novels[novelIndex] = Novel;
                Database.UpdateNovel(Novel, oldTitle);
            }
            else
            {
                Novel.CreationDate = DateTime.Now;
                CreationViewModel.Novels.Add(Novel);
                Database.AddNovel(Novel);
            }
            CreationViewModel.CreationContent = null;
            CreationViewModel.ContentIsOpen = false;
        }

        private void Cancel()
        {
            CreationViewModel.CreationContent = null;
            CreationViewModel.ContentIsOpen = false;
        }
    }
}
