using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku.Helpers;
using Sudoku.GameLogic;
using Sudoku.Properties;
using Sudoku.Settings;
using Sudoku.ViewModels;
using System.IO;

namespace Sudoku.Models
{
    internal class MainWindowModel
    {
        #region Constructors
        internal MainWindowModel(MainWindowViewModel mainWindowViewModel)
        {
            // initialize fields:
            _mainWindowViewModel = mainWindowViewModel;
            folderAppSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SudokuGame");
            appSettings = new AppSettings(folderAppSettings);
            doBlockInput = false;
            doStopTrophy = false;
            hasLoadedFromSlot = false;
            currentDifficulty = "";
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
            numberListEasySolved = new NumberListModel();
            numberListMediumSolved = new NumberListModel();
            numberListHardSolved = new NumberListModel();

            // load app settings from file:
            if (!Directory.Exists(folderAppSettings))
            {
                Directory.CreateDirectory(folderAppSettings);
            }
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
            if (appSettingsStruct.SingleSolution)
            {
                _mainWindowViewModel.MenuSingleSolutionCheck = "True";
            }
            else
            {
                _mainWindowViewModel.MenuSingleSolutionCheck = "False";
            }
            // display each existing save slot's date and time:
            _mainWindowViewModel.MenuSaveSlotsLoadText = saveSlotsModel.GetLoadTexts();

            // preload games in three difficulties:
            preloadGameEasy = PreloadGame("Easy");
            preloadGameMedium = PreloadGame("Medium");
            preloadGameHard = PreloadGame("Hard");
        }
        #endregion Constructors

