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
            currentDifficulty = "";

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
            numberListEasySolved = new NumberListModel();
            numberListMediumSolved = new NumberListModel();
            numberListHardSolved = new NumberListModel();

            // initialize property values:
            labelSelectNumberOrMarker = "";
            buttonSelectNumberOrMarker = Colors.ButtonSelectNumber;
            labelSingleSolutionWaitVisibility = "Hidden";
            validationVisibility = "Collapsed";
            selectDifficultyVisibility = "Visible";
            buttonValidateVisibility = "Visible";
            labelValidateVisibility = "Collapsed";
            trophyVisibility = "Collapsed";
            currentlyMarkedCoords = "";
            trophyWidth = "0";
            doStopTrophy = false;

            preloadGameEasy = PreloadGame("Easy");
            preloadGameMedium = PreloadGame("Medium");
            preloadGameHard = PreloadGame("Hard");

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
        }
        #endregion Constructors

        #region Fields
        private readonly AppSettings appSettings;
        private readonly SaveSlotsModel saveSlotsModel;
        private string currentlyMarkedCoords;
        private readonly List<string> highlightedCoordsList;
        private readonly List<string> conflictCoordsList;
        private List<string> generatorCoordsList;
        private readonly string folderAppSettings;
        private bool doBlockInput;
        private string leftOrRightClicked;
        private NumberListModel numberListPreloadedEasy;
        private NumberListModel numberListPreloadedMedium;
        private NumberListModel numberListPreloadedHard;
        private NumberListModel numberListEasySolved;
        private NumberListModel numberListMediumSolved;
        private NumberListModel numberListHardSolved;
        private NumberColorListModel numberColorListPreloadedEasy;
        private NumberColorListModel numberColorListPreloadedMedium;
        private NumberColorListModel numberColorListPreloadedHard;
        private List<string> generatorCoordsPreloadedEasy;
        private List<string> generatorCoordsPreloadedMedium;
        private List<string> generatorCoordsPreloadedHard;
        private Task preloadGameEasy;
        private Task preloadGameMedium;
        private Task preloadGameHard;
        private string currentDifficulty;
        bool doStopTrophy;
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
                currentlyMarkedCoords = "";
                BackgroundReset();
                LabelSelectNumberOrMarker = "";
                ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                SelectDifficultyVisibility = "Visible";
                TrophyVisibility = "Collapsed";
            }
        }
        private void MenuEmptySudokuAction()
        {
            if (! doBlockInput)
            {
                currentDifficulty = "";
                currentlyMarkedCoords = "";
                BackgroundReset();
                HideOverlays();
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
                numberListEasySolved.Clear();
                numberListMediumSolved.Clear();
                numberListHardSolved.Clear();
                NumberList = numberList;
                MarkerList = markerList;
                NumberColorList = numberColorList;
            }
        }
        private void MenuSolveAction()
        {
            if (! doBlockInput)
            {
                currentlyMarkedCoords = "";
                LabelSelectNumberOrMarker = "";
                ButtonSelectNumberOrMarker = Colors.ButtonSelectNumber;
                BackgroundReset();
                if (!SolverGameLogic.IsFull(numberList))
                {
                    bool isValid = true;
                    if (currentDifficulty == "")
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
                    }
                    if (isValid)
                    {
                        HideOverlays();
                        SolverGameLogic solverGameLogic = new SolverGameLogic(numberList);
                        if (currentDifficulty == "")
                        {
                            solverGameLogic.FillSudoku();
                        }
                        markerList = new MarkerListModel();
                        numberColorList = new NumberColorListModel();
                        markerList.InitializeList();
                        numberColorList.InitializeList();
                        foreach (string coords in generatorCoordsList)
                        {
                            int col = int.Parse(coords[0].ToString());
                            int row = int.Parse(coords[1].ToString());

                            numberColorList[col][row] = Colors.CellNumberGenerator;
                        }
                        MarkerList = markerList;
                        NumberColorList = numberColorList;
                        if (currentDifficulty == "Easy") NumberList = new NumberListModel(numberListEasySolved);
                        else if (currentDifficulty == "Medium") NumberList = new NumberListModel(numberListMediumSolved);
                        else if (currentDifficulty == "Hard") NumberList = new NumberListModel(numberListHardSolved);
                        else NumberList = new NumberListModel(solverGameLogic.NumberListSolved);
                    }
                }
                CheckIsFull(false);
            }
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
            if (! doBlockInput)
            {
                currentlyMarkedCoords = "";
                BackgroundReset();
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
            if (! doBlockInput)
            {
                TrophyVisibility = "Collapsed";
                currentDifficulty = "";
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
                    generatorCoordsList = saveSlot.GeneratorNumberList;
                    CheckIsFull(false);
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
                currentlyMarkedCoords = "";
                BackgroundReset();
                LabelSelectNumberOrMarker = "";
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
                            Coords coords = new Coords(int.Parse(param[0].ToString()), int.Parse(param[1].ToString()));
                            if (param == currentlyMarkedCoords && highlightedCoordsList.Count != 0)
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
                            if (conflictCoordsList.Contains(param))
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
                        if (!generatorCoordsList.Contains(param) && numberList[coords.Col][coords.Row] == "")
                        {
                            if (param == currentlyMarkedCoords && highlightedCoordsList.Count != 0)
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
                            if (highlightedCoordsList.Count != 0)
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
                Coords coords = new Coords(int.Parse(param[0].ToString()), int.Parse(param[1].ToString()));
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
                Coords coords = new Coords(int.Parse(param[0].ToString()), int.Parse(param[1].ToString()));
                if (conflictCoordsList.Contains(param))
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
                    if (highlightedCoordsList.Contains(param))
                    {
                        buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundHighlighted;
                    }
                    else
                    {
                        buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundDefault;
                    }
                }
                ButtonBackgroundList = buttonBackgroundList;
            }

            e.Handled = true;
        }
        private void KeyboardAction(object o)
        {
            if (!doBlockInput && currentlyMarkedCoords != "")
            {
                string key = (string)o;
                string param = currentlyMarkedCoords + key;
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
            if (! doBlockInput && currentlyMarkedCoords != "")
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

            string coords = param.Substring(0, 2);
            if (! generatorCoordsList.Contains(coords))
            {
                int col = int.Parse(param[0].ToString());
                int row = int.Parse(param[1].ToString());
                string number = param[2].ToString();

                if (number == "X")
                {
                    numberList[col][row] = number;
                    numberList[col][row] = "";
                    NumberList = numberList;
                    SelectNumberOrMarkerVisibility = "Visible";
                    ValidationVisibility = "Collapsed";
                    TrophyVisibility = "Collapsed";
                    ButtonBackgroundList = buttonBackgroundList;
                    ValidateAll(false);
                    return;
                }
                else
                {
                    numberList[col][row] = number;
                    NumberList = numberList;
                    CheckIsFull(true);
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
                                markerList[col][row][i][j] = "";
                                MarkerList = markerList;
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
                                    if (markerList[col][k][i][j] == number)
                                    {
                                        markerList[col][k][i][j] = "";
                                        MarkerList = markerList;
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
                                    if (markerList[k][row][i][j] == number)
                                    {
                                        markerList[k][row][i][j] = "";
                                        MarkerList = markerList;
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
                                        if (markerList[j][i][k][l] == number)
                                        {
                                            markerList[j][i][k][l] = "";
                                            MarkerList = markerList;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void ChangeMarker(string param)
        {
            InititalizeLists();

            int col = int.Parse(param[0].ToString());
            int row = int.Parse(param[1].ToString());
            string number = param[2].ToString();

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
                            markerList[col][row][i][j] = "";
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
                    if (markerList[col][row][0][0] != "") { markerList[col][row][0][0] = ""; }
                    else { markerList[col][row][0][0] = "1"; }
                }
                else if (number == "2")
                {
                    if (markerList[col][row][1][0] != "") { markerList[col][row][1][0] = ""; }
                    else { markerList[col][row][1][0] = "2"; }
                }
                else if (number == "3")
                {
                    if (markerList[col][row][2][0] != "") { markerList[col][row][2][0] = ""; }
                    else { markerList[col][row][2][0] = "3"; }
                }
                else if (number == "4")
                {
                    if (markerList[col][row][3][0] != "") { markerList[col][row][3][0] = ""; }
                    else { markerList[col][row][3][0] = "4"; }
                }
                else if (number == "5")
                {
                    if (markerList[col][row][0][1] != "") { markerList[col][row][0][1] = ""; }
                    else { markerList[col][row][0][1] = "5"; }
                }
                else if (number == "6")
                {
                    if (markerList[col][row][3][1] != "") { markerList[col][row][3][1] = ""; }
                    else { markerList[col][row][3][1] = "6"; }
                }
                else if (number == "7")
                {
                    if (markerList[col][row][0][2] != "") { markerList[col][row][0][2] = ""; }
                    else { markerList[col][row][0][2] = "7"; }
                }
                else if (number == "8")
                {
                    if (markerList[col][row][1][2] != "") { markerList[col][row][1][2] = ""; }
                    else { markerList[col][row][1][2] = "8"; }
                }
                else if (number == "9")
                {
                    if (markerList[col][row][2][2] != "") { markerList[col][row][2][2] = ""; }
                    else { markerList[col][row][2][2] = "9"; }
                }
                MarkerList = markerList;
            }
        }
        private void CurrentCoordsBackgroundReset()
        {
            if (currentlyMarkedCoords != "" && !conflictCoordsList.Contains(currentlyMarkedCoords))
            {
                Coords coordsOld = new Coords(int.Parse(currentlyMarkedCoords[0].ToString()), int.Parse(currentlyMarkedCoords[1].ToString()));
                if (highlightedCoordsList.Contains(currentlyMarkedCoords))
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
            else if (currentlyMarkedCoords != "" && conflictCoordsList.Contains(currentlyMarkedCoords))
            {
                Coords coordsOld = new Coords(int.Parse(currentlyMarkedCoords[0].ToString()), int.Parse(currentlyMarkedCoords[1].ToString()));
                buttonBackgroundList[coordsOld.Col][coordsOld.Row] = Colors.CellBackgroundConflicts;
            }
        }
        private void BackgroundReset()
        {
            conflictCoordsList.Clear();
            highlightedCoordsList.Clear();
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
                if (!highlightedCoordsList.Contains(tempCoords))
                {
                    if (!conflictCoordsList.Contains(tempCoords))
                    {
                        buttonBackgroundList[coords.Col][i] = Colors.CellBackgroundHighlighted;
                    }
                    highlightedCoordsList.Add(tempCoords);
                }
            }

            // highlight row:
            for (int i = 0; i < 9; i++)
            {
                string tempCoords = i.ToString() + coords.Row.ToString();
                if (!highlightedCoordsList.Contains(tempCoords))
                {
                    if (!conflictCoordsList.Contains(tempCoords))
                    {
                        buttonBackgroundList[i][coords.Row] = Colors.CellBackgroundHighlighted;
                    }
                    highlightedCoordsList.Add(tempCoords);
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
                    if (!highlightedCoordsList.Contains(tempCoords))
                    {
                        if (!conflictCoordsList.Contains(tempCoords))
                        {
                            buttonBackgroundList[j][i] = Colors.CellBackgroundHighlighted;
                        }
                        highlightedCoordsList.Add(tempCoords);
                    }
                }
            }
            ButtonBackgroundList = buttonBackgroundList;
        }
        private void UnhighlightColRowSquare()
        {
            for (int i = 0; i < highlightedCoordsList.Count; i++)
            {
                if (!conflictCoordsList.Contains(highlightedCoordsList[i]))
                {
                    buttonBackgroundList[int.Parse(highlightedCoordsList[i][0].ToString())][int.Parse(highlightedCoordsList[i][1].ToString())] = Colors.CellBackgroundDefault;
                }
                else
                {
                    buttonBackgroundList[int.Parse(highlightedCoordsList[i][0].ToString())][int.Parse(highlightedCoordsList[i][1].ToString())] = Colors.CellBackgroundConflicts;
                }
            }
            if (currentlyMarkedCoords != "")
            {
                buttonBackgroundList[int.Parse(currentlyMarkedCoords[0].ToString())][int.Parse(currentlyMarkedCoords[1].ToString())] = Colors.CellBackgroundConflictsSelected;
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
                NumberList = numberListPreloadedEasy;
                generatorCoordsList = generatorCoordsPreloadedEasy;
                NumberColorList = numberColorListPreloadedEasy;
                currentDifficulty = "Easy";
            }
            else if (difficulty == "Medium")
            {
                await preloadGameMedium;
                NumberList = numberListPreloadedMedium;
                generatorCoordsList = generatorCoordsPreloadedMedium;
                NumberColorList = numberColorListPreloadedMedium;
                currentDifficulty = "Medium";
            }
            else if (difficulty == "Hard")
            {
                if (menuSingleSolutionCheck == "True")
                {
                    LabelSingleSolutionWaitVisibility = "Visible";
                }
                await preloadGameHard;
                NumberList = numberListPreloadedHard;
                generatorCoordsList = generatorCoordsPreloadedHard;
                NumberColorList = numberColorListPreloadedHard;
                LabelSingleSolutionWaitVisibility = "Hidden";
                currentDifficulty = "Hard";
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
                NumberColorListModel tempNumberColorList = new NumberColorListModel();
                bool doRun = true;

                while (doRun)
                {
                    SolverGameLogic solverGameLogic = new SolverGameLogic(numberList);
                    solverGameLogic.FillSudoku();
                    if (difficulty == "Easy")
                    {
                        numberListEasySolved.InitializeList();
                        numberListEasySolved = solverGameLogic.NumberListSolved;
                        generatorGameLogic = new GeneratorGameLogic(difficulty, new NumberListModel(numberListEasySolved));
                    }
                    else if (difficulty == "Medium")
                    {
                        numberListMediumSolved.InitializeList();
                        numberListMediumSolved = solverGameLogic.NumberListSolved;
                        generatorGameLogic = new GeneratorGameLogic(difficulty, new NumberListModel(numberListMediumSolved));
                    }
                    else if (difficulty == "Hard")
                    {
                        numberListHardSolved.InitializeList();
                        numberListHardSolved = solverGameLogic.NumberListSolved;
                        generatorGameLogic = new GeneratorGameLogic(difficulty, new NumberListModel(numberListHardSolved));
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
                foreach (string coords in tempGeneratorCoords)
                {
                    int col = int.Parse(coords[0].ToString());
                    int row = int.Parse(coords[1].ToString());

                    tempNumberColorList[col][row] = Colors.CellNumberGenerator;
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

            foreach (string coords in conflictCoordsList)
            {
                if (currentlyMarkedCoords == coords)
                {
                    buttonBackgroundList[int.Parse(coords[0].ToString())][int.Parse(coords[1].ToString())] = Colors.CellBackgroundSelected;
                }
                else if (highlightedCoordsList.Contains(coords))
                {
                    buttonBackgroundList[int.Parse(coords[0].ToString())][int.Parse(coords[1].ToString())] = Colors.CellBackgroundHighlighted;
                }
                else
                {
                    buttonBackgroundList[int.Parse(coords[0].ToString())][int.Parse(coords[1].ToString())] = Colors.CellBackgroundDefault;
                }
            }
            ButtonBackgroundList = buttonBackgroundList;
            conflictCoordsList.Clear();

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
                        conflictCoordsList.Add(stringCoords);
                    }
                }
            }
            LabelValidateVisibility = "Visible";
            if (!isValid)
            {
                UnhighlightColRowSquare();
            }
            else if (doShowTrophy)
            {
                doStopTrophy = true;
                System.Threading.Thread.Sleep(1);
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
                for (int i = 1; i <= 501; i += 3)
                {
                    HiPerfTimer pt = new HiPerfTimer();
                    if (doStopTrophy)
                    {
                        pt = null;
                        break;
                    }
                    TrophyWidth = i.ToString();
                    pt.Start();
                    pt.Stop();
                    while(pt.Duration < 0.0022)
                    {
                        System.Threading.Thread.Sleep(0);
                        pt.Stop();
                    }
                    pt = null;
                }

                for (int i = 1; i <= 1000; i += 1)
                {
                    HiPerfTimer pt = new HiPerfTimer();
                    if (doStopTrophy)
                    {
                        pt = null;
                        break;
                    }
                    pt.Start();
                    pt.Stop();
                    while (pt.Duration < 0.001)
                    {
                        System.Threading.Thread.Sleep(0);
                        pt.Stop();
                    }
                    pt = null;
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
