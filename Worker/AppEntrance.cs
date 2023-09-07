using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Forms;

namespace YuLinTu.Product.YuLinTuTool
{
    static class AppEntrance
    {
        [STAThread]
        static void Main(string[] args)
        {
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
            AppShellWpf shell = new AppShellWpf();
            shell.Run(args);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Data.SQLite.ProviderDbCSQLite.ShutdownAllConnection();
        }
     
    }
}

