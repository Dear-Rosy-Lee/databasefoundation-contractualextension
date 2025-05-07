using GeoAPI.Geometries;

/*
 * (C) 2016 鱼鳞图公司版权所有，保留所有权利
 * http://www.yulintu.com
 *
 * CLR 版本：   4.0.30319.34014            最低的 Framework 版本：4.0
 * 文 件 名：   ExportJzdx
 * 创 建 人：   颜学铭
 * 创建时间：   2016/9/23 10:18:46
 * 版    本：   1.0.0
 * 备注描述：
 * 修订历史：
*/

using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.NetAux;
using YuLinTu.tGISCNet;

namespace YuLinTu.Library.Aux
{
    /// <summary>
    /// 导出界址点和界址线
    /// </summary>
    public class ExportJzdx
    {
        private class Util
        {
            public static List<IndexItem> QuerySortedJzdIndexItems(DBSpatialite db, HashSet<string> excludeDkbm, string where = null)
            {
                var lst = new List<IndexItem>();
                var sql = "select rowid,AsBinary(shape)," + JzdFields.DKBM + " from " + JzdFields.TABLE_NAME;
                if (!string.IsNullOrEmpty(where))
                {
                    sql += " where " + where;
                }
                db.QueryCallback(sql, r =>
                {
                    var dkbm = SqlUtil.GetString(r, 2);
                    if (!excludeDkbm.Contains(dkbm))
                    {
                        var id = r.GetInt32(0);
                        var g = WKBHelper.fromWKB(r.GetValue(1) as byte[]) as IPoint;
                        if (g != null)
                        {
                            lst.Add(new IndexItem()
                            {
                                rowid = id,
                                intMinX = g.X// (int)(g.X * 10)
                            });
                        }
                    }
                    return true;
                });

                lst.Sort((a, b) =>
                {
                    return a.intMinX == b.intMinX ? 0 : a.intMinX < b.intMinX ? -1 : 1;
                });
                return lst;
            }

            public static List<IndexItem> QuerySortedJzxIndexItems(DBSpatialite db, HashSet<string> excludeDkbm, Action<string> reportWarn, string where = null)
            {
                var lst = new List<IndexItem>();
                var sql = "select rowid,AsBinary(shape)," + JzdFields.DKBM + " from " + JzxFields.TABLE_NAME;
                if (!string.IsNullOrEmpty(where))
                {
                    sql += " where " + where;
                }
                db.QueryCallback(sql, r =>
                {
                    var dkbm = SqlUtil.GetString(r, 2);
                    try
                    {
                        if (!excludeDkbm.Contains(dkbm))
                        {
                            var id = r.GetInt32(0);
                            var g = WKBHelper.fromWKB(r.GetValue(1) as byte[]) as ILineString;
                            if (g != null)
                            {
                                var minx = g.StartPoint.X < g.EndPoint.X ? g.StartPoint.X : g.EndPoint.X;
                                lst.Add(new IndexItem()
                                {
                                    rowid = id,
                                    intMinX = minx// (int)(minx * 10)
                                });
                            }
                        }
                    }
                    catch
                    {
                        reportWarn("地块编码为:" + dkbm + "数据有误，请检查");
                        return true;
                    }
                    return true;
                });

                lst.Sort((a, b) =>
                {
                    return a.intMinX == b.intMinX ? (a.rowid < b.rowid ? -1 : 1) : a.intMinX < b.intMinX ? -1 : 1;
                });
                return lst;
            }

