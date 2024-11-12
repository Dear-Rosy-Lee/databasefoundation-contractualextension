using System;
using System.IO;
using System.Windows.Forms;
using Utils.Tool;
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Product.YuLinTuTool
{
    static class AppEntrance
    {
        [STAThread]
        static void Main(string[] args)
        {
            // 设置环境变量
            string variableName = "Path";
            // 获取环境变量
            string value = Environment.GetEnvironmentVariable(variableName);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Libraries\libs");
            //string sppath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Libraries\x86\Spatialite");
            if (!value.Contains(path))
            {
                value = value + ";" + path;
                Environment.SetEnvironmentVariable(variableName, value);
            }
            //string progressName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            //int produceNumber = 0;
            //foreach (System.Diagnostics.Process progress in System.Diagnostics.Process.GetProcesses())
            //{
            //    if (progress.ProcessName != progressName)
            //    {
            //        continue;
            //    }
            //    produceNumber++;
            //}
            //if (produceNumber > 1)
            //{
            //    MessageBox.Show("软件已经在运行!", "运行提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    Application.Exit();
            //    return;
            //}
            try
            {
                ToolRegEdit.SetRegProduceTime("YuLinTuLandDelayTool", DateTime.Now);
                AppShellWpf shell = new AppShellWpf();
                shell.Run(args);
                shell.Shutdown += (s, e) =>
                {
                    ToolRegEdit.SetRegProduceTime("YuLinTuLandDelayTool", DateTime.Now);
                };
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.WaitForFullGCComplete();
                Data.SQLite.ProviderDbCSQLite.ShutdownAllConnection();
            }
            catch
            {
                MessageBox.Show("检查到系统时间不正确，请修正计算机时间再运行程序!", "运行提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

