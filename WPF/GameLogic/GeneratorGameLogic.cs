using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Models;
using Sudoku.Helpers;
using System.Diagnostics;

namespace Sudoku.GameLogic
{
    public class GeneratorGameLogic
    {
        #region Constructors
        public GeneratorGameLogic(string difficulty, NumberListModel numberList)
        {
            random = new Random();
            checkedList = new List<string>();
            coordsList = new List<Coords>();
            NumberList = numberList;

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    coordsList.Add(new Coords(col, row));
                }
            }

            if (difficulty == "Easy")
            {
                removeNumbers = 42; // 42
            }
            else if (difficulty == "Medium")
            {
                removeNumbers = 50; // 50
            }
            else if (difficulty == "Hard")
            {
                removeNumbers = 54; // 57
            }
        }
        #endregion Constructors

        #region Fields
        private readonly List<Coords> coordsList;
        private readonly List<string> checkedList;
        private readonly Random random;

        public NumberListModel NumberList;
        public NumberListModel SingleSolutionNumberList;
        public int removeNumbers;
        public int Counter = 0;
        public int SingleSolutionCounter = 0;
        public int Tries = 0;
        #endregion Fields

        #region Methods
        public void GenerateSudoku(bool doSingleSolution, bool doLogicallySolvable)
        {
            List<Coords> shuffledCoordsList = coordsList.OrderBy(item => random.Next()).ToList();
            foreach (var coords in shuffledCoordsList)
            {
                string oldNumber = NumberList[coords.Col][coords.Row];
                if (NumberList[coords.Col][coords.Row] != "")
                {
                    if (!HasCurrentColTooFewNumbers(coords.Col) && !HasCurrentRowTooFewNumbers(coords.Row) && !HasCurrentSquareTooFewNumbers(coords.Col, coords.Row) &&
                        !HasAnotherColTooManyNumbers(coords.Col) && !HasAnotherRowTooManyNumbers(coords.Row) && !HasAnotherSquareTooManyNumbers(coords.Col, coords.Row))
                    {
                        bool hasSolvedLogically = false;
                        SingleSolutionNumberList = new NumberListModel(NumberList);
                        if (doSingleSolution || doLogicallySolvable)
                        {
                            SingleSolutionNumberList[coords.Col][coords.Row] = "";
                            SingleSolutionCounter = 0;
                            HasSingleSolution();
                        }
                        if (SingleSolutionCounter < 2)
                        {
                            NumberListModel logicallySolvableNumberList = new NumberListModel(NumberList);
                            logicallySolvableNumberList[coords.Col][coords.Row] = "";
                            SolverGameLogic solverGameLogic = new SolverGameLogic(logicallySolvableNumberList);
                            solverGameLogic.SolveWithMarkerList();

                            if (SolverGameLogic.IsFull(solverGameLogic.NumberList))
                            {
                                hasSolvedLogically = true;
                            }

                            // For medium or hard difficulty only, make sure the sudoku is NOT solvable with the single candidate technique,
                            // if doLogicallySolvable == "False" to avoid making it too easy:
                            if ((doSingleSolution && doLogicallySolvable && hasSolvedLogically) ||
                                (!doSingleSolution && doLogicallySolvable && hasSolvedLogically) ||
                                (doSingleSolution && !doLogicallySolvable && !hasSolvedLogically && Counter >= 49) ||
                                (doSingleSolution && !doLogicallySolvable && Counter < 49) ||
                                (!doSingleSolution && !doLogicallySolvable && !hasSolvedLogically && Counter >= 41) ||
                                (!doSingleSolution && !doLogicallySolvable && Counter < 41))
                            {
                                Counter++;
                                NumberList[coords.Col][coords.Row] = "";
                                checkedList.Clear();
                            }
                        }
                        else
                        {
                            Tries++;
                        }
                    }
                    else
                    {
                        checkedList.Add(coords.Col.ToString() + coords.Row.ToString());
                    }

                    if (Counter < removeNumbers && Tries < 20)
                    {
                        GenerateSudoku(doSingleSolution, doLogicallySolvable);
                    }
                    return;
                }
            }
        }
        private void HasSingleSolution()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (SingleSolutionNumberList[col][row] == "")
                    {
                        for (int i = 1; i < 10; i++)
                        {
                            string number = i.ToString();
                            if (ValidatorGameLogic.IsValid(SingleSolutionNumberList, col, row, number))
                            {
                                SingleSolutionNumberList[col][row] = number;
                                if (SolverGameLogic.IsFull(SingleSolutionNumberList))
                                {
                                    SingleSolutionCounter++;
                                }
                                if (SingleSolutionCounter < 2)
                                {
                                    HasSingleSolution();
                                    SingleSolutionNumberList[col][row] = "";
                                }

                            }
                        }
                        return;
                    }
                }
            }
        }
        private bool HasCurrentColTooFewNumbers(int currentCol)
        {
            int countNumbers = 0;
            for (int row = 0; row < 9; row++)
            {
                if (NumberList[currentCol][row] != "")
                {
                    countNumbers++;
                }
            }
            if (countNumbers < 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool HasCurrentRowTooFewNumbers(int currentRow)
        {
            int countNumbers = 0;
            for (int col = 0; col < 9; col++)
            {
                if (NumberList[col][currentRow] != "")
                {
                    countNumbers++;
                }
            }
            if (countNumbers < 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool HasCurrentSquareTooFewNumbers(int currentCol, int currentRow)
        {
            int squareCol = (currentCol / 3) * 3;
            int squareRow = (currentRow / 3) * 3;
            int countNumbers = 0;

            for (int innerCol = squareCol; innerCol < squareCol + 3; innerCol++)
            {
                for (int innerRow = squareRow; innerRow < squareRow + 3; innerRow++)
                {
                    if (NumberList[innerCol][innerRow] != "")
                    {
                        countNumbers++;
                    }
                }
            }
            if (countNumbers <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool HasAnotherColTooManyNumbers(int currentCol)
        {
            int countCurrentColNumbers = 0;
            for (int row = 0; row < 9; row++)
            {
                if (NumberList[currentCol][row] != "")
                {
                    countCurrentColNumbers++;
                }
            }

            for (int col = 0; col < 9; col++)
            {
                if (col != currentCol)
                {
                    int countNumbers = 0;
                    for (int row = 0; row < 9; row++)
                    {
                        if (NumberList[col][row] != "")
                        {
                            string coords = currentCol.ToString();
                            coords += row.ToString();
                            if (!checkedList.Contains(coords))
                            {
                                countNumbers++;
                            }
                        }
                    }
                    // (81 - RemoveNumbers) / 9 + 1
                    if (countNumbers > (81 - removeNumbers) / 9 + 3 && countCurrentColNumbers < countNumbers)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool HasAnotherRowTooManyNumbers(int currentRow)
        {
            int countCurrentRowNumbers = 0;
            for (int col = 0; col < 9; col++)
            {
                if (NumberList[col][currentRow] != "")
                {
                    countCurrentRowNumbers++;
                }
            }

            for (int row = 0; row < 9; row++)
            {
                if (row != currentRow)
                {
                    int countNumbers = 0;
                    for (int col = 0; col < 9; col++)
                    {
                        if (NumberList[col][row] != "")
                        {
                            string coords = col.ToString();
                            coords += row.ToString();
                            if (!checkedList.Contains(coords))
                            {
                                countNumbers++;
                            }
                        }
                    }
                    // (81 - RemoveNumbers) / 9 + 1
                    if (countNumbers > (81 - removeNumbers) / 9 + 3 && countCurrentRowNumbers < countNumbers)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool HasAnotherSquareTooManyNumbers(int currentCol, int currentRow)
        {
            int currentSquareCol = (currentCol / 3) * 3;
            int currentSquareRow = (currentRow / 3) * 3;
            int countCurrentSquareNumbers = 0;

            for (int col = currentSquareCol; col < currentSquareCol + 3; col++)
            {
                for (int row = currentSquareRow; row < currentSquareRow + 3; row++)
                {
                    if (NumberList[col][row] != "")
                    {
                        countCurrentSquareNumbers++;
                    }
                }
            }

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    int squareCol = (col / 3) * 3;
                    int squareRow = (row / 3) * 3;
                    int countNumbers = 0;

                    if (squareCol != currentSquareCol && squareRow != currentSquareRow)
                    {
                        for (int innerCol = squareCol; innerCol < squareCol + 3; innerCol++)
                        {
                            for (int innerRow = squareRow; innerRow < squareRow + 3; innerRow++)
                            {
                                if (NumberList[innerCol][innerRow] != "")
                                {
                                    string coords = innerCol.ToString();
                                    coords += innerRow.ToString();
                                    if (!checkedList.Contains(coords))
                                    {
                                        countNumbers++;
                                    }
                                }
                            }
                        }

                        if (countNumbers > (81 - removeNumbers) / 9 + 3 && countCurrentSquareNumbers < countNumbers)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion Methods
    }
}
