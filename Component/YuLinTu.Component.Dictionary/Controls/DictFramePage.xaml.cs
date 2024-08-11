/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利 
 */
using System.Windows.Input;
using YuLinTu.Appwork;
using YuLinTu.Library.Command;
using YuLinTu.Windows.Wpf.Metro;

namespace YuLinTu.Component.Dictionary
{
    /// <summary>
    /// 数据字典主界面
    /// </summary>
    [Newable(true,
        Order = 0,
        IsLanguageName = false,
        Name = "数据字典",
        Description = "属性值代码管理",
        Category = "应用",
        Icon = "pack://application:,,,/YuLinTu.Component.Dictionary;component/Resources/Dictionary.png",
        Image = "pack://application:,,,/YuLinTu.Resources;component/Images/Galleries/Apps/Dictionary78.png",
        IsCreateShortcut = true,
        FontIcon = "\uf233",
        IsNeedAuthenticated = false)]
    public partial class DictFramePage : WorkpageFrame
    {
        #region Fields

        /// <summary>
        /// 命令集合
        /// </summary>
        private DictionaryCommand command;

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
        /// 构造函数:初始化数据字典窗体
        /// </summary>
        public DictFramePage()
        {
            InitializeComponent();
            SingleInstance = true;
            command = new DictionaryCommand();
        }

        #endregion

        #region methods

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void OnInstallComponents()
        {
            SetCommandToControl();
            dictManagerPanel.ThePage = Workpage;
            dictListPage.ListPageWorkPage = Workpage;
            dictListPage.SetDataGrid += dictManagerPanel.SetDataGridSource;
        }

        /// <summary>
        /// 绑定命令到控件上
        /// </summary>
        private void SetCommandToControl()
        {
            SetCommandBinding(mbtnAdd, command.Add, command.AddBind);
            SetCommandBinding(mbtnEdit, command.Edit, command.EditBind);
            SetCommandBinding(mbtnDel, command.Del, command.DelBind);
            SetCommandBinding(mbtnClear, command.Clear, command.ClearBind);
            SetCommandBinding(mbtnRefresh, command.Refresh, command.RefreshBind);

            SetCommandBinding(mbtnAddTool, command.AddTool, command.AddToolBind);
            SetCommandBinding(mbtnEditTool, command.EditTool, command.EditToolBind);
            SetCommandBinding(mbtnDelTool, command.DelTool, command.DelToolBind);
            SetCommandBinding(mbtnRefreshTool, command.RefreshTool, command.RefreshToolBind);
        }

        /// <summary>
        /// 创建命令绑定
        /// </summary>
        private void SetCommandBinding(MetroButton button, RoutedCommand cmd, CommandBinding bind)
        {
            bind.CanExecute += CommandBinding_CanExecute;
            bind.Executed += CommandBinding_Executed;
            button.CommandBindings.Add(bind);
            button.Command = cmd;
            button.CommandParameter = cmd.Name;
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
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (dictManagerPanel == null)
            {
                return;
            }
            string parameter = e.Parameter.ToString();
            switch (parameter)
            {
                case DictionaryCommand.AddName:
                    dictManagerPanel.Add();
                    break;
                case DictionaryCommand.EditName:
                    dictManagerPanel.Edit();
                    break;
                case DictionaryCommand.DeleteName:
                    dictManagerPanel.Delete();
                    break;
                case DictionaryCommand.ClearName:
                    dictManagerPanel.Clear();
                    break;
                case DictionaryCommand.RefreshName:
                    dictManagerPanel.Refresh();
                    break;
                case DictionaryCommand.AddToolName:
                    dictListPage.AddTool();
                    break;
                case DictionaryCommand.EditToolName:
                    dictListPage.EditTool();
                    break;
                case DictionaryCommand.DelToolName:
                    dictListPage.DelTool();
                    break;
                case DictionaryCommand.RefreshToolName:
                    dictListPage.RefreshTool();
                    break;
            }
        }

        #endregion
    }
}
