/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包合同模块消息名称定义
    /// </summary>
    public class ConcordMessage
    {
        #region Fields-Const

        /// <summary>
        /// 获取数据字典
        /// </summary>
        public const string CONCORD_GET_DICTIONARY = "Concord_Get_Dictionary";

        /// <summary>
        /// 添加承包合同完成
        /// </summary>
        public const string CONCORD_ADD_COMPLATE = "Concord_Add_Complate";

        /// <summary>
        /// 编辑承包合同完成
        /// </summary>
        public const string CONCORD_EDIT_COMPLATE = "Concord_Edit_Complate";

        /// <summary>
        /// 删除承包合同完成
        /// </summary>
        public const string CONCORD_DELETE_COMPLATE = "Concord_Delete_Complate";

        /// <summary>
        /// 清空合同数据完成
        /// </summary>
        public const string CONCORD_CLEAR_COMPLATE = "Concord_Clear_Complate";

        /// <summary>
        /// 初始化合同数据完成
        /// </summary>
        public const string CONCORD_INITIALIZE_COMPLATE = "Concord_Initialize_Complate";

        /// <summary>
        /// 初始化合同数据完成
        /// </summary>
        public const string CONCORD_REFRESH = "Concord_Refresh";

        #endregion
    }
}
