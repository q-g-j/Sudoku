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
            numbersListValue = new NumbersListModel();
            selectNumberVisibilityValue = "Hidden";
            MessengerService.OnMessageTransmitted += OnMessageReceived;
        }
        #endregion Constructor

        #region Properties
        private NumbersListModel numbersListValue;
        public NumbersListModel NumbersList
        {
            get
            {
                return numbersListValue;
            }
            set
            {
                numbersListValue = value;
                OnPropertyChanged();
            }
        }

        private string selectNumberVisibilityValue;
        public string SelectNumberVisibility
        {
            get
            {
                return selectNumberVisibilityValue;
            }
            set
            {
                selectNumberVisibilityValue = value;
                OnPropertyChanged();
            }
        }
        #endregion Properties

        #region Methods
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
            temp_numbersList = numbersListValue;
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

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion Methods

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events
    }
}
