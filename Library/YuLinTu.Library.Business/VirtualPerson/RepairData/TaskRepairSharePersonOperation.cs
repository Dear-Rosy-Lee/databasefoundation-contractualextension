using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    public class TaskRepairSharePersonOperation : Task
    {
        #region Properties

        #endregion Properties

        #region Fields

        #endregion Fields

        #region Ctor

        public TaskRepairSharePersonOperation()
        {
            Name = "数据修复";
            Description = "家庭成员关联数据修复";
        }

        #endregion Ctor

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");

            var args = Argument as TaskRepairSharePersonOperationArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }

            try
            {
                RepairSharePerson(args.Database, args.CurrentZone);
            }
            catch (System.Exception ex)
            {
                if (!(ex is TaskStopException))
                {
                    this.ReportError(ex.Message + "数据修复时出错!");
                }
            }

            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        #endregion Methods - Override

        private void RepairSharePerson(Data.IDbContext db, Zone zone)
        {
            var vpStation = db.CreateVirtualPersonStation<LandVirtualPerson>();
            int familyCount = vpStation.Count(c => c.ZoneCode.Equals(zone.FullCode));
            if (familyCount == 0)
            {
                this.ReportInfomation(string.Format("{0}下没有承包方数据", zone.Name));
                this.ReportInfomation(string.Format("修复{0}下家庭成员结束", zone.Name));
                return;
            }

            this.ReportProgress(1, string.Format("开始获取{0}承包方数据...", zone.Name));
            List<VirtualPerson> vps = vpStation.GetByZoneCode(zone.FullCode, eLevelOption.Self);
            if (vps == null || vps.Count == 0)
            {
                this.ReportWarn(string.Format("{0}下无承包方数据!", zone.Name));
                return;
            }

            string description = vps.Count > 0 ? ("(" + vps.Count + ")") : "";
            this.ReportProgress(10, string.Format("开始修复{0}下家庭成员数据...", zone.Name));
            var progress = new ToolProgress();
            progress.InitializationPercent(vps.Count, 90, 10);
            foreach (VirtualPerson vp in vps)
            {
                bool shouldRepair = false;
                List<Person> spList = vp.SharePersonList;
                foreach (Person person in spList)
                {
                    if (!person.FamilyID.Equals(vp.ID))
                    {
                        person.FamilyID = vp.ID;
                        shouldRepair = true;
                    }
                }

                if (shouldRepair)
                {
                    vp.SharePersonList = spList;
                    vpStation.Update(vp);
                }

                progress.DynamicProgress();
            }

            vps = null;
            GC.Collect();
            this.ReportAlert(null, string.Format("修复{0}下家庭成员结束", zone.Name));
            this.ReportProgress(100, "完成");
        }

        #endregion Methods
    }
}