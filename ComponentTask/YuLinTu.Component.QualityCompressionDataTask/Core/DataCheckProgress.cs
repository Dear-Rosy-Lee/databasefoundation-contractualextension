using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using DotSpatial.Projections;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using Quality.Business.Entity;
using Quality.Business.TaskBasic;
using YuLinTu.Appwork;
using YuLinTu.Data;
using YuLinTu.Spatial;
using YuLinTu.tGISCNet;
using YuLinTu.Windows;
using File = System.IO.File;
using SpatialReference = YuLinTu.Spatial.SpatialReference;

namespace YuLinTu.Component.QualityCompressionDataTask
{
    public class DataCheckProgress
    {
        /// <summary>
        /// 图形SRID
        /// </summary>
        private int srid;

        //private Dictionary<string, int> codeIndex;

        private string dkShapeFilePath;//用于判断是否读取了不同的地块shp

        /// <summary>
        /// 参数
        /// </summary>
        public QualityCompressionDataArgument DataArgument { get; set; }

        public static string ErrorInfo { get; private set; }

        public IDataSource Source { get; set; }


        public string Check()
        {
            //获取当前路径下shape数据

            string filepath;
            string filename;
            try
            {
                filepath = Path.GetDirectoryName(DataArgument.CheckFilePath);
                filename = Path.GetFileNameWithoutExtension(DataArgument.CheckFilePath);
                if (!CheckFile(filepath, filename))
                    return  ErrorInfo;
                var sr = GetByFile(filepath + "\\" + filename);
                srid = sr.WKID;
                if (srid == 4490)
                {
                    ErrorInfo = "请将当前坐标系转为投影坐标系后，再进行检查！";
                    return ErrorInfo;
                }
                var token = Parameters.Token.ToString();
                if (Parameters.Token.Equals(Guid.Empty))
                {
                    ErrorInfo = "请先登录后,再进行检查";
                    return ErrorInfo;
                }
                List<LandEntity> ls = new List<LandEntity>();
                var landShapeList = InitiallShapeLandList(DataArgument.CheckFilePath, srid,"");
                if (landShapeList.IsNullOrEmpty())
                {
                    return ErrorInfo;
                }
                foreach (var item in landShapeList)
                {
                    var land = new LandEntity();
                    land.dkbm = item.DKBM;
                    var landShape = item.Shape as YuLinTu.Spatial.Geometry;
                    land.ewkt = $"SRID={sr.WKID};{landShape.GeometryText}";
                    land.qqdkbm = item.QQDKBM;
                    ls.Add(land);
                }
                ApiCaller apiCaller = new ApiCaller();
                apiCaller.client = new HttpClient();
                string zonecode = Parameters.Region.ToString();
                string baseUrl = TheApp.Current.GetSystemSection().TryGetValue(AppParameters.stringDefaultSystemService, AppParameters.stringDefaultSystemServiceValue);
                string postGetTaskIdUrl = $"{baseUrl}/ruraland/api/topology/check";
                // 发送 GET 请求
                //res = await apiCaller.GetDataAsync(postUrl);
                // 发送 POST 请求
                string jsonData = JsonConvert.SerializeObject(ls);
                var getTaskID = apiCaller.PostGetTaskIDAsync(token, postGetTaskIdUrl, jsonData);
                string postGetResult = $"{baseUrl}/ruraland/api/tasks/schedule/job";
                var getResult = apiCaller.PostGetResultAsync(token, postGetResult, getTaskID);
                ErrorInfo = apiCaller.ErrorInfo;
                if (!getResult.IsNullOrEmpty())
                {
                    var folderPath = CreateLog();
                    WriteLog(folderPath, getResult);
                    ErrorInfo = "图斑存在拓扑错误,详情请查看检查结果";
                    return ErrorInfo;
                }
                if (getResult == null)
                {
                    return ErrorInfo;
                }
                return "";
            }
            catch (Exception ex)
            {
                ErrorInfo = ex.Message;
                return ErrorInfo;
            }

        }
        public static SpatialReference GetByFile(string fileName)
        {
            var prjFile = Path.ChangeExtension(fileName, ".prj");
            if (!File.Exists(prjFile))
                throw new FileNotFoundException("获取坐标系失败", prjFile);
            var result = new SpatialReference(File.ReadAllText(prjFile));
            if (result.WKID == 0)
            {
                var pi = ProjectionInfo.Open(prjFile);
                var file = "";
                if (pi.Name != null)
                    file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        "Data/SpatialReferences/Projected Coordinate Systems/Gauss Kruger/CGCS2000",
                        pi.Name.Replace("_", " ") + ".prj");
                if (File.Exists(file))
                {
                    return new SpatialReference(File.ReadAllText(file));
                }
                result.WKID = 4490;
            }
            return result;
        }
        public bool CheckFile(string filepath, string filename)
        {
            // 校验文件名称
            if (!Regex.IsMatch(filename, @"^DK\d{10}$"))
            {
                ErrorInfo = "文件名名称不正确，应以DK(6位县级区划代码)(4 位年份代码)为名称";
                return false;
            }
            // 校验文件是否存在
            string[] requiredExtensions = { ".dbf", ".prj", ".shp", ".shx" };

            foreach (string extension in requiredExtensions)
            {
                string fullPath = Path.Combine(filepath, filename + extension);
                if (!File.Exists(fullPath))
                {
                    ErrorInfo = $"缺少文件: {filename + extension}";
                    return false;
                }
            }


            return true;
        }
        public string CreateLog()
        {
            // 指定文件夹路径
            string folderPath = DataArgument.ResultFilePath;
            string fileName = $"检查结果{DateTime.Now.ToString("yyyy年M月d日HH时mm分")}.txt";
            // 合成完整文件路径
            folderPath = Path.Combine(folderPath, fileName);
            File.WriteAllText(folderPath, "检查结果记录:\n");
            return folderPath;
        }

