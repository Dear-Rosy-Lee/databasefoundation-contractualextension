/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
    /// 二轮台账模块消息定义
    /// </summary>
    public class SecondTableLandMessage
    {
        #region const

        /// <summary>
        /// 二轮台账承包方编辑
        /// </summary>
        public const string SECONDPERSON_EDIT_COMPLATE = "SecondPerson_Edit_Complate";

        /// <summary>
        /// 二轮台账承包方增加
        /// </summary>
        public const string SECONDPERSON_ADD_COMPLETE = "SecondPerson_Add_Complate";

        /// <summary>
        /// 二轮台账承包方删除
        /// </summary>
        public const string SECONDPERSON_DELT_COMPLETE = "SecondPerson_Delt_Complate";

        /// <summary>
        /// 二轮台账承包方设置
        /// </summary>
        public const string SECONDPERSON_SET_COMPLATE = "SecondPerson_Set_Complate";

        /// <summary>
        /// 二轮台账字典获取
        /// </summary>
        public const string SECONDLAND_GET_DICTIONARY = "SecondLand_Get_Dictionary";

        /// <summary>
        /// 二轮台账调查表导入完成
        /// </summary>
        public const string SECONDLAND_IMPORT_COMPLETE = "SecondLand_Import_Complete";

        /// <summary>
        /// 清空二轮台账当前地域下的所有地块数据
        /// </summary>
        public const string SECONDLAND_CLEAR_COMPLETE = "SecondLand_Clear_Complete";

        /// <summary>
        /// 当前二轮台账导出表标题
        /// </summary>
        public const string CURRENTZONE_UNITNAME = "CurrentZone_UnitName";

        #endregion
    }
}
