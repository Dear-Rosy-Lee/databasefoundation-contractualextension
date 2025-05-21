/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace YuLinTu.Library.Command
{
    /// <summary>
    /// 地域模块命令定义
    /// </summary>
    public class ZoneCommand
    {
        #region Files - Const

        /// <summary>
        /// 添加命令名称
        /// </summary>
        public const string AddName = "Add";

        /// <summary>
        /// 编辑命令名称
        /// </summary>
        public const string EditName = "Edit";

        /// <summary>
        /// 地域删除命令名称
        /// </summary>
        public const string DelName = "Del";

        /// <summary>
        /// 数据导入命令名称
        /// </summary>
        public const string ImportDataName = "ImportData";

        /// <summary>
        /// 图斑导入命令名称
        /// </summary>
        public const string ImportShapeName = "ImportShape";

        /// <summary>
        /// 数据导出命令名称
        /// </summary>
        public const string ExportDataName = "ExportData";

        /// <summary>
        /// 图斑导出命令名称
        /// </summary>
        public const string ExportShapeName = "ExportShape";

        /// <summary>
        /// 压缩包导出命令名称
        /// </summary>
        public const string ExportPackageName = "ExportPackage";

        /// <summary>
        /// 清理命令名称
        /// </summary>
        public const string ClearName = "Clear";

        /// <summary>
        /// 地域刷新
        /// </summary>
        public const string RefreshName = "Refresh";
        
        /// <summary>
        /// 上传至服务
        /// </summary>
        public const string UpToServiceName = "UpToService";

        #endregion

        #region Files - Command

        /// <summary>
        /// 上传至服务
        /// </summary>
        public RoutedCommand UpToService = new RoutedCommand(UpToServiceName, typeof(Button));

        /// <summary>
        /// 地域添加
        /// </summary>
        public RoutedCommand Add = new RoutedCommand(AddName, typeof(Button));

        /// <summary>
        /// 地域编辑
        /// </summary>
        public RoutedCommand Edit = new RoutedCommand(EditName, typeof(Button));

        /// <summary>
        /// 地域删除
        /// </summary>
        public RoutedCommand Del = new RoutedCommand(DelName, typeof(Button));

        /// <summary>
        /// 数据导入
        /// </summary>
        public RoutedCommand ImportData = new RoutedCommand(ImportDataName, typeof(Button));

        /// <summary>
        /// 图斑导入
        /// </summary>
        public RoutedCommand ImportShape = new RoutedCommand(ImportShapeName, typeof(Button));

        /// <summary>
        /// 数据导出
        /// </summary>
        public RoutedCommand ExportData = new RoutedCommand(ExportDataName, typeof(Button));

        /// <summary>
        /// 图斑导出
        /// </summary>
        public RoutedCommand ExportShape = new RoutedCommand(ExportShapeName, typeof(Button));

        /// <summary>
        /// 压缩包导出
        /// </summary>
        public RoutedCommand ExportPackage = new RoutedCommand(ExportPackageName, typeof(Button));

        /// <summary>
        /// 清理
        /// </summary>
        public RoutedCommand Clear = new RoutedCommand(ClearName, typeof(Button));

        /// <summary>
        /// 地域刷新
        /// </summary>
        public RoutedCommand Refresh = new RoutedCommand(RefreshName, typeof(Button));

        #endregion

        #region Files - Binding
        
        /// <summary>
        /// 上传至服务
        /// </summary>
        public CommandBinding  UpToServiceBind= new CommandBinding();

        /// <summary>
        /// 地域添加
        /// </summary>
        public CommandBinding AddBind = new CommandBinding();

        /// <summary>
        /// 地域编辑
        /// </summary>
        public CommandBinding EditBind = new CommandBinding();

        /// <summary>
        /// 地域删除
        /// </summary>
        public CommandBinding DelBind = new CommandBinding();

        /// <summary>
        /// 数据导入
        /// </summary>
        public CommandBinding ImportDataBind = new CommandBinding();

        /// <summary>
        /// 图斑导入
        /// </summary>
        public CommandBinding ImportShapeBind = new CommandBinding();

        /// <summary>
        /// 数据导出
        /// </summary>
        public CommandBinding ExportDataBind = new CommandBinding();

        /// <summary>
        /// 图斑导出
        /// </summary>
        public CommandBinding ExportShapeBind = new CommandBinding();

        /// <summary>
        /// 压缩包导出
        /// </summary>
        public CommandBinding ExportPackageBind = new CommandBinding();

        /// <summary>
        /// 清理
        /// </summary>
        public CommandBinding ClearBind = new CommandBinding();

        /// <summary>
        /// 地域刷新
        /// </summary>
        public CommandBinding RefreshBind = new CommandBinding();

        #endregion

        #region Properties

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ZoneCommand()
        {
            InstallCommand();
        }

        #endregion

        #region Install

        /// <summary>
        /// 将命令设置到绑定上
        /// </summary>
        public void InstallCommand()
        {
            AddBind.Command = Add;
            EditBind.Command = Edit;
            DelBind.Command = Del;
            ImportDataBind.Command = ImportData;
            ImportShapeBind.Command = ImportShape;
            ExportDataBind.Command = ExportData;
            ExportShapeBind.Command = ExportShape;
            ExportPackageBind.Command = ExportPackage;
            ClearBind.Command = Clear;
            RefreshBind.Command = Refresh;
            UpToServiceBind.Command = UpToService;
        }

        #endregion
    }
}
