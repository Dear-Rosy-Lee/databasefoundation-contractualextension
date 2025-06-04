using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using YuLinTu.Appwork;
using YuLinTu.DF.DependencyInjection;
using YuLinTu.DF.Uow;
using YuLinTu.DF;
using YuLinTu.Windows;
using YuLinTu.DF.Mappers;

namespace YuLinTu.Component.VectorDataDecoding
{
    [Workspace(null)]
    public class WorkspaceContext : TheWorkspaceContext
    {
        #region Fields

        #endregion Fields

        #region Ctor

        public WorkspaceContext(IWorkspace workspace)
            : base(workspace)
        {
            var builder = new ContainerBuilder();
            //var assemblyFiles = Directory.GetFiles(PathManager.LibPath, "YuLinTu.DF.Library.*.dll");
            //var assemblies = new List<Assembly>();

            //foreach (var file in assemblyFiles)
            //{
            //    var assembly = Assembly.LoadFile(file);
            //    assemblies.Add(Assembly.LoadFrom(file));
            //    builder.Register(assembly);
            //}

            //// 注册 IMapper
            //builder.RegisterMappers(assemblies);

            //// 注册工作单元拦截器
            //builder.RegisterInstance(new UnitOfWorkInterceptor());



            //var container = builder.Build();
            //ContainerProvider.Load(container);
            //Register<VectorDataDecodePage, WorkpageContext>();
        }

        #endregion Ctor2

        #region method


        ///// <summary>
        ///// 系统管理选项卡
        ///// </summary>
        //[MessageHandler(ID = EdCore.langInstallOptionsEditor)]
        //private void langInstallOptionsEditor(object sender, InstallOptionsEditorEventArgs e)
        //{
        //    Workspace.Window.Dispatcher.Invoke(new Action(() =>
        //    {
        //        e.Editors.Add(new OptionsEditorMetadata()
        //        {
        //            Name = "文件系统",
        //            //Editor = new VirtualFileOptionsEditor(Workspace),
        //        });
        //    }));
        //}

        #endregion method
    }
}