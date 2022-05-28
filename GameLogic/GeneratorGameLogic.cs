﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Models;
using Sudoku.Helpers;
using System.Diagnostics;

namespace Sudoku.GameLogic
{
    public class GeneratorGameLogic
    {
        public GeneratorGameLogic()
        {
            random = new Random();
            checkedList = new List<string>();
        }

        private readonly Random random;

        public NumbersListModel NumbersList;
        public int RemoveNumbers;
        public int Counter = 0;
        public int Tries = 0;

        public int UniqueCounter = 0;
        public NumbersListModel UniqueNumbersList;

        private readonly List<string> checkedList;

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
                        if (!HasCurrentColTooFewNumbers(col) && !HasCurrentRowTooFewNumbers(row) && !HasCurrentSquareTooFewNumbers(col, row) &&
                            !HasAnotherColTooManyNumbers(col) && !HasAnotherRowTooManyNumbers(row) && !HasAnotherSquareTooManyNumbers(col, row))
                        {
                            Counter++;
                            NumbersList[col][row] = "";
                            checkedList.Clear();
                        }
                        else
                        {
                            checkedList.Add(col.ToString() + row.ToString());
                        }
                        if (Counter < RemoveNumbers)
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
                        if (!HasCurrentColTooFewNumbers(col) && !HasCurrentRowTooFewNumbers(row) && !HasCurrentSquareTooFewNumbers(col, row) &&
                            !HasAnotherColTooManyNumbers(col) && !HasAnotherRowTooManyNumbers(row) && !HasAnotherSquareTooManyNumbers(col, row))
                        {
                            //// DEBUG:
                            //Stopwatch stopwatch = new Stopwatch();
                            //stopwatch.Start();

                            UniqueNumbersList = NumbersListModel.CopyList(NumbersList);
                            UniqueNumbersList[col][row] = "";
                            UniqueCounter = 0;

                            HasUniqueSolution();                            

                            if (UniqueCounter < 2)
                            {
                                //// DEBUG:
                                //stopwatch.Stop();
                                //Console.WriteLine("Elapsed Time is {0} ms" + " " + stopwatch.Elapsed.TotalMilliseconds + ", " + Tries);
                                //Console.WriteLine(Counter.ToString() + ", " + Tries.ToString());

                                Counter++;
                                NumbersList[col][row] = "";
                                checkedList.Clear();
                            }
                            else
                            {
                                Tries++;
                            }
                        }
                        else
                        {
                            checkedList.Add(col.ToString() + row.ToString());
                        }
                        if (Counter < RemoveNumbers && Tries < 20)
                        {
                            GenerateUniqueSudoku();
                        }
                        return;
                    }
                }
            }
        }
        public void HasUniqueSolution()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (UniqueNumbersList[col][row] == "")
                    {
                        List<int> intList = new List<int>();
                        intList.AddRange(Enumerable.Range(1, 9));

                        foreach (int item in intList)
                        {
                            string number = item.ToString();
                            if (ValidatorGameLogic.IsValid(UniqueNumbersList, col, row, number))
                            {
                                UniqueNumbersList[col][row] = number;
                                if (SolverGameLogic.IsFull(UniqueNumbersList))
                                {
                                    UniqueCounter++;
                                }
                                if (UniqueCounter < 2)
                                {
                                    HasUniqueSolution();
                                    UniqueNumbersList[col][row] = "";
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
                if (NumbersList[currentCol][row] != "")
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
                if (NumbersList[col][currentRow] != "")
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
            int squareCol = (int)(currentCol / 3) * 3;
            int squareRow = (int)(currentRow / 3) * 3;
            int countNumbers = 0;

            for (int innerCol = squareCol; innerCol < squareCol + 3; innerCol++)
            {
                for (int innerRow = squareRow; innerRow < squareRow + 3; innerRow++)
                {
                    if (NumbersList[innerCol][innerRow] != "")
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
                if (NumbersList[currentCol][row] != "")
                {
                    string coords = currentCol.ToString();
                    coords += row.ToString();
                    if (!checkedList.Contains(coords))
                    {
                        countCurrentColNumbers++;
                    }
                }
            }

            for (int col = 0; col < 9; col++)
            {
                if (col != currentCol)
                {
                    int countNumbers = 0;
                    for (int row = 0; row < 9; row++)
                    {
                        if (NumbersList[col][row] != "")
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
                    if (countNumbers > (81 - RemoveNumbers) / 9 + 2 && countCurrentColNumbers < countNumbers)
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
                if (NumbersList[col][currentRow] != "")
                {
                    string coords = col.ToString();
                    coords += currentRow.ToString();
                    if (!checkedList.Contains(coords))
                    {
                        countCurrentRowNumbers++;
                    }
                }
            }

            for (int row = 0; row < 9; row++)
            {
                if (row != currentRow)
                {
                    int countNumbers = 0;
                    for (int col = 0; col < 9; col++)
                    {
                        if (NumbersList[col][row] != "")
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
                    if (countNumbers > (81 - RemoveNumbers) / 9 + 2 && countCurrentRowNumbers < countNumbers)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool HasAnotherSquareTooManyNumbers(int currentCol, int currentRow)
        {
            int currentSquareCol = (int)(currentCol / 3) * 3;
            int currentSquareRow = (int)(currentRow / 3) * 3;
            int currentCountNumbers = 0;

            for (int col = currentSquareCol; col < currentSquareCol + 3; col++)
            {
                for (int row = currentSquareRow; row < currentSquareRow + 3; row++)
                {
                    if (NumbersList[col][row] != "")
                    {
                        string coords = col.ToString();
                        coords += row.ToString();
                        if (!checkedList.Contains(coords))
                        {
                            currentCountNumbers++;
                        }
                    }
                }
            }

            for (int col = 0; col < 9; col++)
            {
                for (int row = 0; row < 9; row++)
                {
                    int squareCol = (int)(col / 3) * 3;
                    int squareRow = (int)(row / 3) * 3;
                    int countNumbers = 0;

                    if (squareCol != currentSquareCol && squareRow != currentSquareRow)
                    {
                        for (int innerCol = squareCol; innerCol < squareCol + 3; innerCol++)
                        {
                            for (int innerRow = squareRow; innerRow < squareRow + 3; innerRow++)
                            {
                                if (NumbersList[innerCol][innerRow] != "")
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

                        if (countNumbers > (81 - RemoveNumbers) / 9 + 2 && currentCountNumbers < countNumbers)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