        public void WriteLog(string path, KeyValueList<string, string> mes)
        {
            int i = 0;
            foreach (var item in mes)
            {
                i++;
                IEnumerable<string> stringCollection = new[] { $"{i}. {item.Key}:{item.Value.Substring(0, item.Value.Length - 2)}" };
                File.AppendAllLines(path, stringCollection);
            }
        }

        static private bool CheckField(ShapeFile shp, string ErrorInfo)
        {
            var infoArray = typeof(DKEX).GetProperties();
            for (int i = 0; i < infoArray.Length; i++)
            {

                var info = infoArray[i];
                var index = shp.FindField(info.Name);
                switch (info.Name)
                {
                    case "CBFBM":
                        if (index == -1)
                        {
                            ErrorInfo = "shp文件未包含CBFBM字段；";

                        }
                        break;
                    case "DKBM":
                        if (index == -1)
                        {
                            ErrorInfo += "shp文件未包含DKBM字段；";

                        }
                        break;
                    case "QQDKBM":
                        if (index == -1)
                        {
                            ErrorInfo += "shp文件未包含QQDKBM字段；";

                        }
                        break;
                    case "Shape":
                        //if (index == -1)
                        //{
                        //    ErrorInfo += "shp文件未包含Shape字段；";

                        //}
                        break;
                }
            }
            if (!ErrorInfo.IsNullOrEmpty())
            {
                return false;
            }
            return true;
        }

        static public List<DKEX> InitiallShapeLandList(string filePath, int srid, string zoneCode = "")
        {
            var dkList = new List<DKEX>();

            if (filePath == null || string.IsNullOrEmpty(filePath))
            {
                return dkList;
            }
            //codeIndex.Clear();
            using (var shp = new ShapeFile())
            {
                var err = shp.Open(filePath);
                if (!string.IsNullOrEmpty(err))
                {
                    LogWrite.WriteErrorLog("读取地块Shape文件发生错误" + err);
                    return null;
                }
                var codeIndex = new Dictionary<string, int>();

                if (!CheckField(shp, ErrorInfo))
                    return null;

                foreach (var dk in ForEnumRecord<DKEX>(shp, filePath, codeIndex, srid, DK.CDKBM, zoneCode))
                {
                    dkList.Add(dk);
                }
            }
            return dkList;
        }

