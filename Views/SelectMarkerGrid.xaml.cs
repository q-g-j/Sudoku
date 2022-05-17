using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using Sudoku.Services;

namespace Sudoku.Views
{
    public partial class SelectMarkerGrid : Grid
    {
        public SelectMarkerGrid()
        {
            InitializeComponent();
        }

        public string ButtonIndex;

        private void MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void MouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            var tag = ((Button)sender).Tag;
            string button = ButtonIndex + tag.ToString();

            if (e.ChangedButton == MouseButton.Left)
            {
                MessengerService.BroadCast("SelectNumberGridVisibility", "Hidden");
                MessengerService.BroadCast("ChangeNumber", button);
                e.Handled = true;
            }
        }
    }
}
