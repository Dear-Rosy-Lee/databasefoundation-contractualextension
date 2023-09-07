using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using YuLinTu;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    public class OptionsEditorNavigatorPathConfiguration : NotifyInfoCDObject
    {
        #region Properties

        [Required(AllowEmptyStrings = false, ErrorMessage = "路径不能为空")]
        public string Path
        {
            get { return _Path; }
            set { _Path = value; NotifyPropertyChanged(() => Path); }
        }
        private string _Path;

        #endregion

        #region Ctor

        public OptionsEditorNavigatorPathConfiguration()
        {
        }

        #endregion
    }
}
