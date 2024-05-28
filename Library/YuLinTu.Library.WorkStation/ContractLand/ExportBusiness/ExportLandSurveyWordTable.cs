/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Library.Repository;
using YuLinTu.Windows;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    ///导出地块调查表
    /// </summary>
    [Serializable]
    public class ExportLandSurveyWordTable : AgricultureWordBookWork
    {
        #region Fields

        private List<Dictionary> dictJBLX;
        private List<Dictionary> dictJZXLB;

        #endregion

        #region Properties

        /// <summary>
        /// 是否模版
        /// </summary>
        public bool IsTemplate { get; set; }

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefineWork SettingDefine
        { get; set; }

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

        /// <summary>
        /// 数据源
        /// </summary>
        public IDbContext DbContext { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportLandSurveyWordTable()
        {
            //var dbContext = DataBaseSource.GetDataBaseSource();
            //if (dbContext == null)
            //{
            //    return;
            //}
            //var dictStation = dbContext.CreateDictWorkStation();
            //DictList = dictStation.Get<Dictionary>();
            if (DictList != null && DictList.Count > 0)
            {
                dictJBLX = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.JBLX);
                dictJZXLB = DictList.FindAll(t => t.GroupCode == DictionaryTypeInfo.JZXLB);
            }
            else
            {
                dictJBLX = new List<Dictionary>();
                dictJZXLB = new List<Dictionary>();
            }
        }

        #endregion

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
                if (ListLandValidDot.Count == 0 && ListLandCoil.Count != 0 && ListLandDot.Count != 0)
                {
                    WriteLandDotInformation(ListLandCoil, ListLandDot);
                }
                else if (ListLandValidDot.Count >= 0)
                {
                    WriteLandDotInformation(ListLandValidDot);
                    WriteLandCoilInformation(ListLandCoil, ListLandDot, ListLandValidDot);
                }

                //if (SettingDefine.LandSurvyTableDataCondition)
                //{
                //    InitalizeLandDotInformation(ListLandCoil, ListLandDot);
                //}
                //else
                //{
                //    WriteLandDotInformation(ListLandCoil, ListLandDot);
                //}
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
            coil.Description = (Math.Round(length, 4)).ToString();
        }

        /// <summary>
        /// 书写界址点线信息
        /// </summary>
        /// <param name="land">地块</param>
        private void WriteLandDotInformation(List<BuildLandBoundaryAddressCoil> lineCollection, List<BuildLandBoundaryAddressDot> dotCollection)
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
                    if (!dots.Contains(startDot))
                    {
                        WriteLandDotInformation(startDot, i + index + 1);
                        dots.Add(startDot);
                    }
                    WriteLandDotInformation(endDot, i + index + 2);
                    if (!dots.Contains(endDot))
                    {
                        dots.Add(endDot);
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
        /// 书写界址点线信息
        /// </summary>
        /// <param name="land">地块</param>
        private void InitalizeLandDotInformation(List<BuildLandBoundaryAddressCoil> lineCollection, List<BuildLandBoundaryAddressDot> dotCollection)
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
            InitalizeLineCollection(lineCollection, listIndex, dotCollection);
            lineCollection = null;
            dotCollection = null;
            listIndex = null;
            GC.Collect();
        }

        /// <summary>
        /// 初始化界址线信息
        /// </summary>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> InitalizeLineCollection(List<BuildLandBoundaryAddressCoil> lines, List<Int32> listIndex, List<BuildLandBoundaryAddressDot> dotCollection)
        {
            List<BuildLandBoundaryAddressCoil> lineCollection = new List<BuildLandBoundaryAddressCoil>();
            for (int index = 0; index < listIndex.Count - 1; index++)
            {
                int st = listIndex[index];
                int et = listIndex[index + 1];
                List<BuildLandBoundaryAddressCoil> lineArray = InitalizeLineInfor(lines, st, et, dotCollection);
                List<BuildLandBoundaryAddressDot> dots = new List<BuildLandBoundaryAddressDot>();
                for (int i = 0; i < lineArray.Count; i++)
                {
                    BuildLandBoundaryAddressCoil line = lineArray[i];
                    BuildLandBoundaryAddressDot startDot = dotCollection.Find(dot => dot.ID == line.StartPointID);
                    BuildLandBoundaryAddressDot endDot = dotCollection.Find(dot => dot.ID == line.EndPointID);
                    if (!dots.Contains(startDot))
                    {
                        WriteLandDotInformation(startDot, i + index + 1);
                        dots.Add(startDot);
                    }
                    WriteLandDotInformation(endDot, i + index + 2);
                    if (!dots.Contains(endDot))
                    {
                        dots.Add(endDot);
                    }
                    WriteLandLineInformation(line, i + index + 1);
                }
                dots = null;
            }
            return lineCollection;
        }

        /// <summary>
        /// 初始化界址线信息
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> InitalizeLineInfor(List<BuildLandBoundaryAddressCoil> lines, int startIndex, int endIndex, List<BuildLandBoundaryAddressDot> dots)
        {
            if (lines == null || lines.Count == 0)
            {
                return new List<BuildLandBoundaryAddressCoil>();
            }
            List<BuildLandBoundaryAddressCoil> lineCollection = new List<BuildLandBoundaryAddressCoil>();
            for (int index = startIndex; index < endIndex; index++)
            {
                BuildLandBoundaryAddressCoil curLine = lines[index].Clone() as BuildLandBoundaryAddressCoil;
                if (lineCollection.Count == 0)
                {
                    lineCollection.Add(curLine);
                    continue;
                }
                BuildLandBoundaryAddressCoil coil = lineCollection[lineCollection.Count - 1];
                if (index + 1 == endIndex)
                {
                    if (curLine.NeighborPerson != coil.NeighborPerson)
                    {
                        lineCollection.Add(curLine);
                    }
                    else
                    {
                        if (lineCollection.Count > 3)
                        {
                            coil.EndPointID = curLine.EndPointID;
                        }
                    }
                    continue;
                }
                if (curLine.NeighborPerson != coil.NeighborPerson)
                {
                    lineCollection.Add(curLine);
                }
            }
            if (lineCollection.Count == 2)
            {
                if (lineCollection[1].ID == lines[lines.Count - 1].ID)
                {
                    lineCollection[1] = lines[1];
                }
            }
            lineCollection = InitalizeLineCollection(lines, startIndex, endIndex, dots, lineCollection);
            InitalizeLineDescription(lines, startIndex, endIndex, dots, lineCollection);
            return lineCollection;
        }

        /// <summary>
        /// 初始化界址线信息
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private void InitalizeLineDescription(List<BuildLandBoundaryAddressCoil> lines, int startIndex, int endIndex, List<BuildLandBoundaryAddressDot> dots, List<BuildLandBoundaryAddressCoil> lineCollection)
        {
            if (!SettingDefine.SurveyLimitFillWay)
            {
                return;
            }
            if (lineCollection == null || lineCollection.Count == 0)
            {
                return;
            }
            for (int i = 0; i < lineCollection.Count - 1; i++)
            {
                BuildLandBoundaryAddressCoil firLine = lineCollection[i];
                BuildLandBoundaryAddressCoil secLine = lineCollection[i + 1];
                firLine.EndPointID = secLine.StartPointID;
            }
            lineCollection[lineCollection.Count - 2].EndPointID = lineCollection[lineCollection.Count - 1].StartPointID;
            lineCollection[lineCollection.Count - 1].EndPointID = lineCollection[0].StartPointID;
            foreach (var line in lineCollection)
            {
                if (!string.IsNullOrEmpty(line.Description))
                {
                    continue;
                }
                BuildLandBoundaryAddressDot sd = dots.Find(dt => dt.ID == line.StartPointID);
                BuildLandBoundaryAddressDot ed = dots.Find(dt => dt.ID == line.EndPointID);
                string startNumber = ToolString.GetAllNumberWithInString(sd.DotNumber);
                string endNumber = ToolString.GetAllNumberWithInString(ed.DotNumber);
                int sn = 0;
                int en = 0;
                Int32.TryParse(startNumber, out sn);
                Int32.TryParse(endNumber, out en);
                if (sn + 1 == en)
                {
                    //line.Description = showDescription ? description : ToolMath.SetNumbericFormat(line.CoilLength.ToString(), 2);
                    line.Description = ToolMath.SetNumbericFormat(line.CoilLength.ToString(), 2);
                    continue;
                }
                if (en == 1 && sn == endIndex)
                {
                    //line.Description = showDescription ? description : line.CoilLength.ToString();
                    line.Description = line.CoilLength.ToString();
                    continue;
                }
                double lineLength = 0.0;
                int st = lines.FindIndex(le => le.StartPointID == sd.ID);
                int et = lines.FindIndex(le => le.StartPointID == ed.ID);
                if (et == 0 && st < endIndex)
                {
                    et = endIndex;
                }
                for (int index = st; index < et; index++)
                {
                    BuildLandBoundaryAddressCoil lineCoil = lines[index];
                    lineLength += lineCoil.CoilLength;
                }
                //line.Description = showDescription ? description : ToolMath.SetNumbericFormat(lineLength.ToString(), 2);
                line.Description = ToolMath.SetNumbericFormat(lineLength.ToString(), 2);
            }
        }

        /// <summary>
        /// 初始化界址点集合
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="dots"></param>
        /// <param name="lineCollection"></param>
        /// <returns></returns>
        private List<BuildLandBoundaryAddressCoil> InitalizeLineCollection(List<BuildLandBoundaryAddressCoil> lines, int startIndex, int endIndex, List<BuildLandBoundaryAddressDot> dots, List<BuildLandBoundaryAddressCoil> lineCollection)
        {
            if (lineCollection == null || lineCollection.Count == 0)
            {
                return lineCollection;
            }
            BuildLandBoundaryAddressCoil coil = lineCollection[lineCollection.Count - 1];
            int index = lines.FindIndex(le => le.StartPointID == coil.StartPointID && le.EndPointID == coil.EndPointID);
            if (lineCollection.Count == 1)
            {
                if (lines.Count <= 6)
                {
                    lineCollection = lines;
                }
                else
                {
                    int sd = (endIndex - index) / 3;
                    sd = sd > 0 ? sd : 1;
                    sd = index + sd;
                    lineCollection.Add(lines[sd]);
                    int ed = (endIndex - sd) / 3;
                    ed = ed > 0 ? ed : 1;
                    ed = sd + ed;
                    ed = ed < (endIndex - 1) ? ed : (ed - 1);
                    lineCollection.Add(lines[ed]);
                    lineCollection.Add(lines[endIndex - 1]);
                }
            }
            if (lineCollection.Count == 2)
            {
                if (lines.Count <= 6)
                {
                    lineCollection = lines;
                }
                else
                {
                    int sd = (endIndex - index) / 2;
                    sd = sd > 0 ? sd : 1;
                    sd = index + sd;
                    sd = sd >= lines.Count ? (endIndex - 2) : sd;
                    lineCollection.Add(lines[sd]);
                    lineCollection.Add(lines[endIndex - 1]);
                }
            }
            if (lineCollection.Count == 3)
            {
                if (lines.Count <= 6)
                {
                    lineCollection = lines;
                }
                else
                {
                    lineCollection.Add(lines[endIndex - 1]);
                }
            }
            return lineCollection;
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
        /// 初始化界址线信息
        /// </summary>
        /// <returns></returns>
        private void InitalizeLandDotValue(List<BuildLandBoundaryAddressCoil> lines, List<BuildLandBoundaryAddressDot> dotCollection)
        {
            List<BuildLandBoundaryAddressCoil> lineArray = new List<BuildLandBoundaryAddressCoil>();
            foreach (var dot in dotCollection)
            {
                BuildLandBoundaryAddressCoil line = lines.Find(le => le.StartPointID == dot.ID);
                if (line != null)
                {
                    lineArray.Add(line);
                }
            }
            InitalizeLineDescription(lines, dotCollection, lineArray);
            List<BuildLandBoundaryAddressDot> dots = new List<BuildLandBoundaryAddressDot>();
            for (int i = 0; i < lineArray.Count; i++)
            {
                BuildLandBoundaryAddressCoil line = lineArray[i];
                BuildLandBoundaryAddressDot startDot = dotCollection.Find(dot => dot.ID == line.StartPointID);
                BuildLandBoundaryAddressDot endDot = dotCollection.Find(dot => dot.ID == line.EndPointID);
                if (!dots.Contains(startDot))
                {
                    WriteLandDotInformation(startDot, i + 1);
                    dots.Add(startDot);
                }
                WriteLandDotInformation(endDot, i + 2);
                if (!dots.Contains(endDot))
                {
                    dots.Add(endDot);
                }
                WriteLandLineInformation(line, i + 1);
            }
            dots = null;
        }

        /// <summary>
        /// 初始化界址线信息
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private void InitalizeLineDescription(List<BuildLandBoundaryAddressCoil> lines, List<BuildLandBoundaryAddressDot> dots, List<BuildLandBoundaryAddressCoil> lineCollection)
        {
            if (!SettingDefine.SurveyLimitFillWay)
            {
                return;
            }
            for (int i = 0; i < lineCollection.Count - 1; i++)
            {
                BuildLandBoundaryAddressCoil firLine = lineCollection[i];
                BuildLandBoundaryAddressCoil secLine = lineCollection[i + 1];
                firLine.EndPointID = secLine.StartPointID;
            }
            lineCollection[lineCollection.Count - 2].EndPointID = lineCollection[lineCollection.Count - 1].StartPointID;
            lineCollection[lineCollection.Count - 1].EndPointID = lineCollection[0].StartPointID;
            foreach (var line in lineCollection)
            {
                if (!string.IsNullOrEmpty(line.Description))
                {
                    continue;
                }
                BuildLandBoundaryAddressDot sd = dots.Find(dt => dt.ID == line.StartPointID);
                BuildLandBoundaryAddressDot ed = dots.Find(dt => dt.ID == line.EndPointID);
                string startNumber = ToolString.GetAllNumberWithInString(sd.DotNumber);
                string endNumber = ToolString.GetAllNumberWithInString(ed.DotNumber);
                int sn = 0;
                int en = 0;
                Int32.TryParse(startNumber, out sn);
                Int32.TryParse(endNumber, out en);
                if (sn + 1 == en)
                {
                    //line.Description = showDescription ? description : ToolMath.SetNumbericFormat(line.CoilLength.ToString(), 2);
                    line.Description = ToolMath.SetNumbericFormat(line.CoilLength.ToString(), 2);
                    continue;
                }
                if (en == 1 && sn == dots.Count)
                {
                    //line.Description = showDescription ? description : line.CoilLength.ToString();
                    line.Description = line.CoilLength.ToString();
                    continue;
                }
                double lineLength = 0.0;
                int st = lines.FindIndex(le => le.StartPointID == sd.ID);
                int et = lines.FindIndex(le => le.StartPointID == ed.ID);
                if (sn > en && en != 1)
                {
                    for (int index = st; index < lines.Count; index++)
                    {
                        BuildLandBoundaryAddressCoil lineCoil = lines[index];
                        lineLength += lineCoil.CoilLength;
                    }
                    for (int index = 0; index < et; index++)
                    {
                        BuildLandBoundaryAddressCoil lineCoil = lines[index];
                        lineLength += lineCoil.CoilLength;
                    }
                    line.Description = ToolMath.SetNumbericFormat(lineLength.ToString(), 2);
                    continue;
                }
                for (int index = st; index < et; index++)
                {
                    BuildLandBoundaryAddressCoil lineCoil = lines[index];
                    lineLength += lineCoil.CoilLength;
                }
                //line.Description = showDescription ? description : ToolMath.SetNumbericFormat(lineLength.ToString(), 2);
                line.Description = ToolMath.SetNumbericFormat(lineLength.ToString(), 2);
            }
        }

        /// <summary>
        /// 书写表头信息
        /// </summary>
        private void WriteTitleInformation(ContractLand land, string tissueCode)
        {
            List<Dictionary> listDLDJ = DictList.FindAll(c => c.GroupCode == DictionaryTypeInfo.DLDJ);
            SetBookmarkValue("SenderCode", tissueCode);//发包方编码
            var center = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = center.GetProfile<SystemSetDefineWork>();
            var section = profile.GetSection<SystemSetDefineWork>();
            var config = section.Settings as SystemSetDefineWork;
            SetBookmarkValue("ContractorName", land.OwnerName.InitalizeFamilyName(config.KeepRepeatFlag));//承包方名称
            SetBookmarkValue("LandImageNumber", land.LandExpand.ImageNumber);//图幅号
            //string landNumber = ContractLand.GetLandNumber(land.LandNumber);
            //landNumber = landNumber.Length > 5 ? landNumber.Substring(landNumber.Length - 5) : landNumber;
            SetBookmarkValue("LandNumber", land.LandNumber);//地块编码
            SetBookmarkValue("LandName", land.Name);//地块名称
            SetBookmarkValue("TableArea", (land.TableArea != null && land.TableArea.HasValue) ? ToolMath.SetNumbericFormat(land.TableArea.Value.ToString(), 2) : "");//实测面积
            SetBookmarkValue("ActualArea", ToolMath.SetNumbericFormat(land.ActualArea.ToString(), 2));//实测面积
            SetBookmarkValue("AwareArea", ToolMath.SetNumbericFormat(land.AwareArea.ToString(), 2));//实测面积
            SetBookmarkValue("East", land.NeighborEast != null ? land.NeighborEast : "");//东至
            SetBookmarkValue("South", land.NeighborSouth != null ? land.NeighborSouth : "");//南至
            SetBookmarkValue("West", land.NeighborWest != null ? land.NeighborWest : "");//西至
            SetBookmarkValue("North", land.NeighborNorth != null ? land.NeighborNorth : "");//北至
            string levelString = ToolMath.MatchEntiretyNumber(land.LandLevel.ToString()) ? listDLDJ.Find(c => c.Code == land.LandLevel).Name : "";
            levelString = levelString == "未知" ? "" : levelString;
            levelString = AgricultureSettingWork.UseSystemLandLevelDescription ? levelString : listDLDJ.Find(c => c.Code == land.LandLevel).Name;
            SetBookmarkValue("LandLevel", levelString);//地力等级
            string surveyDate = (land.LandExpand.SurveyDate != null && land.LandExpand.SurveyDate.HasValue) ? ToolDateTime.GetLongDateString(land.LandExpand.SurveyDate.Value) : "";
            string checkDate = (land.LandExpand.CheckDate != null && land.LandExpand.CheckDate.HasValue) ? ToolDateTime.GetLongDateString(land.LandExpand.CheckDate.Value) : "";
            SetBookmarkValue("LandSurveyPerson", land.LandExpand.SurveyPerson);//地块调查员
            SetBookmarkValue("LandSurveyDate", surveyDate);//地块调查日期
            SetBookmarkValue("LandSurveyChronicle", land.LandExpand.SurveyChronicle);//地块调查记事
            SetBookmarkValue("LandCheckPerson", land.LandExpand.CheckPerson);//地块审核员
            SetBookmarkValue("LandCheckDate", checkDate);//地块审核日期
            SetBookmarkValue("LandCheckOpinion", land.LandExpand.CheckOpinion);//地块审核意见
            for (int i = 0; i < 12; i++)
            {
                SetBookmarkValue("LandFefer" + (i == 0 ? "" : i.ToString()), land.LandExpand.ReferPerson);//地块指界人
            }
            object obj = Enum.Parse(typeof(eLandPurposeType), land.Purpose.ToString());
            string platting = (obj == null || ToolMath.MatchEntiretyNumber(obj.ToString())) ? eLandPurposeType.Planting.ToString() : ((eLandPurposeType)obj).ToString();
            SetBookmarkValue(platting, "R");
            string isFamrmerLand = (land.IsFarmerLand == null || !land.IsFarmerLand.HasValue) ? "IsFarmerLand" : (land.IsFarmerLand.Value ? "IsFarmerLand" : "IsFarmerLandNothing");
            SetBookmarkValue(isFamrmerLand, "R");
            if (!string.IsNullOrEmpty(land.LandName))
            {
                WriteLandTypeNameInformation(land);
            }
            else
            {
                WriteLandTypeInformation(land);
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
        private bool WriteLandTypeNameInformation(ContractLand land)
        {
            bool useName = false;
            switch (land.LandName)
            {
                case "水田":
                    SetBookmarkValue("Paddy", "R");
                    useName = true;
                    break;
                case "旱地":
                    SetBookmarkValue("Dry", "R");
                    useName = true;
                    break;
                case "水浇地":
                    SetBookmarkValue("Irrigated", "R");
                    useName = true;
                    break;
                case "果园":
                    SetBookmarkValue("Orchard", "R");
                    useName = true;
                    break;
                case "茶园":
                    SetBookmarkValue("TeaGarden", "R");
                    useName = true;
                    break;
                default:
                    SetBookmarkValue("OtherLandType", "R");
                    break;
            }
            return useName;
        }

        /// <summary>
        /// 书写地类信息
        /// </summary>
        private void WriteLandTypeInformation(ContractLand land)
        {
            switch (land.LandCode)
            {
                case "011":
                    SetBookmarkValue("Paddy", "R");
                    break;
                case "012":
                    SetBookmarkValue("Dry", "R");
                    break;
                case "013":
                    SetBookmarkValue("Irrigated", "R");
                    break;
                case "021":
                    SetBookmarkValue("Orchard", "R");
                    break;
                case "022":
                    SetBookmarkValue("TeaGarden", "R");
                    break;
                default:
                    SetBookmarkValue("OtherLandType", "R");
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
                lineCategory = (eBoundaryLineCategory)obj;
            //Enum.TryParse<eBoundaryLineCategory>(line.CoilType, out lineCategory);
            switch (lineCategory)
            {
                case eBoundaryLineCategory.Baulk:
                    SetBookmarkValue("LineBaulk" + index.ToString(), "√");//田埂
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
                    SetBookmarkValue("LineOther" + index.ToString(), "√");//其他界线
                    break;
            }
            eBoundaryLinePosition linePosition = eBoundaryLinePosition.Left;
            Enum.TryParse<eBoundaryLinePosition>(line.Position, out linePosition);
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

            SetBookmarkValue("LandLineDescription" + index.ToString(), line.Description);//界址线说明
            SetBookmarkValue("LineNeighborPerson" + index.ToString(), line.NeighborPerson);//毗邻地块承包方
            SetBookmarkValue("LandLineNeighborFefer" + index.ToString(), line.NeighborFefer);//毗邻地块指界人
        }

        /// <summary>
        /// 初始化界址点数
        /// </summary>
        /// <returns></returns>
        public int InitalizeDotCount(ContractLand land)
        {
            //int count = 0;
            //try
            //{
            //    if (DbContext == null)
            //    {
            //        return count;
            //    }
            //    var dotStation = DbContext.CreateDotWorkStation();
            //    var coilStation = DbContext.CreateCoilWorkStation();
            //    if (SettingDefine.LandSurvyTableDataCondition)
            //    {
            //        count = InitalizeLandDotCount(land, dotStation, coilStation);
            //    }
            //    else
            //    {
            //        count = coilStation.CountByLandID(land.ID);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    count = 0;
            //    throw new YltException("获取界址信息失败!");
            //}
            //return count;
            return 0;
        }

        /// <summary>
        /// 书写界址点线信息
        /// </summary>
        /// <param name="land">地块</param>
        private int InitalizeLandDotCount(ContractLand land, IBuildLandBoundaryAddressDotWorkStation dotStation, IBuildLandBoundaryAddressCoilWorkStation coilStation)
        {
            List<BuildLandBoundaryAddressDot> dotCollection = dotStation.GetByLandID(land.ID);
            List<BuildLandBoundaryAddressCoil> lineCollection = coilStation.GetByLandID(land.ID);
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
            int count = InitalizeLineCount(lineCollection, listIndex, dotCollection);
            lineCollection = null;
            dotCollection = null;
            listIndex = null;
            GC.Collect();
            return count;
        }

        /// <summary>
        /// 初始化界址线信息
        /// </summary>
        /// <returns></returns>
        private int InitalizeLineCount(List<BuildLandBoundaryAddressCoil> lines, List<Int32> listIndex, List<BuildLandBoundaryAddressDot> dotCollection)
        {
            int count = 0;
            List<BuildLandBoundaryAddressCoil> lineCollection = new List<BuildLandBoundaryAddressCoil>();
            for (int index = 0; index < listIndex.Count - 1; index++)
            {
                int st = listIndex[index];
                int et = listIndex[index + 1];
                List<BuildLandBoundaryAddressCoil> lineArray = InitalizeLineInfor(lines, st, et, dotCollection);
                count += lineArray.Count;
            }
            return count;
        }

        private void Disponse()
        {
            GC.Collect();
        }

        #endregion

        #endregion
    }
}
