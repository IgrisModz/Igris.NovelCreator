using Igris.NovelCreator.ViewModels;
using System.Windows;

namespace Igris.NovelCreator
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Class1 ViewModel { get => DataContext as Class1; set => DataContext = value; }
        public Window1()
        {
            ViewModel = new Class1();
            DataContext = new Class1();
            InitializeComponent();
        }
    }
}
