using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;

namespace YuLinTu.Library.Basic
{
    public abstract class AOPBase : RealProxy
    {
        #region Events

        public event InvokeMethodOverrideEventHandler InvokeMethodOverride;
        public event InvokeMethodOverrideExceptionEventHandler InvokeMethodOverrideException;
        public event InvokeMethodBeginEventHandler InvokeMethodBegin;
        public event InvokeMethodEndEventHandler InvokeMethodEnd;
        public event InvokeMethodFailureEventHandler InvokeMethodFailure;
        public event InvokeMethodExceptionEventHandler InvokeMethodException;

        #endregion

        #region Fields

        private AOPTarget target;

        private InvokeTargetAttributeCollection invokeAttributes;

        #endregion

        #region Ctor

        public AOPBase(AOPTarget target, object[] args)
            : base(target.GetType())
        {
            this.target = target;

            invokeAttributes = new InvokeTargetAttributeCollection(args);
        }

        #endregion

        #region Methods

        #region Methods - Create Object

        #endregion

        #region Methods - Override

        public override IMessage Invoke(IMessage msg)
        {
            object ret = null;

            IMethodCallMessage method = msg as IMethodCallMessage;
            if (method == null)
                return new ReturnMessage(ret, new object[0], 0, null, method);

            object objSync = OnGetSyncObject();


            if (objSync != null)
            {
                System.Threading.Monitor.Enter(objSync);

                ret = Invoke(method);

                System.Threading.Monitor.Exit(objSync);
            }
            else
                ret = Invoke(method);

            return new ReturnMessage(ret, new object[0], 0, null, method);
        }

        protected virtual object OnGetSyncObject()
        {
            return null;
        }

        #endregion

        #region Methods - Invoke

        private object Invoke(IMethodCallMessage method)
        {
            object ret = null;
            bool reInvoke = false;

            try
            {
                InvokeOverrideRetValue retValue = InvokeOverride(method) as InvokeOverrideRetValue;
                if (retValue != null && retValue.IsOverride)
                    return retValue.RetValue;
            }
            catch (Exception ex)
            {
                target.LastException = ex;
                InvokeOverrideException(method, ex);
            }

            try
            {
                //调用方法之前，先进行初始化工作。
                Log log = InvokeBegin(method) as Log;

                if (log == null)
                {
                    //调用方法。
                    ret = InvokeMethod(method);

                    //调用方法结束后，进行一些收尾工作。
                    InvokeEnd(method, ret);
                }
                else
                {
                    target.LastErrorLog = log;
                    //初始化失败时，调用方法获取异常返回值。
                    ret = InvokeFailure(method);
                }

            }
            catch (Exception ex)
            {
                target.LastException = ex;
                //调用过程异常时，获取异常返回值。
                ret = InvokeException(method, ex, ref reInvoke);
            }

            if (reInvoke)
                Invoke(method);

            return ret;
        }

        private object InvokeBegin(IMethodCallMessage method)
        {
            if (InvokeMethodBegin != null)
                InvokeMethodBegin(this, new InvokeMethodBeginEventArgs(method));

            return InvokeMethod(method, null, eInvokeCondition.InvokeBegin);
        }

        private object InvokeOverride(IMethodCallMessage method)
        {
            if (InvokeMethodOverride != null)
                InvokeMethodOverride(this, new InvokeMethodOverrideEventArgs(method));

            return InvokeMethod(method, null, eInvokeCondition.InvokeOverride);
        }

        private object InvokeOverrideException(IMethodCallMessage method, Exception ex)
        {
            if (InvokeMethodOverrideException != null)
                InvokeMethodOverrideException(this, new InvokeMethodOverrideExceptionEventArgs(method, ex));

            return InvokeMethod(method, ex, eInvokeCondition.InvokeOverrideException);
        }

        private object InvokeEnd(IMethodCallMessage method, object ret)
        {
            InvokeMethod(method, ret, eInvokeCondition.InvokeEnd);

            if (InvokeMethodEnd != null)
                InvokeMethodEnd(this, new InvokeMethodEndEventArgs(method, ret));

            return null;
        }

        private object InvokeFailure(IMethodCallMessage method)
        {
            if (InvokeMethodFailure != null)
                InvokeMethodFailure(this, new InvokeMethodFailureEventArgs(method));

            return invokeAttributes.GetExceptionReturnValue(method.MethodBase);
        }

        private object InvokeException(IMethodCallMessage method, Exception ex, ref bool reInvoke)
        {
            InvokeMethod(method, ex, eInvokeCondition.InvokeException);

            if (InvokeMethodException != null)
                reInvoke = InvokeMethodException(this, new InvokeMethodExceptionEventArgs(method, ex));

            return invokeAttributes.GetExceptionReturnValue(method.MethodBase);
        }

        private object InvokeMethod(IMethodCallMessage method)
        {
            return method.MethodBase.Invoke(target, method.Args);
        }

        private object InvokeMethod(IMethodCallMessage method, object arg, eInvokeCondition condition)
        {
            List<InvokeTargetAttribute> list = invokeAttributes.GetAttributes(method.MethodBase);

            foreach (InvokeTargetAttribute attr in list)
                if ((attr.ExecuteCondition & condition) == condition)
                {
                    object result = attr.Do(method, arg);
                    if (result != null)
                        return result;
                }

            return null;
        }

        #endregion

        #endregion
    }
}
