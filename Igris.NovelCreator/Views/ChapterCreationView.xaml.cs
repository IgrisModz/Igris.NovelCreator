using Igris.NovelCreator.Models;
using Igris.NovelCreator.ViewModels;
using System.Windows.Controls;

namespace Igris.NovelCreator.Views
{
    /// <summary>
    /// Logique d'interaction pour ChapterCreationView.xaml
    /// </summary>
    public partial class ChapterCreationView : UserControl
    {
        public ChapterCreationView(NovelGestionViewModel vm, Chapter chapter)
        {
            DataContext = new ChapterCreationViewModel(vm, chapter);
            InitializeComponent();
        }

        public ChapterCreationView(NovelGestionViewModel vm, Volume volume)
        {
            DataContext = new ChapterCreationViewModel(vm, volume);
            InitializeComponent();
        }

        public ChapterCreationView()
        {
        }
    }
}
