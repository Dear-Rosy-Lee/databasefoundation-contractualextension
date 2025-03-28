/*
 * (C) 2016 鱼鳞图公司版权所有,保留所有权利
*/
using System;
using System.Collections.Generic;
using Quality.Business.Entity;

namespace YuLinTu.Component.ImportResultDataBaseTask
{
    /// <summary>
    /// 属性数据集合类
    /// </summary>
    public class DataCollectionDb
    {
        #region Properties

        /// <summary>
        /// 承包经营权证
        /// </summary>
        public List<CBJYQZ> CBJYQZJH { get; set; }

        /// <summary>
        /// 权证补发
        /// </summary>
        public List<CBJYQZ_QZBF> QZBFJH { get; set; }

        /// <summary>
        /// 权证换发
        /// </summary>
        public List<CBJYQZ_QZHF> QZHFJH { get; set; }

        /// <summary>
        /// 权证注销
        /// </summary>
        public List<CBJYQZ_QZZX> QZZXJH { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<QSLYZLFJ> FJJH { get; set; }

        /// <summary>
        /// 家庭成员
        /// </summary>
        public List<CBF_JTCY> JTCYJH { get; set; }

        /// <summary>
        /// 承包方
        /// </summary>
        public List<CBFSC> CBFJH { get; set; }

        /// <summary>
        /// 登记簿
        /// </summary>
        public List<CBJYQZDJB> DJBJH { get; set; }

        /// <summary>
        /// 流转合同
        /// </summary>
        public List<LZHT> LZHTJH { get; set; }

        /// <summary>
        /// 发包方
        /// </summary>
        public List<FBF> FBFJH { get; set; }

        /// <summary>
        /// 承包地块信息
        /// </summary>
        public List<CBDKXXSC> DKXXJH { get; set; }

        /// <summary>
        /// 合同集合
        /// </summary>
        public List<CBHT> HTJH { get; set; }

        #endregion

        #region Methods

        public void Clear()
        {
            if (CBJYQZJH != null)
                CBJYQZJH.Clear();
            if (QZBFJH != null)
                QZBFJH.Clear();
            if (QZHFJH != null)
                QZHFJH.Clear();
            if (QZZXJH != null)
                QZZXJH.Clear();
            if (FJJH != null)
                FJJH.Clear();
            if (JTCYJH != null)
                JTCYJH.Clear();
            if (CBFJH != null)
                CBFJH.Clear();
            if (LZHTJH != null)
                LZHTJH.Clear();
            if (FBFJH != null)
                FBFJH.Clear();
            if (DKXXJH != null)
                DKXXJH.Clear();
            if (HTJH != null)
                HTJH.Clear();
            GC.Collect();
        }

        #endregion
    }

    [Serializable]
    public class CBDKXXSC : CBDKXX
    {
        public const string TableName = "CBDKXX";

        public const string TableNameCN = "承包地块信息";

        public string DKLB { get; set; }
    }

    [Serializable]
    public class CBFSC : CBF
    {
        public const string TableName = "CBF";

        public const string TableNameCN = "承包方";

        public const string FXZDYBM = "XZDYBM";

        public string XZDYBM { get; set; }

        public string QQCBFBM { get; set; }
    }
}
