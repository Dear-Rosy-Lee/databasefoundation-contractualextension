using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Appwork.Apps.Samples.Examples.Dialog
{
    public class FormValidationPersonExtends : FormValidationPerson
    {
        #region Properties

        public PersonExtends Extends
        {
            get { return _Extends; }
            set { _Extends = value; NotifyPropertyChanged(() => Extends); }
        }
        private PersonExtends _Extends = new PersonExtends();


        #endregion
    }
}
