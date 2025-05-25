using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Product.YuLinTuTool
{
    internal static class AppEntrance
    {
        [STAThread]
        private static void Main(string[] args)
        {
            // 设置环境变量
            string variableName = "Path";
            // 获取环境变量
            string value = Environment.GetEnvironmentVariable(variableName);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Libraries\libs");
            string sppath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Libraries\x86");
            if (!value.Contains(path))
            {
                value = value + ";" + path + ";" + sppath;
                Environment.SetEnvironmentVariable(variableName, value);
            }
            AppShellWpf shell = new AppShellWpf();
            shell.Run(args);
        }
    }
}