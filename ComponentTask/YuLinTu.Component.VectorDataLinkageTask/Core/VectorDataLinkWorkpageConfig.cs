using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Component.VectorDataLinkageTask.Core
{
    public class VectorDataLinkWorkpageConfig : NotifyInfoCDObject
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "鉴权码必填，格式：AppID + # + AppKey")]    
        public string AuthenticationCode
        {
            get { return _AuthenticationCode; }
            set { _AuthenticationCode = value; NotifyPropertyChanged("AuthenticationCode"); }
        }
        private string _AuthenticationCode;

    }
}
