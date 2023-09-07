using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Data;
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using YuLinTu.Library.YltDatabase;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Basic;
using System.IO;

namespace YuLinTu.Library.Office
{
    public abstract class ExportExcelBase : ExcelBase
    {
        public delegate void PostProgressDelegate(int progress);
        public event PostProgressDelegate PostProgressEvent;

        protected void PostProgress(int progress)
        {
            if (PostProgressEvent != null)
                PostProgressEvent(progress);
        }
    }

    public class DotFruit : ExportExcelBase
    {
        #region Ctor

        /// <summary>
        /// 带地域与文件名构造方法
        /// </summary>
        /// <param name="zone">地域</param>
        /// <param name="fileName">文件名</param>
        public DotFruit(IDatabase Database)
        {
            this.database = Database;//数据模型
        }

        #endregion

        #region Fields

        private IDatabase database;

        private BuildLandProperty property;

        private string ownArea;

        private string templatePath;

        private string message;

        /// <summary>
        /// 数据模型
        /// </summary>
        public IDatabase Database
        {
            set { this.database = value; }
        }

        #endregion

        #region Public

        public string BeginPrint(BuildLandProperty temp, string templatePath)
        {
            PostProgress(2);

            this.templatePath = templatePath;

            if (string.IsNullOrEmpty(templatePath) || !File.Exists(templatePath))
                return "目标模板文件不存在！";

            if (temp != null)
                property = temp;
            else
                return "集体建设用地信息错误";

            PostProgress(5);

            HouseLandPropertyCollection housePropertys = database.HouseLandProperty.GetByLandID(property.ID);

            if (housePropertys.Count > 0)
            {
                double area = 0.00;
                foreach (HouseLandProperty item in housePropertys)
                {
                    HouseCollection houses = database.House.GetByPropertyID(item.ID);
                    foreach (House house in houses)
                    {
                        area += house.OwnArea.Value;
                    }
                }
                if (area == 0.00)
                    ownArea = "0.00";
            }
            else
                ownArea = "0.00";

            PostProgress(10);

            Write();//写数据

            return message;
        }

