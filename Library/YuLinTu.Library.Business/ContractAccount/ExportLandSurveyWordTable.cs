/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu;
using YuLinTu.Library.Entity;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出地块调查表
    /// </summary>
    [Serializable]
    public class ExportLandSurveyWordTable : AgricultureWordBook
    {
        #region Fields

        private List<Dictionary> dictJBLX;
        private List<Dictionary> dictJZXLB;
        private List<Dictionary> dictJZXWZ;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 是否模版
        /// </summary>
        public bool IsTemplate { get; set; }

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

        /// <summary>
        /// 有效界址点集合
        /// </summary>
        public List<BuildLandBoundaryAddressDot> ListLandValidDot { get; set; }

        /// <summary>
        /// 所有界址点集合
        /// </summary>
        public List<BuildLandBoundaryAddressDot> ListLandDot { get; set; }

        /// <summary>
        /// 界址线集合
        /// </summary>
        public List<BuildLandBoundaryAddressCoil> ListLandCoil { get; set; }

        #endregion Properties

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportLandSurveyWordTable()
        {
            base.TemplateName = "承包地块调查表";
        }

        #endregion Ctor

        #region Methods

        #region Override

        /// <summary>
        /// 填写数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override bool OnSetParamValue(object data)
        {
            if (data == null)
            {
                return false;
            }
            if (DictList != null && DictList.Count > 0)
            {
                dictJBLX = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.JBLX);
                dictJZXLB = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.JZXLB);
                dictJZXWZ = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.JZXWZ);
            }
            else
            {
                dictJBLX = new List<Dictionary>();
                dictJZXLB = new List<Dictionary>();
                dictJZXWZ = new List<Dictionary>();
            }
            ContractLand land = data as ContractLand;
            try
            {
                string code = Tissue != null ? InitalzieTissueCode(Tissue.ZoneCode) : InitalzieTissueCode(land.ZoneCode);
                string tissueCode = (Tissue != null && Tissue.Code.Length == 14) ? Tissue.Code : code;
                WriteTitleInformation(land, tissueCode);
                if (ListLandCoil != null && ListLandCoil.Count == 0)
                {
                    var orderBy = ListLandCoil.OrderBy(le => le.OrderID);
                    List<BuildLandBoundaryAddressCoil> lines = new List<BuildLandBoundaryAddressCoil>();
                    foreach (var line in orderBy)
                    {
                        lines.Add(line);
                    }
                    ListLandCoil = lines.Clone() as List<BuildLandBoundaryAddressCoil>;
                    lines = null;
                }
                if (ListLandDot != null && ListLandDot.Count > 0)
                {
                    ListLandDot.Sort();
                }
                if (ListLandValidDot != null && ListLandValidDot.Count > 0)
                {
                    ListLandValidDot.Sort();
                    ListLandValidDot.Add(ListLandValidDot.FirstOrDefault());
                }
                if ((ListLandValidDot == null || ListLandValidDot.Count == 0) && ListLandCoil.Count != 0 && ListLandDot.Count != 0)
                {
                    WriteLandDotInformation(ListLandCoil, ListLandDot);
                }
                else if (ListLandValidDot.Count >= 0)
                {
                    WriteLandDotInformation(ListLandValidDot);
                    WriteLandCoilInformation(ListLandCoil, ListLandDot, ListLandValidDot);
                }
            }
            catch (SystemException ex)
            {
                YuLinTu.Library.Log.Log.WriteException(this, "OnSetParamValue(导出地块调查表失败!)", ex.Message + ex.StackTrace);
                return false;
            }
            finally
            {
                ListLandCoil = null;
                ListLandDot = null;
                ListLandValidDot = null;
                Disponse();
            }
            return true;
        }

        /// <summary>
        /// 书写有效界址点信息
        /// </summary>
        /// <param name="dotCollection">有效界址点集合</param>
        private void WriteLandDotInformation(List<BuildLandBoundaryAddressDot> validDotCollection)
        {
            if (validDotCollection == null || validDotCollection.Count == 0)
                return;
            for (int i = 0; i < validDotCollection.Count; i++)
            {
                WriteLandDotInformation(validDotCollection[i], i + 1);
            }
            //WriteLandDotInformation(validDotCollection[0], validDotCollection.Count);
        }

        /// <summary>
        /// 书写界址线信息
        /// </summary>
        /// <param name="coilCollection">界址线集合</param>
        /// <param name="dotCollection">界址点集合</param>
        /// <param name="validDotCollection">有效界址点集合</param>
        private void WriteLandCoilInformation(List<BuildLandBoundaryAddressCoil> coilCollection, List<BuildLandBoundaryAddressDot> dotCollection,
            List<BuildLandBoundaryAddressDot> validDotCollection)
        {
            if (dotCollection == null || dotCollection.Count == 0 ||
                coilCollection == null || coilCollection.Count == 0 ||
            validDotCollection == null || validDotCollection.Count == 0)
            {
                return;
            }
            string startNumber = string.Empty;
            string endNumber = string.Empty;
            BuildLandBoundaryAddressCoil coil = null;

            for (int i = 0; i < validDotCollection.Count - 2; i++)
            {
                startNumber = ToolString.GetAllNumberWithInString(validDotCollection[i].DotNumber);
                endNumber = ToolString.GetAllNumberWithInString(validDotCollection[i + 1].DotNumber);
                int startIndex = 0;
                int endIndex = 0;
                Int32.TryParse(startNumber, out startIndex);
                Int32.TryParse(endNumber, out endIndex);
                if (startIndex + 1 == endIndex)
                {
                    coil = coilCollection.Find(c => c.StartPointID == validDotCollection[i].ID && c.EndPointID == validDotCollection[i + 1].ID);
                }
                else
                {
                    coil = coilCollection.Find(c => c.StartPointID == validDotCollection[i].ID);
                }
                if (coil == null)
                    continue;
                InitializeCoilDescription(coil, coilCollection, dotCollection, validDotCollection, startIndex, endIndex, i);
                WriteLandLineInformation(coil, i + 1);
            }

            startNumber = ToolString.GetAllNumberWithInString(validDotCollection[validDotCollection.Count - 2].DotNumber);
            endNumber = ToolString.GetAllNumberWithInString(validDotCollection[validDotCollection.Count - 1].DotNumber);
            int startIndexLast = 0;
            int endIndexLast = 0;
            Int32.TryParse(startNumber, out startIndexLast);
            Int32.TryParse(endNumber, out endIndexLast);
            if (startIndexLast == ListLandCoil.Count && endIndexLast == 1)
            {
                coil = coilCollection.Find(c => c.StartPointID == validDotCollection[validDotCollection.Count - 2].ID &&
                    c.EndPointID == validDotCollection[validDotCollection.Count - 1].ID);
            }
            else
            {
                coil = coilCollection.Find(c => c.StartPointID == validDotCollection[validDotCollection.Count - 2].ID);
            }
            if (coil == null)
                return;
            InitializeCoilDescription(coil, coilCollection, dotCollection, validDotCollection, startIndexLast, endIndexLast, validDotCollection.Count - 2);
            WriteLandLineInformation(coil, validDotCollection.Count - 2 + 1);
        }

        /// <summary>
        /// 初始化界址线说明
        /// </summary>
        /// <param name="coil">待写入界址线</param>
        /// <param name="listCoil">所有界址线</param>
        /// <param name="listDot">所有界址点</param>
        /// <param name="listValidDot">所有有效界址点</param>
        /// <param name="startIndex">起始索引</param>
        /// <param name="endIndex">终止索引</param>
        /// <param name="j">遍历索引号</param>
        private void InitializeCoilDescription(BuildLandBoundaryAddressCoil coil, List<BuildLandBoundaryAddressCoil> listCoil, List<BuildLandBoundaryAddressDot> listDot,
            List<BuildLandBoundaryAddressDot> listValidDot, int startIndex, int endIndex, int j)
        {
            if (!SettingDefine.SurveyLimitFillWay)
            {
                return;
            }
            if (!string.IsNullOrEmpty(coil.Description))
            {
                return;
            }
            double length = 0.0;
            if (j < listValidDot.Count - 2)
            {
                if (startIndex + 1 == endIndex)
                {
                    length = coil.CoilLength;
                }
                else
                {
                    int st = listCoil.FindIndex(le => le.StartPointID == listValidDot[j].ID);
                    int et = listCoil.FindIndex(le => le.StartPointID == listValidDot[j + 1].ID);
                    for (int i = st; i < et; i++)
                    {
                        length += listCoil[i].CoilLength;
                    }
                }
            }
            else
            {
                if (startIndex == listCoil.Count && endIndex == 1)
                {
                    length = coil.CoilLength;
                }
                else
                {
                    BuildLandBoundaryAddressCoil tempCoil = listCoil.Find(c => c.StartPointID == listDot[listDot.Count - 2].ID && c.EndPointID == listDot[listDot.Count - 1].ID);
                    length = tempCoil == null ? 0 : tempCoil.CoilLength;
                    int st = listCoil.FindIndex(le => le.StartPointID == listValidDot[j].ID);
                    int et = listCoil.FindIndex(le => le.StartPointID == listDot[listDot.Count - 1].ID);
                    for (int i = st; i < et; i++)
                    {
                        length += listCoil[i].CoilLength;
                    }
                    st = listCoil.FindIndex(le => le.StartPointID == listDot[0].ID);
                    et = listCoil.FindIndex(le => le.StartPointID == listValidDot[j + 1].ID);
                    for (int i = st; i < et; i++)
                    {
                        length += listCoil[i].CoilLength;
                    }
                }
            }
            coil.Description = (ToolMath.RoundNumericFormat(length, 4)).ToString();
        }

        /// <summary>
        /// 书写界址点线信息
        /// </summary>
        /// <param name="land">地块</param>
        private void WriteLandDotInformation(List<BuildLandBoundaryAddressCoil> lineCollection, List<BuildLandBoundaryAddressDot> dotCollection)
        {
            lineCollection.ForEach(o => o.NeighborPerson = InitalizeFamilyName(o.NeighborPerson));
            lineCollection.ForEach(o => o.NeighborFefer = InitalizeFamilyName(o.NeighborFefer));
            List<Int32> listIndex = new List<int>();
            listIndex.Add(0);
            int startIndex = 0;
            while (startIndex == 0 || startIndex < lineCollection.Count)
            {
                startIndex = InitalizeRingIndex(lineCollection, dotCollection, startIndex);
                if (!listIndex.Contains(startIndex))
                {
                    listIndex.Add(startIndex);
                }
            }
            for (int index = 0; index < listIndex.Count - 1; index++)
            {
                int st = listIndex[index];
                int et = listIndex[index + 1];
                List<BuildLandBoundaryAddressDot> dots = new List<BuildLandBoundaryAddressDot>();
                for (int i = st; i < et; i++)
                {
                    BuildLandBoundaryAddressCoil line = lineCollection[i];
                    if (SettingDefine.SurveyLimitFillWay && string.IsNullOrEmpty(line.Description))
                    {
                        //line.Description = showDescription ? description : ToolMath.SetNumbericFormat(line.CoilLength.ToString(), 2);
                        line.Description = ToolMath.SetNumbericFormat(line.CoilLength.ToString(), 2);
                    }
                    BuildLandBoundaryAddressDot startDot = dotCollection.Find(dot => dot.ID == line.StartPointID);
                    BuildLandBoundaryAddressDot endDot = dotCollection.Find(dot => dot.ID == line.EndPointID);
                    if (startDot != null)
                    {
                        if (!dots.Contains(startDot))
                        {
                            WriteLandDotInformation(startDot, i + index + 1);
                            dots.Add(startDot);
                        }
                        WriteLandDotInformation(endDot, i + index + 2);
                    }
                    if (endDot != null)
                    {
                        if (!dots.Contains(endDot))
                        {
                            dots.Add(endDot);
                        }
                    }
                    WriteLandLineInformation(line, i + index + 1);
                }
                dots = null;
            }
            lineCollection = null;
            dotCollection = null;
            listIndex = null;
            GC.Collect();
        }

        /// <summary>
        /// 初始化环索引
        /// </summary>
        /// <param name="lineCollection"></param>
        /// <param name="dotCollection"></param>
        /// <returns></returns>
        private int InitalizeRingIndex(List<BuildLandBoundaryAddressCoil> lineCollection, List<BuildLandBoundaryAddressDot> dotCollection, int startIndex)
        {
            List<BuildLandBoundaryAddressDot> dots = new List<BuildLandBoundaryAddressDot>();
            int lineIndex = 0;
            for (int index = startIndex; index < lineCollection.Count; index++)
            {
                BuildLandBoundaryAddressCoil line = lineCollection[index];
                BuildLandBoundaryAddressDot startDot = dotCollection.Find(dot => dot.ID == line.StartPointID);
                BuildLandBoundaryAddressDot endDot = dotCollection.Find(dot => dot.ID == line.EndPointID);
                if (!dots.Contains(startDot))
                {
                    dots.Add(startDot);
                }
                if (!dots.Contains(endDot))
                {
                    dots.Add(endDot);
                }
                else
                {
                    lineIndex = startIndex + lineIndex;
                    break;
                }
                lineIndex++;
            }
            dots = null;
            if (lineIndex < startIndex)
            {
                lineIndex = startIndex + lineIndex - 1;
            }
            return lineIndex + 1;
        }

        /// <summary>
        /// 书写表头信息
        /// </summary>
        private void WriteTitleInformation(ContractLand land, string tissueCode)
        {
            List<Dictionary> listDLDJ = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ);
            string landownername = land.OwnerName.InitalizeFamilyName(SystemSet.KeepRepeatFlag);

            string surveyDate = string.Empty;
            string checkDate = string.Empty;
            if (land.LandExpand != null)
            {
                // 图幅编号所在行的行高 = 60*图幅编号的个数除以2的倍数
                string imageNumber = land.LandExpand.ImageNumber;
                if (!imageNumber.IsNullOrEmpty())
                {
                    string[] imageNumbers = imageNumber.Split(new Char[] { '、' });
                    int multiple = (imageNumbers.Length + 1) / 2;
                    if (multiple > 0)
                        SetTableRowHeight(0, 0, 30 * multiple);
                }

                surveyDate = (land.LandExpand.SurveyDate != null && land.LandExpand.SurveyDate.HasValue) ? ToolDateTime.GetLongDateString(land.LandExpand.SurveyDate.Value) : "/";
                checkDate = (land.LandExpand.CheckDate != null && land.LandExpand.CheckDate.HasValue) ? ToolDateTime.GetLongDateString(land.LandExpand.CheckDate.Value) : "/";
            }

            string landnumber = GetLandNumber(land.LandNumber);

            var Landlevelname = listDLDJ.Find(c => c.Code == land.LandLevel);
            string levelString = string.Empty;
            if (!string.IsNullOrEmpty(land.LandLevel))
                levelString = ToolMath.MatchEntiretyNumber(land.LandLevel.ToString()) ? (Landlevelname == null ? "/" : Landlevelname.Name) : "";
            levelString = levelString == "未知" ? "/" : levelString;

            string platting = land.Purpose;
            if (land.Purpose.IsNullOrEmpty() == false)
            {
                object obj = Enum.Parse(typeof(eLandPurposeType), land.Purpose.ToString());
                platting = (obj == null || ToolMath.MatchEntiretyNumber(obj.ToString())) ? eLandPurposeType.Planting.ToString() : ((eLandPurposeType)obj).ToString();
            }
            string isFamrmerLand = (land.IsFarmerLand == null || !land.IsFarmerLand.HasValue) ? "" : (land.IsFarmerLand.Value ? "IsFarmerLand" : "IsFarmerLandNothing");

            for (int i = 0; i < BookMarkCount; i++)
            {
                SetBookmarkValue("SenderCode", i, tissueCode);//发包方编码
                SetBookmarkValue("ContractorName", i, landownername.GetSettingEmptyReplacement());//承包方名称
                if (land.LandExpand == null)
                {
                    SetBookmarkValue("LandImageNumber", i, "/");//图幅号
                    SetBookmarkValue("LandSurveyPerson", i, "/");//地块调查员
                    SetBookmarkValue("LandSurveyDate", i, "/");//地块调查日期
                    SetBookmarkValue("LandSurveyChronicle", i, "/");//地块调查记事
                    SetBookmarkValue("LandCheckPerson", i, "/");//地块审核员
                    SetBookmarkValue("LandCheckDate", i, "/");//地块审核日期
                    SetBookmarkValue("LandCheckOpinion", i, "/");//地块审核意见
                }
                else
                {
                    SetBookmarkValue("LandImageNumber", i, land.LandExpand.ImageNumber.GetSettingEmptyReplacement());//图幅号
                    SetBookmarkValue("LandSurveyPerson", i, land.LandExpand.SurveyPerson);//地块调查员
                    SetBookmarkValue("LandSurveyDate", i, surveyDate);//地块调查日期
                    SetBookmarkValue("LandSurveyChronicle", i, land.LandExpand.SurveyChronicle);//地块调查记事
                    SetBookmarkValue("LandCheckPerson", i, land.LandExpand.CheckPerson);//地块审核员
                    SetBookmarkValue("LandCheckDate", i, checkDate);//地块审核日期
                    SetBookmarkValue("LandCheckOpinion", i, land.LandExpand.CheckOpinion);//地块审核意见
                }

                SetBookmarkValue("LandNumber", i, landnumber.GetSettingEmptyReplacement());//地块编码
                SetBookmarkValue("LandName", i, land.Name.GetSettingEmptyReplacement());//地块名称
                SetBookmarkValue("TableArea", i, land.TableArea.AreaFormat(SystemSet.DecimalPlaces, true));//实测面积

                SetBookmarkValue("ActualArea", i, land.ActualArea.AreaFormat(SystemSet.DecimalPlaces, true));//实测面积
                SetBookmarkValue("AwareArea", i, land.AwareArea.AreaFormat(SystemSet.DecimalPlaces, true));//实测面积

                SetBookmarkValue("East", i, land.NeighborEast.GetSettingEmptyReplacement());//东至
                SetBookmarkValue("South", i, land.NeighborSouth.GetSettingEmptyReplacement());//南至
                SetBookmarkValue("West", i, land.NeighborWest.GetSettingEmptyReplacement());//西至
                SetBookmarkValue("North", i, land.NeighborNorth.GetSettingEmptyReplacement());//北至

                SetBookmarkValue("LandLevel", i, levelString);//地力等级

                SetBookmarkValue("LandFefer", i, land.LandExpand.ReferPerson);//land.LandExpand.ReferPerson);//地块指界人

                //SetBookmarkValue("LandFefer", InitalizeFamilyName(land.LandExpand.ReferPerson));//地块指界人
                if (land.Purpose.IsNullOrEmpty() == false)
                {
                    SetBookmarkValue(platting, i, "R");
                }

                SetBookmarkValue(isFamrmerLand, i, "R");

                if (!string.IsNullOrEmpty(land.LandName))
                {
                    WriteLandTypeNameInformation(land, i);
                }
                else
                {
                    WriteLandTypeInformation(land, i);
                }
            }
        }

        /// <summary>
        /// 初始化集体经济组织代码
        /// </summary>
        /// <param name="tissueCode"></param>
        /// <returns></returns>
        private string InitalzieTissueCode(string zoneCode)
        {
            string tissueCode = string.Empty;
            switch (zoneCode.Length)
            {
                case Zone.ZONE_TOWN_LENGTH:
                    tissueCode = zoneCode + "00000";
                    break;

                case Zone.ZONE_VILLAGE_LENGTH:
                    tissueCode = zoneCode + "00";
                    break;

                case Zone.ZONE_GROUP_LENGTH:
                    tissueCode = zoneCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH) + zoneCode.Substring(Zone.ZONE_VILLAGE_LENGTH, 2);
                    break;

                default:
                    tissueCode = zoneCode;
                    break;
            }
            return tissueCode;
        }

        /// <summary>
        /// 书写地类信息
        /// </summary>
        private bool WriteLandTypeNameInformation(ContractLand land, int i)
        {
            bool useName = false;
            switch (land.LandName)
            {
                case "水田":
                    SetBookmarkValue("Paddy", i, "R");
                    useName = true;
                    break;

                case "旱地":
                    SetBookmarkValue("Dry", i, "R");
                    useName = true;
                    break;

                case "水浇地":
                    SetBookmarkValue("Irrigated", i, "R");
                    useName = true;
                    break;

                case "果园":
                    SetBookmarkValue("Orchard", i, "R");
                    useName = true;
                    break;

                case "茶园":
                    SetBookmarkValue("TeaGarden", i, "R");
                    useName = true;
                    break;

                default:
                    SetBookmarkValue("OtherLandType", i, "R");
                    break;
            }
            return useName;
        }

        /// <summary>
        /// 书写地类信息
        /// </summary>
        private void WriteLandTypeInformation(ContractLand land, int i)
        {
            switch (land.LandCode)
            {
                case "011":
                    SetBookmarkValue("Paddy", i, "R");
                    break;

                case "012":
                    SetBookmarkValue("Dry", i, "R");
                    break;

                case "013":
                    SetBookmarkValue("Irrigated", i, "R");
                    break;

                case "021":
                    SetBookmarkValue("Orchard", i, "R");
                    break;

                case "022":
                    SetBookmarkValue("TeaGarden", i, "R");
                    break;

                default:
                    SetBookmarkValue("OtherLandType", i, "R");
                    break;
            }
        }

        /// <summary>
        /// 书写界址点信息
        /// </summary>
        /// <param name="dot"></param>
        /// <param name="index"></param>
        private void WriteLandDotInformation(BuildLandBoundaryAddressDot dot, int index)
        {
            Dictionary dotType = dictJBLX.Find(c => c.Code.Equals(dot.LandMarkType));
            string name = dotType == null ? "" : dotType.Name;
            eLandMarkType markType = eLandMarkType.Other;
            var obj = EnumNameAttribute.GetValue(typeof(eLandMarkType), name);
            if (obj != null)
                markType = (eLandMarkType)obj;
            //SetBookmarkValue("LandPointNumber" + index.ToString(), useUnitNumber ? dot.UniteDotNumber : dot.DotNumber);//界址点号
            if (SettingDefine.ExportLandTableUseUnitNumber)
                SetBookmarkValue("LandPointNumber" + index.ToString(), dot.UniteDotNumber);//界址点统编号
            else
                SetBookmarkValue("LandPointNumber" + index.ToString(), dot.DotNumber);//界址点号
            //Enum.TryParse<eLandMarkType>(dot.LandMarkType, out markType);
            switch (markType)
            {
                case eLandMarkType.FixTure:
                    SetBookmarkValue("DotFixTure" + index.ToString(), "√");
                    break;

                case eLandMarkType.Cement:
                    SetBookmarkValue("DotCement" + index.ToString(), "√");
                    break;

                case eLandMarkType.Lime:
                    SetBookmarkValue("DotLime" + index.ToString(), "√");
                    break;

                case eLandMarkType.Shoot:
                    SetBookmarkValue("DotShoot" + index.ToString(), "√");
                    break;

                case eLandMarkType.ChinaFlag:
                    SetBookmarkValue("DotChinaFlag" + index.ToString(), "√");
                    break;

                case eLandMarkType.NoFlag:
                    SetBookmarkValue("DotNoFlag" + index.ToString(), "√");
                    break;

                case eLandMarkType.Piling:
                    SetBookmarkValue("DotDeadman" + index.ToString(), "√");//木桩
                    break;

                case eLandMarkType.BuriedStone:
                    SetBookmarkValue("DotBuriedStone" + index.ToString(), "√");//埋石
                    break;

                case eLandMarkType.Other:
                    SetBookmarkValue("DotOther" + index.ToString(), "√");//其他
                    break;

                default:
                    SetBookmarkValue("DotOther" + index.ToString(), "√");//其他
                    break;
            }
            if (markType != eLandMarkType.Piling && markType != eLandMarkType.BuriedStone)
            {
                SetBookmarkValue("DotOther" + index.ToString(), "√");//其他
            }
        }

        /// <summary>
        /// 书写界址线信息
        /// </summary>
        /// <param name="line"></param>
        /// <param name="index"></param>
        private void WriteLandLineInformation(BuildLandBoundaryAddressCoil line, int index)
        {
            Dictionary coilType = dictJZXLB.Find(c => c.Code.Equals(line.CoilType));
            string name = coilType == null ? "" : coilType.Name;
            eBoundaryLineCategory lineCategory = eBoundaryLineCategory.Baulk;
            var obj = EnumNameAttribute.GetValue(typeof(eBoundaryLineCategory), name);
            if (obj != null)
            {
                lineCategory = (eBoundaryLineCategory)obj;
                switch (lineCategory)
                {
                    case eBoundaryLineCategory.Baulk:
                        if (SettingDefine.IsCheckedRibbing)
                        {
                            SetBookmarkValue("LineBaulk" + index.ToString(), "√");//田垄
                        }
                        else if (SettingDefine.IsCheckedBaulk)
                        {
                            SetBookmarkValue("DotBaulk" + index.ToString(), "√");//田埂
                        }
                        break;

                    case eBoundaryLineCategory.Kennel:
                        SetBookmarkValue("LineKennel" + index.ToString(), "√");//沟渠
                        break;

                    case eBoundaryLineCategory.Road:
                        SetBookmarkValue("LineRoad" + index.ToString(), "√");//道路
                        break;

                    case eBoundaryLineCategory.Linage:
                        SetBookmarkValue("LineLinage" + index.ToString(), "√");//行树
                        break;

                    case eBoundaryLineCategory.Enclosure:
                        SetBookmarkValue("LineEnclosure" + index.ToString(), "√");//围墙
                        break;

                    case eBoundaryLineCategory.Wall:
                        SetBookmarkValue("LineWall" + index.ToString(), "√");//墙壁
                        break;

                    case eBoundaryLineCategory.Raster:
                        SetBookmarkValue("LineRaster" + index.ToString(), "√");//栅栏
                        break;

                    case eBoundaryLineCategory.LinkLine:
                        SetBookmarkValue("LineLinkLine" + index.ToString(), "√");//两点连线
                        break;

                    case eBoundaryLineCategory.Other:
                        SetBookmarkValue("LineOther" + index.ToString(), "√");//其他界线
                        break;

                    default:
                        break;
                }
            }
            eBoundaryLinePosition linePosition = eBoundaryLinePosition.Left;
            Dictionary coilposition = dictJZXWZ.Find(c => c.Code.Equals(line.Position));
            string coilpositionname = coilposition == null ? "" : coilposition.Name;

            var linePositionobj = EnumNameAttribute.GetValue(typeof(eBoundaryLinePosition), coilpositionname);
            if (linePositionobj != null)
            {
                linePosition = (eBoundaryLinePosition)linePositionobj;
                switch (linePosition)
                {
                    case eBoundaryLinePosition.Left:
                        SetBookmarkValue("LineWithin" + index.ToString(), "√");//内
                        break;

                    case eBoundaryLinePosition.Middle:
                        SetBookmarkValue("LineMiddle" + index.ToString(), "√");//中
                        break;

                    case eBoundaryLinePosition.Right:
                        SetBookmarkValue("LineOutline" + index.ToString(), "√");//外
                        break;

                    default:
                        break;
                }
            }

            SetBookmarkValue("LandLineDescription" + index.ToString(), line.Description);//界址线说明
            SetBookmarkValue("LineNeighborPerson" + index.ToString(), InitalizeFamilyName(line.NeighborPerson));//毗邻地块承包方
            SetBookmarkValue("LandLineNeighborFefer" + index.ToString(), line.NeighborFefer);//毗邻地块指界人
        }

        private void Disponse()
        {
            GC.Collect();
        }

        #endregion Override

        #endregion Methods
    }
}