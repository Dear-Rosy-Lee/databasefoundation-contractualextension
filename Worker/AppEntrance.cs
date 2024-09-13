using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf;
using System.IO;
using System.Reflection;
using Autofac;
using YuLinTu.Library.Business;

namespace YuLinTu.Product.YuLinTuTool
{
    internal static class AppEntrance
    {
        [STAThread]
        private static void Main(string[] args)
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
            //AppShellWpf shell = new AppShellWpf();
            //shell.Run(args);

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            //Data.SQLite.ProviderDbCSQLite.ShutdownAllConnection();
            var builder = new ContainerBuilder();
            var assemblyFiles = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libraries", "libs"), "YuLinTu.Library.*.dll");
            var assemblies = new List<Assembly>();

            foreach (var file in assemblyFiles)
            {
                var assembly = Assembly.LoadFile(file);
                assemblies.Add(Assembly.LoadFrom(file));
                builder.Register(assembly);
            }

            // 注册工作单元拦截器
            builder.RegisterInstance(new UnitOfWorkInterceptor());

            ContainerProvider.Load(builder);

            builder.RegisterBuildCallback(container =>
            {
                //注册 IValidator
                //container.RegisterValidators(assemblies);
            });

            // 注册 GDAL
            //GdalShapefile.Register();

            AppShellWpf shell = new AppShellWpf();
            shell.Run(args);
        }
    }
}