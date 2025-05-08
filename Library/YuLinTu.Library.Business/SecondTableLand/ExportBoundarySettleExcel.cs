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
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出二轮台账勘界调查表
    /// </summary>
    public class ExportBoundarySettleExcel : ExportExcelBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportBoundarySettleExcel()
        {
            SaveFilePath = string.Empty;
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
        }

        /// <summary>
        /// 进度提示
        /// </summary>    
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            PostProgress(progress, info);
        }

        #endregion

        #region Fields

        private List<SecondTableLandFamily> secondTableLandFamily = new List<SecondTableLandFamily>();
        private ToolProgress toolProgress;    //进度条
        private string zoneCode;
        private string templatePath;
        private bool result = true;
        private int index;//下标
        private Zone currentZone;

        //合计数据
        private int personCount;//人合计
        private double contractorCount;//承包人口
        private int landCount;//地块合计
        private double onlyTableAreaCount;//单个实测面积
        private double totalTebleArea;//总台账面积
        private SecondTableExportDefine secondTableDefine;

        #endregion

        #region Properties

        /// <summary>
        /// 二轮台账导出表格配置实体属性
        /// </summary>
        public SecondTableExportDefine SecondTableDefine
        {
            get { return secondTableDefine; }
            set { secondTableDefine = value; }
        }

        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set { currentZone = value; }
        }

        /// <summary>
        /// 模板文件路径
        /// </summary>
        public string TemplateFile { get; set; }

        /// <summary>
        /// 二轮台账调查表实体
        /// </summary>
        public List<SecondTableLandFamily> SecondTableLandFamilyList
        {
            get { return secondTableLandFamily; }
            set { secondTableLandFamily = value; }
        }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 表标题
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext DbContext { get; set; }

        #endregion

        #region Methods

        #region 开始生成Excel之前的一系列操作

        /// <summary>
        /// 从数据库直接导出Excel
        /// </summary>
        /// <param name="zoneCode">行政地域编码</param>
        /// <param name="templatePath">目标文件路径</param>
        public bool BeginToBoundarySettle(string zoneCode, string templatePath)
        {
            result = true;
            PostProgress(1);
            if (!File.Exists(TemplateFile))
            {
                result = false;
                PostErrorInfo("模板路径不存在！");
            }
            if (string.IsNullOrEmpty(zoneCode))
            {
                result = false;
                PostErrorInfo("地域信息无效。");
            }
            this.zoneCode = zoneCode;
            this.templatePath = templatePath;
            InitizeValue();
            Write();    //写数据
            return result;
        }

        #region Override

        /// <summary>
        /// 读
        /// </summary>
        public override void Read()
        {
        }

        /// <summary>
        /// 写
        /// </summary>
        public override void Write()
        {
            try
            {
                PostProgress(5);
                OpenExcelFile();
                PostProgress(15);
                BeginWrite();
                if (!string.IsNullOrEmpty(SaveFilePath))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(SaveFilePath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(SaveFilePath));
                }
                else
                {
                    string fileName = UnitName + "户籍信息表.xls";
                    SaveFilePath = Path.Combine(AgricultureSetting.SystemDefaultDirectory, fileName);
                }
                if (File.Exists(SaveFilePath))
                {
                    File.SetAttributes(SaveFilePath, FileAttributes.Normal);
                    File.Delete(SaveFilePath);
                }
                toolProgress.DynamicProgress();
                SaveAs(SaveFilePath);
                PostProgress(100);
            }
            catch (System.Exception ex)
            {
                result = false;
                PostErrorInfo(ex.Message.ToString());
                Dispose();
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitizeValue()
        {
            index = 1;
            personCount = 0;//人合计
            landCount = 0;//地块合计
            onlyTableAreaCount = 0.0;//单个确权面积
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        private void OpenExcelFile()
        {
            Open(templatePath);  //打开模板文件
        }

        #endregion

        #endregion

        #region 开始生成Excel

        /// <summary>
        /// 开始向Excel表中写入数据
        /// </summary>
        private bool BeginWrite()
        {
            int number = 1;
            WriteTitle();    //写标题
            int length = 0;
            double height = 17.25;
            int currentIndex = 0;

            #region 打印信息

            try
            {
                toolProgress.InitializationPercent(SecondTableLandFamilyList.Count, 90);
                bool exportData = SecondTableDefine.ExportBoundarySettleTable;
                //string value = ToolConfiguration.GetSpecialAppSettingValue("ExportSurveyTalbeDoNotWriteAwareConent", "true");
                //Boolean.TryParse(value, out exportData);
                foreach (var item in SecondTableLandFamilyList)
                {
                    personCount += (item.Persons == null || item.Persons.Count < 1) ? 0 : item.Persons.Count;
                    landCount += (item.LandCollection == null || item.LandCollection.Count < 1) ? 0 : item.LandCollection.Count;
                    length = item.Persons.Count > item.LandCollection.Count ? item.Persons.Count : item.LandCollection.Count;

                    #region 打印户信息

                    //打印户信息
                    if (!string.IsNullOrEmpty(item.CurrentFamily.FamilyNumber))
                    {
                        Int32.TryParse(item.CurrentFamily.FamilyNumber, out number);
                    }
                    SetRange("A" + index, "A" + (index + length - 1), height, 11, false, number.ToString());
                    SetRange("B" + index, "B" + (index + length - 1), height, 11, false, item.CurrentFamily.Name);
                    SetRange("C" + index, "C" + (index + length - 1), height, 11, false, item.Persons.Count.ToString());

                    #endregion

                    #region 只打印一次的信息

                    SetRange("J" + index, "J" + (index + length - 1), height, 11, false, item.CurrentFamily.PersonCount);
                    double num = 0.0;
                    double.TryParse(item.CurrentFamily.PersonCount, out num);
                    contractorCount += num;

                    double totalArea = 0.0;
                    foreach (var tempLand in item.LandCollection)
                    {
                        totalArea += (double)tempLand.TableArea;
                    }
                    SetRange("K" + index, "K" + (index + length - 1), height, 11, false, item.CurrentFamily != null && !string.IsNullOrEmpty(item.CurrentFamily.TotalTableArea.ToString()) ? ToolMath.SetNumbericFormat(item.CurrentFamily.TotalTableArea.ToString(), 2) : "");
                    SetRange("U" + index, "U" + (index + length - 1), height, 11, false, exportData ? "" : totalArea.ToString());
                    if (!string.IsNullOrEmpty(item.CurrentFamily.TotalArea))
                    {
                        totalTebleArea += ToolMath.RoundNumericFormat(Convert.ToDouble(item.CurrentFamily.TotalArea), 2);
                    }
                    #endregion

                    #region 打印人口信息

                    //打印人口信息
                    currentIndex = index;
                    for (int i = 0; i < length; i++)
                    {
                        Person person = item.Persons.Count > i ? item.Persons[i] : new Person();
                        SetRange("D" + currentIndex, "D" + currentIndex, height, 11, false, person.Name);
                        SetRange("E" + currentIndex, "E" + currentIndex, height, 11, false, GetGender(person.Gender));
                        SetRange("F" + currentIndex, "F" + currentIndex, height, 11, false, person.GetAge() < 1 ? "" : person.GetAge().ToString());
                        SetRange("G" + currentIndex, "G" + currentIndex, height, 11, false, person.ICN);
                        if (person.Name == item.CurrentFamily.Name && person.ICN == item.CurrentFamily.Number)
                        {
                            person.Relationship = "户主";
                        }
                        SetRange("H" + currentIndex, "H" + currentIndex, height, 11, false, person.Relationship);
                        SetRange("I" + currentIndex, "I" + currentIndex, height, 11, false, person.Comment);
                        currentIndex++;
                    }

                    #endregion

                    #region 打印承包经营权与调查信息

                    //地块信息
                    currentIndex = index;
                    for (int i = 0; i < length; i++)
                    {
                        if (!(item.LandCollection.Count > i))
                            continue;
                        SecondTableLand land = item.LandCollection.Count > i ? item.LandCollection[i] : new SecondTableLand() { ID = Guid.Empty };

                        #region 面积

                        SetRange("L" + currentIndex, "L" + currentIndex, height, 11, false, land.TableArea.HasValue && land.TableArea.Value > 0.0 ? ToolMath.SetNumbericFormat(land.TableArea.Value.ToString(), 2) : " ");
                        SetRange("V" + currentIndex, "V" + currentIndex, height, 11, false, exportData ? "" : (land.TableArea.HasValue && land.TableArea.Value > 0.0 ? ToolMath.SetNumbericFormat(land.TableArea.Value.ToString(), 2) : " "));
                        onlyTableAreaCount += land.TableArea.HasValue ? land.TableArea.Value : 0.0;
                        SetRange("M" + currentIndex, "M" + currentIndex, height, 11, false, land.LandName);
                        SetRange("W" + currentIndex, "W" + currentIndex, height, 11, false, exportData ? "" : land.LandName);
                        SetRange("N" + currentIndex, "N" + currentIndex, height, 11, false, land.Name);
                        SetRange("X" + currentIndex, "X" + currentIndex, height, 11, false, exportData ? "" : land.Name);
                        SetRange("S" + currentIndex, "S" + currentIndex, height, 11, false, land.Comment);

                        if (SecondTableDefine.ShowSecondTableDataNeighborWithDirection)
                        {
                            SetRange("O" + currentIndex, "O" + currentIndex, height, 11, false, land.NeighborEast);//四至东
                            SetRange("P" + currentIndex, "P" + currentIndex, height, 11, false, land.NeighborSouth);//四至南
                            SetRange("Q" + currentIndex, "Q" + currentIndex, height, 11, false, land.NeighborWest);//四至西
                            SetRange("R" + currentIndex, "R" + currentIndex, height, 11, false, land.NeighborNorth);//四至北
                        }
                        else
                        {
                            SetRange("O" + currentIndex, "O" + currentIndex, height, 11, false, land.NeighborNorth);//四至上
                            SetRange("P" + currentIndex, "P" + currentIndex, height, 11, false, land.NeighborSouth);//四至下
                            SetRange("Q" + currentIndex, "Q" + currentIndex, height, 11, false, land.NeighborWest);//四至左
                            SetRange("R" + currentIndex, "R" + currentIndex, height, 11, false, land.NeighborEast);//四至右
                        }
                        SetRange("Y" + currentIndex, "Y" + currentIndex, height, 11, false, exportData ? "" : land.NeighborWest);//四至西
                        SetRange("Z" + currentIndex, "Z" + currentIndex, height, 11, false, exportData ? "" : land.NeighborSouth);//四至南
                        SetRange("AA" + currentIndex, "AA" + currentIndex, height, 11, false, exportData ? "" : land.NeighborEast);//四至东
                        SetRange("AB" + currentIndex, "AB" + currentIndex, height, 11, false, exportData ? "" : land.NeighborNorth);//四至北
                        //SetRange("AH" + index, "AH" + (index + length - 1), height, 11, false, item.CurrentFamily.Telephone);
                        SetRange("AH" + index, "AH" + index, height, 11, false, item.CurrentFamily.Telephone);
                        if (!string.IsNullOrEmpty(land.Comment))
                        {
                            SetRange("AI" + currentIndex, "AI" + currentIndex, height, 11, false, exportData ? "" : land.Comment);
                        }

                        #endregion

                        currentIndex++;
                    }

                    #endregion

                    index += length;
                    number++;
                    toolProgress.DynamicProgress();
                }
            }
            catch (Exception ex)
            {
                return PostErrorInfo("生成Excel时出现错误：" + ex.Message.ToString());
            }

            #endregion

            WriteCount();  //合计数据
            SetLineType("A3", "AI" + index);
            return true;
        }

        /// <summary>
        /// 写标题
        /// </summary>
        private void WriteTitle()
        {
            SetRange("A" + index, "AI" + index, 27.75, 18, true, TableName + "区、县（市）农村土地承包经营权勘界确权调查表");
            index++;
            if (currentZone != null && currentZone.FullCode.Length > 0)
            {
                string zoneStr = "单位：" + UnitName;// +"乡、镇(社区服务中心）";
                //zoneStr += GetZoneName(currentZone.FullCode, eZoneLevel.Village) + " 村 ";
                //zoneStr += GetZoneName(currentZone.FullCode, eZoneLevel.Group) + " 组 ";
                SetRange("A" + index, "H" + index, 22.5, 14, false, zoneStr);
            }
            else
            {
                SetRange("A" + index, "H" + index, 22.5, 14, false, "单位:         乡、镇(社区服务中心）          村        组");
            }
            //SetRange("N" + index, "Q" + index, 22.5, 12, false, "日期：" + GetDate());
            if (SecondTableDefine.ShowSecondTableDataNeighborWithDirection)
            {
                SetRange("O5", "O5", "东");
                SetRange("P5", "P5", "南");
                SetRange("Q5", "Q5", "西");
                SetRange("R5", "R5", "北");
            }
            index += 4;
        }

        /// <summary>
        /// 合计数据
        /// </summary>
        private void WriteCount()
        {
            SetRange("A" + index, "A" + index, 28.5, 11, false, "合计");
            SetRange("B" + index, "B" + index, 28.5, 11, false, @"\");
            SetRange("C" + index, "C" + index, 28.5, 11, false, personCount.ToString());
            SetRange("D" + index, "D" + index, 28.5, 11, false, @"\");
            SetRange("E" + index, "E" + index, 28.5, 11, false, @"\");
            SetRange("F" + index, "F" + index, 28.5, 11, false, @"\");
            SetRange("G" + index, "G" + index, 28.5, 11, false, @"\");
            SetRange("I" + index, "I" + index, 28.5, 11, false, @"\");
            SetRange("J" + index, "J" + index, 28.5, 11, false, contractorCount.ToString());
            SetRange("K" + index, "K" + index, 28.5, 11, false, ToolMath.SetNumbericFormat(totalTebleArea > 0.0 ? totalTebleArea.ToString() : onlyTableAreaCount.ToString(), 2));
            SetRange("L" + index, "L" + index, 28.5, 11, false, ToolMath.SetNumbericFormat(onlyTableAreaCount.ToString(), 2));
            SetRange("M" + index, "M" + index, 28.5, 11, false, @"\");
            SetRange("N" + index, "N" + index, 28.5, 11, false, @"\");
            SetRange("O" + index, "O" + index, 28.5, 11, false, @"\");
            SetRange("P" + index, "P" + index, 28.5, 11, false, @"\");
            SetRange("Q" + index, "Q" + index, 28.5, 11, false, @"\");
            SetRange("R" + index, "R" + index, 28.5, 11, false, @"\");
            SetRange("S" + index, "S" + index, 28.5, 11, false, @"\");
            SetRange("U" + index, "U" + index, 28.5, 11, false, @"\");
            SetRange("V" + index, "V" + index, 28.5, 11, false, @"\");
            SetRange("W" + index, "W" + index, 28.5, 11, false, @"\");
            SetRange("X" + index, "X" + index, 28.5, 11, false, @"\");
            SetRange("Y" + index, "Y" + index, 28.5, 11, false, @"\");
            SetRange("Z" + index, "Z" + index, 28.5, 11, false, @"\");
            SetRange("AA" + index, "AA" + index, 28.5, 11, false, @"\");
            SetRange("AB" + index, "AB" + index, 28.5, 11, false, @"\");
            SetRange("AC" + index, "AC" + index, 28.5, 11, false, @"\");
            SetRange("AD" + index, "AD" + index, 28.5, 11, false, @"\");
            SetRange("AE" + index, "AE" + index, 28.5, 11, false, @"\");
            SetRange("AF" + index, "AF" + index, 28.5, 11, false, @"\");
            SetRange("AG" + index, "AG" + index, 28.5, 11, false, @"\");
            SetRange("AH" + index, "AH" + index, 28.5, 11, false, @"\");
            SetRange("AI" + index, "AI" + index, 28.5, 11, false, @"\");
            //index++;
            //SetRange("A" + index, "P" + index, 41.25, 11, false, "注：根据确权方案，征求村组意见：是否公示到地块，还是到户的实测面积、确权面积和机动地面积，"+
            //"但三个面积均要公示\r\n。到地块的实测面积必须要。");
        }

        /// <summary>
        /// 获取性别
        /// </summary>
        /// <param name="gender">性别</param>
        /// <returns></returns>
        private string GetGender(eGender gender)
        {
            switch (gender)
            {
                case eGender.Male:
                    return "男";
                case eGender.Female:
                    return "女";
            }
            return "";
        }
        
        /// <summary>
        /// 配置
        /// </summary>
        public override void GetReplaceMent()
        {
            EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        }

        #endregion

        #endregion
    }
}
