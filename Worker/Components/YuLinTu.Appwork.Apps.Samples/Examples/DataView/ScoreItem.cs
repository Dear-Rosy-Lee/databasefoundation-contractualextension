using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Appwork.Apps.Samples
{
    public class ScoreItem : NotifyCDObject
    {
        #region Properties

        public double Score1
        {
            get { return _Score1; }
            set { _Score1 = value; NotifyPropertyChanged(() => Score1); }
        }
        private double _Score1;

        public double Score2
        {
            get { return _Score2; }
            set { _Score2 = value; NotifyPropertyChanged(() => Score2); }
        }
        private double _Score2;

        public EmployeeItem Employee
        {
            get { return _Employee; }
            set { _Employee = value; NotifyPropertyChanged(() => Employee); }
        }
        private EmployeeItem _Employee;

        #endregion
    }
}
