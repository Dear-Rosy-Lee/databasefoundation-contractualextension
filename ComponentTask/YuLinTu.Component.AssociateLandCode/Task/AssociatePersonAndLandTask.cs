using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Scripting.Utils;
using YuLinTu.Component.Common;
using YuLinTu.Data;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Log;
using Zone = YuLinTu.Library.Entity.Zone;

namespace YuLinTu.Component.AssociateLandCode
{
    [TaskDescriptor(IsLanguageName = false, Name = "按发包方关联确权数据",
    Gallery = "汇交成果处理",
    UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
    UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class AssociatePersonAndLandTask : TaskGroup
    {
        public AssociatePersonAndLandTask()
        {
            Name = "按发包方关联确权数据";
            Description = "按发包方关联两份确权数据，以便输出摸底调查表";
        }

        #region Fields

        private Zone currentZone; //当前地域
        private AssociatePersonAndLandArgument argument;
        private List<string> jtmcs = new List<string>();
        /// <summary>
        /// 待合并数据源
        /// </summary>
        private IDbContext dbContext;  //

        /// <summary>
        /// 本地数据源
        /// </summary>
        private IDbContext oldDbContext;  //
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
            jtmcs.Clear();
            jtmcs.AddRange(new List<string>() { "村集体", "社集体", "集体", "集体地", "组集体", "共有", "争议地" });
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
                this.ReportError(string.Format("关联原编码出错!" + ex.Message));
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
            DataBaseHelper.TryUpdateDatabase(dbContext);
            oldDbContext = DataBaseSource.GetDataBaseSourceByPath(argument.OldDatabaseFilePath);
            DataBaseHelper.TryUpdateDatabase(oldDbContext);
            var zoneStation = dbContext.CreateZoneWorkStation();
            var allZones = zoneStation.GetAll();

            //Dictionary<string, RelationPerson> persondic = new Dictionary<string, RelationPerson>();
            //var oldvpQuery = oldDbContext.CreateQuery<LandVirtualPerson>();
            //oldvpQuery.ForEach((i, p, vp) =>
            //{
            //    foreach (var q in vp.SharePersonList)
            //    {
            //        if (!persondic.ContainsKey(q.ICN))
            //        {
            //            persondic.Add(q.ICN, new RelationPerson()
            //            {
            //                ICN = q.ICN,
            //                VirtualPeronId = q.ID,
            //                IsRepresent = q.ICN == vp.Number,
            //                Name = q.Name,
            //                HH = vp.FamilyNumber,
            //                ZoneCode = vp.ZoneCode
            //            });
            //        }
            //    }
            //    return true;
            //});

            //TO获取关联的发包方表 
            var zrlist = GetRelationZoneFromFile(argument.RelationExcelFilePath);
            var grouplist = zrlist.GroupBy(t => t.OldZoneCode).Where(w => w.Count() > 1).ToList();
            if (grouplist.Count > 0)
            {
                foreach (var zr in grouplist)
                {
                    this.ReportError($"旧地域{zr.Key}中无法挂接到多个新地域,程序无法自动处理，请手动处理数据！");
                }
                return false;
            }

            //TO清空删除数据的记录表
            var vpdquery = dbContext.CreateQuery<VirtualPerson_Del>();
            var lddquery = dbContext.CreateQuery<ContractLand_Del>();

            var towns = allZones.Where(x => x.Level == eZoneLevel.Town).ToList();
            var villages = allZones.Where(x => x.Level == eZoneLevel.Village).ToList();
            var groups = allZones.Where(x => x.Level == eZoneLevel.Group).ToList();

            double vpPercent = 90 / (double)groups.Count;
            this.ReportInfomation("开始挂接挂接数据");
            this.ReportProgress(1, "开始挂接挂接数据...");
            resfile = CreateLog();
            foreach (var town in towns)
            {
                var tzrzone = zrlist.FindAll(t => t.NewZoneCode.StartsWith(town.FullCode));
                if (tzrzone.Count == 0)
                    continue;
                vpdquery.Where(t => t.ZoneCode.StartsWith(town.FullCode)).Delete().Save();
                lddquery.Where(t => t.DYBM.StartsWith(town.FullCode)).Delete().Save();
                this.ReportInfomation($"开始挂接{town.Name}下的数据");
                var townVillages = allZones.Where(x => x.Level == eZoneLevel.Village && x.FullCode.StartsWith(town.FullCode)).ToList();
                AssociateCode(town, townVillages, vpPercent, groups.Count, tzrzone);
            }
            this.ReportInfomation("挂接数据完成");
            return true;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private List<RelationZone> GetRelationZoneFromFile(string filepath)
        {
            var lst = new List<RelationZone>();
            if (!File.Exists(filepath))
                return lst;
            var reader = new ZoneExcelReader();
            lst = reader.InitalizeZoneData(filepath);
            return lst;
        }

        /// <summary>
        /// 挂接数据
        /// </summary>
        public void AssociateCode(Zone currentTown, List<Zone> villages, double vpPercent, int zoneListCount, List<RelationZone> relationZones)
        {
            var sendStation = dbContext.CreateSenderWorkStation();
            var vpStation = dbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var vpOldStation = oldDbContext.CreateVirtualPersonStation<LandVirtualPerson>();
            var senders = sendStation.GetTissues(currentTown.FullCode, eLevelOption.Subs);//获取当前镇级新发包方

            var landOldStation = oldDbContext.CreateContractLandWorkstation();
            var landStation = dbContext.CreateContractLandWorkstation();

            var cldquery = dbContext.CreateQuery<ContractLand_Del>();
            var vpdquery = dbContext.CreateQuery<VirtualPerson_Del>();
            foreach (var village in villages)
            {
                var vps = vpStation.GetByZoneCode(village.FullCode, eLevelOption.Subs);
                var geoLands = landStation.GetCollection(village.FullCode, eLevelOption.Subs);
                var sds = senders.FindAll(t => t.ZoneCode.StartsWith(village.FullCode)).OrderBy(o => o.ZoneCode).ToList();
                foreach (var sd in sds)
                {
                    var upvps = new List<VirtualPerson>();
                    var deloldvps = new List<VirtualPerson_Del>();
                    var uplands = new List<ContractLand>();
                    var deloldlds = new List<ContractLand_Del>();
                    this.ReportProgress(3 + (int)(index * vpPercent), string.Format("({0}/{1})挂接{2}下的数据", index, zoneListCount, sd.Name));
                    var nvps = vps.FindAll(t => t.ZoneCode == sd.ZoneCode);//新承包方
                    if (nvps.Count == 0)
                        continue;
                    nvps.ForEach(t => t.OldVirtualCode = "");
                    var newzons = relationZones.FindAll(t => t.NewZoneCode == sd.ZoneCode);
                    var zonecodelist = newzons.Select(t => t.OldZoneCode).ToList();
                    if (zonecodelist.Count == 0)
                        continue;
                    var zoneNamelist = newzons.Select(t => t.OldName).ToList();
                    this.ReportInfomation($"挂接{sd.Name}下的数据：{zoneNamelist.JoinStrings("、")}");
                    var oldVps = new List<VirtualPerson>();//原承包方
                    var oldLands = new List<ContractLand>();//原地块
                    foreach (var oldz in zonecodelist)
                    {
                        var tvps = vpOldStation.GetByZoneCode(oldz, eLevelOption.Self);//获取原承包方
                        var tlds = landOldStation.GetCollection(oldz, eLevelOption.Self);//获取原地块 
                        if (oldz == sd.ZoneCode)
                        {
                            var tnp = nvps[0];
                            var top = tvps.Find(t => t.FamilyNumber == tnp.FamilyNumber || t.ZoneCode == tnp.ZoneCode);
                            if (top != null)
                            {
                                this.ReportError($"挂接原库中未找到户号为{tnp.FamilyNumber}、地域编码为{tnp.ZoneCode}的农户");
                            }
                            else
                            {
                                if (!CheckVirtualPersonIsSame(tnp, top))
                                {
                                    this.ReportError($"户号为{tnp.FamilyNumber}的承包方在新旧数据库中不是同一个，请确认新数据是否重新编码？请确保新编码未占用旧编码！");
                                }
                            }
                        }
                        oldVps.AddRange(tvps);
                        oldLands.AddRange(tlds);//获取原地块 
                    }
                    //var ovps = vps.FindAll(t => zonecodelist.Contains(t.ZoneCode));//旧承包方
                    var lstvps = ExecuteUpdateVp(sd, nvps, oldVps,/* relationZones,*/ deloldvps);
                    upvps.AddRange(lstvps);
                    var nlds = geoLands.FindAll(t => t.ZoneCode == sd.ZoneCode);//新地块
                    nlds.ForEach(l => l.OldLandNumber = "");
                    var lstlds = AssociteLand(sd, nvps, lstvps, deloldvps, oldVps, nlds, oldLands, relationZones, deloldlds);
                    var upldnums = new HashSet<string>();
                    foreach (var item in lstlds)
                    {
                        if (!upldnums.Contains(item.LandNumber))
                            upldnums.Add(item.LandNumber);
                    }
                    foreach (var item in nlds)
                    {
                        if (upldnums.Contains(item.LandNumber))
                            continue;
                        item.OldLandNumber = "";
                    }
                    uplands.AddRange(nlds);
                    //deloldlds.AddRange(deloldldtemp);
                    index++;
                    this.ReportInfomation($"挂接{sd.Name}下的数据完成，承包方:{lstvps.Count}个，未关联原承包方{deloldvps.Count}个, 地块:{lstlds.Count}块,未关联地块{deloldlds.Count}块");

                    if (argument.SearchInShape)
                    {
                        SearchSameInShape(uplands.FindAll(t => t.OldLandNumber == ""),
                            oldLands, deloldlds);
                    }
                    vpStation.UpdatePersonList(vps);
                    landStation.UpdateOldLandCode(uplands, true);

                    try
                    {
                        dbContext.BeginTransaction();
                        //TO 删除承包方入库
                        foreach (var dvp in deloldvps)
                        {
                            vpdquery.Add(dvp).Save();
                        }
                        //TO 删除地块入库
                        foreach (var dld in deloldlds)
                        {
                            cldquery.Add(dld).Save();
                        }
                        dbContext.CommitTransaction();
                        this.ReportProgress(3 + (int)(index * vpPercent), string.Format("({0}/{1})挂接{2}下的数据完成", index, zoneListCount, village.FullName));
                    }
                    catch (Exception ex)
                    {
                        Log.WriteException(this, "", ex.ToString());
                        dbContext.RollbackTransaction();
                        throw new Exception("关联数据出错" + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 按图形相似性进行图形的查找
        /// </summary>
        private void SearchSameInShape(List<ContractLand> listnewlands, List<ContractLand> listOldLands, List<ContractLand_Del> dellands)
        {
            if (dellands.Count == 0)
                return;
            HashSet<Guid> guids = new HashSet<Guid>();
            foreach (var item in dellands)
            {
                guids.Add(item.ID);
            }
            var dellandents = listOldLands.FindAll(t => guids.Contains(t.ID));
            if (dellandents.Count == 0)
                return;
            GeometrySimilarityChecker checker = new GeometrySimilarityChecker();
            foreach (var land in listnewlands)
            {
                var list = new List<KeyValue<double, ContractLand>>();
                foreach (var item in dellandents)
                {
#if DEBUG
                    if (land.LandNumber == "5116022162090300688" && item.LandNumber == "5116022162090700258")
                    {
                    }
#endif
                    var p = checker.CheckSimilarity((NetTopologySuite.Geometries.Geometry)land.Shape.Instance,
                        (NetTopologySuite.Geometries.Geometry)item.Shape.Instance);
                    if (p > 0.85)
                    {
                        list.Add(new KeyValue<double, ContractLand>(p, item));
                    }

                }
                if (list.Count == 0)
                    continue;
                var nlist = list.OrderByDescending(t => t.Key).ToList();
                land.OldLandNumber = nlist[0].Value.LandNumber;
                dellands.RemoveAll(l => l.ID == nlist[0].Value.ID);
                dellandents.Remove(nlist[0].Value);
            }
        }

        /// <summary>
        /// 关联地块
        /// </summary>
        private List<ContractLand> AssociteLand(CollectivityTissue sender, List<VirtualPerson> nvps, List<VirtualPerson> revps, List<VirtualPerson_Del> delvps, List<VirtualPerson> ovps,
            List<ContractLand> geoLands, List<ContractLand> oldLands, List<RelationZone> relationZones, List<ContractLand_Del> dellands)
        {
            var listLands = new List<ContractLand>();
            var newzons = relationZones.FindAll(t => t.NewZoneCode == sender.ZoneCode);
            var zonecodelist = newzons.Select(t => t.OldZoneCode).ToList();
            if (zonecodelist.Count == 0)
                return listLands;
            var olds = oldLands.FindAll(t => zonecodelist.Contains(t.ZoneCode));//旧地块
            var jtdkset = new HashSet<string>();
            var rlandidset = new HashSet<Guid>();
            var rvpidset = new HashSet<Guid>();

            foreach (var vp in nvps)//先按原地块编码关联一次
            {
                var lands = geoLands.Where(x => x.OwnerId == vp.ID).ToList();//承包方下的现有地块 
                if (lands.Count == 0)
                    continue;
                List<ContractLand> listOldLands = new List<ContractLand>();
                if (jtmcs.Contains(vp.Name))
                {
                    listOldLands.Clear();
                    foreach (var vpcode in vp.OldVirtualCode.Split('/'))
                    {
                        var tvpid = ovps.Where(x => x.ZoneCode + x.FamilyNumber.PadLeft(4, '0') == vpcode).FirstOrDefault()?.ID;
                        if (tvpid != null)
                            listOldLands.AddRange(olds.Where(t => t.OwnerId == tvpid && !jtdkset.Contains(t.LandNumber)).ToList());
                    }
                }
                else
                {
                    foreach (var land in lands)
                    {
                        var old = oldLands.Find(t => t.LandNumber == land.OldLandNumber);
                        if (old != null && !rlandidset.Contains(old.ID))
                        {
                            rlandidset.Add(old.ID);
                            listLands.Add(land);
                        }
                        else
                        {
                            land.OldLandNumber = "";
                        }
                    }
                }
            }
            var checker = new GeometrySimilarityChecker();
            foreach (var vp in revps)//关联上的承包方
            {
                RelationLandInfo(sender.ZoneCode, vp, rvpidset, geoLands, ovps, olds, jtdkset, rlandidset, listLands, dellands, checker);
            }
            foreach (var vp in nvps)//未关联的农户下的地块，按地块编码关联一次
            {
                if (rvpidset.Contains(vp.ID))
                    continue;
                var lands = geoLands.Where(x => x.OwnerId == vp.ID).ToList();//承包方下的现有地块 
                if (lands.Count == 0)
                    continue;
                foreach (var land in lands)
                {
                    var slan = olds.FirstOrDefault(w => w.LandNumber == land.LandNumber);
                    if (slan == null)
                    {
                        continue;
                    }
                    if (!rlandidset.Contains(slan.ID))
                    {
                        land.OldLandNumber = slan.LandNumber;
                        rlandidset.Add(slan.ID);
                        listLands.Add(land);
                    }
                }
            }

            var temps = olds.Where(r => !rlandidset.Contains(r.ID)).ToList();
            foreach (var ld in temps)
            {
                if (dellands.Any(t => t.DKBM == ld.LandNumber))
                    continue;
                string oldfamilynumber = "";
                var ovp = ovps.Find(t => t.ID == ld.OwnerId);
                if (ovp != null)
                {
                    oldfamilynumber = ovp.ZoneCode.PadRight(14, '0') + ovp.FamilyNumber.PadLeft(4, '0');
                }
                dellands.Add(ContractLand_Del.ChangeDataEntity(sender.ZoneCode, ld, oldfamilynumber));
            }
            return listLands;
        }

        /// <summary>
        /// 管理地块
        /// </summary>
        /// <param name="vp">当前承包方</param>
        /// <param name="rvpidset"></param>
        /// <param name="geoLands"></param>
        /// <param name="ovps"></param>
        /// <param name="olds"></param>
        /// <param name="jtdkset"></param>
        /// <param name="rlandidset"></param>
        private void RelationLandInfo(string senderZoneCode, VirtualPerson vp, HashSet<Guid> rvpidset, List<ContractLand> geoLands,
            List<VirtualPerson> ovps, List<ContractLand> olds, HashSet<string> jtdkset, HashSet<Guid> rlandidset,
            List<ContractLand> listLands, List<ContractLand_Del> dellands, GeometrySimilarityChecker checker)
        {
            rvpidset.Add(vp.ID);
            var lands = geoLands.Where(x => x.OwnerId == vp.ID).ToList();//承包方下的现有地块 
            if (lands.Count == 0)
                return;
            List<ContractLand> listOldLands = new List<ContractLand>();
            if (jtmcs.Contains(vp.Name))
            {
                listOldLands.Clear();
                foreach (var vpcode in vp.OldVirtualCode.Split('/'))
                {
                    var tvpid = ovps.Where(x => x.ZoneCode + x.FamilyNumber.PadLeft(4, '0') == vpcode).FirstOrDefault()?.ID;
                    if (tvpid != null)
                        listOldLands.AddRange(olds.Where(t => t.OwnerId == tvpid && !jtdkset.Contains(t.LandNumber)).ToList());
                }
            }
            else
            {
                var oldvpid = ovps.Where(x => x.ZoneCode + x.FamilyNumber.PadLeft(4, '0') == vp.OldVirtualCode).FirstOrDefault().ID;
                listOldLands = olds.Where(t => t.OwnerId == oldvpid).ToList();
            }

            if (lands.Count > listOldLands.Count)
            {
                vp.ChangeSituation = eBHQK.TZZD;
            }
            else if (lands.Count < listOldLands.Count)
            {
                vp.ChangeSituation = eBHQK.TZJD;
            }
            listOldLands.RemoveAll(r => rlandidset.Contains(r.ID));

            foreach (var t in lands)
            {
#if DEBUG
                if (t.LandNumber == "5116022162030300089" || t.LandNumber == "5116022162030300118" || t.LandNumber == "5116022162030300286")
                {
                }
#endif
                if (!string.IsNullOrEmpty(t.OldLandNumber))
                    continue;
                var slan = listOldLands.FirstOrDefault(w => w.LandNumber == t.LandNumber);
                if (slan != null)
                {
                    if (!rlandidset.Contains(slan.ID))
                    {
                        t.OldLandNumber = slan.LandNumber;
                        rlandidset.Add(slan.ID);
                        listLands.Add(t);
                        continue;
                    }
                }
                var rlands = listOldLands.Where(c => c.ActualArea == t.ActualArea).ToList();
                if (rlands.Count == 0)
                {
                    t.OldLandNumber = "";
                }
                else
                {
                    bool hasrelate = false;
                    ContractLand rld = null;// rlands.Find(c => c.Name == t.Name);
                    if (argument.SearchInShape)
                    {
                        rld = rlands.Find(c => c.Name == t.Name && checker.CheckSimilarity((NetTopologySuite.Geometries.Geometry)t.Shape.Instance,
                          (NetTopologySuite.Geometries.Geometry)c.Shape.Instance) >= 0.8);
                    }
                    else
                    {
                        rld = rlands.Find(c => c.Name == t.Name);
                    }
                    if (rld != null)
                    {
                        hasrelate = true;
                        t.OldLandNumber = rld.LandNumber;
                        rlandidset.Add(rld.ID);
                    }
                    else
                    {
                        foreach (var c in rlands)
                        {
                            if (!rlandidset.Contains(c.ID))
                            {
                                if (argument.SearchInShape)
                                {
                                    var chkp = checker.CheckSimilarity((NetTopologySuite.Geometries.Geometry)t.Shape.Instance, (NetTopologySuite.Geometries.Geometry)c.Shape.Instance);
                                    if (chkp > 0.9)
                                    {
                                        hasrelate = true;
                                        t.OldLandNumber = c.LandNumber;
                                        rlandidset.Add(c.ID);
                                        break;
                                    }
                                }
                                else
                                {
                                    hasrelate = true;
                                    t.OldLandNumber = c.LandNumber;
                                    rlandidset.Add(c.ID);
                                    break;
                                }
                            }
                        }
                    }
                    if (!hasrelate)
                    {
                        t.OldLandNumber = "";
                    }
                }
                listLands.Add(t);
            }
            var delandstemp = listOldLands.Where(r => !rlandidset.Contains(r.ID)).ToList();
            foreach (var ld in delandstemp)
            {
                var tdeland = ContractLand_Del.ChangeDataEntity(senderZoneCode, ld);
                tdeland.CBFID = vp.ID;
                dellands.Add(tdeland);
            }
        }

        /// <summary>
        /// 填写日志
        /// </summary>
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

        /// <summary>
        /// 进行承包方数据关联
        /// </summary>
        private List<VirtualPerson> ExecuteUpdateVp(CollectivityTissue sender, List<VirtualPerson> vps, List<VirtualPerson> oldVps,
            /*List<RelationZone> relationZones, */List<VirtualPerson_Del> deloldvps)
        {
            var listVps = new List<VirtualPerson>();
            var receslist = new HashSet<Guid>();
            foreach (var vp in vps)
            {
                vp.OldVirtualCode = "";
                if (jtmcs.Contains(vp.Name))
                {
                    var jts = oldVps.FindAll(t => jtmcs.Contains(t.Name));
                    if (jts.Count == 0)
                    {
                        continue;
                    }
                    string ovpcode = "";
                    foreach (var x in jts)
                    {
                        ovpcode += x.ZoneCode + x.FamilyNumber.PadLeft(4, '0') + "/";
                        receslist.Add(x.ID);
                    }
                    vp.OldVirtualCode = ovpcode.TrimEnd('/');
                    listVps.Add(vp);
                    continue;
                }
                FindRelationPerson(vp, oldVps, receslist, listVps);
                //if (string.IsNullOrEmpty(vp.OldVirtualCode))
                //{
                //    WriteLog(resfile, $"发包方：{sender.Name}{sender.Code} 下的 承包方编码为{vp.ZoneCode + vp.FamilyNumber.PadLeft(4, '0')} 的农户未成功挂接原承包方,以当作新增处理！");
                //}
            }
            foreach (var vp in vps)
            {
                if (!string.IsNullOrEmpty(vp.OldVirtualCode))
                    continue;
                FindRelationPerson(vp, oldVps, receslist, listVps, true);
                if (string.IsNullOrEmpty(vp.OldVirtualCode))
                {
                    var msg = $"{sender.Name} 下的 承包方编码为{vp.ZoneCode + vp.FamilyNumber.PadLeft(4, '0')} 的农户 {vp.Name} 未成功挂接原承包方,已当作新增处理！";
                    this.ReportInfomation(msg);
                    WriteLog(resfile, msg);
                }
            }

            var delvptemp = oldVps.Where(t => !receslist.Contains(t.ID)).ToList().ConvertAll(c => c.ConvertTo<VirtualPerson_Del>());
            foreach (var dvp in delvptemp)
            {
                dvp.OldVirtualCode = dvp.ZoneCode + dvp.FamilyNumber.PadLeft(4, '0');
                dvp.ZoneCode = sender.ZoneCode;
                deloldvps.Add(dvp);
            }
            return listVps;
        }

        /// <summary>
        /// 查找关联的承包方
        /// </summary>
        /// <param name="vp">当前承包方</param>
        /// <param name="oldVps">原承包方集合</param>
        /// <param name="receslist">已关联的承包方id</param>
        /// <param name="listVps">更新数据集合</param>
        private void FindRelationPerson(VirtualPerson vp, List<VirtualPerson> oldVps, HashSet<Guid> receslist, List<VirtualPerson> listVps, bool relationShareperson = false)
        {
            bool isrelation = false;
            if (vp.Number == "512925197109246793")
            {
            }
            foreach (var x in oldVps)
            {
                if (receslist.Contains(x.ID))
                    continue;

                bool result = false;
                if (x.Number == vp.Number)//身份证号一致
                {
                    vp.OldVirtualCode = x.ZoneCode + x.FamilyNumber.PadLeft(4, '0');
                    if (vp.PersonCount == x.PersonCount)
                    {
                        vp.ChangeSituation = eBHQK.RDXXJBB;
                    }
                    else
                    {
                        vp.ChangeSituation = eBHQK.CYXXBH;
                    }
                    result = true;
                }
                else if (vp.Name == "集体" && x.Name == vp.Name)
                {
                    vp.OldVirtualCode = x.ZoneCode + x.FamilyNumber.PadLeft(4, '0');
                    result = true;
                }
                //else
                //{
                //    foreach (var sharePerson in x.SharePersonList)
                //    {
                //        if (sharePerson.ICN == vp.Number)
                //        {
                //            vp.OldVirtualCode = x.ZoneCode + x.FamilyNumber.PadLeft(4, '0');
                //            vp.ChangeSituation = eBHQK.CYXXBH;
                //            result = true;
                //        }
                //    }
                //}

                if (!result && argument.SearchInvpcode)
                {
                    if (vp.FamilyNumber == x.Number && vp.Name == x.Name)
                    {
                        vp.OldVirtualCode = x.ZoneCode + x.FamilyNumber.PadLeft(4, '0');
                        result = true;
                    }
                }


                if (result)
                {
                    isrelation = true;
                    listVps.Add(vp);
                    receslist.Add(x.ID);
                    break;
                }
            }
            if (!isrelation && relationShareperson)
            {
                //WriteLog(resfile, $"承包方编码为{vp.ZoneCode + vp.FamilyNumber.PadLeft(4, '0')} 的农户未成功挂接原承包方,开始以成员查找关联！");
                bool personrelation = false;
                foreach (var person in vp.SharePersonList)
                {
                    foreach (var x in oldVps)
                    {
#if DEBUG
                        if (x.Number == "512925197109246793")
                        {
                        }
#endif
                        if (receslist.Contains(x.ID))
                            continue;
                        if (personrelation)
                            break;
                        if (x.Number == person.ICN)//身份证号一致
                        {
                            vp.OldVirtualCode = x.ZoneCode + x.FamilyNumber.PadLeft(4, '0');
                            vp.ChangeSituation = eBHQK.CYXXBH;
                            listVps.Add(vp);
                            receslist.Add(x.ID);
                            personrelation = true;
                            break;
                        }
                        foreach (var xperson in x.SharePersonList)
                        {
                            if (xperson.ICN == person.ICN)
                            {
                                vp.OldVirtualCode = x.ZoneCode + x.FamilyNumber.PadLeft(4, '0');
                                vp.ChangeSituation = eBHQK.CYXXBH;
                                personrelation = true;
                                break;
                            }
                        }
                    }
                    if (personrelation)
                        break;
                }
            }
        }

        /// <summary>
        /// 参数检查
        /// </summary>
        /// <returns></returns>
        private bool ValidateArgs()
        {
            var args = Argument as AssociatePersonAndLandArgument;

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

        /// <summary>
        /// 检查两个承包方是否一致
        /// </summary>
        /// <returns></returns>
        public bool CheckVirtualPersonIsSame(VirtualPerson nvp, VirtualPerson ovp)
        {
            bool exist = false;
            if (nvp == null || ovp == null)
            {
                return exist;
            }
            var npersons = nvp.SharePersonList;
            var opersons = ovp.SharePersonList;

            foreach (var np in npersons)
            {
                if (opersons.Any(f => f.Name == np.Name || f.ICN == np.ICN))
                {
                    exist = true;
                    break;
                }
            }
            if (!exist)
            {
                foreach (var np in opersons)
                {
                    if (npersons.Any(f => f.Name == np.Name || f.ICN == np.ICN))
                    {
                        exist = true;
                        break;
                    }
                }

            }
            return exist;
        }

        #endregion Method - Private - Pro

        #endregion Methods
    }
}
