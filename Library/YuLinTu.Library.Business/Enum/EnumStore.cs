/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Resources;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 枚举值存储类
    /// </summary>
    public class EnumStore<T>
    {
        #region Properties

        /// <summary>
        /// 关键字名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public T Value { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 获取枚举值封装成EnumStore
        /// </summary>
        /// <returns></returns>
        public static List<EnumStore<T>> GetListByType()
        {
            List<EnumStore<T>> list = new List<EnumStore<T>>();
            try
            {
                EnumNameAttribute[] attributeArray = EnumNameAttribute.GetAttributes(typeof(T));
                for (int i = 0; i < attributeArray.Length; i++)
                {
                    EnumNameAttribute attribute = attributeArray[i];
                    list.Add(new EnumStore<T>() { DisplayName = attribute.Description, Value = (T)attribute.Value });
                }

                Type type = typeof(T);
                string[] names = Enum.GetNames(type);
                string[] array = names;
                foreach (string text in array)
                {
                    DisplayAttribute customAttribute = type.GetField(text).GetCustomAttribute<DisplayAttribute>();
                    if (customAttribute != null)
                    {
                        var cust = list.Find(t => t.Value.ToString() == text);
                        if (cust == null)
                            continue;
                        if (!cust.Value.ToString().Equals(cust.DisplayName))
                            continue;
                        if (cust.DisplayName != customAttribute.Name)
                            cust.DisplayName = customAttribute.Name;
                    }
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteError(null, "GetListByType(获取枚举值封装成EnumStore)", ex.Message + ex.StackTrace);
            }
            return list;
        }

        #endregion
    }
}