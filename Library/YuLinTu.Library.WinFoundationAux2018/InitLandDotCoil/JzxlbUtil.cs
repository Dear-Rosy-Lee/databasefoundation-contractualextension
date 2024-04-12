using GeoAPI.Geometries;
/*
 * (C) 2016 鱼鳞图公司版权所有，保留所有权利
 * http://www.yulintu.com
 *
 * CLR 版本：   4.0.30319.34014            最低的 Framework 版本：4.0
 * 文 件 名：   JzxlbUtil
 * 创 建 人：   颜学铭
 * 创建时间：   2017/4/14 14:29:13
 * 版    本：   1.0.0
 * 备注描述：
 * 修订历史：
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.NetAux;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 2.界址线类别：界址线向外延伸，最近的地物如果存在线状地物或面状地物，
    /// 地物名称是下图中名称，则取相应的代码，否则全部取01.
    /// </summary>
    public class JzxlbUtil
    {
        public struct XDoubleBounds
        {
            public int rowid;
            public double minx;
            public double maxx;
        }
        public class ListXDoubleBounds : List<XDoubleBounds>
        {
            public double xmin, xmax, ymin, ymax;
        }
        public class JzxItem
        {
            public int rowid;
            public IGeometry Shape;
            public string Jzxlb;
        }

        /// <summary>
        /// 线状地物缓存
        /// </summary>
        public class XzdwCache
        {
            private readonly ListXDoubleBounds lstXzdwIdx;
            private int _curPos=0;
            private readonly Dictionary<ILineString, string> _dicJzxlb = new Dictionary<ILineString, string>();
            /// <summary>
            /// 按x方向排序
            /// </summary>
            private readonly List<ILineString> _sortedLines = new List<ILineString>();
            private readonly DBSpatialite _db;
            private readonly ListXDoubleBounds _jzxIdx;
            public XzdwCache(DBSpatialite db,ListXDoubleBounds jzxIdx,double tolerance,Envelope intersectsBounds)
            {
                _db = db;
                _jzxIdx = jzxIdx;

                string wh = null;
                if (intersectsBounds != null)
                {
                    var s = "GeomFromText('" + GeometryHelper.MakePolygon(intersectsBounds).AsText() + "')";
                    wh = "Intersects(" + JzxFields.SHAPE + "," + s + ")=1";
                }
                lstXzdwIdx = buildXBounds(db, XzdwFields.TABLE_NAME, XzdwFields.SHAPE
                    ,XzdwFields.DWMC, (r,e) =>
                {
                    if (e.MaxX + tolerance < jzxIdx.xmin)
                    {
                        return false;
                    }
                    if (e.MinX - tolerance > jzxIdx.xmax)
                    {
                        return false;
                    }
                    if (e.MaxY + tolerance < jzxIdx.ymin)
                    {
                        return false;
                    }
                    if (e.MinY - tolerance > jzxIdx.ymax)
                    {
                        return false;
                    }
                    
                   var dwmc= SqlUtil.GetString(r, 2);
                   var jzxlb=Dwmc2Code(dwmc);
                    
                   return jzxlb!=null&&jzxlb != "01";
                },wh);
            }
            public void Read(double xmin, double xmax,double tolerance)
            {
                if (lstXzdwIdx == null)
                {
                    return;
                }
                if (_curPos >= lstXzdwIdx.Count)
                {
                    return;
                }
                for (; _curPos < lstXzdwIdx.Count; )
                {
                    var x = lstXzdwIdx[_curPos];
                    if (x.minx - tolerance > xmax)
                    {
                        break;
                    }
                    ++_curPos;
                    if (x.maxx + tolerance < xmin)
                    {
                        continue;
                    }
                    IGeometry g;
                    string jzxlb;
                    query(x.rowid, out g, out jzxlb);
                    Split(g, ls =>
                    {
                        var e = ls.EnvelopeInternal;
                        if (!(e.MaxX + tolerance < xmin||e.MinY-tolerance>_jzxIdx.ymax
                            ||e.MaxY+tolerance<_jzxIdx.ymin||e.MinX-tolerance>_jzxIdx.xmax))
                        {
                            _dicJzxlb[ls] = jzxlb;
                        }
                    });
                }
                _sortedLines.Clear();
                foreach (var ls in _dicJzxlb.Keys)
                {
                    _sortedLines.Add(ls);
                }
                _sortedLines.Sort((a1, b1) =>
                {
                    var a = a1.EnvelopeInternal;
                    var b = b1.EnvelopeInternal;
                    if (a.MinX < b.MinX)
                        return -1;
                    if (a.MinX > b.MinX)
                        return 1;
                    if (a.MaxX == b.MaxX)
                        return 0;
                    return a.MaxX < b.MaxX ? -1 : 1;
                });
            }

            [Obsolete]
            public string FindJzxlb(IGeometry jzx,double tolerance,out IGeometry jzxBuffer)
            {
                jzxBuffer = null;
                var e = jzx.EnvelopeInternal;
                var lst = new List<ILineString>();
                for (int i = 0; i < _sortedLines.Count; ++i)
                {
                    var ls = _sortedLines[i];
                    var e1 = ls.EnvelopeInternal;
                    if (e1.MinX - tolerance > e.MaxX)
                    {
                        break;
                    }
                    if (e1.MaxX + tolerance < e.MinX
                        ||e1.MinY-tolerance>e.MaxY
                        ||e1.MaxY+tolerance<e.MinY)
                    {
                        continue;
                    }
                    lst.Add(ls);
                }
                if (lst.Count > 0)
                {
                    Dictionary<string, double> dic = new Dictionary<string, double>();
                    var buffer=jzx.Buffer(tolerance,GeoAPI.Operation.Buffer.BufferStyle.CapButt);
                    jzxBuffer = buffer;
                    foreach (var g in lst)
                    {
                        if (buffer.Intersects(g))
                        {
                            var g1=g.Intersection(buffer);
                            if (g1 != null)
                            {
                                var len = Math.Abs(g1.Length);
                                if (len > 0)
                                {
                                    var jzxlb = _dicJzxlb[g];
                                    if (dic.ContainsKey(jzxlb))
                                    {
                                        dic[jzxlb] = dic[jzxlb] + len;
                                    }
                                    else
                                    {
                                        dic[jzxlb] = len;
                                    }
                                }
                            }
                        }
                    }
                    double dm = -1;
                    string s = null;
                    foreach (var kv in dic)
                    {
                        if (s == null || dm < kv.Value)
                        {
                            s = kv.Key;
                            dm = kv.Value;
                        }
                    }
                    if (s != null)
                    {
                        var d = dm / jzx.Length;
                        if (d >= 0.3)
                        {
                            return s;
                        }
                    }
                }
                return null;
            }
            public void TryRelease(double xmin, double tolerance)
            {
                var lst = new List<ILineString>();
                foreach (var k in _dicJzxlb.Keys)
                {
                    lst.Add(k);
                }
                foreach (var k in lst)
                {
                    var e = k.EnvelopeInternal;
                    if (e.MaxX + tolerance < xmin)
                    {
                        _dicJzxlb.Remove(k);
                    }
                }
            }
            private void query(int rowid, out IGeometry g, out string jzxlb)
            {
               IGeometry g0 = null;
               string dwmc = null;
                var sql = "select "+XzdwFields.DWMC+", AsBinary(" + XzdwFields.SHAPE + ") as shape from " + XzdwFields.TABLE_NAME
                    + " where rowid=" + rowid;
                _db.QueryCallback(sql, r =>
                {
                    dwmc = SqlUtil.GetString(r, 0);
                    g0 = WKBHelper.fromWKB(r.GetValue(1) as byte[]);
                    return false;
                });
                g = g0;
                jzxlb =Dwmc2Code(dwmc);
            }
            private static void Split(ILineString ls, Action<ILineString> callback)
            {
                var coords = new Coordinate[2];
                for (int i = 1; i < ls.Coordinates.Count(); ++i)
                {
                    coords[0] = ls.Coordinates[i - 1];
                    coords[1] = ls.Coordinates[i];
                    var l= GeometryHelper.MakeLine(coords);
                    callback(l);
                }
            }
            private static void Split(IMultiLineString mls, Action<ILineString> callback)
            {
                foreach (var g in mls.Geometries)
                {
                    if (g is ILineString)
                    {
                        Split(g as ILineString, callback);
                    }
                }
            }
            private static void Split(IGeometry g, Action<ILineString> callback)
            {
                if (g is ILineString)
                {
                    Split(g as ILineString, ls =>
                    {
                        callback(ls);
                    });
                }
                else if (g is IMultiLineString)
                {
                    Split(g as IMultiLineString, ls =>
                    {
                        callback(ls);
                    });
                }
            }
        }

        /// <summary>
        /// 面状地物缓存
        /// </summary>
        public class MzdwCache
        {
            private readonly ListXDoubleBounds lstMzdwIdx;
            private int _curPos = 0;
            private readonly Dictionary<IGeometry, string> _dicJzxlb = new Dictionary<IGeometry, string>();
            /// <summary>
            /// 按x方向排序
            /// </summary>
            private readonly List<IGeometry> _sortedMzdw = new List<IGeometry>();
            private readonly DBSpatialite _db;
            private readonly ListXDoubleBounds _jzxIdx;
            public MzdwCache(DBSpatialite db, ListXDoubleBounds jzxIdx, double tolerance,Envelope intersectsBounds)
            {
                _db = db;
                _jzxIdx = jzxIdx;

                string wh = null;
                if (intersectsBounds != null)
                {
                    var s = "GeomFromText('" + GeometryHelper.MakePolygon(intersectsBounds).AsText() + "')";
                    wh = "Intersects(" + JzxFields.SHAPE + "," + s + ")=1";
                }

                lstMzdwIdx = buildXBounds(db, MzdwFields.TABLE_NAME, MzdwFields.SHAPE
                    , MzdwFields.DWMC, (r, e) =>
                    {
                        if (e.MaxX + tolerance < jzxIdx.xmin)
                        {
                            return false;
                        }
                        if (e.MinX - tolerance > jzxIdx.xmax)
                        {
                            return false;
                        }
                        if (e.MaxY + tolerance < jzxIdx.ymin)
                        {
                            return false;
                        }
                        if (e.MinY - tolerance > jzxIdx.ymax)
                        {
                            return false;
                        }

                        var dwmc = SqlUtil.GetString(r, 2);
                        var jzxlb = Dwmc2Code(dwmc);

                        return jzxlb != null && jzxlb != "01";
                    },wh);
            }
            public void Read(double xmin, double xmax, double tolerance)
            {
                if (lstMzdwIdx == null)
                {
                    return;
                }
                if (_curPos >= lstMzdwIdx.Count)
                {
                    return;
                }
                for (; _curPos < lstMzdwIdx.Count; )
                {
                    var x = lstMzdwIdx[_curPos];
                    if (x.minx - tolerance > xmax)
                    {
                        break;
                    }
                    ++_curPos;
                    if (x.maxx + tolerance < xmin)
                    {
                        continue;
                    }
                    IGeometry g;
                    string jzxlb;
                    query(x.rowid, out g, out jzxlb);
                    _dicJzxlb[g] = jzxlb;
                }
                _sortedMzdw.Clear();
                foreach (var ls in _dicJzxlb.Keys)
                {
                    _sortedMzdw.Add(ls);
                }
                _sortedMzdw.Sort((a1, b1) =>
                {
                    var a = a1.EnvelopeInternal;
                    var b = b1.EnvelopeInternal;
                    if (a.MinX < b.MinX)
                        return -1;
                    if (a.MinX > b.MinX)
                        return 1;
                    if (a.MaxX == b.MaxX)
                        return 0;
                    return a.MaxX < b.MaxX ? -1 : 1;
                });
            }

            [Obsolete]
            public string FindJzxlb(IGeometry jzx,IGeometry jzxBuffer, double tolerance)
            {
                var e = jzx.EnvelopeInternal;
                var lst = new List<IGeometry>();
                for (int i = 0; i < _sortedMzdw.Count; ++i)
                {
                    var ls = _sortedMzdw[i];
                    var e1 = ls.EnvelopeInternal;
                    if (e1.MinX - tolerance > e.MaxX)
                    {
                        break;
                    }
                    if (e1.MaxX + tolerance < e.MinX
                        || e1.MinY - tolerance > e.MaxY
                        || e1.MaxY + tolerance < e.MinY)
                    {
                        continue;
                    }
                    lst.Add(ls);
                }
                if (lst.Count > 0)
                {
                    Dictionary<string, double> dic = new Dictionary<string, double>();
                    if (jzxBuffer == null)
                    {
                       jzxBuffer= jzx.Buffer(tolerance, GeoAPI.Operation.Buffer.BufferStyle.CapSquare);
                    } 
                    var buffer = jzxBuffer;

                    foreach (var g in lst)
                    {
                        if (buffer.Intersects(g))
                        {
                            var g1 = g.Intersection(buffer);
                            if (g1 != null)
                            {
                                var len = Math.Abs(g1.Area);
                                if (len > 0)
                                {
                                    var jzxlb = _dicJzxlb[g];
                                    if (dic.ContainsKey(jzxlb))
                                    {
                                        dic[jzxlb] = dic[jzxlb] + len;
                                    }
                                    else
                                    {
                                        dic[jzxlb] = len;
                                    }
                                }
                            }
                        }
                    }
                    double dm = -1;
                    string s = null;
                    foreach (var kv in dic)
                    {
                        if (s == null || dm < kv.Value)
                        {
                            s = kv.Key;
                            dm = kv.Value;
                        }
                    }
                    if (s != null)
                    {
                        var d = dm / jzx.Area;
                        if (d >= 0.1)
                        {
                            return s;
                        }
                    }
                }
                return null;
            }
            public void TryRelease(double xmin, double tolerance)
            {
                var lst = new List<IGeometry>();
                foreach (var k in _dicJzxlb.Keys)
                {
                    lst.Add(k);
                }
                foreach (var k in lst)
                {
                    var e = k.EnvelopeInternal;
                    if (e.MaxX + tolerance < xmin)
                    {
                        _dicJzxlb.Remove(k);
                    }
                }
            }
            private void query(int rowid, out IGeometry g, out string jzxlb)
            {
                IGeometry g0 = null;
                string dwmc = null;
                var sql = "select " + MzdwFields.DWMC + ", AsBinary(" + MzdwFields.SHAPE + ") as shape from " + MzdwFields.TABLE_NAME
                    + " where rowid=" + rowid;
                _db.QueryCallback(sql, r =>
                {
                    dwmc = SqlUtil.GetString(r, 0);
                    g0 = WKBHelper.fromWKB(r.GetValue(1) as byte[]);
                    return false;
                });
                g = g0;
                jzxlb = Dwmc2Code(dwmc);
            }
        }

        /// <summary>
        /// 为界址线表的界址线类别字段赋值；
        /// 在线状地物表和面状地物表中查找与界址线相邻的几何对象并根据几何对象的地物名称属性
        /// 生成界址线的界址线类别代码；
        /// 相邻规则：距离（含）1.5米距离以内
        /// </summary>
        /// <param name="db"></param>
        /// <param name="reportProgress"></param>
        /// <param name="sDydm">地域代码（若不为null，则表示只生成该地域下的数据）</param>
        [Obsolete]
        public int Go(DBSpatialite db,double tolerance, Action<string, int> reportProgress,string sDydm=null)
        {
            string wh = null;
            if (!string.IsNullOrEmpty(sDydm))
            {
                wh = JzxFields.DYDM + " like '" + sDydm + "%'";
            }
            var lstJzxIdx = buildXBounds(db, JzxFields.TABLE_NAME, JzxFields.SHAPE,null,null,wh);
            if (lstJzxIdx == null)
            {
                return 0;
            }
            Envelope jzxBounds = null;
            if (lstJzxIdx.ymax > lstJzxIdx.ymin && lstJzxIdx.xmax > lstJzxIdx.xmin)
            {
                jzxBounds=new Envelope(lstJzxIdx.xmin - tolerance, lstJzxIdx.xmax + tolerance, lstJzxIdx.ymin - tolerance, lstJzxIdx.ymax + tolerance);
            }
            var xzdwCache = new XzdwCache(db, lstJzxIdx, tolerance,jzxBounds);
            var mzdwCache = new MzdwCache(db, lstJzxIdx, tolerance, jzxBounds);

            var sql = "update " + JzxFields.TABLE_NAME + " set " + JzxFields.JZXLB + "='"+JzxlbCode.Tiangen+"',"
                + JzxFields.JZXWZ + "='" + JzxwzType.Middle+"'";
            if (wh != null)
            {
                sql += " where " + wh;
            }
            db.ExecuteNonQuery(sql);

            int nTestFindCount = 0;
            int curProgress = 0;
            int oldProgress = 0;
            var jzdCmp = new JzdComparer(0.001);
            JzxItem preJzx = null;
            var jzx = new JzxItem();
            for (int i = 0; i < lstJzxIdx.Count; ++i)
            {
                ProgressHelper.ReportProgress(reportProgress, "生成界址线类别", lstJzxIdx.Count, ++curProgress, ref oldProgress);
                var jzxIdx=lstJzxIdx[i];
                jzx.rowid=jzxIdx.rowid;
                jzx.Shape=QueryShape(db, JzxFields.TABLE_NAME, JzxFields.SHAPE,jzx.rowid);
                if (preJzx!=null&&IsEqual(jzx.Shape, preJzx.Shape, jzdCmp))
                {//如果与前一条界址线相同则直接用前一条界址线的属性
                    jzx.Jzxlb = preJzx.Jzxlb;
                }
                else
                {//查找附件的地物并获得界址线类别代码
                    xzdwCache.TryRelease(jzxIdx.minx, tolerance);
                    xzdwCache.Read(jzxIdx.minx, jzxIdx.maxx, tolerance);
                    mzdwCache.TryRelease(jzxIdx.minx, tolerance);
                    mzdwCache.Read(jzxIdx.minx, jzxIdx.maxx, tolerance);
                    IGeometry jzxBuffer;
                    jzx.Jzxlb=xzdwCache.FindJzxlb(jzx.Shape, tolerance,out jzxBuffer);
                    if (jzx.Jzxlb == null)
                    {
                        jzx.Jzxlb = mzdwCache.FindJzxlb(jzx.Shape, jzxBuffer, tolerance);
                    }
                }
                if (jzx.Jzxlb != null)
                {
                    updateJzxlb(db, jzx.rowid, jzx.Jzxlb);
                    ++nTestFindCount;
                }

                if (preJzx == null)
                {
                    preJzx = jzx;
                    jzx = new JzxItem();
                }
                else
                {
                    var t = preJzx;
                    preJzx = jzx;
                    jzx = t;
                }      
                
            }
            Console.WriteLine("共找到" + nTestFindCount + "条！");
            return lstJzxIdx.Count;
        }
        /// <summary>
        /// 将地物名称转换为界址线的界址线类别代码
        /// </summary>
        /// <param name="dwmc"></param>
        /// <returns></returns>
        private static string Dwmc2Code(string dwmc)
        {
            if (dwmc.Contains("路"))
            {
                return "03";
            }
            switch (dwmc)
            {
                case "沟渠":
                    return "02";
                case "道路":
                    return "03";
                case "行树":
                    return "04";
                case "围墙":
                    return "05";
                case "墙壁":
                    return "06";
                case "栅栏":
                    return "07";
                case "两点连线":
                    return "08";
                case "其他界线":
                    return "99";
            }
            return "99";// "01";
        }

        /// <summary>
        /// 构造x方向从小到大排序的索引
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <param name="shapeFieldName"></param>
        /// <returns></returns>
        private static ListXDoubleBounds buildXBounds(DBSpatialite db, string tableName, string shapeFieldName
           , string attachField = null, Func<System.Data.SQLite.SQLiteDataReader, Envelope, bool> callback = null
            ,string wh=null)
        {
            ListXDoubleBounds lstXBounds = null;// new ListXDoubleBounds();
            var subFields="rowid," + "AsBinary(" + shapeFieldName + ") as shape";
            if (attachField != null)
            {
                subFields += "," + attachField;
            }
            var sql = "select "+subFields+" from " + tableName
                +" where "+shapeFieldName+" is not null";
            if (wh != null)
            {
                sql += " and (" + wh + ")";
            }
            db.QueryCallback(sql, r =>
            {
                var x = new XDoubleBounds();
                x.rowid = SafeConvertAux.SafeConvertToInt32(r.GetValue(0));
                var e = WKBHelper.fromWKB(r.GetValue(1) as byte[]).EnvelopeInternal;
                x.minx = e.MinX;
                x.maxx = e.MaxX;
                if (lstXBounds==null)
                {
                    lstXBounds = new ListXDoubleBounds();
                    lstXBounds.xmin=e.MinX;
                    lstXBounds.xmax=e.MaxX;
                    lstXBounds.ymin=e.MinY;
                    lstXBounds.ymax = e.MaxY;
                }
                else
                {
                    if (lstXBounds.xmax < e.MaxX)
                    {
                        lstXBounds.xmax = e.MaxX;
                    }
                    if (lstXBounds.ymin > e.MinY)
                    {
                        lstXBounds.ymin = e.MinY;
                    }
                    if (lstXBounds.ymax < e.MaxY)
                    {
                        lstXBounds.ymax = e.MaxY;
                    }
                }
                bool fOk = true;
                if (callback != null)
                {
                    fOk = callback(r,e);
                }
                if (fOk)
                {
                    lstXBounds.Add(x);
                }
                return true;
            });
            if (lstXBounds != null)
            {
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
                lstXBounds.xmin = lstXBounds[0].minx;
            }
            return lstXBounds;
        }
        private static IGeometry QueryShape(DBSpatialite db, string tableName, string shapeFieldName, int rowid)
        {
            IGeometry g = null;
            var sql = "select AsBinary(" + shapeFieldName + ") as shape from " + tableName
                + " where rowid="+rowid ;
            db.QueryCallback(sql, r =>
            {
               g = WKBHelper.fromWKB(r.GetValue(0) as byte[]);
                return false;
            });
            return g;
        }
        private static bool IsEqual(IGeometry a, IGeometry b,JzdComparer cmp)
        {
            if (a == null || b == null)
            {
                return false;
            }
            if (a.Length != b.Length)
            {
                return false;
            }
            if (a.Coordinates.Count() != b.Coordinates.Count())
            {
                return false;
            }
            for (int i = a.Coordinates.Count() - 1; i >= 0; --i)
            {
                var ca = a.Coordinates[i];
                var cb = b.Coordinates[i];
                if (cmp.Compare(ca, cb) != 0)
                {
                    return false;
                }
            }
            return true;
        }
        private static void updateJzxlb(DBSpatialite db, int rowid, string sJzxlb)
        {
            var sql = "update " + JzxFields.TABLE_NAME + " set " + JzxFields.JZXLB + "='" + sJzxlb + "'";
            if (sJzxlb != JzxlbCode.Tiangen)
            {
                sql += "," + JzxFields.JZXWZ + "='" + JzxwzType.Left + "'";
            }
            sql+=" where rowid=" + rowid;
            db.ExecuteNonQuery(sql);
        }
    }
}
