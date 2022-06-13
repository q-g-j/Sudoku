using System.Collections.Generic;
using System;
using System.Linq;
using Sudoku.Models;

namespace Sudoku.GameLogic
{
    public class SolverGameLogic
    {
        #region Constructors
        public SolverGameLogic(NumberListModel list)
        {
            numberList = new NumberListModel(list);
            random = new Random();
        }
        #endregion Constructors

        #region Fields
        private readonly NumberListModel numberList;
        public NumberListModel NumberListSolved;
        private readonly Random random;
        #endregion Fields

        #region Methods
        public static bool IsFull(NumberListModel numberList)
        {
            bool isFull = true;
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (numberList[col][row] == "")
                    {
                        isFull = false;
                    }
                }
            }
            return isFull;
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
                    if (numberList[col][row] == "")
                    {
                        int[] shuffledIntList = Enumerable.Range(1, 9).OrderBy(c => random.Next().ToString()).ToArray();
                        foreach (int item in shuffledIntList)
                        {
                            string number = item.ToString();
                            if (ValidatorGameLogic.IsValid(numberList, col, row, number))
                            {
                                numberList[col][row] = number;
                                if (IsFull(numberList))
                                {
                                    CopySolution(numberList);
                                }
                                else
                                {
                                    FillSudoku();
                                    numberList[col][row] = "";
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