            public static List<IndexItem> QuerySortedCbdIndexItems(DBSpatialite db, HashSet<string> excludeDkbm, Action<string> reportWarning, string where = null)
            {
                var lst = new List<IndexItem>();
                var sql = "select rowid,AsBinary(shape)," + Zd_cbdFields.DKBM + " from " + Zd_cbdFields.TABLE_NAME
                    + " where shape is not null";
                if (!string.IsNullOrEmpty(where))
                {
                    sql += " and(" + where + ")";
                }

                db.QueryCallback(sql, r =>
                {
                    var id = r.GetInt32(0);
                    if (r.IsDBNull(2))
                    {
                        reportWarning(string.Format("rowid={0}的地块编码为null，请检查！", id));
                        return true;
                    }
                    var dkbm = SqlUtil.GetString(r, 2);
                    try
                    {
                        if (!excludeDkbm.Contains(dkbm))
                        {
                            var g = WKBHelper.fromWKB(r.GetValue(1) as byte[]);
                            if (g != null)
                            {
                                var minx = g.EnvelopeInternal.MinX;// g.StartPoint.X < g.EndPoint.X ? g.StartPoint.X : g.EndPoint.X;
                                lst.Add(new IndexItem()
                                {
                                    rowid = id,
                                    intMinX = minx// (int)(minx * 10)
                                });
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine(ee.ToString());
                        reportWarning("地块编码为" + dkbm + "数据有误，请检查！");
                        return true;
                    }
                    return true;
                });

                lst.Sort((a, b) =>
                {
                    return a.intMinX == b.intMinX ? 0 : a.intMinX < b.intMinX ? -1 : 1;
                });
                return lst;
            }

            /// <summary>
            /// 根据索引lst中的rowid按每次最多100条读取界址点数据并存入jzds集合
            /// 每读完一块数据后进行回调；返回jzds集合缓存个数的峰值；
            /// </summary>
            /// <param name="db"></param>
            /// <param name="lst"></param>
            /// <param name="jzds"></param>
            /// <param name="callback"></param>
            /// <returns></returns>
            public static int QueryJzdEntities(DBSpatialite db, List<IndexItem> lst,
                Dictionary<IPoint, ShapeJzdEntity> jzds, Action<ShapeJzdEntity> callback)
            {
                int nMaxCacheCount = 0;

                for (int i = 0; i < lst.Count;)
                {
                    int j = i + 200;
                    if (j > lst.Count)
                    {
                        j = lst.Count;
                    }
                    string sin = lst[i].rowid.ToString();
                    for (int k = i + 1; k < j; ++k)
                    {
                        sin += "," + lst[k].rowid;
                    }
                    var lastJzdEn = Util.queryJzdEntities(db, "rowid in(" + sin + ")", jzds);
                    if (lastJzdEn != null)
                    {
                        if (nMaxCacheCount < jzds.Count)
                        {
                            nMaxCacheCount = jzds.Count;
                        }
                        callback(lastJzdEn);
                    }
                    i = j;
                }
                return nMaxCacheCount;
            }

            /// <summary>
            /// 从索引index的iBegin位置开始查询一组界址线，该组界址线中至少有一条记录的
            /// 最左边的端点的x值大于minx或者查询到索引的结束位置；
            /// 返回下一次查询的位置；结果集已经按从左到右从下到上的顺序排序
            /// </summary>
            /// <param name="db"></param>
            /// <param name="index"></param>
            /// <param name="iBegin"></param>
            /// <param name="x"></param>
            /// <param name="cache"></param>
            /// <returns></returns>
            public static int QueryJzxEntities(DBSpatialite db, List<IndexItem> index, int iBegin, double minx, ListShapeJzxEntity cache)
            {
                for (; iBegin < index.Count;)
                {
                    int j = iBegin + 200;
                    if (j > index.Count)
                    {
                        j = index.Count;
                    }
                    string sin = index[iBegin].rowid.ToString();
                    for (int k = iBegin + 1; k < j; ++k)
                    {
                        sin += "," + index[k].rowid;
                    }
                    iBegin = j;

                    var rightJzx = queryJzxEntities(db, "rowid in(" + sin + ")", cache);
                    if (rightJzx != null && rightJzx.minX > minx)
                    {
                        break;
                    }
                }

                #region 按从左到右从下到上的顺序排序

                cache.Sort((a, b) =>
                {
                    var aStartPoint = a.StartPoint;
                    var bStartPoint = b.StartPoint;
                    if (aStartPoint.X < bStartPoint.X)
                        return -1;
                    if (aStartPoint.X > bStartPoint.X)
                        return 1;
                    if (aStartPoint.Y < bStartPoint.Y)
                        return -1;
                    if (aStartPoint.Y > bStartPoint.Y)
                        return 1;
                    var aEndPoint = a.EndPoint;
                    var bEndPoint = b.EndPoint;
                    if (aEndPoint.X < bEndPoint.X)
                        return -1;
                    if (aEndPoint.X > bEndPoint.X)
                        return 1;
                    if (aEndPoint.Y < bEndPoint.Y)
                        return -1;
                    if (aEndPoint.Y > bEndPoint.Y)
                        return 1;
                    return 0;
                });

                #endregion 按从左到右从下到上的顺序排序

                return iBegin;
            }

            /// <summary>
            /// 从索引index的iBegin位置开始查询一组地块，该组界址线中至少有一条记录的
            /// 最左边的端点的x值大于minx或者查询到索引的结束位置；
            /// 返回下一次查询的位置；
            /// 结果集已经按从左到右从下到上的顺序排序
            /// </summary>
            /// <param name="db"></param>
            /// <param name="index"></param>
            /// <param name="iBegin"></param>
            /// <param name="x"></param>
            /// <param name="cache"></param>
            /// <returns></returns>
            [Obsolete]
            public static int QueryDkEntities(DBSpatialite db, List<IndexItem> index, int iBegin, double minx
                , List<ShapeDkEntity> cache, JzdEqualComparer cmp, string syqxz)
            {
                for (; iBegin < index.Count;)
                {
                    int j = iBegin + 200;
                    if (j > index.Count)
                    {
                        j = index.Count;
                    }
                    string sin = index[iBegin].rowid.ToString();
                    for (int k = iBegin + 1; k < j; ++k)
                    {
                        sin += "," + index[k].rowid;
                    }
                    iBegin = j;

                    ShapeDkEntity rightDk = null;
                    InitLandDotCoilUtil.QueryShapeDkEntities(db, "rowid in(" + sin + ")", cache,
                        en =>
                         {
                             en.SYQXZ = syqxz;
                             if (rightDk == null || rightDk.env.MinX < en.env.MinX)
                             {
                                 rightDk = en;
                             }
                             en.dicJzdh = new Dictionary<Coordinate, string>(cmp);
                             foreach (var c in en.coords)
                             {
                                 en.dicJzdh[c] = null;
                             }
                         });
                    if (rightDk != null && rightDk.env.MinX > minx)
                    {
                        break;
                    }
                }

                #region 按从左到右从下到上的顺序排序

                cache.Sort((a, b) =>
                {
                    if (a.env.MinX < b.env.MinX)
                        return -1;
                    if (a.env.MinX > b.env.MinX)
                        return 1;
                    if (a.env.MinY < b.env.MinY)
                        return -1;
                    if (a.env.MinY > b.env.MinY)
                        return 1;
                    if (a.env.MaxX < b.env.MaxX)
                        return -1;
                    if (a.env.MaxX > b.env.MaxX)
                        return 1;
                    if (a.env.MaxY < b.env.MaxY)
                        return -1;
                    if (a.env.MaxY > b.env.MaxY)
                        return 1;
                    return 0;
                });

                #endregion 按从左到右从下到上的顺序排序

                return iBegin;
            }

            /// <summary>
            /// 按条件查询界址点并构造为ShapeJzdEntity实例存入lst中；
            /// 返回本次查询最右边的一个实例
            /// </summary>
            /// <param name="db"></param>
            /// <param name="where"></param>
            /// <param name="lst"></param>
            /// <returns></returns>
            private static ShapeJzdEntity queryJzdEntities(DBSpatialite db, string where, Dictionary<IPoint, ShapeJzdEntity> lst)
            {
                ShapeJzdEntity lastJzdEn = null;
                var sql = "select " + JzdFields.JZDLX + "," + JzdFields.JBLX + "," + JzdFields.DKBM
                    + ",AsBinary(" + JzdFields.SHAPE + ")," + JzdFields.SFKY + "," + JzdFields.JZDH + "," + JzdFields.TBJZDH + " from " + JzdFields.TABLE_NAME
                    + " where " + where + " and " + JzdFields.DKBM + " is not null and " + JzdFields.SHAPE + " is not null";
                db.QueryCallback(sql, r =>
                {
                    var dkbm = SqlUtil.GetString(r, 2);
                    var wkb = r.GetValue(3) as byte[];

                    var g = WKBHelper.fromWKB(wkb) as IPoint;
                    ShapeJzdEntity en;
                    if (!lst.TryGetValue(g, out en))
                    {
                        en = new ShapeJzdEntity();
                        lst[g] = en;
                        en.dkbm = dkbm;
                        en.pt = g;
                        en.wkb = wkb;
                    }
                    else
                    {
                        if (!en.dkbm.Contains(dkbm))
                        {
                            en.dkbm += "/" + dkbm;
                        }
                    }

                    en.jzdlx = SqlUtil.GetString(r, 0);
                    en.jblx = SqlUtil.GetString(r, 1);
                    en.fKeyJzd = r.GetBoolean(4);
                    if (!r.IsDBNull(5))
                    {
                        var s = r.GetString(5);
                        if (s.Length > 0)
                        {
                            en.jzdhPrefix = s[0].ToString();
                        }
                    }
                    if (!r.IsDBNull(6))
                    {
                        var s = r.GetString(6);
                        if (s.Length > 0)
                        {
                            en.jzdtbh = s;
                        }
                    }
                    if (lastJzdEn == null || lastJzdEn.pt.X < en.pt.X)
                    {
                        lastJzdEn = en;
                    }
                    return true;
                });
                return lastJzdEn;
            }

            private static ShapeJzxEntity queryJzxEntities(DBSpatialite db, string where, ListShapeJzxEntity lst)
            {
                ShapeJzxEntity lastJzdEn = null;

                var sql = "select " + JzxFields.JXXZ + "," + JzxFields.JZXLB + "," + JzxFields.JZXWZ
                    + "," + JzxFields.JZXSM + "," + JzxFields.PLDWQLR + "," + JzxFields.PLDWZJR
                    + "," + JzxFields.DKBM + ",AsBinary(" + JzxFields.SHAPE + "),rowid from " + JzxFields.TABLE_NAME
                    + " where " + where;
                db.QueryCallback(sql, r =>
                {
                    var en = new ShapeJzxEntity();
                    lst.Add(en);
                    en.JXXZ = SqlUtil.GetString(r, 0);
                    en.JZXLB = SqlUtil.GetString(r, 1);
                    //en.JZXWZ = SqlUtil.GetString(r, 2);
                    en.JZXSM = SqlUtil.GetString(r, 3);
                    string position = SqlUtil.GetString(r, 2);
                    if (!string.IsNullOrEmpty(position))
                    {
                        en.JZXWZ = position.Substring(position.Length - 1, 1);
                    }
                    if (!r.IsDBNull(4))
                    {
                        en.PLDWQLR.Add(r.GetString(4));
                    }
                    if (!r.IsDBNull(5))
                    {
                        en.PLDWZJR.Add(r.GetString(5));
                    }
                    if (!r.IsDBNull(6))
                    {
                        en.DKBM.Add(r.GetString(6));
                    }
                    en.wkb = r.GetValue(7) as byte[];
                    en.coords = WKBHelper.fromWKB(en.wkb).Coordinates;// as ILineString;
                    if (en.coords[0].X > en.coords[en.coords.Length - 1].X)
                    {
                        GeometryHelper.Reverse(en.coords);
                    }
                    en.rowid = (int)r.GetInt64(8);
                    SortCoords(en.coords);

                    if (lastJzdEn == null || lastJzdEn.minX < en.minX)
                    {
                        lastJzdEn = en;
                    }
                    return true;
                });
                return lastJzdEn;
            }

            /// <summary>
            /// 如果序列中起点和终点的x坐标相等并且起点的Y坐标大于终点的Y坐标则反序排列
            /// </summary>
            /// <param name="sa"></param>
            private static void SortCoords(Coordinate[] sa)
            {
                var p0 = sa[0];
                var p1 = sa[sa.Length - 1];
                if (p0.X == p1.X && p0.Y > p1.Y)
                {
                    int m = sa.Length / 2;
                    for (int i = 0; i < m; ++i)
                    {
                        var p = sa[i];
                        sa[i] = sa[sa.Length - i - 1];
                        sa[sa.Length - i - 1] = p;
                    }
                }
            }
        }

        private class ShapeJzdEntity
        {
            /// <summary>
            /// 界址点类型
            /// </summary>
            public string jzdlx;

            /// <summary>
            /// 界标类型
            /// </summary>
            public string jblx;

            /// <summary>
            /// 界址点统编号
            /// </summary>
            public string jzdtbh;

            /// <summary>
            /// 地块编码，对多个地块的用“/”分割
            /// </summary>
            public string dkbm;

            /// <summary>
            /// 是否关键界址点
            /// </summary>
            public bool fKeyJzd = false;

            public IPoint pt;
            public byte[] wkb;

            /// <summary>
            /// 界址点号的前缀
            /// </summary>
            internal string jzdhPrefix = "J";
        }

        public class ShapeJzxEntity
        {
            public int rowid;

            /// <summary>
            /// 界线性质
            /// </summary>
            public string JXXZ;

            /// <summary>
            /// 界址线类别
            /// </summary>
            public string JZXLB;

            /// <summary>
            /// 界址线位置
            /// </summary>
            public string JZXWZ;

            /// <summary>
            /// 界址线说明
            /// </summary>
            public string JZXSM;

            /// <summary>
            /// 毗邻地物权利人
            /// </summary>
            public readonly List<string> PLDWQLR = new List<string>();

            /// <summary>
            /// 毗邻地物指界人
            /// </summary>
            public readonly List<string> PLDWZJR = new List<string>();

            ///// <summary>
            ///// 起界址点号
            ///// </summary>
            //public string QJZDH;
            ///// <summary>
            ///// 止界址点号
            ///// </summary>
            //public string ZJZDH;
            /// <summary>
            /// 地块编码
            /// </summary>
            public readonly List<string> DKBM = new List<string>();

            /// <summary>
            /// 起界址点号
            /// </summary>
            public string nQJZDH;//=0;

            /// <summary>
            /// 止界址点号
            /// </summary>
            public string nZJZDH;//=0;

            public string GetPLDWQLR()
            {
                string s = null;
                foreach (var ss in PLDWQLR)
                {
                    if (s == null)
                        s = ss;
                    else
                        s += "/" + ss;
                }
                return s;
            }

            public string GetPLDWZJR()
            {
                string s = null;
                foreach (var ss in PLDWZJR)
                {
                    if (s == null)
                        s = ss;
                    else
                        s += "/" + ss;
                }
                return s;
            }

            public string GetDKBM()
            {
                string s = null;
                foreach (var ss in DKBM)
                {
                    if (s == null)
                        s = ss;
                    else
                        s += "/" + ss;
                }
                return s;
            }

            /// <summary>
            /// 约定：起点的x始终<=终点的x坐标
            /// </summary>
            public Coordinate[] coords;

            public byte[] wkb;

            public double minX { get { return coords[0].X; } }

            public Coordinate StartPoint
            {
                get { return coords[0]; }
            }

            public Coordinate EndPoint
            {
                get { return coords[coords.Length - 1]; }
            }
        }

        public class ShapeDkEntity
        {
            public string DKBM;
            public string DKMC;

            /// <summary>
            /// 使用权性质 ?
            /// </summary>
            public string SYQXZ;

            public string DKLB;
            public string TDLYLX;
            public string DLDJ;
            public string TDYT;
            public string SFJBNT;

            /// <summary>
            /// 实测面积：平方米
            /// </summary>
            public double SCMJ
            {
                get; set;
            }

            public string DKDZ;
            public string DKXZ;
            public string DKNZ;
            public string DKBZ;
            public string DKBZXX;
            public string ZJRXM;

            public string KJZB
            {
                get
                {
                    string s = null;
                    for (int i = 0; i < coords.Length - 1; ++i)
                    {
                        var c = coords[i];
                        string jzdh;
                        if (dicJzdh.TryGetValue(c, out jzdh))
                        {
                            if (!string.IsNullOrEmpty(jzdh))
                            {
                                if (s == null)
                                {
                                    s = jzdh;
                                }
                                else
                                {
                                    if (s.Length + ("/" + jzdh).Length > 254)
                                        break;
                                    s += "/" + jzdh;
                                }
                            }
                        }
                    }
                    return s;
                }
            }

            /// <summary>
            /// 实测面积：亩
            /// </summary>
            public double SCMJM;

            public byte[] wkb;
            public Coordinate[] coords;
            public IEnvelope env;
            public Dictionary<Coordinate, string> dicJzdh = null;// new Dictionary<Coordinate, string>();
        }

        private class ListShapeJzxEntity : List<ShapeJzxEntity>
        {
            //public JzxEntity rightJzx;
            public void AssignJzdh(IPoint pt, string nJzdh, double tolerance, double tolerance2)
            {
                foreach (var jzxEn in this as List<ShapeJzxEntity>)
                {
                    if (pt.X + tolerance < jzxEn.StartPoint.X)
                        break;
                    if (jzxEn.nQJZDH == null
                        && CglHelper.IsSame2(pt.X, pt.Y, jzxEn.StartPoint.X, jzxEn.StartPoint.Y, tolerance2))
                    {
                        jzxEn.nQJZDH = nJzdh;
                    }
                    if (jzxEn.nZJZDH == null
                        && CglHelper.IsSame2(pt.X, pt.Y, jzxEn.EndPoint.X, jzxEn.EndPoint.Y, tolerance2))
                    {
                        jzxEn.nZJZDH = nJzdh;
                    }
                }
            }
        }

        private class ListShapeDkEntity : List<ShapeDkEntity>
        {
            public void AssignJzdh(IPoint pt, string jzdh, double tolerance, double tolerance2)
            {
                var c = new Coordinate(pt.X, pt.Y);
                foreach (var en in this as List<ShapeDkEntity>)
                {
                    if (pt.X + tolerance < en.env.MinX)
                        break;
                    //if (pt.X > en.env.MaxX + tolerance
                    //    ||pt.Y+tolerance<en.env.MinY
                    //    ||pt.Y>en.env.MaxY+tolerance)
                    //    continue;

                    if (en.dicJzdh.ContainsKey(c))
                    {
                        en.dicJzdh[c] = jzdh;// "J" + nJzdh;
                    }
                }
            }
        }

        private class JzdExporter
        {
            private readonly ExportJzdx _p;
            private int _nCurrShapeID;
            private int _nStartBSM;

            //#region 界址点输出名称规则：_shpFilePath+"JZD"+_dydm+".shp"
            //private string _shpFilePath;
            //private string _dydm;
            //#endregion

            public JzdExporter(ExportJzdx p)
            {
                _p = p;
            }

            public void Reset()
            {
                _nStartBSM = _p._param.nJzdBSMStartVal;
                _nCurrShapeID = 0;
                _shp = createJzdShapeFile(MakeShapeFileName(), _p._param.sESRIPrjStr);
            }

            public void OnEnd()
            {
                if (_shp != null)
                {
                    _shp.Dispose();
                    _shp = null;
                }
            }

            private static ShapeFile createJzdShapeFile(string shpFileName, string sPrjStr)
            {
                var shp = new ShapeFile();
                var err = shp.Create(shpFileName, EShapeType.SHPT_POINT, sPrjStr);
                if (err != null)
                    throw new Exception(err);
                int n = shp.AddField("BSM", DBFFieldType.FTInteger, 10);
                shp.AddField("YSDM", DBFFieldType.FTString, 6);
                shp.AddField("JZDH", DBFFieldType.FTString, 10);
                shp.AddField("JZDLX", DBFFieldType.FTString, 1);
                shp.AddField("JBLX", DBFFieldType.FTString, 1);
                shp.AddField("DKBM", DBFFieldType.FTString, 254);
                shp.AddField("XZBZ", DBFFieldType.FTDouble, 15, 3);
                shp.AddField("YZBZ", DBFFieldType.FTDouble, 15, 3);
                return shp;
            }

            [Obsolete]
            public void export(ShapeJzdEntity jzdEn, ListShapeJzxEntity jzxs, ListShapeDkEntity dks)
            {
                var shp = _shp;
                //int nJzdH = 1 + _nCurrShapeID;
                string jzdh = jzdEn.jzdhPrefix + (1 + _nCurrShapeID);
                if (_p._param.UseUniteNumberExport)
                {
                    jzdh = jzdEn.jzdtbh;
                }
                shp.WriteWKB(-1, jzdEn.wkb);
                shp.WriteFieldInt(_nCurrShapeID, 0, _nStartBSM++);
                shp.WriteFieldString(_nCurrShapeID, 1, _p._param.sJzdYSDMVal);
                shp.WriteFieldString(_nCurrShapeID, 2, jzdh);// "J" + jzdh);
                shp.WriteFieldString(_nCurrShapeID, 3, jzdEn.jzdlx);
                shp.WriteFieldString(_nCurrShapeID, 4, jzdEn.jblx);
                shp.WriteFieldString(_nCurrShapeID, 5, jzdEn.dkbm);
                shp.WriteFieldDouble(_nCurrShapeID, 6, (double)Math.Round((decimal)jzdEn.pt.Y, 3));
                shp.WriteFieldDouble(_nCurrShapeID, 7, (double)Math.Round((decimal)jzdEn.pt.X, 3));
                ++_nCurrShapeID;

                if (jzdEn.fKeyJzd)
                {
                    jzxs.AssignJzdh(jzdEn.pt, jzdh, _p._param.tolerance, _p._param.tolerance2);
                }
                dks.AssignJzdh(jzdEn.pt, jzdh, _p._param.tolerance, _p._param.tolerance2);
                _p.reportProgress(++_nExportCount);
                if (_p._param.JzdMaxRecords > 0)
                {
                    if (_nCurrShapeID >= _p._param.JzdMaxRecords)
                    {
                        _shp.Dispose();
                        _nCurrShapeID = 0;
                        _shp = createJzdShapeFile(MakeShapeFileName(), _p._param.sESRIPrjStr);
                    }
                }
            }

            public int ExportedCount
            {
                get { return _nExportCount; }
            }

            #region yxm 2017/6/10

            /// <summary>
            /// 当前正在导出的是第几个shape文件
            /// </summary>
            private int _nShpFileIndex = 0;

            private int _nExportCount = 0;
            private ShapeFile _shp;

            private string MakeShapeFileName()
            {
                if (_p._param.JzdMaxRecords > 0)
                {
                    ++_nShpFileIndex;
                    //var fileName = _p._shpFilePath + "DK" +
                    var fileName = _p._shpFilePath + "JZD" + _p._countName + "-" + _nShpFileIndex;
                    return fileName;
                }
                return _p._shpFilePath + "JZD" + _p._countName;
            }

            #endregion yxm 2017/6/10
        }

        private class JzxExporter
        {
            private readonly ExportJzdx _p;
            private int _nCurrShapeID;
            private int _nStartBSM;

            public JzxExporter(ExportJzdx p)
            {
                _p = p;
            }

            public void Reset()
            {
                _nStartBSM = _p._param.nJzxBSMStartVal;
                _nCurrShapeID = 0;
                _shp = createJzxShapeFile(MakeShapeFileName(), _p._param.sESRIPrjStr);
            }

            public void OnEnd()
            {
                if (_shp != null)
                {
                    _shp.Dispose();
                    _shp = null;
                }
            }

            private static ShapeFile createJzxShapeFile(string shpFileName, string sPrjStr)
            {
                var shp = new ShapeFile();
                shp.Create(shpFileName, EShapeType.SHPT_ARC, sPrjStr);
                int n = shp.AddField("BSM", DBFFieldType.FTInteger, 10);
                shp.AddField("YSDM", DBFFieldType.FTString, 6);
                shp.AddField("JXXZ", DBFFieldType.FTString, 6);
                shp.AddField("JZXLB", DBFFieldType.FTString, 2);
                shp.AddField("JZXWZ", DBFFieldType.FTString, 1);
                shp.AddField("JZXSM", DBFFieldType.FTString, 254);
                shp.AddField("PLDWQLR", DBFFieldType.FTString, 100);
                shp.AddField("PLDWZJR", DBFFieldType.FTString, 100);
                shp.AddField("JZXH", DBFFieldType.FTString, 10);
                shp.AddField("QJZDH", DBFFieldType.FTString, 10);
                shp.AddField("ZJZDH", DBFFieldType.FTString, 10);
                shp.AddField("DKBM", DBFFieldType.FTString, 254);
                return shp;
            }

            public void export(ShapeJzxEntity en)//, ShapeFile shp)
            {
                var shp = _shp;
                int j = 0;
                int nJzxH = 1 + _nCurrShapeID;
                shp.WriteWKB(-1, en.wkb);
                shp.WriteFieldInt(_nCurrShapeID, 0, _nStartBSM++);
                shp.WriteFieldString(_nCurrShapeID, ++j, _p._param.sJzxYSDMVal);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.JXXZ);// "J" + nJzxH);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.JZXLB);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.JZXWZ);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.JZXSM);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.GetPLDWQLR());
                shp.WriteFieldString(_nCurrShapeID, ++j, en.GetPLDWZJR());
                shp.WriteFieldString(_nCurrShapeID, ++j, nJzxH.ToString());
                shp.WriteFieldString(_nCurrShapeID, ++j, en.nQJZDH);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.nZJZDH);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.GetDKBM());
                ++_nCurrShapeID;

