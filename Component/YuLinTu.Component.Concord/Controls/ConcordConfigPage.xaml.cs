/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Windows.Controls;
using YuLinTu.Appwork;
using YuLinTu.Library.Business;
using YuLinTu.Windows;

namespace YuLinTu.Component.Concord
{
    /// <summary>
    /// 承包合同系统配置界面
    /// </summary>
    public partial class ConcordConfigPage : WorkpageOptionsEditor
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConcordConfigPage(IWorkpage workpage)
            : base(workpage)
        {
            InitializeComponent();
        }

        #endregion

        #region Fields
        private ContractConcordSettingDefine config;
        private ContractConcordSettingDefine otherDefine;
        private SettingsProfileCenter systemCenter;
        #endregion

        #region Properties
        /// <summary>
        /// 承包经营权其他设置实体属性
        /// </summary>
        public ContractConcordSettingDefine OtherDefine
        {
            get { return otherDefine; }
            set
            {
                otherDefine = value;
                this.DataContext = OtherDefine;
            }
        }
        #endregion

        #region Method-Override

        /// <summary>
        /// 装载
        /// </summary>
        protected override void OnLoad()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
                var profile = systemCenter.GetProfile<ContractConcordSettingDefine>();
                var section = profile.GetSection<ContractConcordSettingDefine>();
                config = (section.Settings as ContractConcordSettingDefine);
                OtherDefine = config.Clone() as ContractConcordSettingDefine;
            }));
            
        }
        protected override void OnActivated()
        {
            cmbChooseArea.SelectedIndex = OtherDefine.ChooseArea;
            cmbBatch.SelectedIndex = otherDefine.ChooseBatch;
            cmbSenderNamePrefixLevel.SelectedIndex = otherDefine.SendNamePrefixLevel;
            cmbContractorNamePrefixLevel.SelectedIndex = otherDefine.ContractorNamePrefixLevel;
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected override void OnSave()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                config.CopyPropertiesFrom(OtherDefine);
                systemCenter.Save<ContractConcordSettingDefine>();
            }));
        }

        #endregion

        #region Events

        #endregion

        private void cmbChooseArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string a = cmbChooseArea.SelectedIndex.ToString();
            if(OtherDefine!=null)
                OtherDefine.ChooseArea = int.Parse(a);
        }

        private void cmbBatch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string a = cmbBatch.SelectedIndex.ToString();
            if (OtherDefine != null)
                OtherDefine.ChooseBatch = int.Parse(a);
        }

        private void cmbSenderNamePrefixLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string a = cmbSenderNamePrefixLevel.SelectedIndex.ToString();
            if (OtherDefine != null)
                OtherDefine.SendNamePrefixLevel = int.Parse(a);
        }

        private void cmbContractorNamePrefixLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string a = cmbContractorNamePrefixLevel.SelectedIndex.ToString();
            if (OtherDefine != null)
                OtherDefine.ContractorNamePrefixLevel = int.Parse(a);
        }
    }
}
