using System;
using Microsoft.Win32;

namespace DiscViewer
{
    public class OSVersionHelper
    {
        public static string GetWindowsVersion()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        string productName = key.GetValue("ProductName") as string;
                        return productName;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取操作系统版本时发生错误: " + ex.Message);
            }

            return "未知版本";
        }
    }

}
