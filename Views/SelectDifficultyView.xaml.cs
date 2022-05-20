using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sudoku.Services;

namespace Sudoku.Views
{
    /// <summary>
    /// Interaction logic for SelectDifficultyView.xaml
    /// </summary>
    public partial class SelectDifficultyView : StackPanel
    {
        public SelectDifficultyView()
        {
            InitializeComponent();
        }

        public void ButtonEasy(object sender, RoutedEventArgs e)
        {
            MessengerService.BroadCast("NewGame", "Easy");
        }

        public void ButtonMedium(object sender, RoutedEventArgs e)
        {
            MessengerService.BroadCast("NewGame", "Medium");
        }

        public void ButtonHard(object sender, RoutedEventArgs e)
        {
            MessengerService.BroadCast("NewGame", "Hard");
        }
    }
}
