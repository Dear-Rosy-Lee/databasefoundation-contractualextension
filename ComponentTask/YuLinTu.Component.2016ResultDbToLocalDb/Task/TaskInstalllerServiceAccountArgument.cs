/*
 * (C) 2014  鱼鳞图公司版权所有,保留所有权利
 * http://www.yulintu.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using YuLinTu.Software;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ResultDbToLocalDb
{
    /// <summary>
    /// 图表检查工具参数类
    /// </summary>
    public class TaskInstalllerServiceAccountArgument : TaskArgument
    {
        #region Properties

        [DisplayLanguage("导入文件路径", IsLanguageName = false)]
        [DescriptionLanguage("导入文件路径", IsLanguageName = false)]
        [PropertyDescriptor(
            Builder = typeof(PropertyDescriptorBuilderFolderBrowser),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/FilePublishToSharePoint.png")]
        public string ImportFilePath
        {
            get { return _ImportFilePath; }
            set { _ImportFilePath = value; NotifyPropertyChanged("ImportFilePath"); }
        }
        private string _ImportFilePath;

        [Enabled(false)]
        [DisplayLanguage("导入地域设置")]
        [DescriptionLanguage("选择要导入数据的地域")]
        //[PropertyDescriptor(
        //    Builder = typeof(PropertyDescriptorBuilderTest),
        //    Trigger = typeof(PropertyTriggerZone),
        //    UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/property.png")]
        public FileZoneEntity FileZone
        {
            get { return _FileZone; }
            set { _FileZone = value; NotifyPropertyChanged("FileZone"); }
        }
        private FileZoneEntity _FileZone = new FileZoneEntity();
        //private FileZoneEntity _FileZone;

        [DisplayLanguage("处理界址点与界址线")]
        [DescriptionLanguage("处理生成界址点与界址线")]
        public bool GenerateCoilDot
        {
            get { return _GenerateCoilDot; }
            set { _GenerateCoilDot = value; NotifyPropertyChanged("GenerateCoilDot"); }
        }
        private bool _GenerateCoilDot;

        [DisplayLanguage("自动创建集体")]
        [DescriptionLanguage("创建集体户,挂接非承包地")]
        public bool CreatUnit
        {
            get { return _CreatUnit; }
            set { _CreatUnit = value; NotifyPropertyChanged("CreatUnit"); }
        }
        private bool _CreatUnit;

        #endregion

        #region Ctor

        public TaskInstalllerServiceAccountArgument()
        {
            //Override = true;
            //ServiceName = "Account";
            //ServicePort = 11000;
            //ClrVersion = eIISCLRVersion.v40;
            //Enable32 = true;
            GenerateCoilDot = false;
            CreatUnit = false;
        }

        #endregion

        #region Methods

        #endregion
    }
}
