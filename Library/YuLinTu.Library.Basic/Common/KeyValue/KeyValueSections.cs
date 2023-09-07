using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [Serializable()]
    public class KeyValueSections
    {
        #region Properties

        public string Name { get; set; }
        public List<StringObjectPair> Sections { get; set; }

        #endregion

        #region Ctor

        public KeyValueSections()
        {
            Sections = new List<StringObjectPair>();
        }

        #endregion

        #region Methods

        public TValue GetValue<TValue>(string key)
        {
            foreach (var sec in Sections)
                if (sec.Key == key)
                    return (TValue)sec.Value;

            StringObjectPair pair = new StringObjectPair(key, default(TValue));
            Sections.Add(pair);
            return (TValue)pair.Value;
        }

        public TValue GetValue<TValue>(string key, TValue defaultValue)
        {
            foreach (var sec in Sections)
                if (sec.Key == key)
                    return (TValue)sec.Value;

            StringObjectPair pair = new StringObjectPair(key, defaultValue);
            Sections.Add(pair);
            return (TValue)pair.Value;
        }

        public void SetValue(string key, object value)
        {
            for (int i = 0; i < Sections.Count; i++)
                if (Sections[i].Key == key)
                {
                    Sections[i].Value = value;
                    return;
                }

            Sections.Add(new StringObjectPair(key, value));
        }

        public void AddValue(string key, object value)
        {
            for (int i = 0; i < Sections.Count; i++)
                if (Sections[i].Key == key)
                    return;

            Sections.Add(new StringObjectPair(key, value));
        }

        #endregion
    }
}
