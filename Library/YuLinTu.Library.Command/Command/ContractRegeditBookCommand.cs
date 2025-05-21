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
    public class ContractRegeditBookCommand
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
        /// 权证删除命令名称
        /// </summary>
        public const string DelName = "Del";

        /// <summary>
        /// 权证登记命令名称
        /// </summary>
        public const string InitalizeDataName = "InitalizeData";

        /// <summary>
        /// 登记簿设置命令名称
        /// </summary>
        public const string BookSettingName = "BookSetting";

        /// <summary>
        /// 预览登记簿命令名称
        /// </summary>
        public const string PrivewRegeditBookName = "PrivewRegeditBook";

        /// <summary>
        /// 导出登记簿命令名称
        /// </summary>
        public const string ExportRegeditBookName = "ExportRegeditBook";

        ///<summary>
        ///打印登记簿命令名称
        /// </summary>
        public const string PrintRegeditBookName = "PrintRegeditBook";

        ///<summary>
        ///打印设置命令名称
        /// </summary>
        public const string PrintSettingName = "PrintSetting";

        ///<summary>
        ///预览证书命令名称
        /// </summary>
        public const string PrintViewWarrantName = "PrintViewWarrant";

        ///<summary>
        ///导出证书命令名称
        /// </summary>
        public const string ExportWarrantName = "ExportWarrant";

        ///<summary>
        ///打印证书命令名称
        /// </summary>
        //public const string PrintWarrantName = "PrintWarrant";

        ///<summary>
        ///数据汇总表命令名称
        /// </summary>
        public const string ExportWarrantSummeryTableName = "ExportWarrantSummeryTable";

        ///<summary>
        ///单户确认表命令名称
        /// </summary>
        public const string ExportFamilyTableName = "ExportFamilyTable";

        ///<summary>
        ///颁证清册命令名称
        /// </summary>
        public const string ExportAwareTableName = "ExportAwareTable";

        /// <summary>
        /// 清理命令名称
        /// </summary>
        public const string ClearName = "Clear";

        /// <summary>
        /// 权证刷新
        /// </summary>
        public const string RefreshName = "Refresh";

        /// <summary>
        /// 重置流水号
        /// </summary>
        public const string ResetSerialNumberName = "ResetSerialNumber";

        public const string RelationSerialNumberName = "RelationSerialNumber";

        #endregion

        #region Files - Command

        /// <summary>
        /// 权证添加
        /// </summary>
        public RoutedCommand Add = new RoutedCommand(AddName, typeof(Button));

        /// <summary>
        /// 权证编辑
        /// </summary>
        public RoutedCommand Edit = new RoutedCommand(EditName, typeof(Button));

        /// <summary>
        /// 权证删除
        /// </summary>
        public RoutedCommand Del = new RoutedCommand(DelName, typeof(Button));

        /// <summary>
        /// 权证登记
        /// </summary>       
        public RoutedCommand InitalizeData = new RoutedCommand(InitalizeDataName, typeof(Button));

        /// <summary>
        /// 登记簿设置命令名称
        /// </summary>        
        public RoutedCommand BookSetting = new RoutedCommand(BookSettingName, typeof(Button));

        /// <summary>
        /// 预览登记簿命令名称
        /// </summary>
        public RoutedCommand PrivewRegeditBook = new RoutedCommand(PrivewRegeditBookName, typeof(Button));

        /// <summary>
        /// 导出登记簿命令名称
        /// </summary> 
        public RoutedCommand ExportRegeditBook = new RoutedCommand(ExportRegeditBookName, typeof(Button));

        ///<summary>
        ///打印登记簿命令名称
        /// </summary>       
        public RoutedCommand PrintRegeditBook = new RoutedCommand(PrintRegeditBookName, typeof(Button));

        ///<summary>
        ///打印设置命令名称
        /// </summary>      
        public RoutedCommand PrintSetting = new RoutedCommand(PrintSettingName, typeof(Button));

        ///<summary>
        ///预览证书命令名称
        /// </summary>
        public RoutedCommand PrintViewWarrant = new RoutedCommand(PrintViewWarrantName, typeof(Button));

        ///<summary>
        ///导出证书命令名称
        /// </summary>      
        public RoutedCommand ExportWarrant = new RoutedCommand(ExportWarrantName, typeof(Button));

        ///<summary>
        ///打印证书命令名称
        /// </summary>   
        //public RoutedCommand PrintWarrant = new RoutedCommand(PrintWarrantName, typeof(Button));

        ///<summary>
        ///数据汇总表命令名称
        /// </summary>
        public RoutedCommand ExportWarrantSummeryTable = new RoutedCommand(ExportWarrantSummeryTableName, typeof(Button));

        ///<summary>
        ///单户确认表命令名称
        /// </summary>
        public RoutedCommand ExportFamilyTable = new RoutedCommand(ExportFamilyTableName, typeof(Button));

        ///<summary>
        ///颁证清册命令名称
        /// </summary>
        public RoutedCommand ExportAwareTable = new RoutedCommand(ExportAwareTableName, typeof(Button));

        /// <summary>
        /// 清理
        /// </summary>
        public RoutedCommand Clear = new RoutedCommand(ClearName, typeof(Button));

        /// <summary>
        /// 权证刷新
        /// </summary>
        public RoutedCommand Refresh = new RoutedCommand(RefreshName, typeof(Button));

        /// <summary>
        /// 重置流水号
        /// </summary>
        public RoutedCommand ResetSerialNumber = new RoutedCommand(ResetSerialNumberName, typeof(Button));

        /// <summary>
        /// 流水号关联
        /// </summary>
        public RoutedCommand RelationSerialNumber = new RoutedCommand(RelationSerialNumberName, typeof(Button));

        #endregion

        #region Files - Binding

        /// <summary>
        /// 权证添加
        /// </summary>
        public CommandBinding AddBind = new CommandBinding();

        /// <summary>
        /// 权证编辑
        /// </summary>
        public CommandBinding EditBind = new CommandBinding();

        /// <summary>
        /// 权证删除
        /// </summary>
        public CommandBinding DelBind = new CommandBinding();

        /// <summary>
        /// 权证登记命令名称
        /// </summary>
        public CommandBinding InitalizeDataBind = new CommandBinding();

        /// <summary>
        /// 登记簿设置命令名称
        /// </summary>
        public CommandBinding BookSettingBind = new CommandBinding();

        /// <summary>
        /// 预览登记簿命令名称
        /// </summary>
        public CommandBinding PrivewRegeditBookBind = new CommandBinding();

        /// <summary>
        /// 导出登记簿命令名称
        /// </summary>
        public CommandBinding ExportRegeditBookBind = new CommandBinding();

        ///<summary>
        ///打印登记簿命令名称
        /// </summary>
        public CommandBinding PrintRegeditBookBind = new CommandBinding();

        ///<summary>
        ///打印设置命令名称
        /// </summary>
        public CommandBinding PrintSettingBind = new CommandBinding();

        ///<summary>
        ///预览证书命令名称
        /// </summary>
        public CommandBinding PrintViewWarrantBind = new CommandBinding();

        ///<summary>
        ///导出证书命令名称
        /// </summary>
        public CommandBinding ExportWarrantBind = new CommandBinding();

        ///<summary>
        ///打印证书命令名称
        /// </summary>
        //public CommandBinding PrintWarrantBind = new CommandBinding();

        ///<summary>
        ///数据汇总表命令名称
        /// </summary>
        public CommandBinding ExportWarrantSummeryTableBind = new CommandBinding();

        ///<summary>
        ///单户确认表命令名称
        /// </summary>
        public CommandBinding ExportFamilyTableBind = new CommandBinding();

        ///<summary>
        ///颁证清册命令名称
        /// </summary>
        public CommandBinding ExportAwareTableBind = new CommandBinding();

        /// <summary>
        /// 清理
        /// </summary>
        public CommandBinding ClearBind = new CommandBinding();

        /// <summary>
        /// 权证刷新
        /// </summary>
        public CommandBinding RefreshBind = new CommandBinding();

        /// <summary>
        /// 重置流水号
        /// </summary>
        public CommandBinding ResetSerialNumberBinding = new CommandBinding();

        /// <summary>
        /// 重置流水号
        /// </summary>
        public CommandBinding RelationSerialNumberBinding = new CommandBinding();
        #endregion

        #region Properties

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ContractRegeditBookCommand()
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
            InitalizeDataBind.Command = InitalizeData;
            BookSettingBind.Command = BookSetting;
            PrivewRegeditBookBind.Command = PrivewRegeditBook;
            ExportRegeditBookBind.Command = ExportRegeditBook;
            PrintRegeditBookBind.Command = PrintRegeditBook;
            PrintSettingBind.Command = PrintSetting;
            PrintViewWarrantBind.Command = PrintViewWarrant;
            ExportWarrantBind.Command = ExportWarrant;
            //PrintWarrantBind.Command = PrintWarrant;
            ExportWarrantSummeryTableBind.Command = ExportWarrantSummeryTable;
            ExportFamilyTableBind.Command = ExportFamilyTable;
            ExportAwareTableBind.Command = ExportAwareTable;
            ClearBind.Command = Clear;
            RefreshBind.Command = Refresh;
            ResetSerialNumberBinding.Command = ResetSerialNumber;
            RelationSerialNumberBinding.Command = RelationSerialNumber;
        }
        #endregion
    }
}
