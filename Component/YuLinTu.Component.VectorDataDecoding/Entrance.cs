using Autofac;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using YuLinTu.DF;
using YuLinTu.Windows;
using YuLinTu.DF.DependencyInjection;
using System;
using YuLinTu.DF.Mappers;
using YuLinTu.DF.Uow;
using YuLinTu.DF.Zones;

namespace YuLinTu.Component.VectorDataDecoding
{
    public class Entrance : EntranceBase
    {
        #region Methods

        /// <summary>
        /// 重写注册工作空间方法
        /// </summary>
        protected override void OnConnect()
        {
            RegisterWorkspaceContext<WorkspaceContext>();
           
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


        }

        #endregion Methods
    }
}