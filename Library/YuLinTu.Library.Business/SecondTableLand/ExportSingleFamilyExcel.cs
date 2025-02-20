/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Office;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出二轮台账单户调查表
    /// </summary>
    public class ExportSingleFamilyExcel : ExportExcelBase
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportSingleFamilyExcel()
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

        private Zone currentZone;
        private ToolProgress toolProgress;    //进度条
        //private bool result = true;
        //private string zoneCode;
        //private string templatePath;  //模板文件路径
        private int index;//下标
        private VirtualPerson virtualPerson;
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

        /// <summary>
        /// 地块集合
        /// </summary>
        public List<SecondTableLand> listTableLand { get; set; }

        /// <summary>
        /// 承包方集合
        /// </summary>
        public List<VirtualPerson> listPersons { get; set; }

        #endregion

        #region Methods

        #region 开始生成Excel之前的一系列操作

        /// <summary>
        /// 从数据库直接导出Excel
        /// </summary>
        /// <param name="zoneCode">行政地域编码</param>
        /// <param name="templatePath">目标文件路径</param>
        public bool BeginToSingleFamily(VirtualPerson vp, string savePath)
        {
            PostProgress(1);
            if (!File.Exists(TemplateFile))
            {
                PostErrorInfo("模板路径不存在！");
            }
            if (vp == null)
            {
                PostErrorInfo("承包方信息无效！");
            }
            this.virtualPerson = vp;
            this.SaveFilePath = savePath;
            InitizeValue();
            Write();    //写数据
            return true;
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
                OpenExcelFile();   //打开模板文件
                PostProgress(15);
                BeginWrite();     //开始向Excel中写入数据
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
                SaveAs(SaveFilePath);    //保存Excel
                PostProgress(100);
            }
            catch (System.Exception ex)
            {
                PostErrorInfo(ex.Message.ToString());
                Dispose();
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// 打开模板文件
        /// </summary>
        private void OpenExcelFile()
        {
            Open(TemplateFile);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitizeValue()
        {
            index = 1;
        }

        #endregion

        #endregion

        #region 开始生成Excel

        /// <summary>
        /// 开始向Excel表中写入数据
        /// </summary>
        private bool BeginWrite()
        {
            if (this.virtualPerson == null)
            {
                return false;
            }
            //书写标题
            WriteTitle();

            #region 打印信息

            try
            {
                List<Person> persons = virtualPerson.SharePersonList;
                List<SecondTableLand> lands = listTableLand.FindAll(c => c.OwnerId == virtualPerson.ID);
                if (persons == null)
                {
                    persons = new List<Person>();
                }
                if (lands == null)
                {
                    lands = new List<SecondTableLand>();
                }
                int maxrow = lands.Count > persons.Count ? lands.Count : persons.Count;
                maxrow = maxrow > lands.Count ? maxrow : lands.Count;
                if (maxrow > 20)
                {
                    for (int i = 1; i <= (maxrow - 20); i++)
                    {
                        InsertRowCell(index + i);
                    }
                }
                WriteInfomation(maxrow, lands, persons);
                lands = null;
                persons = null;
            }
            catch (Exception ex)
            {
                return PostErrorInfo("生成Excel时出现错误：" + ex.Message.ToString());
            }

            #endregion

            return true;
        }

        /// <summary>
        /// 写标题
        /// </summary>
        private void WriteTitle()
        {
            SetRange("A" + index, "R" + index, 27.75, 18, true, TableName + "区、县（市）农村土地承包经营权摸底调查公示确认表");
            index++;
            if (CurrentZone != null && CurrentZone.FullCode.Length > 0)
            {
                string zoneStr = "单位：" + UnitName;
                SetRange("A" + index, "G" + index, zoneStr);
            }
            else
            {
                SetRange("A" + index, "G" + index, "单位: 乡镇(社区服务中心） 村   组");
            }
            SetRange("H" + index, "L" + index, "户主电话号码：            ");
            SetRange("M" + index, "R" + index, "农户签字：                ");
            if (SecondTableDefine.ShowSecondTableDataNeighborWithDirection)
            {
                SetRange("N5", "N5", "右");
                SetRange("O5", "O5", "下");
                SetRange("P5", "P5", "左");
                SetRange("Q5", "Q5", "上");
            }
            index += 4;
        }

        /// <summary>
        /// 填写表格信息
        /// </summary>
        /// <param name="maxrow"></param>
        private void WriteInfomation(int maxrow, List<SecondTableLand> secondLands, List<Person> persons)
        {
            double landAreaCount = 0;
            for (int i = 0; i < maxrow; i++)
            {
                Person person = i < persons.Count ? persons[i] : null;
                SecondTableLand tableLand = i < secondLands.Count ? secondLands[i] : null;
                if (tableLand != null)
                {
                    landAreaCount += (tableLand.TableArea == null ? 0 : (double)tableLand.TableArea);
                }
                WriteInformation(person, index, tableLand);
                index++;
            }
            index--;
            SetRange("A" + 6, "A" + index, virtualPerson.FamilyNumber);
            SetRange("B" + 6, "B" + index, virtualPerson.Name);
            SetRange("C" + 6, "C" + index, persons.Count.ToString());
            SetRange("I" + 6, "I" + index, virtualPerson != null ? virtualPerson.PersonCount : "");
            SetRange("J" + 6, "J" + index, virtualPerson != null && !string.IsNullOrEmpty(virtualPerson.TotalTableArea.ToString()) ? ToolMath.SetNumbericFormat(virtualPerson.TotalTableArea.ToString(), 2) : "");
        }

        /// <summary>
        /// 书写当前地域信息
        /// </summary> 
        private void WriteInformation(Person person, int index, SecondTableLand tableLand)
        {
            string landTypeName = string.Empty;
            if (tableLand != null)
            {
                //LandType type = DB.LandType.SelectByCode(tableLand.LandCode);
                string landNumber = ContractLand.GetLandNumber(tableLand.CadastralNumber);
                if (landNumber.Length > YuLinTu.Library.Business.AgricultureSetting.AgricultureLandNumberMedian)
                {
                    landNumber = landNumber.Substring(YuLinTu.Library.Business.AgricultureSetting.AgricultureLandNumberMedian);
                }
                //landTypeName = type == null ? "" : type.Name;
                //type = null;
                SetRange("K" + index, "K" + index, tableLand.TableArea.HasValue ? ToolMath.SetNumbericFormat(tableLand.TableArea.Value.ToString(), 2) : " ");
                SetRange("L" + index, "L" + index, tableLand.LandName);
                SetRange("M" + index, "M" + index, tableLand.Name);
                SetRange("R" + index, "R" + index, tableLand.Comment);
                if (SecondTableDefine.ShowSecondTableDataNeighborWithDirection)
                {
                    SetRange("N" + index, "N" + index, tableLand.NeighborEast);//四至东
                    SetRange("O" + index, "O" + index, tableLand.NeighborSouth);//四至南
                    SetRange("P" + index, "P" + index, tableLand.NeighborWest);//四至西
                    SetRange("Q" + index, "Q" + index, tableLand.NeighborNorth);//四至北
                }
                else
                {
                    SetRange("N" + index, "N" + index, tableLand.NeighborEast);//四至东
                    SetRange("O" + index, "O" + index, tableLand.NeighborSouth);//四至南
                    SetRange("P" + index, "P" + index, tableLand.NeighborWest);//四至西
                    SetRange("Q" + index, "Q" + index, tableLand.NeighborNorth);//四至北
                }
            }
            if (person != null)
            {
                SetRange("D" + index, "D" + index, person.Name);
                SetRange("E" + index, "E" + index, person.Gender == eGender.Male ? "男" : (person.Gender == eGender.Female ? "女" : ""));
                SetRange("F" + index, "F" + index, person.Age);
                SetRange("G" + index, "G" + index, person.ICN);
                SetRange("H" + index, "H" + index, person.Relationship);
            }
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
