/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using System.IO;
using YuLinTu.Data;
using YuLinTu.Library.Repository;
using NetTopologySuite.IO;
using NetTopologySuite.Features;
using YuLinTu.Library.WorkStation;
namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出行政区域图斑
    /// </summary>
    [Serializable]
    public class ExportZoneToShape : ExportShapeBase
    {
        #region Fields

        private Zone currentZone;

        #endregion

        #region Property

        /// <summary>
        /// 地域集合
        /// </summary>
        public List<Zone> ZoneList { get; set; }

        /// <summary>
        /// 是14位编码/否16位编码
        /// </summary>
        public bool IsStandCode { get; set; }

        /// <summary>
        /// 地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set { currentZone = value; }
        }

        /// <summary>
        /// 导出表头语言
        /// </summary>
        public eLanguage Lang { get; set; }

        /// <summary>
        /// 地域描述
        /// </summary>
        public string ZoneDesc { get; set; }

        #endregion

        #region Ctor

        public ExportZoneToShape()
        {
            Lang = eLanguage.CN;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 创建Feature集合
        /// </summary>
        public List<IFeature> CreateFeatureList(List<Zone> zones)
        {
            List<IFeature> list = new List<IFeature>();
            if (zones == null || zones.Count == 0)
            {
                return list;
            }
            toolProgress.InitializationPercent(zones.Count, 99, 1);
            foreach (var item in zones)
            {
                toolProgress.DynamicProgress(ZoneDesc);
                AttributesTable attributes = CreateAttributesTable<Zone>(item);
                YuLinTu.Spatial.Geometry geometry = item.Shape as YuLinTu.Spatial.Geometry;
                if (geometry == null)
                {
                    continue;
                }
                SetReference(geometry);
                Feature feature = new Feature(geometry.Instance, attributes);
                list.Add(feature);
            }
            return list;
        }

        /// <summary>
        /// 创建Feature集合
        /// </summary>
        public override List<IFeature> CreateFeatureList()
        {
            List<IFeature> list = new List<IFeature>();
            if (ZoneList == null || ZoneList.Count == 0)
            {
                this.ReportError("未获取到地域数据!");
                return list;
            }
            list = CreateFeatureList(ZoneList);
            if (list.Count == 0)
            {
                this.ReportError("地域中不包含有图斑的地域数据!");
            }
            return list;
        }

        /// <summary>
        /// 获取配置
        /// </summary>  
        public override object GetExportSetting(object exportSetting = null)
        {            
            return null;
        }


        /// <summary>
        /// 创建表头
        /// </summary>
        public override DbaseFileHeader CreateHeader(IFeature feature = null, int count = 0)
        {
            DbaseFileHeader header = new DbaseFileHeader(Encoding.UTF8);//Encoding.GetEncoding(936));
            if (Lang == eLanguage.CN)
            {
                header.AddColumn("名称", 'C', 50, 0);
                header.AddColumn("编码", 'C', 50, 0);
                header.AddColumn("全名称", 'C', 150, 0);
                header.AddColumn("全编码", 'C', 50, 0);
                header.AddColumn("上级名称", 'C', 150, 0);
                header.AddColumn("上级编码", 'C', 50, 0);
                header.AddColumn("要素代码", 'C', 20, 0);
                header.AddColumn("控制面积", 'F', 15, 6);
                header.AddColumn("计算面积", 'F', 15, 6);
                header.AddColumn("备注", 'C', 255, 0);
            }
            else
            {
                header.AddColumn("FullName", 'C', 150, 0);
                header.AddColumn("FullCode", 'C', 50, 0);
                header.AddColumn("UpName", 'C', 150, 0);
                header.AddColumn("UpCode", 'C', 50, 0);
                header.AddColumn("Featrue", 'C', 20, 0);
                header.AddColumn("ConArea", 'F', 15, 6);
                header.AddColumn("ComArea", 'F', 15, 6);
                header.AddColumn("Comment", 'C', 255, 0);
            }
            return header;
        }

        ///<summary>
        ///创建属性表
        ///</summary>
        ///<returns></returns>
        public override AttributesTable CreateAttributesTable<T>(T en)
        {
            Zone zone = en as Zone;
            if (zone == null)
                return null;
            string zoneCode = zone.FullCode;
            if (zone.Level == eZoneLevel.Group && !IsStandCode && zoneCode.Length == 14)
            {
                zoneCode = zoneCode.Substring(0, 12) + "00" + zoneCode.Substring(12, 2);
            }
            AttributesTable attributes = new AttributesTable();
            if (Lang == eLanguage.CN)
            {
                attributes.AddAttribute("名称", zone.Name);
                attributes.AddAttribute("编码", zone.Code);
                attributes.AddAttribute("全名称", zone.FullName);
                attributes.AddAttribute("全编码", zoneCode);
                attributes.AddAttribute("上级名称", zone.UpLevelName);
                attributes.AddAttribute("上级编码", zone.UpLevelCode);
                attributes.AddAttribute("要素代码", "10000");
                attributes.AddAttribute("控制面积", 0);
                attributes.AddAttribute("计算面积", 0);
                attributes.AddAttribute("备注", zone.Comment);
            }
            else
            {
                attributes.AddAttribute("FullName", zone.FullName);
                attributes.AddAttribute("FullCode", zoneCode);
                attributes.AddAttribute("UpName", zone.UpLevelName);
                attributes.AddAttribute("UpCode", zone.UpLevelCode);
                attributes.AddAttribute("Featrue", "10000");
                attributes.AddAttribute("ConArea", 0);
                attributes.AddAttribute("ComArea", 0);
                attributes.AddAttribute("Comment", zone.Comment);
            }
            return attributes;
        }

        #endregion
    }
}
