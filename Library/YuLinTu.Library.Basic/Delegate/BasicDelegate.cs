using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace YuLinTu.Library.Basic
{
    public delegate object MethodReturnObjDelegate();

    public delegate void InvokeDelegate();
    public delegate void ParameterInvokeDelegate(object obj);

    public delegate bool TraversalAssemblyTypesDelegate(Type type);
    public delegate bool TraversalAssemblyDelegate(Assembly asm);
    public delegate bool TraversalPropertiesDelegate(string propertyName, object propertyValue);
    public delegate bool TraversalPropertiesInfoDelegate(PropertyInfo pi, object propertyValue);
    public delegate bool TraversalPropertiesAttributeDelegate(object attr, object propertyValue);
    public delegate bool TraversalDirectoryDelegate(string path, eDirectoryContentType typeContent);

    public delegate IEnumerable ChildrenGetterDelegate(object obj);
    public delegate void ChildrenCreatorDelegate(object obj);
}
