#encoding: utf-8
'''
从string中载入数据。txt文件的内容为：地块号，坐标x，坐标y...；第二块坐标地块号，坐标x,坐标y...
'''


def get_cross_result(edge_sin_list,close_ids_lists):
    '''
    计算出某个至向的结果
    :param edge_sin_list:
    :param close_obj_id_list:
    :return:
    '''
    cross_result={}
    start_index,end_index=get_none_zero_value_index(edge_sin_list)
    left_bar_index=int(3*(end_index-start_index)/10)
    right_bar_index=int(7*(end_index-start_index)/10)
    for i in xrange(start_index,left_bar_index):
        weight_value=0.35*edge_sin_list[i]
        for j in range(len(close_ids_lists[i])):
            try:
                if cross_result.get(close_ids_lists[i][j]):
                    cross_result[close_ids_lists[i][j]] += weight_value
                else:
                    cross_result[close_ids_lists[i][j]] = weight_value
            except:
                pass
    for i in xrange(left_bar_index,right_bar_index):
        weight_value=edge_sin_list[i]
        for j in range(len(close_ids_lists[i])):
            try:
                if cross_result.get(close_ids_lists[i][j]):
                    cross_result[close_ids_lists[i][j]] += weight_value
                else:
                    cross_result[close_ids_lists[i][j]] = weight_value
            except:
                pass

    for i in xrange(right_bar_index,end_index):
        weight_value=0.35*edge_sin_list[i]
        for j in range(len(close_ids_lists[i])):
            try:
                if cross_result.get(close_ids_lists[i][j]):
                    cross_result[close_ids_lists[i][j]] += weight_value
                else:
                    cross_result[close_ids_lists[i][j]] = weight_value
            except:
                pass
    return cross_result


def get_none_zero_value_index(input_list):
    '''
    从一个list中得到第一个非0和最后一个非0元素的index
    :param input_list: 输入的list
    :return: start_index: 第一个非零元素的index
             end_index: 最后一个非零元素的index
    '''
    start_index=0
    end_index=0
    for i in range(len(input_list)):
        if input_list[i]>0:
            start_index=i
            break
    for j in xrange(len(input_list) - 1, 0, -1):
        if input_list[j] > 0:
            end_index=j
            break
    return start_index,end_index

def load_txt_data(input_string):
    '''
    读取数据
    :param input_string: 输入的字符串
    :return: targetID list：目标地块的列表，元组为目标地块ID
             testID list: 测试地块的列表，元组为targetID对应元素的测试地块ID列表
             coordinateDic：地块坐标的字典，key为地块ID，value为地块的各顶点坐标
    '''
    input_string=input_string.replace(';|','&')
    raw_data_list=(input_string.strip(';')).split(';')
    output_Dic={}
    for item in raw_data_list:
        id, coordinatesList=getCoordiantes(item)
        output_Dic[id]=coordinatesList
    return output_Dic

def getCoordiantes(fieldInfoList):
    '''
    从土地信息列表里得到fieldID信息，以及坐标信息
    :param fieldInfoList:
    :return: id：地块的id信息
              coordinatesList: 坐标列表
    '''
    return_list = []
    results_=fieldInfoList.split(':')    #信息的格式为：ID:x,y,x,y...x,y&x,y,x,y...x,y
    id=results_[0].strip()
    item_list=results_[1].split('&')
    for coordinates_string in item_list:
        coordinatesList = []
        item=coordinates_string.split(',')
        # print id
        for i in xrange(0,len(item),2):
            x=float(item[i])
            y=float(item[i+1])
            coordinatesList.append([x,y])
        return_list.append(coordinatesList)
    return id,return_list



def get_east_area(targe_poly_obj,edge_poly_east_west_list,test_poly_objs,prior_option,projection_length=3):
    '''
    实现紧邻投影法判断东方的投影相交线段的投影长度。
    :param targe_poly_obj: 目标多边形的对象
    :param test_poly_objs:  测试多边形的对象列表
    :param prior_option: 查找优先级。
                    0：为默认值，返回一个地块，一个面状物。且地块在面状物之前
                    1: 地块优先，优先返回地块，如果没有地块，则返回面状物
                    2: 距离优先。优先返回最近的地块或面状物
    :param projection_length: 投影线的投影长度，和缓冲距离类似
    :return:东方的相交线投影长度
    '''

    # x_center=(targe_poly_obj.x_max+targe_poly_obj.x_min)/2
    # y_center=(targe_poly_obj.y_max+targe_poly_obj.y_min)/2
    # point_center=[x_center,y_center]
    east_sum=0
    x_start=targe_poly_obj.x_min-1
    cross_result={}
    step=(targe_poly_obj.y_max-targe_poly_obj.y_min)/100
    y_start = targe_poly_obj.y_max-step
    edge_sin_list=[]
    close_ids_lists=[]
    while y_start > targe_poly_obj.y_min:

        # 首先计算出投影矩形的起始点
        projection_point, edge_index, close_obj_id_list = project_east([x_start, y_start], targe_poly_obj, \
                                                                       edge_poly_east_west_list, prior_option,
                                                                       projection_length + 0.2)
        close_obj_id_list = list(set(close_obj_id_list))
        if projection_point:
            try:
                edge_sin_list.append(targe_poly_obj.edge[edge_index].sin)
                close_ids_lists.append(close_obj_id_list)
            except:
                edge_sin_list.append(0)
                close_ids_lists.append([])
        y_start -= step
    cross_result = get_cross_result(edge_sin_list, close_ids_lists)
    return cross_result,east_sum

