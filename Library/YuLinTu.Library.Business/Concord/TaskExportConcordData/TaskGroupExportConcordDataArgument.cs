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

namespace YuLinTu.Library.Business
{
    public class TaskGroupExportConcordDataArgument : TaskArgument
    {
        #region Properties

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public eContractConcordArgType ArgType { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? DateValue { get; set; }

        /// <summary>
        /// 公示时间
        /// </summary>
        public DateTime? PubDateValue { get; set; }
        /// <summary>
        /// 合同导出面积类型
        /// </summary>
        public int AreaType
        {
            get; set;
        }

        #region Properties - 签订合同

        /// <summary>
        /// 承包合同集合(待初始化)
        /// </summary>
        public List<ContractConcord> ConcordsModified { get; set; }

        /// <summary>
        /// 初始化合同信息的地块集合
        /// </summary>
        public List<ContractLand> LandsOfInitialConcord { get; set; }

        /// <summary>
        /// 发包方
        /// </summary>
        public CollectivityTissue Sender { get; set; }

        /// <summary>
        ///  审核申请日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }


        /// <summary>
        /// 合同明细表设置
        /// </summary>
        public DataSummaryDefine SummaryDefine { get; set; }

        /// <summary>
        /// 承包方集合
        /// </summary>
        public List<VirtualPerson> ListPerson { get; set; }

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet { get; set; }


        /// <summary>
        /// 本地域及以下集合
        /// </summary>
        public List<Zone> SelfAndSubsZones { get; set; }

        /// <summary>
        /// 包括镇、村、组地域集合
        /// </summary>
        public List<Zone> AllZones { get; set; }


        #endregion

        #endregion

    }
}
