using System.Windows;
using VisualSportCut.Presentation.ViewModels;

namespace VisualSportCut
{
    public partial class MainWindow : Window
    {
        public MainWindow() : this(null) { }

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}