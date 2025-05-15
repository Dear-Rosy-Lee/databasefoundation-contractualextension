using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
    public class AdjustSender
    {
        public Guid Id { get; set; }

        [Description("户号")]
        public string HH { get; set; }


        [Description("名称")]
        public string MC { get; set; }


        [Description("证件号码")]
        public string ZJHM { get; set; }


        [Description("成员数量")]
        public string CYSL { get; set; }


        [Description("地块数量")]
        public int DKSL { get; set; }


        [Description("发包方名称")]
        public string FBFMC { get; set; }
    }
}
