/*
 * (C) 2016 鱼鳞图公司版权所有,保留所有权利
*/
using System.Collections.Generic;
using System.Linq;

namespace YuLinTu.Component.ResultDbToLocalDb
{
    /// <summary>
    /// 数据分页
    /// </summary>
    public static class ListTake
    {
        /// <summary>
        /// 按照指定数量循环返回每组数据
        /// </summary>
        /// <param name="list">总数据</param>
        /// <param name="count">每次返回数据</param>
        /// <returns></returns>
        public static IEnumerable<List<T>> TakeGroup<T>(this List<T> list, int count)
        {
            if (list == null || list.Count < count)
            {
                yield return list;
                yield break;
            }
            int getCount = count;
            for (int i = 0; i < list.Count; i += count)
            {
                var glist = list.Take(getCount).Skip(i).ToList();
                getCount += count;
                yield return glist;
            }
        }
    }
}
