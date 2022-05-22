using System.Collections.Generic;
using System;
using System.Linq;
using Sudoku.Models;

namespace Sudoku.GameLogic
{
    public class SolverGameLogic
    {
        private NumbersListModel numbersList;
        public NumbersListModel NumbersListSolved;

        public SolverGameLogic(NumbersListModel list)
        {
            numbersList = NumbersListModel.CopyList(list);
        }

        public static bool IsFull(NumbersListModel numbersList)
        {
            bool isFull = true;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (numbersList[col][row] == "")
                    {
                        isFull = false;
                    }
                }
            }
            return isFull;
        }

        private void CopySolution(NumbersListModel numbersList)
        {
            NumbersListSolved = NumbersListModel.CopyList(numbersList);
        }

        public void FillSudoku()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (numbersList[col][row] == "")
                    {
                        List<int> intList = new List<int>();
                        intList.AddRange(Enumerable.Range(1, 9));
                        Random random = new Random();
                        var shuffledIntList = intList.OrderBy(item => random.Next());

                        foreach (int item in shuffledIntList)
                        {
                            string number = item.ToString();
                            if (ValidatorGameLogic.IsValid(numbersList, col, row, number))
                            {
                                numbersList[col][row] = number;
                                if (IsFull(numbersList))
                                {
                                    CopySolution(numbersList);
                                }
                                else
                                {
                                    FillSudoku();
                                    numbersList[col][row] = "";
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
