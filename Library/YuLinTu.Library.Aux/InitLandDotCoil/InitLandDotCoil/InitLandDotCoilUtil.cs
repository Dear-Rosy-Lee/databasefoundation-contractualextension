using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using YuLinTu.NetAux;

namespace YuLinTu.Library.Aux
{
    /// <summary>
    /// 初始化界址点界址线辅助类
    /// </summary>
    public class InitLandDotCoilUtil
    {
        /// <summary>
        /// 根据一组rowid集合查询对应承包地简要信息；
        /// </summary>
        /// <param name="db"></param>
        /// <param name="rowids"></param>
        /// <param name="dic"></param>
        public static void QueryShortZd_cbd(DBSpatialite db, List<int> rowids, Dictionary<int, ShortZd_cbd> dic
            , Action<ShortZd_cbd> callback)
        {
            var xml = new XmlDocument();

            string sin = SqlUtil.ConstructInClause(rowids);
            var subFields = "rowid,AsBinary(" + Zd_cbdFields.Shape + ")," + Zd_cbdFields.Qlrmc
                + "," + Zd_cbdFields.ID + "," + Zd_cbdFields.ZLDM + "," + Zd_cbdFields.DKBM
                + "," + Zd_cbdFields.DKKZXX;
            var sql = "select " + subFields + " from " + Zd_cbdFields.TABLE_NAME
                + " where rowid in(" + sin + ")";
            db.QueryCallback(sql, r =>
            {
                var en = new ShortZd_cbd();
                en.rowid = r.GetInt32(0);
                var g = WKBHelper.fromWKB(r.GetValue(1) as byte[]);
                en.xmin = g.EnvelopeInternal.MinX;
                en.xmax = g.EnvelopeInternal.MaxX;
                en.qlrMc = SqlUtil.GetString(r, 2);
                en.dkID = SqlUtil.GetString(r, 3);
                en.zlDM = SqlUtil.GetString(r, 4);
                en.DKBM = SqlUtil.GetString(r, 5);
                var xmlKzxx = SqlUtil.GetString(r, 6);
                if (xmlKzxx != null)
                {
                    xml.LoadXml(xmlKzxx);
                    var n = xml.SelectSingleNode("/AgricultureLandExpand/Elevation");
                    if (n != null && !string.IsNullOrEmpty(n.InnerText))
                    {
                        en.elevation = SafeConvertAux.SafeConvertToDouble(n.InnerText);
                    }
                    n = xml.SelectSingleNode("/AgricultureLandExpand/ReferPerson");
                    if (n != null && !string.IsNullOrEmpty(n.InnerText))
                    {
                        en.zjrMc = n.InnerText;
                    }
                }
                if (g is Polygon)
                {
                    parsePoints(g as Polygon, en);
                }
                else if (g is MultiPolygon)
                {
                    //en.isMultiPolygon = true;
                    var mg = g as MultiPolygon;
                    foreach (var g1 in mg.Geometries)
                    {
                        parsePoints(g1 as Polygon, en);
                    }
                }
                dic[en.rowid] = en;//.use();
                callback(en);
                return true;
            });
        }

