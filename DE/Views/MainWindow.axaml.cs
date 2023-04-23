using Avalonia.Controls;
using DE.ViewModels;

namespace DE.Views {
    public partial class MainWindow: Window {
        public MainWindow() {
            InitializeComponent();
            DataContext = new MainWindowViewModel(this);
        }
    }
}