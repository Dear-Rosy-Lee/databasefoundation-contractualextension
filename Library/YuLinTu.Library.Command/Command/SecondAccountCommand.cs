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
    /// 二轮台账模块命令定义
    /// </summary>
    public class SecondAccountCommand
    {
        #region Files - Const

        /// <summary>
        /// 承包方管理命令名称
        /// </summary>
        public const string PersonManageName = "personManage";

        /// <summary>
        /// 承包方编辑命令名称
        /// </summary>
        public const string PersonEditName = "personEdit";

        /// <summary>
        /// 承包方删除命令名称
        /// </summary>
        public const string PersonDeltName = "personDelt";

        /// <summary>
        /// 地块添加命令名称
        /// </summary>
        public const string LandAddName = "landAdd";

        /// <summary>
        /// 地块编辑命令名称
        /// </summary>
        public const string LandEditName = "landEdit";

        /// <summary>
        /// 地块删除命令名称
        /// </summary>
        public const string LandDeltName = "landDelt";

        /// <summary>
        /// 台账调查表数据导入命令名称
        /// </summary>
        public const string ImQueryTblName = "imQueryTbl";

        /// <summary>
        /// 导出二轮台账--摸底调查表命令名称
        /// </summary>
        public const string ExRealQueryTblName = "exRealQueryTbl";

        /// <summary>
        /// 导出二轮台账--摸底调查公示表命令名称
        /// </summary>
        public const string ExPublicityTblName = "exPublicityTbl";

        /// <summary>
        /// 导出二轮台账--摸底调查公示确认表命令名称
        /// </summary>
        public const string ExIdentifyTblName = "exIdentify";

        /// <summary>
        /// 导出二轮台账--用户确认表命令名称
        /// </summary>
        public const string ExUserIdentifyTblName = "exUserIdentifyTbl";

        /// <summary>
        /// 导出勘界确权数据--勘界调查表命令名称
        /// </summary>
        public const string ExUserQueryTblName = "exUserQueryTbl";

        /// <summary>
        /// 导出勘界确权数据--用户调查表命令名称
        /// </summary>
        public const string ExHumphreyQueryTblName = "exHumphreyQueryTbl";

        /// <summary>
        /// 清空数据命令名称
        /// </summary>
        public const string ClearName = "clear";

        #endregion

        #region Files - Command

        /// <summary>
        /// 承包方管理命令名称
        /// </summary>
        public RoutedCommand PersonManage = new RoutedCommand(PersonManageName, typeof(Button));

        /// <summary>
        /// 承包方编辑命令名称
        /// </summary>
        public RoutedCommand PersonEdit = new RoutedCommand(PersonEditName, typeof(Button));

        /// <summary>
        /// 承包方删除命令名称
        /// </summary>
        public RoutedCommand PersonDelt = new RoutedCommand(PersonDeltName, typeof(Button));

        /// <summary>
        /// 地块添加命令名称
        /// </summary>
        public RoutedCommand LandAdd = new RoutedCommand(LandAddName, typeof(Button));

        /// <summary>
        /// 地块编辑命令名称
        /// </summary>
        public RoutedCommand LandEdit = new RoutedCommand(LandEditName, typeof(Button));

        /// <summary>
        /// 地块删除命令名称
        /// </summary>
        public RoutedCommand LandDelt = new RoutedCommand(LandDeltName, typeof(Button));

        /// <summary>
        /// 台账调查表数据导入命令名称
        /// </summary>
        public RoutedCommand ImQueryTbl = new RoutedCommand(ImQueryTblName, typeof(Button));

        /// <summary>
        /// 导出二轮台账--摸底调查表命令名称
        /// </summary>
        public RoutedCommand ExRealQueryTbl = new RoutedCommand(ExRealQueryTblName, typeof(Button));

        /// <summary>
        /// 导出二轮台账--摸底调查公示表命令名称
        /// </summary>
        public RoutedCommand ExPublicityTbl = new RoutedCommand(ExPublicityTblName, typeof(Button));

        /// <summary>
        /// 导出二轮台账--摸底调查公示确认表命令名称
        /// </summary>
        public RoutedCommand ExIdentifyTbl = new RoutedCommand(ExIdentifyTblName, typeof(Button));

        /// <summary>
        /// 导出二轮台账--用户确认表命令名称
        /// </summary>
        public RoutedCommand ExUserIdentifyTbl = new RoutedCommand(ExUserIdentifyTblName, typeof(Button));

        /// <summary>
        /// 导出勘界确权数据--勘界调查表命令名称
        /// </summary>
        public RoutedCommand ExUserQueryTbl = new RoutedCommand(ExUserQueryTblName, typeof(Button));

        /// <summary>
        /// 导出勘界确权数据--用户调查表命令名称
        /// </summary>
        public RoutedCommand ExHumphreyQueryTbl = new RoutedCommand(ExHumphreyQueryTblName, typeof(Button));

        /// <summary>
        /// 清空数据命令名称
        /// </summary>
        public RoutedCommand Clear = new RoutedCommand(ClearName, typeof(Button));

        #endregion

        #region Files - Binding

        /// <summary>
        /// 承包方管理命令名称
        /// </summary>
        public CommandBinding PersonManageBind = new CommandBinding();

        /// <summary>
        /// 承包方编辑命令名称
        /// </summary>
        public CommandBinding PersonEditBind = new CommandBinding();

        /// <summary>
        /// 承包方删除命令名称
        /// </summary>
        public CommandBinding PersonDeltBind = new CommandBinding();

        /// <summary>
        /// 地块添加命令名称
        /// </summary>
        public CommandBinding LandAddBind = new CommandBinding();

        /// <summary>
        /// 地块编辑命令名称
        /// </summary>
        public CommandBinding LandEditBind = new CommandBinding();

        /// <summary>
        /// 地块删除命令名称
        /// </summary>
        public CommandBinding LandDeltBind = new CommandBinding();

        /// <summary>
        /// 台账调查表数据导入命令名称
        /// </summary>
        public CommandBinding ImQueryTblBind = new CommandBinding();

        /// <summary>
        /// 导出二轮台账--摸底调查表命令名称
        /// </summary>
        public CommandBinding ExRealQueryTblBind = new CommandBinding();

        /// <summary>
        /// 导出二轮台账--摸底调查公示表命令名称
        /// </summary>
        public CommandBinding ExPublicityTblBind = new CommandBinding();

        /// <summary>
        /// 导出二轮台账--摸底调查公示确认表命令名称
        /// </summary>
        public CommandBinding ExIdentifyTblBind = new CommandBinding();

        /// <summary>
        /// 导出二轮台账--用户确认表命令名称
        /// </summary>
        public CommandBinding ExUserIdentifyTblBind = new CommandBinding();

        /// <summary>
        /// 导出勘界确权数据--勘界调查表命令名称
        /// </summary>
        public CommandBinding ExUserQueryTblBind = new CommandBinding();

        /// <summary>
        /// 导出勘界确权数据--用户调查表命令名称
        /// </summary>
        public CommandBinding ExHumphreyQueryTblBind = new CommandBinding();

        /// <summary>
        /// 清空数据命令名称
        /// </summary>
        public CommandBinding ClearBind = new CommandBinding();

        #endregion

        #region Properties

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public SecondAccountCommand()
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
            PersonManageBind.Command = PersonManage;
            PersonEditBind.Command = PersonEdit;
            PersonDeltBind.Command = PersonDelt;

            LandAddBind.Command = LandAdd;
            LandEditBind.Command = LandEdit;
            LandDeltBind.Command = LandDelt;

            ImQueryTblBind.Command = ImQueryTbl;

            ExRealQueryTblBind.Command = ExRealQueryTbl;
            ExPublicityTblBind.Command = ExPublicityTbl;
            ExIdentifyTblBind.Command = ExIdentifyTbl;
            ExUserIdentifyTblBind.Command = ExUserIdentifyTbl;

            ExUserQueryTblBind.Command = ExUserQueryTbl;
            ExHumphreyQueryTblBind.Command = ExHumphreyQueryTbl;

            ClearBind.Command = Clear;
       
        }

        #endregion
    }
}
