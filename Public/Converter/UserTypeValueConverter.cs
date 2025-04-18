using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HubDeburrSystem.Public.Converter
{
    public class UserTypeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "(null)";
            else if (value.ToString() == "1") return "管理员";
            else if (value.ToString() == "2") return "技术员";
            else if (value.ToString() == "3") return "操作员";

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
