using System.Collections.Generic;
using System.Linq;
using YuLinTu.Spatial;

namespace YuLinTu.Component.CoordinateTransformTask
{
    public class CoordChangeC
    {
        public double a0, a1, a2;
        public double b0, b1, b2;

        /*
         * X1=a0+a1*X+a2*Y;
         * Y1=b0+b1*X+b2*Y;
         */
        public CoordChangeC() { }

        /// <summary>
        /// 计算结果
        /// </summary> 
        public Coordinate CalceData(Coordinate cd)
        {
            var xd = a0 + a1 * cd.X + a2 * cd.Y;
            var yd = b0 + b1 * cd.X + b2 * cd.Y;
            return new Coordinate(xd, yd);
        }

        public Coordinate CalceDataAnd(Coordinate cd, double[] xp, double[] yp)
        {
            var xd = xp[0] + xp[1] * cd.X + xp[2] * cd.Y;
            var yd = yp[0] + yp[1] * cd.X + yp[2] * cd.Y;
            return new Coordinate(xd, yd);
        }


        /// <summary>
        /// 计算转换参数  
        /// </summary>
        /// <param name="scoordinates"></param>
        /// <param name="dcoordinates"></param>
        public bool ComplateParams(List<Coordinate> scoordinates, List<Coordinate> dcoordinates)
        {
            if (scoordinates.Count != dcoordinates.Count || scoordinates.Count < 3)
                return false;
            var groups = Enumerable.Range(0, (scoordinates.Count + 2) / 3).Select((i, index) => scoordinates.Skip(index * 3).Take(3)).ToList(); // 根据索引计算每个组的起始位置并取前三个元素作为一组
            var groupd = Enumerable.Range(0, (dcoordinates.Count + 2) / 3).Select((i, index) => dcoordinates.Skip(index * 3).Take(3)).ToList(); // 根据索引计算每个组的起始位置并取前三个元素作为一组

            int count = groups.Count > 5 ? groups.Count - 2 : groups.Count;
            List<double[]> allparam = new List<double[]>();
            bool iscomplate = false;
            for (var i = 0; i < count; i++)
            {
                var gps = groups[i];
                var gpd = groupd[i];
                var gpsArray = gps.ToArray();
                var gpdArray = gpd.ToArray();
                if (gpsArray.Count() < 3)
                    continue;
                if (gpsArray[0].X == gpsArray[1].X || gpsArray[2].X == gpsArray[1].X || gpsArray[0].X == gpsArray[2].X)
                    continue;
                var xparams = ParamsCalcX(gpsArray, gpdArray);
                var yparams = ParamsCalcY(gpsArray, gpdArray);
                if (xparams == null || yparams == null)
                {
                    gpsArray = new Coordinate[3] { gpsArray[2], gpsArray[0], gpdArray[1] };
                    gpdArray = new Coordinate[3] { gpdArray[2], gpdArray[0], gpdArray[1] };
                    xparams = ParamsCalcX(gpsArray, gpdArray);
                    yparams = ParamsCalcY(gpsArray, gpdArray);
                }
                if (xparams == null || yparams == null)
                {
                    continue;
                }
                if (xparams[0].ToString() == "NaN" || xparams[1].ToString() == "NaN" || xparams[2].ToString() == "NaN"
                     || (yparams[0].ToString() == "NaN" || yparams[1].ToString() == "NaN" || yparams[2].ToString() == "NaN"))
                {
                    continue;
                }
                iscomplate = true;
                double[] dps = new double[6];
                allparam.Add(dps);

                dps[0] = xparams[0];
                dps[1] = xparams[1];
                dps[2] = xparams[2];
                dps[3] = yparams[0];
                dps[4] = yparams[1];
                dps[5] = yparams[2];
            }
            if (!iscomplate)
                return false;
            FixListData(allparam);
            FixListData(allparam);

            a0 = allparam.Sum(t => t[0]) / allparam.Count;//0.08243254837616952
            a1 = allparam.Sum(t => t[1]) / allparam.Count;//0.99824265558976388
            a2 = allparam.Sum(t => t[2]) / allparam.Count;//0.0032323548064533084

            b0 = allparam.Sum(t => t[3]) / allparam.Count;//-0.062789010852089433
            b1 = allparam.Sum(t => t[4]) / allparam.Count;//-0.00028000429725978714
            b2 = allparam.Sum(t => t[5]) / allparam.Count;//1.0029634645786978
            return true;
        }

