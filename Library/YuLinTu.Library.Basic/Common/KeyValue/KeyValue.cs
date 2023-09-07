using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [Serializable()]
    public class KeyValue<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public KeyValue()
        {
        }

        public KeyValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable()]
    public class StringStringPair : KeyValue<string, string>
    {
        public StringStringPair()
        {
        }

        public StringStringPair(string key, string value)
            : base(key, value)
        {
        }
    }

    [Serializable()]
    public class StringObjectPair : KeyValue<string, object>
    {
        public StringObjectPair()
        {
        }

        public StringObjectPair(string key, object value)
            : base(key, value)
        {
        }
    }

}
