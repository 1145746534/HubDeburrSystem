using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HubDeburrSystem.Public.Converter
{
    /// <summary>
    /// 渐变画笔转换器
    /// </summary>
    public class GradientBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string startColorHex = values[0] as string;
            string endColorHex = values[1] as string;

            Color startColor = (Color)ColorConverter.ConvertFromString(startColorHex);
            Color endColor = (Color)ColorConverter.ConvertFromString(endColorHex);

            return new LinearGradientBrush(startColor, endColor, new Point(0, 0), new Point(0, 1));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