def get_west_area(targe_poly_obj,edge_poly_east_west_list,test_poly_objs,prior_option,projection_length=3):
    '''
    实现紧邻投影法判断东方的投影相交线段的投影长度。
    :param targe_poly_obj: 目标多边形的对象
    :param test_poly_objs:  测试多边形的对象列表
    :param prior_option: 查找优先级。
                0：为默认值，返回一个地块，一个面状物。且地块在面状物之前
                1: 地块优先，优先返回地块，如果没有地块，则返回面状物
                2: 距离优先。优先返回最近的地块或面状物
    :param projection_length: 投影线的投影长度，和缓冲距离类似
    :return:东方的相交线投影长度
    '''


    west_sum=0
    x_start=targe_poly_obj.x_max+1

    step = (targe_poly_obj.y_max - targe_poly_obj.y_min) / 100
    y_start = targe_poly_obj.y_max-step
    edge_sin_list=[]    #储存变现的sin值，用于判断西至线段的起始和结束位置
    close_ids_lists=[]    #储存close_id_list的list



    while y_start>targe_poly_obj.y_min:

        #首先计算出投影矩形的起始点
        projection_point, edge_index, close_obj_id_list=project_west([x_start,y_start],targe_poly_obj,\
                                                    edge_poly_east_west_list,prior_option,projection_length+0.2)
        close_obj_id_list=list(set(close_obj_id_list))
        if projection_point:
            try:
                edge_sin_list.append(targe_poly_obj.edge[edge_index].sin)
                close_ids_lists.append(close_obj_id_list)
            except:
                edge_sin_list.append(0)
                close_ids_lists.append([])
        y_start-=step
    cross_result=get_cross_result(edge_sin_list,close_ids_lists)
    return cross_result,west_sum


def get_north_area(targe_poly_obj,edge_poly_north_south_list,test_poly_objs,prior_option,projection_length=3):
    '''
    实现紧邻投影法判断东方的投影相交线段的投影长度。
    :param targe_poly_obj: 目标多边形的对象
    :param test_poly_objs:  测试多边形的对象列表
    :param prior_option: 查找优先级。
                0：为默认值，返回一个地块，一个面状物。且地块在面状物之前
                1: 地块优先，优先返回地块，如果没有地块，则返回面状物
                2: 距离优先。优先返回最近的地块或面状物
    :param projection_length: 投影线的投影长度，和缓冲距离类似
    :return:东方的相交线投影长度
    '''

    north_sum=0
    y_start=targe_poly_obj.y_min-1
    step=(targe_poly_obj.x_max-targe_poly_obj.x_min)/100
    x_start = targe_poly_obj.x_max-step

    edge_cos_list=[]
    close_ids_lists=[]
    while x_start>targe_poly_obj.x_min:
        # 首先计算出投影矩形的起始点
        projection_point, edge_index, close_obj_id_list=project_north([x_start,y_start],targe_poly_obj,\
                                                    edge_poly_north_south_list,prior_option,projection_length+0.2)
        close_obj_id_list=list(set(close_obj_id_list))
        if projection_point:
            try:
                edge_cos_list.append(targe_poly_obj.edge[edge_index].cos)
                close_ids_lists.append(close_obj_id_list)
            except:
                edge_cos_list.append(0)
                close_ids_lists.append([])
        x_start -= step

    cross_result=get_cross_result(edge_cos_list,close_ids_lists)

    return cross_result,north_sum


def get_south_area(targe_poly_obj,edge_poly_north_south_list,test_poly_objs,prior_option,projection_length=3):
    '''
    实现紧邻投影法判断南方的投影相交线段的投影长度。
    :param targe_poly_obj: 目标多边形的对象
    :param test_poly_objs:  测试多边形的对象列表
    :param prior_option: 查找优先级。
            0：为默认值，返回一个地块，一个面状物。且地块在面状物之前
            1: 地块优先，优先返回地块，如果没有地块，则返回面状物
            2: 距离优先。优先返回最近的地块或面状物
    :param projection_length: 投影线的投影长度，和缓冲距离类似
    :return:东方的相交线投影长度
    '''

    south_sum=0
    y_start=targe_poly_obj.y_max+1
    step=(targe_poly_obj.x_max-targe_poly_obj.x_min)/100
    x_start = targe_poly_obj.x_max-step
    edge_cos_list = []
    close_ids_lists = []
    while x_start > targe_poly_obj.x_min:
        # 首先计算出投影矩形的起始点
        projection_point, edge_index, close_obj_id_list = project_south([x_start, y_start], targe_poly_obj, \
                                                edge_poly_north_south_list, prior_option,projection_length + 0.2)
        close_obj_id_list = list(set(close_obj_id_list))
        if projection_point:
            try:
                edge_cos_list.append(targe_poly_obj.edge[edge_index].cos)
                close_ids_lists.append(close_obj_id_list)
            except:
                edge_cos_list.append(0)
                close_ids_lists.append([])
        x_start -= step

    cross_result = get_cross_result(edge_cos_list, close_ids_lists)
    return cross_result,south_sum

def get_cos(point1,point2):
    '''
    返回相对水平线夹角的cos值
    :param point1:
    :param point2:
    :return:水平线夹角的cos值
    '''
    x_length=max((point1[0]-point2[0]),(point2[0]-point1[0]))
    y_length=max((point1[1]-point2[1]),(point2[1]-point1[1]))
    cos=x_length/((x_length**2+y_length**2)**0.5)
    return cos

