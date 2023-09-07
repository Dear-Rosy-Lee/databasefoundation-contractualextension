#encoding:utf-8
'''
界址线毗邻地物权利人查找主文件。
'''
# import time
def data_to_dic(input_string):
    '''
    读取数据
    :param input_string: 输入的字符串
    :return: coordinate_dic：地块坐标的字典，key为地块ID，value为地块的各顶点坐标
             boundary_dic: 界址边坐标的字典，key为地块ID，value为界址边的信息。value格式为：
             [[['J16,J18', (509011.447327, 2519227.910095),...],['J18,J19',(x,y),(x,y)],[...]],[第二圈的信息],[...]]
    '''
    input_string=input_string.replace(';|','&')
    raw_data_list=(input_string.strip(';')).split(';')
    coordinate_dic={}
    boundary_dic={}
    for item in raw_data_list:
        id, coordinates_list,bourndary_list=getCoordiantes(item)
        coordinate_dic[id]=coordinates_list
        boundary_dic[id]=bourndary_list
    return coordinate_dic,boundary_dic

def getCoordiantes(fieldInfoList):
    '''
    从土地信息列表里得到fieldID信息，以及坐标信息
    :param fieldInfoList:
    :return: id：地块的id信息
              coordinates_list: 坐标列表
    '''
    coordinates_list = []
    boundary_list=[]
    results_=fieldInfoList.split(':')    #信息的格式为：ID:x,y,x,y...x,y&x,y,x,y...x,y
    id=results_[0]
    item_list=results_[1].split('&')

    if id[0]!='X' and id[0]!='M':
        for coordinates_string in item_list:
            edge_list,points_list=split_coordinates(coordinates_string)
            boundary_list.append(edge_list)
            coordinates_list.append(points_list)
    else:
        for coordinates_string in item_list:
            temp = []
            item = coordinates_string.split(',')
            # print id
            for i in xrange(0, len(item), 2):
                x = float(item[i])
                y = float(item[i + 1])
                temp.append([x, y])
            coordinates_list.append(temp)
            boundary_list=[]
    return id,coordinates_list,boundary_list

def split_coordinates(point_string):
    '''
    分割界址点，并组合成界址线列表
    :param point_list: 输入的点坐标
    :return:boundary_list: 界址线段的坐标。格式为：{'J1,J2':[(x1,y1),(x2,y2)],'J2,J3':[(x1,y1),(x2,y2)],...}
            all_point_list: 点坐标。格式为[(x1,y1),(x2,y2)...]
    '''
    point_list=point_string.split(',')
    boundary_point_index_list=[]    #指示界址点在all_point_list中的哪些index上
    all_point_list = []             #所有点的坐标
    boundary_point_name_list=[]     #指示界址点的名称
    boundary_list=[]                #界址线段的坐标

    for i in xrange(0,len(point_list),2):
        if '#' in str(point_list[i]):
            boundary_point_index_list.append(i/2)
            x_list=point_list[i].split('#')
            y_list=point_list[i+1].split('#')
            all_point_list.append((float(x_list[1]),float(y_list[1])))
            boundary_point_name_list.append(x_list[0])
        else:
            all_point_list.append((float(point_list[i]), float(point_list[i + 1])))

    for i in range(len(boundary_point_index_list)-1):
        edge_point_list=[]
        for j in xrange(boundary_point_index_list[i],boundary_point_index_list[i+1]+1):
            edge_point_list.append((all_point_list[j]))
        temp=[boundary_point_name_list[i]+'+'+boundary_point_name_list[i+1]]
        temp.extend(edge_point_list)
        boundary_list.append(temp)

    #构造最后一个界址点和第一个界址点的线段
    edge_point_list = all_point_list[boundary_point_index_list[-1]:]
    edge_point_list.extend(all_point_list[:boundary_point_index_list[0]+1])
    temp = [boundary_point_name_list[-1] + '+' + boundary_point_name_list[0]]
    if boundary_point_name_list[-1]==boundary_point_name_list[0]:
        pass
    else:
        temp.extend(edge_point_list)
        boundary_list.append(temp)

    return boundary_list,all_point_list

def get_outer_rectangle_dic(coordinate_dic):
    '''
    从坐标字典中计算出各地块物体的外接矩形顶点坐标字典。
    key为地块id，value为[[x_min,y_min],[x_max,y_max]]
    :param coordinate_dic:坐标字典
    :return:rectangle_dic：外接矩形字典
    '''
    rectangle_dic={}
    for k,v in coordinate_dic.items():
        x_min=y_min=float("inf"); x_max=y_max=0
        for coordinate_dic_list in v:
            for coordinate_pair in coordinate_dic_list:
                if coordinate_pair[0]<x_min: x_min=coordinate_pair[0]
                if coordinate_pair[0]>x_max: x_max=coordinate_pair[0]
                if coordinate_pair[1]<y_min: y_min=coordinate_pair[1]
                if coordinate_pair[1]>y_max: y_max=coordinate_pair[1]
            rectangle_dic[k]=[[x_min,y_min],[x_max,y_max]]
    return rectangle_dic

def get_id_list(rectangle_dic,distance):
    '''
    迭代统计所有的
    :param rectangle_dic:
    :param distance:
    :return:
    '''
    targetID_list=[]
    testID_list=[]
    for item in rectangle_dic.keys():
        if item[0] == 'X' or item[0] == 'M':
            pass
        else:
            targetID_list.append(item)
            testID_list.append(get_close_field(item,rectangle_dic,distance))
    return targetID_list,testID_list

def get_close_field(inputID,rectangle_dic,distance):
    '''
    快速而粗略的判断相邻的地块编码，判断原理如下：
    如果测试地块的x/y_max<目标地块的x/y_min-distance: 在外面
    如果测试地块的x/y_min>目标地块的x/y_max+distance: 在外面
    以上两个(四个）条件均为否，则在里面
    :param inputID: 目标地块的ID
    :param rectangle_dic: 外接矩形字典
    :param distance: 缓冲距离
    :return: return_list 相邻地块编码列表
    '''
    x_min_check=rectangle_dic[inputID][0][0] - distance
    y_min_check = rectangle_dic[inputID][0][1] - distance
    x_max_check=rectangle_dic[inputID][1][0] + distance
    y_max_check = rectangle_dic[inputID][1][1] + distance
    return_list=[]
    for k,v in rectangle_dic.items():
        x_min = v[0][0];  y_min = v[0][1]
        x_max = v[1][0];  y_max = v[1][1]
        if x_min > x_max_check or y_min > y_max_check or x_max < x_min_check or y_max < y_min_check:
            pass
        else:
            return_list.append(k)
    return_list.remove(inputID)
    return return_list

