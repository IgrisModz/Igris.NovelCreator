using Igris.NovelCreator.Models;
using Igris.NovelCreator.ViewModels;
using System.Windows.Controls;

namespace Igris.NovelCreator.Views
{
    /// <summary>
    /// Logique d'interaction pour NovelCreationView.xaml
    /// </summary>
    public partial class NovelCreationView : UserControl
    {
        public NovelCreationView(CreationViewModel vm, Novel novel = null)
        {
            DataContext = new NovelCreationViewModel(vm, novel);
            InitializeComponent();
        }

        private NovelCreationView()
        {
        }
    }
}
