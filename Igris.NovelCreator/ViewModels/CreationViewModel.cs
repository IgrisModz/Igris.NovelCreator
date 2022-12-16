using Igris.Mvvm;
using Igris.NovelCreator.Databases;
using Igris.NovelCreator.Models;
using Igris.NovelCreator.Views;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace Igris.NovelCreator.ViewModels
{
    public class CreationViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Novel> SavedNovels;

        public bool ContentIsOpen { get => GetProperty(() => ContentIsOpen); set => SetProperty(() => ContentIsOpen, value); }
        public UserControl CreationContent { get => GetProperty(() => CreationContent); set => SetProperty(() => CreationContent, value); }
        public ObservableCollection<Novel> Novels { get => GetProperty(() => Novels); set => SetProperty(() => Novels, value); }
        public string FilterText
        {
            get => GetProperty(() => FilterText);
            set
            {
                if (SetProperty(() => FilterText, value))
                {
                    ApplyFilterText(value);
                }
            }
        }

        public ICommand CreateNewNovelCommand { get; }
        public ICommand OpenNovelCommand { get; }
        public ICommand ModifyNovelCommand { get; }
        public ICommand DeleteNovelCommand { get; }

        public CreationViewModel(ObservableCollection<Novel> novels)
        {
            Novels = novels ?? throw new ArgumentNullException(nameof(novels));
            SavedNovels = novels ?? throw new ArgumentNullException(nameof(novels));
            CreateNewNovelCommand = new DelegateCommand(CreateNewNovel);
            OpenNovelCommand = new DelegateCommand<Novel>(n => OpenNovel(n));
            ModifyNovelCommand = new DelegateCommand<Novel>(n => ModifyNovel(n));
            DeleteNovelCommand = new DelegateCommand<Novel>(n => DeleteNovel(n));
        }

        private CreationViewModel()
        {
        }

        private void CreateNewNovel()
        {
            CreationContent = new NovelCreationView(this);
            ContentIsOpen = true;
        }

        private void OpenNovel(Novel novel)
        {
            CreationContent = new NovelGestionView(this, novel);
            ContentIsOpen = true;
        }

        private void ModifyNovel(Novel novel)
        {
            CreationContent = new NovelCreationView(this, novel);
            ContentIsOpen = true;
        }

        private void DeleteNovel(Novel novel)
        {
            int index = Novels.IndexOf(novel);
            if (index > -1)
            {
                Novels.RemoveAt(index);
            }
            Database.DeleteNovel(novel);
        }

        private void ApplyFilterText(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Novels = SavedNovels;
            }
            ObservableCollection<Novel> savedNovels = new();
            foreach (Novel novel in SavedNovels)
            {
                if (novel.Title.ToLower(CultureInfo.CurrentCulture).Contains(value.ToLower(CultureInfo.CurrentCulture)))
                {
                    savedNovels.Add(novel);
                }
            }
            Novels = savedNovels;
        }
    }
}
