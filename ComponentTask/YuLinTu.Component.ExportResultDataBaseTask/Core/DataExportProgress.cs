/*
 * (C) 2014-2015 鱼鳞图公司版权所有，保留所有权利
*/
using Quality.Business.Entity;
using Quality.Business.TaskBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using YuLinTu;
using YuLinTu.Data;

namespace YuLinTu.Component.ExportResultDataBaseTask
{
    /// <summary>
    /// 数据操作
    /// </summary>
    public class DataExportProgress : Task
    {
        #region Fields

        /// <summary>
        /// 本次导出日志文件路径
        /// </summary>
        private string logFileName;

        /// <summary>
        /// 成果数据目录名称
        /// </summary>
        private string rootFolderName;

        private int dkbsm = QuantityValue.Land;
        private int jzdbsm = QuantityValue.Point;
        private int jzxbsm = QuantityValue.Line;

        #endregion

        #region Properties

        /// <summary>
        /// 数据库路径
        /// </summary>
        public string DataBasePath { get; set; }

        /// <summary>
        /// 矢量数据目录
        /// </summary>
        public string ShapeFilePath { get; set; }

        /// <summary>
        /// 导出面积类型
        /// </summary>
        public int AreaType { get; set; }

        /// <summary>
        /// 导出界址
        /// </summary>
        public bool ContainDotLine { get; set; }

        /// <summary>
        /// 是否导出扫描资料
        /// </summary>
        public bool IsExportScan { get; set; }

        /// <summary>示意图
        /// 是否导出
        /// </summary>
        public bool IsExportDKSYT { get; set; }

        /// <summary>
        /// 导出文件配置
        /// </summary>
        public ExportFileEntity ExportFile { get; set; }

