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
        public static string CellBackgroundLeftHighlighted { get => "#d0ebfb"; }
        public static string CellBackgroundRightHighlighted { get => "#f9e1d2"; }
        public static string CellBackgroundLeftHighlightedMouseOver { get => "#8acef5"; }
        public static string CellBackgroundRightHighlightedMouseOver { get => "#eda578"; }
        public static string CellBackgroundLeftSelected { get => "#ffff00"; }
        public static string CellBackgroundRightSelected { get => "#ffff00"; }
        public static string CellBackgroundLeftSelectedMouseOver { get => "#e6e619"; }
        public static string CellBackgroundRightSelectedMouseOver { get => "#e6e619"; }
        public static string CellBackgroundConflicts { get => "Red"; }
        public static string CellBackgroundConflictsSelected { get => "#ff6666"; }
        public static string CellBackgroundConflictsMouseOver { get => "#cc0000"; }
        public static string CellBackgroundMouseOver { get => "LightBlue"; }
        public static string CellNumber { get => "#0066ff"; }
        public static string CellNumberGenerator { get => "Black"; }
        public static string CellMarker { get => "Black"; }
        public static string ButtonSelectNumber { get => "#9ed7fa"; }
        public static string ButtonSelectMarker { get => "#eda578"; }
        public static string ButtonSelectNumberMouseOver { get => "#6ec3f7"; }
        public static string ButtonSelectMarkerMouseOver { get => "#e37835"; }
        public static string ButtonSelectDifficulty { get => "#eda374"; }
        public static string LabelSingleSolutionWait { get => "LightSkyBlue"; }
        public static string LabelValidateHasNoConflicts { get => "#40bf80"; }
        public static string LabelValidateHasConflicts { get => "#ff531a"; }
        public static string BorderDefault { get => "Black"; }
        public static string BorderOutermost { get => "Blue"; }
        public static string LineMiddle { get => "Black"; }
        public static string LineInnermost { get => "#a6a6a6"; }
    }
}
