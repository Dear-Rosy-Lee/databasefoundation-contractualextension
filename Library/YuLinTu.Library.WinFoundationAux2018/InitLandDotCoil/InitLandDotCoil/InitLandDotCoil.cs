using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.NetAux;

namespace YuLinTu.Library.Aux
{
    /// <summary>
    /// 初始化界址点和界址线业务类
    /// </summary>
    public class InitLandDotCoil
    {
        private const int minPointCount = 5;
        /// <summary>
        /// 当前要查询的线面状地物
        /// </summary>
        List<XzdwEntity> queryXzdws = new List<XzdwEntity>();
        List<MzdwEntity> queryMzdws = new List<MzdwEntity>();
        /// <summary>
        /// 界址点表辅助类
        /// </summary>
        class TableBase
        {
            /// <summary>
            /// DKID于界址点rowid的映射
            /// </summary>
            protected readonly Dictionary<string, List<int>> _dic = new Dictionary<string, List<int>>();

            protected readonly InitLandDotCoil _p;
            protected readonly string[] _fields;
            protected readonly SQLiteParam[] _updateParams;
            protected readonly SQLiteParam[] _insertParams;
            //private readonly List<JzdEntity> _jzdCache = new List<JzdEntity>();
            public readonly List<int> _delRowids = new List<int>();

            protected int _nTestInsertCount = 0;
            protected int _nTestUpdateCount = 0;

            protected readonly string _tableName;

            //protected Action _afterSaveDeletedRows=null;
            protected TableBase(InitLandDotCoil p_, string tableName, string[] fields)
            {
                _p = p_;
                _tableName = tableName;

                _fields = fields;
                _updateParams = new SQLiteParam[_fields.Length];
                _insertParams = new SQLiteParam[_fields.Length - 1];

                for (int i = 0, j = 0; i < _fields.Length; ++i)
                {
                    if (_fields[i] == JzdFields.SHAPE)
                        continue;
                    _updateParams[j] = new SQLiteParam()
                    {
                        ParamName = _fields[i]
                    };
                    _insertParams[j++] = new SQLiteParam()
                    {
                        ParamName = _fields[i]
                    };
                }
                _updateParams[_updateParams.Length - 1] = new SQLiteParam() { ParamName = "rowid" };
            }
            public void init(string dkidFieldName)
            {
                _dic.Clear();
                var sql = string.Format("select {0},rowid from {1} where {2} is not null", dkidFieldName, _tableName, dkidFieldName);
                _p._db.QueryCallback(sql, r =>
                {
                    var id = r.GetString(0);
                    List<int> lst;
                    if (!_dic.TryGetValue(id, out lst))
                    {
                        lst = new List<int>();
                        _dic[id] = lst;
                    }
                    lst.Add(r.GetInt32(1));

                    //var g=WKBHelper.fromWKB(r.GetValue(2) as byte[]);
                    return true;
                });
            }
            public void saveDeletedRowids()//DBSpatialite db)
            {
                Console.WriteLine("正在删除多余的" + _tableName + "：" + _delRowids.Count + "个");
                InitLandDotCoilUtil.DeleteByRowids(_p._db, _tableName, _delRowids);
            }
            /// <summary>
            /// 获取添加和修改的记录总数
            /// </summary>
            /// <returns></returns>
            public int GetCreatedCount()
            {
                return _nTestInsertCount + _nTestUpdateCount;
            }
            public void Clear()
            {
                _dic.Clear();
                _delRowids.Clear();

                _nTestInsertCount = 0;
                _nTestUpdateCount = 0;
            }
            public void testLogout(string tableAlias)
            {
                Console.WriteLine("修改" + _tableName + "：" + _nTestUpdateCount + "条");
                Console.WriteLine("插入" + _tableName + "：" + _nTestInsertCount + "条");
                Console.WriteLine("删除" + _tableName + "：" + _delRowids.Count + "条");
            }
            /// <summary>
            /// 根据地块ID查找对应的rowid集合（jzd表）
            /// </summary>
            /// <param name="dkid"></param>
            /// <returns></returns>
            public List<int> GetRowidsByDkid(string dkid)
            {
                List<int> lst;
                if (_dic.TryGetValue(dkid, out lst))
                    return lst;
                return null;
            }

        }

        /// <summary>
        /// 界址点表辅助类
        /// </summary>
        class JzdTable : TableBase
        {
            private readonly JzxTable _jzxTable;

            public JzdTable(InitLandDotCoil p_)
                : base(p_, JzdFields.TABLE_NAME, new string[]{
                    JzdFields.ID,
                    JzdFields.TBJZDH,
                    JzdFields.JZDH,
                    JzdFields.DKID,
                    JzdFields.DYBM,
                    JzdFields.JBLX,
                    JzdFields.JZDLX,
                    JzdFields.DKBM,
                    JzdFields.CJSJ,
                    JzdFields.SFKY,
                    JzdFields.JZDSSQLLX,
                    JzdFields.SHAPE,
                })
            {
                _jzxTable = new JzxTable(p_);
            }
            public void init()
            {
                base.init(JzdFields.DKID);
                _jzxTable.init(JzxFields.DKID);
            }
            /// <summary>
            /// 获取生成的界址线的总数
            /// </summary>
            /// <returns></returns>
            public int GetCreatedJzxCount()
            {
                return _jzxTable.GetCreatedCount();
            }
            public void Clearing()
            {
                base.Clear();
                _jzxTable.Clearing();
            }
            public void SaveDeletedRowids()
            {
                base.saveDeletedRowids();
                _jzxTable.saveDeletedRowids();
            }

            public void testLogout()
            {
                base.testLogout("界址点");
                _jzxTable.testLogout();
            }
            /// <summary>
            /// 保存界址点
            /// </summary>
            /// <param name="db"></param>
            /// <param name="initParam"></param>
            /// <param name="lst"></param>
            public void Save(ShortZd_cbd cbd)//DBSpatialite db, InitLandDotCoilParam initParam, List<JzdEntity> lst,int srid)
            {
                if (!cbd.fSelected)
                    return;
                var initParam = _p._param;
                var db = _p._db;
                var srid = _p._srid;
                var cjsj = initParam.cjsj;// DateTime.Now;//.ToString();

                var jzdRowids = GetRowidsByDkid(cbd.dkID);
                if (jzdRowids != null)
                {
                    for (int i = cbd.lstJzdEntity.Count; i < jzdRowids.Count; ++i)
                    {
                        _delRowids.Add(jzdRowids[i]);
                    }
                }

                int j = 0;
                foreach (var en in cbd.lstJzdEntity)
                {
                    if (jzdRowids != null && j < jzdRowids.Count)
                    {
                        en.rowID = jzdRowids[j++];
                    }
                    save(cbd, en);
                }

                _jzxTable.Save(cbd);

            }

            private void save(ShortZd_cbd cbd, JzdEntity en)
            {
                var db = _p._db;
                var initParam = _p._param;
                var fUpdate = en.rowID > 0;
                var prms = fUpdate ? _updateParams : _insertParams;
                var geomText = en.shape == null ? "null" : "GeomFromText('POINT(" + en.shape.X + " " + en.shape.Y + ")'," + _p._srid + ")";
                int i = 0;
                if (_p.onPresaveJzd != null)
                {
                    _p.onPresaveJzd(en);
                }
                prms[i].ParamValue = en.ID;
                //prms[++i].ParamValue = en.BSM;
                prms[++i].ParamValue = initParam.AddressPointPrefix + en.TBJZDH;// (++_nStartTBJZDH);//
                prms[++i].ParamValue = initParam.AddressPointPrefix + en.JZDH;
                prms[++i].ParamValue = cbd.dkID;// en.dkID;
                prms[++i].ParamValue = cbd.zlDM;// en.DYBM;
                prms[++i].ParamValue = en.JBLX.ToString();
                prms[++i].ParamValue = en.JZDLX.ToString();
                prms[++i].ParamValue = cbd.DKBM;// en.JZDLX.ToString();
                prms[++i].ParamValue = initParam.cjsj;// cjsj;
                prms[++i].ParamValue = en.SFKY;
                prms[++i].ParamValue = en.JZDSSQLLX;
                if (fUpdate)
                {
                    prms[++i].ParamValue = en.rowID;
                    var updateSql = InitLandDotCoilUtil.constructUpdateSql(_tableName, _fields, "rowid=@rowid"
                        , geomText);
                    db.ExecuteNonQuery(updateSql, prms);
                    ++_nTestUpdateCount;
                }
                else
                {
                    var insertSql = InitLandDotCoilUtil.constructInsertSql(_tableName, _fields, geomText);
                    db.ExecuteNonQuery(insertSql, prms);
                    ++_nTestInsertCount;
                }
            }
        }

        /// <summary>
        /// 界址线表辅助类
        /// </summary>
        class JzxTable : TableBase
        {
            private readonly List<JzdEntity> _points = new List<JzdEntity>();
            private readonly JzxEntity _jzxEn = new JzxEntity();
            private readonly HashSet<ShortZd_cbd> _qlrSet = new HashSet<ShortZd_cbd>();
            private readonly Dictionary<ShortZd_cbd, IGeometry> _polygonCache = new Dictionary<ShortZd_cbd, IGeometry>();

            /// <summary>
            /// 非正常释放的地块数
            /// </summary>
            private int _nTestReleasedDk = 0;

            public JzxTable(InitLandDotCoil p_)
                : base(p_, JzxFields.TABLE_NAME, new string[]{
                    JzxFields.ID,
                    JzxFields.JZXCD,
                    JzxFields.JXXZ,
                    JzxFields.JZXLB,
                    JzxFields.JZXWZ,
                    JzxFields.JZXSXH,
                    JzxFields.JZXQD,
                    JzxFields.JZXZD,
                    JzxFields.DKID,
                    JzxFields.CJSJ,
                    JzxFields.ZHXGSJ,
                    JzxFields.JZXSM,
                    JzxFields.PLDWZJR,
                    JzxFields.PLDWQLR,
                    JzxFields.DYDM,
                    JzxFields.TDQSLX,
                    JzxFields.DKBM,
                    JzxFields.JZXQDH,
                    JzxFields.JZXZDH,
                    JzxFields.SHAPE,
                })
            { }

            public void Clearing()
            {
                base.Clear();
                _points.Clear();
                _qlrSet.Clear();
                _polygonCache.Clear();
                _nTestReleasedDk = 0;
            }

