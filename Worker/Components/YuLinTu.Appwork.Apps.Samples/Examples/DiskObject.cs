using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Appwork.Apps.Samples
{
    public class DiskObject : NotifyCDObject
    {
        #region Properties

        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged("Name"); }
        }
        private string _Name;

        public string Path
        {
            get { return _Path; }
            set { _Path = value; NotifyPropertyChanged("Path"); }
        }
        private string _Path;

        public bool IsFile
        {
            get { return _IsFile; }
            set { _IsFile = value; NotifyPropertyChanged("IsFile"); }
        }
        private bool _IsFile;

        public DateTime DateTimeModified
        {
            get { return _DateTimeModified; }
            set { _DateTimeModified = value; NotifyPropertyChanged("DateTimeModified"); }
        }
        private DateTime _DateTimeModified = DateTime.Now;

        public long Size
        {
            get { return _Size; }
            set { _Size = value; NotifyPropertyChanged("Size"); }
        }
        private long _Size;

        public System.Windows.Visibility Visibility
        {
            get { return _Visibility; }
            set { _Visibility = value; NotifyPropertyChanged("Visibility"); }
        }
        private System.Windows.Visibility _Visibility;

        public System.Collections.ObjectModel.ObservableCollection<DiskObject> Children { get; set; }

        #endregion

        #region Ctor

        public DiskObject()
        {
            Children = new System.Collections.ObjectModel.ObservableCollection<DiskObject>();
        }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            var d = obj as DiskObject;
            return d == null ? false : Path == d.Path;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
