﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sudoku.Services;

namespace Sudoku.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private string currentButtonDown;

        private void MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button button = (Button)sender;
            currentButtonDown = button.Tag.ToString();
            e.Handled = true;
        }

        private void MouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            Button button = (Button)sender;
            var buttonIndex = button.Tag.ToString();
            SelectNumberGrid.ButtonIndex = buttonIndex;

            if (e.ChangedButton == MouseButton.Left && currentButtonDown == buttonIndex)
            {
                MessengerService.BroadCast("SelectNumberGridVisibility", "Visible");
                e.Handled = true;
            }
        }
    }
}
