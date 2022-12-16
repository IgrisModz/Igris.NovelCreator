using Igris.NovelCreator.Models;
using Igris.NovelCreator.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Igris.NovelCreator.Views
{
    /// <summary>
    /// Logique d'interaction pour ChapterCreationView.xaml
    /// </summary>
    public partial class NovelGestionView : UserControl
    {
        public NovelGestionView(CreationViewModel vm, Novel novel)
        {
            DataContext = new NovelGestionViewModel(vm, novel);
            InitializeComponent();
        }

        private NovelGestionView()
        {
        }

        private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FilterTextBox.Focus();
            Keyboard.Focus(FilterTextBox);
        }
    }
}
