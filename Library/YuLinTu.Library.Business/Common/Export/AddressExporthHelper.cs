using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
   public class AddressExporthHelper
    {
        /// <summary>
        /// 处理全地址到镇
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetNewAddressToTown(string value)
        {
            string ret = value;

            if (value.IsNullOrEmpty()) return value;

            if (value.Contains("区"))
            {
                var nameindex = value.IndexOf("区");
                ret = value.Substring(nameindex+1, value.Length - nameindex - 1);
            }
            else if (value.Contains("县"))
            {
                var nameindex = value.IndexOf("县");
                ret = value.Substring(nameindex+1, value.Length - nameindex - 1);
            }

            return ret;
        }
    }
}
