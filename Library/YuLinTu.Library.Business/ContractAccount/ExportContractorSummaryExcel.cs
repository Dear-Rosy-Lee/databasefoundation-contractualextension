/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using YuLinTu.Data;
using System.IO;
using YuLinTu.Library.WorkStation;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 导出农村土地承包经营权数据汇总表
    /// </summary>
    public class ExportContractorSummaryExcel : ExportExcelBase
    {
        #region Ctro

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportContractorSummaryExcel(IDbContext DbContext)
        {
            SaveFilePath = string.Empty;
            ListConcord = new List<ContractConcord>();
            listLand = new List<ContractLand>();
            ListPerson = new List<VirtualPerson>();
            ListBook = new List<ContractRegeditBook>();
            if (DbContext == null)
            {
                PostErrorInfo("数据源错误!");
                return;
            }
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
        }

        /// <summary>
        /// 进度
        /// </summary>
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            PostProgress(progress, StatuDes);
        }

        #endregion

        #region Fields

        private bool result = true;
        private Zone currentZone;
        private List<VirtualPerson> listPerson;
        private List<ContractLand> listLand;
        private List<ContractConcord> listConcord;
        private List<ContractRegeditBook> listBook;
        private ToolProgress toolProgress;  //进度条
        private string templatePath; //模板路径
        private List<VirtualPerson> listVp;   //存放排序后的承包方集合(按户号)
        private int columnIndex; //当前列名索引
        private int index; //索引值
        int high;  //单元格合并数量 

        //合计字段
        private int personCount;//人合计
        private int landValue;//地块数
        private double totalTableArea;//总台账面积
        private double totalActualArea;//总实测面积
        private double totalAwareArea;//总确权面积
        private SystemSetDefine SystemSet = SystemSetDefine.GetIntence();

        #endregion

        #region Properties

        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 状态描述
        /// </summary>
        public string StatuDes { get; set; }

        /// <summary>
        /// 百分比进度
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// 当前百分比进度
        /// </summary>
        public double CurrentPercent { get; set; }

        /// <summary>
        /// 地域描述
        /// </summary>
        public string ZoneDesc { get; set; }

        /// <summary>
        /// 导出表类型
        /// </summary>
        public eContractAccountType ArgType { get; set; }

        /// <summary>
        /// 汇总表设置实体
        /// </summary>
        public DataSummaryDefine SummaryDefine = new DataSummaryDefine();

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

        ///// <summary>
        ///// 日期
        ///// </summary>
        //public DateTime? Date { get; set; }

        /// <summary>
        /// 当前选择地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set { currentZone = value; }
        }

        /// <summary>
        /// 承包方集合(传入的没有经过排序的承包方集合)
        /// </summary>
        public List<VirtualPerson> ListPerson
        {
            get { return listPerson; }
            set { listPerson = value; }
        }

        /// <summary>
        /// 承包地块集合
        /// </summary>
        public List<ContractLand> ListLand
        {
            get { return listLand; }
            set { listLand = value; }
        }

        /// <summary>
        /// 承包合同集合
        /// </summary>
        public List<ContractConcord> ListConcord
        {
            get { return listConcord; }
            set { listConcord = value; }
        }

        /// <summary>
        /// 承包权证集合
        /// </summary>
        public List<ContractRegeditBook> ListBook
        {
            get { return listBook; }
            set { listBook = value; }
        }

        #endregion

        #region Methods

        #region Method-开始生成Excel操作

        /// <summary>
        /// 开始导出数据
        /// </summary>
        /// <param name="templaePath">模板保存路径</param>
        public bool BeginExcel(string templatePath)
        {
            this.templatePath = templatePath;
            result = true;
            //PostProgress(1, StatuDes);
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
            Write();   //写数据
            return result;
        }

        /// <summary>
        /// 写数据
        /// </summary>
        public override void Write()
        {
            try
            {
                //PostProgress(5, StatuDes);
                Open(templatePath);   //打开模板
                //PostProgress(15, StatuDes);
                if (!InitalizeValue())
                {
                    return;
                }
                //PostProgress(30, StatuDes);
                WriteFamilyInformation();
                if (!string.IsNullOrEmpty(SaveFilePath))
                {
                    SaveFilePath = WordOperator.InitalizeValideFileName(SaveFilePath);
                    if (!Directory.Exists(Path.GetDirectoryName(SaveFilePath)))
                    {

                        Directory.CreateDirectory(Path.GetDirectoryName(SaveFilePath));
                    }
                }
                else
                {
                    string fileName = "数据汇总表.xls";
                    switch (ArgType)
                    {
                        case eContractAccountType.ExportSummaryExcel:
                            fileName = "数据汇总表.xls";
                            break;
                        case eContractAccountType.ExportConcordInformationTable:
                            fileName = "合同明细表.xls";
                            break;
                        case eContractAccountType.ExportWarrentSummaryTable:
                            fileName = "权证数据汇总表.xls";
                            break;
                        default:
                            break;
                    }
                    SaveFilePath = Path.Combine(Path.GetTempPath(), fileName);
                }
                if (File.Exists(SaveFilePath))
                {
                    File.SetAttributes(SaveFilePath, FileAttributes.Normal);
                    File.Delete(SaveFilePath);
                }
                toolProgress.DynamicProgress();
                SaveAs(SaveFilePath);    //保存文件
            }
            catch (Exception ex)
            {
                result = false;
                PostErrorInfo(ex.Message.ToString());
                Dispose();
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private bool InitalizeValue()
        {
            personCount = 0;
            if (currentZone == null)
            {
                return PostErrorInfo("行政区域信息无效!");
            }
            //PostProgress(19, StatuDes);
            List<VirtualPerson> persons = ListPerson;
            if (persons == null)
            {
                return PostErrorInfo("数据源错误.");
            }

            //排序
            var orderVps = persons.OrderBy(c =>
            {
                int num = 0;
                Int32.TryParse(c.FamilyNumber, out num);
                return num;
            });

            listVp = new List<VirtualPerson>();
            foreach (var vp in orderVps)
            {
                listVp.Add(vp);
            }
            persons.Clear();

            //此处应该根据配置文件判断是否显示集体户和机动地信息
            if (ArgType == eContractAccountType.ExportSummaryExcel)
            {
                if (SettingDefine.DisplayCollectUsingCBdata)
                {
                    //List<VirtualPerson> listVirtualPeron = listVp.FindAll(c => c.Name == "集体");
                    //foreach (var Vp in listVirtualPeron)
                    //{
                    //    listVp.Remove(Vp);
                    //}
                    //listVirtualPeron.Clear();

                    listVp.RemoveAll(c => c.FamilyExpand.ContractorType != eContractorType.Farmer);
                }
            }

            //PostProgress(25, StatuDes);
            return true;
        }

        #endregion

        #region Method-开始往Excel添加值

        /// <summary>
        /// 开始写数据
        /// </summary>
        private void WriteFamilyInformation()
        {
            //写标题信息
            WriteTitle();

            //开始写内容
            WriteContractorContent();

            ListConcord.Clear();
            ListLand.Clear();
            ListPerson.Clear();
            listVp.Clear();
            GC.Collect();
        }

        /// <summary>
        /// 写标题
        /// </summary>
        private void WriteTitle()
        {
            string titleName = ArgType == eContractAccountType.ExportConcordInformationTable ? "合同明细表" : "数据汇总表";
            SummaryDefine.WarrantNumber = false;    // 权证编号不显示
            SummaryDefine.ConcordNumberValue = false;   // 合同编号不显示
            SummaryDefine = DataSummaryDefine.GetIntence();
            if (SummaryDefine.ColumnCount <= 7)
            {
                SetRange("A1", "G1", 32.25, 18, true, UnitName + titleName);
            }
            else
            {
                SetRange("A1", LandOutputDefine.GetColumnValue(SummaryDefine.ColumnCount) + "1", 32.25, 18, true, UnitName + titleName);
            }
            SetRange("A2", "D2", 21.75, 11, false, "单位:  " + SystemSet.GetTBDWStr(CurrentZone));
            SetRange("E2", LandOutputDefine.GetColumnValue(SummaryDefine.ColumnCount) + "2", 21.75, 11, false, "日期:" + GetDate() + "               ");

            if (SummaryDefine.NumberNameValue)
            {
                SetRangeWidth("A3", "A5", "编号", 6.0, true);
                SetRange("A3", "A5", 16.5, 11, false, "编号");
                SetRangeWidth("B3", "B5", "户主姓名", 12.0, true);
                SetRange("B3", "B5", 16.5, 11, false, "户主姓名");
            }
            else
            {
                SetRangeWidth("A4", "A5", "编号", 6.0, true);
                SetRange("A4", "A5", 16.5, 11, false, "编号");
                SetRangeWidth("B4", "B5", "户主姓名", 12.0, true);
                SetRange("B4", "B5", 16.5, 11, false, "户主姓名");
            }
            columnIndex = 2;
            if (SummaryDefine.NumberValue)
            {
                columnIndex++;
                SetRangeWidthAndHeight(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "家庭成员数（个）", 10, 24);
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 16.5, 11, false, "家庭成员数（个）");
            }
            if (SummaryDefine.NumberNameValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 16.5, 11, false, "姓名");
            }
            if (SummaryDefine.NumberGenderValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 16.5, 11, false, "性别");
            }
            if (SummaryDefine.NumberAgeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 16.5, 11, false, "年龄");
            }
            if (SummaryDefine.NumberIcnValue)
            {
                columnIndex++;
                SetRangeWidth(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "证件号码", 23, true);
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 16.5, 11, false, "证件号码");
            }
            if (SummaryDefine.NumberRelatioinValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 16.5, 11, false, "家庭关系");
            }
            if (SummaryDefine.CommentValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 16.5, 11, false, "备注");
            }
            if (columnIndex > 2)
            {
                if (SummaryDefine.NumberNameValue)
                {
                    SetRange(LandOutputDefine.GetColumnValue(3) + 3, LandOutputDefine.GetColumnValue(columnIndex) + 3, 16.5, 11, false, "家庭成员情况（含户主）");
                }
                else
                {
                    SetRange("A3", LandOutputDefine.GetColumnValue(columnIndex) + 3, 16.5, 11, false, "承包方基本情况");
                    SetRangeHeight("A3", LandOutputDefine.GetColumnValue(columnIndex) + 3, 16.50);
                }
            }
            int startIndex = columnIndex + 1;
            if (SummaryDefine.ConcordNumberValue && ArgType != eContractAccountType.ExportSummaryExcel)
            {
                columnIndex++;
                SetRangeWidth(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "合同编号", 30, true);
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 16.5, 11, false, "合同编号");
            }
            if (SummaryDefine.WarrantNumber && ArgType != eContractAccountType.ExportSummaryExcel)
            {
                columnIndex++;
                SetRangeWidth(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "权证编号", 30, true);
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 16.5, 11, false, "权证编号");
            }
            if (SummaryDefine.LandCount)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 16.5, 11, false, "宗地数");
            }
            if (SummaryDefine.TableArea)
            {
                columnIndex++;
                SetRangeWidth(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "二轮合同面积(亩)", 12, true);
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 16.5, 11, false, "二轮合同面积(亩)");
            }
            if (SummaryDefine.ActualArea)
            {
                columnIndex++;
                SetRangeWidth(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "实测面积(亩)", 12, true);
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 16.5, 11, false, "实测面积(亩)");
            }
            if (SummaryDefine.AwareArea)
            {
                columnIndex++;
                SetRangeWidth(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, "确权面积(亩)", 12, true);
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + 4, LandOutputDefine.GetColumnValue(columnIndex) + 5, 16.5, 11, false, "确权面积(亩)");
            }
            if ((columnIndex - startIndex) >= 0)
            {

                SetRange(LandOutputDefine.GetColumnValue(startIndex) + 3, LandOutputDefine.GetColumnValue(columnIndex) + 3, 16.5, 11, false, "农村土地承包经营权汇总情况");
            }
        }

        /// <summary>
        /// 书写承包方内容
        /// </summary>
        public void WriteContractorContent()
        {
            toolProgress.InitializationPercent(listVp.Count, Percent, CurrentPercent);
            high = 1;//得到每个户中的最大条数
            int number = 1;//编号
            bool numberByFamilyNumber;
            string value = ToolConfiguration.GetSpecialAppSettingValue("ExportAgriLandTableNumberByFamilyNumber", "true");
            Boolean.TryParse(value, out numberByFamilyNumber);
            bool useFamilyNumber = false;
            switch (ArgType)
            {
                case eContractAccountType.ExportSummaryExcel:
                    useFamilyNumber = numberByFamilyNumber;
                    break;
                case eContractAccountType.ExportConcordInformationTable:
                    useFamilyNumber = true;  //按序号列出
                    break;
                case eContractAccountType.ExportWarrentSummaryTable:
                    useFamilyNumber = true;  //按序号列出
                    break;
            }

            #region 户信息

            //根据户读取其家庭成员及承包地
            foreach (VirtualPerson item in listVp)
            {
                columnIndex = 2;
                List<Person> sharePersons = SortSharePerson(item.SharePersonList, item.Name);
                if (SystemSet.PersonTable)
                    sharePersons = sharePersons.FindAll(c => c.IsSharedLand.Equals("是"));
                sharePersons.ForEach(o => o.Name = InitalizeFamilyName(o.Name, SystemSet.KeepRepeatFlag));
                personCount += sharePersons.Count;
                if (SummaryDefine.NumberNameValue)
                {
                    high = sharePersons.Count;
                }
                if (high == 0)
                    high = 1;
                if (useFamilyNumber && !string.IsNullOrEmpty(item.FamilyNumber))
                {
                    Int32.TryParse(item.FamilyNumber, out number);//输出户信息
                }
                WriteNumber(high, number.ToString(), item.Name, sharePersons.Count.ToString()); //书写编号信息           
                WritePerson(sharePersons);   //填写承包方数据   
                WriteLandSummeryInformation(high, item);   //填写地块统计信息
                number++;
                index += high;
                toolProgress.DynamicProgress(ZoneDesc + item.Name);
                sharePersons = null;
            }

            #endregion

            WriteCount();   //书写合计信息
        }

        /// <summary>
        /// 对共有人进行排序
        /// </summary>
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
        /// 书写编号信息
        /// </summary>
        private void WriteNumber(int high, string number, string HouseholderName, string Count)
        {
            SetRange("A" + index, "A" + (high + index - 1), 16.5, 11, false, number);//编号
            SetRange("B" + index, "B" + (high + index - 1), 16.5, 11, false, HouseholderName.InitalizeFamilyName(SystemSet.KeepRepeatFlag));//户主
            if (SummaryDefine.NumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + (index + high - 1), 16.5, 11, false, Count); //家庭成员个数
            }
        }

        /// <summary>
        /// 写现实人口数据
        /// </summary>
        private void WritePerson(List<Person> ps)
        {
            int pindex = index;
            int age = 0;   //家庭成员年龄
            int curIndex = columnIndex;
            List<Person> sharePersons = new List<Person>();
            if (SummaryDefine.NumberNameValue)
            {
                sharePersons = ps;
            }
            else
            {
                sharePersons.Add(ps[0]);
            }
            foreach (Person person in sharePersons)
            {
                columnIndex = curIndex;
                if (SummaryDefine.NumberNameValue)
                {
                    columnIndex++;
                    SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.Name);//成员姓名
                }
                string value = EnumNameAttribute.GetDescription(person.Gender);
                if (SummaryDefine.NumberGenderValue)
                {
                    columnIndex++;
                    SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, value == "未知" ? "" : value);//成员性别
                }
                if (SummaryDefine.NumberAgeValue)
                {
                    age = person.GetAge();
                    columnIndex++;
                    if (age > 0 && age < 120)
                    {
                        SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, age.ToString());//成员年龄
                    }
                    else
                    {
                        SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, "");//成员年龄
                    }
                }
                if (SummaryDefine.NumberIcnValue)
                {
                    columnIndex++;
                    SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.ICN);//成员身份证号
                }
                if (SummaryDefine.NumberRelatioinValue)
                {
                    columnIndex++;
                    SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.Relationship);//家庭成员关系
                }
                if (SummaryDefine.CommentValue)
                {
                    columnIndex++;
                    SetRange(LandOutputDefine.GetColumnValue(columnIndex) + pindex, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, person.Comment);//备注
                }
                pindex++;
            }
        }

        /// <summary>
        /// 填写地块统计信息
        /// </summary>
        private void WriteLandSummeryInformation(int height, VirtualPerson vp)
        {
            List<ContractConcord> concordCollection = new List<ContractConcord>();
            List<ContractRegeditBook> bookCollection = new List<ContractRegeditBook>();
            int landCount = 0;
            double tableArea = 0.0;
            double actualArea = 0.0;
            double awareArea = 0.0;
            if (ArgType == eContractAccountType.ExportSummaryExcel)
            {
                List<ContractLand> lands = listLand.FindAll(ld => ((ld.OwnerId != null && ld.OwnerId.HasValue) ? ld.OwnerId.Value : Guid.NewGuid()) == vp.ID);
                foreach (ContractLand land in lands)
                {
                    tableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                    actualArea += land.ActualArea;
                    awareArea += land.AwareArea;
                }
                totalTableArea += tableArea;
                totalActualArea += actualArea;
                totalAwareArea += awareArea;
                landCount = lands != null ? lands.Count : 0;
                landValue += landCount;
                lands.Clear();
            }
            else
            {
                //有关合同
                if (ListConcord != null)
                {
                    concordCollection = ListConcord.FindAll(cd => ((cd.ContracterId != null && cd.ContracterId.HasValue) ? cd.ContracterId.Value : Guid.NewGuid()) == vp.ID);
                    if (concordCollection == null || concordCollection.Count == 0)
                    {
                        concordCollection = ListConcord.FindAll(cd => cd.ContracterName == vp.Name);
                    }
                    if (concordCollection == null)
                    {
                        concordCollection = new List<ContractConcord>();
                    }
                    List<ContractLand> lands = new List<ContractLand>();
                    if (concordCollection.Count > 0 && ConcordIsValide(concordCollection))
                    {
                        List<ContractLand> landArray = new List<ContractLand>();
                        foreach (ContractConcord concord in concordCollection)
                        {
                            landArray = listLand.FindAll(ld => ((ld.ConcordId != null && ld.ConcordId.HasValue) ? ld.ConcordId.Value : Guid.NewGuid()) == concord.ID);
                            if (landArray != null)
                            {
                                foreach (ContractLand land in landArray)
                                {
                                    lands.Add(land);
                                }
                            }
                        }
                        landArray.Clear();
                    }
                    else
                    {
                        lands = listLand.FindAll(ld => ((ld.OwnerId != null && ld.OwnerId.HasValue) ? ld.OwnerId.Value : Guid.NewGuid()) == vp.ID);
                    }
                    landCount = lands != null ? lands.Count : 0;
                    landValue += landCount;
                    if (lands != null)
                    {
                        foreach (ContractLand land in lands)
                        {
                            tableArea += (land.TableArea != null && land.TableArea.HasValue) ? land.TableArea.Value : 0.0;
                            actualArea += land.ActualArea;
                            awareArea += land.AwareArea;
                        }
                        totalTableArea += tableArea;
                        totalActualArea += actualArea;
                        totalAwareArea += awareArea;
                    }
                    lands.Clear();
                }
                foreach (ContractConcord concord in concordCollection)
                {
                    //承包权证
                    ContractRegeditBook book = ListBook.Find(c => c.ID == concord.ID);
                    if (book != null)
                    {
                        bookCollection.Add(book);
                    }
                }
            }
            int pindex = index + height - 1;
            if (SummaryDefine.ConcordNumberValue && ArgType != eContractAccountType.ExportSummaryExcel)
            {
                columnIndex++;
                if (concordCollection != null)
                {
                    string concordNumber = string.Empty;
                    foreach (ContractConcord concord in concordCollection)
                    {
                        concordNumber += concord.ConcordNumber;
                        concordNumber += "、";
                    }
                    concordNumber = string.IsNullOrEmpty(concordNumber) ? concordNumber : concordNumber.Substring(0, concordNumber.Length - 1);
                    SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, concordNumber);//合同编号
                }
            }
            if (SummaryDefine.WarrantNumber && ArgType != eContractAccountType.ExportSummaryExcel)
            {
                columnIndex++;
                if (bookCollection != null)
                {
                    string warrantNumber = string.Empty;
                    foreach (ContractRegeditBook book in bookCollection)
                    {
                        warrantNumber += book.RegeditNumber;
                        warrantNumber += "、";
                    }
                    warrantNumber = string.IsNullOrEmpty(warrantNumber) ? warrantNumber : warrantNumber.Substring(0, warrantNumber.Length - 1);
                    SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, warrantNumber);//权证编号
                }
            }
            if (SummaryDefine.LandCount)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, landCount > 0 ? landCount.ToString() : "");//宗地数
            }
            if (SummaryDefine.TableArea)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, tableArea > 0.0 ? ToolMath.SetNumbericFormat(tableArea.ToString(), 2) : SystemSet.InitalizeAreaString());//台账面积
            }
            if (SummaryDefine.ActualArea)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, actualArea > 0.0 ? ToolMath.SetNumbericFormat(actualArea.ToString(), 2) : SystemSet.InitalizeAreaString());//实测面积
            }
            if (SummaryDefine.AwareArea)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + pindex, 16.5, 11, false, awareArea > 0.0 ? ToolMath.SetNumbericFormat(awareArea.ToString(), 2) : SystemSet.InitalizeAreaString());//确权面积
            }
            concordCollection = null;
            bookCollection = null;
        }

        /// <summary>
        /// 书写合计信息
        /// </summary>
        private void WriteCount()
        {
            columnIndex = 2;
            SetRange("A" + index, "A" + index, 16.5, 11, false, "合计");//合计
            SetRange("B" + index, "B" + index, 16.5, 11, false, listVp.Count > 0 ? listVp.Count.ToString() : "\\");//合计
            if (SummaryDefine.NumberValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, 16.5, 11, false, personCount.ToString());//PersonCount
            }
            if (SummaryDefine.NumberNameValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, 16.5, 11, false, "\\");
            }
            if (SummaryDefine.NumberGenderValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, 16.5, 11, false, "\\");
            }
            if (SummaryDefine.NumberAgeValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, 16.5, 11, false, "\\");
            }
            if (SummaryDefine.NumberIcnValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, 16.5, 11, false, "\\");
            }
            if (SummaryDefine.NumberRelatioinValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, 16.5, 11, false, "\\");
            }
            if (SummaryDefine.CommentValue)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, 16.5, 11, false, "\\");
            }
            if (SummaryDefine.ConcordNumberValue && ArgType != eContractAccountType.ExportSummaryExcel)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, 16.5, 11, false, "\\");
            }
            if (SummaryDefine.WarrantNumber && ArgType != eContractAccountType.ExportSummaryExcel)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, 16.5, 11, false, "\\");
            }
            if (SummaryDefine.LandCount)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, 16.5, 11, false, landValue > 0 ? landValue.ToString() : "\\");
            }
            if (SummaryDefine.TableArea)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, 16.5, 11, false, totalTableArea > 0 ? ToolMath.SetNumbericFormat(totalTableArea.ToString(), 2) : "\\");
            }
            if (SummaryDefine.ActualArea)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, 16.5, 11, false, totalActualArea > 0 ? ToolMath.SetNumbericFormat(totalActualArea.ToString(), 2) : "\\");
            }
            if (SummaryDefine.AwareArea)
            {
                columnIndex++;
                SetRange(LandOutputDefine.GetColumnValue(columnIndex) + index, LandOutputDefine.GetColumnValue(columnIndex) + index, 16.5, 11, false, totalAwareArea > 0 ? ToolMath.SetNumbericFormat(totalAwareArea.ToString(), 2) : "\\");
            }
            SetLineType("A1", LandOutputDefine.GetColumnValue(SummaryDefine.ColumnCount) + index);
        }

        /// <summary>
        /// 合同是否有效
        /// </summary>
        /// <param name="concords"></param>
        /// <returns></returns>
        private bool ConcordIsValide(List<ContractConcord> concords)
        {
            bool success = true;
            foreach (ContractConcord concord in concords)
            {
                if (concord.IsValid)
                {
                    continue;
                }
                success = false;
                break;
            }
            return success;
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