        /// <summary>
        /// 根据一组rowid集合查询对应承包地简要信息；
        /// </summary>
        /// <param name="db"></param>
        /// <param name="rowids"></param>
        /// <param name="dic"></param>
        public static void QueryShortZd_cbd1(DBSpatialite db, List<int> rowids, Dictionary<int, ShortZd_cbd1> dic
            , double distance, Action<ShortZd_cbd1> callback)
        {
            var xml = new XmlDocument();

            string sin = SqlUtil.ConstructInClause(rowids);
            var subFields = "rowid,AsBinary(" + Zd_cbdFields.Shape + ")," + Zd_cbdFields.Qlrmc
                + "," + Zd_cbdFields.ID + "," + Zd_cbdFields.ZLDM + "," + Zd_cbdFields.DKBM
                + "," + Zd_cbdFields.DKKZXX;
            var sql = "select " + subFields + " from " + Zd_cbdFields.TABLE_NAME
                + " where rowid in(" + sin + ")";
            db.QueryCallback(sql, r =>
            {
                var en = new ShortZd_cbd1();
                en.rowid = r.GetInt32(0);
                var g = WKBHelper.fromWKB(r.GetValue(1) as byte[]);
                //en.xmin = g.EnvelopeInternal.MinX;
                //en.xmax = g.EnvelopeInternal.MaxX;
                //en.ymin = g.EnvelopeInternal.MinY;
                //en.ymax = g.EnvelopeInternal.MaxY;
                en.dkGeo = g.Buffer(distance);
                //en.EnvelopeInternal = g.EnvelopeInternal;
                //en.EnvelopeInternal.ExpandBy(distance);
                en.dkID = SqlUtil.GetString(r, 3);
                //var xmlKzxx = SqlUtil.GetString(r, 6);
                //if (xmlKzxx != null)
                //{
                //    xml.LoadXml(xmlKzxx);
                //    var n = xml.SelectSingleNode("/AgricultureLandExpand/Elevation");
                //    if (n != null && !string.IsNullOrEmpty(n.InnerText))
                //    {
                //        en.elevation = SafeConvertAux.SafeConvertToDouble(n.InnerText);
                //    }
                //    n = xml.SelectSingleNode("/AgricultureLandExpand/ReferPerson");
                //    if (n != null && !string.IsNullOrEmpty(n.InnerText))
                //    {
                //        en.zjrMc = n.InnerText;
                //    }
                //}

                dic[en.rowid] = en;//.use();
                callback(en);
                return true;
            });
        }

        public static void QueryShapeDkEntities(DBSpatialite db, string where, List<ExportJzdx.ShapeDkEntity> lst
            , Action<ExportJzdx.ShapeDkEntity> callback)
        {
            var xml = new XmlDocument();

            //string sin = SqlUtil.ConstructInClause(rowids);
            var subFields = Zd_cbdFields.DKBM + "," + Zd_cbdFields.DKMC
            + "," + Zd_cbdFields.DKLB + "," + Zd_cbdFields.TDLYLX
            + "," + Zd_cbdFields.DLDJ + "," + Zd_cbdFields.TDYT
            + "," + Zd_cbdFields.SFJBNT + "," + Zd_cbdFields.SCMJ
            + "," + Zd_cbdFields.DKDZ + "," + Zd_cbdFields.DKXZ
            + "," + Zd_cbdFields.DKNZ + "," + Zd_cbdFields.DKBZ
            + "," + Zd_cbdFields.DKBZXX + "," + Zd_cbdFields.DKKZXX
            + ",AsBinary(" + Zd_cbdFields.Shape + ")";
            var sql = "select " + subFields + " from " + Zd_cbdFields.TABLE_NAME
                + " where " + where;
            db.QueryCallback(sql, r =>
            {
                var en = new ExportJzdx.ShapeDkEntity();
                int i = 0;
                en.DKBM = SqlUtil.GetString(r, 0);
                en.DKMC = SqlUtil.GetString(r, ++i);
                en.DKLB = SqlUtil.GetString(r, ++i);
                en.TDLYLX = SqlUtil.GetString(r, ++i);
                en.DLDJ = SqlUtil.GetString(r, ++i);
                en.TDYT = SqlUtil.GetString(r, ++i);
                int index = ++i;
                if (r.IsDBNull(index))
                {
                    en.SFJBNT = "0";
                }
                else
                {
                    en.SFJBNT = SqlUtil.GetBoolean(r, index) == true ? "1" : "2";
                }
                en.SCMJM = r.GetDouble(++i);
                //en.SCMJ = en.SCMJM * 10000 / 15.0;
                en.DKDZ = SqlUtil.GetString(r, ++i);
                en.DKXZ = SqlUtil.GetString(r, ++i);
                en.DKNZ = SqlUtil.GetString(r, ++i);
                en.DKBZ = SqlUtil.GetString(r, ++i);
                en.DKBZXX = SqlUtil.GetString(r, ++i);
                var xmlKzxx = SqlUtil.GetString(r, ++i);
                en.wkb = r.GetValue(++i) as byte[];
                if (en.wkb != null)
                {
                    var g = WKBHelper.fromWKB(en.wkb);
                    en.env = g.EnvelopeInternal;
                    en.coords = g.Coordinates;
                    en.SCMJ = (double)Math.Round((decimal)g.Area, 2);
                    if (xmlKzxx != null)
                    {
                        xml.LoadXml(xmlKzxx);
                        var n = xml.SelectSingleNode("/AgricultureLandExpand/ReferPerson");
                        if (n != null && !string.IsNullOrEmpty(n.InnerText))
                        {
                            en.ZJRXM = n.InnerText;
                        }
                    }
                    lst.Add(en);

                    callback(en);
                }
                return true;
            });
        }

