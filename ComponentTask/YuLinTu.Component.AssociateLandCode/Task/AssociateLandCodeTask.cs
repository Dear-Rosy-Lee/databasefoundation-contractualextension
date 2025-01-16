using Microsoft.Practices.ObjectBuilder2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using Zone = YuLinTu.Library.Entity.Zone;

namespace YuLinTu.Component.AssociateLandCode
{
    [TaskDescriptor(IsLanguageName = false, Name = "关联确权数据编码",
    Gallery = "汇交成果处理",
    UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
    UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class AssociateLandCodeTask : TaskGroup
    {
        public AssociateLandCodeTask()
        {
            Name = "关联确权数据编码";
            Description = "关联确权数据编码";
        }

        #region Fields

        private Zone currentZone; //当前地域
        private AssociateLandCodeArgument argument; 
        private IDbContext dbContext;  //待合并数据源
        private IDbContext oldDbContext;  //本地数据源
        private double averagePercent;  //平均百分比
        private double currentPercent;  //当前百分比
        private string resfile;
        private int index;
        private int cindex;

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
                    this.ReportError(string.Format("关联原编码出错!"));
                    return;
                }
                //UpdateLandBoundary();
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnGo(关联原编码失败!)", ex.Message + ex.StackTrace);
                this.ReportError(string.Format("关联原编码出错!"));
                return;
            }
            this.ReportProgress(100);
            this.ReportInfomation("关联原编码成功。");
        }

        #endregion Method - Override

        #region Method - Private

        private bool Beginning()
        {
            index = 1;
            cindex = 1;
            dbContext = DataBaseSource.GetDataBaseSourceByPath(argument.DatabaseFilePath);
            oldDbContext = DataBaseSource.GetDataBaseSourceByPath(argument.OldDatabaseFilePath);
            var zoneStation = dbContext.CreateZoneWorkStation();
            var allZones = zoneStation.GetAll();
            var towns = allZones.Where(x => x.Level == eZoneLevel.Town).ToList();
            var villages = allZones.Where(x => x.Level == eZoneLevel.Village).ToList();
            var groups = allZones.Where(x => x.Level == eZoneLevel.Group).ToList();
            double vpPercent = 50 / (double)villages.Count;
            double landPercent = 50 / (double)groups.Count;
            this.ReportInfomation("开始挂接原承包方编码");
            resfile = CreateLog();
            foreach (var town in towns)
            {
                this.ReportInfomation($"开始挂接{town.Name}原承包方编码");
                var townVillages = allZones.Where(x => x.Level == eZoneLevel.Village && x.FullCode.StartsWith(town.FullCode)).ToList();
                AssociateVpCode(town,townVillages, vpPercent, villages.Count, resfile);
            }
            this.ReportInfomation("开始挂接原地块编码");
            
            foreach (var town in towns)
            {
                this.ReportInfomation($"开始挂接{town.Name}原地块编码");
                var townVillages = allZones.Where(x => x.Level == eZoneLevel.Village && x.FullCode.StartsWith(town.FullCode)).ToList();
                var townGroups = allZones.Where(x => x.Level == eZoneLevel.Group && x.FullCode.StartsWith(town.FullCode)).ToList();
                AssociateLandCode(town, townVillages,townGroups, landPercent, groups.Count);
            }

            return true;
        }
        public void AssociateVpCode(Zone currentTown, List<Zone> villages, double vpPercent, int zoneListCount,string resfile)
        {
            
            var vpStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var vpOldStation = oldDbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            //获取当前镇级原承包方
            var oldVps = vpOldStation.GetByZoneCode(currentTown.FullCode, eLevelOption.Subs);
            foreach (var village in villages)
            {
                this.ReportProgress(3 + (int)(index * vpPercent), string.Format("({0}/{1})挂接{2}原承包方编码", index, zoneListCount, village.FullName));
                var vps = vpStation.GetByZoneCode(village.FullCode, eLevelOption.Subs);
                vps = ExecuteUpdateVp(vps, oldVps,village);
                vpStation.UpdatePersonList(vps);
                index++;
            }
            
        }

        
        public void AssociateLandCode(Zone currentTwon, List<Zone> villages, List<Zone> groups, double landPercent, int zoneListCount)
        {
            var vpOldStation = oldDbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var landOldStation = oldDbContext.CreateContractLandWorkstation();
            //获取当前镇级所有承包方
            var oldVps = vpOldStation.GetByZoneCode(currentTwon.FullCode, eLevelOption.Subs);
            //获取当前镇级所有地块
            this.ReportProgress(53,$"正在获取{currentTwon.Name}的地块数据，请稍等");
            var geoOldLands = landOldStation.GetShapeCollection(currentTwon.FullCode, eLevelOption.Subs);

            foreach (var village in villages) 
            {
                var villageGroups = groups.Where(x=>x.FullCode.StartsWith(village.FullCode)).ToList();
                AssociteLand(village, villageGroups, landPercent, zoneListCount, oldVps, geoOldLands);
            }

        }
        private void AssociteLand(Zone currentVillage, List<Zone> groups,double landPercent, int zoneListCount,List<VirtualPerson> oldVps,List<ContractLand> oldLands)
        {
            
            var vpStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var vp = vpStation.GetByZoneCode(currentVillage.FullCode, eLevelOption.Subs);
            var landStation = dbContext.CreateContractLandWorkstation();
            var geoLands = landStation.GetShapeCollection(currentVillage.FullCode, eLevelOption.Subs);

            foreach (var group in groups) 
            {
                this.ReportProgress(53 + (int)(cindex * landPercent), string.Format("({0}/{1})挂接{2}原地块编码", cindex, zoneListCount, group.FullName));
                var oldlands = GetOldLands(vp.Where(x => x.ZoneCode.StartsWith(group.FullCode)).ToList(), oldVps, oldLands);
                var upLands = ExecuteUpdateLand(vp.Where(x => x.ZoneCode.StartsWith(group.FullCode)).ToList(), oldVps,
                                  geoLands.Where(x => x.LandNumber.StartsWith(group.FullCode)).ToList(),oldlands,group);
                landStation.UpdateOldLandCode(upLands,true);
                cindex++;
            }
        }
        private void AssociteLandWithShape(List<ContractLand> contractLands,List<ContractLand> geoOldLands)
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
        public string CreateLog()
        {
            // 指定文件夹路径
            string folderPath = argument.ResultFilePath;
            string fileName = $"挂接结果{DateTime.Now.ToString("yyyy年M月d日HH时mm分")}.txt";
            // 合成完整文件路径
            folderPath = Path.Combine(folderPath, fileName);
            File.WriteAllText(folderPath, "检查结果记录:\n");
            return folderPath;
        }
        public void WriteLog(string path, string mes)
        {
             IEnumerable<string> stringCollection = new[] { mes };
             File.AppendAllLines(path, stringCollection);
        }


