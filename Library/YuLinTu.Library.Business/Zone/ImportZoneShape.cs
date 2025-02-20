/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using System.IO;
using System.Collections;
using YuLinTu.Library.Office;
using System.Text.RegularExpressions;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using System.Diagnostics;
using NetTopologySuite.IO;
using NetTopologySuite.Geometries;
using YuLinTu.Spatial;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入地域图斑类
    /// </summary>
    [Serializable]
    public class ImportZoneShape : Task
    {
        #region Fields

        private ToolProgress toolProgress;//进度工具
        private SortedList zones;//地域集合
        private SortedList zoneList;//地域列表
        private int dataCount;//数据总数
        private int updateCount;//更新数

        #endregion

        #region Propertys

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DataInstance { get; set; }

        /// <summary>
        /// 工作站
        /// </summary>
        public IZoneWorkStation Station { get; set; }

        /// <summary>
        /// 是否清空数据
        /// </summary>
        public bool IsClear { get; set; }

        /// <summary>
        /// 是否合并图形要素
        /// </summary>
        public bool IsShapeCombine { get; set; }

        #endregion

        #region Ctor

        public ImportZoneShape()
        {
            zoneList = new SortedList();
            zones = new SortedList();
            toolProgress = new ToolProgress();
        }

        #endregion

        #region Methods - ReadInformation

        /// <summary>
        /// 读取地域信息
        /// </summary>
        public void ReadZoneInformation(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            try
            {
                ShapefileDataReader dataReader = new ShapefileDataReader(fileName, GeometryFactory.Default);
                YuLinTuDataCommentCollection dataCollection = YuLinTuDataComment.DeserializeXml();
                YuLinTuDataComment dataComment = dataCollection.Find(dc => dc.Name == "行政区域");
                if (dataComment == null)
                {
                    return;
                }
                this.ReportProgress(5, "读取配置文件");
                dataComment.LayerName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                InitalizeMappingData data = new InitalizeMappingData();
                data.FileName = fileName;
                data.DataCollection = new YuLinTuDataCommentCollection() { dataComment };
                data.DataReader = dataReader;
                bool canContinue = data.StartCreate();
                if (!canContinue)
                {
                    this.ReportError("配置字段失败!");
                }
                zoneList = dataComment.CurrentObject as SortedList;
                if (zoneList == null || zoneList.Count == 0)
                {
                    this.ReportError("从文件中获取数据失败!");
                }
                else
                {
                    dataCount = zoneList.Count;
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "ReadZoneInformation", ex.Message + ex.StackTrace);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region Methods - Initalzie

        /// <summary>
        /// 初始化地域列表
        /// </summary>
        public bool InitalizeZoneList()
        {
            zones = new SortedList();
            string fullcode = string.Empty;
            string name = string.Empty;
            for (int i = 0; i < zoneList.Count; i++)
            {
                fullcode = zoneList.GetKey(i).ToString();
                if (string.IsNullOrEmpty(fullcode.Trim()))
                {
                    this.ReportError(string.Format("第{0}行数据地域编码为空!", i));
                    continue;
                }               
                zones.Add(fullcode, zoneList.GetByIndex(i));
            }
            zoneList = null;
            return true;
        }

        #endregion

        #region Methods - Import

        /// <summary>
        /// 导入图斑数据
        /// </summary>
        public void InportZoneShape()
        {
            string name = typeof(Zone).GetAttribute<DataTableAttribute>().TableName;
            var sr = ReferenceHelper.GetDbReference<Zone>(DataInstance, name, "Shape");
            toolProgress.InitializationPercent(zones.Count, 90);
            DataInstance.BeginTransaction();
            try
            {
                List<Zone> baseZoneList = Station.Get();
                double step = 90 / (double)zones.Count;
                for (int i = 0; i < zones.Count; i++)
                {
                    //if(i==114)
                    //{

                    //}
                    string fullcode = zones.GetKey(i).ToString();
                   
                    Zone zone = baseZoneList.Find(t => t.FullCode == fullcode);
                    if (zone == null && fullcode.Length == 16)
                    {
                        fullcode = fullcode.Substring(0, 12) + fullcode.Substring(14, 2);
                        zone = baseZoneList.Find(t => t.FullCode == fullcode);
                    }
                    if (zone == null)
                    {
                        continue;
                    }
                    YuLinTu.Spatial.Geometry g = zones.GetByIndex(i) as YuLinTu.Spatial.Geometry;
                   //行政地域不加限制，地块需要此限制
                    //if (g.IsValid()==false)
                    //{
                    //    this.ReportWarn(zone.FullName+ "空间数据无效!请检查是否自交或该图形少于3个节点等等...!");
                    //    continue;
                    //}
                    if(g.GeometryType!=eGeometryType.Polygon && g.GeometryType!=eGeometryType.MultiPolygon)
                    {
                        this.ReportWarn(zone.FullName + "空间数据无效，该空间数据可能是点或者是线");
                        continue;
                    }
                    var geo = YuLinTu.Spatial.Geometry.FromBytes(g.AsBinary(), 0);
                    if (geo != null && sr != null)
                    {
                        geo.Instance.SRID = sr.WKID;
                        geo = YuLinTu.Spatial.Geometry.FromInstance(geo.Instance);
                    }
                    //if (!geo.IsValid() || geo.IsSelfIntersects())
                    //{
                    //    this.ReportError(string.Format("矢量文件中编码为{0}的图形存在问题，不能导入!", zone.FullCode));
                    //    continue;
                    //}
                    bool result = UpdateZone(zone, Station, geo);//, sr);
                    if (result)
                    {
                        this.ReportProgress(10 + (int)(step * (i + 1)), "导入" + zone.Name + "图斑");
                    }
                }
                DataInstance.CommitTransaction();
                this.ReportInfomation(string.Format("当前文件共有{0}条数据,成功导入{1}条图斑记录", zones.Count, updateCount));
                int ErrorCount = zones.Count - updateCount;
                if(ErrorCount>0)
                    this.ReportWarn(string.Format("{0}条图斑记录未匹配成功", ErrorCount));
                zones = null;
                baseZoneList = null;
            }
            catch (Exception ex)
            {
                DataInstance.RollbackTransaction();
                this.ReportError("导入图斑数据时发生错误！");
                YuLinTu.Library.Log.Log.WriteException(this, "InportZoneShape", ex.Message + ex.StackTrace);
            }
            GC.Collect();
        }

        /// <summary>
        /// 更新地域信息
        /// </summary>
        private bool UpdateZone(Zone zone, IZoneWorkStation station, YuLinTu.Spatial.Geometry geo)//, SpatialReference sr)
        {
            try
            {
                //geo.Instance.SRID = sr.WKID;
                //geo = geo.Project(sr);
                //if (!geo.SpatialReference.IsValid())
                //{
                //    geo.SpatialReference = sr;
                //}
                if (zone.Shape == null || IsShapeCombine && zone.Shape != null || zone.Shape.IsValid() == false)
                {
                    zone.Shape = geo;
                }
                else if(zone.Shape !=null)
                {
                    zone.Shape = zone.Shape == null ? geo : zone.Shape.Union(geo);
                }
                try
                {
                    if (station.Update(zone) < 1)
                    {
                        string errorInfo = string.Format("{0} 在写入数据库时出错", zone.FullName);
                        this.ReportError(errorInfo);
                        return false;
                    }
                    else
                    {
                        updateCount++;
                    }
                }
                catch (Exception ex)
                {
                    this.ReportError(zone.FullName + "图斑信息更新失败!");
                    YuLinTu.Library.Log.Log.WriteException(this, "UpdateZone", ex.Message + ex.StackTrace);
                    return false;
                }
            }
            catch (Exception ex)
            {
                this.ReportError(zone.FullName + "图斑数据处理出现错误:" + ex.Message);
                YuLinTu.Library.Log.Log.WriteException(this, "UpdateZone", ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }

        #endregion
    }
}
