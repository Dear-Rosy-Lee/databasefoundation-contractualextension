/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using YuLinTu.Data;
using System.IO;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出地块类别汇总表
    /// </summary>
    public class ExportCategorySummaryExcel : ExportExcelBase
    {
        #region Ctro

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportCategorySummaryExcel()
        {
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
        }

        /// <summary>
        /// 进度
        /// </summary>
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            PostProgress(progress, info);
        }

        #endregion Ctro

        #region Fields

        private int index; //索引值
        private string templatePath;

        //private string StatuDes;
        private ToolProgress toolProgress;  //进度条

        private bool result;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        public Zone CurrentZone { get; set; }
        public IDbContext DB { get; set; }
        public string UnitName { get; set; }

        #endregion Properties

        #region Methods

        #region Method-开始生成Excel操作

        /// <summary>
        /// 开始导出数据
        /// </summary>
        /// <param name="templaePath">模板保存路径</param>
        public bool BeginExcel(string templatePath)
        {
            this.templatePath = templatePath;
            result = true;
            //PostProgress(1, StatuDes);
            if (!File.Exists(templatePath))
            {
                result = false;
                PostErrorInfo("模板路径不存在！");
            }
            if (CurrentZone == null)
            {
                result = false;
                PostErrorInfo("目标地域不存在！");
            }
            if (DB == null)
            {
                result = false;
                PostErrorInfo("数据源错误!");
            }
            index = 4;
            Write();   //写数据
            return result;
        }

        /// <summary>
        /// 写数据
        /// </summary>
        public override void Write()
        {
            try
            {
                Open(templatePath);   //打开模板
                WriteFamilyInformation();
                SaveAs(FilePath);    //保存文件
            }
            catch (Exception ex)
            {
                result = false;
                PostErrorInfo(ex.Message.ToString());
                Dispose();
            }
        }

        #endregion Method-开始生成Excel操作

        #region Method-开始往Excel添加值

        /// <summary>
        /// 开始写数据
        /// </summary>
        private void WriteFamilyInformation()
        {
            //写标题信息
            SetRange("A1", "S1", UnitName + "地块类别统计表");
            //先按照村统计，再按照大的行政单位统计
            //开始写内容
            List<Zone> SelfAndSubsZones = new List<Zone>();
            var zoneStation = DB.CreateZoneWorkStation();
            var vpstation = DB.CreateVirtualPersonStation<LandVirtualPerson>();
            var landstation = DB.CreateContractLandWorkstation();
            SelfAndSubsZones = zoneStation.GetChildren(CurrentZone.FullCode, eLevelOption.SelfAndSubs);  //当前地域下的
            List<VirtualPerson> VPS = vpstation.GetByZoneCode(CurrentZone.FullCode, eLevelOption.SelfAndSubs);

            toolProgress.InitializationPercent(SelfAndSubsZones.Count, 90);
            List<StaticClass> allStaticinfo = new List<StaticClass>();

            for (int i = 0; i < SelfAndSubsZones.Count; i++)
            {
                StaticClass staticinfo = new StaticClass();
                Zone zone = SelfAndSubsZones[i];
                staticinfo.zoneName = zone.Name;
                staticinfo.zoneLevel = zone.Level;
                staticinfo.zoneCode = zone.FullCode;
                allStaticinfo.Add(staticinfo);

                //if (zone.Level > eZoneLevel.Village)
                //{
                //    continue;
                //}

                if (zone.Level == eZoneLevel.Village)
                {
                    var vgZones = SelfAndSubsZones.FindAll(fa => fa.FullCode.StartsWith(zone.FullCode));
                    if (vgZones.Count > 0)
                    {
                        continue;
                    }
                }

                List<VirtualPerson> vp = VPS.FindAll(c => c.ZoneCode.StartsWith(zone.FullCode));
                staticinfo.AllvpCount = vp.Count;

                int persons = 0;
                vp.ForEach(c => persons += c.SharePersonList.Count);
                staticinfo.AllpersonsCount = persons;

                List<ContractLand> lands = landstation.Get(c => c.ZoneCode.StartsWith(zone.FullCode));

                int allcount = 0;
                double tablearea = 0.0, actularea = 0.0;

                int Ccount = 0, Pcount = 0, Wcount = 0, Mcount = 0;
                double CTarea = 0.0, PTarea = 0.0, WTarea = 0.0, MTarea = 0.0;
                double CAarea = 0.0, PAarea = 0.0, WAarea = 0.0, MAarea = 0.0;
                double CAwarearea = 0.0;
                lands.ForEach(c =>
                {
                    tablearea += (c.TableArea == null ? 0 : c.TableArea.Value);
                    actularea += c.ActualArea;
                    allcount++;

                    if (c.LandCategory == ((int)eLandCategoryType.ContractLand).ToString())
                    {
                        Ccount++;
                        CTarea += (c.TableArea == null ? 0 : c.TableArea.Value);
                        CAarea += c.ActualArea;
                        CAwarearea += c.AwareArea;
                    }
                    if (c.LandCategory == ((int)eLandCategoryType.PrivateLand).ToString())
                    {
                        Pcount++;
                        PTarea += (c.TableArea == null ? 0 : c.TableArea.Value);
                        PAarea += c.ActualArea;
                    }
                    if (c.LandCategory == ((int)eLandCategoryType.WasteLand).ToString())
                    {
                        Wcount++;
                        WTarea += (c.TableArea == null ? 0 : c.TableArea.Value);
                        WAarea += c.ActualArea;
                    }
                    if (c.LandCategory == ((int)eLandCategoryType.MotorizeLand).ToString())
                    {
                        Mcount++;
                        MTarea += (c.TableArea == null ? 0 : c.TableArea.Value);
                        MAarea += c.ActualArea;
                    }
                });
                tablearea = ToolMath.SetNumericFormat(tablearea, 4, 1);
                actularea = ToolMath.SetNumericFormat(actularea, 4, 1);
                CTarea = ToolMath.SetNumericFormat(CTarea, 4, 1);
                CAarea = ToolMath.SetNumericFormat(CAarea, 4, 1);
                CAwarearea = ToolMath.SetNumericFormat(CAwarearea, 4, 1);
                PTarea = ToolMath.SetNumericFormat(PTarea, 4, 1);
                PAarea = ToolMath.SetNumericFormat(PAarea, 4, 1);
                WTarea = ToolMath.SetNumericFormat(WTarea, 4, 1);
                WAarea = ToolMath.SetNumericFormat(WAarea, 4, 1);
                MTarea = ToolMath.SetNumericFormat(MTarea, 4, 1);
                MAarea = ToolMath.SetNumericFormat(MAarea, 4, 1);

                staticinfo.AllTarea = tablearea;
                staticinfo.AllAarea = actularea;
                staticinfo.AlllandCount = allcount;

                staticinfo.Ccount = Ccount;
                staticinfo.CTarea = CTarea;
                staticinfo.CAarea = CAarea;
                staticinfo.CAwarearea = CAwarearea;
                

                staticinfo.Pcount = Pcount;
                staticinfo.PTarea = PTarea;
                staticinfo.PAarea = PAarea;

                staticinfo.Wcount = Wcount;
                staticinfo.WTarea = WTarea;
                staticinfo.WAarea = WAarea;

                staticinfo.Mcount = Mcount;
                staticinfo.MTarea = MTarea;
                staticinfo.MAarea = MAarea;

                toolProgress.DynamicProgress(zone.Name);
                vp.Clear();
                lands.Clear();
            }

            foreach (var item in allStaticinfo)
            {
                int villagezones = 0;
                if (item.zoneLevel == eZoneLevel.Village)
                {
                    var vgZones = SelfAndSubsZones.FindAll(fa => fa.FullCode.StartsWith(item.zoneCode));
                    villagezones = vgZones.Count;
                }

                if (item.zoneLevel == eZoneLevel.Group || villagezones == 0)
                {
                    continue;
                }

                var szzonetjs = allStaticinfo.FindAll(z => z.zoneCode.StartsWith(item.zoneCode));
                szzonetjs.Remove(item);
                item.AllvpCount = szzonetjs.Sum(sz => sz.AllvpCount);
                item.AllpersonsCount = szzonetjs.Sum(sz => sz.AllpersonsCount);
                item.AlllandCount = szzonetjs.Sum(sz => sz.AlllandCount);

                item.AllTarea = szzonetjs.Sum(sz => sz.AllTarea);
                item.AllAarea = szzonetjs.Sum(sz => sz.AllAarea);

                item.Ccount = szzonetjs.Sum(sz => sz.Ccount);
                item.CTarea = szzonetjs.Sum(sz => sz.CTarea);
                item.CAarea = szzonetjs.Sum(sz => sz.CAarea);
                item.CAwarearea = szzonetjs.Sum(sz => sz.CAwarearea);

                item.Pcount = szzonetjs.Sum(sz => sz.Pcount);
                item.PTarea = szzonetjs.Sum(sz => sz.PTarea);
                item.PAarea = szzonetjs.Sum(sz => sz.PAarea);

                item.Wcount = szzonetjs.Sum(sz => sz.Wcount);
                item.WTarea = szzonetjs.Sum(sz => sz.WTarea);
                item.WAarea = szzonetjs.Sum(sz => sz.WAarea);

                item.Mcount = szzonetjs.Sum(sz => sz.Mcount);
                item.MTarea = szzonetjs.Sum(sz => sz.MTarea);
                item.MAarea = szzonetjs.Sum(sz => sz.MAarea);
            }

            PrintValue(allStaticinfo);
            allStaticinfo.Clear();

            SelfAndSubsZones.Clear();
            VPS.Clear();

            GC.Collect();
        }

        private void SetLandCategory(List<ContractLand> lands)
        {
            //eLandCategoryType.ContractLand;//承包地块
            //eLandCategoryType.PrivateLand;//自留地
            //eLandCategoryType.WasteLand;//开荒地
            //eLandCategoryType.MotorizeLand;//机动地

            int Ccount = 0, Pcount = 0, Wcount = 0, Mcount = 0;
            double CTarea = 0.0, PTarea = 0.0, WTarea = 0.0, MTarea = 0.0;
            double CAarea = 0.0, PAarea = 0.0, WAarea = 0.0, MAarea = 0.0;
            double CAwarearea = 0.0;
            lands.ForEach(c =>
            {
                if (c.LandCategory == ((int)eLandCategoryType.ContractLand).ToString())
                {
                    Ccount++;
                    CTarea += (c.TableArea == null ? 0 : c.TableArea.Value);
                    CAarea += c.ActualArea;
                    CAwarearea += c.AwareArea;
                }
                if (c.LandCategory == ((int)eLandCategoryType.PrivateLand).ToString())
                {
                    Pcount++;
                    PTarea += (c.TableArea == null ? 0 : c.TableArea.Value);
                    PAarea += c.ActualArea;
                }
                if (c.LandCategory == ((int)eLandCategoryType.WasteLand).ToString())
                {
                    Wcount++;
                    WTarea += (c.TableArea == null ? 0 : c.TableArea.Value);
                    WAarea += c.ActualArea;
                }
                if (c.LandCategory == ((int)eLandCategoryType.MotorizeLand).ToString())
                {
                    Mcount++;
                    MTarea += (c.TableArea == null ? 0 : c.TableArea.Value);
                    MAarea += c.ActualArea;
                }
            });
            CTarea = ToolMath.SetNumericFormat(CTarea, 4, 1);
            CAarea = ToolMath.SetNumericFormat(CAarea, 4, 1);
            PTarea = ToolMath.SetNumericFormat(PTarea, 4, 1);
            PAarea = ToolMath.SetNumericFormat(PAarea, 4, 1);
            WTarea = ToolMath.SetNumericFormat(WTarea, 4, 1);
            WAarea = ToolMath.SetNumericFormat(WAarea, 4, 1);
            MTarea = ToolMath.SetNumericFormat(MTarea, 4, 1);
            MAarea = ToolMath.SetNumericFormat(MAarea, 4, 1);

            SetRange("G" + index, "G" + index, Ccount.ToString());//地块数
            SetRange("H" + index, "H" + index, ToolMath.SetNumbericFormat(CTarea.ToString(), 2));//二轮合同面积
            SetRange("I" + index, "I" + index, ToolMath.SetNumbericFormat(CTarea.ToString(), 2));//确权面积
            SetRange("J" + index, "J" + index, ToolMath.SetNumbericFormat(CAarea.ToString(), 2));//实测面积

            SetRange("K" + index, "K" + index, Pcount.ToString());//地块数
            SetRange("L" + index, "L" + index, ToolMath.SetNumbericFormat(PTarea.ToString(), 2));//二轮合同面积
            SetRange("M" + index, "M" + index, ToolMath.SetNumbericFormat(PAarea.ToString(), 2));//实测面积

            SetRange("N" + index, "N" + index, Mcount.ToString());//地块数
            SetRange("O" + index, "O" + index, ToolMath.SetNumbericFormat(MTarea.ToString(), 2));//二轮合同面积

            SetRange("P" + index, "P" + index, ToolMath.SetNumbericFormat(MAarea.ToString(), 2));//实测面积

            SetRange("Q" + index, "Q" + index, Wcount.ToString());//地块数
            SetRange("R" + index, "R" + index, ToolMath.SetNumbericFormat(WTarea.ToString(), 2));//二轮合同面积
            SetRange("S" + index, "S" + index, ToolMath.SetNumbericFormat(WAarea.ToString(), 2));//实测面积
        }

        private void PrintValue(List<StaticClass> allStaticinfo)
        {
            if (allStaticinfo.Count == 0) return;

            foreach (var item in allStaticinfo)
            {
                SetRange("A" + index, "A" + index, item.zoneName);//地域名称
                SetRange("B" + index, "B" + index, item.AllvpCount.ToString());//总户数
                SetRange("C" + index, "C" + index, item.AllpersonsCount.ToString());//总人数
                SetRange("D" + index, "D" + index, item.AlllandCount.ToString());//总地块数
                var tablearea = ToolMath.SetNumericFormat(item.AllTarea, 4, 1);
                var actularea = ToolMath.SetNumericFormat(item.AllAarea, 4, 1);
                SetRange("E" + index, "E" + index, ToolMath.SetNumbericFormat(tablearea.ToString(), 2));//二轮合同面积
                SetRange("F" + index, "F" + index, ToolMath.SetNumbericFormat(actularea.ToString(), 2));//实测面积

                var CTarea = ToolMath.SetNumericFormat(item.CTarea, 4, 1);
                var CAarea = ToolMath.SetNumericFormat(item.CAarea, 4, 1);
                var CAwarearea = ToolMath.SetNumericFormat(item.CAwarearea, 4, 1);
                var PTarea = ToolMath.SetNumericFormat(item.PTarea, 4, 1);
                var PAarea = ToolMath.SetNumericFormat(item.PAarea, 4, 1);
                var WTarea = ToolMath.SetNumericFormat(item.WTarea, 4, 1);
                var WAarea = ToolMath.SetNumericFormat(item.WAarea, 4, 1);
                var MTarea = ToolMath.SetNumericFormat(item.MTarea, 4, 1);
                var MAarea = ToolMath.SetNumericFormat(item.MAarea, 4, 1);

                SetRange("G" + index, "G" + index, item.Ccount.ToString());//地块数
                SetRange("H" + index, "H" + index, ToolMath.SetNumbericFormat(CTarea.ToString(), 2));//二轮合同面积
                SetRange("I" + index, "I" + index, ToolMath.SetNumbericFormat(CAwarearea.ToString(), 2));//确权面积
                SetRange("J" + index, "J" + index, ToolMath.SetNumbericFormat(CAarea.ToString(), 2));//实测面积

                SetRange("K" + index, "K" + index, item.Pcount.ToString());//地块数
                SetRange("L" + index, "L" + index, ToolMath.SetNumbericFormat(PTarea.ToString(), 2));//二轮合同面积
                SetRange("M" + index, "M" + index, ToolMath.SetNumbericFormat(PAarea.ToString(), 2));//实测面积

                SetRange("N" + index, "N" + index, item.Mcount.ToString());//地块数
                SetRange("O" + index, "O" + index, ToolMath.SetNumbericFormat(MTarea.ToString(), 2));//二轮合同面积
                SetRange("P" + index, "P" + index, ToolMath.SetNumbericFormat(MAarea.ToString(), 2));//实测面积

                SetRange("Q" + index, "Q" + index, item.Wcount.ToString());//地块数
                SetRange("R" + index, "R" + index, ToolMath.SetNumbericFormat(WTarea.ToString(), 2));//二轮合同面积
                SetRange("S" + index, "S" + index, ToolMath.SetNumbericFormat(WAarea.ToString(), 2));//实测面积

                index++;
            }
        }

        /// <summary>
        /// 配置
        /// </summary>
        public override void GetReplaceMent()
        {
            EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        }

        #endregion Method-开始往Excel添加值

        #endregion Methods
    }

    internal class StaticClass
    {
        public string zoneName;
        public string zoneCode;
        public eZoneLevel zoneLevel;

        public int Ccount = 0, Pcount = 0, Wcount = 0, Mcount = 0;
        public double CTarea = 0.0, PTarea = 0.0, WTarea = 0.0, MTarea = 0.0;
        public double CAarea = 0.0, PAarea = 0.0, WAarea = 0.0, MAarea = 0.0;
        public double CAwarearea = 0.0;
        public int AllvpCount;
        public int AllpersonsCount;
        public int AlllandCount = 0;
        public double AllTarea = 0.0;
        public double AllAarea = 0.0;
    }
}