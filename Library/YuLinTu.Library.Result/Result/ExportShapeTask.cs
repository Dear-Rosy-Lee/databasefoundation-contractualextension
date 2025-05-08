using System;
using System.Collections.Generic;
using System.IO;
using YuLinTu.Library.Aux;
using YuLinTu.NetAux;

namespace YuLinTu.Library.Result
{
    public class ExportShapeTask : Task
    {
        protected override void OnStarted()
        {
            var arg = Argument as ExportShapeTaskArgument;
            testExportJzd(arg.DbFile, arg.ShapeOutPath, arg.ZoneCode, arg.ZoneYearCode,
                arg.LandNumber, arg.PointNumber, arg.LineNumber, arg.OnlyKey, arg.ESRIPrjStr, arg.UseUniteNumberExport, arg.DataNumber);
        }

        protected override void OnGo()
        {
            //var arg = Argument as ExportShapeTaskArgument; 
            //testExportJzd(Arg.DbFile, Arg.ShapeOutPath, Arg.ZoneCode, Arg.ZoneYearCode,
            //    Arg.LandNumber, Arg.PointNumber, Arg.LineNumber);
        }

        /// <summary>
        /// 导出界址点、界址线和地块
        /// 张建民 2017年10月09日修改
        /// </summary>
        public void testExportJzd(string dbFile, string shapeFileOutputPath,
            string currentZoneCode, string zoneYearCode,
            int dknumber, int jzdnumber, int jzxnumber, bool onlykey, string prj, bool useUniteNumberExport, int datanum)
        {
            var db = new DBSpatialite();
            {
                db.Open(dbFile);//@"C:\通川区安云乡.sqlite");
                var prms = new ExportJzdxParam(0.05);

                prms.sDkSYQXZ = "";
                prms.exportOnlyKey = onlykey;
                prms.UseUniteNumberExport = useUniteNumberExport;
                prms.JzdMaxRecords = jzdnumber > 6000000 ? 6000000 : 0;//每个界址点文件最多 条
                prms.JzxMaxRecords = jzxnumber > 2000000 ? 2000000 : 0; //每个界址线文件最多 条
                prms.DkMaxRecords = dknumber > 1300000 ? 1300000 : 0;////每个界址线文件最多 条

                #region 删除文件    
                var dkPath = string.Format(@"{0}\{1}{2}.shp", shapeFileOutputPath, "DK", zoneYearCode);
                var pointPath = string.Format(@"{0}\{1}{2}.shp", shapeFileOutputPath, "JZX", zoneYearCode);
                var linePath = string.Format(@"{0}\{1}{2}.shp", shapeFileOutputPath, "JZD", zoneYearCode);
                DeleteFile(prms.DkMaxRecords > 0, dkPath);
                DeleteFile(prms.JzdMaxRecords > 0, pointPath);
                DeleteFile(prms.JzxMaxRecords > 0, linePath);
                #endregion

                string prjString = prj;

                #region 输出.prj文件需要

                var srid = db.QuerySRID(Zd_cbdFields.TABLE_NAME);
                if (srid >= 0)
                {
                    prms.sESRIPrjStr = prjString; //SpatialReferenceUtil.FindEsriSpatialReferenceString(spatialReferencePath, srid);
                }
                #endregion
                var exp = new ExportJzdx(db, prms, shapeFileOutputPath + @"\", currentZoneCode, zoneYearCode, datanum);

                exp.ReportProgress += (msg, i) =>
                {//进度
                    this.ReportProgress(i, msg);
                };
                exp.ReportInfomation += msg =>
                {//提示信息
                    this.ReportInfomation(msg);
                };
                exp.ReportWarn += msg =>
                {//提示警告信息
                    this.ReportWarn(msg);
                };
                exp.OnPresaveDk += en =>
                {//这里可以根据需要修改地块实体的内容
                    if (en.SYQXZ != "10" && en.SYQXZ != "30" &&
                        en.SYQXZ != "31" && en.SYQXZ != "32" &&
                        en.SYQXZ != "33" && en.SYQXZ != "34")
                        en.SYQXZ = "30";
                };
                exp.DoExport(new HashSet<string>());
            }
        }

        private void DeleteFile(bool del, string path)
        {
            if (!del)
                return;
            try
            {
                File.Delete(path);
                var prj = Path.ChangeExtension(path, ".prj");
                if (File.Exists(prj))
                    File.Delete(prj);
                var dbf = Path.ChangeExtension(path, ".dbf");
                if (File.Exists(dbf))
                    File.Delete(dbf);
                var shx = Path.ChangeExtension(path, ".shx");
                if (File.Exists(shx))
                    File.Delete(shx);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
