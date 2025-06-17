using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    public class UploadVectorDataAfterDecodeArgument : TaskArgument
    {
        #region Properties

        [DisplayLanguage("脱密数据文件夹路径", IsLanguageName = false)]
        [DescriptionLanguage("存放脱密数据文件夹的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFolderBrowserExtsion),
      UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        [WatermaskLanguage("请选择脱密数据文件夹路径")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "必填")]
        public string ResultFilePath
        {
            get { return resultFilePath; }
            set
            {
                resultFilePath = value;
                //qualityCompressionDataSetDefine.ResultFilePath = ResultFilePath;
                NotifyPropertyChanged("ResultFilePath");
                CheckShpFile(resultFilePath);
            }
        }
        //public event EventHandler<string> LogPathChanged;


        private string resultFilePath;
        [DisplayLanguage("数据量", IsLanguageName = false)]
        [DescriptionLanguage("矢量文件个数及地块总数量", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBoxCustom),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]

        public string DataCount
        {
            get { return _DataCount; }
            set
            {
                _DataCount = value;
                NotifyPropertyChanged("DataCount");
            }
        }
        private string _DataCount;

        [DisplayLanguage("数据信息", IsLanguageName = false)]
        [DescriptionLanguage("对数据进行初步检查并提示", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderMultiLineReadOnlyTextBoxCustom),
          UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]
        [WatermaskLanguage("对数据进行初步检查并提示")]
        public string CheckInfo
        {
            get { return checkInfo; }
            set
            {
                checkInfo = value;
                NotifyPropertyChanged("CheckInfo");

            }
        }
        private string checkInfo;

        [Enabled(false)]
        public List<ShpFileDescription> ShpFilesInfo;
        #endregion

        #region Ctor

        public UploadVectorDataAfterDecodeArgument()
        {
        
        }

        #endregion

        private async System.Threading.Tasks.Task CheckShpFile(string resultFilePathP)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                var shps = ShpFolderDescription.GetFilesByExtensionLegacy(resultFilePathP);

                int fileCount = 0; int dataCount = 0;
                ShpFilesInfo = new List<ShpFileDescription>();
                foreach (var shp in shps)
                {
                    var shpInfo = ShpFolderDescription.GetShpFileDescription(shp);
                    fileCount++; dataCount += shpInfo.DataCount;
                    DataCount = string.Format("共{0}个矢量文件个数,{1}个地块", fileCount, dataCount);
                    CheckInfo = CheckInfo + shp.ReplaceFirst(ResultFilePath, ".") + "  地块数量：" + shpInfo.DataCount + "  " + shpInfo.Description + "\n";
                    ShpFilesInfo.Add(shpInfo);
                }
            });

        }
    }
}
