using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
   public static class InitializeNameBySet
    {
        private static SystemSetDefine SystemSet = SystemSetDefine.GetIntence();

        /// <summary>
        /// 初始化户主名称
        /// </summary>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public static string InitalizeFamilyName(string familyName)
        {
            if (string.IsNullOrEmpty(familyName))
            {
                return "";
            }
            if (!SystemSet.KeepRepeatFlag)
            {
                return familyName;
            }
            string number = ToolString.GetAllNumberWithInString(familyName);
            if (!string.IsNullOrEmpty(number))
            {
                return familyName.Replace(number, "");
            }
            int index = familyName.IndexOf("(");
            if (index > 0)
            {
                return familyName.Substring(0, index);
            }
            index = familyName.IndexOf("（");
            if (index > 0)
            {
                return familyName.Substring(0, index);
            }
            return familyName;
        }



    }
}
