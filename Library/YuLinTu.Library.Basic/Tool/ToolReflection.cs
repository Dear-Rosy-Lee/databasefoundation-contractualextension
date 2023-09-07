using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace YuLinTu.Library.Basic
{
    public class ToolReflection
    {
        #region Methods - Reflection

        public static T GetAttribute<T>(ICustomAttributeProvider provider) where T : Attribute
        {
            object[] objs = provider.GetCustomAttributes(typeof(T), true);
            if (objs.Length == 0)
                return null;

            return (T)objs[0];
        }

        public static T[] GetAttributes<T>(ICustomAttributeProvider provider) where T : Attribute
        {
            object[] objs = provider.GetCustomAttributes(typeof(T), true);

            List<T> listAttr = new List<T>();
            foreach (object obj in objs)
                listAttr.Add((T)obj);

            return listAttr.ToArray<T>();
        }

        public static Attribute GetAttribute(ICustomAttributeProvider provider, Type attrType)
        {
            object[] objs = provider.GetCustomAttributes(attrType, true);
            if (objs.Length == 0)
                return null;

            return (Attribute)objs[0];
        }

        public static T CopyFrom<T>(object source)
        {
            if (source == null)
                return default(T);

            Type typeSource = source.GetType();
            Type typeTarget = typeof(T);

            T target = Activator.CreateInstance<T>();

            PropertyInfo[] listSourceProperty = typeSource.GetProperties();
            PropertyInfo[] listTargetProperty = typeTarget.GetProperties();

            foreach (PropertyInfo piSource in listSourceProperty)
            {
                try
                {
                    object val = piSource.GetValue(source, null);
                    foreach (PropertyInfo piTarget in listTargetProperty)
                        if (piTarget.Name == piSource.Name && piTarget.CanWrite)
                        {
                            piTarget.SetValue(target, val, null);
                            break;
                        }
                }
                catch { }
            }

            return (T)target;
        }

        public static void CopyMember(object source, object target)
        {
            if (source == null || target == null)
                return;

            Type typeSource = source.GetType();
            Type typeTarget = target.GetType();

            PropertyInfo[] listSourceProperty = typeSource.GetProperties();
            PropertyInfo[] listTargetProperty = typeTarget.GetProperties();

            foreach (PropertyInfo piSource in listSourceProperty)
            {
                try
                {
                    object val = piSource.GetValue(source, null);
                    foreach (PropertyInfo piTarget in listTargetProperty)
                        if (piTarget.Name == piSource.Name &&
                            piTarget.PropertyType.FullName == piSource.PropertyType.FullName &&
                            piTarget.CanWrite)
                        {
                            piTarget.SetValue(target, val, null);
                            break;
                        }
                }
                catch { }
            }
        }

        public static Assembly GetAssembly(string nameAssembly)
        {
            Assembly[] listAssembly = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly asm in listAssembly)
                if (asm.FullName == nameAssembly)
                    return asm;

            return null;
        }

        public static void TraversalAppDomainAssemblies(TraversalAssemblyDelegate method)
        {
            if (method == null)
                return;

            Assembly[] listAssembly = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly asm in listAssembly)
            {
                if (!method(asm))
                    return;
            }
        }

        public static void TraversalAssemblyTypes(TraversalAssemblyTypesDelegate method)
        {
            if (method == null)
                return;

            Assembly[] listAssembly = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly asm in listAssembly)
            {
                try
                {
                    Type[] listType = asm.GetTypes();
                    foreach (Type type in listType)
                        if (!method(type))
                            return;
                }
                catch { }
            }
        }

        public static void TraversalAssemblyTypes(Assembly asm, TraversalAssemblyTypesDelegate method)
        {
            if (method == null || asm == null)
                return;

            Type[] listType = asm.GetTypes();
            foreach (Type type in listType)
                if (!method(type))
                    return;
        }

        public static void TraversalProperties(TraversalPropertiesDelegate method, object target)
        {
            if (method == null || target == null)
                return;

            PropertyInfo[] listPropertyInfo = target.GetType().GetProperties();

            foreach (PropertyInfo pi in listPropertyInfo)
            {
                object val = pi.GetValue(target, null);
                if (!method(pi.Name, val))
                    return;
            }
        }

        public static void TraversalPropertiesInfo(TraversalPropertiesInfoDelegate method, object target)
        {
            if (method == null || target == null)
                return;

            PropertyInfo[] listPropertyInfo = target.GetType().GetProperties();

            foreach (PropertyInfo pi in listPropertyInfo)
            {
                object val = pi.GetValue(target, null);
                if (!method(pi, val))
                    return;
            }
        }

        public static void TraversalPropertiesAttribute(TraversalPropertiesAttributeDelegate method, object target, Type typeAttr)
        {
            if (method == null || target == null)
                return;

            PropertyInfo[] listPropertyInfo = target.GetType().GetProperties();

            foreach (PropertyInfo pi in listPropertyInfo)
            {
                object attr = ToolReflection.GetAttribute(pi, typeAttr);
                if (attr == null)
                    continue;

                object val = pi.GetValue(target, null);
                if (!method(attr, val))
                    return;
            }
        }

        public static object GetPropertyValue(object target, string propertyName)
        {
            if (target == null || string.IsNullOrEmpty(propertyName))
                return null;

            object value = null;

            Type type = target.GetType();
            PropertyInfo pi = type.GetProperty(propertyName);
            if (pi == null)
                return null;

            value = pi.GetValue(target, null);

            return value;
        }

        public static void SetPropertyValue(object target, string propertyName, object propertyValue)
        {
            if (target == null || string.IsNullOrEmpty(propertyName))
                return;

            Type type = target.GetType();
            PropertyInfo pi = type.GetProperty(propertyName);
            if (pi == null)
                return;

            pi.SetValue(target, propertyValue, null);
        }

        #endregion
    }
}
