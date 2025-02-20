/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包方模块消息定义
    /// </summary>
    public class VirtualPersonMessage
    {
        #region Const

        /// <summary>
        /// 获取地域下承包方
        /// </summary>
        public const string VIRTUALPERSON_GEEBYZONE = "VirtualPerson_GetByZone";

        /// <summary>
        /// 添加承包方完成
        /// </summary>
        public const string VIRTUALPERSON_ADD_COMPLATE = "VirtualPerson_Add_Complate";

        /// <summary>
        /// 编辑承包方完成
        /// </summary>
        public const string VIRTUALPERSON_EDIT_COMPLATE = "VirtualPerson_Edit_Complate";

        /// <summary>
        /// 添加共有人完成
        /// </summary>
        public const string SHAREPERSON_ADD_COMPLATE = "SharePerson_Add_Complate";

        /// <summary>
        /// 编辑共有人完成
        /// </summary>
        public const string SHAREPERSON_EDIT_COMPLATE = "SharePerson_Edit_Complate";

        /// <summary>
        /// 删除承包方完成
        /// </summary>
        public const string VIRTUALPERSON_DEL_COMPLATE = "VirtualPerson_Del_Complate";

        /// <summary>
        /// 删除共有人完成
        /// </summary>
        public const string SHAREPERSON_DEL_COMPLATE = "SharePerson_Del_Complate";

        /// <summary>
        /// 清空数据完成
        /// </summary>
        public const string CLEAR_COMPLATE = "Clear_Complate";

        /// <summary>
        /// 承包方所在地域名称
        /// </summary>
        public const string VIRTUALPERSON_ZONENAME = "VirtualPerson_ZoneName";

        /// <summary>
        /// 承包方所在单位名称
        /// </summary>
        public const string VIRTUALPERSON_UNITNAME = "VirtualPerson_UinitName";

        /// <summary>
        /// 承包方锁定状态改变
        /// </summary>
        public const string VIRTUALPERSON_STATUSCHANGE = "VirtualPerson_StatusChange";

        /// <summary>
        /// 设置户主完成
        /// </summary>
        public const string VIRTUALPERSON_SET_COMPLATE = "VirtualPerson_Set_Complate";

        /// <summary>
        /// 分户完成
        /// </summary>
        public const string VIRTUALPERSON_SPLIT_COMPLATE = "VirtualPerson_Split_Complate";

        /// <summary>
        /// 合并承包方
        /// </summary>
        public const string VIRTUALPERSON_COMBINE_COMPLATE = "VirtualPerson_Combine_Complate";

        /// <summary>
        /// 初始化承包方
        /// </summary>
        public const string VIRTUALPERSON_INITIAL_COMPLATE = "VirtualPerson_Initial_Complate";

        /// <summary>
        /// 导入承包方数据完成
        /// </summary>
        public const string VIRTUALPERSON_IMPORT_COMPLETE = "VirtualPerson_Import_Complate";

        /// <summary>
        /// 导出承包方调查表Excel
        /// </summary>
        public const string VIRTUALPERSON_EXPORT_EXCEL = "VirtualPerson_Export_Excel";

        /// <summary>
        /// 导出承包方调查表Word
        /// </summary>
        public const string VIRTUALPERSON_EXPORT_WORD = "VIRTUALPERSON_EXPORT_WORD";

        #endregion
    }
}
