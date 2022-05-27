using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Sudoku.GameLogic;
using Sudoku.Models;
using Sudoku.Helpers;
using Sudoku.Properties;

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

            List<string> list = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                if      (i == 0) list.Add(Resources.MenuGameSaveSlotsLoadFromSlot1);
                else if (i == 1) list.Add(Resources.MenuGameSaveSlotsLoadFromSlot2);
                else if (i == 2) list.Add(Resources.MenuGameSaveSlotsLoadFromSlot3);
                else if (i == 3) list.Add(Resources.MenuGameSaveSlotsLoadFromSlot4);
                else if (i == 4) list.Add(Resources.MenuGameSaveSlotsLoadFromSlot5);
                string filename = Path.Combine(folderAppSettings, "slot" + (i + 1).ToString() + ".json");
                if (File.Exists(filename))
                {
                    using (var file = File.OpenText(filename))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Dictionary<string, object> listsDict = (Dictionary<string, object>)serializer.Deserialize(file, typeof(Dictionary<string, object>));

                        DateTime dateAndTime = (DateTime)listsDict["DateAndTime"];
                        list[i] += " (" + dateAndTime.ToString() + ")";
                    }
                }
            }

            MenuSaveSlotsText = list;

            generatorNumbers = new List<string>();
            numbersListValue = new NumbersListModel();
            numbersColorsListValue = new NumbersColorsListModel();
            numbersListValue.InitializeList();
            numbersColorsListValue.InitializeList();

            MenuNewCommand = new AsyncRelayCommand(MenuNewAction);
            MenuSolveCommand = new AsyncRelayCommand(MenuSolveAction);
            MenuSettingsCommand = new AsyncRelayCommand(MenuSettingsAction);
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
        readonly string folderAppSettings;
        #endregion Private variables

        #region Property values
        private NumbersListModel numbersListValue;
        private MarkersListModel markersListValue;
        private NumbersColorsListModel numbersColorsListValue;

        private List<string> menuSaveSlotsTextValue;

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
        public IAsyncRelayCommand MenuSolveCommand { get; }
        public IAsyncRelayCommand MenuSettingsCommand { get; }
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

        public List<string> MenuSaveSlotsText
        {
            get => menuSaveSlotsTextValue;
            set { menuSaveSlotsTextValue = value; OnPropertyChanged(); }
        }
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
                HideOverlays();
                HideValidation();
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
        private async Task MenuSolveAction()
        {
            await Task.Run(() =>
            {
                if (!SolverGameLogic.IsFull(numbersListValue))
                {
                    HideOverlays();
                    SolverGameLogic solverGameLogic = new SolverGameLogic(numbersListValue);
                    solverGameLogic.FillSudoku();
                    markersListValue = new MarkersListModel();
                    numbersColorsListValue = new NumbersColorsListModel();
                    markersListValue.InitializeList();
                    numbersColorsListValue.InitializeList();
                    foreach (string coords in generatorNumbers)
                    {
                        int col = int.Parse(coords[0].ToString());
                        int row = int.Parse(coords[1].ToString());

                        numbersColorsListValue[col][row] = "Black";
                    }
                    MarkersList = markersListValue;
                    NumbersColorsList = numbersColorsListValue;
                    NumbersList = NumbersListModel.CopyList(solverGameLogic.NumbersListSolved);
                    ChangeButtonValidateVisibility();
                }
            });
        }
        private async Task MenuSettingsAction()
        {
            await Task.Run(() =>
            {
                ;
            });
        }
        private async Task MenuSaveToSlotAction(object o)
        {
            await Task.Run(() =>
            {
                if (numbersListValue != null && markersListValue != null && numbersColorsListValue != null && generatorNumbers != null)
                {
                    DateTime now = DateTime.Now;
                    string slotNumber = (string)o;
                    SaveSlotsModel saveSlots = new SaveSlotsModel(folderAppSettings);
                    saveSlots.SaveAll(numbersListValue, markersListValue, numbersColorsListValue, generatorNumbers, now, slotNumber);
                    List<string> tempList = menuSaveSlotsTextValue;
                    if      (slotNumber == "1") tempList[0] = Resources.MenuGameSaveSlotsLoadFromSlot1 + " (" + now + ")";
                    else if (slotNumber == "2") tempList[1] = Resources.MenuGameSaveSlotsLoadFromSlot2 + " (" + now + ")";
                    else if (slotNumber == "3") tempList[2] = Resources.MenuGameSaveSlotsLoadFromSlot3 + " (" + now + ")";
                    else if (slotNumber == "4") tempList[3] = Resources.MenuGameSaveSlotsLoadFromSlot4 + " (" + now + ")";
                    else if (slotNumber == "5") tempList[4] = Resources.MenuGameSaveSlotsLoadFromSlot5 + " (" + now + ")";
                    MenuSaveSlotsText = tempList;
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
                    HideOverlays();
                    SaveSlotsModel saveSlots = new SaveSlotsModel(folderAppSettings);
                    SaveSlotsModel.ListsStruct listsStruct = saveSlots.LoadAll(slotNumber);
                    NumbersList = listsStruct.NumbersList;
                    MarkersList = listsStruct.MarkersList;
                    NumbersColorsList = listsStruct.NumbersColorsList;
                    generatorNumbers = listsStruct.GeneratorNumbers;
                    if (SolverGameLogic.IsFull(numbersListValue))
                    {
                        ShowValidation();
                    }
                    else 
                    {
                        HideValidation();
                    }
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
                HideOverlays();
                NewGame("Easy");
            });
        }
        private async Task ButtonDifficultyMediumAction()
        {
            await Task.Run(() =>
            {
                HideOverlays();
                NewGame("Medium");
            });
        }
        private async Task ButtonDifficultyHardAction()
        {
            await Task.Run(() =>
            {
                HideOverlays();
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
                    HideValidation();
                }
                else
                {
                    ShowValidation();
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
            GeneratorGameLogic generatorGameLogic = null;

            bool doRun = true;

            while (doRun)
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
                    generatorGameLogic.RemoveNumbers = 56; // 57
                }

                generatorGameLogic.NumbersList = numbersListSolved;

                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();

                generatorGameLogic.GenerateSudoku();

                //stopwatch.Stop();
                //Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);

                if (generatorGameLogic.Counter == generatorGameLogic.RemoveNumbers)
                {
                    doRun = false;
                }
            }

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
            HideValidation();
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
                        LabelValidateText = Resources.LabelValidateConflicts;
                        return;
                    }
                }
            }
            LabelValidateText = Resources.LabelValidateNoConflicts;
        }
        private void HideOverlays()
        {
            SelectDifficultyVisibility = "Hidden";
            SelectNumberVisibility = "Hidden";
            SelectMarkerVisibility = "Hidden";
        }
        private void ShowValidation()
        {
            LabelValidateVisibility = "Collapsed";
            ButtonValidateVisibility = "Visible";
            ButtonDifficultyWidth = "250";
        }
        private void HideValidation()
        {
            ButtonValidateVisibility = "Collapsed";
            LabelValidateVisibility = "Collapsed";
            ButtonDifficultyWidth = "350";
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
