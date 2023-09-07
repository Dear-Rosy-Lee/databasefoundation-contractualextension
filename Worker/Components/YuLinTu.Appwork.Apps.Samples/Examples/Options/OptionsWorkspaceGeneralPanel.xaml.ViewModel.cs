using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;
using YuLinTu.Windows.Wpf.Metro;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    [View(typeof(OptionsWorkspaceGeneralPanel))]
    public partial class OptionsWorkspaceGeneralPanelViewModel : ViewModelObject, IDisposable
    {
        #region Properties

        #region Properties - Common

        private string _Description = "为了便于演示，在工具栏上新增了一个工作空间选项对话框快捷按钮，\n点击该按钮，即可打开工作空间选项对话框，\n这与通过“文件”-“选项”按钮打开的对话框保持一致";
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged("Description"); }
        }

        #endregion

        #region Properties - System

        public ITheWorkpage Workpage { get; private set; }

        #endregion

        #endregion

        #region Commands

        #region Commands - Loaded

        public DelegateCommand CommandLoaded { get { return _CommandLoaded ?? (_CommandLoaded = new DelegateCommand(obj => OnLoaded(obj))); } }
        private DelegateCommand _CommandLoaded;

        #endregion

        #region Commands - OpenOptionsDialog

        public DelegateCommand CommandOpenOptionsDialog { get { return _CommandOpenOptionsDialog ?? (_CommandOpenOptionsDialog = new DelegateCommand(args => OnOpenOptionsDialog(args), args => OnCanOpenOptionsDialog(args))); } }
        private DelegateCommand _CommandOpenOptionsDialog;

        private bool OnCanOpenOptionsDialog(object args)
        {
            return true;
        }

        private void OnOpenOptionsDialog(object args)
        {
            Workpage.Workspace.Message.Send(this, new MsgEventArgs<string>(
                EdCore.langShowOptionsDialog)
            { Parameter = LanguageAttribute.GetLanguage("lang1030450") });
        }

        #endregion

        #endregion

        #region Ctor

        public OptionsWorkspaceGeneralPanelViewModel(ITheWorkpage workpage) : base(workpage.Message)
        {
            Workpage = workpage;
        }

        #endregion

        #region Methods

        #region Methods - Public

        #endregion

        #region Methods - Override

        protected override void OnStartup()
        {
            base.OnStartup();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        #endregion

        #region Methods - Private

        private void OnLoaded(object obj)
        {
            // 一般不使用该方法处理页面的加载过程，请使用 Message 文件中的
            // InstallWorkpageContent 初始化界面组件，
            // InstallAccountData 初始化用户数据。
        }

        #endregion

        #region Methods - System

        public void Dispose()
        {
            Workpage = null;
        }

        #endregion

        #endregion
    }
}
