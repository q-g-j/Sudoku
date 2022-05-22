using System.Collections.Generic;
using Sudoku.Models;

namespace Sudoku.GameLogic
{
    public static class ValidatorGameLogic
    {
        public static bool IsValid(NumbersListModel numbersList, int col, int row, string number)
        {
            List<List<string>> tempNumbersList = new List<List<string>>();


            for (int i = 0; i < 9; i++)
            {
                List<string> tempList = new List<string>();
                for (int j = 0; j < 9; j++)
                {
                    tempList.Add(numbersList[i][j]);
                }
                tempNumbersList.Add(tempList);
            }

            tempNumbersList[col][row] = number;

            if (number == "")
            {
                return true;
            }

            // validate row:
            for (int i = 0; i < 9; i++)
            {
                if (i != col)
                {
                    if (tempNumbersList[i][row] == number)
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
                    if (tempNumbersList[col][i] == number)
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
                        if (tempNumbersList[j][i] == number)
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