            /// <summary>
            /// 保存界址点/修改保存界址线信息
            /// </summary>
            /// <param name="db"></param>
            /// <param name="initParam"></param>
            /// <param name="lst"></param>
            public void Save(ShortZd_cbd cbd)//DBSpatialite db, InitLandDotCoilParam initParam, List<JzdEntity> lst,int srid)
            {
                var initParam = _p._param;
                var jzxRowids = GetRowidsByDkid(cbd.dkID);
                int i = 0;
                queryPoints(cbd, points =>
                {
                    var jzxEn = _jzxEn;
                    jzxEn.Clear();
                    if (jzxRowids != null)
                    {
                        if (i < jzxRowids.Count)
                        {
                            _jzxEn.rowid = jzxRowids[i];
                        }
                    }
                    var jzdEnd = points[points.Count - 1];
                    jzxEn.JZXQD = points[0].ID;
                    jzxEn.JZXZD = jzdEnd.ID;
                    jzxEn.JZXQDH = initParam.AddressPointPrefix + points[0].JZDH;
                    jzxEn.JZXZDH = initParam.AddressPointPrefix + jzdEnd.JZDH;
                    jzxEn.TDQSLX = jzdEnd.JZDSSQLLX;
                    jzxEn.JZXLB = initParam.AddressLineCatalog;
                    jzxEn.JZXWZ = initParam.AddressLinePosition;

                    ILineString shape;
                    //毗邻地//如果是自己，界址线位置为外，其它信息还是自己，因为界址线位置外是自有
                    bool fTwin;//是否同毗邻地田挨田（无间隙）
                    ShortZd_cbd pld;//毗邻地
                    jzxEn.JZXCD = (double)Math.Round((decimal)calcLength(cbd, points, out shape, out pld, out fTwin), 2);

                    //如果范围内什么都没有
                    if (_qlrSet.Count == 0 || pld == null)
                    {
                        //如果没有权利人则取该地块的村名称（含乡镇名）
                        jzxEn.PLDWQLR = _p._xzdyUtil.GetShortQmc(cbd.zlDM);
                        jzxEn.PLDWZJR = jzxEn.PLDWQLR;
                        if (_p.queryXzdws.Count == 0 && _p.queryMzdws.Count == 0)
                        {
                            jzxEn.JZXWZ = JzxwzType.Right;
                        }
                    }

                    //如果共边，就是中
                    if (fTwin && pld != null)
                    {
                        saveJzxinfo(jzxEn, cbd, pld, JzxwzType.Middle);
                    }
                    else //就是范围内看谁近，如果是线 面状地物近，就是内，如果是地块近，就是外
                    {
                        bool hasgetxz = false;
                        bool hasgetmz = false;
                        bool hasgetdk = false;

                        Dictionary<object, int> nowgetmaxinfos = new Dictionary<object, int>();

                        XzdwEntity maxinterxzdw = null;
                        IGeometry maxinterxzdwgeo = null;

                        MzdwEntity maxintermzdw = null;
                        IGeometry maxintermzdwgeo = null;

                        ShortZd_cbd maxintercbd = null;
                        IGeometry maxintercbdgeo = null;

                        //循环缓冲距离下的buffer,先找出相交的数据
                        var bufferlines = gettargetlinebuffer(shape, initParam.AddressLinedbiDistance);
                        var intermzdws = _p.queryMzdws.FindAll(qf => qf.Shape.Intersects(bufferlines[bufferlines.Count - 1]));
                        var interxzdws = _p.queryXzdws.FindAll(qf => qf.Shape.Intersects(bufferlines[bufferlines.Count - 1]));

                        #region 查找相交的地物
                        for (int bf = 0; bf < bufferlines.Count; bf++)
                        {
                            if (_qlrSet.Count > 0 && hasgetdk == false)
                            {
                                double maxinterarea = 0.0;
                                foreach (var cbditem in _qlrSet)
                                {
                                    var g = getCachePolygon(cbditem);
                                    var intergeo = g.Intersection(bufferlines[bf]);
                                    if (intergeo.IsEmpty)
                                        continue;
                                    if (g != null && intergeo != null)
                                    {
                                        var interarea = Math.Abs(intergeo.Area);
                                        if (maxintercbd == null)
                                        {
                                            maxintercbd = cbditem;
                                            maxintercbdgeo = intergeo;
                                        }
                                        else if (maxinterarea < interarea)
                                        {
                                            maxinterarea = interarea;
                                            maxintercbd = cbditem;
                                            maxintercbdgeo = intergeo;
                                        }
                                    }
                                }

                                if (maxintercbd != null)
                                {
                                    hasgetdk = true;
                                    nowgetmaxinfos[maxintercbd] = bf;
                                }
                            }

                            if (interxzdws.Count() > 0 && hasgetxz == false)
                            {
                                double maxinterarea = 0.0;
                                foreach (var mzdwitem in interxzdws)
                                {
                                    var intergeo = mzdwitem.Shape.Intersection(bufferlines[bf]);
                                    if (intergeo.IsEmpty) continue;
                                    if (intergeo != null)
                                    {
                                        var interarea = Math.Abs(intergeo.Length);
                                        if (maxinterxzdw == null)
                                        {
                                            maxinterxzdw = mzdwitem;
                                            maxinterxzdwgeo = intergeo;
                                        }
                                        else if (maxinterarea < interarea)
                                        {
                                            maxinterarea = interarea;
                                            maxinterxzdw = mzdwitem;
                                            maxinterxzdwgeo = intergeo;
                                        }
                                    }
                                }

                                if (maxinterxzdw != null)
                                {
                                    hasgetxz = true;
                                    nowgetmaxinfos[maxinterxzdw] = bf;
                                }
                            }

                            if (intermzdws.Count() > 0 && hasgetmz == false)
                            {
                                double maxinterarea = 0.0;
                                foreach (var mzdwitem in intermzdws)
                                {
                                    var intergeo = mzdwitem.Shape.Intersection(bufferlines[bf]);
                                    if (intergeo.IsEmpty) continue;
                                    if (intergeo != null)
                                    {
                                        var interarea = Math.Abs(intergeo.Area);
                                        if (maxintermzdw == null)
                                        {
                                            maxintermzdw = mzdwitem;
                                            maxintermzdwgeo = intergeo;
                                        }
                                        else if (maxinterarea < interarea)
                                        {
                                            maxinterarea = interarea;
                                            maxintermzdw = mzdwitem;
                                            maxintermzdwgeo = intergeo;
                                        }
                                    }
                                }

                                if (maxintermzdw != null)
                                {
                                    hasgetmz = true;
                                    nowgetmaxinfos[maxintermzdw] = bf;
                                }
                            }
                        }


                        #endregion

                        hasgetxz = false;
                        hasgetmz = false;
                        hasgetdk = false;

                        #region 根据求出的相交来获取最大相交更新界址线信息

                        if (nowgetmaxinfos.Count > 0)
                        {
                            nowgetmaxinfos.OrderBy(d => d.Value);
                            var newgrops = nowgetmaxinfos.GroupBy(d => d.Value).First();//第一组是靠的最近的数据  
                            if (newgrops.Count() > 0)
                            {
                                double maxWh = 0.0;
                                object maxintergeo = null;
                                foreach (var item in newgrops)
                                {
                                    var newitemcbd = item.Key as ShortZd_cbd;
                                    if (newitemcbd != null)
                                    {
                                        var cbdmaxwh = Math.Max(maxintercbdgeo.EnvelopeInternal.Width, maxintercbdgeo.EnvelopeInternal.Height);
                                        if (maxWh < cbdmaxwh)
                                        {
                                            maxWh = cbdmaxwh;
                                            maxintergeo = newitemcbd;
                                        }
                                    }
                                    var newitemxzdw = item.Key as XzdwEntity;
                                    if (newitemxzdw != null)
                                    {
                                        var cbdmaxwh = Math.Max(maxinterxzdwgeo.EnvelopeInternal.Width, maxinterxzdwgeo.EnvelopeInternal.Height);
                                        if (maxWh < cbdmaxwh)
                                        {
                                            maxWh = cbdmaxwh;
                                            maxintergeo = newitemxzdw;
                                        }
                                    }
                                    var newitemmzdw = item.Key as MzdwEntity;
                                    if (newitemmzdw != null)
                                    {
                                        var cbdmaxwh = Math.Max(maxintermzdwgeo.EnvelopeInternal.Width, maxintermzdwgeo.EnvelopeInternal.Height);
                                        if (maxWh < cbdmaxwh)
                                        {
                                            maxWh = cbdmaxwh;
                                            maxintergeo = newitemmzdw;
                                        }
                                    }
                                }

                                if (maxintergeo != null)
                                {
                                    var newitemcbd = maxintergeo as ShortZd_cbd;
                                    if (newitemcbd != null)
                                    {
                                        saveJzxinfo(jzxEn, cbd, newitemcbd, JzxwzType.Right);
                                    }
                                    var newitemxzdw = maxintergeo as XzdwEntity;
                                    if (newitemxzdw != null)
                                    {
                                        saveJzxinfoByxzdw(jzxEn, initParam, newitemxzdw);
                                    }
                                    var newitemmzdw = maxintergeo as MzdwEntity;
                                    if (newitemmzdw != null)
                                    {
                                        saveJzxinfoBymzdw(jzxEn, initParam, newitemmzdw);
                                    }
                                }
                            }
                        }

                        #endregion

                        nowgetmaxinfos.Clear();

                        maxinterxzdw = null;
                        maxinterxzdwgeo = null;

                        maxintermzdw = null;
                        maxintermzdwgeo = null;

                        maxintercbd = null;
                        maxintercbdgeo = null;
                    }

                    #region 界址线位置设置  

                    if (jzxEn.JZXLB.IsNullOrEmpty())
                    {
                        jzxEn.JZXLB = initParam.AddressLineCatalog;
                    }

                    if (initParam.IsSetAddressLinePosition)//按照设置重新设置线位置放最后
                    {
                        if (jzxEn.JZXWZ == JzxwzType.Right)
                        {
                            jzxEn.JZXWZ = JzxwzType.Left;
                        }
                    }

                    #endregion

                    jzxEn.Shape = shape;
                    saveJzxsm(jzxEn, points[0], jzdEnd);

                    jzxEn.JZXSXH = i + 1;
                    jzxEn.DKBM = cbd.DKBM;
                    jzxEn.DKID = cbd.dkID;
                    jzxEn.DYDM = cbd.zlDM;
                    jzxEn.JXXZ = initParam.AddressLineType;

                    save(_jzxEn);
                    ++i;

                });
                if (jzxRowids != null)
                {
                    for (; i < jzxRowids.Count; ++i)
                    {
                        _delRowids.Add(jzxRowids[i]);
                    }
                }
            }

