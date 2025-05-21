using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YuLinTu.Spatial;

namespace YuLinTu.Component.CoordinateTransformTask
{
    /// <summary>
    /// 坐标参数模拟计算
    /// </summary>
    public class CoordinateParam
    {
        public double[] paramForwardY;
        public double[] paramReverseY;

        public double[] paramForwardX;
        public double[] paramReverseX;

        public int dimensionl;
        private static CoordinateParam en;
        private static CoordChangeC ccc;
        //private Matrix[] matrix;
        public List<CoordChangeParam> coordChangeParams;
        public static object lockobj = new object();

        /// <summary>
        /// 是否取消
        /// </summary>
        [XmlIgnore]
        public Func<bool> IsCancel { get; set; }

        private CoordinateParam()
        {
            paramForwardX = paramReverseX = paramForwardY = paramReverseY = new double[3] { 0, 0, 0 };
            dimensionl = 2;
        }

        static public CoordinateParam Getinstance()
        {
            if (en == null)
            {
                en = new CoordinateParam();
                ccc = new CoordChangeC();
            }
            return en;
        }

        static public CoordChangeC GetinstanceC()
        {
            if (ccc == null)
                ccc = new CoordChangeC();

            return ccc;
        }

        /// <summary>
        /// 参数计算
        /// </summary>
        /// <returns></returns>
        public EnData CoordnateCalcParam(EnCenterData enData, List<CodeData> targetData)
        {
            //var result = ConvertDataToCoord(enData.AllLandList.ToList(), targetData, false);
            var spoint = new List<Coordinate>();
            var tpoint = new List<Coordinate>();
            foreach (var sd in enData.AllLandList.ToList())
            {
                var ditem = targetData.Find(t => t.Code == sd.Code);
                var cods = sd.Shape.ToCoordinates();
                var tods = ditem.Shape.ToCoordinates();
                if (cods.Count() == tods.Count())
                {
                    if (cods.Count() < 15)
                    {
                        var part = (int)Math.Truncate(cods.Count() / 3.0);

                        spoint.Add(cods[0]);
                        tpoint.Add(tods[0]);
                        spoint.Add(cods[part]);
                        tpoint.Add(tods[part]);
                        spoint.Add(cods[part * 2]);
                        tpoint.Add(tods[part * 2]);
                    }
                    else
                    {
                        var part = (int)Math.Truncate(cods.Count() / 6.0);
                        spoint.Add(cods[0]);
                        tpoint.Add(tods[0]);
                        spoint.Add(cods[part]);
                        tpoint.Add(tods[part]);
                        spoint.Add(cods[part * 2]);
                        tpoint.Add(tods[part * 2]);

                        spoint.Add(cods[part * 3]);
                        tpoint.Add(tods[part * 3]);
                        spoint.Add(cods[part * 4]);
                        tpoint.Add(tods[part * 4]);
                        spoint.Add(cods[part * 5]);
                        tpoint.Add(tods[part * 5]);

                    }
                }
            }
            bool isstot = false;
            bool result = true;
            if (isstot)
                result = ccc.ComplateParams(spoint, tpoint);
            else
                result = ccc.ComplateParams(tpoint, spoint);
            if (!result)
            {
                return null;
            }
            var data = new EnData()
            {
                Shape = enData.EnvelopShape,
                A0 = ccc.a0,
                A1 = ccc.a1,
                A2 = ccc.a2,
                B0 = ccc.b0,
                B1 = ccc.b1,
                B2 = ccc.b2,
            };

            return data;
        }

        /// <summary>
        /// 计算参数
        /// </summary>
        private bool ConvertDataToCoord(List<CodeData> sourdata, List<CodeData> targetdata, bool isstot)
        {
            var spoint = new List<Coordinate>();
            var tpoint = new List<Coordinate>();
            foreach (var sd in sourdata)
            {
                var ditem = targetdata.Find(t => t.Code == sd.Code);
                var cods = sd.Shape.ToCoordinates();
                var tods = ditem.Shape.ToCoordinates();
                if (cods.Count() == tods.Count())
                {
                    if (cods.Count() < 15)
                    {
                        var part = (int)Math.Truncate(cods.Count() / 3.0);

                        spoint.Add(cods[0]);
                        tpoint.Add(tods[0]);
                        spoint.Add(cods[part]);
                        tpoint.Add(tods[part]);
                        spoint.Add(cods[part * 2]);
                        tpoint.Add(tods[part * 2]);
                    }
                    else
                    {
                        var part = (int)Math.Truncate(cods.Count() / 6.0);

                        spoint.Add(cods[0]);
                        tpoint.Add(tods[0]);
                        spoint.Add(cods[part]);
                        tpoint.Add(tods[part]);
                        spoint.Add(cods[part * 2]);
                        tpoint.Add(tods[part * 2]);

                        spoint.Add(cods[part * 3]);
                        tpoint.Add(tods[part * 3]);
                        spoint.Add(cods[part * 4]);
                        tpoint.Add(tods[part * 4]);
                        spoint.Add(cods[part * 5]);
                        tpoint.Add(tods[part * 5]);

                    }
                }
            }
            if (isstot)
                return ccc.ComplateParams(spoint, tpoint);
            else
                return ccc.ComplateParams(tpoint, spoint);
        }

