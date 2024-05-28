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
    /// 发包方模块消息定义
    /// </summary>
    public class SenderMessage
    {
        #region Const
        /// <summary>
        ///  合并发包方
        /// </summary>
        public const string SENDER_COMBINE= "Sender_Combine";

        /// <summary>
        /// 添加发包方
        /// </summary>
        public const string SENDER_ADD = "Sender_Add";

        /// <summary>
        /// 添加发包方完成
        /// </summary>
        public const string SENDER_ADD_COMPLETE = "Sender_Add_Complete";

        /// <summary>
        /// 更新发包方
        /// </summary>
        public const string SENDER_UPDATE = "Sender_Update";

        /// <summary>
        /// 更新发包方完成
        /// </summary>
        public const string SENDER_UPDATE_COMPLETE = "Sender_Update_Complete";

        /// <summary>
        /// 删除发包方
        /// </summary>
        public const string SENDER_DELETE = "Sender_Delete";

        /// <summary>
        /// 删除发包方完成
        /// </summary>
        public const string SENDER_DELETE_COMPLETE = "Sender_Delete_Complete";

        /// <summary>
        /// 导入发包方调查表
        /// </summary>
        public const string SENDER_IMPORTTABLE = "Sender_ImportTalbe";

        /// <summary>
        /// 导入发包方调查表完成
        /// </summary>
        public const string SENDER_IMPORTTABLE_COMPLETE = "Sender_ImportTalbe_Complete";

        /// <summary>
        /// 导出发包方为Excel
        /// </summary>
        public const string SENDER_EXPORTEXCEL = "Sender_ExportExcel";

        /// <summary>
        /// 导出发包方Excel完成
        /// </summary>
        public const string SENDER_EXPORTEXCEL_COMPLETE = "Sender_ExportExcel_Complete";

        /// <summary>
        /// 导出发包方为Word
        /// </summary>
        public const string SENDER_EXPORTWORD = "Sender_ExportWord";

        /// <summary>
        /// 导出发包方Word完成
        /// </summary>
        public const string SENDER_EXPORTWORD_COMPLETE = "Sender_ExportWord_Complete";

        /// <summary>
        /// 导出发包方Excel模板
        /// </summary>
        public const string SENDER_EXCELTEMPLATE = "Sender_ExcelTemplate";

        /// <summary>
        /// 导出发包方Excel模板完成
        /// </summary>
        public const string SENDER_EXCELTEMPLATE_COMPLETE = "Sender_ExcelTemplate_Complete";

        /// <summary>
        /// 导出发包方Word模板
        /// </summary>
        public const string SENDER_WORDTEMPLATE = "Sender_WordTemplate";

        /// <summary>
        /// 导出发包方Word模板完成
        /// </summary>
        public const string SENDER_WORDTEMPLATE_COMPLETE = "Sender_WordTemplate_Complete";

        /// <summary>
        /// 获取地域下发包方数据
        /// </summary>
        public const string SENDER_GETDATA = "Sender_GetData";

        /// <summary>
        /// 获取地域及子地域下发包方数据
        /// </summary>
        public const string SENDER_GETCHILDRENDATA = "Sender_GetChildrenData";

        /// <summary>
        /// 获取发包方编码
        /// </summary>
        public const string SENDER_CREATCODE = "Sender_CreatCode";

        /// <summary>
        /// 发包方名称是否重复
        /// </summary>
        public const string SENDER_NAMEEXIT = "Sender_NameExit";

        /// <summary>
        /// 获取指定编码的发包方
        /// </summary>
        public const string SENDER_GET = "Sender_Get";

        /// <summary>
        /// 获取指定ID的发包方
        /// </summary>
        public const string SENDER_GET_ID = "Sender_Get_Id";

        /// <summary>
        /// 是否是默认发包方
        /// </summary>
        public const string SENDER_ISDEFAULT = "Sender_Isdefault";

        /// <summary>
        /// 地域下是否存在发包方
        /// </summary>
        public const string SENDER_EXITS = "Sender_Exists";

        /// <summary>
        /// 删除地域下发包方
        /// </summary>
        public const string SENDER_CLEARBYZONECODE = "Sender_ClearByZoneCode";

        /// <summary>
        /// 清空发包方
        /// </summary>
        public const string SENDER_CLEAR = "Sender_Clear";

        /// <summary>
        /// 更新地域时处理发包方
        /// </summary>
        public const string SENDER_UPDATEZONE = "Sender_UpdateZone";

        /// <summary>
        /// 导出发包方Excel
        /// </summary>
        public const string SENDER_EXPORT_COM = "Sender_Export";

        /// <summary>
        /// 导出发包方Word
        /// </summary>
        public const string SENDER_EXPORT_WORD = "Sender_Export_Word";

        #endregion
    }
}