            /// <summary>
            /// 获取当前界址线的缓冲集合，从0.1-0.5  标准范围内查找界址线位置
            /// </summary>
            /// <returns></returns>
            private List<IGeometry> gettargetlinebuffer(IGeometry targetShape, double searchDistence)
            {
                List<IGeometry> bufferlines = new List<IGeometry>();
                var bp = new NetTopologySuite.Operation.Buffer.BufferParameters();
                bp.EndCapStyle = GeoAPI.Operation.Buffer.EndCapStyle.Flat;
                bp.JoinStyle = GeoAPI.Operation.Buffer.JoinStyle.Bevel;

                int buffercount = (int)(searchDistence / 0.5);
                var ys = searchDistence % 0.5;
                if (ys != 0.0)
                {
                    buffercount++;
                }
                double bufferset = 0.5;//默认缓冲距离，标准距离

                for (int i = 1; i < buffercount + 1; i++)
                {
                    if (bufferset >= searchDistence)
                    {
                        bufferset = searchDistence;
                    }

                    var offLine = targetShape.Buffer(bufferset, bp);
                    bufferlines.Add(offLine);
                    bufferset = (i + 1) * bufferset;
                }

                return bufferlines;
            }

            /// <summary>
            /// 根据地块赋值界址线的信息，位置  界址线毗邻人，指界人
            /// </summary>
            /// <param name="jzxEn"></param>
            /// <param name="qjzd"></param>
            private void saveJzxinfo(JzxEntity jzxEn, ShortZd_cbd targetcbd, ShortZd_cbd neighborcbd, string jzxwz)
            {
                jzxEn.PLDWQLR = neighborcbd.qlrMc;// calcPldwQlr(cbd, points);
                jzxEn.PLDWZJR = neighborcbd.zjrMc != null ? neighborcbd.zjrMc : neighborcbd.qlrMc;
                jzxEn.JZXWZ = jzxwz;
            }

            /// <summary>
            /// 根据线状地物赋值界址线的信息，位置  界址线毗邻人，指界人
            /// </summary>
            /// <param name="jzxEn"></param>
            /// <param name="targetcbd"></param>
            /// <param name="neighborcbd"></param>
            private void saveJzxinfoByxzdw(JzxEntity jzxEn, InitLandDotCoilParam initParam, XzdwEntity shortdistancexz)
            {
                jzxEn.JZXWZ = JzxwzType.Left;
                if (shortdistancexz.DWMC.IsNullOrEmpty() == false)
                {
                    jzxEn.PLDWQLR = shortdistancexz.DWMC;
                    jzxEn.PLDWZJR = shortdistancexz.DWMC;
                    jzxEn.JZXLB = initParam.jzxdicxmlstrs.Find(jf => jf.JzxlbdicNameContains.Contains(shortdistancexz.DWMC))?.JzxlbdicNameCode;
                }
                else if (shortdistancexz.BZ.IsNullOrEmpty() == false)
                {
                    jzxEn.PLDWQLR = shortdistancexz.BZ;
                    jzxEn.PLDWZJR = shortdistancexz.BZ;
                    jzxEn.JZXLB = initParam.jzxdicxmlstrs.Find(jf => jf.JzxlbdicNameContains.Contains(shortdistancexz.BZ))?.JzxlbdicNameCode;
                }
            }

            /// <summary>
            /// 根据面状地物赋值界址线的信息，位置  界址线毗邻人，指界人
            /// </summary>
            /// <param name="jzxEn"></param>
            /// <param name="targetcbd"></param>
            /// <param name="neighborcbd"></param>
            private void saveJzxinfoBymzdw(JzxEntity jzxEn, InitLandDotCoilParam initParam, MzdwEntity shortdistancemz)
            {
                jzxEn.JZXWZ = JzxwzType.Left;
                jzxEn.JZXLB = initParam.AddressLineCatalog;
                if (shortdistancemz.DWMC.IsNullOrEmpty() == false)
                {
                    jzxEn.PLDWQLR = shortdistancemz.DWMC;
                    jzxEn.PLDWZJR = shortdistancemz.DWMC;
                    jzxEn.JZXLB = initParam.jzxdicxmlstrs.Find(jf => jf.JzxlbdicNameContains.Contains(shortdistancemz.DWMC))?.JzxlbdicNameCode;
                }
                else if (shortdistancemz.BZ.IsNullOrEmpty() == false)
                {
                    jzxEn.PLDWQLR = shortdistancemz.BZ;
                    jzxEn.PLDWZJR = shortdistancemz.BZ;
                    jzxEn.JZXLB = initParam.jzxdicxmlstrs.Find(jf => jf.JzxlbdicNameContains.Contains(shortdistancemz.BZ))?.JzxlbdicNameCode;
                }
            }

            /// <summary>
            /// 最后保存，赋值处理界址线说明
            /// </summary>
            private void saveJzxsm(JzxEntity jzxEn, JzdEntity qjzd, JzdEntity zjzd)
            {
                if (_p._param.LineDescription == EnumDescription.LineLength)
                {
                    jzxEn.JZXSM = jzxEn.JZXCD.ToString();
                }
                else if (_p._param.LineDescription == EnumDescription.LineFind)
                {
                    Aspect a = new Aspect(0);
                    a.Assign(qjzd.shape.X, qjzd.shape.Y, zjzd.shape.X, zjzd.shape.Y);
                    var qjzdh = jzxEn.JZXQDH;
                    var zjzdh = jzxEn.JZXZDH;
                    if (_p._param.IsUnit)
                    {
                        qjzdh = _p._param.AddressPointPrefix + qjzd.TBJZDH.ToString();
                        zjzdh = _p._param.AddressPointPrefix + zjzd.TBJZDH.ToString();
                    }
                    var jszsm = qjzdh + "沿" + a.toAzimuthString() + "方" + (double)Math.Round((decimal)jzxEn.JZXCD, 2) + "米到" + zjzdh;
                    jzxEn.JZXSM = jszsm;
                }
                else if (_p._param.LineDescription == EnumDescription.LineFindType)
                {
                    Aspect a = new Aspect(0);
                    a.Assign(qjzd.shape.X, qjzd.shape.Y, zjzd.shape.X, zjzd.shape.Y);
                    var qjzdh = jzxEn.JZXQDH;
                    var zjzdh = jzxEn.JZXZDH;
                    if (_p._param.IsUnit)
                    {
                        qjzdh = _p._param.AddressPointPrefix + qjzd.TBJZDH.ToString();
                        zjzdh = _p._param.AddressPointPrefix + zjzd.TBJZDH.ToString();
                    }

                    string nzw = string.Empty;
                    if (jzxEn.JZXWZ == JzxwzType.Left)
                    {
                        nzw = "内侧";
                    }
                    else if (jzxEn.JZXWZ == JzxwzType.Right)
                    {
                        nzw = "外侧";
                    }
                    else if (jzxEn.JZXWZ == JzxwzType.Middle)
                    {
                        nzw = "中间";
                    }

                    string jzxlbstr = string.Empty;
                    var jzxlbdic = _p._param.Jzxlbdics.Find(jd => jd.Code == jzxEn.JZXLB);
                    jzxlbstr = jzxlbdic?.Name;

                    var jszsm = qjzdh + "沿" + jzxlbstr + nzw + a.toAzimuthString() + "方" + (double)Math.Round((decimal)jzxEn.JZXCD, 2) + "米到" + zjzdh;
                    jzxEn.JZXSM = jszsm;
                }
            }

            public void testLogout()
            {
                base.testLogout("界址线");
                Console.WriteLine("非正常释放的地块数：" + _nTestReleasedDk);
            }
            private void save(JzxEntity en)//,List<JzdEntity> points)
            {
                //var initParam = _p._param;
                var db = _p._db;
                var srid = _p._srid;
                var cjsj = _p._param.cjsj;// initParam.cjsj;// DateTime.Now;//.ToString();

                var fUpdate = en.rowid > 0;
                var prms = fUpdate ? _updateParams : _insertParams;
                //var geomText = en.shape == null ? "null" : "GeomFromText('POINT(" + en.shape.X + " " + en.shape.Y + ")'," + srid + ")";
                var geomText = "GeomFromText('" + en.Shape.AsText() + "'," + srid + ")";

                int i = 0;
                prms[i].ParamValue = en.ID;
                prms[++i].ParamValue = en.JZXCD;
                prms[++i].ParamValue = en.JXXZ;// initParam.AddressLineType;// initParam.AddressPointPrefix + (++_nStartTBJZDH);// en.TBJZDH;
                prms[++i].ParamValue = en.JZXLB;// initParam.AddressLineCatalog;// initParam.AddressPointPrefix + en.JZDH;
                prms[++i].ParamValue = en.JZXWZ;// initParam.AddressLineCatalog;// cbd.dkID;// en.dkID;
                prms[++i].ParamValue = en.JZXSXH;// cbd.zlDM;// en.DYBM;
                prms[++i].ParamValue = en.JZXQD;// en.JBLX.ToString();
                prms[++i].ParamValue = en.JZXZD;// en.JZDLX.ToString();
                prms[++i].ParamValue = en.DKID;// cbd.DKBM;// en.JZDLX.ToString();
                prms[++i].ParamValue = cjsj;
                prms[++i].ParamValue = cjsj;
                prms[++i].ParamValue = en.JZXSM;
                prms[++i].ParamValue = en.PLDWZJR;
                prms[++i].ParamValue = en.PLDWQLR;
                prms[++i].ParamValue = en.DYDM;
                prms[++i].ParamValue = en.TDQSLX;
                prms[++i].ParamValue = en.DKBM;
                prms[++i].ParamValue = en.JZXQDH;
                prms[++i].ParamValue = en.JZXZDH;
                if (fUpdate)
                {
                    prms[++i].ParamValue = en.rowid;
                    var updateSql = InitLandDotCoilUtil.constructUpdateSql(_tableName, _fields, "rowid=@rowid"
                        , geomText);
                    db.ExecuteNonQuery(updateSql, prms);
                    ++_nTestUpdateCount;
                }
                else
                {
                    var insertSql = InitLandDotCoilUtil.constructInsertSql(_tableName, _fields, geomText);
                    db.ExecuteNonQuery(insertSql, prms);
                    ++_nTestInsertCount;
                }
            }
            /// <summary>
            /// 根据最后关键点构建最终界址线，包含折线
            /// </summary>
            /// <param name="cbd"></param>
            /// <param name="callback"></param>
            private void queryPoints(ShortZd_cbd cbd, Action<List<JzdEntity>> callback)
            {
                _points.Clear();
                JzdEntity cBegin = null;
                for (int i = 0; i < cbd.lstJzdEntity.Count;)
                {
                    //bool fEnd = false;
                    var jzdEn = cbd.lstJzdEntity[i];
                    if (cBegin == null)
                    {
                        cBegin = jzdEn;
                    }
                    _points.Add(jzdEn);
                    if (_points.Count == 1)
                    {
                        ++i;
                        continue;
                    }
                    if (jzdEn.SFKY && jzdEn.fRingLastPoint)
                    {
                        callback(_points);
                        _points.Clear();
                        _points.Add(jzdEn);
                        _points.Add(cBegin);
                        cBegin = null;
                        callback(_points);
                        _points.Clear();
                        ++i;
                        continue;
                    }
                    if (jzdEn.SFKY)
                    {
                        callback(_points);
                        _points.Clear();
                        continue;
                    }
                    if (jzdEn.fRingLastPoint)
                    {
                        _points.Add(cBegin);
                        cBegin = null;
                        callback(_points);
                        _points.Clear();
                    }
                    ++i;
                }
            }