        #region Fields
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly AppSettings appSettings;
        private readonly SaveSlotsModel saveSlotsModel;
        private string currentlySelectedCoords;
        private readonly List<string> highlightedCoordsList;
        private readonly List<string> conflictCoordsList;
        private List<string> generatorCoordsList;
        private readonly string folderAppSettings;
        private bool doStopTrophy;
        private bool hasLoadedFromSlot;
        private string leftOrRightClicked;
        private NumberListModel numberList;
        private MarkerListModel markerList;
        private NumberColorListModel numberColorList;
        private ButtonBackgroundListModel buttonBackgroundList;
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
        #endregion Fields

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
            if (_mainWindowViewModel.NumberColorList == null)
            {
                _mainWindowViewModel.NumberColorList = new NumberColorListModel();
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
                    _mainWindowViewModel.NumberList = numberList;
                    _mainWindowViewModel.SelectNumberOrMarkerVisibility = "Visible";
                    _mainWindowViewModel.ValidationVisibility = "Collapsed";
                    if (_mainWindowViewModel.TrophyVisibility == "Visible")
                    {
                        doStopTrophy = true;
                        System.Threading.Thread.Sleep(100);
                        _mainWindowViewModel.TrophyVisibility = "Collapsed";
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
                    _mainWindowViewModel.NumberList = numberList;
                    _mainWindowViewModel.SelectNumberOrMarkerVisibility = "Visible";
                    _mainWindowViewModel.ValidationVisibility = "Collapsed";
                    if (_mainWindowViewModel.TrophyVisibility == "Visible")
                    {
                        doStopTrophy = true;
                        System.Threading.Thread.Sleep(100);
                        _mainWindowViewModel.TrophyVisibility = "Collapsed";
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
                    _mainWindowViewModel.MarkerList = markerList;
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
                            _mainWindowViewModel.MarkerList = markerList;
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
                _mainWindowViewModel.MarkerList = markerList;
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
                buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundSelected;
            }
            else
            {
                if (highlightedCoordsList.Contains(stringCoords))
                {
                    buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundHighlighted;
                }
                else
                {
                    buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundDefault;
                }
            }
            _mainWindowViewModel.ButtonBackgroundList = buttonBackgroundList;
        }
        private void RestoreOldCoordsBackground()
        {
            if (currentlySelectedCoords != "" && !conflictCoordsList.Contains(currentlySelectedCoords))
            {
                Coords coordsOld = Coords.StringToCoords(currentlySelectedCoords);
                if (highlightedCoordsList.Contains(currentlySelectedCoords))
                {
                    buttonBackgroundList[coordsOld.Col][coordsOld.Row] = Colors.CellBackgroundHighlighted;
                }
                else
                {
                    buttonBackgroundList[coordsOld.Col][coordsOld.Row] = Colors.CellBackgroundDefault;
                }
                _mainWindowViewModel.LabelSelectNumberOrMarker = "";
            }
            else if (currentlySelectedCoords != "" && conflictCoordsList.Contains(currentlySelectedCoords))
            {
                Coords coordsOld = Coords.StringToCoords(currentlySelectedCoords);
                buttonBackgroundList[coordsOld.Col][coordsOld.Row] = Colors.CellBackgroundConflicts;
            }
            _mainWindowViewModel.ButtonBackgroundList = buttonBackgroundList;
        }
        private void ResetBackground()
        {
            highlightedCoordsList.Clear();
            conflictCoordsList.Clear();
            _mainWindowViewModel.ButtonBackgroundList = new ButtonBackgroundListModel();
            _mainWindowViewModel.ButtonBackgroundList.InitializeList();
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
                        buttonBackgroundList[coords.Col][i] = Colors.CellBackgroundHighlighted;
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
                        buttonBackgroundList[i][coords.Row] = Colors.CellBackgroundHighlighted;
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
                            buttonBackgroundList[j][i] = Colors.CellBackgroundHighlighted;
                        }
                        highlightedCoordsList.Add(stringCoords);
                    }
                }
            }

            buttonBackgroundList[coords.Col][coords.Row] = Colors.CellBackgroundSelected;
            _mainWindowViewModel.ButtonBackgroundList = buttonBackgroundList;
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
            _mainWindowViewModel.ButtonBackgroundList = buttonBackgroundList;
            highlightedCoordsList.Clear();
        }
        private async Task NewGame(string difficulty)
        {
            doBlockInput = true;

            markerList = new MarkerListModel();
            markerList.InitializeList();
            _mainWindowViewModel.MarkerList = markerList;

            if (difficulty == "Easy")
            {
                await preloadGameEasy;
                _mainWindowViewModel.NumberList = numberListPreloadedEasy;
                generatorCoordsList = generatorCoordsPreloadedEasy;
                _mainWindowViewModel.NumberColorList = numberColorListPreloadedEasy;
                currentDifficulty = "Easy";
            }
            else if (difficulty == "Medium")
            {
                await preloadGameMedium;
                _mainWindowViewModel.NumberList = numberListPreloadedMedium;
                generatorCoordsList = generatorCoordsPreloadedMedium;
                _mainWindowViewModel.NumberColorList = numberColorListPreloadedMedium;
                currentDifficulty = "Medium";
            }
            else if (difficulty == "Hard")
            {
                if (_mainWindowViewModel.MenuSingleSolutionCheck == "True")
                {
                    _mainWindowViewModel.LabelSingleSolutionWaitVisibility = "Visible";
                }
                await preloadGameHard;
                _mainWindowViewModel.NumberList = numberListPreloadedHard;
                generatorCoordsList = generatorCoordsPreloadedHard;
                _mainWindowViewModel.NumberColorList = numberColorListPreloadedHard;
                _mainWindowViewModel.LabelSingleSolutionWaitVisibility = "Hidden";
                currentDifficulty = "Hard";
            }

            preloadGameEasy = PreloadGame("Easy");
            preloadGameMedium = PreloadGame("Medium");
            preloadGameHard = PreloadGame("Hard");

            currentlySelectedCoords = "";

            _mainWindowViewModel.SelectNumberOrMarkerVisibility = "Visible";
            _mainWindowViewModel.ValidationVisibility = "Collapsed";

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

                    generatorGameLogic.GenerateSudoku(_mainWindowViewModel.MenuSingleSolutionCheck);

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
                _mainWindowViewModel.SelectNumberOrMarkerVisibility = "Visible";
                _mainWindowViewModel.ValidationVisibility = "Collapsed";
            }
            else
            {
                ValidateAll(doShowTrophy);
                _mainWindowViewModel.SelectNumberOrMarkerVisibility = "Collapsed";
                _mainWindowViewModel.ValidationVisibility = "Visible";
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

            _mainWindowViewModel.LabelValidate  = Resources.LabelValidateNoConflicts;
            _mainWindowViewModel.LabelValidateBackground = Colors.LabelValidateHasNoConflicts;

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    string number = numberList[col][row];
                    if (numberList[col][row] != "" && !ValidatorGameLogic.IsValid(numberList, col, row, number))
                    {
                        isValid = false;
                        _mainWindowViewModel.LabelValidate  = Resources.LabelValidateConflicts;
                        _mainWindowViewModel.LabelValidateBackground = Colors.LabelValidateHasConflicts;
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

            _mainWindowViewModel.ButtonBackgroundList = buttonBackgroundList;

            _mainWindowViewModel.LabelValidateVisibility = "Visible";

            if (isValid && doShowTrophy)
            {
                doStopTrophy = true;
                System.Threading.Thread.Sleep(100);
                doStopTrophy = false;
                HideOverlays();

                _mainWindowViewModel.TrophyWidth = "0";
                _mainWindowViewModel.TrophyVisibility = "Visible";
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
                    _mainWindowViewModel.TrophyWidth = i.ToString();
                    pt.Start();
                    while (pt.Duration < 0.01)
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
                _mainWindowViewModel.TrophyVisibility = "Collapsed";
            });
        }
        private void HideOverlays()
        {
            _mainWindowViewModel.SelectDifficultyVisibility = "Hidden";
        }
        #endregion Methods
    }
}
