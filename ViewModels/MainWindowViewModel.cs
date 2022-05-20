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
            numbersColorsListValue = new NumbersColorsListModel();

            selectNumberVisibilityValue = "Hidden";
            selectMarkerVisibilityValue = "Hidden";
            selectDifficultyVisibilityValue = "Hidden";


            MessengerService.OnMessageTransmittedTwoParams += OnMessageReceivedTwoParams;
            MessengerService.OnMessageTransmittedThreeParams += OnMessageWithButtonReceived;
        }
        #endregion Constructor

        private List<string> generatorNumbers;

        #region Properties
        private NumbersListModel numbersListValue;
        private MarkersListModel markersListValue;
        private NumbersColorsListModel numbersColorsListValue;
        private string labelValidateValue;

        private string selectNumberVisibilityValue;
        private string selectMarkerVisibilityValue;
        private string selectDifficultyVisibilityValue;
        private string buttonValidateVisibilityValue;

        public NumbersListModel NumbersList
        {
            get => numbersListValue;
            set { numbersListValue = value; OnPropertyChanged(); }
        }

        public MarkersListModel MarkersList
        {
            get => markersListValue;
            set {  markersListValue = value; OnPropertyChanged(); }
        }

        public NumbersColorsListModel NumbersColorsList
        {
            get => numbersColorsListValue;
            set { numbersColorsListValue = value; OnPropertyChanged(); }
        }

        public string SelectNumberVisibility
        {
            get => selectNumberVisibilityValue;
            set { selectNumberVisibilityValue = value; OnPropertyChanged(); }
        }

        public string SelectMarkerVisibility
        {
            get => selectMarkerVisibilityValue;
            set { selectMarkerVisibilityValue = value; OnPropertyChanged(); }
        }

        public string SelectDifficultyVisibility
        {
            get => selectDifficultyVisibilityValue;
            set { selectDifficultyVisibilityValue = value; OnPropertyChanged(); }
        }

        public string ButtonValidateVisibility
        {
            get => buttonValidateVisibilityValue;
            set { buttonValidateVisibilityValue = value; OnPropertyChanged(); }
        }

        public string LabelValidate
        {
            get => labelValidateValue;
            set { labelValidateValue = value; OnPropertyChanged(); }
        }
        #endregion Properties

        #region Methods
        private void ChangeNumber(string button)
        {
            if (generatorNumbers == null)
            {
                generatorNumbers = new List<string>();
            }
            string coords = button.Substring(0, 2);
            if (! generatorNumbers.Contains(coords))
            {
                int col = int.Parse(button[0].ToString());
                int row = int.Parse(button[1].ToString());
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
                                MarkersListModel temp_markersList;
                                temp_markersList = markersListValue;
                                temp_markersList[col][row][i][j] = "";
                                MarkersList = temp_markersList;
                            }
                        }
                    }
                    for (int k = 0; k < 9; k++)
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
                                    MarkersListModel temp_markersList;
                                    temp_markersList = markersListValue;
                                    if (temp_markersList[col][k][i][j] == number)
                                    {
                                        temp_markersList[col][k][i][j] = "";
                                        MarkersList = temp_markersList;
                                    }
                                }
                            }
                        }
                    }
                    for (int k = 0; k < 9; k++)
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
                                    MarkersListModel temp_markersList;
                                    temp_markersList = markersListValue;
                                    if (temp_markersList[k][row][i][j] == number)
                                    {
                                        temp_markersList[k][row][i][j] = "";
                                        MarkersList = temp_markersList;
                                    }
                                }
                            }
                        }
                    }

                    int col2 = (int)(col / 3) * 3;
                    int row2 = (int)(row / 3) * 3;

                    for (int i = row2; i < row2 + 3; i++)
                    {
                        for (int j = col2; j < col2 + 3; j++)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                for (int l = 0; l < 3; l++)
                                {
                                    if (k == 1 && l == 1)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        MarkersListModel temp_markersList;
                                        temp_markersList = markersListValue;
                                        if (temp_markersList[j][i][k][l] == number)
                                        {
                                            temp_markersList[j][i][k][l] = "";
                                            MarkersList = temp_markersList;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ChangeMarker(string button)
        {
            int col = int.Parse(button[0].ToString());
            int row = int.Parse(button[1].ToString());
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

        private void SelectDifficulty()
        {
            SelectDifficultyVisibility = "Visible";
        }

        private void NewGame(string difficulty)
        {
            SolverGameLogic solverGameLogic = new SolverGameLogic
            {
                numbersList = new NumbersListModel()
            };
            solverGameLogic.SolvePuzzle();
            numbersListValue = solverGameLogic.numbersList;

            GeneratorGameLogic generatorGameLogic = new GeneratorGameLogic();

            if (difficulty == "Easy")
            {
                generatorGameLogic.RemoveNumbers = 25;
            }
            else if (difficulty == "Medium")
            {
                generatorGameLogic.RemoveNumbers = 40;
            }
            else if (difficulty == "Hard")
            {
                generatorGameLogic.RemoveNumbers = 57;
            }
            generatorGameLogic.numbersList = new NumbersListModel(numbersListValue);
            generatorGameLogic.GenerateSudoku();
            NumbersList = generatorGameLogic.numbersList;

            generatorNumbers = new List<string>();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (numbersListValue[i][j] != "")
                    {
                        string coords = i.ToString();
                        coords += j.ToString();
                        generatorNumbers.Add(coords);
                    }
                }
            }

            NumbersColorsListModel tempList = new NumbersColorsListModel();
            foreach (string coords in generatorNumbers)
            {
                int col = int.Parse(coords[0].ToString());
                int row = int.Parse(coords[1].ToString());

                tempList[col][row] = "Black";
            }
            NumbersColorsList = tempList;
            LabelValidate = "";
        }

        private void ValidateAll()
        {
            string coords;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    coords = i.ToString();
                    coords += j.ToString();
                    if (! ValidatorGameLogic.IsValid(numbersListValue, coords))
                    {
                        LabelValidate = "Konflikte gefunden!";
                        return;
                    }
                }
            }

            LabelValidate = "Keine Konflikte!";
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

        protected void OnMessageReceivedTwoParams(string type, string message)
        {
            if (type == "SelectDifficultyVisibility")
            {
                if (SelectDifficultyVisibility == "Visible")
                {
                    SelectDifficultyVisibility = "Hidden";
                }
                else
                {
                    SelectDifficulty();
                }
            }
            else if (type == "Validate")
            {
                SelectDifficultyVisibility = "Hidden";
                SelectNumberVisibility = "Hidden";
                SelectMarkerVisibility = "Hidden";
                ValidateAll();
            }
            else if (type == "NewGame")
            {
                SelectDifficultyVisibility = "Hidden";
                SelectNumberVisibility = "Hidden";
                SelectMarkerVisibility = "Hidden";
                NewGame(message);
            }
            else if (type == "ChangeNumber")
            {
                ChangeNumber(message);
            }
            else if (type == "ChangeMarker")
            {
                ChangeMarker(message);
            }
        }

        protected void OnMessageWithButtonReceived(string type, string message, string button)
        {
            if (SelectDifficultyVisibility == "Visible")
            {
                SelectDifficultyVisibility = "Hidden";
                return;
            }

            LabelValidate = "";

            if (generatorNumbers == null)
            {
                generatorNumbers = new List<string>();
            }

            if (type == "SelectNumberGridVisibility")
            {
                if ((SelectNumberVisibility == "Visible" && message == "Visible")
                    || (SelectMarkerVisibility == "Visible" && message == "Visible"))
                {
                    SelectMarkerVisibility = "Hidden";
                    SelectNumberVisibility = "Hidden";
                }
                else
                {
                    if (!generatorNumbers.Contains(button))
                    {
                        SelectNumberVisibility = message;
                    }
                }
            }
            else if (type == "SelectMarkerGridVisibility")
            {
                if ((SelectNumberVisibility == "Visible" && message == "Visible")
                    || (SelectMarkerVisibility == "Visible" && message == "Visible"))
                {
                    SelectMarkerVisibility = "Hidden";
                    SelectNumberVisibility = "Hidden";
                }
                else
                {
                    if (!generatorNumbers.Contains(button))
                    {
                        SelectMarkerVisibility = message;
                    }
                }
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
