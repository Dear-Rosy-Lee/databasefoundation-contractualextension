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
using System.Windows.Forms;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出承包方调查表
    /// </summary>
    [Serializable]
    public class ExportContractorExcel : ExportExcelBase
    {
        #region Fields

        private bool result = true;
        private int index;//索引值
        private int high;//单元格合并数量
        private ToolProgress toolProgress;//进度条
        private int columnIndex;//当前列名
        private int PersonCount;//人合计
        private int packageCount;//土地延包份数
        private int familyCount;//总户数
        //private bool exportByFamilyNumber;//是否根据户号导出

        private double secCordTotolAreaCount;
        private int secCordLandCount;
        private SystemSetDefine systemseting = SystemSetDefine.GetIntence();

        #endregion Fields

        #region Property

        /// <summary>
        /// 承包方集合
        /// </summary>
        public List<VirtualPerson> FamilyList
        { get; set; }

        /// <summary>
        /// 二轮承包方集合
        /// </summary>
        public List<VirtualPerson> TableFamilyList { get; set; }

        /// <summary>
        /// 承包地集合
        /// </summary>
        public List<ContractLand> LandList { get; set; }

        /// <summary>
        /// 当前行政地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 模板文件
        /// </summary>
        public string TemplateFile { get; set; }

        /// <summary>
        /// 承包方导出设置
        /// </summary>
        public FamilyOutputDefine OutputDefine = FamilyOutputDefine.GetIntence();

        /// <summary>
        /// 承包方其它设置
        /// </summary>
        public FamilyOtherDefine OtherDefine = FamilyOtherDefine.GetIntence();

        /// <summary>
        /// 系统设置
        /// </summary>
        public SystemSetDefine SystemDefine = SystemSetDefine.GetIntence();

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool NotShow { get; set; }

        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get; set; }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eVirtualType PersonType { get; set; }

        ///// <summary>
        ///// 日期
        ///// </summary>
        //public DateTime? Date { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 村单位名称
        /// </summary>
        public string UnitVillageName { get; set; }

        /// <summary>
        /// 进度百分比
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// 当前进度百分比
        /// </summary>
        public double CurrentPercent { get; set; }

        /// <summary>
        /// 地域描述
        /// </summary>
        public string ZoneDesc { get; set; }

        #endregion Property

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ExportContractorExcel()
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

        #endregion Ctor

        #region Methods

        #region 开始生成Excel操作

        /// <summary>
        /// 开始导出数据
        /// </summary>
        public bool BeginExcel(string templatePath)
        {
            result = true;
            //PostProgress(1);
            if (!File.Exists(templatePath))
            {
                result = false;
                PostErrorInfo("模板路径不存在！");
            }
            if (CurrentZone == null)
            {
                result = false;
                PostErrorInfo("目标地域不存在！");
            }
            index = 6;
            Write();//写数据
            return result;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
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
                //PostProgress(5);
                Open(TemplateFile);
                //PostProgress(10);
                WriteFamilyInformation();
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
                //toolProgress.DynamicProgress();
                SaveAs(SaveFilePath);
                // PrintView();
                //PostProgress(100);
                GC.Collect();
            }
            catch (System.Exception e)
            {
                result = false;
                PostErrorInfo(e.Message.ToString());
                Dispose();
            }
        }

        #endregion 开始生成Excel操作

        #region 开始往Excel中添加值

        /// <summary>
        /// 开始写数据
        /// </summary>
        private void WriteFamilyInformation()
        {
            //写标题信息
            WriteTitle();
            //开始写内容
            WriteContractorContent();
        }

        /// <summary>
        /// 写现实人口数据
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="ts"></param>
        private void WritePerson(List<Person> ps, List<Person> ts, VirtualPerson vp)
        {
            int pindex = index;
            int age = 0;//家庭成员年龄
            int curIndex = columnIndex;
            VirtualPersonExpand familyExpand = vp.FamilyExpand;
            if (ps == null || ps.Count == 0)
            {
                WriteContractorInformation();
                WriteTableInformation();
                pindex++;
            }
            else
            {
                foreach (Person person in ps)
                {
                    columnIndex = curIndex;
                    if (OutputDefine.NumberNameValue)
                    {
                        columnIndex++;
                        SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.Name.InitalizeFamilyName(SystemDefine.KeepRepeatFlag));//成员姓名
                    }
                    string value = EnumNameAttribute.GetDescription(person.Gender);
                    if (OutputDefine.NumberGenderValue)
                    {
                        columnIndex++;
                        SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, value == "未知" ? "" : value);//成员性别
                    }
                    age = person.GetAge();
                    if (OutputDefine.NumberAgeValue)
                    {
                        columnIndex++;
                        if (age > 0 && age < 120)
                            SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, age.ToString());//成员年龄
                        else
                            SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, "");//成员年龄
                    }
                    if (OutputDefine.NumberCartTypeValue)
                    {
                        columnIndex++;
                        SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, EnumNameAttribute.GetDescription(person.CardType));//成员身份证类型
                    }
                    if (OutputDefine.NumberIcnValue)
                    {
                        columnIndex++;
                        SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.ICN);//成员身份证号
                    }
                    if (OutputDefine.NumberRelatioinValue)
                    {
                        columnIndex++;
                        SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.Relationship);//家庭成员关系
                    }
                    if (OutputDefine.NationValue)
                    {
                        columnIndex++;
                        SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, EnumNameAttribute.GetDescription(person.Nation));//民族
                    }
                    if (OutputDefine.AccountNatureValue)
                    {
                        columnIndex++;
                        SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.AccountNature);//户口性质
                    }
                    if (OutputDefine.IsSharedLandValue)
                    {
                        columnIndex++;
                        SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.IsSharedLand == "否" ? "否" : "是");//是否享有承包地
                    }
                    if (OutputDefine.CommentValue)
                    {
                        columnIndex++;
                        SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.Comment);//备注
                    }
                    if (OutputDefine.CommonOpinion)
                    {
                        columnIndex++;
                        SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.Opinion);//共有人信息修改意见
                    }
                    if (OutputDefine.CencueCommentValue)
                    {
                        columnIndex++;
                        SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.CencueComment);//户籍备注
                    }
                    Person tablePerson = ts.Find(per => per.Name == person.Name);
                    if (tablePerson != null)
                    {
                        WriteTablePerson(tablePerson, pindex);
                        ts.Remove(tablePerson);
                    }
                    else
                    {
                        WriteTableInformation();
                    }
                    pindex++;
                }
            }
            foreach (Person pson in ts)
            {
                columnIndex = curIndex;
                WriteContractorInformation();
                WriteTablePerson(pson, pindex);
                pindex++;
            }
        }

        /// <summary>
        /// 写二轮延包人口数据
        /// </summary>
        /// <param name="person"></param>
        /// <param name="pindex"></param>
        private void WriteTablePerson(Person person, int pindex)
        {
            if (OutputDefine.ExPackageNameValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.Name);//成员姓名
            }
            if (OutputDefine.ExPackageNumberValue)
            {
                columnIndex++;
                int count = 0;
                Int32.TryParse(person.ExtensionPackageNumber, out count);
                packageCount += count;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.ExtensionPackageNumber);//成员延包土地份数
            }
            if (OutputDefine.IsDeadedValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.IsDeaded);//成员已死亡
            }
            if (OutputDefine.LocalMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.LocalMarriedRetreatLand);//出嫁后未退承包地人员
            }
            if (OutputDefine.PeasantsRetreatLandValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.PeasantsRetreatLand);//农转非后未退承包地人员
            }
            if (OutputDefine.ForeignMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.ForeignMarriedRetreatLand);//婚进但在婚出地未退承包地人员
            }
        }

        /// <summary>
        /// 写二轮延包表信息
        /// </summary>
        private void WriteTableInformation()
        {
            if (OutputDefine.ExPackageNameValue)
            {
                columnIndex++;
            }
            if (OutputDefine.ExPackageNumberValue)
            {
                columnIndex++;
            }
            if (OutputDefine.IsDeadedValue)
            {
                columnIndex++;
            }
            if (OutputDefine.LocalMarriedRetreatLandValue)
            {
                columnIndex++;
            }
            if (OutputDefine.PeasantsRetreatLandValue)
            {
                columnIndex++;
            }
            if (OutputDefine.ForeignMarriedRetreatLandValue)
            {
                columnIndex++;
            }
        }

        /// <summary>
        /// 写承包方信息
        /// </summary>
        private void WriteContractorInformation()
        {
            if (OutputDefine.NumberNameValue)
            {
                columnIndex++;
            }
            if (OutputDefine.NumberGenderValue)
            {
                columnIndex++;
            }
            if (OutputDefine.NumberAgeValue)
            {
                columnIndex++;
            }
            if (OutputDefine.NumberCartTypeValue)
            {
                columnIndex++;
            }
            if (OutputDefine.NumberIcnValue)
            {
                columnIndex++;
            }
            if (OutputDefine.NumberRelatioinValue)
            {
                columnIndex++;
            }
            if (OutputDefine.NationValue)
            {
                columnIndex++;
            }
            if (OutputDefine.AccountNatureValue)
            {
                columnIndex++;
            }
            if (OutputDefine.IsSharedLandValue)
            {
                columnIndex++;
            }
            if (OutputDefine.CommentValue)
            {
                columnIndex++;
            }
            if (OutputDefine.CencueCommentValue)
            {
                columnIndex++;
            }
        }

        /// <summary>
        /// 书写编号信息
        /// </summary>
        /// <param name="high"></param>
        /// <param name="number"></param>
        /// <param name="HouseholderName"></param>
        /// <param name="Count"></param>
        private void InitalzieFamilyInformation(int high, string number, VirtualPerson family, string Count)
        {
            try
            {
                SetRange("A" + index, "A" + (index + high - 1), 16.5, 11, false, number);//编号
                SetRange("B" + index, "B" + (index + high - 1), 16.5, 11, false, family.Name.InitalizeFamilyName(SystemDefine.KeepRepeatFlag));//户主
                if (OutputDefine.ContractorTypeValue)
                {
                    if (PersonType != eVirtualType.Land)
                    {
                        SetColumnVisible(columnIndex);
                    }
                    columnIndex++;
                    SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, EnumNameAttribute.GetDescription(family.FamilyExpand.ContractorType));//承包方类型
                }
                if (OutputDefine.NumberValue)
                {
                    columnIndex++;

                    SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, Count);//家庭成员个数
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 书写其他信息
        /// </summary>
        private void WriteFamilyExpandInformation(int high, VirtualPerson family, VirtualPersonExpand expand)
        {
            if (OutputDefine.AllocationPersonValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, expand != null ? expand.AllocationPerson : "");//实际分配人数
            }
            if (OutputDefine.ContractorAddressValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, family.Address);
            }
            if (OutputDefine.PostNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, family.PostalNumber);
            }
            if (OutputDefine.TelephoneValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, family.Telephone);
            }
            if (OutputDefine.SecondConcordNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, expand != null ? expand.ConcordNumber : "");
            }
            if (OutputDefine.SecondWarrantNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, expand != null ? expand.WarrantNumber : "");
            }

            /* 修改于2016/8/31 导出新增配置项 */
            if (OutputDefine.SecondConcordTotalAreaValue)
            {
                columnIndex++;
                var tableAreaCount = LandList.Where(t => t.OwnerId.Equals(family.ID)).Sum(t => t.TableArea == null ? 0 : (double)t.TableArea);
                //secCordTotolAreaCount += expand.SecondConcordTotalArea;
                secCordTotolAreaCount += tableAreaCount;
                //SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false,
                //    expand != null ? (expand.SecondConcordTotalArea == 0.0 ? "" : ToolMath.SetNumbericFormat(expand.SecondConcordTotalArea.ToString(), 2)) : "");
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false,
                    ToolMath.SetNumbericFormat(tableAreaCount.ToString(), 2));
            }
            if (OutputDefine.SecondConcordTotalLandCountValue)
            {
                columnIndex++;
                secCordLandCount += expand.SecondConcordTotalLandCount;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false,
                    expand != null ? (expand.SecondConcordTotalLandCount == 0 ? "" : expand.SecondConcordTotalLandCount.ToString()) : "");
            }

            if (OutputDefine.StartTimeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, (expand.ConcordStartTime != null && expand.ConcordStartTime.HasValue) ? ToolDateTime.GetLongDateString(expand.ConcordStartTime.Value) : "");
            }
            if (OutputDefine.EndTimeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, (expand.ConcordEndTime != null && expand.ConcordEndTime.HasValue) ? ToolDateTime.GetLongDateString(expand.ConcordEndTime.Value) : "");
            }
            if (OutputDefine.ConstructTypeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, expand != null ? EnumNameAttribute.GetDescription(expand.ConstructMode) : "");
            }
            if (OutputDefine.SurveyPersonValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, expand.SurveyPerson);
            }
            if (OutputDefine.SurveyDateValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, (expand.SurveyDate != null && expand.SurveyDate.HasValue) ? ToolDateTime.GetLongDateString(expand.SurveyDate.Value) : "");
            }
            if (OutputDefine.SurveyChronicleValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, expand.SurveyChronicle);
            }
            if (OutputDefine.CheckPersonValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, expand.CheckPerson);
            }
            if (OutputDefine.CheckDateValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, (expand.CheckDate != null && expand.CheckDate.HasValue) ? ToolDateTime.GetLongDateString(expand.CheckDate.Value) : "");
            }
            if (OutputDefine.CheckOpinionValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, expand.CheckOpinion);
            }
            if (OutputDefine.EquityNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, expand.EquityValue > 0 ? expand.EquityValue.ToString() : "");
            }
            if (OutputDefine.EquityAreaValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, expand.EquityArea > 0.0 ? expand.EquityArea.ToString() : "");
            }
        }

        /// <summary>
        /// 书写合计信息
        /// </summary>
        private void WriteCount()
        {
            columnIndex = 2;
            SetRange("A" + index, "A" + index, 16.5, 11, false, "合计");//合计
            SetRange("B" + index, "B" + index, 16.5, 11, false, familyCount > 0 ? familyCount.ToString() : "\\");//合计
            if (OutputDefine.ContractorTypeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);//PersonCount
            }
            if (OutputDefine.NumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, PersonCount.ToString(), true);//PersonCount
            }
            if (OutputDefine.NumberNameValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.NumberGenderValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.NumberAgeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.NumberCartTypeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.NumberIcnValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.NumberRelatioinValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.NationValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.AccountNatureValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.IsSharedLandValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.CommentValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.CencueCommentValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.ExPackageNameValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.ExPackageNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, packageCount > 0 ? packageCount.ToString() : "\\", true);
            }
            if (OutputDefine.IsDeadedValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.LocalMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.PeasantsRetreatLandValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.ForeignMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.AllocationPersonValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.ContractorAddressValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.PostNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.TelephoneValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.SecondConcordNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.SecondWarrantNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }

            /* 修改于2016/8/31 新增配置项汇总 */
            if (OutputDefine.SecondConcordTotalAreaValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index,
                    secCordTotolAreaCount == 0.0 ? "" : ToolMath.SetNumbericFormat(secCordTotolAreaCount.ToString(), 2), true);
            }
            if (OutputDefine.SecondConcordTotalLandCountValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index,
                    secCordLandCount == 0 ? "" : secCordLandCount.ToString(), true);
            }

            if (OutputDefine.StartTimeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.EndTimeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.ConstructTypeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.SurveyPersonValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.SurveyDateValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.SurveyChronicleValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.CheckPersonValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.CheckDateValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.CheckOpinionValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.EquityNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            if (OutputDefine.EquityAreaValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, "\\", true);
            }
            SetLineType("A1", LandOutputDefine.GetColumnValue(OutputDefine.ColumnCount) + index);
        }

        /// <summary>
        /// 对共有人进行排序
        /// </summary>
        /// <returns></returns>
        private List<Person> SortSharePerson(List<Person> personCollection, string houseName)
        {
            List<Person> sharePersonCollection = new List<Person>();
            Person p = personCollection.Find(t => t.Name == houseName);
            if (p != null)
            {
                sharePersonCollection.Add(p);
            }
            foreach (Person person in personCollection)
            {
                if (person.Name != houseName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            personCollection.Clear();
            return sharePersonCollection;
        }

        /// <summary>
        /// 比较现实与二轮延包家庭成员数
        /// </summary>
        private int ComparePersonValue(List<Person> localPerson, List<Person> tablePerson)
        {
            foreach (Person person in localPerson)
            {
                Person per = tablePerson.Find(pr => pr.Name == person.Name);
                if (per != null)
                {
                    tablePerson.Remove(per);
                }
            }
            return localPerson.Count + tablePerson.Count;
        }

        /// <summary>
        /// 书写承包方内容
        /// </summary>
        private void WriteContractorContent()
        {
            toolProgress.InitializationPercent(FamilyList.Count, 99, 1);
            high = 0;//得到每个户中的最大条数
            int number = 1;//编号
            bool numberByFamilyNumber;
            string value = ToolConfiguration.GetSpecialAppSettingValue("ExportFamilyTableByFamilyNumber", "true");
            Boolean.TryParse(value, out numberByFamilyNumber);
            //bool OnlyFamilyInformtion = ToolConfiguration.GetSpecialAppSettingValue("PulicliyFamilyInformation", "true").ToLower() == "true";
            string orderByNumber = ToolConfiguration.GetSpecialAppSettingValue("ExportFamilyTableNumberByFamilyNumber", "false");

            if (SystemDefine.SortTable)
            {
                FamilyList.Sort((a, b) =>
                {
                    int numbera = 0;
                    int numberb = 0;
                    int.TryParse(a.FamilyNumber, out numbera);
                    int.TryParse(b.FamilyNumber, out numberb);
                    if (numbera > numberb)
                        return 1;
                    else
                        return -1;
                });
            }

            #region 户信息

            //根据户读取其家庭成员及承包地
            foreach (VirtualPerson item in FamilyList)
            {
                columnIndex = 2;
                //是否显示集体信息
                if (OtherDefine.ShowFamilyInfomation && (item.Name.IndexOf("集体") >= 0 || item.Name.IndexOf("机动地") >= 0))
                {
                    continue;
                }
                familyCount++;
                item.Name = InitalizeFamilyName(item.Name, SystemDefine.KeepRepeatFlag);
                VirtualPerson vp = null;// DB.TableVirtualPerson.Get(item.ID);
                if (TableFamilyList != null && TableFamilyList.Count > 0)
                {
                    vp = TableFamilyList.Find(t => t.ID == item.ID);
                    if (vp == null)
                    {
                        vp = TableFamilyList.Find(t => t.Name == item.Name && t.Number == item.Number);
                    }
                }
                if (vp != null && string.IsNullOrEmpty(vp.SharePerson))
                {
                    vp = null;
                }
                List<Person> sharePersons = SortSharePerson(item.SharePersonList, item.Name);
                //判断是否共有人
                if (SystemDefine.PersonTable)
                    sharePersons = sharePersons.FindAll(p => p.IsSharedLand == "是");
                sharePersons.ForEach(o => o.Name = InitalizeFamilyName(o.Name, SystemDefine.KeepRepeatFlag));
                List<Person> tablePersons = vp == null ? new List<Person>() : vp.SharePersonList;
                PersonCount += sharePersons.Count;
                if (OutputDefine.IsSharePersonValue && sharePersons.Count > 0)
                {
                    high = sharePersons.Count;
                }
                else
                {
                    high = 1;
                }
                if (OutputDefine.IsExpandValue)
                {
                    int personCount = ComparePersonValue(sharePersons, tablePersons);
                    high = high > personCount ? high : personCount;//获取单户最大值
                }
                //输出户信息
                if ((numberByFamilyNumber || orderByNumber.ToLower() == "true") && !string.IsNullOrEmpty(item.FamilyNumber))
                {
                    Int32.TryParse(item.FamilyNumber, out number);
                }
                int diedPersonCount = 0;
                foreach (var person in sharePersons)
                {
                    if (person.Comment == null) continue;
                    if (person.Comment.Contains("已故") || person.Comment.Contains("去世") || person.Comment.Contains("死亡") || person.Comment.Contains("逝世"))
                    {
                        diedPersonCount++;
                    }
                }
                string exportSharePersonCount = "";
                if (systemseting.ExportVPTableCountContainsDiedPerson == false)
                {
                    exportSharePersonCount = (sharePersons.Count - diedPersonCount).ToString();
                }
                else
                {
                    exportSharePersonCount = sharePersons.Count.ToString();
                }

                InitalzieFamilyInformation(high, number.ToString(), item, exportSharePersonCount);
                //输出人口数据
                WritePerson(sharePersons, tablePersons, item);
                WriteFamilyExpandInformation(high, item, item.FamilyExpand);//扩展信息

                number++;
                index += high;
                toolProgress.DynamicProgress(ZoneDesc + item.Name);
                vp = null;
                sharePersons.Clear();
                tablePersons.Clear();
            }

            #endregion 户信息

            WriteCount();
            FamilyList.Clear();
            PersonCount = 0;
            packageCount = 0;
            columnIndex = 0;
            index = 0;
            high = 0;
        }

        /// <summary>
        /// 书写标题
        /// </summary>
        private void WriteTitle()
        {
            //string title = GetRangeToValue("A1", "T1").ToString();
            //string titleName = PersonType == eVirtualType.Land ? "承包方调查表" : "承包方信息表";
            string titleName = "第二轮土地承包到期后再延长三十年承包方调查表";
            SetRange("A1", LandOutputDefine.GetColumnValue(OutputDefine.ColumnCount) + "1", 32.25, 18, true, UnitName + titleName);
            InitalizeRangeValue("A3", "A5", PersonType == eVirtualType.Land ? "承包方编号" : "承包方编号");
            SetRange("A3", "A5", 16.50, 11, false, PersonType == eVirtualType.Land ? "承包方编号" : "承包方编号");
            InitalizeRangeValue("B3", "B5", PersonType == eVirtualType.Land ? "承包方姓名" : "承包方姓名");
            SetRange("B3", "B5", 16.50, 11, false, PersonType == eVirtualType.Land ? "承包方姓名" : "承包方姓名");
            SetRange("A2", "D2", 16.50, 11, false, 1, 0, "单位:  " + UnitVillageName);// (SystemDefine.CountryTableHead ? UnitVillageName : UnitName));
            SetRange("A2", "D2", 16.50, 11, false, "单位:  " + UnitVillageName);// (SystemDefine.CountryTableHead ? UnitVillageName : UnitName));
            SetRange("E2", LandOutputDefine.GetColumnValue(OutputDefine.ColumnCount) + "2", 21.75, 10, false, 3, 2, "日期:" + GetDate() + "               ");
            columnIndex = 2;
            if (OutputDefine.ContractorTypeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 3, LandOutputDefine.GetColumnValue(columnIndex) + 5, PersonType == eVirtualType.Land ? "承包方类型" : "承包方类型", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 3, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            int startIndex = columnIndex + 1;
            if (OutputDefine.NumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "家庭成员数（个）", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.NumberNameValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "姓名", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.NumberGenderValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "性别", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.NumberAgeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "年龄", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.NumberCartTypeValue)
            {
                columnIndex++;
                SetRangeWidth(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "证件类型", 10, true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.NumberIcnValue)
            {
                columnIndex++;
                SetRangeWidth(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "证件号码", 20, true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.NumberRelatioinValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "家庭关系", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.NationValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "民族", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.AccountNatureValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "户口性质", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.IsSharedLandValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "是否共有人", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.CommentValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "备注", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.CommonOpinion)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "共有人信息修改意见", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.CencueCommentValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "户籍备注", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (columnIndex > 2)
            {
                SetRange(LandOutputDefine.GetColumnValue(startIndex) + 3, LandOutputDefine.GetColumnValue(columnIndex) + 3, "家庭成员情况（含户主）", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 3, LandOutputDefine.GetColumnValue(columnIndex) + 3, 10, 000, false, false);
            }
            startIndex = columnIndex + 1;
            if (OutputDefine.ExPackageNameValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "二轮土地延包姓名", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.ExPackageNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "延包土地份数", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.IsDeadedValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "已死亡人员", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.LocalMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "出嫁后未退承包地人员", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.PeasantsRetreatLandValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "农转非后未退承包地人员", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.ForeignMarriedRetreatLandValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "婚进在婚出地未退承包地人员", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if ((columnIndex - startIndex) >= 0)
            {
                SetRange(LandOutputDefine.GetColumnValue(startIndex) + 3, LandOutputDefine.GetColumnValue(columnIndex) + 3, "二轮土地延包人口和现有家庭人口变动情况", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 3, LandOutputDefine.GetColumnValue(columnIndex) + 3, 10, 000, false, false);
            }
            startIndex = columnIndex + 1;
            if (OutputDefine.AllocationPersonValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "实际分配人数", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.ContractorAddressValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, PersonType == eVirtualType.Land ? "承包方地址" : "承包方地址", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.PostNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "邮政编码", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.TelephoneValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "联系电话", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.SecondConcordNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "承包合同编号", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.SecondWarrantNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "经营权证编号", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }

            /* 修改于2016/8/31 新增配置选项 */
            if (OutputDefine.SecondConcordTotalAreaValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "二轮承包合同总面积", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.SecondConcordTotalLandCountValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "二轮承包合同地块总数", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }

            if (OutputDefine.StartTimeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "承包起始日期", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.EndTimeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "承包结束日期", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.ConstructTypeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "取得承包方式", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.SurveyPersonValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "调查员", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.SurveyDateValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "调查日期", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.SurveyChronicleValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "调查记事", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.CheckPersonValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "审核人", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.CheckDateValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "审核日期", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.CheckOpinionValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "审核意见", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if ((columnIndex - startIndex) >= 0)
            {
                SetRange(LandOutputDefine.GetColumnValue(startIndex) + 3, LandOutputDefine.GetColumnValue(columnIndex) + 3, PersonType == eVirtualType.Land ? "承包方调查信息" : "承包方调查信息", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 3, LandOutputDefine.GetColumnValue(columnIndex) + 3, 10, 000, false, false);
            }
            if (OutputDefine.EquityNumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 3, LandOutputDefine.GetColumnValue(columnIndex) + 5, "入股份数", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
            if (OutputDefine.EquityAreaValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 3, LandOutputDefine.GetColumnValue(columnIndex) + 5, "入股面积", true);
                SetRangeFont(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 11, 000, false, false);
            }
        }

        /// <summary>
        /// 配置
        /// </summary>
        public override void GetReplaceMent()
        {
            EmptyReplacement = WorkStationExtend.GetSystemSetReplacement();
        }

        #endregion 开始往Excel中添加值

        #endregion Methods
    }
}