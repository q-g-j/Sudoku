using System.Collections.Generic;

namespace Sudoku.Models
{
    public class ButtonBackgroundListModel : List<List<string>>
    {
        #region Constructors
        public ButtonBackgroundListModel()
        {
        }

        public ButtonBackgroundListModel(int capacity) : base(capacity)
        {
        }

        public ButtonBackgroundListModel(IEnumerable<List<string>> collection) : base(collection)
        {
        }
        public ButtonBackgroundListModel(ButtonBackgroundListModel list)
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
                    tempList.Add("white");
                }
                Add(tempList);
            }
        }
        #endregion Methods
    }
}
