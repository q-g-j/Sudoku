using System.Collections.Generic;
using Sudoku.Models;

namespace Sudoku.GameLogic
{
    public static class ValidatorGameLogic
    {
        #region Methods
        public static bool IsValid(NumberListModel numberList, int col, int row, string number)
        {
            if (number == "")
            {
                return true;
            }

            // validate row:
            for (int i = 0; i < 9; i++)
            {
                if (i != col)
                {
                    if (numberList[i][row] == number)
                    {
                        return false;
                    }
                }
            }

            // validate col:
            for (int i = 0; i < 9; i++)
            {
                if (i != row)
                {
                    if (numberList[col][i] == number)
                    {
                        return false;
                    }
                }
            }

            // validate square:
            int col2 = (int)(col / 3) * 3;
            int row2 = (int)(row / 3) * 3;

            for (int i = row2; i < row2 + 3; i++)
            {
                for (int j = col2; j < col2 + 3; j++)
                {
                    if (i != row && j != col)
                    {
                        if (numberList[j][i] == number)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        #endregion Methods
    }
}
