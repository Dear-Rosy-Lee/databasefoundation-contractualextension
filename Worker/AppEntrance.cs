using System;
using System.IO;
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
using AutoMapper;
using Autofac;
using System.Reflection;
using System.Collections.Generic;
using OSGeo.OGR;

namespace YuLinTu.Product.YuLinTuTool
{
    internal static class AppEntrance
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Registerdll();
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

        /// <summary>
        /// 注册Ogr驱动
        /// </summary>
        public static bool Registerdll()
        {
            try
            {
              
                Ogr.RegisterAll();
                //OSGeo.GDAL.Gdal.AllRegister(); 支持栅格数据
                // 为了支持中文路径，请添加下面这句代码
                OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
                // 为了使属性表字段支持中文，请添加下面这句
                OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "CP936");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}