def get_formated_data(field_info_string,distance):
    '''
    从字符串和缓冲距离中，查找出可能的毗邻地块或面线状物
    :param field_info_string: 输入的字符串
    :param distance: 缓冲距离
    :return: targetID_list: 需要测试的目标地块列表
             testID_list: 对应目标地块的测试地块列表
             coordinates_dic：坐标字典
             boundary_dic：界址边字典
    '''
    coordinates_dic, boundary_dic = data_to_dic(field_info_string)
    rectangle_dic=get_outer_rectangle_dic(coordinates_dic)
    targetID_list, testID_list = get_id_list(rectangle_dic, distance)
    return targetID_list, testID_list, coordinates_dic,boundary_dic

def orientation_data_to_dic(orientation_string):
    '''
    从地块四至字符串中获取地块四至的dic
    :param orientation_string:
                地块四至字符串，格式为：
                地块1编码:东至地块编码,西至地块编码,南至地块编码,北至地块编码;地块2编码:...

                某个至向如果有多个地块，各地块编码之间用"|"连接。


    :return:
    '''
    field_list=orientation_string.split(';')
    return_dic={}
    for field_info in field_list:
        field_info_list=field_info.split(':')
        if len(field_info_list)==2:
            field_id=field_info_list[0]
            orientation_info=field_info_list[1]
            orientation_list=orientation_info.split(',')
            formated_orientation_list = []
            if (len(orientation_list))==4:
                for i in range(4):
                    one_orientation_list=orientation_list[i].split("|")
                    formated_orientation_list.append(one_orientation_list)
            return_dic[field_id]=formated_orientation_list
    return return_dic


'''
多边形的类
'''

class Poly(object):
    '''
    多边形的类
    '''

    def __init__(self,ID,point_array_list,boundary_array_list):
        '''
        类的构造函数
        edge的格式为[ymax,ymin,delta-x,point1(y较大),point2(y较小)]
        注意这里：x_new=x_old减去delta-x
        :param pointList:库多边形得各顶点的坐标列表
        :return:
        '''
        self.edge=[]
        self.boundary_edge_out=[]
        self.boundary_edge_in=[]
        self.y_max=0
        self.y_min=float("inf")
        self.x_max=0
        self.x_min=float("inf")
        self.id=ID

        # if ID[0]!='X':
        #     #如果不是线状物，则将第一个点添加到点列表中。最后一个点会和第一个点组成一个线段
        #     for pointList in point_array_list:
        #         pointList.append(pointList[0])

        for pointList in point_array_list:
            firstPoint=pointList[0]
            lastPoint= pointList[len(pointList)-1]
            if firstPoint[0]!= lastPoint[0] and firstPoint[1]!= lastPoint[1]:
                pointList.append(pointList[0])
            for n in range(len(pointList)-1):
                if pointList[n][1]>pointList[n+1][1]:  #如果第n个点较大:
                    y_max=pointList[n][1]
                    y_min=pointList[n+1][1]
                    x_difference=(float(pointList[n+1][0]-pointList[n][0]))
                    if x_difference!=0:
                        delta_x=(y_min-y_max)/x_difference
                    else:
                        delta_x=0
                    point1=pointList[n]
                    point2=pointList[n+1]
                    self.edge.append(Edge(y_max, y_min, delta_x, point1, point2,ID))
                    if y_max>self.y_max: self.y_max=y_max
                    if y_min<self.y_min: self.y_min=y_min
                    if max([pointList[n+1][0],pointList[n][0]])>self.x_max:self.x_max=max([pointList[n+1][0],pointList[n][0]])
                    if min([pointList[n+1][0],pointList[n][0]])<self.x_min:self.x_min=min([pointList[n+1][0],pointList[n][0]])
                else: #如果第n+1个点较大:
                    y_max=pointList[n+1][1]
                    y_min=pointList[n][1]
                    x_difference=(float(pointList[n][0]-pointList[n+1][0]))
                    if x_difference!=0:
                        delta_x=(y_min-y_max)/x_difference
                    else:
                        delta_x=0
                    point1=pointList[n+1]
                    point2=pointList[n]
                    self.edge.append(Edge(y_max,y_min,delta_x,point1,point2,ID))
                    if y_max>self.y_max: self.y_max=y_max
                    if y_min<self.y_min: self.y_min=y_min
                    if max([pointList[n+1][0],pointList[n][0]])>self.x_max:self.x_max=max([pointList[n+1][0],pointList[n][0]])
                    if min([pointList[n+1][0],pointList[n][0]])<self.x_min:self.x_min=min([pointList[n+1][0],pointList[n][0]])
        self.center_point=((self.x_max+self.x_min)/2,(self.y_min+self.y_max)/2)
        self.edgeNum=len(self.edge)
        self.pointList=point_array_list
        self.boundary_array_list=boundary_array_list


        for i in range(len(boundary_array_list)):
            for boundary_array in boundary_array_list[i]:
                if i==0:
                    boundary_edge_out_temp=Boundary_edge(boundary_array,ID)
                    self.boundary_edge_out.append(boundary_edge_out_temp)
                else:
                    boundary_edge_in_temp=Boundary_edge(boundary_array,ID)
                    self.boundary_edge_in.append(boundary_edge_in_temp)

