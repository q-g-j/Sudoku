using Sudoku.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Sudoku.GameLogic
{
    public class SolverGameLogic
    {
        public NumbersListModel numbersList;
        public int counter = 1;
        private readonly Random random;

        public SolverGameLogic()
        {
            random = new Random();
        }

        public bool SolvePuzzle()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (numbersList[j][i] == "")
                    {
                        List<int> intList = new List<int>();
                        intList.AddRange(Enumerable.Range(1, 9));
                        var shuffledIntList = intList.OrderBy(item => random.Next());

                        foreach (int item in shuffledIntList)
                        {
                            string num = item.ToString();
                            string coords = j.ToString();
                            coords += i.ToString();
                            numbersList[j][i] = num;
                            bool isValid = ValidatorGameLogic.IsValid(numbersList, coords);
                            numbersList[j][i] = "";
                            if (isValid)
                            {
                                numbersList[j][i] = num;

                                if (SolvePuzzle())
                                {
                                    if (counter == 1)
                                        return true;
                                    //else if (counter == 100)
                                    //    return false;
                                    else
                                        counter++;
                                }
                                else
                                    numbersList[j][i] = "";
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        //public bool UniqueSolver()
        //{
        //    for (int i = 0; i < 9; i++)
        //    {
        //        for (int j = 0; j < 9; j++)
        //        {
        //            if (numbersList[j][i] == "")
        //            {
        //                List<int> intList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        //                var shuffledIntList = intList.OrderBy(item => random.Next());

        //                foreach (int item in shuffledIntList)
        //                {
        //                    string num = item.ToString();
        //                    string coords = j.ToString();
        //                    coords += i.ToString();
        //                    numbersList[j][i] = num;
        //                    bool isValid = ValidatorGameLogic.IsValid(numbersList, coords);
        //                    numbersList[j][i] = "";
        //                    if (isValid)
        //                    {
        //                        numbersList[j][i] = num;

        //                        if (SolvePuzzle())
        //                        {
        //                            if (counter == 1)
        //                                return true;
        //                            //else if (counter == 100)
        //                            //    return false;
        //                            else
        //                                counter++;
        //                        }
        //                        else
        //                            numbersList[j][i] = "";
        //                    }
        //                }
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}
    }
}
