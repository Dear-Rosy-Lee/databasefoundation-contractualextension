using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundOrientation
{
    public class EdgeBase
    {
        /*A, B, C, D：分别为东南西北。
             I,J,分别为为东南(偏东)，东南(偏南)
             K,L,分别为为东北(偏东)，东北(偏北)
             M,N,分别为为西南(偏西)，西南(偏南)
             O,P,分别为为西北(偏西)，西北(偏北)*/

        public const string A = "A";
        public const string B = "B";
        public const string C = "C";
        public const string D = "D";

        /// <summary>
        /// E, F, G, H分别为为东南，东北，西南，西北
        /// </summary>
        public const string E = "E";
        public const string F = "F";
        public const string G = "G";
        public const string H = "H";

        public const string I = "I";
        public const string J = "J";

        public const string K = "K";
        public const string L = "L";

        public const string M = "M";
        public const string N = "N";

        public const string O = "O";
        public const string P = "P";

        //private const string M = "M";///中
        //private const string I = "I";///内
        //private const string O = "O";///外

        /*'''
        向东发出射线，并返回相近的边的列表
        :param point:测试点
        :param boundary_edge：界址边
        :param test_edge_list:多边形的对象的列表
        :param projection_length: 缓冲距离
        :return:cross_point_target：界址边上发出射线的点
        close_obj_id_list：相近的毗邻地物
        closest_id: 最近的毗邻地物
        '''*/
        public Point project_east(double x, double y, BoundaryEdge boundary_edge, List<Edge> test_edge_list, double projection_length,
             List<string> close_obj_id_list, ref string closest_id)
        {
            var rep = new Point(-1, -1);
            var cross_point_target = new Point(x, y);
            //var close_obj_id_list = new List<string>();
            //string closest_id = null;
            double x_cross = 0;
            double closest_x = double.MaxValue;
            ///如果点在多边形的上方或者下方，则肯定在多边形外，且无交点：
            if (cross_point_target.Y < boundary_edge.y_min || cross_point_target.Y > boundary_edge.y_max)
            {
                return rep;// false,[],None;
            }

            foreach (var boundary_edge_element in boundary_edge.edge_list)
            {
                ///如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
                if (cross_point_target.Y > boundary_edge_element.y_max || cross_point_target.Y < boundary_edge_element.y_min)
                {
                    continue;
                }
                else
                {
                    ///计算线段与水平直线的交点
                    var y0 = cross_point_target.Y;
                    var x1 = boundary_edge_element.point1.X;
                    var y1 = boundary_edge_element.point1.Y;
                    var x2 = boundary_edge_element.point2.X;
                    var y2 = boundary_edge_element.point2.Y;
                    var y_difference = y2 - y1;
                    ///如果线段不是水平线
                    if (y_difference != 0)
                        x_cross = (y0 * (x2 - x1) - (x2 * y1 - x1 * y2)) / y_difference;
                    ///如果交点在右侧（射线为往右的方向）
                    if (x_cross > cross_point_target.X)
                        cross_point_target.X = x_cross;
                }
            }
            /// print cross_point_target
            if (cross_point_target.X == x)
                return rep;
            else
            {
                for (var i = 0; i < test_edge_list.Count; i++)
                {
                    if (cross_point_target.Y > test_edge_list[i].y_max ||
                        cross_point_target.Y < test_edge_list[i].y_min)
                        continue;
                    else
                    {
                        /// 计算线段与水平直线的交点
                        var y0 = cross_point_target.Y;
                        var x1 = test_edge_list[i].point1.X;
                        var y1 = test_edge_list[i].point1.Y;
                        var x2 = test_edge_list[i].point2.X;
                        var y2 = test_edge_list[i].point2.Y;
                        var y_difference = (y2 - y1);
                        /// 如果线段不是水平线
                        if (y_difference != 0)
                        {
                            x_cross = (y0 * (x2 - x1) - (x2 * y1 - x1 * y2)) / y_difference;
                            /// 如果交点在右侧的缓冲距离之内（射线为往右的方向）
                            if (x_cross >= cross_point_target.X - 0.2 &&
                                x_cross < cross_point_target.X + projection_length)
                            {
                                close_obj_id_list.Add(test_edge_list[i].id);
                                if (x_cross < closest_x)
                                {
                                    closest_x = x_cross;
                                    closest_id = test_edge_list[i].id;
                                }
                            }
                        }
                    }
                }
            }
            return cross_point_target;
            //return cross_point_target, close_obj_id_list, closest_id;
        }


        /*'''
        向西发出射线，并返回相近的边的列表
        :param point:测试点
        :param boundary_edge：界址边
        :param test_edge_list:多边形的对象的列表
        :param projection_length: 缓冲距离
        :return:cross_point_target：界址边上发出射线的点
        close_obj_id_list：相近的毗邻地物
        closest_id: 最近的毗邻地物
        '''*/
        public Point project_west(double x, double y, BoundaryEdge boundary_edge, List<Edge> test_edge_list, double projection_length,
             List<string> close_obj_id_list, ref string closest_id)
        {
            var rep = new Point(-1, -1);
            var cross_point_target = new Point(x, y);
            //var close_obj_id_list = new List<string>();
            //string closest_id = null;
            //double x_cross = 0;
            double closest_x = 0;
            ///如果点在多边形的上方或者下方，则肯定在多边形外，且无交点：
            if (cross_point_target.Y < boundary_edge.y_min || cross_point_target.Y > boundary_edge.y_max)
            {
                return rep;// false,[],None;
            }

            foreach (var boundary_edge_element in boundary_edge.edge_list)
            {
                ///如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
                if (cross_point_target.Y > boundary_edge_element.y_max || cross_point_target.Y < boundary_edge_element.y_min)
                {
                    continue;
                }
                else
                {
                    ///计算线段与水平直线的交点
                    var y0 = cross_point_target.Y;
                    var x1 = boundary_edge_element.point1.X;
                    var y1 = boundary_edge_element.point1.Y;
                    var x2 = boundary_edge_element.point2.X;
                    var y2 = boundary_edge_element.point2.Y;
                    var y_difference = y2 - y1;
                    ///如果线段不是水平线
                    if (y_difference != 0)
                    {
                        var x_cross = (y0 * (x2 - x1) - (x2 * y1 - x1 * y2)) / y_difference;
                        ///如果交点在左侧（射线为往左的方向）
                        if (x_cross < cross_point_target.X)
                            cross_point_target.X = x_cross;
                    }
                }
            }
            /// print cross_point_target
            if (cross_point_target.X == x)
                return rep;
            else
            {
                for (var i = 0; i < test_edge_list.Count; i++)
                {
                    if (cross_point_target.Y > test_edge_list[i].y_max || cross_point_target.Y < test_edge_list[i].y_min)
                        continue;
                    else
                    {
                        /// 计算线段与水平直线的交点
                        var y0 = cross_point_target.Y;
                        var x1 = test_edge_list[i].point1.X;
                        var y1 = test_edge_list[i].point1.Y;
                        var x2 = test_edge_list[i].point2.X;
                        var y2 = test_edge_list[i].point2.Y;
                        var y_difference = (y2 - y1);
                        /// 如果线段不是水平线
                        if (y_difference != 0)
                        {
                            var x_cross = (y0 * (x2 - x1) - (x2 * y1 - x1 * y2)) / y_difference;
                            /// 如果交点在右侧的缓冲距离之内（射线为往右的方向）
                            if (x_cross <= cross_point_target.X + 0.2 &&
                                x_cross > cross_point_target.X - projection_length)
                            {
                                close_obj_id_list.Add(test_edge_list[i].id);
                                if (x_cross > closest_x)
                                {
                                    closest_x = x_cross;
                                    closest_id = test_edge_list[i].id;
                                }
                            }
                        }
                    }
                }
            }
            return cross_point_target;
            //return cross_point_target, close_obj_id_list, closest_id;
        }


        /*'''
        向北发出射线，并返回相近的边的列表
        :param point:测试点
        :param boundary_edge：界址边
        :param test_edge_list:多边形的对象的列表
        :param projection_length: 缓冲距离
        :return:cross_point_target：界址边上发出射线的点
        close_obj_id_list：相近的毗邻地物
        closest_id: 最近的毗邻地物
        '''*/
        public Point project_north(double x, double y, BoundaryEdge boundary_edge, List<Edge> test_edge_list, double projection_length,
             List<string> close_obj_id_list, ref string closest_id)
        {
            var rep = new Point(-1, -1);
            var cross_point_target = new Point(x, y);
            //var close_obj_id_list = new List<string>();
            //string closest_id = null;
            double closest_y = double.MaxValue;
            ///如果点在多边形的左方或者右方，则肯定在多边形外，且无交点：
            if (cross_point_target.X < boundary_edge.x_min || cross_point_target.X > boundary_edge.x_max)
            {
                return rep;// false,[],None;
            }

            foreach (var boundary_edge_element in boundary_edge.edge_list)
            {
                ///如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
                if (cross_point_target.X > boundary_edge_element.x_max || cross_point_target.X < boundary_edge_element.x_min)
                {
                    continue;
                }
                else
                {
                    ///计算线段与水平直线的交点
                    var x0 = cross_point_target.X;
                    var x1 = boundary_edge_element.point1.X;
                    var y1 = boundary_edge_element.point1.Y;
                    var x2 = boundary_edge_element.point2.X;
                    var y2 = boundary_edge_element.point2.Y;
                    var x_difference = x2 - x1;
                    ///如果线段不是水平线
                    if (x_difference != 0)
                    {
                        var y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference;
                        ///如果交点在上侧（射线为往北的方向）
                        if (y_cross > cross_point_target.Y)
                            cross_point_target.Y = y_cross;
                    }
                }
            }
            /// print cross_point_target
            if (cross_point_target.Y == y)
                return rep;
            else
            {
                for (var i = 0; i < test_edge_list.Count; i++)
                {
                    if (cross_point_target.X > test_edge_list[i].x_max ||
                        cross_point_target.X < test_edge_list[i].x_min)
                        continue;
                    else
                    {
                        /// 计算线段与水平直线的交点
                        var x0 = cross_point_target.X;
                        var x1 = test_edge_list[i].point1.X;
                        var y1 = test_edge_list[i].point1.Y;
                        var x2 = test_edge_list[i].point2.X;
                        var y2 = test_edge_list[i].point2.Y;
                        var x_difference = (x2 - x1);
                        /// 如果线段不是水平线
                        if (x_difference != 0)
                        {
                            var y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference;
                            /// 如果交点在上侧的缓冲距离之内（射线为往北的方向）
                            if (y_cross >= cross_point_target.Y - 0.2 &&
                                y_cross < cross_point_target.Y + projection_length)
                            {
                                close_obj_id_list.Add(test_edge_list[i].id);
                                if (y_cross < closest_y)
                                {
                                    closest_y = y_cross;
                                    closest_id = test_edge_list[i].id;
                                }
                            }
                        }
                    }
                }
            }
            return cross_point_target;
            //return cross_point_target, close_obj_id_list, closest_id;
        }

        /*'''
        向南发出射线，并返回相近的边的列表
        :param point:测试点
        :param boundary_edge：界址边
        :param test_edge_list:多边形的对象的列表
        :param projection_length: 缓冲距离
        :return:cross_point_target：界址边上发出射线的点
        close_obj_id_list：相近的毗邻地物
        closest_id: 最近的毗邻地物
        '''*/
        public Point project_south(double x, double y, BoundaryEdge boundary_edge, List<Edge> test_edge_list, double projection_length,
             List<string> close_obj_id_list, ref string closest_id)
        {
            var rep = new Point(-1, -1);
            var cross_point_target = new Point(x, y);
            //var close_obj_id_list = new List<string>();
            //string closest_id = null;
            //double y_cross = double.MinValue;
            double closest_y = 0;
            ///如果点在多边形的左方或者右方，则肯定在多边形外，且无交点：
            if (cross_point_target.X < boundary_edge.x_min ||
                cross_point_target.X > boundary_edge.x_max)
            {
                return rep;// false,[],None;
            }

            foreach (var boundary_edge_element in boundary_edge.edge_list)
            {
                ///如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
                if (cross_point_target.X > boundary_edge_element.x_max ||
                    cross_point_target.X < boundary_edge_element.x_min)
                {
                    continue;
                }
                else
                {
                    ///计算线段与水平直线的交点
                    var x0 = cross_point_target.X;
                    var x1 = boundary_edge_element.point1.X;
                    var y1 = boundary_edge_element.point1.Y;
                    var x2 = boundary_edge_element.point2.X;
                    var y2 = boundary_edge_element.point2.Y;
                    var x_difference = x2 - x1;
                    ///如果线段不是水平线
                    if (x_difference != 0)
                    {
                        var y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference;
                        ///如果交点在下侧（射线为往南的方向）
                        if (y_cross < cross_point_target.Y)
                            cross_point_target.Y = y_cross;
                    }
                }
            }
            /// print cross_point_target
            if (cross_point_target.Y == y)
                return rep;
            else
            {
                for (var i = 0; i < test_edge_list.Count; i++)
                {
                    if (cross_point_target.X > test_edge_list[i].x_max ||
                        cross_point_target.X < test_edge_list[i].x_min)
                        continue;
                    else
                    {
                        /// 计算线段与水平直线的交点
                        var x0 = cross_point_target.X;
                        var x1 = test_edge_list[i].point1.X;
                        var y1 = test_edge_list[i].point1.Y;
                        var x2 = test_edge_list[i].point2.X;
                        var y2 = test_edge_list[i].point2.Y;
                        var x_difference = (x2 - x1);
                        /// 如果线段不是水平线
                        if (x_difference != 0)
                        {
                            var y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference;
                            /// 如果交点在右侧的缓冲距离之内（射线为往右的方向）
                            if (y_cross <= cross_point_target.Y + 0.2 &&
                                y_cross > cross_point_target.Y - projection_length)
                            {
                                close_obj_id_list.Add(test_edge_list[i].id);
                                if (y_cross > closest_y)
                                {
                                    closest_y = y_cross;
                                    closest_id = test_edge_list[i].id;
                                }
                            }
                        }
                    }
                }
            }
            return cross_point_target;
            //return cross_point_target, close_obj_id_list, closest_id;
        }

        /*'''
        实现界址边的东向投影算法
        :param boundary_edge: 界址边对象
        :param edge_poly_list: 测试多边形的边对象列表
        :param test_poly_objs: 测试多边形的对象列表
        :param distance: 投影线的投影长度，和缓冲距离类似
        :return:
        '''*/
        public void get_east_close_field(BoundaryEdge boundary_edge, List<Edge> edge_poly_list,
           List<BoundaryPoly> test_poly_objs, double distance,
           Dictionary<string, int> cross_result, Dictionary<string, int> closest_result)
        {
            //var cross_result = new Dictionary<string, int>();
            //var closest_result = new Dictionary<string, int>();
            
            if (boundary_edge.length < 0.0001)
            {
                //重复点排除
                YuLinTu.Library.Log.Log.WriteException(this, "根据四至更新地块", "根据四至更新地块界址信息点位有误,对应X坐标分别为:" + boundary_edge.x_max.ToString() + "和" + boundary_edge.x_min.ToString() + "对应Y坐标分别为:"
                    + boundary_edge.y_max.ToString() + "和" + boundary_edge.y_min.ToString()
                    );
                Console.WriteLine(boundary_edge.x_max.ToString() + "和" + boundary_edge.x_min.ToString());
                return;
            }

            var x_start = boundary_edge.x_min - 1;
            var step = (boundary_edge.y_max - boundary_edge.y_min) / 100;
            var y_start = boundary_edge.y_max - step;
            while (y_start > boundary_edge.y_min)
            {
                /// 首先计算出投影矩形的起始点
                var result = new Point(-1, -1);
                var close_obj_id_list = new List<string>();
                var closest_obj_id = string.Empty;
                var projection_point = project_east(x_start, y_start, boundary_edge, edge_poly_list, distance,
                      close_obj_id_list, ref closest_obj_id);
                if (projection_point.X != result.X && projection_point.Y != result.Y)
                {
                    for (var j = 0; j < test_poly_objs.Count; j++)
                    {
                        if (close_obj_id_list.Contains(test_poly_objs[j].id))
                            if (cross_result.ContainsKey(test_poly_objs[j].id))
                                cross_result[test_poly_objs[j].id] += 1;
                            else
                                cross_result[test_poly_objs[j].id] = 1;

                        if (test_poly_objs[j].id == closest_obj_id)
                            if (closest_result.ContainsKey(test_poly_objs[j].id))
                                closest_result[test_poly_objs[j].id] += 1;
                            else
                                closest_result[test_poly_objs[j].id] = 1;
                    }
                }
                y_start -= step;
            }
        }

        /*'''
        实现界址边的西向投影算法
        :param boundary_edge: 界址边对象
        :param edge_poly_list: 测试多边形的边对象列表
        :param test_poly_objs: 测试多边形的对象列表
        :param distance: 投影线的投影长度，和缓冲距离类似
        :return:
        '''*/
        public void get_west_close_field(BoundaryEdge boundary_edge, List<Edge> edge_poly_list,
           List<BoundaryPoly> test_poly_objs, double distance,
           Dictionary<string, int> cross_result, Dictionary<string, int> closest_result)
        {
            //var cross_result = new Dictionary<string, int>();
            //var closest_result = new Dictionary<string, int>();

            if (boundary_edge.length < 0.0001)
            {
                //重复点排除
                YuLinTu.Library.Log.Log.WriteException(this, "根据四至更新地块", "根据四至更新地块界址信息点位有误,对应X坐标分别为:" + boundary_edge.x_max.ToString() + "和" + boundary_edge.x_min.ToString() + "对应Y坐标分别为:"
                    + boundary_edge.y_max.ToString() + "和" + boundary_edge.y_min.ToString()
                    );
                Console.WriteLine(boundary_edge.x_max.ToString() + "和" + boundary_edge.x_min.ToString());
                return;
            }


            var x_start = boundary_edge.x_max + 1;
            var step = (boundary_edge.y_max - boundary_edge.y_min) / 100;
            var y_start = boundary_edge.y_max - step;
            while (y_start > boundary_edge.y_min)
            {
                /// 首先计算出投影矩形的起始点
                var result = new Point(-1, -1);
                var close_obj_id_list = new List<string>();
                var closest_obj_id = string.Empty;
                var projection_point = project_west(x_start, y_start, boundary_edge, edge_poly_list, distance,
                      close_obj_id_list, ref closest_obj_id);
                if (projection_point.X != result.X && projection_point.Y != result.Y)
                {
                    for (var j = 0; j < test_poly_objs.Count; j++)
                    {
                        if (close_obj_id_list.Contains(test_poly_objs[j].id))
                            if (cross_result.ContainsKey(test_poly_objs[j].id))
                                cross_result[test_poly_objs[j].id] += 1;
                            else
                                cross_result[test_poly_objs[j].id] = 1;

                        if (test_poly_objs[j].id == closest_obj_id)
                            if (closest_result.ContainsKey(test_poly_objs[j].id))
                                closest_result[test_poly_objs[j].id] += 1;
                            else
                                closest_result[test_poly_objs[j].id] = 1;
                    }
                }
                y_start -= step;
            }
        }

        /*'''
        实现界址边的北向投影算法
        :param boundary_edge: 界址边对象
        :param edge_poly_list: 测试多边形的边对象列表
        :param test_poly_objs: 测试多边形的对象列表
        :param distance: 投影线的投影长度，和缓冲距离类似
        :return:
        '''*/
        public void get_north_close_field(BoundaryEdge boundary_edge, List<Edge> edge_poly_list,
           List<BoundaryPoly> test_poly_objs, double distance,
           Dictionary<string, int> cross_result, Dictionary<string, int> closest_result)
        {
            if (boundary_edge.length < 0.0001)
            {
                //重复点排除
                YuLinTu.Library.Log.Log.WriteException(this, "根据四至更新地块", "根据四至更新地块界址信息点位有误,对应X坐标分别为:" + boundary_edge.x_max.ToString() + "和" + boundary_edge.x_min.ToString() + "对应Y坐标分别为:"
                    + boundary_edge.y_max.ToString() + "和" + boundary_edge.y_min.ToString()
                    );
                Console.WriteLine(boundary_edge.x_max.ToString() + "和" + boundary_edge.x_min.ToString());
                return;
            }
            //var cross_result = new Dictionary<string, int>();
            //var closest_result = new Dictionary<string, int>();
            var y_start = boundary_edge.y_min - 1;
            var pvalid = boundary_edge.x_max - boundary_edge.x_min;
            var step = (pvalid) / 100;
            var x_start = boundary_edge.x_max - step;
            while (x_start > boundary_edge.x_min)
            {
                /// 首先计算出投影矩形的起始点
                var result = new Point(-1, -1);
                var close_obj_id_list = new List<string>();
                var closest_obj_id = string.Empty;
                var projection_point = project_north(x_start, y_start, boundary_edge, edge_poly_list, distance,
                      close_obj_id_list, ref closest_obj_id);
                if (projection_point.X != result.X && projection_point.Y != result.Y)
                {
                    for (var j = 0; j < test_poly_objs.Count; j++)
                    {
                        if (close_obj_id_list.Contains(test_poly_objs[j].id))
                            if (cross_result.ContainsKey(test_poly_objs[j].id))
                                cross_result[test_poly_objs[j].id] += 1;
                            else
                                cross_result[test_poly_objs[j].id] = 1;

                        if (test_poly_objs[j].id == closest_obj_id)
                            if (closest_result.ContainsKey(test_poly_objs[j].id))
                                closest_result[test_poly_objs[j].id] += 1;
                            else
                                closest_result[test_poly_objs[j].id] = 1;
                    }
                }
                x_start -= step;
            }
        }

        /*'''
        实现界址边的南向投影算法
        :param boundary_edge: 界址边对象
        :param edge_poly_list: 测试多边形的边对象列表
        :param test_poly_objs: 测试多边形的对象列表
        :param distance: 投影线的投影长度，和缓冲距离类似
        :return:
        '''*/
        public void get_south_close_field(BoundaryEdge boundary_edge, List<Edge> edge_poly_list,
           List<BoundaryPoly> test_poly_objs, double distance,
           Dictionary<string, int> cross_result, Dictionary<string, int> closest_result)
        {
            //var cross_result = new Dictionary<string, int>();
            //var closest_result = new Dictionary<string, int>();

            if (boundary_edge.length < 0.0001)
            {
                //重复点排除
                YuLinTu.Library.Log.Log.WriteException(this, "根据四至更新地块", "根据四至更新地块界址信息点位有误,对应X坐标分别为:" + boundary_edge.x_max.ToString() + "和" + boundary_edge.x_min.ToString() + "对应Y坐标分别为:"
                    + boundary_edge.y_max.ToString() + "和" + boundary_edge.y_min.ToString()
                    );
                Console.WriteLine(boundary_edge.x_max.ToString() + "和" + boundary_edge.x_min.ToString());
                return;
            }

            var y_start = boundary_edge.y_max + 1;
            var step = (boundary_edge.x_max - boundary_edge.x_min) / 100;
            var x_start = boundary_edge.x_max - step;
            while (x_start > boundary_edge.x_min)
            {
                /// 首先计算出投影矩形的起始点
                var result = new Point(-1, -1);
                var close_obj_id_list = new List<string>();
                var closest_obj_id = string.Empty;
                var projection_point = project_south(x_start, y_start, boundary_edge, edge_poly_list, distance,
                      close_obj_id_list, ref closest_obj_id);
                if (projection_point.X != result.X && projection_point.Y != result.Y)
                {
                    for (var j = 0; j < test_poly_objs.Count; j++)
                    {
                        if (close_obj_id_list.Contains(test_poly_objs[j].id))
                            if (cross_result.ContainsKey(test_poly_objs[j].id))
                                cross_result[test_poly_objs[j].id] += 1;
                            else
                                cross_result[test_poly_objs[j].id] = 1;

                        if (test_poly_objs[j].id == closest_obj_id)
                            if (closest_result.ContainsKey(test_poly_objs[j].id))
                                closest_result[test_poly_objs[j].id] += 1;
                            else
                                closest_result[test_poly_objs[j].id] = 1;
                    }
                }
                x_start -= step;
            }
        }


        public Dictionary<string, int> sort_dic(Dictionary<string, int> input_dic)
        {
            /*'''
            将字典按值从大到小进行排序，并改写成列表
            : param input_dic:
            :return:
              '''*/
            if (input_dic != null && input_dic.Count > 0)
            {
                var result = input_dic.Where(t => t.Value > 30).OrderByDescending(t => t.Value).ToDictionary(s => s.Key, s => s.Value);
                return result;
            }
            return new Dictionary<string, int>();
        }

        /*'''
        根据四至的结果，更新界址线毗邻地物权利人，保证
        :param orientation_info_list: 四至的列表
        :param target_poly: 目标地块对象
        :return:
        '''*/
        public string fullfil_blanks(List<string> orientation_info_list, BoundaryPoly target_poly)
        {
            ///# 查找手填的四至的内容
            var missing_field_list = new List<Tuple<string, int>>();
            for (var i = 0; i < orientation_info_list.Count; i++)
            {
                var item = orientation_info_list[i];
                if (item.StartsWith(N))
                    missing_field_list.Add(new Tuple<string, int>(item, i));
            }

            ///# 查找空缺界址线毗邻权利人的界址线
            var missing_boundary_list = new List<BoundaryNumber>();//new Dictionary<BoundaryEdge, int>();
            var boundary_orientation_list = new List<BoundaryNumber>(); //Dictionary<BoundaryEdge, int>(); ;///# 创建一个空的界址线对应的四至列表
            foreach (var boundary in target_poly.boundary_edge_out)
            {
                ///# 如果某一边的相临边界址线长度为0，则
                if (boundary.closest_result.Count == 0)
                    missing_boundary_list.Add(new BoundaryNumber(boundary, check_orientation_for_boundary(boundary)));
                else
                    boundary_orientation_list.Add(new BoundaryNumber(boundary, check_orientation_for_boundary(boundary)));
            }
            ///# print missing_boundary_list
            ///# 以手动添加的四至为基础，优先查找
            foreach (var missing_field in missing_field_list)
            {
                var fill_for_missing_field = 0;  ///# 用以标记是否该地块是否已经填到毗邻地物联系人里

                ///# 如果存在空的界址线，则优先在空的界址线里查找
                if (missing_boundary_list.Count > 0)
                {
                    foreach (var item in missing_boundary_list)
                    {
                        if (missing_field.Item2 == item.Value)  ///# 如果至向相同,则把最毗邻地物联系人填上去
                        {
                            item.Key.closest_first_result = missing_field.Item1;
                            var newclosest_result = new Dictionary<string, int>();
                            newclosest_result.Add(missing_field.Item1, 31);
                            item.Key.closest_result = newclosest_result;///# 注意这里31，是为了匹配closest_result。
                            item.Value = 5; ///# 标记为已经被填过了
                            fill_for_missing_field = 1;
                            break;
                        }
                    }
                }
                ///# 如果没有在空的界址线里查找到，则在一般的界址线里查找对应的四至
                if (fill_for_missing_field == 0)
                {
                    foreach (var item in boundary_orientation_list)
                    {
                        if (missing_field.Item2 == item.Value)  ///# 如果至向相同,则把它加到最毗邻地物联系人列表上填上去
                        {
                            if (!item.Key.closest_result.ContainsKey(missing_field.Item1))
                            {
                                item.Key.closest_result.Add(missing_field.Item1, 31); ///# 注意这里31，是为了匹配closest_result。
                                fill_for_missing_field = 1;
                                break;
                            }
                        }
                    }
                }
                ///# 如果在一般的界址线里也没查找到对应的四至，则填在第一个界址线上
                if (fill_for_missing_field == 0)
                {
                    ///# 注意这里31，是为了匹配closest_result。
                    try
                    {
                        if (boundary_orientation_list[0].Key.closest_result.Count < 0)
                            if (!boundary_orientation_list[0].Key.closest_result.ContainsKey(missing_field.Item1))
                                boundary_orientation_list[0].Key.closest_result.Add(missing_field.Item1, 31);
                    }
                    catch (Exception)
                    {
                        if (!missing_boundary_list[0].Key.closest_result.ContainsKey(missing_field.Item1))
                            missing_boundary_list[0].Key.closest_result.Add(missing_field.Item1, 31);
                    }
                }
            }
            ///# 检查是否还有界址线缺少对应的权利人
            foreach (var missing_boundary in missing_boundary_list)
            {
                var orientation_index = missing_boundary.Value;
                if (orientation_index != 5)
                {///#     missing_boundary[0].closest_first_result = orientation_info_list[orientation_index][0]
                 ///#     ///# 注意这里31，是为了匹配closest_result。
                 ///#     missing_boundary[0].closest_result = [(orientation_info_list[orientation_index][0], 4)]
                    missing_boundary.Key.closest_first_result = "1";
                    var newdic = new Dictionary<string, int>();
                    newdic.Add("1", 31);
                    missing_boundary.Key.closest_result = newdic;
                }
            }
            ///# 构造返回的字符串
            var return_string = "";

            foreach (var boundary_edge in target_poly.boundary_edge_out)
            {
                return_string += string.Format("['{0}','{1}','{2}','{3}','{4}','{5}']%",
                    boundary_edge.name,
                    boundary_edge.boundary_simple_orientation,
                    boundary_edge.length, boundary_edge.position,
                    list_to_string(boundary_edge.closest_result),
                    boundary_edge.closest_first_result);
            }
            ///# print return_string
            return return_string;
        }

        public string list_to_string(Dictionary<string, int> input_list)
        {
            /*'''
            将列表转换成字符串，中间以"|"连接
            :param input_list: 输入的列表,格式为： [('4501051052060101356', 20),('4501051052060101356', 20)]
            :return: return_string: 字符串，格式为4501051052060101356|4501051052060101356
            '''*/
            string return_string = "";
            foreach (var item in input_list)
                return_string = return_string + item.Key + '|';
            return return_string.Trim('|');
        }

        /*'''
           从界址线的信息中判断界址线的方向 :
           param boundary:界址线的对象 :
           return:方向信息
           '''*/
        public int check_orientation_for_boundary(BoundaryEdge boundary)
        {

            ///# 界址线走向为东，则返回北至
            if (boundary.boundary_orientation == A || boundary.boundary_orientation == I || boundary.boundary_orientation == K)
                return 3;
            ///# 界址线走向为西，则返回南至
            if (boundary.boundary_orientation == C || boundary.boundary_orientation == M || boundary.boundary_orientation == O)
                return 1;
            ///# 界址线走向为南，则返回东至
            if (boundary.boundary_orientation == B || boundary.boundary_orientation == J || boundary.boundary_orientation == N)
                return 0;
            ///# 界址线走向为北，则返回西至
            if (boundary.boundary_orientation == D || boundary.boundary_orientation == L || boundary.boundary_orientation == P)
                return 2;
            return 0;
        }


        class BoundaryNumber
        {
            public BoundaryEdge Key { get; set; }
            public int Value { get; set; }

            public BoundaryNumber() { }

            public BoundaryNumber(BoundaryEdge boundaryeEdge, int number)
            {
                Key = boundaryeEdge;
                Value = number;
            }
        }
    }
}
