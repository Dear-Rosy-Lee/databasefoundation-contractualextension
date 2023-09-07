using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Appwork.Apps.Samples
{
    public class CountryItem : NotifyCDObject
    {
        #region Properties

        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged("Name"); }
        }
        private string _Name;

        public System.Collections.ObjectModel.ObservableCollection<EmployeeItem> Children { get; set; }

        #endregion

        #region Ctor

        public CountryItem()
        {
            Children = new System.Collections.ObjectModel.ObservableCollection<EmployeeItem>();
        }

        #endregion
    }
}
