using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotSpatial.Projections;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using YuLinTu.Data;
using YuLinTu.Data.Shapefile;


namespace YuLinTu.Component.QualityCompressionDataTask
{
    /// <summary>
    /// 质检、加密、压缩矢量数据成果任务
    /// </summary>
    [TaskDescriptor(IsLanguageName = false, Name = "加密矢量数据成果",
        //Gallery = "矢量数据成果处理",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class QualityCompressionData : Task
    {
        #region Ctor

        public QualityCompressionData()
        {
            Name = "质检、加密、压缩矢量数据成果";
            Description = "质检、加密、压缩矢量数据成果";
        }

        #endregion Ctor

        #region Fields

        private QualityCompressionDataArgument argument;

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

            this.ReportProgress(1, "开始检查...");
            this.ReportInfomation("开始检查...");
            System.Threading.Thread.Sleep(100);

            //TODO 加密方式
            //var password = TheApp.Current.GetSystemSection().TryGetValue(
            //    Parameters.stringZipPassword,
            //    Parameters.stringZipPasswordValue);
            var sourceFolder = argument.CheckFilePath.Substring(0, argument.CheckFilePath.Length - 4);
            try
            {
                //进行质检
                var dcp = new DataCheckProgress();
                dcp.DataArgument = argument;
                dcp.ReportErrorMethod += (msg) =>
                {
                    this.ReportError(msg);
                };
                dcp.ReportProcess += (p) =>
                {
                    this.ReportProgress(p, "拓扑检查中...");
                };
                var falg = dcp.Check();
                this.ReportProgress(70);
                if (!falg)
                {
                    return;
                }
                else
                {
                    var vlgzonelist = dcp.VallageList;
                    var srid = dcp.Srid;
                    var prjinfo = dcp.prjInfo;
                    var zipFilePath = $"{argument.ResultFilePath}\\质量检查导出";
                    var p = 50.0 / vlgzonelist.Count;
                    int pi = 0;
                    foreach (var v in vlgzonelist)
                    {
                        List<QCDK> dks = DataCheckProgress.InitiallShapeLandList(argument.CheckFilePath, srid, v);
                        if (dks.Count == 0)
                            continue;
                        ExprotAndCompress(dks, v, zipFilePath, prjinfo);
                        pi++;
                        this.ReportProgress(70 + (int)(pi * p), "数据打包中");
                    }
                }
                //CompressAndEncryptFolder(sourceFolderPath, zipFilePath, password);
            }
            catch (Exception ex)
            {
                Library.Log.Log.WriteException(this, "OnGo(数据检查失败!)", ex.Message + ex.StackTrace);
                this.ReportError(string.Format("数据检查时出错!"));
            }
            //if (DataCheck())
            //{
            //    try
            //    {
            //        CompressFolder(sourceFolder, zipFilePath);
            //        var path = dcp.CreateLog();
            //        dcp.WriteLog(path, new KeyValueList<string, string>
            //        {
            //            new KeyValue<string, string>("", "已通过数据质检!\r")
            //        });
            //        this.ReportProgress(100);
            //        this.ReportInfomation("数据检查、加密成功。");
            //        //CompressAndEncryptFolder(sourceFolderPath, zipFilePath, password);
            //    }
            //    catch (Exception ex)
            //    {
            //        Library.Log.Log.WriteException(this, "OnGo(数据压缩失败!)", ex.Message + ex.StackTrace);
            //        this.ReportError(string.Format("数据压缩时出错!" + ex.Message));
            //    }
            //}
        }

        /// <summary>
        /// 导出及压缩
        /// </summary>
        public void ExprotAndCompress(List<QCDK> dks, string zonecode, string zipFilePath, ProjectionInfo projectionInfo)
        {
            List<IFeature> list = new List<IFeature>();
            foreach (var land in dks)
            {
                var attributes = CreateAttributesSimple(land);
                Feature feature = new Feature((land.Shape as Spatial.Geometry).Instance, attributes);
                list.Add(feature);
            }
            //输出Shape文件
            var dir = Path.Combine(zipFilePath, zonecode);
            Directory.CreateDirectory(dir);
            string filename = Path.Combine(dir, $"DK{zonecode}.shp");
            ExportToShape(filename, list, projectionInfo);
            // 加密压缩文件
            var zipPath = $"{zipFilePath}\\DK{zonecode}.zip";
            CompressFolder(filename, zipPath, zonecode);
            Directory.Delete(dir, true);
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
        /// 创建表头
        /// </summary> 
        /// <returns></returns>
        private DbaseFileHeader CreateHeader()
        {
            DbaseFileHeader header = new DbaseFileHeader(Encoding.UTF8);//Encoding.GetEncoding(936));  
            //header.AddColumn("DKBM", 'C', 19, 0);
            //header.AddColumn("DKMC", 'C', 50, 0);
            //header.AddColumn("QQDKBM", 'C', 19, 0);
            //header.AddColumn("CBFBM", 'C', 18, 0);

            header.AddColumn("DKBM", 'C', 19, 0);
            header.AddColumn("CBFBM", 'C', 18, 0);
            header.AddColumn("DKMC", 'C', 50, 0);
            header.AddColumn("SYQXZ", 'C', 4, 0);
            header.AddColumn("DKLB", 'C', 4, 0);
            header.AddColumn("TDLYLX", 'C', 4, 0);
            header.AddColumn("DLDJ", 'C', 4, 0);
            header.AddColumn("TDYT", 'C', 4, 0);
            header.AddColumn("SFJBNT", 'C', 4, 0);
            header.AddColumn("SCMJ", 'F', 15, 2);
            header.AddColumn("SCMJM", 'F', 15, 2);
            header.AddColumn("DKDZ", 'C', 50, 0);
            header.AddColumn("DKXZ", 'C', 50, 0);
            header.AddColumn("DKNZ", 'C', 50, 0);
            header.AddColumn("DKBZ", 'C', 50, 0);
            header.AddColumn("DKBZXX", 'C', 100, 0);
            header.AddColumn("ZJRXM", 'C', 100, 0);

            return header;
        }

        ///<summary>
        ///创建属性表
        ///</summary> 
        public AttributesTable CreateAttributesSimple(QCDK en)
        {
            AttributesTable attributes = new AttributesTable();
            attributes.AddAttribute("DKBM", en.DKBM);
            attributes.AddAttribute("CBFBM", en.DKBM);
            attributes.AddAttribute("DKMC", en.DKBM);
            attributes.AddAttribute("SYQXZ", en.DKBM);
            attributes.AddAttribute("DKLB", en.DKBM);
            attributes.AddAttribute("TDLYLX", en.DKBM);
            attributes.AddAttribute("DLDJ", en.DKBM);
            attributes.AddAttribute("TDYT", en.DKBM);
            attributes.AddAttribute("SFJBNT", en.DKBM);
            attributes.AddAttribute("SCMJ", en.DKBM);
            attributes.AddAttribute("SCMJM", en.DKBM);
            attributes.AddAttribute("DKDZ", en.DKBM);
            attributes.AddAttribute("DKXZ", en.DKBM);
            attributes.AddAttribute("DKNZ", en.DKBM);
            attributes.AddAttribute("DKBZ", en.DKBM);
            attributes.AddAttribute("DKBZXX", en.DKBM);
            attributes.AddAttribute("ZJRXM", en.DKBM);
            return attributes;
        }

        #endregion Method - Override

        #region Method - Private

        /// <summary>
        /// 参数验证
        /// </summary>
        /// <returns></returns>
        private bool ValidateArgs()
        {
            var args = Argument as QualityCompressionDataArgument;

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
                    this.ReportError(string.Format("待检查数据文件路径为空，请重新选择"));
                    return false;
                }
                if (args.ResultFilePath.IsNullOrEmpty())
                {
                    this.ReportError(string.Format("压缩文件路径为空，请重新选择"));
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
            //var loginRegion = AppGlobalSettings.Current.TryGetValue(Parameters.RegionName, "");
            var fileInfo = new FileInfo(sourceFile);
            var fname = Path.GetFileNameWithoutExtension(sourceFile);
            string[] matchingFiles = Directory.GetFiles(fileInfo.DirectoryName, $"{fname}.*", SearchOption.AllDirectories);

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
                //进行质检
                var dcp = new DataCheckProgress();
                dcp.DataArgument = argument;
                dcp.ReportErrorMethod += (msg) =>
                {
                    this.ReportError(msg);
                };
                var falg = dcp.Check();
                this.ReportProgress(50);
                if (!falg)
                {
                    return false;
                }
                else
                {
                    this.ReportInfomation("数据检查通过。");
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

        #endregion Method - Private

        #endregion Methods
    }
}