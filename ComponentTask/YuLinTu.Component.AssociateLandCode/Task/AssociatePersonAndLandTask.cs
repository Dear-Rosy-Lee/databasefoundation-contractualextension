using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Scripting.Utils;
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
            jtmcs.AddRange(new List<string>() { "村集体", "社集体", "集体", "集体地", "组集体" });
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
            oldDbContext = DataBaseSource.GetDataBaseSourceByPath(argument.OldDatabaseFilePath);
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
                var sds = senders.FindAll(t => t.ZoneCode.StartsWith(village.FullCode));
                var upvps = new List<VirtualPerson>();
                var deloldvps = new List<VirtualPerson_Del>();

                var uplands = new List<ContractLand>();
                var deloldlds = new List<ContractLand_Del>();
                foreach (var sd in sds)
                {
                    this.ReportProgress(3 + (int)(index * vpPercent), string.Format("({0}/{1})挂接{2}下的数据", index, zoneListCount, sd.Name));
                    var nvps = vps.FindAll(t => t.ZoneCode == sd.ZoneCode);//新承包方
                    if (nvps.Count == 0)
                        continue;
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
                        oldVps.AddRange(vpOldStation.GetByZoneCode(oldz, eLevelOption.Self));//获取原承包方
                        oldLands.AddRange(landOldStation.GetCollection(oldz, eLevelOption.Self));//获取原地块 
                    }
                    //var ovps = vps.FindAll(t => zonecodelist.Contains(t.ZoneCode));//旧承包方
                    var lstvps = ExecuteUpdateVp(sd, nvps, oldVps, relationZones, deloldvps);
                    upvps.AddRange(lstvps);
                    var nlds = geoLands.FindAll(t => t.ZoneCode == sd.ZoneCode);//新地块
                    var deloldldtemp = new List<ContractLand_Del>();
                    var lstlds = AssociteLand(sd, lstvps, deloldvps, oldVps, nlds, oldLands, relationZones, deloldldtemp);
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
                    deloldlds.AddRange(deloldldtemp);
                    index++;
                    this.ReportInfomation($"挂接{sd.Name}下的数据完成，承包方:{lstvps.Count} 地块:{lstlds.Count},未关联地块{deloldldtemp.Count}");
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

        /// <summary>
        /// 关联地块
        /// </summary>
        private List<ContractLand> AssociteLand(CollectivityTissue sender, List<VirtualPerson> revps, List<VirtualPerson_Del> delvps, List<VirtualPerson> ovps,
            List<ContractLand> geoLands, List<ContractLand> oldLands, List<RelationZone> relationZones, List<ContractLand_Del> dellands)
        {
            var listLands = new List<ContractLand>();
            var newzons = relationZones.FindAll(t => t.NewZoneCode == sender.ZoneCode);
            var zonecodelist = newzons.Select(t => t.OldZoneCode).ToList();
            if (zonecodelist.Count == 0)
                return listLands;
            var olds = oldLands.FindAll(t => zonecodelist.Contains(t.ZoneCode));//旧地块

            var rlandset = new HashSet<Guid>();
            foreach (var vp in revps)//关联上的承包方
            {
                var lands = geoLands.Where(x => x.OwnerId == vp.ID).ToList();//承包方下的现有地块 
                List<ContractLand> listOldLands = new List<ContractLand>();
                if (jtmcs.Contains(vp.Name))
                {
                    listOldLands.Clear();
                    foreach (var vpcode in vp.OldVirtualCode.Split('/'))
                    {
                        var tvpid = ovps.Where(x => x.ZoneCode + x.FamilyNumber.PadLeft(4, '0') == vpcode).FirstOrDefault()?.ID;
                        if (tvpid != null)
                            listOldLands.AddRange(olds.Where(t => t.OwnerId == tvpid).ToList());
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
                foreach (var t in lands)
                {
                    var slan = listOldLands.FirstOrDefault(w => w.LandNumber == t.LandNumber);
                    if (slan != null && !rlandset.Contains(slan.ID))
                    {
                        t.OldLandNumber = slan.LandNumber;
                        rlandset.Add(slan.ID);
                        listLands.Add(t);
                        continue;
                    }
                    var rlands = listOldLands.Where(c => c.ActualArea == t.ActualArea).ToList();
                    if (rlands.Count == 0)
                    {
                        t.OldLandNumber = "";
                    }
                    else
                    {
                        bool hasrelate = false;
                        foreach (var c in rlands)
                        {
                            if (!rlandset.Contains(c.ID))
                            {
                                hasrelate = true;
                                t.OldLandNumber = c.LandNumber;
                                rlandset.Add(c.ID);
                                break;
                            }
                        }
                        if (!hasrelate)
                        {
                            t.OldLandNumber = "";
                        }
                    }
                    listLands.Add(t);
                }
                var delandstemp = listOldLands.Where(r => !rlandset.Contains(r.ID)).ToList();
                foreach (var ld in delandstemp)
                {
                    dellands.Add(ContractLand_Del.ChangeDataEntity(sender.ZoneCode, ld));
                }
            }
            var temps = olds.Where(r => !rlandset.Contains(r.ID)).ToList();
            foreach (var ld in temps)
            {
                if (dellands.Any(t => t.DKBM == ld.LandNumber))
                    continue;
                dellands.Add(ContractLand_Del.ChangeDataEntity(sender.ZoneCode, ld));
            }
            return listLands;
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
            List<RelationZone> relationZones, List<VirtualPerson_Del> deloldvps)
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
                    WriteLog(resfile, $"发包方：{sender.Name}{sender.Code} 下的 承包方编码为{vp.ZoneCode + vp.FamilyNumber.PadLeft(4, '0')} 的农户未成功挂接原承包方,以当作新增处理！");
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
                else
                {
                    foreach (var sharePerson in x.SharePersonList)
                    {
                        if (sharePerson.ICN == vp.Number)
                        {
                            vp.OldVirtualCode = x.ZoneCode + x.FamilyNumber.PadLeft(4, '0');
                            vp.ChangeSituation = eBHQK.CYXXBH;
                            result = true;
                        }
                    }
                }

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
                        if (receslist.Contains(x.ID))
                            continue;
                        if (x.Number == person.ICN)//身份证号一致
                        {
                            vp.OldVirtualCode = x.ZoneCode + x.FamilyNumber.PadLeft(4, '0');
                            vp.ChangeSituation = eBHQK.CYXXBH;
                            listVps.Add(vp);
                            receslist.Add(x.ID);
                            personrelation = true;
                            break;
                        }
                    }
                    if (personrelation)
                        break;
                }
            }
        }

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

        #endregion Method - Private - Pro

        #endregion Methods

    }
}
