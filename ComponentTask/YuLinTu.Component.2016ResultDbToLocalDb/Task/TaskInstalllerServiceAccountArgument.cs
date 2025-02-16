/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利
 * http://www.yulintu.com
*/
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.ResultDbToLocalDb
{
    /// <summary>
    /// 成果导入参数类
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


        [DisplayLanguage("导入前清空数据库")]
        [DescriptionLanguage("导入前清空数据库")]
        public bool DelOldData
        {
            get { return _DelOldData; }
            set { _DelOldData = value; NotifyPropertyChanged("DelOldData"); }
        }
        private bool _DelOldData;

        //public bool NeedCheck
        //{
        //    get { return _NeedCheck; }
        //    set { _NeedCheck = value; NotifyPropertyChanged("NeedCheck"); }
        //}
        //private bool _NeedCheck;

        [DisplayLanguage("自动创建数据库")]
        [DescriptionLanguage("自动创建新的数据库,数据将导入到新创建的库中，并设置为数据源")]
        public bool CreatDataBase
        {
            get { return _CreatDataBase; }
            set { _CreatDataBase = value; NotifyPropertyChanged("CreatDataBase"); }
        }
        private bool _CreatDataBase;


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
            CreatDataBase = false;
            _DelOldData = true;
        }

        #endregion

        #region Methods

        #endregion
    }
}
