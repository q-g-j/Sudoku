using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sudoku.Models;
using Sudoku.Services;

namespace Sudoku.ViewModels
{
    public class MainWindowViewModel :INotifyPropertyChanged
    {
        #region Constructor
        public MainWindowViewModel()
        {
            _numbersList = new NumbersListModel();
            _selectNumberVisibility = "Hidden";
            MessengerService.OnMessageTransmitted += OnMessageReceived;
        }
        #endregion Constructor

        #region Properties
        private NumbersListModel _numbersList;
        public NumbersListModel NumbersList
        {
            get
            {
                return _numbersList;
            }
            set
            {
                _numbersList = value;
                OnPropertyChanged("NumbersList");
            }
        }

        private string _selectNumberVisibility;
        public string SelectNumberVisibility
        {
            get
            {
                return _selectNumberVisibility;
            }
            set
            {
                _selectNumberVisibility = value;
                OnPropertyChanged("SelectNumberVisibility");
            }
        }
        #endregion Properties

        #region Member methods
        private void ChangeNumber(string button)
        {
            Int16 col1 = Int16.Parse(button[6].ToString());
            Int16 row1 = Int16.Parse(button[7].ToString());
            Int16 col2 = Int16.Parse(button[8].ToString());
            Int16 row2 = Int16.Parse(button[9].ToString());
            string number = button[10].ToString();
            if (number == "X")
            {
                number = "";
            }
            NumbersListModel temp_numbersList;
            temp_numbersList = _numbersList;
            System.Diagnostics.Debug.WriteLine(col1.ToString() + row1.ToString() + col1.ToString() + row2.ToString());
            temp_numbersList[col1][row1][col2][row2][1][1] = number;
            NumbersList = temp_numbersList;
        }

        private void OnMessageReceived(string type, string message)
        {
            if (type == "SelectNumberGridVisibility")
            {
                if (SelectNumberVisibility == "Visible" && message == "Visible")
                {
                    SelectNumberVisibility = "Hidden";
                }
                else
                {
                    SelectNumberVisibility = message;
                }
            }
            else if (type == "ChangeNumber")
            {
                ChangeNumber(message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion Member methods
    }
}