def get_sin(point1,point2):
    '''
    返回相对水平线夹角的sin值
    :param point1:
    :param point2:
    :return:水平线夹角的sin值
    '''
    x_length=max((point1[0]-point2[0]),(point2[0]-point1[0]))
    y_length=max((point1[1]-point2[1]),(point2[1]-point1[1]))
    sin= y_length/((x_length**2+y_length**2)**0.5)
    return sin



def get_outer_rectangle_dic(coordinate_dic):
    '''
    从字典中计算出各地块物体的外接矩形顶点坐标字典。
    key为地块id，value为[[x_min,y_min],[x_max,y_max]]
    :param coordinate_dic:
    :return:
    '''
    output_dic={}
    for k,v in coordinate_dic.items():
        x_min=y_min=float("inf"); x_max=y_max=0
        for coordinate_dic_list in v:
            for coordinate_pair in coordinate_dic_list:
                if coordinate_pair[0]<x_min: x_min=coordinate_pair[0]
                if coordinate_pair[0]>x_max: x_max=coordinate_pair[0]
                if coordinate_pair[1]<y_min: y_min=coordinate_pair[1]
                if coordinate_pair[1]>y_max: y_max=coordinate_pair[1]
            output_dic[k]=[[x_min,y_min],[x_max,y_max]]
    return output_dic

