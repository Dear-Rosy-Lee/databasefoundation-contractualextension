using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace YuLinTu.Component.ContractedLand.BoundaryCalculateTask
{
    public class ShapeProcess
    {
        public delegate void ReportInformation(string msg);
        public ReportInformation Info { get; set; }

        public delegate void ReportProcess(double msg);
        public ReportProcess Process { get; set; }

        public string extName;

        private string dkPath;
        private string destinPath;
        public string splitePath;
        private List<string> shpList = new List<string>();
        public ShapeProcess(string dkPath, string destinPath)
        {
            this.dkPath = dkPath;
            this.destinPath = destinPath;
            //splitePath = destinPath + "\\分图层数据";
            //Directory.CreateDirectory(splitePath);
        }

        /// <summary>
        /// 注册Ogr驱动
        /// </summary>
        public static bool Registerdll()
        {
            try
            {
                Ogr.RegisterAll();
                // 为了支持中文路径，请添加下面这句代码 
                OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");
                // 为了使属性表字段支持中文，请添加下面这句 
                OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "CP936");
            }
            catch (Exception ex)
            {
                throw new Exception("注册Gdal驱动失败,无法进行相关的shape文件导出:" + ex.Message + ex.StackTrace);
            }
            return true;
        }

        #region 合并文件 

        /// <summary>
        /// 处理地块
        /// </summary>
        public string ProcessDkPath()
        {
            this.Info("正在准备处地块数据");
            this.Process(1);
            var name = Path.GetFileNameWithoutExtension(dkPath);
            if (string.IsNullOrEmpty(extName))
            {
                var norName = new Regex("-\\d+").Replace(name, "");
                extName = new Regex("[a-zA-Z]+").Replace(norName, "");
            }
            var oldFolder = Path.GetDirectoryName(dkPath);
            var newPath = "";
            var gex = new Regex("[a-zA-Z]{2}\\d{10}-\\d+");//说明不是单文件
            string strDriverName = "ESRI Shapefile"; //创建数据,创建ESRI的shp文件

            var driver = OSGeo.OGR.Ogr.GetDriverByName(strDriverName);
            if (driver == null)
                return "";
            var folder = Path.GetDirectoryName(dkPath);
            var oldOds = driver.CreateDataSource(folder, null);
            if (oldOds == null)
                return "";
            var oldlayer = oldOds.GetLayerByName(name);
            if (oldlayer == null)
                return "";
            var featrueDfn = oldlayer.GetLayerDefn();

            var dic = IsFieldComplate(featrueDfn);//实测/空间坐标字段是否存在

            newPath = Path.Combine(destinPath, "DK" + extName + ".shp");//新地块文件路径
            if (File.Exists(newPath))
            {
                var files = Directory.GetFiles(destinPath);
                for (int i = 0; i < files.Length; i++)
                {
                    var fname = Path.GetFileNameWithoutExtension(files[i]);
                    if (fname == "DK" + extName)
                        File.Delete(files[i]);
                }
                File.Delete(newPath);
            }
            if (gex.IsMatch(name))
            {
                MuiltFileProcess(dic, oldFolder, name, newPath, driver, oldOds, oldlayer, gex);
            }
            else//单文件
            {
                SingeFileProcess(dic, oldFolder, name, newPath, driver, oldOds, oldlayer);
            }
            oldOds.Dispose();
            driver.Dispose();
            this.Process(100);
            return newPath;
        }

        /// <summary>
        /// 单个文件处理
        /// </summary>
        /// <param name="dic">字段处理</param>
        /// <param name="oldFolder">原始目录</param>
        /// <param name="name">原文件名称</param>
        /// <param name="newPath">新文件全路径</param>
        /// <param name="driver">驱动</param>
        /// <param name="oldOds">原数据源</param>
        /// <param name="oldlayer">原图层</param>
        private void SingeFileProcess(Dictionary<string, bool> dic, string oldFolder, string name,
            string newPath, Driver driver, DataSource oldOds, Layer oldlayer)
        {
            if (dic["KJZB"] && dic["SCMJM"])
            {
                Copy(oldFolder, name, destinPath, "DK", extName);
            }
            else
            {
                var ods = driver.CreateDataSource(newPath, null);
                if (ods == null)
                    return;
                Layer layer = ods.CreateLayer(newPath, oldlayer.GetSpatialRef(), oldlayer.GetGeomType(), null);

                var defn = oldlayer.GetLayerDefn();
                for (int i = 0; i < defn.GetFieldCount(); i++)
                {
                    var fd = defn.GetFieldDefn(i);
                    layer.CreateField(fd, 1);
                }
                if (!dic["KJZB"])
                {
                    var field = new FieldDefn("KJZB", FieldType.OFTString);
                    field.SetWidth(254);
                    layer.CreateField(field, 1);
                }
                if (!dic["SCMJM"])
                {
                    var field = new FieldDefn("SCMJM", FieldType.OFTReal);
                    field.SetWidth(16);
                    field.SetPrecision(3);
                    layer.CreateField(field, 1);
                }
                WriteData(layer, !dic["SCMJM"], oldlayer);
                layer.Dispose();
                ods.Dispose();
            }
            oldlayer.Dispose();
        }

        /// <summary>
        /// 多个文件处理
        /// </summary>
        private void MuiltFileProcess(Dictionary<string, bool> dic, string oldFolder, string name,
            string newPath, Driver driver, DataSource oldOds, Layer oldlayer, Regex gx)
        {
            var ods = driver.CreateDataSource(newPath, null);
            if (ods == null)
                return;

            Layer layer = ods.CreateLayer(newPath, oldlayer.GetSpatialRef(), oldlayer.GetGeomType(), null);

            var defn = oldlayer.GetLayerDefn();
            for (int i = 0; i < defn.GetFieldCount(); i++)
            {
                var fd = defn.GetFieldDefn(i);
                layer.CreateField(fd, 1);
            }
            if (!dic["KJZB"])
            {
                var field = new FieldDefn("KJZB", FieldType.OFTString);
                field.SetWidth(254);
                layer.CreateField(field, 1);
            }
            if (!dic["SCMJM"])
            {
                var field = new FieldDefn("SCMJM", FieldType.OFTReal);
                field.SetWidth(16);
                field.SetPrecision(3);
                layer.CreateField(field, 1);
            }

            oldlayer.Dispose();

            var preName = Regex.Replace(name, "-\\d+", "");
            var files = Directory.GetFiles(oldFolder);
            List<string> pathList = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                var path = files[i];
                var fname = Path.GetFileNameWithoutExtension(files[i]);
                var shp = Path.GetExtension(files[i]);
                if (shp.ToLower() != ".shp")
                    continue;
                if (gx.IsMatch(fname) && fname.StartsWith(preName))
                {
                    pathList.Add(path);
                }
            }
            double s = 99 / pathList.Count;
            int index = 1;
            foreach (var item in pathList)
            {
                var sname = Path.GetFileNameWithoutExtension(item);
                Layer layer2 = oldOds.GetLayerByName(sname);
                WriteData(layer, !dic["SCMJM"], layer2);
                layer2.Dispose();
                this.Process(index * s);
                index++;
            }
            layer.Dispose();
            ods.Dispose();
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        private void WriteData(Layer layer, bool setmj, Layer layer2)
        {
            var newfeature = layer.GetLayerDefn();
            var count = layer2.GetFeatureCount(1);
            var index = layer2.FindFieldIndex("SCMJ", 1);
            for (int i = 0; i < count; i++)
            {
                var feature = layer2.GetFeature(i);
                Feature f = new Feature(newfeature);
                f.SetFrom(feature, 1);
                if (setmj && index > -1)
                {
                    var v = feature.GetFieldAsDouble(index);
                    var vm = Math.Round(v * 0.0015, 2);
                    f.SetField("SCMJM", vm);
                }
                layer.CreateFeature(f);
            }
        }

        /// <summary>
        /// 字段中是否包含SCMJM、KJZB
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, bool> IsFieldComplate(FeatureDefn featureDefn)
        {
            bool kjzb = false;
            bool scmjm = false;
            for (int i = 0; i < featureDefn.GetFieldCount(); i++)
            {
                var fieldDefn = featureDefn.GetFieldDefn(i);
                if (fieldDefn.GetName().ToLower() == "kjzb")
                    kjzb = true;
                if (fieldDefn.GetName().ToLower() == "scmjm")
                    scmjm = true;
            }
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            dic.Add("KJZB", kjzb);
            dic.Add("SCMJM", scmjm);
            return dic;
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        public bool Copy(string sourcePath, string name, string destPath,
            string newName, string exteName)
        {
            try
            {
                string templatePath = sourcePath;
                var fileArray = Directory.GetFiles(templatePath);
                for (int i = 0; i < fileArray.Length; i++)
                {
                    var fileName = fileArray[i];
                    if (Path.GetFileNameWithoutExtension(fileName) == name)
                    {
                        var destFilePath = Path.Combine(destPath, newName + exteName + Path.GetExtension(fileName));
                        File.Copy(fileName, destFilePath, true);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        #region 分割矢量图形

        /// <summary>
        /// 图层分表
        /// </summary>
        public void SplitFile(List<string> pathList)
        {
            string strDriverName = "ESRI Shapefile"; //创建数据,创建ESRI的shp文件

            var driver = OSGeo.OGR.Ogr.GetDriverByName(strDriverName);
            if (driver == null)
                return;
            var folder = Path.GetDirectoryName(pathList[0]);
            var ods = driver.Open(folder, 1);
            if (ods == null)
                return;
            //Directory.CreateDirectory(splitePath);
            var files = Directory.GetFiles(splitePath);
            List<string> delList = new List<string>();

            pathList.ForEach(t => delList.Add(Path.GetFileNameWithoutExtension(t)));

            for (int i = 0; i < files.Length; i++)
            {
                var n = Path.GetFileNameWithoutExtension(files[i]);
                if (delList.Exists(t => t == n))
                {
                    try
                    {
                        File.Delete(files[i]);
                    }
                    catch
                    { }
                }
            }

            int dknumber = 1500000;
            int jzdnumber = 6100000;
            int jzxnumber = 220000;
            try
            {
                var spliteNumber = System.Configuration.ConfigurationManager.AppSettings["SplitNumber"].ToString();
                var ss = spliteNumber.Split('|');
                if (ss.Length == 3)
                {
                    int.TryParse(ss[0], out dknumber);
                    int.TryParse(ss[1], out jzdnumber);
                    int.TryParse(ss[2], out jzxnumber);
                }
            }
            catch
            { }

            foreach (var item in pathList)
            {
                int number = 1000000;
                var name = Path.GetFileNameWithoutExtension(item);
                var layer = ods.GetLayerByName(name);
                if (name.ToUpper().Contains("DK"))
                {
                    number = dknumber;
                }
                else if (name.ToUpper().Contains("JZD"))
                {
                    number = jzdnumber;
                }
                else if (name.ToUpper().Contains("JZX"))
                {
                    number = jzxnumber;
                }

                if (layer != null)
                {
                    SpaliteFile(name, layer, splitePath, number);
                    layer.Dispose();
                }
            }
            ods.Dispose();
            driver.Dispose();
        }

        /// <summary>
        /// 分割矢量文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="layer"></param>
        /// <param name="folder"></param>
        private void SpaliteFile(string name, Layer layer, string folder, int number)
        {
            FeatureDefn oDefn = layer.GetLayerDefn();
            int layerIndex = 0;
            var count = layer.GetFeatureCount(1);
            if (number > count)
                return;
            double per = 100 / (double)count;
            Layer newLayer = null;
            string strDriverName = "ESRI Shapefile";
            OSGeo.OGR.Driver driver = Ogr.GetDriverByName(strDriverName);
            OSGeo.OGR.DataSource datasource = null;
            if (driver == null)
            {
                return;
            }
            Info(string.Format("按{0}条记录分割文件{1}", number, name + ".shp"));
            for (int i = 0; i < count; i++)
            {
                Process(Math.Round(per * i, 2));
                if (i == 0 || i % number == 0 || i == count - 1)
                {
                    if (i == count - 1)
                    {
                        newLayer.CreateFeature(layer.GetFeature(i));
                        if (newLayer != null)
                            newLayer.Dispose();
                        if (datasource != null)
                            datasource.Dispose();
                        break;
                    }
                    if (newLayer != null)
                        newLayer.Dispose();
                    if (datasource != null)
                        datasource.Dispose();
                    layerIndex++;
                    var newname = name + "-" + layerIndex;
                    var newp = folder + "\\" + newname + ".shp";
                    if (File.Exists(newp))
                    {
                        File.Delete(newp);
                        File.Delete(Path.ChangeExtension(newp, ".shx"));
                        File.Delete(Path.ChangeExtension(newp, ".dbf"));
                        File.Delete(Path.ChangeExtension(newp, ".prj"));
                    }
                    datasource = driver.CreateDataSource(newp, null); // 创建数据源
                    if (datasource != null)
                    {
                        var type = layer.GetGeomType();
                        var spatial = layer.GetSpatialRef();

                        newLayer = datasource.CreateLayer(newp, spatial, type, null);

                        int iFieldCount = oDefn.GetFieldCount();
                        for (int iField = 0; iField < iFieldCount; iField++)
                        {
                            FieldDefn oFieldDefn = oDefn.GetFieldDefn(iField);
                            newLayer.CreateField(oFieldDefn, 1);
                        }
                    }
                }
                newLayer.CreateFeature(layer.GetFeature(i));
            }
            if (newLayer != null)
                newLayer.Dispose();
            if (datasource != null)
                datasource.Dispose();
        }

        #endregion

    }
}
