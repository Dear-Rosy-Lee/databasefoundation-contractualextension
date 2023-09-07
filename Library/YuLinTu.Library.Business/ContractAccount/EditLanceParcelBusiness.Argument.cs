using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Library.Business.ContractAccount
{
    public class EditLanceParcelBusinessArgument : TaskArgument
    {
        #region Properties

        private string _Parameter1;

        public string Parameter1
        {
            get { return _Parameter1; }
            set { _Parameter1 = value; NotifyPropertyChanged("Parameter1"); }
        }

        private int _Parameter2;

        public int Parameter2
        {
            get { return _Parameter2; }
            set { _Parameter2 = value; NotifyPropertyChanged("Parameter2"); }
        }

        private bool _Parameter3;

        public bool Parameter3
        {
            get { return _Parameter3; }
            set { _Parameter3 = value; NotifyPropertyChanged("Parameter3"); }
        }

        #endregion

        #region Ctor

        public EditLanceParcelBusinessArgument()
        {
        }

        #endregion
    }
}
