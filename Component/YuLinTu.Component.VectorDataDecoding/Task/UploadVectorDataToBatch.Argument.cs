using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.DF;
using YuLinTu.Web;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    public class UploadVectorDataToBatchArgument : TaskArgument
    {
        #region Fields

        private string shapeFilePath;

        private string userName;
   
        private int? landCount;
        private string checkInfo;
        private string batchCode;
        #endregion Fields

       
        [DisplayLanguage("批次名称", IsLanguageName = false)]
        [DescriptionLanguage("待处理批次名称", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBox),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/16/objectsendbackward.png")]

        public string BatchName
        {
            get { return _BatchName; }
            set
            {
                _BatchName = value;
                NotifyPropertyChanged("BatchName");
            }
        }
        private string _BatchName;


        [DisplayLanguage("批次编码", IsLanguageName = false)]
        [DescriptionLanguage("待处理批次号", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBox),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/16/objectsendbackward.png")]

        public string BatchCode
        {
            get { return batchCode; }
            set
            {
                batchCode = value;
                NotifyPropertyChanged("BatchCode");
            }
        }

        private string zoneCode;

        [DisplayLanguage("地域编码", IsLanguageName = false)]
        [DescriptionLanguage("待处理矢量数据所在地域编码", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBoxCustom),
         UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/globe.png")]

        public string ZoneCode
        {
            get { return zoneCode; }
            set
            {
                zoneCode = value;

                NotifyPropertyChanged("ZoneCode");
            }
        }

        #region Properties

        [DisplayName("数据类型")]
        [DescriptionLanguage("待处理数据类型", IsLanguageName = false)]
        [PropertyDescriptor(
           Builder = typeof(PropertyDescriptorBuilderComboBoxEnum),//typeof(PropertyDescriptorBuilderEnum),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/16/odata.png")]
        public DataTypeEum DataType
        {
            get => _DataType;
            set
            {
                _DataType = value;
                NotifyPropertyChanged("DataType");
            }
        }
        public DataTypeEum _DataType= DataTypeEum.承包地;

       

        #region 选择单个文件
        //[DisplayLanguage("矢量数据路径", IsLanguageName = false)]
        //[DescriptionLanguage("待处理矢量数据文件的路径", IsLanguageName = false)]
        //[PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFileBrowserShp),
        //    UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]
        //[WatermaskLanguage("请选择.shp矢量文件路径")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "必填")]
        //public string ShapeFilePath
        //{
        //    get { return shapeFilePath; }
        //    set
        //    {
        //        shapeFilePath = value;
        //        NotifyPropertyChanged("ShapeFilePath");
        //        CheckShpFile(shapeFilePath);
        //    }
        //}


        //[DisplayLanguage("地块数量", IsLanguageName = false)]
        //[DescriptionLanguage("待处理矢量数据地块数量", IsLanguageName = false)]
        //[PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBox),
        //    UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]

        //public int? LandCount
        //{
        //    get { return landCount; }
        //    set
        //    {
        //        landCount = value;
        //        NotifyPropertyChanged("LandCount");
        //    }
        //} 
        #endregion
        #region 选择文件夹
        [DisplayLanguage("数据路径", IsLanguageName = false)]
        [DescriptionLanguage("存放待处理数据文件夹的路径", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderFolderBrowserExtsion),
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/folder-horizontal-open.png")]
        [WatermaskLanguage("请选择待处理矢量数据文件夹路径")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "必填")]
        public string ResultFilePath
        {
            get { return resultFilePath; }
            set
            {
                resultFilePath = value;
             
                NotifyPropertyChanged("ResultFilePath");
                CheckShpFile(resultFilePath);
            }
        }
        private string resultFilePath;
        //public event EventHandler<string> LogPathChanged;



        [DisplayLanguage("数据量", IsLanguageName = false)]
        [DescriptionLanguage("矢量文件个数及图斑总数量", IsLanguageName = false)]
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
        #endregion

        //[DisplayName("是否覆盖")]
        //[DescriptionLanguage("选择是否覆盖上传", IsLanguageName = false)]
        //public bool IsCover
        //{
        //    get { return _IsCover; }
        //    set
        //    {
        //        _IsCover = value;
        //        NotifyPropertyChanged("IsCover");
        //    }
        //}
        //public bool _IsCover = true;

        [DisplayName("加载模式")]
        [DescriptionLanguage("选择追加或覆盖模式", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderComboBoxEnum),
           UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/16/clipboard.png")]
        public UploadDataModel UploadModel
        {
            get { return _UploadModel; }
            set
            {
                _UploadModel = value;
                NotifyPropertyChanged("UploadModel");
            }
        }
        public UploadDataModel _UploadModel= UploadDataModel.追加上传;
        //[DisplayLanguage("初检信息", IsLanguageName = false)]
        //[DescriptionLanguage("对数据进行初步检查并提示", IsLanguageName = false)]
        //[PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderMultiLineReadOnlyTextBox),
        //    UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]
        //[WatermaskLanguage("对数据进行初步检查并提示")]
        //public string CheckInfo
        //{
        //    get { return checkInfo; }
        //    set
        //    {
        //        checkInfo = value;
        //        NotifyPropertyChanged("CheckInfo");
        //    }
        //}




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
        public bool checkPass=false;

        #endregion Properties
        [Enabled(false)]
        public List<ShpFileDescription> ShpFilesInfo;
        #region Ctor

        public UploadVectorDataToBatchArgument()
        {
        }
        [Enabled(false)]
        public int ShpFileCount { get; set; }
        #endregion

        #region 单个文件上传检查
        //private void CheckShpFile(string shapeFilePath)
        //{
        //    bool check = false; int? count = null;
        //    var error = VectorDataProgress.CheckShpFile(shapeFilePath, out check, out count);
        //    if (error.Count == 0) error.Add("未发现矢量数据错误，检查通过！");
        //    error.ForEach(t =>
        //    {
        //        CheckInfo += t + "\n";
        //    });
        //    ConfirmEnabled = check;
        //    LandCount = count;  //单个文件上传
        //} 
        #endregion



        private async System.Threading.Tasks.Task CheckShpFile(string resultFilePathP)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                var shps = ShpFolderDescription.GetFilesByExtensionLegacy(resultFilePathP);

                 ShpFileCount = 0; int dataCount = 0;
                //fileCount = shps.Count();
                ShpFilesInfo = new List<ShpFileDescription>();
                foreach (var shp in shps)
                {
                    var shpInfo = ShpFolderDescription.GetShpFileDescription(shp);
                    ShpFileCount++; dataCount += shpInfo.DataCount;
                    DataCount = string.Format("共{0}个矢量文件个数,{1}个图斑", ShpFileCount, dataCount);
                    //CheckInfo = CheckInfo + shp.ReplaceFirst(ResultFilePath, ".") + "  地块数量：" + shpInfo.DataCount + "  " + shpInfo.Description + "\n";
                    ShpFilesInfo.Add(shpInfo);
                }
                ConfirmEnabled = true;
            });

        }
    }
}
