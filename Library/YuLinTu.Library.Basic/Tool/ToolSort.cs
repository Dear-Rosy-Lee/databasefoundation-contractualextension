using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace YuLinTu.Library.Basic
{
    public class ToolSort
    {
        #region Methods - Sort

        public static IList SortList(IList array)
        {
            object entity = null;
            for (int i = 0; i < array.Count; i++)
            {
                for (int j = i + 1; j < array.Count; j++)
                {
                    if (((array[i] as YltEntity).Compare(array[j] as YltEntity)) == 1)
                    {
                        entity = array[i];
                        array[i] = array[j];
                        array[j] = entity;
                    }
                }
            }
            return array;
        }

        #endregion
    }
}
