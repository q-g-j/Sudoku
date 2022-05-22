using System.Collections.Generic;

namespace Sudoku.Models
{
    public class MarkersListModel : List<List<List<List<string>>>>
    {
        public MarkersListModel()
        {
        }

        public MarkersListModel(int capacity) : base(capacity)
        {
        }

        public MarkersListModel(IEnumerable<List<List<List<string>>>> collection) : base(collection)
        {
        }

        public void InitializeList()
        {
            for (int i = 0; i < 9; i++)
            {
                List<List<List<string>>> tempList1 = new List<List<List<string>>>();
                for (int j = 0; j < 9; j++)
                {
                    List<List<string>> tempList2 = new List<List<string>>();
                    for (int k = 0; k < 3; k++)
                    {
                        List<string> tempList3 = new List<string>();
                        for (int l = 0; l < 3; l++)
                        {
                            tempList3.Add("");
                        }
                        tempList2.Add(tempList3);
                    }
                    tempList1.Add(tempList2);
                }
                Add(tempList1);
            }
        }

        public static MarkersListModel CopyList(MarkersListModel list)
        {
            MarkersListModel returnList = new MarkersListModel();
            for (int i = 0; i < 9; i++)
            {
                List<List<List<string>>> tempList1 = new List<List<List<string>>>();
                for (int j = 0; j < 9; j++)
                {
                    List<List<string>> tempList2 = new List<List<string>>();
                    for (int k = 0; k < 3; k++)
                    {
                        List<string> tempList3 = new List<string>();
                        for (int l = 0; l < 3; l++)
                        {
                            tempList3.Add(list[i][j][k][l]);
                        }
                        tempList2.Add(tempList3);
                    }
                    tempList1.Add(tempList2);
                }
                returnList.Add(tempList1);
            }
            return returnList;
        }
    }
}
