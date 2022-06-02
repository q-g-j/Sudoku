using System.Collections.Generic;

namespace Sudoku.Models
{
    public class NumbersColorsListModel : List<List<string>>
    {
        #region Constructors
        public NumbersColorsListModel()
        {
        }

        public NumbersColorsListModel(int capacity) : base(capacity)
        {
        }

        public NumbersColorsListModel(IEnumerable<List<string>> collection) : base(collection)
        {
        }
        public NumbersColorsListModel(NumbersColorsListModel list)
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
