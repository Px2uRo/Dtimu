using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;
using System.Windows;
using System.Threading;

namespace Dtimu.Launcher
{
    internal static class ExplorerHelper
    {
        // Win32 API 导入
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_SHOWMAXIMIZED = 3;
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;

        public static void EmbedExplorer(Window window,FrameworkElement host)
        {
            // 启动 Explorer 进程
            var explorerProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = "C:\\", // 你可以在这里更改路径
                UseShellExecute = true
            });
            Thread.Sleep(7000);
            if (explorerProcess == null || explorerProcess.MainWindowHandle == IntPtr.Zero)
            {
                MessageBox.Show("无法启动 Explorer 进程。");
                return;
            }

            // 获取 WPF 窗口的句柄
            IntPtr wpfWindowHandle = new WindowInteropHelper(window).Handle;

            // 将 Explorer 窗口嵌入到 WPF 窗口中
            SetParent(explorerProcess.MainWindowHandle, wpfWindowHandle);

            // 调整嵌入窗口的大小和位置
            SetWindowPos(explorerProcess.MainWindowHandle, IntPtr.Zero, 0, 0,
                         (int)host.ActualWidth, (int)host.ActualHeight,
                         SWP_NOZORDER | SWP_NOACTIVATE);

            // 显示 Explorer 窗口
            ShowWindow(explorerProcess.MainWindowHandle, SW_SHOWMAXIMIZED);
        }
    }
}

