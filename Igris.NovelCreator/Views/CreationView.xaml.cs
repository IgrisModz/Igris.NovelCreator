using Igris.NovelCreator.Models;
using Igris.NovelCreator.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Igris.NovelCreator.Views
{
    /// <summary>
    /// Logique d'interaction pour CreationView.xaml
    /// </summary>
    public partial class CreationView : UserControl
    {
        public CreationView(ObservableCollection<Novel> novels)
        {
            DataContext = new CreationViewModel(novels);
            InitializeComponent();
        }

        private CreationView()
        {
        }

        private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FilterTextBox.Focus();
            Keyboard.Focus(FilterTextBox);
        }
    }
}