        private List<ContractLand> ExecuteUpdateLand(List<VirtualPerson> vps, List<VirtualPerson> oldVps, List<ContractLand> clands, List<ContractLand> oldlands,Zone group)
        {
            var ListLands = new List<ContractLand>();
            foreach (var vp in vps)
            {
                
                if (vp.OldVirtualCode != null)
                {
                    var oldvpid = oldVps.Where(x => x.ZoneCode + x.FamilyNumber.PadLeft(4, '0') == vp.OldVirtualCode).FirstOrDefault().ID;
                    var ListOldLands = oldlands.Where(x => x.OwnerId == oldvpid).ToList();
                    var lands = clands.Where(x => x.OwnerId == vp.ID).ToList();
                    
                    lands.ForEach(t =>
                    {
                        if(t.OldLandNumber == null)
                        {
                            if (ListOldLands.Where(c => c.ActualArea == t.ActualArea).ToList() != null)
                            {
                                if (ListOldLands.Where(c => c.ActualArea == t.ActualArea).FirstOrDefault() != null)
                                {
                                    t.OldLandNumber = ListOldLands.Where(c => c.ActualArea == t.ActualArea).FirstOrDefault().LandNumber;
                                    ListOldLands.Remove(ListOldLands.Where(c => c.ActualArea == t.ActualArea).FirstOrDefault());
                                    ListLands.Add(t);
                                }
                                
                            }
                            
                        }
                        
                        
                    });

                    CreateLandLogs(vp, oldVps,lands, oldlands,group);
                    
                }

            }
            return ListLands;
        }
        private List<ContractLand> GetOldLands(List<VirtualPerson> vps ,List<VirtualPerson> Oldvps, List<ContractLand> oldclands)
        {
            var ListLands = new List<ContractLand>();
            foreach (var vp in vps)
            {
                if (vp.OldVirtualCode != null)
                {
                    var oldvpid = Oldvps.Where(x => x.ZoneCode + x.FamilyNumber.PadLeft(4, '0') == vp.OldVirtualCode).FirstOrDefault().ID;
                    var oldlands = oldclands.Where(x => x.OwnerId == oldvpid).ToList();
                    ListLands.AddRange(oldlands);
                }
            }
            return ListLands;
        }
        private List<VirtualPerson> ExecuteUpdateVp(List<VirtualPerson> vps, List<VirtualPerson> oldVps,Zone village)
        {
            var ListVps = new List<VirtualPerson>();
            foreach (var vp in vps)
            {
                if (vp.OldVirtualCode == null)
                {
                    oldVps.ForEach(x =>
                    {
                        if (x.Number == vp.Number)
                        {
                            vp.OldVirtualCode = x.ZoneCode + x.FamilyNumber.PadLeft(4, '0');
                            ListVps.Add(vp);
                        }
                        else
                        {
                            foreach (var sharePerson in x.SharePersonList)
                            {
                                if (sharePerson.ICN == vp.Number)
                                {
                                    vp.OldVirtualCode = x.ZoneCode + x.FamilyNumber.PadLeft(4, '0');
                                    ListVps.Add(vp);
                                }
                            }
                        }
                      
                    });
                    
                }
                if (vp.OldVirtualCode == null)
                {
                    WriteLog(resfile,$"{village.FullName}{village.FullCode}地域下的承包方编码为{vp.ZoneCode + vp.FamilyNumber.PadLeft(4, '0')}" +
                        $"的农户未成功挂接原承包方编码请进行比对核实");
                }
            }
            return ListVps;

        }
        private void CreateLandLogs(VirtualPerson vp, List<VirtualPerson> oldVps, List<ContractLand> clands, List<ContractLand> oldlands,Zone group)
        {
            
            var oldvpid = oldVps.Where(x => x.ZoneCode + x.FamilyNumber.PadLeft(4, '0') == vp.OldVirtualCode).FirstOrDefault().ID;
            oldlands = oldlands.Where(x => x.OwnerId == oldvpid).ToList();
            var ListOldLand = oldlands;
            foreach (var land in clands)
            {
                if(!ListOldLand.Where(x => x.LandNumber == land.OldLandNumber).IsNullOrEmpty())
                    ListOldLand.Remove(ListOldLand.Where(x => x.LandNumber == land.OldLandNumber).FirstOrDefault());
            }
            
            if (!ListOldLand.IsNullOrEmpty())
            {
                var cland = clands.Where(x => x.OldLandNumber.IsNullOrEmpty()).ToList();
                if (cland != null)
                {
                    cland.ForEach(t => 
                    {
                        var res = string.Join(",", ListOldLand.Select(x => x.LandNumber));
                        WriteLog(resfile, $"{group.FullName}{group.FullCode}地域下的地块编码为{t.LandNumber}地块，未挂接确权地块编码；" +
                            $"当前承包方中未挂接的确权地块编码有{res},请进行对比核实");

                    });
                }
               
            }
            
        }
        private void UpdateLandBoundary()
        {
            
            //foreach (var entity in contractLands)
            //{
            //    entity.NeighborEast = geoOldLands.Where(x => x.LandNumber == entity.OldLandNumber).FirstOrDefault().NeighborEast;
            //    entity.NeighborWest = geoOldLands.Where(x => x.LandNumber == entity.OldLandNumber).FirstOrDefault().NeighborWest;
            //    entity.NeighborSouth = geoOldLands.Where(x => x.LandNumber == entity.OldLandNumber).FirstOrDefault().NeighborSouth;
            //    entity.NeighborNorth = geoOldLands.Where(x => x.LandNumber == entity.OldLandNumber).FirstOrDefault().NeighborNorth;
            //    entity.Name = geoOldLands.Where(x => x.LandNumber == entity.OldLandNumber).FirstOrDefault().Name;
            //    landStation.UpdateLandBoundary(entity);
            //}

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
