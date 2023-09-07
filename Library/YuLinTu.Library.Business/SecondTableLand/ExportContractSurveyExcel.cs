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
using System.Windows.Forms;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出二轮台账模拟调查表
    /// </summary>
    [Serializable]
    public class ExportContractSurveyExcel : ExportExcelBase
    {
        #region Fields

        //private bool result = true;
        private int index;//下标
        private List<SecondTableLandFamily> secondTableLandFamily;
        private ToolProgress toolProgress;    //进度条

        //合计数据
        private int personCount;//人合计
        private double contractorCount;//承包人口
        private int landCount;//地块合计
        private double onlyTableAreaCount;//单个实测面积
        private double totalTebleArea;//总台账面积
        private SecondTableExportDefine secondTableDefine;

        #endregion 

        #region Propertys

        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get; set; }

        /// <summary>
        /// 当前行政地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 模板文件
        /// </summary>
        public string TemplateFile { get; set; }

        public List<SecondTableLandFamily> SecondTableLandFamilyList
        {
            get { return secondTableLandFamily;}
            set { secondTableLandFamily = value;}
        }

        /// <summary>
        /// 二轮台账导出表格配置实体属性
        /// </summary>
        public SecondTableExportDefine SecondTableDefine
        {
            get { return secondTableDefine; }
            set { secondTableDefine = value; }
        }

        ///// <summary>
        ///// 日期
        ///// </summary>
        //public DateTime? Date { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 摸底调查公示表
        /// </summary>
        public string DeclareTableName { get; set; }

        #endregion

        #region Ctor

        public ExportContractSurveyExcel()
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

        #region Methods

        #region 开始生成Excel之前的一系列操作

        /// <summary>
        /// 从数据导出到Excel
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="templatePath"></param>
        public bool BeginToZone(string zoneCode)
        {
            //result = true;
            PostProgress(1);
            if (!File.Exists(TemplateFile))
            {
                //result = false;
                PostErrorInfo("模板路径不存在！");
            }
            if (string.IsNullOrEmpty(zoneCode))
            {
                //result = false;
                PostErrorInfo("地域信息无效。");
            }
            InitizeValue();
            Write();//写数据
            return true;
        }

        public override void Read()
        {
        }

        /// <summary>
        /// 写数据
        /// </summary>
        public override void Write()
        {
            try
            {
                PostProgress(5);
                OpenExcelFile(TemplateFile);
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
            catch (System.Exception e)
            {
                //result = false;
                PostErrorInfo(e.Message.ToString());
                Dispose();
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        private void OpenExcelFile(string templateFile)
        {
            Open(templateFile);
        }

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
   
        #endregion

        #region 开始生成Excel

        /// <summary>
        /// 开始向excel表中写入数据
        /// </summary>
        /// <returns></returns>
        private bool BeginWrite()
        {
            int number = 1;

            //表的标题
            WriteTitle();

            //写表的内容
            int length = 0;
            double height = 17.25;
            int currentIndex = 0;

            #region 打印信息

            try
            {
                toolProgress.InitializationPercent(secondTableLandFamily.Count, 90);
                foreach (var item in secondTableLandFamily)
                {
                    personCount += (item.Persons == null || item.Persons.Count < 1) ? 0 : item.Persons.Count;
                    landCount += (item.LandCollection == null || item.LandCollection.Count < 1) ? 0 : item.LandCollection.Count;
                    length = item.Persons.Count > item.LandCollection.Count ? item.Persons.Count : item.LandCollection.Count;

                    #region 打印户信息

                    //打印户信息
                    Int32.TryParse(item.CurrentFamily.FamilyNumber, out number);
                    SetRange("A" + index, "A" + (index + length - 1), height, 11, false, number.ToString());
                    SetRange("B" + index, "B" + (index + length - 1), height, 11, false, item.CurrentFamily.Name);
                    SetRange("C" + index, "C" + (index + length - 1), height, 11, false, item.Persons.Count.ToString());

                    #endregion

                    #region 只打印一次的信息
                    VirtualPerson vp = item.CurrentFamily;
                    SetRange("H" + index, "H" + (index + length - 1), height, 11, false, vp != null ? vp.PersonCount : "");
                    SetRange("I" + index, "I" + (index + length - 1), height, 11, false, vp != null && !string.IsNullOrEmpty(vp.TotalTableArea.ToString()) ? ToolMath.SetNumbericFormat(vp.TotalTableArea.ToString(), 2) : "");
                    if (!string.IsNullOrEmpty(vp.TotalArea))
                    {
                        totalTebleArea += ToolMath.SetNumericFormat(Convert.ToDouble(vp.TotalArea), 2, 1);
                    }
                    double num = 0.0;
                    double.TryParse(vp.PersonCount, out num);
                    contractorCount += num;
                    #endregion

                    #region 打印人口信息

                    //打印人口信息
                    currentIndex = index;
                    for (int i = 0; i < length; i++)
                    {
                        Person person = item.Persons.Count > i ? item.Persons[i] : new Person();
                        SetRange("D" + currentIndex, "D" + currentIndex, height, 11, false, person.Name);
                        SetRange("E" + currentIndex, "E" + currentIndex, height, 11, false, GetGender(person.Gender));
                        if (person.Name == item.CurrentFamily.Name && person.ICN == item.CurrentFamily.Number)
                        {
                            person.Relationship = "户主";
                        }
                        SetRange("F" + currentIndex, "F" + currentIndex, height, 11, false, person.Relationship);
                        SetRange("G" + currentIndex, "G" + currentIndex, height, 11, false, person.Comment);//备注
                        currentIndex++;
                    }

                    #endregion

                    #region 打印二轮台账信息
                    //地块信息
                    currentIndex = index;
                    for (int i = 0; i < length; i++)
                    {
                        if (!(item.LandCollection.Count > i))
                            continue;
                        SecondTableLand land = item.LandCollection.Count > i ? item.LandCollection[i] : new SecondTableLand() { ID = Guid.Empty };

                        #region 面积
                        SetRange("J" + currentIndex, "J" + currentIndex, height, 11, false, land.TableArea.HasValue ? ToolMath.SetNumbericFormat(land.TableArea.Value.ToString(), 2) : " ");
                        onlyTableAreaCount += land.TableArea.HasValue ? land.TableArea.Value : 0.0;
                        SetRange("K" + currentIndex, "K" + currentIndex, height, 11, false, land.LandName);
                        SetRange("L" + currentIndex, "L" + currentIndex, height, 11, false, land.Name);

                        if (SecondTableDefine.ShowSecondTableDataNeighborWithDirection)
                        {
                            SetRange("M" + currentIndex, "M" + currentIndex, height, 11, false, land.NeighborEast);//四至东
                            SetRange("N" + currentIndex, "N" + currentIndex, height, 11, false, land.NeighborSouth);//四至南
                            SetRange("O" + currentIndex, "O" + currentIndex, height, 11, false, land.NeighborWest);//四至西
                            SetRange("P" + currentIndex, "P" + currentIndex, height, 11, false, land.NeighborNorth);//四至北
                        }
                        else
                        {
                            SetRange("M" + currentIndex, "M" + currentIndex, height, 11, false, land.NeighborNorth);//四至上
                            SetRange("N" + currentIndex, "N" + currentIndex, height, 11, false, land.NeighborSouth);//四至下
                            SetRange("O" + currentIndex, "O" + currentIndex, height, 11, false, land.NeighborWest);//四至左
                            SetRange("P" + currentIndex, "P" + currentIndex, height, 11, false, land.NeighborEast);//四至右
                        }
                        if (!string.IsNullOrEmpty(land.Comment))
                        {
                            SetRange("Q" + currentIndex, "Q" + currentIndex, height, 11, false, land.Comment);//备注
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

            WriteCount();
            SetLineType("A3", "Q" + index);
            return true;
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
            SetRange("H" + index, "H" + index, 28.5, 11, false, contractorCount.ToString());
            SetRange("I" + index, "I" + index, 28.5, 11, false, ToolMath.SetNumbericFormat(totalTebleArea > 0.0 ? totalTebleArea.ToString() : onlyTableAreaCount.ToString(), 2));
            SetRange("J" + index, "J" + index, 28.5, 11, false, ToolMath.SetNumbericFormat(onlyTableAreaCount.ToString(), 2));
            SetRange("K" + index, "K" + index, 28.5, 11, false, @"\");
            SetRange("L" + index, "L" + index, 28.5, 11, false, @"\");
            SetRange("M" + index, "M" + index, 28.5, 11, false, @"\");
            SetRange("N" + index, "N" + index, 28.5, 11, false, @"\");
            SetRange("O" + index, "O" + index, 28.5, 11, false, @"\");
            SetRange("P" + index, "P" + index, 28.5, 11, false, @"\");
            SetRange("Q" + index, "Q" + index, 28.5, 11, false, @"\");
            //index++;
            //SetRange("A" + index, "P" + index, 41.25, 11, false, "注：根据确权方案，征求村组意见：是否公示到地块，还是到户的实测面积、确权面积和机动地面积，"+
            //"但三个面积均要公示\r\n。到地块的实测面积必须要。");
        }

        /// <summary>
        /// 用户性别
        /// </summary>
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
        /// excel表标题
        /// </summary>
        private void WriteTitle()
        {
            SetRange("A" + index, "S" + index, 27.75, 18, true, DeclareTableName + "区、县（市）农村土地承包经营权摸底调查公示表");
            index++;
            if (CurrentZone != null && CurrentZone.FullCode.Length > 0)
            {
                string zoneStr = "单位：" + UnitName; // "乡、镇(社区服务中心）";
                //zoneStr += GetZoneName(currentZone.FullCode, eZoneLevel.Village) + " 村 ";
                //zoneStr += GetZoneName(currentZone.FullCode, eZoneLevel.Group) + " 组 ";
                SetRange("A" + index, "M" + index, 22.5, 12, false, zoneStr);
            }
            else
                SetRange("A" + index, "M" + index, 22.5, 12, false, "单位:         乡镇(社区服务中心）          村        组");

            SetRange("N" + index, "Q" + index, 22.5, 12, false, "日期：" + GetDate());
            if (SecondTableDefine.ShowSecondTableDataNeighborWithDirection)
            {
                SetRange("M5", "M5", "东");
                SetRange("N5", "N5", "南");
                SetRange("O5", "O5", "西");
                SetRange("P5", "P5", "北");
            }
            index += 4;
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
