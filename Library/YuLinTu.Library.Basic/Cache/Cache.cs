using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public class Cache
    {
        #region Properties

        #endregion

        #region Fields

        private static readonly object objSync;
        private static SortedList<string, object> cache;

        #endregion

        #region Ctor

        static Cache()
        {
            objSync = new object();
            cache = new SortedList<string, object>();
        }

        #endregion

        #region Methods

        #region Methods - Public

        public static void Add(string key, object arg)
        {
            key = key.Trim();
            if (string.IsNullOrEmpty(key))
            {
                WriteTrace(Id.exCacheKeyInvalid, key, eLogGrade.Error);
                return;
            }

            if (cache.Keys.Contains(key))
            {
                WriteTrace(Id.exCacheKeyExists, key, eLogGrade.Error);
                return;
            }

            lock (objSync)
                cache.Add(key, arg);
        }

        public static object Get(string key)
        {
            if (cache.Keys.Contains(key))
                return cache[key];
            else
                WriteTrace(Id.exCacheKeyNotExists, key, eLogGrade.Warn);

            return null;
        }

        #endregion

        #region Methods - Helper

        private static void WriteTrace(int id, string key, eLogGrade grade)
        {
            string msg = Id.GetName(id);
            msg = string.Format(msg, key);

            Trace.WriteLine(new Log()
            {
                EventID = id,
                Grade = grade,
                Description = msg,
                TargetType = eOperationTargetType.Memory,
                Source = typeof(Cache).FullName
            });
        }

        #endregion

        #endregion
    }
}