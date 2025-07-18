using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Interop;
using DotSpatial.Projections;
using Newtonsoft.Json;
using YuLinTu.Appwork;
using YuLinTu.Component.Account.Models;
using YuLinTu.Library.Log;
using YuLinTu.tGISCNet;

namespace YuLinTu.Component.VectorDataLinkageTask
{
    /// <summary>
    /// 地块矢量上传接入任务
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "接入地块图斑上传",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/map--arrow.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/map.png")]
    public class VectorDataLinkageTask : Task
    {
        #region Ctor

        public VectorDataLinkageTask()
        {
            Name = "上传地块图斑至接入系统";
            Description = "将矢量图斑按地块编码匹配更新接入系统中";
        }

        #endregion Ctor

        #region Fields

        private VectorDataLinkageArgument argument;

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
            if (!ValidateArgs())
            {
                this.ReportProgress(100);
                return;
            }

            //var token = AppGlobalSettings.Current.TryGetValue(Parameters.TokeName, "");
            //if (string.IsNullOrEmpty(token))
            //{
            //    this.ReportError("请先登录后,再次进行操作");
            //    return;
            //}
            this.ReportProgress(1, "正在准备...");
            this.ReportInfomation("正在准备...");
            System.Threading.Thread.Sleep(200);

            string zonecode = AppGlobalSettings.Current.TryGetValue(Parameters.RegionName, "");// Parameters.Region.ToString();
            string baseUrl = AppGlobalSettings.Current.TryGetValue("DeliveryServiceAddres", AppParameters.stringDefaultAccountOnlineServiceValue); //AppParameters.stringDefaultSystemServiceValue);
            string postGetTaskIdUrl = $"/ruraland/api/open/api/dzzw/sjgj/dk/shape/upload/to/dk";
            var sourceFolder = argument.CheckFilePath.Substring(0, argument.CheckFilePath.Length - 4);
            //进行质检 
            if (DataCheck())
            {
                try
                {
                    ProcessDataOnline(baseUrl, postGetTaskIdUrl, "", zonecode);
                    this.ReportProgress(100);
                    this.ReportInfomation("数据上传到接入系统成功。");
                    //CompressAndEncryptFolder(sourceFolderPath, zipFilePath, password);
                }
                catch (Exception ex)
                {
                    Log.WriteException(this, $"OnGo(数据上传到接入系统失败!){baseUrl}", ex.Message + ex.StackTrace);
                    this.ReportError(string.Format($"{ex.Message} "));
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
            var args = Argument as VectorDataLinkageArgument;

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
                //var filelist = GetAllFiles(argument.CheckFilePath);
                string filepath = Path.GetDirectoryName(argument.CheckFilePath);
                string filename = Path.GetFileNameWithoutExtension(argument.CheckFilePath);
                if (!CheckFile(filepath, filename))
                    return false;
                var sr = VectorDataProgress.GetByFile(filepath + "\\" + filename);
                srid = sr.WKID;
                prjstr = sr.WKT;
                if (srid == 0)
                {
                    this.ReportError("未正确获取到矢量文件的srid,请修正数据后重试！");
                    return false;
                }
                if (srid == 4490 || srid == 4326)
                {
                    this.ReportError("上传的矢量文件需要投影坐标数据,请修正数据后重试！");
                    return false;
                }
                bool resut = true;
                VectorDataProgress.LandShapeCheck(argument.CheckFilePath, srid, (msg) =>
                {
                    resut = false;
                    this.ReportError(msg);

                }, (crow, carea) =>
                {
                    if (carea > 25000000)
                    {
                        resut = false;
                        this.ReportError("数据范围超过了25平方千米");
                    }
                });
                return resut;
                //CompressAndEncryptFolder(sourceFolderPath, zipFilePath, password);
            }
            catch (Exception ex)
            {
                Library.Log.Log.WriteException(this, "OnGo(数据检查失败!)", ex.Message + ex.StackTrace);
                this.ReportError(string.Format("数据检查时出错!"));
                return false;
            }
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
            if (!File.Exists(argument.CheckFilePath))
            {
                return;
            }
            var inp = VectorDataProgress.GetByFile(Path.ChangeExtension(argument.CheckFilePath, ".prj"));
            if (inp != null)
            {
                srid = inp.WKID;
            }
            if (srid == 0)
            {
                throw new Exception("未能正确获取矢量数据的坐标信息，请检查prj文件是否有EPSG值");
            }

            this.ReportProgress(5, "开始处理数据");
            var datacount = 0;
            int dindex = 0;
            using (var shp = new ShapeFile())
            {
                var err = shp.Open(argument.CheckFilePath);
                if (!string.IsNullOrEmpty(err))
                {
                    throw new Exception("读取地块Shape文件发生错误" + err);
                }
                datacount = shp.GetRecordCount();

                double p = 90.0 / datacount;

                var dkbmindex = VectorDataProgress.FileRecordIndex(shp, "dkbm");
                var zonelist = dkbmindex.Keys.Select(t => t.Substring(0, 14)).Distinct().ToList();

                foreach (var zoneCode in zonelist)
                {
                    UpdataCollection updataCollection = new UpdataCollection();
                    updataCollection.dks = new List<LandEntity>();
                    updataCollection.dybm = zoneCode;

                    foreach (var dk in VectorDataProgress.ForEnumRecord<QCDK>(shp, "", dkbmindex, srid, QCDK.CDKBM, zoneCode))
                    {
                        var land = new LandEntity();
                        land.dkbm = dk.DKBM;
                        var ygeo = dk.Shape as YuLinTu.Spatial.Geometry;
                        land.ewkt = $"SRID={srid};{ygeo.GeometryText}";
                        updataCollection.dks.Add(land);
                        dindex++;
                    }
                    DataProcessOnLine(url, murl, updataCollection, "3ca04787775f4ca682980cd58dd551d9", "3baSPLk4o0DB3AgGr4QqvGSdqr2G/SmVjTHXE196wQkGz2uxIeH9hA==");
                    this.ReportProgress(5 + (int)(p * dindex), "数据上传中...");
                    this.ReportInfomation($"地域{zoneCode}共上传到接入系统 {updataCollection.dks.Count} 条数据");
                }
            }
            this.ReportInfomation($"文件{argument.CheckFilePath}共上传到接入系统{datacount}条数据");
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
        private string DataProcessOnLine(string baseUrl, string murl, UpdataCollection data, string appid, string appkey)
        {
            ApiCaller apiCaller = new ApiCaller();
            apiCaller.client = new HttpClient();
            string jsonData = JsonConvert.SerializeObject(data);
            try
            {
                var getResult = apiCaller.PostDataAsync($"{baseUrl}{murl}", jsonData, appid, appkey);
                return getResult;// ConstructCoordsFromString(getResult);
            }
            catch (Exception ex)
            {
                ErrorInfo = "上传处理发生错误" + ex.Message;
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