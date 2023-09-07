/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包权证模块消息名称定义
    /// </summary>
    public class ContractRegeditBookMessage
    {
        #region Fields-Const

        /// <summary>
        /// 承包权证添加完成
        /// </summary>
        public const string CONTRACTREGEDITBOOK_GET_COMPLATE = "ContractRegeditBook_Get_Complate";

        /// <summary>
        /// 清空数据完成
        /// </summary>
        public const string CLEAR_COMPLATE = "Clear_Complate";
        
        /// <summary>
        /// 刷新界面
        /// </summary>
        public const string ContractRegeditBook_Refresh = "ContractRegeditBook_Refresh";

        #endregion
    }
}
