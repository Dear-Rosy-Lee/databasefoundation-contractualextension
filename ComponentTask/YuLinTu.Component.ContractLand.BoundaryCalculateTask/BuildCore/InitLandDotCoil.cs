using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using YuLinTu.NetAux;
using YuLinTu.tGISCNet;
namespace YuLinTu.Library.BuildJzdx
{
    /// <summary>
    /// 初始化界址点和界址线业务类
    /// </summary>
    public class InitLandDotCoil
    {
        /// <summary>
        /// 界址点表辅助类
        /// </summary>
        class TableBase
        {
            ///// <summary>
            ///// DKID于界址点rowid的映射
            ///// </summary>
            //protected readonly Dictionary<string, List<int>> _dic = new Dictionary<string, List<int>>();
            protected int _nCurrShapeID = 0;
            protected int _nindex = 1;

            protected readonly InitLandDotCoil _p;
            //protected readonly string[] _fields;
            //protected readonly SQLiteParam[] _updateParams;
            //protected readonly SQLiteParam[] _insertParams;
            //public readonly List<int> _delRowids = new List<int>();


            //protected int _nTestInsertCount = 0;
            //protected int _nTestUpdateCount = 0;

            //protected readonly string _tableName;

            protected TableBase(InitLandDotCoil p_)//, string[] fields)
            {
                _p = p_;
                //_tableName = tableName;
                //_fields = fields;
                //_updateParams = new SQLiteParam[_fields.Length];
                //_insertParams = new SQLiteParam[_fields.Length - 1];

                //for (int i = 0, j = 0; i < _fields.Length; ++i)
                //{
                //    if (_fields[i] == JzdFields.SHAPE)
                //        continue;
                //    _updateParams[j] = new SQLiteParam()
                //    {
                //        ParamName = _fields[i]
                //    };
                //    _insertParams[j++] = new SQLiteParam()
                //    {
                //        ParamName = _fields[i]
                //    };
                //}
                //_updateParams[_updateParams.Length - 1] = new SQLiteParam() { ParamName = "rowid" };
            }

            public int ExportedCount
            {
                get
                {
                    return _nCurrShapeID;
                }
            }
            //public void init()//string dkidFieldName)
            //{
            //    //_dic.Clear();
            //    //var sql = string.Format("select {0},rowid from {1} where {2} is not null", dkidFieldName, _tableName, dkidFieldName);
            //    //_p._db.QueryCallback(sql, r =>
            //    //{
            //    //    var id = r.GetString(0);
            //    //    List<int> lst;
            //    //    if (!_dic.TryGetValue(id, out lst))
            //    //    {
            //    //        lst = new List<int>();
            //    //        _dic[id] = lst;
            //    //    }
            //    //    lst.Add(r.GetInt32(1));

            //    //    //var g=WKBHelper.fromWKB(r.GetValue(2) as byte[]);
            //    //    return true;
            //    //});
            //}
            public void saveDeletedRowids()//DBSpatialite db)
            {
                //Console.WriteLine("正在删除多余的" + _tableName + "：" + _delRowids.Count + "个");
                //InitLandDotCoilUtil.DeleteByRowids(_p._db, _tableName, _delRowids);
            }
            ///// <summary>
            ///// 获取添加和修改的记录总数
            ///// </summary>
            ///// <returns></returns>
            //public int GetCreatedCount()
            //{
            //    return _nTestInsertCount;// +_nTestUpdateCount;
            //}
            public void Clear()
            {
                _nCurrShapeID = 0;
                //_dic.Clear();
                //_delRowids.Clear();

                //_nTestInsertCount = 0;
                //_nTestUpdateCount = 0;
            }
            public void testLogout(string tableAlias)
            {
                //Console.WriteLine("修改" + _tableName + "：" + _nTestUpdateCount + "条");
                //Console.WriteLine("插入" + _tableName + "：" + _nTestInsertCount + "条");
                //Console.WriteLine("删除" + _tableName + "：" + _delRowids.Count + "条");
            }
            ///// <summary>
            ///// 根据地块ID查找对应的rowid集合（jzd表）
            ///// </summary>
            ///// <param name="dkid"></param>
            ///// <returns></returns>
            //public List<int> GetRowidsByDkid(string dkid)
            //{
            //    List<int> lst;
            //    if (_dic.TryGetValue(dkid, out lst))
            //        return lst;
            //    return null;
            //}

            //public void AddJzd(JzdEntity en)
            //{
            //    _jzdCache.Add(en);
            //    if (_jzdCache.Count > 5000)
            //    {
            //        Flush();
            //    }
            //}

            ///// <summary>
            ///// 保存界址点
            ///// </summary>
            ///// <param name="db"></param>
            ///// <param name="initParam"></param>
            ///// <param name="lst"></param>
            //public void Save(ShortZd_cbd cbd)//DBSpatialite db, InitLandDotCoilParam initParam, List<JzdEntity> lst,int srid)
            //{
            //    var initParam = _p._param;
            //    var db = _p._db;
            //    var srid = _p._srid;
            //    var cjsj = DateTime.Now;//.ToString();

            //    var jzdRowids = GetRowidsByDkid(cbd.dkID);
            //    if (jzdRowids != null)
            //    {
            //        for (int i = cbd.lstJzdEntity.Count; i < jzdRowids.Count; ++i)
            //        {
            //            _delRowids.Add(jzdRowids[i]);
            //        }
            //    }

            //    var trans = db.BeginTransaction();
            //    //int nJzdh = 0;
            //    int j = 0;
            //    foreach (var en in cbd.lstJzdEntity)
            //    {
            //        if (jzdRowids != null && j < jzdRowids.Count)
            //        {
            //            en.rowID = jzdRowids[j++];
            //        }
            //        var fUpdate = en.rowID > 0;
            //        var prms = fUpdate ? _updateParams : _insertParams;
            //        var geomText = en.shape == null ? "null" : "GeomFromText('POINT(" + en.shape.X + " " + en.shape.Y + ")'," + srid + ")";
            //        int i = 0;
            //        prms[i].ParamValue = en.ID;
            //        prms[++i].ParamValue = en.BSM;
            //        prms[++i].ParamValue = initParam.AddressPointPrefix + (++_nStartTBJZDH);// en.TBJZDH;
            //        prms[++i].ParamValue = initParam.AddressPointPrefix + en.JZDH;
            //        prms[++i].ParamValue = cbd.dkID;// en.dkID;
            //        prms[++i].ParamValue = cbd.zlDM;// en.DYBM;
            //        prms[++i].ParamValue = en.JBLX.ToString();
            //        prms[++i].ParamValue = en.JZDLX.ToString();
            //        prms[++i].ParamValue = cbd.DKBM;// en.JZDLX.ToString();
            //        prms[++i].ParamValue = cjsj;
            //        prms[++i].ParamValue = en.SFKY;
            //        if (fUpdate)
            //        {
            //            prms[++i].ParamValue = en.rowID;
            //            var updateSql = InitLandDotCoilUtil.constructUpdateSql(JzdFields.TABLE_NAME, _fields, "rowid=@rowid"
            //                , geomText);
            //            db.ExecuteNonQuery(updateSql, prms);
            //            ++_nTestUpdateCount;
            //        }
            //        else
            //        {
            //            var insertSql = InitLandDotCoilUtil.constructInsertSql(JzdFields.TABLE_NAME, _fields, geomText);
            //            db.ExecuteNonQuery(insertSql, prms);
            //            ++_nTestInsertCount;
            //        }
            //    }
            //    trans.Commit();
            //}

        }
        /// <summary>
        /// 界址点表辅助类
        /// </summary>
        class JzdTable : TableBase
        {
            ///// <summary>
            ///// 统编界址点号
            ///// </summary>
            //private int _nStartTBJZDH = 0;
            private int _nStartBSM;
            private readonly JzxTable _jzxTable;
            private ShapeFile _dkShp;
            private ShapeFile _jzdShp;
            private int _nKJZBField;
            private NetTopologySuite.Geometries.Point _tmpPoint = new NetTopologySuite.Geometries.Point(0, 0);
            private readonly int[] _jzdDbfFieldIndex = new int[8];
            public JzdTable(InitLandDotCoil p_)
                : base(p_)
            {
                _jzxTable = new JzxTable(p_);
            }
            public void init(ShapeFile dkShp, ShapeFile jzdShp, ShapeFile jzxShp)
            {
                _dkShp = dkShp;
                _jzdShp = jzdShp;
                _nKJZBField = dkShp.FindField("KJZB");
                if (_nKJZBField < 0)
                {
                    //throw new Exception("未找到字段：KJZB");
                }
                //base.init();//JzdFields.DKID);
                var saFieldName = new string[]{
                    JzdFields.BSM,
                    JzdFields.YSDM,
                    JzdFields.JZDH,
                    JzdFields.JZDLX,
                    JzdFields.JBLX,
                    JzdFields.DKBM,
                    JzdFields.XZBZ,
                    JzdFields.YZBZ,
                };
                for (int i = 0; i < saFieldName.Length; ++i)
                {
                    var iField = _jzdShp.FindField(saFieldName[i]);
                    if (iField < 0)
                    {
                        //throw new Exception("未找到字段：" + saFieldName[i]);
                    }
                    _jzdDbfFieldIndex[i] = iField;
                }

                //_jzdDbfFieldIndex[0] = _jzdShp.FindField(JzdFields.BSM);
                //_jzdDbfFieldIndex[++i] = _jzdShp.FindField(JzdFields.YSDM);
                //_jzdDbfFieldIndex[++i] = _jzdShp.FindField(JzdFields.JZDH);
                //_jzdDbfFieldIndex[++i] = _jzdShp.FindField(JzdFields.JZDLX);
                //_jzdDbfFieldIndex[++i] = _jzdShp.FindField(JzdFields.JBLX);
                //_jzdDbfFieldIndex[++i] = _jzdShp.FindField(JzdFields.DKBM);
                //_jzdDbfFieldIndex[++i] = _jzdShp.FindField(JzdFields.XZBZ);
                //_jzdDbfFieldIndex[++i] = _jzdShp.FindField(JzdFields.YZBZ);
                _jzxTable.init(jzxShp);//JzxFields.DKID);
            }
            /// <summary>
            /// 获取生成的界址线的总数
            /// </summary>
            /// <returns></returns>
            public int GetExportedJzxCount()
            {
                return _jzxTable.ExportedCount;
            }
            public new void Clear()
            {
                base.Clear();
                //_nStartTBJZDH = 0;
                //_nCurrShapeID = 0;
                _jzxTable.Clear();
                _nStartBSM = _p._param.nJzdBSMStartVal;
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
                string sKJZB = null;
                foreach (var en in cbd.lstJzdEntity)
                {
                    var jzdh = save(cbd, en);
                    if (jzdh == null)
                        continue;
                    if (sKJZB == null)
                        sKJZB = jzdh;
                    else if (sKJZB.Length + jzdh.Length + 1 < 254)
                        sKJZB += "/" + jzdh;
                }
                if (_nKJZBField >= 0)
                {
                    System.Diagnostics.Debug.Assert(sKJZB.Length < 254);
                    _dkShp.WriteFieldString(cbd.rowid, _nKJZBField, sKJZB);
                }

                _jzxTable.Save(cbd);
                //trans.Commit();
            }

