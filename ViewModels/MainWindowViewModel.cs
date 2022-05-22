using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sudoku.Debug;
using Sudoku.GameLogic;
using Sudoku.Models;
using Sudoku.Helpers;
using System.Windows.Input;

namespace Sudoku.ViewModels
{

    public class MainWindowViewModel :INotifyPropertyChanged
    {
        #region Constructor
        public MainWindowViewModel()
        {
            selectNumberVisibilityValue = "Hidden";
            selectMarkerVisibilityValue = "Hidden";
            selectDifficultyVisibilityValue = "Hidden";
            buttonDifficultyTextValue = "Neues Spiel";

            ButtonDifficultyCommand = new RelayCommand(o => ButtonDifficultyClick());
            ButtonValidateCommand = new RelayCommand(o => ButtonValidateClick());
            ButtonDifficultyEasyCommand = new RelayCommand(o => ButtonDifficultyEasyClick());
            ButtonDifficultyMediumCommand = new RelayCommand(o => ButtonDifficultyMediumClick());
            ButtonDifficultyHardCommand = new RelayCommand(o => ButtonDifficultyHardClick());
            ButtonSquareLeftCommand = new RelayCommand(o => ButtonSquareLeftClick(o));
            ButtonSquareRightCommand = new RelayCommand(o => ButtonSquareRightClick(o));
            ButtonSelectNumberCommand = new RelayCommand(o => ButtonSelectNumberClick(o));
            ButtonSelectMarkerCommand = new RelayCommand(o => ButtonSelectMarkerClick(o));
        }
        #endregion Constructor

        private string currentButtonIndex = "";

        private List<string> generatorNumbers;

        #region Properties
        private NumbersListModel numbersListValue;
        private MarkersListModel markersListValue;
        private NumbersColorsListModel numbersColorsListValue;
        private string buttonDifficultyTextValue;
        private string labelValidateValue;

        private string selectNumberVisibilityValue;
        private string selectMarkerVisibilityValue;
        private string selectDifficultyVisibilityValue;
        private string buttonValidateVisibilityValue;

        public ICommand ButtonDifficultyCommand { get; }
        public ICommand ButtonValidateCommand { get; }
        public ICommand ButtonDifficultyEasyCommand { get; }
        public ICommand ButtonDifficultyMediumCommand { get; }
        public ICommand ButtonDifficultyHardCommand { get; }
        public ICommand ButtonSquareLeftCommand { get; }
        public ICommand ButtonSquareRightCommand { get; }
        public ICommand ButtonSelectNumberCommand { get; }
        public ICommand ButtonSelectMarkerCommand { get; }

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

        public string ButtonDifficultyText
        {
            get => buttonDifficultyTextValue;
            set { buttonDifficultyTextValue = value; OnPropertyChanged(); }
        }
        public string LabelValidate
        {
            get => labelValidateValue;
            set { labelValidateValue = value; OnPropertyChanged(); }
        }


        #endregion Properties

        #region Methods
        private void ButtonDifficultyClick()
        {
            if (SelectDifficultyVisibility == "Hidden")
            {
                SelectDifficultyVisibility = "Visible";
            }
            else
            {
                SelectDifficultyVisibility = "Hidden";
            }
        }

        private void ButtonValidateClick()
        {
            ValidateAll();
        }

        private void ButtonDifficultyEasyClick()
        {
            SelectDifficultyVisibility = "Hidden";
            SelectNumberVisibility = "Hidden";
            SelectMarkerVisibility = "Hidden";
            NewGame("Easy");
        }

        private void ButtonDifficultyMediumClick()
        {
            SelectDifficultyVisibility = "Hidden";
            SelectNumberVisibility = "Hidden";
            SelectMarkerVisibility = "Hidden";
            NewGame("Medium");
        }

        private void ButtonDifficultyHardClick()
        {
            SelectDifficultyVisibility = "Hidden";
            SelectNumberVisibility = "Hidden";
            SelectMarkerVisibility = "Hidden";
            NewGame("Hard");
        }

