/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf.Metro.Components;
using YuLinTu.Library.Entity;
using YuLinTu.Component.Common;
using YuLinTu.Windows;

namespace YuLinTu.Component.DataCheckerTask
{
    /// <summary>
    /// 检查农村土地调查数据库数据任务参数
    /// </summary>
    public class TaskDataCheckerArgument : TaskArgument
    {
        #region Fields

        private string zoneNameAndCode;   //当前地域名称+编码
        private bool isCheckCardNumber;   //是否检查证件号码
        private bool isCheckCBF;
        private bool isCheckFBF;
        private bool isCheckLand;
        private bool isCheckHT;
        private SettingsProfileCenter center;
        public DataCheckerSetDefine dataCheckerSetDefine;

        #endregion

        #region Properties

        [DisplayLanguage("行政地域")]
        [DescriptionLanguage("请选择行政地域编码")]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderSelectedZoneTextBox),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/globe.png")]
        public string ZoneNameAndCode
        {
            get { return zoneNameAndCode; }
            set
            {
                zoneNameAndCode = value;
                dataCheckerSetDefine.ZoneNameAndCode = zoneNameAndCode;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("ZoneNameAndCode");
            }
        }

        [DisplayLanguage("检查证件号码重复")]
        [DescriptionLanguage("是否检查证件号码重复")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool IsCheckCardNumber
        {
            get { return isCheckCardNumber; }
            set { isCheckCardNumber = value;
                dataCheckerSetDefine.IsCheckCardNumber = isCheckCardNumber;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("IsCheckCardNumber"); }
        }
        [DisplayLanguage("是否检查发包方")]
        [DescriptionLanguage("是否检查发包方")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
                    UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool IsCheckFBF
        {
            get { return isCheckFBF; }
            set { isCheckFBF = value;
                dataCheckerSetDefine.IsCheckFBF = isCheckFBF;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("IsCheckFBF"); }
        }
        [DisplayLanguage("是否检查承包方")]
        [DescriptionLanguage("是否检查承包方")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
                    UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool IsCheckCBF
        {
            get { return isCheckCBF; }
            set { isCheckCBF = value;
                dataCheckerSetDefine.IsCheckCBF = isCheckCBF;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("IsCheckCBF"); }
        }
        [DisplayLanguage("是否检查承包地块")]
        [DescriptionLanguage("是否检查承包地块")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
                   UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool IsCheckLand
        {
            get { return isCheckLand; }
            set { isCheckLand = value;
                dataCheckerSetDefine.IsCheckLand = isCheckLand;
                SaveExportLastResSetDefine();
                NotifyPropertyChanged("IsCheckLand"); }
        }

        [DisplayLanguage("是否检查合同")]
        [DescriptionLanguage("是否检查合同")]
        [PropertyDescriptor(Builder = typeof(PropertyBuilderCheckCardBoolean),
                   UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public bool IsCheckHT
        {
            get { return isCheckHT; }
            set { isCheckHT = value;
                dataCheckerSetDefine.IsCheckHT = isCheckHT;
                SaveExportLastResSetDefine();

                NotifyPropertyChanged("IsCheckHT"); }
        }
        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskDataCheckerArgument()
        {

            //读出配置
            center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<DataCheckerSetDefine>();
            var section = profile.GetSection<DataCheckerSetDefine>();
            dataCheckerSetDefine = (section.Settings as DataCheckerSetDefine);

            ZoneNameAndCode = dataCheckerSetDefine.ZoneNameAndCode;
            IsCheckCardNumber = dataCheckerSetDefine.IsCheckCardNumber;
            IsCheckCBF = dataCheckerSetDefine.IsCheckCBF;
            IsCheckFBF = dataCheckerSetDefine.IsCheckFBF;
            IsCheckHT = dataCheckerSetDefine.IsCheckHT;
            IsCheckLand = dataCheckerSetDefine.IsCheckLand;
        }

        #endregion

        #region Method
        private void SaveExportLastResSetDefine()
        {
            center.Save<DataCheckerSetDefine>();
        }
        #endregion
    }
}