        /// <summary>
        /// 按条件删除界址点
        /// </summary>
        /// <param name="db"></param>
        /// <param name="wh"></param>
        public static void DeleteJzd(DBSpatialite db, string wh)
        {
            var sql = "delete from " + JzdFields.TABLE_NAME + " where " + wh;
            db.ExecuteNonQuery(sql);
        }

        public static void DeleteByRowids(DBSpatialite db, string tableName, List<int> rowids)
        {
            for (int i = 0; i < rowids.Count;)
            {
                int j = i + 100;
                if (j > rowids.Count)
                {
                    j = rowids.Count;
                }
                string sin = null;
                for (int k = i; k < j; ++k)
                {
                    if (sin != null)
                        sin += ",";
                    sin += rowids[k].ToString();
                }
                if (sin != null)
                {
                    db.Delete(tableName, "rowid in(" + sin + ")");
                }
                i = j;
            }
        }

        /// <summary>
        /// 按西北角顺时针排序
        /// </summary>
        /// <param name="coords">in,out</param>
        /// <param name="fCW">输入集合coords是否按顺时针排序</param>
        public static void SortCoordsByWNOrder(JzxRing r, bool fCW, JzdCache jzdcache, Action<Coordinate> callback)
        {
            int len = r.Count;
            var p0 = r[0].qJzd;
            double leftX = p0.X;
            double topY = p0.Y;
            int ip = len - 1;
            var lenDic = new Dictionary<int, double>();
            for (int i = 1; i < len; ip = i++)
            {
                var p = r[ip].qJzd;
                var q = r[i].qJzd;

                if (leftX >= q.X)
                {
                    leftX = q.X;
                }
                if (topY < q.Y)
                    topY = q.Y;
            }
            double d2 = 0;
            Coordinate dp = null;
            int n = 0;
            for (int i = 0; i < len; ++i)
            {
                var p = r[i].qJzd;
                double dx = p.X - leftX;
                double dy = p.Y - topY;
                double d = dx * dx + dy * dy;
                if (d < d2 || i == 0)
                {
                    d2 = d;
                    dp = p;
                    n = i;
                }
                lenDic.Add(i, d);
            }
            //查找起止点时 不仅是找与x最小 y最大 距离最近的点，还要与x最小的点比较 如果距离平方不超过50 并且斜率小于45 就选择x最小的点
            //if (lenDic[rowQ] != d2)
            //{
            //    var tan = (dp.Y - r[rowQ].qJzd.Y) / (dp.X - r[rowQ].qJzd.X);
            //    if (tan < 1)
            //        n = rowQ;
            //    else
            //    {
            //        JzdEdges lst;
            //        JzdEdges lstQ;
            //        if (jzdcache.TryGetValue(dp, out lst) && jzdcache.TryGetValue(r[rowQ].qJzd, out lstQ))
            //        {
            //            if (lstQ.Count > lst.Count)
            //            {
            //                n = rowQ;
            //            }
            //            else
            //            {
            //                for (int k = n; k > 0; k--)
            //                {
            //                    var tan1 = (dp.Y - r[k].qJzd.Y) / (dp.X - r[k].qJzd.X);
            //                    if ((tan1 < 1 && (lenDic[k] - d2) < 50) || (tan1 < 1.5 && (lenDic[k] - d2) < 20))
            //                        n = k;
            //                }
            //            }
            //        }
            //    }
            //}
            n = SearchStartCoord(r, dp, lenDic, n, jzdcache, len);
            Coordinate c0 = null;
            if (fCW)
            {
                for (int i = n; i < len; ++i)
                {
                    if (c0 == null)
                        c0 = r[i].qJzd;
                    callback(r[i].qJzd);
                }
                for (int i = 0; i < n; ++i)
                {
                    callback(r[i].qJzd);
                }
            }
            else
            {
                for (int i = n; i >= 0; --i)
                {
                    if (c0 == null)
                        c0 = r[i].qJzd;
                    callback(r[i].qJzd);
                }
                for (int i = len - 1; i > n; --i)
                {
                    callback(r[i].qJzd);
                }
            }
        }

