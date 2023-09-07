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
    /// 导出二轮台用户确认表
    /// </summary>
    [Serializable]
    public class ExportSecondLandFamilyExcel : ExportExcelBase
    {
        #region Fields

        private int index;//下标
        private ToolProgress toolProgress;    //进度条
        private List<Person> persons;
        private SecondTableExportDefine secondTableDefine;

        #endregion

        #region Propertys

        /// <summary>
        /// 二轮台账导出表格配置实体属性
        /// </summary>
        public SecondTableExportDefine SecondTableDefine
        {
            get { return secondTableDefine; }
            set { secondTableDefine = value; }
        }

        /// <summary>
        /// 当前行政地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 当前承包方
        /// </summary>
        public VirtualPerson VP { get; set; }

        public List<SecondTableLand> LandCollection
        {
            get;
            set;
        }

        /// <summary>
        /// 家庭成员集合（共有人）
        /// </summary>
        public List<Person> Persons
        {
            get { return persons; }
            set { persons = value; }
        }

        /// <summary>
        /// 文件保存路径
        /// </summary>
        public string SaveFilePath { get; set; }

        public string SavePath { get; set; }

        /// <summary>
        /// 模板文件
        /// </summary>
        public string TemplateFileName { get; set; }

        /// <summary>
        /// 自定义地块导出
        /// </summary>
        public FamilyOutputDefine FamilyDefine { get; set; }

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
        public ExportSecondLandFamilyExcel()
        {
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
        /// 从数据库直接导出Excel
        /// </summary>
        public void BeginToVirtualPerson()
        {
            if (!File.Exists(TemplateFileName))
            {
                PostErrorInfo("模板路径不存在！");
                return;
            }
            string fileName = Path.Combine(SaveFilePath, (VP.Name == "" ? VP.FamilyNumber : VP.Name) + "-农村土地承包经营权单户摸底调查公示确认表.xls");
            this.SavePath = fileName;
            InitizeValue();
            Write();//写数据
        }

        private void InitizeValue()
        {
            index = 1;
        }

        public override void Read()
        {
        }

        public override void Write()
        {
            try
            {
                PostProgress(5);
                OpenExcelFile(TemplateFileName);
                PostProgress(15);
                BeginWrite();
                SaveAs(SavePath);
                Dispose();
            }
            catch (System.Exception e)
            {
                PostErrorInfo(e.Message.ToString());
                Dispose();
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public void OpenExcelFile(string templateFile)
        {
            Open(templateFile);
        }

        private bool SetValue()
        {
            index = 1;
            return true;
        }

        #endregion

        #region 开始生成Excel

        private bool BeginWrite()
        {
            if (VP == null)
                return false;
            WriteTitle();

            #region 打印信息

            try
            {
                int maxrow = LandCollection.Count > persons.Count ? LandCollection.Count : persons.Count;
                maxrow = maxrow > LandCollection.Count ? maxrow : LandCollection.Count;
                if (maxrow > 20)
                {
                    for (int i = 1; i <= (maxrow - 20); i++)
                    {
                        InsertRowCell(index + i);
                    }
                }
                WriteInfomation(maxrow, LandCollection, persons);
                LandCollection = null;
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
            SetRange("A" + 6, "A" + index, VP.FamilyNumber);
            SetRange("B" + 6, "B" + index, VP.Name);
            SetRange("C" + 6, "C" + index, persons.Count.ToString());
            SetRange("I" + 6, "I" + index, VP != null ? VP.PersonCount : "");
            SetRange("J" + 6, "J" + index, VP != null && !string.IsNullOrEmpty(VP.TotalTableArea.ToString()) ? ToolMath.SetNumbericFormat(VP.TotalTableArea.ToString(), 2) : "");
        }

        /// <summary>
        /// 书写当前地域信息
        /// </summary> 
        private void WriteInformation(Person person, int index, SecondTableLand tableLand)
        {
            string landTypeName = string.Empty;
            if (tableLand != null)
            {
                // LandType type = DB.LandType.SelectByCode(tableLand.LandCode);
                string landNumber = ContractLand.GetLandNumber(tableLand.CadastralNumber);
                if (landNumber.Length > YuLinTu.Library.Business.AgricultureSetting.AgricultureLandNumberMedian)
                {
                    landNumber = landNumber.Substring(YuLinTu.Library.Business.AgricultureSetting.AgricultureLandNumberMedian);
                }
                landTypeName = tableLand == null ? "" : tableLand.Name;
                //tableLand = null;
                SetRange("K" + index, "K" + index, tableLand.TableArea.HasValue ? ToolMath.SetNumbericFormat(tableLand.TableArea.Value.ToString(), 2) : " ");
                SetRange("L" + index, "L" + index, tableLand.LandName);
                SetRange("M" + index, "M" + index, tableLand.Name);
                SetRange("S" + index, "S" + index, tableLand.Comment);

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
                SetRange("F" + index, "F" + index, person.ICN);
                SetRange("G" + index, "G" + index, person.Relationship);
                SetRange("H" + index, "H" + index, person.Comment);
            }
        }

        private void WriteTitle()
        {
            SetRange("A" + index, "S" + index, 27.75, 18, true, DeclareTableName + "区、县（市）农村土地承包经营权摸底调查公示确认表");
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
            SetRange("H" + index, "M" + index, "日期：" + GetDate());
            SetRange("N" + index, "S" + index, "户号：" + this.VP.FamilyNumber);
            if (SecondTableDefine.ShowSecondTableDataNeighborWithDirection)
            {
                SetRange("N5", "N5", "东");
                SetRange("O5", "O5", "南");
                SetRange("P5", "P5", "西");
                SetRange("Q5", "Q5", "北");
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
