/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包合同信息
    /// </summary>
    public class ContractConcordInfo
    {
        /// <summary>
        /// 添加合同
        /// </summary>
        public const string AddConcrod = "添加合同";

        /// <summary>
        /// 锁定的承包方不能添加合同
        /// </summary>
        public const string AddConcordLock = "承包方被锁定,不能添加合同!";

        /// <summary>
        /// 编辑合同
        /// </summary>
        public const string EditConcord = "编辑合同";

        /// <summary>
        /// 锁定的承包方不能编辑合同
        /// </summary>
        public const string EditConcordLock = "承包方被锁定,不能编辑合同!";

        /// <summary>
        /// 清空数据
        /// </summary>
        public const string Clear = "清空数据";

        /// <summary>
        /// 清空数据
        /// </summary>
        public const string ClearConfirm = "确定清空当前地域下的合同数据?";

        /// <summary>
        /// 当前地域下没有可供清空数据
        /// </summary>
        public const string ClearNoValidData = "当前地域下没有可供清空合同数据!";

        /// <summary>
        /// 清空当前地域下的数据失败
        /// </summary>
        public const string ContractConcordClearFail = "清空数据失败!";

        /// <summary>
        /// 删除数据
        /// </summary>
        public const string DelData = "删除合同";

        /// <summary>
        /// 锁定的承包方不能删除合同
        /// </summary>
        public const string DelDataLock = "承包方被锁定,不能删除合同!";

        /// <summary>
        /// 删除合同数据失败
        /// </summary>
        public const string DelDataFail = "删除合同数据失败!";

        /// <summary>
        /// 选择删除数据
        /// </summary>
        public const string DelDataNo = "请选择一条合同进行删除!";

        /// <summary>
        /// 确定删除数据
        /// </summary>
        public const string DelDataWarring = "确定删除选择合同?";

        /// <summary>
        /// 添加数据无地域
        /// </summary>
        public const string AddDataNoZone = "请选择添加合同所在行政区域!";

        /// <summary>
        /// 合同处理
        /// </summary>
        public const string ProcessConcordData = "合同处理";

        /// <summary>
        /// 编辑合同时没有选择待编辑对象
        /// </summary>
        public const string EditDataNoSelected = "请选择待编辑的合同!";

        /// <summary>
        /// 编辑合同时没有地块对应的合同
        /// </summary>
        public const string EditDataNoData = "没找到对应合同!";


        /// <summary>
        /// 村、组级地域下添加合同
        /// </summary>
        public const string AddDataInVillage = "请在村、组级地域下添加合同!";

        /// <summary>
        /// 承包方数据为空
        /// </summary>
        public const string VirtualPersonDataNull = "当前地域下需要签订合同的承包方数据为空";
        /// <summary>
        /// 地块数据为空
        /// </summary>
        public const string LandDataNull = "当前地域下需要签订合同的地块数据为空";

        /// <summary>
        /// 签订合同时所选地域下没有合同数据
        /// </summary>
        public const string ConcordDataNull = "当前地域下没有合同数据,无法进行签订合同操作!";

        /// <summary>
        /// 承包方数据全部都签订了合同
        /// </summary>
        public const string VirtualPersoAllHave = "当前地域下所有承包方都已签订合同";

        /// <summary>
        /// 未选中签订合同的耕地类型
        /// </summary>
        public const string NoLandTypeSelect = "至少选中一种签订合同的地块类别";

        /// <summary>
        /// 公示结果日期应大于调查日期
        /// </summary>
        public const string ResultAndSurveyDateError = "公示结果日期应大于记事日期";

        /// <summary>
        /// 公示审核日期应大于公示结果日期
        /// </summary>
        public const string CheckAndResultDateError = "公示审核日期应大于公示结果日期";

        /// <summary>
        /// 公示审核日期应大于公示记事日期
        /// </summary>
        public const string CheckAndSurveyDateError = "公示审核日期应大于记事日期";

        /// <summary>
        /// 承包期限结束日期应大于起始日期
        /// </summary>
        public const string ContractDateError = "承包期限结束日期应大于起始日期";

        /// <summary>
        /// 没有选择当前地域信息
        /// </summary>
        public const string CurrentZoneNoSelected = "当前地域信息不存在,请选择地域!";

        /// <summary>
        /// 合同数据处理之签订合同
        /// </summary>
        public const string InitialConcordInfo = "初始化合同信息";

        /// <summary>
        /// 初始化合同信息失败
        /// </summary>
        public const string InitialConcordInfoFail = "初始化合同信息失败!";

        /// <summary>
        /// 签订合同时没有选择承包方(集合)
        /// </summary>
        public const string InitialConcordInfoNoSelectedPerson = "请选择签订合同的承包方!";

        /// <summary>
        /// 批量初始化合同信息
        /// </summary>
        public const string InitialConcordInfoVolumn = "批量初始化合同信息";

        /// <summary>
        /// 批量初始化承包合同数据时没有正确选择地域
        /// </summary>
        public const string InitialBatchSelectedZoneError = "批量初始化时请选择镇级(包括镇级)以下的地域!";

        /// <summary>
        /// 合同数据处理
        /// </summary>
        public const string ConcordDataHandle = "合同数据处理";

        /// <summary>
        /// 合同数据处理之预览合同
        /// </summary>
        public const string PreviewConcord = "预览合同";

        /// <summary>
        /// 单条预览合同时没有选择合同信息
        /// </summary>
        public const string PreviewConcordNoSelected = "请选择一条合同进行预览!";

        /// <summary>
        /// 导出合同时没有选择合同信息
        /// </summary>
        public const string ExportConcordNoSelected = "请至少选择一条合同导出!";

        /// <summary>
        /// 合同处理之导出合同
        /// </summary>
        public const string ExportConcord = "导出合同";

        /// <summary>
        /// 成功导出单个选中的合同
        /// </summary>
        public const string ExportConcordComplete = "成功导出当前选中的合同.";

        /// <summary>
        /// 导出合同描述
        /// </summary>
        public const string ExportConcordDesc = "导出农村土地承包经营权承包合同";

        /// <summary>
        /// 当前地域下不存在承包方信息
        /// </summary>
        public const string CurrentZoneNoPersons = "当前地域下没有承包方信息!";

        /// <summary>
        /// 当前地域下不存在合同信息
        /// </summary>
        public const string CurrentZoneNoConcords = "当前地域下没有合同信息";

        /// <summary>
        /// 导出合同时选择地域大于镇级
        /// </summary>
        public const string ExportConcordSelectZoneError = "请在镇级(包括镇)地域以下进行导出合同数据操作!";

        /// <summary>
        /// 导出合同明细表
        /// </summary>
        public const string ExportConcordInformationTable = "导出合同明细表";

        /// <summary>
        /// 导出合同明细表标题
        /// </summary>
        public const string ExportConcordTableName = "土地承包经营权管理";

        /// <summary>
        /// 导出合同明细表描述
        /// </summary>
        public const string ExportConcordInfoDesc = "导出农村土地承包经营权合同明细表";

        /// <summary>
        /// 签订合同
        /// </summary>
        public const string InitialContractConcord = "签订合同";

        /// <summary>
        /// 在签订合同时当前地域下尚不存在合同数据
        /// </summary>
        public const string NoConcordsUnderCurrentZone = "当前地域下尚不存在合同,是否继续签订?";

    }
}
