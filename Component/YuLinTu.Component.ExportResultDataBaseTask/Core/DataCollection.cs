/*
 * (C) 2014-2015 鱼鳞图公司版权所有，保留所有权利
*/
using System;
using System.Collections.Generic;
using YuLinTuQuality.Business.Entity;

namespace YuLinTu.Component.ExportResultDataBaseTask
{
    /// <summary>
    /// 数据导出集合
    /// </summary>
    public class DataCollection : IDisposable
    {
        #region Property

        /// <summary>
        /// 发包方集合
        /// </summary>
        public List<FBF> FBFJH { get; set; }

        /// <summary>
        /// 承包方集合
        /// </summary>
        public List<CBF> CBFJH { get; set; }

        /// <summary>
        /// 家庭成员集合
        /// </summary>
        public List<CBF_JTCY> JTCYJH { get; set; }

        /// <summary>
        /// 承包地块信息集合
        /// </summary>
        public List<CBDKXX> CBDKXXJH { get; set; }

        /// <summary>
        /// 承包合同集合
        /// </summary>
        public List<CBHT> HTJH { get; set; }

        /// <summary>
        /// 登记簿集合
        /// </summary>
        public List<CBJYQZDJB> DJBJH { get; set; }

        /// <summary>
        /// 承包经营权证集合
        /// </summary>
        public List<CBJYQZ> CBJYQZJH { get; set; }

        /// <summary>
        /// 权证补发集合
        /// </summary>
        public List<CBJYQZ_QZBF> QZBFExJH { get; set; }

        /// <summary>
        /// 权证换发集合
        /// </summary>
        public List<CBJYQZ_QZHF> QZHFExJH { get; set; }

        /// <summary>
        /// 权证注销集合
        /// </summary>
        public List<CBJYQZ_QZZX> QZZXJH { get; set; }

        /// <summary>
        /// 承包经营权流转合同集合
        /// </summary>
        public List<LZHT> LZHTJH { get; set; }

        /// <summary>
        /// 权属来源资料附件集合
        /// </summary>
        public List<QSLYZLFJ> FJExJH { get; set; }

        /// <summary>
        /// 界址点集合
        /// </summary>
        public List<SqliteJZD> JZDJH { get; set; }

        /// <summary>
        /// 界址线集合
        /// </summary>
        public List<SqliteJZX> JZXJH { get; set; }

        /// <summary>
        /// 空间地块集合
        /// </summary>
        public List<SqliteDK> KJDKJH { get; set; }

        #endregion

        #region Method

        /// <summary>
        /// 清空
        /// </summary>
        private void Clear()
        {
            FBFJH.Clear();
            CBFJH.Clear();
            JTCYJH.Clear();
            HTJH.Clear();
            DJBJH.Clear();
            CBJYQZJH.Clear();
            QZBFExJH.Clear();
            QZHFExJH.Clear();
            QZZXJH.Clear();
            LZHTJH.Clear();
            FJExJH.Clear();
            CBDKXXJH.Clear();
            FBFJH = null;
            CBFJH = null;
            JTCYJH = null;
            HTJH = null;
            DJBJH = null;
            CBJYQZJH = null;
            QZBFExJH = null;
            QZHFExJH = null;
            QZZXJH = null;
            LZHTJH = null;
            FJExJH = null;
            CBDKXXJH = null;
            JZDJH = null;
            JZXJH = null;
            KJDKJH = null;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Clear();
            GC.Collect();
        }
        #endregion

        #region Ctor

        public DataCollection()
        {
            FBFJH = new List<FBF>();
            CBFJH = new List<CBF>();
            JTCYJH = new List<CBF_JTCY>();
            HTJH = new List<CBHT>();
            DJBJH = new List<CBJYQZDJB>();
            CBJYQZJH = new List<CBJYQZ>();
            QZBFExJH = new List<CBJYQZ_QZBF>();
            QZHFExJH = new List<CBJYQZ_QZHF>();
            QZZXJH = new List<CBJYQZ_QZZX>();
            LZHTJH = new List<LZHT>();
            FJExJH = new List<QSLYZLFJ>();
            CBDKXXJH = new List<CBDKXX>();
            JZDJH = new List<SqliteJZD>();
            JZXJH = new List<SqliteJZX>();
            KJDKJH = new List<SqliteDK>();
        }

        #endregion

    }
}