class Boundary_edge(object):
    '''
    多边形的界址边的类
    '''

    def __init__(self,boundary_array,ID):
        self.position = ''
        self.boundary_simple_orientation = ''
        self.closest_first_result = ''  # 最毗邻地物
        self.closest_result=[]          #最毗邻地物列表


        edge_list = []
        self.close_filed_list=[]
        self.y_max = 0
        self.y_min = float("inf")
        self.x_max = 0
        self.x_min = float("inf")
        self.name=boundary_array[0]
        pointList=boundary_array[1:]
        length = 0
        for n in range(len(pointList) - 1):
            length+=((pointList[n+1][1]-pointList[n][1])**2+(pointList[n+1][0]-pointList[n][0])**2)**0.5
            if pointList[n][1] > pointList[n + 1][1]:  # 如果第n个点较大:
                y_max = pointList[n][1]
                y_min = pointList[n + 1][1]
                x_difference = (float(pointList[n + 1][0] - pointList[n][0]))
                if x_difference != 0:
                    delta_x = (y_min - y_max) / x_difference
                else:
                    delta_x = 0
                point1 = pointList[n]
                point2 = pointList[n + 1]
                edge_list.append(Edge(y_max, y_min, delta_x, point1, point2, ID))
                if y_max > self.y_max: self.y_max = y_max
                if y_min < self.y_min: self.y_min = y_min
                if max([pointList[n + 1][0], pointList[n][0]]) > self.x_max: self.x_max = max(
                    [pointList[n + 1][0], pointList[n][0]])
                if min([pointList[n + 1][0], pointList[n][0]]) < self.x_min: self.x_min = min(
                    [pointList[n + 1][0], pointList[n][0]])
            else:  # 如果第n+1个点较大:
                y_max = pointList[n + 1][1]
                y_min = pointList[n][1]
                x_difference = (float(pointList[n][0] - pointList[n + 1][0]))
                if x_difference != 0:
                    delta_x = (y_min - y_max) / x_difference
                else:
                    delta_x = 0
                point1 = pointList[n + 1]
                point2 = pointList[n]
                edge_list.append(Edge(y_max, y_min, delta_x, point1, point2, ID))
                if y_max > self.y_max: self.y_max = y_max
                if y_min < self.y_min: self.y_min = y_min
                if max([pointList[n + 1][0], pointList[n][0]]) > self.x_max: self.x_max = max(
                    [pointList[n + 1][0], pointList[n][0]])
                if min([pointList[n + 1][0], pointList[n][0]]) < self.x_min: self.x_min = min(
                    [pointList[n + 1][0], pointList[n][0]])
        self.boundary_orientation=get_edge_orientation(pointList[0],pointList[-1])

        # 将界址线的方向简化为东南，东北，西南，西北
        # E, F, G, H分别为为东南，东北，西南，西北
        if self.boundary_orientation == 'A' or self.boundary_orientation == 'B' or \
                        self.boundary_orientation == 'C' or self.boundary_orientation == 'D':
            self.boundary_simple_orientation = self.boundary_orientation
        if self.boundary_orientation == 'I' or self.boundary_orientation == 'J':
            self.boundary_simple_orientation = 'E'
        if self.boundary_orientation == 'M' or self.boundary_orientation == 'N':
            self.boundary_simple_orientation = 'G'
        if self.boundary_orientation == 'K' or self.boundary_orientation == 'L':
            self.boundary_simple_orientation = 'F'
        if self.boundary_orientation == 'O' or self.boundary_orientation == 'P':
            self.boundary_simple_orientation = 'H'
        self.length=length
        self.edge_list=edge_list

class Edge(object):
    '''
    多边形的边的类
    '''
    def __init__(self,y_max,y_min,delta_x,point1,point2,id):
        '''
        类的构造函数
        :param y_max:
        :param y_min:
        :param delta_x: 两个点的坐标x的差值
        :param point1:
        :param point2:
        '''
        self.y_max=y_max
        self.y_min=y_min
        self.x_max=max([point1[0],point2[0]])
        self.x_min=min([point1[0],point2[0]])
        self.delta_x=delta_x
        self.point1=point1
        self.point2=point2
        self.id=id
        try:
            sin=(self.y_max-self.y_min)/(
                ((self.y_max-self.y_min)**2+(self.x_max-self.x_min)**2)**0.5)
            if sin<0.4:
                self.sin=0
            elif sin<0.7:
                self.sin=sin/10
            else:
                self.sin=sin

            cos = (self.x_max - self.x_min) / (
                ((self.y_max - self.y_min) ** 2 + (self.x_max - self.x_min) ** 2) ** 0.5)
            if cos<0.4:
                self.cos=0
            elif cos<0.7:
                self.cos=cos/10
            else:
                self.cos=cos

        except Exception, e:
            self.sin=0
            self.cos=0

def get_edge_orientation(point1,point2):
    '''
    以第一个点和最后一个点为基准，计算界址线段的方位
    :param point_list: 坐标点列表，以顺时针排序
    :return: 该界址线段的方位
             A,B,C,D：分别为东南西北。
             I,J,分别为为东南(偏东)，东南(偏南)
             K,L,分别为为东北(偏东)，东北(偏北)
             M,N,分别为为西南(偏西)，西南(偏南)
             O,P,分别为为西北(偏西)，西北(偏北)
    '''
    x1,y1=point1
    x2,y2=point2
    #避免除数为0
    if x1==x2:
        x1+=0.0001
    tan=(y2-y1)/(x2-x1)         #计算正切值
    if -0.414<=tan and tan<0.414:
        # 正东方和正西的角度为-22.5到+22.5. 对应的正切为：-0.414 21 到0.414 21 之间
        if x2>=x1:
            orientation='A'      #正东方
        else:
            orientation='C'      #正西方
    if 0.414<=tan and tan<1:
        # 东北（偏东）和西南（偏西）的角度为+22.5到+45. 对应的正切为：0.414 21 到1之间
        if x2>=x1:
            orientation='K'      #东北（偏东）
        else:
            orientation='M'      #西南（偏西）

    if 1<=tan and tan<2.414:
        # 东北（偏北）和西南（偏南）的角度为+45到+67.5. 对应的正切为：1 到2.414 2 之间
        if x2>=x1:
            orientation='L'      #东北（偏北）
        else:
            orientation='N'      #西南（偏南）

    if 2.414<=tan or tan<-2.414:
        # 正北方和正南的角度为+67.5到112.5. 对应的正切为：大于2.414 2，或者小于-2.414 2 之间
        if y2>=y1:
            orientation = 'D'    #正北方
        else:
            orientation = 'B'    #正南方

    if -2.414<=tan and tan<-1:
        # 西北(偏北)和东南(偏南)方向的角度为112.5到135. 对应的正切为：-2.414 2和-1之间。
        if y2>=y1:
            orientation = 'P'    #西北（偏北）
        else:
            orientation= 'J'     #东南(偏南)

    if -1<=tan and tan<-0.414:
        # 西北(偏西)和东南(偏东)方向的角度为135到157.5. 对应的正切为：-1和-0.414 21之间。
        if y2>=y1:
            orientation = 'O'    #西北(偏西)
        else:
            orientation= 'I'     #东南(偏东)
    return orientation

def east_north_cross(point,edge):
    '''
    计算点向东北或西南方发出的射线与边的交点
    :param point: 发出射线的点
    :param edge: 边的对象
    :return: [x,y]，交点坐标
    '''
    x1,y1=edge.point1
    x2,y2=edge.point2
    x0,y0=point
    dividend=float(x2-x1+y1-y2)
    if dividend!=0:
        x=(x2*y1-x1*y2-(y0-x0)*(x2-x1))/dividend
        y=x+(y0-x0)
        return [x,y]
    else:
        return None

