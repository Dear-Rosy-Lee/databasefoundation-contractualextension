using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.CompareResult
{
    /// <summary>
    /// 文件路径
    /// </summary>
    public class FilePathInfo
    {
        #region Property

        /// <summary>
        /// 矢量数据
        /// </summary>
        public string VictorFilePath { get; set; }

        /// <summary>
        /// 数据库全路径
        /// </summary>
        public string DataBasePath { get; set; }

        /// <summary>
        /// 根目录
        /// </summary>
        public string RootPath { get; set; }

        /// <summary>
        /// 选择的成果目录
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 矢量图层
        /// </summary>
        public List<FileCondition> ShapeFileList { get; set; }

        /// <summary>
        /// 原矢量文件
        /// </summary>
        public FileCondition SourceShapeFile { get; set; }

        /// <summary>
        /// 目标矢量文件
        /// </summary>
        public FileCondition TargetShapeFile { get; set; }

        /// <summary>
        /// 对比结果矢量文件
        /// </summary>
        public FileCondition ResultShapeFile { get; set; }

        /// <summary>
        /// 地域编码
        /// </summary>
        public string ZoneCode { get; set; }

        /// <summary>
        /// 年份编码
        /// </summary>
        public string YearCode { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        #endregion
    }
}
