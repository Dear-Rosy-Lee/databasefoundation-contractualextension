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
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using System.Collections.ObjectModel;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 发包方数据参数
    /// </summary>
    public class TaskVirtualPersonArgument : TaskArgument
    {
        #region Fields

        private bool initiallNumber;   //初初始化户编号始化
        private bool initiallZip;      //初始化邮编
        private bool initiallNation;   //初始化民族
        //private bool initiallSurvey;   //初始化调查信息
        private string address;   //承包方地址
        private string zipCode;   //邮政编码
        private eNation cNation;  //民族
        private VirtualPersonExpand expand;  //承包方扩展信息

        #endregion

        #region Property

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 清空数据
        /// </summary>
        public bool IsClear { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public ePersonArgType ArgType { get; set; }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType VirtualType { get; set; }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType virtualType;

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? DateValue { get; set; }

        /// <summary>
        /// 公示时间
        /// </summary>
        public DateTime? PubDateValue { get; set; }

        #region 初始化承包方信息

        /// <summary>
        /// 初始化户编号
        /// </summary>
        public bool InitiallNumber
        {
            get { return initiallNumber; }
            set { initiallNumber = value; }
        }

        /// <summary>
        /// 初始化邮编
        /// </summary>
        public bool InitiallZip
        {
            get { return initiallZip; }
            set { initiallZip = value; }
        }

        /// <summary>
        /// 初始化民族
        /// </summary>
        public bool InitiallNation
        {
            get { return initiallNation; }
            set { initiallNation = value; }
        }

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
        public bool InitiallSurveyAccount  { get; set; }

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
        public VirtualPersonExpand Expand
        {
            get { return expand; }
            set { expand = value; }
        }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string ZipCode
        {
            get { return zipCode; }
            set { zipCode = value; }
        }

        /// <summary>
        /// 民族
        /// </summary>
        public eNation CNation
        {
            get { return cNation; }
            set { cNation = value; }
        }

        #endregion

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskVirtualPersonArgument()
        {
            virtualType = eVirtualType.Land;
        }

        #endregion
    }
}