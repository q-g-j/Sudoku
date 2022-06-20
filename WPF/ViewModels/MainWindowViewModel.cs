using System;
using System.Printing;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Sudoku.GameLogic;
using Sudoku.Models;
using Sudoku.Helpers;
using Sudoku.Properties;
using Sudoku.Settings;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Sudoku.Views;

namespace Sudoku.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructors
        public MainWindowViewModel()
        {
            // initialize fields:
            folderAppSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SudokuGame");
            appSettings = new AppSettings(folderAppSettings);
            doBlockInput = false;
            doStopTrophy = false;
            currentlySelectedCoords = "";

            // initialize lists:
            saveSlotsModel = new SaveSlotsModel(folderAppSettings);
            generatorCoordsList = new List<string>();
            numberList = new NumberListModel();
            markerList = new MarkerListModel();
            numberColorList = new NumberColorListModel();
            buttonBackgroundList = new ButtonBackgroundListModel();
            numberList.InitializeList();
            markerList.InitializeList();
            numberColorList.InitializeList();
            buttonBackgroundList.InitializeList();
            highlightedCoordsList = new List<string>();
            conflictCoordsList = new List<string>();

            // initialize property values:
            labelSelectNumberOrMarker = "";
            buttonSelectNumberOrMarker = Colors.ButtonSelectNumber;
            labelSingleSolutionWaitVisibility = "Hidden";
            validationVisibility = "Collapsed";
            selectDifficultyVisibility = "Visible";
            buttonValidateVisibility = "Visible";
            labelValidateVisibility = "Collapsed";
            trophyVisibility = "Collapsed";
            trophyWidth = "0";

            // initialize commands:
            MenuNewGameCommand = new RelayCommand(MenuNewGameAction);
            MenuEmptySudokuCommand = new RelayCommand(MenuEmptySudokuAction);
            MenuSolveCommand = new RelayCommand(MenuSolveAction);
            MenuFillAllMarkersCommand = new RelayCommand(MenuFillAllMarkersAction);
            MenuSettingsSingleSolutionCommand = new RelayCommand(MenuSettingsSingleSolutionAction);
            MenuSaveToSlotCommand = new RelayCommand<object>(o => MenuSaveToSlotAction(o));
            MenuLoadFromSlotCommand = new RelayCommand<object>(o => MenuLoadFromSlotAction(o));
            MenuDeleteAllSlotsCommand = new RelayCommand(MenuDeleteAllSlotsAction);
            MenuPrintCommand = new RelayCommand<object>(o => MenuPrintAction(o));
            MenuQuitCommand = new RelayCommand(MenuQuitAction);
            ButtonDifficultyEasyCommand = new AsyncRelayCommand(ButtonDifficultyEasyAction);
            ButtonDifficultyMediumCommand = new AsyncRelayCommand(ButtonDifficultyMediumAction);
            ButtonDifficultyHardCommand = new AsyncRelayCommand(ButtonDifficultyHardAction);
            ButtonSelectNumberOrMarkerCommand = new RelayCommand<object>(o => ButtonSelectNumberOrMarkerAction(o));

            ButtonSquareDownCommand = new AsyncRelayCommand<object>(o => ButtonSquareDownAction(o));
            ButtonSquareUpCommand = new AsyncRelayCommand<object>(o => ButtonSquareUpAction(o));
            ButtonSquareMouseEnterCommand = new RelayCommand<object>(o => ButtonSquareMouseEnterAction(o));
            ButtonSquareMouseLeaveCommand = new RelayCommand<object>(o => ButtonSquareMouseLeaveAction(o));
            KeyboardCommand = new RelayCommand<object>(o => KeyboardAction(o));

            // load app settings from file:
            if (!Directory.Exists(folderAppSettings))
            {
                Directory.CreateDirectory(folderAppSettings);
            }
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
            if (appSettingsStruct.SingleSolution)
            {
                menuSingleSolutionCheck = "True";
            }
            else
            {
                menuSingleSolutionCheck = "False";
            }
            // display each existing save slot's date and time:
            menuSaveSlotsLoadText = saveSlotsModel.GetLoadTexts();

            // preload games in three difficulties:
            PreloadGames();
        }
        #endregion Constructors

        #region Fields
        private readonly AppSettings appSettings;
        private readonly SaveSlotsModel saveSlotsModel;
        private string currentlySelectedCoords;
        private readonly List<string> highlightedCoordsList;
        private readonly List<string> conflictCoordsList;
        private List<string> generatorCoordsList;
        private readonly string folderAppSettings;
        private bool doBlockInput;
        private bool doStopTrophy;
        private string leftOrRightClicked;
        private NumberListModel numberListPreloadedEasy;
        private NumberListModel numberListPreloadedMedium;
        private NumberListModel numberListPreloadedHard;
        private NumberColorListModel numberColorListPreloadedEasy;
        private NumberColorListModel numberColorListPreloadedMedium;
        private NumberColorListModel numberColorListPreloadedHard;
        private List<string> generatorCoordsPreloadedEasy;
        private List<string> generatorCoordsPreloadedMedium;
        private List<string> generatorCoordsPreloadedHard;
        private Task preloadGameEasy;
        private Task preloadGameMedium;
        private Task preloadGameHard;
        #endregion Fields

        #region Property Values
        private NumberListModel numberList;
        private MarkerListModel markerList;
        private NumberColorListModel numberColorList;
        private ButtonBackgroundListModel buttonBackgroundList;

        private List<string> menuSaveSlotsLoadText;
        private List<string> menuSaveSlotsSaveText;
        private string menuSingleSolutionCheck;

        private string trophyWidth;

        private string labelSelectNumberOrMarker;
        private string buttonSelectNumberOrMarker;
        private string labelValidate;
        private string labelValidateBackground;

        private string selectNumberOrMarkerVisibility;
        private string validationVisibility;
        private string selectDifficultyVisibility;
        private string buttonValidateVisibility;
        private string labelValidateVisibility;
        private string labelSingleSolutionWaitVisibility;
        private string trophyVisibility;
        #endregion Property Values

        #region Properties
        public RelayCommand MenuNewGameCommand { get; }
        public RelayCommand MenuEmptySudokuCommand { get; }
        public RelayCommand MenuSolveCommand { get; }
        public RelayCommand MenuFillAllMarkersCommand { get; }
        public RelayCommand MenuSettingsCommand { get; }
        public RelayCommand MenuSettingsSingleSolutionCommand { get; }
        public RelayCommand<object> MenuSaveToSlotCommand { get; }
        public RelayCommand<object> MenuLoadFromSlotCommand { get; }
        public RelayCommand MenuDeleteAllSlotsCommand { get; }
        public RelayCommand<object> MenuPrintCommand { get; }
        public RelayCommand MenuQuitCommand { get; }
        public IAsyncRelayCommand ButtonDifficultyEasyCommand { get; }
        public IAsyncRelayCommand ButtonDifficultyMediumCommand { get; }
        public IAsyncRelayCommand ButtonDifficultyHardCommand { get; }
        public RelayCommand<object> ButtonSelectNumberOrMarkerCommand { get; }
        public RelayCommand<object> ButtonSelectMarkerCommand { get; }
        public IAsyncRelayCommand ButtonSquareDownCommand { get; }
        public IAsyncRelayCommand ButtonSquareUpCommand { get; }
        public RelayCommand<object> ButtonSquareMouseEnterCommand { get; }
        public RelayCommand<object> ButtonSquareMouseLeaveCommand { get; }
        public RelayCommand<object> KeyboardCommand { get; }

        public string TrophyWidth
        {
            get => trophyWidth;
            set { trophyWidth = value; OnPropertyChanged(); }
        }
        public List<string> MenuSaveSlotsLoadText
        {
            get => menuSaveSlotsLoadText;
            set { menuSaveSlotsLoadText = value; OnPropertyChanged(); }
        }
        public List<string> MenuSaveSlotsSaveText
        {
            get => menuSaveSlotsSaveText;
            set { menuSaveSlotsSaveText = value; OnPropertyChanged(); }
        }
        public string MenuSingleSolutionCheck
        {
            get => menuSingleSolutionCheck;
            set { menuSingleSolutionCheck = value; OnPropertyChanged(); }
        }
        public NumberListModel NumberList
        {
            get => numberList;
            set { numberList = value; OnPropertyChanged(); }
        }
        public MarkerListModel MarkerList
        {
            get => markerList;
            set { markerList = value; OnPropertyChanged(); }
        }
        public NumberColorListModel NumberColorList
        {
            get => numberColorList;
            set { numberColorList = value; OnPropertyChanged(); }
        }
        public ButtonBackgroundListModel ButtonBackgroundList
        {
            get => buttonBackgroundList;
            set { buttonBackgroundList = value; OnPropertyChanged(); }
        }
        public string LabelSelectNumberOrMarker
        {
            get => labelSelectNumberOrMarker;
            set { labelSelectNumberOrMarker = value; OnPropertyChanged(); }
        }
        public string ButtonSelectNumberOrMarker
        {
            get => buttonSelectNumberOrMarker;
            set { buttonSelectNumberOrMarker = value; OnPropertyChanged(); }
        }
        public string LabelValidate
        {
            get => labelValidate;
            set { labelValidate = value; OnPropertyChanged(); }
        }
        public string LabelValidateBackground
        {
            get => labelValidateBackground;
            set { labelValidateBackground = value; OnPropertyChanged(); }
        }
        public string SelectNumberOrMarkerVisibility
        {
            get => selectNumberOrMarkerVisibility;
            set { selectNumberOrMarkerVisibility = value; OnPropertyChanged(); }
        }
        public string ValidationVisibility
        {
            get => validationVisibility;
            set { validationVisibility = value; OnPropertyChanged(); }
        }
        public string SelectDifficultyVisibility
        {
            get => selectDifficultyVisibility;
            set { selectDifficultyVisibility = value; OnPropertyChanged(); }
        }
        public string ButtonValidateVisibility
        {
            get => buttonValidateVisibility;
            set { buttonValidateVisibility = value; OnPropertyChanged(); }
        }
        public string LabelValidateVisibility
        {
            get => labelValidateVisibility;
            set { labelValidateVisibility = value; OnPropertyChanged(); }
        }
        public string LabelSingleSolutionWaitVisibility
        {
            get => labelSingleSolutionWaitVisibility;
            set { labelSingleSolutionWaitVisibility = value; OnPropertyChanged(); }
        }
        public string TrophyVisibility
        {
            get => trophyVisibility;
            set { trophyVisibility = value; OnPropertyChanged(); }
        }
        #endregion Properties

        #region Command Actions
        private void MenuNewGameAction()
        {
            if (!doBlockInput)
            {
                currentlySelectedCoords = "";
                ResetBackground();
                LabelSelectNumberOrMarker = "";
                ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                SelectDifficultyVisibility = "Visible";
                TrophyVisibility = "Collapsed";
            }
        }
        private void MenuEmptySudokuAction()
        {
            if (!doBlockInput)
            {
                HideOverlays();
                currentlySelectedCoords = "";
                ResetBackground();
                LabelSelectNumberOrMarker = "";
                ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                SelectNumberOrMarkerVisibility = "Visible";
                ValidationVisibility = "Collapsed";
                TrophyVisibility = "Collapsed";
                numberList = new NumberListModel();
                markerList = new MarkerListModel();
                numberColorList = new NumberColorListModel();
                generatorCoordsList = new List<string>();
                numberList.InitializeList();
                markerList.InitializeList();
                numberColorList.InitializeList();
                NumberList = numberList;
                MarkerList = markerList;
                NumberColorList = numberColorList;
            }
        }
        private async void MenuSolveAction()
        {
            if (!doBlockInput)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        string stringCoords = i.ToString() + j.ToString();
                        RestoreCoordsBackground(stringCoords);
                    }
                }
                var menuSolveTask = MenuSolveTask();
                await menuSolveTask;
            }
        }
        private async Task MenuSolveTask()
        {
            await Task.Run(async () =>
            {
                doBlockInput = true;
                bool isValid = true;
                LabelSelectNumberOrMarker = "";
                ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                SolverGameLogic solverGameLogic = new SolverGameLogic(numberList);

                if (!SolverGameLogic.IsFull(numberList))
                {
                    for (int col = 0; col < 9; col++)
                    {
                        if (isValid)
                        {
                            for (int row = 0; row < 9; row++)
                            {
                                string number = numberList[col][row];
                                if (!ValidatorGameLogic.IsValid(numberList, col, row, number))
                                {
                                    isValid = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (isValid)
                    {
                        HideOverlays();
                        LabelSingleSolutionWaitVisibility = "Visible";
                        bool hasUserPlacedANumber = false;
                        bool isEmpty = true;
                        for (int col = 0; col < 9; col++)
                        {
                            if (hasUserPlacedANumber)
                            {
                                break;
                            }
                            for (int row = 0; row < 9; row++)
                            {
                                string stringCoords = col.ToString() + row.ToString();
                                if (numberList[col][row] != "" && !generatorCoordsList.Contains(stringCoords))
                                {
                                    hasUserPlacedANumber = true;
                                    break;
                                }
                            }
                        }
                        for (int col = 0; col < 9; col++)
                        {
                            if (!isEmpty)
                            {
                                break;
                            }
                            for (int row = 0; row < 9; row++)
                            {
                                if (numberList[col][row] != "")
                                {
                                    isEmpty = false;
                                    break;
                                }
                            }
                        }
                        Task fillSudokuTask = FillSudokuTask(solverGameLogic);
                        await fillSudokuTask;
                        LabelSingleSolutionWaitVisibility = "Collapsed";
                        numberColorList = new NumberColorListModel();
                        numberColorList.InitializeList();
                        foreach (string stringCoords in generatorCoordsList)
                        {
                            Coords coords = Coords.StringToCoords(stringCoords);
                            numberColorList[coords.Col][coords.Row] = Colors.CellNumberGenerator;
                        }
                        NumberColorList = numberColorList;
                        if (solverGameLogic.Tries < 700000 && solverGameLogic.NumberListSolved != null)
                        {
                            numberList = new NumberListModel(solverGameLogic.NumberListSolved);
                        }
                        NumberList = numberList;
                    }

                    if (solverGameLogic.Tries < 700000 && solverGameLogic.NumberListSolved != null)
                    {
                        ValidateAll(false);
                        SelectNumberOrMarkerVisibility = "Collapsed";
                        ValidationVisibility = "Visible";
                    }
                    else
                    {
                        ValidateAll(false);
                        LabelValidate = Resources.LabelValidateUnsolvable;
                        LabelValidateBackground = Colors.LabelValidateHasConflicts;
                        SelectNumberOrMarkerVisibility = "Collapsed";
                        LabelValidateVisibility = "Visible";
                        ValidationVisibility = "Visible";
                    }
                    for (int col = 0; col < 9; col++)
                    {
                        for (int row = 0; row < 9; row++)
                        {
                            if (numberList[col][row] != "")
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        markerList[col][row][i][j] = "";
                                    }
                                }
                            }
                        }
                    }
                    MarkerList = markerList;

                    if (currentlySelectedCoords != "")
                    {
                        Coords currentCoords = Coords.StringToCoords(currentlySelectedCoords);
                        if (numberList[currentCoords.Col][currentCoords.Row] != "" && leftOrRightClicked == "Right")
                        {
                            currentlySelectedCoords = "";
                            UnhighlightColRowSquare();
                        }
                    }
                }
                doBlockInput = false;
            });
        }
        private async Task FillSudokuTask(SolverGameLogic solverGameLogic)
        {
            await Task.Run(() =>
            {
                solverGameLogic.FillSudoku();
            });
        }
        private void MenuFillAllMarkersAction()
        {
            if (!doBlockInput)
            {
                markerList = new MarkerListModel();
                markerList.InitializeList();

                HideOverlays();
                for (int col = 0; col < 9; col++)
                {
                    for (int row = 0; row < 9; row++)
                    {
                        // 00|10|20|30
                        // 01|11|21|31
                        // 02|12|22|32

                        if (markerList[col][row][0][0] == "" && numberList[col][row] == "")
                        {
                            if (ValidatorGameLogic.IsValid(numberList, col, row, "1"))
                            {
                                markerList[col][row][0][0] = "1";
                            }
                        }
                        if (markerList[col][row][1][0] == "" && numberList[col][row] == "")
                        {
                            if (ValidatorGameLogic.IsValid(numberList, col, row, "2"))
                            {
                                markerList[col][row][1][0] = "2";
                            }
                        }
                        if (markerList[col][row][2][0] == "" && numberList[col][row] == "")
                        {
                            if (ValidatorGameLogic.IsValid(numberList, col, row, "3"))
                            {
                                markerList[col][row][2][0] = "3";
                            }
                        }
                        if (markerList[col][row][3][0] == "" && numberList[col][row] == "")
                        {
                            if (ValidatorGameLogic.IsValid(numberList, col, row, "4"))
                            {
                                markerList[col][row][3][0] = "4";
                            }
                        }
                        if (markerList[col][row][0][1] == "" && numberList[col][row] == "")
                        {
                            if (ValidatorGameLogic.IsValid(numberList, col, row, "5"))
                            {
                                markerList[col][row][0][1] = "5";
                            }
                        }
                        if (markerList[col][row][3][1] == "" && numberList[col][row] == "")
                        {
                            if (ValidatorGameLogic.IsValid(numberList, col, row, "6"))
                            {
                                markerList[col][row][3][1] = "6";
                            }
                        }
                        if (markerList[col][row][0][2] == "" && numberList[col][row] == "")
                        {
                            if (ValidatorGameLogic.IsValid(numberList, col, row, "7"))
                            {
                                markerList[col][row][0][2] = "7";
                            }
                        }
                        if (markerList[col][row][1][2] == "" && numberList[col][row] == "")
                        {
                            if (ValidatorGameLogic.IsValid(numberList, col, row, "8"))
                            {
                                markerList[col][row][1][2] = "8";
                            }
                        }
                        if (markerList[col][row][2][2] == "" && numberList[col][row] == "")
                        {
                            if (ValidatorGameLogic.IsValid(numberList, col, row, "9"))
                            {
                                markerList[col][row][2][2] = "9";
                            }
                        }
                    }
                    MarkerList = markerList;
                }
            }
        }
        private void MenuSettingsSingleSolutionAction()
        {
            if (!doBlockInput)
            {
                if (menuSingleSolutionCheck == "True")
                {
                    appSettings.ChangeSingleSolution(true);
                }
                else
                {
                    appSettings.ChangeSingleSolution(false);
                }
            }
        }
        private void MenuSaveToSlotAction(object o)
        {
            if (!doBlockInput)
            {
                currentlySelectedCoords = "";
                LabelSelectNumberOrMarker = "";
                ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                if (numberList != null && markerList != null && numberColorList != null && generatorCoordsList != null)
                {
                    DateTime now = DateTime.Now;
                    string slotNumber = (string)o;
                    saveSlotsModel.SaveAll(numberList, markerList, numberColorList, generatorCoordsList, now, slotNumber);
                    List<string> tempLoadList = menuSaveSlotsLoadText;
                    if (slotNumber == "1") tempLoadList[0] = Resources.MenuGameSaveSlotsLoadFromSlot1 + " (" + now + ")";
                    else if (slotNumber == "2") tempLoadList[1] = Resources.MenuGameSaveSlotsLoadFromSlot2 + " (" + now + ")";
                    else if (slotNumber == "3") tempLoadList[2] = Resources.MenuGameSaveSlotsLoadFromSlot3 + " (" + now + ")";
                    else if (slotNumber == "4") tempLoadList[3] = Resources.MenuGameSaveSlotsLoadFromSlot4 + " (" + now + ")";
                    else if (slotNumber == "5") tempLoadList[4] = Resources.MenuGameSaveSlotsLoadFromSlot5 + " (" + now + ")";
                    MenuSaveSlotsLoadText = tempLoadList;
                }
                ValidateAll(false);
            }
        }
        private void MenuLoadFromSlotAction(object o)
        {
            if (!doBlockInput)
            {
                TrophyVisibility = "Collapsed";
                currentlySelectedCoords = "";
                ResetBackground();
                LabelSelectNumberOrMarker = "";
                ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                string slotNumber = (string)o;
                string filename = Path.Combine(folderAppSettings, "slot" + slotNumber + ".json");
                if (File.Exists(filename))
                {
                    HideOverlays();
                    SaveSlotStruct saveSlot = saveSlotsModel.LoadAll(slotNumber);
                    NumberList = saveSlot.NumberList;
                    MarkerList = saveSlot.MarkerList;
                    NumberColorList = saveSlot.NumberColorsList;
                    generatorCoordsList = saveSlot.GeneratorNumberList;
                    CheckIsFull(false);
                    currentlySelectedCoords = "";
                    UnhighlightColRowSquare();
                }
            }
        }
        private void MenuDeleteAllSlotsAction()
        {
            if (!doBlockInput)
            {
                for (int i = 1; i < 6; i++)
                {
                    string filename = Path.Combine(folderAppSettings, "slot" + i.ToString() + ".json");
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                }

                List<string> tempLoadList = menuSaveSlotsLoadText;
                tempLoadList[0] = Resources.MenuGameSaveSlotsLoadFromSlot1;
                tempLoadList[1] = Resources.MenuGameSaveSlotsLoadFromSlot2;
                tempLoadList[2] = Resources.MenuGameSaveSlotsLoadFromSlot3;
                tempLoadList[3] = Resources.MenuGameSaveSlotsLoadFromSlot4;
                tempLoadList[4] = Resources.MenuGameSaveSlotsLoadFromSlot5;
                MenuSaveSlotsLoadText = tempLoadList;
            }
        }
        private void MenuPrintAction(object o)
        {
            if (!doBlockInput)
            {
                TrophyVisibility = "Collapsed";
                HideOverlays();

                MainWindow mainWindow = (MainWindow)o;
                PrintView printControl = new PrintView
                {
                    DataContext = mainWindow.DataContext
                };
                Viewbox sudokuGrid = (Viewbox)printControl.FindName("SudokuGrid");

                PrintDialog printDialog = new PrintDialog();

                if (printDialog.ShowDialog().GetValueOrDefault())
                {
                    System.Threading.Thread.Sleep(100);

                    //store original scale
                    Transform originalScale = sudokuGrid.LayoutTransform;

                    //get selected printer capabilities
                    PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);

                    //get scale of the print wrt to screen of WPF visual
                    double scale = capabilities.PageImageableArea.ExtentWidth / sudokuGrid.Width;

                    double factor = 0.5;

                    //Transform the Visual to scale
                    sudokuGrid.LayoutTransform = new ScaleTransform(Math.Round(scale * factor, 1), Math.Round(scale * factor, 1));

                    //get the size of the printer page
                    Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                    //update the layout of the visual to the printer page size.
                    sudokuGrid.Measure(sz);

                    sudokuGrid.Arrange(new Rect(
                        new Point(
                        capabilities.PageImageableArea.OriginWidth,
                        capabilities.PageImageableArea.OriginHeight),
                        sz));

                    //now print the visual to printer to fit on the one page.
                    printDialog.PrintVisual(sudokuGrid, "My Print");

                    //apply the original transform.
                    sudokuGrid.LayoutTransform = originalScale;
                }

                ValidateAll(false);
            }
        }
        private void MenuQuitAction()
        {
            Application.Current.Shutdown();
        }
        private async Task ButtonDifficultyEasyAction()
        {
            if (!doBlockInput)
            {
                HideOverlays();
                await NewGame("Easy");
            }
        }
        private async Task ButtonDifficultyMediumAction()
        {
            if (!doBlockInput)
            {
                HideOverlays();
                await NewGame("Medium");
            }
        }
        private async Task ButtonDifficultyHardAction()
        {
            if (!doBlockInput)
            {
                HideOverlays();
                await NewGame("Hard");
            }
        }
        private async Task ButtonSquareDownAction(object o)
        {
            var e = (MouseButtonEventArgs)((CompositeCommandParameter)o).EventArgs;
            if (!doBlockInput)
            {
                await Task.Run(() =>
                {
                    var param = (string)((CompositeCommandParameter)o).Parameter;

                    if (e.ChangedButton == MouseButton.Left)
                    {
                        if (SelectDifficultyVisibility == "Visible")
                        {
                            SelectDifficultyVisibility = "Hidden";
                            return;
                        }
                        if (!generatorCoordsList.Contains(param))
                        {
                            leftOrRightClicked = "Left";
                            Coords coords = Coords.StringToCoords(param);
                            UnhighlightColRowSquare();
                            HighlightColRowSquare(coords);
                            ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                            RestoreOldCoordsBackground();
                            currentlySelectedCoords = param;
                            LabelSelectNumberOrMarker = Resources.LabelSelectNumber;
                            if (conflictCoordsList.Contains(param))
                            {
                                buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundConflictsSelected;
                            }
                            else
                            {
                                buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundLeftSelected;
                            }
                            ButtonBackgroundList = buttonBackgroundList;
                        }
                        else
                        {
                            UnhighlightColRowSquare();
                            ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                            LabelSelectNumberOrMarker = "";
                            currentlySelectedCoords = "";
                            leftOrRightClicked = "";
                        }
                    }
                    else if (e.ChangedButton == MouseButton.Right)
                    {
                        if (SelectDifficultyVisibility == "Visible")
                        {
                            SelectDifficultyVisibility = "Hidden";
                            return;
                        }
                        Coords coords = Coords.StringToCoords(param);
                        if (!generatorCoordsList.Contains(param) && numberList[coords.Col][coords.Row] == "")
                        {
                            leftOrRightClicked = "Right";
                            UnhighlightColRowSquare();
                            HighlightColRowSquare(coords);
                            ButtonSelectNumberOrMarker = Colors.ButtonSelectMarker;
                            RestoreOldCoordsBackground();
                            currentlySelectedCoords = param;
                            LabelSelectNumberOrMarker = Resources.LabelSelectMarker;
                            buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundRightSelected;
                            ButtonBackgroundList = buttonBackgroundList;
                        }
                        else if (!generatorCoordsList.Contains(param) && numberList[coords.Col][coords.Row] != "")
                        {
                            if (highlightedCoordsList.Count != 0)
                            {
                                UnhighlightColRowSquare();
                            }
                            ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                            LabelSelectNumberOrMarker = "";
                            currentlySelectedCoords = "";
                            leftOrRightClicked = "";
                            buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundMouseOver;
                            ButtonBackgroundList = buttonBackgroundList;
                        }
                        else
                        {
                            if (highlightedCoordsList.Count != 0)
                            {
                                UnhighlightColRowSquare();
                            }
                            ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                            LabelSelectNumberOrMarker = "";
                            currentlySelectedCoords = "";
                            leftOrRightClicked = "";
                        }
                    }
                });
            }

            e.Handled = true;
        }
        private async Task ButtonSquareUpAction(object o)
        {
            var e = (MouseButtonEventArgs)((CompositeCommandParameter)o).EventArgs;
            await Task.Run(() =>
            {
            });

            e.Handled = true;
        }
        private void ButtonSquareMouseEnterAction(object o)
        {
            var e = (MouseEventArgs)((CompositeCommandParameter)o).EventArgs;
            if (!doBlockInput)
            {
                var param = (string)((CompositeCommandParameter)o).Parameter;
                Coords coords = Coords.StringToCoords(param);
                buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundMouseOver;
                ButtonBackgroundList = buttonBackgroundList;
            }

            e.Handled = true;
        }
        private void ButtonSquareMouseLeaveAction(object o)
        {
            var e = (MouseEventArgs)((CompositeCommandParameter)o).EventArgs;
            if (!doBlockInput)
            {
                var param = (string)((CompositeCommandParameter)o).Parameter;
                RestoreCoordsBackground(param);
            }

            e.Handled = true;
        }
        private void KeyboardAction(object o)
        {
            if (!doBlockInput && currentlySelectedCoords != "")
            {
                string key = (string)o;
                string param = currentlySelectedCoords + key;
                if (leftOrRightClicked == "Left")
                {
                    ChangeNumber(param);
                }
                else if (leftOrRightClicked == "Right")
                {
                    ChangeMarker(param);
                }
            }
        }
        private void ButtonSelectNumberOrMarkerAction(object o)
        {
            if (!doBlockInput && currentlySelectedCoords != "")
            {
                var tag = (string)o;
                string param = currentlySelectedCoords + tag;
                if (leftOrRightClicked == "Left")
                {
                    ChangeNumber(param);
                }
                else if (leftOrRightClicked == "Right")
                {
                    ChangeMarker(param);
                }
            }
        }
        #endregion Command Actions

        #region Methods
        private void InititalizeLists()
        {
            if (numberList == null)
            {
                numberList = new NumberListModel();
                numberList.InitializeList();
            }
            if (markerList == null)
            {
                markerList = new MarkerListModel();
                markerList.InitializeList();
            }
            if (numberColorList == null)
            {
                numberColorList = new NumberColorListModel();
                numberColorList.InitializeList();
            }
        }
        private void ChangeNumber(string param)
        {
            InititalizeLists();

            if (generatorCoordsList == null)
            {
                generatorCoordsList = new List<string>();
            }

            string stringCoords = param.Substring(0, 2);
            if (!generatorCoordsList.Contains(stringCoords))
            {
                Coords coords = Coords.StringToCoords(param);
                string number = param[2].ToString();

                if (number == "X")
                {
                    numberList[coords.Col][coords.Row] = "";
                    NumberList = numberList;
                    SelectNumberOrMarkerVisibility = "Visible";
                    ValidationVisibility = "Collapsed";
                    if (TrophyVisibility == "Visible")
                    {
                        doStopTrophy = true;
                        System.Threading.Thread.Sleep(100);
                        TrophyVisibility = "Collapsed";
                        CheckIsFull(true);
                    }
                    ResetBackground();
                    HighlightColRowSquare(coords);
                    ValidateAll(false);
                    return;
                }
                else if (numberList[coords.Col][coords.Row] != number)
                {
                    numberList[coords.Col][coords.Row] = number;
                    NumberList = numberList;
                    SelectNumberOrMarkerVisibility = "Visible";
                    ValidationVisibility = "Collapsed";
                    if (TrophyVisibility == "Visible")
                    {
                        doStopTrophy = true;
                        System.Threading.Thread.Sleep(100);
                        TrophyVisibility = "Collapsed";
                        CheckIsFull(true);
                    }
                    else
                    {
                        CheckIsFull(true);
                    }
                    ResetBackground();
                    HighlightColRowSquare(coords);
                    ValidateAll(false);
                    for (int j = 0; j < 3; j++)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (i == 1 && j == 1)
                            {
                                continue;
                            }
                            else
                            {
                                markerList[coords.Col][coords.Row][i][j] = "";
                            }
                        }
                    }
                    for (int k = 0; k < 9; k++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (i == 1 && j == 1)
                                {
                                    continue;
                                }
                                else
                                {
                                    if (markerList[coords.Col][k][i][j] == number)
                                    {
                                        markerList[coords.Col][k][i][j] = "";
                                    }
                                }
                            }
                        }
                    }
                    for (int k = 0; k < 9; k++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                if (i == 1 && j == 1)
                                {
                                    continue;
                                }
                                else
                                {
                                    if (markerList[k][coords.Row][i][j] == number)
                                    {
                                        markerList[k][coords.Row][i][j] = "";
                                    }
                                }
                            }
                        }
                    }

                    int col2 = (int)(coords.Col / 3) * 3;
                    int row2 = (int)(coords.Row / 3) * 3;

                    for (int i = row2; i < row2 + 3; i++)
                    {
                        for (int j = col2; j < col2 + 3; j++)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                for (int l = 0; l < 3; l++)
                                {
                                    if (k == 1 && l == 1)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        if (markerList[j][i][k][l] == number)
                                        {
                                            markerList[j][i][k][l] = "";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    MarkerList = markerList;
                }
            }
        }
        private void ChangeMarker(string param)
        {
            InititalizeLists();

            Coords coords = Coords.StringToCoords(param);
            string number = param[2].ToString();

            if (numberList[coords.Col][coords.Row] != "")
            {
                return;
            }

            if (number == "X")
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if ((i == 1 && j == 1) || (i == 2 && j == 1) || (i == 3 && j == 2))
                        {
                            continue;
                        }
                        else
                        {
                            markerList[coords.Col][coords.Row][i][j] = "";
                            MarkerList = markerList;
                        }
                    }
                }
                return;
            }
            else
            {
                // 00|10|20|30
                // 01|11|21|31
                // 02|12|22|32

                if (number == "1")
                {
                    if (markerList[coords.Col][coords.Row][0][0] != "") { markerList[coords.Col][coords.Row][0][0] = ""; }
                    else { markerList[coords.Col][coords.Row][0][0] = "1"; }
                }
                else if (number == "2")
                {
                    if (markerList[coords.Col][coords.Row][1][0] != "") { markerList[coords.Col][coords.Row][1][0] = ""; }
                    else { markerList[coords.Col][coords.Row][1][0] = "2"; }
                }
                else if (number == "3")
                {
                    if (markerList[coords.Col][coords.Row][2][0] != "") { markerList[coords.Col][coords.Row][2][0] = ""; }
                    else { markerList[coords.Col][coords.Row][2][0] = "3"; }
                }
                else if (number == "4")
                {
                    if (markerList[coords.Col][coords.Row][3][0] != "") { markerList[coords.Col][coords.Row][3][0] = ""; }
                    else { markerList[coords.Col][coords.Row][3][0] = "4"; }
                }
                else if (number == "5")
                {
                    if (markerList[coords.Col][coords.Row][0][1] != "") { markerList[coords.Col][coords.Row][0][1] = ""; }
                    else { markerList[coords.Col][coords.Row][0][1] = "5"; }
                }
                else if (number == "6")
                {
                    if (markerList[coords.Col][coords.Row][3][1] != "") { markerList[coords.Col][coords.Row][3][1] = ""; }
                    else { markerList[coords.Col][coords.Row][3][1] = "6"; }
                }
                else if (number == "7")
                {
                    if (markerList[coords.Col][coords.Row][0][2] != "") { markerList[coords.Col][coords.Row][0][2] = ""; }
                    else { markerList[coords.Col][coords.Row][0][2] = "7"; }
                }
                else if (number == "8")
                {
                    if (markerList[coords.Col][coords.Row][1][2] != "") { markerList[coords.Col][coords.Row][1][2] = ""; }
                    else { markerList[coords.Col][coords.Row][1][2] = "8"; }
                }
                else if (number == "9")
                {
                    if (markerList[coords.Col][coords.Row][2][2] != "") { markerList[coords.Col][coords.Row][2][2] = ""; }
                    else { markerList[coords.Col][coords.Row][2][2] = "9"; }
                }
                MarkerList = markerList;
            }
        }
        private void RestoreCoordsBackground(string stringCoords)
        {
            Coords coords = Coords.StringToCoords(stringCoords);
            if (conflictCoordsList.Contains(stringCoords))
            {
                if (currentlySelectedCoords == stringCoords)
                {
                    buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundConflictsSelected;
                }
                else
                {
                    buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundConflicts;
                }
            }
            else if (currentlySelectedCoords == stringCoords)
            {
                if (leftOrRightClicked == "Left")
                {
                    buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundLeftSelected;
                }
                else
                {
                    buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundRightSelected;
                }
            }
            else
            {
                if (highlightedCoordsList.Contains(stringCoords))
                {

                    if (leftOrRightClicked == "Left")
                    {
                        buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundLeftHighlighted;
                    }
                    else
                    {
                        buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundRightHighlighted;
                    }
                }
                else
                {
                    buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundDefault;
                }
            }
            ButtonBackgroundList = buttonBackgroundList;
        }
        private void RestoreOldCoordsBackground()
        {
            if (currentlySelectedCoords != "" && !conflictCoordsList.Contains(currentlySelectedCoords))
            {
                Coords coordsOld = Coords.StringToCoords(currentlySelectedCoords);
                if (highlightedCoordsList.Contains(currentlySelectedCoords))
                {
                    if (leftOrRightClicked == "Left")
                    {
                        buttonBackgroundList[coordsOld.Col][coordsOld.Row] = Colors.CellBackgroundLeftHighlighted;
                    }
                    else
                    {
                        buttonBackgroundList[coordsOld.Col][coordsOld.Row] = Colors.CellBackgroundRightHighlighted;
                    }
                }
                else
                {
                    buttonBackgroundList[coordsOld.Col][coordsOld.Row] = Colors.CellBackgroundDefault;
                }
                LabelSelectNumberOrMarker = "";
            }
            else if (currentlySelectedCoords != "" && conflictCoordsList.Contains(currentlySelectedCoords))
            {
                Coords coordsOld = Coords.StringToCoords(currentlySelectedCoords);
                buttonBackgroundList[coordsOld.Col][coordsOld.Row] = Colors.CellBackgroundConflicts;
            }
            ButtonBackgroundList = buttonBackgroundList;
        }
        private void ResetBackground()
        {
            highlightedCoordsList.Clear();
            conflictCoordsList.Clear();
            ButtonBackgroundList = new ButtonBackgroundListModel();
            ButtonBackgroundList.InitializeList();
        }
        private void HighlightColRowSquare(Coords coords)
        {
            // highlight column:
            for (int i = 0; i < 9; i++)
            {
                string stringCoords = coords.Col.ToString() + i.ToString();
                if (!highlightedCoordsList.Contains(stringCoords))
                {
                    if (!conflictCoordsList.Contains(stringCoords))
                    {
                        if (leftOrRightClicked == "Left")
                        {
                            buttonBackgroundList[coords.Col][i] = Colors.CellBackgroundLeftHighlighted;
                        }
                        else
                        {
                            buttonBackgroundList[coords.Col][i] = Colors.CellBackgroundRightHighlighted;
                        }
                    }
                    highlightedCoordsList.Add(stringCoords);
                }
            }

            // highlight row:
            for (int i = 0; i < 9; i++)
            {
                string stringCoords = i.ToString() + coords.Row.ToString();
                if (!highlightedCoordsList.Contains(stringCoords))
                {
                    if (!conflictCoordsList.Contains(stringCoords))
                    {
                        if (leftOrRightClicked == "Left")
                        {
                            buttonBackgroundList[i][coords.Row] = Colors.CellBackgroundLeftHighlighted;
                        }
                        else
                        {
                            buttonBackgroundList[i][coords.Row] = Colors.CellBackgroundRightHighlighted;
                        }
                    }
                    highlightedCoordsList.Add(stringCoords);
                }
            }

            // highlight square:
            int col = (int)(coords.Col / 3) * 3;
            int row = (int)(coords.Row / 3) * 3;

            for (int i = row; i < row + 3; i++)
            {
                for (int j = col; j < col + 3; j++)
                {
                    string stringCoords = j.ToString() + i.ToString();
                    if (!highlightedCoordsList.Contains(stringCoords))
                    {
                        if (!conflictCoordsList.Contains(stringCoords))
                        {
                            if (leftOrRightClicked == "Left")
                            {
                                buttonBackgroundList[j][i] = Colors.CellBackgroundLeftHighlighted;
                            }
                            else
                            {
                                buttonBackgroundList[j][i] = Colors.CellBackgroundRightHighlighted;
                            }
                        }
                        highlightedCoordsList.Add(stringCoords);
                    }
                }
            }

            if (leftOrRightClicked == "Left")
            {
                buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundLeftSelected;
            }
            else
            {
                buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundRightSelected;
            }
            ButtonBackgroundList = buttonBackgroundList;
        }
        private void UnhighlightColRowSquare()
        {
            for (int i = 0; i < highlightedCoordsList.Count; i++)
            {
                Coords highlightedCoords = Coords.StringToCoords(highlightedCoordsList[i]);
                if (!conflictCoordsList.Contains(highlightedCoordsList[i]))
                {
                    buttonBackgroundList[highlightedCoords.Col][highlightedCoords.Row] = Colors.CellBackgroundDefault;
                }
                else
                {
                    buttonBackgroundList[highlightedCoords.Col][highlightedCoords.Row] = Colors.CellBackgroundConflicts;
                }
            }
            if (currentlySelectedCoords != "" && conflictCoordsList.Contains(currentlySelectedCoords))
            {
                Coords currentCoords = Coords.StringToCoords(currentlySelectedCoords);
                buttonBackgroundList[currentCoords.Col][currentCoords.Row] = Colors.CellBackgroundConflictsSelected;
            }
            ButtonBackgroundList = buttonBackgroundList;
            highlightedCoordsList.Clear();
        }
        private async Task NewGame(string difficulty)
        {
            doBlockInput = true;

            markerList = new MarkerListModel();
            markerList.InitializeList();
            MarkerList = markerList;

            if (difficulty == "Easy")
            {
                await preloadGameEasy;
                numberList = numberListPreloadedEasy;
                generatorCoordsList = generatorCoordsPreloadedEasy;
                NumberColorList = numberColorListPreloadedEasy;
            }
            else if (difficulty == "Medium")
            {
                await preloadGameMedium;
                numberList = numberListPreloadedMedium;
                generatorCoordsList = generatorCoordsPreloadedMedium;
                NumberColorList = numberColorListPreloadedMedium;
            }
            else if (difficulty == "Hard")
            {
                if (menuSingleSolutionCheck == "True")
                {
                    LabelSingleSolutionWaitVisibility = "Visible";
                }
                await preloadGameHard;
                numberList = numberListPreloadedHard;
                generatorCoordsList = generatorCoordsPreloadedHard;
                NumberColorList = numberColorListPreloadedHard;
                LabelSingleSolutionWaitVisibility = "Hidden";
            }
            NumberList = numberList;

            currentlySelectedCoords = "";

            SelectNumberOrMarkerVisibility = "Visible";
            ValidationVisibility = "Collapsed";

            PreloadGames();

            doBlockInput = false;
        }
        private async void PreloadGames()
        {
            preloadGameEasy = PreloadGameTask("Easy");
            await preloadGameEasy;
            preloadGameMedium = PreloadGameTask("Medium");
            await preloadGameMedium;
            preloadGameHard = PreloadGameTask("Hard");
            await preloadGameHard;
        }
        private async Task PreloadGameTask(string difficulty)
        {
            await Task.Run(() =>
            {
                NumberListModel tempNumberList = new NumberListModel();
                tempNumberList.InitializeList();
                GeneratorGameLogic generatorGameLogic = null;
                NumberColorListModel tempNumberColorList = new NumberColorListModel();
                bool doRun = true;

                while (doRun)
                {
                    SolverGameLogic solverGameLogic = new SolverGameLogic(tempNumberList);
                    solverGameLogic.FillSudoku();
                    if (difficulty == "Easy")
                    {
                        generatorGameLogic = new GeneratorGameLogic(difficulty, new NumberListModel(solverGameLogic.NumberListSolved));
                    }
                    else if (difficulty == "Medium")
                    {
                        generatorGameLogic = new GeneratorGameLogic(difficulty, new NumberListModel(solverGameLogic.NumberListSolved));
                    }
                    else if (difficulty == "Hard")
                    {
                        generatorGameLogic = new GeneratorGameLogic(difficulty, new NumberListModel(solverGameLogic.NumberListSolved));
                    }

                    generatorGameLogic.GenerateSudoku(menuSingleSolutionCheck);

                    if (generatorGameLogic.Counter == generatorGameLogic.removeNumbers)
                    {
                        doRun = false;
                    }
                }

                List<string> tempGeneratorCoords = new List<string>();

                for (int col = 0; col < 9; col++)
                {
                    for (int row = 0; row < 9; row++)
                    {
                        if (generatorGameLogic.NumberList[col][row] != "")
                        {
                            string coords = col.ToString();
                            coords += row.ToString();
                            tempGeneratorCoords.Add(coords);
                        }
                    }
                }

                tempNumberColorList.InitializeList();
                foreach (string stringCoords in tempGeneratorCoords)
                {
                    Coords coords = Coords.StringToCoords(stringCoords);

                    tempNumberColorList[coords.Col][coords.Row] = Colors.CellNumberGenerator;
                }

                if (difficulty == "Easy")
                {
                    numberListPreloadedEasy = new NumberListModel(generatorGameLogic.NumberList);
                    numberColorListPreloadedEasy = new NumberColorListModel(tempNumberColorList);
                    generatorCoordsPreloadedEasy = tempGeneratorCoords;
                }
                else if (difficulty == "Medium")
                {
                    numberListPreloadedMedium = new NumberListModel(generatorGameLogic.NumberList);
                    numberColorListPreloadedMedium = new NumberColorListModel(tempNumberColorList);
                    generatorCoordsPreloadedMedium = tempGeneratorCoords;
                }
                else if (difficulty == "Hard")
                {
                    numberListPreloadedHard = new NumberListModel(generatorGameLogic.NumberList);
                    numberColorListPreloadedHard = new NumberColorListModel(tempNumberColorList);
                    generatorCoordsPreloadedHard = tempGeneratorCoords;
                }

                doBlockInput = false;
            });
        }
        private void CheckIsFull(bool doShowTrophy)
        {
            if (!SolverGameLogic.IsFull(numberList))
            {
                ValidateAll(false);
                SelectNumberOrMarkerVisibility = "Visible";
                ValidationVisibility = "Collapsed";
            }
            else
            {
                ValidateAll(doShowTrophy);
                SelectNumberOrMarkerVisibility = "Collapsed";
                ValidationVisibility = "Visible";
            }
        }
        private async void ValidateAll(bool doShowTrophy)
        {
            if (numberList == null)
            {
                numberList = new NumberListModel();
                numberList.InitializeList();
            }

            bool isValid = true;

            LabelValidate = Resources.LabelValidateNoConflicts;
            LabelValidateBackground = Colors.LabelValidateHasNoConflicts;

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    string number = numberList[col][row];
                    if (numberList[col][row] != "" && !ValidatorGameLogic.IsValid(numberList, col, row, number))
                    {
                        isValid = false;
                        LabelValidate = Resources.LabelValidateConflicts;
                        LabelValidateBackground = Colors.LabelValidateHasConflicts;
                        buttonBackgroundList[col][row] = Colors.CellBackgroundConflicts;
                        string stringCoords = col.ToString() + row.ToString();
                        conflictCoordsList.Add(stringCoords);
                    }
                }
            }
            if (conflictCoordsList.Contains(currentlySelectedCoords))
            {
                Coords currentCoords = Coords.StringToCoords(currentlySelectedCoords);
                buttonBackgroundList[currentCoords.Col][currentCoords.Row] = Colors.CellBackgroundConflictsSelected;
            }

            ButtonBackgroundList = buttonBackgroundList;

            LabelValidateVisibility = "Visible";
            
            if (isValid && doShowTrophy)
            {
                doStopTrophy = true;
                System.Threading.Thread.Sleep(100);
                doStopTrophy = false;
                HideOverlays();
                
                TrophyWidth = "0";
                TrophyVisibility = "Visible";
                Task zoomInTrophy = ZoomInTrophy();
                await zoomInTrophy;
            }
        }
        private async Task ZoomInTrophy()
        {
            await Task.Run(() =>
            {
                for (int i = 1; i <= 501; i += 10)
                {
                    HiPerfTimer pt = new HiPerfTimer();
                    if (doStopTrophy)
                    {
                        break;
                    }
                    TrophyWidth = i.ToString();
                    pt.Start();
                    while(pt.Duration < 0.01)
                    {
                        System.Threading.Thread.Sleep(0);
                        pt.Stop();
                    }
                }

                for (int i = 1; i <= 10; i += 1)
                {
                    HiPerfTimer pt = new HiPerfTimer();
                    if (doStopTrophy)
                    {
                        break;
                    }
                    pt.Start();
                    while (pt.Duration < 0.1)
                    {
                        System.Threading.Thread.Sleep(0);
                        pt.Stop();
                    }
                }
                TrophyVisibility = "Collapsed";
            });
        }
        private void HideOverlays()
        {
            SelectDifficultyVisibility = "Hidden";
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
