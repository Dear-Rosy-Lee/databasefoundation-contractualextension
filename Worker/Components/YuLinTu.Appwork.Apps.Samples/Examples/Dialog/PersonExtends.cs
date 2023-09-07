using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples.Dialog
{
    [GridDescriptor(Column = 2)]
    public class PersonExtends : NotifyInfoCDObject
    {
        #region Properties

        #region Properties - Extends

        [DisplayLanguage("参数 PInt")]
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Catalog = "分组 1",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/hyperlinksverify.png")]
        [Range(0, 300, ErrorMessage = "值不能超出范围(0 ~ 300)")]
        public int PInt
        {
            get { return _PInt; }
            set { _PInt = value; NotifyPropertyChanged(() => PInt); }
        }
        private int _PInt = -1;

        [DisplayLanguage("参数 PDouble")]
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Catalog = "分组 1",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/inbrowsergallery.png")]
        public double PDouble
        {
            get { return _PDouble; }
            set { _PDouble = value; NotifyPropertyChanged(() => PDouble); }
        }
        private double _PDouble;

        [DisplayLanguage("参数 PBool")]
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Catalog = "分组 1",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/insertlink.png")]
        public bool PBool
        {
            get { return _PBool; }
            set { _PBool = value; NotifyPropertyChanged(() => PBool); }
        }
        private bool _PBool;

        [DisplayLanguage("参数 PString")]
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Catalog = "分组 1",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/insertdatasourcegallery.png")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "值不能为空")]
        public string PString
        {
            get { return _PString; }
            set { _PString = value; NotifyPropertyChanged(() => PString); }
        }
        private string _PString;

        [DisplayLanguage("参数 PDateTime")]
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Catalog = "分组 1", Converter = typeof(DateTimeConverter),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/inbrowsergallery.png")]
        public DateTime PDateTime
        {
            get { return _PDateTime; }
            set { _PDateTime = value; NotifyPropertyChanged(() => PDateTime); }
        }
        private DateTime _PDateTime = DateTime.Now;

        [DisplayLanguage("参数 PGuid")]
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Catalog = "分组 1",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/insertbookmarkspd.png")]
        public Guid PGuid
        {
            get { return _PGuid; }
            set { _PGuid = value; NotifyPropertyChanged(() => PGuid); }
        }
        private Guid _PGuid = Guid.NewGuid();

        [DisplayLanguage("参数 PComment")]
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Catalog = "分组 2", ColumnSpan = 2, Height = 100,
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/inkmenu.png")]
        public string PComment
        {
            get { return _PComment; }
            set { _PComment = value; NotifyPropertyChanged(() => PComment); }
        }
        private string _PComment;













        [DisplayLanguage("参数 CInt")]
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Class = "Class 2", Catalog = "分组 1",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/hyperlinksverify.png")]
        [Range(0, 300, ErrorMessage = "值不能超出范围(0 ~ 300)")]
        public int CInt
        {
            get { return _CInt; }
            set { _CInt = value; NotifyPropertyChanged(() => CInt); }
        }
        private int _CInt = -1;

        [DisplayLanguage("参数 CDouble")]
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Class = "Class 2", Catalog = "分组 1",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/inbrowsergallery.png")]
        public double CDouble
        {
            get { return _CDouble; }
            set { _CDouble = value; NotifyPropertyChanged(() => CDouble); }
        }
        private double _CDouble;

        [DisplayLanguage("参数 CBool")]
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Class = "Class 2", Catalog = "分组 1",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/insertlink.png")]
        public bool CBool
        {
            get { return _CBool; }
            set { _CBool = value; NotifyPropertyChanged(() => CBool); }
        }
        private bool _CBool;

        [DisplayLanguage("参数 CString")]
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Class = "Class 2", Catalog = "分组 1",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/insertdatasourcegallery.png")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "值不能为空")]
        public string CString
        {
            get { return _CString; }
            set { _CString = value; NotifyPropertyChanged(() => CString); }
        }
        private string _CString;

        [DisplayLanguage("参数 CDateTime")]
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Class = "Class 2", Catalog = "分组 1",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/inbrowsergallery.png")]
        public DateTime CDateTime
        {
            get { return _CDateTime; }
            set { _CDateTime = value; NotifyPropertyChanged(() => CDateTime); }
        }
        private DateTime _CDateTime = DateTime.Now;

        [DisplayLanguage("参数 CGuid")]
        [Windows.Wpf.Metro.Components.PropertyDescriptor(Class = "Class 2", Catalog = "分组 1",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/images/office/2013/16/insertbookmarkspd.png")]
        public Guid CGuid
        {
            get { return _CGuid; }
            set { _CGuid = value; NotifyPropertyChanged(() => CGuid); }
        }
        private Guid _CGuid = Guid.NewGuid();



        #endregion

        #endregion
    }
}
