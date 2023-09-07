using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundOrientation
{
    /// <summary>
    /// 查找四至
    /// </summary>
    public class FindLandBoundary : EdgeBase
    {
        public FindLandBoundary()
        {
        }

        /// <summary>
        /// 获取界址线信息
        /// </summary>
        public string Get_boundary_for_fields(string field_orientation_string, string field_info_string, double distance, int priori = 1)
        {
            var return_string = string.Empty;/// 存储最终结果的字符串

            /// 获取地块四至信息
            var orientation_dic = Orientation_data_to_dic(field_orientation_string);

            #region newMethod
            /*'''
            从字符串和缓冲距离中，查找出可能的毗邻地块或面线状物 
            : param field_info_string: 输入的字符串
            : param distance: 缓冲距离
            : return: targetID_list: 需要测试的目标地块列表
            testID_list: 对应目标地块的测试地块列表
            coordinates_dic：坐标字典
            boundary_dic：界址边字典
            '''*/

            /*'''
            读取数据
            :param input_string: 输入的字符串
            :return: coordinate_dic：地块坐标的字典，key为地块ID，value为地块的各顶点坐标
                     boundary_dic: 界址边坐标的字典，key为地块ID，value为界址边的信息。value格式为：
                     [[['J16,J18', (509011.447327, 2519227.910095),...],['J18,J19',(x, y),(x, y)],[...]],[第二圈的信息],[...]]
            '''*/
            var coordinate_dic = new Dictionary<string, List<List<Point>>>();
            var boundary_dic = new Dictionary<string, List<List<BoundaryEdge>>>();
            field_info_string = field_info_string.Replace(";|", "&");
            var raw_data_list = (field_info_string.Trim(';')).Split(';');
            foreach (var item in raw_data_list)
            {
                var coordinates_list = new List<List<Point>>();
                var boundary_list = new List<List<BoundaryEdge>>();
                var id = getCoordiantes(item, coordinates_list, boundary_list);
                coordinate_dic[id] = coordinates_list;
                boundary_dic[id] = boundary_list;
            }
            var rectangle_dic = Get_outer_rectangle_dic(coordinate_dic);

            var testID_list = new List<List<string>>();
            var targetID_list = Get_id_list(rectangle_dic, distance, testID_list);

            #endregion

            /// 初始化对象
            //targetID_list, testID_list, coordinates_dic, boundary_dic = get_formated_data(field_info_string, distance)
            var poly_dic = new Dictionary<string, BoundaryPoly>();
            foreach (var kv in coordinate_dic)
            {
                var k = kv.Key;
                var v = kv.Value;
                poly_dic[k] = new BoundaryPoly(k, v, boundary_dic[k]);
            }
            
            for (int j = 0; j < targetID_list.Count; j++)
            {
                var target_poly = poly_dic[targetID_list[j]];
                var target_poly_x_max_with_distance = target_poly.x_max + distance;
                var target_poly_x_min_with_distance = target_poly.x_min - distance;
                var target_poly_y_max_with_distance = target_poly.y_max + distance;
                var target_poly_y_min_with_distance = target_poly.y_min - distance;
                var test_poly_objs = new List<BoundaryPoly>();
                var edge_poly_list = new List<Edge>();
                foreach (var item in testID_list[j].Reverse<string>())
                {
                    test_poly_objs.Add(poly_dic[item]);
                    foreach (var test_poly in test_poly_objs)
                    {
                        for (int i = 0; i < test_poly.edgeNum; i++)
                        {
                            var c_edge = test_poly.edge[i];
                            if (target_poly_y_min_with_distance > c_edge.y_max ||
                                target_poly_y_max_with_distance < c_edge.y_min ||
                                target_poly_x_min_with_distance > c_edge.x_max ||
                                target_poly_x_max_with_distance < c_edge.x_min)
                                continue;
                            else
                                edge_poly_list.Add(test_poly.edge[i]);
                        }
                    }
                }
                foreach (var boundary_edge in target_poly.boundary_edge_out)
                {
                    Dictionary<string, int> close_field_dic = new Dictionary<string, int>();
                    Dictionary<string, int> closest_dic = new Dictionary<string, int>();
                    get_boundary_close_fields(boundary_edge, edge_poly_list, test_poly_objs, distance, "out", close_field_dic, closest_dic);
                    /// close_field_list=sort_dic(close_field_dic) ///计算所有的毗邻地物
                    boundary_edge.closest_result = sort_dic(closest_dic); /// 计算最近的毗邻地物（距离优先模式）

                    if (boundary_edge.closest_result.Count > 0)
                    {
                        boundary_edge.closest_first_result = boundary_edge.closest_result.First().Key;

                        /// 如果最毗邻地物不是线，面状物，则位置为中，否则为外
                        if (!boundary_edge.closest_result.First().Key.StartsWith("X") &&
                            !boundary_edge.closest_result.First().Key.StartsWith("M"))
                            boundary_edge.position = M;
                        else
                            boundary_edge.position = I;
                    }
                    else
                    {
                        /// 如果没找到毗邻地物，也为外
                        boundary_edge.position = O;
                    }
                }
                var boundary_result_string = fullfil_blanks(orientation_dic[targetID_list[j]], target_poly);
                var field_result_string = "'" + targetID_list[j] + "'" + ":" + boundary_result_string.Trim('%') + ';';
                return_string += field_result_string;
            }
            return return_string;
        }


        /*'''
        实现紧邻投影法判断方位
        地块落在紧邻投影范围内的算法由扫线法实现
        投影起始点为地块的各个边
        '''*/
        public void get_boundary_close_fields(BoundaryEdge boundary_edge, List<Edge> edge_poly_list,
           List<BoundaryPoly> test_poly_objs, double distance, string in_or_out,
           Dictionary<string, int> cross_result, Dictionary<string, int> closest_result)
        {
            /*  '''
            查询界址线的最近毗邻地块
            :param boundary_edge:
            :param edge_poly_list:
            :param test_poly_objs:
            :param distance:
            :param in_or_out:
            :param priori:
            :return:
            '''*/
            string outString = "out";
            string inString = "in";
            var boundary_edge_orientation = boundary_edge.boundary_orientation;
            // var cross_result = new Dictionary<string, int>();
            // var closest_result = new Dictionary<string, int>();
            if (boundary_edge_orientation == B)
            {
                if (in_or_out == outString)
                {
                    ///圈的向南的界址边，需要向东发出射线
                    // close_obj_result,closest_result = 
                    get_east_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
                if (in_or_out == inString)
                {
                    ///内圈的向南的界址边，需要向西发出射线
                    //close_obj_result,closest_result = 
                    get_west_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
            }
            else if (boundary_edge_orientation == D)
            {
                if (in_or_out == outString)
                {
                    ///外圈的向北的界址边，需要向西发出射线
                    get_west_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
                if (in_or_out == inString)
                {
                    ///外圈的向北的界址边，需要向东发出射线
                    get_east_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
            }
            else if (boundary_edge_orientation == A)
            {
                if (in_or_out == outString)
                {
                    ///外圈的向东的界址边，需要向北发出射线
                    get_north_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
                if (in_or_out == inString)
                {
                    ///外圈的向东的界址边，需要向南发出射线
                    get_south_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
            }
            else if (boundary_edge_orientation == C)
            {
                if (in_or_out == outString)
                {
                    ///外圈的向西的界址边，需要向南发出射线
                    get_south_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
                if (in_or_out == inString)
                {
                    get_north_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
            }
            else if (boundary_edge_orientation == J || boundary_edge_orientation == N)
            {
                if (in_or_out == outString)
                {
                    ///外圈的东南偏南，或西南偏南方向的界址边，需要向东发出射线
                    get_east_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
                if (in_or_out == inString)
                {
                    ///内圈的东南偏南，或西南偏南方向的界址边，需要向西发出射线
                    get_east_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
            }
            else if (boundary_edge_orientation == L || boundary_edge_orientation == P)
            {
                if (in_or_out == outString)
                {
                    ///外圈的东北偏北，或西北偏北的界址边，需要向西发出射线
                    get_west_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
                if (in_or_out == inString)
                {
                    ///内圈的东北偏北，或西北偏北的界址边，需要向东发出射线
                    get_east_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
            }
            else if (boundary_edge_orientation == I || boundary_edge_orientation == K)
            {
                if (in_or_out == outString)
                {
                    ///外圈的东南偏东，或东北偏东方向的界址边，需要向北发出射线
                    get_north_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
                if (in_or_out == inString)
                {
                    ///内圈的东南偏东，或东北偏东方向的界址边，需要向南发出射线
                    get_south_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
            }
            else if (boundary_edge_orientation == M || boundary_edge_orientation == O)
            {
                if (in_or_out == outString)
                {
                    ///外圈的西南偏西，或西北偏西方向的界址边，需要向南发出射线
                    get_south_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
                if (in_or_out == inString)
                {
                    ///内圈的西南偏西，或西北偏西方向的界址边，需要向北发出射线
                    get_north_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance, cross_result, closest_result);
                }
            }
        }

        /// <summary>
        /// 构建四至信息字典
        /// </summary>
        public Dictionary<string, List<string>> Orientation_data_to_dic(string orientation_string)
        {
            /*'''
            从地块四至字符串中获取地块四至的dic
            :param orientation_string:
                        地块四至字符串，格式为：
                        地块1编码:东至地块编码,西至地块编码,南至地块编码,北至地块编码;地块2编码:...
                        某个至向如果有多个地块，各地块编码之间用"|"连接。
            :return:
            '''
            */

            var return_dic = new Dictionary<string, List<string>>();
            var field_list = orientation_string.Split(';');
            foreach (var field_info in field_list)
            {
                var field_info_list = field_info.Split(':');
                if (field_info_list.Length != 2)
                {
                    continue;
                }
                var field_id = field_info_list[0];//'483c654f-0469-4c23-828c-469c808b55a5'
                var orientation_info = field_info_list[1];//'N赵占军,N沟渠,N刘虎,N道路'
                var orientation_list = orientation_info.Split(',');
                var formated_orientation_list = new List<string>();
                if (orientation_list.Length == 4)
                {
                    for (var i = 0; i < orientation_list.Length; i++)
                    {
                        var one_orientation_list = SplitLikePython(orientation_list[i], '|');
                        formated_orientation_list.Add(one_orientation_list);
                    }
                }
                return_dic[field_id] = formated_orientation_list;
            }
            return return_dic;
        }

        public string SplitLikePython(string value, char flag)
        {
            if (!value.Contains(flag))
            {
                return value;
            }
            var array = value.Split(flag);
            var returnString = string.Empty;
            for (int i = 0; i < array.Length; i++)
            {
                returnString += "'" + array[i] + "',";
            }
            returnString = /*"[" +*/ returnString.TrimEnd(',')/* + "]"*/;
            return returnString;
        }
        /*
        public void get_formated_data(string field_info_string, int distance)
        {
            /*'''
            从字符串和缓冲距离中，查找出可能的毗邻地块或面线状物 
            : param field_info_string: 输入的字符串
            : param distance: 缓冲距离
            : return: targetID_list: 需要测试的目标地块列表
            testID_list: 对应目标地块的测试地块列表
            coordinates_dic：坐标字典
            boundary_dic：界址边字典
            '''*
            var coordinate_dic = new Dictionary<string, List<Point>>();
            var boundary_dic = new Dictionary<string, List<BoundaryEdge>>();

            Data_to_dic(field_info_string, coordinate_dic, boundary_dic);
            var rectangle_dic = Get_outer_rectangle_dic(coordinate_dic);


            var testID_list = new List<string>();
            var targetID_list = Get_id_list(rectangle_dic, distance, testID_list);

            return targetID_list, testID_list, coordinates_dic,boundary_dic;
        }
        */

        /// <summary>
        /// 获取ID列表
        /// </summary>
        public List<string> Get_id_list(Dictionary<string, List<Point>> rectangle_dic,
            double distance, List<List<string>> testColseIDList)
        {
            /*'''
            迭代统计所有的
            :param rectangle_dic:
            :param distance:
            :return:
            '''*/
            var targetID_list = new List<string>();
            foreach (var item in rectangle_dic.Keys)
            {
                if (item[0] == 'X' || item[0] == 'M')
                    continue;
                targetID_list.Add(item);
                var closeList = get_close_field(item, rectangle_dic, distance);
                testColseIDList.Add(closeList);
            }
            return targetID_list;
        }

        /// <summary>
        /// 获取相近地块ID
        /// </summary>
        /// <param name="inputID"></param>
        /// <param name="rectangle_dic"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public List<string> get_close_field(string inputID, Dictionary<string, List<Point>> rectangle_dic, double distance)
        {
            /*'''
            快速而粗略的判断相邻的地块编码，判断原理如下：
            如果测试地块的x/y_max<目标地块的x/y_min-distance: 在外面
            如果测试地块的x/y_min> 目标地块的x/y_max+distance: 在外面
             以上两个(四个）条件均为否，则在里面
             :param inputID: 目标地块的ID
             :param rectangle_dic: 外接矩形字典
             :param distance: 缓冲距离
             :return: return_list 相邻地块编码列表
            '''
            */
            var x_min_check = rectangle_dic[inputID][0].X - distance;
            var y_min_check = rectangle_dic[inputID][0].Y - distance;
            var x_max_check = rectangle_dic[inputID][1].X + distance;
            var y_max_check = rectangle_dic[inputID][1].Y + distance;
            var return_list = new List<string>();
            foreach (var item in rectangle_dic)
            {
                var v = item.Value;
                var x_min = v[0].X;
                var y_min = v[0].Y;
                var x_max = v[1].X;
                var y_max = v[1].Y;

                if (x_min > x_max_check || y_min > y_max_check ||
                    x_max < x_min_check || y_max < y_min_check)
                {
                    continue;
                }
                else
                {
                    return_list.Add(item.Key);
                }
            }
            return_list.Remove(inputID);
            return return_list;
        }

        /// <summary>
        /// 获取外矩形框字典
        /// </summary>
        /// <param name="coordinate_dic"></param>
        public Dictionary<string, List<Point>> Get_outer_rectangle_dic(Dictionary<string, List<List<Point>>> coordinate_dic)
        {
            /*'''
           从坐标字典中计算出各地块物体的外接矩形顶点坐标字典。
           key为地块id，value为[[x_min, y_min],[x_max, y_max]]
           :param coordinate_dic:坐标字典
           :return:rectangle_dic：外接矩形字典
           '''*/
            var rectangle_dic = new Dictionary<string, List<Point>>();
            foreach (var cd in coordinate_dic)
            {
                var max_x = 0.0;
                var max_y = 0.0;
                var min_x = double.MaxValue;
                var min_y = double.MaxValue;

                foreach (var item in cd.Value)
                {
                    var pointlist = item;
                    foreach (var p in pointlist)
                    {
                        if (p.X < min_x)
                            min_x = p.X;
                        if (p.X > max_x)
                            max_x = p.X;

                        if (p.Y < min_y)
                            min_y = p.Y;
                        if (p.Y > max_y)
                            max_y = p.Y;
                    }
                }
                rectangle_dic.Add(cd.Key, new List<Point>()
                {
                    new Point() { X=min_x,Y=min_y},
                    new Point() { X=max_x,Y=max_y}
                });
            }
            return rectangle_dic;
        }

        /// <summary>
        /// 构造点 线 字典
        /// </summary>
        /// <param name="input_string"></param>
        public void Data_to_dic(string input_string, Dictionary<string, List<List<Point>>> coordinate_dic,
        Dictionary<string, List<List<BoundaryEdge>>> boundary_dic)
        {
            /*'''
            读取数据
            :param input_string: 输入的字符串
            :return: coordinate_dic：地块坐标的字典，key为地块ID，value为地块的各顶点坐标
                     boundary_dic: 界址边坐标的字典，key为地块ID，value为界址边的信息。value格式为：
                     [[['J16,J18', (509011.447327, 2519227.910095),...],['J18,J19',(x, y),(x, y)],[...]],[第二圈的信息],[...]]
            '''*/
            input_string = input_string.Replace(";|", "&");
            var raw_data_list = (input_string.Trim(';')).Split(';');
            //var coordinate_dic = new Dictionary<Guid, List<Point>>();
            //var boundary_dic = new Dictionary<Guid, List<BoundaryEdge>>();
            foreach (var item in raw_data_list)
            {
                var coordinates_list = new List<List<Point>>();
                var boundary_list = new List<List<BoundaryEdge>>();
                var id = getCoordiantes(item, coordinates_list, boundary_list);
                coordinate_dic[id] = coordinates_list;
                boundary_dic[id] = boundary_list;
            }
        }

        /// <summary>
        /// 获取坐标
        /// </summary>
        public string getCoordiantes(string fieldInfoList, List<List<Point>> coordinates_list,
            List<List<BoundaryEdge>> boundary_list)
        {
            /*'''
            从土地信息列表里得到fieldID信息，以及坐标信息
            :param fieldInfoList:
            :return: id：地块的id信息
                      coordinates_list: 坐标列表
            '''*/
            //coordinates_list = new List<Point>();
            // boundary_list = new List<BoundaryEdge>();
            var results_ = fieldInfoList.Split(':');    ///信息的格式为：ID:x,y,x,y...x,y&x,y,x,y...x,y
            var id = results_[0];
            var item_list = results_[1].Split('&');

            if (id[0] != 'X' && id[0] != 'M')
            {
                foreach (var coordinates_string in item_list)
                {
                    var pointList = new List<Point>();
                    var edge_list = Split_coordinates(coordinates_string, pointList, id);
                    boundary_list.Add(edge_list);
                    coordinates_list.Add(pointList);
                }
            }
            else
            {
                var pointlist = new List<Point>();
                foreach (var coordinates_string in item_list)
                {
                    var item = coordinates_string.Split(',');
                    for (var i = 0; i < item.Length; i += 2)
                    {
                        var x = item[i];
                        var y = item[i + 1];
                        pointlist.Add(new Point(x, y));
                    }
                }
                coordinates_list.Add(pointlist);
                boundary_list = new List<List<BoundaryEdge>>();// new List<BoundaryEdge>();
            }
            return id;
        }

        /// <summary>
        /// 分割坐标点
        /// </summary>
        public List<BoundaryEdge> Split_coordinates(string point_string, List<Point> allPointList, string guid)
        {
            /*'''
            point_string: 'J3#463619.029474,J3#4540878.590711,J4#463594.811715,J4#4540876.777178,J1#463585.874853,J1#4540948.71927,J2#463610.032521,J2#4540950.751931'
            分割界址点，并组合成界址线列表
            : param point_list: 输入的点坐标
            : return:boundary_list: 界址线段的坐标。格式为：{ 'J1,J2':[(x1, y1),(x2, y2)],'J2,J3':[(x1, y1),(x2, y2)],...}
            all_point_list: 点坐标。格式为[(x1, y1), (x2, y2)...]
            '''*/
            var point_list = point_string.Split(',');//['J3#463619.029474', 'J3#4540878.590711', 'J4#463594.811715', 'J4#4540876.777178', 'J1#463585.874853', 'J1#4540948.71927', 'J2#463610.032521', 'J2#4540950.751931']
            var boundary_point_index_list = new List<int>();    ///指示界址点在all_point_list中的哪些index上
            var all_point_list = new List<Point>();   ///所有点的坐标
            var boundary_point_name_list = new List<string>();   ///指示界址点的名称
            var boundary_list = new List<BoundaryEdge>();        ///界址线段的坐标

            for (int i = 0; i < point_list.Length; i += 2)
            {
                if (point_list[i].Contains('#'))
                {
                    boundary_point_index_list.Add(i / 2);
                    var x_list = point_list[i].Split('#');///['J3', '463619.029474']
                    var y_list = point_list[i + 1].Split('#');///['J3', '463619.029474']
                    all_point_list.Add(new Point(x_list[1], y_list[1]));
                    boundary_point_name_list.Add(x_list[0]);
                }
                else
                {
                    all_point_list.Add(new Point(point_list[i], point_list[i + 1]));
                }
            }
            var id = guid;
            for (var i = 0; i < boundary_point_index_list.Count - 1; i++)
            {
                var edge_point_list = new List<Point>();
                var j = boundary_point_index_list[i];
                var j1 = boundary_point_index_list[i + 1];
                var name = boundary_point_name_list[i] + '+' + boundary_point_name_list[i + 1];
                var plist = new List<Point>();
                for (int k = j; k <= j1; k++)
                {
                    plist.Add(all_point_list[k]);
                }
                var be = new BoundaryEdge(name, plist, id);
                boundary_list.Add(be);
            }
            /// 构造最后一个界址点和第一个界址点的线段
            var lastPoint = all_point_list.Last();
            var firstPoint = all_point_list.First();
            if (lastPoint.X != firstPoint.X && lastPoint.Y != firstPoint.Y)
            {
                var name = boundary_point_name_list[boundary_point_name_list.Count - 1] + '+' + boundary_point_name_list[0];
                var be = new BoundaryEdge(name, new List<Point>() { lastPoint, firstPoint }, id);
                boundary_list.Add(be);
            }
            foreach (var item in all_point_list)
            {
                allPointList.Add(item);
            }
            return boundary_list;
        }
    }
}