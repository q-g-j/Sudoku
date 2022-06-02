using System.Collections.Generic;
using System;
using System.Linq;
using Sudoku.Models;

namespace Sudoku.GameLogic
{
    public class SolverGameLogic
    {
        #region Constructors
        public SolverGameLogic(NumbersListModel list)
        {
            numbersList = new NumbersListModel(list);
            random = new Random();
        }
        #endregion Constructors

        #region Fields
        private readonly NumbersListModel numbersList;
        public NumbersListModel NumbersListSolved;
        private readonly Random random;
        #endregion Fields

        #region Methods
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
            NumbersListSolved = new NumbersListModel(numbersList);
        }

        public void FillSudoku()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (numbersList[col][row] == "")
                    {
                        int[] shuffledIntList = Enumerable.Range(1, 9).OrderBy(c => random.Next()).ToArray();
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
        #endregion Methods
    }
}
