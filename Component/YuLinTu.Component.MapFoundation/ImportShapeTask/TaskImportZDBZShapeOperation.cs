/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Repository;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Component.MapFoundation
{
    public class TaskImportZDBZShapeOperation : Task
    {
        #region Fields      

        private object returnValue;
        #endregion
        #region Properties

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

        /// <summary>
        /// 导入界址点图斑设置实体
        /// </summary>

        /// <summary>
        /// 导出公示调查表日期设置
        /// </summary>
        public DateSetting PublishDateSetting { get; set; }

        /// <summary>
        /// 返回值
        /// </summary>
        public object ReturnValue
        {
            get { return returnValue; }
        }

        /// <summary>
        /// 是否批量
        /// </summary>
        public bool IsBatch { get; set; }

        /// <summary>
        /// 当前被选中的承包方
        /// </summary>
        public List<VirtualPerson> SelectContractor { get; set; }

        /// <summary>
        /// 数据库服务上下文
        /// </summary>
        public IDbContext dbContext { get; set; }

        /// <summary>
        /// 选中的承包方集合
        /// </summary>
        public List<VirtualPerson> SelectedPersons { get; set; }

        #endregion

        public TaskImportZDBZShapeOperation()
        {

        }

        /// <summary>
        /// 开始执行子任务
        /// </summary>
        protected override void OnGo()
        {
            returnValue = null;
            TaskImportZDBZShapeArgument metadata = Argument as TaskImportZDBZShapeArgument;
            if (metadata == null)
            {
                return;
            }
            if(metadata.CurrentZone==null)
            {
                this.ReportError("未选择行政地域");
                this.ReportProgress(100);
                return;
            }
            if (metadata.Type == "Del")
            {
                IContractLandMarkWorkStation dczdstation = new ContainerFactory(metadata.Database).CreateWorkstation<IContractLandMarkWorkStation, IContractLandMarkRepository>();
                this.ReportProgress(10);
                int count = dczdstation.Count(c => c.ZoneCode.StartsWith(metadata.CurrentZone.FullCode));
                dczdstation.Delete(c=>c.ZoneCode.StartsWith(metadata.CurrentZone.FullCode));
                this.ReportProgress(100);
                this.ReportInfomation("清空" + count + "条宗地标注数据");
                return;
            }
            string fileName = metadata.FileName;
            //bool isClear = metadata.IsClear;
            dbContext = metadata.Database;
            Zone zone = metadata.CurrentZone;
            //SelectContractor = metadata.SelectContractor;
            AccountLandBusinessMark landBusiness = new AccountLandBusinessMark(dbContext);
            landBusiness.Alert += ReportInfo;
            landBusiness.ProgressChanged += ReportPercent;
            //landBusiness.UseContractorInfoImport = metadata.UseContractorInfoImport;
            //landBusiness.UseLandCodeBindImport = metadata.UseLandCodeBindImport;
            landBusiness.shapeAllcolNameList = metadata.DotAllcolNameList;
            var zoneStation = dbContext.CreateZoneWorkStation();
            List<Zone> childrenZone = zoneStation.GetChildren(zone.FullCode, eLevelOption.Subs);
            Zone parent = landBusiness.GetParent(zone);
            ImportLandDataShape(zone, childrenZone, fileName, landBusiness);
            this.ReportProgress(100);
            GC.Collect();

        }

        #region Methods - Privates-承包台账地块图斑导入

        /// <summary>
        /// 导入承包方shape图斑数据
        /// </summary>
        private void ImportLandDataShape(Zone currentZone, List<Zone> childrenZone, string fileName, AccountLandBusinessMark accountBusiness)
        {
            //获取当前路径下shape数据
            IList landShapeList = null;
            string filepath = string.Empty;
            string filename = string.Empty;
            IDbContext ds = null;
            try
            {
                filepath = System.IO.Path.GetDirectoryName(fileName);
                filename = System.IO.Path.GetFileNameWithoutExtension(fileName);
                ds = ProviderShapefile.CreateDataSource(filepath, false) as IDbContext;
                var dq = new DynamicQuery(ds);
                var landdata = dq.Get(null, filename).Result as IList;
                landShapeList = landdata;
            }
            catch
            {
                this.ReportError("当前打开Shape文件有误，请检查文件是否正确");
                return;
            }

            var importShpType = ds.DataSource as IProviderShapefile;
            if (importShpType.GetGeometryType(filename) != Spatial.eGeometryType.Point)
            {
                this.ReportError("当前Shape文件不为点文件，请重新选择点文件导入");
                return;
            }
            if (landShapeList.Count == 0)
            {
                this.ReportInfomation("当前导入文件没有数据");
                return;
            }
            var dps = ds.DataSource as ProviderShapefile;
            YuLinTu.Spatial.SpatialReference shpRef = dps.GetSpatialReference(filename);
            accountBusiness.shpRef = shpRef;

            if (landShapeList.Count == 0 || landShapeList == null)
            {
                return;
            }
            double currentPercent = 0.0;
            if(currentZone.Level>eZoneLevel.Village)
            {
                this.ReportWarn("请选择镇以下级别的地域进行导入");
                this.ReportProgress(100);
                return;
            }
            if (currentZone.Level == eZoneLevel.Village)
            {
                if (childrenZone == null)
                {
                    var zoneStation = dbContext.CreateZoneWorkStation();
                    childrenZone = zoneStation.GetChildren(currentZone.FullCode, eLevelOption.SelfAndSubs);
                }
                else
                {
                    childrenZone.Add(currentZone);
                }
                int importCount = 0;//总数
                int importZoneCount = 0;//各组个数
                this.ReportProgress(0, "开始");
                double indexCount = (double)childrenZone.Count / 95;//均分进度
                int zoneCurrentIndex = 0;//进度索引

                foreach (var itemZone in childrenZone)
                {
                    currentPercent = 5 + indexCount * zoneCurrentIndex;
                    importZoneCount = accountBusiness.ImportLandShapeDataInfo(itemZone, landShapeList, currentPercent, indexCount);
                    this.ReportProgress((int)currentPercent, string.Format("导入{0}下地块", itemZone.FullName));
                    zoneCurrentIndex++;
                    importCount = importCount + importZoneCount;
                }
                if (importCount != 0)
                {
                    this.ReportProgress(100, "完成");
                    this.ReportInfomation(string.Format("{0}共导入{1}条信息", currentZone.Name, importCount));
                }
                currentPercent = 0;
            }
            else if (currentZone.Level == eZoneLevel.Group)
            {
                int importCount = 0;
                this.ReportProgress(0, "开始");
                importCount = accountBusiness.ImportLandShapeDataInfo(currentZone, landShapeList, currentPercent);
                if (importCount != 0)
                {
                    this.ReportProgress(100, "完成");
                    this.ReportInfomation(string.Format("{0}共导入{1}条信息", currentZone.Name, importCount));
                }
                currentPercent = 0;
            }

            landShapeList = null;
        }

        #endregion




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

    }




}