def west_north_cross(point,edge):
    '''
    计算点向西北或东南方发出的射线与边的交点
    :param point: 发出射线的点
    :param edge: 边的对象
    :return: [x,y]，交点坐标
    '''
    x1,y1=edge.point1
    x2,y2=edge.point2
    x0,y0=point
    dividend=float(x2-x1+y2-y1)
    if dividend!=0:
        x=((x0+y0)*(x2-x1)-(x2*y1-x1*y2))/dividend
        y=-x+(y0+x0)
        return [x,y]
    else:
        return None

'''
实现紧邻投影法判断方位
地块落在紧邻投影范围内的算法由扫线法实现
投影起始点为地块的各个边
'''

def get_boundary_close_fields(boundary_edge,edge_poly_list,test_poly_objs,distance,in_or_out,priori):
    '''
    查询界址线的最近毗邻地块
    :param boundary_edge:
    :param edge_poly_list:
    :param test_poly_objs:
    :param distance:
    :param in_or_out:
    :param priori:
    :return:
    '''
    boundary_edge_orientation=boundary_edge.boundary_orientation
    close_obj_result={}
    closest_result={}
    if boundary_edge_orientation=='B':
        if in_or_out=='out':
            #外圈的向南的界址边，需要向东发出射线
            close_obj_result,closest_result = get_east_close_field(boundary_edge,edge_poly_list,test_poly_objs,distance)
        if in_or_out == 'in':
            # 内圈的向南的界址边，需要向西发出射线
            close_obj_result,closest_result = get_west_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance)
    elif boundary_edge_orientation=='D':
        if in_or_out=='out':
            # 外圈的向北的界址边，需要向西发出射线
            close_obj_result,closest_result = get_west_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance)
        if in_or_out == 'in':
            # 外圈的向北的界址边，需要向东发出射线
            close_obj_result,closest_result = get_east_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance)
    elif boundary_edge_orientation=='A':
        if in_or_out=='out':
            # 外圈的向东的界址边，需要向北发出射线
            close_obj_result,closest_result = get_north_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance)
        if in_or_out == 'in':
            # 外圈的向东的界址边，需要向南发出射线
            close_obj_result,closest_result = get_south_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance)
    elif boundary_edge_orientation=='C':
        if in_or_out=='out':
            # 外圈的向西的界址边，需要向南发出射线
            close_obj_result,closest_result = get_south_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance)
        if in_or_out=='in':
            close_obj_result,closest_result = get_north_close_field(boundary_edge, edge_poly_list, test_poly_objs, distance)
    elif boundary_edge_orientation=='J' or boundary_edge_orientation=='N':
        if in_or_out=='out':
            # 外圈的东南偏南，或西南偏南方向的界址边，需要向东发出射线
            close_obj_result, closest_result = get_east_close_field(boundary_edge, edge_poly_list, test_poly_objs,
                                                                     distance)
        if in_or_out == 'in':
            # 内圈的东南偏南，或西南偏南方向的界址边，需要向西发出射线
            close_obj_result, closest_result = get_east_close_field(boundary_edge, edge_poly_list, test_poly_objs,
                                                                          distance)
    elif boundary_edge_orientation=='L' or boundary_edge_orientation=='P':
        if in_or_out=='out':
            # 外圈的东北偏北，或西北偏北的界址边，需要向西发出射线
            close_obj_result, closest_result = get_west_close_field(boundary_edge, edge_poly_list, test_poly_objs,
                                                                     distance)
        if in_or_out == 'in':
            # 内圈的东北偏北，或西北偏北的界址边，需要向东发出射线
            close_obj_result, closest_result = get_east_close_field(boundary_edge, edge_poly_list, test_poly_objs,
                                                                          distance)
    elif boundary_edge_orientation=='I' or boundary_edge_orientation=='K':
        if in_or_out=='out':
            # 外圈的东南偏东，或东北偏东方向的界址边，需要向北发出射线
            close_obj_result, closest_result = get_north_close_field(boundary_edge, edge_poly_list, test_poly_objs,
                                                                     distance)
        if in_or_out == 'in':
            # 内圈的东南偏东，或东北偏东方向的界址边，需要向南发出射线
            close_obj_result, closest_result = get_south_close_field(boundary_edge, edge_poly_list, test_poly_objs,
                                                                          distance)
    elif boundary_edge_orientation=='M'or boundary_edge_orientation=='O':
        if in_or_out=='out':
            # 外圈的西南偏西，或西北偏西方向的界址边，需要向南发出射线
            close_obj_result, closest_result = get_south_close_field(boundary_edge, edge_poly_list, test_poly_objs,
                                                                     distance)
        if in_or_out == 'in':
            # 内圈的西南偏西，或西北偏西方向的界址边，需要向北发出射线
            close_obj_result, closest_result = get_north_close_field(boundary_edge, edge_poly_list, test_poly_objs,
                                                                          distance)
    else:
        pass
    return close_obj_result,closest_result


def get_east_close_field(boundary_edge,edge_poly_list,test_poly_objs,distance):
    '''
    实现界址边的东向投影算法
    :param boundary_edge: 界址边对象
    :param edge_poly_list: 测试多边形的边对象列表
    :param test_poly_objs: 测试多边形的对象列表
    :param distance: 投影线的投影长度，和缓冲距离类似
    :return:
    '''
    cross_result={}
    closest_result={}
    x_start = boundary_edge.x_min - 1
    step=(boundary_edge.y_max-boundary_edge.y_min)/100
    y_start = boundary_edge.y_max - step
    while y_start>boundary_edge.y_min:
        # 首先计算出投影矩形的起始点
        projection_point, close_obj_id_list,closest_obj_id=project_east([x_start,y_start],boundary_edge,edge_poly_list,distance)
        if projection_point:
            for j in range(len(test_poly_objs)):
                if test_poly_objs[j].id in close_obj_id_list:
                    if cross_result.get(test_poly_objs[j].id):
                        cross_result[test_poly_objs[j].id] += 1
                    else:
                        cross_result[test_poly_objs[j].id] = 1
                if test_poly_objs[j].id==closest_obj_id:
                    if closest_result.get(test_poly_objs[j].id):
                        closest_result[test_poly_objs[j].id] += 1
                    else:
                        closest_result[test_poly_objs[j].id] = 1
        y_start-=step
    return cross_result,closest_result

