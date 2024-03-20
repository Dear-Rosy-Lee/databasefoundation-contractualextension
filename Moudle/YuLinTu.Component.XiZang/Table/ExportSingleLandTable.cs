using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Library.Business;
using GeoAPI;
using YuLinTu;
using GeoAPI.Geometries;
using YuLinTu.Spatial;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 导出地块调查表信息
    /// </summary>
    public class ExportSingleLandTable : ExportExcelBase
    {
        #region Fields

        private int currrentIndex;//下标
        private bool useUnitNumber;//是否使用统编号
        private GetDictionary dic;

        #endregion Fields

        #region Property

        public Zone CurrentZone { get; set; }//当前地域

        /// <summary>
        /// 模版文件路径
        /// </summary>
        public string TempletePath { get; set; }

        public VirtualPerson Contracter { get; set; }//承包方

        /// <summary>
        /// 承包地块
        /// </summary>
        public ContractLand CurrentLand { get; set; }

        public ContractConcord Concord { get; set; }//承包合同

        public CollectivityTissue Tissue { get; set; }

        public List<BuildLandBoundaryAddressCoil> LineList { get; set; }//界址线

        public List<BuildLandBoundaryAddressDot> DotList { get; set; }//界址点

        #endregion Property

        #region Ctor

        public ExportSingleLandTable(string dictoryName)
        {
            useUnitNumber = false;
            dic = new GetDictionary(dictoryName);
            dic.Read();
            currrentIndex = 11;
        }

        #endregion Ctor

        #region Methods

        #region 开始生成Excel之前的一系列操作

        /// <summary>
        /// 从数据库直接导出Excel
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="Metadata.FolderNameDestination"></param>
        public void BeginToZone(Zone zone)
        {
            if (string.IsNullOrEmpty(TempletePath) || !System.IO.File.Exists(TempletePath))
            {
                PostErrorInfo("模板路径不存在!");
                return;
            }
            if (zone == null)
            {
                PostErrorInfo("行政区域为空!");
                return;
            }
            CurrentZone = zone;
            Write();//写数据
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        public override void Read()
        {
        }

        public override void Write()
        {
            try
            {
                Open(TempletePath, 0);
                BeginWrite();
            }
            catch (System.Exception e)
            {
                PostErrorInfo(e.Message.ToString());
                Dispose();
                if (e is TaskStopException)
                    throw e;
            }
        }

        #endregion 开始生成Excel之前的一系列操作

        #region 开始生成Excel

        /// <summary>
        /// 开始写数据
        /// </summary>
        /// <returns></returns>
        private bool BeginWrite()
        {
            try
            {
                var lines = LineList.OrderBy(le => le.OrderID).ToList();
                //var lineCollection = lines.Clone() as List<BuildLandBoundaryAddressCoil>;
                // var dotCollection = DotList;
                InitalizeDotAngleInformation(lines, DotList);
                //lineCollection = null;
                // dotCollection = null;
                WriteTitle();
                Dispose();
            }
            catch (Exception ex)
            {
                return PostErrorInfo("生成Excel时出现错误：" + ex.Message.ToString());
            }
            return true;
        }

        #region 书写表头

        /// <summary>
        /// 书写表头
        /// </summary>
        private void WriteTitle()
        {
            string ContracterName = "";
            if (Contracter != null)
                ContracterName = Contracter.Name;

            string code = Tissue != null ? InitalzieTissueCode(Tissue.ZoneCode) : InitalzieTissueCode(CurrentLand.LocationCode);
            string TissueCode = (Tissue != null && Tissue.Code.Length == 14) ? Tissue.Code : code;
            WriteTitleInformation(CurrentLand, TissueCode, ContracterName);

            InitalizeRangeValue("B4", "C4", CurrentZone.Code);//承包方编码
            InitalizeRangeValue("B6", "C6", CurrentLand.LandNumber);//宗地编码
            InitalizeRangeValue("E6", "E6", CurrentLand.ActualArea.ToString("f2"));//宗地面积
        }

        /// <summary>
        /// 书写表头信息
        /// </summary>
        private void WriteTitleInformation(ContractLand land, string TissueCode, string ContracterName)
        {
            InitalizeRangeValue("C2", "H3", TissueCode);//发包方编码
            InitalizeRangeValue("M3", "Q3", InitalizeFamilyName(land.OwnerName));//承包方名称
            if (ContracterName == null || ContracterName.Trim() == "")
                ContracterName = dic.translante(land.OwnerName);
            InitalizeRangeValue("M2", "Q2", ContracterName);//承包方名称
            string number = "";
            if (Contracter != null)
            {
                int familyNumber = 0;
                Int32.TryParse(Contracter.FamilyNumber, out familyNumber);
                string zoneCode = Contracter.ZoneCode;
                if (Contracter.ZoneCode.Length == Zone.ZONE_GROUP_LENGTH)
                {
                    zoneCode = Contracter.ZoneCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH) + Contracter.ZoneCode.Substring(Zone.ZONE_VILLAGE_LENGTH + 2);
                }
                number = string.Format("{0:D4}", familyNumber);
            }
            InitalizeRangeValue("T2", "U3", number);//承包方编码（缩略码）
            string landNumber = ContractLand.GetLandNumber(land.CadastralNumber);
            if (landNumber.Length > AgricultureSetting.AgricultureLandNumberMedian)
            {
                landNumber = landNumber.Substring(AgricultureSetting.AgricultureLandNumberMedian);
            }
            //landNumber = landNumber.Length > 5 ? landNumber.Substring(landNumber.Length - 5) : landNumber;
            InitalizeRangeValue("C4", "H4", landNumber);//地块编码
            InitalizeRangeValue("L4", "R4", land.Name);//地块名称
            InitalizeRangeValue("U4", "U4", land.TableArea.Value.ToString("f2"));//合同面积
            //string[] neighors = ContractLand.GetLandNeighbor(land.CadastralNumber);
            InitalizeRangeValue("B5", "F5", land.NeighborEast);//东至
            InitalizeRangeValue("I5", "M5", land.NeighborSouth);//南至
            InitalizeRangeValue("P5", "R5", land.NeighborWest);//西至
            InitalizeRangeValue("T5", "U5", land.NeighborNorth);//北至

            var landPurType = GetEnumDesp<eLandPurposeType>(land.Purpose);
            string chinaname = GetRangeToValue("B7", "I7") == null ? "" : GetRangeToValue("B7", "I7").ToString();
            string zangwen = GetRangeToValue("B6", "I6") == null ? "" : GetRangeToValue("B6", "I6").ToString();
            List<string> getT = SetNames(chinaname, zangwen, landPurType);
            InitalizeRangeValue("B7", "I7", getT[0]);//土地用途
            InitalizeRangeValue("B6", "I6", getT[1]);//土地用途
            var landLevel = string.IsNullOrEmpty(land.LandLevel) ? string.Empty : GetEnumDesp<eContractLandLevel>(land.LandLevel);
            InitalizeRangeValue("L6", "N7", landLevel);//地力等级
            chinaname = GetRangeToValue("Q7", "S7") == null ? "" : GetRangeToValue("Q7", "S7").ToString();
            zangwen = GetRangeToValue("Q6", "S6") == null ? "" : GetRangeToValue("Q6", "S6").ToString();
            getT = SetNames(chinaname, zangwen, land.LandName);
            InitalizeRangeValue("Q7", "S7", getT[0]);//土地利用类型
            InitalizeRangeValue("Q6", "S6", getT[1]);//土地利用类型

            string isFamrmerLand = (land.IsFarmerLand == null || !land.IsFarmerLand.HasValue) ? "未知" : (land.IsFarmerLand.Value ? "是" : "否");
            Object obj = EnumNameAttribute.GetDescription(isFamrmerLand);
            chinaname = GetRangeToValue("U7", "U7") == null ? "" : GetRangeToValue("U7", "U7").ToString();
            zangwen = GetRangeToValue("U6", "U6") == null ? "" : GetRangeToValue("U6", "U6").ToString();
            getT = SetNames(chinaname, zangwen, obj == null ? "" : isFamrmerLand);
            InitalizeRangeValue("U7", "U7", getT[0]);//是否基本农田
            InitalizeRangeValue("U6", "U6", getT[1]);//是否基本农田
            if (currrentIndex == 11)
                currrentIndex += 2;
            else
                currrentIndex++;
            InitalizeRangeValue("U11", "U" + currrentIndex, land.LandExpand.ReferPerson);//地块指界人
            string surveyDate = (land.LandExpand.SurveyDate != null && land.LandExpand.SurveyDate.HasValue && land.LandExpand.SurveyDate.Value.Year > 1980) ? ToolDateTime.GetLongDateString(land.LandExpand.SurveyDate.Value) : "    年  月  日";
            string checkDate = (land.LandExpand.CheckDate != null && land.LandExpand.CheckDate.HasValue && land.LandExpand.CheckDate.Value.Year > 1980) ? ToolDateTime.GetLongDateString(land.LandExpand.CheckDate.Value) : "    年  月  日";
            currrentIndex++;
            InitalizeRangeValue("B" + currrentIndex, "U" + currrentIndex, land.LandExpand.SurveyChronicle == null ? "" : land.LandExpand.SurveyChronicle);//调查记事
            currrentIndex += 2;
            InitalizeRangeValue("D" + (currrentIndex - 1), "P" + currrentIndex, land.LandExpand.SurveyPerson == null ? "" : land.LandExpand.SurveyPerson);//调查员
            InitalizeRangeValue("S" + currrentIndex, "U" + currrentIndex, surveyDate);//地块调查日期
            currrentIndex++;
            InitalizeRangeValue("B" + currrentIndex, "U" + currrentIndex, land.LandExpand.CheckOpinion == null ? "" : land.LandExpand.CheckOpinion);//地块审核意见
            currrentIndex += 2;
            InitalizeRangeValue("D" + currrentIndex, "P" + currrentIndex, land.LandExpand.CheckPerson == null ? "" : land.LandExpand.CheckPerson);//地块审核员
            InitalizeRangeValue("S" + currrentIndex, "U" + currrentIndex, checkDate);//地块审核意见
        }

        /// <summary>
        /// 初始化名称
        /// </summary>
        private string InitalizeFamilyName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "";
            }
            string number = ToolString.GetAllNumberWithInString(name);
            if (!string.IsNullOrEmpty(number))
            {
                return name.Replace(number, "");
            }
            int index = name.IndexOf("(");
            if (index > 0)
            {
                return name.Substring(0, index);
            }
            return name;
        }

        /// <summary>
        /// 初始化集体经济组织代码
        /// </summary>
        /// <param name="TissueCode"></param>
        /// <returns></returns>
        private string InitalzieTissueCode(string zoneCode)
        {
            string TissueCode = string.Empty;
            switch (zoneCode.Length)
            {
                case Zone.ZONE_TOWN_LENGTH:
                    TissueCode = zoneCode + "00000";
                    break;

                case Zone.ZONE_VILLAGE_LENGTH:
                    TissueCode = zoneCode + "00";
                    break;

                case Zone.ZONE_GROUP_LENGTH:
                    TissueCode = zoneCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH) + zoneCode.Substring(Zone.ZONE_VILLAGE_LENGTH + 2);
                    break;

                default:
                    TissueCode = zoneCode;
                    break;
            }
            return TissueCode;
        }

        /// <summary>
        /// 勾选选型
        /// </summary>
        private List<string> SetNames(string chinaname, string zangwen, string name)
        {
            List<string> str = new List<string>();
            bool isOther = true;     //是否属于其他
            string c = "";
            string z = "";
            string[] china = chinaname.Split('□');
            string[] zangw = zangwen.Split('∩');
            if (name == "")
            {
                str.Add(chinaname);
                str.Add(zangwen);
                return str;
            }
            for (int i = 0; i < china.Count(); i++)
            {
                if (china[i] == "")
                    continue;
                if (china[i].Contains(name) || name.Contains(china[i].Trim()))
                {
                    c += "√" + china[i];
                    z += "Μ" + zangw[i];
                    isOther = false;           //不属于其他
                }
                else
                {
                    if (i == 0)
                    {
                        c += china[i];
                        z += zangw[i];
                    }
                    else if (i == china.Count() - 1 && isOther)        //属于其他
                    {
                        c += "√" + china[i];
                        z += "Μ" + zangw[i];
                    }
                    else
                    {
                        c += "□" + china[i];
                        z += "∩" + zangw[i];
                    }
                }
            }
            str.Add(c);
            str.Add(z);
            return str;
        }

        #endregion 书写表头

        #region Angle界址点界址线

        /// <summary>
        /// 书写界址点线信息
        /// </summary>
        /// <param name="land">地块</param>
        private void InitalizeDotAngleInformation(List<BuildLandBoundaryAddressCoil> lineCollection, List<BuildLandBoundaryAddressDot> dotCollection)
        {
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
            InitalizeLineAngleCollection(lineCollection, listIndex, dotCollection);
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
            var dots = new List<BuildLandBoundaryAddressDot>();
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
        /// 初始化界址线信息
        /// </summary>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> InitalizeLineAngleCollection(List<BuildLandBoundaryAddressCoil> lines,
            List<int> listIndex, List<BuildLandBoundaryAddressDot> dotCollection)
        {
            var lineCollection = new List<BuildLandBoundaryAddressCoil>();

            for (int index = 0; index < listIndex.Count - 1; index++)
            {
                int st = listIndex[index];
                int et = listIndex[index + 1];
                List<IPoint> sharePoints = InitalizeLandNeighborData(CurrentLand.Shape);
                List<BuildLandBoundaryAddressCoil> lineArray = null;
                if (sharePoints == null || sharePoints.Count == 0)
                {
                    lineArray = InitalizeLineInformationFilter(lines, st, et, dotCollection);
                }
                else
                {
                    lineArray = InitalizeLineInformationFilter(lines, st, et, dotCollection, sharePoints);
                }
                sharePoints = null;
                //InitalizeTemplate(lineArray.Count);
                List<BuildLandBoundaryAddressDot> dots = new List<BuildLandBoundaryAddressDot>();
                if (lineArray.Count > 1)
                    InsertRowCell(currrentIndex + 2, lineArray.Count * 2 - 1);
                bool flag = false;
                for (int i = 0; i < lineArray.Count; i++)
                {
                    if (i == lineArray.Count - 1)
                        flag = true;
                    BuildLandBoundaryAddressCoil line = lineArray[i];
                    BuildLandBoundaryAddressDot startDot = dotCollection.Find(dot => dot.ID == line.StartPointID);
                    BuildLandBoundaryAddressDot endDot = dotCollection.Find(dot => dot.ID == line.EndPointID);
                    //InitalizeLineDescription(startDot, endDot, line);
                    if (!dots.Contains(startDot))
                    {
                        WriteLandDotInformation(startDot, currrentIndex);
                        dots.Add(startDot);
                    }
                    WriteLandDotInformation(endDot, currrentIndex + 2);
                    if (!dots.Contains(endDot))
                    {
                        dots.Add(endDot);
                    }
                    WriteLandLineInformation(line, currrentIndex + 1, flag);
                    currrentIndex += 2;
                }

                foreach (var dot in dotCollection)
                {
                    bool exist = dots.Exists(dt => dt.ID == dot.ID);
                    if (dot.IsValid == exist)
                    {
                        continue;
                    }
                    dot.IsValid = exist;
                    // DataInstance.BuildLandBoundaryAddressDot.Update(dot);
                }
                dots = null;
            }
            return lineCollection;
        }

        private List<IPoint> InitalizeLandNeighborData(object arcLand)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 初始化界址线信息
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> InitalizeLineInformationFilter(List<BuildLandBoundaryAddressCoil> lines, int startIndex, int endIndex, List<BuildLandBoundaryAddressDot> dots)
        {
            if (lines == null || lines.Count == 0)
            {
                return new List<BuildLandBoundaryAddressCoil>();
            }
            List<BuildLandBoundaryAddressCoil> lineCollection = new List<BuildLandBoundaryAddressCoil>();
            if (lines != null && lines.Count <= 4)
            {
                //InitalizeCoilDescription(lines, dots, lines);
                return lines;
            }
            for (int index = startIndex; index < endIndex; index++)
            {
                lineCollection.Add(lines[index]);
            }
            List<BuildLandBoundaryAddressCoil> lineArray = new List<BuildLandBoundaryAddressCoil>();
            for (int i = 0; i < lineCollection.Count; i++)
            {
                BuildLandBoundaryAddressCoil firLine = lineCollection[i];
                BuildLandBoundaryAddressDot sd = dots.Find(dt => dt.ID == firLine.StartPointID);
                for (int j = i + 1; j < lineCollection.Count; j++)
                {
                    BuildLandBoundaryAddressCoil secLine = lineCollection[j];
                    BuildLandBoundaryAddressDot md = dots.Find(dt => dt.ID == secLine.StartPointID);
                    BuildLandBoundaryAddressDot ed = dots.Find(dt => dt.ID == secLine.EndPointID);
                    double firAngle = InitalizeLineAngleDegree(md, sd);
                    double secAngle = InitalizeLineAngleDegree(md, ed);
                    double angle = secAngle - firAngle;
                    if (angle < 0)
                    {
                        angle += 360;
                    }
                    if (angle >= 150 && angle <= 250)
                    {
                        continue;
                    }
                    firLine.EndPointID = secLine.StartPointID;
                    lineArray.Add(firLine);
                    i = j - 1;
                    break;
                }
            }
            for (int i = 0; i < lineArray.Count - 2; i++)
            {
                BuildLandBoundaryAddressCoil pLine = lineArray[i];
                BuildLandBoundaryAddressCoil nLine = lineArray[i + 1];
                if (pLine.EndPointID != nLine.StartPointID)
                {
                    pLine.EndPointID = nLine.StartPointID;
                }
            }
            BuildLandBoundaryAddressCoil endLine = lineArray[lineArray.Count - 1];
            if (endLine.EndPointID != lineArray[0].StartPointID)
            {
                endLine = lines[lines.Count - 1];
                endLine.EndPointID = lineArray[0].StartPointID;
                endLine.StartPointID = lineArray[lineArray.Count - 1].EndPointID;
                lineArray.Add(endLine);
            }
            //InitalizeCoilDescription(lineArray, dots, lines);
            return lineArray;
        }

        /// <summary>
        /// 初始化界址线描述
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="dots"></param>
        private void InitalizeCoilDescription(List<BuildLandBoundaryAddressCoil> lines, List<BuildLandBoundaryAddressDot> dots, List<BuildLandBoundaryAddressCoil> lineArray)
        {
            foreach (var line in lines)
            {
                BuildLandBoundaryAddressDot sd = dots.Find(dt => dt.ID == line.StartPointID);
                BuildLandBoundaryAddressDot ed = dots.Find(dt => dt.ID == line.EndPointID);
                if (sd == null || ed == null)
                {
                    continue;
                }
                string angleString = InitalizeLineAngleString(sd, ed);
                string startNumber = ToolString.GetAllNumberWithInString(sd.DotNumber);
                string endNumber = ToolString.GetAllNumberWithInString(ed.DotNumber);
                int sn = 0;
                int en = 0;
                Int32.TryParse(startNumber, out sn);
                Int32.TryParse(endNumber, out en);
                double lineLength = 0.0;
                if (sn + 1 == en)
                {
                    lineLength = InitalizeLineLength(sd, ed);
                    line.Description = angleString + "\n" + ToolMath.SetNumbericFormat(lineLength.ToString(), 2);
                    continue;
                }
                if (en == 1 && sn == dots.Count)
                {
                    lineLength = InitalizeLineLength(sd, ed);
                    line.Description = angleString + "\n" + ToolMath.SetNumbericFormat(lineLength.ToString(), 2);
                    continue;
                }
                int st = lineArray.FindIndex(le => le.StartPointID == sd.ID);
                int et = lineArray.FindIndex(le => le.StartPointID == ed.ID);
                if (et == 0 && st < lineArray.Count)
                {
                    et = lineArray.Count;
                }
                for (int index = st; index < et; index++)
                {
                    BuildLandBoundaryAddressCoil lineCoil = lineArray[index];
                    lineLength += lineCoil.CoilLength;
                }
                lineLength = Math.Round(lineLength, 2);
                line.Description = angleString + "\n" + ToolMath.SetNumbericFormat(lineLength.ToString(), 2);
            }
        }

        /// <summary>
        /// 初始化界址线角度
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="ed"></param>
        /// <returns></returns>
        private string InitalizeLineAngleString(BuildLandBoundaryAddressDot sd, BuildLandBoundaryAddressDot ed)
        {
            var edx = (ed.Shape.Instance as IPoint).X;
            var edy = (ed.Shape.Instance as IPoint).Y;

            var sdx = (sd.Shape.Instance as IPoint).X;
            var sdy = (sd.Shape.Instance as IPoint).Y;

            double xLength = Math.Abs(edx - sdx);
            double yLength = Math.Abs(edy - sdy);
            if (xLength <= 0.001)
            {
                if (edy >= sdy)
                {
                    return "0°";
                }
                else
                {
                    return "180°";
                }
            }
            if (yLength <= 0.001)
            {
                if (edx >= sdx)
                {
                    return "90°";
                }
                else
                {
                    return "270°";
                }
            }
            double angle = 0.0;
            if (edx > sdx && edy > sdy)
            {
                angle = Math.Tanh(Math.Tan(xLength / yLength));
                angle = Math.Abs(angle);
            }
            if (edx > sdx && edy < sdy)
            {
                angle = Math.Tanh(Math.Tan(yLength / xLength));
                angle = Math.Abs(angle);
                angle += 0.5 * Math.PI;
            }
            if (edx < sdx && edy < sdy)
            {
                angle = Math.Tanh(Math.Tan(xLength / yLength));
                angle = Math.Abs(angle);
                angle += 1.0 * Math.PI;
            }
            if (edx < sdx && edy > sdy)
            {
                angle = Math.Tanh(Math.Tan(yLength / xLength));
                angle = Math.Abs(angle);
                angle += 1.5 * Math.PI;
            }
            angle = Math.Abs(angle);
            angle = angle * 180 / Math.PI;
            int degree = (int)angle;
            int second = (int)((angle - degree) * 60);
            return degree.ToString() + "°" + second.ToString() + "′";
        }

        /// <summary>
        /// 初始化界址线长度
        /// </summary>
        /// <returns></returns>
        private double InitalizeLineLength(BuildLandBoundaryAddressDot sd, BuildLandBoundaryAddressDot ed)
        {
            var edx = (ed.Shape.Instance as IPoint).X;
            var edy = (ed.Shape.Instance as IPoint).Y;

            var sdx = (sd.Shape.Instance as IPoint).X;
            var sdy = (sd.Shape.Instance as IPoint).Y;
            double xLength = Math.Abs(edx - sdx);
            double yLength = Math.Abs(edy - sdy);
            double length = Math.Sqrt(xLength * xLength + yLength * yLength);
            length = Math.Round(length, 2);
            return length;
        }

        /// <summary>
        /// 初始化界址线描述获取角度
        /// </summary>
        ///  /// <summary>
        /// 初始化界址线角度
        /// </summary>
        /// <param name="sd"></param>
        /// <param name="ed"></param>
        /// <returns></returns>
        private int InitalizeLineAngleDegree(BuildLandBoundaryAddressDot sd, BuildLandBoundaryAddressDot ed)
        {
            var edx = (ed.Shape.Instance as IPoint).X;
            var edy = (ed.Shape.Instance as IPoint).Y;

            var sdx = (sd.Shape.Instance as IPoint).X;
            var sdy = (sd.Shape.Instance as IPoint).Y;
            double xLength = Math.Abs(edx - sdx);
            double yLength = Math.Abs(edy - sdy);
            if (xLength <= 0.001)
            {
                if (edy >= sdy)
                {
                    return 90;
                }
                else
                {
                    return 270;
                }
            }
            if (yLength <= 0.001)
            {
                if (edx >= sdx)
                {
                    return 0;
                }
                else
                {
                    return 180;
                }
            }
            double angle = 0.0;
            if (edx > sdx && edy > sdy)
            {
                angle = Math.Tanh(Math.Tan(yLength / xLength));
                angle = Math.Abs(angle);
            }
            if (edx > sdx && edy < sdy)
            {
                angle = Math.Tanh(Math.Tan(xLength / yLength));
                angle = Math.Abs(angle);
                angle += 1.5 * Math.PI;
            }
            if (edx < sdx && edy < sdy)
            {
                angle = Math.Tanh(Math.Tan(yLength / xLength));
                angle = Math.Abs(angle);
                angle += 1.0 * Math.PI;
            }
            if (edx < sdx && edy > sdy)
            {
                angle = Math.Tanh(Math.Tan(xLength / yLength));
                angle = Math.Abs(angle);
                angle += 0.5 * Math.PI;
            }
            angle = Math.Abs(angle);
            angle = angle * 180 / Math.PI;
            int degree = (int)angle;
            return degree;
        }

        /// <summary>
        /// 初始化界址线信息
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> InitalizeLineInformationFilter(List<BuildLandBoundaryAddressCoil> lines, int startIndex, int endIndex, List<BuildLandBoundaryAddressDot> dots, List<IPoint> points)
        {
            if (lines == null || lines.Count == 0)
            {
                return new List<BuildLandBoundaryAddressCoil>();
            }
            List<BuildLandBoundaryAddressCoil> lineCollection = new List<BuildLandBoundaryAddressCoil>();
            if (lines != null && lines.Count <= 4)
            {
                //InitalizeCoilDescription(lines, dots, lines);
                return lines;
            }
            for (int index = startIndex; index < endIndex; index++)
            {
                lineCollection.Add(lines[index]);
            }
            List<BuildLandBoundaryAddressCoil> lineArray = new List<BuildLandBoundaryAddressCoil>();
            for (int i = 0; i < lineCollection.Count; i++)
            {
                BuildLandBoundaryAddressCoil firLine = lineCollection[i];
                BuildLandBoundaryAddressDot sd = dots.Find(dt => dt.ID == firLine.StartPointID);
                for (int j = i + 1; j < lineCollection.Count; j++)
                {
                    BuildLandBoundaryAddressCoil secLine = lineCollection[j];
                    BuildLandBoundaryAddressDot md = dots.Find(dt => dt.ID == secLine.StartPointID);
                    BuildLandBoundaryAddressDot ed = dots.Find(dt => dt.ID == secLine.EndPointID);
                    bool sharePoint = InitalizeSharePoint(md, points);
                    if (!sharePoint)
                    {
                        double firAngle = InitalizeLineAngleDegree(md, sd);
                        double secAngle = InitalizeLineAngleDegree(md, ed);
                        double angle = secAngle - firAngle;
                        if (angle < 0)
                        {
                            angle += 360;
                        }
                        if (angle >= 150 && angle <= 250)
                        {
                            continue;
                        }
                    }
                    firLine.EndPointID = secLine.StartPointID;
                    lineArray.Add(firLine);
                    i = j - 1;
                    break;
                }
            }
            BuildLandBoundaryAddressCoil endLine = lineArray[lineArray.Count - 1];
            if (endLine.EndPointID != lineArray[0].StartPointID)
            {
                endLine = lines[lines.Count - 1];
                endLine.EndPointID = lineArray[0].StartPointID;
                endLine.StartPointID = lineArray[lineArray.Count - 1].EndPointID;
                lineArray.Add(endLine);
            }
            //InitalizeCoilDescription(lineArray, dots, lines);
            return lineArray;
        }

        /// <summary>
        /// 判断是否是共用点
        /// </summary>
        /// <param name="dot"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        private bool InitalizeSharePoint(BuildLandBoundaryAddressDot dot, List<IPoint> points)
        {
            //var sdx = (dot.Shape.Instance as IPoint).X;
            //var sdy = (dot.Shape.Instance as IPoint).Y;
            //IPoint point = new PointClass();
            //point.PutCoords(sdx, sdy);
            //double distance = 0.0;
            //bool onPoint = false;
            //foreach (var pt in points)
            //{
            //    distance = GeometryUtility.Distance(point, pt);
            //    if (distance <= 0.05)
            //    {
            //        onPoint = true;
            //        break;
            //    }
            //}
            //return onPoint;

            return true;
        }

        /// <summary>
        /// 书写界址点信息
        /// </summary>
        /// <param name="dot"></param>
        /// <param name="index"></param>
        private void WriteLandDotInformation(BuildLandBoundaryAddressDot dot, int index)
        {
            string Number = dot.DotNumber;
            if (useUnitNumber)
                Number = dot.UniteDotNumber;

            int index1 = index + 1;
            InitalizeRangeValue("A" + index, "A" + index1, Number);//界址点号
            InitalizeRangeValue("B" + index, "B" + index1, " ");//木桩
            InitalizeRangeValue("C" + index, "C" + index1, " ");//埋石
            InitalizeRangeValue("D" + index, "D" + index1, " ");//埋石
            InitalizeRangeValue("E" + index, "E" + index1, " ");//其他
            InitalizeRangeValue("E" + index, "E" + index1, " ");//其他
            eLandMarkType landmarkType = new eLandMarkType();
            Enum.TryParse<eLandMarkType>(dot.LandMarkType, out landmarkType);
            switch (landmarkType)
            {
                case eLandMarkType.Piling:
                    InitalizeRangeValue("B" + index, "B" + index1, "√");//木桩
                    break;

                case eLandMarkType.BuriedStone:
                    InitalizeRangeValue("C" + index, "C" + index1, "√");//埋石
                    break;

                case eLandMarkType.Other:
                    InitalizeRangeValue("E" + index, "E" + index1, "√");//其他
                    break;

                default:
                    InitalizeRangeValue("E" + index, "E" + index1, "√");//其他
                    break;
            }
            //SetBookmarkValue("LandPointNumber" + index.ToString(), Number);//界址点号
            //SetBookmarkValue("PrePointNumber" + index.ToString(), dot.DotNumber);//界址点号
            //switch (dot.LandmarkType)
            //{
            //    case eLandMarkType.FixTure:
            //        SetBookmarkValue("DotFixTure" + index.ToString(), "√");
            //        break;
            //    case eLandMarkType.Cement:
            //        SetBookmarkValue("DotCement" + index.ToString(), "√");
            //        break;
            //    case eLandMarkType.Lime:
            //        SetBookmarkValue("DotLime" + index.ToString(), "√");
            //        break;
            //    case eLandMarkType.Shoot:
            //        SetBookmarkValue("DotShoot" + index.ToString(), "√");
            //        break;
            //    case eLandMarkType.ChinaFlag:
            //        SetBookmarkValue("DotChinaFlag" + index.ToString(), "√");
            //        break;
            //    case eLandMarkType.NoFlag:
            //        SetBookmarkValue("DotNoFlag" + index.ToString(), "√");
            //        break;
            //    case eLandMarkType.Piling:
            //        SetBookmarkValue("DotDeadman" + index.ToString(), "√");//木桩
            //        break;
            //    case eLandMarkType.BuriedStone:
            //        SetBookmarkValue("DotBuriedStone" + index.ToString(), "√");//埋石
            //        break;
            //    case eLandMarkType.Other:
            //        SetBookmarkValue("DotOther" + index.ToString(), "√");//其他
            //        break;
            //    default:
            //        SetBookmarkValue("DotOther" + index.ToString(), "√");//其他
            //        break;
            //}
            //if (dot.LandmarkType != eLandMarkType.Piling && dot.LandmarkType != eLandMarkType.BuriedStone)
            //{
            //    SetBookmarkValue("DotNothing" + index.ToString(), "√");//其他
            //}
        }

        /// <summary>
        /// 书写界址线信息
        /// </summary>
        /// <param name="line"></param>
        /// <param name="index"></param>
        private void WriteLandLineInformation(BuildLandBoundaryAddressCoil line, int index, bool flag)
        {
            int index1 = flag ? index + 2 : index + 1;
            if (index <= 12)
                index = index - 1;
            InitalizeRangeValue("G" + index, "G" + index1, " ");//沟渠
            InitalizeRangeValue("H" + index, "H" + index1, " ");//道路
            InitalizeRangeValue("I" + index, "I" + index1, " ");//行树
            InitalizeRangeValue("J" + index, "J" + index1, " ");//围墙
            InitalizeRangeValue("K" + index, "K" + index1, " ");//墙壁
            InitalizeRangeValue("L" + index, "L" + index1, " ");//栅栏
            InitalizeRangeValue("M" + index, "M" + index1, " ");//两点连线
            InitalizeRangeValue("N" + index, "N" + index1, " ");//内
            InitalizeRangeValue("O" + index, "O" + index1, " ");//中
            InitalizeRangeValue("P" + index, "P" + index1, " ");//外
            InitalizeRangeValue("G" + index, "G" + index1, " ");//田埂
            InitalizeRangeValue("F" + index, "F" + index1, " ");//田垄

            eBoundaryLineCategory coilType = new eBoundaryLineCategory();
            Enum.TryParse<eBoundaryLineCategory>(line.CoilType, out coilType);

            switch (coilType)
            {
                case eBoundaryLineCategory.Baulk:
                    InitalizeRangeValue("F" + index, "F" + index1, "√");//田垄
                    break;

                case eBoundaryLineCategory.Kennel:
                    InitalizeRangeValue("G" + index, "G" + index1, "√");//沟渠
                    break;

                case eBoundaryLineCategory.Road:
                    InitalizeRangeValue("H" + index, "H" + index1, "√");//道路
                    break;

                case eBoundaryLineCategory.Linage:
                    InitalizeRangeValue("I" + index, "I" + index1, "√");//行树
                    break;

                case eBoundaryLineCategory.Enclosure:
                    InitalizeRangeValue("J" + index, "J" + index1, "√");//围墙
                    break;

                case eBoundaryLineCategory.Wall:
                    InitalizeRangeValue("K" + index, "K" + index1, "√");//墙壁
                    break;

                case eBoundaryLineCategory.Raster:
                    InitalizeRangeValue("L" + index, "L" + index1, "√");//栅栏
                    break;

                case eBoundaryLineCategory.LinkLine:
                    InitalizeRangeValue("M" + index, "M" + index1, "√");//两点连线
                    break;

                default:
                    break;
            }
            eBoundaryLinePosition position = new eBoundaryLinePosition();
            Enum.TryParse<eBoundaryLinePosition>(line.Position, out position);
            switch (position)
            {
                case eBoundaryLinePosition.Left:
                    InitalizeRangeValue("N" + index, "N" + index1, "√");//内
                    break;

                case eBoundaryLinePosition.Middle:
                    InitalizeRangeValue("O" + index, "O" + index1, "√");//中
                    break;

                case eBoundaryLinePosition.Right:
                    InitalizeRangeValue("P" + index, "P" + index1, "√");//外
                    break;

                default:
                    break;
            }
            InitalizeRangeValue("Q" + index, "R" + index1, line.Description);//界址线说明
            InitalizeRangeValue("S" + index, "S" + index1, line.NeighborPerson);//毗邻地块权利人
            InitalizeRangeValue("T" + index, "T" + index1, line.NeighborFefer);//毗邻地块指界人
        }

        #endregion Angle界址点界址线

        #region Helper

        /// <summary>
        /// 初始化地块边界数据
        /// </summary>
        private List<IPoint> InitalizeLandNeighborData(Geometry geometry)
        {
            if (geometry == null || geometry.IsEmpty())
            {
                return new List<IPoint>();
            }
            List<IPoint> points = new List<IPoint>();
            try
            {
                var landGeometry = geometry.Instance as NetTopologySuite.Geometries.Polygon;
                //IGeometry landBuffer =//YuLinTu.Spatial.Geometry GeometryUtility.InitalizeBufferGeometry(land.Shape, 0.1);
                //IQueryFilter filter = YuLinTu.ArcGIS.Common.ConstructQueryFilter.GetQueryFilter(landBuffer, esriSpatialRelEnum.esriSpatialRelIntersects);
                // List<IFeature> features = FeatureUtility.GetFeatureCollection(FeatureClass, filter);
                //IPoint op = null;
                //for (int i = 0; i < landGeometry.PointCount - 1; i++)
                //{
                //    IPoint point = landGeometry.get_Point(i);
                //    double x = point.X;
                //    double y = point.Y;
                //    foreach (IFeature feature in features)
                //    {
                //        if (feature.OID == land.OBJECTID)
                //        {
                //            continue;
                //        }
                //        op = InitalizeNeighborPoint(point, feature, landGeometry);
                //        if (op == null)
                //        {
                //            continue;
                //        }
                //        points.Add(point);
                //        break;
                //    }
                //}
                //filter = null;
                //features = null;
                GC.Collect();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return points;
        }

        /// <summary>
        /// 初始化邻宗地节点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="feature"></param>
        //private IPoint InitalizeNeighborPoint(IPoint point, IFeature feature, Polygon sourcePolygon)
        //{
        //    if (point == null || feature == null)
        //    {
        //        return null;
        //    }
        //    Polygon landGeometry = feature.ShapeCopy as Polygon;
        //    if (landGeometry == null)
        //    {
        //        return null;
        //    }
        //    IProximityOperator mity = landGeometry as IProximityOperator;
        //    double distance = 0.0;
        //    distance = mity.ReturnDistance(point);
        //    if (distance > 1.0)
        //    {
        //        return null;
        //    }
        //    int index = InitalizeIndexPoint(landGeometry, point);
        //    if (index == -1)
        //    {
        //        return null;
        //    }
        //    IPoint spt = null;
        //    if (index >= 0 && index + 1 < landGeometry.PointCount)
        //    {
        //        spt = landGeometry.get_Point(index + 1);
        //        if (!PointOnPolygon(sourcePolygon, spt))
        //        {
        //            return spt;
        //        }
        //    }
        //    if (index == landGeometry.PointCount)
        //    {
        //        spt = landGeometry.get_Point(index - 1);
        //        if (!PointOnPolygon(sourcePolygon, spt))
        //        {
        //            return spt;
        //        }
        //    }
        //    if (index > 0)
        //    {
        //        spt = landGeometry.get_Point(index - 1);
        //        if (!PointOnPolygon(sourcePolygon, spt))
        //        {
        //            return spt;
        //        }
        //    }
        //    if (index == 0)
        //    {
        //        spt = landGeometry.get_Point(landGeometry.PointCount - 1);
        //    }
        //    if (!PointOnPolygon(sourcePolygon, spt))
        //    {
        //        return spt;
        //    }
        //    return null;
        //}

        /// <summary>
        /// 初始化点索引值
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        //private int InitalizeIndexPoint(Polygon polygon, IPoint point)
        //{
        //    int index = -1;
        //    for (int i = 0; i < polygon.PointCount; i++)
        //    {
        //        IPoint pt = polygon.get_Point(i);
        //        double distance = GeometryUtility.Distance(pt, point);
        //        if (distance <= 1.0)
        //        {
        //            index = i;
        //            break;
        //        }
        //    }
        //    return index;
        //}

        /// <summary>
        /// 点在面上
        /// </summary>
        /// <returns></returns>
        //private bool PointOnPolygon(Polygon polygon, IPoint point)
        //{
        //    if (polygon == null || point == null)
        //    {
        //        return false;
        //    }
        //    bool isVertex = false;
        //    for (int i = 0; i < polygon.PointCount; i++)
        //    {
        //        IPoint pt = polygon.get_Point(i);
        //        if (GeometryUtility.Equal(point, pt))
        //        {
        //            isVertex = true;
        //            break;
        //        }
        //    }
        //    return isVertex;
        //}

        #endregion Helper

        #region convert

        /// <summary>
        /// 根据枚举值获取枚举描述
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="enumValueStr">枚举值字符串</param>
        /// <returns></returns>
        private string GetEnumDesp<TEnum>(string enumValueStr) where TEnum : struct
        {
            TEnum tEnum = new TEnum();
            Enum.TryParse<TEnum>(enumValueStr, out tEnum);

            return EnumNameAttribute.GetDescription(tEnum);
        }

        #endregion convert

        #endregion 开始生成Excel

        #endregion Methods
    }
}