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
    public class MainWindowViewModel :INotifyPropertyChanged
    {
        #region Constructors
        public MainWindowViewModel()
        {
            // initialize variables:
            folderAppSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SudokuGame");
            appSettings = new AppSettings(folderAppSettings);
            doBlockInput = false;

            // initialize lists:
            saveSlotsModel = new SaveSlotsModel(folderAppSettings);
            generatorCoords = new List<string>();
            numberList = new NumberListModel();
            markerList = new MarkerListModel();
            numberColorList = new NumberColorListModel();
            buttonBackgroundList = new ButtonBackgroundListModel();
            numberList.InitializeList();
            markerList.InitializeList();
            numberColorList.InitializeList();
            buttonBackgroundList.InitializeList();
            highlightedCoords = new List<string>();
            conflictCoords = new List<string>();

            // initialize property values:
            labelSelectNumberOrMarker = "";
            buttonSelectNumberOrMarker = Colors.ButtonSelectNumber;
            labelSingleSolutionWaitVisibility = "Hidden";
            validationVisibility = "Collapsed";
            selectDifficultyVisibility = "Visible";
            buttonValidateVisibility = "Visible";
            labelValidateVisibility = "Collapsed";
            currentlyMarkedCoords = "";

            preloadGameEasy = PreloadGame("Easy");
            preloadGameMedium = PreloadGame("Medium");
            preloadGameHard = PreloadGame("Hard");

            // initialize commands:
            MenuNewGameCommand = new AsyncRelayCommand(MenuNewGameAction);
            MenuEmptySudokuCommand = new AsyncRelayCommand(MenuEmptySudokuAction);
            MenuSolveCommand = new AsyncRelayCommand(MenuSolveAction);
            MenuFillAllMarkersCommand = new AsyncRelayCommand(MenuFillAllMarkersAction);
            MenuSettingsSingleSolutionCommand = new AsyncRelayCommand(MenuSettingsSingleSolutionAction);
            MenuSaveToSlotCommand = new AsyncRelayCommand<object>(o => MenuSaveToSlotAction(o));
            MenuLoadFromSlotCommand = new AsyncRelayCommand<object>(o => MenuLoadFromSlotAction(o));
            MenuDeleteAllSlotsCommand = new AsyncRelayCommand(MenuDeleteAllSlotsAction);
            MenuPrintCommand = new RelayCommand<object>(o => MenuPrintAction(o));
            MenuQuitCommand = new RelayCommand(MenuQuitAction);
            ButtonDifficultyEasyCommand = new AsyncRelayCommand(ButtonDifficultyEasyAction);
            ButtonDifficultyMediumCommand = new AsyncRelayCommand(ButtonDifficultyMediumAction);
            ButtonDifficultyHardCommand = new AsyncRelayCommand(ButtonDifficultyHardAction);
            ButtonSelectNumberOrMarkerCommand = new AsyncRelayCommand<object>(o => ButtonSelectNumberOrMarkerAction(o));

            ButtonSquareDownCommand = new AsyncRelayCommand<object>(o => ButtonSquareDownAction(o));
            ButtonSquareUpCommand = new AsyncRelayCommand<object>(o => ButtonSquareUpAction(o));
            ButtonSquareMouseEnterCommand = new AsyncRelayCommand<object>(o => ButtonSquareMouseEnterAction(o));
            ButtonSquareMouseLeaveCommand = new AsyncRelayCommand<object>(o => ButtonSquareMouseLeaveAction(o));

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
        }
        #endregion Constructors

        #region Fields
        private readonly AppSettings appSettings;
        private readonly SaveSlotsModel saveSlotsModel;
        private string currentlyMarkedCoords;
        private readonly List<string> highlightedCoords;
        private readonly List<string> conflictCoords;
        private List<string> generatorCoords;
        private readonly string folderAppSettings;
        private bool doBlockInput;
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
        #endregion Property Values

        #region Properties
        public IAsyncRelayCommand MenuNewGameCommand { get; }
        public IAsyncRelayCommand MenuEmptySudokuCommand { get; }
        public IAsyncRelayCommand MenuSolveCommand { get; }
        public IAsyncRelayCommand MenuFillAllMarkersCommand { get; }
        public IAsyncRelayCommand MenuSettingsCommand { get; }
        public IAsyncRelayCommand MenuSettingsSingleSolutionCommand { get; }
        public IAsyncRelayCommand MenuSaveToSlotCommand { get; }
        public IAsyncRelayCommand MenuLoadFromSlotCommand { get; }
        public IAsyncRelayCommand MenuDeleteAllSlotsCommand { get; }
        public RelayCommand<object> MenuPrintCommand { get; }
        public RelayCommand MenuQuitCommand { get; }
        public IAsyncRelayCommand ButtonDifficultyEasyCommand { get; }
        public IAsyncRelayCommand ButtonDifficultyMediumCommand { get; }
        public IAsyncRelayCommand ButtonDifficultyHardCommand { get; }
        public IAsyncRelayCommand ButtonSelectNumberOrMarkerCommand { get; }
        public IAsyncRelayCommand ButtonSelectMarkerCommand { get; }
        public IAsyncRelayCommand ButtonSquareDownCommand { get; }
        public IAsyncRelayCommand ButtonSquareUpCommand { get; }
        public IAsyncRelayCommand ButtonSquareMouseEnterCommand { get; }
        public IAsyncRelayCommand ButtonSquareMouseLeaveCommand { get; }

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
            set {  markerList = value; OnPropertyChanged(); }
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
        #endregion Properties

        #region Command Actions
        private async Task MenuNewGameAction()
        {
            if (!doBlockInput)
            {
                await Task.Run(() =>
                {
                    currentlyMarkedCoords = "";
                    BackgroundReset();
                    LabelSelectNumberOrMarker = "";
                    ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                    SelectDifficultyVisibility = "Visible";
                });
            }
        }
        private async Task MenuEmptySudokuAction()
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                    currentlyMarkedCoords = "";
                    BackgroundReset();
                    LabelSelectNumberOrMarker = "";
                    ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                    SelectNumberOrMarkerVisibility = "Visible";
                    ValidationVisibility = "Collapsed";
                    numberList = new NumberListModel();
                    markerList = new MarkerListModel();
                    numberColorList = new NumberColorListModel();
                    generatorCoords = new List<string>();
                    numberList.InitializeList();
                    markerList.InitializeList();
                    numberColorList.InitializeList();
                    NumberList = numberList;
                    MarkerList = markerList;
                    NumberColorList = numberColorList;
                });
            }
        }
        private async Task MenuSolveAction()
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                    currentlyMarkedCoords = "";
                    LabelSelectNumberOrMarker = "";
                    ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                    BackgroundReset();
                    if (!SolverGameLogic.IsFull(numberList))
                    {
                        bool isValid = true;
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
                            SolverGameLogic solverGameLogic = new SolverGameLogic(numberList);
                            solverGameLogic.FillSudoku();
                            markerList = new MarkerListModel();
                            numberColorList = new NumberColorListModel();
                            markerList.InitializeList();
                            numberColorList.InitializeList();
                            foreach (string coords in generatorCoords)
                            {
                                int col = int.Parse(coords[0].ToString());
                                int row = int.Parse(coords[1].ToString());

                                numberColorList[col][row] = Colors.CellNumberGenerator;
                            }
                            MarkerList = markerList;
                            NumberColorList = numberColorList;
                            NumberList = new NumberListModel(solverGameLogic.NumberListSolved);
                        }
                    }
                    CheckIsFull();
                });
            }
        }
        private async Task MenuFillAllMarkersAction()
        {
            if (!doBlockInput)
            {
                await Task.Run(() =>
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
                });
            }
        }

        private async Task MenuSettingsSingleSolutionAction()
        {
            if (!doBlockInput)
            {
                await Task.Run(() =>
                {
                    if (menuSingleSolutionCheck == "True")
                    {
                        appSettings.ChangeSingleSolution(true);
                    }
                    else
                    {
                        appSettings.ChangeSingleSolution(false);
                    }
                });
            }
        }
        private async Task MenuSaveToSlotAction(object o)
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                    currentlyMarkedCoords = "";
                    BackgroundReset();
                    LabelSelectNumberOrMarker = "";
                    ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                    if (numberList != null && markerList != null && numberColorList != null && generatorCoords != null)
                    {
                        DateTime now = DateTime.Now;
                        string slotNumber = (string)o;
                        saveSlotsModel.SaveAll(numberList, markerList, numberColorList, generatorCoords, now, slotNumber);
                        List<string> tempLoadList = menuSaveSlotsLoadText;
                        if (slotNumber == "1") tempLoadList[0] = Resources.MenuGameSaveSlotsLoadFromSlot1 + " (" + now + ")";
                        else if (slotNumber == "2") tempLoadList[1] = Resources.MenuGameSaveSlotsLoadFromSlot2 + " (" + now + ")";
                        else if (slotNumber == "3") tempLoadList[2] = Resources.MenuGameSaveSlotsLoadFromSlot3 + " (" + now + ")";
                        else if (slotNumber == "4") tempLoadList[3] = Resources.MenuGameSaveSlotsLoadFromSlot4 + " (" + now + ")";
                        else if (slotNumber == "5") tempLoadList[4] = Resources.MenuGameSaveSlotsLoadFromSlot5 + " (" + now + ")";
                        MenuSaveSlotsLoadText = tempLoadList;
                    }
                });
            }
        }
        private async Task MenuLoadFromSlotAction(object o)
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                    currentlyMarkedCoords = "";
                    BackgroundReset();
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
                        generatorCoords = saveSlot.GeneratorNumberList;
                        ValidateAll();
                    }
                });
            }
        }
        private async Task MenuDeleteAllSlotsAction()
        {
            if (!doBlockInput)
            {
                await Task.Run(() =>
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
                });
            }
        }
        private void MenuPrintAction(object o)
        {
            if (!doBlockInput)
            {
                //currentlyMarkedCoords = "";
                BackgroundReset();
                LabelSelectNumberOrMarker = "";

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
                ValidateAll();
            }
        }
        private void MenuQuitAction()
        {
            Application.Current.Shutdown();
        }
        private async Task ButtonDifficultyEasyAction()
        {
            if (! doBlockInput)
            {
                HideOverlays();
                await NewGame("Easy");
            }
        }
        private async Task ButtonDifficultyMediumAction()
        {
            if (! doBlockInput)
            {
                HideOverlays();
                await NewGame("Medium");
            }
        }
        private async Task ButtonDifficultyHardAction()
        {
            if (! doBlockInput)
            {
                HideOverlays();
                await NewGame("Hard");
            }
        }
        private async Task ButtonSquareDownAction(object o)
        {
            var e = (MouseButtonEventArgs)((CompositeCommandParameter)o).EventArgs;
            if (! doBlockInput)
            {
                var param = (string)((CompositeCommandParameter)o).Parameter;

                await Task.Run(() =>
                {
                    if (e.ChangedButton == MouseButton.Left)
                    {
                        if (SelectDifficultyVisibility == "Visible")
                        {
                            SelectDifficultyVisibility = "Hidden";
                            return;
                        }
                        if (!generatorCoords.Contains(param))
                        {
                            Coords coords = new Coords(int.Parse(param[0].ToString()), int.Parse(param[1].ToString()));
                            if (param == currentlyMarkedCoords && highlightedCoords.Count != 0)
                            {
                                UnhighlightColRowSquare();
                            }
                            else
                            {
                                HighlightColRowSquare(coords);
                            }
                            ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                            CurrentCoordsBackgroundReset();
                            currentlyMarkedCoords = param;
                            leftOrRightClicked = "Left";
                            LabelSelectNumberOrMarker = Resources.LabelSelectNumber;
                            if (conflictCoords.Contains(param))
                            {
                                buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundConflictsSelected;
                            }
                            else
                            {
                                buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundSelected;
                            }
                            ButtonBackgroundList = buttonBackgroundList;
                        }
                        else
                        {
                            UnhighlightColRowSquare();
                            ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                            CurrentCoordsBackgroundReset();
                            currentlyMarkedCoords = "";
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
                        Coords coords = new Coords(int.Parse(param[0].ToString()), int.Parse(param[1].ToString()));
                        if (!generatorCoords.Contains(param) && numberList[coords.Col][coords.Row] == "")
                        {
                            if (param == currentlyMarkedCoords && highlightedCoords.Count != 0)
                            {
                                UnhighlightColRowSquare();
                            }
                            else
                            {
                                HighlightColRowSquare(coords);
                            }
                            ButtonSelectNumberOrMarker = Colors.ButtonSelectMarker;
                            CurrentCoordsBackgroundReset();
                            currentlyMarkedCoords = param;
                            leftOrRightClicked = "Right";
                            LabelSelectNumberOrMarker = Resources.LabelSelectMarker;
                            buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundSelected;
                            ButtonBackgroundList = buttonBackgroundList;
                        }
                        else if (numberList[coords.Col][coords.Row] != "")
                        {
                            if (highlightedCoords.Count != 0)
                            {
                                UnhighlightColRowSquare();
                            }
                            ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                            CurrentCoordsBackgroundReset();
                            currentlyMarkedCoords = "";
                            leftOrRightClicked = "";
                            buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundMouseOver;
                            ButtonBackgroundList = buttonBackgroundList;
                        }
                        else
                        {
                            UnhighlightColRowSquare();
                            ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                            CurrentCoordsBackgroundReset();
                            currentlyMarkedCoords = "";
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
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                });
            }

            e.Handled = true;
        }
        private async Task ButtonSquareMouseEnterAction(object o)
        {
            var e = (MouseEventArgs)((CompositeCommandParameter)o).EventArgs;
            if (!doBlockInput)
            {
                await Task.Run(() =>
                {
                    var param = (string)((CompositeCommandParameter)o).Parameter;
                    Coords coords = new Coords(int.Parse(param[0].ToString()), int.Parse(param[1].ToString()));
                    buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundMouseOver;
                    ButtonBackgroundList = buttonBackgroundList;
                });
            }

            e.Handled = true;
        }
        private async Task ButtonSquareMouseLeaveAction(object o)
        {
            var e = (MouseEventArgs)((CompositeCommandParameter)o).EventArgs;
            if (!doBlockInput)
            {
                await Task.Run(() =>
                {
                    var param = (string)((CompositeCommandParameter)o).Parameter;
                    Coords coords = new Coords(int.Parse(param[0].ToString()), int.Parse(param[1].ToString()));
                    if (conflictCoords.Contains(param))
                    {
                        if (currentlyMarkedCoords == param)
                        {
                            buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundConflictsSelected;
                        }
                        else
                        {
                            buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundConflicts;
                        }
                    }
                    else if (currentlyMarkedCoords == param)
                    {
                        buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundSelected;
                    }
                    else
                    {
                        if (highlightedCoords.Contains(param))
                        {
                            buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundHighlighted;
                        }
                        else
                        {
                            buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundDefault;
                        }
                    }
                    ButtonBackgroundList = buttonBackgroundList;
                });
            }

            e.Handled = true;
        }
        private async Task ButtonSelectNumberOrMarkerAction(object o)
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                    var tag = (string)o;
                    string param = currentlyMarkedCoords + tag;
                    if (leftOrRightClicked == "Left")
                    {
                        ChangeNumber(param);
                    }
                    else if (leftOrRightClicked == "Right")
                    {
                        ChangeMarker(param);
                    }
                });
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
        private void ChangeNumber(string button)
        {
            InititalizeLists();

            if (generatorCoords == null)
            {
                generatorCoords = new List<string>();
            }

            string coords = button.Substring(0, 2);
            if (! generatorCoords.Contains(coords))
            {
                int col = int.Parse(button[0].ToString());
                int row = int.Parse(button[1].ToString());
                string number = button[2].ToString();
                NumberListModel tempNumberList;
                tempNumberList = numberList;

                if (number == "X")
                {
                    tempNumberList[col][row] = number;
                    tempNumberList[col][row] = "";
                    NumberList = tempNumberList;
                    SelectNumberOrMarkerVisibility = "Visible";
                    ValidationVisibility = "Collapsed";
                    ValidateAll();
                    ButtonBackgroundList = buttonBackgroundList;
                    return;
                }
                else
                {
                    tempNumberList[col][row] = number;
                    NumberList = tempNumberList;
                    CheckIsFull();
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
                                MarkerListModel tempMarkerList;
                                tempMarkerList = markerList;
                                tempMarkerList[col][row][i][j] = "";
                                MarkerList = tempMarkerList;
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
                                    MarkerListModel tempMarkerList;
                                    tempMarkerList = markerList;
                                    if (tempMarkerList[col][k][i][j] == number)
                                    {
                                        tempMarkerList[col][k][i][j] = "";
                                        MarkerList = tempMarkerList;
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
                                    MarkerListModel tempMarkerList;
                                    tempMarkerList = markerList;
                                    if (tempMarkerList[k][row][i][j] == number)
                                    {
                                        tempMarkerList[k][row][i][j] = "";
                                        MarkerList = tempMarkerList;
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
                                        MarkerListModel tempMarkerList;
                                        tempMarkerList = markerList;
                                        if (tempMarkerList[j][i][k][l] == number)
                                        {
                                            tempMarkerList[j][i][k][l] = "";
                                            MarkerList = tempMarkerList;
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
            InititalizeLists();

            int col = int.Parse(button[0].ToString());
            int row = int.Parse(button[1].ToString());
            string number = button[2].ToString();

            if (numberList[col][row] != "")
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
                            MarkerListModel tempNumberList;
                            tempNumberList = markerList;
                            tempNumberList[col][row][i][j] = "";
                            MarkerList = tempNumberList;
                        }
                    }
                }
                return;
            }
            else
            {
                MarkerListModel tempNumberList;
                tempNumberList = markerList;

                // 00|10|20|30
                // 01|11|21|31
                // 02|12|22|32

                if (number == "1")
                {
                    if (markerList[col][row][0][0] != "") { tempNumberList[col][row][0][0] = ""; }
                    else { tempNumberList[col][row][0][0] = "1"; }
                }
                else if (number == "2")
                {
                    if (markerList[col][row][1][0] != "") { tempNumberList[col][row][1][0] = ""; }
                    else { tempNumberList[col][row][1][0] = "2"; }
                }
                else if (number == "3")
                {
                    if (markerList[col][row][2][0] != "") { tempNumberList[col][row][2][0] = ""; }
                    else { tempNumberList[col][row][2][0] = "3"; }
                }
                else if (number == "4")
                {
                    if (markerList[col][row][3][0] != "") { tempNumberList[col][row][3][0] = ""; }
                    else { tempNumberList[col][row][3][0] = "4"; }
                }
                else if (number == "5")
                {
                    if (markerList[col][row][0][1] != "") { tempNumberList[col][row][0][1] = ""; }
                    else { tempNumberList[col][row][0][1] = "5"; }
                }
                else if (number == "6")
                {
                    if (markerList[col][row][3][1] != "") { tempNumberList[col][row][3][1] = ""; }
                    else { tempNumberList[col][row][3][1] = "6"; }
                }
                else if (number == "7")
                {
                    if (markerList[col][row][0][2] != "") { tempNumberList[col][row][0][2] = ""; }
                    else { tempNumberList[col][row][0][2] = "7"; }
                }
                else if (number == "8")
                {
                    if (markerList[col][row][1][2] != "") { tempNumberList[col][row][1][2] = ""; }
                    else { tempNumberList[col][row][1][2] = "8"; }
                }
                else if (number == "9")
                {
                    if (markerList[col][row][2][2] != "") { tempNumberList[col][row][2][2] = ""; }
                    else { tempNumberList[col][row][2][2] = "9"; }
                }
                MarkerList = tempNumberList;
            }
        }
        private void CurrentCoordsBackgroundReset()
        {
            if (currentlyMarkedCoords != "" && !conflictCoords.Contains(currentlyMarkedCoords))
            {
                Coords coordsOld = new Coords(int.Parse(currentlyMarkedCoords[0].ToString()), int.Parse(currentlyMarkedCoords[1].ToString()));
                if (highlightedCoords.Contains(currentlyMarkedCoords))
                {
                    buttonBackgroundList[coordsOld.Col][coordsOld.Row] = Colors.CellBackgroundHighlighted;
                }
                else
                {
                    buttonBackgroundList[coordsOld.Col][coordsOld.Row] = Colors.CellBackgroundDefault;
                }
                ButtonBackgroundList = buttonBackgroundList;
                LabelSelectNumberOrMarker = "";
            }
            else if (currentlyMarkedCoords != "" && conflictCoords.Contains(currentlyMarkedCoords))
            {
                Coords coordsOld = new Coords(int.Parse(currentlyMarkedCoords[0].ToString()), int.Parse(currentlyMarkedCoords[1].ToString()));
                buttonBackgroundList[coordsOld.Col][coordsOld.Row] = Colors.CellBackgroundConflicts;
            }
        }
        private void BackgroundReset()
        {
            conflictCoords.Clear();
            highlightedCoords.Clear();
            buttonBackgroundList.Clear();
            buttonBackgroundList.InitializeList();
            ButtonBackgroundList = buttonBackgroundList;
        }
        private void HighlightColRowSquare(Coords coords)
        {
            UnhighlightColRowSquare();

            // highlight column:
            for (int i = 0; i < 9; i++)
            {
                string tempCoords = coords.Col.ToString() + i.ToString();
                if (!highlightedCoords.Contains(tempCoords))
                {
                    if (!conflictCoords.Contains(tempCoords))
                    {
                        buttonBackgroundList[coords.Col][i] = Colors.CellBackgroundHighlighted;
                    }
                    highlightedCoords.Add(tempCoords);
                }
            }

            // highlight row:
            for (int i = 0; i < 9; i++)
            {
                string tempCoords = i.ToString() + coords.Row.ToString();
                if (!highlightedCoords.Contains(tempCoords))
                {
                    if (!conflictCoords.Contains(tempCoords))
                    {
                        buttonBackgroundList[i][coords.Row] = Colors.CellBackgroundHighlighted;
                    }
                    highlightedCoords.Add(tempCoords);
                }
            }

            // highlight square:
            int col = (int)(coords.Col / 3) * 3;
            int row = (int)(coords.Row / 3) * 3;

            for (int i = row; i < row + 3; i++)
            {
                for (int j = col; j < col + 3; j++)
                {
                    string tempCoords = j.ToString() + i.ToString();
                    if (!highlightedCoords.Contains(tempCoords))
                    {
                        if (!conflictCoords.Contains(tempCoords))
                        {
                            buttonBackgroundList[j][i] = Colors.CellBackgroundHighlighted;
                        }
                        highlightedCoords.Add(tempCoords);
                    }
                }
            }
            ButtonBackgroundList = buttonBackgroundList;
        }
        private void UnhighlightColRowSquare()
        {
            for (int i = 0; i < highlightedCoords.Count; i++)
            {
                if (!conflictCoords.Contains(highlightedCoords[i]))
                {
                    buttonBackgroundList[int.Parse(highlightedCoords[i][0].ToString())][int.Parse(highlightedCoords[i][1].ToString())] = Colors.CellBackgroundDefault;
                }
                else
                {
                    buttonBackgroundList[int.Parse(highlightedCoords[i][0].ToString())][int.Parse(highlightedCoords[i][1].ToString())] = Colors.CellBackgroundConflicts;
                }
            }
            if (currentlyMarkedCoords != "")
            {
                buttonBackgroundList[int.Parse(currentlyMarkedCoords[0].ToString())][int.Parse(currentlyMarkedCoords[1].ToString())] = Colors.CellBackgroundConflictsSelected;
            }
            ButtonBackgroundList = buttonBackgroundList;
            highlightedCoords.Clear();
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
                NumberList = numberListPreloadedEasy;
                generatorCoords = generatorCoordsPreloadedEasy;
                NumberColorList = numberColorListPreloadedEasy;
            }
            else if (difficulty == "Medium")
            {
                await preloadGameMedium;
                NumberList = numberListPreloadedMedium;
                generatorCoords = generatorCoordsPreloadedMedium;
                NumberColorList = numberColorListPreloadedMedium;
            }
            else if (difficulty == "Hard")
            {
                if (menuSingleSolutionCheck == "True")
                {
                    LabelSingleSolutionWaitVisibility = "Visible";
                }
                await preloadGameHard;
                NumberList = numberListPreloadedHard;
                generatorCoords = generatorCoordsPreloadedHard;
                NumberColorList = numberColorListPreloadedHard;
                LabelSingleSolutionWaitVisibility = "Hidden";
            }

            preloadGameEasy = PreloadGame("Easy");
            preloadGameMedium = PreloadGame("Medium");
            preloadGameHard = PreloadGame("Hard");

            currentlyMarkedCoords = "";

            SelectNumberOrMarkerVisibility = "Visible";
            ValidationVisibility = "Collapsed";

            doBlockInput = false;
        }
        private async Task PreloadGame(string difficulty)
        {
            await Task.Run(() =>
            {
                GeneratorGameLogic generatorGameLogic = null;
                NumberListModel tempNumberList = new NumberListModel();
                NumberColorListModel tempNumberColorList = new NumberColorListModel();
                bool doRun = true;

                while (doRun)
                {
                    tempNumberList.InitializeList();

                    SolverGameLogic solverGameLogic = new SolverGameLogic(tempNumberList);
                    solverGameLogic.FillSudoku();
                    NumberListModel numberListSolved = solverGameLogic.NumberListSolved;

                    generatorGameLogic = new GeneratorGameLogic(difficulty, numberListSolved);
                    generatorGameLogic.GenerateSudoku(menuSingleSolutionCheck);

                    if (generatorGameLogic.Counter == generatorGameLogic.removeNumbers)
                    {
                        doRun = false;
                    }
                }

                List<string>  tempGeneratorCoords = new List<string>();

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
                foreach (string coords in tempGeneratorCoords)
                {
                    int col = int.Parse(coords[0].ToString());
                    int row = int.Parse(coords[1].ToString());

                    tempNumberColorList[col][row] = Colors.CellNumberGenerator;
                }

                tempNumberList = generatorGameLogic.NumberList;

                if (difficulty == "Easy")
                {
                    numberListPreloadedEasy = tempNumberList;
                    numberColorListPreloadedEasy = tempNumberColorList;
                    generatorCoordsPreloadedEasy = tempGeneratorCoords;
                }
                else if (difficulty == "Medium")
                {
                    numberListPreloadedMedium = tempNumberList;
                    numberColorListPreloadedMedium = tempNumberColorList;
                    generatorCoordsPreloadedMedium = tempGeneratorCoords;
                }
                else if (difficulty == "Hard")
                {
                    numberListPreloadedHard = tempNumberList;
                    numberColorListPreloadedHard = tempNumberColorList;
                    generatorCoordsPreloadedHard = tempGeneratorCoords;
                }

                doBlockInput = false;
            });
        }
        private void CheckIsFull()
        {
            if (!SolverGameLogic.IsFull(numberList))
            {
                SelectNumberOrMarkerVisibility = "Visible";
                ValidationVisibility = "Collapsed";
            }
            else
            {
                ValidateAll();
                SelectNumberOrMarkerVisibility = "Collapsed";
                ValidationVisibility = "Visible";
            }
        }
        private void ValidateAll()
        {
            if (numberList == null)
            {
                numberList = new NumberListModel();
                numberList.InitializeList();
            }
            bool isValid = true;

            LabelValidate = Resources.LabelValidateNoConflicts;
            LabelValidateBackground = Colors.LabelValidateHasNoConflicts;

            foreach (string coords in conflictCoords)
            {
                if (currentlyMarkedCoords == coords)
                {
                    buttonBackgroundList[int.Parse(coords[0].ToString())][int.Parse(coords[1].ToString())] = Colors.CellBackgroundSelected;
                }
                else if (highlightedCoords.Contains(coords))
                {
                    buttonBackgroundList[int.Parse(coords[0].ToString())][int.Parse(coords[1].ToString())] = Colors.CellBackgroundHighlighted;
                }
                else
                {
                    buttonBackgroundList[int.Parse(coords[0].ToString())][int.Parse(coords[1].ToString())] = Colors.CellBackgroundDefault;
                }
            }
            ButtonBackgroundList = buttonBackgroundList;
            conflictCoords.Clear();

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    string number = numberList[col][row];
                    if (!ValidatorGameLogic.IsValid(numberList, col, row, number))
                    {
                        isValid = false;
                        LabelValidate = Resources.LabelValidateConflicts;
                        LabelValidateBackground = Colors.LabelValidateHasConflicts;
                        buttonBackgroundList[col][row] = Colors.CellBackgroundConflicts;
                        ButtonBackgroundList = buttonBackgroundList;
                        string stringCoords = col.ToString() + row.ToString();
                        conflictCoords.Add(stringCoords);
                    }
                }
            }
            if (!isValid)
            {
                UnhighlightColRowSquare();
            }
            LabelValidateVisibility = "Visible";
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
