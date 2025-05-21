/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入界址线图斑数据(任务)
    /// </summary>
    public class ImportBoundaryAddressCoil : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImportBoundaryAddressCoil()
        { }

        #endregion

        #region Fields

        //private list

        #endregion

        #region Properties

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 界址点图斑文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 当前地域下的所有承包地块
        /// </summary>
        public List<ContractLand> ListLand { get; set; }

        /// <summary>
        /// 导入界址点图斑设置
        /// </summary>
        public ImportBoundaryAddressCoilDefine ImportCoilDefine = ImportBoundaryAddressCoilDefine.GetIntence();

        /// <summary>
        /// 描述信息
        /// </summary>
        public string MarkDesc { get; set; }

        /// <summary>
        /// 当前地域下匹配的界址点数据
        /// </summary>
        public List<BuildLandBoundaryAddressDot> CurrentZoneDotList { get; set; }

        #endregion

        #region Method

        /// <summary>
        /// 导入界址线图斑数据-先进行数据检查，后导入
        /// </summary>
        public void CreateImportBoundaryCoilTask()
        {
            if (CurrentZone == null)
            {
                this.ReportError("未选择导入地域!");
                return;
            }
            string filePath = System.IO.Path.GetDirectoryName(FileName);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(FileName);
            var ds = ProviderShapefile.CreateDataSource(filePath, false) as IDbContext;
            var dp = new DynamicQuery(ds);
            QueryResult qr = dp.Get(null, fileName);
            IList dotResultList = qr.Result as IList;
            if (dotResultList.Count == 0)
            {
                this.ReportInfomation("当前导入文件没有数据");
                return;
            }
            var importShpType = ds.DataSource as IProviderShapefile;
            if (importShpType.GetGeometryType(fileName) != Spatial.eGeometryType.Polyline)
            {
                this.ReportError("当前Shape文件不为线文件，请重新选择点文件导入");
                return;
            }
            List<Dictionary> dictsJXXZ = new List<Dictionary>();   //界线性质
            List<Dictionary> dictsJZXLB = new List<Dictionary>();  //界址线类别
            List<Dictionary> dictsJZXWZ = new List<Dictionary>();  //界址线位置
            List<Dictionary> dictsTDQSLX = new List<Dictionary>();  //土地权属类型
            IDbContext dbContext = null;
            try
            {
                dbContext = DataBaseSource.GetDataBaseSource();
                if (dbContext == null)
                {
                    this.ReportError(DataBaseSource.ConnectionError);
                    return;
                }
                var dictStation = dbContext.CreateDictWorkStation();
                dictsJXXZ = dictStation.GetByGroupCodeWork(DictionaryTypeInfo.JXXZ);
                dictsJZXLB = dictStation.GetByGroupCodeWork(DictionaryTypeInfo.JZXLB);
                dictsJZXWZ = dictStation.GetByGroupCodeWork(DictionaryTypeInfo.JZXWZ);
                dictsTDQSLX = dictStation.GetByGroupCodeWork(DictionaryTypeInfo.TDQSLX);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetByGroupCodeWork(获取数据字典失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取数据字典失败," + ex.Message);
            }
            List<BuildLandBoundaryAddressCoil> coilList = new List<BuildLandBoundaryAddressCoil>();

            // Transaction Error

            int index = 1;
            dbContext.BeginTransaction();
            var targetSpatialReference = dbContext.CreateSchema().GetElementSpatialReference(
                ObjectContext.Create(typeof(BuildLandBoundaryAddressCoil)).Schema,
                ObjectContext.Create(typeof(BuildLandBoundaryAddressCoil)).TableName);
            foreach (var coil in dotResultList)
            {
                string zoneCode = string.Empty;
                string lineType = string.Empty;
                string lineTypeCode = string.Empty;
                string coilType = string.Empty;
                string coilTypeCode = string.Empty;
                string position = string.Empty;
                string positionCode = string.Empty;
                string landType = string.Empty;
                string landTypeCode = string.Empty;
                string description = string.Empty;
                string neighborFefer = string.Empty;
                string neighborPerson = string.Empty;
                string landNumber = string.Empty;
                string startNumber = string.Empty;
                short orderid = 1;
                Guid startId = Guid.Empty;
                Guid endid = Guid.Empty;
                string endNumber = string.Empty;
                double coillength = 0.0;

                if (ImportCoilDefine.ZoneCodeIndex != "None")
                {
                    var zonecode = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.ZoneCodeIndex) as string;
                    if (!string.IsNullOrEmpty(zonecode))
                    {
                        if (zonecode == CurrentZone.FullCode)
                        {
                            zoneCode = zonecode;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                if (ImportCoilDefine.LandNumber != "None")
                {
                    landNumber = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.LandNumber) as string;
                }
                if (ImportCoilDefine.StartNumber != "None")
                {
                    startNumber = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.StartNumber) as string;
                }
                if (ImportCoilDefine.EndNumber != "None")
                {
                    endNumber = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.EndNumber) as string;
                }

                if (ImportCoilDefine.CoilLength != "None")
                {
                    var coillengthstr = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.CoilLength) as string;
                    if (coillengthstr != null)
                    {
                        Double.TryParse(coillengthstr, out coillength);
                    }
                }

                if (ImportCoilDefine.LineType != "None")
                {
                    lineType = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.LineType) as string;
                    var dictJXXZ = dictsJXXZ.Find(c => c.Name == lineType);
                    if (dictJXXZ == null || dictJXXZ.Code == "")
                    {
                        this.ReportError(string.Format("第{0}条记录界址线性质名称'" + lineType + "'错误", index.ToString()));
                        dbContext.RollbackTransaction();
                        return;
                    }
                    else
                    {
                        lineTypeCode = dictJXXZ.Code;
                    }
                }
                if (ImportCoilDefine.CoilType != "None")
                {
                    coilType = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.CoilType) as string;
                    var dictJZXLB = dictsJZXLB.Find(c => c.Name == coilType);
                    if (dictJZXLB == null || dictJZXLB.Code == "")
                    {
                        this.ReportError(string.Format("第{0}条记录界址线类别名称'" + coilType + "'错误", index.ToString()));
                        dbContext.RollbackTransaction();
                        return;
                    }
                    else
                    {
                        coilTypeCode = dictJZXLB.Code;
                    }
                }
                if (ImportCoilDefine.Position != "None")
                {
                    position = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.Position) as string;
                    var dictJZXWZ = dictsJZXWZ.Find(c => c.Name == position);
                    if (dictJZXWZ == null || dictJZXWZ.Code == "")
                    {
                        this.ReportError(string.Format("第{0}条记录界址线位置错误", index.ToString()));
                        dbContext.RollbackTransaction();
                        return;
                    }
                    else
                    {
                        positionCode = dictJZXWZ.Code;
                    }
                }
                if (ImportCoilDefine.LandType != "None")
                {
                    landType = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.LandType) as string;
                    var dictTDQSLX = dictsTDQSLX.Find(c => c.Name == landType);
                    if (dictTDQSLX == null || dictTDQSLX.Code == "")
                    {
                        this.ReportError(string.Format("第{0}条记录界址线所属权利类型名称'" + landType + "'错误", index.ToString()));
                        dbContext.RollbackTransaction();
                        return;
                    }
                    else
                    {
                        landTypeCode = dictTDQSLX.Code;
                    }
                }
                if (ImportCoilDefine.Description != "None")
                {
                    description = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.Description) as string;
                }
                if (ImportCoilDefine.NeighborPerson != "None")
                {
                    neighborPerson = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.NeighborPerson) as string;
                }
                if (ImportCoilDefine.NeighborFefer != "None")
                {
                    neighborFefer = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.NeighborFefer) as string;
                }
                Geometry geo = null;
                try
                {
                    var g = ObjectExtension.GetPropertyValue(coil, "Shape") as Geometry;
                    geo = YuLinTu.Spatial.Geometry.FromBytes(g.AsBinary(), 0);
                    geo.SpatialReference = targetSpatialReference;
                }
                catch
                {
                    this.ReportError(string.Format("第{0}条记录shape数据无效", index.ToString()));
                    dbContext.RollbackTransaction();
                    return;
                }

                if (ImportCoilDefine.OrderID != "None")
                {
                    var value = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.OrderID);
                    if (value != null)
                        short.TryParse(value.ToString(), out orderid);                  
                }

                if (ImportCoilDefine.StartID != "None")
                {
                    var gu = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.StartID);
                    if (gu != null)
                        Guid.TryParse(gu.ToString(), out startId);
                }
                if (ImportCoilDefine.EndID != "None")
                {
                    var gu = ObjectExtension.GetPropertyValue(coil, ImportCoilDefine.EndID);
                    if (gu != null)
                        Guid.TryParse(gu.ToString(), out endid);
                }

                coilList.Add(new BuildLandBoundaryAddressCoil()
                {
                    ZoneCode = zoneCode,
                    LandNumber = landNumber,
                    StartNumber = startNumber,
                    EndNumber = endNumber,
                    LineType = lineTypeCode,
                    CoilLength = coillength,
                    CoilType = coilTypeCode,
                    Position = positionCode,
                    LandType = landTypeCode,
                    Description = description,
                    NeighborPerson = neighborPerson,
                    NeighborFefer = neighborFefer,
                    OrderID = orderid,
                    StartPointID = startId,
                    EndPointID = endid,
                    Shape = geo,
                });
                index++;
            }
            if (coilList.Count > 0)
            {
                var coilStation = dbContext.CreateBoundaryAddressCoilWorkStation();
                try
                {
                    //dotStation.DeleteByZoneCode(CurrentZone.FullCode, eSearchOption.Precision, ((int)eLandPropertyRightType.AgricultureLand).ToString());
                    coilStation.DeleteByZoneCode(CurrentZone.FullCode, eSearchOption.Precision);
                }
                catch (Exception ex)
                {
                    YuLinTu.Library.Log.Log.WriteException(this, "DeleteByZoneCode(清空当前地域下的界址线数据失败)", ex.Message + ex.StackTrace);
                    this.ReportError("清空当前地域下的界址线数据失败," + ex.Message);
                    dbContext.RollbackTransaction();
                }
                ImportCoilEntity(coilList, dbContext);
            }
            else
            {
                this.ReportInfomation(string.Format("{0}共导入{1}条界址线图斑数据", MarkDesc, 0));
                this.ReportProgress(100, "完成");
            }
            dbContext.CommitTransaction();

        }

        /// <summary>
        /// 导入界址线图斑数据到数据库
        /// </summary>
        /// <param name="listDot">界址点图斑数据</param>
        private void ImportCoilEntity(List<BuildLandBoundaryAddressCoil> listCoil, IDbContext dbContext)
        {
            if (!listCoil.Any(c => c.Shape.GeometryType == eGeometryType.Polyline))
            {
                //选择的图斑数据不是线数据
                this.ReportError("文件中数据要素不是线要素!");
                return;
            }
            this.ReportProgress(1, "开始");
            if (ListLand.Count == 0)
            {
                this.ReportWarn(string.Format("{0}没有承包地块数据", MarkDesc));
                this.ReportProgress(100, null);
                return;
            }
            if (CurrentZoneDotList.Count == 0)
            {
                this.ReportWarn(string.Format("{0}没有界址点数据", MarkDesc));
                this.ReportProgress(100, null);
                return;
            }
            var dotStation = dbContext.CreateBoundaryAddressCoilWorkStation();
            int indexOfLand = 1;
            int indexOfCoil = 0;
            double percent = 99 / (double)ListLand.Count;
            foreach (var land in ListLand)
            {
                List<BuildLandBoundaryAddressCoil> matchCoil = listCoil.FindAll(c => c.LandNumber.Equals(land.LandNumber));
                List<BuildLandBoundaryAddressDot> matchDotList = CurrentZoneDotList.FindAll(d => d.LandNumber.Equals(land.LandNumber));
                //int index = 1;
                if (matchCoil != null)
                {
                    matchCoil = matchCoil.OrderBy(ld => { int number = 0; Int32.TryParse(ToolString.GetAllNumberWithInString(ld.StartNumber), out number); return number; }).ToList();
                    matchCoil.ForEach(c =>
                    {
                        if (string.IsNullOrEmpty(c.ZoneCode))
                        {
                            c.ZoneCode = CurrentZone.FullCode;
                        }
                        c.ID = Guid.NewGuid();
                        //c.OrderID = (short)(index++);
                        c.LandType = "4";
                        c.LandID = land.ID;
                        c.Founder = "Admin";
                        var matchDotS = matchDotList.Find(o => o.DotNumber == c.StartNumber);
                        c.StartPointID = matchDotS == null ? new Guid() : matchDotS.ID;
                        var matchDotE = matchDotList.Find(o => o.DotNumber == c.EndNumber);
                        c.EndPointID = matchDotE == null ? new Guid() : matchDotE.ID;
                    });
                    matchCoil.OrderBy(d=>d.OrderID);
                    dotStation.AddRange(matchCoil);
                    indexOfCoil = indexOfCoil + matchCoil.Count();
                }
                this.ReportProgress((int)(1 + percent * indexOfLand), MarkDesc + land.OwnerName);
                indexOfLand++;
            }
            if (indexOfCoil == 0)
            {
                this.ReportWarn(string.Format("{0}下无匹配界址线图斑数据进行导入", MarkDesc));
            }
            else
            {
                this.ReportInfomation(string.Format("{0}共导入{1}条界址线图斑数据", MarkDesc, indexOfCoil));
            }
            this.ReportProgress(100, "完成");

        }

        #endregion
    }
}
