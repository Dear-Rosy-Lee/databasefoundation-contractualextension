/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利
 */

using NetTopologySuite.Features;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    public class ExportLandGeoToShape : ExportShapeBase
    {
        #region Fields

        private Zone currentZone;

        private int bsmindex;

        #endregion Fields

        #region Property

        /// <summary>
        /// 地域集合
        /// </summary>
        public List<Zone> ZoneList { get; set; }

        /// <summary>
        /// 有空间数据的地块集合
        /// </summary>
        public List<ContractLand> ListGeoLand { get; set; }

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
        /// 当前地域下所有承包方
        /// </summary>
        public List<VirtualPerson> ListVp
        {
            get;
            set;
        }

        /// <summary>
        /// 导出表头语言
        /// </summary>
        public eLanguage Lang { get; set; }

        public int Exportway { get; set; }

        /// <summary>
        /// 地域描述
        /// </summary>
        public string ZoneDesc { get; set; }

        /// <summary>
        /// 导出shape图斑设置实体
        /// </summary>
        public ExportContractLandShapeDefine exportContractLandShapeDefine = ExportContractLandShapeDefine.GetIntence();

        /// <summary>
        /// 数据字典
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        #endregion Property

        #region Ctor

        public ExportLandGeoToShape(ExportContractLandShapeDefine exportSet)
            : base(exportSet)
        {
            bsmindex = 100000;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// 创建Feature集合
        /// </summary>
        public List<IFeature> CreateFeatureList(List<ContractLand> geoLands)
        {
            List<IFeature> list = new List<IFeature>();
            if (geoLands == null || geoLands.Count == 0)
            {
                return list;
            }
            toolProgress.InitializationPercent(geoLands.Count, 99, 1);
            foreach (var item in geoLands)
            {
                try
                {
                    toolProgress.DynamicProgress(ZoneDesc);
                    AttributesTable attributes = null;

                    switch (Exportway)
                    {
                        case 0:
                            attributes = CreateAttributesTableStand(item);
                            break;
                        case 1:
                            attributes = CreateAttributesTable<ContractLand>(item);
                            break;
                        case 2:
                            attributes = CreateAttributesSimple(item);
                            break;
                        default:
                            attributes = CreateAttributesTableStand(item);
                            break;
                    }
                    YuLinTu.Spatial.Geometry geometry = item.Shape as YuLinTu.Spatial.Geometry;
                    if (geometry == null || geometry.Instance == null)
                    {
                        continue;
                    }
                    //if (geometry.IsValid() == false)
                    //{
                    //    this.ReportInfomation(string.Format("地块{0}数据图斑无效，已忽略，请检查!", item.LandNumber));
                    //    continue;
                    //}
                    if (geometry.GeometryType.Equals(Spatial.eGeometryType.Unknown) || geometry.GeometryType.Equals(Spatial.eGeometryType.GeometryCollection))
                    {
                        this.ReportInfomation(string.Format("地块{0}数据图斑不是面状数据，已忽略，请检查!", item.LandNumber));
                        continue;
                    }
                    SetReference(geometry);
                    Feature feature = new Feature(geometry.Instance, attributes);
                    list.Add(feature);
                }
                catch
                {
                    this.ReportInfomation(string.Format("地块{0}数据有图斑异常，已忽略，请检查!", item.LandNumber));
                }
            }
            return list;
        }

        /// <summary>
        /// 创建Feature集合
        /// </summary>
        public override List<IFeature> CreateFeatureList()
        {
            List<IFeature> list = new List<IFeature>();
            if (ListGeoLand == null || ListGeoLand.Count == 0)
            {
                this.ReportError("未获取到有空间信息的地块数据!");
                return list;
            }
            list = CreateFeatureList(ListGeoLand);
            if (list.Count == 0)
            {
                this.ReportInfomation("地域中不包含有图斑的地块数据!");
            }
            return list;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        public override object GetExportSetting(object exportSetting = null)
        {
            return exportContractLandShapeDefine as object;
        }

        /// <summary>
        /// 创建表头
        /// </summary>
        public override DbaseFileHeader CreateHeader(IFeature feature = null, int count = 0)
        {
            DbaseFileHeader header = new DbaseFileHeader();
            switch (Exportway)
            {
                case 0:
                    header = CreateStandHeader(feature);
                    break;
                case 1:
                    header = CreateDefaultHeader(feature);
                    break;
                case 2:
                    header = CreateSampleHeader(feature);
                    break;
                default:
                    header = CreateStandHeader(feature);
                    break;
            }
            return header;
        }

        private DbaseFileHeader CreateSampleHeader(IFeature feature = null, int count = 0)
        {
            DbaseFileHeader header = new DbaseFileHeader(Encoding.UTF8);//Encoding.GetEncoding(936));  
            header.AddColumn(DKFiled.CDKBM, 'C', 19, 0);
            header.AddColumn(DKFiled.CDKMC, 'C', 50, 0);
            header.AddColumn("QQDKBM", 'C', 19, 0);
            header.AddColumn("CBFBM", 'C', 18, 0);
            header.AddColumn("DKLB", 'C', 4, 0);
            return header;
        }

        /// <summary>
        /// 创建表头
        /// </summary>
        private DbaseFileHeader CreateStandHeader(IFeature feature = null, int count = 0)
        {
            DbaseFileHeader header = new DbaseFileHeader(Encoding.UTF8);//Encoding.GetEncoding(936));
            header.AddColumn(DKFiled.CBSM, 'N', 10, 0);
            header.AddColumn(DKFiled.CYSDM, 'C', 6, 0);
            header.AddColumn(DKFiled.CDKBM, 'C', 19, 0);
            header.AddColumn(DKFiled.CDKMC, 'C', 50, 0);
            header.AddColumn(DKFiled.CSYQXZ, 'C', 2, 0);
            header.AddColumn(DKFiled.CDKLB, 'C', 2, 0);
            header.AddColumn(DKFiled.CDLDJ, 'C', 2, 0);
            header.AddColumn(DKFiled.CTDLYLX, 'C', 3, 0);
            header.AddColumn(DKFiled.CSFJBNT, 'C', 1, 0);
            header.AddColumn(DKFiled.CSCMJ, 'F', 15, 2);
            header.AddColumn(DKFiled.CSCMJM, 'F', 15, 2);
            header.AddColumn(DKFiled.CTDYT, 'C', 1, 0);
            header.AddColumn(DKFiled.CDKDZ, 'C', 50, 0);
            header.AddColumn(DKFiled.CDKXZ, 'C', 50, 0);
            header.AddColumn(DKFiled.CDKNZ, 'C', 50, 0);
            header.AddColumn(DKFiled.CDKBZ, 'C', 50, 0);
            header.AddColumn(DKFiled.CZJRXM, 'C', 100, 0);
            header.AddColumn(DKFiled.CDKBZXX, 'C', 254, 0);
            header.AddColumn(DKFiled.CKJZB, 'C', 254, 0);

            return header;
        }

        /// <summary>
        /// 创建表头
        /// </summary>
        private DbaseFileHeader CreateDefaultHeader(IFeature feature = null, int count = 0)
        {
            DbaseFileHeader header = new DbaseFileHeader(Encoding.UTF8);//Encoding.GetEncoding(936));
            if (exportContractLandShapeDefine == null)
                return header;
            if (Lang == eLanguage.CN)
            {
                if (exportContractLandShapeDefine.NameIndex)
                    header.AddColumn("承包方名称", 'C', 150, 0);
                header.AddColumn("承包方户号", 'C', 20, 0);
                if (exportContractLandShapeDefine.VPNumberIndex) header.AddColumn("证件号码", 'C', 150, 0);
                if (exportContractLandShapeDefine.VPCommentIndex) header.AddColumn("户主备注", 'C', 250, 0);
                if (exportContractLandShapeDefine.LandNameIndex) header.AddColumn("地块名称", 'C', 150, 0);
                if (exportContractLandShapeDefine.CadastralNumberIndex) header.AddColumn("地块编码", 'C', 150, 0);
                if (exportContractLandShapeDefine.SurveyNumberIndex) header.AddColumn("调查编码", 'C', 150, 0);
                header.AddColumn("地域名称", 'C', 150, 0);
                header.AddColumn("地域编码", 'C', 150, 0);
                if (exportContractLandShapeDefine.ImageNumberIndex) header.AddColumn("图幅编号", 'C', 150, 0);
                if (exportContractLandShapeDefine.TableAreaIndex) header.AddColumn("二轮面积", 'F', 15, 6);
                if (exportContractLandShapeDefine.ActualAreaIndex) header.AddColumn("实测面积", 'F', 15, 6);
                if (exportContractLandShapeDefine.EastIndex) header.AddColumn("四至东", 'C', 150, 0);
                if (exportContractLandShapeDefine.SourthIndex) header.AddColumn("四至南", 'C', 150, 0);
                if (exportContractLandShapeDefine.WestIndex) header.AddColumn("四至西", 'C', 150, 0);
                if (exportContractLandShapeDefine.NorthIndex) header.AddColumn("四至北", 'C', 150, 0);
                if (exportContractLandShapeDefine.LandPurposeIndex) header.AddColumn("土地用途", 'C', 150, 0);
                if (exportContractLandShapeDefine.LandLevelIndex) header.AddColumn("地力等级", 'C', 150, 0);
                if (exportContractLandShapeDefine.LandTypeIndex) header.AddColumn("利用类型", 'C', 150, 0);
                if (exportContractLandShapeDefine.IsFarmerLandIndex) header.AddColumn("基本农田", 'C', 150, 0);
                if (exportContractLandShapeDefine.ReferPersonIndex) header.AddColumn("指界人", 'C', 150, 0);
                if (exportContractLandShapeDefine.LandTypeNameIndex) header.AddColumn("地类名称", 'C', 150, 0);
                if (exportContractLandShapeDefine.IsFlyLandIndex) header.AddColumn("是否飞地", 'C', 150, 0);
                if (exportContractLandShapeDefine.VPTelephoneIndex) header.AddColumn("电话号码", 'C', 150, 0);
                if (exportContractLandShapeDefine.ElevationIndex) header.AddColumn("海拔高度", 'C', 150, 0);
                if (exportContractLandShapeDefine.ArableTypeIndex) header.AddColumn("地块类别", 'C', 150, 0);
                if (exportContractLandShapeDefine.AwareAreaIndex) header.AddColumn("确权面积", 'F', 15, 6);
                if (exportContractLandShapeDefine.MotorizeAreaIndex) header.AddColumn("机动地面积", 'F', 15, 6);
                if (exportContractLandShapeDefine.ConstructModeIndex) header.AddColumn("承包方式", 'C', 150, 0);
                if (exportContractLandShapeDefine.PlotNumberIndex) header.AddColumn("畦数", 'C', 150, 0);
                if (exportContractLandShapeDefine.PlatTypeIndex) header.AddColumn("种植类型", 'C', 150, 0);
                if (exportContractLandShapeDefine.ManagementTypeIndex) header.AddColumn("经营方式", 'C', 150, 0);
                if (exportContractLandShapeDefine.LandPlantIndex) header.AddColumn("耕保类型", 'C', 150, 0);
                if (exportContractLandShapeDefine.SourceNameIndex) header.AddColumn("原户主姓名", 'C', 150, 0);
                if (exportContractLandShapeDefine.LandLocationIndex) header.AddColumn("座落方位", 'C', 150, 0);
                if (exportContractLandShapeDefine.IsTransterIndex) header.AddColumn("是否流转", 'C', 150, 0);
                if (exportContractLandShapeDefine.TransterModeIndex) header.AddColumn("流转方式", 'C', 150, 0);
                if (exportContractLandShapeDefine.TransterTermIndex) header.AddColumn("流转期限", 'C', 150, 0);
                if (exportContractLandShapeDefine.TransterAreaIndex) header.AddColumn("流转面积", 'F', 15, 6);
                if (exportContractLandShapeDefine.LandSurveyPersonIndex) header.AddColumn("调查员", 'C', 150, 0);
                if (exportContractLandShapeDefine.LandSurveyDateIndex) header.AddColumn("调查日期", 'C', 150, 0);
                if (exportContractLandShapeDefine.LandSurveyChronicleIndex) header.AddColumn("调查记事", 'C', 150, 0);
                if (exportContractLandShapeDefine.LandCheckPersonIndex) header.AddColumn("审核人", 'C', 150, 0);
                if (exportContractLandShapeDefine.LandCheckDateIndex) header.AddColumn("审核日期", 'C', 150, 0);
                if (exportContractLandShapeDefine.LandCheckOpinionIndex) header.AddColumn("审核意见", 'C', 150, 0);
                if (exportContractLandShapeDefine.CommentIndex) header.AddColumn("地块备注", 'C', 250, 0);
                if (exportContractLandShapeDefine.QQDKBM) header.AddColumn("原地块编码", 'C', 250, 0);
            }
            return header;
        }

        ///<summary>
        ///创建属性表
        ///</summary>
        ///<returns></returns>
        public override AttributesTable CreateAttributesTable<T>(T en)
        {
            if (exportContractLandShapeDefine == null) return null;
            ContractLand geoland = en as ContractLand;
            if (geoland == null)
                return null;
            AttributesTable attributes = new AttributesTable();

            if (Lang == eLanguage.CN)
            {
                VirtualPerson vp = null;
                if (ListVp != null)
                    vp = ListVp.Find(v => v.ID == geoland.OwnerId);
                if (exportContractLandShapeDefine.NameIndex) attributes.AddAttribute("承包方名称", geoland.OwnerName);

                if (exportContractLandShapeDefine.VPNumberIndex)
                {
                    if (vp == null)
                    {
                        attributes.AddAttribute("证件号码", "");
                    }
                    else
                    {
                        attributes.AddAttribute("证件号码", vp.Number);
                    }
                }
                if (exportContractLandShapeDefine.VPCommentIndex)
                {
                    if (vp == null)
                    {
                        attributes.AddAttribute("户主备注", "");
                    }
                    else
                    {
                        attributes.AddAttribute("户主备注", vp.Comment);
                    }
                }

                if (exportContractLandShapeDefine.LandNameIndex) attributes.AddAttribute("地块名称", geoland.Name);
                if (exportContractLandShapeDefine.CadastralNumberIndex)
                {
                    int getlandnumcount = exportContractLandShapeDefine.LandNumberGetCount;
                    int length = geoland.LandNumber == null ? 0 : geoland.LandNumber.Length;
                    if ((geoland.LandNumber != "" || geoland.LandNumber != null) && length > getlandnumcount)
                    {
                        string landendnum = geoland.LandNumber.Substring(getlandnumcount, length - getlandnumcount);
                        attributes.AddAttribute("地块编码", getlandnumcount == 0 ? geoland.LandNumber : landendnum);
                    }
                    else
                    {
                        attributes.AddAttribute("地块编码", "");
                    }
                }
                if (exportContractLandShapeDefine.SurveyNumberIndex) attributes.AddAttribute("调查编码", geoland.SurveyNumber);
                attributes.AddAttribute("地域名称", geoland.SenderName.IsNullOrEmpty() ? geoland.ZoneName : geoland.SenderName);
                attributes.AddAttribute("地域编码", geoland.SenderCode.IsNullOrEmpty() ? geoland.ZoneCode : geoland.SenderCode);
                if (exportContractLandShapeDefine.ImageNumberIndex) attributes.AddAttribute("图幅编号", geoland.LandExpand != null ? geoland.LandExpand.ImageNumber : "");
                if (exportContractLandShapeDefine.TableAreaIndex) attributes.AddAttribute("二轮面积", geoland.TableArea != null ? geoland.TableArea.Value.ToString("0.00") : "0");
                if (exportContractLandShapeDefine.ActualAreaIndex) attributes.AddAttribute("实测面积", geoland.ActualArea.ToString("0.00"));

                if (exportContractLandShapeDefine.EastIndex) attributes.AddAttribute("四至东", geoland.NeighborEast);
                if (exportContractLandShapeDefine.SourthIndex) attributes.AddAttribute("四至南", geoland.NeighborSouth);
                if (exportContractLandShapeDefine.WestIndex) attributes.AddAttribute("四至西", geoland.NeighborWest);
                if (exportContractLandShapeDefine.NorthIndex) attributes.AddAttribute("四至北", geoland.NeighborNorth);
                if (exportContractLandShapeDefine.LandPurposeIndex)
                {
                    var dictTDYT = DictList.Find(c => c.Code == geoland.Purpose && c.GroupCode == DictionaryTypeInfo.TDYT);
                    attributes.AddAttribute("土地用途", dictTDYT == null ? "" : dictTDYT.Name);
                }
                if (exportContractLandShapeDefine.LandLevelIndex)
                {
                    var dictDLDJ = DictList.Find(c => c.Code == geoland.LandLevel && c.GroupCode == DictionaryTypeInfo.DLDJ);
                    attributes.AddAttribute("地力等级", dictDLDJ == null ? "" : dictDLDJ.Name);
                }
                if (exportContractLandShapeDefine.LandTypeIndex)
                {
                    var dictTDLYLX = DictList.Find(c => c.Code == geoland.LandCode && c.GroupCode == DictionaryTypeInfo.TDLYLX);
                    attributes.AddAttribute("利用类型", dictTDLYLX == null ? "" : dictTDLYLX.Name);
                }
                if (exportContractLandShapeDefine.IsFarmerLandIndex) attributes.AddAttribute("基本农田", geoland.IsFarmerLand != null ? (geoland.IsFarmerLand == true ? "是" : "否") : "");
                if (exportContractLandShapeDefine.ReferPersonIndex) attributes.AddAttribute("指界人", geoland.LandExpand != null ? geoland.LandExpand.ReferPerson : "");
                if (exportContractLandShapeDefine.LandTypeNameIndex)
                {
                    attributes.AddAttribute("地类名称", geoland.LandName);
                }
                if (exportContractLandShapeDefine.IsFlyLandIndex) attributes.AddAttribute("是否飞地", geoland.IsFlyLand == true ? "是" : "否");
                if (exportContractLandShapeDefine.VPTelephoneIndex)
                {
                    if (vp == null)
                    {
                        attributes.AddAttribute("电话号码", "");
                    }
                    else
                    {
                        attributes.AddAttribute("电话号码", vp.Telephone);
                    }
                }

                if (exportContractLandShapeDefine.ElevationIndex)
                {
                    if (geoland.LandExpand == null || geoland.LandExpand.Elevation == null)
                    {
                        attributes.AddAttribute("海拔高度", "");
                    }
                    else
                    {
                        attributes.AddAttribute("海拔高度", geoland.LandExpand.Elevation != null ? geoland.LandExpand.Elevation.Value.ToString() : null);
                    }
                }
                if (exportContractLandShapeDefine.ArableTypeIndex)
                {
                    var dictDKLB = DictList.Find(c => c.Code == geoland.LandCategory && c.GroupCode == DictionaryTypeInfo.DKLB);
                    attributes.AddAttribute("地块类别", dictDKLB == null ? "" : dictDKLB.Name);
                }
                if (exportContractLandShapeDefine.AwareAreaIndex) attributes.AddAttribute("确权面积", geoland.AwareArea.ToString("0.00"));
                if (exportContractLandShapeDefine.MotorizeAreaIndex) attributes.AddAttribute("机动地面积", (geoland.MotorizeLandArea != null ? geoland.MotorizeLandArea.Value.ToString("0.00") : "0"));
                if (exportContractLandShapeDefine.ConstructModeIndex)
                {
                    var dictCBFS = DictList.Find(c => c.Code == geoland.ConstructMode && c.GroupCode == DictionaryTypeInfo.CBJYQQDFS);
                    attributes.AddAttribute("承包方式", dictCBFS == null ? "" : dictCBFS.Name);
                }
                if (exportContractLandShapeDefine.PlotNumberIndex) attributes.AddAttribute("畦数", geoland.PlotNumber);
                if (exportContractLandShapeDefine.PlatTypeIndex)
                {
                    var dictZZLX = DictList.Find(c => c.Code == geoland.PlatType && c.GroupCode == DictionaryTypeInfo.ZZLX);

                    attributes.AddAttribute("种植类型", dictZZLX == null ? "" : dictZZLX.Name);
                }
                if (exportContractLandShapeDefine.ManagementTypeIndex)
                {
                    var dictJYFS = DictList.Find(c => c.Code == geoland.ManagementType && c.GroupCode == DictionaryTypeInfo.JYFS);

                    attributes.AddAttribute("经营方式", dictJYFS == null ? "" : dictJYFS.Name);
                }
                if (exportContractLandShapeDefine.LandPlantIndex)
                {
                    var dictGBLX = DictList.Find(c => c.Code == geoland.PlantType && c.GroupCode == DictionaryTypeInfo.GBZL);

                    attributes.AddAttribute("耕保类型", dictGBLX == null ? "" : dictGBLX.Name);
                }
                if (exportContractLandShapeDefine.SourceNameIndex) attributes.AddAttribute("原户主姓名", geoland.FormerPerson);
                if (exportContractLandShapeDefine.LandLocationIndex) attributes.AddAttribute("座落方位", geoland.ZoneName);
                if (exportContractLandShapeDefine.IsTransterIndex) attributes.AddAttribute("是否流转", geoland.IsTransfer == true ? "是" : "否");
                if (exportContractLandShapeDefine.TransterModeIndex)
                {
                    var dictLZFS = DictList.Find(c => c.Code == geoland.TransferType && c.GroupCode == DictionaryTypeInfo.LZLX);
                    attributes.AddAttribute("流转方式", dictLZFS == null ? "" : dictLZFS.Name);
                }
                if (exportContractLandShapeDefine.TransterTermIndex) attributes.AddAttribute("流转期限", geoland.TransferTime);
                if (exportContractLandShapeDefine.TransterAreaIndex) attributes.AddAttribute("流转面积", geoland.PertainToArea);
                if (exportContractLandShapeDefine.LandSurveyPersonIndex) attributes.AddAttribute("调查员", geoland.LandExpand != null ? geoland.LandExpand.SurveyPerson : "");
                if (exportContractLandShapeDefine.LandSurveyDateIndex) attributes.AddAttribute("调查日期", geoland.LandExpand != null ? geoland.LandExpand.SurveyDate : null);
                if (exportContractLandShapeDefine.LandSurveyChronicleIndex) attributes.AddAttribute("调查记事", geoland.LandExpand != null ? geoland.LandExpand.SurveyChronicle : "");
                if (exportContractLandShapeDefine.LandCheckPersonIndex) attributes.AddAttribute("审核人", geoland.LandExpand != null ? geoland.LandExpand.CheckPerson : "");
                if (exportContractLandShapeDefine.LandCheckDateIndex) attributes.AddAttribute("审核日期", geoland.LandExpand != null ? geoland.LandExpand.CheckDate : null);
                if (exportContractLandShapeDefine.LandCheckOpinionIndex) attributes.AddAttribute("审核意见", geoland.LandExpand != null ? geoland.LandExpand.CheckOpinion : "");
                if (exportContractLandShapeDefine.CommentIndex) attributes.AddAttribute("地块备注", geoland.Comment);
                if (exportContractLandShapeDefine.QQDKBM) attributes.AddAttribute("原地块编码", geoland.OldLandNumber);
                if (vp != null)
                {
                    attributes.AddAttribute("承包方户号", vp.FamilyNumber);
                }
                else
                {
                    attributes.AddAttribute("承包方户号", "");
                }
            }
            return attributes;
        }

        ///<summary>
        ///创建属性表
        ///</summary> 
        public AttributesTable CreateAttributesTableStand(ContractLand en)
        {
            AttributesTable attributes = new AttributesTable();
            bsmindex++;

            attributes.AddAttribute(DKFiled.CBSM, bsmindex);
            attributes.AddAttribute(DKFiled.CYSDM, "211011");
            attributes.AddAttribute(DKFiled.CDKBM, en.LandNumber);
            attributes.AddAttribute(DKFiled.CDKMC, en.Name);
            attributes.AddAttribute(DKFiled.CSYQXZ, en.OwnRightType);
            attributes.AddAttribute(DKFiled.CDKLB, en.LandCategory);
            attributes.AddAttribute(DKFiled.CDLDJ, en.LandLevel);
            attributes.AddAttribute(DKFiled.CTDLYLX, en.LandCode);
            attributes.AddAttribute(DKFiled.CSFJBNT, en.IsFarmerLand == null ? "" : (en.IsFarmerLand.Value ? "1" : "2"));
            attributes.AddAttribute(DKFiled.CSCMJ, ToolMath.RoundNumericFormat(en.Shape.Area(), 2));
            attributes.AddAttribute(DKFiled.CSCMJM, en.ActualArea);
            attributes.AddAttribute(DKFiled.CTDYT, en.Purpose);
            attributes.AddAttribute(DKFiled.CDKDZ, en.NeighborEast);
            attributes.AddAttribute(DKFiled.CDKXZ, en.NeighborWest);
            attributes.AddAttribute(DKFiled.CDKNZ, en.NeighborSouth);
            attributes.AddAttribute(DKFiled.CDKBZ, en.NeighborNorth);

            if (en.LandCategory == "10" && string.IsNullOrEmpty(en.LandExpand.ReferPerson))
                attributes.AddAttribute(DKFiled.CZJRXM, en.OwnerName);
            else
                attributes.AddAttribute(DKFiled.CZJRXM, en.LandExpand.ReferPerson);
            attributes.AddAttribute(DKFiled.CDKBZXX, en.Comment);
            attributes.AddAttribute(DKFiled.CKJZB, " ");
            return attributes;
        }

        ///<summary>
        ///创建属性表
        ///</summary> 
        public AttributesTable CreateAttributesSimple(ContractLand en)
        {
            AttributesTable attributes = new AttributesTable();
            bsmindex++;

            attributes.AddAttribute(DKFiled.CDKBM, en.LandNumber);
            attributes.AddAttribute(DKFiled.CDKMC, en.Name);
            attributes.AddAttribute("QQDKBM", en.OldLandNumber);
            attributes.AddAttribute("DKLB", en.LandCategory);
            var cbfbm = "";
            if (ListVp != null)
            {
                var vp = ListVp.Find(v => v.ID == en.OwnerId);
                cbfbm = vp == null ? "" : vp.ZoneCode.PadRight(14, '0') + vp.FamilyNumber.PadLeft(4, '0');
            }
            attributes.AddAttribute("CBFBM", cbfbm);
            return attributes;
        }
        #endregion
    }

    public class DKFiled
    {
        /// <summary>
        /// 表名
        /// </summary>
        public const string TableName = "DK";

        /// <summary>
        /// 表名
        /// </summary>
        public const string TableNameCN = "地块";

        /// <summary>
        /// 标识码(M)
        /// </summary>
        public const string CBSM = "BSM";

        /// <summary>
        /// 要素代码(M)(eFeatureType)
        /// </summary>
        public const string CYSDM = "YSDM";

        /// <summary>
        /// 地块代码
        /// </summary>
        public const string CDKBM = "DKBM";

        /// <summary>
        /// 地块名称
        /// </summary>
        public const string CDKMC = "DKMC";

        /// <summary>
        /// 地块类别
        /// </summary>
        public const string CDKLB = "DKLB";

        /// <summary>
        /// 所有权性质
        /// </summary>
        public const string CSYQXZ = "SYQXZ";

        /// <summary>
        /// 土地利用类型
        /// </summary>
        public const string CTDLYLX = "TDLYLX";

        /// <summary>
        /// 地力等级
        /// </summary>
        public const string CDLDJ = "DLDJ";

        /// <summary>
        /// 土地用途
        /// </summary>
        public const string CTDYT = "TDYT";

        /// <summary>
        /// 是否基本农田
        /// </summary>
        public const string CSFJBNT = "SFJBNT";

        /// <summary>
        /// 实测面积
        /// </summary>
        public const string CSCMJ = "SCMJ";

        /// <summary>
        /// 实测面积亩
        /// </summary>
        public const string CSCMJM = "SCMJM";

        /// <summary>
        /// 东至
        /// </summary>
        public const string CDKDZ = "DKDZ";
        /// <summary>
        /// 西至
        /// </summary>
        public const string CDKXZ = "DKXZ";

        /// <summary>
        /// 南至
        /// </summary>
        public const string CDKNZ = "DKNZ";

        /// <summary>
        /// 北至
        /// </summary>
        public const string CDKBZ = "DKBZ";

        /// <summary>
        /// 备注信息
        /// </summary>
        public const string CDKBZXX = "DKBZXX";

        /// <summary>
        /// 指界人姓名
        /// </summary>
        public const string CZJRXM = "ZJRXM";

        /// <summary>
        /// 空间坐标
        /// </summary>
        public const string CKJZB = "KJZB";

        /// <summary>
        /// 空间坐标
        /// </summary>
        public const string CShape = "Shape";
    }
}
