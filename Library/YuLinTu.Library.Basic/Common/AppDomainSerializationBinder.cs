using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using System.Reflection;

namespace YuLinTu.Library.Basic
{
    public class AppDomainSerializationBinder : SerializationBinder
    {
        #region Fields

        private static Hashtable htAssemblies;

        #endregion

        #region Methods

        #region Methods - Static

        private static Type GetAssemblyTypes(string nameAssembly, string nameType)
        {
            if (htAssemblies == null)
                htAssemblies = new Hashtable();

            Type retType = null;

            if (!htAssemblies.ContainsKey(nameAssembly))
                CreateAssemblyTypesList(nameAssembly);

            Hashtable ht = htAssemblies[nameAssembly] as Hashtable;
            if (ht != null && ht.ContainsKey(nameType))
                retType = ht[nameType] as Type;

            return retType;
        }

        private static void CreateAssemblyTypesList(string nameAssembly)
        {
            Assembly asm = ToolReflection.GetAssembly(nameAssembly);
            if (asm == null)
                return;

            Hashtable htTypes = new Hashtable();
            htAssemblies[nameAssembly] = htTypes;


            Type[] types = asm.GetTypes();

            foreach (Type type in types)
                htTypes[type.FullName] = type;
        }

        #endregion

        #region Methods - Override

        public override Type BindToType(string assemblyName, string typeName)
        {
            return GetAssemblyTypes(assemblyName, typeName);
        }

        #endregion

        #endregion
    }
}