def get_close_field(inputID,rectangle_dic,distance):
    '''
    快速而粗略的判断相邻的地块编码，判断原理如下：
    如果测试地块的x/y_max<目标地块的x/y_min-distance: 在外面
    如果测试地块的x/y_min>目标地块的x/y_max+distance: 在外面
    以上两个(四个）条件均为否，则在里面
    :param inputID:
    :param rectangle_dic:
    :param distance:
    :return: 相邻地块编码列表
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


'''
多边形的类
'''

class Ploy(object):
    '''
    多边形的类
    '''

    def __init__(self,ID,point_array_list):
        '''
        类的构造函数
        edge的格式为[ymax,ymin,delta-x,point1(y较大),point2(y较小)]
        注意这里：x_new=x_old减去delta-x
        :param pointList:库多边形得各顶点的坐标列表
        :return:
        '''
        self.edge=[]
        self.y_max=0
        self.y_min=float("inf")
        self.x_max=0
        self.x_min=float("inf")
        self.id=ID

        if ID[0]=='X' or ID[0]=='M':
            self.type='others'
        else:
            self.type='field'

        if ID[0]!='X':
            #如果不是线状物，则将第一个点添加到点列表中。最后一个点会和第一个点组成一个线段
            for pointList in point_array_list:
                pointList.append(pointList[0])

        for pointList in point_array_list:

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
                    self.edge.append(Edge(y_max, y_min, delta_x, point1, point2,ID,self.type))
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
                    self.edge.append(Edge(y_max,y_min,delta_x,point1,point2,ID,self.type))
                    if y_max>self.y_max: self.y_max=y_max
                    if y_min<self.y_min: self.y_min=y_min
                    if max([pointList[n+1][0],pointList[n][0]])>self.x_max:self.x_max=max([pointList[n+1][0],pointList[n][0]])
                    if min([pointList[n+1][0],pointList[n][0]])<self.x_min:self.x_min=min([pointList[n+1][0],pointList[n][0]])
        self.center_point=((self.x_max+self.x_min)/2,(self.y_min+self.y_max)/2)
        self.edgeNum=len(self.edge)
        self.pointList=point_array_list

class Edge(object):
    '''
    多边形的边的类
    '''
    def __init__(self,y_max,y_min,delta_x,point1,point2,id,type):
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
        self.type=type
        try:
            sin=(self.y_max-self.y_min)/(
                ((self.y_max-self.y_min)**2+(self.x_max-self.x_min)**2)**0.5)
            self.true_sin=sin
            if sin<0.7:
                self.sin=0
            elif sin<0.8:
                self.sin=sin/10
            else:
                self.sin=sin

            cos = (self.x_max - self.x_min) / (
                ((self.y_max - self.y_min) ** 2 + (self.x_max - self.x_min) ** 2) ** 0.5)
            self.true_cos=cos
            if cos<0.7:
                self.cos=0
            elif cos<0.8:
                self.cos=cos/10
            else:
                self.cos=cos

        except Exception, e:
            self.sin=0
            self.cos=0

def project_east(point,target_obj,edge_poly_east_west_list,prior_option,projection_length):
    '''
    向东进行相交边投影测试，返回附件地块的id
    :param point:测试点
    :param polyObj:多边形的对象
    :param prior_option: 优先级。0为默认，1为地块优先，2为距离优先
    :return:cross_point_target：东向射线是否与目标多边形的最东方的交点，如果没有，返回False
            edge_index: 如果有交点，返回目标多边形东向相交边最右的边的index
            close_obj_id_list: 附近地块的test_obj的id列表
    '''

    cross_point_target=[point[0],point[1]]
    close_obj_id_list=[]
    #如果点在多边形的上方或者下方，则肯定在多边形外，且无交点：
    if point[1]<target_obj.y_min or point[1]>target_obj.y_max:
        return False,None,[]

    for i in range(target_obj.edgeNum):
        #如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
        if point[1]>target_obj.edge[i].y_max or point[1]<target_obj.edge[i].y_min:
            continue
        else:
            #计算线段与水平直线的交点
            y0=point[1]
            x1,y1=target_obj.edge[i].point1[0],target_obj.edge[i].point1[1]
            x2,y2=target_obj.edge[i].point2[0],target_obj.edge[i].point2[1]
            y_difference=float(y2-y1)
            #如果线段不是水平线
            if y_difference !=0:
                x_cross=(y0*(x2-x1)-(x2*y1-x1*y2))/y_difference
                #如果交点在左侧（射线为往右的方向）
                if x_cross>cross_point_target[0]:
                    cross_point_target[0]=x_cross
                    edge_index=i
    # print cross_point_target

    if cross_point_target[0]==point[0]:
        return False,None,[]
    else:
        if target_obj.edge[edge_index].true_sin > 0.5:
            projection_length = projection_length / target_obj.edge[edge_index].true_sin
        best_x_cross=float("inf")
        best_id=None
        for i in range(len(edge_poly_east_west_list)):
            if cross_point_target[1] > edge_poly_east_west_list[i].y_max or cross_point_target[1] < edge_poly_east_west_list[i].y_min:
                continue
            else:
                # 计算线段与水平直线的交点
                y0 = point[1]
                x1, y1 = edge_poly_east_west_list[i].point1[0], edge_poly_east_west_list[i].point1[1]
                x2, y2 = edge_poly_east_west_list[i].point2[0], edge_poly_east_west_list[i].point2[1]
                y_difference = float(y2 - y1)
                # 如果线段不是水平线
                if y_difference != 0:
                    x_cross = (y0 * (x2 - x1) - (x2 * y1 - x1 * y2)) / y_difference
                    # 如果交点在左侧的缓冲距离之内（射线为往右的方向）
                    if x_cross >= cross_point_target[0]-0.2 and x_cross<cross_point_target[0]+projection_length:
                        # cross_point_target[0] = x_cross
                        if prior_option!=2:
                            #如果不是距离优先
                            close_obj_id_list.append(edge_poly_east_west_list[i].id)
                        else:
                            #如果是距离优先
                            if x_cross<best_x_cross:
                                best_x_cross=x_cross
                                best_id=edge_poly_east_west_list[i].id

    if prior_option != 2:
        return cross_point_target,edge_index,close_obj_id_list
    else:
        if best_id!=None:
            return cross_point_target, edge_index, [best_id]
        else:
            return cross_point_target, edge_index, ["emputy"]


def project_west(point,target_obj,edge_poly_east_west_list,prior_option,projection_length):
    '''
    向西进行相交边投影测试，返回附件地块的id
    :param point:测试点
    :param polyObj:多边形的对象
    :param prior_option: 优先级。0为默认，1为地块优先，2为距离优先
    :return:cross_point_target：东向射线是否与目标多边形的最西方的交点，如果没有，返回False
            edge_index: 如果有交点，返回目标多边形西向相交边最右的边的index
            close_obj_id_list: 附近地块的test_obj的id列表
    '''
    cross_point_target=[point[0],point[1]]
    close_obj_id_list=[]
    #如果点在多边形的上方或者下方，则肯定在多边形外，且无交点：
    if point[1]<target_obj.y_min or point[1]>target_obj.y_max:
        return False,None,[]

    for i in range(target_obj.edgeNum):
        #如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
        if point[1]>target_obj.edge[i].y_max or point[1]<target_obj.edge[i].y_min:
            continue
        else:
            #计算线段与水平直线的交点
            y0=point[1]
            x1,y1=target_obj.edge[i].point1[0],target_obj.edge[i].point1[1]
            x2,y2=target_obj.edge[i].point2[0],target_obj.edge[i].point2[1]
            y_difference=float(y2-y1)
            #如果线段不是水平线
            if y_difference !=0:
                x_cross=(y0*(x2-x1)-(x2*y1-x1*y2))/y_difference
                #如果交点在左侧（射线为往左的方向）
                if x_cross<cross_point_target[0]:
                    cross_point_target[0]=x_cross
                    edge_index=i
    if cross_point_target[0]==point[0]:
        return False,None,[]
    else:
        if target_obj.edge[edge_index].true_sin > 0.5:
            projection_length = projection_length / target_obj.edge[edge_index].true_sin
        best_x_cross=0
        best_id=None
        for i in range(len(edge_poly_east_west_list)):

            if cross_point_target[1] > edge_poly_east_west_list[i].y_max or cross_point_target[1] < edge_poly_east_west_list[i].y_min:
                continue
            else:
                # 计算线段与水平直线的交点
                y0 = point[1]
                x1, y1 = edge_poly_east_west_list[i].point1[0], edge_poly_east_west_list[i].point1[1]
                x2, y2 = edge_poly_east_west_list[i].point2[0], edge_poly_east_west_list[i].point2[1]
                y_difference = float(y2 - y1)
                # 如果线段不是水平线
                if y_difference != 0:
                    x_cross = (y0 * (x2 - x1) - (x2 * y1 - x1 * y2)) / y_difference
                    # 如果交点在左侧的缓冲距离之内（射线为往左的方向）
                    if x_cross <= cross_point_target[0]+0.2 and x_cross>cross_point_target[0]-projection_length:
                        if prior_option!=2:
                            #如果不是距离优先
                            close_obj_id_list.append(edge_poly_east_west_list[i].id)
                        else:
                            # 如果是距离优先
                            if x_cross > best_x_cross:
                                best_x_cross = x_cross
                                best_id = edge_poly_east_west_list[i].id

    if prior_option != 2:
        return cross_point_target,edge_index,close_obj_id_list
    else:
        if best_id!=None:
            return cross_point_target, edge_index, [best_id]
        else:
            return cross_point_target, edge_index, ["emputy"]




def project_north(point,target_obj,edge_poly_north_south_list,prior_option,projection_length):
    '''
    判断点是否在多边形内，并返回向北水平射线的交点的坐标
    :param point:测试点
    :param polyObj:多边形的对象
    :param prior_option: 优先级。0为默认，1为地块优先，2为距离优先
    :return:cross_point_target：东向射线是否与目标多边形的最北方的交点，如果没有，返回False
            edge_index: 如果有交点，返回目标多边形北向相交边最右的边的index
            close_obj_id_list: 附近地块的test_obj的id列表
    '''

    cross_point_target = [point[0], point[1]]
    close_obj_id_list=[]
    if point[0]<target_obj.x_min or point[0]>target_obj.x_max:
        return False,None,[]

    for i in range(target_obj.edgeNum):
        #如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
        if point[0] < target_obj.edge[i].x_min or point[0] > target_obj.edge[i].x_max:
            continue
        else:
            #计算线段与垂直直线的交点
            x0=point[0]
            x1,y1=target_obj.edge[i].point1[0],target_obj.edge[i].point1[1]
            x2,y2=target_obj.edge[i].point2[0],target_obj.edge[i].point2[1]
            x_difference=float(x2-x1)
            #如果线段不是水平线
            if x_difference !=0:
                y_cross=(x0*(y2-y1)-(y2*x1-y1*x2))/x_difference
                #如果交点在上方（射线为往北的方向）
                if y_cross>cross_point_target[1]:
                    cross_point_target[1]=y_cross
                    edge_index=i

    if cross_point_target[1]==point[1]:
        return False,None,[]
    else:
        if target_obj.edge[edge_index].true_cos> 0.5:
            projection_length = projection_length / target_obj.edge[edge_index].true_cos
        best_y_cross=float("inf")
        best_id=None
        for i in range(len(edge_poly_north_south_list)):
            if cross_point_target[0] > edge_poly_north_south_list[i].x_max or cross_point_target[0] < edge_poly_north_south_list[i].x_min:
                continue
            else:
                # 计算线段与垂直直线的交点
                x0 = point[0]
                x1, y1 = edge_poly_north_south_list[i].point1[0], edge_poly_north_south_list[i].point1[1]
                x2, y2 = edge_poly_north_south_list[i].point2[0], edge_poly_north_south_list[i].point2[1]
                x_difference = float(x2 - x1)
                # 如果线段不是水平线
                if x_difference != 0:
                    y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference
                    # 如果交点在上方的缓冲距离之内（射线为往北的方向）
                    if y_cross >= cross_point_target[1]-0.2 and y_cross<cross_point_target[1]+projection_length:
                        if prior_option!=2:
                            #如果不是距离优先
                            close_obj_id_list.append(edge_poly_north_south_list[i].id)
                        else:
                            # 如果是距离优先
                            if y_cross < best_y_cross:
                                best_y_cross = y_cross
                                best_id = edge_poly_north_south_list[i].id

    if prior_option != 2:
        return cross_point_target,edge_index,close_obj_id_list
    else:
        if best_id!=None:
            return cross_point_target, edge_index, [best_id]
        else:
            return cross_point_target, edge_index, ["emputy"]


def project_south(point,target_obj,edge_poly_north_south_list,prior_option,projection_length):
    '''
    判断点是否在多边形内，并返回向南水平射线的交点的坐标
    :param point:测试点
    :param polyObj:多边形的对象
    :param prior_option: 优先级。0为默认，1为地块优先，2为距离优先
    :return:cross_point_target：东向射线是否与目标多边形的最北方的交点，如果没有，返回False
            edge_index: 如果有交点，返回目标多边形北向相交边最右的边的index
            close_obj_id_list: 附近地块的test_obj的id列表
    '''

    cross_point_target = [point[0], point[1]]
    close_obj_id_list=[]
    if point[0]<target_obj.x_min or point[0]>target_obj.x_max:
        return False,None,[]

    for i in range(target_obj.edgeNum):
        #如果该点的y坐标在线段的y坐标之外，肯定是没有交点的
        if point[0] < target_obj.edge[i].x_min or point[0] > target_obj.edge[i].x_max:
            continue
        else:
            #计算线段与垂直直线的交点
            x0=point[0]
            x1,y1=target_obj.edge[i].point1[0],target_obj.edge[i].point1[1]
            x2,y2=target_obj.edge[i].point2[0],target_obj.edge[i].point2[1]
            x_difference=float(x2-x1)
            #如果线段不是水平线
            if x_difference !=0:
                y_cross=(x0*(y2-y1)-(y2*x1-y1*x2))/x_difference

                #如果交点在下方（射线为往南的方向）
                if y_cross<cross_point_target[1]:
                    cross_point_target[1]=y_cross
                    edge_index=i

    if cross_point_target[1]==point[1]:
        return False,None,[]
    else:
        if target_obj.edge[edge_index].true_cos> 0.5:
            projection_length = projection_length / target_obj.edge[edge_index].true_cos
        best_y_cross=0
        best_id=None
        for i in range(len(edge_poly_north_south_list)):

            if cross_point_target[0] > edge_poly_north_south_list[i].x_max or cross_point_target[0] < edge_poly_north_south_list[i].x_min:
                continue
            else:
                # 计算线段与垂直直线的交点
                x0 = point[0]
                x1, y1 = edge_poly_north_south_list[i].point1[0], edge_poly_north_south_list[i].point1[1]
                x2, y2 = edge_poly_north_south_list[i].point2[0], edge_poly_north_south_list[i].point2[1]
                x_difference = float(x2 - x1)
                # 如果线段不是水平线
                if x_difference != 0:
                    y_cross = (x0 * (y2 - y1) - (y2 * x1 - y1 * x2)) / x_difference
                    # 如果交点在下方的缓冲距离之内（射线为往南的方向）
                    if y_cross <= cross_point_target[1]+0.2and y_cross>cross_point_target[1]-projection_length:
                        # cross_point_target[1] = y_cross
                        if prior_option!=2:
                            #如果不是距离优先
                            close_obj_id_list.append(edge_poly_north_south_list[i].id)
                        else:
                            # 如果是距离优先
                            if y_cross > best_y_cross:
                                best_y_cross = y_cross
                                best_id = edge_poly_north_south_list[i].id

    if prior_option != 2:
        return cross_point_target,edge_index,close_obj_id_list
    else:
        if best_id!=None:
            return cross_point_target, edge_index, [best_id]
        else:
            return cross_point_target, edge_index, ["emputy"]

def get_formated_data(field_info_string,distance):
    coordinateDic=load_txt_data(field_info_string)
    rectangle_dic=get_outer_rectangle_dic(coordinateDic)
    targetID_list, testID_list = get_id_list(rectangle_dic, distance)
    return targetID_list, testID_list, coordinateDic

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


def get_first_result(input_list):
    '''
    返回一个新的列表，列表中包含原列表中的第一个地块元素，以及所有的线面状地物。如果原列表为空，则返回一个空列表
    :param input_list:
    :return:
    '''
    return_list=[]
    if len(input_list)>0:
        for i in range(len(input_list)):
            if i==0:
                return_list.append(input_list[i])
            else:
                if input_list[i][0]=='X' or input_list[i][0]=='M':
                    return_list.append(input_list[i])
    return return_list

def get_result_list(result_dic,threshold):
    dic_list = result_dic.items()
    sorted_list = sorted(dic_list, key=lambda x: x[1], reverse=True)
    try:
        threshold=sorted_list[0][1]*threshold
    except Exception as e:
        threshold=0
    field_result = [x[0] for x in sorted_list if ((x[0].find('M')==-1 and (x[0].find('X')==-1)) and x[0]!='emputy'\
                                                  and x[1] >= threshold)]
    M_result=[x[0] for x in sorted_list if (x[0].find('M')!=-1 or x[0].find('X')!=-1 and x[0]!='emputy'\
                                            and x[1]>=threshold)]
    return field_result,M_result

def get_result_list_all(result_dic,threshold):
    dic_list = result_dic.items()
    sorted_list = sorted(dic_list, key=lambda x: x[1], reverse=True)
    try:
        threshold=sorted_list[0][1]*threshold
    except Exception as e:
        threshold=0
    first_result=[x[0] for x in sorted_list if x[1]>=threshold]
    return first_result

def get_field_orientation_result(field_info_string,distance,one_field_only,field_only,prior_option=2,\
                                 threshold_value=0.07):
    '''
    从字符串中提取信息，计算地块四至，并返回结果字符串
    :param field_info_string: 地块坐标信息。具体格式如下：
           地块ID01：坐标x1,坐标y1,...坐标xn,坐标yn;地块ID02:...;地块ID03：...;|地块ID11...
           其中，地块ID01为需要给出四至的地块，地块ID02和地块ID03为在缓冲距离内的候选地块，各地块之间用“;”间隔。
           地块ID11为另一个需要给出四至的地块。地块ID01字符串和地块ID11字符串之间用“|”间隔。
           间隔符均为英文字符。

           如果某个地块是空心地块，有多组坐标。则传递的格式如下。多组坐标之间用";|"分割。
           地块1：x1,y1,x2,y2...xn,yn;|x1,y1,x2,y2...xn,yn;|x1,y1,x2,y2...xn,yn;地块2：x1,y1,x2,y2...xn,yn;


           也可以只输入一个待测试地块的信息。

    :param distance:  缓冲距离，接受int或者float型数值。单位为米
    :param one_field_only: 是否只返回一个地块。Int型（只能取0或1.避免python与c#的布尔值定义可能的区别）.
                           1为只返回一个地块，0为返回一个列表结果
    :param field_only: 是否只查询地块。Int型（只能取0或1.避免python与c#的布尔值定义可能的区别）.
                        1为只返回地块结果，0为同时返回地块和面状物结果。
    :param prior_option: 查找优先级。仅当one_field_only==1和field_only==0时生效。其他条件时，需要设置为0。

                        0：为默认值，返回一个地块，一个面状物。且地块在面状物之前
                        1: 地块优先，优先返回地块，如果没有地块，则返回面状物
                        2: 距离优先。优先返回最近的地块或面状物

    :return: 地块四至结果的字符串，格式为：
             测试地块的ID：[东至结果列表][南至结果列表][西至结果列表][北至结果列表]|下一个结果
             两个结果之间由"|"分隔。
             如果某一个至向结果为空，则返回的结果是"[]"

             例：
             5101151052490200391:[] ['M87fd0a4f-1985-420b-aafe-c2b36f83b966'] [] ['5101151052490200418']|\
             5101151052490200395:['5101151052490200254', 'X82d72a4b-d363-4db3-b1a3-4e0a5ed13742'] \
             ['5101151052490200382', 'X82d72a4b-d363-4db3-b1a3-4e0a5ed13742'] ['5101151052490200382', \
             '5101151052490200306'] ['5101151052490200306', '5101151052490200137', '5101151052490200339']
    '''
    targetIDList, testIDList, coordinateDic = get_formated_data(field_info_string,distance)  # 从字符串中读取数据

    poly_dic={}
    for k,v in coordinateDic.items():
        poly_dic[k]=Ploy(k,v)

    # print 'b max is:', b.edgeNum
    resultString =''
    # print time.strftime('%H:%M:%S', time.localtime(time.time()))
    for j in range(len(targetIDList)):
        # if j%10==0:
        #     print j,": ",time.strftime('%H:%M:%S',time.localtime(time.time()))
        if True:
        # if targetIDList[j]=='00284':
            target_poly=poly_dic[targetIDList[j]]
            test_poly_objs_field=[]
            test_poly_objs_others=[]
            east_west_list_field=[]
            north_south_list_field = []
            east_west_list_others = []
            north_south_list_others = []

            #step1：构造字典

            #根据地块的属性构造一个地块字典，以及一个其他地物地块字典
            for item in testIDList[j]:
                if poly_dic[item].type=='field':
                    test_poly_objs_field.append(poly_dic[item])
                else:
                    test_poly_objs_others.append(poly_dic[item])

            #从地块字典中，构造出可能的地块的相交边
            for test_poly in test_poly_objs_field:
                for i in range(test_poly.edgeNum):
                    if target_poly.y_min <= test_poly.edge[i].y_max and \
                                    target_poly.y_max >= test_poly.edge[i].y_min:
                        east_west_list_field.append(test_poly.edge[i])
                    if target_poly.x_min <= test_poly.edge[i].x_max and \
                                    target_poly.x_max >= test_poly.edge[i].x_min:
                        north_south_list_field.append(test_poly.edge[i])
            if field_only!=1:
                #如果需要查找面状物，则从面线状物字典中，构造出可能的其他地物的相交边
                for test_poly in test_poly_objs_others:
                    for i in range(test_poly.edgeNum):
                        if target_poly.y_min <= test_poly.edge[i].y_max and \
                                        target_poly.y_max >= test_poly.edge[i].y_min:
                            east_west_list_others.append(test_poly.edge[i])
                        if target_poly.x_min <= test_poly.edge[i].x_max and \
                                        target_poly.x_max >= test_poly.edge[i].x_min:
                            north_south_list_others.append(test_poly.edge[i])

            if field_only == 1:
                #如果只查地块，则只进行地块的查找
                east_field,east_sum_field= get_east_area(target_poly,east_west_list_field,test_poly_objs_field,prior_option,distance)
                west_field,west_sum_field= get_west_area(target_poly,east_west_list_field,test_poly_objs_field,prior_option,distance)
                north_field,north_sum_field= get_north_area(target_poly,north_south_list_field,test_poly_objs_field,prior_option,distance)
                south_field,south_sum_field= get_south_area(target_poly,north_south_list_field,test_poly_objs_field,prior_option,distance)

                east_field_result, east_M_result=get_result_list(east_field,threshold_value)
                west_field_result, west_M_result = get_result_list(west_field, threshold_value )
                south_field_result, south_M_result = get_result_list(south_field, threshold_value)
                north_field_result, north_M_result = get_result_list(north_field, threshold_value)

                if one_field_only ==1:
                    #如果只查地块，且只返回一个地块：
                    resultString +=str(target_poly.id)+":"+str(east_field_result[:1])+" "+str(south_field_result[:1])\
                                  +" "+str(west_field_result[:1])+" "+ str(north_field_result[:1])+'|'
                elif one_field_only ==0:
                    # 如果只查地块，且只返回所有地块：
                    resultString +=str(target_poly.id)+":"+str(east_field_result[:3])+" "+str(south_field_result[:3])\
                                  +" "+str(west_field_result[:3])+" "+ str(north_field_result[:3])+'|'

            if field_only == 0:
                #如果同时查地块和面状物，则只进行需要将面状物和地块的可能的边合并成一个list
                east_west_list=east_west_list_field
                east_west_list.extend(east_west_list_others)
                north_south_list=north_south_list_field
                north_south_list.extend(north_south_list_others)
                test_poly_objs_field.extend(test_poly_objs_others)

                if one_field_only==0:
                    # 如果同时查地块和面状物,且需要返回一个列表
                    east_res, east_sum = get_east_area(target_poly, east_west_list_field,\
                                                               test_poly_objs_field,prior_option,distance)
                    west_res, west_sum = get_west_area(target_poly, east_west_list_field,\
                                                               test_poly_objs_field,prior_option,distance)
                    north_res, north_sum = get_north_area(target_poly, north_south_list_field,\
                                                                  test_poly_objs_field,prior_option,distance)
                    south_res, south_sum = get_south_area(target_poly, north_south_list_field,\
                                                                  test_poly_objs_field,prior_option,distance)


                    east_field_result, east_M_result = get_result_list(east_res, threshold_value)
                    west_field_result, west_M_result = get_result_list(west_res, threshold_value)
                    south_field_result, south_M_result = get_result_list(south_res, threshold_value)
                    north_field_result, north_M_result = get_result_list(north_res, threshold_value)

                    east_result = east_field_result
                    east_result.extend(east_M_result)
                    west_result = west_field_result
                    west_result.extend(west_M_result)
                    south_result = south_field_result
                    south_result.extend(south_M_result)
                    north_result = north_field_result
                    north_result.extend(north_M_result)
                    resultString += str(target_poly.id) + ":" + str(east_result[:3]) + " " + str(south_result[:3]) \
                                    + " " + str(west_result[:3]) + " " + str(north_result[:3]) + '|'

                if one_field_only == 1:
                    #如果同时查地块和面状物,且需要一个值

                    if prior_option==0:
                        #优先级为默认的话，返回两个值，一个地块，一个面状物，地块在前
                        east_res, east_sum = get_east_area(target_poly, east_west_list_field, \
                                                           test_poly_objs_field, prior_option,distance)
                        west_res, west_sum = get_west_area(target_poly, east_west_list_field, \
                                                           test_poly_objs_field, prior_option,distance)
                        north_res, north_sum = get_north_area(target_poly, north_south_list_field, \
                                                              test_poly_objs_field,prior_option,distance)
                        south_res, south_sum = get_south_area(target_poly, north_south_list_field, \
                                                              test_poly_objs_field,prior_option,distance)
                        east_field_result, east_M_result = get_result_list(east_res, threshold_value )
                        west_field_result, west_M_result = get_result_list(west_res, threshold_value )
                        south_field_result, south_M_result = get_result_list(south_res, threshold_value )
                        north_field_result, north_M_result = get_result_list(north_res, threshold_value )
                        east_result=east_field_result[:1]
                        east_result.extend(east_M_result[:1])
                        west_result=west_field_result[:1]
                        west_result.extend(west_M_result[:1])
                        south_result=south_field_result[:1]
                        south_result.extend(south_M_result[:1])
                        north_result=north_field_result[:1]
                        north_result.extend(north_M_result[:1])

                        resultString +=str(target_poly.id)+":"+str(east_result)+" "+str(south_result)\
                                      +" "+str(west_result)+" "+ str(north_result)+'|'

                    if prior_option==1:
                        #优先级为默认的话，返回一个值，如果有地块，返回地块，否则返回面状物
                        # 优先级为默认的话，返回两个值，一个地块，一个面状物，地块在前
                        east_res, east_sum = get_east_area(target_poly, east_west_list_field, \
                                                           test_poly_objs_field, prior_option,distance)
                        west_res, west_sum = get_west_area(target_poly, east_west_list_field, \
                                                           test_poly_objs_field, prior_option,distance)
                        north_res, north_sum = get_north_area(target_poly, north_south_list_field, \
                                                              test_poly_objs_field, prior_option,distance)
                        south_res, south_sum = get_south_area(target_poly, north_south_list_field, \
                                                              test_poly_objs_field, prior_option,distance)
                        east_field_result, east_M_result = get_result_list(east_res, threshold_value )
                        west_field_result, west_M_result = get_result_list(west_res, threshold_value )
                        south_field_result, south_M_result = get_result_list(south_res, threshold_value)
                        north_field_result, north_M_result = get_result_list(north_res, threshold_value)

                        east_result=east_field_result[:1]
                        west_result=west_field_result[:1]
                        south_result=south_field_result[:1]
                        north_result=north_field_result[:1]

                        if east_result==[]:
                            east_result=east_M_result[:1]
                        if west_result==[]:
                            west_result=west_M_result[:1]
                        if south_result==[]:
                            south_result=south_M_result[:1]
                        if north_result==[]:
                            north_result=north_M_result[:1]

                        resultString += str(target_poly.id) + ":" + str(east_result) + " " + str(south_result) \
                                        + " " + str(west_result) + " " + str(north_result) + '|'

                    if prior_option==2:
                        #如果为距离优先，则只返回一个结果，计算时不分地块和线面状物。一起计算，最后取最大的一个结果返回。
                        east_res_dis, east_sum = get_east_area(target_poly, east_west_list_field, \
                                                                   test_poly_objs_field, prior_option,distance)
                        west_res_dis, west_sum = get_west_area(target_poly, east_west_list_field, \
                                                                   test_poly_objs_field, prior_option,distance)
                        north_res_dis, north_sum = get_north_area(target_poly, north_south_list_field, \
                                                                      test_poly_objs_field,prior_option, distance)
                        south_res_dis, south_sum = get_south_area(target_poly, north_south_list_field, \
                                                                      test_poly_objs_field,prior_option, distance)
                        east_result=get_result_list_all(east_res_dis,threshold_value)[:1]
                        west_result=get_result_list_all(west_res_dis,threshold_value)[:1]
                        north_result=get_result_list_all(north_res_dis,threshold_value)[:1]
                        south_result=get_result_list_all(south_res_dis,threshold_value)[:1]

                        resultString += str(target_poly.id) + ":" + str(east_result) + " " + str(south_result) \
                                        + " " + str(west_result) + " " + str(north_result) + '|'


    return  resultString.strip("|")

# if __name__ == '__main__':
#     import time
#     print time.strftime('%H:%M:%S',time.localtime(time.time()))
#     with open(u'D:/1.txt') as fr:
#         my_string = fr.read().strip()
#         print len(my_string)
#         result_string = get_field_orientation_result(my_string,3,0,0,2,0.1)
#         # get_field_orientation_result(field_info_string, distance, one_field_only, field_only, prior_option, \
#         #                              threshold_value=0.25):
#     # print result_string
#     print time.strftime('%H:%M:%S',time.localtime(time.time()))
#     with open(u'D:/result.txt','w') as fr2:
#         fr2.writelines(result_string)

