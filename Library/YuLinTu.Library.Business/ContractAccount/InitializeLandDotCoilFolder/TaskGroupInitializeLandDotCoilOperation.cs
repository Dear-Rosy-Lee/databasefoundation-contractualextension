/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;
using YuLinTu.NetAux;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 批量初始化界址点线数据操作任务类
    /// </summary>
    public class TaskGroupInitializeLandDotCoilOperation : ParallelTaskGroup
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskGroupInitializeLandDotCoilOperation()
        {
            int threadcount = Environment.ProcessorCount;
            if (threadcount > 10)
            {
                MaxThreadCount = 10;
            }
            else
            {
                MaxThreadCount = threadcount;
            }

            MininumPercent = 5;
            MaxinumPercent = 45;
        }

        #endregion

        #region Method

        ///// <summary>
        ///// 开始执行组任务
        ///// </summary>
        //protected override void OnGo()
        //{
        //    var meta = Argument as TaskGroupInitializeLandDotCoilArgument;
        //    var sw = System.Diagnostics.Stopwatch.StartNew();

        //    var singleTaskArgument = new TaskInitializeLandDotCoilArgument();
        //    singleTaskArgument.CurrentZone = meta.CurrentZone;
        //    singleTaskArgument.Database = meta.Database;
        //    //singleTaskArgument.InstallArg.AddressLinedbiDistance = meta.AddressLinedbiDistance;
        //    //singleTaskArgument.InstallArg.IsLineFindDescription = meta.IsLineFindDescription;
        //    //singleTaskArgument.InstallArg.IsUnit = meta.IsUnit;
        //    //singleTaskArgument.InstallArg.IsLineVpNameUseImport = meta.IsLineVpNameUseImport;
        //    //singleTaskArgument.InstallArg.LineVpNameUseImportValue = meta.LineVpNameUseImportValue;
        //    //singleTaskArgument.InstallArg.UseAddAlgorithm = meta.UseAddAlgorithm;
        //    try
        //    {
        //        using (var db = new DBSpatialite())
        //        {
        //            var dbFile = meta.Database.DataSource.ConnectionString;
        //            dbFile = dbFile.Substring(dbFile.IndexOf('=') + 1);

        //            db.Open(dbFile);//@"C:\myprojects\工单\20160816建库工具\通川区安云乡.sqlite");
        //            var prms = new InitLandDotCoilParam();
        //            prms.AddressDotMarkType = SafeConvertAux.SafeConvertToInt16(meta.AddressDotMarkType);
        //            prms.AddressDotType = SafeConvertAux.SafeConvertToInt16(meta.AddressDotType);
        //            prms.AddressLineCatalog = meta.AddressLineCatalog;
        //            prms.AddressLinedbiDistance = meta.AddressLinedbiDistance;
        //            prms.AddressLinePosition = meta.AddressLinePosition;
        //            prms.AddressLineType = meta.AddressLineType;
        //            prms.AddressPointPrefix = meta.AddressPointPrefix;
        //            prms.IsLineDescription = meta.IsLineDescription;
        //            prms.MinAngleFileter = meta.MinAngleFileter;
        //            prms.MaxAngleFilter = meta.MaxAngleFilter;
        //            prms.IsFilter = meta.IsFilterDot;
        //            prms.UseAddAlgorithm = meta.UseAddAlgorithm;
        //            var t = new InitLandDotCoil(db, prms);
        //            t.ReportProgress += (msg, i) =>
        //            {
        //                this.ReportProgress(i, msg);
        //                //tbProgress.Text = msg + ":" + i + "%";
        //                //Application.DoEvents();
        //            };
        //            t.onPresaveJzd += en =>
        //            {
        //                if (meta.IsFilterDot == false)
        //                {
        //                    en.SFKY = true;
        //                }
        //            };
        //            t.ReportInfomation += msg =>
        //            {
        //                this.ReportInfomation(msg);
        //            };

        //            var wh = string.Format(Zd_cbdFields.ZLDM + " like '{0}%'", meta.CurrentZone.FullCode);// zldm like '511702208200%'";
        //            var sql = string.Format("select count(*) from {0} where {1} not like '{2}%'", Zd_cbdFields.TABLE_NAME, Zd_cbdFields.ZLDM, meta.CurrentZone.FullCode);
        //            int n = db.QueryOneInt(sql);
        //            if (n == 0)
        //            {
        //                wh = null;
        //            }
        //            t.DoInit(wh);
        //            string zoneCode = meta.CurrentZone.FullCode == "86" ? "" : meta.CurrentZone.FullCode;

        //            if (meta.IsLineFindDescription)
        //            {
        //                JzxsmUtil.MakeJzxsm(db, (msg, i) =>//生成界址线说明
        //                {
        //                    this.ReportProgress(i, msg);
        //                }, meta.IsLineDescription, zoneCode);
        //            }
        //        }
        //        if (meta.InitialJZXInfoUseSN)
        //        {
        //            this.ReportProgress(0, "开始更新界址线信息");
        //            InitialJZXInfoUseSN initialJZXInfoUseSN = new InitialJZXInfoUseSN(singleTaskArgument);
        //            initialJZXInfoUseSN.ProgressChanged += ReportPercent;
        //            initialJZXInfoUseSN.Alert += ReportInfo;
        //            initialJZXInfoUseSN.MainHandle();
        //            this.ReportProgress(100, "完成更新界址线信息");
        //        }
        //        var coilStation = meta.Database.CreateCoilWorkStation();
        //        var landStation = meta.Database.CreateContractLandWorkstation();
        //        var zoneStation = meta.Database.CreateZoneWorkStation();
        //        if (meta.IsSetAddressLinePosition)
        //        {
        //            this.ReportProgress(0, "开始修改界址点位置信息");
        //            List<BuildLandBoundaryAddressCoil> currentZoneUpdateCoils = new List<BuildLandBoundaryAddressCoil>();
        //            var zoneList = zoneStation.GetAllZones(meta.CurrentZone);
        //            var pro = 100 / zoneList.Count();
        //            var count = pro;
        //            foreach (var zone in zoneList)
        //            {
        //                this.ReportProgress(count, "正在修改界址点位置信息");
        //                count += pro;
        //                var currentZoneCoils = coilStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
        //                if (currentZoneCoils.Count == 0)
        //                {
        //                    this.ReportWarn(string.Format("当前地域{0}下无界址线,未能进行界址线位置修改操作", zone.FullName));
        //                    continue;
        //                }
        //                foreach (var line in currentZoneCoils)
        //                {
        //                    if (line.Position == "3")
        //                    {
        //                        line.Position = "1";
        //                        currentZoneUpdateCoils.Add(line);
        //                    }
        //                }
        //                //var landList = landStation.GetLandsByZoneCodeInCadastralNumber(zone.FullCode);
        //                //foreach (var land in landList)
        //                //{
        //                //    var landLineList = currentZoneCoils.FindAll(x => x.LandID == land.ID);
        //                //    foreach (var line in landLineList)
        //                //    {
        //                //        if (line.Position == "3")
        //                //        {
        //                //            line.Position = "1";
        //                //            currentZoneUpdateCoils.Add(line);
        //                //        }
        //                //    }
        //                //}
        //                coilStation.UpdateRange(currentZoneUpdateCoils);
        //                currentZoneUpdateCoils.Clear();
        //            }


        //            this.ReportProgress(100, "完成修改界址点位置信息");
        //        }

        //    }
        //    finally
        //    {
        //        sw.Stop();
        //        this.ReportInfomation(string.Format("共耗时：{0}", sw.Elapsed));
        //    }
        //    return;

        //}

        protected override void OnGo()
        {
            var metadata = Argument as TaskInitializeLandDotCoilArgument;
            if (metadata == null)
            {
                return;
            }
            InitializeLandDotCoil importDot = new InitializeLandDotCoil();
            importDot.ProgressChanged += ReportPercent;
            importDot.Alert += ReportInfo;
            importDot.Argument = metadata;

            //var sw = System.Diagnostics.Stopwatch.StartNew();
            importDot.ContractLandInitialTool();
        }

        #region Methods—任务辅助方法

        /// <summary>
        /// 进度报告
        /// </summary>
        /// <param name="progress"></param>
        private void ReportPercent(object sender, TaskProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.ReportProgress(e.Percent, e.UserState);
            }
        }


        /// <summary>
        /// 错误信息报告
        /// </summary>
        /// <param name="message"></param>
        private void ReportInfo(object sender, TaskAlertEventArgs e)
        {
            if (e != null)
            {
                this.ReportAlert(e.Grade, e.UserState, e.Description);
            }
        }

        #endregion


        #endregion
    }
}