            /// <summary>
            /// 获取毗邻权利人，缓冲相交最长那个地块
            /// </summary>          
            private double calcLength(ShortZd_cbd cbd, List<JzdEntity> lst, out ILineString shape, out ShortZd_cbd qlr, out bool fTwin)
            {
                _qlrSet.Clear();
                qlr = null;
                fTwin = false;

                var coords = new Coordinate[lst.Count];
                double len = 0;
                var pre = lst[0];
                coords[0] = pre.shape;
                for (int i = 1; i < lst.Count; ++i)
                {
                    #region 获取权利人
                    var jzx = findOutEdge(cbd, pre.shape);
                    if (jzx != null && jzx.lstQlr != null)
                    {
                        if (jzx.fQlrFind)
                        {
                            fTwin = true;// jzx.lstQlr[0];
                        }
                        foreach (var q in jzx.lstQlr)
                        {
                            _qlrSet.Add(q);
                        }
                    }
                    #endregion

                    len += Math.Sqrt(CglHelper.GetDistance2(pre.shape, lst[i].shape));
                    coords[i] = lst[i].shape;
                    pre = lst[i];
                }
                shape = GeometryHelper.MakeLine(coords);

                //if (_qlrSet.Count == 1)
                //{
                //    qlr = _qlrSet.First();//.qlrMc;
                //}
                //else 
                if (_qlrSet.Count >= 1)
                {
                    //var s = onlyOneQlr(_qlrSet);
                    //if (s != null)
                    //{
                    //    qlr = s;
                    //}
                    //else
                    //{
                    double maxLen = 0;
                    ShortZd_cbd maxLenDk = null;
                    var bp = new NetTopologySuite.Operation.Buffer.BufferParameters();
                    bp.EndCapStyle = GeoAPI.Operation.Buffer.EndCapStyle.Flat;
                    bp.JoinStyle = GeoAPI.Operation.Buffer.JoinStyle.Bevel;
                    var offLine = shape.Buffer(_p._param.AddressLinedbiDistance, bp);//,new GeoAPI.Operation.Buffer.IBufferParameters().JoinStyle. offsetLeft(lst);
                    foreach (var q in _qlrSet)
                    {
                        var g = getCachePolygon(q);//.MakePolygon();
                        if (g != null)
                        {
                            var oi = YuLinTu.tGISCNet.Topology.Intersect(g.AsBinary(), offLine.AsBinary());
                            if (oi != null)
                            {
                                var gi = WKBHelper.fromWKB(oi);// offLine.Intersection(g);
                                if (gi != null)
                                {
                                    var len1 = Math.Abs(gi.Area);
                                    if (maxLenDk == null || maxLen < len1)
                                    {
                                        maxLenDk = q;
                                        maxLen = len1;
                                    }
                                }
                            }
                        }
                    }

                    if (maxLenDk != null)
                    {
                        qlr = maxLenDk;//.qlrMc;
                    }
                    //}
                }

                return len;
            }

            private Jzx findOutEdge(ShortZd_cbd cbd, Coordinate p)
            {
                JzdEdges edges;
                if (_p._jzdCache.TryGetValue(p, out edges))
                {
                    foreach (var je in edges)
                    {
                        if (je.dk == cbd)
                        {
                            return je.OutEdge;
                        }
                    }
                }
                return null;
            }
            private ShortZd_cbd onlyOneQlr(HashSet<ShortZd_cbd> lst)
            {
                var q0 = _qlrSet.First();
                foreach (var q in _qlrSet)
                {
                    if (q0 != q && q0.qlrMc != q.qlrMc)
                    {
                        return null;
                    }
                }
                return q0;//.qlrMc;
            }

            private IGeometry getCachePolygon(ShortZd_cbd cbd)
            {
                IGeometry g;
                if (_polygonCache.TryGetValue(cbd, out g))
                {
                    return g;
                }
                g = cbd.MakePolygon();
                if (g == null)
                {
                    g = _p._db.GetShape(Zd_cbdFields.TABLE_NAME, Zd_cbdFields.Shape, cbd.rowid);
                    ++_nTestReleasedDk;
                }
                if (_polygonCache.Count > 5000)
                {
                    _polygonCache.Clear();
                }
                if (g != null)
                {
                    _polygonCache[cbd] = g;
                }
                return g;
            }
        }

        /// <summary>
        /// 保存界址点和界址线的缓存
        /// </summary>
        class SaveCacheHelper
        {
            private readonly InitLandDotCoil p;
            private readonly List<ShortZd_cbd> _cacheList = new List<ShortZd_cbd>();
            private readonly List<ShortZd_cbd> _saveList = new List<ShortZd_cbd>();
            private readonly List<ShortZd_cbd> _releaseList = new List<ShortZd_cbd>();

            #region 确保至少4个界址点相关部分
            class JzdOutEdge
            {
                public JzdEntity jzd;
                /// <summary>
                /// 该界址点出度长度的平方
                /// </summary>
                public double len2;
                public JzdOutEdge(JzdEntity je, double len2_)
                {
                    jzd = je;
                    len2 = len2_;
                }
            }
            class MyList
            {
                //[JzdEntity,出度长的平方]
                public readonly List<JzdOutEdge> points = new List<JzdOutEdge>();
                public int nSplitIndex = 0;
                public double minLen2 = 0;
                /// <summary>
                /// 总长度
                /// </summary>
                public double sumLen2 = 0;
                public void Clear()
                {
                    nSplitIndex = 0;
                    minLen2 = 0;
                    sumLen2 = 0;
                }
            }

            /// <summary>
            /// 确保至少_param.MinKeyJzdCount个界址点
            /// </summary>
            /// <param name="lstJzd"></param>
            /// <param name="nKeyJzdCount">当前有效界址点数量</param>
            /// <param name="MinKeyJzdCount">设置的最小保证界址点数据量</param>
            public void Ensure4KeyJzd(List<JzdEntity> lstJzd, int nKeyJzdCount, int MinKeyJzdCount)
            {
                if (lstJzd.Count <= MinKeyJzdCount)
                {
                    foreach (var jzdEn in lstJzd)
                    {
                        jzdEn.SFKY = true;
                    }
                    return;
                }

                var ll = new List<MyList>();//所有的有效界址点间的界址点集合
                var lst = new MyList();

                for (int i = 0; i < lstJzd.Count; ++i)
                {
                    var jzd = lstJzd[i];
                    var nextJzd = lstJzd[i == lstJzd.Count - 1 ? 0 : (i + 1)];
                    var len2 = Math.Sqrt(CglHelper.GetDistance2(jzd.shape, nextJzd.shape));
                    var tpl = new JzdOutEdge(jzd, len2);
                    if (jzd.SFKY)
                    {
                        if (lst.points.Count > 0)
                        {
                            if (lst.points.Count > 1)
                            {
                                ll.Add(lst);
                                lst = new MyList();
                            }
                            else
                            {
                                lst.points.Clear();
                                lst.sumLen2 = 0;
                                lst.nSplitIndex = 0;
                                lst.minLen2 = 0;
                            }
                        }
                    }
                    lst.points.Add(tpl);
                    lst.sumLen2 += len2;
                }
                if (lst.points.Count > 0)
                {
                    ll.Add(lst);
                }
                if (nKeyJzdCount >= MinKeyJzdCount)
                {
                    return;
                }
                while (true)
                {
                    MyList splitItem = null;//要被分割的界址点集合项目
                    foreach (var ml in ll)
                    {
                        if (ml.nSplitIndex == 0)
                        {

                            double leftLen2 = 0;
                            for (int i = 0; i < ml.points.Count; ++i)
                            {
                                var p = ml.points[i];
                                leftLen2 += p.len2;
                                var rLen2 = ml.sumLen2 - leftLen2;
                                if (leftLen2 > rLen2)
                                {
                                    var pl2 = leftLen2 - p.len2;
                                    if (pl2 > rLen2)
                                    {
                                        ml.nSplitIndex = i;// i - 1;
                                        ml.minLen2 = pl2;
                                    }
                                    else
                                    {
                                        ml.nSplitIndex = Math.Min(i + 1, ml.points.Count - 1);
                                        ml.minLen2 = rLen2;
                                    }
                                    break;
                                }
                            }
                        }
                        if (splitItem == null || splitItem.minLen2 < ml.minLen2)
                        {
                            splitItem = ml;
                        }
                    }
                    if (splitItem == null)
                    {
                        break;
                    }

                    splitItem.points[splitItem.nSplitIndex].jzd.SFKY = true;
                    SetKeyJzd(splitItem.points[splitItem.nSplitIndex].jzd.shape);

                    ++nKeyJzdCount;
                    if (nKeyJzdCount >= MinKeyJzdCount)
                    {
                        break;
                    }
                    if (splitItem.points.Count - splitItem.nSplitIndex > 2)
                    {
                        var rml = new MyList();
                        for (int i = splitItem.nSplitIndex; i < splitItem.points.Count; ++i)
                        {
                            var spi = splitItem.points[i];
                            rml.points.Add(spi);
                            rml.sumLen2 += spi.len2;
                        }
                        ll.Add(rml);
                    }
                    for (int i = splitItem.points.Count - 1; i >= splitItem.nSplitIndex; --i)
                    {
                        var spi = splitItem.points[i];
                        splitItem.sumLen2 -= spi.len2;
                        splitItem.points.RemoveAt(i);
                    }
                    splitItem.nSplitIndex = 0;
                    splitItem.minLen2 = 0;
                    if (splitItem.points.Count < 3)
                    {
                        ll.Remove(splitItem);
                    }
                }
            }

            /// <summary>
            /// 按角度查找
            /// </summary>
            /// <param name="myList"></param>
            /// <param name="keyJzdCount"></param>
            private bool SplitByAngle(MyList myList, ref int keyJzdCount)
            {
                if (myList.points.Count <= minPointCount)
                    return false;
                var lstJzd = new List<JzdEntity>();
                myList.points.ForEach(t => lstJzd.Add(t.jzd));
                if (lstJzd[0].shape.X > lstJzd[lstJzd.Count - 1].shape.X ||
                    (lstJzd[0].shape.X == lstJzd[lstJzd.Count - 1].shape.X) &&
                    (lstJzd[0].shape.Y < lstJzd[lstJzd.Count - 1].shape.Y))
                    lstJzd.Reverse();
                var arrayJzd = lstJzd.ToArray();

                if (arrayJzd.Length < minPointCount) return false;

                // 判断关键界址点的下下一个界址点是否为待选关键界址点
                // 关键界址点的下一个界址点必不是待选关键界址点，关键界址点的上一个界址点必不是待选关键界址点
                int i = 2;
                for (; i < arrayJzd.Length - 2; i++)
                {
                    // 保证两线段同方向的夹角
                    var angle = (180 - CalcAngle(arrayJzd[0].shape.X, arrayJzd[0].shape.Y, arrayJzd[1].shape.X, arrayJzd[1].shape.Y, arrayJzd[i].shape.X, arrayJzd[i].shape.Y, arrayJzd[i + 1].shape.X, arrayJzd[i + 1].shape.Y));
                    if (isKeyAngle(angle))
                    {
                        arrayJzd[i].SFKY = true;
                        SetKeyJzd(arrayJzd[i].shape);
                        keyJzdCount++;
                        return true;
                    }
                }
                return false;
            }

