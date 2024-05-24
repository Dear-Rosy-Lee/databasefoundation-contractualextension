/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
    /// 发包方模块命令定义
    /// </summary>
    public class SenderCommand
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
        /// 发包方删除命令名称
        /// </summary>
        public const string DelName = "Del";

        /// <summary>
        /// 数据导入命令名称
        /// </summary>
        public const string ImportDataName = "ImportData";

        /// <summary>
        /// 导出Excel命令名称
        /// </summary>
        public const string ExportExcelName = "ExportExcel";

        /// <summary>
        /// 导出Word命令名称
        /// </summary>
        public const string ExportWordName = "ExportWord";

        /// <summary>
        /// 导出Excel模板命令名称
        /// </summary>
        public const string ExcelTemplateName = "ExportExcelTemplate";

        /// <summary>
        /// 导出Word模板命令名称
        /// </summary>
        public const string WordTemplateName = "ExportWordTemplate";

        /// <summary>
        /// 刷新
        /// </summary>
        public const string FreshName = "Fresh";

        /// <summary>
        /// 初始化
        /// </summary>
        public const string InitilizeName = "Initialize";

        /// <summary>
        /// 初始化
        /// </summary>
        public const string CombineData = "CombineData";


        #endregion

        #region Files - Command

        /// <summary>
        /// 发包方添加
        /// </summary>
        public RoutedCommand Add = new RoutedCommand(AddName, typeof(Button));

        /// <summary>
        /// 发包方编辑
        /// </summary>
        public RoutedCommand Edit = new RoutedCommand(EditName, typeof(Button));

        /// <summary>
        /// 发包方删除
        /// </summary>
        public RoutedCommand Del = new RoutedCommand(DelName, typeof(Button));

        /// <summary>
        /// 数据导入
        /// </summary>
        public RoutedCommand ImportData = new RoutedCommand(ImportDataName, typeof(Button));

        /// <summary>
        /// 导出Excel
        /// </summary>
        public RoutedCommand ExportExcel = new RoutedCommand(ExportExcelName, typeof(Button));

        /// <summary>
        /// 导出Word
        /// </summary>
        public RoutedCommand ExportWord = new RoutedCommand(ExportWordName, typeof(Button));

        /// <summary>
        /// 导出Excel模板
        /// </summary>
        public RoutedCommand ExportExcelTemplate = new RoutedCommand(ExcelTemplateName, typeof(Button));

        /// <summary>
        /// 导出Word模板
        /// </summary>
        public RoutedCommand ExportWordTemplate = new RoutedCommand(WordTemplateName, typeof(Button));

        /// <summary>
        /// 刷新
        /// </summary>
        public RoutedCommand Fresh = new RoutedCommand(FreshName, typeof(Button));

        /// <summary>
        /// 初始化
        /// </summary>
        public RoutedCommand Initialize = new RoutedCommand(InitilizeName, typeof(Button));

        public RoutedCommand combinesender = new RoutedCommand(CombineData, typeof(Button));

        #endregion

        #region Files - Binding

        /// <summary>
        /// 发包方添加
        /// </summary>
        public CommandBinding AddBind = new CommandBinding();

        /// <summary>
        /// 发包方编辑
        /// </summary>
        public CommandBinding EditBind = new CommandBinding();

        /// <summary>
        /// 发包方删除
        /// </summary>
        public CommandBinding DelBind = new CommandBinding();

        /// <summary>
        /// 数据导入
        /// </summary>
        public CommandBinding ImportDataBind = new CommandBinding();

        /// <summary>
        /// 导出Excel
        /// </summary>
        public CommandBinding ExportExcelBind = new CommandBinding();

        /// <summary>
        /// 导出Word
        /// </summary>
        public CommandBinding ExportWordBind = new CommandBinding();

        /// <summary>
        /// 导出Excel模板
        /// </summary>
        public CommandBinding ExcelTemplateBind = new CommandBinding();

        /// <summary>
        /// 导出Word模板
        /// </summary>
        public CommandBinding WordTemplateBind = new CommandBinding();

        /// <summary>
        /// 刷新
        /// </summary>
        public CommandBinding FreshBind = new CommandBinding();

        /// <summary>
        /// 初始化
        /// </summary>
        public CommandBinding InitializeBind = new CommandBinding();

        /// <summary>
        /// 合并数据
        /// </summary>
        public CommandBinding CombineSenderData = new CommandBinding();

        #endregion

        #region Properties

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public SenderCommand()
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
            ExcelTemplateBind.Command = ExportExcelTemplate;
            ExportExcelBind.Command = ExportExcel;
            ExportWordBind.Command = ExportWord;
            WordTemplateBind.Command = ExportWordTemplate;
            FreshBind.Command = Fresh;
            InitializeBind.Command = Initialize;
            CombineSenderData.Command = combinesender;
        }

        #endregion
    }
}
