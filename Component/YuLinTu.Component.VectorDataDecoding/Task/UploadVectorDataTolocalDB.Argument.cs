using AutoMapper.Configuration.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu;
using YuLinTu.DF.Controls.Builders;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    public class UploadVectorDataTolocalDBArgument : TaskArgument
    {
        #region Fields

        private string shapeFilePath;
        private string resultFilePath;
        private string userName;
        private string zoneCode;
        private int? landCount;
        private string checkInfo;
        private string batchCode;
        #endregion Fields

        #region Properties

        [DisplayLanguage("处理批次编码", IsLanguageName = false)]
        [DescriptionLanguage("待处理批次号", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBox),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]

        public string BatchCode
        {
            get { return batchCode; }
            set
            {
                batchCode = value;
                NotifyPropertyChanged("BatchCode");
            }
        }


        [DisplayLanguage("矢量数据路径", IsLanguageName = false)]
        [DescriptionLanguage("待处理矢量数据文件的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFileBrowserShp),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]
        [WatermaskLanguage("请选择.shp矢量文件路径")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "必填")]
        public string ShapeFilePath
        {
            get { return shapeFilePath; }
            set
            {
                shapeFilePath = value;                
                NotifyPropertyChanged("ShapeFilePath");
                CheckShpFile(shapeFilePath);
            }
        }

       
        [DisplayLanguage("地块数量", IsLanguageName = false)]
        [DescriptionLanguage("待处理矢量数据地块数量", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBox),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]

        public int? LandCount
        {
            get { return landCount; }
            set
            {
                landCount = value;
                NotifyPropertyChanged("LandCount");
            }
        }

        [DisplayLanguage("初检信息", IsLanguageName = false)]
        [DescriptionLanguage("对数据进行初步检查并提示", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderMultiLineReadOnlyTextBox),
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

        [Enabled(false)]
   
        public bool ConfirmEnabled
        {
            get { return checkPass; }
            set
            {
                checkPass = value;
                NotifyPropertyChanged("ConfirmEnabled");
            }
        }
        public bool checkPass;



        #endregion Properties

        private  void CheckShpFile(string shapeFilePath)
        {
            bool check = false;int? count= null;
            var error=VectorDataProgress.CheckShpFile(shapeFilePath,out check, out count);
            if (error.Count == 0) error.Add("未发现矢量数据错误，检查通过！");
            error.ForEach(t =>
            {
                CheckInfo += t + "\n";
            });
            ConfirmEnabled = check; LandCount = count;
        }

        #region Ctor

        public UploadVectorDataTolocalDBArgument()
        {
        }

        #endregion
    }
}
