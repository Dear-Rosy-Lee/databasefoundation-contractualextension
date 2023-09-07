using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;
using YuLinTu.Library.Basic;
using YuLinTu.Library.YltDatabase;
using System.IO;

namespace YuLinTu.Library.Office
{
    public class InvestigateForm : ExportExcelBase
    {
        #region Ctor

        /// <summary>
        /// 带地域与文件名构造方法
        /// </summary>
        /// <param name="zone">地域</param>
        /// <param name="fileName">文件名</param>
        public InvestigateForm(IDatabase Database)
        {
            this.database = Database;//数据模型
            landLength = EnumNameAttribute.GetAttributes(typeof(eLandMarkType)).Length;//得到界标种类个数
            //界址线类别 
            categoryLength = EnumNameAttribute.GetAttributes(typeof(eBoundaryLineCategory)).Length + landLength + 3;
            positionLength = EnumNameAttribute.GetAttributes(typeof(eBoundaryLinePosition)).Length + categoryLength;

            BuildLandBoundaryAddressDot tempdot = new BuildLandBoundaryAddressDot();
            BuildLandBoundaryAddressCoil tempCoil = new BuildLandBoundaryAddressCoil();
            BuildLandProperty tempproperty = new BuildLandProperty();
        }

        #endregion

        #region Fields

        IDatabase database;

        BuildLandProperty buildLandProperty;

        Zone zone;

        BuildLandBoundaryAddressCoilCollection coils;

        BuildLandBoundaryAddressDotCollection dots;

        PropertyLineNeighborCollection neighbors;

        CadastralInvestigate investigate;

        // 界址点、界址线类别及界址线位置
        EnumNameAttribute[] tempMark;
        EnumNameAttribute[] tempCategory;
        EnumNameAttribute[] tempPosition;


        double collWidth;
        int collCount;
        List<string> collList;

        int landLength;
        int categoryLength;
        int positionLength;

        string message;
        string templatePath; //模板路径

        #endregion

        #region Methods

        #region 开始生成Excel之前的一系列操作

        public string BeginExcel(BuildLandProperty property, string templatePath)
        {
            this.templatePath = templatePath;

            if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                return "目标模板文件不存在！";

            if (property != null)
                buildLandProperty = property;
            else
                return "集体建设用地信息错误";

            Write();//写数据
            return message;
        }

        public override void Read()
        {
        }

