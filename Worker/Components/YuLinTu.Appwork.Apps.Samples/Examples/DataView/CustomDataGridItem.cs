using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Appwork.Apps.Samples
{
    public class CustomDataGridItem : NotifyCDObject
    {
        #region Properties

        public eMessageGrade? Grade
        {
            get { return _Grade; }
            set { _Grade = value; NotifyPropertyChanged(() => Grade); }
        }
        private eMessageGrade? _Grade;

        public EmployeeItem Employee
        {
            get { return _Employee; }
            set { _Employee = value; NotifyPropertyChanged(() => Employee); }
        }
        private EmployeeItem _Employee;

        #endregion
    }
}