            /// <summary>
            /// 返回界址点号，can be null
            /// </summary>
            /// <param name="cbd"></param>
            /// <param name="jzdEn"></param>
            /// <returns></returns>
            private string save(ShortZd_cbd cbd, JzdEntity jzdEn)
            {
                if (_p._param.fOnlyExportKeyJzd && jzdEn.SFKY == false)
                {
                    return null;
                }
                if (jzdEn.fInsertedPoint)
                {//如果是插入的界址点则不保存
                    return null;
                }

                string jzdh = _p._param.AddressPointPrefix + (1 + _nCurrShapeID);
                string dkbm = null;
                JzdEdges val;
                if (_p._jzdCache.TryGetValue(jzdEn.shape, out val))
                {
                    if (val.fHasExported)
                        return val.Jzdh;
                    //val.fHasExported = true;
                    val.Jzdh = jzdh;
                    foreach (var je in val)
                    {
                        if (dkbm == null)
                        {
                            dkbm = je.dk.DKBM;
                        }
                        else if (je.dk != null && je.dk.DKBM != null)
                        {
                            if (dkbm.Length + je.dk.DKBM.Length < 254)
                            {
                                dkbm += "/" + je.dk.DKBM;
                            }
                        }
                    }
                }
                else
                {//程序没有写错的话不会走到这里来
                    Console.WriteLine("err1");
                    System.Diagnostics.Debug.Assert(false);
                    //_p._jzdCache.AddPoint(jzdEn.shape);
                    return null;
                }

                if (_nCurrShapeID > _nindex * 2000000)
                {
                    _jzdShp.Close();
                    var shppath = Path.Combine(_p._param.shapefilePath, $"JZD{_p._param.extName}_{_nindex}.shp");
                    _jzdShp = ShapeFileHelper.createJzdShapeFile(shppath, _p._param.prjStr);
                    _jzdShp.Open(shppath, "rb+");
                    _nindex++;
                }
                var shp = _jzdShp;
                var wkb = toWKB(jzdEn.shape);
                try
                {
                    shp.WriteWKB(-1, wkb);// jzdEn.wkb);
                    int i = -1;
                    shp.WriteFieldInt(_nCurrShapeID, _jzdDbfFieldIndex[++i], _nStartBSM++);
                    shp.WriteFieldString(_nCurrShapeID, _jzdDbfFieldIndex[++i], _p._param.sJzdYSDMVal);// .sJzdYSDMVal);
                    shp.WriteFieldString(_nCurrShapeID, _jzdDbfFieldIndex[++i], jzdh);// "J" + jzdh);
                    shp.WriteFieldString(_nCurrShapeID, _jzdDbfFieldIndex[++i], _p._param.sJZDLXVal);// jzdEn.JZDLX);
                    shp.WriteFieldString(_nCurrShapeID, _jzdDbfFieldIndex[++i], _p._param.sJBLXVal);// jzdEn.jblx);
                    shp.WriteFieldString(_nCurrShapeID, _jzdDbfFieldIndex[++i], dkbm);
                    shp.WriteFieldDouble(_nCurrShapeID, _jzdDbfFieldIndex[++i], Math.Round(jzdEn.shape.Y, 3));
                    shp.WriteFieldDouble(_nCurrShapeID, _jzdDbfFieldIndex[++i], Math.Round(jzdEn.shape.X, 3));
                    ++_nCurrShapeID;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return jzdh;
            }
            private byte[] toWKB(Coordinate c)
            {
                _tmpPoint.X = c.X;
                _tmpPoint.Y = c.Y;
                return _tmpPoint.ToBinary();
            }
        }

        /// <summary>
        /// 界址线表辅助类
        /// </summary>
        class JzxTable : TableBase
        {
            /// <summary>
            /// 界址线位置常量
            /// </summary>
            class JzxwzType
            {
                /// <summary>
                /// 内
                /// </summary>
                public const string Left = "1";
                /// <summary>
                /// 中
                /// </summary>
                public const string Middle = "2";
                /// <summary>
                /// 外
                /// </summary>
                public const string Right = "3";
            }

            private readonly List<JzdEntity> _points = new List<JzdEntity>();
            private readonly JzxEntity _jzxEn = new JzxEntity();
            private readonly HashSet<ShortZd_cbd> _qlrSet = new HashSet<ShortZd_cbd>();
            private readonly Dictionary<ShortZd_cbd, IGeometry> _polygonCache = new Dictionary<ShortZd_cbd, IGeometry>();

            /// <summary>
            /// 非正常释放的地块数
            /// </summary>
            private int _nTestReleasedDk = 0;
            private ShapeFile _jzxShp;
            private int[] _jzxDbfFieldIndex = new int[12];
            //private int _nCurrShapeID = 0;
            private int _nStartBSM = 0;
            public JzxTable(InitLandDotCoil p_)
                : base(p_)
            {
            }
            public void init(ShapeFile jzxShp)
            {
                _jzxShp = jzxShp;
                int i = 0;
                _jzxDbfFieldIndex[0] = _jzxShp.FindField(JzxFields.BSM);
                _jzxDbfFieldIndex[++i] = _jzxShp.FindField(JzxFields.YSDM);
                _jzxDbfFieldIndex[++i] = _jzxShp.FindField(JzxFields.JXXZ);
                _jzxDbfFieldIndex[++i] = _jzxShp.FindField(JzxFields.JZXLB);
                _jzxDbfFieldIndex[++i] = _jzxShp.FindField(JzxFields.JZXWZ);
                _jzxDbfFieldIndex[++i] = _jzxShp.FindField(JzxFields.JZXSM);
                _jzxDbfFieldIndex[++i] = _jzxShp.FindField(JzxFields.PLDWQLR);
                _jzxDbfFieldIndex[++i] = _jzxShp.FindField(JzxFields.PLDWZJR);
                _jzxDbfFieldIndex[++i] = _jzxShp.FindField(JzxFields.JZXH);
                _jzxDbfFieldIndex[++i] = _jzxShp.FindField(JzxFields.QJZDH);
                _jzxDbfFieldIndex[++i] = _jzxShp.FindField(JzxFields.ZJZDH);
                _jzxDbfFieldIndex[++i] = _jzxShp.FindField(JzxFields.DKBM);
            }
            public new void Clear()
            {
                base.Clear();
                _points.Clear();
                _qlrSet.Clear();
                _polygonCache.Clear();
                _nTestReleasedDk = 0;
                //_nCurrShapeID = 0;
                _nStartBSM = _p._param.nJzxBSMStartVal;

            }
            /// <summary>
            /// 保存界址线
            /// </summary>
            /// <param name="db"></param>
            /// <param name="initParam"></param>
            /// <param name="lst"></param>
            public void Save(ShortZd_cbd cbd)//DBSpatialite db, InitLandDotCoilParam initParam, List<JzdEntity> lst,int srid)
            {
                var initParam = _p._param;
                int i = 0;
                queryPoints(cbd, points =>
                {
                    var jzxEn = _jzxEn;
                    jzxEn.Clear();
                    var jzdEnd = points[points.Count - 1];
                    Coordinate[] shape;
                    ShortZd_cbd pld;//毗邻地
                    bool fTwin;//是否同毗邻地田挨田（无间隙）
                    var JZXCD = Math.Round(calcLength(cbd, points, out shape, out pld, out fTwin), 2);
                    //if (pld == null)
                    //{//如果没有权利人则取该地块的村名称（含乡镇名）
                    //    //jzxEn.PLDWQLR = _p._xzdyUtil.GetShortQmc(cbd.zlDM);
                    //    jzxEn.PLDWZJR = jzxEn.PLDWQLR;
                    //}
                    //else
                    if (pld != null)
                    {
                        jzxEn.PLDWQLR = pld.qlrMc;// calcPldwQlr(cbd, points);
                        //jzxEn.PLDWZJR = pld.zjrMc != null ? pld.zjrMc : pld.qlrMc;
                    }

                    if (fTwin == false)
                    {
                        jzxEn.JZXWZ = JzxwzType.Right;
                    }
                    else if (pld != null)
                    {
                        if (cbd.elevation > 9000 || pld.elevation > 9000
                            || cbd.elevation == pld.elevation)
                        {
                            jzxEn.JZXWZ = JzxwzType.Middle;
                        }
                        else if (pld.elevation > cbd.elevation)
                        {
                            jzxEn.JZXWZ = JzxwzType.Left;
                        }
                        else
                        {
                            jzxEn.JZXWZ = JzxwzType.Right;
                        }
                    }
                    //jzxEn.JZXWZ = initParam.AddressLinePosition;
                    jzxEn.Shape = shape;
                    GetLineDescription(jzxEn, points, JZXCD);
                    //if (_p._param.IsLineDescription)
                    //{
                    //    jzxEn.JZXSM = JZXCD.ToString();// jzxEn.JZXCD.ToString();
                    //}
                    //jzxEn.DKBM = cbd.DKBM;
                    //jzxEn.JXXZ = initParam.JXXZ;
                    //jzxEn.JZXLB = initParam.JZXLB;

                    save(_jzxEn);//, points);
                    ++i;
                });
            }

