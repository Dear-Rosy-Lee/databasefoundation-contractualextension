using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace YuLinTu.Library.Basic
{
    public class Cache<T>
    {
        #region Properties

        public T this[object scope, object key]
        {
            get
            {
                lock (objSync)
                {
                    Hashtable ht = (Hashtable)cache[scope];
                    ht = (Hashtable)ht[typeof(T)];
                    return (T)ht[key];
                }
            }
            set
            {
                lock (objSync)
                {
                    Hashtable ht = (Hashtable)cache[scope];
                    ht = (Hashtable)ht[typeof(T)];
                    ht[key] = value;
                }
            }
        }

        #endregion

        #region Fields

        private static object objSync;
        private static Hashtable cache;

        #endregion

        #region Ctor

        static Cache()
        {
            objSync = new object();
            cache = new Hashtable();
        }

        public Cache(object scope)
        {
            lock (objSync)
            {
                if (!cache.ContainsKey(scope))
                    cache.Add(scope, new Hashtable());

                Hashtable ht = (Hashtable)cache[scope];

                if (!ht.ContainsKey(typeof(T)))
                    ht.Add(typeof(T), new Hashtable());
            }
        }

        #endregion

        #region Methods

        #region Methods - Public

        public bool ContainsKey(object scope, object key)
        {
            if (scope == null || key == null)
                return false;

            lock (objSync)
            {
                Hashtable ht = (Hashtable)cache[scope];
                ht = (Hashtable)ht[typeof(T)];
                return ht.ContainsKey(key);
            }
        }

        public void Set(object scope, object key, T arg)
        {
            if (scope == null || key == null)
                return;

            lock (objSync)
            {
                Hashtable ht = (Hashtable)cache[scope];
                ht = (Hashtable)ht[typeof(T)];

                if (ht.ContainsKey(key))
                    ht[key] = arg;
                else
                    ht.Add(key, arg);
            }
        }

        public void Remove(object scope, object key)
        {
            if (scope == null || key == null)
                return;

            lock (objSync)
            {
                Hashtable ht = (Hashtable)cache[scope];
                ht = (Hashtable)ht[typeof(T)];

                if (ht.ContainsKey(key))
                    ht.Remove(key);
            }
        }

        public void Clear(object scope)
        {
            Clear(scope, typeof(T));
        }

        public void Clear(object scope, Type type)
        {
            if (scope == null || type == null)
                return;

            lock (objSync)
            {
                Hashtable ht = (Hashtable)cache[scope];

                if (ht.ContainsKey(type))
                {
                    ht = (Hashtable)ht[type];
                    ht.Clear();
                }
            }
        }

        public T Get(object scope, object key)
        {
            if (scope == null || key == null)
                return default(T);

            lock (objSync)
            {
                Hashtable ht = (Hashtable)cache[scope];
                ht = (Hashtable)ht[typeof(T)];

                if (ht.ContainsKey(key))
                    return (T)ht[key];
            }

            WriteTrace(Id.exCacheKeyNotExists, key, eLogGrade.Warn);
            return default(T);
        }

        #endregion

        #region Methods - Helper

        private void WriteTrace(int id, object key, eLogGrade grade)
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