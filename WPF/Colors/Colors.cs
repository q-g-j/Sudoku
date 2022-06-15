using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public static class Colors
    {
        public static string CellBackgroundDefault { get => "White"; }
        public static string CellBackgroundHighlighted { get => "LightYellow"; }
        public static string CellBackgroundSelected { get => "Yellow"; }
        public static string CellBackgroundConflicts { get => "Red"; }
        public static string CellBackgroundConflictsSelected { get => "#ff6666"; }
        public static string CellBackgroundMouseOver { get => "LightBlue"; }
        public static string CellNumber { get => "#0066ff"; }
        public static string CellNumberGenerator { get => "Black"; }
        public static string CellMarker { get => "Black"; }
        public static string ButtonSelectNumber { get => "#d8effd"; }
        public static string ButtonSelectDifficulty { get => "#eda374"; }
        public static string LabelSingleSolutionWait { get => "LightSkyBlue"; }
        public static string LabelValidateHasNoConflicts { get => "#40bf80"; }
        public static string LabelValidateHasConflicts { get => "#ff531a"; }
        public static string BorderDefault { get => "Black"; }
        public static string BorderOutermost { get => "Blue"; }
        public static string LineMiddle { get => "#ff4000"; }
        public static string LineInnermost { get => "#a6a6a6"; }


    }
}
