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
            NumberList = new NumberListModel(list);
            random = new Random();
            Tries = 0;
        }
        #endregion Constructors

        #region Fields
        internal readonly NumberListModel NumberList;
        internal NumberListModel NumberListSolved;
        private readonly Random random;
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
                    }
                }
            }
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
                                else if (Tries <= 500000)
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
        #endregion Methods
    }
}