            #endregion

            public SaveCacheHelper(InitLandDotCoil p_)
            {
                p = p_;
            }
            public void Add(ShortZd_cbd en)
            {
                System.Diagnostics.Debug.Assert(en.lstJzdEntity == null);
                System.Diagnostics.Debug.Assert(en.shell.Count > 0);
                var _jzdTable = p._jzdTable;

                #region 生成en.lstJzdEntity并将en加入到_cacheList
                en.lstJzdEntity = new List<JzdEntity>();
                var lstJzd = en.lstJzdEntity;
                toJzdList(en.shell, true, p._jzdCache, lstJzd);
                Assign(en, lstJzd, 0);
                if (en.holes != null)
                {
                    foreach (var h in en.holes)
                    {
                        int iBegin = lstJzd.Count;
                        toJzdList(h, false, p._jzdCache, lstJzd);
                        Assign(en, lstJzd, iBegin);
                    }
                }
                _cacheList.Add(en);
                #endregion

                if (_cacheList.Count >= 1000)
                {
                    for (int i = _cacheList.Count - 1; i >= 0; --i)
                    {
                        var c = _cacheList[i];
                        if (canSave(c))
                        {
                            //c.fTobeRemove = true;
                            _cacheList.RemoveAt(i);
                            _saveList.Add(c);
                        }
                    }
                }

                if (_saveList.Count >= 1000)
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
                    }
                    _cacheList.Clear();
                    Console.WriteLine("正在保存最后一个缓存：" + _saveList.Count + "个");
                }
                else
                {
                    Console.WriteLine("正在保存缓存：" + _saveList.Count + "个");
                }
                var trans = p._db.BeginTransaction();
                foreach (var c in _saveList)
                {
                    p._jzdTable.Save(c);//p._db, p._param, c.lstJzd, p._srid);//保存界址信息的时候，更新界址点  线的信息再保存
                    c.fSaved = true;
                    _releaseList.Add(c);
                }
                trans.Commit();

                _saveList.Clear();

                if (_releaseList.Count >= 1000)
                {
                    tryRelease();
                }


