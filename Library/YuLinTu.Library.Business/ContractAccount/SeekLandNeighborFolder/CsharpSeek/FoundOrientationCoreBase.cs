using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundOrientation
{
    public class FoundOrientationCoreBase
    {
        /*
        从string中载入数据。txt文件的内容为：地块号，坐标x，坐标y...；第二块坐标地块号，坐标x,坐标y...
        */
        public Dictionary<string, double> get_cross_result(List<double> edge_sin_list, List<List<string>> close_ids_lists)
        {
            /*
             计算出某个至向的结果
             {param edge_sin_list{
             {param close_obj_id_list{
             {return{
             */
            var cross_result = new Dictionary<string, double>();
            var get_none_zero_value_index_result = get_none_zero_value_index(edge_sin_list);
            var start_index = get_none_zero_value_index_result[0];
            var end_index = get_none_zero_value_index_result[1];
            var left_bar_index = (int)(3 * (end_index - start_index) / 10);
            var right_bar_index = (int)(7 * (end_index - start_index) / 10);
            for (int i = start_index; i < left_bar_index; i++)
            {
                var weight_value = 0.35 * edge_sin_list[i];
                for (int j = 0; j < close_ids_lists[i].Count; j++)
                {
                    try
                    {
                        if (cross_result.ContainsKey(close_ids_lists[i][j]))
                        {
                            cross_result[close_ids_lists[i][j]] += weight_value;
                        }
                        else
                        {
                            cross_result[close_ids_lists[i][j]] = weight_value;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            for (int i = left_bar_index; i < right_bar_index; i++)
            {
                var weight_value = edge_sin_list[i];
                for (int j = 0; j < close_ids_lists[i].Count; j++)
                {
                    try
                    {
                        if (cross_result.ContainsKey(close_ids_lists[i][j]))
                        {
                            cross_result[close_ids_lists[i][j]] += weight_value;
                        }
                        else
                        {
                            cross_result[close_ids_lists[i][j]] = weight_value;
                        }
                    }
                    catch
                    {
                    }
                }
            }

            for (int i = right_bar_index; i < end_index; i++)
            {
                var weight_value = 0.35 * edge_sin_list[i];
                for (int j = 0; j < close_ids_lists[i].Count; j++)
                {
                    try
                    {
                        if (cross_result.ContainsKey(close_ids_lists[i][j]))
                        {
                            cross_result[close_ids_lists[i][j]] += weight_value;
                        }
                        else
                        {
                            cross_result[close_ids_lists[i][j]] = weight_value;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return cross_result;
        }

        /// <summary>
        ///从一个list中得到第一个非0和最后一个非0元素的index
        ///{param input_list{ 输入的list
        ///{return{ start_index{ 第一个非零元素的index   end_index{ 最后一个非零元素的index
        /// </summary>
        /// <param name="input_list"></param>
        /// <returns></returns>
        public int[] get_none_zero_value_index(List<double> input_list)
        {
            var start_index = 0;
            var end_index = 0;
            for (int i = 0; i < input_list.Count; i++)
            {
                if (input_list[i] > 0)
                {
                    start_index = i;
                    break;
                }
            }
            for (int j = input_list.Count - 1; j >= 0; j--)
            {
                if (input_list[j] > 0)
                {
                    end_index = j;
                    break;
                }
            }
            var array = new int[2] { start_index, end_index };
            return array;
        }

        /*
          读取数据
          {param input_string{ 输入的字符串
          {return{ targetID list：目标地块的列表，元组为目标地块ID
               testID list{ 测试地块的列表，元组为targetID对应元素的测试地块ID列表
               coordinateDic：地块坐标的字典，key为地块ID，value为地块的各顶点坐标
        */
        public Dictionary<string, List<List<Point>>> load_txt_data(string input_string)
        {
            input_string = input_string.Replace("\r\n", "");
            input_string = input_string.Replace(";|", "&");
            var raw_data_list = (input_string.Trim(';')).Split(';');
            var output_Dic = new Dictionary<string, List<List<Point>>>();
            //var raw_data_list = order(raw_data_list_old);
            foreach (var item in raw_data_list)
            {
                var coordinatesList = new List<List<Point>>();
                var id = getCoordiantes(item.Trim(), coordinatesList);
                output_Dic[id] = coordinatesList;
            }
            return output_Dic;
        }

        /*
         从土地信息列表里得到fieldID信息，以及坐标信息
         {param fieldInfoList{
         {return{ id：地块的id信息
                   coordinatesList{ 坐标列表
        */
        public string getCoordiantes(string fieldInfoList, List<List<Point>> return_list)
        {

            //var return_list = new List<List<Point>>();
            var results_ = fieldInfoList.Split(':'); //信息的格式为：ID{x,y,x,y...x,y&x,y,x,y...x,y
            var id = results_[0];
            var item_list = results_[1].Split('&');
            foreach (var coordinates_string in item_list)
            {
                var coordinatesList = new List<Point>();
                var item = coordinates_string.Split(',');
                // print id
                for (int i = 0; i < item.Length; i += 2)
                {
                    var x = item[i].ToDouble();
                    var y = item[i + 1].ToDouble();
                    coordinatesList.Add(new Point(x, y));
                }
                return_list.Add(coordinatesList);
            }
            return id;
        }

        /*
        向东进行相交边投影测试，返回附件地块的id
        {param point{测试点
        {param polyObj{多边形的对象
        {param prior_option{ 优先级。0为默认，1为地块优先，2为距离优先
        {return{cross_point_target：东向射线是否与目标多边形的最东方的交点，如果没有，返回False
                edge_index{ 如果有交点，返回目标多边形东向相交边最右的边的index
                close_obj_id_list{ 附近地块的test_obj的id列表
        */
        public object[] project_east(Point point, Poly target_obj, List<Edge> edge_poly_east_west_list, int prior_option, double projection_length)
        {
            object[] result = new object[3];
            var cross_point_target = new Point(point.X, point.Y);
            var close_obj_id_list = new List<string>();
            var best_id = "None";
            var edge_index = 0;
            ////如果点在多边形的上方或者下方，则肯定在多边形外，且无交点：
            if (point.Y < target_obj.y_min || point.Y > target_obj.y_max)
            {
                result[0] = null;
                result[1] = best_id;
                result[2] = close_obj_id_list;
                return result;// False,None,[]; 
            }
            for (int i = 0; i < target_obj.edgeNum; i++)
            {
                ////如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
                if (point.Y > target_obj.edge[i].y_max || point.Y < target_obj.edge[i].y_min)
                {
                    continue;
                }
                else
                {
                    // //计算线段与水平直线的交点
                    var y0 = point.Y;
                    var x1 = target_obj.edge[i].point1.X;
                    var y1 = target_obj.edge[i].point1.Y;
                    var x2 = target_obj.edge[i].point2.X;
                    var y2 = target_obj.edge[i].point2.Y;
                    var y_difference = y2 - y1;
                    ////如果线段不是水平线
                    if (y_difference != 0)
                    {
                        var x_cross = (y0 * (x2 - x1) - (x2 * y1 - x1 * y2)) / y_difference;
                        ////如果交点在左侧（射线为往右的方向）
                        if (x_cross > cross_point_target.X)
                        {
                            cross_point_target.X = x_cross;
                            edge_index = i;
                        }
                    }
                }
            }
            // print cross_point_target

            if (cross_point_target.X == point.X)
            {
                result[0] = null;
                result[1] = best_id;
                result[2] = close_obj_id_list;
                return result;// False,None,[];
            }
            else
            {
                if (target_obj.edge[edge_index].true_sin > 0.5)
                {
                    projection_length = projection_length / target_obj.edge[edge_index].true_sin;
                }
                double best_x_cross = double.MaxValue;
                for (int i = 0; i < edge_poly_east_west_list.Count; i++)
                {
                    if (cross_point_target.Y > edge_poly_east_west_list[i].y_max ||
                        cross_point_target.Y < edge_poly_east_west_list[i].y_min)
                    {
                        continue;
                    }
                    else
                    {
                        //// 计算线段与水平直线的交点
                        var y0 = point.Y;
                        var x1 = edge_poly_east_west_list[i].point1.X;
                        var y1 = edge_poly_east_west_list[i].point1.Y;
                        var x2 = edge_poly_east_west_list[i].point2.X;
                        var y2 = edge_poly_east_west_list[i].point2.Y;
                        var y_difference = y2 - y1;
                        //// 如果线段不是水平线
                        if (y_difference != 0)
                        {
                            var x_cross = (y0 * (x2 - x1) - (x2 * y1 - x1 * y2)) / y_difference;
                            //// 如果交点在左侧的缓冲距离之内（射线为往右的方向）
                            if (x_cross >= cross_point_target.X - 0.2 &&
                                x_cross < cross_point_target.X + projection_length)
                            {
                                // cross_point_target[0] = x_cross
                                if (prior_option != 2)
                                {
                                    ////如果不是距离优先
                                    close_obj_id_list.Add(edge_poly_east_west_list[i].id);
                                }
                                else
                                {
                                    ////如果是距离优先
                                    if (x_cross < best_x_cross)
                                    {
                                        best_x_cross = x_cross;
                                        best_id = edge_poly_east_west_list[i].id;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            result[0] = cross_point_target;
            result[1] = edge_index;
            if (prior_option != 2)
            {
                result[2] = close_obj_id_list;
                return result;// cross_point_target, edge_index, close_obj_id_list;
            }
            else
            {
                if (best_id != "None")
                {
                    result[2] = new List<string>() { best_id };
                    return result;// cross_point_target, edge_index, [best_id];
                }
                else
                {
                    result[2] = new List<string>() { "emputy" };
                    return result;// cross_point_target, edge_index, ["emputy"]
                }
            }
        }

        /*
         向西进行相交边投影测试，返回附件地块的id
         {param point{测试点
         {param polyObj{多边形的对象
         {param prior_option{ 优先级。0为默认，1为地块优先，2为距离优先
         {return{cross_point_target：东向射线是否与目标多边形的最西方的交点，如果没有，返回False
                 edge_index{ 如果有交点，返回目标多边形西向相交边最右的边的index
                 close_obj_id_list{ 附近地块的test_obj的id列表
         */
        public object[] project_west(Point point, Poly target_obj, List<Edge> edge_poly_east_west_list, int prior_option, double projection_length)
        {

            object[] result = new object[3];
            var cross_point_target = new Point(point.X, point.Y);
            var close_obj_id_list = new List<string>();
            var best_id = "None";
            var edge_index = 0;

            ////如果点在多边形的上方或者下方，则肯定在多边形外，且无交点：
            if (point.Y < target_obj.y_min || point.Y > target_obj.y_max)
            {
                result[0] = null;
                result[1] = best_id;
                result[2] = close_obj_id_list;
                return result;// False,None,[]; 
            }
            for (int i = 0; i < target_obj.edgeNum; i++)
            {
                // //如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
                if (point.Y > target_obj.edge[i].y_max ||
                    point.Y < target_obj.edge[i].y_min)
                {
                    continue;
                }
                else
                {
                    ////计算线段与水平直线的交点
                    var y0 = point.Y;
                    var x1 = target_obj.edge[i].point1.X;
                    var y1 = target_obj.edge[i].point1.Y;
                    var x2 = target_obj.edge[i].point2.X;
                    var y2 = target_obj.edge[i].point2.Y;
                    var y_difference = y2 - y1;
                    ////如果线段不是水平线
                    if (y_difference != 0)
                    {
                        var x_cross = (y0 * (x2 - x1) - (x2 * y1 - x1 * y2)) / y_difference;
                        ////如果交点在左侧（射线为往左的方向）
                        if (x_cross < cross_point_target.X)
                        {
                            cross_point_target.X = x_cross;
                            edge_index = i;
                        }
                    }
                }
            }

            if (cross_point_target.X == point.X)
            {
                result[0] = null;
                result[1] = best_id;
                result[2] = close_obj_id_list;
            }
            else
            {
                if (target_obj.edge[edge_index].true_sin > 0.5)
                {
                    projection_length = projection_length / target_obj.edge[edge_index].true_sin;
                }
                double best_x_cross = 0;
                for (int i = 0; i < edge_poly_east_west_list.Count; i++)
                {
                    if (cross_point_target.Y > edge_poly_east_west_list[i].y_max ||
                        cross_point_target.Y < edge_poly_east_west_list[i].y_min)
                    {
                        continue;
                    }
                    else
                    {
                        // 计算线段与水平直线的交点
                        var y0 = point.Y;
                        var x1 = edge_poly_east_west_list[i].point1.X;
                        var y1 = edge_poly_east_west_list[i].point1.Y;
                        var x2 = edge_poly_east_west_list[i].point2.X;
                        var y2 = edge_poly_east_west_list[i].point2.Y;
                        var y_difference = y2 - y1;
                        // 如果线段不是水平线
                        if (y_difference != 0)
                        {
                            var x_cross = (y0 * (x2 - x1) - (x2 * y1 - x1 * y2)) / y_difference;
                            // 如果交点在左侧的缓冲距离之内（射线为往左的方向）
                            if (x_cross <= cross_point_target.X + 0.2 &&
                                x_cross > cross_point_target.X - projection_length)
                            {
                                if (prior_option != 2)
                                {
                                    //如果不是距离优先
                                    close_obj_id_list.Add(edge_poly_east_west_list[i].id);
                                }
                                else
                                {
                                    // 如果是距离优先
                                    if (x_cross > best_x_cross)
                                    {
                                        best_x_cross = x_cross;
                                        best_id = edge_poly_east_west_list[i].id;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            result[0] = cross_point_target;
            result[1] = edge_index;
            if (prior_option != 2)
            {
                result[2] = close_obj_id_list;
                return result;// cross_point_target, edge_index, close_obj_id_list;
            }
            else
            {
                if (best_id != "None")
                {
                    result[2] = new List<string>() { best_id };
                    return result;// cross_point_target, edge_index, [best_id];
                }
                else
                {
                    result[2] = new List<string>() { "emputy" };
                    return result;// cross_point_target, edge_index, ["emputy"]
                }
            }
        }

        /*
          判断点是否在多边形内，并返回向北水平射线的交点的坐标
          {param point{测试点
          {param polyObj{多边形的对象
          {param prior_option{ 优先级。0为默认，1为地块优先，2为距离优先
          {return{cross_point_target：东向射线是否与目标多边形的最北方的交点，如果没有，返回False
          edge_index{ 如果有交点，返回目标多边形北向相交边最右的边的index
          close_obj_id_list{ 附近地块的test_obj的id列表
         */
        public object[] project_north(Point point, Poly target_obj, List<Edge> edge_poly_north_south_list, int prior_option, double projection_length)
        {
            object[] result = new object[3];
            var cross_point_target = new Point(point.X, point.Y);
            var close_obj_id_list = new List<string>();
            var best_id = "None";
            var edge_index = 0;

            if (point.X < target_obj.x_min || point.X > target_obj.x_max)
            {
                result[0] = null;
                result[1] = best_id;
                result[2] = close_obj_id_list;
                return result;// False,None,[]; 
            }

            for (int i = 0; i < target_obj.edgeNum; i++)
            {
                //如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
                if (point.X < target_obj.edge[i].x_min ||
                    point.X > target_obj.edge[i].x_max)
                {
                    continue;
                }
                else
                {
                    //计算线段与垂直直线的交点
                    var x0 = point.X;
                    var x1 = target_obj.edge[i].point1.X;
                    var y1 = target_obj.edge[i].point1.Y;
                    var x2 = target_obj.edge[i].point2.X;
                    var y2 = target_obj.edge[i].point2.Y;
                    var x_difference = (x2 - x1);
                    //如果线段不是水平线
                    if (x_difference != 0)
                    {
                        var y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference;
                        //如果交点在上方（射线为往北的方向）
                        if (y_cross > cross_point_target.Y)
                        {
                            cross_point_target.Y = y_cross;
                            edge_index = i;
                        }
                    }
                }
            }

            if (cross_point_target.Y == point.Y)
            {
                result[0] = null;
                result[1] = best_id;
                result[2] = close_obj_id_list;
                return result;// False,None,[]; 
            }
            else
            {
                if (target_obj.edge[edge_index].true_cos > 0.5)
                {
                    projection_length = projection_length / target_obj.edge[edge_index].true_cos;
                }
                double best_y_cross = double.MaxValue;
                for (int i = 0; i < edge_poly_north_south_list.Count; i++)
                {
                    if (cross_point_target.X > edge_poly_north_south_list[i].x_max ||
                        cross_point_target.X < edge_poly_north_south_list[i].x_min)
                    {
                        continue;
                    }
                    else
                    {
                        // 计算线段与垂直直线的交点
                        var x0 = point.X;
                        var x1 = edge_poly_north_south_list[i].point1.X;
                        var y1 = edge_poly_north_south_list[i].point1.Y;
                        var x2 = edge_poly_north_south_list[i].point2.X;
                        var y2 = edge_poly_north_south_list[i].point2.Y;
                        var x_difference = (x2 - x1);
                        // 如果线段不是水平线
                        if (x_difference != 0)
                        {
                            var y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference;

                            // 如果交点在上方的缓冲距离之内（射线为往北的方向）
                            if (y_cross >= cross_point_target.Y - 0.2 &&
                                y_cross < cross_point_target.Y + projection_length)
                            {
                                if (prior_option != 2)
                                {
                                    //如果不是距离优先
                                    close_obj_id_list.Add(edge_poly_north_south_list[i].id);
                                }
                                else
                                {
                                    // 如果是距离优先
                                    if (y_cross < best_y_cross)
                                    {
                                        best_y_cross = y_cross;
                                        best_id = edge_poly_north_south_list[i].id;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            result[0] = cross_point_target;
            result[1] = edge_index;
            if (prior_option != 2)
            {
                result[2] = close_obj_id_list;
                return result;// cross_point_target, edge_index, close_obj_id_list;
            }
            else
            {
                if (best_id != "None")
                {
                    result[2] = new List<string>() { best_id };
                    return result;// cross_point_target, edge_index, [best_id];
                }
                else
                {
                    result[2] = new List<string>() { "emputy" };
                    return result;// cross_point_target, edge_index, ["emputy"]
                }
            }
        }

        /*
         判断点是否在多边形内，并返回向南水平射线的交点的坐标
         {param point{测试点
         {param polyObj{多边形的对象
         {param prior_option{ 优先级。0为默认，1为地块优先，2为距离优先
         {return{cross_point_target：东向射线是否与目标多边形的最北方的交点，如果没有，返回False
                 edge_index{ 如果有交点，返回目标多边形北向相交边最右的边的index
                 close_obj_id_list{ 附近地块的test_obj的id列表
        */
        public object[] project_south(Point point, Poly target_obj, List<Edge> edge_poly_north_south_list, int prior_option, double projection_length)
        {

            object[] result = new object[3];
            var cross_point_target = new Point(point.X, point.Y);
            var close_obj_id_list = new List<string>();
            var best_id = "None";
            var edge_index = 0;

            if (point.X < target_obj.x_min ||
                point.X > target_obj.x_max)
            {
                result[0] = null;
                result[1] = best_id;
                result[2] = close_obj_id_list;
                return result;// False,None,[]; 
            }

            for (int i = 0; i < target_obj.edgeNum; i++)
            {
                //如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
                if (point.X < target_obj.edge[i].x_min ||
                    point.X > target_obj.edge[i].x_max)
                {
                    continue;
                }
                else
                {
                    //计算线段与垂直直线的交点
                    var x0 = point.X;
                    var x1 = target_obj.edge[i].point1.X;
                    var y1 = target_obj.edge[i].point1.Y;
                    var x2 = target_obj.edge[i].point2.X;
                    var y2 = target_obj.edge[i].point2.Y;
                    var x_difference = (x2 - x1);
                    //如果线段不是水平线
                    if (x_difference != 0)
                    {
                        var y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference;

                        //如果交点在下方（射线为往南的方向）
                        if (y_cross < cross_point_target.Y)
                        {
                            cross_point_target.Y = y_cross;
                            edge_index = i;
                        }
                    }
                }
            }

            if (cross_point_target.Y == point.Y)
            {
                result[0] = null;
                result[1] = best_id;
                result[2] = close_obj_id_list;
                return result;// False,None,[]; 
            }
            else
            {
                if (target_obj.edge[edge_index].true_cos > 0.5)
                {
                    projection_length = projection_length / target_obj.edge[edge_index].true_cos;
                }
                double best_y_cross = 0;
                for (int i = 0; i < edge_poly_north_south_list.Count; i++)
                {
                    if (cross_point_target.X > edge_poly_north_south_list[i].x_max ||
                        cross_point_target.X < edge_poly_north_south_list[i].x_min)
                    {
                        continue;
                    }
                    else
                    {
                        // 计算线段与垂直直线的交点
                        var x0 = point.X;
                        var x1 = edge_poly_north_south_list[i].point1.X;
                        var y1 = edge_poly_north_south_list[i].point1.Y;
                        var x2 = edge_poly_north_south_list[i].point2.X;
                        var y2 = edge_poly_north_south_list[i].point2.Y;
                        var x_difference = (x2 - x1);
                        // 如果线段不是水平线
                        if (x_difference != 0)
                        {
                            var y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference;
                            // 如果交点在下方的缓冲距离之内（射线为往南的方向）
                            if (y_cross <= cross_point_target.Y + 0.2 &&
                                y_cross > cross_point_target.Y - projection_length)
                            {
                                // cross_point_target[1] = y_cross
                                if (prior_option != 2)
                                {
                                    //如果不是距离优先
                                    close_obj_id_list.Add(edge_poly_north_south_list[i].id);
                                }
                                else
                                {
                                    // 如果是距离优先
                                    if (y_cross > best_y_cross)
                                    {
                                        best_y_cross = y_cross;
                                        best_id = edge_poly_north_south_list[i].id;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            result[0] = cross_point_target;
            result[1] = edge_index;
            if (prior_option != 2)
            {
                result[2] = close_obj_id_list;
                return result;
            }
            else
            {
                if (best_id != "None")
                {
                    result[2] = new List<string>() { best_id };
                    return result;
                }
                else
                {
                    result[2] = new List<string>() { "emputy" };
                    return result;
                }
            }
        }

        /*
         从字典中计算出各地块物体的外接矩形顶点坐标字典。
         key为地块id，value为[[x_min, y_min],[x_max, y_max]]
         {param coordinate_dic{
         {return{
         */
        public Dictionary<string, List<Point>> get_outer_rectangle_dic(Dictionary<string, List<List<Point>>> coordinate_dic)
        {
            var output_dic = new Dictionary<string, List<Point>>();
            foreach (var kv in coordinate_dic)
            {
                double x_min = double.MaxValue;
                double y_min = double.MaxValue;
                double x_max = 0;
                double y_max = 0;
                foreach (var coordinate_dic_list in kv.Value)
                {
                    foreach (var coordinate_pair in coordinate_dic_list)
                    {
                        if (coordinate_pair.X < x_min)
                        {
                            x_min = coordinate_pair.X;
                        }
                        if (coordinate_pair.X > x_max)
                        {
                            x_max = coordinate_pair.X;
                        }
                        if (coordinate_pair.Y < y_min)
                        {
                            y_min = coordinate_pair.Y;
                        }
                        if (coordinate_pair.Y > y_max)
                        {
                            y_max = coordinate_pair.Y;
                        }
                    }
                    output_dic[kv.Key] = new List<Point>() { new Point(x_min, y_min), new Point(x_max, y_max) };
                }
            }
            return output_dic;
        }

        /*
         快速而粗略的判断相邻的地块编码，判断原理如下：
         如果测试地块的x/y_max<目标地块的x/y_min-distance{ 在外面
         如果测试地块的x/y_min> 目标地块的x/y_max+distance{ 在外面
          以上两个(四个）条件均为否，则在里面
          {param inputID{
         {param rectangle_dic{
         {param distance{
         {return{ 相邻地块编码列表
         */
        public List<string> get_close_field(string inputID, Dictionary<string, List<Point>> rectangle_dic, double distance)
        {
            var x_min_check = rectangle_dic[inputID][0].X - distance;
            var y_min_check = rectangle_dic[inputID][0].Y - distance;

            var x_max_check = rectangle_dic[inputID][1].X + distance;
            var y_max_check = rectangle_dic[inputID][1].Y + distance;

            var return_list = new List<string>();
            foreach (var kv in rectangle_dic)
            {
                var x_min = kv.Value[0].X;
                var y_min = kv.Value[0].Y;
                var x_max = kv.Value[1].X;
                var y_max = kv.Value[1].Y;
                if (x_min > x_max_check || y_min > y_max_check ||
                    x_max < x_min_check || y_max < y_min_check)
                {
                    continue;
                }
                else
                {
                    return_list.Add(kv.Key);
                }
            }
            return_list.Remove(inputID);
            return return_list;
        }

        public void get_formated_data(string field_info_string, double distance)
        {
            var coordinateDic = load_txt_data(field_info_string);
            var rectangle_dic = get_outer_rectangle_dic(coordinateDic);
            var targetID_list = new List<string>();
            var testID_list = new List<List<string>>();
            foreach (var item in rectangle_dic.Keys)
            {
                if (item[0] == 'X' || item[0] == 'M')
                {
                    continue;
                }
                else
                {
                    targetID_list.Add(item);
                    testID_list.Add(get_close_field(item, rectangle_dic, distance));
                }
            }
            //targetID_list, testID_list = get_id_list(rectangle_dic, distance);
            return;// targetID_list, testID_list, coordinateDic;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        public List<string> get_result_list_all(Dictionary<string, double> result_dic, double threshold)
        {
            var tupleList = new List<Tuple<string, double>>();
            foreach (var item in result_dic)
            {
                tupleList.Add(new Tuple<string, double>(item.Key, item.Value));
            }
            var sorted_list = tupleList.OrderByDescending(t => t.Item2).ToList();// (dic_list, key = lambda x{
                                                                                 // x[1], reverse = True)
            try
            {
                threshold = sorted_list.Count == 0 ? 0 : sorted_list[0].Item2 * threshold;
            }
            catch
            {
                threshold = 0;
            }

            var first_result = new List<string>();
            foreach (var x in sorted_list)
            {
                if (x.Item2 > threshold)
                    first_result.Add(x.Item1);
            }
            return first_result;
        }

        public List<List<string>> get_result_list(Dictionary<string, double> result_dic, double threshold)
        {
            var tupleList = new List<Tuple<string, double>>();
            foreach (var item in result_dic)
            {
                tupleList.Add(new Tuple<string, double>(item.Key, item.Value));
            }
            var sorted_list = tupleList.OrderByDescending(t => t.Item2).ToList();
            try
            {
                threshold = sorted_list.Count == 0 ? 0 : sorted_list[0].Item2 * threshold;
            }
            catch
            {
                threshold = 0;
            }
            var field_result = new List<string>();
            var M_result = new List<string>();
            foreach (var x in sorted_list)
            {
                if (!x.Item1.Contains("M") && !x.Item1.Contains("X")
                    && x.Item1 != "emputy" && x.Item2 >= threshold)
                {
                    field_result.Add(x.Item1);
                }
            }

            foreach (var x in sorted_list)
            {
                if ((x.Item1.Contains("M") || x.Item1.Contains("X"))
                    && x.Item1 != "emputy" && x.Item2 >= threshold)
                {
                    M_result.Add(x.Item1);
                }
            }
            return new List<List<string>>() { field_result, M_result };
        }

        public string str(List<string> list, int index = -1)
        {
            if (list.Count == 0)
                return "[]";
            var s = "[";
            if (index > list.Count || index == -1)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var v = list[i];
                    if (v != "")
                        v = "'" + v + "'";
                    s += v + ",";
                }
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    var v = list[i];
                    if (v != "")
                        v = "'" + v + "'";
                    s += v + ",";
                }
            }
            s = s.TrimEnd(',') + "]";
            return s;
        }

        public List<string> Part(List<string> list, int index, bool pro = false)
        {
            if (pro && list.Count == 2 && index == 1 && list.Contains("emputy"))
            {
                list.Remove("emputy");
                return list;
            }
            var sList = new List<string>();
            if (index > list.Count)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    sList.Add(list[i]);
                }
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    sList.Add(list[i]);
                }
            }
            return sList;
        }

        public List<Edge> Copy(List<Edge> list)
        {
            var newList = new List<Edge>();
            foreach (var item in list)
            {
                newList.Add(item.Clone() as Edge);
            }
            return newList;
        }
    }

    static public class OrientationHelper
    {
        static public double ToDouble(this string v1)
        {
            double v2 = 0;
            double.TryParse(v1, out v2);
            return v2;
        }

        /// <summary>
        /// 取大值
        /// </summary>
        static public double Max(double v1, double v2)
        {
            return v1 > v2 ? v1 : v2;
        }

        /// <summary>
        /// 取小值
        /// </summary>
        static public double Min(double v1, double v2)
        {
            return v1 > v2 ? v2 : v1;
        }

        ///*
        // 返回相对水平线夹角的cos值
        // {param point1{
        // {param point2{
        // {return{水平线夹角的cos值
        // */
        //static public double get_cos(Point point1, Point point2)
        //{

        //    var x_length = Max((point1.X - point2.X), (point2.X - point1.X));
        //    var y_length = Max((point1.Y - point2.Y), (point2.Y - point1.Y));
        //    var cos = x_length / Math.Sqrt(Math.Pow(x_length, 2) + Math.Pow(y_length, 2));
        //    return cos;
        //}

        ///*
        // 返回相对水平线夹角的sin值
        // {param point1{
        // {param point2{
        // {return{水平线夹角的sin值
        // */
        //static public double get_sin(Point point1, Point point2)
        //{

        //    var x_length = Max((point1.X - point2.X), (point2.X - point1.X));
        //    var y_length = Max((point1.Y - point2.Y), (point2.Y - point1.Y));
        //    var sin = y_length / Math.Sqrt(Math.Pow(x_length, 2) + Math.Pow(y_length, 2));
        //    return sin;
        //}
    }
}
