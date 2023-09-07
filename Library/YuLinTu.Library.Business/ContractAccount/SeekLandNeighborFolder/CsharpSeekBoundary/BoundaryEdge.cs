using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundOrientation
{
    public class BoundaryEdge : EdgeBase
    {
        public string position = "";
        public string boundary_orientation;
        public string boundary_simple_orientation = "";
        public string name;

        public string closest_first_result = "";  /// 最毗邻地物
        public Dictionary<string, int> closest_result = new Dictionary<string, int>();         ///最毗邻地物列表 
        public List<Edge> edge_list = new List<Edge>();
        public Dictionary<string, int> close_filed_list = new Dictionary<string, int>();
        public List<Point> pointList;

        public double y_max = 0;
        public double y_min = double.MinValue;
        public double x_max = 0;
        public double x_min = double.MinValue;
        public double length = 0;

        /* ""'
         多边形的界址边的类
         ""'
         */
        public BoundaryEdge(string name, List<Point> pointList, string ID)
        {
            edge_list.Clear();
            this.name = name;
            this.x_min = pointList[0].X;
            this.y_min = pointList[0].Y;
            this.pointList = pointList;
            length = 0;
            for (int n = 0; n < pointList.Count - 1; n++)
            {
                length += Math.Sqrt(Math.Pow((pointList[n + 1].Y - pointList[n].Y), 2) +
                    Math.Pow((pointList[n + 1].X - pointList[n].X), 2));
                double delta_x = 0;
                if (pointList[n].Y > pointList[n + 1].Y)/// 如果第n个点较大
                {
                    var y_max_t = pointList[n].Y;
                    var y_min_t = pointList[n + 1].Y;
                    var x_difference = pointList[n + 1].X - pointList[n].X;
                    if (x_difference != 0)
                    {
                        delta_x = (y_min_t - y_max_t) / x_difference;
                    }
                    else
                    {
                        delta_x = 0;
                    }
                    var point1 = pointList[n];
                    var point2 = pointList[n + 1];
                    edge_list.Add(new Edge(y_max_t, y_min_t, delta_x, point1, point2, ID));
                    if (y_max_t > this.y_max)
                    {
                        this.y_max = y_max_t;
                    }
                    if (y_min_t < this.y_min)
                    {
                        this.y_min = y_min_t;
                    }
                    var x_max_C = OrientationHelper.Max(pointList[n + 1].X, pointList[n].X);
                    if (x_max_C > this.x_max)
                    {
                        this.x_max = x_max_C;
                    }

                    var x_min_C = OrientationHelper.Min(pointList[n + 1].X, pointList[n].X);
                    if (x_min_C < this.x_min)
                    {
                        this.x_min = x_min_C;
                    }
                }
                else
                {  /// 如果第n+1个点较大{
                    var y_max_t = pointList[n + 1].Y;
                    var y_min_t = pointList[n].Y;
                    var x_difference = pointList[n].X - pointList[n + 1].X;
                    if (x_difference != 0)
                    {
                        delta_x = (y_min_t - y_max_t) / x_difference;
                    }
                    else
                    {
                        delta_x = 0;
                    }
                    var point1 = pointList[n + 1];
                    var point2 = pointList[n];
                    edge_list.Add(new Edge(y_max_t, y_min_t, delta_x, point1, point2, ID));
                    if (y_max_t > this.y_max)
                    {
                        this.y_max = y_max_t;
                    }
                    if (y_min_t < this.y_min)
                    {
                        this.y_min = y_min_t;
                    }
                    var x_max_C = OrientationHelper.Max(pointList[n + 1].X, pointList[n].X);
                    if (x_max_C > this.x_max)
                    {
                        this.x_max = x_max_C;
                    }
                    var x_min_C = OrientationHelper.Min(pointList[n + 1].X, pointList[n].X);
                    if (x_min_C < this.x_min)
                    {
                        this.x_min = x_min_C;
                    }
                }
            }
            this.boundary_orientation = Get_edge_orientation(pointList[0], pointList[pointList.Count - 1]);

            /// 将界址线的方向简化为东南，东北，西南，西北
            /// E, F, G, H分别为为东南，东北，西南，西北
            if (this.boundary_orientation == A || this.boundary_orientation == B ||
                this.boundary_orientation == C || this.boundary_orientation == D)
            {
                this.boundary_simple_orientation = this.boundary_orientation;
            }
            if (this.boundary_orientation == I || this.boundary_orientation == J)
            {
                this.boundary_simple_orientation = E;
            }
            if (this.boundary_orientation == M || this.boundary_orientation == N)
            {
                this.boundary_simple_orientation = G;
            }
            if (this.boundary_orientation == K || this.boundary_orientation == L)
            {
                this.boundary_simple_orientation = F;
            }
            if (this.boundary_orientation == O || this.boundary_orientation == P)
            {
                this.boundary_simple_orientation = H;
            }
        }

        /// <summary>
        /// 计算方位
        /// </summary>
        public string Get_edge_orientation(Point point1, Point point2)
        {
            /*  以第一个点和最后一个点为基准，计算界址线段的方位
            { param point_list{ 坐标点列表，以顺时针排序
            { return{ 该界址线段的方位
                A, B, C, D：分别为东南西北。
              I,J,分别为为东南(偏东)，东南(偏南)
              K,L,分别为为东北(偏东)，东北(偏北)
              M,N,分别为为西南(偏西)，西南(偏南)
              O,P,分别为为西北(偏西)，西北(偏北)
              '''*/
            var orientation = "";
            var x1 = point1.X;
            var y1 = point1.Y;
            var x2 = point2.X;
            var y2 = point2.Y;
            ///避免除数为0
            if (x1 == x2)
            {
                x1 += 0.0001;
            }
            var tan = (y2 - y1) / (x2 - x1);        ///计算正切值

            if (-0.414 <= tan && tan < 0.414)
            {
                /// 正东方和正西的角度为-22.5到+22.5. 对应的正切为：-0.414 21 到0.414 21 之间
                if (x2 >= x1)
                {
                    orientation = A;     ///正东方
                }
                else
                {
                    orientation = C;      ///正西方
                }
            }
            else if (0.414 <= tan && tan < 1)
            {
                /// 东北（偏东）和西南（偏西）的角度为+22.5到+45. 对应的正切为：0.414 21 到1之间
                if (x2 >= x1)
                {
                    orientation = K;      ///东北（偏东）
                }
                else
                {
                    orientation = M;     ///西南（偏西）
                }
            }
            else if (1 <= tan && tan < 2.414)
            {
                /// 东北（偏北）和西南（偏南）的角度为+45到+67.5. 对应的正切为：1 到2.414 2 之间
                if (x2 >= x1)
                {
                    orientation = L;      ///东北（偏北）
                }
                else
                {
                    orientation = N;     ///西南（偏南）
                }
            }
            else if (2.414 <= tan || tan < -2.414)
            {
                /// 正北方和正南的角度为+67.5到112.5. 对应的正切为：大于2.414 2，或者小于-2.414 2 之间
                if (y2 >= y1)
                {
                    orientation = D;    ///正北方
                }
                else
                {
                    orientation = B;    ///正南方
                }
            }
            else if (-2.414 <= tan && tan < -1)
            {
                /// 西北(偏北)和东南(偏南)方向的角度为112.5到135. 对应的正切为：-2.414 2和-1之间。
                if (y2 >= y1)
                {
                    orientation = P;    ///西北（偏北）
                }
                else
                {
                    orientation = J;     ///东南(偏南)
                }
            }
            else if (-1 <= tan && tan < -0.414)
            {
                /// 西北(偏西)和东南(偏东)方向的角度为135到157.5. 对应的正切为：-1和-0.414 21之间。
                if (y2 >= y1)
                {
                    orientation = O;   ///西北(偏西)
                }
                else
                {
                    orientation = I;     ///东南(偏东)
                }
            }
            return orientation;
        }
    }
}