def get_west_close_field(boundary_edge,edge_poly_list,test_poly_objs,distance):
    '''
    实现界址边的西向投影算法
    :param boundary_edge: 界址边对象
    :param edge_poly_list: 测试多边形的边对象列表
    :param test_poly_objs: 测试多边形的对象列表
    :param distance: 投影线的投影长度，和缓冲距离类似
    :return:
    '''
    cross_result={}
    closest_result={}
    x_start = boundary_edge.x_max + 1
    step=(boundary_edge.y_max-boundary_edge.y_min)/100
    y_start = boundary_edge.y_max - step
    while y_start>boundary_edge.y_min:
        # 首先计算出投影矩形的起始点
        projection_point, close_obj_id_list,closest_obj_id=project_west([x_start,y_start],boundary_edge,edge_poly_list,distance)
        if projection_point:
            for j in range(len(test_poly_objs)):
                if test_poly_objs[j].id in close_obj_id_list:
                    if cross_result.get(test_poly_objs[j].id):
                        cross_result[test_poly_objs[j].id] += 1
                    else:
                        cross_result[test_poly_objs[j].id] = 1
                if test_poly_objs[j].id==closest_obj_id:
                    if closest_result.get(test_poly_objs[j].id):
                        closest_result[test_poly_objs[j].id] += 1
                    else:
                        closest_result[test_poly_objs[j].id] = 1
        y_start-=step
    return cross_result,closest_result


def get_north_close_field(boundary_edge,edge_poly_list,test_poly_objs,distance):
    '''
    实现界址边的北向投影算法
    :param boundary_edge: 界址边对象
    :param edge_poly_list: 测试多边形的边对象列表
    :param test_poly_objs: 测试多边形的对象列表
    :param distance: 投影线的投影长度，和缓冲距离类似
    :return:
    '''
    cross_result={}
    closest_result={}
    y_start = boundary_edge.y_min - 1
    step=(boundary_edge.x_max-boundary_edge.x_min)/100
    x_start = boundary_edge.x_max - step
    while x_start>boundary_edge.x_min:
        # 首先计算出投影矩形的起始点
        projection_point, close_obj_id_list,closest_obj_id=project_north([x_start,y_start],boundary_edge,edge_poly_list,distance)
        if projection_point:
            for j in range(len(test_poly_objs)):
                if test_poly_objs[j].id in close_obj_id_list:
                    if cross_result.get(test_poly_objs[j].id):
                        cross_result[test_poly_objs[j].id] += 1
                    else:
                        cross_result[test_poly_objs[j].id] = 1
                if test_poly_objs[j].id==closest_obj_id:
                    if closest_result.get(test_poly_objs[j].id):
                        closest_result[test_poly_objs[j].id] += 1
                    else:
                        closest_result[test_poly_objs[j].id] = 1
        x_start-=step
    return cross_result,closest_result

def get_south_close_field(boundary_edge,edge_poly_list,test_poly_objs,distance):
    '''
    实现界址边的南向投影算法
    :param boundary_edge: 界址边对象
    :param edge_poly_list: 测试多边形的边对象列表
    :param test_poly_objs: 测试多边形的对象列表
    :param distance: 投影线的投影长度，和缓冲距离类似
    :return:
    '''
    cross_result={}
    closest_result={}
    y_start = boundary_edge.y_max + 1
    step=(boundary_edge.x_max-boundary_edge.x_min)/100
    x_start = boundary_edge.x_max - step
    while x_start>boundary_edge.x_min:
        # 首先计算出投影矩形的起始点
        projection_point, close_obj_id_list,closest_obj_id=project_south([x_start,y_start],boundary_edge,edge_poly_list,distance)
        if projection_point:
            for j in range(len(test_poly_objs)):
                if test_poly_objs[j].id in close_obj_id_list:
                    if cross_result.get(test_poly_objs[j].id):
                        cross_result[test_poly_objs[j].id] += 1
                    else:
                        cross_result[test_poly_objs[j].id] = 1
                    if test_poly_objs[j].id == closest_obj_id:
                        if closest_result.get(test_poly_objs[j].id):
                            closest_result[test_poly_objs[j].id] += 1
                        else:
                            closest_result[test_poly_objs[j].id] = 1
        x_start-=step
    return cross_result,closest_result



def get_east_north_close_field(boundary_edge,edge_poly_list,test_poly_objs,distance):
    '''
    实现界址边的东北向投影算法
    :param boundary_edge: 界址边对象
    :param edge_poly_list: 测试多边形的边对象列表
    :param test_poly_objs: 测试多边形的对象列表
    :param distance: 投影线的投影长度，和缓冲距离类似
    :return:
    '''
    cross_result={}
    closest_result={}
    x_start = boundary_edge.x_min - 1
    step=(boundary_edge.y_max-boundary_edge.y_min)/100
    y_start = boundary_edge.y_max - step
    while y_start>boundary_edge.y_min-20*step:
        # 首先计算出投影矩形的起始点
        projection_point, close_obj_id_list,closest_obj_id=project_east([x_start,y_start],boundary_edge,edge_poly_list,distance)
        if projection_point:
            for j in range(len(test_poly_objs)):
                if test_poly_objs[j].id in close_obj_id_list:
                    if cross_result.get(test_poly_objs[j].id):
                        cross_result[test_poly_objs[j].id] += 1
                    else:
                        cross_result[test_poly_objs[j].id] = 1
                if test_poly_objs[j].id==closest_obj_id:
                    if closest_result.get(test_poly_objs[j].id):
                        closest_result[test_poly_objs[j].id] += 1
                    else:
                        closest_result[test_poly_objs[j].id] = 1
        y_start-=step
    return cross_result,closest_result


