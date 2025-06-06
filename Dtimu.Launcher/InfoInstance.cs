using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Dtimu.Launcher
{
    internal class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)(value))
            {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    internal class InfoInstance : INotifyPropertyChanging
    {
        public string Descrition { get; set; }
        public string CDTitle { get; set; }
        public event PropertyChangingEventHandler PropertyChanging;
        void RaiseEvent(string propname)
        {
            PropertyChanging.Invoke(this,new PropertyChangingEventArgs(propname));
        }

        internal static InfoInstance Parse(string v)
        {
            var ins = new InfoInstance();
            string[] ar = v.Split(new string[] { "&sprt;" }, StringSplitOptions.None);
            ins.CDTitle = ar[0];
            ins.Descrition = ar[1];
            return ins;
        }
#if DEBUG
        public InfoInstance()
        {
            CDTitle = "调试标题";
            Descrition = "感谢您使用 DTIMU! \n这是一个演示简介，实际使用中应换成您对光盘的介绍";
        }
#else
        public InfoInstance()
        {

        }
#endif
    }
}
