using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoundOrientation
{
    public class FoundOrientationCore : FoundOrientationCoreBase
    {
        /*
         实现紧邻投影法判断东方的投影相交线段的投影长度。
         {param targe_poly_obj{ 目标多边形的对象
         {param test_poly_objs{  测试多边形的对象列表
         {param prior_option{ 查找优先级。
                         0：为默认值，返回一个地块，一个面状物。且地块在面状物之前
                         1{ 地块优先，优先返回地块，如果没有地块，则返回面状物
                         2{ 距离优先。优先返回最近的地块或面状物
         {param projection_length{ 投影线的投影长度，和缓冲距离类似
         {return{东方的相交线投影长度
         */
        // x_center=(targe_poly_obj.x_max+targe_poly_obj.x_min)/2
        // y_center=(targe_poly_obj.y_max+targe_poly_obj.y_min)/2
        // point_center=[x_center,y_center]
        public Dictionary<string, double> get_east_area(Poly targe_poly_obj, List<Edge> edge_poly_east_west_list, /*test_poly_objs, */int prior_option, double projection_length = 3)
        {
            var x_start = targe_poly_obj.x_min - 1;
            var len = (targe_poly_obj.y_max - targe_poly_obj.y_min);
            var step = len / (len > 100 ? 100 : (int)(len / 2));
            var y_start = targe_poly_obj.y_max - step;
            var edge_sin_list = new List<double>();
            var close_ids_lists = new List<List<string>>();
            while (y_start > targe_poly_obj.y_min)
            {
                ///// 首先计算出投影矩形的起始点
                object[] resultObjArray = project_east(new Point(x_start, y_start),
                    targe_poly_obj, edge_poly_east_west_list, prior_option, projection_length + 0.2);
                var projection_point = resultObjArray[0];//(Point)resultObjArray;
                var edge_index = resultObjArray[1];
                var close_obj_id_list_obj = resultObjArray[2];
                var close_obj_id_list = close_obj_id_list_obj as List<string>;// list(set(close_obj_id_list))
                if (projection_point != null)
                {
                    try
                    {
                        edge_sin_list.Add(targe_poly_obj.edge[(int)edge_index].sin);
                        close_ids_lists.Add(close_obj_id_list);
                    }
                    catch
                    {
                        edge_sin_list.Add(0);
                        close_ids_lists.Add(new List<string>());
                    }
                }
                y_start -= step;
            }
            var cross_result = get_cross_result(edge_sin_list, close_ids_lists);
            return cross_result;
        }

        /*
         实现紧邻投影法判断东方的投影相交线段的投影长度。
         {param targe_poly_obj{ 目标多边形的对象
         {param test_poly_objs{  测试多边形的对象列表
         {param prior_option{ 查找优先级。
                     0：为默认值，返回一个地块，一个面状物。且地块在面状物之前
                     1{ 地块优先，优先返回地块，如果没有地块，则返回面状物
                     2{ 距离优先。优先返回最近的地块或面状物
         {param projection_length{ 投影线的投影长度，和缓冲距离类似
         {return{东方的相交线投影长度
        */
        public Dictionary<string, double> get_west_area(Poly targe_poly_obj, List<Edge> edge_poly_east_west_list, /*test_poly_objs, */int prior_option, double projection_length = 3)
        {
            var x_start = targe_poly_obj.x_max + 1;
            var step = (targe_poly_obj.y_max - targe_poly_obj.y_min) / 100;
            var y_start = targe_poly_obj.y_max - step;
            var edge_sin_list = new List<double>();   // //储存变现的sin值，用于判断西至线段的起始和结束位置
            var close_ids_lists = new List<List<string>>();   // //储存close_id_list的list 

            while (y_start > targe_poly_obj.y_min)
            {
                ////首先计算出投影矩形的起始点 
                object[] resultObjArray = project_west(new Point(x_start, y_start),
                    targe_poly_obj, edge_poly_east_west_list, prior_option, projection_length + 0.2);
                var projection_point = resultObjArray[0];
                var edge_index = resultObjArray[1];
                var close_obj_id_list_obj = resultObjArray[2];
                var close_obj_id_list = close_obj_id_list_obj as List<string>;

                if (projection_point != null)
                {
                    try
                    {
                        edge_sin_list.Add(targe_poly_obj.edge[(int)edge_index].sin);
                        close_ids_lists.Add(close_obj_id_list);
                    }
                    catch
                    {
                        {
                            edge_sin_list.Add(0);
                            close_ids_lists.Add(new List<string>());
                        }
                    }
                }
                y_start -= step;
            }
            var cross_result = get_cross_result(edge_sin_list, close_ids_lists);
            return cross_result;
        }
        /*
          实现紧邻投影法判断东方的投影相交线段的投影长度。
          {param targe_poly_obj{ 目标多边形的对象
          {param test_poly_objs{  测试多边形的对象列表
          {param prior_option{ 查找优先级。
                      0：为默认值，返回一个地块，一个面状物。且地块在面状物之前
                      1{ 地块优先，优先返回地块，如果没有地块，则返回面状物
                      2{ 距离优先。优先返回最近的地块或面状物
          {param projection_length{ 投影线的投影长度，和缓冲距离类似
          {return{东方的相交线投影长度
        */
        public Dictionary<string, double> get_north_area(Poly targe_poly_obj, List<Edge> edge_poly_north_south_list,
          int prior_option, double projection_length = 3)
        {
            var y_start = targe_poly_obj.y_min - 1;
            var step = (targe_poly_obj.x_max - targe_poly_obj.x_min) / 100;
            var x_start = targe_poly_obj.x_max - step;
            var edge_cos_list = new List<double>();   // //储存变现的sin值，用于判断西至线段的起始和结束位置
            var close_ids_lists = new List<List<string>>();   // //储存close_id_list的list 

            while (x_start > targe_poly_obj.x_min)
            {
                ////首先计算出投影矩形的起始点 
                object[] resultObjArray = project_north(new Point(x_start, y_start),
                    targe_poly_obj, edge_poly_north_south_list, prior_option, projection_length + 0.2);
                var projection_point = resultObjArray[0];//(Point)resultObjArray;
                var edge_index = resultObjArray[1];
                var close_obj_id_list_obj = resultObjArray[2];
                var close_obj_id_list = close_obj_id_list_obj as List<string>;// list(set(close_obj_id_list))

                if (projection_point != null)
                {
                    try
                    {
                        edge_cos_list.Add(targe_poly_obj.edge[(int)edge_index].cos);
                        close_ids_lists.Add(close_obj_id_list);
                    }
                    catch
                    {
                        edge_cos_list.Add(0);
                        close_ids_lists.Add(new List<string>());
                    }
                }
                x_start -= step;
            }
            var cross_result = get_cross_result(edge_cos_list, close_ids_lists);
            return cross_result;
        }

        /*
          实现紧邻投影法判断南方的投影相交线段的投影长度。
          {param targe_poly_obj{ 目标多边形的对象
          {param test_poly_objs{  测试多边形的对象列表
          {param prior_option{ 查找优先级。
                  0：为默认值，返回一个地块，一个面状物。且地块在面状物之前
                  1{ 地块优先，优先返回地块，如果没有地块，则返回面状物
                  2{ 距离优先。优先返回最近的地块或面状物
          {param projection_length{ 投影线的投影长度，和缓冲距离类似
          {return{东方的相交线投影长度
         */
        public Dictionary<string, double> get_south_area(Poly targe_poly_obj, List<Edge> edge_poly_north_south_list,
            int prior_option, double projection_length = 3)
        {
            var y_start = targe_poly_obj.y_max + 1;
            var step = (targe_poly_obj.x_max - targe_poly_obj.x_min) / 100;
            var x_start = targe_poly_obj.x_max - step;
            var edge_cos_list = new List<double>();
            var close_ids_lists = new List<List<string>>();
            while (x_start > targe_poly_obj.x_min)
            {
                ///// 首先计算出投影矩形的起始点
                object[] resultObjArray = project_south(new Point(x_start, y_start), targe_poly_obj,
                    edge_poly_north_south_list, prior_option, projection_length + 0.2);
                var projection_point = resultObjArray[0];//(Point)resultObjArray;
                var edge_index = resultObjArray[1];
                var close_obj_id_list_obj = resultObjArray[2];
                var close_obj_id_list = close_obj_id_list_obj as List<string>;// list(set(close_obj_id_list))
                if (projection_point != null)
                {
                    try
                    {
                        edge_cos_list.Add(targe_poly_obj.edge[(int)edge_index].cos);
                        close_ids_lists.Add(close_obj_id_list);
                    }
                    catch
                    {
                        edge_cos_list.Add(0);
                        close_ids_lists.Add(new List<string>());
                    }
                }
                x_start -= step;
            }
            var cross_result = get_cross_result(edge_cos_list, close_ids_lists);
            return cross_result;
        }

        /*
          从字符串中提取信息，计算地块四至，并返回结果字符串
          {param field_info_string{ 地块坐标信息。具体格式如下：
          地块ID01：坐标x1,坐标y1,...坐标xn,坐标yn;地块ID02{...;地块ID03：...;|地块ID11...
          其中，地块ID01为需要给出四至的地块，地块ID02和地块ID03为在缓冲距离内的候选地块，各地块之间用“;”间隔。
          地块ID11为另一个需要给出四至的地块。地块ID01字符串和地块ID11字符串之间用“|”间隔。
          间隔符均为英文字符。
          
          如果某个地块是空心地块，有多组坐标。则传递的格式如下。多组坐标之间用";|"分割。
          地块1：x1,y1,x2,y2...xn,yn;|x1,y1,x2,y2...xn,yn;|x1,y1,x2,y2...xn,yn;地块2：x1,y1,x2,y2...xn,yn;
          
          
          也可以只输入一个待测试地块的信息。
          
          {param distance{  缓冲距离，接受int或者float型数值。单位为米
          {param one_field_only{ 是否只返回一个地块。Int型（只能取0或1.避免python与c//的布尔值定义可能的区别）.
                         1为只返回一个地块，0为返回一个列表结果
          {param field_only{ 是否只查询地块。Int型（只能取0或1.避免python与c//的布尔值定义可能的区别）.
                      1为只返回地块结果，0为同时返回地块和面状物结果。
          {param prior_option{ 查找优先级。仅当one_field_only==1和field_only==0时生效。其他条件时，需要设置为0。
          
                      0：为默认值，返回一个地块，一个面状物。且地块在面状物之前
                      1{ 地块优先，优先返回地块，如果没有地块，则返回面状物
                      2{ 距离优先。优先返回最近的地块或面状物
          
          {return{ 地块四至结果的字符串，格式为：
           测试地块的ID：[东至结果列表]
          [南至结果列表]
          [西至结果列表]
          [北至结果列表]|下一个结果
          两个结果之间由"|"分隔。
           如果某一个至向结果为空，则返回的结果是"[]"
          
           例：
           5101151052490200391{[]
          ['M87fd0a4f-1985-420b-aafe-c2b36f83b966']
          []
          ['5101151052490200418']|\
           5101151052490200395{['5101151052490200254', 'X82d72a4b-d363-4db3-b1a3-4e0a5ed13742'] \
           ['5101151052490200382', 'X82d72a4b-d363-4db3-b1a3-4e0a5ed13742']
          ['5101151052490200382', \
           '5101151052490200306']
          ['5101151052490200306', '5101151052490200137', '5101151052490200339']
        */
        public string get_field_orientation_result(string field_info_string, double distance, int one_field_only, int field_only,
            int prior_option = 2, double threshold_value = 0.07)
        {
            var coordinateDic = load_txt_data(field_info_string);
            var rectangle_dic = get_outer_rectangle_dic(coordinateDic);
            var targetIDList = new List<string>();
            var testIDList = new List<List<string>>();
            foreach (var item in rectangle_dic.Keys)
            {
                if (item[0] == 'X' || item[0] == 'M')
                {
                    continue;
                }
                else
                {
                    targetIDList.Add(item);
                    testIDList.Add(get_close_field(item, rectangle_dic, distance));
                }
            }
            var poly_dic = new Dictionary<string, Poly>();
            foreach (var kv in coordinateDic)
            {
                poly_dic[kv.Key] = new Poly(kv.Key, kv.Value);
            }
            var resultString = string.Empty;
            for (int j = 0; j < targetIDList.Count; j++)
            {
                var target_poly = poly_dic[targetIDList[j]];

                var test_poly_objs_field = new List<Poly>();
                var test_poly_objs_others = new List<Poly>();

                var east_west_list_field = new List<Edge>();
                var north_south_list_field = new List<Edge>();
                var east_west_list_others = new List<Edge>();
                var north_south_list_others = new List<Edge>();

                //step1：构造字典k
                //根据地块的属性构造一个地块字典，以及一个其他地物地块字典
                foreach (var item in testIDList[j])
                {
                    if (poly_dic[item].type == "field")
                    {
                        test_poly_objs_field.Add(poly_dic[item]);
                    }
                    else
                    {
                        test_poly_objs_others.Add(poly_dic[item]);
                    }
                }
                //从地块字典中，构造出可能的地块的相交边
                foreach (var test_poly in test_poly_objs_field)
                {
                    for (int i = 0; i < test_poly.edgeNum; i++)
                    {
                        if (target_poly.y_min <= test_poly.edge[i].y_max &&
                            target_poly.y_max >= test_poly.edge[i].y_min)
                        {
                            east_west_list_field.Add(test_poly.edge[i]);
                        }
                        if (target_poly.x_min <= test_poly.edge[i].x_max &&
                            target_poly.x_max >= test_poly.edge[i].x_min)
                        {
                            north_south_list_field.Add(test_poly.edge[i]);
                        }
                    }
                }
                if (field_only != 1)
                {
                    //如果需要查找面状物，则从面线状物字典中，构造出可能的其他地物的相交边
                    foreach (var test_poly in test_poly_objs_others)
                    {
                        for (int i = 0; i < test_poly.edgeNum; i++)
                        {
                            if (target_poly.y_min <= test_poly.edge[i].y_max &&
                                target_poly.y_max >= test_poly.edge[i].y_min)
                            {
                                east_west_list_others.Add(test_poly.edge[i]);
                            }
                            if (target_poly.x_min <= test_poly.edge[i].x_max &&
                                            target_poly.x_max >= test_poly.edge[i].x_min)
                            {
                                north_south_list_others.Add(test_poly.edge[i]);
                            }
                        }
                    }
                }
                #region field_only == 1
                if (field_only == 1)
                {
                    //如果只查地块，则只进行地块的查找
                    var east_field = get_east_area(target_poly, east_west_list_field, prior_option, distance);
                    var west_field = get_west_area(target_poly, east_west_list_field, prior_option, distance);
                    var north_field = get_north_area(target_poly, north_south_list_field, prior_option, distance);
                    var south_field = get_south_area(target_poly, north_south_list_field, prior_option, distance);

                    var eastResult = get_result_list(east_field, threshold_value);
                    var east_field_result = eastResult[0];
                    var east_M_result = eastResult[1];

                    var westResult = get_result_list(west_field, threshold_value);
                    var west_field_result = westResult[0];
                    var west_M_result = westResult[1];

                    var southResult = get_result_list(south_field, threshold_value);
                    var south_field_result = southResult[0];
                    var south_M_result = southResult[1];

                    var northResult = get_result_list(north_field, threshold_value);
                    var north_field_result = northResult[0];
                    var north_M_result = northResult[1];


                    if (one_field_only == 1)
                    {
                        //如果只查地块，且只返回一个地块：
                        resultString += target_poly.id + ":" + str(east_field_result, 1) + " " + str(south_field_result, 1)
                              + " " + str(west_field_result, 1) + " " + str(north_field_result, 1) + '|';
                    }
                    else if (one_field_only == 0)
                    {
                        // 如果只查地块，且只返回所有地块：
                        resultString += target_poly.id + ":" + str(east_field_result, 3) + " " + str(south_field_result, 3)
                              + " " + str(west_field_result, 3) + " " + str(north_field_result, 3) + '|';
                    }
                }
                #endregion

                #region field_only == 0    
                if (field_only == 0)
                {
                    field_onlyEqualZero(east_west_list_field, east_west_list_others, north_south_list_field, north_south_list_others, test_poly_objs_field, test_poly_objs_others, target_poly,
                        one_field_only, threshold_value, prior_option, distance, ref resultString);
                }
                #endregion
            }
            return resultString.Trim('|');
        }


        private void field_onlyEqualZero(List<Edge> east_west_list_field, List<Edge> east_west_list_others,
            List<Edge> north_south_list_field, List<Edge> north_south_list_others,
            List<Poly> test_poly_objs_field, List<Poly> test_poly_objs_others,
            Poly target_poly, int one_field_only,
           double threshold_value, int prior_option, double distance, ref string resultString)
        {
            //如果同时查地块和面状物，则只进行需要将面状物和地块的可能的边合并成一个list
            var east_west_list = Copy(east_west_list_field);
            east_west_list.AddRange(east_west_list_others);

            var north_south_list = Copy(north_south_list_field);
            north_south_list.AddRange(north_south_list_others);

            test_poly_objs_field.AddRange(test_poly_objs_others);

            if (one_field_only == 0)
            {
                // 如果同时查地块和面状物,且需要返回一个列表
                var east_res = get_east_area(target_poly, east_west_list, prior_option, distance);
                var west_res = get_west_area(target_poly, east_west_list, prior_option, distance);
                var north_res = get_north_area(target_poly, north_south_list, prior_option, distance);
                var south_res = get_south_area(target_poly, north_south_list, prior_option, distance);

                var eastResult = get_result_list(east_res, threshold_value);
                var east_field_result = eastResult[0];
                var east_M_result = eastResult[1];

                var westResult = get_result_list(west_res, threshold_value);
                var west_field_result = westResult[0];
                var west_M_result = westResult[1];

                var southResult = get_result_list(south_res, threshold_value);
                var south_field_result = southResult[0];
                var south_M_result = southResult[1];

                var northResult = get_result_list(north_res, threshold_value);
                var north_field_result = northResult[0];
                var north_M_result = northResult[1];

                var east_result = east_field_result;
                east_result.AddRange(east_M_result);

                var west_result = west_field_result;
                west_result.AddRange(west_M_result);

                var south_result = south_field_result;
                south_result.AddRange(south_M_result);

                var north_result = north_field_result;
                north_result.AddRange(north_M_result);

                resultString += target_poly.id + ":" + str(east_result, 3) + " " + str(south_result, 3)
                        + " " + str(west_result, 3) + " " + str(north_result, 3) + '|';
            }
            if (one_field_only == 1)
            {
                //如果同时查地块和面状物,且需要一个值
                if (prior_option == 0)
                {
                    //优先级为默认的话，返回两个值，一个地块，一个面状物，地块在前
                    var east_res = get_east_area(target_poly, east_west_list, prior_option, distance);
                    var west_res = get_west_area(target_poly, east_west_list, prior_option, distance);
                    var north_res = get_north_area(target_poly, north_south_list, prior_option, distance);
                    var south_res = get_south_area(target_poly, north_south_list, prior_option, distance);

                    var eastResult = get_result_list(east_res, threshold_value);
                    var east_field_result = eastResult[0];
                    var east_M_result = eastResult[1];

                    var westResult = get_result_list(west_res, threshold_value);
                    var west_field_result = westResult[0];
                    var west_M_result = westResult[1];

                    var southResult = get_result_list(south_res, threshold_value);
                    var south_field_result = southResult[0];
                    var south_M_result = southResult[1];

                    var northResult = get_result_list(north_res, threshold_value);
                    var north_field_result = northResult[0];
                    var north_M_result = northResult[1];//

                    var east_result = Part(east_field_result, 1);
                    east_result.AddRange(Part(east_M_result, 1));

                    var west_result = Part(west_field_result, 1);
                    west_result.AddRange(Part(west_M_result, 1));

                    var south_result = Part(south_field_result, 1);
                    south_result.AddRange(Part(south_M_result, 1));

                    var north_result = Part(north_field_result, 1);
                    north_result.AddRange(Part(north_M_result, 1));

                    resultString += target_poly.id + ":" + str(east_result) + " " + str(south_result)
                          + " " + str(west_result) + " " + str(north_result) + '|';
                }
                if (prior_option == 1)
                {
                    //优先级为默认的话，返回一个值，如果有地块，返回地块，否则返回面状物
                    // 优先级为默认的话，返回两个值，一个地块，一个面状物，地块在前
                    var east_res = get_east_area(target_poly, east_west_list, prior_option, distance);
                    var west_res = get_west_area(target_poly, east_west_list, prior_option, distance);
                    var north_res = get_north_area(target_poly, north_south_list, prior_option, distance);
                    var south_res = get_south_area(target_poly, north_south_list, prior_option, distance);

                    var eastResult = get_result_list(east_res, threshold_value);
                    var east_field_result = eastResult[0];
                    var east_M_result = eastResult[1];

                    var westResult = get_result_list(west_res, threshold_value);
                    var west_field_result = westResult[0];
                    var west_M_result = westResult[1];

                    var southResult = get_result_list(south_res, threshold_value);
                    var south_field_result = southResult[0];
                    var south_M_result = southResult[1];

                    var northResult = get_result_list(north_res, threshold_value);
                    var north_field_result = northResult[0];
                    var north_M_result = northResult[1];


                    var east_result = Part(east_field_result, 1, true);
                    var west_result = Part(west_field_result, 1, true);
                    var south_result = Part(south_field_result, 1, true);
                    var north_result = Part(north_field_result, 1, true);

                    if (east_result.Count == 0)
                        east_result = Part(east_M_result, 1);
                    if (west_result.Count == 0)
                        west_result = Part(west_M_result, 1);
                    if (south_result.Count == 0)
                        south_result = Part(south_M_result, 1);
                    if (north_result.Count == 0)
                        north_result = Part(north_M_result, 1);

                    resultString += target_poly.id + ":" + str(east_result) + " " + str(south_result)
                            + " " + str(west_result) + " " + str(north_result) + '|';
                }

                if (prior_option == 2)
                {
                    var east_res_dis = get_east_area(target_poly, east_west_list, prior_option, distance);
                    var west_res_dis = get_west_area(target_poly, east_west_list, prior_option, distance);
                    var north_res_dis = get_north_area(target_poly, north_south_list, prior_option, distance);
                    var south_res_dis = get_south_area(target_poly, north_south_list, prior_option, distance);

                    var east_result = Part(get_result_list_all(east_res_dis, threshold_value), 1, true);
                    var west_result = Part(get_result_list_all(west_res_dis, threshold_value), 1, true);
                    var north_result = Part(get_result_list_all(north_res_dis, threshold_value), 1, true);
                    var south_result = Part(get_result_list_all(south_res_dis, threshold_value), 1, true);

                    resultString += target_poly.id + ":" + str(east_result) + " " + str(south_result)
                            + " " + str(west_result) + " " + str(north_result) + '|';
                }
            }
        }
    }
}
