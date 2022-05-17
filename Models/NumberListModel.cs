using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Models
{
    public class NumbersListModel : List<List<List<List<List<List<string>>>>>>
    {
        public NumbersListModel()
        {
            for (int i = 0; i < 3; i++)
            {
                List<List<List<List<List<string>>>>> tempList1 = new List<List<List<List<List<string>>>>>();
                for (int j = 0; j < 3; j++)
                {
                    List<List<List<List<string>>>> tempList2 = new List<List<List<List<string>>>>();
                    for (int k = 0; k < 3; k++)
                    {
                        List<List<List<string>>> tempList3 = new List<List<List<string>>>();
                        for (int l = 0; l < 3; l++)
                        {
                            List<List<string>> tempList4 = new List<List<string>>();
                            for (int m = 0; m < 3; m++)
                            {
                                List<string> tempList5 = new List<string>();
                                for (int n = 0; n < 3; n++)
                                {
                                    tempList5.Add("");
                                }
                                tempList4.Add(tempList5);
                            }
                            tempList3.Add(tempList4);
                        }
                        tempList2.Add(tempList3);
                    }
                    tempList1.Add(tempList2);
                }
                Add(tempList1);
            }
        }
    }
}
