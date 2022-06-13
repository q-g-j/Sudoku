using System.Collections.Generic;

namespace Sudoku.Models
{
    public class NumberListModel : List<List<string>>
    {
        #region Constructors
        public NumberListModel()
        {
        }

        public NumberListModel(int capacity) : base(capacity)
        {
        }

        public NumberListModel(IEnumerable<List<string>> collection) : base(collection)
        {
        }

        public NumberListModel(NumberListModel list)
        {
            for (int i = 0; i < 9; i++)
            {
                Add(new List<string>(list[i]));
            }
        }
        #endregion Constructors

        #region Methods
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
        #endregion Methods
    }
}
