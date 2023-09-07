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
    public class ExportLandCoilGeoToShape : ExportShapeBase
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
        /// 有空间数据的界址线集合
        /// </summary>
        public List<BuildLandBoundaryAddressCoil> ListGeoLandCoil { get; set; }

        /// <summary>
        /// 地块编码ID 字典
        /// </summary>
        public Dictionary<Guid, string> LandIdDictionary { get; set; }

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

        #endregion

        #region Ctor

        public ExportLandCoilGeoToShape()
            : base()
        {

        }



        #endregion

        #region Methods

        /// <summary>
        /// 创建Feature集合
        /// </summary>
        public List<IFeature> CreateFeatureList(List<BuildLandBoundaryAddressCoil> geoLandCoils)
        {
            List<IFeature> list = new List<IFeature>();
            if (geoLandCoils == null || geoLandCoils.Count == 0)
            {
                return list;
            }
            toolProgress.InitializationPercent(geoLandCoils.Count, 99, 1);
            foreach (var item in geoLandCoils)
            {
                toolProgress.DynamicProgress(ZoneDesc);
                if (item.LandNumber != null && item.LandNumber.Length != 19)
                {
                    if (LandIdDictionary.ContainsKey(item.LandID))
                        item.LandNumber = LandIdDictionary[item.LandID];
                }
                AttributesTable attributes = CreateAttributesTable<BuildLandBoundaryAddressCoil>(item);
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
            if (ListGeoLandCoil == null || ListGeoLandCoil.Count == 0)
            {
                this.ReportError("未获取到有空间信息的地块界址线数据!");
                return list;
            }
            list = CreateFeatureList(ListGeoLandCoil);
            if (list.Count == 0)
            {
                this.ReportInfomation("地域中不包含有图斑的地块界址线数据!");
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
                header.AddColumn("标识码", 'C', 150, 0);
                header.AddColumn("界址线长度", 'C', 150, 0);
                header.AddColumn("界线性质", 'C', 150, 0);
                header.AddColumn("界址线类别", 'C', 150, 0);
                header.AddColumn("界址线位置", 'C', 150, 0);
                header.AddColumn("协议书编号", 'C', 150, 0);
                header.AddColumn("界线协议书", 'C', 150, 0);
                header.AddColumn("缘由书编号", 'C', 150, 0);
                header.AddColumn("争议缘由书", 'C', 150, 0);
                header.AddColumn("线顺序号", 'C', 150, 0);
                header.AddColumn("起点", 'C', 150, 0);
                header.AddColumn("终点", 'C', 150, 0);
                header.AddColumn("使用权ID", 'C', 150, 0);
                header.AddColumn("创建者", 'C', 150, 0);
                header.AddColumn("创建时间", 'C', 150, 0);
                header.AddColumn("最后修改者", 'C', 150, 0);
                header.AddColumn("最后改时间", 'C', 150, 0);
                header.AddColumn("说明", 'C', 250, 0);
                header.AddColumn("毗邻指界人", 'C', 150, 0);
                header.AddColumn("毗邻承包方", 'C', 150, 0);
                header.AddColumn("地域代码", 'C', 150, 0);
                header.AddColumn("权利类型", 'C', 150, 0);
                header.AddColumn("备注", 'C', 250, 0);
                header.AddColumn("地块编码", 'C', 150, 0);
                header.AddColumn("起点号", 'C', 150, 0);
                header.AddColumn("终点号", 'C', 150, 0);
                header.AddColumn("起点ID", 'C', 150, 0);
                header.AddColumn("终点ID", 'C', 150, 0);

            }

            return header;
        }

        ///<summary>
        ///创建属性表
        ///</summary>
        ///<returns></returns>
        public override AttributesTable CreateAttributesTable<T>(T en)
        {

            BuildLandBoundaryAddressCoil geolandcoil = en as BuildLandBoundaryAddressCoil;
            if (geolandcoil == null)
                return null;
            AttributesTable attributes = new AttributesTable();
            if (Lang == eLanguage.CN)
            {
                attributes.AddAttribute("标识码", geolandcoil.ID.ToString());
                attributes.AddAttribute("界址线长度", geolandcoil.CoilLength);
                var dictJXXZ = DictList.Find(c => c.Code == geolandcoil.LineType && c.GroupCode == DictionaryTypeInfo.JXXZ);
                attributes.AddAttribute("界线性质", dictJXXZ == null ? "" : dictJXXZ.Name);
                var dictJXLB = DictList.Find(c => c.Code == geolandcoil.CoilType && c.GroupCode == DictionaryTypeInfo.JZXLB);
                attributes.AddAttribute("界址线类别", dictJXLB == null ? "" : dictJXLB.Name);
                var dictJXWZ = DictList.Find(c => c.Code == geolandcoil.Position && c.GroupCode == DictionaryTypeInfo.JZXWZ);
                attributes.AddAttribute("界址线位置", dictJXWZ == null ? "" : dictJXWZ.Name);
                attributes.AddAttribute("协议书编号", geolandcoil.AgreementNumber);
                attributes.AddAttribute("界线协议书", geolandcoil.AgreementBook);
                attributes.AddAttribute("缘由书编号", geolandcoil.ControversyNumber);
                attributes.AddAttribute("争议缘由书", geolandcoil.ControversyBook);
                attributes.AddAttribute("线顺序号", geolandcoil.OrderID);
                attributes.AddAttribute("起点", geolandcoil.StartPointID.ToString());
                attributes.AddAttribute("终点", geolandcoil.EndPointID.ToString());
                attributes.AddAttribute("使用权ID", geolandcoil.LandID.ToString());
                attributes.AddAttribute("创建者", geolandcoil.Founder);
                attributes.AddAttribute("创建时间", geolandcoil.CreationTime == null ? null : geolandcoil.CreationTime.Value.ToString());
                attributes.AddAttribute("最后修改者", geolandcoil.Modifier);
                attributes.AddAttribute("最后改时间", geolandcoil.ModifiedTime == null ? null : geolandcoil.ModifiedTime.Value.ToString());
                attributes.AddAttribute("说明", geolandcoil.Description);
                attributes.AddAttribute("毗邻指界人", geolandcoil.NeighborFefer);
                attributes.AddAttribute("毗邻承包方", geolandcoil.NeighborPerson);
                attributes.AddAttribute("地域代码", geolandcoil.ZoneCode);
                var dictJXQLLX = DictList.Find(c => c.Code == geolandcoil.LandType && c.GroupCode == DictionaryTypeInfo.TDQSLX);
                attributes.AddAttribute("权利类型", dictJXQLLX == null ? "" : dictJXQLLX.Name);
                attributes.AddAttribute("备注", geolandcoil.Comment);
                attributes.AddAttribute("地块编码", geolandcoil.LandNumber);
                attributes.AddAttribute("起点号", geolandcoil.StartNumber);
                attributes.AddAttribute("终点号", geolandcoil.EndNumber);
                attributes.AddAttribute("起点ID", geolandcoil.StartPointID);
                attributes.AddAttribute("终点ID", geolandcoil.EndPointID);
            }
            return attributes;
        }

        #endregion



    }
}
