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
    /// 导入界址点图斑数据(任务)
    /// </summary>
    public class ImportBoundaryAddressDot : Task
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ImportBoundaryAddressDot()
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
        public ImportBoundaryAddressDotDefine ImportDotDefine = ImportBoundaryAddressDotDefine.GetIntence();

        /// <summary>
        /// 描述信息
        /// </summary>
        public string MarkDesc { get; set; }

        #endregion

        #region Method

        /// <summary>
        /// 导入界址点图斑数据-先进行数据检查，后导入
        /// </summary>
        public void CreateImportBoundaryDotTask()
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
            dp.Dispose();

            var importShpType = ds.DataSource as IProviderShapefile;
            if (importShpType.GetGeometryType(fileName) != Spatial.eGeometryType.Point)
            {
                this.ReportError("当前Shape文件不为点文件，请重新选择点文件导入");
                this.ReportProgress(100);
                return;
            }
            if (dotResultList.Count == 0)
            {
                this.ReportInfomation("当前导入文件没有数据");
                this.ReportProgress(100);
                return;
            }
            List<BuildLandBoundaryAddressDot> dotList = new List<BuildLandBoundaryAddressDot>();
            List<Dictionary> dictsJBLX = new List<Dictionary>();   //界标类型
            List<Dictionary> dictsJZDLX = new List<Dictionary>();  //界址点类型
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
                dictsJBLX = dictStation.GetByGroupCodeWork(DictionaryTypeInfo.JBLX);
                dictsJZDLX = dictStation.GetByGroupCodeWork(DictionaryTypeInfo.JZDLX);
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "GetByGroupCodeWork(获取数据字典失败)", ex.Message + ex.StackTrace);
                this.ReportError("获取数据字典失败," + ex.Message);
                this.ReportProgress(100);
            }
            string zoneCode = string.Empty;
            string landNumber = string.Empty;
            string dotNumber = string.Empty;
            string dotType = string.Empty;
            string dotUnitNumber = string.Empty;
            string landMarkType = string.Empty;
            string dotTypeCode = string.Empty;
            string landMarkTypeCode = string.Empty;
            Guid landid = Guid.Empty;
            Guid pointid = Guid.Empty;
            bool keyDot = true;
            int index = 1;
            // Transaction Error
            dbContext.BeginTransaction();
            var targetSpatialReference = dbContext.CreateSchema().GetElementSpatialReference(
               ObjectContext.Create(typeof(BuildLandBoundaryAddressDot)).Schema,
               ObjectContext.Create(typeof(BuildLandBoundaryAddressDot)).TableName);
            foreach (var dot in dotResultList)
            {
                if (ImportDotDefine.ZoneCodeIndex != "None")
                {
                    var zonecode = ObjectExtension.GetPropertyValue(dot, ImportDotDefine.ZoneCodeIndex) as string;
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

                if (ImportDotDefine.LandNumberIndex != "None")
                {
                    landNumber = ObjectExtension.GetPropertyValue(dot, ImportDotDefine.LandNumberIndex) as string;
                }
                if (ImportDotDefine.DotNumberIndex != "None")
                {
                    dotNumber = ObjectExtension.GetPropertyValue(dot, ImportDotDefine.DotNumberIndex) as string;
                }
                if (ImportDotDefine.UnitDotNumberIndex != "None")
                {
                    dotUnitNumber = ObjectExtension.GetPropertyValue(dot, ImportDotDefine.UnitDotNumberIndex) as string;
                }

                if (ImportDotDefine.DotTypeIndex != "None")
                {
                    dotType = ObjectExtension.GetPropertyValue(dot, ImportDotDefine.DotTypeIndex) as string;
                    var dictJZDLX = dictsJZDLX.Find(c => c.Name == dotType);
                    if (dictJZDLX == null || dictJZDLX.Code == "")
                    {
                        this.ReportError(string.Format("第{0}条记录界址点类型名称'" + dotType + "'错误", index.ToString()));
                        dbContext.RollbackTransaction();
                        return;
                    }
                    else
                    {
                        dotTypeCode = dictJZDLX.Code;
                    }
                }
                if (ImportDotDefine.LandMarkTypeIndex != "None")
                {
                    landMarkType = ObjectExtension.GetPropertyValue(dot, ImportDotDefine.LandMarkTypeIndex) as string;
                    var dictJBLX = dictsJBLX.Find(c => c.Name == landMarkType);
                    if (dictJBLX == null || dictJBLX.Code == "")
                    {
                        this.ReportError(string.Format("第{0}条记录界标类型名称'" + landMarkType + "'错误", index.ToString()));
                        dbContext.RollbackTransaction();
                        return;
                    }
                    else
                    {
                        landMarkTypeCode = dictJBLX.Code;
                    }
                }

                if (ImportDotDefine.KeyDotIndex != "None")
                {
                    var isvalidinfo = (ObjectExtension.GetPropertyValue(dot, ImportDotDefine.KeyDotIndex));
                    if (isvalidinfo != null)
                    {
                        keyDot = Convert.ToBoolean(isvalidinfo);
                    }
                }

                if (ImportDotDefine.LandIdIndex != "None")
                {
                    var isvalidinfo = (ObjectExtension.GetPropertyValue(dot, ImportDotDefine.LandIdIndex));
                    if (isvalidinfo != null)
                    {
                        Guid.TryParse(isvalidinfo.ToString(), out landid);
                    }
                }

                if (ImportDotDefine.PointIdIndex != "None")
                {
                    var isvalidinfo = (ObjectExtension.GetPropertyValue(dot, ImportDotDefine.PointIdIndex));
                    if (isvalidinfo != null)
                    {
                        Guid.TryParse(isvalidinfo.ToString(), out pointid);
                    }
                }
                Geometry geo = null;
                try
                {
                    var g = ObjectExtension.GetPropertyValue(dot, "Shape") as Geometry;
                    geo = YuLinTu.Spatial.Geometry.FromBytes(g.AsBinary(), 0);
                    geo.SpatialReference = targetSpatialReference;
                }
                catch
                {
                    this.ReportError(string.Format("第{0}条记录shape数据无效", index.ToString()));
                    dbContext.RollbackTransaction();
                    return;
                }

                dotList.Add(new BuildLandBoundaryAddressDot()
                {
                    ID = pointid == Guid.Empty ? Guid.NewGuid() : pointid,
                    LandID = landid,
                    ZoneCode = zoneCode,
                    LandType = "4",
                    LandNumber = landNumber,
                    UniteDotNumber = dotUnitNumber,
                    DotNumber = dotNumber,
                    DotType = dotTypeCode,
                    LandMarkType = landMarkTypeCode,
                    Shape = geo,
                    IsValid = keyDot,
                });

                index++;
            }
            if (dotList.Count > 0)
            {
                var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
                try
                {
                    dotStation.DeleteByZoneCode(CurrentZone.FullCode, eSearchOption.Precision);
                }
                catch (Exception ex)
                {
                    YuLinTu.Library.Log.Log.WriteException(this, "DeleteByZoneCode(清空当前地域下的界址点数据失败)", ex.Message + ex.StackTrace);
                    this.ReportError("清空当前地域下的界址点数据失败," + ex.Message);
                    dbContext.RollbackTransaction();
                }
                ImportDotEntity(dotList, dbContext);
            }
            else
            {
                this.ReportInfomation(string.Format("{0}共导入{1}条界址点图斑数据", MarkDesc, 0));
                this.ReportProgress(100, "完成");
            }

            dbContext.CommitTransaction();

        }

        /// <summary>
        /// 导入界址点图斑数据到数据库
        /// </summary>
        /// <param name="listDot">界址点图斑数据</param>
        private void ImportDotEntity(List<BuildLandBoundaryAddressDot> listDot, IDbContext dbContext)
        {
            if (!listDot.Any(c => c.Shape.GeometryType == eGeometryType.Point))
            {
                //选择的图斑数据不是点数据
                this.ReportError("文件中数据要素不是点要素!");
                return;
            }
            this.ReportProgress(1, "开始");
            if (ListLand.Count == 0)
            {
                this.ReportWarn(string.Format("{0}没有承包地块数据", MarkDesc));
                this.ReportProgress(100, null);
                return;
            }
            var dotStation = dbContext.CreateBoundaryAddressDotWorkStation();
            int indexOfLand = 1;
            int indexOfDot = 0;
            double percent = 99 / (double)ListLand.Count;
            foreach (var land in ListLand)
            {
                var matchDots = listDot.FindAll(c => c.LandNumber.Equals(land.LandNumber));
                if (matchDots == null || matchDots.Count == 0)
                {
                    var sendercode = CurrentZone.FullCode.PadRight(14, '0');
                    matchDots = listDot.FindAll(c => (sendercode + c.LandNumber).Equals(land.LandNumber));
                }
                if (matchDots != null && matchDots.Count > 0)
                {
                    matchDots.ForEach(c =>
                    {
                        c.ID = Guid.NewGuid();
                        c.LandID = land.ID;
                        c.Description = "";
                        c.Founder = "Admin";
                        c.CreationTime = DateTime.Now;
                        if (string.IsNullOrEmpty(c.ZoneCode))
                        {
                            c.ZoneCode = CurrentZone.FullCode;
                        }
                    });
                    dotStation.AddRange(matchDots);
                    indexOfDot += matchDots.Count;
                }
                this.ReportProgress((int)(1 + percent * indexOfLand), MarkDesc + land.OwnerName);
                indexOfLand++;
            }
            if (indexOfDot == 0)
            {
                this.ReportWarn(string.Format("{0}下无匹配界址点图斑数据进行导入", MarkDesc));
            }
            else
            {
                this.ReportInfomation(string.Format("{0}共导入{1}条界址点图斑数据", MarkDesc, indexOfDot));
            }
            this.ReportProgress(100, "完成");
        }

        #endregion
    }
}
