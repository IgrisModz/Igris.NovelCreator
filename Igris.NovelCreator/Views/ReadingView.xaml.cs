using Igris.NovelCreator.Models;
using Igris.NovelCreator.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Igris.NovelCreator.Views
{
    /// <summary>
    /// Logique d'interaction pour ReadingView.xaml
    /// </summary>
    public partial class ReadingView : UserControl
    {
        public ReadingView(ObservableCollection<Novel> novels)
        {
            DataContext = new ReadingViewModel(novels);
            InitializeComponent();
        }

        private ReadingView()
        {
        }

        private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FilterTextBox.Focus();
            Keyboard.Focus(FilterTextBox);
        }
    }
}
