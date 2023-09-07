using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundOrientation
{
    /// <summary>
    /// 点
    /// </summary>
    public struct Point
    {
        public double X;
        public double Y;

        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point(string x, string y)
        {
            var xvalue = double.Parse(x);
            var yvalue = double.Parse(y);
            this.X = xvalue;
            this.Y = yvalue;
        }
    }

    public class Entitybase : ICloneable
    {
        /// <summary>
        /// 线状地物标识
        /// </summary>
        public const char X = 'X';

        /// <summary>
        /// 面状地物标识
        /// </summary>
        public const char M = 'M';

        public string id;
        public string type;

        public double y_max;
        public double y_min;
        public double x_max;
        public double x_min;

        /// <summary>
        /// 取Sin值
        /// </summary>
        protected double Sin()
        {
            var dis = Distance(x_min, y_min, x_max, y_max);
            if (dis == 0)
                return 0;
            var sin = (this.y_max - this.y_min) / dis;
            return sin;
        }


        /// <summary>
        /// 取Cos值
        /// </summary>
        protected double Cos()
        {
            var dis = Distance(x_min, y_min, x_max, y_max);
            if (dis == 0)
                return 0;
            var cos = (this.x_max - this.x_min) / dis;
            return cos;
        }

        /// <summary>
        /// 距离
        /// </summary>
        /// <returns></returns>
        protected double Distance(double xmin, double ymin, double xmax, double ymax)
        {
            var yPow = Math.Pow(ymax - ymin, 2);
            var xPow = Math.Pow(xmax - xmin, 2);
            return Math.Sqrt(yPow + xPow);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    // 多边形的边的类 
    public class Edge : Entitybase
    {
        public double delta_x;
        public Point point1;
        public Point point2;
        public double sin = 0;
        public double cos = 0;
        public double true_sin = 0;
        public double true_cos = 0;

        /// <summary>
        /// 类的构造函数 
        /// </summary>
        /// <param name="y_max"></param>
        /// <param name="y_min"></param>
        /// <param name="delta_x">两个点的坐标x的差值</param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public Edge(double y_max, double y_min, double delta_x, Point point1, Point point2, string id, string type)
        {

            this.y_max = y_max;
            this.y_min = y_min;
            this.x_max = OrientationHelper.Max(point1.X, point2.X);
            this.x_min = OrientationHelper.Min(point1.X, point2.X);
            this.delta_x = delta_x;
            this.point1 = point1;
            this.point2 = point2;
            this.id = id;
            this.type = type;
            SetAngle();
        }

        /// <summary>
        /// <param name="delta_x">两个点的坐标x的差值</param>
        /// </summary>
        public Edge(double y_max, double y_min, double delta_x, Point point1, Point point2, string id)
        {
            this.y_max = y_max;
            this.y_min = y_min;
            this.x_max = OrientationHelper.Max(point1.X, point2.X);
            this.x_min = OrientationHelper.Min(point1.X, point2.X);
            this.delta_x = delta_x;
            this.point1 = point1;
            this.point2 = point2;
            this.id = id;
            SetAngle();
        }

        private void SetAngle()
        {
            try
            {
                sin = Sin();
                this.true_sin = sin;
                if (sin < 0.55)
                    this.sin = 0;
                else if (sin < 0.8)
                    this.sin = sin / 10;

                cos = Cos();
                this.true_cos = cos;
                if (cos < 0.55)
                    this.cos = 0;
                else if (cos < 0.8)
                    this.cos = cos / 10;
            }
            catch
            {
                this.sin = 0;
                this.cos = 0;
            }
        }
    }

    public class TypePloy
    {
        public const string Others = "others";
        public const string Field = "field";
    }

    /*多边形的类*/
    public class Poly : Entitybase
    {
        public int edgeNum;
        public Point center_point;
        public List<Edge> edge;
        public List<List<Point>> pointList;

        public Poly(string ID, List<List<Point>> point_array_list)
        {
            /*
            类的构造函数
            edge的格式为[ymax, ymin, delta - x, point1(y较大), point2(y较小)]
            注意这里：x_new=x_old减去delta-x
            {param pointList{库多边形得各顶点的坐标列表
            {return{
            */
            edge = new List<Edge>();
            this.y_max = 0;
            this.y_min = double.MaxValue;
            this.x_max = 0;
            this.x_min = double.MaxValue;
            this.id = ID;


            if (ID[0] == X || ID[0] == M)
            {
                this.type = TypePloy.Others;
            }
            else
            {
                this.type = TypePloy.Field;
            }


            //if (ID[0] != X)   //如果不是线状物，则将第一个点添加到点列表中。最后一个点会和第一个点组成一个线段
            //{
            //    foreach (var pointList in point_array_list)
            //    {
            //        pointList.Add(pointList[0]);
            //    }
            //}
            foreach (var pointList in point_array_list)
            {
                var firstPoint = pointList[0];
                var lastPoint = pointList[pointList.Count - 1];
                if (firstPoint.X != lastPoint.X && firstPoint.Y != lastPoint.Y)
                    pointList.Add(pointList[0]);
                for (int n = 0; n < pointList.Count - 1; n++)
                {
                    if (pointList[n].Y > pointList[n + 1].Y)
                    {  //如果第n个点较大
                        var y_max = pointList[n].Y;
                        var y_min = pointList[n + 1].Y;
                        double delta_x = 0;
                        var x_difference = pointList[n + 1].X - pointList[n].X;
                        if (x_difference != 0)
                        {
                            delta_x = (y_min - y_max) / x_difference;
                        }
                        else
                        {
                            delta_x = 0;
                        }
                        var point1 = pointList[n];
                        var point2 = pointList[n + 1];
                        this.edge.Add(new Edge(y_max, y_min, delta_x, point1, point2, ID, type));
                        if (y_max > this.y_max)
                        {
                            this.y_max = y_max;
                        }
                        if (y_min < this.y_min)
                        {
                            this.y_min = y_min;
                        }
                        if (OrientationHelper.Max(pointList[n + 1].X, pointList[n].X) > this.x_max)
                        {
                            this.x_max = OrientationHelper.Max(pointList[n + 1].X, pointList[n].X);
                        }
                        if (OrientationHelper.Min(pointList[n + 1].X, pointList[n].X) < this.x_min)
                        {
                            this.x_min = OrientationHelper.Min(pointList[n + 1].X, pointList[n].X);
                        }
                    }
                    else
                    {   //如果第n+1个点较大
                        var y_max = pointList[n + 1].Y;
                        var y_min = pointList[n].Y;
                        var x_difference = pointList[n].X - pointList[n + 1].X;
                        double delta_x = 0;
                        if (x_difference != 0)
                        {
                            delta_x = (y_min - y_max) / x_difference;
                        }
                        else
                        {
                            delta_x = 0;
                        }
                        var point1 = pointList[n + 1];
                        var point2 = pointList[n];
                        this.edge.Add(new Edge(y_max, y_min, delta_x, point1, point2, ID, this.type));
                        if (y_max > this.y_max)
                        {
                            this.y_max = y_max;
                        }
                        if (y_min < this.y_min)
                        {
                            this.y_min = y_min;
                        }
                        if (OrientationHelper.Max(pointList[n + 1].X, pointList[n].X) > this.x_max)
                        {
                            this.x_max = OrientationHelper.Max(pointList[n + 1].X, pointList[n].X);
                        }
                        if (OrientationHelper.Min(pointList[n + 1].X, pointList[n].X) < this.x_min)
                        {
                            this.x_min = OrientationHelper.Min(pointList[n + 1].X, pointList[n].X);
                        }
                    }
                }
                this.center_point = new Point((this.x_max + this.x_min) / 2, (this.y_min + this.y_max) / 2);
                this.edgeNum = this.edge.Count;
                this.pointList = point_array_list;
            }
        }
    }
}