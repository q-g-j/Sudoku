using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sudoku.GameLogic;
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
            markersListValue = new MarkersListModel();
            generatorGameLogic = new GeneratorGameLogic();
            solverGameLogic = new SolverGameLogic();

            selectNumberVisibilityValue = "Hidden";
            selectMarkerVisibilityValue = "Hidden";

            MessengerService.OnMessageTransmitted += OnMessageReceived;

            //numbersListValue[3][0] = "8";
            //numbersListValue[4][0] = "3";
            //numbersListValue[4][1] = "7";
            //numbersListValue[5][1] = "4";
            //numbersListValue[7][1] = "5";
            //numbersListValue[2][3] = "4";
            //numbersListValue[5][3] = "6";
            //numbersListValue[8][3] = "8";
            //numbersListValue[0][4] = "2";
            //numbersListValue[4][4] = "8";
            //numbersListValue[8][4] = "9";
            //numbersListValue[1][5] = "6";
            //numbersListValue[3][5] = "1";
            //numbersListValue[5][5] = "2";
            //numbersListValue[6][5] = "4";
            //numbersListValue[2][6] = "5";
            //numbersListValue[3][6] = "7";
            //numbersListValue[6][6] = "9";
            //numbersListValue[8][6] = "3";
            //numbersListValue[0][7] = "9";
            //numbersListValue[1][7] = "8";
            //numbersListValue[8][7] = "5";
            //numbersListValue[2][8] = "1";
            //numbersListValue[4][8] = "6";
            //numbersListValue[5][8] = "5";
            //numbersListValue[8][8] = "4";

            generatorGameLogic.counter = 1;
            solverGameLogic.counter = 1;

            solverGameLogic.numbersList = new NumbersListModel(numbersListValue);
            solverGameLogic.Solver();
            numbersListValue = solverGameLogic.numbersList;

            generatorGameLogic.numbersList = new NumbersListModel(numbersListValue);
            generatorGameLogic.GenerateSudoku();
            numbersListValue = generatorGameLogic.numbersList;
        }
        #endregion Constructor

        private readonly GeneratorGameLogic generatorGameLogic;
        private readonly SolverGameLogic solverGameLogic;

        #region Properties
        private NumbersListModel numbersListValue = new NumbersListModel();
        private MarkersListModel markersListValue = new MarkersListModel();
        private string labelValidateValue;

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

        public MarkersListModel MarkersList
        {
            get
            {
                return markersListValue;
            }
            set
            {
                markersListValue = value;
                OnPropertyChanged();
            }
        }

        private string selectNumberVisibilityValue;
        private string selectMarkerVisibilityValue;

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

        public string SelectMarkerVisibility
        {
            get
            {
                return selectMarkerVisibilityValue;
            }
            set
            {
                selectMarkerVisibilityValue = value;
                OnPropertyChanged();
            }
        }

        public string LabelValidate
        {
            get
            {
                return labelValidateValue;
            }
            set
            {
                labelValidateValue = value;
                OnPropertyChanged();
            }
        }
        #endregion Properties

        #region Methods
        private void ChangeNumber(string button)
        {
            Int16 col = Int16.Parse(button[0].ToString());
            Int16 row = Int16.Parse(button[1].ToString());
            string number = button[2].ToString();
            NumbersListModel temp_numbersList;
            temp_numbersList = numbersListValue;

            if (number == "X")
            {
                temp_numbersList[col][row] = number;
                temp_numbersList[col][row] = "";
                NumbersList = temp_numbersList;
                return;
            }
            else
            {
                temp_numbersList[col][row] = number;
                NumbersList = temp_numbersList;
            }
        }

        private void ChangeMarker(string button)
        {
            Int16 col = Int16.Parse(button[0].ToString());
            Int16 row = Int16.Parse(button[1].ToString());
            string number = button[2].ToString();

            if (numbersListValue[col][row] != "")
            {
                return;
            }

            if (number == "X")
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 1 && j == 1)
                        {
                            continue;
                        }
                        else
                        {
                            MarkersListModel temp_numbersList;
                            temp_numbersList = markersListValue;
                            temp_numbersList[col][row][i][j] = "";
                            MarkersList = temp_numbersList;
                        }
                    }
                }
                return;
            }
            else
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 1 && j == 1)
                        {
                            continue;
                        }
                        else if (markersListValue[col][row][i][j] == number)
                        {
                            MarkersListModel temp_numbersList;
                            temp_numbersList = markersListValue;
                            temp_numbersList[col][row][i][j] = "";
                            MarkersList = temp_numbersList;
                            return;
                        }
                    }
                }
         
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 1 && j == 1)
                        {
                            continue;
                        }
                        MarkersListModel temp_numbersList;
                        temp_numbersList = markersListValue;
                        if (temp_numbersList[col][row][i][j] == "")
                        {
                            temp_numbersList[col][row][i][j] = number;
                            MarkersList = temp_numbersList;
                            return;
                        }
                    }
                }
            }
        }

        private void SolveGame()
        {
            solverGameLogic.numbersList = numbersListValue;
            solverGameLogic.Solver();
            NumbersList = solverGameLogic.numbersList;
        }

        private void ValidateBoard()
        {
            string coords;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    coords = i.ToString();
                    coords += j.ToString();
                    if (! GameLogic.ValidatorGameLogic.IsValid(numbersListValue, coords))
                    {
                        LabelValidate = "Konflikte gefunden!";
                        return;
                    }
                }
            }

            LabelValidate = "Keine Konflikte gefunden!";
        }

        public static void DebugPrintNumbersList(NumbersListModel numbersList)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (numbersList[col][row] == "")
                        System.Diagnostics.Debug.Write("0" + "  ");
                    else
                        System.Diagnostics.Debug.Write(numbersList[col][row] + "  ");

                }
                System.Diagnostics.Debug.Write("\n");
            }
            System.Diagnostics.Debug.Write("\n");
            System.Diagnostics.Debug.Write("\n");
        }

        private void OnMessageReceived(string type, string message)
        {
            if (type == "SelectNumberGridVisibility")
            {
                if ((SelectNumberVisibility == "Visible" && message == "Visible") || (SelectMarkerVisibility == "Visible" && message == "Visible"))
                {
                    SelectMarkerVisibility = "Hidden";
                    SelectNumberVisibility = "Hidden";
                }
                else
                {
                    SelectNumberVisibility = message;
                }
            }
            else if (type == "SelectMarkerGridVisibility")
            {
                if ((SelectNumberVisibility == "Visible" && message == "Visible") || (SelectMarkerVisibility == "Visible" && message == "Visible"))
                {
                    SelectMarkerVisibility = "Hidden";
                    SelectNumberVisibility = "Hidden";
                }
                else
                {
                    SelectMarkerVisibility = message;
                }
            }
            else if (type == "ChangeNumber")
            {
                ChangeNumber(message);
            }
            else if (type == "ChangeMarker")
            {
                ChangeMarker(message);
            }
            else if (type == "Solve")
            {
                SolveGame();
            }
            else if (type == "Validate")
            {
                ValidateBoard();
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
