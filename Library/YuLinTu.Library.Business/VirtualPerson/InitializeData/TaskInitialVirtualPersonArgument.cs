/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using Microsoft.Scripting.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 初始化承包方基本信息任务参数
    /// </summary>
    public class TaskInitialVirtualPersonArgument : TaskArgument
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
        /// 类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 初始化户编号
        /// </summary>
        public bool InitiallNumber { get; set; }

        /// <summary>
        /// 初始化邮编
        /// </summary>
        public bool InitiallZip { get; set; }

        /// <summary>
        /// 初始化家庭成员备注
        /// </summary>
        public bool InitPersonComment { get; set; }

        /// <summary>
        /// 初始化共有人备注
        /// </summary>
        public bool InitSharePersonComment { get; set; }

        /// <summary>
        /// 初始化民族
        /// </summary>
        public bool InitiallNation { get; set; }

        /// <summary>
        /// 初始化姓名
        /// </summary>
        public bool InitiallSex { get; set; }

        #region 初始化调查信息

        /// <summary>
        /// 初始化承包方地址
        /// </summary>
        public bool InitiallVpAddress { get; set; }

        /// <summary>
        /// 初始化调查员
        /// </summary>
        public bool InitiallSurveyPerson { get; set; }

        /// <summary>
        /// 初始化调查日期
        /// </summary>
        public bool InitiallSurveyDate { get; set; }

        /// <summary>
        /// 初始化调查记事
        /// </summary>
        public bool InitiallSurveyAccount { get; set; }

        /// <summary>
        /// 初始化审核人
        /// </summary>
        public bool InitiallCheckPerson { get; set; }

        /// <summary>
        /// 初始化审核日期
        /// </summary>
        public bool InitiallCheckDate { get; set; }

        /// <summary>
        /// 初始化审核意见
        /// </summary>
        public bool InitiallCheckOpinion { get; set; }

        /// <summary>
        /// 初始化公示记事人
        /// </summary>
        public bool InitiallPublishAccountPerson { get; set; }

        /// <summary>
        /// 初始化公示日期
        /// </summary>
        public bool InitiallPublishDate { get; set; }

        /// <summary>
        /// 初始化公示审核人
        /// </summary>
        public bool InitiallPublishCheckPerson { get; set; }

        /// <summary>
        /// 初始化公示记事
        /// </summary>
        public bool InitiallcbPublishAccount { get; set; }

        #endregion

        /// <summary>
        /// 承包方扩展信息
        /// </summary>
        public VirtualPersonExpand Expand { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// 家庭成员备注
        /// </summary>
        public string PersonComment { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public eNation CNation { get; set; }

        /// <summary>
        /// 承包方集合(待初始化)
        /// </summary>
        public List<VirtualPerson> ListPerson { get; set; }

        /// <summary>
        /// 按村级进行户号、地块编码统一初始及签订
        /// </summary>
        public bool VillageInlitialSet { get; set; }

        /// <summary>
        /// 农户家庭户号
        /// </summary>
        public int[] FarmerFamilyNumberIndex { get; set; }

        /// <summary>
        /// 个人家庭户号
        /// </summary>
        public int[] PersonalFamilyNumberIndex { get; set; }

        /// <summary>
        /// 单位家庭户号
        /// </summary>
        public int[] UnitFamilyNumberIndex { get; set; }
        /// <summary>
        /// 只初始化空项
        /// </summary>
        public bool InitialNull { get; set; }
        /// <summary>
        /// 承包方式
        /// </summary>
        public bool InitialContractWay { get; set; }
        /// <summary>
        /// 合同编号
        /// </summary>
        public bool InitConcordNumber { get; set; }
        /// <summary>
        /// 权证编码
        /// </summary>
        public bool InitWarrentNumber { get; set; }
        /// <summary>
        /// 承包开始时间
        /// </summary>
        public bool InitStartTime { get; set; }
        /// <summary>
        /// 承包结束时间
        /// </summary>
        public bool InitEndTime { get; set; }

        public bool InitAllNum { get; set; }

        /// <summary>
        /// 初始化起始编码
        /// </summary>         
        public int InitiallStartNum { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskInitialVirtualPersonArgument()
        {
            VirtualType = eVirtualType.Land;
            FarmerFamilyNumberIndex = new int[] { 1 };
        }

        #endregion 
    }
}
