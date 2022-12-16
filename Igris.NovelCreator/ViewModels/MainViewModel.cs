using Igris.Mvvm;
using Igris.NovelCreator.Databases;
using Igris.NovelCreator.Models;
using Igris.NovelCreator.Views;
using System.Collections.ObjectModel;

namespace Igris.NovelCreator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public CreationView CreationView { get => GetProperty(() => CreationView); set => SetProperty(() => CreationView, value); }
        public ReadingView ReadingView { get => GetProperty(() => ReadingView); set => SetProperty(() => ReadingView, value); }

        public MainViewModel()
        {
            ObservableCollection<Novel> novels = Database.GetNovels();
            CreationView = new(novels);
            ReadingView = new(novels);
        }
    }
}