                #region yxm 2017/6/10

                ++_nExportCount;
                if (_p._param.JzxMaxRecords > 0)
                {
                    if (_nCurrShapeID >= _p._param.JzxMaxRecords)
                    {
                        _shp.Dispose();
                        _nCurrShapeID = 0;
                        _shp = createJzxShapeFile(MakeShapeFileName(), _p._param.sESRIPrjStr);
                    }
                }

                #endregion yxm 2017/6/10
            }

            public int ExportedCount
            {
                get { return _nExportCount; }
            }

            #region yxm 2017/6/10

            /// <summary>
            /// 当前正在导出的是第几个shape文件
            /// </summary>
            private int _nShpFileIndex = 0;

            private int _nExportCount = 0;
            private ShapeFile _shp;

            private string MakeShapeFileName()
            {
                if (_p._param.JzxMaxRecords > 0)
                {
                    ++_nShpFileIndex;
                    var fileName = _p._shpFilePath + "JZX" + _p._countName + "-" + _nShpFileIndex;
                    return fileName;
                }
                return _p._shpFilePath + "JZX" + _p._countName;
            }

            #endregion yxm 2017/6/10
        }

        private class DkExporter
        {
            private readonly ExportJzdx _p;
            private int _nCurrShapeID;
            private int _nStartBSM;

