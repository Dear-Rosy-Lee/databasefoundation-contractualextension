/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Library.WorkStation;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出颁证清册
    /// </summary>
    [Serializable]
    public class ExportAwareInventoryTable : ExportExcelBase
    {
        #region Propertys

        /// <summary>
        /// 系统信息常规设置
        /// </summary>
        public SystemSetDefine SystemSet
        {
            get
            {
                var center = TheApp.Current.GetSystemSettingsProfileCenter();
                var profile = center.GetProfile<SystemSetDefine>();
                var section = profile.GetSection<SystemSetDefine>();
                var config = section.Settings as SystemSetDefine;
                return config;
            }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportAwareInventoryTable(IDbContext db)
        {
            if (db == null)
                return;
            dbContext = db;
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
        }

        /// <summary>
        /// 进度
        /// </summary>
        /// <param name="progress"></param>
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            PostProgress(progress, info);
        }

        #endregion

        #region Fileds

        private IDbContext dbContext;  //数据库
        private ToolProgress toolProgress; //进度
        private string templatePath;  //模板路径
        private int index=1;//下标
        private string endColumn;  //结束列
        private List<ContractConcord> listConcord;  //合同集合
        private List<VirtualPerson> listPerson;  //承包方集合
        private double allArea;  //总面积

        #endregion

        #region Properties

        /// <summary>
        /// 当前行政地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 合同集合
        /// </summary>
        public List<ContractConcord> ListConcord
        {
            get { return listConcord; }
            set { listConcord = value; }
        }

        /// <summary>
        /// 承包方集合
        /// </summary>
        public List<VirtualPerson> ListPerson
        {
            get { return listPerson; }
            set { listPerson = value; }
        }

        /// <summary>
        /// 状态描述
        /// </summary>
        public string StatuDes { get; set; }

        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get; set; }

        /// <summary>
        /// 进度百分比
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// 当前百分比
        /// </summary>
        public double CurrentPercent { get; set; }

        /// <summary>
        /// 地域描述
        /// </summary>
        public string ZoneDesc { get; set; }
        public string UnitName { get; set; }

        #endregion

        #region Methods

        #region 开始生成Excel之前的一系列操作

        /// <summary>
        /// 从数据库直接导出Excel
        /// </summary>
        public void BeginToZone(string templatePath)
        {
            if (!File.Exists(templatePath))
                PostErrorInfo("模板路径不存在！");
            if (CurrentZone == null)
                PostErrorInfo("地域信息无效。");
            if (dbContext == null)
                PostErrorInfo("数据源错误！");
            this.templatePath = templatePath;
            Write(); //写数据
        }

        /// <summary>
        /// 写数据
        /// </summary>
        public override void Write()
        {
            try
            {
                OpenExcelFile();
                dbContext.OpenConnection();
                BeginWrite();
                WriteCount();
                dbContext.CloseConnection();
                SaveAs(SaveFilePath);  
            }
            catch (Exception ex)
            {
                dbContext.CloseConnection();
                PostErrorInfo(ex.Message.ToString());
                Dispose();
            }
        }

        /// <summary>
        /// 打开模板文件
        /// </summary>
        private void OpenExcelFile()
        {
            Open(templatePath);
        }

      

        #endregion

        #region 开始生成Excel

        /// <summary>
        /// 开始写数据
        /// </summary>
        /// <returns></returns>
        private bool BeginWrite()
        {
            int number = 1;
            object objValue = GetRangeToValue("E2", "E2");
            string icnValue = objValue != null ? objValue.ToString() : "";
            endColumn = icnValue == "身份证号码" ? "G" : "F";
            WriteTitle();  //书写标题
            double height = 19.5;

            #region 打印信息

            try
            {
                toolProgress.InitializationPercent(listConcord.Count, Percent, CurrentPercent);
                bool useActualAreaForAwareArea;
                string value = ToolConfiguration.GetSpecialAppSettingValue("UseActualAreaForAwareArea", "true");
                Boolean.TryParse(value, out useActualAreaForAwareArea);
                if (useActualAreaForAwareArea)
                {
                    SetRange("D2", "D2", 25.50, "实测面积（亩）");
                }
                foreach (ContractConcord concord in listConcord)
                {
                    if (concord.ContracterId == null || !concord.ContracterId.HasValue)
                    {
                        continue;
                    }
                    VirtualPerson vp = listPerson.Find(c => c.ID == concord.ContracterId.Value);
                    if (vp == null)
                    {
                        continue;
                    }
                    if (vp.Name.IndexOf("机动地") >= 0 || vp.Name.IndexOf("集体") >= 0)
                    {
                        continue;
                    }
                    SetRange("A" + index, "A" + index, height, 11, false, number.ToString());
                    SetRange("B" + index, "B" + index, height, 11, false, vp.Name.InitalizeFamilyName(SystemSet.KeepRepeatFlag));
                    SetRange("C" + index, "C" + index, height, 11, false, concord.ConcordNumber);
                    SetRange("D" + index, "D" + index, height, 11, false, useActualAreaForAwareArea ? concord.CountActualArea.AreaFormat() : concord.CountAwareArea.AreaFormat());
                    if (endColumn == "G")
                        SetRange("E" + index, "E" + index, height, 11, false, vp.Number);
                    allArea += useActualAreaForAwareArea ? concord.CountActualArea : concord.CountAwareArea;
                    index++;
                    number++;
                    toolProgress.DynamicProgress(ZoneDesc + concord.ContracterName);
                    vp = null;
                }
                listConcord.Clear();
                GC.Collect();
            }
            catch (Exception ex)
            {
                dbContext.CloseConnection();
                return PostErrorInfo("生成Excel时出现错误：" + ex.Message.ToString());
            }

            #endregion

            return true;
        }

        /// <summary>
        /// 书写标题
        /// </summary>
        private void WriteTitle()
        {
            string title = string.Empty;
            //title = GetTitle() + "农村土地承包经营权颁证清册";
            title = UnitName + "农村土地承包经营权颁证清册";
            SetRange("A" + index, endColumn + index, 27.75, 18, true, title);
            index += 2;
        }

        /// <summary>
        /// 书写标题
        /// </summary>
        /// <returns></returns>
        private string GetTitle()
        {
            ZoneDataBusiness zoneBusiness = new ZoneDataBusiness();
            zoneBusiness.Station = dbContext.CreateZoneWorkStation();
            if (CurrentZone != null && CurrentZone.FullCode.Length > 0)
            {
                Zone county = zoneBusiness.Get(CurrentZone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH));
                return CurrentZone.FullName.Replace(county.FullName, "");
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 书写合计信息
        /// </summary>
        private void WriteCount()
        {
            SetRange("A" + index, "A" + index, 19.5, 11, false, "合计");
            SetRange("B" + index, "B" + index, 19.5, 11, false, @"\");
            SetRange("C" + index, "C" + index, 19.5, 11, false, @"\");
            SetRange("D" + index, "D" + index, 19.5, 11, false, allArea.ToString());
            SetRange("E" + index, "E" + index, 19.5, 11, false, @"\");
            SetRange("F" + index, "F" + index, 19.5, 11, false, @"\");
            if (endColumn == "G")
            {
                SetRange(endColumn + index, endColumn + index, 19.5, 11, false, @"\");
            }
            SetLineType("A3", endColumn + index);
        }

        /// <summary>
        /// 配置
        /// </summary>
        public override void GetReplaceMent()
        {
            EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        }
        #endregion

        #endregion
    }
}
