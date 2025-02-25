/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.Formula.Functions;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Business
{
    public class SeekLandNeighborBus : Task
    {
        #region Field

        private Zone parentZone;//当前地域的父地域
        private string markDesc;//地域描述
        /// <summary>
        /// 单方向查找
        /// </summary>
        private bool simplePositionQuery;

        /// <summary>
        /// 某方向为空时使用村民小组名称填充
        /// </summary>
        private bool useGroupName;
        private string useGroupNameContext;

        /// <summary>
        /// 查找地块名称
        /// </summary>
        private bool searchLandName;

        /// <summary>
        /// 识别地块类别
        /// </summary>
        private bool landIdentify;

        /// <summary>
        /// 识别土地利用类型
        /// </summary>
        private bool landType;

        /// <summary>
        /// 只填充空白四至
        /// </summary>
        private bool fillEmptyNeighbor;

        /// <summary>
        /// 设置宗地查找缓冲距离(米)
        /// </summary>
        private double bufferDistence;

        /// <summary>
        /// 查询线状\面状地物
        /// </summary>
        public bool isQueryXMzdw { set; get; }

        /// <summary>
        /// 查找规则   "默认" 为0, "地块优先" 1,"距离优先" 2
        /// </summary>
        public int SearchDeteilRule { set; get; }

        /// <summary>
        /// 同至去重复地物名称
        /// </summary>
        public bool isDeleteSameDWMC { set; get; }

        /// <summary>
        /// 查询阈值(米)
        /// </summary>
        public double queryThreshold { set; get; }

        public bool onlyCurrentZone { get; set; }

        private List<Dictionary> DKLBdics;
        private List<Dictionary> TDLYLXdics;

        /// <summary>
        /// 当前地域下地块集合
        /// </summary>
        private List<ContractLand> currentZoneLandList;

        ///// <summary>
        ///// 当前地域下有空白四至地块集合
        ///// </summary>
        //private List<ContractLand> currentZoneNeighborBullLandList;

        /// <summary>
        /// 当前地域下要查询的地块集合
        /// </summary>
        private List<ContractLand> currentZoneQueryLandList;

        /// <summary>
        /// 当前地域对应要查询的线状集合
        /// </summary>
        private List<XZDW> currentZonexzdws;

        /// <summary>
        /// 当前地域对应要查询的面状集合
        /// </summary>
        private List<MZDW> currentZonemzdws;

        ///// 当前地域对应的镇下线状集合
        //private Dictionary<Guid, string> currrentZonexzdwXY;

        ///// 当前地域对应的镇下面状集合
        //private Dictionary<Guid, string> currrentZonemzdwXY;

        // 当前地域对应的区域缓冲下线状集合查询字符串
        private string currrentZoneQueryxzdwXYStr;

        // 当前地域对应的区域缓冲下面状集合查询字符串
        private string currrentZoneQuerymzdwXYStr;

        /// <summary>
        /// 数据库
        /// </summary>
        private IDbContext dbContext;
        private Zone currentZone;//当前地域

        private IContractLandWorkStation landStation;

        #endregion

        #region Method
        /// <summary>
        /// 查找四至
        /// </summary> 
        public void ContractLandInitialTool()
        {
            this.ReportProgress(0, "开始");
            var checkres = CheckData();
            if (checkres == false) return;

            InitializeFieldDatas();
            string getQueryString = GetQueryString2();
            if (getQueryString.IsNullOrEmpty())
            {
                this.ReportInfomation("查询字符串为空，请检查地域图斑或设置。");
                this.ReportProgress(100, "完成");
                return;
            }
            this.ReportProgress(31, "开始调用查询算法，请等待");
            string getLandNeighborString = QueryNeighborString(getQueryString);

            this.ReportProgress(40, "查询结束，开始更新数据");
            InitializeNeighborInfo(getLandNeighborString);

            this.ReportProgress(100, "完成");

            DisposeAll();
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <returns></returns>
        private bool CheckData()
        {
            TaskSeekLandNeighborArgument meta = Argument as TaskSeekLandNeighborArgument;
            currentZoneLandList = meta.CurrentZoneLandList;
            dbContext = meta.Database;
            currentZone = meta.CurrentZone;
            markDesc = GetMarkDesc(meta.CurrentZone, meta.Database);
            if (dbContext == null)
            {
                this.ReportProgress(100, null);
                this.ReportWarn(string.Format("{0}未连接到数据库", markDesc));
                return false;
            }
            if (currentZone == null)
            {
                this.ReportProgress(100, null);
                this.ReportWarn("未选择初始化数据的地域!");
                return false;
            }
            if (currentZoneLandList.Count == 0)
            {
                this.ReportProgress(100, null);
                this.ReportWarn(string.Format("{0}无地块数据", markDesc));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化参数设置
        /// </summary>
        private void InitializeFieldDatas()
        {
            TaskSeekLandNeighborArgument meta = Argument as TaskSeekLandNeighborArgument;
            simplePositionQuery = meta.seekLandNeighborSet.SimplePositionQuery;
            useGroupName = meta.seekLandNeighborSet.UseGroupName;
            useGroupNameContext = meta.seekLandNeighborSet.UseGroupNameContext == null ? "" : meta.seekLandNeighborSet.UseGroupNameContext;
            searchLandName = meta.seekLandNeighborSet.SearchLandName;
            landIdentify = meta.seekLandNeighborSet.LandIdentify;
            landType = meta.seekLandNeighborSet.LandType;
            fillEmptyNeighbor = meta.seekLandNeighborSet.FillEmptyNeighbor;
            bufferDistence = meta.seekLandNeighborSet.BufferDistance;//缓冲距离
            isDeleteSameDWMC = meta.seekLandNeighborSet.IsDeleteSameDWMC;
            isQueryXMzdw = meta.seekLandNeighborSet.IsQueryXMzdw;
            queryThreshold = meta.seekLandNeighborSet.QueryThreshold;
            SearchDeteilRule = meta.seekLandNeighborSet.SearchDeteilRule;
            onlyCurrentZone = meta.seekLandNeighborSet.OnlyCurrentZone;

            TDLYLXdics = meta.DicList.FindAll(d => d.GroupCode == DictionaryTypeInfo.TDLYLX);
            DKLBdics = meta.DicList.FindAll(d => d.GroupCode == DictionaryTypeInfo.DKLB);

            landStation = dbContext.CreateContractLandWorkstation();

            GetCurrentZoneIntersects(currentZone, dbContext, bufferDistence);

            int xzdwerrorCount = currentZonexzdws.RemoveAll(x => x.Shape == null);
            int mzdwerrorCount = currentZonemzdws.RemoveAll(m => m.Shape == null);

            if (xzdwerrorCount > 0)
            {
                this.ReportWarn(string.Format("当前镇下有{0}个线状地物空间图形有误(为空等)，不能参与查询", xzdwerrorCount));
            }

            if (mzdwerrorCount > 0)
            {
                this.ReportWarn(string.Format("当前镇下有{0}个面状地物空间图形有误(为空等)，不能参与查询", mzdwerrorCount));
            }

            if (isQueryXMzdw)
            {
                this.ReportInfomation("初始化线状地物坐标信息" + currentZonexzdws.Count + "条");
                this.ReportProgress(0, "初始化线状地物坐标信息");
                GetXzdwXY();

                this.ReportInfomation("初始化面状地物坐标信息" + currentZonemzdws.Count + "条");
                this.ReportProgress(0, "初始化面状地物坐标信息");
                GetMzdwXY();
            }
        }

        /// <summary>
        /// 初始化所有的线状地物的坐标
        /// </summary>
        private void GetXzdwXY()
        {
            currrentZoneQueryxzdwXYStr = "";
            if (currentZonexzdws.Count == 0) return;
            StringBuilder builder = new StringBuilder();
            for (int im = 0; im < currentZonexzdws.Count; im++)
            {
                var geo = currentZonexzdws[im].Shape;
                if (geo == null) continue;
                if (geo.IsValid() == false)
                {
                    this.ReportInfomation("空间信息错误(自交等):ID为" + currentZonexzdws[im].ID + "名称:" + currentZonexzdws[im].DWMC);
                }
                var interlandcts = geo.ToCoordinates();
                builder.Append("X" + currentZonexzdws[im].ID + ":");

                for (int i = 0; i < interlandcts.Count(); i++)
                {
                    if (i != interlandcts.Count() - 1)
                    {
                        builder.Append(interlandcts[i].X + "," + interlandcts[i].Y + ",");
                    }
                    else
                    {
                        builder.Append(interlandcts[i].X + "," + interlandcts[i].Y + ";");
                    }
                }
            }
            currrentZoneQueryxzdwXYStr = builder.ToString();
        }

        /// <summary>
        /// 初始化所有的面状地物的坐标
        /// </summary>
        private void GetMzdwXY()
        {
            currrentZoneQuerymzdwXYStr = "";
            try
            {
                if (currentZonemzdws.Count == 0) return;
                StringBuilder builder = new StringBuilder();

                for (int im = 0; im < currentZonemzdws.Count; im++)
                {
                    var geo = currentZonemzdws[im].Shape;
                    if (geo == null) continue;

                    if (geo.IsValid() == false)
                    {
                        this.ReportInfomation("空间信息错误(自交等):ID为" + currentZonemzdws[im].ID + "名称:" + currentZonemzdws[im].DWMC);
                    }
                    builder.Append("M" + currentZonemzdws[im].ID + ":");

                    var geoGroupCdts = geo.ToGroupCoordinates();
                    bool isGroupCdts = geoGroupCdts.Count() > 1 ? true : false;

                    for (int w = 0; w < geoGroupCdts.Count; w++)
                    {
                        var interlandcts = geoGroupCdts[w];

                        for (int i = 0; i < interlandcts.Count(); i++)
                        {
                            if (i != interlandcts.Count() - 1)
                            {
                                builder.Append(interlandcts[i].X + "," + interlandcts[i].Y + ",");
                            }
                            else
                            {
                                builder.Append(interlandcts[i].X + "," + interlandcts[i].Y + ";");
                            }
                        }
                        if (isGroupCdts && w != geoGroupCdts.Count - 1)
                        {
                            builder.Append("|");
                        }
                    }
                }
                currrentZoneQuerymzdwXYStr = builder.ToString();
            }
            catch (Exception ex)
            {
                var errorinfo = ex.Message;
            }

        }

        /// <summary>
        /// 获取查询的语句
        /// </summary>
        /// <returns></returns>
        private string GetQueryString()
        {
            string nowQueryString = "";
            int index = 1;   //地块索引
            double landPercent = 0.0;  //百分比 

            var landStation = dbContext.CreateContractLandWorkstation();

            landPercent = 30 / (double)currentZoneQueryLandList.Count;
            List<ContractLand> queryLands = currentZoneQueryLandList;//当前要查询的地
            //if (fillEmptyNeighbor)
            //{
            //    queryLands = currentZoneQueryLandList.FindAll(cl => cl.NeighborEast.IsNullOrEmpty() || cl.NeighborNorth.IsNullOrEmpty() || cl.NeighborSouth.IsNullOrEmpty() || cl.NeighborWest.IsNullOrEmpty());
            //    if (queryLands.Count == 0) return nowQueryString;
            //}
            try
            {
                foreach (var land in queryLands)
                {
                    this.ReportProgress((int)(landPercent * index), string.Format("生成{0}的地块查询语句", markDesc + land.OwnerName));
                    //if (land.LandNumber == null)
                    //{
                    //    this.ReportWarn(string.Format("{0}的地块有为空的地块编码", markDesc + land.OwnerName));
                    //}

                    //if (queryLands.FindAll(lf => lf.LandNumber == land.LandNumber).Count > 1)
                    //{                      
                    //    this.ReportWarn(markDesc + "有至少两个相同地块编码的地块，地块编码为:" + land.LandNumber);
                    //}

                    Geometry currentLandGeo = land.Shape;//当前地块shape 
                    if (currentLandGeo.IsValid() == false)
                    {
                        this.ReportWarn(string.Format("{0}的地块空间信息有误(自交等)，地块编码:{1}", markDesc + land.OwnerName, land.LandNumber));
                    }

                    nowQueryString += land.ID.ToString() + ":";
                    var geoGroupCdts = currentLandGeo.ToGroupCoordinates();
                    bool isGroupCdts = geoGroupCdts.Count() > 1 ? true : false;

                    for (int w = 0; w < geoGroupCdts.Count; w++)
                    {
                        var targetlandcts = geoGroupCdts[w];
                        for (int i = 0; i < targetlandcts.Count(); i++)
                        {
                            if (i != targetlandcts.Count() - 1)
                            {
                                nowQueryString += targetlandcts[i].X + "," + targetlandcts[i].Y + ",";
                            }
                            else
                            {
                                nowQueryString += targetlandcts[i].X + "," + targetlandcts[i].Y + ";";
                            }
                        }
                        if (isGroupCdts && w != geoGroupCdts.Count - 1)
                        {
                            nowQueryString += "|";
                        }
                    }

                    index++;
                }

                if (isQueryXMzdw)
                {
                    nowQueryString += currrentZoneQueryxzdwXYStr;
                    nowQueryString += currrentZoneQuerymzdwXYStr;
                }

                this.ReportProgress(30, "查询的语句完成");
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteError(this, "查找数据构建查询条件", ex.Message + ex.StackTrace);
            }
            return nowQueryString;
        }

        /// <summary>
        /// 获取查询的语句
        /// </summary>
        /// <returns></returns>
        private string GetQueryString2()
        {
            //string nowQueryString = "";
            int index = 1;   //地块索引
            double landPercent = 0.0;  //百分比 
            var sb = new StringBuilder();
            var landStation = dbContext.CreateContractLandWorkstation();

            landPercent = 30 / (double)currentZoneQueryLandList.Count;
            List<ContractLand> queryLands = currentZoneQueryLandList;//当前要查询的地
            try
            {
                foreach (var land in queryLands)
                {
                    this.ReportProgress((int)(landPercent * index), string.Format("生成{0}的地块查询语句", markDesc + land.OwnerName));
                    Geometry currentLandGeo = land.Shape;//当前地块shape 
                    if (currentLandGeo.IsValid() == false)
                    {
                        this.ReportWarn(string.Format("{0}的地块空间信息有误(自交等)，地块编码:{1}", markDesc + land.OwnerName, land.LandNumber));
                    }

                    //nowQueryString += land.ID.ToString() + ":";
                    sb.Append(land.ID.ToString() + ":");
                    var geoGroupCdts = currentLandGeo.ToGroupCoordinates();
                    bool isGroupCdts = geoGroupCdts.Count() > 1 ? true : false;

                    for (int w = 0; w < geoGroupCdts.Count; w++)
                    {
                        var targetlandcts = geoGroupCdts[w];
                        for (int i = 0; i < targetlandcts.Count(); i++)
                        {
                            if (i != targetlandcts.Count() - 1)
                            {
                                //nowQueryString += targetlandcts[i].X + "," + targetlandcts[i].Y + ",";
                                sb.Append(targetlandcts[i].X + "," + targetlandcts[i].Y + ",");
                            }
                            else
                            {
                                //nowQueryString += targetlandcts[i].X + "," + targetlandcts[i].Y + ";";
                                sb.Append(targetlandcts[i].X + "," + targetlandcts[i].Y + ";");
                            }
                        }
                        if (isGroupCdts && w != geoGroupCdts.Count - 1)
                        {
                            //nowQueryString += "|";
                            sb.Append("|");
                        }
                    }

                    index++;
                }

                if (isQueryXMzdw)
                {
                    //nowQueryString += currrentZoneQueryxzdwXYStr;
                    //nowQueryString += currrentZoneQuerymzdwXYStr;
                    sb.Append(currrentZoneQueryxzdwXYStr);
                    sb.Append(currrentZoneQuerymzdwXYStr);
                }

                this.ReportProgress(30, "查询的语句完成");
            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteError(this, "查找数据构建查询条件", ex.Message + ex.StackTrace);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 根据查询的字符串，调用python的接口，进行调用。
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private string QueryNeighborString(string queryString)
        {
            //using (FileStream fs = new FileStream("D:\\work\\exportdata\\" + currentZone.FullName + "(查找四至).txt", FileMode.Create))
            //{
            //    StreamWriter sw = new StreamWriter(fs);

            //    sw.Write(queryString);

            //    sw.Flush();
            //    sw.Close();
            //    fs.Close();
            //}

            string landNeighborString = "";

            try
            {

                //ScriptRuntime pyRuntime = Python.CreateRuntime();
                ////创建一下运行环境
                //string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Python\find_orientation.py";
                //dynamic obj = pyRuntime.UseFile(fileName); //调用一个Python文件
                //if (obj == null)
                //{
                //    this.ReportWarn("未在安装目录对应的位置下找到对应的Python查找文件find_orientation.py，请检查。");
                //    return "";
                //}


                //函数get_field_orientation_result(field_info_string,distance,one_field_only)

                // field_info_string：地块信息字符串。具体格式如下：
                //地块ID01：坐标x1,坐标y1,...坐标xn,坐标yn；地块ID02：坐标x1,坐标y1,坐标x2,坐标y2,...坐标xn,坐标yn；地块ID03：坐标x1,坐标y1,坐标x2,坐标y2,...坐标xn,坐标yn | 地块ID11...
                //其中，地块ID01为需要给出四至的地块，地块ID02和地块ID03为在缓冲距离内的候选地块，各地块之间用“;”间隔。地块ID11为另一个需要给出四至的地块。地块ID01字符串和地块ID11字符串之间用“|”间隔。间隔符均为英文字符。
                //也可以只输入一个待测试地块的信息。

                //distance：缓冲距离，接受int或者float型数值。单位为米

                //one_field_only：是否只返回一个地块。Int型（只能取0或1.避免python与c#的布尔值定义可能的区别），1为只返回一个地块，0为返回一个列表结果。

                int one_field_only = simplePositionQuery ? 1 : 0;
                int field_only = isQueryXMzdw ? 0 : 1;

                var foCore = new FoundOrientation.FoundOrientationCore();

                //var pythonQueryResult = obj.get_field_orientation_result(queryString, bufferDistence, one_field_only, field_only, SearchDeteilRule, queryThreshold); //调用Python文件中的求和函数
                var pythonQueryResult = foCore.get_field_orientation_result(queryString, bufferDistence, one_field_only, field_only, SearchDeteilRule, queryThreshold); //调用Python文件中的求和函数
                //var pythonQueryResult = obj.get_field_orientation_result(queryString, bufferDistence, one_field_only, field_only, SearchDeteilRule, queryThreshold); //调用Python文件中的求和函数
                if (pythonQueryResult == null) return "";
                landNeighborString = pythonQueryResult.ToString();
                //obj = null;
                //pyRuntime.Shutdown();
                //pyRuntime = null;
                //返回值为字符串，格式为如下：
                //测试地块的ID：[东至结果列表],[南至结果列表],[西至结果列表],[北至结果列表]|下一个结果
                //如果某一个至向结果为空，则返回的结果是"[]"
                //以下为一个示例（注：目前结果中并未实现地块优先，不过我会在明后两天改好）：
                //        5101151052490200391:
                //        []
                //        ['M87fd0a4f-1985-420b-aafe-c2b36f83b966']
                //        []
                //        ['5101151052490200418']|
                //        5101151052490200395:['5101151052490200254', 'X82d72a4b-d363-4db3-b1a3-4e0a5ed13742']
                //        ['X82d72a4b-d363-4db3-b1a3-4e0a5ed13742', '5101151052490200382']
                //        ['5101151052490200382', '5101151052490200306']
                //        ['5101151052490200306', '5101151052490200137', '5101151052490200339']|

            }
            catch (Exception ex)
            {
                this.ReportError(ex.Message);
                YuLinTu.Library.Log.Log.WriteError(this, "查找数据调用Python接口", ex.Message + ex.StackTrace);
                return landNeighborString;
            }
            return landNeighborString;
        }

        /// <summary>
        /// 根据查询的结果处理对应的业务。
        /// </summary>
        private void InitializeNeighborInfo(string queryString)
        {

            //测试地块的ID：[东至结果列表][南至结果列表][西至结果列表][北至结果列表]| 下一个结果
            // queryString = "5101151052490200391:" +
            // "[]" +
            //"['M87fd0a4f-1985-420b-aafe-c2b36f83b966']" +
            //"[]" +
            //"['5101151052490200418']|" +
            // "5101151052490200395:['5101151052490200254', 'X82d72a4b-d363-4db3-b1a3-4e0a5ed13742']" +
            //"['X82d72a4b-d363-4db3-b1a3-4e0a5ed13742', '5101151052490200382']" +
            //"['5101151052490200382', '5101151052490200306']" +
            //"['5101151052490200306', '5101151052490200137', '5101151052490200339']|";

            var landStation = dbContext.CreateContractLandWorkstation();
            List<ContractLand> updateLands = new List<ContractLand>();

            var landneiborghinfos = queryString.Split('|').ToList();//当前的地块集合
            int landneiborghinfocount = landneiborghinfos.Count;

            int index = 1;   //地块索引
            double landPercent = 0.0;  //百分比 
            landPercent = 58 / (double)landneiborghinfocount;
            ContractLand targetLand = null;
            for (int i = 0; i < landneiborghinfocount; i++)
            {
                if (landneiborghinfos[i].IsNullOrEmpty()) continue;
                string landneiborgheastStr = "";
                string landneiborghsouthStr = "";
                string landneiborghwestStr = "";
                string landneiborghnorthtStr = "";
                var landinfos = landneiborghinfos[i].Split(':');
                //var targetlandnumber = landinfos[0];
                //if (targetlandnumber != null)
                //{
                //    targetLand = currentZoneLandList.Find(cf => cf.LandNumber == targetlandnumber);
                //    if (targetLand == null) continue;
                //}
                var targetlandid = landinfos[0];
                if (targetlandid != null)
                {
                    Guid landid = Guid.NewGuid();
                    Guid.TryParse(targetlandid, out landid);
                    targetLand = currentZoneQueryLandList.Find(cf => cf.ID == landid);
                    if (targetLand == null || targetLand.ZoneCode == null)
                        continue;
                    if (onlyCurrentZone && !targetLand.ZoneCode.StartsWith(currentZone.FullCode))
                        continue;
                }

                //if (targetLand.LandNumber == "1508261072090700291")
                //{
                //    var dd = "123";
                //}

                var targetlandneiborghinfo = landinfos[1];

                targetLand.AliasNameB = landneiborghinfos[i];//留下当前查找的结果，用于与用户填制的四至对比
                                                             //if (targetLand.LandNumber == "5101151052490100147")
                                                             //{
                                                             //    var dd = "ds";
                                                             //}
                var targetlandneiborghs = targetlandneiborghinfo.Split('[');
                if (targetlandneiborghs[0].IsNullOrEmpty())
                {
                    //获取东南西北的根据业务相关的所有四至信息
                    landneiborgheastStr = GetOneNeighborInfo(targetlandneiborghs[1]);
                    landneiborghsouthStr = GetOneNeighborInfo(targetlandneiborghs[2]);
                    landneiborghwestStr = GetOneNeighborInfo(targetlandneiborghs[3]);
                    landneiborghnorthtStr = GetOneNeighborInfo(targetlandneiborghs[4]);
                }

                InitializeLandNeighbor(targetLand, landneiborgheastStr, landneiborghsouthStr, landneiborghwestStr, landneiborghnorthtStr);

                updateLands.Add(targetLand);
                this.ReportProgress(40 + (int)(landPercent * index), string.Format("更新{0}的地块四至信息", markDesc + targetLand.OwnerName));
            }
            if (updateLands.Count > 0)
            {
                this.ReportProgress(98, "查找完成，开始更新数据库");
                var metadata = Argument as TaskSeekLandNeighborArgument;
                if (metadata.UpdateLandList != null || metadata.UpdateLandList.Count > 0)
                {
                    var tuplist = updateLands.FindAll(t => metadata.UpdateLandList.Any(a => a.LandNumber == t.LandNumber));
                    landStation.UpdateRange(tuplist);
                }
                else
                {
                    landStation.UpdateRange(updateLands);
                }
            }
            this.ReportInfomation(string.Format("在{0}下共查找{1}个地块", markDesc, currentZoneLandList.Count));
        }

        /// <summary>
        /// 根据处理返回的四至结果，更改地块四至
        /// </summary>
        /// <param name="targetLand"></param>
        /// <param name="landneiborgheastStr"></param>
        /// <param name="landneiborghsouthStr"></param>
        /// <param name="landneiborghwestStr"></param>
        /// <param name="landneiborghnorthtStr"></param>
        /// <returns></returns>
        private ContractLand InitializeLandNeighbor(ContractLand targetLand, string landneiborgheastStr, string landneiborghsouthStr, string landneiborghwestStr, string landneiborghnorthtStr)
        {
            if (fillEmptyNeighbor)
            {
                if (targetLand.NeighborEast.IsNullOrEmpty())
                {
                    //if (useGroupName && landneiborgheastStr.IsNullOrEmpty())
                    //{
                    //    landneiborgheastStr = parentZone.Name + currentZone.Name;
                    //}
                    targetLand.NeighborEast = NeighborChange(landneiborgheastStr);
                }
                if (targetLand.NeighborSouth.IsNullOrEmpty())
                {
                    //if (useGroupName && landneiborghsouthStr.IsNullOrEmpty())
                    //{
                    //    landneiborghsouthStr = parentZone.Name + currentZone.Name;
                    //}
                    targetLand.NeighborSouth = NeighborChange(landneiborghsouthStr);
                }
                if (targetLand.NeighborWest.IsNullOrEmpty())
                {
                    //if (useGroupName && landneiborghwestStr.IsNullOrEmpty())
                    //{
                    //    landneiborghwestStr = parentZone.Name + currentZone.Name;
                    //}
                    targetLand.NeighborWest = NeighborChange(landneiborghwestStr);
                }
                if (targetLand.NeighborNorth.IsNullOrEmpty())
                {
                    //if (useGroupName && landneiborghnorthtStr.IsNullOrEmpty())
                    //{
                    //    landneiborghnorthtStr = parentZone.Name + currentZone.Name;
                    //}
                    targetLand.NeighborNorth = NeighborChange(landneiborghnorthtStr);
                }
            }
            else
            {
                //if (useGroupName && landneiborgheastStr.IsNullOrEmpty())
                //{
                //    landneiborgheastStr = parentZone.Name + currentZone.Name;
                //}
                targetLand.NeighborEast = NeighborChange(landneiborgheastStr);

                //if (useGroupName && landneiborghsouthStr.IsNullOrEmpty())
                //{
                //    landneiborghsouthStr = parentZone.Name + currentZone.Name;
                //}
                targetLand.NeighborSouth = NeighborChange(landneiborghsouthStr);

                //if (useGroupName && landneiborghwestStr.IsNullOrEmpty())
                //{
                //    landneiborghwestStr = parentZone.Name + currentZone.Name;
                //}
                targetLand.NeighborWest = NeighborChange(landneiborghwestStr);

                //if (useGroupName && landneiborghnorthtStr.IsNullOrEmpty())
                //{
                //    landneiborghnorthtStr = parentZone.Name + currentZone.Name;
                //}
                targetLand.NeighborNorth = NeighborChange(landneiborghnorthtStr);
            }

            return targetLand;
        }

        /// <summary>
        /// 四至替换
        /// </summary>
        private string NeighborChange(string landneiborghnorthtStr)
        {
            if (useGroupName && landneiborghnorthtStr.IsNullOrEmpty())
            {
                return string.IsNullOrEmpty(useGroupNameContext.Trim()) ? parentZone.Name + currentZone.Name : useGroupNameContext.Trim();
            }
            return landneiborghnorthtStr;
        }

        /// <summary>
        /// 根据读出来的一个至，返回对应处理好的业务至
        /// </summary>
        /// <param name="oneNeighborInfo"></param>
        /// <returns></returns>
        private string GetOneNeighborInfo(string oneNeighborInfo)
        {
            string oneNeighborALLInfo = "";

            List<string> neighborInfos = new List<string>();
            if (oneNeighborInfo.Contains(","))
            {
                neighborInfos = oneNeighborInfo.Split(',').ToList();
            }
            else
            {
                neighborInfos.Add(oneNeighborInfo);
            }

            //单个至里面循环获取要素
            string cbdoneNeighborALLInfo = "";
            string xzdwoneNeighborALLInfo = "";
            string mzdwoneNeighborALLInfo = "";
            foreach (var item in neighborInfos)
            {
                if (item.Length <= 1 && neighborInfos.Count == 1) return oneNeighborALLInfo;
                var idinfo = item.GetBetween("'", "'");
                if (idinfo.IsNullOrEmpty())
                {
                    continue;
                }
                else if (idinfo.StartsWith("X"))
                {
                    var getxzdwmc = GetXZDWDWMC(idinfo);
                    if (getxzdwmc.IsNullOrEmpty()) continue;
                    if (xzdwoneNeighborALLInfo.IsNullOrEmpty())
                    {
                        xzdwoneNeighborALLInfo = getxzdwmc;
                    }
                    else if (isDeleteSameDWMC && xzdwoneNeighborALLInfo.Contains(getxzdwmc))
                    {
                        continue;
                    }
                    else
                    {
                        xzdwoneNeighborALLInfo += "," + getxzdwmc;
                    }
                }
                else if (idinfo.StartsWith("M"))
                {
                    var getmzdwmc = GetMZDWDWMC(idinfo);
                    if (getmzdwmc.IsNullOrEmpty()) continue;
                    if (mzdwoneNeighborALLInfo.IsNullOrEmpty())
                    {
                        mzdwoneNeighborALLInfo = getmzdwmc;
                    }
                    else if (isDeleteSameDWMC && mzdwoneNeighborALLInfo.Contains(getmzdwmc))
                    {
                        continue;
                    }
                    else
                    {
                        mzdwoneNeighborALLInfo += "," + getmzdwmc;
                    }
                }
                else
                {
                    Guid landID = Guid.NewGuid();
                    Guid.TryParse(idinfo, out landID);
                    if (landID == Guid.Empty) continue;
                    var neighborLand = currentZoneQueryLandList.Find(cd => cd.ID == landID);
                    if (neighborLand == null)
                    {
                        neighborLand = landStation.Get(landID);
                    }
                    if (neighborLand == null) continue;

                    if (neighborLand != null)
                    {
                        var neighborlandOwnerName = string.IsNullOrEmpty(neighborLand.OwnerName) ? "" : InitializeNameBySet.InitalizeFamilyName(neighborLand.OwnerName);

                        if (cbdoneNeighborALLInfo.IsNullOrEmpty())
                        {
                            cbdoneNeighborALLInfo = neighborlandOwnerName;
                        }
                        else if (isDeleteSameDWMC && cbdoneNeighborALLInfo.Contains(neighborlandOwnerName))
                        {
                            continue;
                        }
                        else
                        {
                            cbdoneNeighborALLInfo += "," + neighborlandOwnerName;
                        }

                        #region 其他属性赋值
                        if (landIdentify)
                        {
                            if (neighborLand != null && neighborLand.LandCategory != null)
                            {
                                var dic = DKLBdics.Find(d => d.Code == neighborLand.LandCategory);
                                if (dic != null)
                                {
                                    cbdoneNeighborALLInfo += "(" + dic.Name + ")";
                                }
                            }
                        }
                        if (landType)
                        {

                            if (neighborLand != null && neighborLand.LandCode != null)
                            {
                                var dic = TDLYLXdics.Find(d => d.Code == neighborLand.LandCode);
                                if (dic != null)
                                {
                                    cbdoneNeighborALLInfo += "(" + dic.Name + ")";
                                }
                            }
                        }
                        if (searchLandName)
                        {
                            cbdoneNeighborALLInfo += string.IsNullOrEmpty(neighborLand.Name) ? "" : "(" + neighborLand.Name + ")";
                        }
                        #endregion                        
                    }
                }
            }

            oneNeighborALLInfo = CombineResultNeighborString(cbdoneNeighborALLInfo, xzdwoneNeighborALLInfo, mzdwoneNeighborALLInfo);
            return oneNeighborALLInfo;
        }

        /// <summary>
        /// 获取线状地物名称
        /// </summary> 
        private string GetXZDWDWMC(string idinfo)
        {
            string xzdwoneNeighborALLInfo = "";
            idinfo = idinfo.Substring(1);
            Guid xzdwID = Guid.NewGuid();
            Guid.TryParse(idinfo, out xzdwID);
            if (xzdwID == Guid.Empty) return xzdwoneNeighborALLInfo;
            var xzdw = currentZonexzdws.Find(xx => xx.ID == xzdwID);
            if (xzdw != null)
            {
                xzdwoneNeighborALLInfo = string.IsNullOrEmpty(xzdw.DWMC) ? "" : xzdw.DWMC;
            }

            return xzdwoneNeighborALLInfo;
        }

        /// <summary>
        /// 获取面状地物名称
        /// </summary> 
        private string GetMZDWDWMC(string idinfo)
        {
            string mzdwoneNeighborALLInfo = "";
            idinfo = idinfo.Substring(1);
            Guid mzdwID = Guid.NewGuid();
            Guid.TryParse(idinfo, out mzdwID);
            if (mzdwID == Guid.Empty) return mzdwoneNeighborALLInfo;
            var mzdw = currentZonemzdws.Find(xx => xx.ID == mzdwID);
            if (mzdw != null)
            {
                mzdwoneNeighborALLInfo = string.IsNullOrEmpty(mzdw.DWMC) ? "" : mzdw.DWMC;
            }
            return mzdwoneNeighborALLInfo;
        }

        /// <summary>
        /// 合并最终查询出来的结果字符串
        /// </summary>       
        private string CombineResultNeighborString(string cbdoneNeighborALLInfo, string xzdwoneNeighborALLInfo, string mzdwoneNeighborALLInfo)
        {
            string oneNeighborALLInfo = "";

            if (cbdoneNeighborALLInfo.IsNotNullOrEmpty())
            {
                oneNeighborALLInfo += cbdoneNeighborALLInfo;
            }
            if (xzdwoneNeighborALLInfo.IsNotNullOrEmpty())
            {
                if (oneNeighborALLInfo.IsNullOrEmpty())
                {
                    oneNeighborALLInfo = xzdwoneNeighborALLInfo;
                }
                else
                {
                    oneNeighborALLInfo += "," + xzdwoneNeighborALLInfo;
                }
            }
            if (mzdwoneNeighborALLInfo.IsNotNullOrEmpty())
            {
                if (oneNeighborALLInfo.IsNullOrEmpty())
                {
                    oneNeighborALLInfo = mzdwoneNeighborALLInfo;
                }
                else
                {
                    oneNeighborALLInfo += "," + mzdwoneNeighborALLInfo;
                }
            }
            return oneNeighborALLInfo;
        }

        #region 辅助方法

        /// <summary>
        /// 获取当前区域下，缓冲距离内的地块、线状、面状地物
        /// </summary>       
        private void GetCurrentZoneIntersects(Zone currentZone, IDbContext db, double buffer)
        {
            Geometry fullExtend = null;//当前地域缓冲出来的查询范围 
            if (currentZone != null && currentZone.Shape != null)
            {
                fullExtend = currentZone.Shape.Buffer(buffer);
            }
            else
            {
                Spatial.Envelope extent = new Envelope();
                foreach (var item in currentZoneLandList)
                {
                    if (item.Shape == null) continue;
                    var ext = item.Shape.GetEnvelope();
                    extent.Union(ext);
                }
                fullExtend = extent.ToGeometry().Buffer(buffer);
            }
            if (fullExtend == null) return;

            var landQuery = db.CreateQuery<ContractLand>();
            var xzdwQuery = db.CreateQuery<XZDW>();
            var mzdwQuery = db.CreateQuery<MZDW>();

            currentZoneQueryLandList = landQuery.Where(c => c.Shape.Intersects(fullExtend)).ToList();
            currentZonexzdws = xzdwQuery.Where(c => c.Shape.Intersects(fullExtend)).ToList();
            currentZonemzdws = mzdwQuery.Where(c => c.Shape.Intersects(fullExtend)).ToList();
        }

        private void DisposeAll()
        {
            //if (DKLBdics != null)
            //{
            //    DKLBdics.Clear();
            //    DKLBdics = null;
            //}

            //if (TDLYLXdics != null)
            //{
            //    TDLYLXdics.Clear();
            //    TDLYLXdics = null;
            //}

            //if (currentZoneLandList != null)
            //{
            //    currentZoneLandList.Clear();
            //    currentZoneLandList = null;
            //}

            //if (currentZoneNeighborBullLandList != null)
            //{
            //    currentZoneNeighborBullLandList.Clear();
            //    currentZoneNeighborBullLandList = null;
            //}
            //TaskSeekLandNeighborArgument meta = Argument as TaskSeekLandNeighborArgument;
            //if (meta != null)
            //{
            //    meta.seekLandNeighborSet = null;
            //    meta = null;
            //}

            //if (currentZonemzdws != null)
            //{
            //    currentZonemzdws.Clear();
            //    currentZonemzdws = null;
            //}

            //if (currentZonexzdws != null)
            //{
            //    currentZonexzdws.Clear();
            //    currentZonexzdws = null;
            //}

            //currrentZoneQuerymzdwXYStr = null;
            //currrentZoneQueryxzdwXYStr = null;
            //if (VillageContractLands != null)
            //{
            //    VillageContractLands.Clear();
            //    VillageContractLands = null;
            //}

            //if (currrentZonexzdwXY != null)
            //{
            //    currrentZonexzdwXY.Clear();
            //    currrentZonexzdwXY = null;
            //}

            //if (currrentZonemzdwXY != null)
            //{
            //    currrentZonemzdwXY.Clear();
            //    currrentZonemzdwXY = null;
            //}

            GC.Collect();
        }

        /// <summary>
        /// 根据当前地域获得任务描述信息
        /// </summary>
        private string GetMarkDesc(Zone zone, IDbContext Database)
        {
            parentZone = GetParent(zone, Database);  //获取上级地域
            string excelName = string.Empty;
            if (zone.Level == eZoneLevel.Town)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                excelName = parentZone.Name + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Group)
            {
                Zone parentTowm = GetParent(parentZone, Database);
                excelName = parentTowm.Name + parentZone.Name + zone.Name;
            }
            return excelName;
        }

        /// <summary>
        /// 获取上级地域
        /// </summary>
        /// <param name="zone"></param>
        /// <returns></returns>
        public Zone GetParent(Zone zone, IDbContext Database)
        {
            ModuleMsgArgs arg = new ModuleMsgArgs();
            arg.Datasource = Database;
            arg.Parameter = zone;
            arg.Name = ZoneMessage.ZONE_PARENT_ZONE;
            TheBns.Current.Message.Send(this, arg);
            return (arg.ReturnValue as Zone);
        }





        #endregion

        #endregion
    }
}
