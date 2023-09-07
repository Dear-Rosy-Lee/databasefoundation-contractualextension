using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace YuLinTu.Library.Basic
{
    public class Id
    {
        #region Fields

        #region Fields - Static

        private static readonly object objSync;
        private static SortedList<string, SortedList<int, string>> listLanguage;

        #endregion

        #region Fields - Msg

        [Language("ex20100")]
        public const int exCacheKeyExists = 20100;
        [Language("ex20101")]
        public const int exCacheKeyNotExists = 20101;
        [Language("ex20102")]
        public const int exCacheKeyInvalid = 20102;

        [Language("ex20105")]
        public const int exUnhandledTaskException = 20105;

        #endregion

        #endregion

        #region Ctor

        static Id()
        {
            objSync = new object();
            listLanguage = new SortedList<string, SortedList<int, string>>();

            LanguageAttribute.AddLanguage(Properties.Resources.langChs);
        }

        #endregion

        #region Methods

        #region Methods - Static

        public static string GetName(int index)
        {
            return GetName(typeof(Id), index);
        }

        public static string GetName(Type type, int index)
        {
            if (!listLanguage.Keys.Contains(type.FullName))
                InitializeType(type);

            SortedList<int, string> list = listLanguage[type.FullName];

            if (list.Keys.Contains(index))
                return list[index];

            return string.Empty;
        }

        #endregion

        #region Methods - Private

        private static void InitializeType(Type type)
        {
            if (listLanguage.Keys.Contains(type.FullName))
                return;

            SortedList<int, string> list = new SortedList<int, string>();

            FieldInfo[] listField = type.GetFields();
            foreach (FieldInfo fi in listField)
            {
                LanguageAttribute attr = ToolReflection.GetAttribute<LanguageAttribute>(fi);
                if (attr == null)
                    continue;

                int index = -1;

                try { index = (int)fi.GetValue(null); }
                catch { continue; }

                if (list.Keys.Contains(index))
                    continue;

                list.Add(index, attr.Language);
            }

            listLanguage.Add(type.FullName, list);
        }

        #endregion

        #endregion
    }
}