using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku.GameLogic;
using Sudoku.Models;

namespace Sudoku.GameLogic
{
    public class GeneratorGameLogic
    {
        public GeneratorGameLogic()
        {
            random = new Random();
            solverGameLogic = new SolverGameLogic();
            emptiedList = new List<string>();
        }

        public int counter;
        private readonly Random random;
        public NumbersListModel numbersList;
        private readonly SolverGameLogic solverGameLogic;
        private List<string> emptiedList;

        public void Inititialize()
        {
        }

        public bool GenerateSudoku()
        {
            List<int> intRowList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            var shuffledIntRowList = intRowList.OrderBy(item => random.Next());
            foreach (var i in shuffledIntRowList)
            {
                List<int> intColList = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                var shuffledIntColList = intColList.OrderBy(item => random.Next());
                foreach (var j in shuffledIntColList)
                {
                    string coords = j.ToString();
                    coords += i.ToString();

                    if (! emptiedList.Contains(coords) && counter < 60)
                    {
                        string oldNum = numbersList[j][i].ToString();
                        //numbersList[j][i] = "";
                        //NumbersListModel tempList = new NumbersListModel();
                        //solverGameLogic.numbersList = tempList;
                        //solverGameLogic.counter = 1;
                        //bool isSolvable = solverGameLogic.Solver();
                        //numbersList[j][i] = oldNum;
                        //if (isSolvable)
                        {
                            numbersList[j][i] = "";
                            counter++;
                            emptiedList.Add(coords);

                            if (GenerateSudoku())
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
