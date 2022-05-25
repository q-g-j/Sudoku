using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sudoku.GameLogic;
using Sudoku.Models;
using Sudoku.Helpers;
using Sudoku.Settings;
using Sudoku.SaveGame;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Diagnostics;
using System.Windows;

namespace Sudoku.ViewModels
{
    public class MainWindowViewModel :INotifyPropertyChanged
    {
        #region Constructor
        public MainWindowViewModel()
        {
            selectNumberVisibilityValue = "Hidden";
            selectMarkerVisibilityValue = "Hidden";
            selectDifficultyVisibilityValue = "Visible";
            buttonValidateVisibilityValue = "Collapsed";
            labelValidateVisibilityValue = "Collapsed";
            labelValidateTextValue = "";
            buttonDifficultyWidthValue = "350";
            currentButtonIndex = "";
            folderAppSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SudokuGame");

            if (!Directory.Exists(folderAppSettings))
            {
                Directory.CreateDirectory(folderAppSettings);
            }

            generatorNumbers = new List<string>();
            numbersListValue = new NumbersListModel();
            numbersListValue.InitializeList();

            MenuNewCommand = new AsyncRelayCommand(MenuNewAction);
            MenuSaveToSlotCommand = new AsyncRelayCommand<object>(o => MenuSaveToSlotAction(o));
            MenuLoadFromSlotCommand = new AsyncRelayCommand<object>(o => MenuLoadFromSlotAction(o));
            ButtonDifficultyCommand = new AsyncRelayCommand(ButtonDifficultyAction);
            ButtonValidateCommand = new AsyncRelayCommand(ButtonValidateAction);
            ButtonDifficultyEasyCommand = new AsyncRelayCommand(ButtonDifficultyEasyAction);
            ButtonDifficultyMediumCommand = new AsyncRelayCommand(ButtonDifficultyMediumAction);
            ButtonDifficultyHardCommand = new AsyncRelayCommand(ButtonDifficultyHardAction);
            ButtonSelectNumberCommand = new AsyncRelayCommand<object>(o => ButtonSelectNumberAction(o));
            ButtonSelectMarkerCommand = new AsyncRelayCommand<object>(o => ButtonSelectMarkerAction(o));

            ButtonSquareDownCommand = new AsyncRelayCommand<CompositeCommandParameter>(o => ButtonSquareDownAction(o));
            ButtonSquareUpCommand = new AsyncRelayCommand<CompositeCommandParameter>(o => ButtonSquareUpAction(o));
        }
        #endregion Constructor

        #region Private variables
        private string currentButtonIndex;
        private List<string> generatorNumbers;
        string folderAppSettings;
        #endregion Private variables

        #region Property values
        private NumbersListModel numbersListValue;
        private MarkersListModel markersListValue;
        private NumbersColorsListModel numbersColorsListValue;

        private string buttonDifficultyTextValue;
        private string labelValidateTextValue;
        private string buttonDifficultyWidthValue;

        private string selectNumberVisibilityValue;
        private string selectMarkerVisibilityValue;
        private string selectDifficultyVisibilityValue;
        private string buttonValidateVisibilityValue;
        private string labelValidateVisibilityValue;
        #endregion Property values

        #region Properties
        public IAsyncRelayCommand MenuNewCommand { get; }
        public IAsyncRelayCommand MenuSaveToSlotCommand { get; }
        public IAsyncRelayCommand MenuLoadFromSlotCommand { get; }
        public IAsyncRelayCommand ButtonDifficultyCommand { get; }
        public IAsyncRelayCommand ButtonValidateCommand { get; }
        public IAsyncRelayCommand ButtonDifficultyEasyCommand { get; }
        public IAsyncRelayCommand ButtonDifficultyMediumCommand { get; }
        public IAsyncRelayCommand ButtonDifficultyHardCommand { get; }
        public IAsyncRelayCommand ButtonSquareLeftCommand { get; }
        public IAsyncRelayCommand ButtonSquareRightCommand { get; }
        public IAsyncRelayCommand ButtonSelectNumberCommand { get; }
        public IAsyncRelayCommand ButtonSelectMarkerCommand { get; }
        public IAsyncRelayCommand ButtonSquareDownCommand { get; }
        public IAsyncRelayCommand ButtonSquareUpCommand { get; }

