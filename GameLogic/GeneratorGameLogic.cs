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
        public int counter = 0;
        public int tries = 0;

        private int uniqueCounter;
        private NumbersListModel uniqueNumbersList;

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
                        uniqueNumbersList = NumbersListModel.CopyList(NumbersList);
                        uniqueCounter = 0;
                        HasUniqueSolution();
                        if (uniqueCounter != 1)
                        {
                            tries++;
                            NumbersList[col][row] = oldNumber;
                        }
                        else
                        {
                            counter++;
                        }
                        if (counter < RemoveNumbers && tries < 20)
                        {
                            GenerateSudoku();
                        }
                        return;
                    }
                }
            }
        }

        private void HasUniqueSolution()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (uniqueNumbersList[col][row] == "")
                    {
                        List<int> intList = new List<int>();
                        intList.AddRange(Enumerable.Range(1, 9));

                        foreach (int item in intList)
                        {
                            string number = item.ToString();
                            if (ValidatorGameLogic.IsValid(uniqueNumbersList, col, row, number))
                            {
                                uniqueNumbersList[col][row] = number;
                                if (SolverGameLogic.IsFull(uniqueNumbersList))
                                {
                                    uniqueCounter++;
                                }
                                if (uniqueCounter < 2)
                                {
                                    HasUniqueSolution();
                                    uniqueNumbersList[col][row] = "";
                                }
                            }
                        }
                        return;
                    }
                }
            }
        }
    }
}
