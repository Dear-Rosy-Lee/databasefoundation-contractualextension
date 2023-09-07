using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Library.Controls;
using YuLinTu.Library.Entity;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Component.CombineWordTask
{
    public class TaskCombineWordGroupArgument : TaskCombineWordArgument
    {
        [DisplayLanguage("行政地域级别")]
        [DescriptionLanguage("选择源数据路径的行政地域级别")]
        //[PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderEnumZoneLevel))]
        public eZoneLevel ZoneLevel
        {
            get { return _ZoneLevel; }
            set { _ZoneLevel = value; NotifyPropertyChanged(() => ZoneLevel); }
        }

        private eZoneLevel _ZoneLevel;

        [DisplayLanguage("合并Word类型")]
        [DescriptionLanguage("选择合并Word类型")]
        //[PropertyDescriptor(Builder = typeof(PropertyDescriptorBuilderEnumCombineWordType))]
        public eCombineWordType CombineWordType
        {
            get { return _CombineWordType; }
            set { _CombineWordType = value; NotifyPropertyChanged(() => CombineWordType); }
        }

        private eCombineWordType _CombineWordType;

        public TaskCombineWordGroupArgument()
        {
            ZoneLevel = eZoneLevel.Group;
            CombineWordType = eCombineWordType.RegisterBookAndParcel;
        }
    }
}