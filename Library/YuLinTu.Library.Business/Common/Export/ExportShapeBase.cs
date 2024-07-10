/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using System.IO;
using YuLinTu.Data;
using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using YuLinTu.Spatial;
using YuLinTu.Data.Shapefile;
using DotSpatial.Projections;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出图斑基础类
    /// </summary>
    [Serializable]
    public abstract class ExportShapeBase : Task
    {
        #region Fields

        protected DbaseFileHeader dbaseFileHeader;//表头
        protected AttributesTable attributesTable;//表属性       
        protected SpatialReference sr = null;//坐标信息
        protected ToolProgress toolProgress;//进度        

        #endregion

        #region Property

        /// <summary>
        /// Shape文件路径
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件导出配置
        /// </summary>
        public object FileFieldSet { get; set; }

        #endregion

        #region Ctor

        public ExportShapeBase(object exportSet = null)
        {
            FileFieldSet = GetExportSetting(exportSet);
            dbaseFileHeader = CreateHeader(null, 0);
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(OnPostProgress);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 导出为Shape文件
        /// 导出前先设置SetReference
        /// </summary>
        public virtual void ExportToShape()
        {
            if (FileName.IsNullOrBlank())
            {
                throw new ArgumentNullException("fileName");
            }
            this.ReportProgress(1, "开始");
            var builder = new ShapefileConnectionStringBuilder();
            builder.DirectoryName = Path.GetDirectoryName(FileName);
            var elementName = Path.GetFileNameWithoutExtension(FileName);
            List<IFeature> list = CreateFeatureList();
            if (list == null || list.Count == 0)
            {
                return;
            }
            var ds = DataSource.Create<IDbContext>(ShapefileConnectionStringBuilder.ProviderType, builder.ConnectionString);
            var provider = ds.DataSource as IProviderShapefile;
            ClearExistFiles(provider, elementName);
            var writer = provider.CreateShapefileDataWriter(elementName);
            var hedear = CreateHeader(list[0], list.Count);
            writer.Header = dbaseFileHeader == null ? hedear : dbaseFileHeader;
            writer.Header.NumRecords = list.Count;
            writer.Write(list);
            if (sr != null)
            {
                ProjectionInfo info = sr.CreateProjectionInfo();
                if (info == null)
                    return;
                info.SaveAs(Path.ChangeExtension(FileName, "prj"));
            }
            this.ReportInfomation(string.Format("成功导出{0}条数据", list.Count));
        }

        /// <summary>
        /// 设置坐标系
        /// </summary>
        public void SetReference(YuLinTu.Spatial.Geometry geometry)
        {
            if (geometry == null || sr != null)
            {
                return;
            }
            if ((sr == null || !sr.IsValid()) && geometry != null &&
                   geometry.SpatialReference != null &&
                   geometry.SpatialReference.IsValid())
            {
                sr = geometry.SpatialReference;
            }
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
        /// 创建要素集合
        /// </summary>
        public abstract object GetExportSetting(object exportSetting = null);

        /// <summary>
        /// 创建要素集合
        /// </summary>
        public abstract List<IFeature> CreateFeatureList();

        /// <summary>
        /// 创建Shape文件表头
        /// </summary>
        public abstract DbaseFileHeader CreateHeader(IFeature feature = null, int count = 0);

        /// <summary>
        /// 创建属性表
        /// </summary>
        public abstract AttributesTable CreateAttributesTable<T>(T en);

        /// <summary>
        /// 报告进度
        /// </summary>
        public void OnPostProgress(int progress, string info = "")
        {
            this.ReportProgress(progress, info);
        }

        #endregion
    }
}