        private void ButtonSquareLeftClick(object o)
        {
            var tag = (string)o;
            currentButtonIndex = tag;

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

            if ((SelectNumberVisibility == "Visible") || (SelectMarkerVisibility == "Visible"))
            {
                SelectMarkerVisibility = "Hidden";
                SelectNumberVisibility = "Hidden";
            }
            else
            {
                if (!generatorNumbers.Contains(tag))
                {
                    SelectNumberVisibility = "Visible";
                }
            }
        }

        private void ButtonSquareRightClick(object o)
        {
            var tag = (string)o;
            currentButtonIndex = tag;

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

            if ((SelectNumberVisibility == "Visible") || (SelectMarkerVisibility == "Visible"))
            {
                SelectMarkerVisibility = "Hidden";
                SelectNumberVisibility = "Hidden";
            }
            else
            {
                if (!generatorNumbers.Contains(tag))
                {
                    SelectMarkerVisibility = "Visible";
                }
            }
        }

        private void ButtonSelectNumberClick(object o)
        {
            SelectNumberVisibility = "Hidden";
            var tag = (string)o;
            string param = currentButtonIndex + tag;
            ChangeNumber(param);
        }

        private void ButtonSelectMarkerClick(object o)
        {
            SelectMarkerVisibility = "Hidden";
            var tag = (string)o;
            string param = currentButtonIndex + tag;
            ChangeMarker(param);
        }

        private void InititalizeValues()
        {
            if (numbersListValue == null)
            {
                numbersListValue = new NumbersListModel();
                numbersListValue.InitializeList();
            }
            if (markersListValue == null)
            {
                markersListValue = new MarkersListModel();
                markersListValue.InitializeList();
            }
            if (numbersColorsListValue == null)
            {
                numbersColorsListValue = new NumbersColorsListModel();
                numbersColorsListValue.InitializeList();
            }
        }

        private void ChangeNumber(string button)
        {
            InititalizeValues();

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
            InititalizeValues();

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

        private void NewGame(string difficulty)
        {
            GeneratorGameLogic generatorGameLogic = null;

            bool doRun = true;
            bool unique = false;

            do
            {
                numbersListValue = new NumbersListModel();
                numbersListValue.InitializeList();

                SolverGameLogic solverGameLogic = new SolverGameLogic(numbersListValue);
                solverGameLogic.FillSudoku();
                NumbersListModel numbersListSolved = solverGameLogic.NumbersListSolved;

                generatorGameLogic = new GeneratorGameLogic();

                if (difficulty == "Easy")
                {
                    generatorGameLogic.RemoveNumbers = 40;
                }
                else if (difficulty == "Medium")
                {
                    generatorGameLogic.RemoveNumbers = 48;
                }
                else if (difficulty == "Hard")
                {
                    generatorGameLogic.RemoveNumbers = 57;
                }

                generatorGameLogic.NumbersList = NumbersListModel.CopyList(numbersListSolved);
                generatorGameLogic.GenerateSudoku();

                if (generatorGameLogic.counter == generatorGameLogic.RemoveNumbers)
                {
                    doRun = false;
                }
            } while (doRun && unique);

            generatorNumbers = new List<string>();

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    if (generatorGameLogic.NumbersList[col][row] != "")
                    {
                        string coords = col.ToString();
                        coords += row.ToString();
                        generatorNumbers.Add(coords);
                    }
                }
            }

            numbersColorsListValue = new NumbersColorsListModel();
            numbersColorsListValue.InitializeList();
            foreach (string coords in generatorNumbers)
            {
                int col = int.Parse(coords[0].ToString());
                int row = int.Parse(coords[1].ToString());

                numbersColorsListValue[col][row] = "Black";
            }
            NumbersColorsList = numbersColorsListValue;

            LabelValidate = "";

            NumbersList = generatorGameLogic.NumbersList;

            markersListValue = new MarkersListModel();
            markersListValue.InitializeList();
            MarkersList = markersListValue;

            ButtonDifficultyText = "Neues Spiel";
        }

        private void ValidateAll()
        {
            if (numbersListValue == null)
            {
                numbersListValue = new NumbersListModel();
                numbersListValue.InitializeList();
            }

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    string number = numbersListValue[col][row];
                    if (! ValidatorGameLogic.IsValid(numbersListValue, col, row, number))
                    {
                        LabelValidate = "Konflikte gefunden!";
                        return;
                    }
                }
            }
            LabelValidate = "Keine Konflikte!";
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
