using Igris.Mvvm;
using System.Collections.ObjectModel;

namespace Igris.NovelCreator.Models
{
    public class Volume : ViewModelBase
    {
        public int Id { get => GetProperty(() => Id); set => SetProperty(() => Id, value); }
        public string Title { get => GetProperty(() => Title); set => SetProperty(() => Title, value); }
        public VolumeType Type { get => GetProperty(() => Type); set => SetProperty(() => Type, value); }
        public ObservableCollection<Chapter> Chapters { get => GetProperty(() => Chapters); set => SetProperty(() => Chapters, value); }
    }
}
