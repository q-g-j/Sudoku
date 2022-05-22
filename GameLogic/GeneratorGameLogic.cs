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
        }

        private readonly Random random;

        public NumbersListModel NumbersList;
        public int RemoveNumbers;
        public int counter = 1;
        public int tries = 0;

        public void GenerateSudoku()
        {
            List<int> intRowList = new List<int>();
            intRowList.AddRange(Enumerable.Range(0, 9));
            var shuffledIntRowList = intRowList.OrderBy(item => random.Next());
            foreach (var row in shuffledIntRowList)
            {
                List<int> intColList = new List<int>();
                intColList.AddRange(Enumerable.Range(0, 9));
                var shuffledIntColList = intColList.OrderBy(item => random.Next());
                foreach (var col in shuffledIntColList)
                {
                    if (NumbersList[col][row] != "")
                    {
                        string oldNumber = NumbersList[col][row];
                        NumbersList[col][row] = "";
                        SolverGameLogic solverGameLogic = new SolverGameLogic(NumbersList);
                        solverGameLogic.HasUniqueSolution();
                        if (solverGameLogic.counter != 1)
                        {
                            tries++;
                            NumbersList[col][row] = oldNumber;
                        }
                        else
                        {
                            counter++;
                        }
                        if (counter <= RemoveNumbers && tries < 20)
                        {
                            GenerateSudoku();
                        }
                        return;
                    }
                }
            }
        }
    }
}
