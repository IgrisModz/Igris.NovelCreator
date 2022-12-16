using Igris.Mvvm;
using Igris.NovelCreator.Models;
using Igris.NovelCreator.Views;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace Igris.NovelCreator.ViewModels
{
    public class ReadingViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Novel> SavedNovels;

        public bool ContentIsOpen { get => GetProperty(() => ContentIsOpen); set => SetProperty(() => ContentIsOpen, value); }
        public UserControl ContentControl { get => GetProperty(() => ContentControl); set => SetProperty(() => ContentControl, value); }
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

        public ICommand OpenNovelCommand { get; }

        public ReadingViewModel(ObservableCollection<Novel> novels)
        {
            Novels = novels ?? throw new ArgumentNullException(nameof(novels));
            SavedNovels = novels ?? throw new ArgumentNullException(nameof(novels));
            OpenNovelCommand = new DelegateCommand<Novel>(n => OpenNovel(n));
        }

        private ReadingViewModel()
        {
        }

        private void OpenNovel(Novel novel)
        {
            ContentControl = new ChapterListView(this, novel);
            ContentIsOpen = true;
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