def project_east(point,boundary_edge,test_edge_list,projection_length):
    '''
    向东发出射线，并返回相近的边的列表
    :param point:测试点
    :param boundary_edge：界址边
    :param test_edge_list:多边形的对象的列表
    :param projection_length: 缓冲距离
    :return:cross_point_target：界址边上发出射线的点
                close_obj_id_list：相近的毗邻地物
                closest_id: 最近的毗邻地物
    '''

    cross_point_target=[point[0],point[1]]
    close_obj_id_list=[]
    closest_id=None
    closest_x=float("inf")
    #如果点在多边形的上方或者下方，则肯定在多边形外，且无交点：
    if point[1]<boundary_edge.y_min or point[1]>boundary_edge.y_max:
        return False,[],None

    for boundary_edge_element in boundary_edge.edge_list:
        #如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
        if point[1]>boundary_edge_element.y_max or point[1]<boundary_edge_element.y_min:
            continue
        else:
            #计算线段与水平直线的交点
            y0=point[1]
            x1,y1=boundary_edge_element.point1[0],boundary_edge_element.point1[1]
            x2,y2=boundary_edge_element.point2[0],boundary_edge_element.point2[1]
            y_difference=float(y2-y1)
            #如果线段不是水平线
            if y_difference !=0:
                x_cross=(y0*(x2-x1)-(x2*y1-x1*y2))/y_difference
                #如果交点在右侧（射线为往右的方向）
                if x_cross>cross_point_target[0]:
                    cross_point_target[0]=x_cross
    # print cross_point_target
    if cross_point_target[0]==point[0]:
        return False,[],None
    else:
        for i in range(len(test_edge_list)):

            if cross_point_target[1] > test_edge_list[i].y_max or cross_point_target[1] < test_edge_list[i].y_min:
                continue
            else:
                # 计算线段与水平直线的交点
                y0 = point[1]
                x1, y1 = test_edge_list[i].point1[0], test_edge_list[i].point1[1]
                x2, y2 = test_edge_list[i].point2[0], test_edge_list[i].point2[1]
                y_difference = float(y2 - y1)
                # 如果线段不是水平线
                if y_difference != 0:
                    x_cross = (y0 * (x2 - x1) - (x2 * y1 - x1 * y2)) / y_difference
                    # 如果交点在右侧的缓冲距离之内（射线为往右的方向）
                    if x_cross >= cross_point_target[0]-0.2 and x_cross<cross_point_target[0]+projection_length:

                        close_obj_id_list.append(test_edge_list[i].id)
                        if x_cross<closest_x:
                            closest_x=x_cross
                            closest_id=test_edge_list[i].id
    return cross_point_target,close_obj_id_list,closest_id


def project_west(point,boundary_edge,test_edge_list,projection_length):
    '''
    向西发出射线，并返回相近的边的列表
    :param point:测试点
    :param boundary_edge：界址边
    :param test_edge_list:多边形的对象的列表
    :param projection_length: 缓冲距离
    :return:cross_point_target：界址边上发出射线的点
            close_obj_id_list：相近的毗邻地物
            closest_id: 最近的毗邻地物
    '''

    cross_point_target=[point[0],point[1]]
    close_obj_id_list=[]
    closest_id=None
    closest_x=0
    #如果点在多边形的上方或者下方，则肯定在多边形外，且无交点：
    if point[1]<boundary_edge.y_min or point[1]>boundary_edge.y_max:
        return False,[],None

    for boundary_edge_element in boundary_edge.edge_list:
        #如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
        if point[1]>boundary_edge_element.y_max or point[1]<boundary_edge_element.y_min:
            continue
        else:
            #计算线段与水平直线的交点
            y0=point[1]
            x1,y1=boundary_edge_element.point1[0],boundary_edge_element.point1[1]
            x2,y2=boundary_edge_element.point2[0],boundary_edge_element.point2[1]
            y_difference=float(y2-y1)
            #如果线段不是水平线
            if y_difference !=0:
                x_cross=(y0*(x2-x1)-(x2*y1-x1*y2))/y_difference
                #如果交点在右侧（射线为往左的方向）
                if x_cross<cross_point_target[0]:
                    cross_point_target[0]=x_cross
    # print cross_point_target
    if cross_point_target[0]==point[0]:
        return False,[],None
    else:
        for i in range(len(test_edge_list)):
            if cross_point_target[1] > test_edge_list[i].y_max or cross_point_target[1] < test_edge_list[i].y_min:
                continue
            else:
                # 计算线段与水平直线的交点
                y0 = point[1]
                x1, y1 = test_edge_list[i].point1[0], test_edge_list[i].point1[1]
                x2, y2 = test_edge_list[i].point2[0], test_edge_list[i].point2[1]
                y_difference = float(y2 - y1)
                # 如果线段不是水平线
                if y_difference != 0:
                    x_cross = (y0 * (x2 - x1) - (x2 * y1 - x1 * y2)) / y_difference
                    # 如果交点在左侧的缓冲距离之内（射线为往左的方向）
                    if x_cross <= cross_point_target[0]+0.2 and x_cross>cross_point_target[0]-projection_length:
                        close_obj_id_list.append(test_edge_list[i].id)
                        if x_cross>closest_x:
                            closest_x=x_cross
                            closest_id=test_edge_list[i].id
        return cross_point_target,close_obj_id_list,closest_id

def project_north(point,boundary_edge,test_edge_list,projection_length):
    '''
    向北发出射线，并返回相近的边的列表
    :param point:测试点
    :param boundary_edge：界址边
    :param test_edge_list:多边形的对象的列表
    :param projection_length: 缓冲距离
    :return:cross_point_target：界址边上发出射线的点
                close_obj_id_list：相近的毗邻地物
                closest_id: 最近的地物
    '''

    cross_point_target=[point[0],point[1]]
    close_obj_id_list=[]
    closest_id=None
    closest_y=float("inf")
    #如果点在多边形的左方或者右方，则肯定在多边形外，且无交点：
    if point[0]<boundary_edge.x_min or point[0]>boundary_edge.x_max:
        return False,[],None

    for boundary_edge_element in boundary_edge.edge_list:
        #如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
        if point[0]>boundary_edge_element.x_max or point[0]<boundary_edge_element.x_min:
            continue
        else:
            # 计算线段与垂直直线的交点
            x0 = point[0]
            x1, y1 = boundary_edge_element.point1[0], boundary_edge_element.point1[1]
            x2, y2 = boundary_edge_element.point2[0], boundary_edge_element.point2[1]
            x_difference = float(x2 - x1)
            # 如果线段不是水平线
            if x_difference != 0:
                y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference
                # 如果交点在上方（射线为往北的方向）
                if y_cross > cross_point_target[1]:
                    cross_point_target[1] = y_cross
    # print cross_point_target
    if cross_point_target[1]==point[1]:
        return False,[],None
    else:
        for i in range(len(test_edge_list)):

            if cross_point_target[0] > test_edge_list[i].x_max or cross_point_target[0] < test_edge_list[i].x_min:
                continue
            else:
                # 计算线段与垂直直线的交点
                x0 = point[0]
                x1, y1 = test_edge_list[i].point1[0], test_edge_list[i].point1[1]
                x2, y2 = test_edge_list[i].point2[0], test_edge_list[i].point2[1]
                x_difference = float(x2 - x1)
                # 如果线段不是水平线
                if x_difference != 0:
                    y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference
                    # 如果交点在上方的缓冲距离之内（射线为往北的方向）
                    if y_cross >= cross_point_target[1] - 0.2 and y_cross < cross_point_target[1] + projection_length:
                        # cross_point_target[1] = y_cross
                        close_obj_id_list.append(test_edge_list[i].id)
                        if y_cross<closest_y:
                            closest_y=y_cross
                            closest_id=test_edge_list[i].id
    return cross_point_target,close_obj_id_list,closest_id


