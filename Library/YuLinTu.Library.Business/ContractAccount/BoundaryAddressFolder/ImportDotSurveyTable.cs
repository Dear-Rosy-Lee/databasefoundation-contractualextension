/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using System.Collections;
using YuLinTu;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导入界址调查信息
    /// </summary>
    public class ImportDotSurveyTable : ExcelBase
    {
        #region Fields

        private int currentIndex;//当前索引值
        private int order;//顺序号
        private bool isLandEndFlag;//地块结束标志
        private ArrayList numberList;//地块列表
        private ArrayList errorList;//错误列表
        private int rowCount;//行数
        private int colCount;//列数
        private bool hasContractor = false;//是否含有户主名称

        #endregion

        #region Propertys

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 地块类型
        /// </summary>
        public LanderType LandorType { get; set; }

        /// <summary>
        /// 界址点信息
        /// </summary>
        public List<BuildLandBoundaryAddressDot> DotCollection { get; set; }

        /// <summary>
        /// 界址线信息
        /// </summary>
        public List<BuildLandBoundaryAddressCoil> LineCollection { get; set; }

        /// <summary>
        /// 错误集合
        /// </summary>
        public List<string> ErrorCollection { get; set; }

        /// <summary>
        /// 成功处理的地块总数
        /// </summary>
        public int LandCountHandle { get; set; }

        /// <summary>
        /// 地块数
        /// </summary>
        public int LandCount { get; set; }

        /// <summary>
        /// 界址点数
        /// </summary>
        public int DotCount { get; set; }

        /// <summary>
        /// 界址线数
        /// </summary>
        public int LineCount { get; set; }

        /// <summary>
        /// 界址点类型
        /// </summary>
        public List<Dictionary> DotTypeList { get; set; }

        /// <summary>
        /// 界线性质
        /// </summary>
        public List<Dictionary> LinePropertyList { get; set; }

        /// <summary>
        /// 界线位置
        /// </summary>
        public List<Dictionary> LinePositionList { get; set; }

        /// <summary>
        /// 界址线类型
        /// </summary>
        public List<Dictionary> LineTypeList { get; set; }

        /// <summary>
        /// 界标类型
        /// </summary>
        public List<Dictionary> MarkTypeList { get; set; }

        /// <summary>
        /// Srid
        /// </summary>
        public int Srid { get; set; }

        #endregion

        #region Ctor

        public ImportDotSurveyTable()
        {
            DotCollection = new List<BuildLandBoundaryAddressDot>();
            LineCollection = new List<BuildLandBoundaryAddressCoil>();
            ErrorCollection = new List<string>();
            numberList = new ArrayList();
            errorList = new ArrayList();
            // hasContractor =AgricultureSetting.IsUseHeiLongJiangRegion;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 读方法
        /// </summary>
        public override void Read()
        {
            if (string.IsNullOrEmpty(FileName))
            {
                return;
            }
            try
            {
                Open(FileName);//打开文件
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 写方法
        /// </summary>
        public override void Write()
        {
        }

        /// <summary>
        /// 获取开始行数据
        /// </summary>
        /// <param name="range">值范围</param>
        /// <param name="allItem">所有值</param>
        /// <returns></returns>
        private int InitalizeStartRow(int rowCount, object[,] allItem)
        {
            int startIndex = 0;
            for (int i = 0; i < rowCount; i++)
            {
                string rowValue = GetString(allItem[i, 0]);//编号栏数据
                if (!string.IsNullOrEmpty(rowValue) && ToolMath.MatchEntiretyNumber(rowValue))
                {
                    startIndex = i;
                    break;
                }
            }
            return startIndex;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        public bool ReadInformation()
        {
            object[,] allItem = GetAllRangeValue();//获取所有使用域值
            if (allItem == null)
            {
                return false;
            }
            string information = "";
            colCount = GetRangeColumnCount();
            int columnCount = (LandorType == LanderType.AgricultureLand || LandorType == LanderType.WoodLand) ? 13 : 11;
            if (colCount < columnCount)
            {
                information = "表中数据调查格式与调查表格式不符,列数不足!";
                AddErrorInformation(information);
                return false;
            }
            rowCount = GetRangeRowCount();
            int calIndex = 3;// InitalizeStartRow(rowCount, allItem);//获取数据开始行数
            string landNumber = string.Empty;
            currentIndex = 0;
            for (int index = calIndex; index < rowCount; index += 2)
            {
                currentIndex = index;//当前行数
                string parcelNumber = GetString(allItem[currentIndex, hasContractor ? 2 : 0]);//地块编码栏数据
                if (!string.IsNullOrEmpty(parcelNumber))
                {
                    landNumber = parcelNumber;
                    if (numberList.Contains(parcelNumber))
                    {
                        errorList.Add(parcelNumber);
                    }
                    else
                    {
                        numberList.Add(parcelNumber);
                    }
                    order = 1;
                }
                if (isLandEndFlag)
                {
                    isLandEndFlag = false;
                    continue;
                }
                BuildLandBoundaryAddressDot startDot = InitalizeDotInformation(allItem, landNumber);
                currentIndex += 1;
                BuildLandBoundaryAddressCoil line = InitalizeLineInformation(allItem);
                line.OrderID = (short)order;
                line.LandNumber = landNumber;
                currentIndex += 1;
                BuildLandBoundaryAddressDot endDot = InitalizeDotInformation(allItem, landNumber);
                startDot.LandNumber = landNumber;
                endDot.LandNumber = landNumber;
                line.StartPointID = startDot.ID;
                line.EndPointID = endDot.ID;
                line.CoilLength = InitalizeLineLength(startDot, endDot);
                order++;
                if (currentIndex + 2 >= rowCount)
                {
                    break;
                }
            }
            allItem = null;
            rowCount = 0;
            currentIndex = 0;
            foreach (string number in errorList)
            {
                string errorInformation = string.Format("表中地块编码{0}重复!", number);
                ErrorCollection.Add(errorInformation);
            }
            LandCount = numberList.Count;
            VerifyDotInformation();
            numberList.Clear();
            errorList.Clear();
            GC.Collect();
            return (ErrorCollection != null && ErrorCollection.Count == 0) ? true : false;
        }

        /// <summary>
        /// 初始化界址点信息
        /// </summary>
        private BuildLandBoundaryAddressDot InitalizeDotInformation(object[,] allItem, string landNumber)
        {
            string information = string.Empty;
            BuildLandBoundaryAddressDot dot = new BuildLandBoundaryAddressDot();
            if (currentIndex >= rowCount)
            {
                return dot;
            }
            dot.DotNumber = 1 < colCount ? GetString(allItem[currentIndex, hasContractor ? 3 : 1]) : "";//点号栏数据
            if (string.IsNullOrEmpty(dot.DotNumber))
            {
                information = string.Format("表中第{0}行数据界址点号为空!", currentIndex + 1);
                AddErrorInformation(information);
            }
            string dotType = 2 < colCount ? GetString(allItem[currentIndex, hasContractor ? 4 : 2]) : "";//点号栏数据
            if (string.IsNullOrEmpty(dotType))
            {
                information = string.Format("表中第{0}行数据界址点类型为空!", currentIndex + 1);
                AddErrorInformation(information);
            }
            else
            {
                var dic = DotTypeList.Find(testc => testc.Name == dotType);
                if (dic != null)
                {
                    dot.DotType = dic.Code;
                }
                else
                {
                    information = string.Format("表中第{0}行数据界址点类型不符合界址点类型填写要求!", currentIndex + 1);
                    AddErrorInformation(information);
                }
            }
            string markType = 3 < colCount ? GetString(allItem[currentIndex, hasContractor ? 5 : 3]) : "";//界标栏数据
            if (string.IsNullOrEmpty(markType))
            {
                information = string.Format("表中第{0}行数据界标类型为空!", currentIndex + 1);
                AddErrorInformation(information);
            }
            else
            {
                var dic = MarkTypeList.Find(testc => testc.Name == markType);
                if (dic != null)
                {
                    dot.LandMarkType = dic.Code;
                }
                else
                {
                    information = string.Format("表中第{0}行数据界标类型不符合界标类型填写要求!", currentIndex + 1);
                    AddErrorInformation(information);
                }
            }
            string coordinate = (hasContractor ? 6 : 4) < colCount ? GetString(allItem[currentIndex, hasContractor ? 6 : 4]) : "";//X坐标栏数据
            if (string.IsNullOrEmpty(coordinate))
            {
                information = string.Format("表中第{0}行数据界址点X坐标为空!", currentIndex + 1);
                AddErrorInformation(information);
            }
            double xvalue = 0.0;
            double.TryParse(coordinate, out xvalue);
            if (!string.IsNullOrEmpty(coordinate) && xvalue == 0.0)
            {
                information = string.Format("表中第{0}行数据界址点X坐标填写不符合坐标值要求!", currentIndex + 1);
                AddErrorInformation(information);
            }
            coordinate = (hasContractor ? 7 : 5) < colCount ? GetString(allItem[currentIndex, hasContractor ? 7 : 5]) : "";//Y坐标栏数据
            if (string.IsNullOrEmpty(coordinate))
            {
                information = string.Format("表中第{0}行数据界址点Y坐标为空!", currentIndex + 1);
                AddErrorInformation(information);
            }
            double yvalue = 0.0;
            double.TryParse(coordinate, out yvalue);
            if (!string.IsNullOrEmpty(coordinate) && yvalue == 0.0)
            {
                information = string.Format("表中第{0}行数据界址点Y坐标填写不符合坐标值要求!", currentIndex + 1);
                AddErrorInformation(information);
            }
            Spatial.Coordinate corrdnate = new Spatial.Coordinate(xvalue, yvalue);
            dot.Shape = YuLinTu.Spatial.Geometry.CreatePoint(corrdnate, Srid);
            dot.LandNumber = landNumber;
            BuildLandBoundaryAddressDot sp = DotCollection.Find(dt => dt.DotNumber == dot.DotNumber && dt.LandNumber == landNumber);
            if (sp == null)
            {
                DotCollection.Add(dot);
            }
            isLandEndFlag = sp != null;
            return sp != null ? sp : dot;
        }

        /// <summary>
        /// 初始化界址线信息
        /// </summary>
        private BuildLandBoundaryAddressCoil InitalizeLineInformation(object[,] allItem)
        {
            string information = string.Empty;
            BuildLandBoundaryAddressCoil line = new BuildLandBoundaryAddressCoil();
            if (currentIndex >= rowCount)
            {
                return line;
            }
            double length = 0.0;
            string value = (hasContractor ? 8 : 6) < colCount ? GetString(allItem[currentIndex, hasContractor ? 8 : 6]) : "";//界址长度数据
            double.TryParse(value, out length);
            line.CoilLength = length;
            string lineType = (hasContractor ? 9 : 7) < colCount ? GetString(allItem[currentIndex, hasContractor ? 9 : 7]) : "";//界线性质栏数据
            if (string.IsNullOrEmpty(lineType))
            {
                information = string.Format("表中第{0}行数据界线性质为空!", currentIndex + 1);
                AddErrorInformation(information);
            }
            else
            {
                try
                {
                    line.LineType = LinePropertyList.Find(t => t.Name == lineType).Code;
                }
                catch
                {
                    information = string.Format("表中第{0}行数据界线性质填写不符合界线性质填写要求!", currentIndex + 1);
                    AddErrorInformation(information);
                }
            }
            string coilType = (hasContractor ? 10 : 8) < colCount ? GetString(allItem[currentIndex, hasContractor ? 10 : 8]) : "";//界线类别栏数据
            if (string.IsNullOrEmpty(coilType))
            {
                information = string.Format("表中第{0}行数据界址线类别为空!", currentIndex + 1);
                AddErrorInformation(information);
            }
            else
            {
                try
                {
                    line.CoilType = LineTypeList.Find(t => t.Name == coilType).Code;
                }
                catch
                {
                    information = string.Format("表中第{0}行数据界址线类别填写不符合界址线类别填写要求!", currentIndex + 1);
                    AddErrorInformation(information);
                }
            }
            string positionType = (hasContractor ? 11 : 9) < colCount ? GetString(allItem[currentIndex, hasContractor ? 11 : 9]) : "";//界线位置 栏数据
            if (string.IsNullOrEmpty(positionType))
            {
                information = string.Format("表中第{0}行数据界址线位置为空!", currentIndex + 1);
                AddErrorInformation(information);
            }
            else
            {
                try
                {
                    line.Position = LinePositionList.Find(t => t.Name == positionType).Code;
                }
                catch
                {
                    information = string.Format("表中第{0}行数据界址线位置填写不符合界址线位置填写要求!", currentIndex + 1);
                    AddErrorInformation(information);
                }
            }
            line.Description = (hasContractor ? 12 : 10) < colCount ? GetString(allItem[currentIndex, hasContractor ? 12 : 10]) : "";//界线说明
            if (LandorType == LanderType.AgricultureLand || LandorType == LanderType.WoodLand)
            {
                line.NeighborPerson = (hasContractor ? 13 : 11) < colCount ? GetString(allItem[currentIndex, hasContractor ? 13 : 11]) : "";//界线毗邻地物承包方
                line.NeighborFefer = (hasContractor ? 14 : 12) < colCount ? GetString(allItem[currentIndex, hasContractor ? 14 : 12]) : "";//界线毗邻地物指界人
            }
            LineCollection.Add(line);
            return line;
        }

        /// <summary>
        /// 初始化直线长度
        /// </summary>
        /// <param name="firDot"></param>
        /// <param name="secDot"></param>
        /// <returns></returns>
        private double InitalizeLineLength(BuildLandBoundaryAddressDot firDot, BuildLandBoundaryAddressDot secDot)
        {
            double distance = firDot.Shape.Distance(secDot.Shape);
            return ToolMath.RoundNumericFormat(distance, 2);
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="information"></param>
        private void AddErrorInformation(string information)
        {
            if (ErrorCollection.Contains(information))
            {
                return;
            }
            ErrorCollection.Add(information);
        }

        /// <summary>
        /// 检查界址点线信息
        /// </summary>
        private void VerifyDotInformation()
        {
            string errorInformation = "";
            foreach (string landNumber in numberList)
            {
                int dotCount = DotCollection.Count(dt => dt.LandNumber == landNumber);
                if (dotCount < 3)
                {
                    errorInformation = string.Format("地块编码{0}中不相同界址点个数{1}无效,应至少包含3个不相同的界址点!", landNumber, dotCount);
                    ErrorCollection.Add(errorInformation);
                }
                int lineCount = LineCollection.Count(le => le.LandNumber == landNumber);
                if (lineCount < 3)
                {
                    errorInformation = string.Format("地块编码{0}中不相同界址线条数{1}无效,应至少包含3条不相同的界址线!", landNumber, lineCount == 0 ? 1 : lineCount);
                    ErrorCollection.Add(errorInformation);
                }
            }
        }

        #endregion
    }
}