            private void GetLineDescription(JzxEntity jzxEn, List<JzdEntity> points, double length)
            {
                if (points == null || points.Count < 2) return;

                if (_p._param.LineDescription == eLineDescription.Length)
                {
                    jzxEn.JZXSM = length.ToString();
                }
                else
                {
                    var flag = _p._param.AddressPointPrefix;
                    var startPoint = points[0];
                    var endPoint = points[points.Count - 1];
                    string direction = GetLineDirection(startPoint, endPoint);
                    if (_p._param.LineDescription == eLineDescription.LengthDirectrion)
                    {
                        jzxEn.JZXSM = string.Format("沿{0}方向{1}米", direction, length);
                    }
                    else if (_p._param.LineDescription == eLineDescription.LengthDirectrionPosition)
                    {

                        jzxEn.JZXSM = string.Format("沿{0}{1}{2}方向{3}米", GetLineType(_p._param.JZXLB), GetLinePosition(jzxEn.JZXWZ), direction, length);
                    }
                }
            }

            private string GetLineType(string jzxlb)
            {
                string result = string.Empty;
                switch (jzxlb)
                {
                    case "01":
                        result = "田垄(埂)";
                        break;
                    case "02":
                        result = "沟渠";
                        break;
                    case "03":
                        result = "道路";
                        break;
                    case "04":
                        result = "行树";
                        break;
                    case "05":
                        result = "围墙";
                        break;
                    case "06":
                        result = "墙壁";
                        break;
                    case "07":
                        result = "栅栏";
                        break;
                    case "08":
                        result = "两点连线";
                        break;
                    case "99":
                        result = "其他界线";
                        break;
                    default:
                        break;
                }
                return result;
            }

            private string GetLinePosition(string jzxwz)
            {
                string result = string.Empty;
                switch (jzxwz)
                {
                    case "1":
                        result = "内侧";
                        break;
                    case "2":
                        result = "中间";
                        break;
                    case "3":
                        result = "外侧";
                        break;
                }
                return result;
            }

            private string GetLineDirection(JzdEntity sd, JzdEntity ed)
            {
                var edx = ed.shape.X;
                var edy = ed.shape.Y;
                var sdx = sd.shape.X;
                var sdy = sd.shape.Y;

                double xLength = edx - sdx;
                double yLength = edy - sdy;

                string direction = "";
                if (xLength > 0.0 && yLength > 0.0)
                {
                    direction = "东北";
                }
                else if (xLength < 0.0 && yLength < 0.0)
                {
                    direction = "西南";
                }
                else if (xLength < 0.0 && yLength > 0.0)
                {
                    direction = "西北";
                }
                else if (xLength > 0.0 && yLength < 0.0)
                {
                    direction = "东南";
                }
                else if (xLength == 0.0 && yLength > 0.0)
                {
                    direction = "正北";
                }
                else if (xLength == 0.0 && yLength < 0.0)
                {
                    direction = "正南";
                }
                else if (xLength > 0.0 && yLength == 0.0)
                {
                    direction = "正东";
                }
                else if (xLength < 0.0 && yLength == 0.0)
                {
                    direction = "正西";
                }
                return direction;
            }

            public void testLogout()
            {
                base.testLogout("界址线");
                Console.WriteLine("非正常释放的地块数：" + _nTestReleasedDk);
            }


            private void save(JzxEntity en)//,List<JzdEntity> points)
            {
                //if (InitLandDotCoilUtil.testIsEqual(en.Shape[0], 552386.27529999986, 3907072.9354500007))
                //{
                //    Console.WriteLine(en.Shape[0]);
                //}
                string qJzdh = null;
                string zJzdh = null;
                JzdEdges val;
                if (_p._jzdCache.TryGetValue(en.Shape[0], out val))
                {

                    qJzdh = val.Jzdh;
                    if (val.lstJzxEntities == null)
                    {
                        val.lstJzxEntities = new List<Coordinate[]>();
                        val.lstJzxEntities.Add(en.Shape);
                    }
                    else
                    {
                        bool fFind = false;
                        foreach (var je in val.lstJzxEntities)
                        {
                            if (isSameLine(en.Shape, je))
                            {
                                fFind = true;
                                break;
                            }
                        }
                        if (fFind)
                        {
                            return;
                        }
                        val.lstJzxEntities.Add(en.Shape);
                    }
                    //if (val.fHasExported)
                    //    return;
                    //val.fHasExported = true;
                }
                else
                {//程序没有写错的话不会走到这里来
                    Console.WriteLine("err2");
                    System.Diagnostics.Debug.Assert(false);
                }
                if (en.Shape == null || en.Shape.Length == 0)
                    return;

                string dkbh = null;
                string jzr = null;
                string qlr = null;
                #region 查找界址线的地块编号、止界址点号、指界人、权利人
                var lstDk = new List<ShortZd_cbd>();
                if (val != null)
                {
                    foreach (var je in val)
                    {
                        lstDk.Add(je.dk);
                    }

                    for (int j = 1; j < en.Shape.Length; ++j)
                    {
                        var pt = en.Shape[j];
                        if (_p._jzdCache.TryGetValue(pt, out val))
                        {
                            var lstDk1 = new List<ShortZd_cbd>();
                            foreach (var je in val)
                            {
                                if (lstDk.Contains(je.dk))
                                {
                                    lstDk1.Add(je.dk);
                                }
                            }
                            lstDk.Clear();
                            lstDk.AddRange(lstDk1);
                            if (j == en.Shape.Length - 1)
                            {
                                zJzdh = val.Jzdh;
                            }
                        }
                    }
                    foreach (var dk in lstDk)
                    {
                        if (dkbh == null)
                        {
                            dkbh = dk.DKBM;
                            jzr = dk.zjrMc;
                            qlr = _p.OnQueryCbdQlr(dk);
                        }
                        else
                        {
                            dkbh += "/" + dk.DKBM;
                            jzr += "/" + dk.zjrMc;
                            qlr += "/" + _p.OnQueryCbdQlr(dk);
                        }
                    }
                }
                #endregion

                // 界址线说明补充起止界址点号
                if (_p._param.LineDescription == eLineDescription.LengthDirectrionPosition)
                {
                    en.JZXSM = qJzdh + en.JZXSM + "到" + zJzdh;
                }

                if (_nCurrShapeID > _nindex * 2000000)
                {
                    _jzxShp.Close();
                    var shppath = Path.Combine(_p._param.shapefilePath, $"JZX{_p._param.extName}_{_nindex}.shp");
                    _jzxShp = ShapeFileHelper.createJzdShapeFile(shppath, _p._param.prjStr);
                    _jzxShp.Open(shppath, "rb+");
                    _nindex++;
                }
                var shp = _jzxShp;
                //string jzdh = _p._param.AddressPointPrefix + (1 + _nCurrShapeID);
                string jzxh = (1 + _nCurrShapeID).ToString();
                var wkb = toWKB(en.Shape);
                shp.WriteWKB(-1, wkb);// jzdEn.wkb);
                int i = -1;
                shp.WriteFieldInt(_nCurrShapeID, _jzxDbfFieldIndex[++i], _nStartBSM++);
                shp.WriteFieldString(_nCurrShapeID, _jzxDbfFieldIndex[++i], _p._param.sJzxYSDMVal);// .sJzdYSDMVal);
                shp.WriteFieldString(_nCurrShapeID, _jzxDbfFieldIndex[++i], _p._param.JXXZ);// "J" + jzdh);
                shp.WriteFieldString(_nCurrShapeID, _jzxDbfFieldIndex[++i], _p._param.JZXLB);// jzdEn.JZDLX);
                shp.WriteFieldString(_nCurrShapeID, _jzxDbfFieldIndex[++i], en.JZXWZ);
                shp.WriteFieldString(_nCurrShapeID, _jzxDbfFieldIndex[++i], en.JZXSM);
                shp.WriteFieldString(_nCurrShapeID, _jzxDbfFieldIndex[++i], qlr);// en.PLDWQLR);
                shp.WriteFieldString(_nCurrShapeID, _jzxDbfFieldIndex[++i], jzr);
                shp.WriteFieldString(_nCurrShapeID, _jzxDbfFieldIndex[++i], jzxh);
                shp.WriteFieldString(_nCurrShapeID, _jzxDbfFieldIndex[++i], qJzdh);
                shp.WriteFieldString(_nCurrShapeID, _jzxDbfFieldIndex[++i], zJzdh);
                shp.WriteFieldString(_nCurrShapeID, _jzxDbfFieldIndex[++i], dkbh);
                ++_nCurrShapeID;
            }


