using System.Windows;

namespace Sudoku.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void MenuQuitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
