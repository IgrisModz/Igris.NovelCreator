using Igris.Mvvm;
using Igris.NovelCreator.Databases;
using Igris.NovelCreator.Models;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Windows.Input;

namespace Igris.NovelCreator.ViewModels
{
    public class ChapterCreationViewModel : ViewModelBase
    {
        private readonly IDialogCoordinator dialog;
        private readonly bool modify;
        private readonly int chapterIndex;
        private readonly float oldId;
        private readonly string oldTitle;

        public NovelGestionViewModel NovelGestionViewModel { get; }

        public string IsModify { get => GetProperty(() => IsModify); set => SetProperty(() => IsModify, value); }
        public Volume Volume { get => GetProperty(() => Volume); set => SetProperty(() => Volume, value); }
        public Chapter Chapter { get => GetProperty(() => Chapter); set => SetProperty(() => Chapter, value); }

        public ICommand AddModifyCommand { get; }
        public ICommand CancelCommand { get; }

        public ChapterCreationViewModel(NovelGestionViewModel vm, Chapter chapter)
        {
            dialog = DialogCoordinator.Instance;
            NovelGestionViewModel = vm ?? throw new ArgumentNullException(nameof(vm));
            IsModify = "Modifier";

            AddModifyCommand = new DelegateCommand(AddModify);
            CancelCommand = new DelegateCommand(Cancel);

            Volume = vm.Novel.Volumes.FirstOrDefault(v => v.Chapters.Contains(chapter));
            chapterIndex = Volume.Chapters.IndexOf(chapter);
            oldId = chapter != null ? chapter.Id : default;
            oldTitle = chapter != null ? chapter.Title : string.Empty;
            Chapter = chapter != null ? new()
            {
                Id = chapter.Id,
                Title = chapter.Title,
                Type = chapter.Type,
                AuthorDescription = chapter.AuthorDescription,
                Text = chapter.Text,
                CreationDate = chapter.CreationDate
            } : new();
            modify = true;
        }

        public ChapterCreationViewModel(NovelGestionViewModel vm, Volume volume)
        {
            dialog = DialogCoordinator.Instance;
            NovelGestionViewModel = vm ?? throw new ArgumentNullException(nameof(vm));
            Volume = volume ?? new();
            Volume.Chapters = volume.Chapters ?? new();
            IsModify = "Ajouter";

            AddModifyCommand = new DelegateCommand(AddModify, AllowAddModify);
            CancelCommand = new DelegateCommand(Cancel);

            Chapter = new();
            Chapter.Id = Volume.Chapters.Count != 0 ? Volume.Chapters.FirstOrDefault().Id + 1 : 0;
            modify = false;
        }

        private bool AllowAddModify()
        {
            return !string.IsNullOrEmpty(Chapter.Title) && !string.IsNullOrEmpty(Chapter.AuthorDescription) && !string.IsNullOrEmpty(Chapter.Text);
        }

        private async void AddModify()
        {
            if (oldId != Chapter.Id && Volume.Chapters.Any(c => c.Id == Chapter.Id))
            {
                if (await dialog.ShowMessageAsync(this, "Erreur", "Cet identifiant de chapitre existe déjà dans ce volume, voulez-vous annuler les modifications et quitter?", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    Cancel();
                }
                return;
            }
            if (modify)
            {
                if (chapterIndex <= -1)
                {
                    return;
                }
                Volume.Chapters[chapterIndex] = Chapter;
                Database.UpdateChapter(Chapter, oldId, oldTitle, NovelGestionViewModel.Novel.Title, Volume.Title);
            }
            else
            {
                Chapter.CreationDate = DateTime.Now;
                Volume.Chapters.Add(Chapter);
                Database.AddChapter(Chapter, NovelGestionViewModel.Novel.Title, Volume.Title);
            }

            Volume.Chapters = new(Volume.Chapters.OrderByDescending(c => c.Id).ToList());
            NovelGestionViewModel.NovelGestionContent = null;
            NovelGestionViewModel.ContentIsOpen = false;
        }

        private void Cancel()
        {
            NovelGestionViewModel.NovelGestionContent = null;
            NovelGestionViewModel.ContentIsOpen = false;
        }
    }
}