        /// <summary>
        /// 查找起始点
        /// </summary>
        static private int SearchStartCoord(JzxRing r, Coordinate dp, Dictionary<int, double> lenDic,
            int cn, JzdCache jzdcache, int len)
        {
            JzdEdges lst;
            try
            {


                jzdcache.TryGetValue(dp, out lst);
                if (cn == 0 || (lst != null && lst.Count > 1))//如果起始点是0 或者是共点  直接返回
                    return cn;

                var nlen = lenDic[cn];
                var lkv = lenDic.Where(t => (t.Value > nlen - 50 && t.Value < nlen + 50)).ToList();
                if (lkv.Any(t => t.Key == 0))//如果附件范围内包含起始点,直接返回
                    return 0;
                if (lkv.Count == 2)
                {
                    JzdEdges lst2;
                    var newPoint = lkv.Find(t => t.Key != cn).Key;
                    jzdcache.TryGetValue(r[newPoint].qJzd, out lst2);
                    if (lst != null && lst2 != null && lst2.Count != lst.Count)
                        return lst2.Count > lst.Count ? newPoint : cn;
                }
                List<Coordinate> jzdList = new List<Coordinate>();
                Coordinate c0 = null;
                for (int i = cn; i < len; ++i)
                {
                    if (c0 == null)
                        c0 = r[i].qJzd;
                    jzdList.Add(r[i].qJzd);
                }
                for (int i = 0; i < cn; ++i)
                {
                    jzdList.Add(r[i].qJzd);
                }

                bool find = false;
                int index = 0;
                var previewPoint = jzdList.Last();
                for (; index < jzdList.Count; index++)
                {
                    var jzd = jzdList[index];
                    if (jzdcache.TryGetValue(jzd, out lst))
                    {
                        //判断是否关键界址点
                        if (lst != null && lst.Count > 1)
                        {
                            find = true;
                            break;
                        }
                    }

                    // 保证两线段同方向的夹角
                    var angle = 0.0;
                    if (index == 0)
                    {
                        angle = (180 - CalcAngle(previewPoint.X, previewPoint.Y,
                            jzdList[0].X, jzdList[0].Y,
                            jzdList[index].X, jzdList[index].Y,
                            jzdList[index + 1].X, jzdList[index + 1].Y));
                    }
                    else if (index == jzdList.Count - 1)
                    {
                        angle = (180 - CalcAngle(jzdList[index - 1].X, jzdList[index - 1].Y,
                            jzdList[index].X, jzdList[index].Y,
                            jzdList[index].X, jzdList[index].Y,
                              jzdList[0].X, jzdList[0].Y));
                    }
                    else
                    {
                        angle = (180 - CalcAngle(jzdList[index - 1].X, jzdList[index - 1].Y,
                            jzdList[index].X, jzdList[index].Y,
                            jzdList[index].X, jzdList[index].Y,
                            jzdList[index + 1].X, jzdList[index + 1].Y));
                    }
                    if (isKeyAngle(angle))
                    {
                        find = true;
                        break;
                    }
                }
                cn = cn + (find ? index : 0);
            }
            catch (Exception ex)
            {
                var dd = ex.Message;
            }
            return cn >= r.Count ? cn - r.Count : cn;
        }

