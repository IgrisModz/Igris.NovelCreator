using Igris.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace Igris.NovelCreator.Models
{
    public class Novel : ViewModelBase
    {
        public string Title { get => GetProperty(() => Title); set => SetProperty(() => Title, value); }
        public string AlternativeTitle { get => GetProperty(() => AlternativeTitle); set => SetProperty(() => AlternativeTitle, value); }
        public string Author { get => GetProperty(() => Author); set => SetProperty(() => Author, value); }
        public string Synopsy { get => GetProperty(() => Synopsy); set => SetProperty(() => Synopsy, value); }
        public ObservableCollection<Volume> Volumes { get => GetProperty(() => Volumes); set => SetProperty(() => Volumes, value); }
        public ObservableCollection<string> Genres { get => GetProperty(() => Genres); set => SetProperty(() => Genres, value); }
        public bool OnGoing { get => GetProperty(() => OnGoing); set => SetProperty(() => OnGoing, value); }
        public BitmapImage Cover { get => GetProperty(() => Cover); set => SetProperty(() => Cover, value); }
        public DateTime CreationDate { get => GetProperty(() => CreationDate); set => SetProperty(() => CreationDate, value); }
    }
}
