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
    /// 承包方提示信息定义
    /// </summary>
    public class VirtualPersonInfo
    {
        #region Files - Const

        /// <summary>
        /// 添加承包方
        /// </summary>
        public const string AddVirtualPerson = "添加承包方";

        /// <summary>
        /// 添加承包方失败
        /// </summary>
        public const string AddVPFail = "添加承包方失败!";

        /// <summary>
        /// 编辑承包方
        /// </summary>
        public const string EditVirtualPerson = "编辑承包方";

        /// <summary>
        /// 编辑承包方失败
        /// </summary>
        public const string EditVPFail = "编辑承包方失败!";

        /// <summary>
        /// 添加共有人无发包方
        /// </summary>
        public const string AddNoVirtual = "请选择一个承包方进行添加!";

        /// <summary>
        /// 添加共有人
        /// </summary>
        public const string AddSharePerson = "添加共有人";

        /// <summary>
        /// 共有人处理
        /// </summary>
        public const string SharePersonProc = "共有人处理";

        /// <summary>
        /// 编辑共有人
        /// </summary>
        public const string EditSharePerson = "编辑共有人";
        
        /// <summary>
        /// 添加共有人失败
        /// </summary>
        public const string AddPersonFail = "添加共有人失败!";

        /// <summary>
        /// 没有数据可以删除
        /// </summary>
        public const string DelShareNoData = "没有承包方可以删除!";

        /// <summary>
        /// 没有数据可以编辑
        /// </summary>
        public const string EditShareNoData = "没有承包方可以编辑!";

        /// <summary>
        /// 删除数据
        /// </summary>
        public const string DelData = "删除数据";

        /// <summary>
        /// 锁定的承包方不能被删除
        /// </summary>
        public const string DelPersonLock = "承包方被锁定,不能删除!";

        /// <summary>
        /// 锁定的承包方不能被编辑
        /// </summary>
        public const string EditPersonLock = "承包方被锁定,只能查看信息!";

        /// <summary>
        /// 锁定的承包方不能被替换
        /// </summary>
        public const string PersonSetLock = "承包方被锁定,不能替换!";

        /// <summary>
        /// 锁定的承包方不能添加共有人
        /// </summary>
        public const string AddSharePersonLock = "承包方被锁定,不能添加共有人!";

        /// <summary>
        /// 选择删除数据
        /// </summary>
        public const string DelDataNo = "请选择一条数据进行删除!";

        /// <summary>
        /// 没有承包方可以删除
        /// </summary>
        public const string DelNoData = "没有承包方可以删除!";

        /// <summary>
        /// 编辑数据
        /// </summary>
        public const string EditData = "编辑数据";

        /// <summary>
        /// 选择编辑数据
        /// </summary>
        public const string EditDataNo = "请选择一条数据进行编辑!";

        /// <summary>
        /// 编辑数据失败
        /// </summary>
        public const string EditDataFail = "更新数据失败!";

        /// <summary>
        /// 删除承包方
        /// </summary>
        public const string DelVirtualPerson = "删除承包方";

        /// <summary>
        /// 删除共有人
        /// </summary>
        public const string DelSharePerson = "删除共有人";

        /// <summary>
        /// 删除承包方失败
        /// </summary>
        public const string DelVPersonFail = "删除承包方失败!";

        /// <summary>
        ///  删除共有人失败
        /// </summary>
        public const string DelPersonFail = "删除共有人失败!";

        /// <summary>
        ///  确定删除此承包方
        /// </summary>
        public const string DelVPersonWarring = "确定删除此承包方?";

        /// <summary>
        ///  确定删除此共有人
        /// </summary>
        public const string DelPersonWarring = "确定删除此共有人?";

        /// <summary>
        ///  不能删除此共有人
        /// </summary>
        public const string DelPersonForbidden = "不能删除此共有人!";

        /// <summary>
        /// 清空数据
        /// </summary>
        public const string Clear = "清空数据";

        /// <summary>
        /// 清空数据时没有选择地域
        /// </summary>
        public const string ClearNoSelectZone = "请选择地域执行清空数据操作!";

        /// <summary>
        /// 清空数据失败
        /// </summary>
        public const string ClearFail = "清空数据失败!";

        /// <summary>
        /// 清空数据确定
        /// </summary>
        public const string ClearConfirm = "确定清空当前地域及子级地域未锁定承包方数据?";

        /// <summary>
        /// 当前地域下没有可供清空数据
        /// </summary>
        public const string ClearNoValidData = "当前地域下没有可供清空数据!";

        /// <summary>
        /// 实体不存在
        /// </summary>
        public const string EntityNull = "承包方实体不存在";

        /// <summary>
        /// 承包方名称空
        /// </summary>
        public const string VpNameNull = "承包方名称不能为空";

        /// <summary>
        /// 联系电话不能为空
        /// </summary>
        public const string VpTelNull = "联系电话不能为空";

        /// <summary>
        /// 承包方户号空
        /// </summary>
        public const string VpCodeNull = "承包方户号不能为空!";

        /// <summary>
        /// 身份证号码不正确
        /// </summary>
        public const string CardCodeError = "身份证号码不正确!";

        /// <summary>
        /// 身份证号码为空
        /// </summary>
        public const string CardIcnCodeEmpty = "身份证号码不能为空!";

        /// <summary>
        /// 名称不能为空
        /// </summary>
        public const string NameEmpty = "名称不能为空!";

        /// <summary>
        /// 证件号码不能为空
        /// </summary>
        public const string CardCodeEmpty = "证件号码不能为空!";

        /// <summary>
        /// 开始时间大于结束时间
        /// </summary>
        public const string StartBigEnd = "承包开始时间必需小于承包结束时间";

        /// <summary>
        /// 选择导入地域
        /// </summary>
        public const string ImportNoZone = "请选择导入调查表所在行政区域!";

        /// <summary>
        /// 选择导入地域
        /// </summary>
        public const string ImportVirtual = "导入承包方";

        /// <summary>
        /// 选择批量导入承包方数据的地域
        /// </summary>
        public const string VolumnImportVirtual = "批量导入承包方数据";

        /// <summary>
        /// 是否清空数据后再执行导入操作
        /// </summary>
        public const string ImportPersonIsClear = "当前地域已存在承包方数据，是否清空未被锁定的承包方后再导入?";

        /// <summary>
        /// 批量导入数据时选择的地域级别不是村
        /// </summary>
        public const string VolumnImportErrorZone = "请在村级地域下批量导入数据!";

        /// <summary>
        /// 导入地域级别不正确
        /// </summary>
        public const string ImportErrorZone = "请在村、组级地域下导入数据!";

        /// <summary>
        /// 导入承包方调查表描述
        /// </summary>
        public const string ImportDataComment = "导入承包方调查表中承包方数据";

        /// <summary>
        /// 导入调查表
        /// </summary>
        public const string ImportData = "导入调查表";

        /// <summary>
        /// 导入调查表失败
        /// </summary>
        public const string ImportDataFail = "导入调查表失败!";

        /// <summary>
        /// 批量导入调查表
        /// </summary>
        public const string VolumnImportData = "批量导入调查表";

        /// <summary>
        /// 批量导入调查表失败
        /// </summary>
        public const string VolumnImportFail = "批量导入调查表失败";

        /// <summary>
        /// 分户
        /// </summary>
        public const string SplitFamily = "分户";

        /// <summary>
        /// 分户确定
        /// </summary>
        public const string SplitFamilyConfirm = "确定进行分户操作?";

        /// <summary>
        /// 请选择承包方进行分户
        /// </summary>
        public const string SplitNoPerson = "请选择承包方进行分户!";

        /// <summary>
        /// 锁定的承包方不能进行分户操作
        /// </summary>
        public const string SplitPersonLock = "该承包方被锁定,不能进行分户操作!";

        /// <summary>
        /// 类型无法分户
        /// </summary>
        public const string SplitMainPerson = "承包方类型是个人或单位不能进行分户!";

        /// <summary>
        /// 不包含其他人员无法分户
        /// </summary>
        public const string SplitPersonNull = "户下不包含其他人员不能进行分户!";

        /// <summary>
        /// 分户失败
        /// </summary>
        public const string SplitFail = "分户失败!";

        /// <summary>
        /// 户主设置
        /// </summary>
        public const string PersonSet = "设置户主";

        /// <summary>
        /// 户主设置
        /// </summary>
        public const string PersonSetFail = "设置户主失败!";

        /// <summary>
        /// 选择共有人户主设置
        /// </summary>
        public const string PersonSetNull = "请选择一个共有人进行设置";

        /// <summary>
        /// 已是承包方
        /// </summary>
        public const string PersonIsMain = "已是承包方,不能进行设置!";

        /// <summary>
        /// 户主设置定
        /// </summary>
        public const string PersonSetConfirm = "确定设置此共有人为户主?";

        /// <summary>
        /// 锁定解锁
        /// </summary>
        public const string PersonLock = "锁定/解锁";

        /// <summary>
        /// 锁定解锁失败
        /// </summary>
        public const string PersonLockFail = "锁定或解锁户失败!";

        /// <summary>
        /// 合并承包方
        /// </summary>
        public const string PersonCombine = "合并承包方";

        /// <summary>
        /// 合并承包方失败
        /// </summary>
        public const string PersonCombineFail = "合并承包方失败!";

        /// <summary>
        /// 导出模板不存在
        /// </summary>
        public const string ExportTemplateNotExit = "导出模板不存在!";

        /// <summary>
        /// 导出无地域
        /// </summary>
        public const string ExportNoZone = "请选择导出承包方所在行政区域!";

        /// <summary>
        /// 预览无地域
        /// </summary>
        public const string PreivewNoZone = "请选择预览承包方所在行政区域!";

        /// <summary>
        /// 导出数据
        /// </summary>
        public const string ExportData = "导出数据";

        /// <summary>
        /// 预览数据
        /// </summary>
        public const string PreviewData = "预览数据";

        /// <summary>
        /// 承包方查找
        /// </summary>
        public const string PersonSearch = "承包方查找";

        /// <summary>
        /// 承包方最后
        /// </summary>
        public const string PersonLastOne = "已经是最后一个承包方!";

        /// <summary>
        /// 承包方最前
        /// </summary>
        public const string PersonFirstOne = "已经是第一个承包方!";

        /// <summary>
        /// 承包方初始化
        /// </summary>
        public const string PersonInitiall = "初始化数据";

        /// <summary>
        /// 承包方初始化
        /// </summary>
        public const string PersonInitiallFail = "初始化数据失败";

        /// <summary>
        /// 承包方初始化描述
        /// </summary>
        public const string PersonInitialDesc = "初始化承包方基本信息";

        /// <summary>
        /// 地域信息不存在
        /// </summary>
        public const string InitialPersonInfoNoZone = "请选择初始化信息所在的地域!";

        /// <summary>
        /// 批量初始化时选择地域大于镇
        /// </summary>
        public const string InitialPersonSelectZoneError = "请在镇级地域(包括镇)以下进行承包方信息初始化!";

        /// <summary>
        /// 地域承包方存在
        /// </summary>
        public const string PersonInZoneExist = "该地域下已存在该承包方!";

        /// <summary>
        /// 户号存在
        /// </summary>
        public const string PersonNumberRe = "该地域下已存在该户号!";

        /// <summary>
        /// 共有人已存在
        /// </summary>
        public const string PersonInExist = "该地域下已存在该共有人!";

        /// <summary>
        /// 集体承包方只能添加或导入
        /// </summary>
        public const string CollPersonMustAdd = "不能将承包方类型为农户或个人的承包方修改为集体,集体只能添加或导入!";

        /// <summary>
        /// 证件号码必需填写
        /// </summary>
        public const string PersonNumberMust = "证件号码必需填写!";

        /// <summary>
        /// 邮政编码不正确
        /// </summary>
        public const string PersonNumberError = "邮政编码不正确!";

        /// <summary>
        /// 导出承包方调查表描述
        /// </summary>
        public const string ExportDataExcel = "导出承包方Excel调查表数据";

        /// <summary>
        /// 导出承包方调查表描述
        /// </summary>
        public const string ExportDataWord = "导出承包方Word调查表数据";

        /// <summary>
        /// 导出调查表
        /// </summary>
        public const string ExportTable = "导出调查表";

        /// <summary>
        /// 导出户主声明书
        /// </summary>
        public const string ExportApplyBook = "导出户主声明书";

        /// <summary>
        /// 导出户主声明书描述
        /// </summary>
        public const string ExportApplyBookComment = "批量导出地域下的户主声明书";

        /// <summary>
        /// 导出委托声明书
        /// </summary>
        public const string ExportDelegateBook = "导出委托声明书";

        /// <summary>
        /// 导出委托声明书描述
        /// </summary>
        public const string ExportDelegateBookComment = "批量导出地域下的委托声明书";

        /// <summary>
        /// 导出无异议声明书
        /// </summary>
        public const string ExportIdeaBook = "导出无异议声明书";

        /// <summary>
        /// 导出无异议声明书日期设置错误
        /// </summary>
        public const string ExportIdeaBookDateSettingError = "声明日期不能小于公示日期!";

        /// <summary>
        /// 导出无异议声明书描述
        /// </summary>
        public const string ExportIdeaBookComment = "批量导出地域下的无异议声明书";

        /// <summary>
        /// 导出测绘申请书
        /// </summary>
        public const string ExportSurveyBook = "导出测绘申请书";

        /// <summary>
        /// 导出测绘申请书描述
        /// </summary>
        public const string ExportSurveyBookComment = "批量导出地域下的测绘申请书";

        /// <summary>
        /// 选择数据导出
        /// </summary>
        public const string ExportDataNo = "请选择一条数据进行导出!";

        /// <summary>
        /// 选择数据预览
        /// </summary>
        public const string PreviewDataNo = "请选择一条数据进行预览!";

        /// <summary>
        /// 必须填写共有人家庭关系
        /// </summary>
        public const string RelationShipMust = "共有人家庭关系不能为空!";

        /// <summary>
        /// 没有缺失户号
        /// </summary>
        public const string MissNumnberNo = "无缺失户号";

        /// <summary>
        /// 户号详情
        /// </summary>
        public const string FamilyNumber = "户号详情";

        /// <summary>
        /// 所选地域下没有承包方信息
        /// </summary>
        public const string CurrentZoneNoPerson = "当前地域下没有承包方信息!";

        /// <summary>
        /// 请选择在镇级以下(包括镇)地域批量导出
        /// </summary>
        public const string VolumnExportZoneError = "请选择在镇级以下(包括镇)地域批量导出!";

        /// <summary>
        /// 成功导出当前选中承包方
        /// </summary>
        public const string ExportVpDataComplete = "成功导出当前选中承包方信息.";

        /// <summary>
        /// 调查日期应该小于审核日期
        /// </summary>
        public const string SurveyDatePassCheckDate = "调查日期应小于审核日期!";

        /// <summary>
        /// 审核日期应该小于公示日期
        /// </summary>
        public const string CheckDatePassPubDate = "审核日期应小于公示日期!";

        #endregion
    }
}