        /// <summary>
        /// 按西北角顺时针排序
        /// </summary>
        /// <param name="coords">in,out</param>
        /// <param name="fCW">输入集合coords是否按顺时针排序</param>
        public static void SortCoordsByWNOrder(JzxRing r, bool fCW, Action<JzdEntity> callback)//, bool fContainInsertedJzd = false)
        {
            //System.Diagnostics.Debug.Assert(coords.Length > 0);
            //var orderCdts = new Coordinate[coords.Length];
            int len = r.Count;
            //double area = 0.0f;
            var p0 = r[0].qJzd;// coords[0];
            double leftX = p0.X;
            double topY = p0.Y;
            int ip = len - 1;
            int rowQ = 0;
            double rowY = 0;
            var lenDic = new Dictionary<int, double>();
            for (int i = 1; i < len; ip = i++)
            {
                var p = r[ip].qJzd;
                var q = r[i].qJzd;
                //double a1 = p.X * q.Y;
                //double a2 = q.X * p.Y;
                ////area += a1 - a2;
                //if (i > 0)
                //{
                if (leftX >= q.X)
                {
                    leftX = q.X;
                    if (rowY < q.Y)
                        rowQ = i;
                }
                if (topY < q.Y)
                    topY = q.Y;
                //}
            }
            //fCCW = area > 0;
            double d2 = 0;
            Coordinate dp = null;
            int n = 0;
            for (int i = 0; i < len; ++i)
            {
                var p = r[i].qJzd;
                double dx = p.X - leftX;
                double dy = p.Y - topY;
                double d = dx * dx + dy * dy;
                if (d < d2 || i == 0)
                {
                    d2 = d;
                    dp = p;
                    n = i;
                }
                lenDic.Add(i, d);
            }
            //查找起止点时 不仅是找与x最小 y最大 距离最近的点，还要与x最小的点比较 如果距离平方不超过50 并且斜率小于45 就选择x最小的点
            if (lenDic[rowQ] != d2)
            {
                var tan = (dp.Y - r[rowQ].qJzd.Y) / (dp.X - r[rowQ].qJzd.X);
                if (tan < 1)
                    n = rowQ;
                else
                {
                    for (int k = n; k > 0; k--)
                    {
                        var tan1 = (dp.Y - r[k].qJzd.Y) / (dp.X - r[k].qJzd.X);
                        if (tan1 < 1 && (lenDic[k] - d2) < 50)
                            n = k;
                    }
                }
            }
            Coordinate c0 = null;
            if (fCW)
            {
                for (int i = n; i < len; ++i)
                {
                    var jzd = new JzdEntity() { shape = r[i].qJzd, fInsertedPoint = r[i].fInserted };
                    callback(jzd);
                    //if (!fContainInsertedJzd && r[i].fInserted)
                    //    continue;
                    //callback(r[i].qJzd);
                }
                for (int i = 0; i < n; ++i)
                {
                    var jzd = new JzdEntity() { shape = r[i].qJzd, fInsertedPoint = r[i].fInserted };
                    callback(jzd);
                    //if (!fContainInsertedJzd && r[i].fInserted)
                    //    continue;
                    //callback(r[i].qJzd);
                }
            }
            else
            {
                for (int i = n; i >= 0; --i)
                {
                    var jzd = new JzdEntity() { shape = r[i].qJzd, fInsertedPoint = r[i].fInserted };
                    callback(jzd);
                    //if (!fContainInsertedJzd && r[i].fInserted)
                    //    continue;
                    //callback(r[i].qJzd);
                }
                for (int i = len - 1; i > n; --i)
                {
                    var jzd = new JzdEntity() { shape = r[i].qJzd, fInsertedPoint = r[i].fInserted };
                    callback(jzd);
                    //if (!fContainInsertedJzd && r[i].fInserted)
                    //    continue;
                    //callback(r[i].qJzd);
                }
            }
        }

        /// <summary>
        /// 是否关键角度
        /// </summary> 
        static private bool isKeyAngle(double angle)
        {
            return angle >= (double)10 && angle <= (double)120;
        }

