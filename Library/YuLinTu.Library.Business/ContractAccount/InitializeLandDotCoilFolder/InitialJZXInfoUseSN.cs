/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Aux;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.NetAux;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 调用四至的接口，更新已有界址线的信息
    /// </summary>
    public class InitialJZXInfoUseSN : Task
    {
        #region Fields

        private IContractLandWorkStation landStation;
        private IBuildLandBoundaryAddressCoilWorkStation coilStation;
        private IBuildLandBoundaryAddressDotWorkStation dotStation;
        private IXZDWWorkStation xzdwStation;
        private IMZDWWorkStation mzdwStation;
        private Zone currentZone;
        private double bufferdistence;
        private string markDesc;//地域描述
        private Zone parentZone;//当前地域的父地域
        private List<Dictionary> coilType;
        private List<Dictionary> coilLocation;
        private int currentZoneLandCount;
        /// <summary>
        /// 是否根据四至查找，界址线毗邻权利人为空的情况，使用输入的自定义的权利人名称
        /// </summary>
        private bool IsLineVpNameUseImport;

        /// <summary>
        /// 是否根据四至查找，界址线毗邻权利人为空的情况，使用输入的自定义的权利人名称-值
        /// </summary>
        private string LineVpNameUseImportValue;

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext database;

        /// <summary>
        /// 当前要处理的界址点
        /// </summary>
        private List<BuildLandBoundaryAddressDot> currentZoneDots;

        /// <summary>
        /// 当前要处理的界址线
        /// </summary>
        private List<BuildLandBoundaryAddressCoil> currentZoneCoils;

        /// <summary>
        /// 当前地域对应要查询的线状集合
        /// </summary>
        private List<XZDW> currentZonexzdws;

        /// <summary>
        /// 当前地域对应要查询的面状集合
        /// </summary>
        private List<MZDW> currentZonemzdws;

        /// <summary>
        /// 要进行初始化的地块
        /// </summary>
        private List<ContractLand> listGeoLand;

        /// <summary>
        /// 要进行初始化的地块
        /// </summary>
        private List<ContractLand> currentZoneGeoLand;

        // 当前地域对应的区域缓冲下线状集合查询字符串
        private string currrentZoneQueryxzdwXYStr;

        // 当前地域对应的区域缓冲下面状集合查询字符串
        private string currrentZoneQuerymzdwXYStr;

        // 当前地域对应的地块四至查询字符串
        private string currrentZoneQuerycbdNeighborStr;

        private TaskInitializeLandDotCoilArgument metaArgument;

        #endregion

        #region Ctor

        public InitialJZXInfoUseSN(TaskInitializeLandDotCoilArgument MetaArgument)
        {
            metaArgument = MetaArgument;
            currentZone = MetaArgument.CurrentZone;
            database = MetaArgument.Database;
            bufferdistence = MetaArgument.InstallArg.AddressLinedbiDistance;//.AddressLinedbiDistance;
        }

        #endregion

        #region Method

        /// <summary>
        /// 进行初始化操作
        /// </summary>
        public void MainHandle()
        {
            var ret = InitialAllData();
            if (ret == false) return;

            var queryStr = GetQueryString2();
            this.ReportProgress(30, "开始调用算法进行更新");

            var queryResult = QueryNeighborString(queryStr);
            this.ReportProgress(35, "查找结束，开始进行获取更新");

            InitializeJZXInfo(queryResult);
            DisposeAll();
        }

        private void DisposeAll()
        {
            if (coilType != null)
            {
                coilType.Clear();
                coilType = null;
            }

            landStation = null;
            coilStation = null;
            dotStation = null;

            if (currentZonemzdws != null)
            {
                currentZonemzdws.Clear();
                currentZonemzdws = null;
            }

            if (currentZonexzdws != null)
            {
                currentZonexzdws.Clear();
                currentZonexzdws = null;
            }

            currrentZoneQuerymzdwXYStr = null;
            currrentZoneQueryxzdwXYStr = null;
            currrentZoneQuerycbdNeighborStr = null;

            GC.Collect();
        }

        /// <summary>
        /// 初始化获取所有数据
        /// </summary>
        public bool InitialAllData()
        {
            if (database == null)
            {
                this.ReportInfomation("当前数据源为空");
                return false;
            }
            if (currentZone == null)
            {
                this.ReportInfomation("当前地域为空");
                return false;
            }

            IsLineVpNameUseImport = !string.IsNullOrEmpty(metaArgument.SetArg.InitialJZXInfoSet);
            LineVpNameUseImportValue = metaArgument.SetArg.InitialJZXInfoSet;

            currrentZoneQuerycbdNeighborStr = "";
            markDesc = GetMarkDesc(currentZone, database);
            dotStation = database.CreateDotWorkStation();
            coilStation = database.CreateCoilWorkStation();
            landStation = database.CreateContractLandWorkstation();
            xzdwStation = database.CreateXZDWWorkStation();
            mzdwStation = database.CreateMZDWWorkStation();
            var dicStation = database.CreateDictWorkStation();
            coilType = dicStation.GetByGroupCode(DictionaryTypeInfo.JZXLB, false);
            coilLocation = dicStation.GetByGroupCode(DictionaryTypeInfo.JZXWZ, false);
            var searchLevel = eLevelOption.Self;
            if (currentZone.Level >= eZoneLevel.Village)
            {
                searchLevel = eLevelOption.SelfAndSubs;
            }

            List<ContractLand> initLandList = landStation.GetCollection(currentZone.FullCode, searchLevel);
            currentZoneGeoLand = initLandList;
            currentZoneLandCount = initLandList.Count;

            listGeoLand = initLandList == null ? new List<ContractLand>() : initLandList.FindAll(c => c.Shape != null);

            if (listGeoLand.Count == 0)
            {
                this.ReportInfomation("当前地域下地块为空");
                return false;
            }

            Geometry fullExtend = GetFullExtend();//当前地域缓冲出来的查询范围             
            if (fullExtend == null) return false;

            var landQuery = database.CreateQuery<ContractLand>();
            var xzdwQuery = database.CreateQuery<XZDW>();
            var mzdwQuery = database.CreateQuery<MZDW>();

            currentZonexzdws = xzdwQuery.Where(c => c.Shape.Intersects(fullExtend)).ToList();
            currentZonemzdws = mzdwQuery.Where(c => c.Shape.Intersects(fullExtend)).ToList();
            listGeoLand = landQuery.Where(c => c.Shape.Intersects(fullExtend)).ToList();

            if (currentZonexzdws.Count == 0 || currentZonemzdws.Count == 0 || listGeoLand.Count == 0)
            {
                using (var db = new DBSpatialite())
                {
                    var dbFile = metaArgument.Database.DataSource.ConnectionString;
                    dbFile = dbFile.Substring(dbFile.IndexOf('=') + 1);
                    db.Open(dbFile);

                    var wh = string.Format(Zd_cbdFields.ZLDM + " like '{0}%'", currentZone.FullCode);
                    var env = db.QueryEnvelope(Zd_cbdFields.TABLE_NAME, Zd_cbdFields.Shape, wh, new List<string> { "rowid" });
                    if (currentZonexzdws.Count == 0)
                    {
                        db.QueryIntersectsCallback(XzdwFields.TABLE_NAME, XzdwFields.SHAPE, env, r =>
                        {
                            var e = WKBHelper.fromWKB(r.GetValue(1) as byte[]);
                            var x = new XZDW()
                            {
                                ID = r.GetGuid(0),
                                Shape = Geometry.FromInstance(e),
                                DWMC = SqlUtil.GetString(r, 2),
                                Comment = SqlUtil.GetString(r, 3)
                            };

                            currentZonexzdws.Add(x);
                            return true;
                        }, new List<string> { "ID", XzdwFields.SHAPE, XzdwFields.DWMC, XzdwFields.BZ });
                    }

                    if (currentZonemzdws.Count == 0)
                    {
                        db.QueryIntersectsCallback(MzdwFields.TABLE_NAME, MzdwFields.SHAPE, env, r =>
                        {
                            var e = WKBHelper.fromWKB(r.GetValue(1) as byte[]);
                            var x = new MZDW()
                            {
                                ID = r.GetGuid(0),
                                Shape = Geometry.FromInstance(e),
                                DWMC = SqlUtil.GetString(r, 2),
                                Comment = SqlUtil.GetString(r, 3)
                            };

                            currentZonemzdws.Add(x);
                            return true;
                        }, new List<string> { "ID", MzdwFields.SHAPE, MzdwFields.DWMC, MzdwFields.BZ });
                    }

                    if (listGeoLand.Count == 0)
                    {
                        var icc = new IntCoordConter();
                        icc.Init(env.MinX);
                        db.QueryIntersectsCallback(Zd_cbdFields.TABLE_NAME, Zd_cbdFields.Shape, env, r =>
                        {
                            var e = WKBHelper.fromWKB(r.GetValue(1) as byte[]);
                            var x = new ContractLand()
                            {
                                ID = r.GetGuid(0),
                                Shape = Geometry.FromInstance(e),
                                OwnerName = SqlUtil.GetString(r,2),
                                LandNumber = SqlUtil.GetString(r,3)
                            };

                            listGeoLand.Add(x);
                            return true;
                        }, new List<string> { "ID", Zd_cbdFields.Shape, Zd_cbdFields.Qlrmc, Zd_cbdFields.DKBM});
                    }
                }
            }

            currentZoneDots = dotStation.GetByZoneCode(currentZone.FullCode, searchLevel);
            currentZoneCoils = coilStation.GetByZoneCode(currentZone.FullCode, searchLevel);

            if (currentZoneDots.Count == 0)
            {
                this.ReportInfomation(string.Format("当前地域{0}下界址点为空", markDesc));
                return false;
            }
            if (currentZoneCoils.Count == 0)
            {
                this.ReportInfomation(string.Format("当前地域{0}下界址线为空", markDesc));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 得到查找范围
        /// </summary>
        /// <returns></returns>
        private Geometry GetFullExtend()
        {
            Geometry fullExtend = null;//当前地域缓冲出来的查询范围 
            if (currentZone != null && currentZone.Shape != null)
            {
                fullExtend = currentZone.Shape.Buffer(bufferdistence);
            }
            else
            {
                Spatial.Envelope extent = new Spatial.Envelope();
                foreach (var item in listGeoLand)
                {
                    if (item.Shape == null) continue;
                    var ext = item.Shape.GetEnvelope();
                    extent.Union(ext);
                }
                fullExtend = extent.ToGeometry().Buffer(bufferdistence);
            }
            return fullExtend;
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
        /// 获取总共查询的语句
        /// </summary>
        /// <returns></returns>
        private string GetQueryString2()
        {
            GetXzdwXY();
            GetMzdwXY();

            //string nowQueryString = "";
            var sb = new StringBuilder();
            int index = 1;   //地块索引
            double landPercent = 0.0;  //百分比 

            landPercent = 28 / (double)listGeoLand.Count;
            List<ContractLand> queryLands = listGeoLand;//当前要查询的地
            var dotGroupList = currentZoneDots.GroupBy(t => t.LandID).Select(s => new { LandID = s.Key, List = s.ToList() }).ToList();
            try
            {
                foreach (var land in queryLands)
                {
                    this.ReportProgress((int)(landPercent * index), string.Format("生成{0}的地块查询语句", markDesc + land.OwnerName));
                    var dotList = dotGroupList.Find(fa => fa.LandID == land.ID);
                    var landdots = dotList == null ? new List<BuildLandBoundaryAddressDot>() : dotList.List;
                    if (landdots.Count == 0)
                    {
                        landdots = dotStation.GetByLandId(land.ID);
                    }
                    if (landdots.Count == 0)
                    {
                        //this.ReportInfomation(markDesc + "没有界址点，地块编码为:" + land.LandNumber);
                        continue;
                    }

                    Geometry currentLandGeo = land.Shape;//当前地块shape 
                    if (currentLandGeo.IsValid() == false)
                    {
                        this.ReportWarn(string.Format("{0}的地块空间信息有误(自交等)，地块编码:{1}", markDesc + land.OwnerName, land.LandNumber));
                    }

                    if (currrentZoneQuerycbdNeighborStr.IsNullOrEmpty())
                    {
                        currrentZoneQuerycbdNeighborStr = GetQueryLandNeighborInfo(land);
                    }
                    else
                    {
                        currrentZoneQuerycbdNeighborStr += ";" + GetQueryLandNeighborInfo(land);
                    }

                    bool hasaddlandid = false;
                    var geoGroupCdts = currentLandGeo.ToGroupCoordinates();
                    bool isGroupCdts = geoGroupCdts.Count() > 1 ? true : false;

                    for (int w = 0; w < geoGroupCdts.Count; w++)
                    {
                        var targetlandcts = geoGroupCdts[w];


                        if (hasaddlandid == false)
                        {
                            sb.Append(land.ID + ":");// nowQueryString += 
                            hasaddlandid = true;
                        }

                        int querycdts = targetlandcts.Count();
                        var targetlandcts1 = targetlandcts.ToList();
                        int cfindex = 0;
                        bool iscf = false;
                        for (int i = 0; i < targetlandcts.Count(); i++)
                        {
                            if (targetlandcts1.Count(dd => dd.X == targetlandcts[i].X && dd.Y == targetlandcts[i].Y) >= 2)
                            {
                                cfindex = i;
                                iscf = true;
                                break;
                            }
                        }

                        if (iscf)
                        {
                            targetlandcts1.RemoveAt(cfindex);
                        }

                        for (int i = 0; i < targetlandcts1.Count; i++)
                        {
                            var nowdot = landdots.Find(lf =>
                            {
                                if (Math.Abs(lf.Shape.ToCoordinates()[0].X - targetlandcts1[i].X) < 0.01 && Math.Abs(lf.Shape.ToCoordinates()[0].Y - targetlandcts1[i].Y) < 0.01)
                                {
                                    return true;
                                }
                                return false;
                            });
                            if (nowdot == null)
                            {
                                continue;
                            }

                            var dotnumber = nowdot.DotNumber;
                            if (nowdot.IsValid)
                            {
                                sb.Append(dotnumber + "#" + targetlandcts1[i].X + "," + dotnumber + "#" + targetlandcts1[i].Y);
                            }
                            else
                            {
                                sb.Append(targetlandcts1[i].X + "," + targetlandcts1[i].Y);
                            }

                            if (i != targetlandcts1.Count() - 1)
                            {
                                sb.Append(",");
                            }
                            else
                            {
                                sb.Append(";");
                            }
                        }
                        if (isGroupCdts && w != geoGroupCdts.Count - 1)
                        {
                            sb.Append("|");
                        }
                    }
                    index++;
                }

                sb.Append(currrentZoneQueryxzdwXYStr);
                sb.Append(currrentZoneQuerymzdwXYStr);

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
        /// 根据每个地的扩展字段(使用查找四至的功能得到结果)和已有四至(用户手填、处理过)，重新组织查询用的四至字符串，如果是手填的前面需要加N,如果都没有就用0代替
        /// </summary>
        /// <param name="land"></param>
        /// <returns></returns>
        private string GetQueryLandNeighborInfo(ContractLand land)
        {
            string neighborinfo = land.ID + ":";
            var neiborAllinfo = land.AliasNameB;

            //if (land.ID.ToString() == "33d20c58-febe-4bc1-8b5c-e8090bf6a734")
            //{
            //    var dd = "fds";
            //}

            var eastinfo = land.NeighborEast.IsNullOrEmpty() ? "0" : "N" + land.NeighborEast;
            var southinfo = land.NeighborSouth.IsNullOrEmpty() ? "0" : "N" + land.NeighborSouth;
            var westinfo = land.NeighborWest.IsNullOrEmpty() ? "0" : "N" + land.NeighborWest;
            var northinfo = land.NeighborNorth.IsNullOrEmpty() ? "0" : "N" + land.NeighborNorth;

            if (neiborAllinfo.IsNullOrEmpty())
            {
                neighborinfo += eastinfo + "," + southinfo + "," + westinfo + "," + northinfo;
                return neighborinfo;
            }

            var targetlandneiborghs = neiborAllinfo.Split('[');

            neighborinfo += GetQueryLandNeighborSimpleInfo(targetlandneiborghs[1], eastinfo);
            neighborinfo += "," + GetQueryLandNeighborSimpleInfo(targetlandneiborghs[2], southinfo);
            neighborinfo += "," + GetQueryLandNeighborSimpleInfo(targetlandneiborghs[3], westinfo);
            neighborinfo += "," + GetQueryLandNeighborSimpleInfo(targetlandneiborghs[4], northinfo);

            return neighborinfo;
        }

        /// <summary>
        /// 获取单方向的四至
        /// </summary>
        /// <param name="landSimpleNeighborInfo"></param>
        /// <returns></returns>
        private string GetQueryLandNeighborSimpleInfo(string landSimpleNeighborInfo, string simpleNeiborInfo)
        {
            string retlandSimpleNeighborInfo = "";

            List<string> neighborInfos = new List<string>();
            if (landSimpleNeighborInfo.Contains(","))
            {
                neighborInfos = landSimpleNeighborInfo.Split(',').ToList();
            }
            else
            {
                neighborInfos.Add(landSimpleNeighborInfo);
            }

            //单个至里面循环获取要素          
            foreach (var item in neighborInfos)
            {
                if (item.Length <= 1 && neighborInfos.Count == 1)
                    return simpleNeiborInfo;

                var idinfo = item.GetBetween("'", "'");

                if (idinfo.IsNullOrEmpty() || idinfo == "emputy")
                    return simpleNeiborInfo;

                if (retlandSimpleNeighborInfo.IsNullOrEmpty())
                {
                    retlandSimpleNeighborInfo = idinfo;
                }
                else
                {
                    retlandSimpleNeighborInfo += "|" + idinfo;
                }
            }
            return retlandSimpleNeighborInfo;
        }

        /// <summary>
        /// 根据查询的字符串，调用python的接口，进行调用。
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        private string QueryNeighborString(string queryString)
        {
            string landNeighborString = "";
            try
            {
                //ScriptRuntime pyRuntime = Python.CreateRuntime();
                ////创建一下运行环境
                //string fileName = AppDomain.CurrentDomain.BaseDirectory + @"Python\find_boundary.py";
                //dynamic obj = pyRuntime.UseFile(fileName); //调用一个Python文件
                //if (obj == null)
                //{
                //    this.ReportWarn("未在安装目录对应的位置下找到对应的Python查找文件find_boundary.py，请检查。");
                //    return "";
                //}

                var obj = new FoundOrientation.FindLandBoundary();

                var pythonQueryResult = obj.Get_boundary_for_fields(currrentZoneQuerycbdNeighborStr, queryString, bufferdistence); //调用Python文件 暂时只能实现距离优先
                if (pythonQueryResult == null || pythonQueryResult == "") return "";
                landNeighborString = pythonQueryResult.ToString();
                obj = null;
                //pyRuntime.Shutdown();
                //pyRuntime = null;
                queryString = null;
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
        /// 根据查询的结果解析字符串，并更新界址数据
        /// </summary>
        private void InitializeJZXInfo(string queryString)
        {
            //{ '地块1_ID':[['J1J2','A','183.2','I',['51011510', 'X82d72a4b-']],['J2J3',...]],'地块2_ID':...}

            //        其中地块_ID后面所跟的信息说明如下：
            //        J1J2：界址边的名称，J1和J2为别为该边的第一，第二个界址点。以顺时针为排序。
            //        A:代表界址边的方位。A,B,C,D：分别为东南西北，E,F,G,H分别为为东南，东北，西南，西北
            //        183.2: 为界址线的长度，以米为单位，格式为字符串。
            //        I：为界址线位置，I为内，M为中，英文字母O为外
            //        ['51011510', 'X82d72a4b-']：为缓冲出来的毗邻权利人，包含有地块和面线状物的ID

            //            '地块1_ID':['J1+J2','A','183.2','M','51011510|X82d72a4b-']\
            //                       ['J2+J6',...]\
            //                        ......;
            //            '地块2_ID':... 
            //界址点之间用加号 界址线之间用斜杠   界址线内部信息用竖杠 地块之间用冒号  M是中 O是外

            var landneiborghinfos = queryString.Split(';').ToList();//当前的地块集合
            int landneiborghinfocount = landneiborghinfos.Count;

            List<BuildLandBoundaryAddressCoil> currentZoneUpdateCoils = new List<BuildLandBoundaryAddressCoil>();

            int index = 1;   //地块索引
            double landPercent = 0.0;  //百分比 
            landPercent = 60 / (double)landneiborghinfocount;
            ContractLand targetLand = null;
            for (int i = 0; i < landneiborghinfocount; i++)
            {
                if (landneiborghinfos[i].IsNullOrEmpty())
                    continue;

                var landinfos = landneiborghinfos[i].Split(':');
                var targetlandnumber = landinfos[0].GetBetween("'", "'");

                if (targetlandnumber == "3e71bc93-0317-41ee-a950-fd14c4fcbdfa")
                {
                    //var dd = "fdsa";
                }

                var targetlandid = landinfos[0].GetBetween("'", "'");
                if (targetlandid != null)
                {
                    Guid landid = Guid.NewGuid();
                    Guid.TryParse(targetlandid, out landid);
                    targetLand = listGeoLand.Find(cf => cf.ID == landid);
                    if (targetLand == null)
                        continue;
                }

                var targetLandCoils = currentZoneCoils.FindAll(czc => czc.LandID == targetLand.ID);
                if (targetLandCoils.Count == 0)
                    continue;

                var targetLandDots = currentZoneDots.FindAll(czc => czc.LandID == targetLand.ID);
                if (targetLandDots.Count == 0)
                {
                    targetLandDots = dotStation.GetByLandId(targetLand.ID);
                }
                targetLandCoils = GetUpdateCoilByResultStr(targetLandCoils, targetLandDots, landinfos[1]);
                currentZoneUpdateCoils.AddRange(targetLandCoils);
                this.ReportProgress(35 + (int)(landPercent * index), string.Format("更新{0}的地块界址信息", markDesc + targetLand.OwnerName));
            }

            this.ReportProgress(98, "查找完成，开始上传");
            coilStation.UpdateRange(currentZoneUpdateCoils);

            this.ReportInfomation(string.Format("在{0}下共更新{1}个地块", markDesc, currentZoneLandCount));
        }

        /// <summary>
        /// 解析地块结果字符串，然后更新对应的界址线
        /// </summary>
        /// <param name="updateCoils"></param>
        /// <param name="landResultStr"></param>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> GetUpdateCoilByResultStr(List<BuildLandBoundaryAddressCoil> updateCoils, List<BuildLandBoundaryAddressDot> landDots, string landResultStr)
        {
            List<BuildLandBoundaryAddressCoil> currentZoneUpdateCoils = new List<BuildLandBoundaryAddressCoil>();
            var interCoilAllStrs = landResultStr.Split('%');
            //每一条界址线
            for (int i = 0; i < interCoilAllStrs.Count(); i++)
            {
                var interStr0 = interCoilAllStrs[i].GetBetween("[", "]");
                var interStrInfos = interStr0.Split(',');

                //循环每个 'J1+J2','A','183.2','M','51011510|X82d72a4b-,'51011510''
                var interNumberStr0 = interStrInfos[0].GetBetween("'", "'");
                var nowCoilNums = interNumberStr0.Split('+');

                var nowcoil = updateCoils.Find(dc => dc.StartNumber == nowCoilNums[0] && dc.EndNumber == nowCoilNums[1]);
                if (nowcoil == null) continue;

                nowcoil.Description = GetDescription(nowCoilNums[0], nowCoilNums[1], interStrInfos[1], interStrInfos[2], landDots);

                var nowcoilpostion = GetLocationName(interStrInfos[3]);
                if (nowcoilpostion.IsNotNullOrEmpty())
                {
                    nowcoil.Position = nowcoilpostion;
                }

                nowcoil.NeighborPerson = GetNeighborInfo(interStrInfos[4]);
                nowcoil.NeighborFefer = nowcoil.NeighborPerson;

                var nowcoiltype = GetCoiltype(interStrInfos[5]);
                if (nowcoiltype.IsNotNullOrEmpty())
                {
                    nowcoil.CoilType = nowcoiltype;
                }

                currentZoneUpdateCoils.Add(nowcoil);
            }
            return currentZoneUpdateCoils;
        }

        private string GetCoiltype(string info)
        {
            var iteminfo = info.GetBetween("'", "'");
            if (iteminfo.StartsWith("X"))
            {
                var nowxzdw = currentZonexzdws.Find(czx => czx.ID.ToString() == iteminfo.Substring(1));
                if (nowxzdw != null)
                {
                    var coiltypedic = coilType.Find(dd => (nowxzdw.DWMC != null && nowxzdw.DWMC.Contains(dd.Name)) ||
                        (nowxzdw.Comment != null && nowxzdw.Comment.Contains(dd.Name)));
                    if (coiltypedic != null)
                    {
                        return coiltypedic.Code;
                    }
                }
            }
            else if (iteminfo.StartsWith("M"))
            {
                var nowmzdw = currentZonemzdws.Find(czx => czx.ID.ToString() == iteminfo.Substring(1));
                if (nowmzdw != null)
                {
                    var coiltypedic = coilType.Find(dd => (nowmzdw.DWMC != null && nowmzdw.DWMC.Contains(dd.Name)) || (nowmzdw.Comment != null && nowmzdw.Comment.Contains(dd.Name)));
                    if (coiltypedic != null)
                    {
                        return coiltypedic.Code;
                    }
                }
            }
            return "01";//如果是地块，或者没有找到地、手填，默认为田埂返回
        }

        /// <summary>
        /// 获取临宗信息，包括毗邻权利人
        /// </summary>
        /// <param name="info"></param>
        /// <param name="updateCoil"></param>
        /// <returns></returns>
        private string GetNeighborInfo(string info)
        {
            ///处理界址线类别和批量权利人
            var neighborPersonAllinfo = info.GetBetween("'", "'");
            var neighborPersonAllinfos = neighborPersonAllinfo.Split('|');
            string neighborPersonInfos = "";

            foreach (var iteminfo in neighborPersonAllinfos)
            {
                if (iteminfo.StartsWith("X"))
                {
                    var nowxzdw = currentZonexzdws.Find(czx => czx.ID.ToString() == iteminfo.Substring(1));
                    if (nowxzdw != null && nowxzdw.DWMC.IsNotNullOrEmpty())
                    {
                        if (neighborPersonInfos.IsNullOrEmpty())
                        {
                            neighborPersonInfos = nowxzdw.DWMC;
                        }
                        else
                        {
                            if (neighborPersonInfos.Contains(nowxzdw.DWMC) == false)
                            {
                                neighborPersonInfos += "," + nowxzdw.DWMC;
                            }
                        }
                    }
                }
                else if (iteminfo.StartsWith("M"))
                {
                    var nowmzdw = currentZonemzdws.Find(czx => czx.ID.ToString() == iteminfo.Substring(1));
                    if (nowmzdw != null && nowmzdw.DWMC.IsNotNullOrEmpty())
                    {
                        if (neighborPersonInfos.IsNullOrEmpty())
                        {
                            neighborPersonInfos = nowmzdw.DWMC;
                        }
                        else
                        {
                            if (neighborPersonInfos.Contains(nowmzdw.DWMC) == false)
                            {
                                neighborPersonInfos += "," + nowmzdw.DWMC;
                            }
                        }
                    }
                }
                else if (iteminfo == "1")//没有查到,是否根据设置手动赋值权利人
                {
                    neighborPersonInfos = "";

                    if (IsLineVpNameUseImport)
                    {
                        neighborPersonInfos = LineVpNameUseImportValue.IsNotNullOrEmpty() ? InitializeNameBySet.InitalizeFamilyName(LineVpNameUseImportValue) : "";
                    }
                }
                else if (iteminfo.StartsWith("N"))//手填
                {
                    string nowvpname = iteminfo.Substring(1);

                    ////UNICODE字符转为中文
                    string outStr = "";
                    if (!string.IsNullOrEmpty(nowvpname))
                    {
                        string[] strlist = nowvpname.Replace("\\", "").Split('u');
                        try
                        {
                            for (int i = 1; i < strlist.Length; i++)
                            {
                                //将unicode字符转为10进制整数，然后转为char中文字符  
                                outStr += (char)int.Parse(strlist[i], System.Globalization.NumberStyles.HexNumber);
                            }
                        }
                        catch (FormatException ex)
                        {
                            outStr = ex.Message;
                        }
                    }

                    var useoutStr = outStr.IsNullOrEmpty() ? nowvpname : InitializeNameBySet.InitalizeFamilyName(outStr);

                    if (neighborPersonInfos.IsNullOrEmpty())
                    {
                        neighborPersonInfos = useoutStr;
                    }
                    else
                    {
                        if (neighborPersonInfos.Contains(useoutStr) == false)
                        {
                            neighborPersonInfos += "," + useoutStr;
                        }
                    }
                }
                else //地块id
                {
                    Guid landID = Guid.NewGuid();
                    Guid.TryParse(iteminfo, out landID);
                    if (landID == Guid.Empty) continue;
                    var neighborland = listGeoLand.Find(cd => cd.ID == landID);
                    if (neighborland == null)
                    {
                        neighborland = landStation.Get(landID);
                    }

                    if (neighborland != null && neighborland.OwnerName.IsNotNullOrEmpty())
                    {
                        if (neighborPersonInfos.IsNullOrEmpty())
                        {
                            neighborPersonInfos = neighborland.OwnerName;
                        }
                        else
                        {
                            if (neighborPersonInfos.Contains(neighborland.OwnerName) == false)
                            {
                                neighborPersonInfos += "," + neighborland.OwnerName;
                            }
                        }
                    }
                }
            }

            return neighborPersonInfos;
        }

        /// <summary>
        /// 获取界址线描述信息
        /// </summary>
        /// <param name="startDotNum">起界址点号</param>
        /// <param name="endDotNum"></param>
        /// <param name="infostr1">返回信息第一个，如A B C D</param>
        /// <param name="infostr2">返回信息第二个，如183.3</param>
        /// <param name="landDots"></param>
        /// <returns></returns>
        private string GetDescription(string startDotNum, string endDotNum, string infostr1, string infostr2, List<BuildLandBoundaryAddressDot> landDots)
        {
            string retrurninfo = "";
            var lengthinfo = infostr2.GetBetween("'", "'");
            double lengthdouble = 0.0;
            double.TryParse(lengthinfo, out lengthdouble);
            lengthdouble = ToolMath.CutNumericFormat(lengthdouble, 2);
            var lengthdoubleStr = lengthdouble.ToString();
            if (metaArgument.InstallArg.LineDescription == EnumDescription.LineLength)
            {
                retrurninfo = lengthdoubleStr;
            }
            else if (metaArgument.InstallArg.LineDescription == EnumDescription.LineFind)
            {
                var trendinfo = infostr1.GetBetween("'", "'");
                var locationname = GetTrendName(trendinfo);
                if (metaArgument.InstallArg.IsUnit)
                {
                    var startDot = landDots.Find(ld => ld.DotNumber == startDotNum);
                    var startDotUnitNumber = startDot != null ? startDot.UniteDotNumber : "";

                    var endDot = landDots.Find(ld => ld.DotNumber == endDotNum);
                    var endDotUnitNumber = endDot != null ? endDot.UniteDotNumber : "";

                    retrurninfo = startDotUnitNumber + "沿" + locationname + "方" + lengthdoubleStr + "米到" + endDotUnitNumber;
                }
                else
                {
                    retrurninfo = startDotNum + "沿" + locationname + "方" + lengthdoubleStr + "米到" + endDotNum;
                }
            }

            return retrurninfo;
        }

        private string GetTrendName(string info)
        {
            string returnInfo = "";
            switch (info)
            {
                case "A":
                    returnInfo = "东";
                    break;
                case "B":
                    returnInfo = "南";
                    break;
                case "C":
                    returnInfo = "西";
                    break;
                case "D":
                    returnInfo = "北";
                    break;
                case "E":
                    returnInfo = "东南";
                    break;
                case "F":
                    returnInfo = "东北";
                    break;
                case "G":
                    returnInfo = "西南";
                    break;
                case "H":
                    returnInfo = "西北";
                    break;
                default:
                    break;
            }
            return returnInfo;
        }

        /// <summary>
        /// 界址线位置
        /// </summary>      
        private string GetLocationName(string info)
        {
            string returnInfo = "";
            info = info.GetBetween("'", "'");
            switch (info)
            {
                case "M":
                    returnInfo = coilLocation.Find(c => c.Name == "中").Code;
                    break;
                case "O":
                    returnInfo = coilLocation.Find(c => c.Name == "外").Code;
                    break;
                case "I":
                    returnInfo = coilLocation.Find(c => c.Name == "内").Code;
                    break;
                default:
                    break;
            }
            return returnInfo;

        }

        #region 辅助方法

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
