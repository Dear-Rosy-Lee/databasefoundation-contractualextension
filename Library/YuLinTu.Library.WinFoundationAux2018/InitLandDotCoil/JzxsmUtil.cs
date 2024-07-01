using GeoAPI.Geometries;
/*
 * (C) 2016 鱼鳞图公司版权所有，保留所有权利
 * http://www.yulintu.com
 *
 * CLR 版本：   4.0.30319.34014            最低的 Framework 版本：4.0
 * 文 件 名：   JzxsmUtil
 * 创 建 人：   颜学铭
 * 创建时间：   2017/4/14 11:32:18
 * 版    本：   1.0.0
 * 备注描述：
 * 修订历史：
*/
using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.NetAux;

namespace YuLinTu.Library.Aux
{
    /// <summary>
    /// 1.界址线说明：要求生成“j1沿西南方多少米到j2。”如:J1沿西南方2.25米到J2.
    /// (因现在是折线，所以就算方向只计算起点与终点，需有个属性设置，
    /// 设置界址点号时界址号还是界址统编号)
    /// </summary>
    public class JzxsmUtil
    {
        public class JzxEn
        {
            public int rowid;
            public string jzxqdh;
            public string jzxzdh;
            public string jzxqdID;
            public string jzxzdID;
            public IGeometry Shape;
        }
        /// <summary>
        /// 生成界址线说明
        /// </summary>
        /// <param name="db"></param>
        /// <param name="reportProgress"></param>
        /// <param name="fUseTbjzdh">是否使用统编界址点号</param>
        /// <param name="sDydm">地域代码（若不为null，则表示只生成该地域下的数据）</param>
        public static void MakeJzxsm(DBSpatialite db, Action<string, int> reportProgress, bool fUseTbjzdh = false, string sDydm = null)
        {
            string wh = null;
            if (!string.IsNullOrEmpty(sDydm))
            {
                wh = JzxFields.DYDM + " like '" + sDydm + "%'";
            }
            var sql = "select count(*) from " + JzxFields.TABLE_NAME;
            if (wh != null)
            {
                sql += " where " + wh;
            }
            int cnt = db.QueryOneInt(sql);
            int curProgress = 0;
            int oldProgress = 0;
            Aspect a = new Aspect(0);
            Dictionary<string, string> dicJzdTbh = null;// new Dictionary<Coordinate, string>(new JzdEqualComparer(0.00001));

            var transaction = db.BeginTransaction();
            try
            {
                if (fUseTbjzdh)
                {
                    Console.WriteLine("正在查询统编界址点号...");
                    string wh1 = null;
                    if (!string.IsNullOrEmpty(sDydm))
                    {
                        wh1 = JzdFields.DYBM + " like '" + sDydm + "%'";
                    }
                    dicJzdTbh = queryJzdtbh(db, wh1);
                }
                QueryShapeJzxEntities(db, en =>
                {
                    ProgressHelper.ReportProgress(reportProgress, "生成界址线说明", cnt, ++curProgress, ref oldProgress);
                    if (en.Shape != null)
                    {
                        var p0 = en.Shape.Coordinates[0];
                        var p1 = en.Shape.Coordinates[en.Shape.Coordinates.Count() - 1];
                        a.Assign(p0.X, p0.Y, p1.X, p1.Y);
                        string qjzdh = en.jzxqdh;
                        var zjzdh = en.jzxzdh;
                        if (fUseTbjzdh)
                        {
                            string s;
                            if (dicJzdTbh.TryGetValue(en.jzxqdID, out s))
                            {
                                qjzdh = s;
                            }
                            if (dicJzdTbh.TryGetValue(en.jzxzdID, out s))
                            {
                                zjzdh = s;
                            }
                        }
                        var jszsm = qjzdh + "沿" + a.toAzimuthString() + "方" + Math.Round(en.Shape.Length, 2) + "米到" + zjzdh;
                        updateJzxsm(db, en.rowid, jszsm);
                    }
                }, wh);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine(ex.Message);
            }
        }

        private static void QueryShapeJzxEntities(DBSpatialite db
            , Action<JzxEn> callback, string wh = null)
        {
            var subFields = "rowid," + JzxFields.JZXQDH + "," + JzxFields.JZXZDH
            + "," + JzxFields.JZXQD + "," + JzxFields.JZXZD + ",AsBinary(" + JzxFields.SHAPE + ")";
            var sql = "select " + subFields + " from " + JzxFields.TABLE_NAME;
            if (wh != null)
            {
                sql += " where " + wh;
            }
            db.QueryCallback(sql, r =>
            {
                var en = new JzxEn();
                int i = 0;
                en.rowid = (int)r.GetInt64(0);
                en.jzxqdh = SqlUtil.GetString(r, ++i);
                en.jzxzdh = SqlUtil.GetString(r, ++i);
                en.jzxqdID = SqlUtil.GetString(r, ++i);
                en.jzxzdID = SqlUtil.GetString(r, ++i);
                var wkb = r.GetValue(++i) as byte[];
                if (wkb != null)
                {
                    var g = WKBHelper.fromWKB(wkb);
                    en.Shape = g;
                }
                callback(en);
                return true;
            });
        }

        private static void updateJzxsm(DBSpatialite db, int rowid, string sJzxsm)
        {
            var sql = "update " + JzxFields.TABLE_NAME + " set " + JzxFields.JZXSM + "='" + sJzxsm + "' where rowid=" + rowid;
            db.ExecuteNonQuery(sql);
        }

        ///// <summary>
        ///// 查询界址点统编号
        ///// </summary>
        ///// <param name="db"></param>
        ///// <returns></returns>
        //private static Dictionary<Coordinate, string> queryJzdtbh(DBSpatialite db)
        //{
        //    var dicJzdTbh = new Dictionary<Coordinate, string>(new JzdEqualComparer(0.00001));
        //    var subFields = JzdFields.TBJZDH + ",AsBinary(" + JzdFields.SHAPE + ")";
        //    var sql = "select " + subFields + " from " + JzdFields.TABLE_NAME;
        //    db.QueryCallback(sql, r =>
        //    {
        //        var wkb = r.GetValue(1) as byte[];
        //        if (wkb != null)
        //        {
        //            var g = WKBHelper.fromWKB(wkb);
        //            var c = g.Coordinates[0];
        //            if (!dicJzdTbh.ContainsKey(c))
        //            {
        //                var tbjzdh = SqlUtil.GetString(r, 0);
        //                dicJzdTbh[c] = tbjzdh;
        //            }
        //        }
        //        return true;
        //    });
        //    return dicJzdTbh;
        //}

        /// <summary>
        /// 查询界址点统编号
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static Dictionary<string, string> queryJzdtbh(DBSpatialite db, string wh)
        {
            var dicJzdTbh = new Dictionary<string, string>();
            var subFields = JzdFields.TBJZDH + "," + JzdFields.ID;
            var sql = "select " + subFields + " from " + JzdFields.TABLE_NAME;
            if (!string.IsNullOrEmpty(wh))
            {
                sql += " where " + wh;
            }
            db.QueryCallback(sql, r =>
            {
                var id = SqlUtil.GetString(r, 1);
                var tbjzdh = SqlUtil.GetString(r, 0);
                dicJzdTbh[id] = tbjzdh;
                return true;
            });
            return dicJzdTbh;
        }
    }


}
