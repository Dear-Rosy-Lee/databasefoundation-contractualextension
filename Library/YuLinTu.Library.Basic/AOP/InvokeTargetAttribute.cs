using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;
using YuLinTu.Library.Basic;

namespace YuLinTu.Library.Basic
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class InvokeTargetAttribute : Attribute, IComparable
    {
        #region Fields

        #endregion

        #region Properties

        public eInvokeCondition ExecuteCondition { get; set; }
        public uint Order { get; set; }
        public bool ValidValue { get; set; }

        #endregion

        #region Ctor

        public InvokeTargetAttribute()
        {
            ExecuteCondition = eInvokeCondition.InvokeBegin;
            ValidValue = true;
        }

        #endregion

        #region Methods

        #region Methods - Public

        public virtual void SetArguments(object[] args)
        {
        }

        #endregion

        #region Methods - Override

        public virtual object Do(IMethodCallMessage method, object arg)
        {
            return null;
        }

        #endregion

        #region Methods - Protected

        protected Log WriteInfomation(int id, string msg, eOperationTargetType targetType)
        {
            Log log = new Log()
            {
                EventID = id,
                Grade = eLogGrade.Infomation,
                TargetType = targetType,
                Description = msg,
                Source = this.GetType().FullName,
            };

            Trace.WriteLineDebugOnly(log);
            return log;
        }

        protected Log WriteError(int id, string msg, eOperationTargetType targetType)
        {
            Log log = new Log()
            {
                EventID = id,
                Grade = eLogGrade.Error,
                TargetType = targetType,
                Description = msg,
                Source = this.GetType().FullName,
            };

            Trace.WriteLine(log);
            return log;
        }

        #endregion

        #region Methods - Interface

        public int CompareTo(object obj)
        {
            int ret = 0;

            InvokeTargetAttribute attr = obj as InvokeTargetAttribute;

            if (this.Order > attr.Order)
                ret = 1;
            else if (this.Order < attr.Order)
                ret = -1;

            return ret;
        }

        #endregion

        #endregion
    }
}
