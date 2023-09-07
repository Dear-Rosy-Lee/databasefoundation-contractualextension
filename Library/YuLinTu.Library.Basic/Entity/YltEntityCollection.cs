using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace YuLinTu.Library.Basic
{
    [Serializable()]
    public class YltEntityCollection<T> : List<T>, ICloneable where T : YltEntity
    {
        #region Properties

        [Browsable(false)]
        public new int Capacity
        {
            get { return base.Capacity; }
            set { base.Capacity = value; }
        }

        [Browsable(false)]
        public new int Count
        {
            get { return base.Count; }
        }

        #endregion

        #region Methods

        public object Clone()
        {
            object obj = Activator.CreateInstance(this.GetType());
            IList list = obj as IList;

            foreach (T item in this)
                list.Add(item.Clone());

            return obj;
        }

        #endregion
    }
}