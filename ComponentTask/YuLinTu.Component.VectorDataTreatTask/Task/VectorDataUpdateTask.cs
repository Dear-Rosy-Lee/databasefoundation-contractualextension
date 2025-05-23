using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using DotSpatial.Projections;
using Newtonsoft.Json;
using YuLinTu.Appwork;
using YuLinTu.Component.Account.Models;
using YuLinTu.Library.Log;
using YuLinTu.Windows;

namespace YuLinTu.Component.VectorDataTreatTask
{
    /// <summary>
    /// 地块空间数据挂接任务
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "地块矢量数据挂接",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/map--arrow.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/map.png")]
    public class VectorDataUpdateTask : Task
    {
        #region Ctor

        public VectorDataUpdateTask()
        {
            Name = "地块矢量数据挂接";
            Description = "将矢量图斑按地块编码匹配更新到平台数据库中";
        }

        #endregion Ctor

        #region Fields

        private VectorDataUpdateArgument argument;

        private int srid = 0;
        private string prjstr;

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

            string zonecode = AppGlobalSettings.Current.TryGetValue(Parameters.RegionName, "");// Parameters.Region.ToString();
            string baseUrl = AppGlobalSettings.Current.TryGetValue("DefaultSystemService", AppParameters.stringDefaultSystemServiceValue);
            string postGetTaskIdUrl = $"/ruraland/api/topology/update/shape/{zonecode}/false/by/dkbm";
            var sourceFolder = argument.CheckFilePath.Substring(0, argument.CheckFilePath.Length - 4);
            //进行质检 
            if (DataCheck())
            {
                try
                {
                    ProcessDataOnline(baseUrl, postGetTaskIdUrl, token, zonecode);
                    this.ReportProgress(100);
                    this.ReportInfomation("数据处理成功。");
                    //CompressAndEncryptFolder(sourceFolderPath, zipFilePath, password);
                }
                catch (Exception ex)
                {
                    Log.WriteException(this, "OnGo(数据处理失败!)", ex.Message + ex.StackTrace);
                    this.ReportError(string.Format($"数据处理出错!{ex.Message} {baseUrl}"));
                }
            }
        }

        #endregion Method - Override

        #region Method - Private

