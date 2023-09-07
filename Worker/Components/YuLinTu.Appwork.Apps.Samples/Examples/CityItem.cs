using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Appwork.Apps.Samples
{
    public class CityItem : NotifyCDObject
    {
        #region Properties

        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged("Name"); }
        }
        private string _Name;

        #endregion

        #region Ctor

        public CityItem()
        {
        }

        #endregion
    }
}
