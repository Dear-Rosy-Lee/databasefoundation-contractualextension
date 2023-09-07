using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;

namespace YuLinTu.Library.Basic
{
    public delegate void InvokeMethodOverrideEventHandler(object sender, InvokeMethodOverrideEventArgs e);
    public delegate void InvokeMethodOverrideExceptionEventHandler(object sender, InvokeMethodOverrideExceptionEventArgs e);
    public delegate void InvokeMethodBeginEventHandler(object sender, InvokeMethodBeginEventArgs e);
    public delegate void InvokeMethodEndEventHandler(object sender, InvokeMethodEndEventArgs e);
    public delegate void InvokeMethodFailureEventHandler(object sender, InvokeMethodFailureEventArgs e);
    public delegate bool InvokeMethodExceptionEventHandler(object sender, InvokeMethodExceptionEventArgs e);

    public abstract class InvokeMethodEventArgs : EventArgs
    {
        #region Properties

        public IMethodCallMessage Method { get; set; }
        public string MethodName { get { return Method.MethodBase.ToString(); } }

        #endregion

        #region Ctor

        public InvokeMethodEventArgs(IMethodCallMessage method)
        {
            Method = method;
        }

        #endregion
    }

    public class InvokeMethodOverrideEventArgs : InvokeMethodEventArgs
    {
        #region Properties


        #endregion

        #region Ctor

        public InvokeMethodOverrideEventArgs(IMethodCallMessage method)
            : base(method)
        {
        }

        #endregion
    }

    public class InvokeMethodOverrideExceptionEventArgs : InvokeMethodEventArgs
    {
        #region Properties

        public Exception Exception { get; set; }

        #endregion

        #region Ctor

        public InvokeMethodOverrideExceptionEventArgs(IMethodCallMessage method, Exception ex)
            : base(method)
        {
            Exception = ex;
        }

        #endregion
    }

    public class InvokeMethodBeginEventArgs : InvokeMethodEventArgs
    {
        #region Properties


        #endregion

        #region Ctor

        public InvokeMethodBeginEventArgs(IMethodCallMessage method)
            : base(method)
        {
        }

        #endregion
    }

    public class InvokeMethodEndEventArgs : InvokeMethodEventArgs
    {
        #region Properties

        public object ReturnValue { get; set; }

        #endregion

        #region Ctor

        public InvokeMethodEndEventArgs(IMethodCallMessage method, object retVal)
            : base(method)
        {
            ReturnValue = retVal;
        }

        #endregion
    }

    public class InvokeMethodFailureEventArgs : InvokeMethodEventArgs
    {
        #region Ctor

        public InvokeMethodFailureEventArgs(IMethodCallMessage method)
            : base(method)
        {
        }

        #endregion
    }

    public class InvokeMethodExceptionEventArgs : InvokeMethodEventArgs
    {
        #region Properties

        public Exception Exception { get; set; }

        #endregion

        #region Ctor

        public InvokeMethodExceptionEventArgs(IMethodCallMessage method, Exception ex)
            : base(method)
        {
            Exception = ex;
        }

        #endregion
    }
}
