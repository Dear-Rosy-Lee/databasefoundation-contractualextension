using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    public class TabControlConfiguration : NotifyCDObject
    {
        #region Properties

        [DisplayLanguage("可见度")]
        [PropertyDescriptor(Catalog = "选择条",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/eye.png")]
        public System.Windows.Visibility SelectorVisibility
        {
            get { return _SelectorVisibility; }
            set { _SelectorVisibility = value; NotifyPropertyChanged(() => SelectorVisibility); }
        }
        private System.Windows.Visibility _SelectorVisibility;


        [DisplayLanguage("长度")]
        [PropertyDescriptor(Catalog = "选择条",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/ColumnWidth.png")]
        [Range(0, 10)]
        public double SelectorLength
        {
            get { return _SelectorLength; }
            set { _SelectorLength = value; NotifyPropertyChanged(() => SelectorLength); }
        }
        private double _SelectorLength;


        [DisplayLanguage("反转")]
        [PropertyDescriptor(Catalog = "选择条")]
        public bool SelectorReverse
        {
            get { return _SelectorReverse; }
            set { _SelectorReverse = value; NotifyPropertyChanged(() => SelectorReverse); }
        }
        private bool _SelectorReverse;


        [DisplayLanguage("方向")]
        [PropertyDescriptor(Catalog = "选项卡",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/ZoomFitToWindow.png")]
        public YuLinTu.Windows.eDirection Direction
        {
            get { return _Direction; }
            set { _Direction = value; NotifyPropertyChanged(() => Direction); }
        }
        private YuLinTu.Windows.eDirection _Direction;

        [DisplayLanguage("选择序号")]
        [PropertyDescriptor(Catalog = "选项卡")]
        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set { _SelectedTabIndex = value; NotifyPropertyChanged(() => SelectedTabIndex); }
        }
        private int _SelectedTabIndex;


        #endregion

        #region Ctor

        public TabControlConfiguration()
        {
            SelectorLength = 3.0;
        }

        #endregion
    }
}
