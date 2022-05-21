using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku.GameLogic;
using Sudoku.Models;
using Sudoku.Debug;

namespace Sudoku.GameLogic
{
    public class GeneratorGameLogic
    {
        public GeneratorGameLogic()
        {
            random = new Random();
            emptiedList = new List<string>();
        }

        private readonly List<string> emptiedList;
        private readonly Random random;

        public NumbersListModel NumbersList;
        public int RemoveNumbers;
        public int counter = 1;

        public bool GenerateSudoku()
        {
            List<int> intRowList = new List<int>();
            intRowList.AddRange(Enumerable.Range(0, 9));
            var shuffledIntRowList = intRowList.OrderBy(item => random.Next());
            foreach (var i in shuffledIntRowList)
            {
                List<int> intColList = new List<int>();
                intColList.AddRange(Enumerable.Range(0, 9));
                var shuffledIntColList = intColList.OrderBy(item => random.Next());
                foreach (var j in shuffledIntColList)
                {
                    string coords = j.ToString();
                    coords += i.ToString();

                    if (! emptiedList.Contains(coords) && counter <= RemoveNumbers)
                    {
                        NumbersList[j][i] = "";
                        counter++;
                        emptiedList.Add(coords);

                        if (GenerateSudoku())
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
