/*
 * (C) 2014 鱼鳞图公司版权所有,保留所有权利
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTuQuality.Business.TaskBasic;
using YuLinTuQuality.Business.Entity;

namespace YuLinTu.Component.ResultDbof2016ToLocalDb
{
    /// <summary>
    /// 属性数据集合类
    /// </summary>
    public class DataCollection
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
        public List<CBF> CBFJH { get; set; }

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
        public List<CBDKXXEX> DKXXJH { get; set; }

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

    /// <summary>
    /// 空间数据集合类
    /// </summary>
    public class SpaceDataCollection
    {
        #region Properties

        /// <summary>
        /// 控制点
        /// </summary>
        public List<KZD> KZDS { get; set; }

        /// <summary>
        /// 基本农田保护区
        /// </summary>
        public List<JBNTBHQ> BHQS { get; set; }

        /// <summary>
        /// 区域界线
        /// </summary>
        public List<QYJX> QYJXS { get; set; }

        /// <summary>
        /// 点状地物
        /// </summary>
        public List<DZDW> DZDWS { get; set; }

        /// <summary>
        /// 线状地物
        /// </summary>
        public List<XZDW> XZDWS { get; set; }

        /// <summary>
        /// 面状地物
        /// </summary>
        public List<MZDW> MZDWS { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            if (KZDS != null)
                KZDS.Clear();
            if (BHQS != null)
                BHQS.Clear();
            if (QYJXS != null)
                QYJXS.Clear();
            if (DZDWS != null)
                DZDWS.Clear();
            if (XZDWS != null)
                XZDWS.Clear();
            if (MZDWS != null)
                MZDWS.Clear();
            GC.Collect();
        }

        /// <summary>
        /// 最大数量
        /// </summary>
        public int MaxRecord()
        {
            int maxRecord = KZDS == null ? 0 : KZDS.Count;
            if (BHQS != null && BHQS.Count > maxRecord)
                maxRecord = BHQS.Count;
            if (QYJXS != null && QYJXS.Count > maxRecord)
                maxRecord = QYJXS.Count;
            if (DZDWS != null && DZDWS.Count > maxRecord)
                maxRecord = DZDWS.Count;
            if (XZDWS != null && XZDWS.Count > maxRecord)
                maxRecord = XZDWS.Count;
            if (MZDWS != null && MZDWS.Count > maxRecord)
                maxRecord = MZDWS.Count;
            return maxRecord;
        }

        /// <summary>
        /// 获取指定序列的数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">数据集合</param>
        /// <param name="startindex">开始序号</param>
        /// <param name="dataNumber">数据条数</param>
        /// <returns></returns>
        public List<T> GetDataList<T>(List<T> list, int startindex, int dataNumber)
        {
            if (list == null || list.Count == 0 || startindex > list.Count)
            {
                return null;
            }
            int endNumber = startindex + dataNumber;
            if (startindex + dataNumber > list.Count)
            {
                endNumber = list.Count;
            }
            List<T> enList = new List<T>();
            for (int i = startindex; i < endNumber; i++)
            {
                enList.Add(list[i]);
            }
            return enList;
        }

        #endregion
    }
}
