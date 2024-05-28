using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CombineWordTask
{
    /// <summary>
    /// 合并Word参数
    /// </summary>
    public class TaskCombineWordArgument : TaskArgument
    {
        [DisplayLanguage("源数据路径")]
        [DescriptionLanguage("源数据路径")]
        [PropertyDescriptor(
           Builder = typeof(PropertyDescriptorBuilderFolderBrowser),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string SourceFolder
        {
            get { return _SourceFolder; }
            set { _SourceFolder = value; NotifyPropertyChanged(() => SourceFolder); }
        }
        private string _SourceFolder;

        [DisplayLanguage("数据保存路径")]
        [DescriptionLanguage("数据保存路径")]
        [PropertyDescriptor(
           Builder = typeof(PropertyDescriptorBuilderFolderBrowser),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        public string SaveFolder
        {
            get { return _SaveFolder; }
            set { _SaveFolder = value; NotifyPropertyChanged(() => SaveFolder); }
        }
        private string _SaveFolder;

        [DisplayLanguage("采用源文档格式")]
        [DescriptionLanguage("采用源文档格式")]
        public bool KeepSourceFormat
        {
            get { return _KeepSourceFormat; }
            set { _KeepSourceFormat = value; NotifyPropertyChanged(() => KeepSourceFormat); }
        }
        private bool _KeepSourceFormat = true;


    }
}