def project_south(point,boundary_edge,test_edge_list,projection_length):
    '''
    向南发出射线，并返回相近的边的列表
    :param point:测试点
    :param boundary_edge：界址边
    :param test_edge_list:多边形的对象的列表
    :param projection_length: 缓冲距离
    :return:cross_point_target：界址边上发出射线的点
                close_obj_id_list：相近的毗邻地物
                closest_id: 最近的地物
    '''
    cross_point_target=[point[0],point[1]]
    close_obj_id_list=[]
    closest_id=None
    closest_y=0
    #如果点在多边形的左方或者右方，则肯定在多边形外，且无交点：
    if point[0]<boundary_edge.x_min or point[0]>boundary_edge.x_max:
        return False,[],None

    for boundary_edge_element in boundary_edge.edge_list:
        #如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
        if point[0]>boundary_edge_element.x_max or point[0]<boundary_edge_element.x_min:
            continue
        else:
            # 计算线段与垂直直线的交点
            x0 = point[0]
            x1, y1 = boundary_edge_element.point1[0], boundary_edge_element.point1[1]
            x2, y2 = boundary_edge_element.point2[0], boundary_edge_element.point2[1]
            x_difference = float(x2 - x1)
            # 如果线段不是水平线
            if x_difference != 0:
                y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference
                # 如果交点在下方（射线为往南的方向）
                if y_cross < cross_point_target[1]:
                    cross_point_target[1] = y_cross
    # print cross_point_target
    if cross_point_target[1]==point[1]:
        return False,[],None
    else:
        for i in range(len(test_edge_list)):

            if cross_point_target[0] > test_edge_list[i].x_max or cross_point_target[0] < test_edge_list[i].x_min:
                continue
            else:
                # 计算线段与垂直直线的交点
                x0 = point[0]
                x1, y1 = test_edge_list[i].point1[0], test_edge_list[i].point1[1]
                x2, y2 = test_edge_list[i].point2[0], test_edge_list[i].point2[1]
                x_difference = float(x2 - x1)
                # 如果线段不是水平线
                if x_difference != 0:
                    y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference
                    # 如果交点在下方的缓冲距离之内（射线为往南的方向）
                    if y_cross <= cross_point_target[1] + 0.2 and y_cross > cross_point_target[1] - projection_length:
                        # cross_point_target[1] = y_cross
                        close_obj_id_list.append(test_edge_list[i].id)
                        if y_cross>closest_y:
                            closest_y=y_cross
                            closest_id=test_edge_list[i].id
    return cross_point_target,close_obj_id_list,closest_id

def sort_dic(input_dic):
    '''
    将字典按值从大到小进行排序，并改写成列表
    :param input_dic:
    :return:
    '''
    if input_dic != None:
        # 如果值过小，则不会参与运算
        dic_list = [x for x in input_dic.items() if x[1] > 30]
        sorted_list = sorted(dic_list, key=lambda x: x[1], reverse=True)
        return sorted_list
    else:
        return []


def list_to_string(input_list):
    '''
    将列表转换成字符串，中间以"|"连接
    :param input_list: 输入的列表,格式为： [('4501051052060101356', 20),('4501051052060101356', 20)]
    :return: return_string: 字符串，格式为4501051052060101356|4501051052060101356
    '''
    return_string = ''
    for item in input_list:
        return_string = return_string + item[0] + '|'
    return return_string.strip('|')


def fullfil_blanks(orientation_info_list, target_poly):
    '''
    根据四至的结果，更新界址线毗邻地物权利人，保证
    :param orientation_info_list: 四至的列表
    :param target_poly: 目标地块对象
    :return:
    '''
    # 查找手填的四至的内容
    missing_field_list = []
    for i in range(len(orientation_info_list)):
        for item in orientation_info_list[i]:
            if item[0] == 'N':
                missing_field_list.append((item, i))

    # 查找空缺界址线毗邻权利人的界址线
    missing_boundary_list = []
    boundary_orientation_list = []  # 创建一个空的界址线对应的四至列表
    for boundary in target_poly.boundary_edge_out:
        # 如果某一边的相临边界址线长度为0，则
        if len(boundary.closest_result) == 0:
            missing_boundary_list.append([boundary, check_orientation_for_boundary(boundary)])
        else:
            boundary_orientation_list.append([boundary, check_orientation_for_boundary(boundary)])
    # print missing_boundary_list
    # 以手动添加的四至为基础，优先查找
    for missing_field in missing_field_list:
        fill_for_missing_field = 0  # 用以标记是否该地块是否已经填到毗邻地物联系人里

        # 如果存在空的界址线，则优先在空的界址线里查找
        if len(missing_boundary_list) > 0:
            for item in missing_boundary_list:
                if missing_field[1] == item[1]:  # 如果至向相同,则把最毗邻地物联系人填上去
                    item[0].closest_first_result = missing_field[0]
                    item[0].closest_result = [(missing_field[0], 31)]  # 注意这里31，是为了匹配closest_result。
                    item[1] = 5  # 标记为已经被填过了
                    fill_for_missing_field = 1
                    break

        # 如果没有在空的界址线里查找到，则在一般的界址线里查找对应的四至
        if fill_for_missing_field == 0:
            for item in boundary_orientation_list:
                if missing_field[1] == item[1]:  # 如果至向相同,则把它加到最毗邻地物联系人列表上填上去
                    item[0].closest_result.append((missing_field[0], 31))  # 注意这里31，是为了匹配closest_result。
                    fill_for_missing_field = 1
                    break

        #如果在一般的界址线里也没查找到对应的四至，则填在第一个界址线上
        if fill_for_missing_field==0:
            # 注意这里31，是为了匹配closest_result。
            try:
                boundary_orientation_list[0][0].closest_result.append((missing_field[0],31))
            except Exception as e:
                missing_boundary_list[0][0].closest_result.append((missing_field[0], 31))

    # 检查是否还有界址线缺少对应的权利人
    for missing_boundary in missing_boundary_list:
        orientation_index = missing_boundary[1]
        if orientation_index != 5:
        #     missing_boundary[0].closest_first_result = orientation_info_list[orientation_index][0]
        #     # 注意这里31，是为了匹配closest_result。
        #     missing_boundary[0].closest_result = [(orientation_info_list[orientation_index][0], 4)]
            missing_boundary[0].closest_first_result = '1'
            missing_boundary[0].closest_result = [('1', 31)]
    # 构造返回的字符串
    return_string = ''

    for boundary_edge in target_poly.boundary_edge_out:
        return_string += str([boundary_edge.name, boundary_edge.boundary_simple_orientation, \
                              str(boundary_edge.length), boundary_edge.position, \
                              list_to_string(boundary_edge.closest_result), boundary_edge.closest_first_result]) + '%'
    # print return_string
    return return_string