            public DkExporter(ExportJzdx p)
            {
                _p = p;
            }

            public void Reset()
            {
                _nStartBSM = _p._param.nDkBSMStartVal;
                _nCurrShapeID = 0;
                _shp = createDkShapeFile(MakeShapeFileName(), _p._param.sESRIPrjStr);
            }

            public void OnEnd()
            {
                if (_shp != null)
                {
                    _shp.Dispose();
                    _shp = null;
                }
            }

            private static ShapeFile createDkShapeFile(string shpFileName, string sPrjStr)
            {
                var shp = new ShapeFile();
                shp.Create(shpFileName, EShapeType.SHPT_POLYGON, sPrjStr);
                shp.AddField("BSM", DBFFieldType.FTInteger, 10);
                shp.AddField("YSDM", DBFFieldType.FTString, 6);
                shp.AddField("DKBM", DBFFieldType.FTString, 19);
                shp.AddField("DKMC", DBFFieldType.FTString, 50);
                shp.AddField("SYQXZ", DBFFieldType.FTString, 2);
                shp.AddField("DKLB", DBFFieldType.FTString, 2);
                shp.AddField("TDLYLX", DBFFieldType.FTString, 3);
                shp.AddField("TDYT", DBFFieldType.FTString, 1);
                shp.AddField("SFJBNT", DBFFieldType.FTString, 1);
                shp.AddField("SCMJ", DBFFieldType.FTDouble, 16, 2);
                shp.AddField("DKDZ", DBFFieldType.FTString, 50);
                shp.AddField("DKXZ", DBFFieldType.FTString, 50);
                shp.AddField("DKNZ", DBFFieldType.FTString, 50);
                shp.AddField("DKBZ", DBFFieldType.FTString, 50);
                shp.AddField("DKBZXX", DBFFieldType.FTString, 254);
                shp.AddField("ZJRXM", DBFFieldType.FTString, 100);
                shp.AddField("KJZB", DBFFieldType.FTString, 254);
                shp.AddField("SCMJM", DBFFieldType.FTDouble, 16, 2);
                shp.AddField("DLDJ", DBFFieldType.FTString, 2);
                return shp;
            }

