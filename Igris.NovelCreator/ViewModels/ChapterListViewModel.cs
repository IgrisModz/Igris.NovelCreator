using Igris.Mvvm;
using Igris.NovelCreator.Models;
using Igris.NovelCreator.Views;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace Igris.NovelCreator.ViewModels
{
    public class ChapterListViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Volume> SavedVolumes;

        public bool ContentIsOpen { get => GetProperty(() => ContentIsOpen); set => SetProperty(() => ContentIsOpen, value); }
        public ReadingViewModel ReadingViewModel { get => GetProperty(() => ReadingViewModel); set => SetProperty(() => ReadingViewModel, value); }
        public ChapterReadingView ContentControl { get => GetProperty(() => ContentControl); set => SetProperty(() => ContentControl, value); }
        public Novel Novel { get => GetProperty(() => Novel); set => SetProperty(() => Novel, value); }
        public string OnGoing { get => GetProperty(() => OnGoing); set => SetProperty(() => OnGoing, value); }

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

        public ICommand CloseCommand { get; }
        public ICommand OpenChapterCommand { get; }

        public ChapterListViewModel(ReadingViewModel vm, Novel novel)
        {
            ReadingViewModel = vm ?? throw new ArgumentNullException(nameof(vm));
            Novel = novel ?? throw new ArgumentNullException(nameof(novel));
            SavedVolumes = novel.Volumes ?? throw new ArgumentNullException(nameof(novel.Volumes));
            CloseCommand = new DelegateCommand(Close);
            OpenChapterCommand = new DelegateCommand<Chapter>(c => OpenChapter(c));
            OnGoing = novel.OnGoing ? "En cours" : "Terminé";
        }

        private ChapterListViewModel()
        {
        }

        private void Close()
        {
            ReadingViewModel.ContentControl = null;
            ReadingViewModel.ContentIsOpen = false;
        }

        private void OpenChapter(Chapter chapter)
        {
            Volume volume = Novel.Volumes.FirstOrDefault(v => v.Chapters.Contains(chapter));
            ContentControl = new(this, volume.Chapters, chapter);
            ContentIsOpen = true;
        }

        private void ApplyFilterText(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Novel.Volumes = SavedVolumes;
                return;
            }
            ObservableCollection<Volume> savedVolumes = new();
            ObservableCollection<Chapter> chapters = new();
            foreach (Volume volume in SavedVolumes)
            {
                if (volume.Title.ToLower(CultureInfo.CurrentCulture).Contains(value.ToLower(CultureInfo.CurrentCulture)))
                {
                    savedVolumes.Add(volume);
                }
                else
                {
                    foreach (Chapter chapter in volume.Chapters)
                    {
                        if (chapter.Title.ToLower(CultureInfo.CurrentCulture).Contains(value.ToLower(CultureInfo.CurrentCulture)) || chapter.Id.ToString(CultureInfo.CurrentCulture).Contains(value)
                            || $"Chapitre {chapter.Id}: {chapter.Title}".ToLower(CultureInfo.CurrentCulture).StartsWith(value.ToLower(CultureInfo.CurrentCulture), StringComparison.CurrentCulture))
                        {
                            chapters.Add(chapter);
                        }
                    }
                    savedVolumes.Add(new()
                    {
                        Id = volume.Id,
                        Title = volume.Title,
                        Type = volume.Type,
                        Chapters = chapters
                    });
                    chapters = new();
                }
            }
            Novel.Volumes = savedVolumes;
        }
    }
}
