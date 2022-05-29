using System.Collections.Generic;

namespace Sudoku.Models
{
    public class NumbersListModel : List<List<string>>
    {
        public NumbersListModel()
        {
        }

        public NumbersListModel(int capacity) : base(capacity)
        {
        }

        public NumbersListModel(IEnumerable<List<string>> collection) : base(collection)
        {
        }

        public NumbersListModel(NumbersListModel list)
        {
            for (int i = 0; i < 9; i++)
            {
                Add(new List<string>(list[i]));
            }
        }

        public void InitializeList()
        {
            for (int i = 0; i < 9; i++)
            {
                List<string> tempList = new List<string>();
                for (int j = 0; j < 9; j++)
                {
                    tempList.Add("");
                }
                Add(tempList);
            }
        }
    }
}