        private void FixListData(List<double[]> allparam)
        {
            allparam.RemoveAll(t => t[0] > 10000 || t[0] < -10000);
            List<double> valulis = new List<double>();
            for (int i = 0; i < allparam.Count - 1; i++)
            {
                valulis.Add(System.Math.Abs(allparam[i][0] - allparam[i + 1][0]));
            }
            List<int> removeindex = new List<int>();
            for (int i = 0; i < valulis.Count; i++)
            {
                if (valulis[i] <= 600)
                {
                    continue;
                }
                else if (i == 0)
                {
                    if (valulis.Count > 1 && valulis[i + 1] <= 600)
                    {
                        removeindex.Add(0);
                        continue;
                    }
                }
                else if (i == valulis.Count - 1)
                {
                    removeindex.Add(i + 1);
                    continue;
                }
                if (valulis[i] > 600 && valulis.Count > 1 && valulis[i + 1] > 600)
                {
                    removeindex.Add(i + 1);
                }
            }
            for (int i = removeindex.Count - 1; i >= 0; i--)
            {
                allparam.RemoveAt(removeindex[i]);
            }
        }

        private double[] ParamsCalcX(Coordinate[] scdarray, Coordinate[] dcdarray)
        {
            var x1 = scdarray[0].X;
            var x2 = scdarray[1].X;
            var x3 = scdarray[2].X;

            var Y1 = scdarray[0].Y;
            var Y2 = scdarray[1].Y;
            var Y3 = scdarray[2].Y;

            var x1d = dcdarray[0].X;
            var x2d = dcdarray[1].X;
            var x3d = dcdarray[2].X;

            var xcz1 = (x2d - x1d) / (x2 - x1);
            var xcz2 = (x3d - x2d) / (x3 - x2);
            var ycz1 = (Y2 - Y1) / (x2 - x1);
            var ycz2 = (Y3 - Y2) / (x3 - x2);


            var xcd1 = (x3d - x2d) / (Y3 - Y2);
            var xcd2 = (x2d - x1d) / (Y2 - Y1);
            var ycd1 = (x3 - x2) / (Y3 - Y2);
            var ycd2 = (x2 - x1) / (Y2 - Y1);

            //X1=a0+a1*X+a2*Y;
            var a1 = (xcd1 - xcd2) / (ycd1 - ycd2);
            var a2 = (xcz1 - xcz2) / (ycz1 - ycz2);
            var a0 = x1d - a1 * x1 - a2 * Y1;
            if (a0 > 5000 || a0 < -5000)
                return null;

            return new double[] { a0, a1, a2 };
        }

        /*
         *  X1=a0+a1*X+a2*Y;
         *  Y1=b0+b1*X+b2*Y;
         */
        /// <param name="scdarray"></param>
        /// <param name="dcdarray"></param>
        /// <returns></returns>
        private double[] ParamsCalcY(Coordinate[] scdarray, Coordinate[] dcdarray)
        {
            var x1 = scdarray[0].X;
            var x2 = scdarray[1].X;
            var x3 = scdarray[2].X;

            var Y1 = scdarray[0].Y;
            var Y2 = scdarray[1].Y;
            var Y3 = scdarray[2].Y;

            var y1d = dcdarray[0].Y;
            var y2d = dcdarray[1].Y;
            var y3d = dcdarray[2].Y;


            var xcz1 = (y2d - y1d) / (x2 - x1);
            var xcz2 = (y3d - y2d) / (x3 - x2);
            var ycz1 = (Y2 - Y1) / (x2 - x1);
            var ycz2 = (Y3 - Y2) / (x3 - x2);


            var xcd1 = (y3d - y2d) / (Y3 - Y2);
            var xcd2 = (y2d - y1d) / (Y2 - Y1);
            var ycd1 = (x3 - x2) / (Y3 - Y2);
            var ycd2 = (x2 - x1) / (Y2 - Y1);

            //X1=a0+a1*X+a2*Y;
            var a1 = (xcd1 - xcd2) / (ycd1 - ycd2);
            var a2 = (xcz1 - xcz2) / (ycz1 - ycz2);
            var a0 = y1d - a1 * x1 - a2 * Y1;
            if (a0 > 5000 || a0 < -5000)
                return null;
            return new double[] { a0, a1, a2 };
        }
    }
}