using Igris.NovelCreator.Models;
using Igris.NovelCreator.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Igris.NovelCreator.Views
{
    /// <summary>
    /// Logique d'interaction pour ChapterListView.xaml
    /// </summary>
    public partial class ChapterListView : UserControl
    {
        public ChapterListView(ReadingViewModel vm, Novel novel)
        {
            DataContext = new ChapterListViewModel(vm, novel);
            InitializeComponent();
        }

        private ChapterListView()
        {
        }

        private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FilterTextBox.Focus();
            Keyboard.Focus(FilterTextBox);
        }
    }
}