            private byte[] toWKB(Coordinate[] coords)
            {
                return GeometryHelper.MakeLine(coords).AsBinary();
            }
            private bool isSameLine(Coordinate[] l1, Coordinate[] l2)
            {
                if (l1.Length != l2.Length)
                    return false;
                for (int i = 0; i < l1.Length; ++i)
                {
                    var c1 = l1[i];
                    var c2 = l2[i];
                    if (!CglHelper.IsSame2(c1, c2, _p._param.LineOverlapTolerance, _p._param.LineOverlapTolerance2))
                        return false;
                }
                return true;
            }

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

            private double calcLength(ShortZd_cbd cbd, List<JzdEntity> lst, out Coordinate[] lineWkb, out ShortZd_cbd qlr, out bool fTwin)
            {
                _qlrSet.Clear();
                qlr = null;
                fTwin = false;
                //if (cbd.DKBM == "5117022082000301316")
                //{
                //    qlr = null;
                //}
                //ILineString shape = null;

                var coords = new Coordinate[lst.Count];
                double len = 0;
                var pre = lst[0];
                coords[0] = pre.shape;
                for (int i = 1; i < lst.Count; ++i)
                {
                    //if (InitLandDotCoilUtil.testIsEqual(lst[i].shape, 452090.738708, 3484712.526672))
                    //{
                    //    Console.WriteLine(lst[i].shape.ToString());
                    //}
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
                var shape = GeometryHelper.MakeLine(coords);

                if (coords[0].X > coords[coords.Length - 1].X
                    || coords[0].X == coords[coords.Length - 1].X && coords[0].Y > coords[coords.Length - 1].Y)
                {
                    GeometryHelper.Reverse(coords);
                }
                lineWkb = coords;

                if (_qlrSet.Count == 1)
                {
                    qlr = _qlrSet.First();//.qlrMc;
                }
                else if (_qlrSet.Count > 1)
                {
                    var s = onlyOneQlr(_qlrSet);
                    if (s != null)
                    {
                        qlr = s;
                    }
                    else
                    {
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
                                //var gi = offLine.Intersection(g);
                                var oi = YuLinTu.tGISCNet.Topology.Intersect(g.AsBinary(), offLine.AsBinary());
                                if (oi != null)
                                {
                                    //g = geometryPrecisionReducer.Reduce(g);
                                    //var gi = offLine.Intersection(g);
                                    var gi = WKBHelper.fromWKB(oi);

                                    if (gi != null)
                                    {
                                        var len1 = gi.Area;
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
                    }
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
            private IGeometry offsetLeft(List<JzdEntity> lst)
            {
                //var coords =new List<Coordinate>();// new Coordinate[lst.Count];
                var lines = new List<ILineString>();
                var distance = _p._param.AddressLinedbiDistance;
                var ptFrom = lst[0].shape;
                //coords[0] = CglHelper.deflection_distance(lst[1].shape, ptFrom, -90, distance);
                for (int i = 1; i < lst.Count; ++i)
                {
                    var ptTo = lst[i].shape;
                    var ls = CglHelper.OffsetLeft(ptFrom, ptTo, distance);
                    lines.Add(ls);
                    //var c1 =CglHelper.deflection_distance(ptFrom, ptTo, 90, distance);
                    //var c2 = CglHelper.deflection_distance(ptTo, ptFrom, -90, distance);
                    //coords.Add(c1);
                    //coords.Add(c2);
                    //coords[i] = c1;
                    ptFrom = ptTo;
                }
                return new NetTopologySuite.Geometries.MultiLineString(lines.ToArray());
                //return GeometryHelper.MakeLine(coords.ToArray());
            }


            private IGeometry getCachePolygon(ShortZd_cbd cbd)
            {
                IGeometry g;
                if (_polygonCache.TryGetValue(cbd, out g))
                {
                    return g;
                }
                g = cbd.MakePolygon();
                //if (g == null)
                //{
                //    g = _p._db.GetShape(Zd_cbdFields.TABLE_NAME, Zd_cbdFields.Shape, cbd.rowid);
                //    ++_nTestReleasedDk;
                //}
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
            private const int CACHE_BUFFER_SIZE = 2000;
            private int _flushCount = 0;

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
            /// <param name="nKeyJzdCount"></param>
            /// <param name="MinKeyJzdCount"></param>
            public void Ensure4KeyJzd(List<JzdEntity> lstJzd, int nKeyJzdCount, int MinKeyJzdCount)
            {
                if (lstJzd.Count <= MinKeyJzdCount)
                {
                    foreach (var jzdEn in lstJzd)
                    {
                        jzdEn.SFKY = true;
                        SetKeyJzd(jzdEn.shape);
                    }
                    return;
                }

                var ll = new List<MyList>();
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
                while (true)
                {
                    MyList splitItem = null;
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
                        break;
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


            #endregion

            public SaveCacheHelper(InitLandDotCoil p_)
            {
                p = p_;
            }
            public void Add(ShortZd_cbd en)//,bool fReleaseCache)
            {
                System.Diagnostics.Debug.Assert(en.lstJzdEntity == null);
                System.Diagnostics.Debug.Assert(en.shell.Count > 0);

                #region 生成en.lstJzdEntity并将en加入到_cacheList
                en.lstJzdEntity = new List<JzdEntity>();
                var lstJzd = en.lstJzdEntity;

                toJzdList(en.shell, true, lstJzd);
                Assign(en, lstJzd, 0);
                if (en.holes != null)
                {
                    foreach (var h in en.holes)
                    {
                        int iBegin = lstJzd.Count;
                        toJzdList(h, false, lstJzd);
                        Assign(en, lstJzd, iBegin);
                    }
                }
                _cacheList.Add(en);
                #endregion

                if (_cacheList.Count >= CACHE_BUFFER_SIZE)
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

                if (_saveList.Count >= CACHE_BUFFER_SIZE)
                {
                    Flush(false);

                    //if (_releaseList.Count >= CACHE_BUFFER_SIZE)
                    //{
                    //    if (fReleaseCache)
                    //    {
                    //        tryRelease();
                    //    }
                    //}
                }
            }

            public void Clear()
            {
                _cacheList.Clear();
                _saveList.Clear();
                _releaseList.Clear();
                _flushCount = 0;
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
                    if (this.p.ReportSaveCount != null)
                    {
                        _flushCount += _saveList.Count;
                        this.p.ReportSaveCount(_flushCount);
                    }
                }
                else
                {
                    Console.WriteLine("正在保存缓存：" + _saveList.Count + "个");
                    if (this.p.ReportSaveCount != null)
                    {
                        _flushCount += _saveList.Count;
                        this.p.ReportSaveCount(_flushCount);
                    }
                }
                //var trans=p._db.BeginTransaction();
                foreach (var c in _saveList)
                {
                    p._jzdTable.Save(c);//p._db, p._param, c.lstJzd, p._srid);
                    c.fSaved = true;
                    _releaseList.Add(c);
                }
                //trans.Commit();

                _saveList.Clear();

                if (_releaseList.Count >= CACHE_BUFFER_SIZE)
                {
                    if (!fLast)
                    {
                        tryRelease();
                    }
                }

                //if (!fLast)
                //{
                //    #region 清除所有不再使用的界址点
                //    p._jzdCache.RemoveAllNotUsedJzd(p._cbdCache);
                if (p._nTestMaxCacheJzdCount < p._jzdCache.Count)
                {
                    p._nTestMaxCacheJzdCount = p._jzdCache.Count;
                }
                //#endregion
                //}
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

            private static bool Less(Coordinate c0, Coordinate c1)
            {
                if (c0.X < c1.X)
                    return true;
                if (c0.X > c1.X)
                    return false;
                return c0.Y < c1.Y;
            }

            private static double calcLen(List<JzdEntity> lstJzd)
            {
                double d = 0;
                var preJzd = lstJzd[lstJzd.Count - 1];
                for (int i = 0; i < lstJzd.Count; ++i)
                {
                    var jzd = lstJzd[i];
                    d += calcLen(preJzd, jzd);//lstJzd[i - 1], lstJzd[i]);
                    preJzd = jzd;
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
            /// 从iBegin开始为集合中的实体赋值
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
                    remainLen -= len;
                    if (jzdEn.SFKY != true)
                    {
                        //if (jzdEn.shape.Y < 3484512 && en.DKBM == "5117022082000600145")
                        //{
                        //    jzdEn.SFKY = false;
                        //}
                        //if (InitLandDotCoilUtil.testIsEqual(jzdEn.shape, 452077.964294, 3485078.776306))
                        //{
                        //    jzdEn.SFKY = false;
                        //}
                        if (len1 > _param.Tolerance && remainLen > _param.Tolerance)
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
                                                //Coordinate q0 = null, q1 = null;
                                                var p0 = lst[0].InEdge.qJzd;
                                                var p1 = lst[0].OutEdge.zJzd;
                                                //if (Less(p0, p1))
                                                //{
                                                //    p0 = p1;
                                                //    p1 = lst[0].InEdge.qJzd;
                                                //}
                                                var q0 = lst[1].InEdge.qJzd;
                                                var q1 = lst[1].OutEdge.zJzd;
                                                //if (Less(q0, q1))
                                                //{
                                                //    q0 = q1;
                                                //    q1 = lst[1].InEdge.qJzd;
                                                //}
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
                    nKeyJzdCount += jzdEn.SFKY ? 1 : 0;
                    if (jzdEn.SFKY)
                    {
                        len1 = 0;
                    }
                    preJzd = jzdEn;
                }

                if (nKeyJzdCount < _param.MinKeyJzdCount)
                {
                    //if (en.rowid == 94884)
                    //{
                    //    en.rowid = 94884;
                    //}
                    Ensure4KeyJzd(lstJzd, nKeyJzdCount, _param.MinKeyJzdCount);
                }
            }

            /// <summary>
            /// 判断角度是否可判断为关键界址点
            /// </summary>
            /// <param name="angle"></param>
            /// <returns></returns>
            private bool isKeyAngle(double angle)
            {
                return angle >= (double)p._param.MinAngleFileter
                                            && angle <= (double)p._param.MaxAngleFilter;
            }
            private void toJzdList(ShortZd_cbd cbd, List<JzdEntity> lst)
            {
                //InitLandDotCoilUtil.SortCoordsByWNOrder(r, fShell, c =>
                cbd.QueryPoint(c =>
                {
                    var en = new JzdEntity();
                    lst.Add(en);
                    en.shape = c;
                    //en.ID = InitLandDotCoilUtil.CreateNewID();// Guid.NewGuid().ToString().Trim(new char[] { '{', '}' });
                    //en.BSM = en.ID;
                });
            }
            private void toJzdList(JzxRing r, bool fShell, List<JzdEntity> lst)
            {
                InitLandDotCoilUtil.SortCoordsByWNOrder(r, fShell, c =>
                {
                    lst.Add(c);
                    //var en = new JzdEntity();
                    //lst.Add(en);
                    //en.shape = c;
                    //en.ID = InitLandDotCoilUtil.CreateNewID();// Guid.NewGuid().ToString().Trim(new char[] { '{', '}' });
                    //en.BSM = en.ID;
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
                            if (jzdEn.SFKY == false && p._jzdEqualComparer.Equals(jzdEn.shape, jzd))
                            {
                                jzdEn.SFKY = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 缓存界址点对应的出度的集合
        /// </summary>
        class JzdCache
        {
            private readonly InitLandDotCoil p;
            private readonly Dictionary<Coordinate, JzdEdges> _dicJzdOutEdges;

            public JzdCache(InitLandDotCoil p_)
            {
                p = p_;
                _dicJzdOutEdges = new Dictionary<Coordinate, JzdEdges>(p._jzdEqualComparer);
            }

            /// <summary>
            /// 从界址线中获取界址点
            /// </summary>
            /// <param name="r"></param>
            /// <param name="dicJzd"></param>
            private void AcquireJzd(JzxRing r, ShortZd_cbd rebuildCbd = null)
            {
                //var dicJzd = _dicJzdOutEdges;
                var preJzx = r[r.Count - 1];
                for (int i = 0; i < r.Count; ++i)
                {
                    var jzx = r[i];
                    var jzd = jzx.qJzd;

                    //if (InitLandDotCoilUtil.testIsEqual(jzd, 374145.28367387172, 3844386.5327095818, 0.1))
                    //{
                    //    Console.WriteLine(jzd);
                    //}

                    JzdEdges lst;
                    if (!TryGetValue(jzd, out lst))
                    {
                        lst = new JzdEdges();
                        _dicJzdOutEdges[jzd] = lst;
                    }
                    else
                    {
                        if (rebuildCbd != null)
                        {
                            lst.RemoveAll(a =>
                            {
                                return a.dk == rebuildCbd;
                            });
                        }
                    }

                    lst.Add(new JzdEdge() { OutEdge = jzx, InEdge = preJzx });
                    preJzx = jzx;
                }
            }

            public void AcquireJzd(ShortZd_cbd c, bool fRebuild = false)
            {
                AcquireJzd(c.shell, fRebuild ? c : null);
                if (c.holes != null)
                {
                    foreach (var h in c.holes)
                    {
                        AcquireJzd(h, fRebuild ? c : null);
                    }
                }
            }
            public void ReAcquireJzd(ShortZd_cbd en)
            {
                if (en.lstJzdEntity != null)
                {
                    foreach (var c in en.lstJzdEntity)
                    {
                        JzdEdges jes;
                        if (_dicJzdOutEdges.TryGetValue(c.shape, out jes))
                        {
                            for (int i = jes.Count - 1; i <= 0; --i)
                            {
                                var je = jes[i];
                                if (je.dk == en)
                                {
                                    jes.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
                AcquireJzd(en, true);
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
            public bool HasExported(Coordinate pt)
            {
                JzdEdges val;
                if (_dicJzdOutEdges.TryGetValue(pt, out val))
                {
                    return val.fHasExported;
                }
                return false;
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

            ///// <summary>
            ///// 清除所有不再使用的界址点
            ///// </summary>
            //public void RemoveAllNotUsedJzd(ShortZd_cbdCache cache)
            //{
            //    var set = new HashSet<ShortZd_cbd>();
            //    foreach (var dk in cache)
            //    {
            //        set.Add(dk);
            //    }

            //    var dkSet = new HashSet<ShortZd_cbd>();
            //    var lstDel = new List<Coordinate>();
            //    foreach (var kv in _dicJzdOutEdges)
            //    {
            //        bool fRemove = true;
            //        foreach (var jzx in kv.Value)
            //        {
            //            if (jzx.dk != null && !jzx.dk.fRemovedFromCache)
            //            {
            //                fRemove = false;
            //                break;
            //            }
            //        }
            //        if (fRemove)
            //        {
            //            lstDel.Add(kv.Key);
            //            foreach (var jzx in kv.Value)
            //            {
            //                if (jzx.dk != null && jzx.dk.fRemovedFromCache)
            //                {
            //                    dkSet.Add(jzx.dk);
            //                }
            //            }
            //        }
            //    }
            //    foreach (var jzd in lstDel)
            //    {
            //        _dicJzdOutEdges.Remove(jzd);
            //    }

            //    foreach (var dk in dkSet)
            //    {
            //        bool fClear = true;
            //        foreach (var v in _dicJzdOutEdges.Values)
            //        {
            //            foreach (var jzx in v)
            //            {
            //                if (jzx.dk == dk)
            //                {
            //                    fClear = false;
            //                    break;
            //                }
            //            }
            //            if (fClear == false)
            //                break;
            //        }
            //        if (fClear)
            //        {
            //            System.Diagnostics.Debug.Assert(!cache.Contains(dk));
            //            dk.Clear();
            //        }
            //    }
            //}

            public void Clear()
            {
                _dicJzdOutEdges.Clear();
            }
        }

        //private readonly DBSpatialite _db;
        private InitLandDotCoilParam _param;

        private int _curProgress;
        private int _oldProgress;//进度相关
        private int _progressCount;// { get { return _rowids.Count; } }
        //private int _srid;

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
        ///// <summary>
        ///// 待处理地块的rowid集合
        ///// </summary>
        //private readonly HashSet<int> _rowids = new HashSet<int>();

        private readonly JzdEqualComparer _jzdEqualComparer;


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
        public Action<int> ReportSaveCount;
        /// <summary>
        /// 获取承包地的权利人名称
        /// </summary>
        public Func<ShortZd_cbd, string> OnQueryCbdQlr;


        public InitLandDotCoil(/*DBSpatialite db,*/InitLandDotCoilParam param)
        {
            //_db = db;
            _param = param;
            //_srid=db.QuerySRID(Zd_cbdFields.TABLE_NAME);
            _jzdEqualComparer = new JzdEqualComparer(_param.Tolerance);
            //_dicJzdOutEdges = new Dictionary<Coordinate, JzdEdges>(_jzdEqualComparer);
            _saveCache = new SaveCacheHelper(this);
            _jzdCache = new JzdCache(this);
            _jzdTable = new JzdTable(this);
            _xzdyUtil.Init();//db);
        }
        public void DoInit(string dkShapeFile, string jzdShapeFile, string jzxShapeFile)
        {
            _param.AddressLinedbiDistance = 1.5;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            ShapeFile jzdShp = null;
            ShapeFile jzxShp = null;
            var prjStr = ShapeFileHelper.GetPrjString(dkShapeFile);
            if (File.Exists(jzdShapeFile))
            {
                ShapeFileHelper.DeleteShapeFile(jzdShapeFile);
                //throw new Exception("文件" + jzdShapeFile + "不存在！");
            }
            if (File.Exists(jzxShapeFile))
            {
                ShapeFileHelper.DeleteShapeFile(jzxShapeFile);
                //throw new Exception("文件" + jzxShapeFile + "不存在！");
            }
            var dkShp = new ShapeFile();

            try
            {
                jzdShp = ShapeFileHelper.createJzdShapeFile(jzdShapeFile, prjStr);
                jzxShp = ShapeFileHelper.createJzxShapeFile(jzxShapeFile, prjStr);
                dkShp.Open(dkShapeFile, "rb+");
                //jzdShp.Open(jzdShapeFile, "rb+");
                //jzxShp.Open(jzxShapeFile, "rb+");
                DoInit(dkShp, jzdShp, jzxShp);
                dkShp.Close();
                GC.Collect();
                jzxShp.Close();
                GC.Collect();
                jzdShp.Close();
                GC.Collect();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    dkShp.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                if (jzxShp != null)
                {
                    try
                    {
                        jzxShp.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                if (jzdShp != null)
                {
                    try
                    {
                        jzdShp.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            sw.Stop();
            ReportInfomation("总耗时：" + sw.Elapsed);

        }
        /// <summary>
        /// 开始初始化界址点和界址线
        /// </summary>
        /// <param name="wh"></param>
        private void DoInit(ShapeFile dkShp, ShapeFile jzdShp, ShapeFile jzxShp)//string wh)
        {
            //Console.WriteLine("开始时间：" + DateTime.Now);
            ReportInfomation("开始时间：" + DateTime.Now);
            //_rowids.Clear();
            _curProgress = 0;
            _oldProgress = 0;
            _nInsertedJzdCount = 0;
            _nGreat1QlrJzxCount = 0;
            _nTestMaxCacheJzdCount = 0;
            Clear();
            _jzdTable.init(dkShp, jzdShp, jzxShp);
            //_param.cjsj = DateTime.Now;

            int nDkShpCount = dkShp.GetRecordCount();
            _progressCount = nDkShpCount;// dkShp.GetRecordCount();

            //_jzdTable.init();// InitLandDotCoilUtil.QueryJzdRowIDs(_db, _testDic);


            //Console.WriteLine(string.Format("正在删除表{0}的空间索引...", JzdFields.TABLE_NAME));
            //_db.DropSpatialIndex(JzdFields.TABLE_NAME, JzdFields.SHAPE);

            var lstXBounds = new List<XBounds>();
            //var env = _db.QueryEnvelope(Zd_cbdFields.TABLE_NAME, Zd_cbdFields.Shape, wh, new List<string> { "rowid" }, r =>
            //{
            //    _rowids.Add(r.GetInt32(1));
            //});

            //dkShp.GetRecordCount()

            //Console.WriteLine("已选择地块个数共计："+_rowids.Count+"个");
            ReportInfomation("地块个数：" + nDkShpCount);

            //env.ExpandBy(_param.AddressLinedbiDistance);
            var env = dkShp.GetFullExtent();
            //var icc=new IntCoordConter();
            //icc.Init(env.MinX);
            //_db.QueryIntersectsCallback(Zd_cbdFields.TABLE_NAME, Zd_cbdFields.Shape, env, r =>
            for (int i = 0; i < nDkShpCount; ++i)
            {
                var x = new XBounds();
                x.rowid = i;//SafeConvertAux.SafeConvertToInt32(r.GetValue(0));
                var wkb = dkShp.GetWKB(i, false);
                var e = WKBHelper.fromWKB(wkb).EnvelopeInternal;
                x.minx = e.MinX;// icc.toInt(e.MinX);
                //x.maxx = e.MaxX;// icc.toInt(e.MaxX);
                lstXBounds.Add(x);
            }
            lstXBounds.Sort((a, b) =>
            {
                return a.minx < b.minx ? -1 : 1;
                //if(a.minx<b.minx)
                //    return -1;
                //if(a.minx>b.minx)
                //    return 1;
                //if (a.maxx == b.maxx)
                //    return 0;
                //return a.maxx < b.maxx ? -1 : 1;
            });

            int nTestMaxCacheSize = 0;

            var cacheRowids = new List<int>();
            Dictionary<int, ShortZd_cbd> dicCbd = new Dictionary<int, ShortZd_cbd>();
            ShortZd_cbd preCbd = null;
            for (int i = 0; i < lstXBounds.Count;)
            //for (int i = 2027447-1000; i < lstXBounds.Count; )
            {
                int j = i + 100;
                //bool fLast = false;
                if (j >= lstXBounds.Count)
                {
                    j = lstXBounds.Count - 1;
                    //fLast = true;
                }
                for (int k = i; k <= j; ++k)
                {
                    cacheRowids.Add(lstXBounds[k].rowid);
                }
                InitLandDotCoilUtil.QueryShortZd_cbd(dkShp, cacheRowids, dicCbd, en =>
                {
                    en.qlrMc = OnQueryCbdQlr(en);
                    _jzdCache.AcquireJzd(en);
                });
                for (int k = i; k <= j; ++k)
                {
                    var xb = lstXBounds[k];
                    ShortZd_cbd cbd;
                    if (dicCbd.TryGetValue(xb.rowid, out cbd))
                    {
                        //try
                        //{
                        var fProcessed = processLeft(cbd, preCbd);//, !fLast);
                        if (fProcessed)
                        {
                            preCbd = cbd;
                        }
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
            processLeft(null, null);//,false);

            ReportProgress("保存缓存数据", 100);
            _saveCache.Flush(true);
            //bool fSleep = lstXBounds.Count > 500000;
            lstXBounds.Clear();


            _jzdTable.testLogout();


            Console.WriteLine("共发现插入点" + _nInsertedJzdCount + "个");
            Console.WriteLine("包含一个以上毗邻地权利人的界址线个数是：" + _nGreat1QlrJzxCount);
            Console.WriteLine("nTestMaxCacheSize=" + nTestMaxCacheSize);
            Console.WriteLine("缓存界址点的最大数量为：" + Math.Max(_jzdCache.Count, _nTestMaxCacheJzdCount));


            ReportInfomation("生成界址点共计：" + _jzdTable.ExportedCount + "个");
            ReportInfomation("生成界址线共计：" + _jzdTable.GetExportedJzxCount() + "条");
            Clear();
            ReportInfomation("结束时间：" + DateTime.Now);
            //System.Threading.Thread.Sleep(1000);
        }
        private void Clear()
        {
            _jzdCache.Clear();
            //_lstSaveCache.Clear();
            _jzdTable.Clear();
            _cbdCache.Clear();
            _saveCache.Clear();
        }
        private bool processLeft(ShortZd_cbd current, ShortZd_cbd preCbd)//,bool fReleaseCache)
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
            var lstSortedJzx = new List<Jzx>();
            buildSortedJzxList(left, lstSortedJzx);

            if (_param.fSplitLine)
            {
                var tolerance = _param.LineOverlapTolerance;
                var tolerance2 = tolerance * tolerance;

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
                        var qJzd = jzx.minYPoint;//.qJzd;// lstJzx.GetFromCoord(jzx);


                        if (pt.Y < qJzd.Y - tolerance)
                        {
                            //++j;
                            //fBreak = true;
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

                                //if (InitLandDotCoilUtil.testIsEqual(p1, 552358.7197840896, 3906931.1383719193))
                                //{
                                //    Console.WriteLine(pt.ToString());
                                //}
                                if (!Overlaps(jzx.lstInsertJzd, p1))
                                {
                                    jzx.lstInsertJzd.Add(p1);
                                    ++_nInsertedJzdCount;
                                }
                            }

                        }
                    }
                }
                //if(false)
                for (int i = 0; i < left.Count; ++i)
                {
                    var cbd = left[i];
                    //if (!cbd.fSelected)//!isSelected(cbd)) //yxm 
                    //    continue;
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
                        _jzdCache.ReAcquireJzd(cbd);
                        //var g = cbd.MakePolygon();
                        //_db.UpdateShape(Zd_cbdFields.TABLE_NAME, Zd_cbdFields.Shape, "rowid=" + cbd.rowid, g);
                    }
                }
                #endregion
            }


            #region 查找毗邻地权利人
            buildSortedJzxList(left, lstSortedJzx);
            int nTestJzxCount = lstSortedJzx.Count;
            for (int i = lstSortedJzx.Count - 1; i >= 0; --i)
            {
                var jzx = lstSortedJzx[i];

                #region 查找相邻的地块（外接矩形在1.5米范围内相交的地块）
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

                var twin = _jzdCache.findTwin(jzx);
                if (twin != null)
                {
                    if (jzx.lstQlr == null)
                    {
                        jzx.lstQlr = new List<ShortZd_cbd>();
                    }
                    jzx.lstQlr.Clear();
                    jzx.lstQlr.Add(twin.dk);

                    //System.Diagnostics.Debug.Assert(jzx != twin);
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

                if (_param.fUserBufferToFindPldwqlr)
                {
                    var fSelected = jzx.dk.fSelected;// isSelected(jzx.dk);
                    var buffer = InitLandDotCoilUtil.BufferLeft(jzx.qJzd, jzx.zJzd, _param.AddressLinedbiDistance);
                    var env = buffer.EnvelopeInternal;
                    for (int j = lstSortedJzx.Count - 1; j >= 0; --j)
                    {
                        if (j == i)
                            continue;
                        var jzxJ = lstSortedJzx[j];
                        if (jzxJ.maxYPoint.Y + _param.AddressLinedbiDistance < jzx.minYPoint.Y)
                        {
                            Console.WriteLine("err");
                            System.Diagnostics.Debug.Assert(false);//在查找相邻的地块处已经处理了，应该不会走到这里
                            lstSortedJzx.RemoveAt(j);
                            continue;
                        }
                        if (jzx.maxYPoint.Y + _param.AddressLinedbiDistance < jzxJ.minYPoint.Y)
                        {
                            break;
                        }
                        //if (!fSelected)//yxm
                        //{
                        //    continue;
                        //}
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
                }
                if (jzx.lstQlr != null && jzx.lstQlr.Count > 1)
                {
                    ++_nGreat1QlrJzxCount;
                }
            }
            #endregion

            #region 将_cbdCache中所有在当前地块最左边1.5米前完全出现的地块加入到保存缓存中并从_cbdCache中移除
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
                    //if (cbd.fSelected) //yxm
                    {
                        _saveCache.Add(cbd);//, fReleaseCache);
                    }
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

        ///// <summary>
        ///// 保存缓存的地块并清除不再使用的地块和缓存中不再使用的界址点
        ///// </summary>
        ///// <param name="dk"></param>
        //private void saveCache(ShortZd_cbdCache left,bool fLast=false)
        //{
        //    //if (!(fLast||_lstSaveCache.Count > 5000))
        //    //{
        //    //    return;
        //    //}
        //    if (_lstSaveCache.Count == 0)
        //        return;
        //    Console.WriteLine("正在保存缓存：" + _lstSaveCache.Count + "个");
        //    System.Data.SQLite.SQLiteTransaction trans=null;
        //    //try
        //    //{
        //        trans = _db.BeginTransaction();

        //        foreach (var en in _lstSaveCache)
        //        {
        //            saveJzdJzx(en);
        //        }
        //        trans.Commit();
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    trans.Rollback();
        //    //    throw ex;
        //    //}

        //    //foreach (var dk in _lstSaveCache)
        //    //{
        //    //    dk.Clear();
        //    //}
        //    _lstSaveCache.Clear();
        //    if (!fLast)
        //    {
        //        #region 清除所有不再使用的界址点
        //        RemoveAllNotUsedJzd(left);
        //        if (_nTestMaxCacheJzdCount < _dicJzdOutEdges.Count)
        //        {
        //            _nTestMaxCacheJzdCount = _dicJzdOutEdges.Count;
        //        }
        //        #endregion
        //    }
        //    //InitLandDotCoilUtil.DeleteJzd(_db, JzdFields.DKID + "='" + dk.dkID + "'");
        //    //dk.QueryRing((r, fShell) =>
        //    //{

        //    //});
        //}

        ///// <summary>
        ///// 保存界址点和界址线
        ///// </summary>
        ///// <param name="en"></param>
        //private void saveJzdJzx(ShortZd_cbd en)
        //{
        //    System.Diagnostics.Debug.Assert(en.shell.Count > 0);
        //    var jzdRowids = _jzdTable.GetRowidsByDkid(en.dkID);
        //    var lstJzd = new List<JzdEntity>();
        //    toJzdList(en.shell, true, lstJzd);
        //    Assign(en,jzdRowids, lstJzd, 0);
        //    if (en.holes != null)
        //    {
        //        foreach (var h in en.holes)
        //        {
        //            int iBegin = lstJzd.Count;
        //            toJzdList(h, false, lstJzd);
        //            Assign(en, jzdRowids, lstJzd, iBegin);
        //        }
        //    }
        //    /*
        //    int nKeyJzdCount = 0;//关键界址点的个数
        //    int i = 0;
        //    short nJzdh = 0;//界址点点号
        //    JzdEntity preJzd = lstJzd[lstJzd.Count - 1];
        //    foreach (var jzdEn in lstJzd)
        //    {
        //        jzdEn.dkID = en.dkID;
        //        jzdEn.JZDH = ++nJzdh;
        //        if (jzdRowids!=null&&i < jzdRowids.Count)
        //        {
        //            jzdEn.rowID = jzdRowids[i++];
        //        }
        //        #region 检查是否关键界址点
        //        JzdEdges lst;
        //        if (_dicJzdOutEdges.TryGetValue(jzdEn.shape, out lst))
        //        {                    
        //            if (lst.fKeyJzd == null)
        //            {//判断是否关键界址点
        //                if (lst.Count >= 3)
        //                {
        //                    lst.fKeyJzd = true;
        //                }
        //                else 
        //                {
        //                    if (_param.MinAngleFileter != null)
        //                    {
        //                        if (lst.Count == 1)
        //                        {
        //                            var angle = CglHelper.CalcAngle(jzdEn.shape, preJzd.shape, lst[0].zJzd);
        //                            if (isKeyAngle(angle))
        //                            {
        //                                lst.fKeyJzd = true;
        //                            }
        //                        }else if (lst.Count == 2)
        //                        {
        //                            var fEqual = CglHelper.isSamePoint(preJzd.shape, lst[0].zJzd, _param.Tolerance);
        //                            var angle = CglHelper.CalcAngle(jzdEn.shape, preJzd.shape,fEqual?lst[1].zJzd:lst[0].zJzd);
        //                            if (isKeyAngle(angle))
        //                            {
        //                                lst.fKeyJzd = true;
        //                            }
        //                            else if (!fEqual)
        //                            {
        //                                fEqual = CglHelper.isSamePoint(preJzd.shape, lst[1].zJzd, _param.Tolerance);
        //                                if (!fEqual)
        //                                {
        //                                    angle = CglHelper.CalcAngle(jzdEn.shape, lst[0].zJzd, lst[1].zJzd);
        //                                    if (isKeyAngle(angle))
        //                                    {
        //                                        lst.fKeyJzd = true;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            if (lst.fKeyJzd == null)
        //            {
        //                lst.fKeyJzd = false;
        //            }
        //            jzdEn.SFKY = (bool)lst.fKeyJzd;
        //            nKeyJzdCount += jzdEn.SFKY ? 1 : 0;
        //        }
        //        preJzd = jzdEn;
        //        #endregion
        //    }

        //    #region 若关键界址点个数小于4（或3，具体值由参数确定）
        //    if (nKeyJzdCount < _param.MinKeyJzdCount)
        //    {
        //        if (lstJzd.Count <= _param.MinKeyJzdCount)
        //        {
        //            foreach (var jzdEn in lstJzd)
        //            {
        //                jzdEn.SFKY = true;
        //            }
        //        }
        //        else
        //        {
        //            if (nKeyJzdCount == 0)
        //            {
        //                lstJzd[0].SFKY = true;
        //            }
        //            var dicLen2 = new Dictionary<Coordinate, double>();

        //            var ll = new List<List<Coordinate>>();
        //        }
        //    }
        //    #endregion
        //    */
        //    if (jzdRowids != null)
        //    {
        //        for (int i=lstJzd.Count; i < jzdRowids.Count; ++i)
        //        {
        //            _jzdTable._delRowids.Add(jzdRowids[i]);
        //        }
        //    }
        //    _jzdTable.SaveJzd(_db, _param, lstJzd,_srid);
        //}


        ///// <summary>
        ///// 是否是被选中需要生成界址点和界址线的地块
        ///// </summary>
        ///// <param name="cbd"></param>
        ///// <returns></returns>
        //private bool isSelected(ShortZd_cbd cbd)
        //{
        //    return _rowids.Contains(cbd.rowid);
        //}

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
    /// 初始化界址点界址线的参数类
    /// </summary>
    public class InitLandDotCoilParam
    {
        /// <summary>
        /// 领宗地距离，单位：米
        /// </summary>
        public double AddressLinedbiDistance = 1.5;

        /// <summary>
        /// 点重叠容差
        /// </summary>
        internal readonly double Tolerance = 0.05;//视为同一个点的距离容差，单位：米
        internal readonly double Tolerance2;
        /// <summary>
        /// 线重叠容差
        /// </summary>
        internal readonly double LineOverlapTolerance = 0.01;
        internal readonly double LineOverlapTolerance2;
        /// <summary>
        /// 界址标识
        /// </summary>
        public string AddressPointPrefix = "J";
        /// <summary>
        /// 最小过滤角度值，单位：度
        /// </summary>
        public double? MinAngleFileter = 5;

        /// <summary>
        /// 最大过滤角度值，单位：度
        /// </summary>
        public double? MaxAngleFilter = 120;

        /// <summary>
        /// 一个地块包含的最少关键界址点个数
        /// </summary>
        public short MinKeyJzdCount = 4;

        ///// <summary>
        ///// 界址点类型
        ///// </summary>
        //public short AddressDotType=1;

        ///// <summary>
        ///// 界标类型
        ///// </summary>
        //public short AddressDotMarkType=3;

        /// <summary>
        /// 界线性质
        /// </summary>
        public string JXXZ = "600001";

        /// <summary>
        /// 界址线类型
        /// </summary>
        public string JZXLB = "01";

        /// <summary>
        /// 界址线位置
        /// </summary>
        public string AddressLinePosition = "1";

        /// <summary>
        /// 界址线说明填写长度
        /// </summary>
        public bool IsLineDescription = true;

        /// <summary>
        /// 界址线说明
        /// </summary>
        public eLineDescription LineDescription;

        ///// <summary>
        ///// 创建时间
        ///// </summary>
        //public DateTime cjsj;

        /// <summary>
        /// 界址点标识码初始值
        /// </summary>
        public int nJzdBSMStartVal = 50000000;

        /// <summary>
        /// 界址线标识码初始值
        /// </summary>
        public int nJzxBSMStartVal = 20000000;

        /// <summary>
        /// 界址点要素代码
        /// </summary>
        public string sJzdYSDMVal = "211021";

        /// <summary>
        /// 界址线要素代码
        /// </summary>
        public string sJzxYSDMVal = "211031";

        /// <summary>
        /// 界标类型
        /// </summary>
        public string sJBLXVal = "3";
        /// <summary>
        /// 界址点类型
        /// </summary>
        public string sJZDLXVal = "1";

        /// <summary>
        /// 是否只导出关键界址点
        /// </summary>
        public bool fOnlyExportKeyJzd = true;

        /// <summary>
        /// 是否执行打断线操作
        /// </summary>
        public bool fSplitLine = false;

        /// <summary>
        /// 是否缓冲一定距离查找毗邻地权利人
        /// </summary>
        internal bool fUserBufferToFindPldwqlr = false;

        public string shapefilePath;

        public string prjStr;

        public string extName;

        public InitLandDotCoilParam(double tolerance = 0.05, double lineOverlapTolerance = 0.01)
        {
            Tolerance = tolerance;
            Tolerance2 = tolerance * tolerance;
            LineOverlapTolerance = lineOverlapTolerance;
            LineOverlapTolerance2 = lineOverlapTolerance * lineOverlapTolerance;
        }
    }
    public class JzdEdge
    {
        /// <summary>
        /// 出度
        /// </summary>
        public Jzx OutEdge;
        /// <summary>
        /// 入度
        /// </summary>
        public Jzx InEdge;
        public ShortZd_cbd dk { get { return OutEdge.dk; } }
    }
    public class JzdEdges : List<JzdEdge>
    {
        /// <summary>
        /// 是否关键界址点 
        /// </summary>
        public bool? fKeyJzd = null;
        public bool fHasExported
        {
            get
            {
                return Jzdh != null;
            }
        }
        public List<Coordinate[]> lstJzxEntities = null;
        /// <summary>
        /// 界址点号
        /// </summary>
        public string Jzdh;
    }
    /*
    /// <summary>
    /// 用整数表示坐标值
    /// </summary>
    public class IntCoordConter
    {
        private double _minValue;
        public void Init(double minValue)
        {
            _minValue = minValue;
        }
        public int toInt(double value)
        {
            return (int)((value - _minValue) * 10000);
        }
        public double toDouble(int value)
        {
            return value / 10000.0 + _minValue;
        }
    }
    */
    /// <summary>
    /// 记录每个地块的外切横坐标
    /// </summary>
    public struct XBounds
    {
        public int rowid;
        public double minx;
        //public double maxx;
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
                return true;// (_bit & (1 << 1)) != 0;
            }
            //set
            //{
            //    if (value)
            //    {
            //        _bit |= 1<<1;
            //    }
            //    else
            //    {
            //        _bit &= ~(1<<1);
            //    }
            //}
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

        //public bool fHaveAcquireJzd
        //{
        //    get
        //    {
        //        return (_bit & (1 << 3)) != 0;
        //    }
        //    set
        //    {
        //        if (value)
        //        {
        //            _bit |= 1 << 3;
        //        }
        //        else
        //        {
        //            _bit &= ~(1 << 3);
        //        }
        //    }
        //}

        public int rowid;
        public double xmin;
        public double xmax;
        //public double ymin;
        //public double ymax;
        /// <summary>
        /// 权利人名称
        /// </summary>
        internal string qlrMc;
        /// <summary>
        /// 指界人名称
        /// </summary>
        public string zjrMc;
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

        /// <summary>
        /// 遍历环
        /// </summary>
        /// <param name="callback">【环，是否shell】</param>
        public void QueryRing(Action<JzxRing, bool> callback)
        {
            callback(shell, true);
            if (holes != null)
            {
                foreach (var h in holes)
                {
                    callback(h, false);
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
                System.Diagnostics.Debug.Assert(fSaved == true);
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
                    //foreach (var p in jzx.lstInsertJzd)
                    //{
                    //    lstInsertedJzd.Add(p);
                    //}
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

    /// <summary>
    /// 承包地缓存
    /// </summary>
    public class ShortZd_cbdCache : List<ShortZd_cbd>
    {
        public double x1;
        //private readonly Dictionary<int, ShortZd_cbd> _dic = new Dictionary<int, ShortZd_cbd>();
        //public ShortZd_cbd Get(int rowid)
        //{
        //    ShortZd_cbd en;
        //    if (_dic.TryGetValue(rowid, out en))
        //    {
        //        return en;
        //    }
        //    return null;
        //}
        //public void Clear()
        //{
        //    _dic.Clear();
        //}
    }



    /// <summary>
    /// //按Y轴优先排序
    /// </summary>
    public class JzdComparer : Comparer<Coordinate>
    {
        private double _tolerace;
        public JzdComparer(double tolerace)
        {
            _tolerace = tolerace;
        }
        public override int Compare(Coordinate a, Coordinate b)
        {
            if (a.Y + _tolerace < b.Y)
                return -1;
            if (b.Y + _tolerace < a.Y)
                return 1;
            if (a.X + _tolerace < b.X)
                return -1;
            if (b.X + _tolerace < a.X)
                return 1;
            return 0;
        }
    }

    /// <summary>
    /// 点相等的比较
    /// </summary>
    public class JzdEqualComparer : IEqualityComparer<Coordinate>
    {
        private double _tolerace, _tolerace2;
        private Coordinate _tmpC = new Coordinate();
        public JzdEqualComparer(double tolerance)
        {
            _tolerace = tolerance;
            _tolerace2 = tolerance * tolerance;
        }
        public bool Equals(Coordinate a, Coordinate b)
        {
            return CglHelper.IsSame2(a, b, _tolerace2);
            //if (a.Y + _tolerace < b.Y || b.Y + _tolerace < a.Y
            //    ||a.X + _tolerace < b.X||b.X + _tolerace < a.X)
            //    return false;
            //var dx = a.X - b.X;
            //var dy = a.Y - b.Y;
            //var d2 = dx * dx + dy * dy;
            //return d2 < _tolerace2;
        }

        public int GetHashCode(Coordinate obj)
        {
            _tmpC.X = func(obj.X);// Math.Round(obj.X, 3);
            _tmpC.Y = func(obj.Y);// Math.Round(obj.Y, 3);
            return _tmpC.GetHashCode();
        }
        private static double func(double x)
        {
            long n = (long)(x * 1);
            return n / 1.0;
        }
    }
    public class Jzx
    {
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
        public bool fQlrFind = false;
        public bool fInserted = false;
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
        public string GetQlr()
        {
            string s = "";
            if (lstQlr != null)
            {
                foreach (var q in lstQlr)
                {
                    if (!string.IsNullOrEmpty(s))
                        s += ";";
                    s += q.qlrMc;
                }
            }
            return s;
        }
        /// <summary>
        /// 保存从要打断的界址点ID
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
            //var l = Clone();
            //l.lstInsertJzd.Clear();
            lstInsertJzd.Add(zJzd);
            var preJzd = qJzd;
            for (int i = 0; i < lstInsertJzd.Count; ++i)
            {
                var p1 = lstInsertJzd[i];
                var jzx = new Jzx(preJzd, p1, dk, isShell);
                if (i > 0)
                {
                    jzx.fInserted = true;
                }
                lst.Add(jzx);
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
        //public Jzx Clone()
        //{
        //    var j = new Jzx(qJzd,zJzd,dk,isShell);
        //    foreach (var s in lstQlr)
        //    {
        //        j.lstQlr.Add(s);
        //    }
        //    foreach (var c in lstInsertJzd)
        //    {
        //        j.lstInsertJzd.Add(c);
        //    }
        //    return j;
        //}
    }

    //public class JzdOutEdge
    //{
    //    public readonly Coordinate qJzd;
    //    public readonly Coordinate zJzd;
    //    public readonly double Length2;
    //    public JzdOutEdge(Coordinate f, Coordinate t)
    //    {
    //        qJzd = f;
    //        zJzd = t;
    //        Length2 = CglHelper.GetDistance2(f, t);
    //    }
    //}
}
