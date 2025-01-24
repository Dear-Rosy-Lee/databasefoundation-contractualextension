using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using YuLinTu.Data;
using Newtonsoft.Json;
using Quality.Business.Entity;
using YuLinTu.Spatial;
using Quality.Business.TaskBasic;
using YuLinTu.tGISCNet;
using System.Reflection;
using File = System.IO.File;
using System.Net.Http;
using SpatialReference = YuLinTu.Spatial.SpatialReference;
using DotSpatial.Projections;
using YuLinTu.Appwork;
using YuLinTu.Windows;

namespace YuLinTu.Component.UpdateShpByLandNumberTask
{
    public class DataCheckProgress
    {
        /// <summary>
        /// 图形SRID
        /// </summary>
        private int srid;

        private Dictionary<string, int> codeIndex;

        private string dkShapeFilePath;//用于判断是否读取了不同的地块shp

        /// <summary>
        /// 参数
        /// </summary>
        public UpdateShpByLandNumberDataArgument DataArgument { get; set; }

        public string ErrorInfo { get; set; }

        public IDataSource Source { get; set; }


        public bool Check()
        {
            //获取当前路径下shape数据

            string filepath;
            string filename;
            try
            {   
                filepath = Path.GetDirectoryName(DataArgument.UpdateFilePath);
                filename = Path.GetFileNameWithoutExtension(DataArgument.UpdateFilePath);
                var sr = GetByFile(filepath + "\\" + filename);
                srid = sr.WKID;
                var token = Parameters.Token.ToString();
                if (Parameters.Token.Equals(Guid.Empty))
                {
                    ErrorInfo = "请先登录后,再进行检查";
                    return false;
                }
                List<LandEntity> ls = new List<LandEntity>();
                var landShapeList = InitiallShapeLandList(DataArgument.UpdateFilePath, "");
                if (landShapeList.IsNullOrEmpty())
                {
                    ErrorInfo = "读取图斑文件错误,请检查图斑文件";
                    return false;
                }
                foreach (var item in landShapeList)
                {
                    var land = new LandEntity();
                    land.dkbm = item.DKBM;
                    var landShape = item.Shape as Geometry;
                    land.ewkt = $"SRID={sr.WKID};{landShape.GeometryText}";
                    
                    ls.Add(land);
                }
                ApiCaller apiCaller = new ApiCaller();
                apiCaller.client = new HttpClient();
                string baseUrl = TheApp.Current.GetSystemSection().TryGetValue(AppParameters.stringDefaultSystemService, AppParameters.stringDefaultSystemServiceValue);
                string postGetTaskIdUrl = $"{baseUrl}/ruraland/api/topology/check";
                // 发送 GET 请求
                //res = await apiCaller.GetDataAsync(postUrl);
                // 发送 POST 请求
                string jsonData = JsonConvert.SerializeObject(ls);
                using (var writer = System.IO.File.CreateText("data.json"))
                {
                    writer.Write(jsonData);
                }
                var getTaskID = apiCaller.PostGetTaskIDAsync(token,postGetTaskIdUrl, jsonData);
                string postGetResult = $"{baseUrl}/ruraland/api/tasks/schedule/job";
                var getResult = apiCaller.PostGetResultAsync(token, postGetResult, getTaskID);
                ErrorInfo = apiCaller.ErrorInfo;
                if (!getResult.IsNullOrEmpty())
                {
                    var folderPath = CreateLog();
                    WriteLog(folderPath , getResult);
                    ErrorInfo = "图斑存在拓扑错误,详情请查看检查结果";
                    return false;
                }
                if (getResult == null)
                {
                    return false;
                }
                return true;
            }
            catch(Exception ex)
            {
                ErrorInfo = ex.Message;
                return false;
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
            }
            result.WKID = 4490;
            return result;
        }

        public string CreateLog()
        {
            // 指定文件夹路径
            string folderPath = DataArgument.ResultFilePath;
            string fileName = $"更新结果{DateTime.Now.ToString("yyyy年M月d日HH时mm分")}.txt";
            // 合成完整文件路径
            folderPath = Path.Combine(folderPath, fileName);
            File.WriteAllText(folderPath, "更新结果记录:\n");
            return folderPath;
        }

        public void WriteLog(string path,KeyValueList<string, string> mes)
        {
            foreach (var item in mes)
            {
                IEnumerable<string> stringCollection = new[] { $"{item.Key}:{item.Value.Substring(0, item.Value.Length - 2)}" };
                File.AppendAllLines(path, stringCollection );
            }
        }

        public List<DKEX> InitiallShapeLandList(string filePath, string zoneCode = "")
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
                codeIndex = new Dictionary<string, int>();
                foreach (var dk in ForEnumRecord<DKEX>(shp, filePath, codeIndex, DK.CDKBM, zoneCode))
                {
                    dkList.Add(dk);
                }
            }
            return dkList;
        }

        public IEnumerable<T> ForEnumRecord<T>(ShapeFile shp, string fileName, Dictionary<string, int> codeIndex,
            string mainField = "", string zoneCode = "", bool setGeo = true) where T : class, new()
        {
            bool isSameDkShp = true;
            if (dkShapeFilePath.IsNullOrEmpty())
            {
                dkShapeFilePath = fileName;
            }
            else if (dkShapeFilePath.Equals(fileName) == false)
            {
                isSameDkShp = false;
            }

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

            if (codeIndex.Count > 0 && isSameDkShp)
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
        private object FieldValue(int row, int colum, ShapeFile dataReader, PropertyInfo info)
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
    }
}