        public NumbersListModel NumbersList
        {
            get => numbersListValue;
            set { numbersListValue = value; OnPropertyChanged();
            }
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
        public string LabelValidateVisibility
        {
            get => labelValidateVisibilityValue;
            set { labelValidateVisibilityValue = value; OnPropertyChanged(); }
        }
        public string ButtonDifficultyText
        {
            get => buttonDifficultyTextValue;
            set { buttonDifficultyTextValue = value; OnPropertyChanged(); }
        }
        public string ButtonDifficultyWidth
        {
            get => buttonDifficultyWidthValue;
            set { buttonDifficultyWidthValue = value; OnPropertyChanged(); }
        }
        public string LabelValidateText
        {
            get => labelValidateTextValue;
            set { labelValidateTextValue = value; OnPropertyChanged(); }
        }
        #endregion Properties

        #region Methods
        private async Task MenuNewAction()
        {
            await Task.Run(() =>
            {
                HideAll();
                numbersListValue = new NumbersListModel();
                markersListValue = new MarkersListModel();
                numbersColorsListValue = new NumbersColorsListModel();
                generatorNumbers = new List<string>();
                numbersListValue.InitializeList();
                markersListValue.InitializeList();
                numbersColorsListValue.InitializeList();
                NumbersList = numbersListValue;
                MarkersList = markersListValue;
                NumbersColorsList = numbersColorsListValue;
            });
        }
        private async Task MenuSaveToSlotAction(object o)
        {
            await Task.Run(() =>
            {
                if (numbersListValue != null && markersListValue != null && numbersColorsListValue != null)
                {
                    string slotNumber = (string)o;
                    SaveSlots saveSlots = new SaveSlots(folderAppSettings);
                    saveSlots.SaveAll(numbersListValue, markersListValue, numbersColorsListValue, slotNumber);
                }
            });
        }
        private async Task MenuLoadFromSlotAction(object o)
        {
            await Task.Run(() =>
            {
                string slotNumber = (string)o;
                string filename = Path.Combine(folderAppSettings, "slot" + slotNumber + ".json");
                if (File.Exists(filename))
                {
                    HideAll();
                    SaveSlots saveSlots = new SaveSlots(folderAppSettings);
                    SaveSlots.ListsStruct listsStruct = saveSlots.LoadAll(slotNumber);
                    NumbersList = listsStruct.NumbersList;
                    MarkersList = listsStruct.MarkersList;
                    NumbersColorsList = listsStruct.NumbersColorsList;
                }
            });
        }
        private async Task ButtonDifficultyAction()
        {
            await Task.Run(() => {
                SelectNumberVisibility = "Hidden";
                SelectMarkerVisibility = "Hidden";

                if (SelectDifficultyVisibility == "Hidden")
                {
                    SelectDifficultyVisibility = "Visible";
                }
                else
                {
                    SelectDifficultyVisibility = "Hidden";
                }
            });
        }
        private async Task ButtonValidateAction()
        {
            await Task.Run(() =>
            {
                ValidateAll();
            });
        }
        private async Task ButtonDifficultyEasyAction()
        {
            await Task.Run(() => {
                HideAll();
                NewGame("Easy");
            });
        }
        private async Task ButtonDifficultyMediumAction()
        {
            await Task.Run(() =>
            {
                HideAll();
                NewGame("Medium");
            });
        }
        private async Task ButtonDifficultyHardAction()
        {
            await Task.Run(() =>
            {
                HideAll();
                NewGame("Hard");
            });
        }
        private async Task ButtonSquareDownAction(CompositeCommandParameter o)
        {
            var e = (MouseButtonEventArgs)o.EventArgs;
            var param = (string)o.Parameter;

            await Task.Run(() =>
            {
                currentButtonIndex = param;

                if (e.ChangedButton == MouseButton.Left)
                {
                    if (SelectDifficultyVisibility == "Visible")
                    {
                        SelectDifficultyVisibility = "Hidden";
                        return;
                    }

                    if ((SelectNumberVisibility == "Visible") || (SelectMarkerVisibility == "Visible"))
                    {
                        SelectMarkerVisibility = "Hidden";
                        SelectNumberVisibility = "Hidden";
                    }
                    else if (!generatorNumbers.Contains(param))
                    {
                        SelectNumberVisibility = "Visible";
                    }
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    if (SelectDifficultyVisibility == "Visible")
                    {
                        SelectDifficultyVisibility = "Hidden";
                        return;
                    }
                    if ((SelectNumberVisibility == "Visible") || (SelectMarkerVisibility == "Visible"))
                    {
                        SelectMarkerVisibility = "Hidden";
                        SelectNumberVisibility = "Hidden";
                    }
                    else if (!generatorNumbers.Contains(param))
                    {
                        SelectMarkerVisibility = "Visible";
                    }
                }

                if (!SolverGameLogic.IsFull(numbersListValue))
                {
                    LabelValidateVisibility = "Collapsed";
                    ButtonValidateVisibility = "Collapsed";
                    ButtonDifficultyWidth = "350";
                }
                else
                {
                    LabelValidateVisibility = "Collapsed";
                    ButtonValidateVisibility = "Visible";
                }
            });

            e.Handled = true;
        }
        private async Task ButtonSquareUpAction(CompositeCommandParameter o)
        {
            var e = (MouseButtonEventArgs)o.EventArgs;
            await Task.Run(() =>
            {
            });

            e.Handled = true;
        }
        private async Task ButtonSelectNumberAction(object o)
        {
            await Task.Run(() =>
            {
                SelectNumberVisibility = "Hidden";
                var tag = (string)o;
                string param = currentButtonIndex + tag;
                ChangeNumber(param);
            });
        }
        private async Task ButtonSelectMarkerAction(object o)
        {
            await Task.Run(() =>
            {
                SelectMarkerVisibility = "Hidden";
                var tag = (string)o;
                string param = currentButtonIndex + tag;
                ChangeMarker(param);
            });
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
        private void ChangeButtonValidateVisibility()
        {
            if (SolverGameLogic.IsFull(numbersListValue))
            {
                ButtonDifficultyWidth = "250";
                ButtonValidateVisibility = "Visible";
            }
            else
            {
                ButtonValidateVisibility = "Collapsed";
                ButtonDifficultyWidth = "350";
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
                    ChangeButtonValidateVisibility();
                    return;
                }
                else
                {
                    temp_numbersList[col][row] = number;
                    NumbersList = temp_numbersList;
                    ChangeButtonValidateVisibility();
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
            GeneratorGameLogic generatorGameLogic;

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
                    generatorGameLogic.RemoveNumbers = 42; // 42
                }
                else if (difficulty == "Medium")
                {
                    generatorGameLogic.RemoveNumbers = 50; // 50
                }
                else if (difficulty == "Hard")
                {
                    generatorGameLogic.RemoveNumbers = 57; // 57
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

            LabelValidateText = "";

            NumbersList = generatorGameLogic.NumbersList;

            markersListValue = new MarkersListModel();
            markersListValue.InitializeList();
            MarkersList = markersListValue;

            ButtonDifficultyText = "Neues Spiel";
        }
        private void ValidateAll()
        {
            ButtonValidateVisibility = "Collapsed";
            LabelValidateVisibility = "Visible";

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
                        LabelValidateText = "Konflikte gefunden!";
                        return;
                    }
                }
            }
            LabelValidateText = "Keine Konflikte!";
        }
        private void HideAll()
        {
            SelectDifficultyVisibility = "Hidden";
            SelectNumberVisibility = "Hidden";
            SelectMarkerVisibility = "Hidden";
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
