using Igris.Mvvm;
using Igris.NovelCreator.Databases;
using Igris.NovelCreator.Models;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Windows.Input;

namespace Igris.NovelCreator.ViewModels
{
    public class VolumeCreationViewModel : ViewModelBase
    {
        private readonly IDialogCoordinator dialog;
        private readonly bool modify;
        private readonly int volumeIndex;
        private readonly string oldTitle;

        public NovelGestionViewModel NovelGestionViewModel { get; }

        public string IsModify { get => GetProperty(() => IsModify); set => SetProperty(() => IsModify, value); }
        public Volume Volume { get => GetProperty(() => Volume); set => SetProperty(() => Volume, value); }

        public ICommand AddModifyCommand { get; }
        public ICommand CancelCommand { get; }

        public VolumeCreationViewModel(NovelGestionViewModel vm, Volume volume = null)
        {
            dialog = DialogCoordinator.Instance;
            NovelGestionViewModel = vm ?? throw new ArgumentNullException(nameof(vm));
            IsModify = volume == null ? "Ajouter" : "Modifier";

            AddModifyCommand = new DelegateCommand(AddModify, AllowAddModify);
            CancelCommand = new DelegateCommand(Cancel);

            if (volume != null)
            {
                volumeIndex = vm.Novel.Volumes.IndexOf(volume);
                modify = true;
                oldTitle = volume.Title;
                Volume = new()
                {
                    Id = volume.Id,
                    Title = volume.Title,
                    Type = volume.Type,
                    Chapters = volume.Chapters
                };
            }
            else
            {
                Volume = new();
                Volume.Id = NovelGestionViewModel.Novel.Volumes.Count;
            }
        }

        private bool AllowAddModify()
        {
            return !string.IsNullOrEmpty(Volume.Title);
        }

        private async void AddModify()
        {
            if (oldTitle != Volume.Title && NovelGestionViewModel.Novel.Volumes.Any(v => v.Title == Volume.Title))
            {
                await dialog.ShowMessageAsync(this, "Erreur", "Ce titre de volume existe déjà dans ce roman");
                return;
            }
            if (modify)
            {
                if (volumeIndex <= -1)
                {
                    return;
                }
                NovelGestionViewModel.Novel.Volumes[volumeIndex] = Volume;
                Database.UpdateVolume(Volume, oldTitle, NovelGestionViewModel.Novel.Title);
            }
            else
            {
                if (Volume.Type == VolumeType.Spécial)
                {
                    Volume.Id = NovelGestionViewModel.Novel.Volumes.Count(v => Volume.Type == VolumeType.Spécial);
                }
                NovelGestionViewModel.Novel.Volumes.Add(Volume);
                Database.AddVolume(Volume, NovelGestionViewModel.Novel.Title);
            }
            NovelGestionViewModel.Novel.Volumes = new(NovelGestionViewModel.Novel.Volumes.OrderByDescending(v => v.Type).ThenByDescending(v => v.Id).ToList());
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
