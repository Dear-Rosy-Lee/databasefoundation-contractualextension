using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Component.Common;
using System.ComponentModel.DataAnnotations;

namespace YuLinTu.Component.BatchDataBaseTask
{
    public class TaskBuildExportResultDataBaseArgument : TaskArgument
    {
        private SettingsProfileCenter center;

        public ExportLastResSetDefine ExportLastResSettingDefine;

        #region Properties

        [DisplayLanguage("行政地域")]
        [DescriptionLanguage("行政地域")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderSelectedZoneTextBox),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/globe.png")]
        public string SelectZoneName
        {
            get { return _SelectZoneName; }
            set
            {
                _SelectZoneName = value.TrimSafe();
                ExportLastResSettingDefine.SelectZoneName = _SelectZoneName;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("SelectZoneName");
            }
        }
        private string _SelectZoneName;

        [DisplayLanguage("数据保存路径")]
        [DescriptionLanguage("数据保存路径")]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderFolderBrowser),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string SaveFolderName
        {
            get { return _SaveFolderName; }
            set
            {
                _SaveFolderName = value;
                ExportLastResSettingDefine.SaveFolderName = _SaveFolderName;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("SaveFolderName");
            }
        }
        private string _SaveFolderName;

        #endregion

        #region Ctor

        public TaskBuildExportResultDataBaseArgument()
        {

            //读出配置
            center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<ExportLastResSetDefine>();
            var section = profile.GetSection<ExportLastResSetDefine>();
            ExportLastResSettingDefine = (section.Settings as ExportLastResSetDefine);

            SelectZoneName = ExportLastResSettingDefine.SelectZoneName;
            SaveFolderName = ExportLastResSettingDefine.SaveFolderName;
        }

        #endregion

        #region Methods

        private void SaveExportLastResSetDefine()
        {
            center.Save<ExportLastResSetDefine>();
        }

        #endregion
    }
    
    /// <summary>
    /// 导出MDB库cbdkxx表确权(合同)面积下拉选项，选择以哪种面积导出
    /// </summary>
    public enum CbdkxxAwareAreaExportEnum
    {
        /// <summary>
        /// 确权面积
        /// </summary>
        确权面积 = 0,

        /// <summary>
        /// 实测面积
        /// </summary>
        实测面积 =1,
        
    }
}
