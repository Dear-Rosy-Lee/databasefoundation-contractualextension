using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Entity
{
    public enum eCombineWordType
    {
        /// <summary>
        /// 权证与地块示意图合并
        /// </summary>
        [EnumName("权证与地块示意图合并")]
        WarrantAndParcel = 1,

        /// <summary>
        /// 登记簿与地块示意图合并
        /// </summary>
        [EnumName("登记簿与地块示意图合并")]
        RegisterBookAndParcel = 2,

        /// <summary>
        /// 权证与地块示意图扩展页合并
        /// </summary>
        [EnumName("权证与地块示意图扩展页合并")]
        ExtendWarrantAndParcel = 3
    }
}