        public override void Write()
        {
            try
            {
                OpenExcelFile();
                PostProgress(20);
                WriteHeadRow(1);//写表头
                PostProgress(60);
                WriteLandInformation();//写数据
                PostProgress(100);

                if(string.IsNullOrEmpty(message))
                    Show();//显示数据
            }
            catch (System.Exception e)
            {
                message = e.Message.ToString();
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
            
            WriteSecondHead();//写第二行表头   
            PostProgress(30);
            WriteThreeHead();//第三行数据
            PostProgress(40);

        }

        /// <summary>
        /// 写第一行表头
        /// </summary>
        private void WriteFirstHead(string count, string number)
        {
            Range range = workSheet.get_Range("A1", "H2");
            range.Merge(0);
            range.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            range.RowHeight = 40;
            range.Font.Size = 26;
            range.Value2 = "界 址 点 成 果 表";
            PostProgress(68);

            Range range1 = workSheet.get_Range("I1", "I1");
            range1.Merge(0);
            range1.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            range1.RowHeight = 20;
            range1.Font.Bold = false;
            range1.Font.Size = 11;
            range1.Value2 = "第 " + number + " 页";
            PostProgress(72);

            Range range2 = workSheet.get_Range("I2", "I2");
            range2.Merge(0);
            range2.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            range2.RowHeight = 20;
            range2.Font.Bold = false;
            range2.Font.Size = 11;
            range2.Value2 = "共 " + count + " 页";
            PostProgress(75);
        }

        /// <summary>
        /// 写第二行表头
        /// </summary>
        private void WriteSecondHead()
        {
            Range range = workSheet.get_Range("A3", "I3");
            range.Merge(0);
            range.VerticalAlignment = XlVAlign.xlVAlignTop;
            range.RowHeight = 30;
            range.Font.Bold = false;
            range.Font.Size = 16;
            range.Value2 = "宗 地 号 " + property.LandNumber;
            PostProgress(21);

            Range range1 = workSheet.get_Range("A4", "I4");
            range1.Merge(0);
            range1.VerticalAlignment = XlVAlign.xlVAlignTop;
            range1.RowHeight = 30;
            range1.Font.Bold = false;
            range1.Font.Size = 16;
            range1.Value2 = "宗 地 名 " + property.HouseHolderName;
            PostProgress(23);

            Range range2 = workSheet.get_Range("A5", "I5");
            range2.Merge(0);
            range2.VerticalAlignment = XlVAlign.xlVAlignTop;
            range2.RowHeight = 30;
            range2.Font.Bold = false;
            range2.Font.Size = 16;
            range2.Value2 = "宗 地 面 积(平方米) " + property.AwareArea;
            PostProgress(25);

            Range range3 = workSheet.get_Range("A6", "I6");
            range3.Merge(0);
            range3.VerticalAlignment = XlVAlign.xlVAlignTop;
            range3.RowHeight = 30;
            range3.Font.Bold = false;
            range3.Font.Size = 16;
            range3.Value2 = "建 筑 占 地(平方米) " + ownArea;
            PostProgress(26);

            Range range4 = workSheet.get_Range("A7", "I7");
            range4.Merge(0);
            range4.VerticalAlignment = XlVAlign.xlVAlignTop;
            range4.RowHeight = 10;
            range4.Font.Bold = false;
            range4.Font.Size = 5;
            range4.Value2 = "";
            PostProgress(27);

            Range range5 = workSheet.get_Range("A8", "I8");
            range5.Merge(0);
            range5.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            range5.RowHeight = 30;
            range5.Font.Size = 16;
            range5.Font.Bold = false;
            range5.Value2 = "界 址 点 坐 标";
            PostProgress(28);
        }

        /// <summary>
        /// 第三行数据
        /// </summary>
        private void WriteThreeHead()
        {
            Range range3 = workSheet.get_Range("C10", "E10");
            range3 = SetRangeInfo(range3, 10, 11);
            range3.Value2 = "x(m)";
            PostProgress(33);

            Range range4 = workSheet.get_Range("F10", "H10");
            range4 = SetRangeInfo(range4, 10, 11);
            range4.Value2 = "y(m)";
            PostProgress(35);

            Range range2 = workSheet.get_Range("C9", "H9");
            range2 = SetRangeInfo(range2, 10, 13);
            range2.Value2 = "坐               标";
            PostProgress(37);

            Range range = workSheet.get_Range("A9", "A10");
            range = SetRangeInfo(range, 20, 14);
            range.Value2 = "序 号";
            PostProgress(38);

            Range range1 = workSheet.get_Range("B9", "B10");
            range1 = SetRangeInfo(range1, 20, 14);
            range1.Value2 = "点 号";
            PostProgress(39);

            Range range5 = workSheet.get_Range("I9", "I10");
            range5 = SetRangeInfo(range5, 20, 14);
            range5.Value2 = "边 长";


        }

        private Range SetRangeInfo(Range range, double highe, double FontSize)
        {
            range.Merge(0);
            range.HorizontalAlignment = XlVAlign.xlVAlignCenter;
            range.RowHeight = highe;
            range.Font.Bold = false;
            range.Font.Size = FontSize;
            return range;
        }

        /// <summary>
        /// 写Excel表中数据
        /// </summary>
        private void WriteLandInformation()
        {
            BuildLandBoundaryAddressDotCollection dots = database.BuildLandBoundaryAddressDot.GetByLandID(property.ID);
            BuildLandBoundaryAddressCoilCollection coils = database.BuildLandBoundaryAddressCoil.GetByLandID(property.ID);
            dots = ToolSort.SortList(dots) as BuildLandBoundaryAddressDotCollection;
            coils = ToolSort.SortList(coils) as BuildLandBoundaryAddressCoilCollection;
            PostProgress(65);
            WriteInfo(dots, coils);

        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="index"></param>
        private void WriteInfo(BuildLandBoundaryAddressDotCollection dots, BuildLandBoundaryAddressCoilCollection coils)
        {
            WriteFirstHead(GetNumber(dots.Count), "1");
            try
            {
                int i = 11, j = 0, index = 1;

                PostProgress(80);
                foreach (BuildLandBoundaryAddressDot dot in dots)
                {
                    if (j % 20 == 0 && j != 0)
                    {
                        WriteInfo(dots[j].DotNumber, coils[j].CoilLength.ToString(), dots[j].XCoordinate.Value,
                            dots[j].YCoordinate.Value, coils.Count, i, j, false);
                        WriteInfo(i+2);
                        index++;
                        WriteHeadRow(index);
                        WriteFirstHead(GetNumber(dots.Count), index.ToString());
                        i = 11;
                    }

                    WriteInfo(dot.DotNumber, coils[j].CoilLength.ToString(), dot.XCoordinate.Value, 
                        dot.YCoordinate.Value, coils.Count, i, j,true);
                    i += 2;
                    j++;
                }
                WriteFinality(i, dots);
                i += 2;

                if (dots.Count > 20)
                {
                    int tempcount = dots.Count;
                    while (tempcount > 0)
                    {
                        tempcount -= 20;
                    }

                    //补长
                    i = WriteNull(tempcount + 20, i, j);
                    i += 2;
                    WriteInfo(i);
                }
            }
            catch (System.Exception e)
            {
                if (e is TaskStopException)
                    throw e;
            }
        }

        private int WriteNull(int count,int i,int j)
        {
            for (int ji = 0; ji < 19 - count; ji++)
            {
                WriteInfo("", null, null, null, j + 2, i, j,true);
                i += 2;
                j++;
            }
            return i;
        }

        private void WriteInfo(int i)
        {
            Range range = workSheet.get_Range("A" + i.ToString(), "B" + (i + 1).ToString());
            range = SetRangeInfo(range, 15, 11);
            range.Value2 = "制表：" + ToolConfiguration.GetAppSettingValue(ConstStringManage.DOTDRAWPERSON);

            Range range1 = workSheet.get_Range("D" + i.ToString(), "E" + (i + 1).ToString());
            range1 = SetRangeInfo(range1, 15, 11);
            range1.Value2 = "审校：" + ToolConfiguration.GetAppSettingValue(ConstStringManage.DOTDRAWPERSON);

            Range range2 = workSheet.get_Range("H" + i.ToString(), "I" + (i + 1).ToString());
            range2 = SetRangeInfo(range2, 15, 11);
            range2.Value2 = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日";
            PostProgress(95);

            Range range4 = workSheet.get_Range("C" + i.ToString(), "C" + (i + 1).ToString());
            range4 = SetRangeInfo(range4, 15, 11);
            range4.Value2 = "";

            Range range5 = workSheet.get_Range("F" + i.ToString(), "G" + (i + 1).ToString());
            range5 = SetRangeInfo(range5, 15, 11);
            range5.Value2 = "";
            SetLineType("I" + (i +1));
        }

        private void WriteInfo(string dotnumber, string coilLength, double? xCoordinate, double? yCoordinate, int coilcount, int i, int j,bool writeLength)
        {
            Range range = workSheet.get_Range("A" + i.ToString(), "A" + (i + 1).ToString());
            range = SetRangeInfo(range, 15, 11);
            range.Value2 = SubString(dotnumber);

            Range range1 = workSheet.get_Range("B" + i.ToString(), "B" + (i + 1).ToString());
            range1 = SetRangeInfo(range1, 15, 11);
            range1.Value2 = dotnumber;

            if (writeLength)
            {
                Range range2 = workSheet.get_Range("I" + (i + 1).ToString(), "I" + (i + 2).ToString());
                range2 = SetRangeInfo(range2, 15, 11);
                if (j <= coilcount - 1)
                    range2.Value2 = coilLength;
                else
                    range2.Value2 = "";
            }

            Range range3 = workSheet.get_Range("C" + i.ToString(), "E" + (i + 1).ToString());
            range3 = SetRangeInfo(range3, 15, 11);
            if (xCoordinate == null || xCoordinate.Value == 0.0)
                range3.Value2 = "-";
            else
                range3.Value2 = xCoordinate;

            Range range4 = workSheet.get_Range("F" + i.ToString(), "H" + (i + 1).ToString());
            range4 = SetRangeInfo(range4, 15, 11);
            if (yCoordinate == null || yCoordinate == 0.0)
                range4.Value2 = "-";
            else
                range4.Value2 = yCoordinate;
        }

        /// <summary>
        /// 写末尾数据
        /// </summary>
        /// <param name="i"></param>
        private void WriteFinality(int i, BuildLandBoundaryAddressDotCollection dots)
        {
            Range range5 = workSheet.get_Range("A" + i.ToString(), "A" + (i + 1).ToString());
            range5 = SetRangeInfo(range5, 15, 11);
            range5.Value2 = SubString(dots[0].DotNumber);

            Range range6 = workSheet.get_Range("B" + i.ToString(), "B" + (i + 1).ToString());
            range6 = SetRangeInfo(range6, 15, 11);
            range6.Value2 = dots[0].DotNumber;

            Range range7 = workSheet.get_Range("C" + i.ToString(), "E" + (i + 1).ToString());
            range7 = SetRangeInfo(range7, 15, 11);
            if (dots[0].XCoordinate == null || dots[0].XCoordinate.Value == 0.0)
                range7.Value2 = "-";
            else
                range7.Value2 = dots[0].XCoordinate.Value;

            Range range8 = workSheet.get_Range("F" + i.ToString(), "H" + (i + 1).ToString());
            range8 = SetRangeInfo(range8, 15, 11);
            if (dots[0].YCoordinate == null || dots[0].YCoordinate.Value == 0.0)
                range8.Value2 = "-";
            else
                range8.Value2 = dots[0].YCoordinate.Value;
        }

        /// <summary>
        /// 得到对应的界址点号
        /// </summary>
        /// <param name="dotNumber"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private string SubString(string dotNumber)
        {
            if (string.IsNullOrEmpty(dotNumber))
                return "";
            char[] array = dotNumber.ToCharArray();
            int index = 0;

            for (int i = 0; i < array.Count(); i++)
            {
                if (Regex.Replace(array[i].ToString(), @"[\d]", "").Length > 0)
                    index = i;
            }

            return dotNumber.Substring(index + 1);
        }

        private string GetNumber(int dotCount)
        {
            double number = (dotCount / 20) + 1;
            char[] array = number.ToString().ToCharArray();
            for (int i = 0; i < array.Count(); i++)
            {
                if (array[i].ToString() == ".")
                    return number.ToString().Substring(0, i + 1);
            }
            return number.ToString();
        }

        public override void Read()
        {

        }

        /// <summary>
        /// 设置边框
        /// </summary>
        private void SetLineType(string end)
        {
            Range range = workSheet.get_Range("A1", end);

            SetRangeLineType(range, XlLineStyle.xlContinuous);
        }

        #endregion
    }
}
