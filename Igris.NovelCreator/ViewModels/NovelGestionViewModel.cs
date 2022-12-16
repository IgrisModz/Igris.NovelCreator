using Igris.Mvvm;
using Igris.NovelCreator.Databases;
using Igris.NovelCreator.Models;
using Igris.NovelCreator.Views;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Igris.NovelCreator.ViewModels
{
    public class NovelGestionViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Volume> SavedVolumes;

        public CreationViewModel CreationViewModel { get; }

        public bool ContentIsOpen { get => GetProperty(() => ContentIsOpen); set => SetProperty(() => ContentIsOpen, value); }
        public UserControl NovelGestionContent { get => GetProperty(() => NovelGestionContent); set => SetProperty(() => NovelGestionContent, value); }
        public Novel Novel { get => GetProperty(() => Novel); set => SetProperty(() => Novel, value); }
        public string IsModify { get => GetProperty(() => IsModify); set => SetProperty(() => IsModify, value); }
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

        public ICommand OpenVolumeCreationCommand { get; }
        public ICommand CustomVolumeCommand { get; }
        public ICommand DeleteVolumeCommand { get; }
        public ICommand OpenChapterCreationCommand { get; }
        public ICommand CustomChapterCommand { get; }
        public ICommand DeleteChapterCommand { get; }
        public ICommand CloseCommand { get; }

        public NovelGestionViewModel(CreationViewModel vm, Novel novel)
        {
            CreationViewModel = vm ?? throw new ArgumentNullException(nameof(vm));
            Novel = novel ?? throw new ArgumentNullException(nameof(novel));
            Novel.Volumes = novel.Volumes ?? new ObservableCollection<Volume>();
            SavedVolumes = novel.Volumes ?? new ObservableCollection<Volume>();

            OpenVolumeCreationCommand = new DelegateCommand(OpenVolumeCreation);
            CustomVolumeCommand = new DelegateCommand<Volume>(v => CustomVolume(v));
            DeleteVolumeCommand = new DelegateCommand<Volume>(v => DeleteVolume(v));
            OpenChapterCreationCommand = new DelegateCommand<Volume>(v => OpenChapterCreation(v));
            CustomChapterCommand = new DelegateCommand<Chapter>(c => CustomChapter(c));
            DeleteChapterCommand = new DelegateCommand<Chapter>(c => DeleteChapter(c));
            CloseCommand = new DelegateCommand(Close);
        }

        private void OpenVolumeCreation()
        {
            NovelGestionContent = new VolumeCreationView(this);
            ContentIsOpen = true;
        }

        private void CustomVolume(Volume volume)
        {
            NovelGestionContent = new VolumeCreationView(this, volume);
            ContentIsOpen = true;
        }

        private void DeleteVolume(Volume volume)
        {
            int volumeIndex = Novel.Volumes.IndexOf(volume);
            if (volumeIndex > -1)
            {
                Novel.Volumes.RemoveAt(volumeIndex);
                Database.DeleteVolume(volume, Novel.Title);
            }
        }

        private void OpenChapterCreation(Volume volume)
        {
            NovelGestionContent = new ChapterCreationView(this, volume);
            ContentIsOpen = true;
        }

        private void CustomChapter(Chapter chapter)
        {
            NovelGestionContent = new ChapterCreationView(this, chapter);
            ContentIsOpen = true;
        }

        private void DeleteChapter(Chapter chapter)
        {
            Volume volume = Novel.Volumes.FirstOrDefault(v => v.Chapters.Contains(chapter));
            int volumeIndex = Novel.Volumes.IndexOf(volume);
            int chapterIndex = Novel.Volumes[volumeIndex].Chapters.IndexOf(chapter);
            if (chapterIndex > -1)
            {
                Novel.Volumes[volumeIndex].Chapters.RemoveAt(chapterIndex);
                Database.DeleteChapter(chapter, Novel.Title, volume.Title);
            }
        }

        private void Close()
        {
            CreationViewModel.CreationContent = null;
            CreationViewModel.ContentIsOpen = false;
        }

        private void ApplyFilterText(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Novel.Volumes = SavedVolumes;
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
