using Igris.NovelCreator.Models;
using Igris.NovelCreator.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Igris.NovelCreator.Views
{
    /// <summary>
    /// Logique d'interaction pour ChapterReadingView.xaml
    /// </summary>
    public partial class ChapterReadingView : UserControl
    {
        public ChapterReadingView(ChapterListViewModel vm,  ObservableCollection<Chapter> chapters, Chapter chapter)
        {
            DataContext = new ChapterReadingViewModel(vm, chapters, chapter);
            InitializeComponent();
        }

        private ChapterReadingView()
        {
        }
    }
}
