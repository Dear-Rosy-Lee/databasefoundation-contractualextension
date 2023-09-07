/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;
using YuLinTu.Data.Shapefile;
using YuLinTu.Data.Dynamic;
using System.Collections;
using YuLinTu.Spatial;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using YuLinTu.NetAux;


namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 初始化界址点线信息任务
    /// </summary>
    public class InitializeLandCoil : Task
    {
        #region Field

        /// <summary>
        /// 线段平移的步长，主要用来判断移动的方向
        /// </summary>
        //private double step = 0.0001;//平方米
        private CollectivityTissue sender;//当前地域的发包方
        private string descripToVillage;//到村的描述
        #endregion

        #region Methods

        /// <summary>
        /// 初始化承包地块界址点线
        /// </summary>
        public void ContractLandInitialTool()
        {
            TaskInitializeLandCoilArgument meta = Argument as TaskInitializeLandCoilArgument;
            if (meta.CurrentZone == null)
            {
                this.ReportError("未选择初始化数据的地域!");
                return;
            }
            string markDesc = GetMarkDesc(meta.CurrentZone, meta.Database);
            var dbContext = meta.Database;
            var targetSpatialReference = dbContext.CreateSchema().GetElementSpatialReference(
               ObjectContext.Create(typeof(Zone)).Schema,
               ObjectContext.Create(typeof(Zone)).TableName);
            List<Dictionary> allListDict = new List<Dictionary>();  //字典内容
            List<Dictionary> dictsJXXZ = new List<Dictionary>();   //界线性质
            List<Dictionary> dictsJZXLB = new List<Dictionary>();  //界址线类别
            List<Dictionary> dictsJZXWZ = new List<Dictionary>();  //界址线位置
            List<Dictionary> dictsTDQSLX = new List<Dictionary>();  //土地权属类型
            List<Dictionary> dictsJBLX = new List<Dictionary>();   //界标类型
            List<Dictionary> dictsJZDLX = new List<Dictionary>();  //界址点类型
            List<ContractLand> landsOfStatus = new List<ContractLand>();

            try
            {
                var dictStation = dbContext.CreateDictWorkStation();
                var landStation = dbContext.CreateContractLandWorkstation();
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();

                List<BuildLandBoundaryAddressDot> currentZoneAlldots = dotStation.GetByZoneCode(meta.CurrentZone.FullCode, eLevelOption.Self);
                if (currentZoneAlldots == null || currentZoneAlldots.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format(string.Format("{0}下没有获取界址点", markDesc)));
                    return;
                }
                List<BuildLandBoundaryAddressDot> currentZoneUseabledots = currentZoneAlldots.FindAll(d => d.IsValid == true);
                if (currentZoneUseabledots == null || currentZoneUseabledots.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format(string.Format("{0}下无有效界址点", markDesc)));
                    return;
                }

                allListDict = dictStation.Get();
                dictsJXXZ = allListDict.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.JXXZ);
                dictsJZXLB = allListDict.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.JZXLB);
                dictsJZXWZ = allListDict.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.JZXWZ);
                dictsTDQSLX = allListDict.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.TDQSLX);
                dictsJBLX = allListDict.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.JBLX);
                dictsJZDLX = allListDict.FindAll(c => !string.IsNullOrEmpty(c.GroupCode) && c.GroupCode == DictionaryTypeInfo.JZDLX);
                sender = dbContext.CreateSenderWorkStation().Get(meta.CurrentZone.ID);
                if (sender == null)
                {
                    sender = dbContext.CreateSenderWorkStation().GetTissues(meta.CurrentZone.FullCode, eLevelOption.Self).FirstOrDefault();
                }
                descripToVillage = GetVillageLevelDesc(meta.CurrentZone, meta.Database);

                if (meta.IsSingleLand)
                {
                    landsOfStatus.Add(meta.SingleLand);
                }
                else
                {
                    Guid[] idsArray = null;
                    if (meta.IsSelectedLands)
                    {
                        List<Guid> ids = new List<Guid>(meta.SelectedObligees.Count);
                        meta.SelectedObligees.RemoveAll(c => c.Status == eVirtualPersonStatus.Lock); //移除锁定承包方
                        meta.SelectedObligees.ForEach(c => ids.Add(c.ID));
                        idsArray = ids.ToArray();
                    }
                    var allLands = landStation.GetCollection(meta.CurrentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.Self);
                    landsOfStatus = GetInitialLandData(meta,
                            () => { return allLands; },
                            () => { return landStation.GetLandsWithoutSiteInfoByZone(meta.CurrentZone.FullCode, eVirtualPersonStatus.Right, eLevelOption.Self); },
                            () => { return landStation.GetLandsByObligeeIds(idsArray); });

                    if (landsOfStatus == null || landsOfStatus.Count == 0)
                    {
                        this.ReportProgress(100, null);
                        this.ReportWarn(string.Format(string.Format("{0}下没有获取未锁定待初始化承包地块", markDesc)));
                        return;
                    }
                }
                var shapeLandsOfStatus = landsOfStatus.FindAll(c => c.Shape != null);
                if (shapeLandsOfStatus == null || shapeLandsOfStatus.Count == 0)
                {
                    this.ReportProgress(100, null);
                    this.ReportWarn(string.Format("没有获取未锁定待初始化空间地块"));
                    return;
                }
                dbContext.BeginTransaction();

                this.ReportProgress(0, "开始");

                if (meta.IsAllLands)
                {
                    int coilDelCount = coilStation.DeleteByZoneCode(meta.CurrentZone.FullCode, eLevelOption.Self, eVirtualPersonStatus.Right);
                }
                else if (meta.IsSelectedLands || meta.IsSingleLand)
                {
                    List<Guid> landIds = new List<Guid>(shapeLandsOfStatus.Count);
                    shapeLandsOfStatus.ForEach(c => landIds.Add(c.ID));
                    int coilDelCount = coilStation.DeleteByLandIds(landIds.ToArray());
                }

                this.ReportProgress(1, "删除已有未锁定界址数据");

                //当前地块的界址点和界址线
                List<BuildLandBoundaryAddressCoil> currentLandCoils = new List<BuildLandBoundaryAddressCoil>();

                var build = new BuidCoil(shapeLandsOfStatus, meta.OrderCoordsKv, meta.IsSingleLand);

                build.Build(i =>
                {
                    this.ReportProgress(i, "正在生成...");
                }, meta.AddressLinedbiDistance, BuidCoil.FilterDotType.noFilter, meta.MinAngleFileter != null ? meta.MinAngleFileter.Value : 160, meta.MaxAngleFilter != null ? meta.MaxAngleFilter.Value : 200);
                var lstJzx = build.GetCoilList();

                int nOldPercent = 0;
                int indexShow = 0;
                List<GeoAPI.Geometries.Coordinate> geoapijzds;

                List<Geometry> endJzxs = new List<Geometry>();//最终的界址线

                //循环每个地//每组界址点排序
                foreach (var landitem in shapeLandsOfStatus)
                {
                    int nPercent = (int)(indexShow * 100.0 / shapeLandsOfStatus.Count);
                    if (nPercent != nOldPercent)
                    {
                        this.ReportProgress(nPercent, "正在写入界址线");
                        nOldPercent = nPercent;
                    }
                    indexShow++;

                    var currentlanduseablejzd = currentZoneUseabledots.FindAll(jzd => jzd.LandID == landitem.ID);
                    if (currentlanduseablejzd != null && currentlanduseablejzd.Count <= 2)
                    {
                        this.ReportWarn(string.Format("地块：{0},没有有效界址点信息"), landitem.LandNumber);
                        continue;
                    }
                    var currentlandjzds = currentZoneAlldots.FindAll(jzd => jzd.LandID == landitem.ID);
                    geoapijzds = new List<GeoAPI.Geometries.Coordinate>();
                    foreach (var jzditem in currentlandjzds)
                    {
                        var geojzd = jzditem.Shape.Instance.Coordinate;
                        if (geojzd != null)
                        {
                            geoapijzds.Add(geojzd);
                        }
                    }

                    bool isCCW = false;
                    bool fClosed = CglHelper.IsSame2(geoapijzds[0], geoapijzds[geoapijzds.Count - 1], 0.05 * 0.05);
                    //按西北角排序，并按顺时针方向组织
                    var coords1 = GeometryHelper.SortCoordsByWNOrder(geoapijzds.ToArray(), fClosed, out isCCW);
                    if (isCCW)
                    {
                        for (int i = coords1.Length - 1, j = 0; i >= 0; --i, ++j)
                        {
                            geoapijzds[j] = coords1[i];
                        }
                        isCCW = false;
                    }
                    else
                    {
                        for (int i = 0; i < coords1.Length; ++i)
                        {
                            geoapijzds[i] = coords1[i];
                        }
                    }
                    //寻找开头有效的界址点序号
                    int startIndex = 0;
                    foreach (var jzditem in geoapijzds)
                    {
                        var jzd = currentlandjzds.Find(d => d.Shape.Instance.Coordinate.X == jzditem.X && d.Shape.Instance.Coordinate.Y == jzditem.Y);

                        if (jzd != null && jzd.IsValid == true)
                        {
                            break;
                        }
                        startIndex++;
                    }
                    if (startIndex == geoapijzds.Count) continue;//最后点都没有有效
                    //以有效界址点开头，有效界址点结尾
                    if (startIndex == 0)
                    {
                        geoapijzds.Add(geoapijzds[0]);
                    }
                    else
                    {
                        List<GeoAPI.Geometries.Coordinate> geoapiJzds = geoapijzds.GetRange(0, startIndex + 1);
                        geoapijzds.RemoveRange(0, startIndex);
                        geoapijzds.AddRange(geoapiJzds);
                    }
                    List<Coordinate> lineGeoList = new List<Coordinate>();
                    bool hasStartvalidjzd = false;
                    int jzxsxh = 1;
                    BuildLandBoundaryAddressDot startJzd = null;
                    BuildLandBoundaryAddressDot endJzd = null;
                    foreach (var jzditem in geoapijzds)
                    {
                        var jzd = currentlandjzds.Find(d => d.Shape.Instance.Coordinate.X.ToString("0.00") == jzditem.X.ToString("0.00")
                                                         && d.Shape.Instance.Coordinate.Y.ToString("0.000") == jzditem.Y.ToString("0.000"));
                        if (jzd == null) continue;

                        if (jzd.IsValid == true && hasStartvalidjzd == false)
                        {
                            lineGeoList.Add(new Coordinate(jzditem.X, jzditem.Y));
                            startJzd = jzd;
                            hasStartvalidjzd = true;
                        }
                        else if (jzd.IsValid == false && hasStartvalidjzd)
                        {
                            lineGeoList.Add(new Coordinate(jzditem.X, jzditem.Y));
                        }
                        else if (jzd.IsValid && hasStartvalidjzd)
                        {
                            lineGeoList.Add(new Coordinate(jzditem.X, jzditem.Y));
                            var coiladdgeo = Geometry.CreatePolyline(lineGeoList);
                            coiladdgeo = YuLinTu.Spatial.Geometry.FromInstance(coiladdgeo.Instance);
                            coiladdgeo.SpatialReference = targetSpatialReference;
                            lineGeoList.Clear();
                            endJzd = jzd;

                            lineGeoList.Add(new Coordinate(jzditem.X, jzditem.Y));

                            var currentlandjzx = lstJzx.Find(jzx => jzx.zJzdCoord.X.ToString("0.00") == jzditem.X.ToString("0.00") && jzx.zJzdCoord.Y.ToString("0.00") == jzditem.Y.ToString("0.00"));
                            ContractLand jzdjzxland = null;
                            ContractLand jzdjzxneighborland = null;
                            if (currentlandjzx != null)
                            {
                                jzdjzxland = currentlandjzx.land;
                                jzdjzxneighborland = currentlandjzx.neighborLand;
                            }
                            //赋值属性
                            var initializeCoil = GetInitializeCoil(meta, dictsJXXZ, dictsJZXLB, dictsJZXWZ, dictsTDQSLX,
                               jzdjzxland, jzdjzxneighborland, dbContext, coiladdgeo, targetSpatialReference, jzxsxh);
                            jzxsxh++;
                            initializeCoil.StartPointID = startJzd.ID;//.StartPointID;
                            initializeCoil.EndPointID = endJzd.ID;//.EndPointID;                   
                            initializeCoil.StartNumber = startJzd.DotNumber.ToString();
                            initializeCoil.EndNumber = endJzd.DotNumber.ToString();
                            currentLandCoils.Add(initializeCoil);
                            startJzd = jzd;
                        }
                    }
                }

                this.ReportProgress(90, "初始化完成，保存中..");

                int coilUpCount = coilStation.AddRange(currentLandCoils);
                currentLandCoils.Clear();
                this.ReportInfomation(string.Format("{0}成功初始化{1}个空间地块界址信息", markDesc, shapeLandsOfStatus.Count));
                dbContext.CommitTransaction();
                this.ReportProgress(100, "完成");
            }
            catch (Exception ex)
            {
                dbContext.RollbackTransaction();
                this.ReportError(string.Format("初始化界址点线数据失败,原因是:{0}", ex.Message));
                YuLinTu.Library.Log.Log.WriteError(this, "ContractLandInitialTool(提交初始化数据)", ex.Message + ex.StackTrace);
            }
            finally
            {
                allListDict = null;
                landsOfStatus = null;
                dictsJZXLB = null;
                dictsJZXWZ = null;
                dictsTDQSLX = null;
                dictsJBLX = null;
                dictsJZDLX = null;
            }
        }

        /// <summary>
        /// 根据条件获取初始化地块数据
        /// </summary>
        private List<ContractLand> GetInitialLandData(TaskInitializeLandCoilArgument argument, Func<List<ContractLand>> getAllLands,
            Func<List<ContractLand>> getLandsWithoutInfo, Func<List<ContractLand>> getSelectedLands)
        {
            List<ContractLand> listLands = getAllLands();
            if (argument.IsLandsWithoutInfo)
                listLands = getLandsWithoutInfo();
            else if (argument.IsSelectedLands)
                listLands = getSelectedLands();
            return listLands;
        }

        #endregion

        #region Methods—初始界址信息辅助方法

        /// <summary>
        /// 获取初始化后的界址线数据-新
        /// </summary>
        private BuildLandBoundaryAddressCoil GetInitializeCoil(TaskInitializeLandCoilArgument meta, List<Dictionary> dictsJXXZ, List<Dictionary> dictsJZXLB, List<Dictionary> dictsJZXWZ,
            List<Dictionary> dictsTDQSLX, ContractLand geoLand, ContractLand neighborLand, IDbContext dbContext, Geometry getCoilShape, SpatialReference targetSpatialReference, int i)
        {
            BuildLandBoundaryAddressCoil coiladd = new BuildLandBoundaryAddressCoil();
            if (meta == null || geoLand == null)
                return coiladd;

            var dictJXXZ = dictsJXXZ.Find(c => c.Name == meta.AddressLineType);
            coiladd.LineType = dictJXXZ == null ? "" : dictJXXZ.Code;

            var dictJZXLB = dictsJZXLB.Find(c => c.Name == meta.AddressLineCatalog);
            coiladd.CoilType = dictJZXLB == null ? "" : dictJZXLB.Code;

            var dictJZXWZ = dictsJZXWZ.Find(c => c.Name == meta.AddressLinePosition);
            coiladd.Position = dictJZXWZ == null ? "" : dictJZXWZ.Code;

            var dictTDQSLX = dictsTDQSLX.Find(c => c.Code == DictionaryTypeInfo.JTNYD);
            coiladd.LandType = dictTDQSLX == null ? "" : dictTDQSLX.Code;

            coiladd.LandID = geoLand.ID;
            coiladd.LandNumber = geoLand.LandNumber;
            coiladd.ZoneCode = geoLand.LocationCode;
            coiladd.OrderID = (short)(i);

            double lineLength = ToolMath.CutNumericFormat(getCoilShape.Length(), 2);
            coiladd.Shape = getCoilShape;
            coiladd.CoilLength = lineLength;
            coiladd.LandNumber = geoLand.LandNumber;
            //将当前的长度赋值给说明
            if (meta.IsLineDescription)
            {
                coiladd.Description = lineLength.ToString();
            }

            if (meta.IsPostion)
            {
                if (neighborLand != null)
                {
                    coiladd.Position = ((int)eBoundaryLinePosition.Middle).ToString();
                }
                else
                {
                    coiladd.Position = ((int)eBoundaryLinePosition.Right).ToString();
                }
                if (neighborLand != null && geoLand.LandExpand != null && neighborLand.LandExpand != null && geoLand.LandExpand.Elevation != null)
                {
                    if (geoLand.LandExpand.Elevation.Value != 0 && neighborLand.LandExpand.Elevation > geoLand.LandExpand.Elevation.Value)
                    {
                        coiladd.Position = ((int)eBoundaryLinePosition.Left).ToString();
                    }
                    else if (geoLand.LandExpand.Elevation.Value != 0 && neighborLand.LandExpand.Elevation < geoLand.LandExpand.Elevation.Value)
                    {
                        coiladd.Position = ((int)eBoundaryLinePosition.Right).ToString();
                    }
                }
            }
            if (meta.IsNeighbor)
            {
                if (neighborLand != null)
                {
                    //如果相交的地块有多个，取第一个来赋值
                    coiladd.NeighborPerson = neighborLand.OwnerName;
                    coiladd.NeighborFefer = neighborLand.OwnerName;
                }
                else
                {
                    //如果相交的地块没有，则将当前地块的承包方和法人赋值  
                    coiladd.NeighborPerson = sender != null ? sender.Name : "";
                    if (meta.IsNeighborExportVillageLevel)
                    {
                        coiladd.NeighborPerson = descripToVillage;
                    }
                    coiladd.NeighborFefer = geoLand.OwnerName;
                }
            }
            return coiladd;
        }

        #endregion

        #region Methods—任务辅助方法

        /// <summary>
        /// 根据当前地域获得任务描述信息
        /// </summary>
        private string GetMarkDesc(Zone zone, IDbContext Database)
        {
            Zone parent = GetParent(zone, Database);  //获取上级地域
            string excelName = string.Empty;
            if (zone.Level == eZoneLevel.Town)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                excelName = parent.Name + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Group)
            {
                Zone parentTowm = GetParent(parent, Database);
                excelName = parentTowm.Name + parent.Name + zone.Name;
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

        /// <summary>
        /// 查找，到村的描述
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="Database"></param>
        /// <returns></returns>
        private string GetVillageLevelDesc(Zone zone, IDbContext Database)
        {
            Zone parent = GetParent(zone, Database);  //获取上级地域
            string excelName = string.Empty;
            if (zone.Level == eZoneLevel.Town)
            {
                excelName = zone.Name;
            }
            else if (zone.Level == eZoneLevel.Village)
            {
                excelName = parent.Name + zone.Name;
            }
            else if (zone.Level == eZoneLevel.Group)
            {
                Zone parentTowm = GetParent(parent, Database);
                excelName = parentTowm.Name + parent.Name;
            }
            return excelName;
        }

        #endregion
    }

    #region yxm add 2016-6-13
    /// <summary>
    /// 根据地块集合生成界线数据
    /// </summary>
    class BuidCoil
    {
        /// <summary>
        /// 过滤类型
        /// 根据需求需改于2016.7.15 修改人:陈明
        /// </summary>
        public enum FilterDotType
        {
            /// <summary>
            /// 不过滤
            /// </summary>
            noFilter,
            /// <summary>
            /// 按角度过滤
            /// </summary>
            filterByAngle,
            /// <summary>
            /// 按承包方过滤
            /// </summary>
            //filterByQlr,
        }
        /// <summary>
        /// 界址线
        /// </summary>
        public class Jzx
        {
            /// <summary>
            /// 起点坐标
            /// </summary>
            public GeoAPI.Geometries.Coordinate qJzdCoord;
            /// <summary>
            /// 止点坐标
            /// </summary>
            public GeoAPI.Geometries.Coordinate zJzdCoord;
            /// <summary>
            /// 界址线顺序号
            /// </summary>
            public int jzxsxh;

            /// <summary>
            /// 起界址点
            /// </summary>
            public Jzd qJzd;
            /// <summary>
            /// 止界址点
            /// </summary>
            public Jzd zJzd;
            ///// <summary>
            ///// 其界址点ID
            ///// </summary>
            //public Guid StartPointID;
            ///// <summary>
            ///// 止界址点ID
            ///// </summary>
            //public Guid EndPointID;
            /// <summary>
            /// 是否逆时针方向（用于确定向外缓冲的方向）；
            /// </summary>
            internal bool isCCW;
            /// <summary>
            /// 对应地块
            /// </summary>
            public ContractLand land;

            /// <summary>
            /// 毗邻地
            /// </summary>
            public ContractLand neighborLand;
            ///// <summary>
            ///// 毗邻地块集合
            ///// </summary>
            //public HashSet<ContractLand> lstNeighborLands = new HashSet<ContractLand>();
            //public string GetQlr()
            //{
            //    return null;
            //    //string s = "";
            //    //foreach (var q in lstQlr)
            //    //{
            //    //    if (!string.IsNullOrEmpty(s))
            //    //        s += ";";
            //    //    s += q;
            //    //}
            //    //return s;
            //}
            internal GeoAPI.Geometries.ILineString Buffer(double distance)
            {
                if (isCCW)
                {
                    return CglHelper.OffsetRight(qJzdCoord, zJzdCoord, distance);
                }
                else
                {
                    return CglHelper.OffsetLeft(qJzdCoord, zJzdCoord, distance);
                }
            }
            /// <summary>
            /// 交换起止点方向
            /// </summary>
            private void swap()
            {
                var t = qJzdCoord;
                qJzdCoord = zJzdCoord;
                zJzdCoord = t;
                isCCW = !isCCW;
                var t1 = qJzd;// StartPointID;
                qJzd = zJzd;
                zJzd = t1;
            }
            /// <summary>
            /// 按顺时针排序
            /// </summary>
            internal void makeCW()
            {
                if (isCCW)
                {
                    swap();
                }
            }
        }
        public class Jzd
        {
            public GeoAPI.Geometries.Coordinate coord;
            /// <summary>
            /// 是否关键界址点
            /// </summary>
            public bool sfky = false;
            public ContractLand land;
            /// <summary>
            /// 界址点号
            /// </summary>
            public int jzdh;
            /// <summary>
            /// 界址点ID
            /// </summary>
            public Guid ID;
        }
        class JzxList
        {
            private readonly BuidCoil _p;
            internal readonly List<Jzx> _lst = new List<Jzx>();
            public JzxList(BuidCoil p)
            {
                _p = p;
            }
            public int Count
            {
                get { return _lst.Count; }
            }
            public Jzx get(int i)
            {
                return _lst[i];
            }
            public void Add(Jzx jzx)
            {
                var q = GetFromCoord(jzx);
                var z = GetToCoord(jzx);
                if (q.X > z.X || q.X == z.X && q.Y > z.Y)
                {//x方向排序，确保jzx.qJzd的x坐标<=jzx.zJzd
                    var t = jzx.qJzdCoord;
                    jzx.qJzdCoord = jzx.zJzdCoord;
                    jzx.zJzdCoord = t;
                    jzx.isCCW = !jzx.isCCW;
                }
                _lst.Add(jzx);
            }
            public IEnumerable<Jzx> Enum
            {
                get
                {
                    return _lst;
                }
            }
            public void Clear()
            {
                _lst.Clear();
            }
            public void Sort()
            {
                _lst.Sort((a, b) =>
                {
                    var ac = GetFromCoord(a);
                    var bc = GetFromCoord(b);
                    if (ac.X < bc.X)
                        return -1;
                    if (ac.X > bc.X)
                        return 1;
                    return ac.Y < bc.Y ? -1 : 1;
                });
            }
            public GeoAPI.Geometries.Coordinate GetFromCoord(Jzx jzx)
            {
                return jzx.qJzdCoord;// _p.GetJzdCoord(jzx.qJzd);
            }
            public GeoAPI.Geometries.Coordinate GetToCoord(Jzx jzx)
            {
                return jzx.zJzdCoord;// _p.GetJzdCoord(jzx.zJzd);// _p.lstJzd[jzx.zJzd];
            }
        }
        /// <summary>
        /// 所有界址点（地块节点）坐标的集合
        /// </summary>
        //private readonly List<GeoAPI.Geometries.Coordinate> _lstVertex = new List<GeoAPI.Geometries.Coordinate>();

        private readonly List<ContractLand> _lstLands = new List<ContractLand>();
        private readonly JzxList _lstJzx;
        private List<Jzd> _lstJzd;
        public BuidCoil(List<ContractLand> lstLands, KeyValue<bool, List<GeoAPI.Geometries.Coordinate>> orderCoordsKv, bool isSingleLand = true)
        {
            _lstJzx = new JzxList(this);
            //lstLands.Sort((a, b) =>
            //{
            //    var ae=a.Shape.Instance.EnvelopeInternal;
            //    var be = b.Shape.Instance.EnvelopeInternal;
            //    return ae.MinX > be.MinX ? -1 : 1;
            //});
            foreach (var land in lstLands)
            {
                if (land.Shape == null)
                    continue;
                _lstLands.Add(land);

                if (!isSingleLand)
                {
                    orderCoordsKv = land.Shape.GetWNOrderCoordinates();
                    orderCoordsKv.Value = orderCoordsKv.Value.Distinct().ToList();
                }
                var coords = orderCoordsKv.Value.ToArray();
                GeoAPI.Geometries.Coordinate firstCoord = null;
                int jzxsxh = 0;
                for (int i = 0; i < coords.Length; ++i)
                {
                    var jzd = coords[i];

                    if (i == 0)
                    {
                        firstCoord = jzd;
                    }
                    else
                    {
                        var jzx = new Jzx
                        {
                            qJzdCoord = coords[i - 1],
                            zJzdCoord = jzd,
                        };
                        jzx.jzxsxh = ++jzxsxh;
                        jzx.isCCW = orderCoordsKv.Key;
                        jzx.land = land;
                        _lstJzx.Add(jzx);
                    }
                }
                var endJzx = new Jzx
                {
                    qJzdCoord = coords[coords.Length - 1],
                    zJzdCoord = firstCoord,
                    jzxsxh = jzxsxh + 1,
                    isCCW = orderCoordsKv.Key,
                    land = land,
                };
                _lstJzx.Add(endJzx);

                //int jzxsxh = 0;
                //var g = land.Shape.Instance;
                //if (g is NetTopologySuite.Geometries.Polygon)
                //{
                //    parsePoints(g as NetTopologySuite.Geometries.Polygon, land, ref jzxsxh);
                //}
                //else if (g is NetTopologySuite.Geometries.MultiPolygon)
                //{
                //    var mg = g as NetTopologySuite.Geometries.MultiPolygon;
                //    foreach (var g1 in mg.Geometries)
                //    {
                //        parsePoints(g1 as NetTopologySuite.Geometries.Polygon, land, ref jzxsxh);
                //    }
                //}
            }
            _lstJzx.Sort();
            _lstLands.Sort((a, b) =>
            {
                var ae = a.Shape.Instance.EnvelopeInternal;
                var be = b.Shape.Instance.EnvelopeInternal;
                return ae.MinX > be.MinX ? -1 : 1;
            });
        }
        public void Build(Action<int> progress, double distance
            , FilterDotType filterType, double fromAngle, double toAngle)
        {
            var lstDKEnvelope = _lstLands;
            var lstJzx = _lstJzx;
            var lstNeighborLands = new HashSet<ContractLand>();
            int oldProgress = 0;
            for (int i = 0; i < lstJzx.Count; ++i)
            {
                int nPercent = (int)(i * 100.0 / lstJzx.Count);
                if (nPercent != oldProgress)
                {
                    progress(nPercent);
                    oldProgress = nPercent;
                }
                //progress("正在生成毗邻地承包方" + (i * 100 / lstJzx.Count) + "%");
                var jzx = lstJzx.get(i);
                var offsetLine = jzx.Buffer(distance);
                var env = offsetLine.EnvelopeInternal;
                for (int j = lstDKEnvelope.Count - 1; j >= 0; --j)
                {
                    var land = lstDKEnvelope[j];
                    var g = land.Shape.Instance;
                    var de = g.EnvelopeInternal;
                    if (env.MaxX < de.MinX)
                        break;
                    if (env.MinX - distance > de.MaxX)
                    {
                        lstDKEnvelope.RemoveAt(j);
                        continue;
                    }
                    if (env.MinX > de.MaxX || env.MinY > de.MaxY || env.MaxY < de.MinY)
                        continue;
                    if (land != jzx.land)
                    {
                        if (g.Intersects(offsetLine))
                        {
                            lstNeighborLands.Add(land);
                        }
                    }
                }
                ContractLand neighborLand = null;
                if (lstNeighborLands.Count == 1)
                {
                    neighborLand = lstNeighborLands.First();
                }
                else
                {
                    double maxLen = 0;
                    foreach (var nb in lstNeighborLands)
                    {
                        var g = nb.Shape.Instance;
                        var il = offsetLine.Intersection(g);
                        if (neighborLand == null)
                        {
                            neighborLand = nb;
                            maxLen = il.Length;
                        }
                        else if (maxLen < il.Length)
                        {
                            neighborLand = nb;
                            maxLen = il.Length;
                        }
                    }
                }
                jzx.neighborLand = neighborLand;
                //if(lstNeighborLands.Count==1){
                //    jzx.neighborLand=lstNeighborLands.f[0];
                //}
                lstNeighborLands.Clear();
            }
            _lstLands.Clear();
            _lstJzx._lst.Sort((a, b) =>
            {
                int ai = a.land.GetHashCode();
                int bi = b.land.GetHashCode();
                if (ai == bi)
                {
                    return a.jzxsxh < b.jzxsxh ? -1 : 1;
                }
                return ai < bi ? -1 : 1;
            });
            _lstJzd = GetJzdList(filterType, fromAngle, toAngle);

        }
        /// <summary>
        /// 返回界址线集合
        /// </summary>
        /// <returns></returns>
        public List<Jzx> GetCoilList()
        {
            return _lstJzx._lst;
        }
        /// <summary>
        /// 返回界址点列表
        /// </summary>
        /// <returns></returns>
        public List<Jzd> GetJzdList()
        {
            return _lstJzd;
        }
        /// <summary>
        /// 返回界址点列表
        /// </summary>
        /// <param name="filterType"></param>
        /// <param name="fromAngle"></param>
        /// <param name="toAngle"></param>
        /// <returns></returns>
        private List<Jzd> GetJzdList(FilterDotType filterType, double fromAngle, double toAngle)//,Action<int> progress)
        {
            //int nOldPercent = 0;
            ContractLand land = null;
            var lstJzx = new List<Jzx>();
            var lst = new List<Jzd>();
            for (int i = 0; i < _lstJzx.Count; ++i)
            {
                //int nPercent=(int)(i*100.0/_lstJzx.Count);
                //if (nPercent != nOldPercent)
                //{
                //    progress(nPercent);
                //    nOldPercent = nPercent;
                //}
                var jzx = _lstJzx.get(i);
                if (land != jzx.land)
                {
                    buildLandJzds(lstJzx, lst, filterType, fromAngle, toAngle);
                    lstJzx.Clear();
                }
                land = jzx.land;
                lstJzx.Add(jzx);
            }
            buildLandJzds(lstJzx, lst, filterType, fromAngle, toAngle);
            HandleJzdIsValid(lst);
            lstJzx.Clear();
            return lst;
        }

        private List<Geometry> listOtherLines = new List<Geometry>();
        /// <summary>
        /// 所有地块的线段
        /// </summary>
        public void GetOtherLandLines(List<ContractLand> lands)
        {
            if (lands == null || lands.Count == 0)
                return;
            for (int i = 0; i < lands.Count; i++)
            {
                var land = lands[i];
                var geo = land.Shape;
                if (geo == null)
                    continue;
                var lines = geo.ToSegmentLines();
                if (lines == null || lines.Length == 0)
                    continue;
                listOtherLines.AddRange(lines.ToList());
            }
        }

        /// <summary>
        /// 2016.7.15  新增处理关键界址点方法  
        /// 在根据角度处理关键界址点基础上新增支出界址线处界址点处理为关键界址点
        /// </summary>
        /// <param name="lst">界址点集合</param>
        private void HandleJzdIsValid(List<Jzd> lst)
        {
            double prescion = 0.0000001;
            if (lst == null || lst.Count == 0)
                return;
            if (listOtherLines == null || listOtherLines.Count == 0)
                return;
            var lstNo = lst.FindAll(c => !c.sfky);
            if (lstNo == null || lstNo.Count == 0)
                return;
            for (int i = 0; i < lstNo.Count; i++)
            {
                var jzd = lstNo[i];
                double x = jzd.coord.X;
                double y = jzd.coord.Y;
                var tempLines = listOtherLines.FindAll(c =>
                {
                    var coords = c.ToCoordinates();
                    if (coords != null && coords.Length == 2)
                    {
                        bool isConnection = coords.Any(t => ToolMath.AlmostEquals(t.X, x, prescion) && ToolMath.AlmostEquals(t.Y, y, prescion));
                        if (isConnection)
                            return true;
                    }
                    return false;
                });
                var connectionLines = tempLines.Distinct(c => c.Instance).ToList();
                if (connectionLines.Count > 2)
                    jzd.sfky = true;
            }
        }

        /// <summary>
        /// 生成一块地的界址点
        /// </summary>
        /// <param name="lstJzx"></param>
        /// <param name="lst"></param>
        private void buildLandJzds(List<Jzx> lstJzx, List<Jzd> lst, FilterDotType filterType, double fromAngle, double toAngle)
        {
            if (lstJzx.Count == 0)
                return;
            int nJzdH = 0;
            if (filterType == FilterDotType.filterByAngle)
            {
                Jzx preJzx = null;
                var jzx = lstJzx[lstJzx.Count - 1];
                var preCoord = jzx.isCCW ? jzx.zJzdCoord : jzx.qJzdCoord;
                for (int i = 0; i < lstJzx.Count; ++i)
                {
                    jzx = lstJzx[i];
                    jzx.makeCW();
                    var jzd = new Jzd();
                    jzd.ID = Guid.NewGuid();
                    jzx.qJzd = jzd;
                    jzd.coord = jzx.isCCW ? jzx.zJzdCoord : jzx.qJzdCoord;
                    jzd.land = jzx.land;
                    jzd.jzdh = ++nJzdH;

                    var jzx1 = i == lstJzx.Count - 1 ? lstJzx[0] : lstJzx[i + 1];
                    var nextCoord = jzx1.isCCW ? jzx1.zJzdCoord : jzx1.qJzdCoord;
                    var angle = MathHelper.GetVectorAngle(jzd.coord.X, jzd.coord.Y, preCoord.X, preCoord.Y, nextCoord.X, nextCoord.Y);
                    jzd.sfky = angle >= fromAngle && angle <= toAngle;

                    lst.Add(jzd);

                    preCoord = jzd.coord;

                    if (i > 0)
                    {
                        preJzx.zJzd = jzd;
                    }
                    preJzx = jzx;
                }
                preJzx.zJzd = lstJzx[0].qJzd;
            }
            //else if (filterType == FilterDotType.filterByQlr)
            //{
            //    var preJzx = lstJzx[lstJzx.Count - 1];
            //    for (int i = 0; i < lstJzx.Count; ++i)
            //    {
            //        var jzx = lstJzx[i];
            //        jzx.makeCW();
            //        var jzd = new Jzd();
            //        jzd.ID = Guid.NewGuid();
            //        jzx.qJzd = jzd;
            //        jzd.coord = jzx.isCCW ? jzx.zJzdCoord : jzx.qJzdCoord;
            //        jzd.land = jzx.land;
            //        jzd.jzdh = ++nJzdH;
            //        jzd.sfky = preJzx.land.OwnerName != jzx.land.OwnerName;// preJzx.land != jzx.land;
            //        lst.Add(jzd);
            //        if (i > 0)
            //        {
            //            preJzx.zJzd = jzd;
            //        }
            //        preJzx = jzx;
            //    }
            //    preJzx.zJzd = lstJzx[0].qJzd;
            //}
            else
            {
                Jzx preJzx = null;
                for (int i = 0; i < lstJzx.Count; ++i)
                {
                    var jzx = lstJzx[i];
                    jzx.makeCW();
                    var jzd = new Jzd();
                    jzd.ID = Guid.NewGuid();
                    jzx.qJzd = jzd;
                    jzd.coord = jzx.isCCW ? jzx.zJzdCoord : jzx.qJzdCoord;
                    jzd.land = jzx.land;
                    jzd.jzdh = ++nJzdH;
                    jzd.sfky = true;
                    lst.Add(jzd);
                    if (i > 0)
                    {
                        preJzx.zJzd = jzd;
                    }
                    preJzx = jzx;
                }
                preJzx.zJzd = lstJzx[0].qJzd;
            }
        }
        private void parsePoints(NetTopologySuite.Geometries.Polygon g, ContractLand land, ref int jzxsxh)
        {
            parsePoints(g.Shell.Coordinates, land, ref jzxsxh);
            foreach (var h in g.Holes)
            {
                parsePoints(h.Coordinates, land, ref jzxsxh);
            }
        }

        private void parsePoints(GeoAPI.Geometries.Coordinate[] coords, ContractLand land, ref int jzxsxh)
        {
            bool isCCW = false;
            bool fClosed = CglHelper.IsSame2(coords[0], coords[coords.Length - 1], 0.05 * 0.05);
            #region 按西北角排序，并按顺时针方向组织
            var coords1 = GeometryHelper.SortCoordsByWNOrder(coords, fClosed, out isCCW);
            if (isCCW)
            {
                for (int i = coords1.Length - 1, j = 0; i >= 0; --i, ++j)
                {
                    coords[j] = coords1[i];
                }
                isCCW = false;
            }
            else
            {
                for (int i = 0; i < coords1.Length; ++i)
                {
                    coords[i] = coords1[i];
                }
            }
            #endregion

            GeoAPI.Geometries.Coordinate iJzd0Index = null;
            for (int i = 0; i < coords.Length; ++i)
            {
                var jzd = coords[i];
                /*if (i == coords.Length - 1)
                {
                    var jzx = new Jzx
                    {
                        qJzdCoord = jzd,
                        zJzdCoord = iJzd0Index,
                    };
                    jzx.jzxsxh = ++jzxsxh;
                    jzx.isCCW = isCCW;
                    jzx.land = land;
                    _lstJzx.Add(jzx);
                    return;
                }*/

                if (i == 0)
                {
                    iJzd0Index = jzd;// _lstVertex.Count - 1;
                }
                else
                {
                    var jzx = new Jzx
                    {
                        qJzdCoord = coords[i - 1],// _lstVertex[_lstVertex.Count - 2],// jzd.id - 1,
                        zJzdCoord = jzd,//_lstVertex[_lstVertex.Count - 1],// jzd.id
                    };
                    jzx.jzxsxh = ++jzxsxh;
                    jzx.isCCW = isCCW;
                    jzx.land = land;
                    _lstJzx.Add(jzx);
                }
            }
        }

    }
    #endregion
}
