/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.Spatial;
using System.IO;
//using IronPython.Hosting; //导入IronPython库文件
using Microsoft.Scripting.Hosting;
using System.Text;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 地块示意图专用-调用查找四至的算法-判断邻宗有无地块
    /// </summary>
    public class SearchNeighborCommon
    {
        #region Field

        #endregion

        #region Property
        public List<XZDW> CurrentZonexzdws { set; get; }
        public List<MZDW> CurrentZonemzdws { set; get; }

        public List<ContractLand> CurrentZoneQueryLandList { set; get; }

        #endregion

        #region Ctor
        public SearchNeighborCommon()
        {

        }
        #endregion

        /// <summary>
        /// 初始化所有的线状地物的坐标
        /// </summary>
        public string GetXzdwXY(List<XZDW> currentZonexzdws)
        {
            string currrentZoneQueryxzdwXYStr = "";
            if (currentZonexzdws.Count == 0) return currrentZoneQueryxzdwXYStr;
            StringBuilder builder = new StringBuilder();
            for (int im = 0; im < currentZonexzdws.Count; im++)
            {
                var geo = currentZonexzdws[im].Shape;
                if (geo == null) continue;
                if (geo.IsValid() == false)
                {
                    YuLinTu.Library.Log.Log.WriteError(this, "查找四至构建线状地物坐标", "空间信息错误(自交等):ID为" + currentZonexzdws[im].ID + "名称:" + currentZonexzdws[im].DWMC);
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
            return currrentZoneQueryxzdwXYStr;
        }

        /// <summary>
        /// 初始化所有的面状地物的坐标
        /// </summary>
        public string GetMzdwXY(List<MZDW> currentZonemzdws)
        {
            string currrentZoneQuerymzdwXYStr = "";
            try
            {
                if (currentZonemzdws.Count == 0) return currrentZoneQuerymzdwXYStr;
                StringBuilder builder = new StringBuilder();

                for (int im = 0; im < currentZonemzdws.Count; im++)
                {
                    var geo = currentZonemzdws[im].Shape;
                    if (geo == null) continue;

                    if (geo.IsValid() == false)
                    {
                        YuLinTu.Library.Log.Log.WriteError(this, "查找四至构建面状地物坐标", "空间信息错误(自交等):ID为" + currentZonemzdws[im].ID + "名称:" + currentZonemzdws[im].DWMC);
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
            return currrentZoneQuerymzdwXYStr;
        }


        /// <summary>
        /// 获取地块的语句
        /// </summary>
        /// <returns></returns>
        public string GetQueryString(List<ContractLand> queryLands)
        {
            string nowQueryString = "";

            if (queryLands.Count() == 0) return nowQueryString;
            try
            {
                foreach (var land in queryLands)
                {
                    Geometry currentLandGeo = land.Shape;//当前地块shape 
                    if (currentLandGeo.IsValid() == false)
                    {
                        YuLinTu.Library.Log.Log.WriteError(this, "查找四至构建地块坐标", string.Format("{0}的地块空间信息有误(自交等)，地块编码:{1}", land.OwnerName, land.LandNumber));
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
                }
            }
            catch (Exception ex)
            {

                YuLinTu.Library.Log.Log.WriteError(this, "查找数据构建查询条件", ex.Message + ex.StackTrace);
            }
            return nowQueryString;
        }

        /// <summary>
        /// 根据查询的字符串，调用python的接口，进行调用。
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public string QueryNeighborString(string queryString, double bufferDistence)
        {
            string landNeighborString = "";

            try
            {
                //ScriptRuntime pyRuntime = Python.CreateRuntime();
                ////创建一下运行环境
                //string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Python\find_orientation.py";
                //dynamic obj = pyRuntime.UseFile(fileName); //调用一个Python文件
                //if (obj == null)
                //{
                //    YuLinTu.Library.Log.Log.WriteError(this, "查找数据调用Python接口", "未在安装目录对应的位置下找到对应的Python查找文件find_orientation.py，请检查。");
                //    return "";
                //}

                //函数get_field_orientation_result(field_info_string,distance,one_field_only)

                // field_info_string：地块信息字符串。具体格式如下：
                //地块ID01：坐标x1,坐标y1,...坐标xn,坐标yn；地块ID02：坐标x1,坐标y1,坐标x2,坐标y2,...坐标xn,坐标yn；地块ID03：坐标x1,坐标y1,坐标x2,坐标y2,...坐标xn,坐标yn | 地块ID11...
                //其中，地块ID01为需要给出四至的地块，地块ID02和地块ID03为在缓冲距离内的候选地块，各地块之间用“;”间隔。地块ID11为另一个需要给出四至的地块。地块ID01字符串和地块ID11字符串之间用“|”间隔。间隔符均为英文字符。
                //也可以只输入一个待测试地块的信息。

                //distance：缓冲距离，接受int或者float型数值。单位为米

                //one_field_only：是否只返回一个地块。Int型（只能取0或1.避免python与c#的布尔值定义可能的区别），1为只返回一个地块，0为返回一个列表结果。

                int one_field_only = 1;
                int field_only = 0; //查询线状面状地物

                /// 查找规则   "默认" 为0, "地块优先" 1,"距离优先" 2
                /// 查询阀值 0.1
                var foCore = new FoundOrientation.FoundOrientationCore();
                var pythonQueryResult = foCore.get_field_orientation_result(queryString, bufferDistence, one_field_only, field_only, 2, 0.1); //调用Python文件中的求和函数
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
                YuLinTu.Library.Log.Log.WriteError(this, "查找数据调用Python接口", ex.Message + ex.StackTrace);
                return landNeighborString;
            }
            return landNeighborString;
        }

        /// <summary>
        /// 根据查询的结果处理对应的业务。获取查询到的数据值,string为地块ID，string为四至名称,bool是否邻宗有地块
        /// </summary>
        public Dictionary<string, Dictionary<string, bool>> InitializeNeighborInfo(List<ContractLand> currentZoneQueryLandList, string queryString)
        {
            Dictionary<string, Dictionary<string, bool>> landneighborhasland = new Dictionary<string, Dictionary<string, bool>>();           
            List<ContractLand> updateLands = new List<ContractLand>();

            var landneiborghinfos = queryString.Split('|').ToList();//当前的地块集合
            int landneiborghinfocount = landneiborghinfos.Count;
            try
            {

           
            ContractLand targetLand = null;
            for (int i = 0; i < landneiborghinfocount; i++)
            {
                if (landneiborghinfos[i].IsNullOrEmpty()) continue;
                Dictionary<string, bool> landneighborinfo = new Dictionary<string, bool>();
                var landinfos = landneiborghinfos[i].Split(':');

                var targetlandid = landinfos[0];
                if (targetlandid != null)
                {
                    Guid landid = Guid.NewGuid();
                    Guid.TryParse(targetlandid, out landid);
                    targetLand = currentZoneQueryLandList.Find(cf => cf.ID == landid);
                    if (targetLand == null) continue;
                }

                var targetlandneiborghinfo = landinfos[1];

                var targetlandneiborghs = targetlandneiborghinfo.Split('[');
                if (targetlandneiborghs[0].IsNullOrEmpty())
                {
                    //获取东南西北的根据业务相关的所有四至信息
                    landneighborinfo.Add("东至", GetOneNeighborInfo(targetlandneiborghs[1]));
                    landneighborinfo.Add("南至", GetOneNeighborInfo(targetlandneiborghs[2]));
                    landneighborinfo.Add("西至", GetOneNeighborInfo(targetlandneiborghs[3]));
                    landneighborinfo.Add("北至", GetOneNeighborInfo(targetlandneiborghs[4]));
                }
                landneighborhasland.Add(targetlandid, landneighborinfo);
            }

            }
            catch (Exception ex)
            {
                var dd = ex;
                throw;
            }

            return landneighborhasland;
        }


        /// <summary>
        /// 根据读出来的一个至，返回对应处理好的业务至
        /// </summary>
        /// <param name="oneNeighborInfo"></param>
        /// <returns></returns>
        private bool GetOneNeighborInfo(string oneNeighborInfo)
        {
            bool hasNeighbor = false;

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
            foreach (var item in neighborInfos)
            {
                if (item.Length <= 1 && neighborInfos.Count == 1) return hasNeighbor;
                var idinfo = item.GetBetween("'", "'");
                if (idinfo.IsNullOrEmpty())
                {
                    continue;
                }
                else if (idinfo.StartsWith("X"))
                {
                    idinfo = idinfo.Substring(1);
                    Guid xzdwID = Guid.NewGuid();
                    Guid.TryParse(idinfo, out xzdwID);
                    if (xzdwID == Guid.Empty) return hasNeighbor;
                    var xzdw = CurrentZonexzdws.Find(xx => xx.ID == xzdwID);
                    if (xzdw != null)
                    {
                        hasNeighbor = true;
                    }
                }
                else if (idinfo.StartsWith("M"))
                {
                    idinfo = idinfo.Substring(1);
                    Guid mzdwID = Guid.NewGuid();
                    Guid.TryParse(idinfo, out mzdwID);
                    if (mzdwID == Guid.Empty) return hasNeighbor;
                    var mzdw = CurrentZonemzdws.Find(xx => xx.ID == mzdwID);
                    if (mzdw != null)
                    {
                        hasNeighbor = true;
                    }
                }
                else
                {
                    Guid landID = Guid.NewGuid();
                    Guid.TryParse(idinfo, out landID);
                    if (landID == Guid.Empty) return hasNeighbor;
                    var neighborLand = CurrentZoneQueryLandList.Find(cd => cd.ID == landID);

                    if (neighborLand != null)
                    {
                        hasNeighbor = true;
                    }
                }
            }
            return hasNeighbor;
        }

        /// <summary>
        /// 获取当前区域下，缓冲距离内的地块、线状、面状地物
        /// </summary>       
        public void GetCurrentZoneIntersects(Zone currentZone, IDbContext db, double buffer, List<ContractLand> currentZoneLandList)
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

            //var landQuery = db.CreateQuery<ContractLand>();
            var xzdwQuery = db.CreateQuery<XZDW>();
            var mzdwQuery = db.CreateQuery<MZDW>();

            //CurrentZoneQueryLandList = landQuery.Where(c => c.Shape.Intersects(fullExtend)).ToList();
            CurrentZonexzdws = xzdwQuery.Where(c => c.Shape.Intersects(fullExtend)).ToList();
            CurrentZonemzdws = mzdwQuery.Where(c => c.Shape.Intersects(fullExtend)).ToList();
        }

    }
}
