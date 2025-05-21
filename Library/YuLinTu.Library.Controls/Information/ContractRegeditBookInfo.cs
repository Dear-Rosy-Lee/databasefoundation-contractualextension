/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包权证信息
    /// </summary>
    public class ContractRegeditBookInfo
    {
        /// <summary>
        /// 清空数据
        /// </summary>
        public const string Clear = "清空数据";

        /// <summary>
        /// 清空数据
        /// </summary>
        public const string ClearConfirm = "确定清空当前地域下的权证数据?";

        /// <summary>
        /// 清空当前地域下的数据失败
        /// </summary>
        public const string ContractRegeditBookClearFail = "清空数据失败!";

        /// <summary>
        /// 当前地域下没有可供清空数据
        /// </summary>
        public const string ClearNoValidData = "当前地域下没有可供清空权证数据!";

        /// <summary>
        /// 删除数据
        /// </summary>
        public const string DelData = "删除数据";

        /// <summary>
        /// 选择删除数据
        /// </summary>
        public const string DelDataNo = "请选择一条权证进行删除!";

        /// <summary>
        /// 确定删除数据
        /// </summary>
        public const string DelDataWarring = "确定删除选择权证?";

        /// <summary>
        /// 添加数据无地域
        /// </summary>
        public const string AddDataNoZone = "请选择添加权证所在行政区域";

        /// <summary>
        /// 合同处理
        /// </summary>
        public const string ProcessConcordData = "权证处理";

        /// <summary>
        /// 村、组级地域下添加合同
        /// </summary>
        public const string AddDataInVillage = "请在村、组级地域下添加权证。";

        /// <summary>
        /// 承包方数据为空
        /// </summary>
        public const string VirtualPersonDataNull = "当前地域下需要签订权证的承包方数据为空";
        /// <summary>
        /// 地块数据为空
        /// </summary>
        public const string LandDataNull = "当前地域下需要签订权证的地块数据为空";

        /// <summary>
        /// 承包方数据全部都签订了合同
        /// </summary>
        public const string VirtualPersoAllHave = "当前地域下所有承包方都已签订权证";

        /// <summary>
        /// 未选中签订合同的耕地类型
        /// </summary>
        public const string NoLandTypeSelect = "至少选中一种签订权证的耕地类型";


        /// <summary>
        /// 导出调查表
        /// </summary>
        public const string ExportTable = "导出调查表";

        /// <summary>
        /// 导出数据
        /// </summary>
        public const string ExportData = "导出数据";

        /// <summary>
        /// 预览数据
        /// </summary>
        public const string PrintViewData = "预览数据";

        /// <summary>
        /// 请选择在镇级以下(包括镇)地域批量导出
        /// </summary>
        public const string VolumnExportZoneError = "请选择在镇级以下(包括镇)地域批量导出!";

        /// <summary>
        /// 导出无地域
        /// </summary>
        public const string ExportNoZone = "请选择导出数据所在行政区域!";

        /// <summary>
        /// 重置流水号无地域
        /// </summary>
        public const string ResetSerialNumberNoZone = "请选择将要重置流水号的行政区域!";

        /// <summary>
        /// 重置流水号选择的行政区域只能是县级行政单位
        /// </summary>
        public const string SelectZoneError = "必须选择市级以下行政单位!";

        /// <summary>
        /// 重置流水号选择的行政区域只能是县级行政单位
        /// </summary>
        public const string SetRangError = "流水号范围设置太小，请重新设置!";

        /// <summary>
        /// 权证导出任务名称
        /// </summary>
        public const string ExportWarrentData = "土地承包经营权管理";

        /// <summary>
        /// 工具之导出权证数据汇总表
        /// </summary>
        public const string ExportWarrentSummaryTable = "权证数据汇总表";

        /// <summary>
        /// 权证登记簿
        /// </summary>
        public const string ExportRegeditBookTable = "导出登记簿";

        /// <summary>
        /// 导出权证登记簿时没有选择承包方
        /// </summary>
        public const string ExportRegeditBookTableNoSelected = "请选择组下要导出登记簿的承包方!";

        /// <summary>
        /// 导出权证登记簿描述
        /// </summary>
        public const string ExportRegeditBookTableDesc = "导出农村土地承包经营权权证登记簿";

        /// <summary>
        /// 工具之导出权证数据汇总表描述
        /// </summary>
        public const string ExportWarrentSummaryTableDesc = "导出农村土地承包经营权权证数据汇总表";

        /// <summary>
        /// 导出表格时当前地域下没有合同数据
        /// </summary>
        public const string ExportTableDataNoConcords = "当前地域下不存在合同数据!";

        /// <summary>
        /// 导出表格时当前地域下没有权证数据
        /// </summary>
        public const string ExportTableDataNoWarrents = "当前地域下不存在权证数据!";

        /// <summary>
        /// 导出表格时当前地域下没有数据可供操作
        /// </summary>
        public const string ExportDataError = "当前地域下没有数据可供操作";

        /// <summary>
        /// 导出证书
        /// </summary>
        public const string ExportWarrant = "导出证书";

        /// <summary>
        /// 导出证书没有选择承包方信息
        /// </summary>
        public const string ExportWarrantNoSelected = "请选择组下要导出证书的承包方!";

        /// <summary>
        /// 导出证书
        /// </summary>
        public const string PrintViewWarrant = "预览证书";

        /// <summary>
        /// 当前地域下没有选择权证
        /// </summary>
        public const string PrintViewWarrantError = "当前地域下没有选择权证!";

        /// <summary>
        /// 导出证书
        /// </summary>
        public const string ExportWarrantWord = "导出农村土地承包经营权证";

        /// <summary>
        /// 工具之导出颁证清册
        /// </summary>
        public const string ExportAwareTable = "颁证清册";

        /// <summary>
        /// 工具之导出颁证清册描述
        /// </summary>
        public const string ExportAwareInventoryTableDesc = "导出农村土地承包经营权颁证清册";

        /// <summary>
        /// 工具之导出单户确认表
        /// </summary>
        public const string ExportSingleFamlilyConfirmTable = "单户确认表";

        /// <summary>
        /// 工具之导出单户确认表描述
        /// </summary>
        public const string ExportFamilyConfirmTableDesc = "导出农村土地承包经营权单户确认信息表";

        /// <summary>
        /// 未选择承包方信息
        /// </summary>
        public const string ExportFamilyConfirmNoSelected = "请至少选择一个承包方信息!";

        /// <summary>
        /// 权证数据处理之签订权证
        /// </summary>
        public const string InitialWarrantInfo = "初始化权证信息";

        /// <summary>
        /// 批量初始化权证信息
        /// </summary>
        public const string InitialWarrantInfoVolumn = "批量初始化权证信息";

        /// <summary>
        /// 批量初始化承包权证数据时没有正确选择地域
        /// </summary>
        public const string InitialBatchSelectedZoneError = "批量初始化时请选择镇级(包括镇级)以下的地域!";

        /// <summary>
        /// 权证数据处理
        /// </summary>
        public const string WarrantDataHandle = "权证数据处理";

        /// <summary>
        /// 在清空承包方数据时当前地域下不存在承包方信息
        /// </summary>
        public const string CurrentZoneNoPerson = "当前地域下没有承包方数据!";

        /// <summary>
        /// 在清空地块数据时当前地域下不存在承包地块信息
        /// </summary>
        public const string CurrentZoneNoLand = "当前地域下没有承包地块数据!";

        /// <summary>
        /// 没有选择当前地域信息
        /// </summary>
        public const string CurrentZoneNoSelected = "当前地域信息不存在,请选择地域!";

        /// <summary>
        /// 权证颁证日期应大于填写日期
        /// </summary>
        public const string WarrantDateError = "权证颁证日期应大于填写日期";

        /// <summary>
        /// 重置流水号
        /// </summary>
        public const string ResetSerialNumber = "重置流水号";

        #region 登记簿



        #endregion

    }
}
