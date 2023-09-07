using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public class Log : YltEntityID
    {
        #region Properties

        public int EventID { get; set; }
        public eLogGrade Grade { get; set; }
        public DateTime OperateTime { get; set; }
        public eOperateType OperateType { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public eOperationTargetType TargetType { get; set; }
        public string Description { get; set; }

        #endregion

        #region Fields

        #endregion

        #region Ctor

        static Log()
        {
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_LogGrade);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_OperationTargetType);
            LanguageAttribute.AddLanguage(Properties.Resources.langChs_OperationType);
        }

        public Log()
        {
            OperateTime = DateTime.Now;
            OperateType = eOperateType.General;
            TargetType = eOperationTargetType.General;
        }

        public Log(int eventID, string description)
            : this()
        {
            EventID = eventID;
            Description = description;
        }

        #endregion

        #region Methods

        #region Methods - Override

        public override string ToString()
        {
            return Description;
        }

        public string ToString(bool withTime)
        {
            if (withTime)
                return string.Format("{0} {1}", OperateTime.ToString(), Description);

            return Description;
        }

        #endregion

        #endregion
    }
}