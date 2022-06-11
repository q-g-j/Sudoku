using System.Collections.Generic;

namespace Sudoku.Models
{
    public class NumberColorListModel : List<List<string>>
    {
        #region Constructors
        public NumberColorListModel()
        {
        }

        public NumberColorListModel(int capacity) : base(capacity)
        {
        }

        public NumberColorListModel(IEnumerable<List<string>> collection) : base(collection)
        {
        }
        public NumberColorListModel(NumberColorListModel list)
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
                    tempList.Add("#0066ff");
                }
                Add(tempList);
            }
        }
        #endregion Methods
    }
}
