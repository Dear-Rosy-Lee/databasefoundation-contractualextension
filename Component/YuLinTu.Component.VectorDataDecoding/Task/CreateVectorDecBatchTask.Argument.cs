using GeoAPI.CoordinateSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using YuLinTu;
using YuLinTu.DF.Enums;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    public class CreateVectorDecBatchTaskArgument : TaskArgument
    {
        #region Properties

        private string zoneName;

        [DisplayLanguage("地域名称", IsLanguageName = false)]
        [DescriptionLanguage("待处理矢量数据所在地域名称", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBox),
         UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]

        public string ZoneName
        {
            get { return zoneName; }
            set
            {
                zoneName = value;
                NotifyPropertyChanged("ZoneName");
            }
        }



        private string zoneCode;

        [DisplayLanguage("地域编码", IsLanguageName = false)]
        [DescriptionLanguage("待处理矢量数据所在地域编码", IsLanguageName = false)]
        [PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderReadOnlyTextBox),
         UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]

        public string ZoneCode
        {
            get { return zoneCode; }
            set
            {
                zoneCode = value;
                //if(zoneCode.Length<9)
                //{
                //    Info ="无法创建矢量数据脱密任务！请选择乡镇或村级地域后创建任务。";
                //}
                NotifyPropertyChanged("ZoneCode");
            }
        }
        //[DisplayLanguage("提示说明", IsLanguageName = false)]
        //[DescriptionLanguage("对数据进行初步检查并提示", IsLanguageName = false)]
        //[PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderMultiLineReadOnlyTextBox),
        // UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/shapeconverttofreeform.png")]
        //[WatermaskLanguage("对数据进行初步检查并提示")]
        //public string Info
        //{
        //    get { return _Info; }
        //    set
        //    {
        //        _Info = value;
        //        NotifyPropertyChanged("Info");
        //    }
        //}

        //private string _Info;

       
        #endregion

        #region Ctor

        public CreateVectorDecBatchTaskArgument()
        {
            
        }

        #endregion
    }
}
