/*
 * (C) 2014-2015 鱼鳞图公司版权所有，保留所有权利
*/
using Quality.Business.Entity;
using Quality.Business.TaskBasic;
using System;
using System.Collections.Generic;
using System.IO;
using YuLinTu.Data;
using YuLinTu.Library.Business;  

namespace YuLinTu.Component.BatchDataBaseTask
{
    /// <summary>
    /// 空间数据处理
    /// </summary>
    public class ArcSpaceDataProgress : Task
    {
        #region Fields
        #endregion

        #region Propertys

        /// <summary>
        /// 矢量保存路径
        /// </summary>
        public string ShapeFilePath { get; set; }

        /// <summary>
        /// 坐标系
        /// </summary>
        public string SpatialText { get; set; }

        /// <summary>
        /// 导出文件配置
        /// </summary>
        public ExportFileEntity ExportFile { get; set; }

        /// <summary>
        /// 数据源上下文
        /// </summary>
        public IDbContext DbContext { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public YuLinTu.Library.Entity.Zone CurrentZone { get; set; }
        #endregion

        #region Ctor

        public ArcSpaceDataProgress(IDbContext dbContext)
        {
            DbContext = dbContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 导出Sqlite数据库中的矢量数据
        /// </summary>
        public void ExportSpaceDataBase(List<YuLinTu.Library.Entity.Zone> zones, SqliteManager manager, bool newStand = false)
        {
            if (ExportFile == null)
                ExportFile = new ExportFileEntity();
            if (manager == null)
            {
                return;
            }
            string tempPath = Path.GetTempPath();
            if (ExportFile.VictorDK.IsExport)
            {
                var fieldList = new ShapeFileInfo().DKZD;
                ProcessExportShape("导出地块时发生错误", DK.TableName, tempPath,
                    OSGeo.OGR.wkbGeometryType.wkbPolygon, fieldList, zones);
            }
        }

        /// <summary>
        /// 导出shape文件
        /// </summary>
        public void ProcessExportShape(string errorInfo, string name, string tempPath, OSGeo.OGR.wkbGeometryType geoType, List<FieldInformation> fieldList, List<YuLinTu.Library.Entity.Zone> zones)
        {
            try
            {
                ProcessExport(name, tempPath, geoType, fieldList, zones);
            }
            catch (Exception ex)
            {
                this.ReportError(errorInfo + "，详情请查看日志");
                LogWrite.WriteErrorLog(errorInfo + ex.Message, ex);
            }
        }


        /// <summary>
        /// 处理文件导出
        /// </summary>
        public void ProcessExport(string layerName, string tempPath, OSGeo.OGR.wkbGeometryType geoType, List<FieldInformation> fieldList, List<YuLinTu.Library.Entity.Zone> zones)
        {
            if (zones == null || zones.Count == 0)
            {
                return;
            }
            int exportCount = 0;
            var shapName = CurrentZone != null && CurrentZone.Name.IsNotNullOrEmpty() ? 
                CurrentZone.Name+"地块" + ".shp":layerName+".shp";
            string fileName = Path.Combine(ShapeFilePath, shapName);
            DelteShapeFile(fileName);
            var writer = new ShapeFileWriter<ExportEntity>(OSGeo.OGR.wkbGeometryType.wkbPoint, ShapeFilePath);
            var layer = writer.InitiallLayer(fileName, layerName, geoType, null);
            var contractLandWorkStation = DbContext.CreateContractLandWorkstation();
            var zoneStation = DbContext.CreateZoneWorkStation();

            double zoneCount = zones.Count;
            int zoneStart = 0;
            int progressIndex = 0;
            foreach (var zone in zones)//循环地域集合，写入矢量数据
            {
                progressIndex = 2 + (int)((++zoneStart / zoneCount) * 97);
                this.ReportProgress(progressIndex, string.Format("正在导出{0}的数据...", zone.FullName));
                var landList = contractLandWorkStation.GetCollection(zone.FullCode, eLevelOption.Self);
                if (landList == null || landList.Count == 0)
                {
                    continue;
                }
                var list = GetExportEntity(landList);
                var err = writer.WriteVectorFile(layer, list);
                if (err.IsNotNullOrEmpty())
                {
                    this.ReportError(string.Format("导出{0}下地块发生错误:{1}", zone.FullName, err));
                    break;
                }
                exportCount += list.Count;
            }

            layer.Dispose();
            writer.Dispose();
            this.ReportInfomation(string.Format("成功导出地块{0}条", exportCount));
            InitalzeGeometryCoordinate(fileName);
        }

        /// <summary>
        /// 获取导出数据中间实体
        /// </summary>
        /// <param name="landList"></param>
        /// <returns></returns>
        private List<ExportEntity> GetExportEntity(List<Library.Entity.ContractLand> landList)
        {
            List<ExportEntity> exportData = new List<ExportEntity>();

            if (landList != null && landList.Count != 0)
            {
                landList.ForEach(s =>
                   exportData.Add(new ExportEntity()
                   {
                       ContractorName = s.OwnerName,
                       LandNumber = s.LandNumber,
                       ZoneCode = s.ZoneCode,
                       Shape = s.Shape
                   }));
            }

            return exportData;
        }

        /// <summary>
        /// 删除矢量文件
        /// </summary>
        private bool DelteShapeFile(string shpPath)
        {
            try
            {
                if (File.Exists(shpPath))
                    File.Delete(shpPath);
                var dbfPath = Path.ChangeExtension(shpPath, ".dbf");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".shx");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".sbn");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".sbx");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".prj");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
                dbfPath = Path.ChangeExtension(shpPath, ".shp.xml");
                if (File.Exists(dbfPath))
                    File.Delete(dbfPath);
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog("删除临时文件:" + ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化坐标系
        /// </summary>
        public void InitalzeGeometryCoordinate(string fileName)
        {
            if (string.IsNullOrEmpty(SpatialText))
            {
                return;
            }
            string prjFile = fileName;
            if (Path.GetExtension(fileName) == ".shp")
            {
                prjFile = Path.ChangeExtension(fileName, ".prj");
            }
            else
            {
                prjFile = fileName + ".prj";
            }
            System.IO.File.WriteAllText(prjFile, SpatialText);
        }
        #endregion
    }
}
