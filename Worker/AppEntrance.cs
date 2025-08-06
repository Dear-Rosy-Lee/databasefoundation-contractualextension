using System;
using System.IO;
using System.Security.Policy;
using System.Windows.Forms;
using Utils.Tool;
using YuLinTu.Library.Business;
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
                var dateTime = DateTime.Now;
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())//网络不可用返回
                {
                    //var tmp = UpdateProgram.GetInternetDate();
                    //dateTime = tmp == null ? dateTime : tmp.Value;
                }
                ToolRegEdit.SetRegProduceTime("YuLinTuLandDelayTool", dateTime);
            }
            catch (Exception ex)
            {
                MessageBox.Show("检查时间出现异常，请修正计算机时间或重新安装程序!" +
                    $"\n----- {ex.Message}-----", "运行提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UpdateProgram.InstallUpdateProgram();
            UpdateProgram.CheckUpdate(false);

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
    }
}

