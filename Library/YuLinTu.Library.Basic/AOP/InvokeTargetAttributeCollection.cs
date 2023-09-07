using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace YuLinTu.Library.Basic
{
    internal class InvokeTargetAttributeCollection
    {
        #region Fields

        private object[] args;

        private SortedList<string, List<InvokeTargetAttribute>> listAttribute;
        private SortedList<string, object> listExceptionReturnValue;

        #endregion

        #region Ctor

        public InvokeTargetAttributeCollection(object[] args)
        {
            this.args = args;

            listAttribute = new SortedList<string, List<InvokeTargetAttribute>>();
            listExceptionReturnValue = new SortedList<string, object>();
        }

        #endregion

        #region Methods

        #region Methods - Public

        public List<InvokeTargetAttribute> GetAttributes(MethodBase method)
        {
            string txt = method.ToString();

            if (!listAttribute.Keys.Contains(txt))
                InitializeMethod(method);

            return listAttribute[txt];
        }

        public object GetExceptionReturnValue(MethodBase method)
        {
            string txt = method.ToString();

            if (!listExceptionReturnValue.Keys.Contains(txt))
                InitializeMethod(method);

            return listExceptionReturnValue[txt];
        }

        #endregion

        #region Methods - Helper

        private void InitializeMethod(MethodBase method)
        {
            string txt = method.ToString();

            List<InvokeTargetAttribute> listAttr = new List<InvokeTargetAttribute>();
            object retValue = null;

            object[] objs = method.GetCustomAttributes(false);
            foreach (object obj in objs)
            {
                if (obj is InvokeTargetAttribute)
                {
                    InvokeTargetAttribute attr = obj as InvokeTargetAttribute;
                    attr.SetArguments(args);
                    listAttr.Add(attr);
                }
                else if (obj is ExceptionValueAttribute)
                    retValue = (obj as ExceptionValueAttribute).Value;
            }

            listAttr.Sort();

            listAttribute.Add(txt, listAttr);
            listExceptionReturnValue.Add(txt, retValue);
        }

        #endregion

        #endregion
    }
}
