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
using YuLinTu;
using YuLinTu.Data;
using System.Collections.ObjectModel;
using System.Collections;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 批量导出汇总数据参数
    /// </summary>
    public class TaskGroupDataSummaryExportArgument : TaskArgument
    {
        #region Properties

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 父级到省级地域
        /// </summary>
        public List<Zone> ParentsToProvince { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public eContractAccountType ArgType { get; set; }

        /// <summary>
        /// 使用者
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 清空数据
        /// </summary>
        public bool IsClear { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 承包台账地块数据业务
        /// </summary>
        public AccountLandBusiness LandBusiness { get; set; }

        /// <summary>
        /// 当前地域下的地块集合
        /// </summary>
        public List<ContractLand> CurrentZoneLandList { get; set; }

        /// <summary>
        /// 是否批量
        /// </summary>
        public bool IsBatch { get; set; }

        /// <summary>
        /// 表格类型
        /// </summary>
        public int TableType { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 当前选择的承包方
        /// </summary>
        public List<VirtualPerson> SelectContractors { get; set; }

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet { get; set; }

        #region 承包台账设置

        /// <summary>
        /// 承包台账导出配置
        /// </summary>
        public PublicityConfirmDefine ContractLandOutputSurveyDefine { get; set; }

        /// <summary>
        /// 承包台账单户调查表设置
        /// </summary>
        public SingleFamilySurveyDefine SingleFamilySurveySetting { get; set; }

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine { get; set; }

        #endregion

        #region 承包方设置

        /// <summary>
        /// 承包方其它配置
        /// </summary>
        public FamilyOtherDefine FamilyOtherSet { get; set; }

        /// <summary>
        /// 承包方导出设置
        /// </summary>
        public FamilyOutputDefine FamilyOutputSet { get; set; }

        #endregion

        #region 承包合同设置

        /// <summary>
        ///  承包合同常规设置
        /// </summary>
        public ContractConcordSettingDefine ConcordSettingDefine { get; set; }

        /// <summary>
        ///  审核申请日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

        /// <summary>
        /// 公示时间
        /// </summary>
        public DateTime? PubDateValue { get; set; }

        #endregion

        #region 承包权证设置

        /// <summary>
        /// 证书共有人数设置-证书数据处理分页设置
        /// </summary>
        public int? BookPersonNum { get; set; }

        /// <summary>
        /// 证书地块数设置-证书数据处理分页设置
        /// </summary>
        public int? BookLandNum { get; set; }

        /// <summary>
        /// 承包权证导出选择扩展模板格式设置
        /// </summary>
        public ContractRegeditBookSettingDefine ExtendUseExcelDefine { get; set; }       



        #endregion

        /// <summary>
        /// 数据字典
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        /// <summary>
        /// 本地域及以下集合
        /// </summary>
        public List<Zone> SelfAndSubsZones { get; set; }

        /// <summary>
        /// 包括镇、村、组地域集合
        /// </summary>
        public List<Zone> AllZones { get; set; }

        /// <summary>
        /// 导出的选择的表单类型
        /// </summary>
        public ExportDataSummarySelectTable ExportDataSummaryTableTypes { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupDataSummaryExportArgument()
        {
            VirtualType = eVirtualType.Land;
        }

        #endregion
    }
}
