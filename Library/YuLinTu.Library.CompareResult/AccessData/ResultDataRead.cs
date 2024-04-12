using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YuLinTu.Data;
using YuLinTu.tGISCNet;

namespace YuLinTu.Library.CompareResult
{
    public class ResultDataRead
    {
        public const string VictorName = "矢量数据";


        #region Properties

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        public FilePathInfo CurrentPath { get; set; }

        public TimeSpan UseTime { get; private set; }

        public SpaceProcess ShpProcess { get; set; }

        public int Srid { get; set; }

        #endregion

        #region Ctor

        public ResultDataRead()
        {
        }

        #endregion

        #region Methods
        public void InitiallPath(bool containsShapes = true)
        {
            CurrentPath = GetCurrent(FilePath);
            ShpProcess = new SpaceProcess(Srid, CurrentPath.ShapeFileList, containsShapes);
        }

        /// <summary>
        /// 获取当前文件路径
        /// </summary>
        public FilePathInfo GetCurrent(string filePath)
        {

            string[] files = Directory.GetFiles(filePath);
            if (files == null || files.Length == 0)
            {
                throw new Exception(filePath + "下不存在任何文件");
            }
            FilePathInfo currentPath = new FilePathInfo();
            currentPath.Path = filePath; 
            if (files.Length > 0)
            {
                currentPath.ShapeFileList = CreateNamePair(files);
            }
            if (currentPath.ShapeFileList.Count != 2)
            {
                throw new Exception(string.Format("请确认{0}下有且仅有2份Shape文件。", filePath));
            }
            else
            {
                currentPath.SourceShapeFile = currentPath.ShapeFileList[0];
                currentPath.TargetShapeFile = currentPath.ShapeFileList[1];
            }

            return currentPath;
        }

        /// <summary>
        /// 获Shap文件信息
        /// </summary>
        /// <param name="directory">文件目录数组</param>
        /// <param name="extentionName">扩展名称</param>
        /// <param name="fileList">文件名集合</param>
        public static List<FileCondition> CreateNamePair(string[] directory, string extentionName = "", List<FileCondition> extentList = null, List<string> fileList = null)
        {
            List<FileCondition> namePairList = new List<FileCondition>();
            if (directory.Length == 0)
            {
                return namePairList;
            }
            List<string> otherTypeFile = new List<string>();
            for (int i = 0; i < directory.Length; i++)
            {
                string extentName = Path.GetExtension(directory[i]);
                if (extentName.Equals(".dbf") || extentName.Equals(".prj") ||
                       extentName.Equals(".shx") || extentName.Equals(".dbf")
                     || extentName.Equals(".sbx") || extentName.Equals(".sbn")
                    )
                {
                    otherTypeFile.Add(directory[i]);
                    continue;
                }
                var gexspare = new Regex("[a-zA-Z]{2,3}\\d{10}-\\d+");
                FileCondition fnp = new FileCondition();
                fnp.IsExist = true;
                fnp.FilePath = directory[i];
                fnp.ExtFileName = Path.GetFileName(fnp.FilePath);
                fnp.FullName = Path.GetFileNameWithoutExtension(fnp.FilePath);
                if (fileList != null)
                {
                    fileList.Add(fnp.FullName);
                }
                if (extentName.ToLower().Equals(".shp"))
                {
                    string dbffileName = Path.ChangeExtension(fnp.FilePath, "dbf");
                    string shxfileName = Path.ChangeExtension(fnp.FilePath, "shx");
                    string prjfileName = Path.ChangeExtension(fnp.FilePath, "prj");
                    if (!File.Exists(shxfileName))
                    {
                        fnp.LackInfo.Add("shx");
                        fnp.IsFileComplate = false;
                    }
                    if (!File.Exists(dbffileName))
                    {
                        fnp.LackInfo.Add("dbf");
                        fnp.IsFileComplate = false;
                    }
                    if (!File.Exists(prjfileName))
                    {
                        fnp.LackInfo.Add("prj");
                        fnp.IsFileComplate = false;
                    }
                    if (!string.IsNullOrEmpty(extentionName))
                    {
                        if (gexspare.IsMatch(fnp.FullName))
                        {
                            var gex = new Regex("\\d{10}-\\d+");
                            fnp.Name = gex.Replace(fnp.FullName, "");
                            //fnp.Name = fnp.FullName.Replace(extentionName + "-", "");
                        }
                        else
                            fnp.Name = fnp.FullName.Replace(extentionName, "");
                    }
                    else
                    {
                        if (gexspare.IsMatch(fnp.FullName))
                        {
                            var gex = new Regex("\\d{10}-\\d+");
                            fnp.Name = gex.Replace(fnp.FullName, "");
                        }
                        else
                        {
                            var gex = new Regex("\\d+");
                            fnp.Name = gex.Replace(fnp.FullName, "");
                        }
                    }
                    try
                    {
                        using (var sf = new ShapeFile())
                        {
                            var err = sf.Open(fnp.FilePath);
                            if (!string.IsNullOrEmpty(err))
                            {
                                fnp.CanRead = false;
                            }
                            else
                            {
                                fnp.CanRead = true;
                                fnp.DataCount = sf.GetRecordCount();
                            }
                        }

                        using (var reader = new ShapefileDataReader(fnp.FilePath, GeometryFactory.Default))
                        {
                            if (!fnp.CanRead)
                            {
                                reader.Read();
                                fnp.DataCount = reader.RecordCount;
                                fnp.CanRead = true;
                            }
                            if (reader.ShapeHeader.ShapeType > ShapeGeometryType.MultiPoint)
                            {
                                fnp.HasZM = true;
                                fnp.CanRead = false;
                            }
                        }
                    }
                    catch
                    {
                        fnp.CanRead = false;
                    }
                    namePairList.Add(fnp);
                }
                else
                {
                    if (extentList == null)
                    {
                        continue;
                    }
                    extentList.Add(fnp);
                }
            }
            foreach (var item in namePairList)
            {
                otherTypeFile.RemoveAll(t => Path.GetFileNameWithoutExtension(t).Equals(item.FullName));
            }
            if (extentList != null)
            {
                otherTypeFile.ForEach(t => extentList.Add(new FileCondition()
                {
                    FilePath = t,
                    FullName = Path.GetFileNameWithoutExtension(t),
                    ExtFileName = Path.GetFileName(t)
                }));
            }
            return namePairList;
        }


        #endregion
    }
}
