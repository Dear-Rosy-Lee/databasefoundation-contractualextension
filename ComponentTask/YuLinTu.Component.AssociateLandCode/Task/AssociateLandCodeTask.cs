using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.AssociateLandCode
{
    [TaskDescriptor(IsLanguageName = false, Name = "关联地块编码",
    Gallery = "汇交成果处理",
    UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
    UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class AssociateLandCodeTask : TaskGroup
    {
        public AssociateLandCodeTask()
        {
            Name = "关联原地块编码";
            Description = "关联原地块编码";
        }

        #region Fields

        private Zone currentZone; //当前地域
        private AssociateLandCodeArgument argument; 
        private IDbContext dbContext;  //待合并数据源
        private IDbContext oldDbContext;  //本地数据源
        private double averagePercent;  //平均百分比
        private double currentPercent;  //当前百分比

        #endregion Fields

        #region Methods

        #region Method - Override
        protected override void OnGo()
        {
            this.ReportProgress(0, "开始验证参数...");
            this.ReportInfomation("开始验证参数...");
            System.Threading.Thread.Sleep(200);
            if (!ValidateArgs())
                return;
            System.Threading.Thread.Sleep(200);
            try
            {
                if (!Beginning())
                {
                    this.ReportError(string.Format("关联原地块编码出错!"));
                    return;
                }
                UpdateLandBoundary();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnGo(关联原地块编码失败!)", ex.Message + ex.StackTrace);
                this.ReportError(string.Format("关联原地块编码出错!"));
                return;
            }
            this.ReportProgress(100);
            this.ReportInfomation("关联原地块编码完成。");
        }

        #endregion Method - Override

        #region Method - Private

        private bool Beginning()
        {
            dbContext = DataBaseSource.GetDataBaseSourceByPath(argument.DatabaseFilePath);
            oldDbContext = DataBaseSource.GetDataBaseSourceByPath(argument.OldDatabaseFilePath);
            var zoneStation = dbContext.CreateZoneWorkStation();
            var allZones = zoneStation.GetAll();
            var villages = allZones.Where(x => x.Level == eZoneLevel.Village).ToList();
            foreach(var village in villages)
            {
                AssociateLandCode(village);
            }
            
            return true;
        }

        public void AssociateLandCode(Zone village)
        {
            var zoneStation = dbContext.CreateZoneWorkStation();
            var zonesToProvince = zoneStation.GetAllZonesToProvince(village);
            var groups = zonesToProvince.Where(x => x.Level == eZoneLevel.Group);
            var landStation = dbContext.CreateContractLandWorkstation();
            var landOldStation = oldDbContext.CreateContractLandWorkstation();
            var geoLands = landStation.GetShapeCollection(village.FullCode, eLevelOption.Subs).ToList();
            var geoOldLands = landOldStation.GetShapeCollection(village.FullCode, eLevelOption.Subs);
            foreach (var group in groups) 
            { 
                AssociteLand(geoLands.Where(x => x.LandNumber.StartsWith(group.Code)).ToList(),
                             geoOldLands.Where(x => x.LandNumber.StartsWith(group.Code)).ToList());
            }
            var noOldNumberLand = geoLands.Where(x => x.OldLandNumber == null).ToList();

            // 查找重复的 LandCode
            var duplicateLandEntries = geoLands
              .GroupBy(l => l.OldLandNumber)          // 按 LandCode 分组
              .Where(g => g.Count() > 1)         // 只保留重复的
              .SelectMany(g => g)                // 展开每个分组，获取重复的所有条目
              .Select(l => new { l.LandNumber, l.OldLandNumber })  // 选择 Id 和 LandCode
              .ToList();

            foreach (var entity in geoLands.Where(x => x.OldLandNumber != null))
            {
                landStation.UpdateOldLandCode(entity);
            }

        }
        private void AssociteLand(List<ContractLand> contractLands,List<ContractLand> geoOldLands)
        {
            foreach(var shpLand in contractLands)
            {
                try
                {
                    var shpArea = shpLand.Shape.Area();
                    var items = new KeyValueList<double, string>();
                    geoOldLands.ForEach(t =>
                    {
                        if (shpLand.OldLandNumber != null)
                        {

                        }
                        var res = t.Shape.Intersects(shpLand.Shape as Spatial.Geometry);
                        if (res == true)
                        {
                            var IntersectArea = t.Shape.Intersection(shpLand.Shape as Spatial.Geometry).Area();
                            items.Add(IntersectArea / shpArea, t.LandNumber);
                        }
                    });
                    if (items.Count != 0)
                    {
                        var maxKeyValue = items/*.Where(kv => kv.Key > 0.6)*/ // 过滤条件
                                               .Aggregate((l, r) => l.Key > r.Key ? l : r);
                        shpLand.OldLandNumber = geoOldLands.Where(x => x.LandNumber == maxKeyValue.Value.ToString()).FirstOrDefault().LandNumber;
                    }

                }
                catch (Exception ex)
                {

                }
            }
        }
        private void UpdateLandBoundary()
        {
            var landStation = dbContext.CreateContractLandWorkstation();
            var landOldStation = oldDbContext.CreateContractLandWorkstation();
            var contractLands = landStation.GetShapeCollection(currentZone.FullCode, eLevelOption.Subs).Where(x => x.OldLandNumber != null).ToList();
            var geoOldLands = landOldStation.GetShapeCollection(currentZone.FullCode, eLevelOption.Subs);
            foreach (var entity in contractLands)
            {
                entity.NeighborEast = geoOldLands.Where(x => x.LandNumber == entity.OldLandNumber).FirstOrDefault().NeighborEast;
                entity.NeighborWest = geoOldLands.Where(x => x.LandNumber == entity.OldLandNumber).FirstOrDefault().NeighborWest;
                entity.NeighborSouth = geoOldLands.Where(x => x.LandNumber == entity.OldLandNumber).FirstOrDefault().NeighborSouth;
                entity.NeighborNorth = geoOldLands.Where(x => x.LandNumber == entity.OldLandNumber).FirstOrDefault().NeighborNorth;
                entity.Name = geoOldLands.Where(x => x.LandNumber == entity.OldLandNumber).FirstOrDefault().Name;
                landStation.UpdateLandBoundary(entity);
            }

        }

        private bool ValidateArgs()
        {
            var args = Argument as AssociateLandCodeArgument;

            if (args == null)
            {
                this.ReportError(string.Format("参数错误!"));
                return false;
            }
            argument = args;
            if (args == null)
            {
                this.ReportError(string.Format("参数错误!"));
                return false;
            }
            if (args.DatabaseFilePath.IsNullOrBlank())
            {
                this.ReportError(string.Format("请选择待关联数据库。"));
                return false;
            }
            if (args.OldDatabaseFilePath.IsNullOrBlank())
            {
                this.ReportError(string.Format("请选择原地块编码数据库。"));
                return false;
            }

            this.ReportInfomation(string.Format("关联原地块编码数据参数正确。"));
            return true;
        }

        #endregion Method - Private - Pro

        #endregion Methods

    }
}