        public bool CoordnateCalc(List<CodeData> sourceData, List<CodeData> targetData,
        Dictionary<Envelope, List<string>> envolplist)
        {
            var sxcord = new List<double>();
            var sycord = new List<double>();
            var txcord = new List<double>();
            var tycord = new List<double>();

            List<CodeData> sourceData1 = new List<CodeData>();
            List<CodeData> targetData1 = new List<CodeData>();
            coordChangeParams = new List<CoordChangeParam>();

            var spoint = new List<Coordinate>();
            var tpoint = new List<Coordinate>();

            foreach (var item in envolplist)
            {
                spoint.Clear();
                tpoint.Clear();
                sourceData1.Clear();
                targetData1.Clear();
                var cp = new CoordChangeParam();
                cp.envelope = item.Key.ToGeometry();
                foreach (var sd in sourceData)
                {
                    if (item.Value.Contains(sd.Code))
                        sourceData1.Add(sd);
                }
                foreach (var sd in targetData)
                {
                    if (item.Value.Contains(sd.Code))
                        targetData1.Add(sd);
                }
                if (sourceData1.Count > 3)
                {
                    foreach (var sd1 in sourceData1)
                    {
                        var cod = sd1.Shape.ToCoordinates()[0];
                        spoint.Add(cod);

                        var ditem = targetData1.Find(t => t.Code == sd1.Code);

                        var tod = ditem.Shape.ToCoordinates()[0];
                        tpoint.Add(tod);
                    }
                }
                else
                {
                    foreach (var sd1 in sourceData1)
                    {
                        var cod = sd1.Shape.ToCoordinates();
                        spoint.Add(cod[0]);
                        spoint.Add(cod[1]);
                        spoint.Add(cod[2]);

                        var ditem = targetData1.Find(t => t.Code == sd1.Code);

                        var tod = ditem.Shape.ToCoordinates();
                        tpoint.Add(tod[0]);
                        tpoint.Add(tod[1]);
                        tpoint.Add(tod[2]);
                    }
                }
                if (ccc.ComplateParams(spoint, tpoint))
                {
                    cp.A0 = ccc.a0;
                    cp.A1 = ccc.a1;
                    cp.A2 = ccc.a2;

                    cp.B0 = ccc.b0;
                    cp.B1 = ccc.b1;
                    cp.B2 = ccc.b2;
                    if (cp.A0 == 0)
                    {

                    }
                    coordChangeParams.Add(cp);
                }
            }
            return true;
        }

        /// <summary>
        /// 计算转换结果；
        /// </summary>
        /// <param name="geo"></param>
        /// <returns></returns>
        public Coordinate[] CalcChangeByShape(YuLinTu.Spatial.Geometry geo)
        {
            var clist = geo.ToCoordinates();
            var palist = coordChangeParams.Where(t => t.envelope.Intersects(geo)).ToList();
            if (palist.Count == 0)
            {
                return clist;
            }
            foreach (var item in clist)
            {
                var g = Geometry.CreatePoint(item, geo.Srid);
                var pa = palist.Find(t => t.envelope.Contains(g));
                if (pa != null)
                {
                    var it = ccc.CalceDataAnd(item,
                        new double[] { pa.A0, pa.A1, pa.A2 },
                        new double[] { pa.B0, pa.B1, pa.B2 });
                    item.X = it.X;
                    item.Y = it.Y;
                }
                else
                {
                    pa = palist[0];
                    var it = ccc.CalceDataAnd(item,
                         new double[] { pa.A0, pa.A1, pa.A2 },
                         new double[] { pa.B0, pa.B1, pa.B2 });
                    item.X = it.X;
                    item.Y = it.Y;
                }
            }

            return clist;// new Coordinate(newcd.X, newcd.y);
        }

        /// <summary>
        /// 计算转换结果；
        /// </summary>
        /// <param name="geo"></param>
        /// <returns></returns>
        public Coordinate[] CalcDataByParams(Coordinate[] clist, EnData pa)
        {
            foreach (var item in clist)
            {
                var nc = CalcSingeData(item, pa);
                item.X = nc.X;
                item.Y = nc.Y;
            }
            return clist;// new Coordinate(newcd.X, newcd.y);
        }

        public Coordinate CalcSingeData(Coordinate data, EnData pa)
        {
            var it = ccc.CalceDataAnd(data,
                new double[] { pa.A0, pa.A1, pa.A2 },
                new double[] { pa.B0, pa.B1, pa.B2 });

            return new Coordinate(it.X, it.Y);
        }
    }

    public class ShapeCoordinateParam
    {
        public List<CodeData> DataList { get; set; }

        public ShapeCoordinateParam()
        {
            DataList = new List<CodeData>();
        }

        public void AddData(string code, Geometry geo)
        {
            var cd = new CodeData();
            cd.Code = code;
            //cd.Coord = geo.ToCoordinates()[0];
            DataList.Add(cd);
        }

    }

    public class CoordChangeParam
    {
        public Geometry envelope { get; set; }
        public double A0 { get; set; }
        public double A1 { get; set; }
        public double A2 { get; set; }
        public double B0 { get; set; }
        public double B1 { get; set; }
        public double B2 { get; set; }

    }
}
