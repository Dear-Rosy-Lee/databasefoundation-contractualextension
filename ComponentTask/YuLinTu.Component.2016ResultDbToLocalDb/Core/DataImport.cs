/*
 * (C) 2014 鱼鳞图公司版权所有,保留所有权利
*/
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTuQuality.Business.TaskBasic;
using YuLinTuQuality.Business.Entity;


namespace YuLinTu.Component.ResultDbof2016ToLocalDb
{
    /// <summary>
    /// 获取数据
    /// </summary>
    public partial class DataImportProgress : Task
    {
        #region 获取数据

        #region 通过编码查询数据

        /// <summary>
        /// 通过权证编码获取数据
        /// </summary>
        private ComplexRightEntity GetRightInfoByBookId(CBJYQZDJB DJB, string zoneCode, List<DKEX> spaceLandList, DataCollection dc)
        {
            string bookId = DJB.CBJYQZBM;
            string CBFBM = DJB.CBFBM;
            string fbfid = DJB.FBFBM;
            ComplexRightEntity right = new ComplexRightEntity();
            right.BookCode = bookId;
            List<CBJYQZDJB> djbs = new List<CBJYQZDJB>();
            if (DJB != null) djbs.Add(DJB);
            right.DJB = djbs;
            right.CBJYQZ = dc.CBJYQZJH.FindAll(t => t.CBJYQZBM == (bookId));
            right.QZBF = dc.QZBFJH.FindAll(t => t.CBJYQZBM == (bookId));
            right.QZHF = dc.QZHFJH.FindAll(t => t.CBJYQZBM == (bookId));
            right.QZZX = dc.QZZXJH.FindAll(t => t.CBJYQZBM == (bookId));
            right.JTCY = dc.JTCYJH.FindAll(t => t.CBFBM == CBFBM);
            right.CBF = dc.CBFJH.Find(t => t.CBFBM == (CBFBM));
            right.FJ = dc.FJJH.FindAll(t => t.CBJYQZBM == (bookId));
            List<LZHT> htList = dc.LZHTJH.FindAll(t => t.CBFBM == (CBFBM));
            List<LZHTEX> exchangeList = new List<LZHTEX>();
            var qcbf = db.CreateQuery<CBF>();
            htList.ForEach(t => exchangeList.Add(ChangeLZHT(t, qcbf)));
            right.LZHT = exchangeList;
            right.HT = dc.HTJH.FindAll(t => t.CBHTBM == (bookId));
            right.FBF = dc.FBFJH.Find(t => t.FBFBM == fbfid);
            List<CBDKXXEX> CBDKJH = dc.DKXXJH.FindAll(t => t.CBJYQZBM == (bookId));
            right.DKXX = ChangeDKXX(CBDKJH, spaceLandList);
            right.VirtualPersonCode = right.CBF == null ? "" : right.CBF.CBFBM;
            right.ZoneCode = zoneCode;
            return right;
        }

        #endregion

        #region 承包方

