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
using YuLinTu.Windows;


namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 二轮台账提示信息定义
    /// </summary>
    public class SecondAccountInfo
    {
        #region Files - Const

        #region

        /// <summary>
        /// 删除承包方
        /// </summary>
        public const string DelVirtualPerson = "删除承包方";

        /// <summary>
        /// 删除承包方失败
        /// </summary>
        public const string DelVPersonFail = "删除承包方失败!";

        /// 删除数据
        /// </summary>
        public const string DelData = "删除数据";

        /// <summary>
        /// 选择删除数据
        /// </summary>
        public const string DelDataNo = "请选择一条数据进行删除!";

        /// <summary>
        /// 编辑数据
        /// </summary>
        public const string EditData = "编辑数据";

        /// <summary>
        /// 选择编辑数据
        /// </summary>
        public const string EditDataNo = "请选择一条数据进行编辑!";

        /// <summary>
        ///  确定删除此承包方
        /// </summary>
        public const string DelVPersonWarring = "确定删除此承包方?";

        /// <summary>
        /// 清空数据
        /// </summary>
        public const string Clear = "清空地块数据";

        /// <summary>
        /// 清空数据确定
        /// </summary>
        public const string ClearConfirm = "确定清空当前地域下的地块数据?";

        /// <summary>
        /// 邮政编码不正确
        /// </summary>
        public const string ZipCodeError = "邮政编码不正确";

        /// <summary>
        /// 选择导入地域
        /// </summary>
        public const string ImportNoZone = "请选择导入调查表所在行政区域!";

        /// <summary>
        /// 导入地域级别不正确
        /// </summary>
        public const string ImportErrorZone = "请在村、组级地域下导入数据!";

        /// <summary>
        /// 导入二轮台账调查表描述
        /// </summary>
        public const string ImportDataComment = "导入二轮台账调查表中地块数据";

        /// <summary>
        /// 导入台账调查表
        /// </summary>
        public const string ImportData = "导入台账调查表";

        /// <summary>
        /// 导入台账调查表失败
        /// </summary>
        public const string ImportDataFail = "导入台账调查表失败!";

        /// <summary>
        /// 没选择地域
        /// </summary>
        public const string ExportNoZone = "没选择地域";

        /// <summary>
        /// 导出数据
        /// </summary>
        public const string ExportData = "导出数据";

        #endregion

        #region 二轮台账

        /// <summary>
        /// 导出二轮台账摸底调查描述
        /// </summary>
        public const string ExportPublicityExcel = "导出二轮台账摸底调查公示表数据描述";

        /// <summary>
        /// 导出二轮台账摸底调查公示表描述
        /// </summary>
        public const string ExportRealQueryExcel = "导出二轮台账摸底调查表数据描述";

        /// <summary>
        /// 导出二轮台账用户确认表描述
        /// </summary>
        public const string ExportIdentifyExcel = "导出二轮台账摸底调查公示确认表数据描述";

        /// <summary>
        /// 导出二轮台账摸底调查公示表描述
        /// </summary>
        public const string ExportUserIdentifyExcel = "导出二轮台账用户确认表数据描述";

        /// <summary>
        /// 二轮台账导出摸底调查表
        /// </summary>
        public const string ExportRealQueryTable = "二轮台账导出摸底调查表";

        /// <summary>
        /// 二轮台账导出摸底调查公示表
        /// </summary>
        public const string ExportPublicityTable = "二轮台账导出摸底调查公示表";

        /// <summary>
        /// 二轮台账导出摸底调查公示确认表
        /// </summary>
        public const string ExportIdentifyTable = "二轮台账导出摸底调查公示确认表";

        /// <summary>
        /// 二轮台账用户确认表
        /// </summary>
        public const string ExportUserIdentifyTable = "二轮台账导出用户确认表";

        /// <summary>
        /// 导出二轮台账勘界确权调查表描述
        /// </summary>
        public const string ExportBoundarySettleExcel = "导出二轮台账Excel勘界确权数据";

        /// <summary>
        /// 二轮台账勘界确权调查表
        /// </summary>
        public const string ExportBoundaryTable = "二轮台账勘界确权调查表";

        /// <summary>
        /// 导出二轮台账单户确权调查表描述
        /// </summary>
        public const string ExportSingleFamilyExcel = "导出二轮台账Excel单户确权数据";

        /// <summary>
        /// 二轮台账单户确权调查表
        /// </summary>
        public const string ExportSingleFamilyTable = "二轮台账单户确权调查表";

        /// <summary>
        /// 是否导入二轮台账摸底调查表数据
        /// </summary>
        public const string ImportTableDataSure = "是否导入二轮台账摸底调查表数据?";

        /// <summary>
        /// 当前选择地域下无数据
        /// </summary>
        public const string ExportNoData = "当前选择地域下无数据!";

        #endregion

        #region 地块

        /// <summary>
        /// 添加二轮地块
        /// </summary>
        public const string SecondLandAdd = "添加二轮台账地块";

        /// <summary>
        /// 当前地域下没有承包方信息
        /// </summary>
        public const string ZoneNoVirtualPerson = "当前地域下没有承包方信息,无法添加地块!";

        /// <summary>
        /// 编辑二轮地块
        /// </summary>
        public const string SecondLandEdit = "编辑二轮台账地块";

        /// <summary>
        /// 没有选择待编辑地块
        /// </summary>
        public const string LandEditSelected = "请选择待编辑地块!";

        /// <summary>
        /// 删除二轮台账地块
        /// </summary>
        public const string SecondLandDel = "删除二轮台账地块";

        /// <summary>
        /// 没有选择待删除地块
        /// </summary>
        public const string LandDelSelected = "请选择待删除地块!";

        /// <summary>
        /// 确定删除选择的地块信息
        /// </summary>
        public const string LandDelConfirm = "确定删除选择的地块信息?";

        /// <summary>
        /// 二轮地块处理
        /// </summary>
        public const string SecondLandPro = "二轮地块处理";

        /// <summary>
        /// 二轮地块台账面积不能为零
        /// </summary>
        public const string TableAreaNoZero = "二轮地块台账面积不能为零!";

        /// <summary>
        /// 请输入地块台账面积
        /// </summary>
        public const string EnterTableArea = "请输入地块台账面积!";

        /// <summary>
        /// 二轮地块处理失败
        /// </summary>
        public const string SecondLandProFail = "二轮地块添加(更新)失败";

        #endregion

        #endregion
    }
}
