using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Component.ResultDbof2016ToLocalDbFX
{
  public class CBJYQZ
    {
        public const string FCBJYQZBM = "CBJYQZBM";
        public const string FFZJG = "FZJG";
        public const string FFZRQ = "FZRQ";
        public const string FQZLQRQ = "QZLQRQ";
        public const string FQZLQRXM = "QZLQRXM";
        public const string FQZLQRZJHM = "QZLQRZJHM";
        public const string FQZLQRZJLX = "QZLQRZJLX";
        public const string FQZSFLQ = "QZSFLQ";
        public const string TableName = "CBJYQZ";
        public const string TableNameCN = "承包经营权证";

        public string CBJYQZBM { get; set; }
        public string FZJG { get; set; }
        public DateTime FZRQ { get; set; }
        public DateTime? QZLQRQ { get; set; }
        public string QZLQRXM { get; set; }
        public string QZLQRZJHM { get; set; }
        public string QZLQRZJLX { get; set; }
        public string QZSFLQ { get; set; }

    }
}
