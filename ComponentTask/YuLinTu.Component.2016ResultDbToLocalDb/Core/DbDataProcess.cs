/*
 * (C) 2016 鱼鳞图公司版权所有,保留所有权利
*/

using Quality.Business.Entity;
using Quality.Business.TaskBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;

namespace YuLinTu.Component.ResultDbToLocalDb
{
    /// <summary>
    /// 数据库数据获取
    /// </summary>
    public class DbDataProcess
    {
        #region Properties

        /// <summary>
        /// 数据库实体
        /// </summary>
        public IDbContext Dbcotext { get; set; }

        public delegate void ReportInformation(string msg);

        /// <summary>
        /// 报告信息
        /// </summary>
        public ReportInformation ReportInfo { get; set; }

        public delegate void ReportWarningInformation(string msg);

        public ReportWarningInformation ReportWarningInfo { get; set; }

        public bool UseZoneCode { get; set; }

        #endregion Properties

        #region Ctor

        public DbDataProcess()
        {
        }

        public DbDataProcess(IDbContext db)
        {
            this.Dbcotext = db;
        }

        #endregion Ctor

        #region 数据获取

        /// <summary>
        /// 根据地域编码获取数据
        /// </summary>
        public List<T> GetData<T>(string zoneCode, Func<T, string> fuc, Action<List<T>> prcFuc = null) where T : class, new()
        {
            List<T> list = new List<T>();
            if (Dbcotext == null)
                return list;
            if (string.IsNullOrEmpty(zoneCode))
                return null;
            var q = Dbcotext.CreateQuery<T>();
            var entitys = q.Where(t => fuc(t) == zoneCode).ToList();
            if (entitys.Count > 0 && prcFuc != null)
            {
                prcFuc(entitys);
            }
            return entitys;
        }

