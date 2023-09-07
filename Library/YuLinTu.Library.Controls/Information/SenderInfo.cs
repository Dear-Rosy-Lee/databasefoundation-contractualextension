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
using YuLinTu.Windows;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 发包方提示信息定义
    /// </summary>
    public class SenderInfo
    {
        #region Files - Const

        /// <summary>
        /// 删除发包方
        /// </summary>
        public const string SenderDel = "删除发包方";

        /// <summary>
        /// 添加发包方
        /// </summary>
        public const string SenderAdd = "添加发包方";

        /// <summary>
        /// 无地域数据
        /// </summary>
        public const string AddNoZone = "请选择添加数据所在的地域!";

        /// <summary>
        /// 发包方名称重复
        /// </summary>
        public const string SenderNameRepeat = "该发包方名称已在系统中存在!";

        /// <summary>
        /// 添加组下级发包方
        /// </summary>
        public const string AddZoneError = "只能在镇、村、组地域下添加数据!";

        /// <summary>
        /// 添加发包方数据失败
        /// </summary>
        public const string AddDataFaile = "添加发包方数据失败!";

        /// <summary>
        /// 删除发包方数据失败
        /// </summary>
        public const string DelDataFaile = "删除发包方数据失败!";

        /// <summary>
        /// 导出调查表
        /// </summary>
        public const string ExportTable = "导出调查表";

        /// <summary>
        /// 导出模板
        /// </summary>
        public const string ExportTemplate = "导出模板";

        /// <summary>
        /// 导出模板不存在
        /// </summary>
        public const string ExportTemplateNotExit = "导出模板不存在!";

        /// <summary>
        /// 导出发包方数据
        /// </summary>
        public const string ExportData = "导出发包方数据";

        /// <summary>
        /// 导出发包方数据描述
        /// </summary>
        public const string ExportDataComment = "导出{0}下所有发包方数据";

        /// <summary>
        /// 请选择导出发包方数据的地域
        /// </summary>
        public const string ExportNoZone = "请选择导出发包方数据的地域!";

        /// <summary>
        /// 导入发包方调查表
        /// </summary>
        public const string ImportData = "导入发包方调查表";

        /// <summary>
        /// 导入发包方调查表描述
        /// </summary>
        public const string ImportDataComment = "导入发包方调查表中发包方数据";

        /// <summary>
        /// 导出地域下的发包方
        /// </summary>
        public const string ExportWord = "导出发包方数据为Word文档";

        /// <summary>
        /// 导出地域下的发包方
        /// </summary>
        public const string ExportExcel = "导出发包方数据为Excel文档";

        /// <summary>
        /// 导出地域下的发包方
        /// </summary>
        public const string ExportWordComment = "导出{0}下所有发包方数据为Word文档";

        /// <summary>
        /// 导出发包方数据无上级
        /// </summary>
        public const string ExportDataNoUp = "请选择导出发包方数据的根级发包方!";

        /// <summary>
        /// 默认发包方不能删除
        /// </summary>
        public const string ForbidDelDefault = "默认发包方不能删除!";

        /// <summary>
        /// 删除发包方
        /// </summary>
        public const string DelNull = "请选择一个发包方进行删除!";

        /// <summary>
        /// 删除发包方?
        /// </summary>
        public const string DelAffirm = "确定删除选择发包方?";

        /// <summary>
        /// 编辑发包方数据
        /// </summary>
        public const string SenderEdit = "编辑发包方";

        /// <summary>
        /// 编辑发包方数据为空
        /// </summary>
        public const string EditNull = "请选择需要编辑的发包方!";

        /// <summary>
        /// 更新发包方数据失败
        /// </summary>
        public const string EditFail = "更新发包方数据失败!";

        /// <summary>
        /// 发包方号码不正确
        /// </summary>
        public const string SenderNumberError = "发包方证件号码不正确!";

        /// <summary>
        /// 请输入发包方名称
        /// </summary>
        public const string SenderNameError = "请输入发包方名称!";

        /// <summary>
        /// 邮政编码只能是6位数字
        /// </summary>
        public const string SenderPosterNumberError = "邮政编码只能是6位数字!";

        /// <summary>
        /// 电话号码只能由数字组成
        /// </summary>
        public const string SenderPoneNumberError = "电话号码只能由数字组成!";

        /// <summary>
        /// 证件号码只能由数字组成
        /// </summary>
        public const string SenderCardNumberError = "证件号码只能由数字组成!";

        /// <summary>
        ///身份证号码只能由15位或18位数字组成
        /// </summary>
        public const string SenderIdentityCardNumberError = "身份证号码不合法!";

        /// <summary>
        ///发包方调查日期应小于审核日期
        /// </summary>
        public const string SenderDateError = "发包方调查日期应小于审核日期!";

        /// <summary>
        /// 导出数据时应选择镇级(包括镇)地域以下
        /// </summary>
        public const string SelectedZoneError = "请选择在镇级以下(包括镇)地域批量导出!";

        #endregion
    }
}
