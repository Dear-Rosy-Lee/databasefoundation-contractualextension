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
using YuLinTu.Component.VectorDataDecoding.Repository;
using YuLinTu.Data;
using YuLinTu.DF.Data;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Security;

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
            //builder.RegisterType<DataSource>().As<IDataSource>();
           ;
            builder.RegisterType<ZoneDecRepository>().WithParameter("ds", Db.GetInstance()).As<IZoneRepository>();
            var container = builder.Build();
            ContainerProvider.Load(container);
            using (var scope = container.BeginLifetimeScope())
            {
                // 这将暴露具体缺失的依赖
                var service = scope.Resolve<IZoneRepository>();
                //service.AnyChildren("11000");
            }
            //var builder2 = new ContainerBuilder();
            //builder2.RegisterType<ZoneDecRepository>().As<IZoneRepository>();
            //ContainerProvider.Load("Navigator", builder2.Build());
            Constants.client_id =  new Authenticate().GetApplicationKey();

        }

        #endregion Methods
    }
}