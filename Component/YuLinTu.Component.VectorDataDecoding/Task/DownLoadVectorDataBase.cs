using DotSpatial.Projections;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu;
using YuLinTu.Data.Shapefile;
using YuLinTu.Data;
using YuLinTu.Component.VectorDataDecoding.Core;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace YuLinTu.Component.VectorDataDecoding.Task
{

    public class DownLoadVectorDataBase : YuLinTu.Task
    {
        #region Properties

        #endregion

        #region Fields
        internal IVectorService vectorService { get; set; }
        #endregion

        #region Ctor

        public DownLoadVectorDataBase()
        {
            Name = "DownLoadVectorDataBase";
            Description = "This is DownLoadVectorDataBase";
        
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected  override void OnGo()
        {
           
            vectorService = new VectorService();
            // TODO : 任务的逻辑实现


        
        }
        internal void ExportToShape(string filename, List<IFeature> list, ProjectionInfo prjinfo)
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

        internal  void CompressFolder(string sourceFile, string zipFilePath, string zonecode)
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
        internal AttributesTable CreateAttributesSimple(SpaceLandEntity en)
        {
            AttributesTable attributes = new AttributesTable();
            attributes.AddAttribute("DKBM", en.DKBM);
            attributes.AddAttribute("DKMC", en.DKMC);

            attributes.AddAttribute("CBFBM", en.CBFBM);
            return attributes;
        }
        /// <summary>
        /// 创建表头
        /// </summary> 
        /// <returns></returns>
        internal DbaseFileHeader CreateHeader()
        {
            DbaseFileHeader header = new DbaseFileHeader(Encoding.UTF8);//Encoding.GetEncoding(936));  
            header.AddColumn("DKBM", 'C', 19, 0);
            header.AddColumn("DKMC", 'C', 50, 0);

            header.AddColumn("CBFBM", 'C', 18, 0);
            return header;
        }
        /// <summary>
        /// 删除已存在的文件
        /// </summary>
        internal void ClearExistFiles(IProviderShapefile provider, string elementName)
        {
            string path = string.Empty;
            try
            {
                var files = Directory.GetFiles(provider.DirectoryName, string.Format("{0}.*", elementName));
                files.ToList().ForEach(c => File.Delete(path = c));
            }
            catch (Exception ex)
            {
                //YuLinTu.Library.Log.Log.WriteException(this, "ClearExistFiles", ex.Message + ex.StackTrace);
                throw new Exception("删除文件" + path + "时发生错误！");
            }
        }
        #endregion

        #endregion
    }
}
