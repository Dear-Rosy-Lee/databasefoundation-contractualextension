using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Appwork.Apps.Samples.Examples.Dialog
{
    public class FormValidationPersonDetailsExtends : FormValidationPerson
    {
        #region Properties

        public PersonDetailsExtends Extends
        {
            get { return _Extends; }
            set { _Extends = value; NotifyPropertyChanged(() => Extends); }
        }
        private PersonDetailsExtends _Extends = new PersonDetailsExtends();


        #endregion
    }
}