        /// <summary>
        /// 获取承包方
        /// </summary>
        private List<CBF> GetZoneCBF(string zoneCode)
        {
            if (string.IsNullOrEmpty(zoneCode))
                return null;
            var q = db.CreateQuery<CBF>();
            List<CBF> entitys = q.Where(t => t.CBFBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
            return entitys;
        }

        /// <summary>
        /// 获取承包方家庭成员
        /// </summary>
        private List<CBF_JTCY> GetZoneJTCY(string zoneCode)
        {
            if (string.IsNullOrEmpty(zoneCode))
                return null;
            var q = db.CreateQuery<CBF_JTCY>();
            List<CBF_JTCY> entitys = q.Where(t => t.CBFBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
            return entitys;
        }

        #endregion

        #region 发包方

        /// <summary>
        /// 获取发包方
        /// </summary>
        private List<FBF> GetZoneFBF(string zoneCode)
        {
            if (string.IsNullOrEmpty(zoneCode))
                return null;
            var q = db.CreateQuery<FBF>();
            List<FBF> entitys = q.Where(t => t.FBFBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
            return entitys;
        }

        #endregion

        #region 合同

        /// <summary>
        /// 获取合同
        /// </summary>
        private List<CBHT> GetZoneCBHT(string zoneCode)
        {
            if (string.IsNullOrEmpty(zoneCode))
                return null;
            var q = db.CreateQuery<CBHT>();
            List<CBHT> entitys = q.Where(t => t.CBFBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
            if (entitys != null && entitys.Count > 0)
            {
                entitys.ForEach(t => t.HTZMJ = Math.Round(t.HTZMJ, 2));
            }
            return entitys;
        }

        /// <summary>
        /// 获取流转合同
        /// </summary>
        private List<LZHT> GetZoneLZHT(string zoneCode)
        {
            if (string.IsNullOrEmpty(zoneCode))
                return null;
            List<LZHT> entitys = new List<LZHT>();
            try
            {
                var q = db.CreateQuery<LZHT>();
                entitys = q.Where(t => t.CBFBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
                if (entitys != null && entitys.Count > 0)
                {
                    entitys.ForEach(t => t.LZMJ = Math.Round(t.LZMJ, 2));
                }
            }
            catch
            {
            }
            return entitys;
        }

        /// <summary>
        /// 转换流转合同
        /// </summary>
        private LZHTEX ChangeLZHT(LZHT ht, IQueryable<CBF> qcbf)
        {
            LZHTEX lzht = new LZHTEX();
            lzht.CBFBM = ht.CBFBM;
            lzht.HTQDRQ = ht.HTQDRQ;
            lzht.LZDKS = ht.LZDKS;
            lzht.LZFS = ht.LZFS;
            lzht.LZHTBM = ht.LZHTBM;
            lzht.LZHTDYT = ht.LZHTDYT;
            lzht.LZJGSM = ht.LZJGSM;
            lzht.LZMJ = Math.Round(ht.LZMJ, 2);
            lzht.LZQTDYT = ht.LZQTDYT;
            lzht.LZQX = ht.LZQX;
            lzht.LZQXJSRQ = ht.LZQXJSRQ;
            lzht.LZQXKSRQ = ht.LZQXKSRQ;
            lzht.SRFBM = ht.SRFBM;
            lzht.LZMJM = ht.LZMJM;
            if (!string.IsNullOrEmpty(lzht.SRFBM))
            {
                CBF en = qcbf.Where(t => t.CBFBM.Equals(lzht.SRFBM)).FirstOrDefault();
                lzht.SRFMC = en == null ? " " : en.CBFMC;
            }
            return lzht;
        }

        #endregion

        #region 地块信息

        /// <summary>
        /// 获取地块信息
        /// </summary>
        private List<CBDKXXEX> GetZoneDKXX(string zoneCode)
        {
            if (string.IsNullOrEmpty(zoneCode))
                return null;
            var q = db.CreateQuery<CBDKXX>();
            List<CBDKXX> entitys = q.Where(t => t.CBJYQZBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
            List<CBDKXXEX> retentitys = new List<CBDKXXEX>();
            CBDKXXEX retentity = null;
            if (entitys != null && entitys.Count > 0)
            {             
                foreach (var item in entitys)
                {
                    item.HTMJ = Math.Round(item.HTMJ, 2);                  
                    retentity = item.ConvertTo<CBDKXXEX>();
                    if (retentity != null)
                        retentitys.Add(retentity);
                }
            }
            return retentitys;
        }

        /// <summary>
        /// 转换地块信息实体
        /// </summary>
        private List<CBDKXXEX> ChangeDKXX(List<CBDKXXEX> landList, List<DKEX> spaceLandList)
        {
            List<CBDKXXEX> dkCollection = new List<CBDKXXEX>();
            foreach (CBDKXXEX dkxx in landList)
            {
                CBDKXXEX cbd = new CBDKXXEX()
                {
                    CBHTBM = dkxx.CBHTBM,
                    CBJYQQDFS = dkxx.CBJYQQDFS,
                    CBJYQZBM = dkxx.CBJYQZBM,
                    CBFBM = dkxx.CBFBM,
                    DKBM = dkxx.DKBM,
                    FBFBM = dkxx.FBFBM,
                    HTMJ = Math.Round(dkxx.HTMJ, 2),
                    HTMJM = dkxx.HTMJM,
                    LZHTBM = dkxx.LZHTBM,
                    SFQQQG = dkxx.SFQQQG, //新规范
                    YHTMJ = dkxx.YHTMJ,
                    YHTMJM = dkxx.YHTMJM,
                };
                cbd.KJDK = spaceLandList.Find(t => t.DKBM == dkxx.DKBM);
                if (cbd.KJDK == null)
                {
                    this.ReportInfomation("地块编码" + cbd.DKBM + "的空间数据为空!");
                }
              
                dkCollection.Add(cbd);
            }
            return dkCollection;
        }

        #endregion

        #region 信息获取

        /// <summary>
        /// 根据区域代码获取权证
        /// </summary> 
        private List<YuLinTuQuality.Business.Entity.CBJYQZ> GetZoneCBJYQZ(string zoneCode)
        {
            var q = db.CreateQuery<YuLinTu.Component.ResultDbof2016ToLocalDbFX.CBJYQZ>();
            List<YuLinTu.Component.ResultDbof2016ToLocalDbFX.CBJYQZ> entitys = q.Where(t => t.CBJYQZBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();

            List<YuLinTuQuality.Business.Entity.CBJYQZ> resEntitys = new List<YuLinTuQuality.Business.Entity.CBJYQZ>();
            foreach (var item in entitys)
            {
                resEntitys.Add(new YuLinTuQuality.Business.Entity.CBJYQZ
                {
                    CBJYQZBM = item.CBJYQZBM,
                    FZJG = item.FZJG,
                    FZRQ = item.FZRQ,
                    QZLQRQ = item.QZLQRQ,
                    QZLQRXM = item.QZLQRXM,
                    QZLQRZJHM = item.QZLQRZJHM,
                    QZLQRZJLX = item.QZLQRZJLX,
                    QZSFLQ = item.QZSFLQ,
                });
            }
            return resEntitys;
        }

        /// <summary>
        /// 根据区域代码获取补发信息
        /// </summary>
        private List<CBJYQZ_QZBF> GetZoneQZBF(string zoneCode)
        {
            var q = db.CreateQuery<CBJYQZ_QZBF>();
            List<CBJYQZ_QZBF> entityList = q.Where(t => t.CBJYQZBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
            if (entityList == null || entityList.Count == 0)
                return new List<CBJYQZ_QZBF>();
            //List<CBJYQZ_QZBF> entitys = new List<CBJYQZ_QZBF>();
            //foreach (CBJYQZ_QZBF item in entityList)
            //{
            //    entitys.Add(new CBJYQZ_QZBF
            //    {
            //        CBJYQZBM = item.CBJYQZBM,
            //        BFLQRZJHM = item.BFLQRZJHM,
            //        BFLQRZJLX = item.BFLQRZJLX,
            //        BFRQ = item.BFRQ,
            //        QZBFLQRQ = item.QZBFLQRQ,
            //        QZBFLQRXM = item.QZBFLQRXM,
            //        QZBFYY = item.QZBFYY
            //    });
            //}
            //entityList = null;
            return entityList;
        }

        /// <summary>
        /// 根据区域代码获换发信息
        /// </summary>
        private List<CBJYQZ_QZHF> GetZoneQZHF(string zoneCode)
        {
            var q = db.CreateQuery<CBJYQZ_QZHF>();
            List<CBJYQZ_QZHF> entityList = q.Where(t => t.CBJYQZBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
            if (entityList == null || entityList.Count == 0)
                return new List<CBJYQZ_QZHF>();
            //List<CBJYQZ_QZHF> entitys = new List<CBJYQZ_QZHF>();
            //foreach (CBJYQZ_QZHF item in entityList)
            //{
            //    entitys.Add(new CBJYQZ_QZHF
            //    {
            //        CBJYQZBM = item.CBJYQZBM,
            //        HFLQRZJHM = item.HFLQRZJHM,
            //        HFLQRZJLX = item.HFLQRZJLX,
            //        HFRQ = item.HFRQ,
            //        QZHFLQRQ = item.QZHFLQRQ,
            //        QZHFLQRXM = item.QZHFLQRXM,
            //        QZHFYY = item.QZHFYY
            //    });
            //}
            //entityList = null;
            return entityList;
        }

        /// <summary>
        /// 根据区域代码获取注销信息
        /// </summary>
        private List<CBJYQZ_QZZX> GetZoneQZZX(string zoneCode)
        {
            var q = db.CreateQuery<CBJYQZ_QZZX>();
            List<CBJYQZ_QZZX> entitys = q.Where(t => t.CBJYQZBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
            return entitys;
        }

        /// <summary>
        /// 根据权证编码获取附件
        /// </summary> 
        private List<QSLYZLFJ> GetZoneFJ(string zoneCode)
        {
            var q = db.CreateQuery<QSLYZLFJ>();
            List<QSLYZLFJ> entityList = q.Where(t => t.CBJYQZBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
            if (entityList == null || entityList.Count == 0)
            {
                return new List<QSLYZLFJ>();
            }
            List<QSLYZLFJ> entitys = new List<QSLYZLFJ>();
            foreach (QSLYZLFJ item in entityList)
            {
                entitys.Add(new QSLYZLFJ
                {
                    CBJYQZBM = item.CBJYQZBM,
                    ZLFJBH = item.ZLFJBH,
                    ZLFJMC = item.ZLFJMC,
                    FJ = item.FJ,
                    ZLFJRQ = item.ZLFJRQ
                });
            }
            entityList = null;
            return entitys;
        }

        /// <summary>
        /// 根据区域代码获取登记簿集合
        /// </summary> 
        private List<CBJYQZDJB> GetDJBCollection(string zoneCode)
        {
            if (string.IsNullOrEmpty(zoneCode))
                return null;
            var q = db.CreateQuery<CBJYQZDJB>();
            List<CBJYQZDJB> list = q.Where(t => t.CBJYQZBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
            return list;
        }

        /// <summary>
        /// 获取所有登记簿
        /// </summary> 
        private List<CBJYQZDJB> GetDJBAll()
        {
            var q = db.CreateQuery<CBJYQZDJB>();
            List<CBJYQZDJB> list = q.ToList();
            return list;
        }

        #endregion

        #region 信息筛选

        /// <summary>
        /// 根据区域代码获取权证
        /// </summary> 
        private List<CBJYQZ> GetZoneCBJYQZ2(List<CBJYQZ> list, string zoneCode)
        {
            List<CBJYQZ> entitys = list.Where(t => t.CBJYQZBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
            return entitys;
        }

        /// <summary>
        /// 根据区域代码获取补发信息
        /// </summary>
        private List<CBJYQZ_QZBF> GetZoneQZBF2(List<CBJYQZ_QZBF> list, string zoneCode)
        {
            return list.Where(t => t.CBJYQZBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
        }

        /// <summary>
        /// 根据区域代码获换发信息
        /// </summary>
        private List<CBJYQZ_QZHF> GetZoneQZHF2(List<CBJYQZ_QZHF> list, string zoneCode)
        {
            return list.Where(t => t.CBJYQZBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
        }

        /// <summary>
        /// 根据区域代码获取注销信息
        /// </summary>
        private List<CBJYQZ_QZZX> GetZoneQZZX2(List<CBJYQZ_QZZX> list, string zoneCode)
        {
            return list.Where(t => t.CBJYQZBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
        }

        /// <summary>
        /// 根据权证编码获取附件
        /// </summary> 
        private List<QSLYZLFJ> GetZoneFJ2(List<QSLYZLFJ> list, string zoneCode)
        {
            return list.Where(t => t.CBJYQZBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
        }

        /// <summary>
        /// 根据区域代码获取登记簿集合
        /// </summary> 
        private List<CBJYQZDJB> GetDJBCollection2(List<CBJYQZDJB> list, string zoneCode)
        {
            return list.Where(t => t.CBJYQZBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
        }

        /// <summary>
        /// 获取承包方
        /// </summary>
        private List<CBF> GetZoneCBF2(List<CBF> list, string zoneCode)
        {
            return list.Where(t => t.CBFBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
        }

        /// <summary>
        /// 获取承包方家庭成员
        /// </summary>
        private List<CBF_JTCY> GetZoneJTCY2(List<CBF_JTCY> list, string zoneCode)
        {
            return list.Where(t => t.CBFBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
        }

        /// <summary>
        /// 获取发包方
        /// </summary>
        private List<FBF> GetZoneFBF2(List<FBF> list, string zoneCode)
        {
            return list.Where(t => t.FBFBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
        }

        /// <summary>
        /// 获取合同
        /// </summary>
        private List<CBHT> GetZoneCBHT2(List<CBHT> list, string zoneCode)
        {
            return list.Where(t => t.CBFBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
        }

        /// <summary>
        /// 获取流转合同
        /// </summary>
        private List<LZHT> GetZoneLZHT2(List<LZHT> list, string zoneCode)
        {
            return list.Where(t => t.CBFBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
        }

        /// <summary>
        /// 转换地块信息实体
        /// </summary>
        private List<CBDKXXEX> GetZoneDKXX2(List<CBDKXXEX> landList, string zoneCode)
        {
            return landList.Where(t => t.FBFBM.Substring(0, (zoneCode).Length) == (zoneCode)).ToList();
        }

        #endregion

        #endregion
    }
}
