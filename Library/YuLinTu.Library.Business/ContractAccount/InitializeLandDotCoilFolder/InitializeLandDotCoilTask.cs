/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using YuLinTu.Data;
using YuLinTu.Library.Aux;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.NetAux;
using YuLinTu.Spatial;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 初始化界址点线信息任务
    /// </summary>
    public class InitializeLandDotCoilTask : Task
    {
        #region Field
        public const string POSITIONOUT = "3";
        public const string POSITIONCEN = "2";
        public const string POSITIONIN = "1";
        private EnumDescription _lineDescription;
        private List<Dictionary> _jzxlbdic;
        #endregion

        #region Methods
        /// <summary>
        /// 初始化承包地块界址点线
        /// </summary>
        public void ContractLandInitialTool()
        {
            var meta = Argument as TaskInitializeLandDotCoilArgument;
            if (meta.CurrentZone == null)
            {
                this.ReportError("未选择初始化数据的地域!");
                return;
            }
            InitialLandInitialTool(meta.Database);
            var dotStation = meta.Database.CreateDotWorkStation();
            var coilStation = meta.Database.CreateCoilWorkStation();

            var zoneCode = meta.CurrentZone.FullCode == "86" ? "" : meta.CurrentZone.FullCode;
            var sw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                GetDicsInfo(meta);
                if (meta.IsInstall)
                {
                    ClearDotLine(zoneCode, coilStation, dotStation);
                    InitialDotCoils(meta);
                }
                if (meta.IsValueSet)
                {
                    EvaluateDotCoils(meta, zoneCode, meta.Database, coilStation);
                }
            }
            finally
            {
                sw.Stop();
                this.ReportInfomation(string.Format("共耗时：{0}", sw.Elapsed));
            }
            return;

        }

        public virtual void InitialLandInitialTool(IDbContext dbcontext)
        {

        }

        /// <summary>
        /// 初始化数据
        /// </summary> 
        public virtual void InitialDotCoils(TaskInitializeLandDotCoilArgument meta)
        {
            using (var db = new DBSpatialite())
            {
                var dbFile = meta.Database.DataSource.ConnectionString;
                dbFile = dbFile.Substring(dbFile.IndexOf('=') + 1);
                var jzxdicxmlstrs = GetJZXLBStrXml();
                db.Open(dbFile);//@"C:\myprojects\工单\20160816建库工具\通川区安云乡.sqlite");
                var prms = new InitLandDotCoilParam();
                prms.AddressDotMarkType = SafeConvertAux.SafeConvertToInt16(meta.InstallArg.AddressDotMarkType);
                prms.AddressDotType = SafeConvertAux.SafeConvertToInt16(meta.InstallArg.AddressDotType);
                prms.AddressLineCatalog = meta.InstallArg.AddressLineCatalog;
                prms.AddressLinedbiDistance = meta.InstallArg.AddressLinedbiDistance;
                prms.AddressLinePosition = meta.InstallArg.AddressLinePosition;//没有任何地物，默认为外
                prms.AddressLineType = meta.InstallArg.AddressLineType;
                prms.AddressPointPrefix = meta.InstallArg.AddressPointPrefix;
                prms.jzxdicxmlstrs = jzxdicxmlstrs;
                prms.IsSetAddressLinePosition = meta.InstallArg.IsSetAddressLinePosition;
                prms.IsUnit = meta.InstallArg.IsUnit;
                prms.Jzxlbdics = meta.InstallArg.Jzxlbdics;
                prms.LineDescription = meta.InstallArg.LineDescription;
                prms.MinAngleFileter = meta.InstallArg.MinAngleFileter;
                prms.MaxAngleFilter = meta.InstallArg.MaxAngleFilter;
                prms.MinAngleFileterExtend = 10;
                prms.MaxAngleFilterExtend = 91;
                prms.IsFilter = meta.InstallArg.IsFilterDot;
                prms.UseAddAlgorithm = meta.InstallArg.UseAddAlgorithm;
                var t = new InitLandDotCoil(db, prms);
                t.ReportProgress += (msg, i) =>
                {
                    this.ReportProgress(i, msg);
                };
                t.ReportInfomation += msg =>
                {
                    this.ReportInfomation(msg);
                };
                t.onPresaveJzd += en =>
                {
                    if (meta.InstallArg.IsFilterDot == false)
                    {
                        en.SFKY = true;
                    }
                };
                string wh = null;
                wh = string.Format(Zd_cbdFields.ZLDM + " like '{0}%'", meta.CurrentZone.FullCode);// zldm like '511702208200%'";
                                                                                                  //}
                t.DoInit(wh);
            }
        }

        /// <summary>
        /// 数据属性赋值
        /// </summary>
        public virtual void EvaluateDotCoils(TaskInitializeLandDotCoilArgument meta, string zoneCode,
            IDbContext db, IBuildLandBoundaryAddressCoilWorkStation coilStation)
        {
            var setArg = meta.SetArg;
            if (setArg.InitialJZXInfoUseSN)//使用四至算法更新界址线性质
            {
                this.ReportProgress(0, "开始更新界址线信息");
                InitialJZXInfoUseSN initialJZXInfoUseSN = new InitialJZXInfoUseSN(meta);
                initialJZXInfoUseSN.ProgressChanged += ReportPercent;
                initialJZXInfoUseSN.Alert += ReportInfo;
                initialJZXInfoUseSN.MainHandle();
                this.ReportProgress(100, "完成更新界址线信息");
                //return;
            }
            if (!setArg.IsReplaceLinePerson && !setArg.IsReplaceLineRefer && !setArg.IsSetAddressLinePosition &&
                !setArg.IsSetLineDescription && !setArg.IsSetLinePropery && !setArg.IsSetLineType && !setArg.IsReplaceLinePersonFromMult)
            {
                return;
            }
            try
            {
                List<MuilPolyGonInInfoClass> mpgis = new List<MuilPolyGonInInfoClass>();

                if (setArg.IsReplaceLinePersonFromMult)
                {
                    var landst = db.CreateContractLandWorkstation();
                    var querylands = landst.GetCollection(zoneCode, eLevelOption.Self);

                    querylands.RemoveAll(q => q.Shape == null);

                    foreach (var item in querylands)
                    {
                        var geoGroupCdts = item.Shape.ToGroupCoordinates();
                        if (geoGroupCdts.Count() == 1) continue;

                        double maxArea = 0;
                        Dictionary<Geometry, bool> itemAllPgn = new Dictionary<Geometry, bool>();
                        for (int i = 0; i < geoGroupCdts.Count(); i++)
                        {
                            var geo = Geometry.CreatePolygon(geoGroupCdts[i].ToList());
                            var geoArea = geo.Area();
                            if (geoArea > maxArea)
                            {
                                maxArea = geoArea;
                                if (itemAllPgn.ContainsKey(geo))
                                {
                                    itemAllPgn[geo] = true;
                                }
                                else
                                {
                                    itemAllPgn.Add(geo, true);
                                }
                            }
                            else
                            {
                                if (itemAllPgn.ContainsKey(geo))
                                {
                                    itemAllPgn[geo] = false;
                                }
                                else
                                {
                                    itemAllPgn.Add(geo, false);
                                }
                            }
                        }

                        MuilPolyGonInInfoClass mpgi = new MuilPolyGonInInfoClass();
                        mpgi.ID = item.ID;
                        mpgi.interGeos = new List<Geometry>();

                        foreach (var itedic in itemAllPgn)
                        {
                            if (itedic.Value) continue;
                            mpgi.interGeos.Add(itedic.Key);
                        }

                        mpgis.Add(mpgi);
                    }
                }

                var lbdic = new Dictionary<string, string>();
                foreach (var lb in meta.InstallArg.Jzxlbdics)
                {
                    if (!lbdic.ContainsKey(lb.Code))
                    {
                        lbdic.Add(lb.Code, lb.Name);
                    }
                }
                db.BeginTransaction();
                //由于界址点、界址线数量太多，不能一次性全部取出，因此按照行政地域提取数据
                var querycoil = db.CreateQuery<BuildLandBoundaryAddressCoil>().Where(t => t.ZoneCode.StartsWith(zoneCode));
                var querydot = db.CreateQuery<BuildLandBoundaryAddressDot>();
                var xzqh = db.CreateZoneWorkStation().GetChildren(zoneCode, eLevelOption.SelfAndSubs)
                    .Select(t => t.FullCode).ToList();
                var dotNumberDic = new Dictionary<Guid, string>();
                int JZXCount = querycoil.Count();
                int index = 0;
                foreach (var value in xzqh)
                {
                    dotNumberDic.Clear();
                    if (setArg.IsSetLineDescription && setArg.IsUnit)
                    {
                        var qwhere = querydot.Where(t => t.ZoneCode == value)
                            .Select(s => new { s.ID, s.DotNumber, s.UniteDotNumber });
                        qwhere.ForEach((i, p, en) =>
                        {
                            if (dotNumberDic.ContainsKey(en.ID))
                                return true;
                            if (setArg.IsUnit)
                                dotNumberDic.Add(en.ID, en.UniteDotNumber);
                            else
                                dotNumberDic.Add(en.ID, en.DotNumber);
                            return true;
                        });
                    }

                    //var percent = 100 / (double) querycoil.Count();
                    Aspect a = new Aspect(0);
                    var coilwhere = querycoil.Where(t => t.ZoneCode == value).ToList();
                    //更改界址线相关属性
                    coilwhere.ForEach(en =>
                    {
                        index++;
                        this.ReportProgress((int)(100 * index / JZXCount), "修改界址属性信息");
                        if (setArg.IsReplaceLinePerson)
                        {
                            SetAddressCoilPerson(en, setArg);
                        }

                        if (setArg.IsReplaceLineRefer)
                        {
                            SetAddressCoilRefer(en, setArg);
                        }

                        if (setArg.IsReplaceLinePersonFromMult)
                        {
                            var land = mpgis.Find(qf => qf.ID == en.LandID);
                            if (land != null && land.interGeos.Count > 0)
                            {
                                var interlands = land.interGeos.FindAll(d => d.Intersects(en.Shape));
                                if (interlands.Count > 0)
                                {
                                    en.NeighborPerson = setArg.ReplaceLinePersonFromMultTo;
                                    en.NeighborFefer = setArg.ReplaceLinePersonFromMultTo;
                                }
                            }
                        }

                        if (setArg.IsSetAddressLinePosition)
                        {
                            if (en.Position == "3")
                            {
                                en.Position = "1";
                            }
                        }

                        if (setArg.IsSetLineDescription)
                        {
                            SetAddressCoilDescription(en, a, setArg, dotNumberDic, lbdic);
                        }

                        if (setArg.IsSetLinePropery)
                        {
                            en.LineType = setArg.SetAddressLineType;
                        }

                        if (setArg.IsSetLineType)
                        {
                            en.CoilType = setArg.SetAddressLineCatalog;
                        }

                        coilStation.Update(en);
                    });
                    this.ReportInfomation($"成功修改区域{value}的界址属性{coilwhere.Count}条数据");
                }

                db.CommitTransaction();
                this.ReportInfomation($"合计修改界址数据{JZXCount}条");
                this.ReportProgress(100, "完成修改界址属性信息");
            }
            catch (Exception ex)
            {
                this.ReportError("更新界址数据属性出错:" + ex.Message);
            }
            finally
            {
                db.RollbackTransaction();
            }
        }

        /// <summary>
        /// 设置界址线说明
        /// </summary>
        /// <param name="en"></param>
        public virtual void SetAddressCoilDescription(BuildLandBoundaryAddressCoil en, Aspect a, SetDotCoilArg setArg,
            Dictionary<Guid, string> dotNumberDic, Dictionary<string, string> lbdic)
        {
            if (en.Shape != null)
            {
                var jszsm = "";
                if (setArg.SetLineDescription == EnumDescription.LineLength)
                {
                    jszsm = ToolMath.RoundNumericFormat(en.Shape.Length(), 2).ToString();
                }
                else
                {
                    var coords = en.Shape.ToCoordinates();
                    var p0 = coords[0];
                    var p1 = coords[coords.Count() - 1];
                    a.Assign(p0.X, p0.Y, p1.X, p1.Y);
                    var qjzdh = en.StartNumber;
                    var zjzdh = en.EndNumber;
                    if (setArg.IsUnit)
                    {
                        dotNumberDic.TryGetValue(en.StartPointID, out qjzdh);
                        dotNumberDic.TryGetValue(en.EndPointID, out zjzdh);
                    }
                    if (setArg.SetLineDescription == EnumDescription.LineFind)
                    {
                        jszsm = string.Format("{0}沿{1}方{2}米到{3}", qjzdh, a.toAzimuthString(), ToolMath.RoundNumericFormat(en.Shape.Length(), 2), zjzdh);
                    }
                    else
                    {
                        var wz = "";
                        switch (en.Position)
                        {
                            case "1":
                                wz = "内侧";
                                break;
                            case "2":
                                wz = "中间";
                                break;
                            case "3":
                                wz = "外侧";
                                break;
                        }
                        var lb = "";
                        lbdic.TryGetValue(en.CoilType, out lb);
                        jszsm = string.Format("{0}沿{1}{2}{3}方{4}米到{5}", qjzdh, lb, wz, a.toAzimuthString(), ToolMath.RoundNumericFormat(en.Shape.Length(), 2), zjzdh);
                    }
                }
                en.Description = jszsm;
            }
        }

        /// <summary>
        /// 设置界址线毗邻权利人
        /// </summary>
        /// <param name="en"></param>
        public virtual void SetAddressCoilPerson(BuildLandBoundaryAddressCoil en, SetDotCoilArg setArg)
        {
            if (en.NeighborPerson == setArg.ReplaceLinePersonFrom)
            {
                en.NeighborPerson = setArg.ReplaceLinePersonTo;
            }
        }

        /// <summary>
        /// 设置界址线毗邻指界人
        /// </summary>
        /// <param name="en"></param>
        public virtual void SetAddressCoilRefer(BuildLandBoundaryAddressCoil en, SetDotCoilArg setArg)
        {
            if (en.NeighborFefer == setArg.ReplaceLineReferFrom)
            {
                en.NeighborFefer = setArg.ReplaceLineReferTo;
            }
        }


        /// <summary>
        /// 删除地域下的界址点
        /// </summary>
        private void ClearDotLine(string zoneCode, IBuildLandBoundaryAddressCoilWorkStation coilStation,
         IBuildLandBoundaryAddressDotWorkStation dotStation)
        {
            if (zoneCode.Length == 14)
            {
                dotStation.Delete(t => t.ZoneCode.Equals(zoneCode));
                coilStation.Delete(t => t.ZoneCode.Equals(zoneCode));
            }
            else
            {
                dotStation.Delete(t => t.ZoneCode.StartsWith(zoneCode));
                coilStation.Delete(t => t.ZoneCode.StartsWith(zoneCode));
            }

        }

        /// <summary>
        /// 初始化字典信息，界址线类别
        /// </summary>
        /// <param name="meta"></param>
        private void GetDicsInfo(TaskInitializeLandDotCoilArgument meta)
        {
            var dicstation = meta.Database.CreateDictWorkStation();
            var jzxlbdics = dicstation.GetByGroupCode(DictionaryTypeInfo.JZXLB, false);
            meta.InstallArg.Jzxlbdics = jzxlbdics;
        }

        #endregion

        #region 单个地块初始化

        /// <summary>
        /// 单个地块初始化
        /// </summary>
        public void SingleContractLandInitialTool()
        {
            var meta = Argument as TaskInitializeLandDotCoilArgument;
            if (meta.SingleLand.Shape == null)
            {
                this.ReportError("选择地块无图形，无法初始化");
                return;
            }
            var dotStation = meta.Database.CreateDotWorkStation();
            var coilStation = meta.Database.CreateCoilWorkStation();
            var landStation = meta.Database.CreateContractLandWorkstation();
            var zoneCode = meta.CurrentZone.FullCode == "86" ? "" : meta.CurrentZone.FullCode;

            try
            {
                var zonestation = meta.Database.CreateZoneWorkStation();
                var zonelist = zonestation.GetParents(meta.CurrentZone);
                var zoneParentName = meta.CurrentZone.Name;
                foreach (var item in zonelist)
                {
                    zoneParentName = item.Name + zoneParentName;
                }
                var prms = new InitLandDotCoilParam();
                prms.AddressDotMarkType = SafeConvertAux.SafeConvertToInt16(meta.InstallArg.AddressDotMarkType);
                prms.AddressDotType = SafeConvertAux.SafeConvertToInt16(meta.InstallArg.AddressDotType);
                prms.AddressLineCatalog = meta.InstallArg.AddressLineCatalog;
                prms.AddressLinedbiDistance = meta.InstallArg.AddressLinedbiDistance;
                prms.AddressLinePosition = meta.InstallArg.AddressLinePosition;
                prms.AddressLineType = meta.InstallArg.AddressLineType;
                prms.AddressPointPrefix = meta.InstallArg.AddressPointPrefix;
                prms.IsLineDescription = meta.InstallArg.LineDescription == EnumDescription.LineLength;
                prms.IsAddressLinePosition = meta.InstallArg.LineDescription == EnumDescription.LineFind;
                prms.MinAngleFileter = meta.InstallArg.MinAngleFileter;
                prms.MaxAngleFilter = meta.InstallArg.MaxAngleFilter;
                prms.MinAngleFileterExtend = 10;
                prms.MaxAngleFilterExtend = 91;
                prms.IsFilter = meta.InstallArg.IsFilterDot;
                prms.UseAddAlgorithm = meta.InstallArg.UseAddAlgorithm;
                ContractLand land = meta.SingleLand;

                dotStation.DeleteByLandIds(new Guid[] { land.ID });
                coilStation.DeleteByLandIds(new Guid[] { land.ID });

                var query = meta.Database.CreateQuery<BuildLandBoundaryAddressDot>();
                var length = query.Max(t => t.UniteDotNumber.Length);
                var maxdot = query.Where(t => t.UniteDotNumber.Length == length).OrderByDescending(t => t.UniteDotNumber).Select(s => s).FirstOrDefault();

                string maxuntiNumber = maxdot == null ? "0" : maxdot.UniteDotNumber;
                var buffGeo = land.Shape.Buffer(prms.AddressLinedbiDistance);
                var lands = landStation.Get(t => t.Shape != null && t.Shape.Intersects(buffGeo));
                if (lands == null || lands.Count == 0)
                    lands = new List<ContractLand>();

                var dots = dotStation.Get(t => t.Shape != null && t.Shape.Intersects(buffGeo));
                var coils = coilStation.Get(t => t.Shape != null && t.Shape.Intersects(buffGeo));

                if (dots == null)
                    dots = new List<BuildLandBoundaryAddressDot>();
                if (coils == null)
                    coils = new List<BuildLandBoundaryAddressCoil>();
                var pointList = ProcessDot(land, prms, maxuntiNumber, dots);//初始化界址点
                _lineDescription = meta.InstallArg.LineDescription;
                _jzxlbdic = meta.InstallArg.Jzxlbdics;
                var lineList = ProcessLine(land, lands, prms, pointList, coils, zoneParentName, meta.InstallArg.IsSetAddressLinePosition, meta.InstallArg.IsUnit);//初始化界址线

                //数据入库
                dotStation.AddRange(pointList);
                coilStation.AddRange(lineList);
            }
            catch (Exception ex)
            {
                this.ReportException(ex);
            }
        }

        /// <summary>
        /// 生成界址点
        /// </summary>
        private List<BuildLandBoundaryAddressDot> ProcessDot(ContractLand land, InitLandDotCoilParam param, string maxuntiNumber, List<BuildLandBoundaryAddressDot> dots)
        {
            var entityList = new List<BuildLandBoundaryAddressDot>();
            var lines = land.Shape.ToPolylines();//地块生成线
            string regex = @"(\d+)";
            var mstr = Regex.Match(maxuntiNumber, regex);
            string imgId = mstr.Groups[1].Value.ToString();
            var startNumber = int.Parse(imgId) + 1;
            int jzdh = 1;
            foreach (var l in lines)
            {
                int jzdc = 1;
                var coordinates = l.ToCoordinates().ToList();
                coordinates.RemoveAt(coordinates.Count - 1);
                var coordinateAarray = SortCoordsByWNOrder(coordinates.ToArray());
                foreach (var item in coordinateAarray)
                {
                    var dot = CreateDot(item, land, jzdh, param);
                    var findcoord = FindSamePoint(dots, item);
                    if (findcoord != null)
                    {
                        dot.UniteDotNumber = findcoord.UniteDotNumber;
                        dot.IsValid = false;
                    }
                    else
                    {
                        dot.UniteDotNumber = param.AddressPointPrefix + startNumber;
                    }
                    if (param.IsFilter)
                    {
                        if (jzdc == 1)
                            dot.IsValid = true;
                        if (jzdc > 1 && jzdc < coordinateAarray.Length)
                        {
                            var angle = CglHelper.CalcAngle(item.X, item.Y, coordinateAarray[jzdc - 2].X, coordinateAarray[jzdc - 2].Y, coordinateAarray[jzdc].X, coordinateAarray[jzdc].Y);
                            if (isKeyAngle(angle, param))
                                dot.IsValid = true;
                        }
                        else if (jzdc == coordinateAarray.Length)
                        {
                            var angle = CglHelper.CalcAngle(item.X, item.Y, coordinateAarray[jzdc - 2].X, coordinateAarray[jzdc - 2].Y, coordinateAarray[0].X, coordinateAarray[0].Y);
                            if (isKeyAngle(angle, param))
                                dot.IsValid = true;
                        }
                    }
                    else
                    {
                        dot.IsValid = true;
                    }

                    //查找之前构造点
                    entityList.Add(dot);
                    jzdh++;
                    jzdc++;
                    startNumber++;
                }
            }
            EnsureThreeKeyJzd(entityList, param);
            return entityList;
        }

        /// <summary>
        /// 为了准确判断四至，从非关键界址点处，增加关键界址点
        /// </summary>
        /// <param name="lstJzd"></param>
        private void EnsureThreeKeyJzd(List<BuildLandBoundaryAddressDot> lstJzd, InitLandDotCoilParam param, int ensureCount = 4)
        {
            var arrayJzd = lstJzd.ToArray();
            int firstKeyPointIndex = -1;
            var recalList = new List<List<BuildLandBoundaryAddressDot>>();
            var list = new List<BuildLandBoundaryAddressDot>();
            recalList.Add(list);
            for (int i = 0; i < arrayJzd.Length; i++)
            {
                if (arrayJzd[i].IsValid)
                {
                    if (firstKeyPointIndex == -1)
                        firstKeyPointIndex = i;
                    list.Add(arrayJzd[i]);
                    if (list.Count > 1)
                    {
                        list = new List<BuildLandBoundaryAddressDot>() { arrayJzd[i] };
                        recalList.Add(list);
                    }
                }
                else
                {
                    list.Add(arrayJzd[i]);
                }
            }
            if (list.Count > 0)
            {
                if (firstKeyPointIndex == 0 && !list.Last().IsValid)
                {
                    list.Add(arrayJzd[0]);
                }
                else
                {
                    for (int i = 0; i <= firstKeyPointIndex; i++)
                    {
                        list.Add(arrayJzd[i]);
                    }
                }
            }
            recalList.RemoveAll(t => t.Count < ensureCount);
            foreach (var item in recalList)
            {
                RecalcKeyPoint(item, ensureCount, param);
            }
        }

        /// <summary>
        /// 计算2个关键界址点之间的点是否需要补充作为关键界址点
        /// </summary>
        /// <param name="lstJzd"></param>
        private void RecalcKeyPoint(List<BuildLandBoundaryAddressDot> lstJzd, int ensureCount, InitLandDotCoilParam param)
        {
            if (lstJzd == null) return;
            var coord0 = lstJzd[0].Shape.ToCoordinates()[0];
            var coord2 = lstJzd[0].Shape.ToCoordinates()[0];
            if (coord0.X > coord2.X || (coord0.X == coord2.X) && (coord0.Y < coord2.Y))
                lstJzd.Reverse();
            var arrayJzd = lstJzd.ToArray();

            if (arrayJzd.Length < ensureCount) return;

            // 判断关键界址点的下下一个界址点是否为待选关键界址点
            // 关键界址点的下一个界址点必不是待选关键界址点，关键界址点的上一个界址点必不是待选关键界址点
            int i = 2;
            for (; i < arrayJzd.Length - 2; i++)
            {
                // 保证两线段同方向的夹角

                var coord10 = arrayJzd[0].Shape.ToCoordinates()[0];
                var coord12 = arrayJzd[1].Shape.ToCoordinates()[0];

                var coord20 = arrayJzd[i].Shape.ToCoordinates()[0];
                var coord22 = arrayJzd[i + 1].Shape.ToCoordinates()[0];

                var angle = (180 - this.CalcAngle(coord10.X, coord10.Y, coord12.X, coord12.Y,
                    coord20.X, coord20.Y, coord22.X, coord22.Y));
                if (isKeyAngle(angle, param))
                {
                    arrayJzd[i].IsValid = true;
                    break;
                }
            }

            // 递归计算新的关键界址点和原来的第二个关键界址点之间的点，是否需要补充作为关键界址点
            if (arrayJzd.Length - i >= ensureCount)
            {
                var newLstJzd = new List<BuildLandBoundaryAddressDot>();
                for (int j = i; j < arrayJzd.Length; j++)
                {
                    newLstJzd.Add(arrayJzd[j]);
                }
                RecalcKeyPoint(newLstJzd, ensureCount, param);
            }
        }

        /// <summary>
        /// 计算2向量的夹角
        /// </summary>
        /// <param name="x1">第一个向量的起点X轴坐标</param>
        /// <param name="y1">第一个向量的起点Y轴坐标</param>
        /// <param name="x2">第一个向量的终点点X轴坐标</param>
        /// <param name="y2">第一个向量的终点点Y轴坐标</param>
        /// <param name="x3">第二个向量的起点X轴坐标</param>
        /// <param name="y3">第二个向量的起点Y轴坐标</param>
        /// <param name="x4">第二个向量的终点点X轴坐标</param>
        /// <param name="y4">第二个向量的终点点Y轴坐标</param>
        /// <returns></returns>
        private double CalcAngle(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            var v1x = x2 - x1;
            var v1y = y2 - y1;
            var v2x = x4 - x3;
            var v2y = y4 - y3;

            var multi = v1x * v2x + v1y * v2y;
            var v1mod = Math.Sqrt(v1x * v1x + v1y * v1y);
            var v2mode = Math.Sqrt(v2x * v2x + v2y * v2y);

            var angle = Math.Acos(ToolMath.RoundNumericFormat(multi / v1mod / v2mode, 6)) * 180 / Math.PI;
            return angle;
        }

        /// <summary>
        /// 查找相同点
        /// </summary>
        private BuildLandBoundaryAddressDot FindSamePoint(List<BuildLandBoundaryAddressDot> dots, Coordinate item)
        {
            BuildLandBoundaryAddressDot findcoord = null;
            foreach (var d in dots)
            {
                if (testIsEqual(d.Shape.ToCoordinates()[0], item.X, item.Y))
                {
                    findcoord = d;
                    break;
                }
            }
            return findcoord;
        }

        /// <summary>
        /// 构建界址点
        /// </summary>
        private BuildLandBoundaryAddressDot CreateDot(Coordinate item, ContractLand land, int jzdh, InitLandDotCoilParam param)
        {
            var dot = new BuildLandBoundaryAddressDot()
            {
                ID = Guid.NewGuid(),
                CreationTime = DateTime.Now,
                ZoneCode = land.ZoneCode,
                Shape = YuLinTu.Spatial.Geometry.CreatePoint(item, land.Shape.Srid),
                Modifier = "",
                ModifiedTime = DateTime.Now,
                Founder = "",
                LandID = land.ID,
                DotNumber = param.AddressPointPrefix + jzdh.ToString(),
                LandNumber = land.LandNumber,
                LandType = land.LandCode,
                DotType = param.AddressDotType.ToString(),
                LandMarkType = param.AddressDotMarkType.ToString(),
            };

            return dot;
        }

        /// <summary>
        /// 初始化界址线
        /// </summary>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> ProcessLine(ContractLand land, List<ContractLand> lands, InitLandDotCoilParam param,
            List<BuildLandBoundaryAddressDot> dots, List<BuildLandBoundaryAddressCoil> coils, string senderName, bool IsSetAddressLinePosition, bool isUnit)
        {
            var entityList = new List<BuildLandBoundaryAddressCoil>();
            if (dots == null || dots.Count == 0)
                return entityList;

            dots.Add(dots[0]);
            var createdots = new List<BuildLandBoundaryAddressDot>();
            short sxh = 1;
            bool hasStartKeyDot = false; //是否已经找到开始界址点
            bool hasEndKeyDot = false; //是否已经找到结束界址点

            foreach (var item in dots)
            {
                if (item.IsValid && hasStartKeyDot == false)
                {
                    createdots.Add(item);
                    hasStartKeyDot = true;
                    continue;
                }

                if (item.IsValid == false)
                {
                    createdots.Add(item);
                    continue;
                }

                if (item.IsValid && hasStartKeyDot && hasEndKeyDot == false)
                {
                    createdots.Add(item);

                    var line = CreateAddressCoil(createdots, land, sxh, param, lands, senderName);
                    entityList.Add(line);

                    createdots.Clear();
                    createdots.Add(item);
                    hasStartKeyDot = true;
                    hasEndKeyDot = false;
                    sxh++;
                }
            }

            LinePropertiesSet(entityList, coils, dots, IsSetAddressLinePosition,
                isUnit, param.IsLineDescription);

            dots.Remove(dots[dots.Count - 1]);
            return entityList;
        }

        /// <summary>
        /// 界址线设置
        /// </summary> 
        private void LinePropertiesSet(List<BuildLandBoundaryAddressCoil> entityList, List<BuildLandBoundaryAddressCoil> coils,
          List<BuildLandBoundaryAddressDot> dots, bool IsSetAddressLinePosition, bool isUnit, bool isUseLengthAndPosition)
        {
            var startIndex = 1;
            foreach (var line in entityList)
            {
                if (coils != null && coils.Count > 0)
                {
                    var coil = coils.Find(t => TestLineEqual(t.Shape, line.Shape));
                    if (coil != null)
                    {
                        line.Position = POSITIONCEN;
                    }
                }
                var charArray = line.StartNumber.Reverse();
                List<char> charlist = new List<char>();
                foreach (var item in charArray)
                {
                    if (item >= 48 && item <= 58)
                    {
                        charlist.Add(item);
                    }
                }
                charlist.Reverse();
                var num = "";
                foreach (var t in charlist)
                {
                    num += t;
                }
                if (num == "1")
                {
                    startIndex = line.OrderID;
                }
                GetLineDescription(line, isUnit, isUseLengthAndPosition, dots);
            }
            if (IsSetAddressLinePosition)
            {
                foreach (var item in entityList)
                {
                    if (item.Position == POSITIONOUT)
                        item.Position = POSITIONIN;
                }
            }
            short orderid = 1;

            for (int i = startIndex; i <= entityList.Count; i++)
            {
                entityList[i - 1].OrderID = orderid;
                orderid++;
            }
            for (int i = 0; i < startIndex - 1; i++)
            {
                entityList[i].OrderID = orderid;
                orderid++;
            }
        }

        /// <summary>
        /// 获取界址线说明
        /// </summary>
        /// <returns></returns>
        private void GetLineDescription(BuildLandBoundaryAddressCoil line, bool isUnit, bool isUseLengthAndPosition, List<BuildLandBoundaryAddressDot> dots)
        {
            Aspect a = new Aspect(0);
            if (line.Shape != null)
            {
                if (_lineDescription == EnumDescription.LineLength)
                {
                    line.Description = ToolMath.RoundNumericFormat(line.Shape.Length(), 2).ToString();
                }
                else
                {
                    var coords = line.Shape.ToCoordinates();
                    var p0 = coords[0];
                    var p1 = coords[coords.Count() - 1];
                    a.Assign(p0.X, p0.Y, p1.X, p1.Y);
                    string qjzdh = line.StartNumber;
                    var zjzdh = line.EndNumber;
                    if (isUnit)
                    {
                        qjzdh = dots.Find(t => t.ID == line.StartPointID).UniteDotNumber;
                        zjzdh = dots.Find(t => t.ID == line.EndPointID).UniteDotNumber;
                    }
                    if (_lineDescription == EnumDescription.LineFind)
                    {
                        line.Description = string.Format("{0}沿{1}方{2}米到{3}", qjzdh, a.toAzimuthString(), ToolMath.RoundNumericFormat(line.Shape.Length(), 2), zjzdh);
                    }
                    else
                    {
                        var wz = "";
                        switch (line.Position)
                        {
                            case "1":
                                wz = "内侧";
                                break;
                            case "2":
                                wz = "中间";
                                break;
                            case "3":
                                wz = "外侧";
                                break;
                        }
                        var lb = _jzxlbdic?.FirstOrDefault(r => r.Code == line.CoilType)?.Name ?? string.Empty;
                        line.Description = string.Format("{0}沿{1}{2}{3}方{4}米到{5}", qjzdh, lb, wz, a.toAzimuthString(), ToolMath.RoundNumericFormat(line.Shape.Length(), 2), zjzdh);
                    }
                }
            }
        } 

        /// <summary>
        /// 创建界址线
        /// </summary>
        private BuildLandBoundaryAddressCoil CreateAddressCoil(List<BuildLandBoundaryAddressDot> list, ContractLand land,
            short sxh, InitLandDotCoilParam param, List<ContractLand> lands, string senderName)
        {
            var linestring = CreatLine(list, land.Shape.Srid);
            var line = new BuildLandBoundaryAddressCoil()
            {
                ID = Guid.NewGuid(),
                CreationTime = DateTime.Now,
                ZoneCode = land.ZoneCode,
                Shape = linestring,
                Modifier = "",
                ModifiedTime = DateTime.Now,
                Founder = "",
                LandID = land.ID,
                LandNumber = land.LandNumber,
                LandType = land.LandCode,
                StartPointID = list[0].ID,
                StartNumber = list[0].DotNumber,
                EndPointID = list[list.Count - 1].ID,
                EndNumber = list[list.Count - 1].DotNumber,
                OrderID = sxh,
                CoilLength = linestring.Length(),
                Position = param.IsAddressLinePosition ? param.AddressLinePosition : "3",
                LineType = param.AddressLineType,
                CoilType = param.AddressLineCatalog,
                Description = linestring.Length().ToString(),
            };
            var linebuffer = linestring.Buffer(param.AddressLinedbiDistance);
            var landList = lands.FindAll(t => t.Shape.Intersects(linebuffer));
            landList.RemoveAll(t => t.ID == land.ID);
            // 获取相邻地块中相交最长的地块
            if (landList.Count > 0)
            {
                ContractLand adjacentLand = landList[0]; // 毗邻地块
                if (landList.Count >= 1)
                {
                    double maxLen = 0;
                    ContractLand maxLenLand = null;
                    foreach (var q in landList)
                    {
                        var g = q.Shape;
                        if (g != null)
                        {
                            var oi = tGISCNet.Topology.Intersect(g.AsBinary(), linebuffer.AsBinary());
                            if (oi != null)
                            {
                                var gi = WKBHelper.fromWKB(oi);
                                if (gi != null)
                                {
                                    var length = Math.Abs(gi.Area);
                                    if (maxLenLand == null || maxLen < length)
                                    {
                                        maxLenLand = q;
                                        maxLen = length;
                                    }
                                }
                            }
                        }
                    }
                    if (maxLenLand != null)
                    {
                        adjacentLand = maxLenLand;
                    }
                }

                line.NeighborPerson = adjacentLand.OwnerName;
                line.NeighborFefer = line.NeighborPerson;
            }
            else
            {
                line.NeighborPerson = senderName;
                line.NeighborFefer = senderName;
            }
            return line;
        }

        /// <summary>
        /// 创建线
        /// </summary>
        private Geometry CreatLine(List<BuildLandBoundaryAddressDot> dots, int srid)
        {
            Coordinate[] corrds = new Coordinate[dots.Count];
            for (int i = 0; i < dots.Count; i++)
            {
                corrds[i] = dots[i].Shape.ToCoordinates()[0];
            }
            var geo = YuLinTu.Spatial.Geometry.CreatePolyline(corrds.ToList(), srid);
            return geo;
        }

        /// <summary>
        /// 判断角度是否可判断为关键界址点
        /// </summary>
        private bool isKeyAngle(double angle, InitLandDotCoilParam param)
        {
            if (param.IsFilter)
            {
                return angle >= (double)param.MinAngleFileter
                    && angle <= (double)param.MaxAngleFilter;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 按西北角顺时针排序
        /// </summary>
        /// <param name="coords">in,out</param>
        /// <param name="fCW">输入集合coords是否按顺时针排序</param>
        public Coordinate[] SortCoordsByWNOrder(Coordinate[] coords)
        {
            double leftX = coords[0].X;
            double topY = coords[0].Y;
            int rowQ = 0;
            double rowY = 0;
            var ip = 0;
            var lenDic = new Dictionary<int, double>();
            for (int i = 1; i < coords.Length; ip = i++)
            {
                var q = coords[i];
                if (leftX >= q.X)
                {
                    leftX = q.X;
                    if (rowY < q.Y)
                        rowQ = i;
                }
                if (topY < q.Y)
                    topY = q.Y;
            }
            double d2 = 0;
            Coordinate dp = null;
            int n = 0;
            for (int i = 0; i < coords.Length; ++i)
            {
                var p = coords[i];
                double dx = p.X - leftX;
                double dy = p.Y - topY;
                double d = dx * dx + dy * dy;
                if (d < d2 || i == 0)
                {
                    d2 = d;
                    dp = p;
                    n = i;
                }
                lenDic.Add(i, d);
            }
            //查找起止点时 不仅是找与x最小 y最大 距离最近的点，还要与x最小的点比较 如果距离平方不超过50 并且斜率小于45 就选择x最小的点
            if (lenDic[rowQ] != d2)
            {
                var tan = (dp.Y - coords[rowQ].Y) / (dp.X - coords[rowQ].X);
                if (tan < 1)
                    n = rowQ;
                else
                {
                    for (int k = n; k > 0; k--)
                    {
                        var tan1 = (dp.Y - coords[k].Y) / (dp.X - coords[k].X);
                        if ((tan1 < 1 && (lenDic[k] - d2) < 50) || (tan1 < 1.5 && (lenDic[k] - d2) < 20))
                            n = k;
                    }
                }
            }
            var newcoords = new List<Coordinate>();
            for (int i = n; i < coords.Length; ++i)
            {
                newcoords.Add(coords[i]);
            }
            for (int i = 0; i < n; ++i)
            {
                newcoords.Add(coords[i]);
            }
            return newcoords.ToArray();
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        public bool testIsEqual(Coordinate c, double x, double y, double tolerance = 0.001)
        {
            return CglHelper.equal(c.X, x, tolerance) && CglHelper.equal(c.Y, y, tolerance);
        }

        /// <summary>
        /// 线是否相等
        /// </summary>
        public bool TestLineEqual(Geometry geo1, Geometry geo2, double tolerance = 0.001)
        {
            var result = true;
            var geoArray1 = geo1.ToCoordinates();
            var geoArray2 = geo2.ToCoordinates();

            if (geoArray1.Length != geoArray2.Length)
                return false;

            if (!testIsEqual(geoArray1[0], geoArray2[0].X, geoArray2[0].Y))
                geoArray2 = geoArray2.Reverse().ToArray();
            if (!testIsEqual(geoArray1[0], geoArray2[0].X, geoArray2[0].Y))
                return false;

            for (int i = 0; i < geoArray1.Length; i++)
            {
                if (!testIsEqual(geoArray1[i], geoArray2[i].X, geoArray2[i].Y))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        #endregion

        #region Methods—初始界址信息辅助方法

        #endregion

        #region Methods—任务辅助方法

        /// <summary>
        /// 进度报告
        /// </summary>
        /// <param name="progress"></param>
        public void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }


        /// <summary>
        /// 错误信息报告
        /// </summary>
        /// <param name="message"></param>
        public void ReportInfo(object sender, TaskAlertEventArgs e)
        {
            if (e != null)
            {
                this.ReportAlert(e.Grade, e.UserState, e.Description);
            }
        }

        /// <summary>
        /// 界址线类别配置
        /// </summary>
        /// <returns></returns>
        public List<JzxlbxmlClass> GetJZXLBStrXml()
        {
            List<JzxlbxmlClass> jzxlbstrxmls = new List<JzxlbxmlClass>();
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\Config\" + "JZXLBPZXml.xml";

            bool hasfile = System.IO.File.Exists(filePath);
            if (hasfile)
            {
                var getxmls = ToolSerialization.DeserializeXml(filePath, jzxlbstrxmls.GetType());
                if (getxmls == null)
                {
                    jzxlbstrxmls = GetInitJZXLBStrs();
                    ToolSerialization.SerializeXml(filePath, jzxlbstrxmls);
                }
                else
                {
                    jzxlbstrxmls = getxmls as List<JzxlbxmlClass>;
                }
            }
            else
            {
                jzxlbstrxmls = GetInitJZXLBStrs();
                ToolSerialization.SerializeXml(filePath, jzxlbstrxmls);
            }
            return jzxlbstrxmls;
        }

        public List<JzxlbxmlClass> GetInitJZXLBStrs()
        {
            List<JzxlbxmlClass> jzxlbstrxmls = new List<JzxlbxmlClass>();
            JzxlbxmlClass jzxlbstr = new JzxlbxmlClass();
            jzxlbstr.JzxlbdicNameCode = "01";
            jzxlbstr.JzxlbdicNameContains = "田埂，田垄";
            jzxlbstrxmls.Add(jzxlbstr);

            JzxlbxmlClass jzxlbstr1 = new JzxlbxmlClass();
            jzxlbstr1.JzxlbdicNameCode = "02";
            jzxlbstr1.JzxlbdicNameContains = "沟渠";
            jzxlbstrxmls.Add(jzxlbstr1);

            JzxlbxmlClass jzxlbstr2 = new JzxlbxmlClass();
            jzxlbstr2.JzxlbdicNameCode = "03";
            jzxlbstr2.JzxlbdicNameContains = "大路,小路,街道,道路";
            jzxlbstrxmls.Add(jzxlbstr2);

            JzxlbxmlClass jzxlbstr3 = new JzxlbxmlClass();
            jzxlbstr3.JzxlbdicNameCode = "04";
            jzxlbstr3.JzxlbdicNameContains = "行树，树木";
            jzxlbstrxmls.Add(jzxlbstr3);

            JzxlbxmlClass jzxlbstr4 = new JzxlbxmlClass();
            jzxlbstr4.JzxlbdicNameCode = "05";
            jzxlbstr4.JzxlbdicNameContains = "围墙";
            jzxlbstrxmls.Add(jzxlbstr4);

            JzxlbxmlClass jzxlbstr5 = new JzxlbxmlClass();
            jzxlbstr5.JzxlbdicNameCode = "06";
            jzxlbstr5.JzxlbdicNameContains = "墙壁";
            jzxlbstrxmls.Add(jzxlbstr5);

            JzxlbxmlClass jzxlbstr6 = new JzxlbxmlClass();
            jzxlbstr6.JzxlbdicNameCode = "07";
            jzxlbstr6.JzxlbdicNameContains = "栅栏";
            jzxlbstrxmls.Add(jzxlbstr6);

            JzxlbxmlClass jzxlbstr7 = new JzxlbxmlClass();
            jzxlbstr7.JzxlbdicNameCode = "08";
            jzxlbstr7.JzxlbdicNameContains = "两点连线";
            jzxlbstrxmls.Add(jzxlbstr7);

            JzxlbxmlClass jzxlbstr8 = new JzxlbxmlClass();
            jzxlbstr8.JzxlbdicNameCode = "99";
            jzxlbstr8.JzxlbdicNameContains = "其他界线，其他，其它";
            jzxlbstrxmls.Add(jzxlbstr8);

            return jzxlbstrxmls;
        }

        #endregion
    }

    public class MuilPolyGonInInfoClass
    {
        public Guid ID;

        public List<Geometry> interGeos;
    }

}