def check_orientation_for_boundary(boundary):
    '''
    从界址线的信息中判断界址线的方向
    :param boundary:界址线的对象
    :return:方向信息
    '''
    # 界址线走向为东，则返回北至
    if boundary.boundary_orientation == 'A' or boundary.boundary_orientation == 'I' or boundary.boundary_orientation == 'K':
        return 3
    # 界址线走向为西，则返回南至
    if boundary.boundary_orientation == 'C' or boundary.boundary_orientation == 'M' or boundary.boundary_orientation == 'O':
        return 1
    # 界址线走向为南，则返回东至
    if boundary.boundary_orientation == 'B' or boundary.boundary_orientation == 'J' or boundary.boundary_orientation == 'N':
        return 0
    # 界址线走向为北，则返回西至
    if boundary.boundary_orientation == 'D' or boundary.boundary_orientation == 'L' or boundary.boundary_orientation == 'P':
        return 2


def get_boundary_for_fields(field_orientation_string, field_info_string, distance, priori=1):
    '''
    从字符串中提取信息，计算地块界址边的方向，长度，毗邻权利人等信息，并返回结果字符串

    :param field_orientation_string:
                    地块四至字符串，格式为：
                    地块1编码:东至地块编码,西至地块编码,南至地块编码,北至地块编码;地块2编码:...

                    某个至向如果有多个地块，各地块编码之间用"|"连接。
                    手动添加的地块，在地块编码（或名称）前面加N，如果某个至向为空，则传的值为0

    :param field_info_string:

                    地块信息字符串，包含有，点坐标，以及界址点坐标的信息。格式如下：

                    地块1编码：x1,y1,J1#x2,J1#y2,J2#x3,J2#y3...xn,yn;|x1,y1,...xn,yn;地块2编码：x1,y1,x2,y2...xn,yn;
                    （注意：如果地块是空心地块，空心地块内部的界址线目前并未做任何的处理）
                    地块坐标应以顺时针为排序。

    :param distance:
                    查找毗邻权利人的缓冲距离，接受int或者float型数值。单位为米。

    :param proiri: 优先级，取值为0,1,2。目前该选项无效，只能以距离优先进行查找
                    0 为全部查找
                    1 为距离优先
                    2 为地块优先

    :return field_boundary_edge_info：
                    地块界址线字典，格式如下：
                    '地块1_ID':['J1+J2','A','183.2','M','51011510|X82d72a4b-','51011510']%['J2+J6',...]%
                    ......;'地块2_ID':...

                    其中地块_ID后面所跟的信息说明如下：
                    J1+J2：界址边的名称，J1和J2为别为该边的第一，第二个界址点。以顺时针为排序。
                    A:代表界址边的方位。A,B,C,D：分别为东南西北，E,F,G,H分别为为东南，东北，西南，西北
                    183.2: 为界址线的长度，以米为单位，格式为字符串。
                    I：为界址线位置，I为内，M为中，英文字母O为外
                    '51011510|X82d72a4b-'：为缓冲出来的毗邻权利人，包含有地块和面线状物的ID
                    '51011510': 为距离优先缓冲出来的毗邻权利人，用来判断界址线类别
    '''

    # 获取地块四至信息
    orientation_dic = orientation_data_to_dic(field_orientation_string)

    return_string = ''  # 存储最终结果的字符串
    # 初始化对象
    targetID_list, testID_list, coordinates_dic, boundary_dic = get_formated_data(field_info_string, distance)
    poly_dic = {}
    for k, v in coordinates_dic.items():
        poly_dic[k] = Poly(k, v, boundary_dic[k])

    for j in range(len(targetID_list)):
        # if j % 10 == 0:
        #     print j, ": ", time.strftime('%H:%M:%S', time.localtime(time.time()))
        if True:
        # if targetID_list[j]=='00412':
            target_poly = poly_dic[targetID_list[j]]
            target_poly_x_max_with_distance = target_poly.x_max + distance
            target_poly_x_min_with_distance = target_poly.x_min - distance
            target_poly_y_max_with_distance = target_poly.y_max + distance
            target_poly_y_min_with_distance = target_poly.y_min - distance
            test_poly_objs = []
            edge_poly_list = []
            for item in testID_list[j]:
                test_poly_objs.append(poly_dic[item])
                for test_poly in test_poly_objs:
                    for i in range(test_poly.edgeNum):
                        if target_poly_y_min_with_distance > test_poly.edge[i].y_max or \
                                        target_poly_y_max_with_distance < test_poly.edge[i].y_min or \
                                        target_poly_x_min_with_distance > test_poly.edge[i].x_max or \
                                        target_poly_x_max_with_distance < test_poly.edge[i].x_min:
                            pass
                        else:
                            edge_poly_list.append(test_poly.edge[i])

            for boundary_edge in target_poly.boundary_edge_out:
                close_field_dic, closest_dic = get_boundary_close_fields(boundary_edge, edge_poly_list, test_poly_objs,
                                                                         distance, 'out', priori)
                # close_field_list=sort_dic(close_field_dic) #计算所有的毗邻地物
                boundary_edge.closest_result = sort_dic(closest_dic)  # 计算最近的毗邻地物（距离优先模式）

                if len(boundary_edge.closest_result) > 0:
                    boundary_edge.closest_first_result = boundary_edge.closest_result[0][0]
                    # 如果最毗邻地物不是线，面状物，则位置为中，否则为内
                    if boundary_edge.closest_result[0][0][0] != 'X' and boundary_edge.closest_result[0][0][0] != 'M':
                        boundary_edge.position = 'M'
                    else:
                        boundary_edge.position = 'I'
                else:
                    # 如果没找到毗邻地物，为中
                    boundary_edge.position = 'O'

            boundary_result_string = fullfil_blanks(orientation_dic[targetID_list[j]], target_poly)
            field_result_string = "'" + targetID_list[j] + "'" + ":" + boundary_result_string.strip('%') + ';'
            return_string += field_result_string
    return return_string.decode("utf-8")

#

# with open('D:/jiezhixian.txt') as fr:
#     field_info_string=fr.read()
# with open('D:/sizhi.txt') as fr:
#     orentation_string = fr.read().decode('utf-8')
# output_string=get_boundary_for_fields(orentation_string,field_info_string,3)
# with open('D:/result.txt','w') as fr:
#     fr.writelines(output_string)
