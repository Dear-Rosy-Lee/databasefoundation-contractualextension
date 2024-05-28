/*
 * (C) 2014 鱼鳞图公司版权所有,保留所有权利
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using YuLinTu.Data;
using YuLinTu.Data.Dynamic;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;
using YuLinTu.Spatial;
using YuLinTu.tGISCNet;

namespace YuLinTu.Component.BatchDataBaseTask
{
    /// <summary>
    /// 获取数据
    /// </summary>
    public partial class DataImportProgress : Task
    {
        #region Fields

        /// <summary>
        /// shape数据
        /// </summary>
        private IList landShapeList = null;

        /// <summary>
        /// 数据库
        /// </summary>
        private IDbContext db;

        /// <summary>
        /// 计时器
        /// </summary>
        private Stopwatch sp;

        /// <summary>
        /// 检查模块信息提示
        /// </summary>
        public delegate void QualityModuleAlertDelegate(TaskAlertEventArgs e);

        #endregion

        #region Propertys

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        #endregion

        #region Ctor

        public DataImportProgress()
        {
            db = DataBaseSource.GetDataBaseSource();
            sp = new Stopwatch();
        }

        #endregion

        #region Methods

        #region 导入数据

        /// <summary>
        /// 导入数据
        /// </summary>
        public void ImportData()
        {
            sp.Start();
            this.ReportProgress(1, "正在检查shape数据...");
            this.ReportInfomation("正在检查shape数据...");
            CheckShapeData(FilePath);//检查导入的shape数据
            this.ReportProgress(5, "开始导入shape数据");
            this.ReportInfomation( "开始导入shape数据");

            string name = typeof(Zone).GetAttribute<DataTableAttribute>().TableName;
            SpatialReference dbReference = ReferenceHelper.GetDbReference<Zone>(db, name, "Shape");
            int srid = dbReference.WKID;
            var landNumRowDic = GetLandNumRowDic("LandNumber");

            var updateSuccess = UpdateShape(landNumRowDic,srid);//更新数据库shape数据

            sp.Stop();
            string timeSpan = string.Format("总用时{0}秒",sp.ElapsedMilliseconds / 1000.0f);//更新总消耗时间

            if (updateSuccess.IsNullOrEmpty())
            {
                this.ReportProgress(100, "导入完成："+timeSpan);
                this.ReportInfomation("导入完成：" + timeSpan);
            }
            else
            {
                this.ReportProgress(100, "导入失败："+timeSpan);
                this.ReportError(updateSuccess);
            }

            landShapeList = null;
        }

        /// <summary>
        /// 更新地块shape数据
        /// </summary>
        /// <param name="landNumRowDic"></param>
        /// <param name="srid"></param>
        /// <returns></returns>
        private string UpdateShape(Dictionary<string, int> landNumRowDic, int srid)
        {

            var landStation = db.CreateContractLandWorkstation();
            var updateSuccess = string.Empty;

            if (landNumRowDic == null || landNumRowDic.Count == 0)
            {
                return string.Empty;
            }

            int landTotal = landNumRowDic.Count;
            int processIndex = 0;
            int currentUpdateCount = 0;
            using (var shp = new ShapeFile())
            {
                var errorInfo = shp.Open(FilePath);
                if (errorInfo.IsNullOrEmpty())
                {
                    EnumContractLand((landList) =>
                    {
                        try
                        {
                            db.BeginTransaction();

                            foreach (var land in landList)
                            {
                                if (landNumRowDic.ContainsKey(land.LandNumber))
                                {
                                    var rowid = landNumRowDic[land.LandNumber];
                                    var geo = shp.GetGeometry(rowid, srid);
                                    land.Shape = geo;
                                    landStation.UpdateShape(land);
                                }
                            }

                            db.CommitTransaction();
                            var updateList = landList.FindAll(s => s.Shape != null);
                            var updateCount = updateList != null ? updateList.Count : 0;//更新的条数
                            currentUpdateCount += updateCount;
                            processIndex += updateCount;
                            var processTag = 6 + (int)((double)processIndex / landTotal * 93);//进度
                            this.ReportProgress(processTag, "正在导入数据...");
                            this.ReportInfomation(string.Format("已更新{0}条数据,用时{1}秒",currentUpdateCount.ToString(), sp.ElapsedMilliseconds / 1000.0f));
                        }
                        catch (System.Exception ex)
                        {
                            db.RollbackTransaction();
                            this.ReportInfomation(ex.Message);
                            updateSuccess = ex.Message;
                        }

                    }, 10000);
                }
            }

            return updateSuccess;
        }

        /// <summary>
        /// 获取shape文件中的地块编码和行号的集合
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> GetLandNumRowDic(string colName)
        {
            Dictionary<string, int> landNumRowDic = new Dictionary<string, int>();
            if (FilePath.IsNotNullOrEmpty())
            {
                using (var shp = new ShapeFile())
                {
                    string err = shp.Open(FilePath);
                    if (err.IsNullOrEmpty())
                    {
                        var fIndex = shp.FindField(colName);
                        if (fIndex > -1)
                        {
                            var temp = shp.GetRecordCount();
                            for (int i = 0; i < shp.GetRecordCount(); i++)
                            {
                                var v = shp.GetFieldString(i, fIndex);
                                if (!string.IsNullOrEmpty(v) && !landNumRowDic.ContainsKey(v))
                                {
                                    landNumRowDic.Add(v, i);
                                }
                            }
                        }
                    }
                }
            }

            return landNumRowDic;
        }

        /// <summary>
        /// 获取指定数量的记录
        /// </summary>
        /// <param name="landAction"></param>
        /// <param name="number"></param>
        private void EnumContractLand(Action<List<ContractLand>> landAction, int number = 100000)
        {
            var landStaion = db.CreateContractLandWorkstation();
            var landList = new List<ContractLand>(100000);

            var zoneStation = db.CreateZoneWorkStation();

            var qurey = db.CreateQuery<ContractLand>();
            qurey.ForEach<ContractLand>((p, i, en) =>
            {
                landList.Add(en);
                if (landList.Count == number)
                {
                    landAction(landList);
                    landList.Clear();
                }

                return true;
            });
            if (landList.Count > 0)
            {
                landAction(landList);
            }
        }

        /// <summary>
        /// 检查需要导入的shape
        /// </summary>
        private void CheckShapeData(string fileName)
        {
            //获取当前路径下shape数据
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
            if (importShpType.GetGeometryType(filename) != Spatial.eGeometryType.Polygon)
            {
                this.ReportError("当前Shape文件不为面文件，请重新选择面文件导入");
                return;
            }
            if (landShapeList.Count == 0)
            {
                this.ReportInfomation("当前导入文件没有数据");
                return;
            }
            var dps = ds.DataSource as ProviderShapefile;
            YuLinTu.Spatial.SpatialReference shpRef = dps.GetSpatialReference(filename);

            if (landShapeList.Count == 0 || landShapeList == null)
            {
                return;
            }
        }

        #endregion

        #endregion
    }
}
