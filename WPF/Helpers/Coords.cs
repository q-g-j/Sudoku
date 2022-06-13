using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Helpers
{
    public struct Coords
    {
        public Coords(int x, int y)
        {
            Col = x;
            Row = y;
        }

        public int Col { get; }
        public int Row { get; }

        public override string ToString() => $"({Col}, {Row})";
    }
}
