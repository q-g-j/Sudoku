using System.Collections.Generic;
using Sudoku.Models;

namespace Sudoku.GameLogic
{
    public static class ValidatorGameLogic
    {
        public static bool IsValid(NumbersListModel numbersList, int col, int row, string number)
        {
            if (number == "")
            {
                return true;
            }

            string oldNum = numbersList[col][row];
            numbersList[col][row] = number;

            // validate row:
            for (int i = 0; i < 9; i++)
            {
                if (i != col)
                {
                    if (numbersList[i][row] == number)
                    {
                        numbersList[col][row] = oldNum;
                        return false;
                    }
                }
            }

            // validate col:
            for (int i = 0; i < 9; i++)
            {
                if (i != row)
                {
                    if (numbersList[col][i] == number)
                    {
                        numbersList[col][row] = oldNum;
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
                        if (numbersList[j][i] == number)
                        {
                            numbersList[col][row] = oldNum;
                            return false;
                        }
                    }
                }
            }

            numbersList[col][row] = oldNum;
            return true;
        }
    }
}