        /// <summary>
        /// 获取本地地域编码
        /// </summary>
        public List<string> CurrentZoneCode()
        {
            var list = new List<string>();
            if (Dbcotext == null)
                return list;
            var query = Dbcotext.CreateQuery<FBF>();
            var bmList = query.Select(t => new { FBFBM = t.FBFBM }).ToList();
            foreach (var item in bmList)
            {
                string zoneCode = item.FBFBM;
                zoneCode = zoneCode.TrimEnd('0');

                if (zoneCode.Length > 12 && zoneCode.Length < 14)
                {
                    zoneCode = zoneCode.PadRight(14, '0');
                }
                if (zoneCode.Length > 9 && zoneCode.Length < 12)
                {
                    zoneCode = zoneCode.PadRight(12, '0');
                }
                if (zoneCode.Length > 6 && zoneCode.Length < 9)
                {
                    zoneCode = zoneCode.PadRight(9, '0');
                }
                if (!list.Any(t => t == zoneCode))
                {
                    list.Add(zoneCode);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取全部承包方
        /// </summary>
        public List<T> GetAll<T>(string whereString = "")
        {
            List<T> list = new List<T>();
            if (Dbcotext == null)
                return list;
            var fieldInfo = typeof(T).GetField("TableName");
            string name = fieldInfo.GetValue(null) as string;
            string sql = string.Format("select * from {0} {1}", name,
                string.IsNullOrEmpty(whereString) ? "" : " where " + whereString);
            var que = Dbcotext.CreateQuery();
            que.CommandContext.CommandText.Append(sql);
            que.CommandContext.Type = eCommandType.Query;
            list = que.GetObjects(typeof(T)) as List<T>;
            return list;
        }

        /// <summary>
        /// 获取条件数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="whereString">条件语句</param>
        /// <returns></returns>
        public List<T> GetDatasByKeyCode<T>(string fieldName, string keyCode)
        {
            List<T> list = new List<T>();

            if (Dbcotext == null)
                return list;

            var fieldInfo = typeof(T).GetField("TableName");
            string name = fieldInfo.GetValue(null) as string;
            if (!Dbcotext.CreateSchema().AnyElement(null, name))
                return list;
            string sql = string.Format("select * from {0} where {1} like '{2}%'", name, fieldName, keyCode);

            try
            {
                var que = Dbcotext.CreateQuery();
                que.CommandContext.CommandText.Append(sql);
                //que.CommandContext.Type = eCommandType.Select;
                list = que.GetObjects(typeof(T)) as List<T>;
            }
            catch
            {
                ReportWarnInfo("当前表" + name + "有异常，请检查。");
            }

            return list;
        }

        /// <summary>
        /// 根据地域编码获取数据
        /// </summary>
        public List<T> GetData<T>(string zoneCode, string sql, Action<List<T>> prcFuc = null) where T : class, new()
        {
            List<T> list = new List<T>();
            if (Dbcotext == null)
                return list;
            if (string.IsNullOrEmpty(zoneCode))
                return null;
            var que = Dbcotext.CreateQuery();
            que.CommandContext.CommandText.Append(sql);
            que.CommandContext.Type = eCommandType.Query;
            list = que.GetObjects(typeof(T)) as List<T>;
            if (list.Count > 0 && prcFuc != null)
                prcFuc(list);
            return list;
        }

        /// <summary>
        /// 获取数据数据集合
        /// </summary>
        public DataCollectionDb GetCollection(string keyCode)
        {
            DataCollectionDb dataDb = new DataCollectionDb();
            dataDb.CBJYQZJH = GetDatasByKeyCode<CBJYQZ>(CBJYQZ.FCBJYQZBM, keyCode);
            dataDb.QZBFJH = GetDatasByKeyCode<CBJYQZ_QZBF>(CBJYQZ_QZBF.FCBJYQZBM, keyCode);
            dataDb.QZHFJH = GetDatasByKeyCode<CBJYQZ_QZHF>(CBJYQZ_QZHF.FCBJYQZBM, keyCode);
            dataDb.QZZXJH = GetDatasByKeyCode<CBJYQZ_QZZX>(CBJYQZ_QZZX.FCBJYQZBM, keyCode);
            dataDb.FJJH = GetDatasByKeyCode<QSLYZLFJ>(QSLYZLFJ.FCBJYQZBM, keyCode);
            dataDb.HTJH = GetDatasByKeyCode<CBHT>(CBHT.FCBHTBM, keyCode);
            dataDb.JTCYJH = GetDatasByKeyCode<CBF_JTCY>(CBF_JTCY.FCBFBM, keyCode);
            dataDb.DJBJH = GetDatasByKeyCode<CBJYQZDJB>(CBJYQZDJB.FCBJYQZBM, keyCode);
            dataDb.LZHTJH = GetDatasByKeyCode<LZHT>(LZHT.FCBHTBM, keyCode);
            dataDb.FBFJH = GetDatasByKeyCode<FBF>(FBF.FFBFBM, keyCode);
            if (!UseZoneCode)
            {
                var cbfs = GetDatasByKeyCode<CBFSC>(CBF.FCBFBM, keyCode);
                dataDb.CBFJH = cbfs.ConvertAll(t =>
                {
                    var cbf = t;//.ConvertTo<CBFSC>();
                    cbf.XZDYBM = cbf.CBFBM.Substring(0, 14);
                    return cbf;
                });
            }
            else
            {
                var cbfs2 = GetDatasByKeyCode<CBFSC>(CBF.FCBFBM, keyCode);
                dataDb.CBFJH = GetDatasByKeyCode<CBFSC>(CBFSC.FXZDYBM, keyCode);
            }
            var dks = GetDatasByKeyCode<CBDKXXSC>(CBDKXX.FDKBM, keyCode);
            foreach (var item in dks)
            {
                if (!string.IsNullOrEmpty(item.DKLB) && item.DKLB != "10")
                {
                    item.CBHTBM = "";
                    item.HTMJ = 0;
                    item.HTMJM = 0;
                }
            }
            dataDb.DKXXJH = dks;// dks.ConvertAll(c => c.ConvertTo<CBDKXX>());
            return dataDb;
        }

        /// <summary>
        /// 获取承包方表代码,名称字典
        /// </summary>
        public Dictionary<string, string> GetPresonCodeName()
        {
            var dic = new Dictionary<string, string>();
            if (Dbcotext == null)
                return dic;
            var dyQuery = new DynamicQuery(Dbcotext);
            dyQuery.ForEach(null, CBF.TableName, (p, i, en) =>
            {
                var code = ObjectExtension.GetPropertyValue(en, CBF.FCBFBM) as string;
                code = code == null ? "" : code;
                if (!dic.ContainsKey(code))
                {
                    var name = ObjectExtension.GetPropertyValue(en, CBF.FCBFMC) as string;
                    dic.Add(code, name);
                }
                return true;
            }, new PropertySection[]
            {
                QuerySection.Property(CBF.FCBFBM, CBF.FCBFBM),
                QuerySection.Property(CBF.FCBFMC, CBF.FCBFMC),
            });
            return dic;
        }

        #endregion 数据获取

        #region 获取承包方中的行政地域编码

        /// <summary>
        /// 获取承包方中地域编码
        /// </summary>
        public List<string> GetZoneCodeByCBF(List<CBF> cbfList)
        {
            List<string> list = new List<string>();
            foreach (var item in cbfList)
            {
                if (string.IsNullOrEmpty(item.CBFBM) || item.CBFBM.Length < 14)
                {
                    continue;
                }
                string zoneCode = item.CBFBM.Substring(0, 14);
                zoneCode = zoneCode.TrimEnd('0');
                //if (zoneCode.Length == 14)
                //{
                //    zoneCode = zoneCode.Substring(0, 12) + "00" + zoneCode.Substring(12, 2);
                //}
                if (zoneCode.Length > 12 && zoneCode.Length < 14)
                {
                    zoneCode = zoneCode.PadRight(14, '0');
                }
                if (zoneCode.Length > 9 && zoneCode.Length < 12)
                {
                    zoneCode = zoneCode.PadRight(12, '0');
                }
                if (zoneCode.Length > 6 && zoneCode.Length < 9)
                {
                    zoneCode = zoneCode.PadRight(9, '0');
                }
                if (!list.Any(t => t == zoneCode))
                {
                    list.Add(zoneCode);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取发包方中地域编码
        /// </summary>
        public List<string> GetZoneCodeByFBF(List<FBF> fbfList)
        {
            List<string> list = new List<string>();
            foreach (var item in fbfList)
            {
                if (string.IsNullOrEmpty(item.FBFBM) || item.FBFBM.Length < 14)
                {
                    continue;
                }
                string zoneCode = item.FBFBM.Substring(0, 14);
                zoneCode = zoneCode.TrimEnd('0');
                //if (zoneCode.Length == 14)
                //{
                //    zoneCode = zoneCode.Substring(0, 12) + "00" + zoneCode.Substring(12, 2);
                //}
                if (zoneCode.Length > 12 && zoneCode.Length < 14)
                {
                    zoneCode = zoneCode.PadRight(14, '0');
                }
                if (zoneCode.Length > 9 && zoneCode.Length < 12)
                {
                    zoneCode = zoneCode.PadRight(12, '0');
                }
                if (zoneCode.Length > 6 && zoneCode.Length < 9)
                {
                    zoneCode = zoneCode.PadRight(9, '0');
                }
                if (!list.Any(t => t == zoneCode))
                {
                    list.Add(zoneCode);
                }
            }
            return list;
        }

        #endregion 获取承包方中的行政地域编码

        #region 构造交换数据实体

        /// <summary>
        /// 通过区域代码获取数据,转换到每个人上的集合体
        /// </summary>
        /// <param name="searchCode">发包方编码</param>
        /// <param name="zoneCode">地域编码</param>
        /// <param name="spaceLandList">地块集合</param>
        /// <param name="djbCollection">登记簿集合</param>
        /// <param name="creList">空户或自留地数据</param>
        public List<ComplexRightEntity> GetRightCollectionByZone(string searchCode, string zoneCode, Dictionary<string, DKEXP> spaceLandList,
            DataCollectionDb townCollection, Dictionary<string, string> dicCodeName, List<ComplexRightEntity> creList, List<string> noPersonLand = null)
        {
            var cbfList = townCollection.CBFJH.FindAll(t => t.CBFBM.StartsWith(searchCode));
            var rightCollections = new List<ComplexRightEntity>();
            var dataCollection = new DataCollectionDb();
            if (cbfList != null && cbfList.Count > 0)
            {
                var fbf = townCollection.FBFJH.Find(t => t.FBFBM == searchCode.PadRight(14, '0'));
                dataCollection.CBJYQZJH = FilterData(townCollection.CBJYQZJH, searchCode, (t) => { return t.CBJYQZBM; });
                dataCollection.QZBFJH = FilterData(townCollection.QZBFJH, searchCode, (t) => { return t.CBJYQZBM; });
                dataCollection.QZHFJH = FilterData(townCollection.QZHFJH, searchCode, (t) => { return t.CBJYQZBM; });
                dataCollection.QZZXJH = FilterData(townCollection.QZZXJH, searchCode, (t) => { return t.CBJYQZBM; });
                dataCollection.FJJH = FilterData(townCollection.FJJH, searchCode, (t) => { return t.CBJYQZBM; });
                dataCollection.HTJH = FilterData(townCollection.HTJH, searchCode, (t) => { return t.CBHTBM; });
                dataCollection.JTCYJH = FilterData(townCollection.JTCYJH, searchCode, (t) => { return t.CBFBM; });
                dataCollection.DJBJH = FilterData(townCollection.DJBJH, searchCode, (t) => { return t.CBJYQZBM; });
                dataCollection.LZHTJH = FilterData(townCollection.LZHTJH, searchCode, (t) => { return t.CBHTBM; });
                dataCollection.DKXXJH = FilterData(townCollection.DKXXJH, searchCode, (t) => { return t.DKBM; });
                if (dataCollection.HTJH.Count == 0)
                {
                    ReportWarnInfo("发包方编码为" + searchCode + "承包方合同为空,请检查成果库中合同编码是否对应发包方表中编码");
                }
                if (dataCollection.DKXXJH.Count == 0)
                {
                    ReportWarnInfo("发包方编码为" + searchCode + "地块信息为空,请检查签订对应地域地块编码等信息是否正确");
                }
                foreach (var cbf in cbfList)
                {
                    if (string.IsNullOrEmpty(cbf.CBFBM) || cbf.CBFBM.Length != QuantityValue.CBFBMLength)
                        continue;
                    var right = GetExchageEntity(fbf, cbf, zoneCode, spaceLandList, dataCollection, dicCodeName, creList);
                    if (right != null && right.DJB != null && right.HT != null && right.DKXX != null && right.DKXX.Count > 0 && right.FBF != null)
                    {
                        rightCollections.Add(right);
                    }
                    else if (right != null && (right.DJB.Count == 0 || right.HT.Count == 0 || right.DKXX.Count == 0 || right.FBF == null))
                    {
                        Report("承包方编码为" + cbf.CBFBM + "信息不完整:登记簿" + (right.DJB != null ? right.DJB.Count : 0) +
                            "个,合同" + (right.HT != null ? right.HT.Count : 0) + "个,地块信息" +
                            (right.DKXX != null ? right.DKXX.Count : 0) + "个,发包方" + (right.FBF != null ? 1 : 0) + "个");
                    }
                    else if (right == null)
                    {
                        Report("承包方编码为" + cbf.CBFBM + "信息不完整,未签订合同及权证,或请检查签订合同的发包方");
                    }
                }
            }
            if (noPersonLand != null)
            {
                var hs = new HashSet<string>();
                if (dataCollection.DKXXJH != null)
                    dataCollection.DKXXJH.ForEach(t => hs.Add(t.DKBM));
                foreach (var item in spaceLandList)
                {
                    if (!hs.Contains(item.Key))
                        noPersonLand.Add(item.Key);
                }
            }
            dataCollection.Clear();
            return rightCollections;
        }

        /// <summary>
        /// 筛选数据
        /// </summary> 
        public DataCollectionDb FileterDataCollection(DataCollectionDb townCollection, string searchCode)
        {
            var dataCollection = new DataCollectionDb();
            if (townCollection == null)
                return dataCollection;
            dataCollection.FBFJH = FilterData(townCollection.FBFJH, searchCode, (t) => { return t.FBFBM; });
            dataCollection.CBFJH = FilterData(townCollection.CBFJH, searchCode, (s) => { return s.XZDYBM; });
            dataCollection.CBJYQZJH = FilterData(townCollection.CBJYQZJH, searchCode, (t) => { return t.CBJYQZBM; });
            dataCollection.QZBFJH = FilterData(townCollection.QZBFJH, searchCode, (t) => { return t.CBJYQZBM; });
            dataCollection.QZHFJH = FilterData(townCollection.QZHFJH, searchCode, (t) => { return t.CBJYQZBM; });
            dataCollection.QZZXJH = FilterData(townCollection.QZZXJH, searchCode, (t) => { return t.CBJYQZBM; });
            dataCollection.FJJH = FilterData(townCollection.FJJH, searchCode, (t) => { return t.CBJYQZBM; });
            dataCollection.HTJH = FilterData(townCollection.HTJH, searchCode, (t) => { return t.CBHTBM; });
            dataCollection.JTCYJH = FilterData(townCollection.JTCYJH, searchCode, (t) => { return t.CBFBM; });
            dataCollection.DJBJH = FilterData(townCollection.DJBJH, searchCode, (t) => { return t.CBJYQZBM; });
            dataCollection.LZHTJH = FilterData(townCollection.LZHTJH, searchCode, (t) => { return t.CBHTBM; });
            dataCollection.DKXXJH = FilterData(townCollection.DKXXJH, searchCode, (t) => { return t.DKBM; });
            return dataCollection;
        }

        /// <summary>
        /// 过滤数据
        /// </summary>
        public List<T> FilterData<T>(List<T> data, string zoneCode, Func<T, string> fuc)
        {
            return data.Where(t => fuc(t).StartsWith(zoneCode)).ToList();
        }

        /// <summary>
        /// 通过权证编码获取数据
        /// </summary>
        public ComplexRightEntity GetExchageEntity(FBF fbf, CBF cbf, string zoneCode, Dictionary<string, DKEXP> spaceLandList,
            DataCollectionDb dc, Dictionary<string, string> dicCodeName, List<ComplexRightEntity> creList)
        {
            string cbfbm = cbf.CBFBM;
            ComplexRightEntity right = new ComplexRightEntity();
            right.VirtualPersonCode = cbfbm;
            right.ZoneCode = zoneCode;
            right.CBF = cbf;
            right.JTCY = dc.JTCYJH.FindAll(t => t.CBFBM == cbfbm);
            right.HT = new List<ICBHT>();
            right.FBF = fbf;
            var CBDKJH = dc.DKXXJH.FindAll(t => t.CBFBM == (cbfbm));
            right.DKXX = ChangeDKXX(CBDKJH, spaceLandList);

            var hts = dc.HTJH.FindAll(t => t.CBFBM == (cbfbm));
            foreach (var ht in hts)
            {
                right.HT.Add(ht as ICBHT);
            }
            if (right.DKXX == null || right.DKXX.Count == 0)
            {
                creList.Add(right);
                return null;
            }
            else
            {
                var qzList = new List<CBJYQZ>();
                var djbList = new List<CBJYQZDJB>();
                var bfList = new List<CBJYQZ_QZBF>();
                var hfList = new List<CBJYQZ_QZHF>();
                var zxList = new List<CBJYQZ_QZZX>();
                var fjList = new List<QSLYZLFJ>();
                foreach (var ht in right.HT)
                {
                    var qz = dc.CBJYQZJH.Find(t => t.CBJYQZBM == ht.CBHTBM);
                    if (qz != null)
                        qzList.Add(qz);

                    var bf = dc.QZBFJH.FindAll(t => t.CBJYQZBM == ht.CBHTBM);
                    bfList.AddRange(bf);

                    var hf = dc.QZHFJH.FindAll(t => t.CBJYQZBM == ht.CBHTBM);
                    hfList.AddRange(hf);

                    var zx = dc.QZZXJH.Find(t => t.CBJYQZBM == ht.CBHTBM);
                    if (zx != null)
                        zxList.Add(zx);

                    var djb = dc.DJBJH.Find(t => t.CBJYQZBM == ht.CBHTBM);
                    if (djb != null)
                        djbList.Add(djb);

                    var fj = dc.FJJH.FindAll(t => t.CBJYQZBM == ht.CBHTBM);
                    fjList.AddRange(fj);
                }
                right.DJB = djbList;
                right.CBJYQZ = qzList;
                right.QZBF = bfList;
                right.QZHF = hfList;
                right.QZZX = zxList;
                right.FJ = fjList;

                var htList = dc.LZHTJH.FindAll(t => t.CBFBM == (cbfbm));
                var exchangeList = new List<LZHTEX>();
                htList.ForEach(t => exchangeList.Add(ChangeLZHT(t, dicCodeName)));
                right.LZHT = exchangeList;
                //var CBDKJH = dc.DKXXJH.FindAll(t => t.CBFBM == (cbfbm));
                //right.DKXX = ChangeDKXX(CBDKJH, spaceLandList);
                right.VirtualPersonCode = right.CBF == null ? "" : right.CBF.CBFBM;
                right.ZoneCode = zoneCode;
            }

            return right;
        }

        /// <summary>
        /// 转换流转合同
        /// </summary>
        public LZHTEX ChangeLZHT(LZHT ht, Dictionary<string, string> dicCodeName)
        {
            var lzht = new LZHTEX();
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
            lzht.CBHTBM = ht.CBHTBM;
            if (!string.IsNullOrEmpty(lzht.SRFBM))
            {
                lzht.SRFMC = "";
                if (dicCodeName.ContainsKey(lzht.SRFBM))
                    lzht.SRFMC = dicCodeName[lzht.SRFBM];
            }
            return lzht;
        }

        /// <summary>
        /// 转换地块信息实体
        /// </summary>
        public List<CBDKXXEX> ChangeDKXX(List<CBDKXXSC> landList,
            Dictionary<string, DKEXP> spaceLandList, List<DKEXP> lands = null)
        {
            var dkCollection = new List<CBDKXXEX>();
            var hashCode = new HashSet<string>();
            foreach (var dkxx in landList)
            {
                hashCode.Add(dkxx.DKBM);
                var cbd = new CBDKXXEX()
                {
                    CBHTBM = dkxx.CBHTBM,
                    CBJYQQDFS = dkxx.CBJYQQDFS,
                    CBJYQZBM = dkxx.CBJYQZBM,
                    CBFBM = dkxx.CBFBM,
                    DKBM = dkxx.DKBM,
                    FBFBM = dkxx.FBFBM,
                    SFQQQG = dkxx.SFQQQG,
                    HTMJ = Math.Round(dkxx.HTMJ, 2),
                    HTMJM = dkxx.HTMJM,
                    YHTMJ = dkxx.YHTMJ != null ? Math.Round(dkxx.YHTMJ.Value, 2) : 0,
                    YHTMJM = dkxx.YHTMJM,
                    LZHTBM = dkxx.LZHTBM
                };
                if (dkxx.YHTMJM == 0 && dkxx.YHTMJ != 0)
                {
                    cbd.YHTMJM = dkxx.YHTMJ;
                }
                if (spaceLandList.ContainsKey(dkxx.DKBM))
                {
                    cbd.KJDK = spaceLandList[dkxx.DKBM];
                }
                if (cbd.KJDK == null)
                    Report("地块编码" + cbd.DKBM + "的空间数据为空!");
                dkCollection.Add(cbd);
            }
            if (lands != null)
            {
                foreach (var item in spaceLandList)
                {
                    if (hashCode.Contains(item.Key))
                        continue;
                    lands.Add(item.Value);
                }
            }
            return dkCollection;
        }

        /// <summary>
        /// 转换地块信息实体
        /// </summary>
        public List<CBDKXXEX> ChangeDKXX(List<DKEX> landList, CBF cbf, FBF fbf)
        {
            var dkCollection = new List<CBDKXXEX>();
            foreach (var dkxx in landList)
            {
                var cbd = new CBDKXXEX()
                {
                    CBFBM = cbf.CBFBM,
                    DKBM = dkxx.DKBM,
                    FBFBM = fbf.FBFBM,
                };
                cbd.KJDK = dkxx;
                if (cbd.KJDK != null)
                    dkCollection.Add(cbd);
            }
            return dkCollection;
        }

        #endregion 构造交换数据实体

        #region 辅助方法

        public void Report(string msg)
        {
            if (ReportInfo != null && !string.IsNullOrEmpty(msg))
                ReportInfo(msg);
        }

        public void ReportWarnInfo(string msg)
        {
            if (ReportWarningInfo != null && !string.IsNullOrEmpty(msg))
                ReportWarningInfo(msg);
        }

        #endregion 辅助方法
    }
}