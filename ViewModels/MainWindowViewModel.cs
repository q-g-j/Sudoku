using System;
using System.Printing;
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
using Sudoku.Settings;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

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
            generatorNumbers = new List<string>();
            numbersList = new NumbersListModel();
            markersList = new MarkersListModel();
            numbersColorsList = new NumbersColorsListModel();
            numbersList.InitializeList();
            markersList.InitializeList();
            numbersColorsList.InitializeList();

            // initialize commands:
            MenuNewCommand = new AsyncRelayCommand(MenuNewAction);
            MenuSolveCommand = new AsyncRelayCommand(MenuSolveAction);
            MenuFillAllMarkersCommand = new AsyncRelayCommand(MenuFillAllMarkersAction);
            MenuSettingsSingleSolutionCommand = new AsyncRelayCommand(MenuSettingsSingleSolutionAction);
            MenuSaveToSlotCommand = new AsyncRelayCommand<object>(o => MenuSaveToSlotAction(o));
            MenuLoadFromSlotCommand = new AsyncRelayCommand<object>(o => MenuLoadFromSlotAction(o));
            MenuDeleteAllSlotsCommand = new AsyncRelayCommand(MenuDeleteAllSlotsAction);
            MenuPrintCommand = new RelayCommand<object>(o => MenuPrintAction(o));
            ButtonDifficultyCommand = new AsyncRelayCommand(ButtonDifficultyAction);
            ButtonValidateCommand = new AsyncRelayCommand(ButtonValidateAction);
            ButtonDifficultyEasyCommand = new AsyncRelayCommand(ButtonDifficultyEasyAction);
            ButtonDifficultyMediumCommand = new AsyncRelayCommand(ButtonDifficultyMediumAction);
            ButtonDifficultyHardCommand = new AsyncRelayCommand(ButtonDifficultyHardAction);
            ButtonSelectNumberCommand = new AsyncRelayCommand<object>(o => ButtonSelectNumberAction(o));
            ButtonSelectMarkerCommand = new AsyncRelayCommand<object>(o => ButtonSelectMarkerAction(o));

            ButtonSquareDownCommand = new AsyncRelayCommand<CompositeCommandParameter>(o => ButtonSquareDownAction(o));
            ButtonSquareUpCommand = new AsyncRelayCommand<CompositeCommandParameter>(o => ButtonSquareUpAction(o));

            // initialize properties:
            selectNumberVisibility = "Hidden";
            selectMarkerVisibility = "Hidden";
            labelUniqueWaitVisibility = "Hidden";
            selectDifficultyVisibility = "Visible";
            buttonValidateVisibility = "Collapsed";
            labelValidateVisibility = "Collapsed";
            labelValidateText = "";
            buttonDifficultyWidth = "350";
            currentButtonIndex = "";

            if (!Directory.Exists(folderAppSettings))
            {
                Directory.CreateDirectory(folderAppSettings);
            }

            // load app settings from file:
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
            if (appSettingsStruct.SingleSolution)
            {
                isMenuSettingsSingleSolutionSet = true;
                menuSingleSolutionCheck = "True";
            }
            else
            {
                isMenuSettingsSingleSolutionSet = false;
                menuSingleSolutionCheck = "False";
            }

            // display each existing save slot's date and time:
            menuSaveSlotsLoadText = saveSlotsModel.GetLoadTexts();
            //menuSaveSlotsSaveText = saveSlotsModel.GetSaveTexts();
        }
        #endregion Constructors

        #region Fields
        private readonly AppSettings appSettings;
        private readonly SaveSlotsModel saveSlotsModel;
        private string currentButtonIndex;
        private List<string> generatorNumbers;
        readonly string folderAppSettings;
        private bool doBlockInput;
        private bool isMenuSettingsSingleSolutionSet;
        #endregion Fields

        #region Property Values
        private NumbersListModel numbersList;
        private MarkersListModel markersList;
        private NumbersColorsListModel numbersColorsList;

        private List<string> menuSaveSlotsLoadText;
        private List<string> menuSaveSlotsSaveText;
        private string menuSingleSolutionCheck;

        private string buttonDifficultyText;
        private string labelValidateText;
        private string buttonDifficultyWidth;

        private string selectNumberVisibility;
        private string selectMarkerVisibility;
        private string selectDifficultyVisibility;
        private string buttonValidateVisibility;
        private string labelValidateVisibility;
        private string labelUniqueWaitVisibility;
        #endregion Property Values

        #region Properties
        public IAsyncRelayCommand MenuNewCommand { get; }
        public IAsyncRelayCommand MenuSolveCommand { get; }
        public IAsyncRelayCommand MenuFillAllMarkersCommand { get; }
        public IAsyncRelayCommand MenuSettingsCommand { get; }
        public IAsyncRelayCommand MenuSettingsSingleSolutionCommand { get; }
        public IAsyncRelayCommand MenuSaveToSlotCommand { get; }
        public IAsyncRelayCommand MenuLoadFromSlotCommand { get; }
        public IAsyncRelayCommand MenuDeleteAllSlotsCommand { get; }
        public RelayCommand<object> MenuPrintCommand { get; }
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
        public IAsyncRelayCommand ButtonSquareLeftDownCommand { get; }
        public IAsyncRelayCommand ButtonSquareLeftUpCommand { get; }

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
        public NumbersListModel NumbersList
        {
            get => numbersList;
            set { numbersList = value; OnPropertyChanged(); }
        }
        public MarkersListModel MarkersList
        {
            get => markersList;
            set {  markersList = value; OnPropertyChanged(); }
        }
        public NumbersColorsListModel NumbersColorsList
        {
            get => numbersColorsList;
            set { numbersColorsList = value; OnPropertyChanged(); }
        }
        public string SelectNumberVisibility
        {
            get => selectNumberVisibility;
            set { selectNumberVisibility = value; OnPropertyChanged(); }
        }
        public string SelectMarkerVisibility
        {
            get => selectMarkerVisibility;
            set { selectMarkerVisibility = value; OnPropertyChanged(); }
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
        public string LabelUniqueWaitVisibility
        {
            get => labelUniqueWaitVisibility;
            set { labelUniqueWaitVisibility = value; OnPropertyChanged(); }
        }
        public string ButtonDifficultyText
        {
            get => buttonDifficultyText;
            set { buttonDifficultyText = value; OnPropertyChanged(); }
        }
        public string ButtonDifficultyWidth
        {
            get => buttonDifficultyWidth;
            set { buttonDifficultyWidth = value; OnPropertyChanged(); }
        }
        public string LabelValidateText
        {
            get => labelValidateText;
            set { labelValidateText = value; OnPropertyChanged(); }
        }
        #endregion Properties

        #region Command Actions
        private async Task MenuNewAction()
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                    HideOverlays();
                    HideValidation();
                    numbersList = new NumbersListModel();
                    markersList = new MarkersListModel();
                    numbersColorsList = new NumbersColorsListModel();
                    generatorNumbers = new List<string>();
                    numbersList.InitializeList();
                    markersList.InitializeList();
                    numbersColorsList.InitializeList();
                    NumbersList = numbersList;
                    MarkersList = markersList;
                    NumbersColorsList = numbersColorsList;
                });
            }
        }
        private async Task MenuSolveAction()
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                    if (!SolverGameLogic.IsFull(numbersList))
                    {
                        bool isValid = true;
                        for (int col = 0; col < 9; col++)
                        {
                            if (isValid)
                            {
                                for (int row = 0; row < 9; row++)
                                {
                                    string number = numbersList[col][row];
                                    if (!ValidatorGameLogic.IsValid(numbersList, col, row, number))
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
                            SolverGameLogic solverGameLogic = new SolverGameLogic(numbersList);
                            solverGameLogic.FillSudoku();
                            markersList = new MarkersListModel();
                            numbersColorsList = new NumbersColorsListModel();
                            markersList.InitializeList();
                            numbersColorsList.InitializeList();
                            foreach (string coords in generatorNumbers)
                            {
                                int col = int.Parse(coords[0].ToString());
                                int row = int.Parse(coords[1].ToString());

                                numbersColorsList[col][row] = "Black";
                            }
                            MarkersList = markersList;
                            NumbersColorsList = numbersColorsList;
                            NumbersList = new NumbersListModel(solverGameLogic.NumbersListSolved);
                            ChangeButtonValidateVisibility();
                        }
                    }
                });
            }
        }
        private async Task MenuFillAllMarkersAction()
        {
            if (!doBlockInput)
            {
                await Task.Run(() =>
                {
                    HideOverlays();
                    for (int col = 0; col < 9; col++)
                    {
                        for (int row = 0; row < 9; row++)
                        {
                            // 00|10|20|30
                            // 01|11|21|31
                            // 02|12|22|32

                            if (markersList[col][row][0][0] == "" && numbersList[col][row] == "")
                            {
                                if (ValidatorGameLogic.IsValid(numbersList, col, row, "1"))
                                {
                                    markersList[col][row][0][0] = "1";
                                }
                            }
                            if (markersList[col][row][1][0] == "" && numbersList[col][row] == "")
                            {
                                if (ValidatorGameLogic.IsValid(numbersList, col, row, "2"))
                                {
                                    markersList[col][row][1][0] = "2";
                                }
                            }
                            if (markersList[col][row][2][0] == "" && numbersList[col][row] == "")
                            {
                                if (ValidatorGameLogic.IsValid(numbersList, col, row, "3"))
                                {
                                    markersList[col][row][2][0] = "3";
                                }
                            }
                            if (markersList[col][row][3][0] == "" && numbersList[col][row] == "")
                            {
                                if (ValidatorGameLogic.IsValid(numbersList, col, row, "4"))
                                {
                                    markersList[col][row][3][0] = "4";
                                }
                            }
                            if (markersList[col][row][0][1] == "" && numbersList[col][row] == "")
                            {
                                if (ValidatorGameLogic.IsValid(numbersList, col, row, "5"))
                                {
                                    markersList[col][row][0][1] = "5";
                                }
                            }
                            if (markersList[col][row][3][1] == "" && numbersList[col][row] == "")
                            {
                                if (ValidatorGameLogic.IsValid(numbersList, col, row, "6"))
                                {
                                    markersList[col][row][3][1] = "6";
                                }
                            }
                            if (markersList[col][row][0][2] == "" && numbersList[col][row] == "")
                            {
                                if (ValidatorGameLogic.IsValid(numbersList, col, row, "7"))
                                {
                                    markersList[col][row][0][2] = "7";
                                }
                            }
                            if (markersList[col][row][1][2] == "" && numbersList[col][row] == "")
                            {
                                if (ValidatorGameLogic.IsValid(numbersList, col, row, "8"))
                                {
                                    markersList[col][row][1][2] = "8";
                                }
                            }
                            if (markersList[col][row][2][2] == "" && numbersList[col][row] == "")
                            {
                                if (ValidatorGameLogic.IsValid(numbersList, col, row, "9"))
                                {
                                    markersList[col][row][2][2] = "9";
                                }
                            }
                        }
                        MarkersList = markersList;
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
                    if (isMenuSettingsSingleSolutionSet)
                    {
                        isMenuSettingsSingleSolutionSet = false;
                        appSettings.ChangeSingleSolution(false);
                    }
                    else
                    {
                        isMenuSettingsSingleSolutionSet = true;
                        appSettings.ChangeSingleSolution(true);
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
                    if (numbersList != null && markersList != null && numbersColorsList != null && generatorNumbers != null)
                    {
                        DateTime now = DateTime.Now;
                        string slotNumber = (string)o;
                        saveSlotsModel.SaveAll(numbersList, markersList, numbersColorsList, generatorNumbers, now, slotNumber);
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
                    string slotNumber = (string)o;
                    string filename = Path.Combine(folderAppSettings, "slot" + slotNumber + ".json");
                    if (File.Exists(filename))
                    {
                        HideOverlays();
                        SaveSlotStruct saveSlot = saveSlotsModel.LoadAll(slotNumber);
                        NumbersList = saveSlot.NumbersList;
                        MarkersList = saveSlot.MarkersList;
                        NumbersColorsList = saveSlot.NumbersColorsList;
                        generatorNumbers = saveSlot.GeneratorNumbers;
                        if (SolverGameLogic.IsFull(numbersList))
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
                HideOverlays();
                var sudokuGrid = (Viewbox)o;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog().GetValueOrDefault())
                {
                    System.Threading.Thread.Sleep(500);
                    //store original scale
                    Transform originalScale = sudokuGrid.LayoutTransform;
                    //get selected printer capabilities
                    PrintCapabilities capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);

                    //get scale of the print wrt to screen of WPF visual
                    double scale = capabilities.PageImageableArea.ExtentWidth / sudokuGrid.ActualWidth;

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

                    //Console.WriteLine(capabilities.PageImageableArea.ExtentWidth.ToString());

                    //apply the original transform.
                    sudokuGrid.LayoutTransform = originalScale;
                }
            }
        }
        private async Task ButtonDifficultyAction()
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
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
        }
        private async Task ButtonValidateAction()
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                    ValidateAll();
                });
            }
        }
        private async Task ButtonDifficultyEasyAction()
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                    HideOverlays();
                    NewGame("Easy");
                });
            }
        }
        private async Task ButtonDifficultyMediumAction()
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                    HideOverlays();
                    NewGame("Medium");
                });
            }
        }
        private async Task ButtonDifficultyHardAction()
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                    HideOverlays();
                    NewGame("Hard");
                });
            }
        }
        private async Task ButtonSquareDownAction(CompositeCommandParameter o)
        {
            var e = (MouseButtonEventArgs)o.EventArgs;
            if (! doBlockInput)
            {
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

                    if (!SolverGameLogic.IsFull(numbersList))
                    {
                        HideValidation();
                    }
                    else
                    {
                        ShowValidation();
                    }
                });
            }

            e.Handled = true;
        }
        private async Task ButtonSquareUpAction(CompositeCommandParameter o)
        {
            var e = (MouseButtonEventArgs)o.EventArgs;
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                });
            }

            e.Handled = true;
        }
        private async Task ButtonSelectNumberAction(object o)
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                    SelectNumberVisibility = "Hidden";
                    var tag = (string)o;
                    string param = currentButtonIndex + tag;
                    ChangeNumber(param);
                });
            }
        }
        private async Task ButtonSelectMarkerAction(object o)
        {
            if (! doBlockInput)
            {
                await Task.Run(() =>
                {
                    SelectMarkerVisibility = "Hidden";
                    var tag = (string)o;
                    string param = currentButtonIndex + tag;
                    ChangeMarker(param);
                });
            }
        }
        #endregion Command Actions

        #region Methods
        private void Inititalizes()
        {
            if (numbersList == null)
            {
                numbersList = new NumbersListModel();
                numbersList.InitializeList();
            }
            if (markersList == null)
            {
                markersList = new MarkersListModel();
                markersList.InitializeList();
            }
            if (numbersColorsList == null)
            {
                numbersColorsList = new NumbersColorsListModel();
                numbersColorsList.InitializeList();
            }
        }
        private void ChangeButtonValidateVisibility()
        {
            if (SolverGameLogic.IsFull(numbersList))
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
            Inititalizes();

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
                temp_numbersList = numbersList;

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
                        for (int i = 0; i < 4; i++)
                        {
                            if (i == 1 && j == 1)
                            {
                                continue;
                            }
                            else
                            {
                                MarkersListModel temp_markersList;
                                temp_markersList = markersList;
                                temp_markersList[col][row][i][j] = "";
                                MarkersList = temp_markersList;
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
                                    MarkersListModel temp_markersList;
                                    temp_markersList = markersList;
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
                            for (int i = 0; i < 4; i++)
                            {
                                if (i == 1 && j == 1)
                                {
                                    continue;
                                }
                                else
                                {
                                    MarkersListModel temp_markersList;
                                    temp_markersList = markersList;
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
                                        MarkersListModel temp_markersList;
                                        temp_markersList = markersList;
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
            Inititalizes();

            int col = int.Parse(button[0].ToString());
            int row = int.Parse(button[1].ToString());
            string number = button[2].ToString();

            if (numbersList[col][row] != "")
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
                            MarkersListModel temp_numbersList;
                            temp_numbersList = markersList;
                            temp_numbersList[col][row][i][j] = "";
                            MarkersList = temp_numbersList;
                        }
                    }
                }
                return;
            }
            else
            {
                MarkersListModel temp_numbersList;
                temp_numbersList = markersList;

                // 00|10|20|30
                // 01|11|21|31
                // 02|12|22|32

                if (number == "1")
                {
                    if (markersList[col][row][0][0] != "") { temp_numbersList[col][row][0][0] = ""; }
                    else { temp_numbersList[col][row][0][0] = "1"; }
                }
                else if (number == "2")
                {
                    if (markersList[col][row][1][0] != "") { temp_numbersList[col][row][1][0] = ""; }
                    else { temp_numbersList[col][row][1][0] = "2"; }
                }
                else if (number == "3")
                {
                    if (markersList[col][row][2][0] != "") { temp_numbersList[col][row][2][0] = ""; }
                    else { temp_numbersList[col][row][2][0] = "3"; }
                }
                else if (number == "4")
                {
                    if (markersList[col][row][3][0] != "") { temp_numbersList[col][row][3][0] = ""; }
                    else { temp_numbersList[col][row][3][0] = "4"; }
                }
                else if (number == "5")
                {
                    if (markersList[col][row][0][1] != "") { temp_numbersList[col][row][0][1] = ""; }
                    else { temp_numbersList[col][row][0][1] = "5"; }
                }
                else if (number == "6")
                {
                    if (markersList[col][row][3][1] != "") { temp_numbersList[col][row][3][1] = ""; }
                    else { temp_numbersList[col][row][3][1] = "6"; }
                }
                else if (number == "7")
                {
                    if (markersList[col][row][0][2] != "") { temp_numbersList[col][row][0][2] = ""; }
                    else { temp_numbersList[col][row][0][2] = "7"; }
                }
                else if (number == "8")
                {
                    if (markersList[col][row][1][2] != "") { temp_numbersList[col][row][1][2] = ""; }
                    else { temp_numbersList[col][row][1][2] = "8"; }
                }
                else if (number == "9")
                {
                    if (markersList[col][row][2][2] != "") { temp_numbersList[col][row][2][2] = ""; }
                    else { temp_numbersList[col][row][2][2] = "9"; }
                }
                MarkersList = temp_numbersList;
            }
        }
        private void NewGame(string difficulty)
        {
            doBlockInput = true;

            if (difficulty == "Hard" && isMenuSettingsSingleSolutionSet)
            {
                LabelUniqueWaitVisibility = "Visible";
            }

            GeneratorGameLogic generatorGameLogic = null;

            bool doRun = true;

            while (doRun)
            {
                numbersList = new NumbersListModel();
                numbersList.InitializeList();

                SolverGameLogic solverGameLogic = new SolverGameLogic(numbersList);
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
                    generatorGameLogic.RemoveNumbers = 55; // 57
                }

                generatorGameLogic.NumbersList = numbersListSolved;

                //// DEBUG:
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();

                if (isMenuSettingsSingleSolutionSet)
                {
                    generatorGameLogic.GenerateUniqueSudoku();
                }
                else
                {
                    generatorGameLogic.GenerateSudoku();
                }


                //// DEBUG:
                //stopwatch.Stop();
                //Console.WriteLine("Elapsed Time is {0} ms" + " " + stopwatch.ElapsedMilliseconds + ", " + generatorGameLogic.Tries);
                //Console.WriteLine(generatorGameLogic.Counter.ToString());

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

            numbersColorsList = new NumbersColorsListModel();
            numbersColorsList.InitializeList();
            foreach (string coords in generatorNumbers)
            {
                int col = int.Parse(coords[0].ToString());
                int row = int.Parse(coords[1].ToString());

                numbersColorsList[col][row] = "Black";
            }
            NumbersColorsList = numbersColorsList;

            LabelValidateText = "";

            NumbersList = generatorGameLogic.NumbersList;

            markersList = new MarkersListModel();
            markersList.InitializeList();
            MarkersList = markersList;
            HideValidation();

            LabelUniqueWaitVisibility = "Hidden";

            doBlockInput = false;
        }
        private void ValidateAll()
        {
            ButtonValidateVisibility = "Collapsed";
            LabelValidateVisibility = "Visible";

            if (numbersList == null)
            {
                numbersList = new NumbersListModel();
                numbersList.InitializeList();
            }

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    string number = numbersList[col][row];
                    if (! ValidatorGameLogic.IsValid(numbersList, col, row, number))
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
