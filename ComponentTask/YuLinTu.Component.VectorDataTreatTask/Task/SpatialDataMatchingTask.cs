using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using DotSpatial.Projections;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using YuLinTu.Component.Account.Models;
using YuLinTu.Component.VectorDataTreatTask.Core;
using YuLinTu.Data;
using YuLinTu.Data.Shapefile;
using YuLinTu.Library.Log;
using YuLinTu.Spatial;


namespace YuLinTu.Component.VectorDataTreatTask
{
    /// <summary>
    /// 在线匹配矢量数据成果任务
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "在线空间数据匹配",
        //Gallery = "矢量数据成果处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/GroupBlogInsertLinks.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/globe.png")]
    public class SpatialDataMatchingTask : Task
    {
        #region Ctor

        public SpatialDataMatchingTask()
        {
            Name = "在线空间数据匹配";
            Description = "将原始矢量图斑匹配到与保密处理的数据套合的位置";
        }

        #endregion Ctor

        #region Fields

        private SpatialDataMatchingArgument argument;

        #endregion Fields

        #region Properties

        private string ErrorInfo { get; set; }

        #endregion Properties

        #region Methods

        #region Method - Override

        /// <summary>
        /// 开始执行任务
        /// </summary>
        protected override void OnGo()
        {
            this.ReportProgress(0, "开始验证检查数据参数...");
            this.ReportInfomation("开始验证检查数据参数...");
            System.Threading.Thread.Sleep(200);
            if (!ValidateArgs())
            {
                this.ReportProgress(100);
                return;
            }
            var token = AppGlobalSettings.Current.TryGetValue(Parameters.TokeName, "");
            if (string.IsNullOrEmpty(token))
            {
                this.ReportError("请先登录后,再次进行操作");
                return;
            }
            this.ReportProgress(1, "开始更新...");
            this.ReportInfomation("开始更新...");
            System.Threading.Thread.Sleep(200);
            System.Threading.Thread.Sleep(200);
            System.Threading.Thread.Sleep(200);
                   var TreatmentKey = GetTreatmentKey(token); //ConfigurationManager.AppSettings.TryGetValue<string>("TreatmentKey", "57f90f07-66a9-46bd-96f2-61143e01365a");
            var TreatmentUrl = ConfigurationManager.AppSettings.TryGetValue<string>("TreatmentUrl", "http://www.scgis.net:8282/DrillingPlatformAPI/API/CrsTrans/Trans2CK_Multi");
            //TODO 加密方式
            //var password = TheApp.Current.GetSystemSection().TryGetValue(
            //    Parameters.stringZipPassword,
            //    Parameters.stringZipPasswordValue);
            var sourceFolder = argument.CheckFilePath.Substring(0, argument.CheckFilePath.Length - 4);
            var zipFilePath = $"{argument.ResultFilePath}\\{Path.GetFileName(sourceFolder)}.zip";
            //进行质检 
            if (DataCheck())
            {
                try
                {
                    string handlZoneCode = string.Empty;
                    bool isMatchSucess = ProcessDataOnline(TreatmentUrl, TreatmentKey, (temp) =>
                    {
                        handlZoneCode = temp;
                        bool result=temp.StartsWith(argument.ZoneCode) || argument.ZoneCode == "86";
                        if(result==false)
                        this.ReportError(string.Format("用户{0}无{1}地域的数据处理权限!", argument.UserName, temp));
                        return result;
                    });

                    if (!isMatchSucess) return;
                    if (argument.AutoComprass)
                        CompressFolder($"{argument.ResultFilePath}\\{Path.GetFileName(sourceFolder)}", zipFilePath, handlZoneCode);
                    this.ReportProgress(100);
                    this.ReportInfomation("数据处理成功。");
                    //CompressAndEncryptFolder(sourceFolderPath, zipFilePath, password);
                }
                catch (Exception ex)
                {
                    Log.WriteException(this, "OnGo(数据处理失败!)", ex.Message + ex.StackTrace);
                    this.ReportError(string.Format("数据处理出错!" + ex.Message));
                }
            }
        }

