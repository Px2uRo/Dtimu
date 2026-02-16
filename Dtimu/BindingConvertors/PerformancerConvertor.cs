using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Dtimu.BindingConvertors
{
    class PerformancerConvertor: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var list = value as IEnumerable<string>;
            return list == null ? null : string.Join(", ", list);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