        static public IEnumerable<T> ForEnumRecord<T>(ShapeFile shp, string fileName, Dictionary<string, int> codeIndex,
          int srid, string mainField = "", string zoneCode = "", bool setGeo = true) where T : class, new()
        {
            var infoArray = typeof(T).GetProperties();
            var fieldIndex = new Dictionary<string, int>();
            bool isSelect = (mainField != "" && zoneCode != "") ? true : false;

            int dkbmindex = -1;
            for (int i = 0; i < infoArray.Length; i++)
            {
                var info = infoArray[i];

                var index = shp.FindField(info.Name);

                if (index >= 0)
                {
                    fieldIndex.Add(info.Name, index);
                }

                if (info.Name == mainField)
                {
                    dkbmindex = index;
                }
            }

            if (codeIndex.Count > 0)
            {
                foreach (var item in codeIndex)
                {
                    if (isSelect)
                    {
                        if (dkbmindex < 0)
                            continue;

                        if (!item.Key.StartsWith(zoneCode))
                            continue;
                    }
                    var en = new T();
                    for (int i = 0; i < infoArray.Length; i++)
                    {
                        var info = infoArray[i];
                        if (!fieldIndex.ContainsKey(info.Name))
                            continue;
                        info.SetValue(en, FieldValue(item.Value, fieldIndex[info.Name], shp, info), null);
                    }
                    ObjectExtension.SetPropertyValue(en, "Shape", shp.GetGeometry(item.Value, srid));
                    yield return en;
                }
            }
            else
            {
                var shapeCount = shp.GetRecordCount();
                for (int i = 0; i < shapeCount; i++)
                {
                    var en = new T();

                    if (isSelect)
                    {
                        if (dkbmindex < 0)
                            continue;

                        var strValue = shp.GetFieldString(i, dkbmindex);
                        if (strValue == null)
                        {
                            continue;
                        }
                        if (!codeIndex.ContainsKey(strValue))
                            codeIndex.Add(strValue, i);
                        if (!strValue.StartsWith(zoneCode))
                            continue;
                    }
                    for (int j = 0; j < infoArray.Length; j++)
                    {
                        var info = infoArray[j];
                        if (!fieldIndex.ContainsKey(info.Name))
                            continue;
                        var value = FieldValue(i, fieldIndex[info.Name], shp, info);
                        info.SetValue(en, value, null);

                    }
                    if (setGeo)
                        ObjectExtension.SetPropertyValue(en, "Shape", shp.GetGeometry(i, srid));
                    yield return en;
                }
            }

        }
        /// <summary>
        /// 字段值获取
        /// </summary>
        static private object FieldValue(int row, int colum, ShapeFile dataReader, PropertyInfo info)
        {
            object value = null;
            if (info.Name == "BSM")
            {
                int bsm = 0;
                int.TryParse(dataReader.GetFieldString(row, colum), out bsm);
                value = bsm;
            }
            else if (info.Name.EndsWith("MJ") || info.Name.EndsWith("MJM") || info.Name == "CD" || info.Name == ZJ.CKD || info.Name == JZD.CXZBZ || info.Name == JZD.CYZBZ ||
                 info.Name == KZD.CX80 || info.Name == KZD.CY80 || info.Name == KZD.CY2000 || info.Name == KZD.CX2000)
            {
                double scmj = 0;
                var mjstr = dataReader.GetFieldString(row, colum);
                double.TryParse(mjstr.IsNullOrEmpty() ? "0" : mjstr, out scmj);
                value = scmj;
            }
            else
            {
                value = dataReader.GetFieldString(row, colum);
                value = value == null ? "" : value;
            }
            return value;
        }

        /// <summary>
        /// 坐标转换
        /// </summary>
        /// <param name="geo"></param>
        /// <param name="sreproject"></param>
        /// <param name="dreproject"></param>
        /// <param name="srid"></param>
        /// <returns></returns>
        public static YuLinTu.Spatial.Geometry ReprojectShape(YuLinTu.Spatial.Geometry geo, ProjectionInfo sreproject, ProjectionInfo dreproject, int srid)
        {
            var geos = geo.ToSingleGeometries();
            YuLinTu.Spatial.Geometry geometry = null;
            foreach (var g in geos)
            {
                GeoAPI.Geometries.Coordinate[] shels = null;
                GeoAPI.Geometries.ILinearRing[] hols = null;
                var list = new List<GeoAPI.Geometries.Coordinate>();
                if (g.Instance is Polygon)
                {
                    var pg = g.Instance as Polygon;
                    shels = pg.Shell.Coordinates;
                    hols = pg.Holes;
                }
                else if (g.Instance is Point)
                {
                    var pg = g.Instance as Point;
                    shels = pg.Coordinates;
                    hols = new LinearRing[0];
                }
                else if (g.Instance is LinearRing)
                {
                    var pg = g.Instance as LinearRing;
                    shels = pg.Coordinates;
                    hols = new LinearRing[0];
                }
                foreach (var sl in shels)
                {
                    var xy = new double[] { sl.X, sl.Y };
                    Reproject.ReprojectPoints(xy, new double[] { 0 }, sreproject, dreproject, 0, 1);
                    list.Add(new GeoAPI.Geometries.Coordinate(xy[0], xy[1]));
                }
                var nholes = new List<GeoAPI.Geometries.ILinearRing>();
                foreach (var ho in hols)
                {
                    var hlist = new List<GeoAPI.Geometries.Coordinate>();
                    foreach (var h in ho.Coordinates)
                    {
                        var xy = new double[] { h.X, h.Y };
                        Reproject.ReprojectPoints(xy, new double[] { 0 }, sreproject, dreproject, 0, 1);
                        hlist.Add(new GeoAPI.Geometries.Coordinate(xy[0], xy[1]));
                    }
                    var hlinearRing = new LinearRing(hlist.ToArray());
                    nholes.Add(hlinearRing);
                }
                var linearRing = new LinearRing(list.ToArray());

                var polygon = new Polygon(linearRing, nholes.ToArray());
                if (geometry == null)
                    geometry = YuLinTu.Spatial.Geometry.FromInstance(polygon);
                else
                    geometry = geometry.Union(YuLinTu.Spatial.Geometry.FromInstance(polygon));
            }
            if (geometry != null)
                geometry.SpatialReference = new SpatialReference(srid);
            return geometry;
        }
    }
}
