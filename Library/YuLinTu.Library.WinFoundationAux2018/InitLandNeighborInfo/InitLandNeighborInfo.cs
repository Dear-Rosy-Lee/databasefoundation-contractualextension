using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YuLinTu.Library.WinFoundationAux.Util;
//using YuLinTu.Library.Entity;
using YuLinTu.NetAux;
using YuLinTu.NetAux.CglLib;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 查找地块的相邻地块
    /// </summary>
    public class InitLandNeighborInfo
    {
        /// <summary>
        /// 辅助类
        /// </summary>
        class TableBase
        {
            /// <summary>
            /// DKID于界址点rowid的映射
            /// </summary>
            protected readonly Dictionary<string, List<int>> _dic = new Dictionary<string, List<int>>();

            protected readonly InitLandNeighborInfo _p;
            protected readonly string[] _fields;
            protected readonly SQLiteParam[] _updateParams;

            public readonly List<int> _delRowids = new List<int>();

            protected int _nTestUpdateCount = 0;

            protected readonly string _tableName;

            //protected Action _afterSaveDeletedRows=null;
            public TableBase(InitLandNeighborInfo p_, string tableName, string[] fields)
            {
                _p = p_;
                _tableName = tableName;

                _fields = fields;
                _updateParams = new SQLiteParam[_fields.Length + 1];

                for (int i = 0, j = 0; i < _fields.Length; ++i)
                {
                    if (_fields[i] == Zd_cbdFields.Shape)
                        continue;
                    _updateParams[j++] = new SQLiteParam()
                    {
                        ParamName = _fields[i]
                    };
                }
                _updateParams[_updateParams.Length - 1] = new SQLiteParam() { ParamName = "rowid" };
            }

            public void save(ShortZd_cbd1 cbd)
            {
                var db = _p._db;
                var initParam = _p._param;
                var prms = _updateParams;

                int i = 0;

                string savestri = string.Empty;
                if (cbd.lstNeibors == null)
                    savestri = " ";
                else if (cbd.lstNeibors.Count > 0)
                {
                    var listneibors = cbd.lstNeibors.ToList();
                    List<ShortZd_cbd1> savelistneibors = new List<ShortZd_cbd1>();

                    foreach (var item in listneibors)
                    {
                        if (savelistneibors.Any(a => a.rowid == item.rowid))
                            continue;
                        savelistneibors.Add(item);
                    }

                    foreach (var item in savelistneibors)
                    {
                        if (savestri.IsNullOrEmpty())
                            savestri = item.dkID;
                        else
                            savestri += "," + item.dkID;
                    }
                }

                if (savestri.IsNullOrEmpty()) savestri = " ";

                prms[i].ParamValue = savestri;
                //prms[++i].ParamValue = cbd.dkID;
                prms[++i].ParamValue = cbd.rowid;
                var updateSql = InitLandDotCoilUtil.constructUpdateSql(_tableName, _fields, "rowid=@rowid");
                var ret = db.ExecuteNonQuery(updateSql, prms);
                ++_nTestUpdateCount;

            }


        }

        /// <summary>
        /// 保存缓存
        /// </summary>
        class SaveCacheHelper
        {
            private readonly InitLandNeighborInfo p;
            private readonly List<ShortZd_cbd1> _cacheList = new List<ShortZd_cbd1>();
            private readonly List<ShortZd_cbd1> _saveList = new List<ShortZd_cbd1>();
            private readonly List<ShortZd_cbd1> _releaseList = new List<ShortZd_cbd1>();

            public SaveCacheHelper(InitLandNeighborInfo p_)
            {
                p = p_;
            }
            public void Add(ShortZd_cbd1 en)
            {
                _cacheList.Add(en);
                if (_cacheList.Count >= 500)
                {
                    for (int i = _cacheList.Count - 1; i >= 0; --i)
                    {
                        var c = _cacheList[i];
                        if (canSave(c))
                        {
                            _cacheList.RemoveAt(i);
                            _saveList.Add(c);
                        }
                    }
                }

                if (_saveList.Count >= 500)
                {
                    Flush(false);
                }
            }

            public void Clear()
            {
                _cacheList.Clear();
                _saveList.Clear();
                _releaseList.Clear();
            }
            public void Flush(bool fLast)
            {
                if (fLast)
                {
                    foreach (var c in _cacheList)
                    {
                        _saveList.Add(c);
                        string lz = string.Empty;

                        //if (c.lstNeibors != null)
                        //{
                        //    var lzs = c.lstNeibors.ToList();

                        //    foreach (var item in lzs)
                        //    {
                        //        lz += "," + item.dkID.ToString();
                        //    }
                        //}

                        //Console.WriteLine("输出临宗：本宗为:" + c.dkID + "临宗为:" + lz + "\n");
                    }
                    _cacheList.Clear();
                    Console.WriteLine("正在保存最后一个缓存：" + _saveList.Count + "个");
                }
                else
                {
                    Console.WriteLine("正在保存缓存：" + _saveList.Count + "个");
                }
                TableBase tabel = new TableBase(p, Zd_cbdFields.TABLE_NAME, new string[]{
                    Zd_cbdFields.YLD
                });
                var trans = p._db.BeginTransaction();
                try
                {

                    foreach (var c in _saveList)
                    {
                        tabel.save(c);
                        c.fSaved = true;
                        _releaseList.Add(c);
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    var dd = ex.Message;
                }


                _saveList.Clear();

                if (_releaseList.Count >= 500)
                {
                    tryRelease();
                }

            }

            private void tryRelease()
            {
                for (int i = _releaseList.Count - 1; i >= 0; --i)
                {
                    var c = _releaseList[i];
                    bool fCanRelease = true;
                    if (c.lstNeibors != null)
                    {
                        foreach (var nb in c.lstNeibors)
                        {
                            if (!nb.fSaved)
                            {
                                fCanRelease = false;
                                break;
                            }
                        }
                    }
                    if (fCanRelease)
                    {
                        c.Clear();
                        _releaseList.RemoveAt(i);
                    }
                }
            }

            /// <summary>
            /// 判断一个地块是否可以进行保存；
            /// </summary>
            /// <param name="cbd"></param>
            /// <returns></returns>
            private bool canSave(ShortZd_cbd1 cbd)
            {
                bool fRemove = true;
                if (cbd.lstNeibors != null)
                {
                    foreach (var nb in cbd.lstNeibors)
                    {
                        if (!nb.fRemovedFromCache)
                        {
                            fRemove = false;
                            break;
                        }
                    }
                }

                return fRemove;
            }
        }
        private readonly DBSpatialite _db;
        private readonly InitLandDotCoilParam _param;

        private int _curProgress, _oldProgress;//进度相关
        private int _progressCount { get { return _rowids.Count; } }
        private int _srid;


        /// <summary>
        /// 待处理地块的rowid集合
        /// </summary>
        private readonly HashSet<int> _rowids = new HashSet<int>();


        /// <summary>
        /// 承包地缓存
        /// </summary>
        private readonly ShortZd_cbdCache1 _cbdCache = new ShortZd_cbdCache1();

        private readonly SaveCacheHelper _saveCache;
        private readonly XzdyUtil _xzdyUtil = new XzdyUtil();

        public Action<string, int> ReportProgress;
        public Action<string> ReportInfomation;

        public InitLandNeighborInfo(DBSpatialite db, InitLandDotCoilParam param)
        {
            _db = db;
            _param = param;
            _srid = db.QuerySRID(Zd_cbdFields.TABLE_NAME);

            _saveCache = new SaveCacheHelper(this);
        }
        /// <summary>
        /// 开始初始化界址点和界址线
        /// </summary>
        /// <param name="wh"></param>
        public void DoInit(string wh)
        {

            ReportInfomation("开始时间：" + DateTime.Now);
            ReportInfomation("开始确定范围及数据行");
            _rowids.Clear();
            _curProgress = 0;
            _oldProgress = 0;

            _cbdCache.Clear();
            _saveCache.Clear();
            _param.cjsj = DateTime.Now;


            var lstXBounds = new List<XBounds>();
            var env = _db.QueryEnvelope(Zd_cbdFields.TABLE_NAME, Zd_cbdFields.Shape, wh, new List<string> { "rowid" }, r =>
            {
                _rowids.Add(r.GetInt32(1));
            });

            ReportInfomation("范围确定，包含处理地块个数共计：" + _rowids.Count + "个");
            if (_rowids.Count == 0)
            {
                ReportInfomation("选择地域没有空间地块数据，请检查!");
                return;
            }

            env.ExpandBy(_param.Tolerance);

            ReportInfomation("开始进行大范围内查找：" + DateTime.Now);

            var icc = new IntCoordConter();
            icc.Init(env.MinX);
            _db.QueryIntersectsCallback(Zd_cbdFields.TABLE_NAME, Zd_cbdFields.Shape, env, r =>
            {
                var x = new XBounds();
                x.rowid = SafeConvertAux.SafeConvertToInt32(r.GetValue(0));
                var e = WKBHelper.fromWKB(r.GetValue(1) as byte[]).EnvelopeInternal;
                x.minx = icc.toInt(e.MinX);
                x.maxx = icc.toInt(e.MaxX);
                lstXBounds.Add(x);
                return true;
            }, new List<string> { Zd_cbdFields.RowID, Zd_cbdFields.Shape });


            lstXBounds.Sort((a, b) =>
            {
                if (a.minx < b.minx)
                    return -1;
                if (a.minx > b.minx)
                    return 1;
                if (a.maxx == b.maxx)
                    return 0;
                return a.maxx < b.maxx ? -1 : 1;
            });

            int nTestMaxCacheSize = 0;

            ReportInfomation("大范围内查找排序结束：" + DateTime.Now);
            ReportInfomation("开始进行批量查询：" + DateTime.Now);
            var cacheRowids = new List<int>();
            Dictionary<int, ShortZd_cbd1> dicCbd = new Dictionary<int, ShortZd_cbd1>();

            for (int i = 0; i < lstXBounds.Count;)
            {
                int j = i + 100;
                if (j >= lstXBounds.Count)
                {
                    j = lstXBounds.Count - 1;
                }
                for (int k = i; k <= j; ++k)
                {
                    cacheRowids.Add(lstXBounds[k].rowid);
                }
                InitLandDotCoilUtil.QueryShortZd_cbd1(_db, cacheRowids, dicCbd, _param.Tolerance, en =>
                {
                    en.fSelected = _rowids.Contains(en.rowid);// isSelected(en);
                });
                for (int k = i; k <= j; ++k)
                {
                    var xb = lstXBounds[k];
                    ShortZd_cbd1 cbd;
                    if (dicCbd.TryGetValue(xb.rowid, out cbd))
                    {
                        var fProcessed = processLeft(cbd);
                        if (_cbdCache.Count > nTestMaxCacheSize)
                        {
                            nTestMaxCacheSize = _cbdCache.Count;
                        }
                    }
                }
                dicCbd.Clear();
                cacheRowids.Clear();
                i = j + 1;
            }
            processLeft(null);

            ReportProgress("保存缓存数据", 100);
            _saveCache.Flush(true);

            ReportInfomation("结束时间：" + DateTime.Now);

            _db.Close();
        }
        private bool processLeft(ShortZd_cbd1 current)
        {         
            var left = _cbdCache;
            if (current != null)
            {
                if (current.dkGeo == null || current.dkGeo.IsEmpty)
                    return false;

                var currentCbdEnv = current.dkGeo.EnvelopeInternal;


                if (left.Count == 0)
                {
                    left.Add(current);
                    left.x1 = currentCbdEnv.MaxX - _param.Tolerance;
                    return false;
                }
                left.Add(current);
                if (left.x1 > currentCbdEnv.MaxX - _param.Tolerance)
                {
                    left.x1 = currentCbdEnv.MaxX - _param.Tolerance;
                }
                if (left.x1  > currentCbdEnv.MinX)
                {
                    return false;
                }
            }
            var tolerance = _param.Tolerance;

            #region 查找毗邻地


            for (int i = 0; i < left.Count; i++)
            {
                var cbd = left[i];
                var cbdenv = cbd.dkGeo.EnvelopeInternal;

                for (int j = 0; j < left.Count; j++)
                {
                    if (j == i)
                        continue;
                    var cbdJ = left[j];
                    var cbdjenv = cbdJ.dkGeo.EnvelopeInternal;

                    if (cbdjenv.MaxX < cbdenv.MinX + tolerance)
                    {
                        continue;
                    }
                    if (cbdjenv.MaxY < cbdenv.MinY + tolerance)
                    {
                        continue;
                    }

                    if (cbdjenv.MinY + tolerance > cbdenv.MaxY)
                    {
                        continue;
                    }


                    if (cbdenv.MaxX < cbdjenv.MinX + tolerance)
                    {
                        break;
                    }

                    var intbool = cbdenv.Intersects(cbdjenv);

                    if (intbool)
                    {
                        var geointbool = cbd.dkGeo.Intersects(cbdJ.dkGeo);

                        if (geointbool == false) continue;

                        if (cbd.lstNeibors == null) cbd.lstNeibors = new HashSet<ShortZd_cbd1>();
                        if (cbdJ.lstNeibors == null) cbdJ.lstNeibors = new HashSet<ShortZd_cbd1>();

                        if (cbd.lstNeibors.Contains(cbdJ) == false)
                        {
                            cbd.lstNeibors.Add(cbdJ);
                        }

                        if (cbdJ.lstNeibors.Contains(cbd) == false)
                        {
                            cbdJ.lstNeibors.Add(cbd);
                        }
                    }
                }
            }

            #endregion

            #region 将left ShortZd_cbdCache中所有在当前地块最左边缓冲距离前完全出现的地块加入到保存缓存中并从left ShortZd_cbdCache中移除
            double? x1 = null;
            for (int i = left.Count - 1; i >= 0; --i)
            {
                var cbd = left[i];
                var cbdenv = cbd.dkGeo.EnvelopeInternal;

                if (current == null || current.dkGeo==null|| cbdenv.MaxX < current.dkGeo.EnvelopeInternal.MinX + _param.Tolerance)
                {
                    if (cbd.fSelected)
                    {
                        ++_curProgress;
                        reportProgress();
                    }

                    left.RemoveAt(i);
                    cbd.fRemovedFromCache = true;
                    _saveCache.Add(cbd);

                }
                else
                {
                    if (x1 == null || (double)x1 < cbdenv.MaxX - _param.Tolerance)
                    {
                        x1 = cbdenv.MaxX - _param.Tolerance;
                    }
                }
            }
            if (x1 != null)
            {
                left.x1 = (double)x1;
            }
            #endregion

            return true;
        }

        private void reportProgress(string msg = "初始化地块信息")
        {
            if (ReportProgress != null)
            {
                ProgressHelper.ReportProgress(ReportProgress, msg, _progressCount, _curProgress, ref _oldProgress);
            }
        }
    }

    /// <summary>
    /// 记录简要信息的承包地类
    /// </summary>
    public class ShortZd_cbd1
    {
        private short _bit = 0;
        public bool fRemovedFromCache
        {
            get
            {
                return (_bit & 1) == 1;
            }
            set
            {
                if (value)
                {
                    _bit |= 1;
                }
                else
                {
                    _bit &= 0xFE;
                }
            }
        }
        /// <summary>
        /// 是否被用户选中要生成界址点和界址线的地块
        /// </summary>
        public bool fSelected
        {
            get
            {
                return (_bit & (1 << 1)) != 0;
            }
            set
            {
                if (value)
                {
                    _bit |= 1 << 1;
                }
                else
                {
                    _bit &= ~(1 << 1);
                }
            }
        }

        public bool fSaved
        {
            get
            {
                return (_bit & (1 << 2)) != 0;
            }
            set
            {
                if (value)
                {
                    _bit |= 1 << 2;
                }
                else
                {
                    _bit &= ~(1 << 2);
                }
            }
        }

        public int rowid;
        //public double xmin;
        //public double xmax;
        //public double ymin;
        //public double ymax;

        //public Envelope EnvelopeInternal;


        /// <summary>
        /// 地块标识
        /// </summary>
        public string dkID;

        public IGeometry dkGeo;

        /// <summary>
        /// 与该地块相邻的所有地块（近似判断）；
        /// </summary>
        public HashSet<ShortZd_cbd1> lstNeibors = null;

        /// <summary>
        /// 清理，避免循环引用
        /// </summary>
        public void Clear()
        {
            if (fRemovedFromCache)
            {
                if (lstNeibors != null)
                {
                    lstNeibors.Clear();
                    lstNeibors = null;
                }

                dkGeo = null;
            }
        }
    }
}

