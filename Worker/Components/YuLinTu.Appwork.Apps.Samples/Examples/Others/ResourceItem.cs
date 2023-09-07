using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace YuLinTu.Appwork.Apps.Samples
{
    public class ResourceItem : NotifyCDObject
    {
        #region Properties

        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged("Name"); }
        }
        private string _Name;

        public ImageSource Image
        {
            get { return _Image; }
            set { _Image = value; NotifyPropertyChanged(() => Image); }
        }
        private ImageSource _Image;

        public string Size
        {
            get { return _Size; }
            set { _Size = value; NotifyPropertyChanged(() => Size); }
        }
        private string _Size;

        public double MaxLength
        {
            get { return _MaxLength; }
            set { _MaxLength = value; NotifyPropertyChanged(() => MaxLength); }
        }
        private double _MaxLength;


        public string UriPath
        {
            get { return _UriPath; }
            set { _UriPath = value; NotifyPropertyChanged(() => UriPath); }
        }
        private string _UriPath;

        #endregion

        #region Ctor

        public ResourceItem()
        {
        }

        #endregion
    }
}