        public override void Write()
        {
            try
            {
                OpenExcelFile();
                WriteHeadRow(1);//写表头
                if (string.IsNullOrEmpty(message))
                    Show();//显示数据
            }
            catch (System.Exception e)
            {
                message = e.Message;
                Dispose();
                if (e is TaskStopException)
                    throw e;
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        private void OpenExcelFile()
        {
            string filePaths = templatePath;

            Open(filePaths);
            Range all = ((Microsoft.Office.Interop.Excel.Worksheet)workbook.ActiveSheet).UsedRange;
            all.Clear();
        }

        /// <summary>
        /// 写表头
        /// </summary>
        private void WriteHeadRow(int index)
        {
            Microsoft.Office.Interop.Excel.Worksheet sheet = ((Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[index]);
            workSheet = sheet;
            zone = database.Zone.Get(buildLandProperty.LandLocatedCode);
            investigate = database.CadastralInvestigate.GetByLandID(buildLandProperty.ID);
            if (investigate == null || zone == null || buildLandProperty == null)
            {
                message = "错误！没有对应的调查表或者地域或者集体建设用地信息！";
                return;
            }
            switch (index)
            {
                case 1:
                    WriteFirstPage();//写第一页 
                    break;
                case 2:
                    WriteSecondPage();//写第二页 
                    break;
                case 3:
                    coils = database.BuildLandBoundaryAddressCoil.GetByLandID(buildLandProperty.ID);
                    dots = database.BuildLandBoundaryAddressDot.GetByLandID(buildLandProperty.ID);
                    coils = ToolSort.SortList(coils) as BuildLandBoundaryAddressCoilCollection;
                    dots = ToolSort.SortList(dots) as BuildLandBoundaryAddressDotCollection;
                    neighbors = database.PropertyLineNeighbor.GetByLandID(buildLandProperty.ID);
                    if (coils == null || dots == null)
                    {
                        message = "错误！没有对应的界址点或者界址线信息！";
                        return;
                    }
                    if (neighbors == null)
                    {
                        message = "错误！没有对应的邻宗地信息！";
                        return;
                    }
                    WriteThirdPage();//写第三页
                    break;
                case 4:
                    break;
                case 5:
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region SetRange

        /// <summary>
        /// 设置Range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="height"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        private void SetRangeInfo(string start, string end, string value, double height, double size, bool bold)
        {
            Range range = workSheet.get_Range(start, end);
            range.Merge(0);
            range.NumberFormatLocal = "@";
            range.WrapText = true;
            range.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            range.Value2 = value;
            range.RowHeight = height;
            range.Font.Size = size;
            range.Font.Bold = bold;
        }

        /// <summary>
        /// 设置Range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <param name="height"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        private void SetWidthAndRange(string start, string end, string value, double height, double size, bool bold)
        {
            Range range = workSheet.get_Range(start, end);
            range.Merge(0);
            range.NumberFormatLocal = "@";
            range.WrapText = true;
            range.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            range.RowHeight = height;
            range.ColumnWidth = collWidth;
            range.Font.Size = size;
            range.Value2 = value;
            range.Font.Bold = bold;
        }

        /// <summary>
        /// 设置边框
        /// </summary>
        private void SetLineType(string end)
        {
            Range range = workSheet.get_Range("A1", end);
            range.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            SetRangeLineType(range, XlLineStyle.xlContinuous);
        }

        #endregion

        #region 第一页

        /// <summary>
        /// 打印第一页信息
        /// </summary>
        private void WriteFirstPage()
        {
            SetRangeInfo("A1", "I4", "", 13.5, 12, false);
            SetRangeInfo("A5", "G6", "", 13.5, 12, false);
            SetRangeInfo("H5", "I5", "", 13.5, 12, false);
            SetRangeInfo("H6", "I6", "编号：" + investigate.Number, 18.75, 14, true);
            SetRangeInfo("A7", "A9", "", 13.5, 16, false);
            SetRangeInfo("B7", "H9", "地  籍  调  查  表", 13.5, 26, true);
            SetRangeInfo("I7", "I9", "", 13.5, 16, false);
            SetRangeInfo("A10", "I27", "", 13.5, 16, false);
            SetRangeInfo("A28", "I29", GetZone(), 13.5, 14, true);
            SetRangeInfo("A30", "I32", "", 13.5, 16, false);
            SetRangeInfo("A33", "C33", "", 18.75, 16, false);
            SetRangeInfo("D33", "G33", GetDate(), 18.75, 14, true);
            SetRangeInfo("H33", "I33", "", 18.75, 16, false);
            SetRangeInfo("A34", "I36", "", 13.5, 16, false);
            SetRangeInfo("A37", "C37", "", 13.5, 16, false);
            SetRangeInfo("H37", "I37", "", 18.75, 16, false);
            SetRangeInfo("D37", "G37", "四川省国土资源厅统一印制", 18.75, 14, true);
            SetRangeInfo("H37", "I37", "", 18.75, 16, false);
            SetRangeInfo("A38", "I41", "", 13.5, 16, false);
            SetLineType("I41");
            //开始第二页的打印
            WriteHeadRow(2);
        }

        /// <summary>
        /// 得到所在地域
        /// </summary>
        /// <returns></returns>
        private string GetZone()
        {
            if (zone.UpLevelCode.Length > 0)
            {
                Zone tempZone = database.Zone.Get(zone.UpLevelCode);
                Zone tempZone2 = database.Zone.Get(tempZone.UpLevelCode);
                string zoneString = GetSpace(tempZone2.Name) + " 县(市、区) ";
                zoneString += GetSpace(tempZone.Name) + " 街道 ";
                zoneString += GetSpace(zone.Name) + " 号 ";
                return zoneString;
            }
            return "          县(市、区)         街道         号";
        }

        /// <summary>
        /// 得到含空格的地域名称
        /// </summary>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        private string GetSpace(string zoneName)
        {
            string temp = "";
            for (int i = 1; i <= 6 - zoneName.Length; i++)
            {
                temp += "　";
            }
            return temp + zoneName;
        }

        /// <summary>
        /// 得到调查表填写日期
        /// </summary>
        /// <returns></returns>
        private string GetDate()
        {
            string date = string.Format("{0:yyyy-MM-dd}", investigate.WriteDate);
            string writeDate = null;
            writeDate += date.Substring(0, 4) + "年";
            writeDate += date.Substring(5, 2) + "月";
            writeDate += date.Substring(8, 2) + "日";
            return writeDate;
        }

        #endregion

        #region 第二页

        /// <summary>
        /// 第二页信息
        /// </summary>
        private void WriteSecondPage()
        {
            InfoOne();
            InfoTwo();
            InfoThree();
        }

        /// <summary>
        /// 构建Excel第二页信息
        /// </summary>
        private void InfoOne()
        {
            SetRangeInfo("A1", "B1", "初始、变更", 13.5, 12, false);
            SetRangeInfo("C1", "I1", "", 13.5, 12, false);
            SetRangeInfo("A2", "B7", "土地使用者", 13.5, 12, true);
            SetRangeInfo("C2", "D4", "姓名", 13.5, 12, true);
            SetRangeInfo("E2", "I4", buildLandProperty.HouseHolderName, 13.5, 12, false);
            SetRangeInfo("C5", "D7", "性质", 13.5, 12, true);
            SetRangeInfo("E5", "I7", EnumNameAttribute.GetDescription(investigate.LandUsePersonType), 13.5, 12, false);
            SetRangeInfo("A8", "C10", "上级主管部门", 13.5, 12, true);
            SetRangeInfo("D8", "I10", investigate.Administration, 13.5, 12, false);
            SetRangeInfo("A11", "C13", "土 地 坐 落", 13.5, 12, true);
            SetRangeInfo("D11", "I13", zone.FullName, 13.5, 12, false);
            SetRangeInfo("A14", "D15", "法人代表或户主", 13.5, 12, true);
            SetRangeInfo("E14", "I15", "代   理   人", 13.5, 12, true);
            SetRangeInfo("A16", "A17", "姓 名", 13.5, 12, true);
            SetRangeInfo("B16", "C17", "身份证号码", 13.5, 12, true);

            Range range = workSheet.get_Range("D16", "D17");
            range.Merge(0);
            range.NumberFormatLocal = "@";
            range.WrapText = true;
            range.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            range.ColumnWidth = 12;
            range.RowHeight = 13.5;
            range.Font.Size = 12;
            range.Value2 = "电话号码";
            range.Font.Bold = true;

            SetRangeInfo("E16", "E17", "姓 名", 13.5, 12, true);
            SetRangeInfo("F16", "G17", "身份证号码", 13.5, 12, true);
            SetRangeInfo("H16", "I17", "电话号码", 13.5, 12, true);

        }

        /// <summary>
        /// 构建Excel第二页信息
        /// </summary>
        private void InfoTwo()
        {
            SetRangeInfo("A18", "A19", investigate.RepresentName, 13.5, 12, false);

            Range range = workSheet.get_Range("B18", "C19");
            range.Merge(0);
            range.NumberFormatLocal = "@";
            range.WrapText = true;
            range.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            range.ColumnWidth = 12;
            range.RowHeight = 13.5;
            range.Font.Size = 12;
            range.Value2 = investigate.RepresentNumber;

            SetRangeInfo("D18", "D19", investigate.RepresentTelphone, 13.5, 12, false);
            SetRangeInfo("E18", "E19", investigate.AgentName, 13.5, 12, false);
            SetRangeInfo("F18", "G19", investigate.AgentCrdentialNumber, 13.5, 10, false);
            SetRangeInfo("H18", "I19", investigate.AgentTelphone, 13.5, 12, false);
            SetRangeInfo("A20", "C22", "土地权属性质", 13.5, 12, true);
            SetRangeInfo("D20", "I22", EnumNameAttribute.GetDescription(buildLandProperty.OwnRightType), 13.5, 12, false);
            SetRangeInfo("A23", "D25", "预 编 地 籍 号", 13.5, 12, true);
            SetRangeInfo("E23", "I25", "地   籍   号", 13.5, 12, true);
            SetRangeInfo("A26", "D28", investigate.PreCadastralNumber, 13.5, 12, false);
            SetRangeInfo("E26", "I28", investigate.CadastralNumber, 13.5, 12, false);
            SetRangeInfo("A29", "C31", "所 在 图 幅 号", 13.5, 12, true);
            SetRangeInfo("D29", "I31", investigate.ImageNumber, 13.5, 12, false);
            SetRangeInfo("A32", "C34", "宗  地  四  至", 13.5, 12, true);
            SetRangeInfo("D32", "I34", GetNeighbor(), 13.5, 12, false);
        }

        /// <summary>
        /// 构建Excel第二页信息
        /// </summary>
        private void InfoThree()
        {
            SetRangeInfo("A35", "C36", "批 准 用 途", 13.5, 12, true);
            SetRangeInfo("D35", "F36", "实 际 用 途", 13.5, 12, true);
            SetRangeInfo("G35", "I36", "使 用 期 限", 13.5, 12, true);
            SetRangeInfo("A37", "C38", database.LandType.SelectByCode(investigate.ApprovePurpose).Name, 13.5, 12, false);
            SetRangeInfo("D37", "F38", database.LandType.SelectByCode(investigate.RealPurpose).Name, 13.5, 12, false);
            SetRangeInfo("G37", "I38", investigate.UseDate, 13.5, 12, false);
            SetRangeInfo("A39", "C40", "共有使用权情况", 13.5, 12, true);
            SetRangeInfo("D39", "I40", investigate.ShareUsage, 13.5, 12, false);
            SetRangeInfo("A41", "C42", "说明", 13.5, 12, true);
            SetRangeInfo("D41", "I42", investigate.Comment, 13.5, 12, false);

            SetLineType("I42");
            //开始第三页信息  
            WriteHeadRow(3);
        }

        /// <summary>
        /// 得到四至信息
        /// </summary>
        private string GetNeighbor()
        {
            if (buildLandProperty.LandNeighbor != null && buildLandProperty.LandNeighbor.Length > 0)
            {
                string neighbor = null;
                string[] names = ToolNeighbors.GetLandNeighbors(buildLandProperty.LandNeighbor);
                if (names.Length >= 1)
                    neighbor += " 东:" + names[0];
                if (names.Length >= 2)
                    neighbor += " 南:" + names[1];
                if (names.Length >= 3)
                    neighbor += " 西:" + names[2];
                if (names.Length >= 4)
                    neighbor += " 北:" + names[3];

                return neighbor;
            }
            return null;
        }

        #endregion

        #region 第三页

        #region 设置界址标示信息

        /// <summary>
        /// 开始打印第三页信息
        /// </summary>
        private void WriteThirdPage()
        {
            //得到界址点、界址线类别及界址线位置
            tempMark = EnumNameAttribute.GetAttributes(typeof(eLandMarkType));
            tempCategory = EnumNameAttribute.GetAttributes(typeof(eBoundaryLineCategory));
            tempPosition = EnumNameAttribute.GetAttributes(typeof(eBoundaryLinePosition));

            //得到每列宽度及所有列名称
            GetWidth();
            //设置Excel中界址点及界线信息的开头信息
            SetDotAndCoilTitle();
            //设置界址点信息
            SetDotInfo();
            //设置界址线相关信息
            SetCoilInfo();
        }

        /// <summary>
        /// 设置Excel中界址点及界线信息的开头信息
        /// </summary>
        private void SetDotAndCoilTitle()
        {
            int landCategory = landLength + 4;

            SetWidthAndRange("A1", collList[collList.Count - 1] + "2", "界   址   标   示", 13.5, 12, true);
            SetWidthAndRange("A3", "A7", "界址点号", 13.5, 12, true);
            SetWidthAndRange("B3", GetCoordinate(landLength, "3"), "界 标 种 类", 13.5, 12, true);
            SetLandMarkType();
            SetWidthAndRange(GetCoordinate(landLength + 1, "3"), GetCoordinate(landLength + 3, "7"), "界 址   间 距   （m）", 13.5, 12, true);
            SetWidthAndRange(GetCoordinate(landCategory, "3"), GetCoordinate(categoryLength, "3"), "界 址 线 类 别", 13.5, 12, true);
            SetCategoryInfo();
            SetWidthAndRange(GetCoordinate(categoryLength + 1, "3"), GetCoordinate(positionLength, "3"), "界址线位置", 13.5, 12, true);
            SetLinePositionInfo();
            SetWidthAndRange(GetCoordinate(positionLength + 1, "3"), collList[collList.Count - 1] + "7", "备   注", 13.5, 12, true);
        }

        /// <summary>
        /// 设置界址线相关信息
        /// </summary>
        private void SetCoilInfo()
        {
            SetCoilLength();
            int dotIndex = 8;
            foreach (BuildLandBoundaryAddressCoil coil in coils)
            {
                SetCoilInfo(coil, dotIndex);
                if (dotIndex == 8)
                    dotIndex++;
                dotIndex += 2;
            }
            dotIndex++;
            //设置邻宗地信息  传入起始坐标
            SetNeighborTitle(dotIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetCoilInfo(BuildLandBoundaryAddressCoil coil, int dotIndex)
        {

            if (dotIndex == 8 || ((dotIndex - 7) == (dots.Count * 2)))
            {
                SetCoilForThree(coil, dotIndex);
                SetWidthAndRange(collList[collList.Count - 5] + dotIndex.ToString(),
                        collList[collList.Count - 1] + (dotIndex + 2).ToString(), coil.Comment, 13.5, 12, false);
            }
            else
            {
                SetCoilForTwo(coil, dotIndex);
                SetWidthAndRange(collList[collList.Count - 5] + dotIndex.ToString(),
                        collList[collList.Count - 1] + (dotIndex + 1).ToString(), coil.Comment, 13.5, 12, false);
            }
        }

        /// <summary>
        /// 在高度为3格的单元格中设置界址线类别及位置 
        /// </summary>
        /// <param name="coil"></param>
        /// <param name="dotIndex"></param>
        private void SetCoilForThree(BuildLandBoundaryAddressCoil coil, int dotIndex)
        {
            int i = landLength + 4;
            int position = categoryLength + 1;
            for (int j = 0; j < tempCategory.Length; j++)
            {
                if (tempCategory[j].Description == EnumNameAttribute.GetDescription(coil.CoilType))
                {
                    SetWidthAndRange(GetCoordinate(i, dotIndex.ToString()),
                        GetCoordinate(i, (dotIndex + 2).ToString()), "√", 13.5, 12, false);
                }
                else
                    SetWidthAndRange(GetCoordinate(i, dotIndex.ToString()),
                        GetCoordinate(i, (dotIndex + 2).ToString()), "", 13.5, 12, false);
                i += 1;
            }
            for (int j = 0; j < tempPosition.Length; j++)
            {
                Console.WriteLine(GetCoordinate(position, (dotIndex + 2).ToString()));
                if (tempPosition[j].Description == EnumNameAttribute.GetDescription(coil.Position))
                {
                    SetWidthAndRange(GetCoordinate(position, dotIndex.ToString()),
                        GetCoordinate(position, (dotIndex + 2).ToString()), "√", 13.5, 12, false);
                }
                else
                    SetWidthAndRange(GetCoordinate(position, dotIndex.ToString()),
                        GetCoordinate(position, (dotIndex + 2).ToString()), "", 13.5, 12, false);
                position++;
            }
        }

        /// <summary>
        /// 在高度为2格的单元格中设置界址线类别及位置 
        /// </summary>
        /// <param name="coil"></param>
        /// <param name="dotIndex"></param>
        private void SetCoilForTwo(BuildLandBoundaryAddressCoil coil, int dotIndex)
        {
            int i = landLength + 4;
            int position = categoryLength + 1;
            for (int j = 0; j < tempCategory.Length; j++)
            {
                if (tempCategory[j].Description == EnumNameAttribute.GetDescription(coil.CoilType))
                {
                    SetWidthAndRange(GetCoordinate(i, dotIndex.ToString()),
                        GetCoordinate(i, (dotIndex + 1).ToString()), "√", 13.5, 12, false);
                }
                else
                    SetWidthAndRange(GetCoordinate(i, dotIndex.ToString()),
                        GetCoordinate(i, (dotIndex + 1).ToString()), "", 13.5, 12, false);
                i += 1;
            }
            for (int j = 0; j < tempPosition.Length; j++)
            {
                if (tempPosition[j].Description == EnumNameAttribute.GetDescription(coil.Position))
                {
                    SetWidthAndRange(GetCoordinate(position, dotIndex.ToString()),
                        GetCoordinate(position, (dotIndex + 1).ToString()), "√", 13.5, 12, false);
                }
                else
                    SetWidthAndRange(GetCoordinate(position, dotIndex.ToString()),
                        GetCoordinate(position, (dotIndex + 1).ToString()), "", 13.5, 12, false);
                position++;
            }
        }

        /// <summary>
        /// 设置界址间距
        /// </summary>
        private void SetCoilLength()
        {
            int dotIndex = 8;
            foreach (BuildLandBoundaryAddressCoil coil in coils)
            {
                if (dotIndex == 8)
                {
                    SetWidthAndRange(GetCoordinate(landLength + 1, "8"), GetCoordinate(landLength + 3, "10"),
                        coil.CoilLength.ToString(), 14.5, 12, false);
                    dotIndex++;
                }
                else
                    SetCoilLength(coil, dotIndex);
                dotIndex += 2;
            }
        }

        /// <summary>
        /// 设置界址间距
        /// </summary>
        /// <param name="coil"></param>
        /// <param name="dotIndex"></param>
        private void SetCoilLength(BuildLandBoundaryAddressCoil coil, int dotIndex)
        {

            //间距
            if ((dotIndex - 7) != (dots.Count * 2))
                SetWidthAndRange(GetCoordinate(landLength + 1, (dotIndex).ToString()),
                    GetCoordinate(landLength + 3, (dotIndex + 1).ToString()),
                    coil.CoilLength.ToString(), 14.5, 12, false);
            else
                SetWidthAndRange(GetCoordinate(landLength + 1, (dotIndex).ToString()),
                    GetCoordinate(landLength + 3, (dotIndex + 2).ToString()),
                    coil.CoilLength.ToString(), 14.5, 12, false);
        }

        /// <summary>
        /// 设置界址点及界址线信息
        /// </summary>
        private void SetDotInfo()
        {
            int dotIndex = 8;
            foreach (BuildLandBoundaryAddressDot dot in dots)
            {
                SetDotInfo(dot, dotIndex);
                dotIndex += 2;
            }
            if (dots == null || dots.Count < 1)
                return;
            SetDotInfo(dots[0], dotIndex);

        }

        /// <summary>
        /// 设置界址点信息
        /// </summary>
        /// <param name="dot"></param>
        /// <param name="dotIndex"></param>
        private void SetDotInfo(BuildLandBoundaryAddressDot dot, int dotIndex)
        {
            SetWidthAndRange("A" + dotIndex.ToString(), "A" + (dotIndex + 1).ToString(), dot.DotNumber, 13.5, 12, false);
            int i = 1;
            for (int j = 0; j < tempMark.Length; j++)
            {
                if (tempMark[j].Description == EnumNameAttribute.GetDescription(dot.LandmarkType))
                {
                    SetWidthAndRange(GetCoordinate(i, dotIndex.ToString()),
                        GetCoordinate(i, (dotIndex + 1).ToString()), "√", 14.5, 12, false);
                }
                else
                    SetWidthAndRange(GetCoordinate(i, dotIndex.ToString()),
                        GetCoordinate(i, (dotIndex + 1).ToString()), "", 14.5, 12, false);
                i++;
            }
        }

        /// <summary>
        /// 设置界址线位置信息
        /// </summary>
        private void SetLinePositionInfo()
        {
            int j = 0;
            for (int i = categoryLength + 1; i <= positionLength; i++)
            {
                if (tempPosition[j].Description.Length > 3)
                    SetWidthAndRange(collList[i] + "4", collList[i] + "7", tempPosition[j].Description, 13.5, 11, true);
                else
                    SetWidthAndRange(collList[i] + "4", collList[i] + "7", tempPosition[j].Description, 13.5, 12, true);
                j++;
            }
        }

        /// <summary>
        /// 设置界址线类别信息
        /// </summary>
        private void SetCategoryInfo()
        {
            int j = 0;
            for (int i = landLength + 4; i <= categoryLength; i++)
            {
                if (tempCategory[j].Description.Length > 3)
                    SetWidthAndRange(collList[i] + "4", collList[i] + "7", tempCategory[j].Description, 13.5, 11, true);
                else
                    SetWidthAndRange(collList[i] + "4", collList[i] + "7", tempCategory[j].Description, 13.5, 12, true);
                j++;
            }
        }

        /// <summary>
        /// 得到坐标
        /// </summary>
        /// <returns></returns>
        private string GetCoordinate(int length, string index)
        {
            return collList[length] + index;
        }

        /// <summary>
        /// 设置界标种类信息 钢钉、水泥柱……
        /// </summary>
        private void SetLandMarkType()
        {
            int i = 1;
            for (i = 1; i <= landLength; i++)
            {
                SetWidthAndRange(collList[i] + "4", collList[i] + "7", tempMark[i - 1].Description, 13.5, 12, true);
            }
        }

        /// <summary>
        /// 得到第三页中每列的宽度
        /// </summary>
        private void GetWidth()
        {
            collCount = 0;
            collCount += EnumNameAttribute.GetAttributes(typeof(eBoundaryLineCategory)).Length;
            collCount += EnumNameAttribute.GetAttributes(typeof(eBoundaryLinePosition)).Length;
            collCount += EnumNameAttribute.GetAttributes(typeof(eLandMarkType)).Length;
            collCount += 9;
            collWidth = 75.42 / collCount;
            collWidth = ((75.42 - (4.96)) / collCount);
            collWidth = double.Parse(collWidth.ToString().Substring(0, 4));
            if (collWidth < 1.75)
            {
                message = "提示：因为过多的列显示导致列宽度不够！请自行调整";
            }
            //得到所有列名称
            GetAllCollName();
        }

        /// <summary>
        /// 得到一个所有列的集合
        /// </summary>
        private void GetAllCollName()
        {
            collList = new List<string>();
            string[] tempList = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", 
                                    "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", 
                                    "V", "U", "W","X", "Y", "Z"};

            for (int i = 0; i < collCount; i++)
            {
                if (i < 26)
                    collList.Add(tempList[i]);
                if (i >= 26)
                    collList.Add(tempList[0] + tempList[i - 26]);
            }
        }

        #endregion

        #region 设置邻宗地信息

        /// <summary>
        /// 设置邻宗地信息标题
        /// </summary>
        /// <param name="dotIndex"></param>
        private void SetNeighborTitle(int dotIndex)
        {
            int index = dotIndex + 1;
            SetRangeInfo("A" + dotIndex.ToString(), "D" + dotIndex.ToString(), "界 址 线", 13.5, 12, true);
            SetRangeInfo("E" + dotIndex.ToString(), "P" + dotIndex.ToString(), "邻  宗  地", 13.5, 12, true);
            SetRangeInfo("Q" + dotIndex.ToString(), "X" + dotIndex.ToString(), "本 宗 地", 13.5, 12, true);
            SetRangeInfo("Y" + dotIndex.ToString(), "AB" + index.ToString(), "日        期", 13.5, 12, true);


            SetRangeInfo("A" + index.ToString(), "B" + index.ToString(), "起点号", 13.5, 11, true);
            SetRangeInfo("C" + index.ToString(), "D" + index.ToString(), "终点号", 13.5, 11, true);
            SetRangeInfo("E" + index.ToString(), "H" + index.ToString(), "地籍号", 13.5, 11, true);
            SetRangeInfo("I" + index.ToString(), "M" + index.ToString(), "指界人姓名", 13.5, 11, true);
            SetRangeInfo("N" + index.ToString(), "P" + index.ToString(), "签章", 13.5, 11, true);
            SetRangeInfo("Q" + index.ToString(), "U" + index.ToString(), "指界人姓名", 13.5, 11, true);
            SetRangeInfo("V" + index.ToString(), "X" + index.ToString(), "签章", 13.5, 11, true);

            index++;
            if (neighbors.Count > 0)
            {
                SetNeighborInfo(index);
            }
            else
                SetNull(index);
        }

        /// <summary>
        /// 设置邻宗地信息
        /// </summary>
        /// <param name="dotIndex"></param>
        private void SetNeighborInfo(int index)
        {
            foreach (PropertyLineNeighbor neighbor in neighbors)
            {
                foreach (BuildLandBoundaryAddressDot dot in dots)
                {
                    if (neighbor.StartPoint == SubInt(dot.DotNumber))
                        SetRangeInfo("A" + index.ToString(), "B" + index.ToString(), dot.DotNumber, 13.5, 11, false);
                    if (neighbor.EndPoint == SubInt(dot.DotNumber))
                        SetRangeInfo("C" + index.ToString(), "D" + index.ToString(), dot.DotNumber, 13.5, 11, false);
                }
                SetRangeInfo("E" + index.ToString(), "H" + index.ToString(), neighbor.NeighborCadastralNumber, 13.5, 11, false);
                SetRangeInfo("I" + index.ToString(), "M" + index.ToString(), neighbor.NeighborName, 13.5, 11, false);
                SetRangeInfo("N" + index.ToString(), "P" + index.ToString(), "", 13.5, 11, false);
                SetRangeInfo("Q" + index.ToString(), "U" + index.ToString(), neighbor.CurrentName, 13.5, 11, false);
                SetRangeInfo("V" + index.ToString(), "X" + index.ToString(), "", 13.5, 11, false);
                SetRangeInfo("Y" + index.ToString(), "AB" + index.ToString(),
                    string.Format("{0:yyyy-MM-dd}", neighbor.RegistTime), 13.5, 11, false);
                index++;
            }
            SetInvestigatePerson(index);
        }

        /// <summary>
        /// 没有邻宗地信息  则显示一行空白
        /// </summary>
        /// <param name="index"></param>
        private void SetNull(int index)
        {
            SetRangeInfo("A" + index.ToString(), "B" + index.ToString(), "", 13.5, 11, false);
            SetRangeInfo("C" + index.ToString(), "D" + index.ToString(), "", 13.5, 11, false);
            SetRangeInfo("E" + index.ToString(), "H" + index.ToString(), "", 13.5, 11, false);
            SetRangeInfo("I" + index.ToString(), "M" + index.ToString(), "", 13.5, 11, false);
            SetRangeInfo("N" + index.ToString(), "P" + index.ToString(), "", 13.5, 11, false);
            SetRangeInfo("Q" + index.ToString(), "U" + index.ToString(), "", 13.5, 11, false);
            SetRangeInfo("V" + index.ToString(), "X" + index.ToString(), "", 13.5, 11, false);
            SetRangeInfo("Y" + index.ToString(), "AB" + index.ToString(),
                "", 13.5, 11, false);
            index++;
            SetInvestigatePerson(index);
        }

        /// <summary>
        /// 设置指界调查员
        /// </summary>
        /// <param name="index"></param>
        private void SetInvestigatePerson(int index)
        {
            SetRangeInfo("A" + index.ToString(), "E" + index.ToString(), "界址调查员姓名", 13.5, 11, true);
            SetRangeInfo("F" + index.ToString(), "AB" + index.ToString(), investigate.PersonName, 13.5, 11, false);
            SetLineType(index);
        }

        /// <summary>
        /// 设置边框
        /// </summary>
        /// <param name="index"></param>
        private void SetLineType(int index)
        {
            Range range = workSheet.get_Range("A1", collList[collList.Count - 1] + index.ToString());

            SetRangeLineType(range, XlLineStyle.xlContinuous);
        }

        /// <summary>
        /// 得到对应的界址点号
        /// </summary>
        /// <param name="dotNumber"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private int SubInt(string dotNumber)
        {
            if (dotNumber == null || dotNumber.Length < 1)
                return 0;
            char[] array = dotNumber.ToCharArray();
            int index = 0;

            for (int i = 0; i < array.Count(); i++)
            {
                if (Regex.Replace(array[i].ToString(), @"[\d]", "").Length > 0)
                    index = i;
            }
            return int.Parse(dotNumber.Substring(index + 1));
        }

        #endregion

        #endregion

        #endregion
    }
}
