/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
namespace YuLinTu.Library.WorkStation
{

    /// <summary>
    /// 处理承包方名称
    /// </summary>
    public static class InitializeFamilyNameHelper
    {        
        /// <summary>
        /// 初始化户主名称
        /// </summary>
        /// <param name="familyName">承包方名称</param>
        public static string InitalizeFamilyName(this string familyName, bool except = true)
        {
            if (string.IsNullOrEmpty(familyName))
            {
                return "";
            }
            if(!except)
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
