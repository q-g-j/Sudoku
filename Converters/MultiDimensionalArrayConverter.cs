using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Sudoku.Converters
{
    public class MultiDimensionalArrayConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            return (values[0] as String[][][][][][])[(Int16)values[1]][(Int16)values[2]][(Int16)values[3]][(Int16)values[4]][(Int16)values[5]][(Int16)values[6]];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