        public int Srid { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public DataExportProgress()
        {
            IsExportDKSYT = true;
        }

        #endregion

        #region Methods

        #region 数据入库

        /// <summary>
        /// 导出数据
        /// </summary>
        public string ExportDataFile(List<ExchangeRightEntity> rightEntitys,/* SqliteManager sqliteManager,*/
            string zoneName, string zoneCode, int persent, string rootFolderName, DataSummary summary,
            /*bool containJzdJzx, */CbdkxxAwareAreaExportEnum cBDKXXAwareAreaExportSet, List<SqliteDK> sqlitLandList)
        {
            string info = string.Empty;
            this.rootFolderName = rootFolderName;
            DataCollection collection = null;
            try
            {
                collection = SetDataToProgress(rightEntitys);
                rightEntitys.Clear();
                ExportSummaryTable.SummaryData(collection, summary, cBDKXXAwareAreaExportSet, sqlitLandList);
                //if (containJzdJzx == false)
                //{
                //    sqliteManager.InsertData(collection.KJDKJH, Srid);
                //}
            }
            catch (Exception ex)
            {
                LogWrite.WriteErrorLog("SetDataToProgress方法中出现错误:" + ex.ToString() + " 地域:" + zoneCode + zoneName);
                throw new Exception("转换数据时发生错误" + ex.Message);
            }
            if (collection == null)
            {
                return info;
            }
            if (ExportFile == null)
                ExportFile = new ExportFileEntity();
            using (IDbContext db = DataBase.CreateDbContext(DataBasePath))
            {
                try
                {
                    //DelByZoneCode(ExportFile, db, zoneCode, DataBasePath);
                    string exportInfo = string.Empty;
                    if (ExportFile.TableFBF.IsExport)
                        exportInfo += ImportDataCollection<FBF>(collection.FBFJH, db, zoneName, "发包方", persent);
                    if (ExportFile.TableCBF.IsExport)
                        exportInfo += ImportDataCollection<CBF>(collection.CBFJH, db, zoneName, "承包方", persent);
                    if (ExportFile.TableJTCY.IsExport)
                        exportInfo += ImportDataCollection<CBF_JTCY>(collection.JTCYJH, db, zoneName, "家庭成员", persent);
                    if (ExportFile.TableCBDKXX.IsExport)
                        exportInfo += ImportDataCollection<CBDKXX>(collection.CBDKXXJH, db, zoneName, "地块信息", persent);
                    if (ExportFile.TableCBHT.IsExport)
                        exportInfo += ImportDataCollection<CBHT>(collection.HTJH, db, zoneName, "承包合同", persent);
                    if (ExportFile.TableQZDJB.IsExport)
                        exportInfo += ImportDataCollection<CBJYQZDJB>(collection.DJBJH, db, zoneName, "登记簿", persent);
                    if (ExportFile.TableCBQZ.IsExport)
                        exportInfo += ImportDataCollection<CBJYQZ>(collection.CBJYQZJH, db, zoneName, "承包经营权证", persent);
                    if (ExportFile.TableQZBF.IsExport)
                        exportInfo += ImportDataCollection<CBJYQZ_QZBF>(collection.QZBFExJH, db, zoneName, "权证补发信息", persent);
                    if (ExportFile.TableQZHF.IsExport)
                        exportInfo += ImportDataCollection<CBJYQZ_QZHF>(collection.QZHFExJH, db, zoneName, "权证换发信息", persent);
                    if (ExportFile.TableQZZX.IsExport)
                        exportInfo += ImportDataCollection<CBJYQZ_QZZX>(collection.QZZXJH, db, zoneName, "权证注销信息", persent);
                    if (ExportFile.TableLZHT.IsExport)
                        exportInfo += ImportDataCollection<LZHT>(collection.LZHTJH, db, zoneName, "流转合同", persent);
                    if (ExportFile.TableFBF.IsExport)
                        exportInfo += ImportDataCollection<QSLYZLFJ>(collection.FJExJH, db, zoneName, "附件", persent);

                    //exportInfo += ImportDataMDB(collection.KJDKJH, db, zoneName, "地块", persent);

                    info = exportInfo;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            if (collection != null)
                collection.Dispose();
            return info;
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        public string ExportDataFile(DataCollection collection, string zoneName, int persent)
        {
            string info = string.Empty;
            if (collection == null)
            {
                return info;
            }
            if (ExportFile == null)
                ExportFile = new ExportFileEntity();
            using (IDbContext db = DataBase.CreateDbContext(DataBasePath))
            {
                try
                {
                    //DelByZoneCode(ExportFile, db, zoneCode, DataBasePath);
                    string exportInfo = string.Empty;
                    if (ExportFile.TableFBF.IsExport)
                        exportInfo += ImportDataCollection<FBF>(collection.FBFJH, db, zoneName, "发包方", persent);
                    if (ExportFile.TableCBF.IsExport)
                        exportInfo += ImportDataCollection<CBF>(collection.CBFJH, db, zoneName, "承包方", persent);
                    if (ExportFile.TableJTCY.IsExport)
                        exportInfo += ImportDataCollection<CBF_JTCY>(collection.JTCYJH, db, zoneName, "家庭成员", persent);
                    if (ExportFile.TableCBDKXX.IsExport)
                        exportInfo += ImportDataCollection<CBDKXX>(collection.CBDKXXJH, db, zoneName, "地块信息", persent);
                    if (ExportFile.TableCBHT.IsExport)
                        exportInfo += ImportDataCollection<CBHT>(collection.HTJH, db, zoneName, "承包合同", persent);
                    if (ExportFile.TableQZDJB.IsExport)
                        exportInfo += ImportDataCollection<CBJYQZDJB>(collection.DJBJH, db, zoneName, "登记簿", persent);
                    if (ExportFile.TableCBQZ.IsExport)
                        exportInfo += ImportDataCollection<CBJYQZ>(collection.CBJYQZJH, db, zoneName, "承包经营权证", persent);
                    if (ExportFile.TableQZBF.IsExport)
                        exportInfo += ImportDataCollection<CBJYQZ_QZBF>(collection.QZBFExJH, db, zoneName, "权证补发信息", persent);
                    if (ExportFile.TableQZHF.IsExport)
                        exportInfo += ImportDataCollection<CBJYQZ_QZHF>(collection.QZHFExJH, db, zoneName, "权证换发信息", persent);
                    if (ExportFile.TableQZZX.IsExport)
                        exportInfo += ImportDataCollection<CBJYQZ_QZZX>(collection.QZZXJH, db, zoneName, "权证注销信息", persent);
                    if (ExportFile.TableLZHT.IsExport)
                        exportInfo += ImportDataCollection<LZHT>(collection.LZHTJH, db, zoneName, "流转合同", persent);
                    if (ExportFile.TableFBF.IsExport)
                        exportInfo += ImportDataCollection<QSLYZLFJ>(collection.FJExJH, db, zoneName, "附件", persent);
                    info = exportInfo;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            if (collection != null)
                collection.Dispose();
            return info;
        }


        ///// <summary>
        ///// 导出未登记数据
        ///// </summary>
        //public string ExportUnRegisterDataFile(List<ComplexRightEntity> rightEntitys, SqliteManager sqliteManager, string zoneName, int persent, string rootFolderName)
        //{
        //    string info = string.Empty;
        //    this.rootFolderName = rootFolderName;
        //    DataCollection collection = null;
        //    try
        //    {
        //        if (ExportFile.TableCBDKXX.IsExport)
        //        {
        //            collection = SetDataToProgress(rightEntitys, sqliteManager, new DataSummary());
        //            int land = collection.KJDKJH.Count(t => t.Shape != null);
        //            info = land > 0 ? "未登记地块数据" + land + "条" : "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWrite.WriteErrorLog("SetDataToProgress方法中出现错误" + ex.Message + ex.StackTrace);
        //        throw new Exception("转换数据时发生错误" + ex.Message);
        //    }
        //    finally
        //    {
        //        collection = null;
        //        GC.Collect();
        //    }
        //    return info;
        //}

        ///<summary>
        ///添加数据集合到数据库中
        ///</summary>
        public string ImportDataCollection<T>(List<T> entityList, IDbContext db, string zoneName, string propertyName, int persent) where T : class
        {
            if (entityList.Count == 0 || db == null)
            {
                return "";
            }
            string tableName = string.Empty;
            FieldInfo[] infoArray = typeof(T).GetFields();
            for (int i = 0; i < infoArray.Length; i++)
            {
                FieldInfo info = infoArray[i];
                if (info.Name == "TableName")
                {
                    tableName = info.GetValue(null).ToString();
                    break;
                }
            }
            infoArray = null;
            T erren = null;
            try
            {
                db.BeginTransaction();
                var qc = db.CreateQuery<T>();
                foreach (T en in entityList)
                {
                    erren = en;
                    qc.Add(en).Save();
                }
                db.CommitTransaction();
                int datacount = entityList.Count;
                entityList.Clear();
                return propertyName + datacount + "条 ";
            }
            catch (Exception ex)
            {
                db.RollbackTransaction();
                var msg = ex.ToString();
                var erroinfo = "数据导出到成果数据" + tableName + "(" + propertyName + ")表时出错:" + msg;
                if (erren != null)
                    msg = ToString<T>(erren) + msg;
                LogWrite.WriteErrorLog(erroinfo + msg);
                throw new Exception(erroinfo);
            }
        }

        ///<summary>
        ///添加数据集合到数据库中
        ///</summary>
        public string ImportDataMDB(List<SqliteDK> entityList, IDbContext db, string zoneName, string propertyName, int persent)
        {
            if (entityList.Count == 0 || db == null)
            {
                return "";
            }
            string tableName = "DK";
            MDBDK erren = null;
            try
            {
                db.BeginTransaction();
                var qc = db.CreateQuery<MDBDK>();
                foreach (var en in entityList)
                {
                    erren = en.ConvertTo<MDBDK>();
                    if (erren.DKLB == "ContractLand")
                        erren.DKLB = "10";
                    qc.Add(erren).Save();
                }
                db.CommitTransaction();
                int datacount = entityList.Count;
                entityList.Clear();
                return propertyName + datacount + "条 ";
            }
            catch (Exception ex)
            {
                db.RollbackTransaction();
                var msg = ex.ToString();
                var erroinfo = "数据导出到成果数据" + tableName + "(" + propertyName + ")表时出错:" + msg;
                if (erren != null)
                    msg = ToString<MDBDK>(erren) + msg;
                LogWrite.WriteErrorLog(erroinfo + msg);
                throw new Exception(erroinfo);
            }
        }

        /// <summary>
        /// 删除地域下的数据
        /// </summary>
        public void DelByZoneCode(ExportFileEntity exportfile, IDbContext db, string zoneCode, string basePath)
        {
            if (zoneCode.Length == 16)
                zoneCode = zoneCode.Substring(0, 12) + zoneCode.Substring(14, 2);
            if (ExportFile.IsAllExport || !File.Exists(basePath))
                return;
            if (exportfile.TableCBDKXX.IsExport)
            {
                var q = db.CreateQuery<CBDKXXEX>();
                db.Queries.Add(q.Where(t => t.DKBM.StartsWith(zoneCode)).Delete());
            }
            if (exportfile.TableCBF.IsExport)
            {
                var q = db.CreateQuery<CBF>();
                db.Queries.Add(q.Where(t => t.CBFBM.StartsWith(zoneCode)).Delete());
            }
            if (exportfile.TableCBHT.IsExport)
            {
                var q = db.CreateQuery<CBHT>();
                db.Queries.Add(q.Where(t => t.CBHTBM.StartsWith(zoneCode)).Delete());
            }
            if (exportfile.TableCBQZ.IsExport)
            {
                var q = db.CreateQuery<CBJYQZ>();
                db.Queries.Add(q.Where(t => t.CBJYQZBM.StartsWith(zoneCode)).Delete());
            }
            if (exportfile.TableFBF.IsExport)
            {
                var q = db.CreateQuery<FBF>();
                db.Queries.Add(q.Where(t => t.FBFBM.StartsWith(zoneCode)).Delete());
            }
            if (exportfile.TableJTCY.IsExport)
            {
                var q = db.CreateQuery<CBF_JTCY>();
                db.Queries.Add(q.Where(t => t.CBFBM.StartsWith(zoneCode)).Delete());
            }
            if (exportfile.TableLZHT.IsExport)
            {
                var q = db.CreateQuery<LZHT>();
                db.Queries.Add(q.Where(t => t.LZHTBM.StartsWith(zoneCode)).Delete());
            }
            if (exportfile.TableQZBF.IsExport)
            {
                var q = db.CreateQuery<CBJYQZ_QZBF>();
                db.Queries.Add(q.Where(t => t.CBJYQZBM.StartsWith(zoneCode)).Delete());
            }
            if (exportfile.TableQZDJB.IsExport)
            {
                var q = db.CreateQuery<CBJYQZDJB>();
                db.Queries.Add(q.Where(t => t.CBJYQZBM.StartsWith(zoneCode)).Delete());
            }
            if (exportfile.TableQZHF.IsExport)
            {
                var q = db.CreateQuery<CBJYQZ_QZHF>();
                db.Queries.Add(q.Where(t => t.CBJYQZBM.StartsWith(zoneCode)).Delete());
            }
            if (exportfile.TableQZZX.IsExport)
            {
                var q = db.CreateQuery<CBJYQZ_QZZX>();
                db.Queries.Add(q.Where(t => t.CBJYQZBM.StartsWith(zoneCode)).Delete());
            }
            if (exportfile.TableZLFJ.IsExport)
            {
                var q = db.CreateQuery<QSLYZLFJ>();
                db.Queries.Add(q.Where(t => t.CBJYQZBM.StartsWith(zoneCode)).Delete());
            }

            db.Queries.Save();
        }

        /// <summary>
        /// 转换实体
        /// </summary>
        private string ToString<T>(T en)
        {
            var str = "";
            var properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var obj = properties[i].GetValue(en, null);
                if (obj != null)
                    str += properties[i].Name + ":" + obj.ToString() + ",";
            }
            return " 数据内容(" + str.TrimEnd(',') + ") ";
        }

        #endregion

        #region 数据转换

        /// <summary>
        /// 设置数据到处理实体(地块、界址点、界址线存入数据库)
        /// </summary>
        /// <param name="rightEntitys">权属交换实体集合</param>
        /// <param name="sqliteManager">sqlite管理类实例</param>
        public DataCollection SetDataToProgress(List<ExchangeRightEntity> rightEntitys)
        {
            var data = new DataCollection();
            if (rightEntitys == null || rightEntitys.Count == 0)
            {
                return data;
            }
            var now = DateTime.Now;
            var fpm = new FilePathManager();
            foreach (var entity in rightEntitys)
            {

                if (entity.DKXX != null)
                {
                    foreach (var dkxx in entity.DKXX)
                    {
                        data.CBDKXXJH.Add(dkxx);//.Initialize(AreaType));
                    }
                }
                if (entity.KJDK != null)
                {
                    foreach (var dk in entity.KJDK)
                    {
                        var kjdk = CopyEntity(dk, data.JZDJH, data.JZXJH);
                        data.KJDKJH.Add(kjdk);
                    }
                }
                if (entity.CBF != null)
                {
                    data.CBFJH.Add(entity.CBF.Initialize());
                }
                if (entity.FBF != null)
                {
                    FBF fbfEntity = entity.FBF;
                    if (!data.FBFJH.Any(t => t.FBFBM == entity.FBF.FBFBM))
                    {
                        data.FBFJH.Add(fbfEntity.Initialize());
                    }
                }
                if (entity.JTCY != null)
                {
                    foreach (CBF_JTCY en in entity.JTCY)
                    {
                        data.JTCYJH.Add(en.Initialize());
                    }
                }
                if (entity.HT != null)
                {
                    entity.HT.ForEach(h =>
                    {
                        if (h.CBQXZ == null)
                            h.CBQXZ = now;
                        data.HTJH.Add(h.Initialize());
                    });
                }
                if (entity.DJB != null)
                {
                    entity.DJB.ForEach(d =>
                    {
                        data.DJBJH.Add(d.Initialize(fpm.ImageName));
                    });
                }
                if (entity.CBJYQZ != null)
                {
                    entity.CBJYQZ.ForEach(h =>
                    {
                        data.CBJYQZJH.Add(h.Initialize());
                    });
                }
                if (entity.QZBF != null)
                {
                    foreach (var bf in entity.QZBF)
                    {
                        data.QZBFExJH.Add(bf.Initialize());
                    }
                }
                if (entity.QZHF != null)
                {
                    foreach (var hf in entity.QZHF)
                    {
                        data.QZHFExJH.Add(hf.Initialize());
                    }
                }
                if (entity.QZZX != null)
                {
                    entity.QZZX.ForEach(zx => data.QZZXJH.Add(zx.Initialize()));
                }
                if (entity.LZHT != null)
                {
                    foreach (var item in entity.LZHT)
                    {
                        if (!data.LZHTJH.Any(t => t.LZHTBM == item.LZHTBM))
                            data.LZHTJH.Add(item.Initialize());
                    }
                }
                if (entity.FJ != null)
                {
                    foreach (var fj in entity.FJ)
                    {
                        data.FJExJH.Add(fj.Initialize(fpm.OtherName));
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 复制实体
        /// </summary>
        private SqliteDK CopyEntity(DKEX dk, List<SqliteJZD> jzdList, List<SqliteJZX> jzxList)
        {
            SqliteDK entity = new SqliteDK();
            entity.BSM = dkbsm++;
            entity.SCMJM = dk.SCMJM;
            entity.DKBM = dk.DKBM;
            entity.DKBZ = dk.DKBZ;
            entity.DKBZXX = dk.DKBZXX;
            entity.DKDZ = dk.DKDZ;
            entity.DKLB = dk.DKLB;
            entity.DKMC = dk.DKMC;
            entity.DKNZ = dk.DKNZ;
            entity.DKXZ = dk.DKXZ;
            entity.DLDJ = dk.DLDJ;
            entity.SCMJ = dk.SCMJ;
            entity.SFJBNT = dk.SFJBNT;
            entity.Shape = dk.Shape as YuLinTu.Spatial.Geometry;
            entity.SYQXZ = dk.SYQXZ;
            entity.TDLYLX = dk.TDLYLX;
            entity.TDYT = dk.TDYT;
            entity.YSDM = dk.YSDM;
            entity.ZJRXM = dk.ZJRXM;
            entity.KJZB = dk.KJZB;
            string kjzbid = string.Empty;
            var tempList = new List<SqliteJZD>();
            if (dk.JZD != null)
            {
                dk.JZD.ForEach(j =>
                {
                    var jzd = ObjectExtension.ConvertTo<SqliteJZD>(j);
                    jzd.BSM = jzdbsm++;
                    jzd.DKBM = entity.DKBM;
                    jzd.YSDM = eFeatureType.JZD;
                    jzd.Shape = j.Shape as YuLinTu.Spatial.Geometry;
                    tempList.Add(jzd);
                    jzdList.Add(jzd);
                    kjzbid += "/" + jzd.BSM.ToString();
                });
            }
            if (dk.JZX != null)
            {
                dk.JZX.ForEach(j =>
                {
                    var jzx = ObjectExtension.ConvertTo<SqliteJZX>(j);
                    jzx.BSM = jzxbsm++;
                    jzx.DKBM = entity.DKBM;
                    var qjzd = tempList.Find(t => t.JZDH == j.QJZDH);
                    var zjzd = tempList.Find(t => t.JZDH == j.ZJZDH);
                    if (qjzd != null && zjzd != null)
                    {
                        jzx.FK_QJZDID = qjzd.BSM;
                        jzx.FK_ZJZDID = zjzd.BSM;
                    }
                    jzx.YSDM = eFeatureType.JZX;
                    jzx.Shape = j.Shape as YuLinTu.Spatial.Geometry;
                    jzxList.Add(jzx);
                });
            }
            entity.FK_KJZBID = kjzbid + "/";
            return entity;
        }

        #endregion

        #region 创建导出目录

        /// <summary>
        /// 创建文件目录
        /// </summary>
        /// <param name="filePath">选择导出目录</param>
        /// <param name="codeYear">6位县级地域编码+4位年份编码</param>
        /// <param name="zoneName">县级地域名称</param>
        public bool CreatFolderFile(string filePath, string code, string year, string zoneName, ExportFileEntity exportFile)
        {
            try
            {
                string rootName = code + zoneName;
                if (string.IsNullOrEmpty(rootName))
                {
                    return false;
                }
                string rootFolder = Path.Combine(filePath, rootName);
                Dictionary<string, string> dic = FilePathManager.GetDataBasePath(rootFolder, IsExportScan);
                if (dic == null)
                {
                    return false;
                }
                bool hasDataExport = HasBaseDataExport(exportFile);
                FilePathManager fpm = new FilePathManager();
                DataBasePath = Path.Combine(dic[fpm.CategoryName], code + year + ".mdb");
                ShapeFilePath = dic[fpm.VictorName];
                if ((hasDataExport && !File.Exists(DataBasePath) && !exportFile.IsAllExport) || exportFile.IsAllExport)
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + @"Template\database.mdb", DataBasePath, true);
                }
                logFileName = Path.Combine(rootFolder, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt");
                string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ShapeTemplate");
                if (Directory.Exists(ShapeFilePath) && Directory.Exists(templatePath))
                {
                    string[] files = Directory.GetFiles(templatePath);
                    if (files != null && files.Length > 0)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            string filename = Path.GetFileNameWithoutExtension(files[i]);
                            FileEntity fe = FileEntityExport(exportFile, filename);
                            if (!ContainDotLine && (filename == JZD.TableName || filename == JZX.TableName))
                            {
                                continue;
                            }
                            string destionName = Path.Combine(ShapeFilePath, filename + code + year + Path.GetExtension(files[i]));
                            if ((fe.IsExport && !File.Exists(destionName) && !exportFile.IsAllExport)
                                || exportFile.IsAllExport)
                                File.Copy(files[i], destionName, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "创建导出目录出错:" + ex.Message;
                LogWrite.WriteErrorLog(errorMsg + ex.StackTrace);
                this.ReportError(errorMsg);
                throw ex;
            }
            return true;
        }

        /// <summary>
        /// 是否有权属数据导出
        /// </summary>
        private bool HasBaseDataExport(ExportFileEntity exportFile)
        {
            bool hasDataExport = false;
            PropertyInfo[] infos = typeof(ExportFileEntity).GetProperties();
            for (int i = 0; i < infos.Length; i++)
            {
                PropertyInfo info = infos[i];
                FileEntity fe = info.GetValue(exportFile, null) as FileEntity;
                if (fe == null || !info.Name.StartsWith("Table"))
                    continue;
                if (fe.IsExport)
                    hasDataExport = true;
            }
            return hasDataExport;
        }

        /// <summary>
        /// 导出实体中的文件状态
        /// </summary>
        private FileEntity FileEntityExport(ExportFileEntity exportFile, string name)
        {
            FileEntity fileEntity = null;
            PropertyInfo[] infos = typeof(ExportFileEntity).GetProperties();
            for (int i = 0; i < infos.Length; i++)
            {
                PropertyInfo info = infos[i];
                FileEntity fe = info.GetValue(exportFile, null) as FileEntity;
                if (fe == null || !info.Name.StartsWith("Victor"))
                    continue;
                if (info.Name == ("Victor" + name))
                {
                    fileEntity = fe;
                    break;
                }
            }
            return fileEntity;
        }

        #endregion

        #endregion
    }

    [DataTable("DK")]
    public class MDBDK
    {
        #region Property

        /// <summary>
        /// 标识码(M)
        /// </summary>
        public int BSM { get; set; }

        /// <summary>
        /// 要素代码(M)(eFeatureType)
        /// </summary>
        public string YSDM { get; set; }

        /// <summary>
        /// 地块编码(M)
        /// </summary>
        public string DKBM { get; set; }

        /// <summary>
        /// 地块名称(M)
        /// </summary>
        public string DKMC { get; set; }

        /// <summary>
        /// 所有权性质(O)(eSYQXZ)
        /// </summary>
        public string SYQXZ { get; set; }

        /// <summary>
        /// 地块类别(M)(eDKLB)
        /// </summary>
        public string DKLB { get; set; }

        /// <summary>
        /// 土地利用类型(O)
        /// </summary>
        public string TDLYLX { get; set; }

        /// <summary>
        /// 地力等级(M)(eDLDJ)
        /// </summary>
        public string DLDJ { get; set; }

        /// <summary>
        /// 土地用途(M)(eTDYT)
        /// </summary>
        public string TDYT { get; set; }

        /// <summary>
        /// 是否基本农田(M)(eWhether)
        /// </summary>
        public string SFJBNT { get; set; }

        /// <summary>
        /// 实测面积(M)
        /// </summary>
        public double SCMJ { get; set; }

        /// <summary>
        /// 实测面积亩（o）
        /// </summary>
        public double? SCMJM { get; set; }

        /// <summary>
        /// 地块东至(O)
        /// </summary>
        public string DKDZ { get; set; }

        /// <summary>
        /// 地块西至(O)
        /// </summary>
        public string DKXZ { get; set; }

        /// <summary>
        /// 地块南至(O)
        /// </summary>
        public string DKNZ { get; set; }

        /// <summary>
        /// 地块北至(O)
        /// </summary>
        public string DKBZ { get; set; }

        /// <summary>
        /// 地块备注信息
        /// </summary>
        public string DKBZXX { get; set; }

        /// <summary>
        /// 指界人姓名
        /// </summary>
        public string ZJRXM { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public MDBDK()
        {
        }

        #endregion
    }
}