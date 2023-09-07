using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public class DictionaryHelper
    {
        private static Dictionary<string, List<Dictionary>> BufferDic = new Dictionary<string, List<Dictionary>>();

        /// <summary>
        /// 以组编码获取数据字典List获取
        /// </summary>
        /// <param name="groupCode">组编码</param>
        /// <param name="isIncludeEmpry">是否包含空的项 默认不包含</param>
        /// <returns></returns>
        public static List<Dictionary> GetDictionaryByGroupCode(string groupCode,bool isIncludeEmpry=false)
        {
            List<Dictionary> result = null;
            if(BufferDic.ContainsKey(groupCode)) result = BufferDic[groupCode];

            if (result == null || result.Count == 0)
            {
                result = DataBaseSource.GetDataBaseSource().CreateDictWorkStation().GetByGroupCode(groupCode, isIncludeEmpry);
                BufferDic[groupCode] = result;
            }
            return result;
            //if(isIncludeEmpry)
            //    return DataBaseSource.GetDataBaseSource().CreateDictWorkStation().Get(o => o.GroupCode==groupCode);
            //return DataBaseSource.GetDataBaseSource().CreateDictWorkStation().Get(o =>!string.IsNullOrEmpty(o.GroupCode)&& o.GroupCode == groupCode);
        }
    }
}
