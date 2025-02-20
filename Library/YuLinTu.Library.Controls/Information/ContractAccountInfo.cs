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
    /// 台账提示信息定义
    /// </summary>
    public class ContractAccountInfo
    {
        #region

        /// <summary>
        /// 下载数据
        /// </summary>
        public const string DownService = "下载数据";

        /// <summary>
        /// 上传数据
        /// </summary>
        public const string UpService = "上传数据";

        /// <summary>
        /// 地块信息
        /// </summary>
        public const string LandInfo = "地块信息";

        /// <summary>
        /// 地块编码已存在
        /// </summary>
        public const string LandNumberExist = "地块编码已在该地域中存在!";

        /// <summary>
        /// 实测面积
        /// </summary>
        public const string ActualAreaZero = "实测面积必需大于0";

        /// <summary>
        /// 地块编码
        /// </summary>
        public const string LandNumberEmpty = "地块编码不能为空";

        /// <summary>
        /// 审核日期不能小于调查日期
        /// </summary>
        public const string SurveyAndCheckDataError = "审核日期不能小于调查日期!";

        /// <summary>
        /// 在添加地块时没有选择承包方信息
        /// </summary>
        public const string LandAddNoSelectedPerson = "请选择添加地块的承包方!";

        /// <summary>
        /// 在修改添加地块时没有选择土地利用类型
        /// </summary>
        public const string LandAddNoContractLandUseType = "请选择土地利用类型!";

        /// <summary>
        /// 承包地块处理
        /// </summary>
        public const string ContractLandPro = "承包地块处理";

        /// <summary>
        /// 承包地块处理失败!
        /// </summary>
        public const string ContractLandProFail = "承包地块处理失败!";

        /// <summary>
        /// 锁定的承包方不能添加承包地块
        /// </summary>
        public const string ContractLandAddLock = "承包方被锁定,不能添加地块!";

        /// <summary>
        /// 锁定的承包方不能编辑承包地块
        /// </summary>
        public const string ContractLandEditLock = "承包方被锁定,不能编辑地块!";

        /// <summary>
        /// 锁定的承包方不能删除承包地块
        /// </summary>
        public const string ContractLandDelLock = "承包方被锁定,不能删除地块!";

        /// <summary>
        /// 添加地块时未获取承包方
        /// </summary>
        public const string ContractLandAddNoSelecteVp = "未获取添加地块的承包方!";

        /// <summary>
        /// 添加承包地块
        /// </summary>
        public const string ContractLandAdd = "添加地块";

        /// <summary>
        /// 编辑承包地块
        /// </summary>
        public const string ContractLandEdit = "编辑地块";

        /// <summary>
        /// 删除承包地块
        /// </summary>
        public const string ContractLandDel = "删除地块";

        /// <summary>
        /// 清空当前地域下的数据
        /// </summary>
        public const string ContractAccountClear = "清空数据";

        /// <summary>
        /// 清空当前地域下的数据失败
        /// </summary>
        public const string ContractAccountClearFail = "清空数据失败!";

        /// <summary>
        /// 初始化界址点和界址线数据失败
        /// </summary>
        public const string ContractAccountInitializeDotAndCoilFail = "初始化界址数据失败!";

        /// <summary>
        /// 工具中显示全部地块
        /// </summary>
        public const string AllLandVisibility = "显示全部地块";

        /// <summary>
        /// 工具中显示承包地块
        /// </summary>
        public const string ContractLandVisibility = "显示承包地块";

        /// <summary>
        /// 工具中显示自留地块
        /// </summary>
        public const string OwnerLandVisibility = "显示自留地块";

        /// <summary>
        /// 工具中显示机动地块
        /// </summary>
        public const string MotorLandVisibility = "显示机动地块";

        /// <summary>
        /// 工具中显示开荒地块
        /// </summary>
        public const string CultivateLandVisibility = "显示开荒地块";

        /// <summary>
        /// 工具中显示其他集体土地
        /// </summary>
        public const string OtherLandVisibility = "显示其他集体土地";

        /// <summary>
        /// 工具中显示经济地块
        /// </summary>
        public const string EconomyLandVisibility = "显示经济地块";

        /// <summary>
        /// 工具中显示饲料地块
        /// </summary>
        public const string FodderLandVisibility = "显示饲料地块";

        /// <summary>
        /// 工具中显示撂荒地块
        /// </summary>
        public const string WasteLandVisibility = "显示撂荒地块";

        /// <summary>
        /// 工具之初始数据
        /// </summary>
        public const string InitialData = "初始数据";

        /// <summary>
        /// 工具中的初始化承包地块属性信息
        /// </summary>
        public const string InitialLandInfo = "初始化地块基本属性数据";

        /// <summary>
        /// 工具中的初始化承包地块周边属性信息
        /// </summary>
        public const string InitialLandNeigborInfo = "初始化查找地块周边地块信息";

        /// <summary>
        /// 工具中的初始化承包地块实测面积和确权面积
        /// </summary>
        public const string InitialLandArea = "初始化地块面积";

        /// <summary>
        /// 工具中的截取承包地块实测面积和确权面积小数位
        /// </summary>
        public const string InitialLandAreaNumericFormat = "截取承包地块面积小数位";

        /// <summary>
        /// 工具中的初始化承包地块界址数据
        /// </summary>
        public const string InitialLandDotCoil = "初始化承包地块界址数据";

        /// <summary>
        /// 根据有效界址点工具中的初始化承包地块界址数据
        /// </summary>
        public const string InitialLandCoil = "初始化承包地块有效界址线数据";

        /// <summary>
        /// 工具中的初始化承包地块是否基本农田
        /// </summary>
        public const string InitialLandIsFarmer = "初始化地块是否基本农田";

        /// <summary>
        /// 工具中批量初始化承包地块属性信息时没有正确选择地域
        /// </summary>
        public const string InitialColumnSelectedZoneError = "批量初始化时请选择镇级(包括镇级)以下的地域!";

        /// <summary>
        /// 工具中检索地块名称为空值
        /// </summary>
        public const string LandNameNullVisibility = "检索地块名称为空";

        /// <summary>
        /// 工具中检索二轮合同面积为空值
        /// </summary>
        public const string ContractAreaNullVisibility = "检索二轮合同面积为空";

        /// <summary>
        /// 工具中检索实测面积为空值
        /// </summary>
        public const string ActualAreaNullVisibility = "检索实测面积为空";

        /// <summary>
        /// 工具中检索确权面积为空值
        /// </summary>
        public const string AwareAreaNullVisibility = "检索确权面积为空";

        /// <summary>
        /// 工具中检索基本农田为空值
        /// </summary>
        public const string FarmerLandNullVisibility = "检索基本农田为空";

        /// <summary>
        /// 工具中检索地力等级为空值
        /// </summary>
        public const string LandLevelNullVisibility = "检索地力等级为空";

        /// <summary>
        /// 工具中检索地块图斑为空值
        /// </summary>
        public const string LandShapeNullVisibility = "检索的地块图斑为空";

        /// <summary>
        /// 空间查看
        /// </summary>
        public const string FindGeometryTool = "空间查看";

        /// <summary>
        /// 工具中进行空间查看时没有选择正确的数据
        /// </summary>
        public const string FindGeometryNoSelectedItem = "请选择正确数据后再进行此操作!";

        /// <summary>
        /// 当前选择数据无空间信息
        /// </summary>
        public const string FindNoGeometryUnderCurrentSelected = "当前选择数据无空间信息!";

        /// <summary>
        /// 清空数据时确定是否执行清空承包方和承包地数据操作
        /// </summary>
        public const string ContractAccountAllClearSure = "是否清空当前地域及子级地域的所有承包方和承包地数据?";

        /// <summary>
        /// 清空数据时确定是否执行清空承包地数据操作
        /// </summary>
        public const string ContractAccountLandClearSure = "是否清空当前地域及子级地域的所有承包地数据?";

        /// <summary>
        /// 在清空承包方数据时当前地域下不存在承包方信息
        /// </summary>
        public const string CurrentZoneNoPerson = "当前地域下未获取没有锁定的承包方数据!";

        /// <summary>
        /// 在清空承包方数据时当前地域下不存在承包方信息
        /// </summary>
        public const string CurrentZoneNoPersonData = "当前地域下没有承包方数据!";

        /// <summary>
        /// 在清空地块数据时当前地域下不存在承包地块信息
        /// </summary>
        public const string CurrentZoneNoLand = "当前地域下没有地块数据!";

        /// <summary>
        /// 当前地域下地块无调查编码
        /// </summary>
        public const string CurrentZoneLandNoSurveyNumber = "当前地域下地块数据无调查编码!";

        /// <summary>
        /// 当前地域下没有空间地块数据
        /// </summary>
        public const string CurrentZoneNoGeoLand = "当前地域下没有空间地块数据!";

        /// <summary>
        /// 当前地域下(包括子级地域)没有空间地块数据
        /// </summary>
        public const string SubAndSelfZoneNoGeoLand = "当前地域下(包括子级地域)没有空间地块数据";

        /// <summary>
        /// 没有选择当前地域信息
        /// </summary>
        public const string CurrentZoneNoSelected = "未选择行政地域!";

        /// <summary>
        /// 添加承包地块时当前选择的地域不是村或组
        /// </summary>
        public const string ZoneSelectedErrorForAdd = "请在村、组下添加承包地块信息!";

        /// <summary>
        /// 编辑承包地块时没有选择目标地块
        /// </summary>
        public const string LandEditNoSelected = "请选择需要编辑的承包地块!";

        /// <summary>
        /// 删除承包地块时没有选择目标地块
        /// </summary>
        public const string LandDelNoSelected = "请选择需要删除的承包地块!";

        /// <summary>
        /// 确定是否删除当前选择的地块信息
        /// </summary>
        public const string CurrentLandDelSure = "是否删除当前选择地块信息?";

        /// <summary>
        /// 导入地块调查表中地块数据
        /// </summary>
        public const string ImportDataComment = "导入地块调查表中地块数据";

        /// <summary>
        /// 导入地块调查表
        /// </summary>
        public const string ImportData = "导入地块调查表";

        /// <summary>
        /// 导入调查表失败
        /// </summary>
        public const string ImportDataFail = "导入调查表失败!";

        /// <summary>
        /// 导入地域级别不正确
        /// </summary>
        public const string ImportErrorZone = "请在村、组级地域下导入数据!";

        /// <summary>
        /// 选择导入地域
        /// </summary>
        public const string ImportZone = "选择导入地域";

        /// <summary>
        /// 选择批量导入承包方数据的地域
        /// </summary>
        public const string VolumnImportLandData = "批量导入地块数据";

        /// <summary>
        /// 请选择在镇级以下(包括镇)地域批量导出
        /// </summary>
        public const string VolumnExportZoneError = "请选择在镇级以下(包括镇)地域批量导出!";

        /// <summary>
        /// 导出调查表
        /// </summary>
        public const string ExportTable = "导出调查表";

        /// <summary>
        /// 批量导出调查表
        /// </summary>
        public const string VolumnExportDataTable = "批量导出调查表";

        /// <summary>
        /// 导入数据
        /// </summary>
        public const string ImportShpData = "导入矢量调查图斑";

        /// <summary>
        /// 导入地块矢量图斑空间数据
        /// </summary>
        public const string ImportShapeData = "导入地块矢量图斑空间数据";

        /// <summary>
        /// 批量导入数据时选择的地域级别不是村
        /// </summary>
        public const string VolumnImportErrorZone = "请在村级地域下(包括村)批量导入数据!";

        /// <summary>
        /// 批量导入数据时选择的地域级别不是区县
        /// </summary>
        public const string VolumnImportErrorCountyZone = "请在区县级地域下(包括区县)批量初始化数据!";

        /// <summary>
        /// 批量导入调查表
        /// </summary>
        public const string VolumnImportDataTable = "批量导入调查表";

        /// <summary>
        /// 批量导入调查表失败
        /// </summary>
        public const string VolumnImportFail = "批量导入调查表失败";

        #region 导出

        /// <summary>
        /// 导出调查报表
        /// </summary>
        public const string ExportSurveyTableData = "导出调查报表";

        /// <summary>
        /// 导出数据
        /// </summary>
        public const string ExportData = "导出数据";

        /// <summary>
        /// 导出调查表Word
        /// </summary>
        public const string ExportDataWord = "导出承包方调查表数据";

        /// <summary>
        /// 导出单个调查表Word时没有选择承包方数据
        /// </summary>
        public const string ExportVPWordNoSelected = "请选择组下要导出承包方Word调查表的承包方!";

        /// <summary>
        /// 导出公示表调查表Word
        /// </summary>
        public const string ExportPublicDataWord = "导出公示结果归户表";

        /// <summary>
        /// 导出单个公示结果归户表时没有选择承包方
        /// </summary>
        public const string ExportPublishDataWordNoSelected = "请选择组下要导出公示结果归户表的承包方!";

        /// <summary>
        /// 导出调查表Excel
        /// </summary>
        public const string ExportDataExcel = "导出调查表";

        /// <summary>
        /// 导出承包地块调查表Excel
        /// </summary>
        public const string ExportContractLandSurveyExcel = "承包地块调查表";

        /// <summary>
        /// 导出承包台账调查表Excel
        /// </summary>
        public const string ExportContractAccountSurveyExcel = "承包台账调查表";

        /// <summary>
        /// 导出台账调查表-土地承包经营权调查公示表
        /// </summary>
        public const string ExportLandSurveyInFoPubTable = "土地承包经营权调查信息公示表";

        /// <summary>
        /// 导出台账调查表-土地承包经营权登记公示表Excel
        /// </summary>
        public const string ExportLandRegPubTable = "土地承包经营权登记公示表";

        /// <summary>
        /// 导出台账调查表-土地承包经营权签字表
        /// </summary>
        public const string ExportRegSignTable = "土地承包经营权签字表";

        /// <summary>
        /// 导出台账调查表-土地承包经营权村组公示表
        /// </summary>
        public const string ExportVillageGroupTable = "土地承包经营权村组公示表";

        /// <summary>
        /// 导出台账调查表-土地承包经营权单户确认表
        /// </summary>
        public const string ExportFamilyConfirmTable = "土地承包经营权单户确认表";

        /// <summary>
        /// 导出台账调查表-土地承包经营权单户调查表
        /// </summary>
        public const string ExportLandSingleSurveyTable = "土地承包经营权单户调查表";

        /// <summary>
        /// 导出村组公示公告Word
        /// </summary>
        public const string ExportVillagesDeclare = "导出村组公示公告";

        /// <summary>
        /// 导出村组公示公告表描述
        /// </summary>
        public const string ExportVillageDeclareDes = "导出农村土地承包经营权村组公示公告表";

        /// <summary>
        /// 批量导出村组公示公告Word
        /// </summary>
        public const string VolumnExportVillageDeclare = "批量导出村组公示公告";

        /// <summary>
        /// 导出无地域
        /// </summary>
        public const string ExportNoZone = "请选择导出数据所在行政区域!";

        /// <summary>
        /// 导出无地域（发包方）
        /// </summary>
        public const string ExportSenderNoZone = "请选择导出发包方所在行政区域!";

        /// <summary>
        /// 选择数据导出
        /// </summary>
        public const string ExportDataNo = "请选择一条数据进行导出!";

        /// <summary>
        /// 数据未定合同
        /// </summary>
        public const string ExportNoConcord = "选择数据未签订合同";

        /// <summary>
        /// 选择数据导出
        /// </summary>
        public const string ExportLandDataNo = "请选择一条地块进行导出!";

        /// <summary>
        /// 导出数据汇总表描述
        /// </summary>
        public const string ExportSummaryDataExcel = "导出农村土地承包经营权数据汇总表";

        /// <summary>
        /// 请选择在镇级以下(包括镇)地域导出
        /// </summary>
        public const string ExportZoneError = "请选择在镇级以下(包括镇)地域导出!";

        #endregion 导出

        /// <summary>
        /// 编码详情
        /// </summary>
        public const string LandNumberDetail = "编码详情";

        /// <summary>
        /// 没有缺失的地块编码顺序号
        /// </summary>
        public const string LandNumberNoMissing = "没有缺失的地块编码顺序号";

        #region 导出 - 地块

        /// <summary>
        /// 导出地块Excel调查表数据
        /// </summary>
        public const string ExportLandDataExcel = "导出地块Excel调查表数据";

        /// <summary>
        /// 导出地块Excel调查表数据
        /// </summary>
        public const string ExportPublishDataExcel = "导出地块Excel公示表数据";

        /// <summary>
        /// 导出发包方Excel调查表数据
        /// </summary>
        public const string ExportSenderDataExcel = "导出发包方调查表数据";

        public const string ExportContractInformationExcel = "导出农村土地承包经营权合同信息表";

        /// <summary>
        /// 导出承包方Excel调查表数据
        /// </summary>
        public const string ExportContractDataExcel = "导出承包方调查表数据";

        /// <summary>
        /// 导出地块调查表Word
        /// </summary>
        public const string ExportLandDataWord = "导出地块调查表数据";

        /// <summary>
        /// 导出地块调查表Word没有选择承包方
        /// </summary>
        public const string ExportLandDataWordNoSelected = "请选择组下要导出地块调查表的承包方!";

        /// <summary>
        /// 导出调查信息公示表Excel数据
        /// </summary>
        public const string ExportLandSurveyInFoPubExcel = "导出调查信息公示表数据";

        /// <summary>
        /// 导出摸底调查核实表Excel数据
        /// </summary>
        public const string ExportLandVerifyExcel = "导出摸底调查核实表";

        /// <summary>
        /// 导出摸底调查核实表Excel数据(打印版)
        /// </summary>
        public const string ExportLandVerifyPrintExcel = "导出摸底调查核实表(打印版)";

        /// <summary>
        /// 导出界址信息Excel数据
        /// </summary>
        public const string ExportBoundaryInfoExcel = "导出界址调查表数据";

        /// <summary>
        /// 批量导出界址信息Excel数据
        /// </summary>
        public const string VolumnExportBoundaryInfoExcel = "批量导出界址调查表数据";

        #endregion 导出 - 地块

        /// <summary>
        /// 查找四至
        /// </summary>
        public const string SeekLandNeighbor = "查找四至";

        /// <summary>
        /// 查找四至的地域级别大于镇
        /// </summary>
        public const string SeekLandNeighborErrorZone = "请在镇级地域下查找四至!";

        /// <summary>
        /// 导出地块示意图
        /// </summary>
        public const string ExportMultiParcelOfFamily = "导出地块示意图";

        /// <summary>
        /// 预览地块示意图
        /// </summary>
        public const string PreviewMultiParcelOfFamily = "预览地块示意图";

        /// <summary>
        /// 导出地块示意图失败
        /// </summary>
        public const string ExportMultiParcelOfFamilyFail = "导出地块示意图失败!";

        /// <summary>
        /// 导出单户地块示意图描述
        /// </summary>
        public const string ExportMultiParcelOfFamilyDesc = "导出农村土地承包经营权标准地块示意图";

        /// <summary>
        /// 导出地块示意图时没有选择承包方信息
        /// </summary>
        public const string ExportMultiParcelNoSelected = "请选择一户导出地块示意图!";

        /// <summary>
        /// 导出地块示意图所选承包方没有空间地块
        /// </summary>
        public const string CurrentPersonNoGeoLands = "当前承包方没有空间地块!";

        /// <summary>
        /// 导出地块示意图所选承包方没有空间地块-按照选项配置
        /// </summary>
        public const string CurrentPersonNoGeoLandsBySetting = "当前承包方没有空间地块，请检查选项配置";

        /// <summary>
        /// 导入界址点图斑
        /// </summary>
        public const string ImportBoundaryAddressDot = "导入界址点图斑";

        /// <summary>
        /// 导出Shape图斑
        /// </summary>
        public const string ExportLandShapeData = "地块图斑Shape数据";

        /// <summary>
        /// 导出界址点图斑
        /// </summary>
        public const string ExportLandDotShapeData = "地块界址点Shape数据";

        /// <summary>
        /// 导出界址线图斑
        /// </summary>
        public const string ExportLandCoilShapeData = "地块界址线Shape数据";

        /// <summary>
        /// 导入界址调查表
        /// </summary>
        public const string ImportBoundaryData = "导入界址调查表";

        /// <summary>
        /// 导入界址线图斑
        /// </summary>
        public const string ImportBoundaryAddressCoil = "导入界址线图斑";

        /// <summary>
        /// 导出界址点成果表
        /// </summary>
        public const string ExportDotResultExcel = "导出界址点成果表";

        /// <summary>
        /// 导入土地调查压缩包
        /// </summary>
        public const string ImportLandZipData = "导入土地调查压缩包";

        /// <summary>
        /// 导出土地调查压缩包
        /// </summary>
        public const string ExportLandZipData = "导出土地调查压缩包";

        /// <summary>
        /// 未选择户信息
        /// </summary>
        public const string ExportDotResultExcelNoSelectedPerson = "请选择户信息导出界址点成果表";

        /// <summary>
        /// 初始化图幅编号
        /// </summary>
        public const string InitialImageNumber = "初始化图幅编号";

        /// <summary>
        /// 按照角度过滤界址点时未填写最大(或最小)角度值
        /// </summary>
        public const string AngleFilterNoAngleValue = "按照角度过滤界址点时,请填写最小(或最大)角度值!";

        /// <summary>
        /// 按照角度过滤界址点时填写最小角度值大于最大角度值
        /// </summary>
        public const string AngleFilterValueError = "按照角度过滤界址点时,最小角度值应小于最大角度值!";

        /// <summary>
        /// 未输入邻宗地距离值
        /// </summary>
        public const string NeighborDistanceValueError = "请输入邻宗地距离值!";

        /// <summary>
        /// 预览时未选择数据
        /// </summary>
        public const string ViewDataNo = "请选择一条数据进行预览";

        #endregion
    }
}