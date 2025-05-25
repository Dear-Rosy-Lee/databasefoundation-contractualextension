using System;
using System.IO;
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
            UpdateProgram.InstallUpdateProgram();
            UpdateProgram.CheckUpdate(false);
            AppShellWpf shell = new AppShellWpf();
            shell.Run(args);
        }
    }
}