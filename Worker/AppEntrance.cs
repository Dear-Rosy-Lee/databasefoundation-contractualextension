using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using YuLinTu.DF;
using YuLinTu.DF.BlobStoring;
using YuLinTu.DF.BlobStoring.FileSystem;
using YuLinTu.DF.Common;
using YuLinTu.DF.DependencyInjection;
using YuLinTu.DF.Files;

using YuLinTu.DF.Mappers;
using YuLinTu.DF.Shapefile.GdalShapefiles;
using YuLinTu.DF.Templates;
using YuLinTu.DF.Uow;
using YuLinTu.DF.Validation;
using YuLinTu.DF.Zones;
using YuLinTu.Windows.Wpf;
using YuLinTu.DF.LandCensus.Zones;
using AutoMapper;

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
            /*
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
            */
            var builder = new ContainerBuilder();
            var assemblyFiles = Directory.GetFiles(PathManager.LibPath, "YuLinTu.DF.Library.*.dll");
            var assemblies = new List<Assembly>();

            foreach (var file in assemblyFiles)
            {
                var assembly = Assembly.LoadFile(file);
                assemblies.Add(Assembly.LoadFrom(file));
                builder.Register(assembly);
            }

            // 注册 IMapper
            builder.RegisterMappers(assemblies);

            // 注册工作单元拦截器
            builder.RegisterInstance(new UnitOfWorkInterceptor());

            builder.RegisterType(typeof(ZoneExtendService))
                .As(typeof(IZoneExtendService));

            var container = builder.Build();
            ContainerProvider.Load(container);
            bool isRegistered = container.IsRegistered<IMapper>();
            Console.WriteLine($"IMapper 注册状态: {isRegistered}"); // 输出检查结果
            // 注册 IValidator
            container.RegisterValidators(assemblies);

            // 配置文件系统Blob
            var fileSetting = VirtualFileSystemSettingDefine.GetInstance();
            var blobConfigProvider = container.Resolve<IBlobContainerConfigurationProvider>();
            var blobConfig = blobConfigProvider.Get(DefaultContainer.Name);
            blobConfig.UseFileSystem(fileSystem => fileSystem.BasePath = fileSetting.SavePath);

            var fileConfigProvider = container.Resolve<IFileContainerConfigurationProvider>();
            var fileConfig = fileConfigProvider.Get(DefaultContainer.Name);
            fileConfig.CopyPropertiesFrom(fileSetting);

            // 备份模板
            //var templateService = container.Resolve<ITemplateService>();
            //templateService.BackUp();

            AppShellWpf shell = new AppShellWpf();
            shell.Run(args);
        }
    }
}