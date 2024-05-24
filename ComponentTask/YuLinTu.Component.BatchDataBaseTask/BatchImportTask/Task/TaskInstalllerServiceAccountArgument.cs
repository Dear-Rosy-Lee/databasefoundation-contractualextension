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

namespace YuLinTu.Component.BatchDataBaseTask
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
            Builder = typeof(PropertyDescriptorBuilderFileBrowser),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/FilePublishToSharePoint.png")]
        public string ImportFilePath
        {
            get { return _ImportFilePath; }
            set { _ImportFilePath = value; NotifyPropertyChanged("ImportFilePath"); }
        }
        private string _ImportFilePath;

        #endregion

        #region Ctor

        public TaskInstalllerServiceAccountArgument()
        {
        }

        #endregion

        #region Methods

        #endregion
    }
}
