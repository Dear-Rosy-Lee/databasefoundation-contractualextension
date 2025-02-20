/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace YuLinTu.Library.Command
{
    /// <summary>
    /// 数据字典命令定义
    /// </summary>
    public class DictionaryCommand
    {
        #region Fields-Const

        /// <summary>
        /// ToolBar添加命令名称
        /// </summary>
        public const string AddName = "Add";

        /// <summary>
        /// ToolBar编辑命令名称
        /// </summary>
        public const string EditName = "Edit";

        /// <summary>
        /// ToolBar删除命令名称
        /// </summary>
        public const string DeleteName = "Del";

        /// <summary>
        /// ToolBar清空命令名称
        /// </summary>
        public const string ClearName = "Clear";

        /// <summary>
        /// ToolBar刷新命令名称
        /// </summary>
        public const string RefreshName = "Refresh";

        /// <summary>
        /// LeftSideBar添加命令名称
        /// </summary>
        public const string AddToolName = "AddTool";

        /// <summary>
        /// LeftSideBar编辑命令名称
        /// </summary>
        public const string EditToolName = "EditTool";

        /// <summary>
        /// LeftSideBar删除命令名称
        /// </summary>
        public const string DelToolName = "DelTool";

        /// <summary>
        /// LeftSideBar刷新命令名称
        /// </summary>
        public const string RefreshToolName = "RefreshTool";

        #endregion

        #region Fields-Command

        /// <summary>
        /// 添加属性
        /// </summary>
        public RoutedCommand Add = new RoutedCommand(AddName, typeof(Button));

        /// <summary>
        /// LeftSideBar添加属性
        /// </summary>
        public RoutedCommand AddTool = new RoutedCommand(AddToolName, typeof(Button));

        /// <summary>
        /// 编辑属性
        /// </summary>
        public RoutedCommand Edit = new RoutedCommand(EditName, typeof(Button));

        /// <summary>
        /// LeftSideBar编辑属性
        /// </summary>
        public RoutedCommand EditTool = new RoutedCommand(EditToolName, typeof(Button));

        /// <summary>
        /// 删除属性
        /// </summary>
        public RoutedCommand Del = new RoutedCommand(DeleteName, typeof(Button));

        /// <summary>
        /// LeftSideBar删除属性
        /// </summary>
        public RoutedCommand DelTool = new RoutedCommand(DelToolName, typeof(Button));

        /// <summary>
        /// 清空
        /// </summary>
        public RoutedCommand Clear = new RoutedCommand(ClearName, typeof(Button));

        /// <summary>
        /// 刷新
        /// </summary>
        public RoutedCommand Refresh = new RoutedCommand(RefreshName, typeof(Button));

        /// <summary>
        /// LeftSideBars刷新属性
        /// </summary>
        public RoutedCommand RefreshTool = new RoutedCommand(RefreshToolName, typeof(Button));

        #endregion

        #region Fields-Binding

        /// <summary>
        /// 添加属性
        /// </summary>
        public CommandBinding AddBind = new CommandBinding();

        /// <summary>
        /// LeftSideBar添加属性
        /// </summary>
        public CommandBinding AddToolBind = new CommandBinding();

        /// <summary>
        /// 编辑属性
        /// </summary>
        public CommandBinding EditBind = new CommandBinding();

        /// <summary>
        /// LeftSideBar编辑属性
        /// </summary>
        public CommandBinding EditToolBind = new CommandBinding();

        /// <summary>
        /// 删除属性
        /// </summary>
        public CommandBinding DelBind = new CommandBinding();

        /// <summary>
        /// LeftSideBar删除属性
        /// </summary>
        public CommandBinding DelToolBind = new CommandBinding();

        /// <summary>
        /// 清空
        /// </summary>
        public CommandBinding ClearBind = new CommandBinding();

        /// <summary>
        /// 刷新
        /// </summary>
        public CommandBinding RefreshBind = new CommandBinding();

        /// <summary>
        /// LeftSideBar刷新
        /// </summary>
        public CommandBinding RefreshToolBind = new CommandBinding();

        #endregion

        #region Ctro

        /// <summary>
        /// 构造方法
        /// </summary>
        public DictionaryCommand()
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
            ClearBind.Command = Clear;
            RefreshBind.Command = Refresh;

            AddToolBind.Command = AddTool;
            EditToolBind.Command = EditTool;
            DelToolBind.Command = DelTool;
            RefreshToolBind.Command = RefreshTool;
        }

        #endregion
    }
}
