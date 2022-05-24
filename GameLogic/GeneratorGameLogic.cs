using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Models;

namespace Sudoku.GameLogic
{
    public class GeneratorGameLogic
    {
        public GeneratorGameLogic()
        {
            random = new Random();
        }

        private readonly Random random;

        public NumbersListModel NumbersList;
        public int RemoveNumbers;
        public int counter = 0;
        public int tries = 0;

        private int uniqueCounter;
        private NumbersListModel uniqueNumbersList;

        public void GenerateSudoku()
        {
            List<int> intRowList = new List<int>();
            intRowList.AddRange(Enumerable.Range(0, 9));
            var shuffledIntRowList = intRowList.OrderBy(item => random.Next());
            foreach (var row in shuffledIntRowList)
            {
                List<int> intColList = new List<int>();
                intColList.AddRange(Enumerable.Range(0, 9));
                var shuffledIntColList = intColList.OrderBy(item => random.Next());
                foreach (var col in shuffledIntColList)
                {
                    if (NumbersList[col][row] != "")
                    {
                        if (!HasCurrentSquareTooFewNumbers(col, row) && !HasAnotherSquareTooManyNumbers(col, row))
                        {
                            counter++;
                            NumbersList[col][row] = "";
                        }
                        if (counter < RemoveNumbers)
                        {
                            GenerateSudoku();
                        }
                        return;
                    }
                }
            }
        }

        public void GenerateUniqueSudoku()
        {
            List<int> intRowList = new List<int>();
            intRowList.AddRange(Enumerable.Range(0, 9));
            var shuffledIntRowList = intRowList.OrderBy(item => random.Next());
            foreach (var row in shuffledIntRowList)
            {
                List<int> intColList = new List<int>();
                intColList.AddRange(Enumerable.Range(0, 9));
                var shuffledIntColList = intColList.OrderBy(item => random.Next());
                foreach (var col in shuffledIntColList)
                {
                    if (NumbersList[col][row] != "")
                    {
                        string oldNumber = NumbersList[col][row];
                        NumbersList[col][row] = "";
                        uniqueNumbersList = NumbersListModel.CopyList(NumbersList);
                        uniqueCounter = 0;
                        HasUniqueSolution();
                        if (uniqueCounter != 1)
                        {
                            tries++;
                            NumbersList[col][row] = oldNumber;
                        }
                        else
                        {
                            counter++;
                        }
                        if (counter < RemoveNumbers && tries < 20)
                        {
                            GenerateUniqueSudoku();
                        }
                        return;
                    }
                }
            }
        }

        private bool HasCurrentSquareTooFewNumbers(int col, int row)
        {
            int squareCol = (int)(col / 3) * 3;
            int squareRow = (int)(row / 3) * 3;
            int countNumbers = 0;

            for (int i = squareCol; i < squareCol + 3; i++)
            {
                for (int j = squareRow; j < squareRow + 3; j++)
                {
                    if (NumbersList[i][j] != "")
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

        private bool HasAnotherSquareTooManyNumbers(int col, int row)
        {
            int currentSquareCol = (int)(col / 3) * 3;
            int currentSquareRow = (int)(row / 3) * 3;
            int currentCountNumbers = 0;

            for (int k = currentSquareCol; k < currentSquareCol + 3; k++)
            {
                for (int l = currentSquareRow; l < currentSquareRow + 3; l++)
                {
                    if (NumbersList[k][l] != "")
                    {
                        currentCountNumbers++;
                    }
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int squareCol = (int)(i / 3) * 3;
                    int squareRow = (int)(j / 3) * 3;
                    int countNumbers = 0;

                    for (int k = squareCol; k < squareCol + 3; k++)
                    {
                        for (int l = squareRow; l < squareRow + 3; l++)
                        {
                            if (NumbersList[k][l] != "")
                            {
                                if (squareCol == currentSquareCol && squareRow == currentSquareRow)
                                {
                                    continue;
                                }
                                else
                                {
                                    countNumbers++;
                                }
                            }
                        }
                    }

                    if (countNumbers > 8 && countNumbers > currentCountNumbers)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void HasUniqueSolution()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (uniqueNumbersList[col][row] == "")
                    {
                        List<int> intList = new List<int>();
                        intList.AddRange(Enumerable.Range(1, 9));

                        foreach (int item in intList)
                        {
                            string number = item.ToString();
                            if (ValidatorGameLogic.IsValid(uniqueNumbersList, col, row, number))
                            {
                                uniqueNumbersList[col][row] = number;
                                if (SolverGameLogic.IsFull(uniqueNumbersList))
                                {
                                    uniqueCounter++;
                                }
                                if (uniqueCounter < 2)
                                {
                                    HasUniqueSolution();
                                    uniqueNumbersList[col][row] = "";
                                }
                            }
                        }
                        return;
                    }
                }
            }
        }
    }
}
