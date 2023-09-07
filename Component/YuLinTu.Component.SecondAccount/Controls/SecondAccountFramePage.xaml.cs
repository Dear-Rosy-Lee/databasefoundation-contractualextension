/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Library.Command;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Log;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.SecondAccount
{
    /// <summary>
    /// 二轮台账管理界面
    /// </summary>
    public partial class SecondAccountFramePage : NavigatableWorkpageFrame
    {
        #region Fields

        /// <summary>
        /// 二轮台账命令
        /// </summary>        
        private SecondAccountCommand command = new SecondAccountCommand();

        /// <summary>
        /// 当前地域
        /// </summary>
        private Zone currentZone;

        #endregion

        #region Properties

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set
            {
                currentZone = value;
                secondAccountPanel.CurrentZone = value;
            }
        }

        /// <summary>
        /// 是否需要授权
        /// </summary>
        public override bool IsNeedAuthenticated
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 初始化
        /// </summary>
        public SecondAccountFramePage()
        {
            InitializeComponent();
            SingleInstance = true;
            NavigatorType = eNavigatorType.TreeView;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化  主要针对二轮台账sencodAccountPanel 数据界面进行设置
        /// </summary>
        protected override void OnInstallComponents()
        {
            base.OnInstallComponents();
            SetCommandToControl();
            secondAccountPanel.ThePage = Workpage;
            //secondAccountPanel.ShowEqualColum = false;
            secondAccountPanel.ShowTaskViewer += () =>
            {
                Workpage.Message.Send(this, new MsgEventArgs(EdCore.langRequestShowTaskViewer));
            };
        }

        /// <summary>
        /// 绑定命令到控件上
        /// </summary>
        private void SetCommandToControl()
        {
            SetCommandBinding(personManageBtn, command.PersonManage, command.PersonManageBind);
            SetCommandBinding(personEditBtn, command.PersonEdit, command.PersonEditBind);
            SetCommandBinding(personDeltBtn, command.PersonDelt, command.PersonDeltBind);

            SetCommandBinding(landAddBtn, command.LandAdd, command.LandAddBind);
            SetCommandBinding(landEditBtn, command.LandEdit, command.LandEditBind);
            SetCommandBinding(landDeltBtn, command.LandDelt, command.LandDeltBind);

            SetCommandBinding(imQueryTblBtn, command.ImQueryTbl, command.ImQueryTblBind);

            SetCommandBinding(exRealQueryTblBtn, command.ExRealQueryTbl, command.ExRealQueryTblBind);
            SetCommandBinding(exPublicityTblBtn, command.ExPublicityTbl, command.ExPublicityTblBind);
            SetCommandBinding(exIdentifyTblBtn, command.ExIdentifyTbl, command.ExIdentifyTblBind);
            SetCommandBinding(exUserIdentifyTblBtn, command.ExUserIdentifyTbl, command.ExUserIdentifyTblBind);

            SetCommandBinding(exUserQueryTblBtn, command.ExUserQueryTbl, command.ExUserQueryTblBind);
            SetCommandBinding(exHumphreyQueryTblBtn, command.ExHumphreyQueryTbl, command.ExHumphreyQueryTblBind);

            SetCommandBinding(clearBtn, command.Clear, command.ClearBind);

        }

        /// <summary>
        /// 创建命令绑定
        /// </summary>
        private void SetCommandBinding(MetroButton button, RoutedCommand cmd, CommandBinding bind)
        {
            bind.CanExecute += CommandBinding_CanExecute;   //注册事件
            bind.Executed += CommandBinding_Executed;
            button.CommandBindings.Add(bind);   //将bind添加到CommandBindings集合上
            button.Command = cmd;  //获取或设置当按此按钮时要调用的命令。
            button.CommandParameter = cmd.Name;  //获取命令名称
        }

        /// <summary>
        /// 是否可以执行命令
        /// </summary>
        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Source != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string parameter = e.Parameter.ToString();
            switch (parameter)
            {
                case SecondAccountCommand.PersonManageName:
                    secondAccountPanel.ManageVirtualPerson();
                    break;
                case SecondAccountCommand.PersonEditName:
                    secondAccountPanel.EditVirtualPerson();
                    break;
                case SecondAccountCommand.PersonDeltName:
                    secondAccountPanel.DeltVirtualPerson();
                    break;
                case SecondAccountCommand.LandAddName:
                    secondAccountPanel.AddLand();
                    break;
                case SecondAccountCommand.LandEditName:
                    secondAccountPanel.EditLand();
                    break;
                case SecondAccountCommand.LandDeltName:
                    secondAccountPanel.DeltLand();
                    break;
                case SecondAccountCommand.ImQueryTblName:
                    secondAccountPanel.ImportQueryTbl();
                    break;
                case SecondAccountCommand.ExRealQueryTblName:
                    secondAccountPanel.ExportRealQueryTbl();
                    break;
                case SecondAccountCommand.ExPublicityTblName:
                    secondAccountPanel.ExportPublicityTbl();
                    break;
                case SecondAccountCommand.ExIdentifyTblName:
                    secondAccountPanel.ExportIdentifyTbl();
                    break;
                case SecondAccountCommand.ExUserIdentifyTblName:
                    secondAccountPanel.ExportUserIdentifyTbl();
                    break;
                case SecondAccountCommand.ExUserQueryTblName:
                    secondAccountPanel.ExportUserQueryTbl();
                    break;
                case SecondAccountCommand.ExHumphreyQueryTblName:
                    secondAccountPanel.ExportHumphreyQueryTbl();
                    break;
                case SecondAccountCommand.ClearName:
                    secondAccountPanel.Clear();
                    break;
            }
        }

        #endregion
    }
}
