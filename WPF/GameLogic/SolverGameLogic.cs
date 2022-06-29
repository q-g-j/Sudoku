using System.Collections.Generic;
using System;
using System.Linq;
using Sudoku.Helpers;
using Sudoku.Models;


namespace Sudoku.GameLogic
{
    internal struct SingleMarkerStruct
    {
        public int Col { get; set; }
        public int Row { get; set; }
        public int InnerCol { get; set; }
        public int InnerRow { get; set; }
        public string Number { get; set; }

        public override string ToString() => Col.ToString() + ", " + Row.ToString() + ", " + InnerCol.ToString() + ", " + InnerRow.ToString() + ", " + Number;
    }
    public class SolverGameLogic
    {
        #region Constructors
        public SolverGameLogic(NumberListModel numberList)
        {
            NumberList = new NumberListModel(numberList);
            random = new Random();
            Tries = 0;
        }
        #endregion Constructors

        #region Fields
        private readonly Random random;
        private MarkerListModel MarkerList;
        internal readonly NumberListModel NumberList;
        internal NumberListModel NumberListSolved;
        internal int Tries;
        #endregion Fields

        #region Methods
        public static bool IsFull(NumberListModel numberList)
        {
            bool _isFull = true;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (numberList[col][row] == "")
                    {
                        _isFull = false;
                        goto BreakLoop;
                    }
                }
            }
            BreakLoop:
            return _isFull;
        }

        private void CopySolution(NumberListModel numberList)
        {
            NumberListSolved = new NumberListModel(numberList);
        }

        public void FillSudoku()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (NumberList[col][row] == "")
                    {
                        Tries += 1;
                        int[] shuffledIntList = Enumerable.Range(1, 9).OrderBy(c => random.Next().ToString()).ToArray();
                        foreach (int item in shuffledIntList)
                        {
                            string number = item.ToString();
                            if (ValidatorGameLogic.IsValid(NumberList, col, row, number))
                            {
                                NumberList[col][row] = number;
                                if (IsFull(NumberList))
                                {
                                    CopySolution(NumberList);
                                }
                                else if (Tries <= 700000)
                                {
                                    FillSudoku();
                                    NumberList[col][row] = "";
                                }
                            }
                        }
                        return;
                    }
                }
            }
        }
        internal MarkerListModel FillAllMarkers(NumberListModel numberList)
        {
            MarkerListModel markerList = new MarkerListModel();
            markerList.InitializeList();

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
            }
            return markerList;
        }
        private List<SingleMarkerStruct> GetSingleMarkersInCells()
        {
            List<SingleMarkerStruct> returnList = new List<SingleMarkerStruct>();

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    if (NumberList[col][row] == "")
                    {
                        int count = 0;
                        bool addToList = false;
                        SingleMarkerStruct tempStruct = new SingleMarkerStruct();
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (MarkerList[col][row][i][j] != "")
                                {
                                    count++;
                                    if (count == 1)
                                    {
                                        addToList = true;
                                        tempStruct.Col = col;
                                        tempStruct.Row = row;
                                        tempStruct.InnerCol = i;
                                        tempStruct.InnerRow = j;
                                        tempStruct.Number = MarkerList[col][row][i][j];
                                    }
                                    else if (count > 1)
                                    {
                                        addToList = false;
                                        goto BreakLoop;
                                    }
                                }
                            }
                        }
                        BreakLoop:
                        if (addToList)
                        {
                            returnList.Add(tempStruct);
                        }
                    }
                }
            }

            return returnList;
        }
        private List<SingleMarkerStruct> GetSingleMarkersInColumns()
        {
            List<SingleMarkerStruct> returnList = new List<SingleMarkerStruct>();

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    if (NumberList[col][row] == "")
                    {
                        SingleMarkerStruct tempStruct = new SingleMarkerStruct();
                        List<Coords> markerCoordsList = new List<Coords>();
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (MarkerList[col][row][i][j] != "")
                                {
                                    markerCoordsList.Add(new Coords(i, j));
                                }
                            }
                        }
                        if (markerCoordsList.Count > 1)
                        {
                            foreach (Coords coords in markerCoordsList)
                            {
                                int count = 0;
                                for (int k = 0; k < 9; k++)
                                {
                                    if (k != row && MarkerList[col][k][coords.Col][coords.Row] != "")
                                    {
                                        count++;
                                    }
                                }
                                if (count == 0)
                                {
                                    tempStruct.Col = col;
                                    tempStruct.Row = row;
                                    tempStruct.InnerCol = coords.Col;
                                    tempStruct.InnerRow = coords.Row;
                                    tempStruct.Number = MarkerList[col][row][coords.Col][coords.Row];
                                    returnList.Add(tempStruct);
                                }
                            }
                        }
                    }
                }
            }

            return returnList;
        }
        private List<SingleMarkerStruct> GetSingleMarkersInRows()
        {
            List<SingleMarkerStruct> returnList = new List<SingleMarkerStruct>();

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    if (NumberList[col][row] == "")
                    {
                        SingleMarkerStruct tempStruct = new SingleMarkerStruct();
                        List<Coords> markerCoordsList = new List<Coords>();
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (MarkerList[col][row][i][j] != "")
                                {
                                    markerCoordsList.Add(new Coords(i, j));
                                }
                            }
                        }
                        if (markerCoordsList.Count > 1)
                        {
                            foreach (Coords coords in markerCoordsList)
                            {
                                int count = 0;
                                for (int k = 0; k < 9; k++)
                                {
                                    if (k != col && MarkerList[k][row][coords.Col][coords.Row] != "")
                                    {
                                        count++;
                                    }
                                }
                                if (count == 0)
                                {
                                    tempStruct.Col = col;
                                    tempStruct.Row = row;
                                    tempStruct.InnerCol = coords.Col;
                                    tempStruct.InnerRow = coords.Row;
                                    tempStruct.Number = MarkerList[col][row][coords.Col][coords.Row];
                                    returnList.Add(tempStruct);
                                }
                            }
                        }
                    }
                }
            }

            return returnList;
        }
        private List<SingleMarkerStruct> GetSingleMarkersInSquares()
        {
            List<SingleMarkerStruct> returnList = new List<SingleMarkerStruct>();

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    if (NumberList[col][row] == "")
                    {
                        SingleMarkerStruct tempStruct = new SingleMarkerStruct();
                        List<Coords> markerCoordsList = new List<Coords>();
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (MarkerList[col][row][i][j] != "")
                                {
                                    markerCoordsList.Add(new Coords(i, j));
                                }
                            }
                        }
                        if (markerCoordsList.Count > 1)
                        {
                            foreach (Coords coords in markerCoordsList)
                            {
                                int count = 0;
                                int col2 = (col / 3) * 3;
                                int row2 = (row / 3) * 3;

                                for (int i = col2; i < col2 + 3; i++)
                                {
                                    for (int j = row2; j < row2 + 3; j++)
                                    {
                                        if (MarkerList[i][j][coords.Col][coords.Row] != "")
                                        {
                                            count++;
                                        }
                                    }
                                }
                                if (count == 1)
                                {
                                    tempStruct.Col = col;
                                    tempStruct.Row = row;
                                    tempStruct.InnerCol = coords.Col;
                                    tempStruct.InnerRow = coords.Row;
                                    tempStruct.Number = MarkerList[col][row][coords.Col][coords.Row];
                                    returnList.Add(tempStruct);
                                }
                            }
                        }
                    }
                }
            }

            return returnList;
        }
        private void SolveWithMarkerListPlaceNumbers(List<SingleMarkerStruct> singleMarkerList)
        {
            foreach (SingleMarkerStruct singleMarkerStruct in singleMarkerList)
            {
                NumberList[singleMarkerStruct.Col][singleMarkerStruct.Row] = singleMarkerStruct.Number;
            }
        }
        internal void SolveWithMarkerList()
        {
            MarkerList = FillAllMarkers(NumberList);
            List<SingleMarkerStruct> singleMarkerList;
            bool doRun = true;
            while (doRun)
            {
                singleMarkerList = GetSingleMarkersInCells();
                if (singleMarkerList.Count != 0)
                {
                    SolveWithMarkerListPlaceNumbers(singleMarkerList);
                    MarkerList = FillAllMarkers(NumberList);
                }
                else
                {
                    singleMarkerList = GetSingleMarkersInColumns();
                    if (singleMarkerList.Count != 0)
                    {
                        SolveWithMarkerListPlaceNumbers(singleMarkerList);
                        MarkerList = FillAllMarkers(NumberList);
                    }
                    else
                    {
                        singleMarkerList = GetSingleMarkersInRows();
                        if (singleMarkerList.Count != 0)
                        {
                            SolveWithMarkerListPlaceNumbers(singleMarkerList);
                            MarkerList = FillAllMarkers(NumberList);
                        }
                        else
                        {
                            singleMarkerList = GetSingleMarkersInSquares();
                            if (singleMarkerList.Count != 0)
                            {
                                SolveWithMarkerListPlaceNumbers(singleMarkerList);
                                MarkerList = FillAllMarkers(NumberList);
                            }
                            else
                            {
                                doRun = false;
                            }
                        }
                    }
                }
            }
        }
        #endregion Methods
    }
}
