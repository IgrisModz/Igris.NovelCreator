using Igris.NovelCreator.Models;
using Igris.NovelCreator.ViewModels;
using System.Windows.Controls;

namespace Igris.NovelCreator.Views
{
    /// <summary>
    /// Logique d'interaction pour VolumeCreationView.xaml
    /// </summary>
    public partial class VolumeCreationView : UserControl
    {
        public VolumeCreationView(NovelGestionViewModel vm, Volume volume = null)
        {
            DataContext = new VolumeCreationViewModel(vm, volume);
            InitializeComponent();
        }

        private VolumeCreationView() { }
    }
}
