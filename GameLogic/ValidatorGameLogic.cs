using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku.Models;

namespace Sudoku.GameLogic
{
    public static class ValidatorGameLogic
    {
        public static bool IsValid(NumbersListModel numbersList, string coords)
        {
            int col = int.Parse(coords[0].ToString());
            int row = int.Parse(coords[1].ToString());
            string number = numbersList[col][row];

            numbersList[col][row] = number;

            if (number == "")
            {
                return true;
            }

            // validate row:
            for (int i = 0; i < 9; i++)
            {
                if (i != col)
                {
                    if (numbersList[i][row] == number)
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
                    if (numbersList[col][i] == number)
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
                        if (numbersList[j][i] == number)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