            public void export(ShapeDkEntity en)//, ShapeFile shp)
            {
                var shp = _shp;
                int j = 0;
                if (_p.OnPresaveDk != null)
                {
                    _p.OnPresaveDk(en);
                }
                //int nJzxH = 1 + _nCurrShapeID;
                shp.WriteWKB(-1, en.wkb);
                shp.WriteFieldInt(_nCurrShapeID, 0, _nStartBSM++);
                shp.WriteFieldString(_nCurrShapeID, ++j, _p._param.sDkYSDMVal);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.DKBM);// "J" + nJzxH);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.DKMC);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.SYQXZ);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.DKLB);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.TDLYLX);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.TDYT);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.SFJBNT);
                shp.WriteFieldDouble(_nCurrShapeID, ++j, (double)Math.Round((decimal)en.SCMJ, 2));
                shp.WriteFieldString(_nCurrShapeID, ++j, en.DKDZ);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.DKXZ);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.DKNZ);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.DKBZ);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.DKBZXX);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.ZJRXM);
                shp.WriteFieldString(_nCurrShapeID, ++j, en.KJZB);
                shp.WriteFieldDouble(_nCurrShapeID, ++j, (double)Math.Round((decimal)en.SCMJM, 2));
                shp.WriteFieldString(_nCurrShapeID, ++j, en.DLDJ);
                ++_nCurrShapeID;

                #region yxm 2017/6/10

                ++_nExportCount;
                if (_p._param.DkMaxRecords > 0)
                {
                    if (_nCurrShapeID >= _p._param.DkMaxRecords)
                    {
                        _shp.Dispose();
                        _shp = null;
                        _nCurrShapeID = 0;
                        _shp = createDkShapeFile(MakeShapeFileName(), _p._param.sESRIPrjStr);
                    }
                }

                #endregion yxm 2017/6/10
            }

            public int ExportedCount
            {
                get { return _nExportCount; }
            }

            #region yxm 2017/6/10

            /// <summary>
            /// 当前正在导出的是第几个shape文件
            /// </summary>
            private int _nShpFileIndex = 0;

            private int _nExportCount = 0;
            private ShapeFile _shp;

            private string MakeShapeFileName()
            {
                if (_p._param.DkMaxRecords > 0)
                {
                    ++_nShpFileIndex;
                    var fileName = _p._shpFilePath + "DK" + _p._countName + "-" + _nShpFileIndex;
                    return fileName;
                }
                return _p._shpFilePath + "DK" + _p._countName;
            }

            #endregion yxm 2017/6/10
        }

        /// <summary>
        /// 点相等的比较
        /// </summary>
        public class JzdItemEqualComparer : IEqualityComparer<IPoint>
        {
            private double _tolerace, _tolerace2;
            private Coordinate _tmpC = new Coordinate();

            public JzdItemEqualComparer(double tolerance)
            {
                _tolerace = tolerance;
                _tolerace2 = tolerance * tolerance;
            }

            public bool Equals(IPoint a, IPoint b)
            {
                return CglHelper.IsSame2(a.X, a.Y, b.X, b.Y, _tolerace2);
            }

            public int GetHashCode(IPoint obj)
            {
                _tmpC.X = func(obj.X);// ToolMath.RoundNumericFormat(obj.X, 3);
                _tmpC.Y = func(obj.Y);// ToolMath.RoundNumericFormat(obj.Y, 3);
                return _tmpC.GetHashCode();
            }

            private static double func(double x)
            {
                long n = (long)(x * 10);
                return n / 10.0;
            }
        }

        private readonly DBSpatialite _db;
        private readonly ExportJzdxParam _param;

        private readonly JzdExporter _jzdExporter;
        private readonly JzxExporter _jzxExporter;
        private readonly DkExporter _dkExporter;

        private int _progressCount;
        private int _oldProgress;
        public Action<string, int> ReportProgress;
        public Action<string> ReportInfomation;
        public Action<string> ReportWarn;

        /// <summary>
        /// 保存地块实体前的回调
        /// </summary>
        public Action<ShapeDkEntity> OnPresaveDk;

        private string _shpFilePath;
        private string _sDydm;
        private string _countName;

        public ExportJzdx(DBSpatialite db, ExportJzdxParam prm,
            string shpFilePath, string sDydm, string countname)
        {
            _db = db;
            _shpFilePath = shpFilePath;
            _sDydm = sDydm;
            _countName = countname;
            _param = prm;
            _jzdExporter = new JzdExporter(this);
            _jzxExporter = new JzxExporter(this);
            _dkExporter = new DkExporter(this);
        }

        private static bool testEqual(Coordinate c, double x, double y)
        {
            var dx = c.X - x;
            if (Math.Abs(dx) > 0.0001)
                return false;
            var dy = c.Y - y;
            if (Math.Abs(dy) > 0.0001)
                return false;
            return true;
        }

        /// <summary>
        /// 导出界址点、界址线和地块
        /// </summary>
        /// <param name="shpFilePath">shape文件的输出路径</param>
        /// <param name="sDydm">当前地域的地域全编码</param>
        /// <param name="excludeDkbm">要排除的地块编码</param>
        public void DoExport(HashSet<string> excludeDkbm)// string shpFileName,string jzxShpFileName,string where=null)
        {
            ReportInfomation("开始时间：" + DateTime.Now);
            string shpFilePath = _shpFilePath;
            string sDydm = _sDydm;
            var countname = _countName;
            if (!(shpFilePath.EndsWith("/") || shpFilePath.EndsWith("\\")))
            {
                shpFilePath += "\\";
            }
            var jzdShpFileName = shpFilePath + "JZD" + countname;
            var jzxShpFileName = shpFilePath + "JZX" + countname;
            var dkShapeFileName = shpFilePath + "DK" + countname;

            _jzdExporter.Reset();
            _jzxExporter.Reset();
            _dkExporter.Reset();
            _oldProgress = 0;

            var jzdEqualCompare = new JzdEqualComparer(_param.tolerance);
            try
            {
                //var wh = _param.ExportJzd ? JzdFields.DYBM + " like '" + sDydm + "%'" : "1<0";
                var wh = JzdFields.DYBM + " like '" + sDydm + "%'";
                if (_param.exportOnlyKey)
                    wh += " and  SFKY=1";
                var jzdIndex = Util.QuerySortedJzdIndexItems(_db, excludeDkbm, wh);
                ReportInfomation("已选择界址点总数：" + jzdIndex.Count + "个");

                wh = JzxFields.DYDM + " like '" + sDydm + "%'";// _param.ExportJzx ? JzxFields.DYDM + " like '" + sDydm + "%'" : "1<0";
                var jzxIndex = Util.QuerySortedJzxIndexItems(_db, excludeDkbm, ReportWarn, wh);
                ReportInfomation("已选择界址线总数：" + jzxIndex.Count + "条");

                wh = Zd_cbdFields.ZLDM + " like '" + sDydm + "%'";
                if (_param.DkOtherWhereClause != null)
                {
                    wh += " and (" + _param.DkOtherWhereClause + ")";
                }
                var dkIndex = Util.QuerySortedCbdIndexItems(_db, excludeDkbm, ReportWarn, wh);

                ReportInfomation("已选择地块总数：" + dkIndex.Count + "个");

                _progressCount = jzdIndex.Count;

                var jzds = new Dictionary<IPoint, ShapeJzdEntity>(new JzdItemEqualComparer(_param.tolerance));
                var jzxs = new ListShapeJzxEntity();
                var dks = new ListShapeDkEntity();
                int i = 0, k = 0;
                int nMaxJzxCacheCount = 0;
                int nMaxDkCacheCount = 0;
                Util.QueryJzdEntities(_db, jzdIndex, jzds, lastJzdEn =>
                {
                    double rightX = lastJzdEn.pt.X + _param.tolerance * 2;

                    #region 读取一部分数据到jzxs

                    var fQueryJzx = i < jzxIndex.Count && (jzxs.Count == 0 || rightX >= jzxs[jzxs.Count - 1].minX);
                    if (fQueryJzx)
                    {
                        i = Util.QueryJzxEntities(_db, jzxIndex, i, rightX, jzxs);
                        if (nMaxJzxCacheCount < jzxs.Count)
                        {
                            nMaxJzxCacheCount = jzxs.Count;
                        }

                        #region 合并重复的界址线

                        for (int i1 = jzxs.Count - 1; i1 > 0; --i1)
                        {
                            var j1 = jzxs[i1];
                            bool fSame = false;
                            for (int j = i1 - 1; j >= 0; --j)
                            {
                                var j0 = jzxs[j];
                                if (j1.minX - j0.minX > _param.tolerance)
                                {
                                    break;
                                }

                                if (isSame(j0.coords, j1.coords))
                                {
                                    foreach (var pldwQlr in j1.PLDWQLR)
                                    {
                                        if (!j0.PLDWQLR.Contains(pldwQlr))
                                        {
                                            j0.PLDWQLR.Add(pldwQlr);
                                        }
                                    }
                                    foreach (var pldwZjr in j1.PLDWZJR)
                                    {
                                        if (!j0.PLDWZJR.Contains(pldwZjr))
                                        {
                                            j0.PLDWZJR.Add(pldwZjr);
                                        }
                                    }
                                    foreach (var dkbm in j1.DKBM)
                                    {
                                        if (!j0.DKBM.Contains(dkbm))
                                        {
                                            j0.DKBM.Add(dkbm);
                                        }
                                    }
                                    fSame = true;
                                    break;
                                }
                            }
                            if (fSame)
                            {
                                jzxs.RemoveAt(i1);
                            }
                        }

                        #endregion 合并重复的界址线
                    }

                    #endregion 读取一部分数据到jzxs

                    #region 读取一部分数据到dks

                    var fQueryDk = k < dkIndex.Count && (dks.Count == 0 || rightX >= dks[dks.Count - 1].env.MinX);
                    if (fQueryDk)
                    {
                        k = Util.QueryDkEntities(_db, dkIndex, k, rightX, dks, jzdEqualCompare, _param.sDkSYQXZ);
                        if (nMaxDkCacheCount < dks.Count)
                        {
                            nMaxDkCacheCount = dks.Count;
                        }
                    }

                    #endregion 读取一部分数据到dks

                    IPoint rightPoint = null;
                    var lstRemove = new List<ShapeJzdEntity>();
                    foreach (var kv in jzds)
                    {
                        if (kv.Key.X + /*_param.tolerance * 2*/0.1 < lastJzdEn.pt.X)
                        {
                            lstRemove.Add(kv.Value);
                            if (rightPoint == null || rightPoint.X < kv.Key.X)
                            {
                                rightPoint = kv.Key;
                            }
                        }
                    }
                    foreach (var je in lstRemove)
                    {
                        if (_param.ExportJzd)
                        {
                            _jzdExporter.export(je, /*shp,*/ jzxs, dks);
                        }
                        jzds.Remove(je.pt);
                    }
                    /* 修改于2016/10/20 有的数据会导致rightPoint为空,在此做特殊处理? */
                    if (rightPoint != null)
                    {
                        try
                        {
                            tryExportJzx(jzxs, rightPoint);//, jzxShp);
                            tryExportDk(dks, rightPoint);//, dkShp);
                        }
                        catch
                        { }
                    }
                });

                #region 导出缓存中剩余部分的数据

                if (_param.ExportJzd)
                {
                    foreach (var kv in jzds)
                    {
                        _jzdExporter.export(kv.Value, /*shp,*/ jzxs, dks);
                    }
                }
                if (_param.ExportJzx)
                {
                    foreach (var jzx in jzxs)
                    {
                        _jzxExporter.export(jzx);//, jzxShp);
                    }
                }

                foreach (var dk in dks)
                {
                    _dkExporter.export(dk);//, dkShp);
                }

                #endregion 导出缓存中剩余部分的数据

                ReportInfomation("已导出界址点总数：" + _jzdExporter.ExportedCount + "个，去掉重复记录：" + (_progressCount - _jzdExporter.ExportedCount));
                ReportInfomation("已导出界址线总数：" + _jzxExporter.ExportedCount + "条，去掉重复记录：" + (jzxIndex.Count - _jzxExporter.ExportedCount));
                ReportInfomation("已导出地块总数：" + _dkExporter.ExportedCount + "个");
                reportProgress(_progressCount);
                ReportInfomation("结束时间：" + DateTime.Now);

                if (_jzdExporter.ExportedCount == 0)
                    ReportWarn("未获取地块界址数据，无法在成果库中导出地块图斑，请进行初始化界址数据后再执行导出操作！");
            }
            finally
            {
                _jzdExporter.OnEnd();
                _jzxExporter.OnEnd();
                _dkExporter.OnEnd();
            }
        }

        /// <summary>
        /// 导出地块
        /// </summary>
        public void DoExportLandOnly(HashSet<string> excludeDkbm)// string shpFileName,string jzxShpFileName,string where=null)
        {
            ReportInfomation("开始时间：" + DateTime.Now);
            string shpFilePath = _shpFilePath;
            string sDydm = _sDydm;
            var countname = _countName;
            if (!(shpFilePath.EndsWith("/") || shpFilePath.EndsWith("\\")))
            {
                shpFilePath += "\\";
            }
            //var jzdShpFileName = shpFilePath + "JZD" + countname;
            _dkExporter.Reset();
            _oldProgress = 0;

            var jzdEqualCompare = new JzdEqualComparer(_param.tolerance);
            try
            {
                var wh = Zd_cbdFields.ZLDM + " like '" + sDydm + "%'";
                if (_param.DkOtherWhereClause != null)
                {
                    wh += " and (" + _param.DkOtherWhereClause + ")";
                }
                var dkIndex = Util.QuerySortedCbdIndexItems(_db, excludeDkbm, ReportWarn, wh);

                ReportInfomation("已选择地块总数：" + dkIndex.Count + "个");

                _progressCount = dkIndex.Count;

                for (int i = 0; i < dkIndex.Count;)
                {
                    int j = i + 200;
                    if (j > dkIndex.Count)
                    {
                        j = dkIndex.Count;
                    }
                    string sin = dkIndex[i].rowid.ToString();
                    for (int k = i + 1; k < j; ++k)
                    {
                        sin += "," + dkIndex[k].rowid;
                    }
                    var cache = new ListShapeDkEntity();
                    InitLandDotCoilUtil.QueryShapeDkEntities(_db, "rowid in(" + sin + ")", cache,
                        en =>
                        {
                            en.SYQXZ = _param.sDkSYQXZ;
                            en.dicJzdh = new Dictionary<Coordinate, string>(jzdEqualCompare);
                            foreach (var c in en.coords)
                            {
                                en.dicJzdh[c] = null;
                            }
                            if (en.wkb != null)
                                en.SCMJ = RoundNumericFormat(Spatial.Geometry.FromBytes(en.wkb).Area(), 2);
                        });
                    foreach (var item in cache)
                    {
                        _dkExporter.export(item);
                    }
                    i = j;
                }

                ReportInfomation("已导出地块总数：" + _dkExporter.ExportedCount + "个");
                reportProgress(_progressCount);
                ReportInfomation("结束时间：" + DateTime.Now);
            }
            finally
            {
                _jzdExporter.OnEnd();
                _jzxExporter.OnEnd();
                _dkExporter.OnEnd();
            }
        }

        [Obsolete]
        private void tryExportDk(List<ShapeDkEntity> dks, IPoint lastExportJzd)//, ShapeFile shp)
        {
            var lstRemove = new List<ShapeDkEntity>();
            for (int i = dks.Count - 1; i >= 0; --i)
            {
                var jzx = dks[i];
                if (jzx.env.MaxX + /*_param.tolerance * 2*/0.1 < lastExportJzd.X)
                {
                    lstRemove.Add(jzx);
                    dks.RemoveAt(i);
                }
            }
            for (int i = lstRemove.Count - 1; i >= 0; --i)
            {
                _dkExporter.export(lstRemove[i]);//, shp);
            }
        }

        private void tryExportJzx(ListShapeJzxEntity jzxs, IPoint lastExportJzd)//, ShapeFile shp)
        {
            var lstRemove = new List<ShapeJzxEntity>();
            for (int i = jzxs.Count - 1; i >= 0; --i)
            {
                var jzx = jzxs[i];
                if (jzx.EndPoint.X + /*_param.tolerance * 2*/0.1 < lastExportJzd.X)
                {
                    lstRemove.Add(jzx);
                    jzxs.RemoveAt(i);
                }
            }
            if (_param.ExportJzx)
            {
                for (int i = lstRemove.Count - 1; i >= 0; --i)
                {
                    _jzxExporter.export(lstRemove[i]);//, shp);
                }
            }
        }

        private bool isSame(Coordinate[] l1, Coordinate[] l2)
        {
            if (l1.Length != l2.Length)
                return false;

            for (int i = 0; i < l1.Length; ++i)
            {
                var c1 = l1[i];
                var c2 = l2[i];
                if (!CglHelper.IsSame2(c1, c2, _param.tolerance, _param.tolerance2))
                    return false;
            }
            return true;
        }

        private void reportProgress(int nCurrShapeID, string msg = "导出界址点")
        {
            ProgressHelper.ReportProgress(ReportProgress, msg, _progressCount, nCurrShapeID, ref _oldProgress);
        }

        /// <summary>
        /// 四舍五入小数位数
        /// </summary>
        /// <param name="value">数值</param>
        /// <param name="digits">位数</param>
        /// <returns></returns>
        public double RoundNumericFormat(double value, int digits)
        {
            double number = value + 0.00000001;
            //   double numeric = ToolMath.RoundNumericFormat(number, digits);

            double numeric = Convert.ToUInt64(number * 100) / 100.0;
            switch (digits)
            {
                case 2:
                    numeric = Convert.ToUInt64(number * 100) / 100.0;
                    break;
                case 3:
                    numeric = Convert.ToUInt64(number * 1000) / 1000.0;
                    break;
                case 4:
                    numeric = Convert.ToUInt64(number * 10000) / 10000.0;
                    break;
                case 5:
                    numeric = Convert.ToUInt64(number * 100000) / 100000.0;
                    break;
                case 6:
                    numeric = Convert.ToUInt64(number * 1000000) / 1000000.0;
                    break;
                case 7:
                    numeric = Convert.ToUInt64(number * 10000000) / 10000000.0;
                    break;
                case 8:
                    numeric = Convert.ToUInt64(number * 100000000) / 100000000.0;
                    break;
            }
            return numeric;
        }
    }

    public class IndexItem
    {
        public int rowid;

        public double intMinX
        {
            get;
            set;
        }
    }

    public class ExportJzdxParam
    {
        /// <summary>
        /// 界址点标识码初始值
        /// </summary>
        public int nJzdBSMStartVal = 50000000;

        /// <summary>
        /// 界址线标识码初始值
        /// </summary>
        public int nJzxBSMStartVal = 20000000;

        /// <summary>
        /// 地块标识码初始值
        /// </summary>
        public int nDkBSMStartVal = 10000000;

        /// <summary>
        /// 界址点要素代码
        /// </summary>
        public string sJzdYSDMVal = "211021";

        /// <summary>
        /// 界址线要素代码
        /// </summary>
        public string sJzxYSDMVal = "211031";

        /// <summary>
        /// 地块要素代码
        /// </summary>
        public string sDkYSDMVal = "211011";

        /// <summary>
        /// 地块使用权性质
        /// </summary>
        public string sDkSYQXZ = "30";

        /// <summary>
        ///ESRI坐标系（ .prj文件的内容）
        /// </summary>
        public string sESRIPrjStr = null;

        /// <summary>
        /// 界址点距离重叠容差
        /// </summary>
        public readonly double tolerance = 0.05;

        public readonly double tolerance2;

        /// <summary>
        /// 地块的附加条件
        /// </summary>
        public string DkOtherWhereClause;

        /// <summary>
        /// 是否导出界址点
        /// </summary>
        public bool ExportJzd = true;

        /// <summary>
        /// 是否导出界址线
        /// </summary>
        public bool ExportJzx = true;

        /// <summary>
        /// 每个地块shape文件包含的最大记录数，若为0则不限制
        /// </summary>
        public int DkMaxRecords = 0;

        /// <summary>
        /// 每个界址点shape文件包含的最大记录数，若为0则不限制
        /// </summary>
        public int JzdMaxRecords = 0;

        /// <summary>
        /// 每个界址线shape文件包含的最大记录数，若为0则不限制
        /// </summary>
        public int JzxMaxRecords = 0;

        public bool exportOnlyKey = false;

        /// <summary>
        /// 使用统编号导出-和台账的初始化处理一致，重复点统编号一致
        /// </summary>
        public bool UseUniteNumberExport = false;

        public ExportJzdxParam(double tolerance_)
        {
            tolerance = tolerance_;
            tolerance2 = tolerance * tolerance;
        }
    }
}