        /// <summary>
        /// 计算2向量的夹角
        /// </summary> 
        public static double CalcAngle(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            var v1x = x2 - x1;
            var v1y = y2 - y1;
            var v2x = x4 - x3;
            var v2y = y4 - y3;

            var multi = v1x * v2x + v1y * v2y;
            var v1mod = Math.Sqrt(v1x * v1x + v1y * v1y);
            var v2mode = Math.Sqrt(v2x * v2x + v2y * v2y);

            var angle = Math.Acos(Math.Round(multi / v1mod / v2mode, 6)) * 180 / Math.PI;
            return angle;
        }

        public static string CreateNewID()
        {
            return Guid.NewGuid().ToString().Trim(new char[] { '{', '}' });
        }
        public static bool testIsEqual(Coordinate c, double x, double y, double tolerance = 0.001)
        {
            return CglHelper.equal(c.X, x, tolerance) && CglHelper.equal(c.Y, y, tolerance);
        }
        /// <summary>
        /// 向左边缓冲distance距离和得到的Polygon对象
        /// </summary>
        /// <param name="ptFrom"></param>
        /// <param name="ptTo"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static IPolygon BufferLeft(Coordinate ptFrom, Coordinate ptTo, double distance)
        {
            var p1 = CglHelper.deflection_distance(ptFrom, ptTo, 90, 0.1);
            if (p1 == null)
            {
                return null;
            }
            var p2 = CglHelper.deflection_distance(ptTo, ptFrom, -90, 0.1);
            var c1 = CglHelper.deflection_distance(ptFrom, ptTo, 90, distance);
            var c2 = CglHelper.deflection_distance(ptTo, ptFrom, -90, distance);

            var coords = new Coordinate[5];
            coords[0] = p1;// ptTo;// deflection_distance(ptFrom, ptTo, 90, distance);
            coords[1] = c1;
            coords[2] = c2;
            coords[3] = p2;// ptFrom;// deflection_distance(ptTo, ptFrom, -90, distance);
            coords[coords.Length - 1] = coords[0];
            return new Polygon(new LinearRing(coords));
        }

        private static void parsePoints(Polygon g, ShortZd_cbd cbd)
        {
            parsePoints(resort(g.Shell.Coordinates, true), cbd, true);
            foreach (var h in g.Holes)
            {
                parsePoints(resort(h.Coordinates, false), cbd, false);
            }
        }

        /// <summary>
        /// 根据地块节点初始化最开始的单段界址线
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="cbd"></param>
        /// <param name="fShell"></param>
        private static void parsePoints(Coordinate[] coords, ShortZd_cbd cbd, bool fShell)
        {
            if (coords.Length < 4)
                return;
            var lstJzx = fShell ? cbd.shell : new JzxRing();
            if (!fShell)
            {
                if (cbd.holes == null)
                {
                    cbd.holes = new List<JzxRing>();
                }
                cbd.holes.Add(lstJzx);
            }

            Coordinate preJzd = null;
            for (int i = 0; i < coords.Length; ++i)
            {
                var jzd = coords[i];
                if (i == coords.Length - 1)
                {
                    var jzx = new Jzx(preJzd, jzd, cbd, fShell);
                    lstJzx.Add(jzx);
                    return;
                }

                if (i == 0)
                {
                    preJzd = jzd;
                }
                else
                {
                    var jzx = new Jzx(preJzd, jzd, cbd, fShell);
                    preJzd = jzd;
                    lstJzx.Add(jzx);
                }
            }
        }

        /// <summary>
        /// 降coords按顺时针方向排序
        /// </summary>
        /// <param name="coords"></param>
        private static Coordinate[] resort(Coordinate[] coords, bool fShell)
        {
            var fCCW = GeometryHelper.IsCCW(coords);
            if (fCCW && fShell || !fCCW && !fShell)
            {
                ArrayUtil.Reverse(coords);
            }
            return coords;
        }