        /// <summary>
        /// 参数验证
        /// </summary>
        /// <returns></returns>
        private bool ValidateArgs()
        {
            var args = Argument as VectorDataUpdateArgument;

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
                    this.ReportError(string.Format("待处理数据文件路径为空，请重新选择"));
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
        /// 数据检查
        /// </summary>
        /// <returns></returns>
        private bool DataCheck()
        {
            try
            {
                ErrorInfo = "";
                var filelist = GetAllFiles(argument.CheckFilePath);
                string filepath = Path.GetDirectoryName(filelist[0]);
                string filename = Path.GetFileNameWithoutExtension(filelist[0]);
                if (!CheckFile(filepath, filename))
                    return false;
                var sr = VectorDataProgress.GetByFile(filepath + "\\" + filename);
                srid = sr.WKID;
                prjstr = sr.WKT;
                if (srid == 0)
                {
                    this.ReportError("未正确获取到矢量文件的srid,请修正数据后再此执行！");
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
        /// 在线处理数据
        /// </summary>
        /// <returns></returns>
        public void ProcessDataOnline(string url, string murl, string key, string szdy)
        {
            int srid = 0;
            var filelist = GetAllFiles(argument.CheckFilePath);
            if (filelist.Count == 0)
            {
                return;
            }

            var landprj = Path.ChangeExtension(filelist[0], "prj");
            using (var sreader = new StreamReader(landprj))
            {
                if (string.IsNullOrEmpty(prjstr))
                    prjstr = sreader.ReadToEnd();
                //int.TryParse(GetMarkValue(prjstr, "EPSG", 6), out srid);
            }

            string prj4490 = "GEOGCS[\"GCS_China_Geodetic_Coordinate_System_2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]]";
            ProjectionInfo sreproject = ProjectionInfo.FromEsriString(prj4490);
            ProjectionInfo dreproject = ProjectionInfo.FromEsriString(prjstr);
            this.ReportProgress(5, "开始处理数据");
            var p = 90.0 / filelist.Count();
            int pindex = 0;
            var datacount = 0;
            int findex = 0;
            foreach (var file in filelist)
            {
                int index = 0;
                int fupdatacount = 0;
                int fcount = 0;
                VectorDataProgress.InitiallShapeLandList(file, srid, false, (rowcount, landShapeList) =>
                {
                    fcount = rowcount;
                    if (landShapeList == null)
                    {
                        return;
                    }
                    datacount += landShapeList.Count;
                    List<LandEntity> ls = new List<LandEntity>();
                    foreach (var item in landShapeList)
                    {
                        var land = new LandEntity();
                        land.dkbm = item.DKBM;
                        land.qqdkbm = item.QQDKBM;
                        if (argument.UseOldNumber)
                        {
                            land.dkbm = land.qqdkbm;
                        }
                        if (string.IsNullOrEmpty(land.dkbm))
                            continue;
                        var ygeo = item.Shape as YuLinTu.Spatial.Geometry;
                        if (!dreproject.Equals(sreproject))
                            ygeo = VectorDataProgress.ReprojectShape(ygeo, dreproject, sreproject, 4490);
                        land.ewkt = $"SRID=4490;{ygeo.GeometryText}";
                        ls.Add(land);
                        if (ls.Count == 100)
                        {
                            var upcountstr = DataProcessOnLine(url, murl, ls, key, szdy);
                            int upint = 0;
                            int.TryParse(upcountstr, out upint);
                            pindex += upint;
                            fupdatacount += upint;
                            ls.Clear();
                        }
                        index++;
                        this.ReportProgress(1 + (int)(findex * p), $"({index}/{rowcount},{Path.GetFileName(file)}) 正在更新数据");
                    }
                    if (ls.Count > 0)
                    {
                        var upcountstr = DataProcessOnLine(url, murl, ls, key, szdy);
                        int upint = 0;
                        int.TryParse(upcountstr, out upint);
                        pindex += upint;
                        fupdatacount += upint;
                        ls.Clear();
                    }

                }, "");
                findex++;
                this.ReportInfomation($"文件{file}共{fcount}条，成功挂接数据{fupdatacount}条");
            }
            this.ReportInfomation($"共{filelist.Count}个文件，共{datacount}条数据，成功更新{pindex}条");
            this.ReportProgress(100);
        }

        /// <summary>
        /// 获取所有的文件
        /// </summary>
        /// <returns></returns>
        private List<string> GetAllFiles(string path)
        {
            List<string> files = new List<string>();
            var dirs = Directory.GetDirectories(path);
            if (dirs.Count() > 0)
            {
                foreach (var dir in dirs)
                {
                    files.AddRange(GetAllFiles(dir));
                }
            }
            var filepaths = Directory.GetFiles(path);
            foreach (var item in filepaths)
            {
                if (Path.GetExtension(item).ToLower() == ".shp")
                    files.Add(item);
            }
            return files;
        }

        /// <summary>
        /// 在线处理数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string DataProcessOnLine(string baseUrl, string murl, List<LandEntity> data, string token, string szdy)
        {
            ApiCaller apiCaller = new ApiCaller();
            apiCaller.client = new HttpClient();
            string jsonData = JsonConvert.SerializeObject(data);
            try
            {
                //var getTaskID = apiCaller.(token, $"{baseUrl}{murl}", jsonData);
                //string postGetResult = $"{baseUrl}/ruraland/api/tasks/schedule/job";
                var getResult = apiCaller.PostDataAsync($"{baseUrl}{murl}", jsonData, token, szdy);
                //var getresult = apiCaller.PostDataAsync(url, jsonData, token);
                return getResult;// ConstructCoordsFromString(getResult);
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
        private string ConstructCoordsFromString(KeyValueList<string, string> pointstr)
        {
            if (pointstr == null)
            {
                return "更新图斑时服务器出现问题";
            }
            if (pointstr.Count > 0)
            {
                var ts = pointstr[0].Value;//.GetValue("results");
                return $"成功更新{ts}条数据";
            }
            return "";
        }

        #endregion Method - Private

        #endregion Methods
    }

}