        protected override void OnAlert(TaskAlertEventArgs e)
        {
            base.OnAlert(e);
            var args = Argument as SpatialDataMatchingArgument;
            //argument.ZoneCode = AppGlobalSettings.Current.TryGetValue(Parameters.RegionName, "");// Parameters.Region.ToString();
            //argument.UserName = AppGlobalSettings.Current.TryGetValue(Parameters.UserCodName, "");
        }
        private string GetTreatmentKey(string token)
        {
            var url= ConfigurationManager.AppSettings.TryGetValue<string>("DefaultBusinessAPIAdress", "https://api.yizhangtu.com");
                url = url + "/stackcloud/api/open/api/dynamic/geoProcessing/tuomi/token";
            ApiCaller apiCaller = new ApiCaller();
            apiCaller.client = new HttpClient();
            Dictionary<string,string>headers = new Dictionary<string,string>();
            headers.Add("token", token);
            headers.Add("t-open-api-app-code", "cbdDataProcessing");
            var en=  apiCaller.GetResultAsync<PostGetAPIEntity>(url, headers);
            return en.token;

        }

        #endregion Method - Override

        #region Method - Private

        /// <summary>
        /// 参数验证
        /// </summary>
        /// <returns></returns>
        private bool ValidateArgs()
        {
            var args = Argument as SpatialDataMatchingArgument;

            if (args == null)
            {
                this.ReportError(string.Format("参数错误!"));
                return false;
            }
            argument = args;
            try
            {
                if (args.CheckFilePath.IsNullOrEmpty())
                {
                    this.ReportError(string.Format("待更新数据文件路径为空，请重新选择"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ValidateArgs(参数合法性检查失败!)", ex.Message + ex.StackTrace);
                return false;
            }
            this.ReportInfomation(string.Format("检查数据参数正确。"));
            return true;
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="sourceFile">待压缩目录</param>
        /// <param name="zipStream">压缩文件</param>
        private static void CompressFolder(string sourceFile, string zipFilePath, string zonecode)
        {
            var fileInfo = new FileInfo(sourceFile);
            string[] matchingFiles = Directory.GetFiles(fileInfo.DirectoryName, $"{fileInfo.Name}.*", SearchOption.AllDirectories);

            using (var fsOut = File.Create(zipFilePath))
            {
                using (var zipStream = new ZipOutputStream(fsOut))
                {
                    zipStream.SetLevel(5); // 压缩级别，0-9，9为最大压缩
                    zipStream.Password = zonecode + "Ylt@dzzw";
                    foreach (string matchingFile in matchingFiles)
                    {
                        var matchingFileInfo = new FileInfo(matchingFile);
                        string matchingFileInfoName = matchingFileInfo.Name;
                        string matchingFileEntryName = matchingFileInfoName; // 移除任何相对路径
                        var newEntry = new ZipEntry(matchingFileEntryName);
                        newEntry.DateTime = matchingFileInfo.LastWriteTime;
                        newEntry.Size = matchingFileInfo.Length;
                        zipStream.PutNextEntry(newEntry);
                        // 将文件内容写入zip流
                        byte[] buffer = new byte[4096];
                        using (FileStream streamReader = File.OpenRead(matchingFile))
                        {
                            StreamUtils.Copy(streamReader, zipStream, buffer);
                        }
                    }
                    zipStream.CloseEntry();
                }
            }
        }

        /// <summary>
        /// 数据检查
        /// </summary>
        /// <returns></returns>
        private bool DataCheck()
        {
            try
            {
                ErrorInfo = "";
                string filepath = Path.GetDirectoryName(argument.CheckFilePath);
                string filename = Path.GetFileNameWithoutExtension(argument.CheckFilePath);
                if (!CheckFile(filepath, filename))
                {
                    this.ReportError(ErrorInfo);
                    return false;
                }
       
                var sr = VectorDataProgress.GetByFile(filepath + "\\" + filename);
                var srid = sr.WKID;
                if (srid == 4490)
                {
                    ErrorInfo = "请将当前坐标系转为投影坐标系后，再进行检查！";
                    this.ReportError(ErrorInfo);
                    return false;
                }
                //CompressAndEncryptFolder(sourceFolderPath, zipFilePath, password);
            }
            catch (Exception ex)
            {
                Library.Log.Log.WriteException(this, "OnGo(数据检查失败!)", ex.Message + ex.StackTrace);
                this.ReportError(string.Format("数据检查时出错!"));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查必要文件
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool CheckFile(string filepath, string filename)
        {
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

        /// <summary>
        /// 构造数据成可解析额字符串（不支持多面）
        /// </summary>
        /// <returns></returns>
        public List<string> ConstructGeometryData(YuLinTu.Spatial.Geometry geo)
        {
            List<string> geostrlist = new List<string>();
            var geos = geo.ToSingleGeometries();
            StringBuilder sb = new StringBuilder();
            GeoAPI.Geometries.ILinearRing[] hols = null;
            foreach (var g in geos)
            {
                GeoAPI.Geometries.Coordinate[] shels = null;
                var list = new List<GeoAPI.Geometries.Coordinate>();
                if (g.Instance is Polygon)
                {
                    var pg = g.Instance as Polygon;
                    shels = pg.Shell.Coordinates;
                    hols = pg.Holes;
                }
                else if (g.Instance is Point)
                {
                    var pg = g.Instance;
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
                    sb.Append($"{sl.X},{sl.Y}");
                    sb.Append("|");
                }
            }
            geostrlist.Add(sb.ToString().TrimEnd('|'));
            sb.Clear();
            if (hols != null && hols.Count() > 0)
            {
                foreach (var ho in hols)
                {
                    var hlist = new List<GeoAPI.Geometries.Coordinate>();
                    foreach (var h in ho.Coordinates)
                    {
                        sb.Append($"{h.X},{h.Y}");
                        sb.Append("|");
                    }
                    geostrlist.Add(sb.ToString().TrimEnd('|'));
                    sb.Clear();
                }
            }
            return geostrlist;
        }

        /// <summary>
        /// 在线处理数据
        /// </summary>
        /// <returns></returns>
        public bool ProcessDataOnline(string url, string key,Func<string,bool> jaugeZone)
        {
            int srid = 0;
            string prjstr = "";
            string zonecode = "";
            var landprj = Path.ChangeExtension(argument.CheckFilePath, "prj");
            using (var sreader = new StreamReader(landprj))
            {
                prjstr = sreader.ReadToEnd();
                int.TryParse(GetMarkValue(prjstr, "EPSG", 6), out srid);
            }
            var landShapeList = VectorDataProgress.InitiallShapeLandList(argument.CheckFilePath, srid, "");
            if (landShapeList == null)
            {
                return false;
            }
            zonecode= landShapeList[0].DKBM.Substring(0, 12);
            bool zoneMatch = jaugeZone(zonecode);
            if (!zoneMatch) return zoneMatch;
            string prj4490 = "GEOGCS[\"GCS_China_Geodetic_Coordinate_System_2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]]";
            ProjectionInfo sreproject = ProjectionInfo.FromEsriString(prj4490);
            ProjectionInfo dreproject = ProjectionInfo.FromEsriString(prjstr);
            this.ReportProgress(5, "开始处理数据");
            var p = 90.0 / landShapeList.Count();
            int pindex = 1;
            List<IFeature> list = new List<IFeature>();
            foreach (var item in landShapeList)
            {
                var land = new SpaceLandEntity();
                land.DKBM = item.DKBM;
                if (string.IsNullOrEmpty(zonecode) && item.DKBM != null && item.DKBM.Length > 12)
                {
                    zonecode = item.DKBM.Substring(0, 12);
                }

                land.DKMC = item.DKMC;
                land.QQDKBM = item.QQDKBM;
                land.CBFBM = item.CBFBM;
                var ygeo = item.Shape as YuLinTu.Spatial.Geometry;
                ygeo = VectorDataProgress.ReprojectShape(ygeo, dreproject, sreproject, 4490);
                var geostr = ConstructGeometryData(ygeo);
                List<List<GeoAPI.Geometries.Coordinate>> coordinateslist = new List<List<GeoAPI.Geometries.Coordinate>>();
                foreach (var gs in geostr)
                {
                    var djen = new DataJsonEntity(gs, key);
                    coordinateslist.Add(DataProcessOnLine(url, djen));
                }

                var linearRing = new LinearRing(coordinateslist[0].ToArray());
                var nholes = new List<GeoAPI.Geometries.ILinearRing>();
                if (coordinateslist.Count > 1)
                {
                    for (int i = 1; i < coordinateslist.Count; i++)
                    {
                        var hlinearRing = new LinearRing(coordinateslist[i].ToArray());
                        nholes.Add(hlinearRing);
                    }
                }
                var polygon = new Polygon(linearRing, nholes.ToArray());
                var geometry = YuLinTu.Spatial.Geometry.FromInstance(polygon);
                geometry.SpatialReference = new SpatialReference(4490);
                land.Shape = VectorDataProgress.ReprojectShape(geometry, sreproject, dreproject, srid);

                var attributes = CreateAttributesSimple(land);
                Feature feature = new Feature(land.Shape.Instance, attributes);
                list.Add(feature);
                this.ReportProgress(5 + (int)(p * pindex), $"({pindex}/{landShapeList.Count})数据处理中..");
                pindex++;
            }

            //输出Shape文件
            string filename = Path.GetFileName(argument.CheckFilePath);
            ExportToShape(Path.Combine(argument.ResultFilePath, filename), list, dreproject);
            // 加密压缩文件

            return zoneMatch;
        }

        /// <summary>
        /// 导出shape
        /// </summary>  
        public void ExportToShape(string filename, List<IFeature> list, ProjectionInfo prjinfo)
        {
            var builder = new ShapefileConnectionStringBuilder();
            builder.DirectoryName = Path.GetDirectoryName(filename);
            var elementName = Path.GetFileNameWithoutExtension(filename);
            if (list == null || list.Count == 0)
            {
                return;
            }
            var ds = DataSource.Create<IDbContext>(ShapefileConnectionStringBuilder.ProviderType, builder.ConnectionString);
            var provider = ds.DataSource as IProviderShapefile;
            ClearExistFiles(provider, elementName);
            var writer = provider.CreateShapefileDataWriter(elementName);
            writer.Header = CreateHeader();
            writer.Header.NumRecords = list.Count;
            writer.Write(list);
            prjinfo.SaveAs(Path.ChangeExtension(filename, "prj"));
            this.ReportInfomation(string.Format("成功导出{0}条数据", list.Count));
        }

        /// <summary>
        /// 创建表头
        /// </summary> 
        /// <returns></returns>
        private DbaseFileHeader CreateHeader()
        {
            DbaseFileHeader header = new DbaseFileHeader(Encoding.UTF8);//Encoding.GetEncoding(936));  
            header.AddColumn("DKBM", 'C', 19, 0);
            header.AddColumn("DKMC", 'C', 50, 0);
            header.AddColumn("QQDKBM", 'C', 19, 0);
            header.AddColumn("CBFBM", 'C', 18, 0);
            return header;
        }

        /// <summary>
        /// 删除已存在的文件
        /// </summary>
        private void ClearExistFiles(IProviderShapefile provider, string elementName)
        {
            string path = string.Empty;
            try
            {
                var files = Directory.GetFiles(provider.DirectoryName, string.Format("{0}.*", elementName));
                files.ToList().ForEach(c => File.Delete(path = c));
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ClearExistFiles", ex.Message + ex.StackTrace);
                throw new Exception("删除文件" + path + "时发生错误！");
            }
        }


        /// <summary>
        /// 在线处理数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<GeoAPI.Geometries.Coordinate> DataProcessOnLine(string url, object data)
        {
            ApiCaller apiCaller = new ApiCaller();
            apiCaller.client = new HttpClient();
            //baseUrl = TheApp.Current.GetSystemSection().TryGetValue(AppParameters.stringDefaultSystemService, AppParameters.stringDefaultSystemServiceValue);
            //string baseUrl = "http://www.scgis.net:8282/DrillingPlatformAPI/API/CrsTrans/Trans2CK_Multi";
            //string postGetTaskIdUrl = $"{baseUrl}/ruraland/api/topology/check";
            // 发送 GET 请求
            //res = await apiCaller.GetDataAsync(postUrl);
            // 发送 POST 请求
            string jsonData = JsonConvert.SerializeObject(data);
            try
            {
                var getresult = apiCaller.PostDataAsync(url, jsonData);
                return ConstructCoordsFromString(getresult);
            }
            catch (Exception ex)
            {
                ErrorInfo = "图形处理发生错误,详情请查看检查结果";
                throw ex;
            }
        }

        /// <summary>
        /// 通过点坐标文件构造图形
        /// </summary>
        private List<GeoAPI.Geometries.Coordinate> ConstructCoordsFromString(string pointstr)
        {
            // "[[107.76314431520983, 31.52865244271285],[108.34139274678111, 32.110412310185495],[105.05750538077046, 28.847489731129755]]
            var array = pointstr.Split(']');
            //YuLinTu.Spatial.Geometry geometry = null;
            //List<List<Coordinate>> coordinateslist = new List<List<Coordinate>>();
            var coordinates = new List<GeoAPI.Geometries.Coordinate>();
            foreach (var ar in array)
            {
                if (string.IsNullOrEmpty(ar))
                    continue;
                var pse = ar.TrimStart(',').Replace("[", "").Replace("]", "");
                var pary = pse.Split(',');
                double x = double.Parse(pary[0]);
                double y = double.Parse(pary[1]);
                //if (x == 1000 && y == 20)
                //{
                //    coordinateslist.Add(coordinates);
                //    coordinates = new List<Coordinate>();
                //}
                //else
                //{
                coordinates.Add(new GeoAPI.Geometries.Coordinate(x, y));
                //}
            }
            return coordinates;
            //用点生成面
            //return YuLinTu.Spatial.Geometry.CreatePolygon(coordinates);
        }


        /// <summary>
        /// 获取标签值
        /// </summary> 
        private string GetMarkValue(string str, string name, int length)
        {
            var restr = string.Empty;
            var indexstart = str.LastIndexOf(name);
            if (indexstart == -1)
            {
                return "error";
            }
            if (indexstart != -1)
            {
                restr = str.Substring(indexstart + length);
                int indexend = restr.IndexOf("]]");
                if (indexend != -1)
                {
                    restr = restr.Substring(0, indexend);
                }
            }
            return restr;
        }

        ///<summary>
        ///创建属性表
        ///</summary> 
        public AttributesTable CreateAttributesSimple(SpaceLandEntity en)
        {
            AttributesTable attributes = new AttributesTable();
            attributes.AddAttribute("DKBM", en.DKBM);
            attributes.AddAttribute("DKMC", en.DKMC);
            attributes.AddAttribute("QQDKBM", en.QQDKBM);
            attributes.AddAttribute("CBFBM", en.CBFBM);
            return attributes;
        }
        #endregion Method - Private

        #endregion Methods
    }
}