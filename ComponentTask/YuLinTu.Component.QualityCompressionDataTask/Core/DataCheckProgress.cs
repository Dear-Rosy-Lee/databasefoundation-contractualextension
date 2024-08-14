using GeoAPI.CoordinateSystems;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Quality.Business.Entity;
using Quality.Business.TaskBasic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Documents;
using Xceed.Wpf.DataGrid;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using ZoneDto = YuLinTu.Library.Entity.Zone;

namespace YuLinTu.Component.QualityCompressionDataTask
{
    public class DataCheckProgress : Task
    {
        /// <summary>
        /// 图形SRID
        /// </summary>
        private int srid;

        private bool flag = true;

        private string filePath;

        /// <summary>
        /// 文件路径
        /// </summary>
        private FilePathInfo currentPath;

        /// <summary>
        /// 服务
        /// </summary>
        public IDbContext LocalService { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public QualityCompressionDataArgument DataArgument { get; set; }

        public bool Check()
        {
            var zoneStation = LocalService.CreateZoneWorkStation();
            var landStation = LocalService.CreateContractLandWorkstation();
            var countyZoneCode = zoneStation.GetByZoneLevel(Library.Entity.eZoneLevel.County).FirstOrDefault().FullCode;
            var villageZoneCode = DataArgument.CheckFilePath.Split('\\').Last();
            srid = GetSrid(LocalService);

            var currentPath = new FilePathInfo();
            currentPath.ShapeFileList = new List<FileCondition>();
            var shapeFileCondition = new FileCondition();
            shapeFileCondition.FilePath = $"{DataArgument.CheckFilePath}\\DK{countyZoneCode}2024.shp";
            currentPath.ShapeFileList.Add(shapeFileCondition);
            var shpProcess = new SpaceDataOperator(srid, currentPath.ShapeFileList);

            var shpLands = shpProcess.InitiallLandList(currentPath.ShapeFileList, villageZoneCode);
            var lands = landStation.GetCollection(villageZoneCode);
            CreateLog();

            foreach (var shpLand in shpLands)
            {
                if (shpLand.YDKBM == "")
                {
                    lands.ForEach(x =>
                    {
                        var res = x.Shape.Intersects(shpLand.Shape as Spatial.Geometry);
                        if (res == true)
                        {
                            WriteLog($"错误，新增地块{shpLand.QLRMC}地块编码为{shpLand.DKBM}；" +
                                     $"与{x.OwnerName}，地块编码为{x.LandNumber}地块有重叠部分。");
                            flag = false;
                        }
                    });
                }
                else
                {
                    var land = lands.Where(x => x.LandNumber == shpLand.YDKBM).FirstOrDefault();
                    var geo = shpLand.Shape as Spatial.Geometry;
                    var res = land.Shape.Contains(geo);
                    if (res == false)
                    {
                        var intersectionArea = land.Shape.Intersection(geo).Area();
                        if (intersectionArea < 1)
                        {
                            WriteLog($"错误，地块编码为{shpLand.DKBM}的地块未与原地块相交，且相交面积少于1平方米");
                            flag = false;
                        }
                    }
                }
            }
            if (flag == true)
                WriteLog($"检查通过！{DateTime.Now}");
            return flag;
        }

        /// <summary>
        /// 获取Srid
        /// </summary>
        private int GetSrid(IDbContext localService)
        {
            var targetSpatialReference = localService.CreateSchema().GetElementSpatialReference(
                    ObjectContext.Create(typeof(Library.Entity.ContractLand)).Schema,
                    ObjectContext.Create(typeof(Library.Entity.ContractLand)).TableName);
            if (targetSpatialReference == null)
                throw new Exception("无法获取到本地是数据库表ZD_CBD的坐标信息");
            return targetSpatialReference.WKID;
        }

        private void CreateLog()
        {
            // 指定文件夹路径
            string folderPath = DataArgument.ResultFilePath;
            string fileName = "检查结果.txt";
            // 合成完整文件路径
            filePath = Path.Combine(folderPath, fileName);
            File.WriteAllText(filePath, "检查结果记录:");
        }

        private void WriteLog(string mes)
        {
            File.AppendAllText(filePath, mes + Environment.NewLine);
        }
    }
}