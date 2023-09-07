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
using YuLinTu.Library.Repository;
using NetTopologySuite.IO;
using NetTopologySuite.Features;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    public class ExportLandDotGeoToShape : ExportShapeBase
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
        /// 有空间数据的界址点集合
        /// </summary>
        public List<BuildLandBoundaryAddressDot> ListGeoLandDot { get; set; }

        /// <summary>
        /// 是14位编码/否16位编码//默认为14位不改动
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

        /// <summary>
        /// 数据字典
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        /// <summary>
        /// 地块编码ID 字典
        /// </summary>
        public Dictionary<Guid, string> LandIdDictionary { get; set; }

        #endregion

        #region Ctor

        public ExportLandDotGeoToShape()
            : base()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// 创建Feature集合
        /// </summary>
        public List<IFeature> CreateFeatureList(List<BuildLandBoundaryAddressDot> geoLandDots)
        {
            List<IFeature> list = new List<IFeature>();
            if (geoLandDots == null || geoLandDots.Count == 0)
            {
                return list;
            }
            toolProgress.InitializationPercent(geoLandDots.Count, 99, 1);
            foreach (var item in geoLandDots)
            {
                toolProgress.DynamicProgress(ZoneDesc);
                if (item.LandNumber != null && item.LandNumber.Length != 19)
                {
                    if (LandIdDictionary.ContainsKey(item.LandID))
                        item.LandNumber = LandIdDictionary[item.LandID];
                }
                toolProgress.DynamicProgress(ZoneDesc);
                AttributesTable attributes = CreateAttributesTable<BuildLandBoundaryAddressDot>(item);
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
            if (ListGeoLandDot == null || ListGeoLandDot.Count == 0)
            {
                this.ReportError("未获取到有空间信息的地块界址点数据!");
                return list;
            }
            list = CreateFeatureList(ListGeoLandDot);
            if (list.Count == 0)
            {
                this.ReportInfomation("地域中不包含有图斑的地块界址点数据!");
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
                header.AddColumn("ID", 'C', 150, 0);
                header.AddColumn("标识码", 'C', 150, 0);
                header.AddColumn("统编号", 'C', 150, 0);
                header.AddColumn("界址点号", 'C', 150, 0);
                header.AddColumn("界标类型", 'C', 150, 0);
                header.AddColumn("界址点类型", 'C', 150, 0);
                header.AddColumn("使用权ID", 'C', 150, 0);
                //header.AddColumn("创建者", 'C', 150, 0);
                //header.AddColumn("创建时间", 'C', 150, 0);
                //header.AddColumn("最后修改者", 'C', 150, 0);
                //header.AddColumn("最后改时间", 'C', 150, 0);
                header.AddColumn("描述信息", 'C', 250, 0);
                header.AddColumn("地域代码", 'C', 150, 0);
                header.AddColumn("权利类型", 'C', 150, 0);
                header.AddColumn("地块编码", 'C', 150, 0);
                header.AddColumn("关键节点", 'L', 5, 0);
            }

            return header;
        }

        ///<summary>
        ///创建属性表
        ///</summary>
        ///<returns></returns>
        public override AttributesTable CreateAttributesTable<T>(T en)
        {
            BuildLandBoundaryAddressDot geolanddot = en as BuildLandBoundaryAddressDot;
            if (geolanddot == null)
                return null;
            AttributesTable attributes = new AttributesTable();
            if (Lang == eLanguage.CN)
            {
                attributes.AddAttribute("ID", geolanddot.ID);
                attributes.AddAttribute("标识码", geolanddot.DotCode);
                attributes.AddAttribute("统编号", geolanddot.UniteDotNumber);
                attributes.AddAttribute("界址点号", geolanddot.DotNumber);
                var dictJBLX = DictList.Find(c => c.Code == geolanddot.LandMarkType && c.GroupCode == DictionaryTypeInfo.JBLX);
                attributes.AddAttribute("界标类型", dictJBLX == null ? "" : dictJBLX.Name);
                var dictJZDLX = DictList.Find(c => c.Code == geolanddot.DotType && c.GroupCode == DictionaryTypeInfo.JZDLX);
                attributes.AddAttribute("界址点类型", dictJZDLX == null ? "" : dictJZDLX.Name);
                attributes.AddAttribute("使用权ID", geolanddot.LandID.ToString());//地块Id
                //attributes.AddAttribute("创建者", geolanddot.Founder);
                //attributes.AddAttribute("创建时间", geolanddot.CreationTime == null ? null : geolanddot.CreationTime.Value.ToString());
                //attributes.AddAttribute("最后修改者", geolanddot.Modifier);
                //attributes.AddAttribute("最后改时间", geolanddot.ModifiedTime == null ? null : geolanddot.ModifiedTime.Value.ToString());
                attributes.AddAttribute("描述信息", geolanddot.Description);
                attributes.AddAttribute("地域代码", geolanddot.ZoneCode);
                var dictJZDQLLX = DictList.Find(c => c.Code == geolanddot.LandType && c.GroupCode == DictionaryTypeInfo.TDQSLX);
                attributes.AddAttribute("权利类型", dictJZDQLLX == null ? "" : dictJZDQLLX.Name);
                attributes.AddAttribute("地块编码", geolanddot.LandNumber);
                attributes.AddAttribute("关键节点", geolanddot.IsValid);
            }
            return attributes;
        }

        #endregion



    }
}