        public static string constructUpdateSql(string tableName, string[] fields, string where, string geometry)
        {
            string updateSql = String.Format("update {0} set ", tableName);
            for (int i = 0; i < fields.Length; ++i)
            {
                var fieldName = fields[i];
                if (i > 0)
                    updateSql += ",";
                if (StringUtil.EqualIgnorCase(fieldName, "shape"))
                {
                    updateSql += fieldName + "=" + geometry;
                }
                else
                {
                    updateSql += fieldName + "=@" + fieldName;
                }
            }
            updateSql += " where " + where;
            return updateSql;
        }

        public static string constructUpdateSql(string tableName, string[] fields, string where)
        {
            string updateSql = String.Format("update {0} set ", tableName);
            for (int i = 0; i < fields.Length; ++i)
            {
                var fieldName = fields[i];
                if (i > 0)
                    updateSql += ",";

                updateSql += fieldName + "=@" + fieldName;

            }
            updateSql += " where " + where;
            return updateSql;
        }



        public static string constructInsertSql(string tableName, string[] fields, string geomText)
        {
            var sql = String.Format("insert into {0}(", tableName);
            for (int i = 0; i < fields.Length; ++i)
            {
                var fieldName = fields[i];
                if (i > 0)
                    sql += ",";
                sql += fieldName;
            }
            sql += ") values(";
            for (int i = 0; i < fields.Length; ++i)
            {
                var fieldName = fields[i];
                if (i > 0)
                    sql += ",";
                if (StringUtil.EqualIgnorCase(fieldName, "shape"))
                {
                    sql += geomText;
                }
                else
                {
                    sql += "@" + fieldName;
                }
            }
            sql += ")";
            return sql;
        }
    }

    /// <summary>
    /// 行政地域辅助类
    /// </summary>
    public class XzdyUtil
    {
        private readonly Dictionary<string, string> _dicXiang = new Dictionary<string, string>();

        /// <summary>
        /// [村的全编码，<村名称，乡全编码>]
        /// </summary>
        private readonly Dictionary<string, Tuple<string, string>> _dicCun = new Dictionary<string, Tuple<string, string>>();

        /// <summary>
        /// 获取村的短全名称（乡镇名+村名），如：安云乡二龙村
        /// 返回映射[地域全编码，短全名称]
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public void Init(DBSpatialite db)
        {
            var sql = "select DYJB,DYQBM,DYMC,SJQBM from JCSJ_XZQY where DYJB in(2,3)";
            db.QueryCallback(sql, r =>
            {
                var jb = r.GetInt32(0);
                var dm = SqlUtil.GetString(r, 1);
                var name = SqlUtil.GetString(r, 2);
                if (string.IsNullOrEmpty(dm) || string.IsNullOrEmpty(name))
                {
                    return false;
                }
                if (jb == 3)
                {
                    _dicXiang[dm] = name;
                }
                else
                {
                    var sjQbm = SqlUtil.GetString(r, 3);
                    _dicCun[dm] = new Tuple<string, string>(name, sjQbm);
                }
                return true;
            });
        }

        /// <summary>
        /// 根据地块的全编码获取村的短全名称（乡镇名+村名），如：安云乡二龙村
        /// </summary>
        /// <param name="cunQbm"></param>
        /// <returns></returns>
        public string GetShortQmc(string zuQbm)
        {
            if (zuQbm == null)
            {
                return null;
            }
            if (zuQbm.Length > 14)
            {//村的全编码是12位
                zuQbm = zuQbm.Substring(0, 14);
            }
            if (zuQbm.Length == 12)
            {//说明为以村发包
                Tuple<string, string> cunName;
                if (_dicCun.TryGetValue(zuQbm, out cunName))
                {
                    var cunMc = cunName.Item1;
                    return cunMc;
                }
            }
            Tuple<string, string> zu;
            if (_dicCun.TryGetValue(zuQbm, out zu))
            {
                string cunMc;
                var zuMc = zu.Item1;
                var cunQbm = zu.Item2;
                if (_dicXiang.TryGetValue(cunQbm, out cunMc))
                {
                    return cunMc + zuMc;
                }
                else
                {
                    return zuMc;
                }
            }
            return null;
        }
    }
}