                if (p._nTestMaxCacheJzdCount < p._jzdCache.Count)
                {
                    p._nTestMaxCacheJzdCount = p._jzdCache.Count;
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
                        foreach (var jzdEn in c.lstJzdEntity)
                        {
                            p._jzdCache.Remove(jzdEn.shape);
                        }
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
            private bool canSave(ShortZd_cbd cbd)
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

            private static double calcLen(List<JzdEntity> lstJzd)
            {
                double d = 0;

                for (int i = 1; i < lstJzd.Count; ++i)
                {
                    d += calcLen(lstJzd[i - 1], lstJzd[i]);
                }
                return d;
            }
            private static double calcLen(JzdEntity j0, JzdEntity j1)
            {
                if (j0 == null || j1 == null)
                    return 0;
                var p0 = j0.shape;
                var p1 = j1.shape;
                var dx = p0.X - p1.X;
                var dy = p0.Y - p1.Y;
                var len = Math.Sqrt(dx * dx + dy * dy);
                return len;
            }

            /// <summary>
            /// 从iBegin开始为集合中的实体赋值-计算角度、常规角度过滤界址点
            /// </summary>
            /// <param name="lst"></param>
            /// <param name="iBegin"></param>
            /// <param name="iEnd"></param>           
            private void Assign(ShortZd_cbd en, List<JzdEntity> lstJzd, int iBegin)
            {
                lstJzd[lstJzd.Count - 1].fRingLastPoint = true;
                var _param = p._param;
                int nKeyJzdCount = 0;//关键界址点的个数
                JzdEntity preJzd = null;
                var remainLen = calcLen(lstJzd);
                double len1 = 0;
                for (int j = iBegin; j < lstJzd.Count; ++j)
                {
                    var jzdEn = lstJzd[j];
                    var len = calcLen(jzdEn, preJzd);
                    len1 += len;
                    remainLen -= len;//最后点和起点的之间剩余长度
                    jzdEn.JZDH = (short)(j + 1);// ++nJzdh;
                    jzdEn.JBLX = _param.AddressDotMarkType;
                    jzdEn.JZDLX = _param.AddressDotType;
                    jzdEn.JZDSSQLLX = _param.AddressLineRightType;
                    if (jzdEn.SFKY != true)
                    {
                        if (len1 >= 0.05 && (remainLen <= 0.00001 || remainLen >= 0.05))
                        {//确保两个关键界址点之间形成的界址线的长度不小于0.05米
                            #region 检查是否关键界址点
                            JzdEdges lst;
                            if (p._jzdCache.TryGetValue(jzdEn.shape, out lst))
                            {
                                if (lst.fKeyJzd == null)
                                {//判断是否关键界址点
                                    if (lst.Count >= 3)
                                    {
                                        lst.fKeyJzd = true;
                                    }
                                    else
                                    {
                                        if (_param.MinAngleFileter != null)
                                        {
                                            if (lst.Count == 1)
                                            {
                                                var angle = CglHelper.CalcAngle(jzdEn.shape, lst[0].InEdge.qJzd, lst[0].OutEdge.zJzd);
                                                if (isKeyAngle(angle))
                                                {
                                                    lst.fKeyJzd = true;
                                                }
                                            }
                                            else if (lst.Count == 2)
                                            {
                                                var p0 = lst[0].InEdge.qJzd;
                                                var p1 = lst[0].OutEdge.zJzd;
                                                var q0 = lst[1].InEdge.qJzd;
                                                var q1 = lst[1].OutEdge.zJzd;
                                                if (!p._jzdEqualComparer.Equals(p0, q1)
                                                    || !p._jzdEqualComparer.Equals(p1, q0))
                                                {
                                                    lst.fKeyJzd = true;
                                                }
                                                else
                                                {
                                                    var angle = CglHelper.CalcAngle(jzdEn.shape, p0, p1);
                                                    if (isKeyAngle(angle))
                                                    {
                                                        lst.fKeyJzd = true;
                                                    }
                                                    else
                                                    {
                                                        angle = CglHelper.CalcAngle(jzdEn.shape, q0, q1);
                                                        if (isKeyAngle(angle))
                                                        {
                                                            lst.fKeyJzd = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (lst.fKeyJzd == null)
                                {
                                    lst.fKeyJzd = false;
                                }
                                jzdEn.SFKY = (bool)lst.fKeyJzd;
                                jzdEn.TBJZDH = lst.jzdh;
                            }
                            //preJzd = jzdEn;
                            #endregion
                        }
                    }
                    if (j == iBegin && jzdEn.SFKY == false)
                    {//总是将西北角开始的第一个点设置为关键界址点                       
                        jzdEn.SFKY = true;
                        SetKeyJzd(jzdEn.shape);
                    }

                    JzdEdges nlst;
                    if (p._jzdCache.TryGetValue(jzdEn.shape, out nlst))
                    {
                        jzdEn.TBJZDH = nlst.jzdh;
                    }

                    nKeyJzdCount += jzdEn.SFKY ? 1 : 0;
                    if (jzdEn.SFKY)
                    {
                        len1 = 0;
                    }
                    preJzd = jzdEn;
                }

                if (_param.UseAddAlgorithm)
                    // 补充关键界址点
                    AddKeyJZD2(lstJzd, ref nKeyJzdCount);

                if (nKeyJzdCount < _param.MinKeyJzdCount)
                {
                    Ensure4KeyJzd(lstJzd, nKeyJzdCount, _param.MinKeyJzdCount);
                }
            }

            #region 补充关键界址点
            /// <summary>
            /// 为了准确判断四至，从非关键界址点处，增加关键界址点
            /// </summary>
            /// <param name="lstJzd"></param>
            private void AddKeyJZD2(List<JzdEntity> lstJzd, ref int KeyJzdCount)
            {
                var arrayJzd = lstJzd.ToArray();
                int firstKeyPointIndex = -1;
                var recalList = new List<List<JzdEntity>>();
                var list = new List<JzdEntity>();
                recalList.Add(list);
                for (int i = 0; i < arrayJzd.Length; i++)
                {
                    if (arrayJzd[i].SFKY)
                    {
                        if (firstKeyPointIndex == -1)
                            firstKeyPointIndex = i;
                        list.Add(arrayJzd[i]);
                        if (list.Count > 1)
                        {
                            list = new List<JzdEntity>() { arrayJzd[i] };
                            recalList.Add(list);
                        }
                    }
                    else
                    {
                        list.Add(arrayJzd[i]);
                    }
                }
                if (list.Count > 0)
                {
                    if (firstKeyPointIndex == 0 && !list.Last().SFKY)
                    {
                        list.Add(arrayJzd[0]);
                    }
                    else
                    {
                        for (int i = 0; i <= firstKeyPointIndex; i++)
                        {
                            list.Add(arrayJzd[i]);
                        }
                    }
                }
                recalList.RemoveAll(t => t.Count < minPointCount);
                foreach (var item in recalList)
                {
                    RecalcKeyPoint(item, ref KeyJzdCount);
                }
            }

            /// <summary>
            /// 计算2个关键界址点之间的点是否需要补充作为关键界址点
            /// </summary>
            /// <param name="lstJzd"></param>
            private void RecalcKeyPoint(List<JzdEntity> lstJzd, ref int KeyJzdCount)
            {
                if (lstJzd == null) return;
                if (lstJzd[0].shape.X > lstJzd[lstJzd.Count - 1].shape.X ||
                    (lstJzd[0].shape.X == lstJzd[lstJzd.Count - 1].shape.X) &&
                    (lstJzd[0].shape.Y < lstJzd[lstJzd.Count - 1].shape.Y))
                    lstJzd.Reverse();
                var arrayJzd = lstJzd.ToArray();

                if (arrayJzd.Length < minPointCount) return;

                // 判断关键界址点的下下一个界址点是否为待选关键界址点
                // 关键界址点的下一个界址点必不是待选关键界址点，关键界址点的上一个界址点必不是待选关键界址点
                int i = 2;
                for (; i < arrayJzd.Length - 2; i++)
                {
                    // 保证两线段同方向的夹角
                    var angle = (180 - CalcAngle(arrayJzd[0].shape.X, arrayJzd[0].shape.Y, arrayJzd[1].shape.X, arrayJzd[1].shape.Y, arrayJzd[i].shape.X, arrayJzd[i].shape.Y, arrayJzd[i + 1].shape.X, arrayJzd[i + 1].shape.Y));
                    if (isKeyAngle(angle))
                    {
                        arrayJzd[i].SFKY = true;
                        SetKeyJzd(arrayJzd[i].shape);
                        KeyJzdCount++;
                        break;
                    }
                }

                // 递归计算新的关键界址点和原来的第二个关键界址点之间的点，是否需要补充作为关键界址点
                if (arrayJzd.Length - i >= minPointCount)
                {
                    List<JzdEntity> newLstJzd = new List<JzdEntity>();
                    for (int j = i; j < arrayJzd.Length; j++)
                    {
                        newLstJzd.Add(arrayJzd[j]);
                    }
                    RecalcKeyPoint(newLstJzd, ref KeyJzdCount);
                }
            }

            /// <summary>
            /// 计算2向量的夹角
            /// </summary>
            /// <param name="x1">第一个向量的起点X轴坐标</param>
            /// <param name="y1">第一个向量的起点Y轴坐标</param>
            /// <param name="x2">第一个向量的终点点X轴坐标</param>
            /// <param name="y2">第一个向量的终点点Y轴坐标</param>
            /// <param name="x3">第二个向量的起点X轴坐标</param>
            /// <param name="y3">第二个向量的起点Y轴坐标</param>
            /// <param name="x4">第二个向量的终点点X轴坐标</param>
            /// <param name="y4">第二个向量的终点点Y轴坐标</param>
            /// <returns></returns>
            public static double CalcAngle(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
            {
                var v1x = x2 - x1;
                var v1y = y2 - y1;
                var v2x = x4 - x3;
                var v2y = y4 - y3;

                var multi = v1x * v2x + v1y * v2y;
                var v1mod = Math.Sqrt(v1x * v1x + v1y * v1y);
                var v2mode = Math.Sqrt(v2x * v2x + v2y * v2y);

                var angle = Math.Acos((double)Math.Round((decimal)(multi / v1mod / v2mode), 6)) * 180 / Math.PI;
                return angle;
            }

            #endregion

            /// <summary>
            /// 判断角度是否可判断为关键界址点
            /// </summary>
            /// <param name="angle"></param>
            /// <returns></returns>
            private bool isKeyAngle(double angle)
            {
                if (p._param.IsFilter)
                {
                    return angle >= (double)p._param.MinAngleFileter
                                            && angle <= (double)p._param.MaxAngleFilter;
                }
                else
                {
                    return true;
                }
            }
            /// <summary>
            /// 处理循环西北角第一个点
            /// </summary>
            /// <param name="r"></param>
            /// <param name="fShell"></param>
            /// <param name="lst"></param>
            private void toJzdList(JzxRing r, bool fShell, JzdCache jzdcache, List<JzdEntity> lst)
            {
                InitLandDotCoilUtil.SortCoordsByWNOrder(r, fShell, jzdcache, c =>
                  {
                      var en = new JzdEntity();
                      lst.Add(en);
                      en.shape = c;
                      en.ID = InitLandDotCoilUtil.CreateNewID();

                  });
            }

            /// <summary>
            /// 设置为关键界址点
            /// </summary>
            public void SetKeyJzd(Coordinate jzd)
            {
                JzdEdges lst;
                if (!p._jzdCache.TryGetValue(jzd, out lst))
                {
                    return;
                }
                if (lst.fKeyJzd != null && true == (bool)lst.fKeyJzd)
                {
                    return;
                }
                lst.fKeyJzd = true;
                foreach (var jzx in lst)
                {
                    if (jzx.dk.lstJzdEntity != null)
                    {
                        foreach (var jzdEn in jzx.dk.lstJzdEntity)
                        {
                            if (!p._jzdEqualComparer.Equals(jzdEn.shape, jzd))
                                continue;
                            if (!jzdEn.SFKY)
                                jzdEn.SFKY = true;
                            jzdEn.TBJZDH = lst.jzdh;
                        }
                    }
                }
            }
        }

        private readonly DBSpatialite _db;
        private readonly InitLandDotCoilParam _param;

        private int _curProgress, _oldProgress;//进度相关
        private int _progressCount { get { return _rowids.Count; } }
        private int _srid;

        /// <summary>
        /// 打断线的点数
        /// </summary>
        private int _nInsertedJzdCount = 0;
        /// <summary>
        /// 集合_dicJzd中存有界址点的最大数量
        /// </summary>
        private int _nTestMaxCacheJzdCount = 0;
        /// <summary>
        /// 包含一个以上毗邻地权利人的界址线的个数
        /// </summary>
        private int _nGreat1QlrJzxCount = 0;
        /// <summary>
        /// 待处理地块的rowid集合
        /// </summary>
        private readonly HashSet<int> _rowids = new HashSet<int>();

        public readonly JzdEqualComparer _jzdEqualComparer;

        private readonly JzdTable _jzdTable;

        /// <summary>
        /// 承包地缓存
        /// </summary>
        private readonly ShortZd_cbdCache _cbdCache = new ShortZd_cbdCache();
        private readonly JzdCache _jzdCache;

        private readonly SaveCacheHelper _saveCache;
        private readonly XzdyUtil _xzdyUtil = new XzdyUtil();

        public Action<string, int> ReportProgress;
        public Action<string> ReportInfomation;
        /// <summary>
        /// 保存界址点前的回调，可以根据需要修改实体的值
        /// </summary>
        public Action<JzdEntity> onPresaveJzd;
        public InitLandDotCoil(DBSpatialite db, InitLandDotCoilParam param)
        {
            _db = db;
            _param = param;
            _srid = db.QuerySRID(Zd_cbdFields.TABLE_NAME);
            _jzdEqualComparer = new JzdEqualComparer(_param.Tolerance);
            _saveCache = new SaveCacheHelper(this);
            _jzdCache = new JzdCache(this);
            _jzdTable = new JzdTable(this);
            _xzdyUtil.Init(db);
        }
        /// <summary>
        /// 开始初始化界址点和界址线
        /// </summary>
        /// <param name="wh"></param>
        public void DoInit(string wh)
        {
            //Console.WriteLine("开始时间：" + DateTime.Now);
            ReportInfomation("开始时间：" + DateTime.Now);
            _rowids.Clear();
            _curProgress = 0;
            _oldProgress = 0;
            _nInsertedJzdCount = 0;
            _nGreat1QlrJzxCount = 0;
            _nTestMaxCacheJzdCount = 0;
            _jzdCache.Clear();
            //_lstSaveCache.Clear();
            _jzdTable.Clearing();
            _cbdCache.Clear();
            _saveCache.Clear();
            _param.cjsj = DateTime.Now;

            bool fRebuildIndex = string.IsNullOrEmpty(wh);
            if (fRebuildIndex)
            {
                _db.DropSpatialIndex(JzdFields.TABLE_NAME, JzdFields.SHAPE);
                _db.DropSpatialIndex(JzxFields.TABLE_NAME, JzxFields.SHAPE);

                StopwatchUtil.Start("开始清空界址点和界址线的数据...");
                _db.Delete(JzdFields.TABLE_NAME);
                _db.Delete(JzxFields.TABLE_NAME);
                StopwatchUtil.Stop();

                StopwatchUtil.Start("开始压缩数据库");
                _db.Vaccum();
                StopwatchUtil.Stop();
            }

            _jzdTable.init();

            var lstXBounds = new List<XBounds>();
            var env = _db.QueryEnvelope(Zd_cbdFields.TABLE_NAME, Zd_cbdFields.Shape, wh, new List<string> { "rowid" }, r =>
            {
                _rowids.Add(r.GetInt32(1));
            });

            ReportInfomation("已选择地块个数共计：" + _rowids.Count + "个");

            env.ExpandBy(_param.AddressLinedbiDistance);

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

            //将线面状地物获取到内存中
            _db.QueryIntersectsCallback(XzdwFields.TABLE_NAME, XzdwFields.SHAPE, env, r =>
            {
                var x = new XzdwEntity();
                x.rowid = SafeConvertAux.SafeConvertToInt32(r.GetValue(0));
                var e = WKBHelper.fromWKB(r.GetValue(1) as byte[]);
                x.Shape = e;
                x.DWMC = SqlUtil.GetString(r, 2);
                x.BZ = SqlUtil.GetString(r, 3);
                queryXzdws.Add(x);
                return true;
            }, new List<string> { XzdwFields.RowID, XzdwFields.SHAPE, XzdwFields.DWMC, XzdwFields.BZ });
            _db.QueryIntersectsCallback(MzdwFields.TABLE_NAME, MzdwFields.SHAPE, env, r =>
            {
                var x = new MzdwEntity();
                x.rowid = SafeConvertAux.SafeConvertToInt32(r.GetValue(0));
                var e = WKBHelper.fromWKB(r.GetValue(1) as byte[]);
                x.Shape = e;
                x.DWMC = SqlUtil.GetString(r, 2);
                x.BZ = SqlUtil.GetString(r, 3);
                queryMzdws.Add(x);
                return true;
            }, new List<string> { MzdwFields.RowID, MzdwFields.SHAPE, MzdwFields.DWMC, MzdwFields.BZ });
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
            int tbjzdh = 1;
            var cacheRowids = new List<int>();
            Dictionary<int, ShortZd_cbd> dicCbd = new Dictionary<int, ShortZd_cbd>();

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
                InitLandDotCoilUtil.QueryShortZd_cbd(_db, cacheRowids, dicCbd, en =>
                {
                    _jzdCache.AcquireJzd(en.shell, ref tbjzdh);
                    if (en.holes != null)
                    {
                        foreach (var h in en.holes)
                        {
                            _jzdCache.AcquireJzd(h, ref tbjzdh);
                        }
                    }
                    en.fSelected = _rowids.Contains(en.rowid);// isSelected(en);
                });
                for (int k = i; k <= j; ++k)
                {
                    var xb = lstXBounds[k];
                    ShortZd_cbd cbd;
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

            var trans = _db.BeginTransaction();
            _jzdTable.SaveDeletedRowids();
            trans.Commit();
            _jzdTable.testLogout();

            if (fRebuildIndex)
            {
                StopwatchUtil.Start("开始重建空间索引...");
                _db.CreateSpatialIndex(JzdFields.TABLE_NAME, JzdFields.SHAPE);
                _db.CreateSpatialIndex(JzxFields.TABLE_NAME, JzxFields.SHAPE);
                StopwatchUtil.Stop();
            }

            Console.WriteLine("共发现插入点" + _nInsertedJzdCount + "个");
            Console.WriteLine("包含一个以上毗邻地权利人的界址线个数是：" + _nGreat1QlrJzxCount);
            Console.WriteLine("nTestMaxCacheSize=" + nTestMaxCacheSize);
            Console.WriteLine("缓存界址点的最大数量为：" + Math.Max(_jzdCache.Count, _nTestMaxCacheJzdCount));

            ReportInfomation("生成界址点共计：" + _jzdTable.GetCreatedCount() + "个");
            ReportInfomation("生成界址线共计：" + _jzdTable.GetCreatedJzxCount() + "条");
            ReportInfomation("结束时间：" + DateTime.Now);

            queryXzdws.Clear();
            queryMzdws.Clear();
        }
        private bool processLeft(ShortZd_cbd current)
        {
            var left = _cbdCache;
            if (current != null)
            {
                if (left.Count == 0)
                {
                    left.Add(current);
                    left.x1 = current.xmax;
                    return false;
                }
                left.Add(current);
                if (left.x1 > current.xmax)
                {
                    left.x1 = current.xmax;
                }
                if (left.x1 + _param.AddressLinedbiDistance > current.xmin)
                {
                    return false;
                }
            }
            var tolerance = _param.Tolerance;
            var tolerance2 = tolerance * tolerance;

            var lstSortedJzx = new List<Jzx>();
            buildSortedJzxList(left, lstSortedJzx);

            #region 打断界址线
            var lstSortedJzd = new SortedSet<Coordinate>(new JzdComparer(tolerance));
            foreach (var cbd in left)
            {
                cbd.QueryPoint(c =>
                {
                    lstSortedJzd.Add(c);
                });
            }

            foreach (var pt in lstSortedJzd)
            {
                for (int i = lstSortedJzx.Count - 1; i >= 0; --i)
                {
                    var jzx = lstSortedJzx[i];

                    var zJzd = jzx.maxYPoint;//.zJzd;

                    if (pt.Y > zJzd.Y + tolerance)
                    {
                        lstSortedJzx.RemoveAt(i);
                        continue;
                    }
                    var qJzd = jzx.minYPoint;

                    if (pt.Y < qJzd.Y - tolerance)
                    {
                        break;
                    }

                    var minX = qJzd.X;
                    var maxX = zJzd.X;
                    if (minX > maxX)
                    {
                        minX = zJzd.X;
                        maxX = qJzd.X;
                    }
                    if (pt.X >= minX && pt.X <= maxX && pt.Y >= qJzd.Y && pt.Y <= zJzd.Y)
                    {
                        if (CglHelper.IsSame2(pt, qJzd, tolerance2))
                        {
                            continue;
                        }
                        if (CglHelper.IsSame2(pt, zJzd, tolerance2))
                        {
                            continue;
                        }
                        if (CglHelper.IsPointOnLine(qJzd, zJzd, pt, tolerance2))
                        {
                            var p1 = CglHelper.GetProjectionPoint(qJzd, zJzd, pt);
                            if (!Overlaps(jzx.lstInsertJzd, p1))
                            {
                                jzx.lstInsertJzd.Add(p1);
                                ++_nInsertedJzdCount;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < left.Count; ++i)
            {
                var cbd = left[i];

                bool fRebuild = cbd.shell.RebuildRing();
                int nRebuildCount = fRebuild ? 1 : 0;
                if (cbd.holes != null)
                {
                    foreach (var r in cbd.holes)
                    {
                        fRebuild = r.RebuildRing();
                        nRebuildCount += fRebuild ? 1 : 0;
                    }
                }
                if (nRebuildCount > 0)
                {//若该地块的边上有插入点则保存地块的shape数据
                    var g = cbd.MakePolygon(_srid);
                    _db.UpdateShape(Zd_cbdFields.TABLE_NAME, Zd_cbdFields.Shape, "rowid=" + cbd.rowid, g);
                }
            }
            #endregion

            #region 查找毗邻地权利人
            buildSortedJzxList(left, lstSortedJzx);
            int nTestJzxCount = lstSortedJzx.Count;
            for (int i = lstSortedJzx.Count - 1; i >= 0; --i)
            {
                var jzx = lstSortedJzx[i];

                #region 查找相邻的地块（外接矩形在缓冲范围内相交的地块)
                for (int j = lstSortedJzx.Count - 1; j >= 0; --j)
                {
                    if (j == i)
                        continue;
                    var jzxJ = lstSortedJzx[j];
                    if (jzxJ.maxYPoint.Y + _param.AddressLinedbiDistance < jzx.minYPoint.Y)
                    {
                        lstSortedJzx.RemoveAt(j);
                        continue;
                    }
                    if (jzx.maxYPoint.Y + _param.AddressLinedbiDistance < jzxJ.minYPoint.Y)
                    {
                        break;
                    }
                    double minx = jzxJ.qJzd.X, maxx = jzxJ.zJzd.X;
                    if (jzxJ.qJzd.X > jzxJ.zJzd.X)
                    {
                        minx = jzxJ.zJzd.X;
                        maxx = jzxJ.qJzd.X;
                    }
                    double minxi = jzx.qJzd.X, maxxi = jzx.zJzd.X;
                    if (minxi > maxxi)
                    {
                        minxi = maxxi;
                        maxxi = jzx.qJzd.X;
                    }
                    if (!(maxxi + _param.AddressLinedbiDistance < minx
                        || maxx + _param.AddressLinedbiDistance < minxi))
                    {
                        if (jzx.dk.lstNeibors == null)
                        {
                            jzx.dk.lstNeibors = new HashSet<ShortZd_cbd>();
                        }
                        jzx.dk.lstNeibors.Add(jzxJ.dk);
                    }
                }
                #endregion               

                if (jzx.fQlrFind)
                {
                    continue;
                }
                //找到共边的界址线
                var twin = _jzdCache.findTwin(jzx);
                if (twin != null)
                {
                    if (jzx.lstQlr == null)
                    {
                        jzx.lstQlr = new List<ShortZd_cbd>();
                    }
                    jzx.lstQlr.Clear();
                    jzx.lstQlr.Add(twin.dk);

                    jzx.fQlrFind = true;
                    if (twin.lstQlr == null)
                    {
                        twin.lstQlr = new List<ShortZd_cbd>();
                    }
                    twin.lstQlr.Clear();
                    twin.lstQlr.Add(jzx.dk);
                    twin.fQlrFind = true;
                    continue;
                }
                var fSelected = jzx.dk.fSelected;// isSelected(jzx.dk);
                var buffer = InitLandDotCoilUtil.BufferLeft(jzx.qJzd, jzx.zJzd, _param.AddressLinedbiDistance);
                if (buffer == null)
                    continue;
                var env = buffer.EnvelopeInternal;
                for (int j = lstSortedJzx.Count - 1; j >= 0; --j)
                {
                    if (j == i)
                        continue;
                    var jzxJ = lstSortedJzx[j];
                    if (jzxJ.maxYPoint.Y + _param.AddressLinedbiDistance < jzx.minYPoint.Y)
                    {
                        System.Diagnostics.Debug.Assert(false);//在查找相邻的地块处已经处理了，应该不会走到这里
                        lstSortedJzx.RemoveAt(j);
                        continue;
                    }
                    if (jzx.maxYPoint.Y + _param.AddressLinedbiDistance < jzxJ.minYPoint.Y)
                    {
                        break;
                    }

                    if (jzx.dk == jzxJ.dk)
                        continue;
                    if (jzx.lstQlr != null && jzx.lstQlr.Contains(jzxJ.dk))
                    {
                        continue;
                    }
                    double minx = jzxJ.qJzd.X, maxx = jzxJ.zJzd.X;
                    if (jzxJ.qJzd.X > jzxJ.zJzd.X)
                    {
                        minx = jzxJ.zJzd.X;
                        maxx = jzxJ.qJzd.X;
                    }
                    if (maxx < env.MinX || minx > env.MaxX)
                        continue;
                    if (buffer.Intersects(GeometryHelper.MakeLine(jzxJ.qJzd, jzxJ.zJzd)))
                    {
                        if (jzx.lstQlr == null)
                        {
                            jzx.lstQlr = new List<ShortZd_cbd>();
                        }
                        jzx.lstQlr.Add(jzxJ.dk);

                        if (jzxJ.fQlrFind == false)
                        {
                            if (jzxJ.lstQlr == null)
                            {
                                jzxJ.lstQlr = new List<ShortZd_cbd>();
                                jzxJ.lstQlr.Add(jzx.dk);
                            }
                            else if (!jzxJ.lstQlr.Contains(jzx.dk))
                            {
                                jzxJ.lstQlr.Add(jzx.dk);
                            }
                        }
                    }
                }
                if (jzx.lstQlr != null && jzx.lstQlr.Count > 1)
                {
                    ++_nGreat1QlrJzxCount;
                }
            }
            #endregion

            #region 将left ShortZd_cbdCache中所有在当前地块最左边缓冲距离前完全出现的地块加入到保存缓存中并从left ShortZd_cbdCache中移除
            double? x1 = null;
            for (int i = left.Count - 1; i >= 0; --i)
            {
                var cbd = left[i];
                if (current == null || cbd.xmax + _param.AddressLinedbiDistance < current.xmin)
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
                    if (x1 == null || (double)x1 < cbd.xmax)
                    {
                        x1 = cbd.xmax;
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

        /// <summary>
        /// 用cache中的地块数据构建一个按y方向降序排列的界址线集合，
        /// 该方法会首先清空lstSortedJzx中的数据；
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="lstSortedJzx"></param>
        private void buildSortedJzxList(ShortZd_cbdCache cache, List<Jzx> lstSortedJzx)
        {
            lstSortedJzx.Clear();
            foreach (var cbd in cache)
            {
                if (cbd.shell.Count == 0)
                {
                    System.Diagnostics.Debug.Assert(cbd.shell.Count > 0);
                    throw new Exception("内部错误");
                }
                foreach (var jzx in cbd.shell)
                {
                    lstSortedJzx.Add(jzx);
                }
                if (cbd.holes != null)
                {
                    foreach (var h in cbd.holes)
                    {
                        foreach (var jzx in h)
                        {
                            lstSortedJzx.Add(jzx);
                        }
                    }
                }
            }
            lstSortedJzx.Sort((a, b) =>
            {//按Y轴优先排序
                if (a.minYPoint.Y > b.minYPoint.Y)
                    return -1;
                if (a.minYPoint.Y < b.minYPoint.Y)
                    return 1;
                return a.minYPoint.X < b.minYPoint.X ? -1 : 1;
            });
        }
        /// <summary>
        /// 判断集合lst中的点是否存在于c重叠的点；
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool Overlaps(List<Coordinate> lst, Coordinate c)
        {
            var cmp = new JzdComparer(_param.Tolerance);
            foreach (var c0 in lst)
            {
                if (cmp.Compare(c0, c) == 0)
                    return true;
            }
            return false;
        }

        private void reportProgress(string msg = "初始化界址点线")
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
    public class ShortZd_cbd
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

        //public NetTopologySuite.Geometries.IGeometry[] 
        //public bool isMultiPolygon;

        public int rowid;
        public double xmin;
        public double xmax;
        //public double ymin;
        //public double ymax;
        /// <summary>
        /// 权利人名称
        /// </summary>
        public string qlrMc;
        /// <summary>
        /// 指界人名称
        /// </summary>
        public string zjrMc;
        /// <summary>
        /// 地块标识
        /// </summary>
        public string dkID;
        /// <summary>
        /// 做落地代码
        /// </summary>
        public string zlDM;
        /// <summary>
        /// 地块编码
        /// </summary>
        public string DKBM;
        /// <summary>
        /// 地块名称
        /// </summary>
        public string DKMC;

        /// <summary>
        /// 海拔
        /// </summary>
        internal double elevation = 10000;

        public JzxRing shell = new JzxRing();

        /// <summary>
        /// may by null
        /// </summary>
        public List<JzxRing> holes = null;

        /// <summary>
        /// 界址点实体
        /// </summary>
        public List<JzdEntity> lstJzdEntity = null;

        /// <summary>
        /// 与该地块相邻的所有地块（近似判断）；
        /// </summary>
        public HashSet<ShortZd_cbd> lstNeibors = null;
        //public bool fTobeRemove = false;
        public void QueryPoint(Action<Coordinate> callback)
        {
            foreach (var j in shell)
            {
                callback(j.qJzd);
            }
            if (holes != null)
            {
                foreach (var h in holes)
                {
                    foreach (var j in h)
                    {
                        callback(j.qJzd);
                    }
                }
            }
        }

        public IPolygon MakePolygon(int srid = -1)
        {
            if (shell.Count == 0)
                return null;
            ILinearRing[] rs = null;
            if (holes != null && holes.Count > 0)
            {
                rs = new ILinearRing[holes.Count];
                for (int i = 0; i < holes.Count; ++i)
                {
                    rs[i] = holes[i].MakeLinearRing();
                }
            }
            return GeometryHelper.MakePolygon(shell.MakeLinearRing(), rs, srid);
        }

        /// <summary>
        /// 清理，避免循环引用
        /// </summary>
        public void Clear()
        {
            if (fRemovedFromCache)
            {
                System.Diagnostics.Debug.Assert(shell.Count > 0);
                shell.clear();
                if (holes != null)
                {
                    foreach (var h in holes)
                    {
                        h.Clear();
                    }
                    holes.Clear();
                    holes = null;
                }
                if (lstJzdEntity != null)
                {
                    lstJzdEntity.Clear();
                    lstJzdEntity = null;
                }
                if (lstNeibors != null)
                {
                    lstNeibors.Clear();
                    lstNeibors = null;
                }
            }
        }
    }

    /// <summary>
    /// 如果是shell则按顺时针方向排列否则按逆时针方向排列
    /// </summary>
    public class JzxRing : List<Jzx>
    {
        /// <summary>
        /// 重塑环：根据每一条边上的插入点打断界址线；
        /// 如果没有插入点则返回false;
        /// </summary>
        public bool RebuildRing()
        {
            if (!hasIntsertedJzd())
                return false;
            var lst = new List<Jzx>();
            var t1 = new List<Jzx>();
            foreach (var jzx in this as List<Jzx>)
            {
                if (jzx.lstInsertJzd.Count > 0)
                {
                    jzx.GetSplitJzxs(t1);
                    foreach (var j in t1)
                    {
                        lst.Add(j);
                    }
                }
                else
                {
                    lst.Add(jzx);
                }
            }
            Clear();
            foreach (var j in lst)
            {
                Add(j);
            }
            return true;
        }
        public void clear()
        {
            foreach (var jzx in this as List<Jzx>)
            {
                jzx.Clear();
            }
            base.Clear();
        }
        /// <summary>
        /// 构造为Ｎts类型的环
        /// </summary>
        /// <returns></returns>
        public ILinearRing MakeLinearRing()
        {
            var coords = new Coordinate[Count + 1];
            for (int i = 0; i < Count; ++i)
            {
                coords[i] = this[i].qJzd;
            }
            coords[Count] = coords[0];
            return GeometryHelper.MakeLinearRing(coords);
        }
        private bool hasIntsertedJzd()
        {
            foreach (var jzx in this as List<Jzx>)
            {
                if (jzx.lstInsertJzd.Count > 0)
                    return true;
            }
            return false;
        }
    }

    public class Jzx
    {
        public bool fInserted = false;
        /// <summary>
        /// 是否起界址点的y值小于止界址点的y值
        /// </summary>
        private readonly bool _isQjzdYLess;
        /// <summary>
        /// 所属地块
        /// </summary>
        public ShortZd_cbd dk;
        public readonly bool isShell;
        /// <summary>
        /// 起界址点ID（对应Jzd.id）
        /// </summary>
        public readonly Coordinate qJzd;
        /// <summary>
        /// 止界址点ID（对应Jzd.id）
        /// </summary>
        public readonly Coordinate zJzd;
        public Coordinate minYPoint
        {
            get
            {
                return _isQjzdYLess ? qJzd : zJzd;
            }
        }
        public Coordinate maxYPoint
        {
            get
            {
                return _isQjzdYLess ? zJzd : qJzd;
            }
        }
        /// <summary>
        /// 毗邻地权利人
        /// </summary>
        public List<ShortZd_cbd> lstQlr = null;// new List<ShortZd_cbd>();
        public bool fQlrFind = false;//已找到毗邻权利人，通过共边查找的
        public Jzx(Coordinate p0, Coordinate p1, ShortZd_cbd dk_, bool fShell)
        {
            qJzd = p0;
            zJzd = p1;
            dk = dk_;
            isShell = fShell;
            _isQjzdYLess = p0.Y < p1.Y;
            if (p0.Y == p1.Y)
            {
                _isQjzdYLess = p0.X < p1.X;
            }
        }

        /// <summary>
        /// 保存重新打断的界址点ID
        /// </summary>
        public readonly List<Coordinate> lstInsertJzd = new List<Coordinate>();
        /// <summary>
        /// 根据 lstInsertJzd中的数据获取打断后的界址线数据；
        /// <param name="lst">会首先清空里面的数据</param>
        /// </summary>
        public List<Jzx> GetSplitJzxs(List<Jzx> lst, bool fClearLstInsertJzd = true)
        {
            lst.Clear();
            if (lstInsertJzd.Count == 0)
            {
                lst.Add(this);
                return lst;
            }
            if (lstInsertJzd.Count > 1)
            {
                if (qJzd.X < zJzd.X)
                {
                    lstInsertJzd.Sort((a, b) =>
                    {
                        return a.X < b.X ? -1 : 1;
                    });
                }
                else if (qJzd.X > zJzd.X)
                {
                    lstInsertJzd.Sort((a, b) =>
                    {
                        return a.X > b.X ? -1 : 1;
                    });
                }
                else if (qJzd.Y < zJzd.Y)
                {
                    lstInsertJzd.Sort((a, b) =>
                    {
                        return a.Y < b.Y ? -1 : 1;
                    });
                }
                else
                {
                    lstInsertJzd.Sort((a, b) =>
                    {
                        return a.Y > b.Y ? -1 : 1;
                    });
                }
            }

            lstInsertJzd.Add(zJzd);
            var preJzd = qJzd;
            for (int i = 0; i < lstInsertJzd.Count; ++i)
            {
                var p1 = lstInsertJzd[i];
                lst.Add(new Jzx(preJzd, p1, dk, isShell));
                preJzd = p1;
            }
            if (fClearLstInsertJzd)
            {
                lstInsertJzd.Clear();
            }
            else
            {
                lstInsertJzd.RemoveAt(lstInsertJzd.Count - 1);
            }
            return lst;
        }

        /// <summary>
        /// 清理，避免循环引用
        /// </summary>
        public void Clear()
        {
            dk = null;
            if (lstQlr != null)
            {
                lstQlr.Clear();
                lstQlr = null;
            }
            lstInsertJzd.Clear();
        }

    }


    /// <summary>
    /// 缓存界址点对应的出度的集合 
    /// </summary>
    public class JzdCache
    {
        private readonly InitLandDotCoil p;
        private readonly Dictionary<Coordinate, JzdEdges> _dicJzdOutEdges;
        private readonly Dictionary<Coordinate, int> _dicJzdInfos;//对应点在哪个环

        public JzdCache(InitLandDotCoil p_)
        {
            p = p_;
            _dicJzdOutEdges = new Dictionary<Coordinate, JzdEdges>(p._jzdEqualComparer);
            _dicJzdInfos = new Dictionary<Coordinate, int>(p._jzdEqualComparer);
        }

        /// <summary>
        /// 从界址线中获取界址点对应的出入度
        /// </summary>
        /// <param name="r"></param>
        /// <param name="dicJzd"></param>
        public void AcquireJzd(JzxRing r, ref int jzdh)
        {
            var dicJzd = _dicJzdOutEdges;
            var preJzx = r[r.Count - 1];
            for (int i = 0; i < r.Count; ++i)
            {
                var jzx = r[i];
                var jzd = jzx.qJzd;

                JzdEdges lst;
                if (!dicJzd.TryGetValue(jzd, out lst))
                {
                    lst = new JzdEdges() { jzdh = jzdh++ };
                    dicJzd[jzd] = lst;
                }
                lst.Add(new JzdEdge() { OutEdge = jzx, InEdge = preJzx });
                preJzx = jzx;
            }

        }
        public void Remove(Coordinate jzd)
        {
            _dicJzdOutEdges.Remove(jzd);
        }
        public int Count
        {
            get { return _dicJzdOutEdges.Count; }
        }
        public bool TryGetValue(Coordinate jzd, out JzdEdges val)
        {
            return _dicJzdOutEdges.TryGetValue(jzd, out val);
        }

        public Jzx findTwin(Jzx jzx)
        {
            for (int i = 0; i < 2; ++i)
            {
                JzdEdges lst;
                if (_dicJzdOutEdges.TryGetValue(i == 0 ? jzx.qJzd : jzx.zJzd, out lst))
                {
                    foreach (var j in lst)
                    {
                        if (jzx.dk != j.dk)
                        {
                            if (p._jzdEqualComparer.Equals(jzx.minYPoint, j.OutEdge.minYPoint)
                                && p._jzdEqualComparer.Equals(jzx.maxYPoint, j.OutEdge.maxYPoint))
                            {
                                return j.OutEdge;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public void Clear()
        {
            _dicJzdOutEdges.Clear();
        }
    }
}
