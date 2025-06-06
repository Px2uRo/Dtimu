using Dtimu.DiscEditor.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Dtimu.DiscEditor.WPFControls
{
    public class SheetColorWithCounterVedio : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Project p)
            {
                return 20;
            }
            return 0;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class StringArrayConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            else if(value is IEnumerable<string> si)
            {
                var res = new List<string>();
                foreach (var item in si)
                {
                    var ii= Utils.ProcessNameOfPerformancer(item);
                    res.Add(ii);
                }
                return string.Join(";", res);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null) return null;
            else if (value is string si)
            {
                return si.Split(';');
            }
            return null;
        }
    }
    internal class ReadBitMap : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (value is TagLib.IPicture pic)
            {

                    try
                    {
                        return Utils.ConvertArrayToBitmap(pic.Data.ToArray());
                    }
                    catch (Exception)
                    {
                        return Utils.ConvertBase64ToBitmapImage(CodeSetting.ImageOfDisc);
                    }
                
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    internal class GetFileName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (value is string tag)
            {
                return Path.GetFileName(tag);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    public class NullRefBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
                if (value is null)
                {
                    return new SolidColorBrush(Colors.Red);
                }
                return new SolidColorBrush(Colors.Green);
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    internal class ShowStatusC : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return 0d;
            }
            else
            {
                return 20d;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    internal class ShowStatusCV : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return 0d;
            }
            else
            {
                return 20d;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    internal class ShowStatusW : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Project p)
            {
                return 20;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class SheetColorWithCounter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is Project p)
            {
                if(value is MusicFileProjInfo pi)
                {
                    if(p.MusicProjects.IndexOf(pi) % 2 == 0)
                    {
                        return new SolidColorBrush(Colors.Aqua);
                    }
                    return new SolidColorBrush(Colors.White);
                }
            }
            return new SolidColorBrush(Colors.White);
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
