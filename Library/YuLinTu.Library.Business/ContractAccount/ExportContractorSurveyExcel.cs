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
using System.Collections;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包台账台账调查表
    /// </summary>
    [Serializable]
    public partial class ExportContractorSurveyExcel : ExportExcelBase
    {
        #region Fields

        private bool result = true;
        private Zone currentZone;//当前地域
        private List<VirtualPerson> familys;//承包方集合
        private List<VirtualPerson> tablefamilys;//承包方集合
        private PersonCollection persons;//无户的人口
        private ContractConcord concord;//承包合同
        private ContractRegeditBook regeditBook;//承包权证
        private string zoneCode;//地域代码
        private string templaePath;//模版文件
        private int index;//索引值
        int high;//单元格合并数量
        private ToolProgress toolProgress;//进度
        //private PublicityConfirmDefine PublicityConfirmDefine;//自定义地块导出
        private int columnIndex;//当前列名
        private bool exportByFamilyNumber;//是否根据户号导出表格
        //合计字段
        private int PersonCount;//人合计
        private double ActualAreaCount;//单个实测面积
        private double AwareAreaCount;//总确权面积
        private double MotorizeLandAreaCount;//总机动地面积
        private double TotalTableAreaCount;//总二轮台账面积
        private double ActualAreaAllCount;//总实测面积
        private int landCount;//地块合计
        private double onlyAwareAreaCount;//单个确权面积
        private double onlyMotorizeLandAreaCount;//单个机动地
        private double onlyTotalTableAreaCount;//单个二轮台账
        private int packageCount;//土地延包份数
        private double secondTableArea;//二轮面积之和
        private double secondTotalTableArea;//二轮总面积之和
        private int secondLandCount;//二轮地块合计
        private PublicityConfirmDefine contractLandOutputSurveyDefine = PublicityConfirmDefine.GetIntence();
        private SystemSetDefine SystemSet = SystemSetDefine.GetIntence();
        #endregion

        #region Ctor

        public ExportContractorSurveyExcel()
        {
            SaveFilePath = string.Empty;
            secondTotalTableArea = 0;
            LandArrays = new List<ContractLand>();
            DictionList = new List<Dictionary>();
            TableLandArrays = new List<SecondTableLand>();
            ConcordCollection = new List<ContractConcord>();
            BookColletion = new List<ContractRegeditBook>();
            toolProgress = new ToolProgress();
            toolProgress.OnPostProgress += new ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
            base.TemplateName = "承包地块调查表";
        }

        /// <summary>
        /// 进度提示
        /// </summary>    
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            PostProgress(progress, info);
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitalizeLandDefine()
        {
            //string filePath = Application.StartupPath + @"\Config\" + "PublicityConfirmDefine.xml";
            //if (TableType > 1)
            //{
            //    filePath = Application.StartupPath + @"\Config\" + "TableOutputDefine.xml";
            //}
            // if (File.Exists(filePath))
            //{
            //contractLandOutputSurveyDefine = ToolSerialization.DeserializeXml(filePath, typeof(PublicityConfirmDefine)) as PublicityConfirmDefine;
            //}
            //if (contractLandOutputSurveyDefine == null)
            //{
            //    contractLandOutputSurveyDefine = new PublicityConfirmDefine();
            //}

        }

        #endregion

        #region Properties

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool NotShow { get; set; }

        /// <summary>
        /// 保存文件路径
        /// </summary>
        public string SaveFilePath { get; set; }

        /// <summary>
        /// 表格类型
        /// </summary>
        public int TableType { get; set; }

        ///// <summary>
        ///// 日期
        ///// </summary>
        //public DateTime? Date { get; set; }

        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson Contractor { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 到镇的地域名称
        /// </summary>
        public string ExcelName { get; set; }


        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone
        {
            get { return currentZone; }
            set { currentZone = value; }
        }

        /// <summary>
        /// 承包方集合
        /// </summary>
        public List<VirtualPerson> Familys
        {
            get { return familys; }
            set { familys = value; }
        }

        /// <summary>
        /// 二轮承包方集合
        /// </summary>
        public List<VirtualPerson> TableFamilys
        {
            get { return tablefamilys; }
            set { tablefamilys = value; }
        }

        ///// <summary>
        ///// 承包台账导出配置
        ///// </summary>
        //public PublicityConfirmDefine ContractLandOutputSurveyDefine
        //{
        //    get { return PublicityConfirmDefine.GetIntence(); }
        //}

        /// <summary>
        /// 承包台账常规设置实体
        /// </summary>
        public ContractBusinessSettingDefine SettingDefine = ContractBusinessSettingDefine.GetIntence();

        /// <summary>
        /// 字典内容
        /// </summary>
        public List<Dictionary> DictionList { get; set; }

        /// <summary>
        /// 地块集合
        /// </summary>
        public List<ContractLand> LandArrays { get; set; }

        /// <summary>
        /// 二轮地块集合
        /// </summary>
        public List<SecondTableLand> TableLandArrays { get; set; }

        /// <summary>
        /// 合同
        /// </summary>
        public List<ContractConcord> ConcordCollection { get; set; }

        /// <summary>
        /// 登记簿集合
        /// </summary>
        public List<ContractRegeditBook> BookColletion { get; set; }

        /// <summary>
        /// 进度百分比
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// 当前百分比
        /// </summary>
        public double CurrentPercent { get; set; }

        #endregion

        #region Methods

        #region 开始生成Excel操作

        /// <summary>
        /// 开始操作
        /// </summary>
        /// <param name="zoneCode"></param>
        /// <param name="templaePath"></param>
        public virtual bool BeginExcel(string zoneCode, string templaePath)
        {
            result = true;
            // PostProgress(1);

            if (!File.Exists(templaePath))
            {
                PostErrorInfo("模板路径不存在！");
                return false;
            }
            if (string.IsNullOrEmpty(zoneCode))
            {
                PostErrorInfo("目标地域不存在！");
                return false;
            }
            this.zoneCode = zoneCode;
            this.templaePath = templaePath;
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
                Open(templaePath);
                //PostProgress(15);
                if (!InitalizeValue())
                {
                    return;
                }
                // PostProgress(30);
                BeginWrite();
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
                    string fileName = "土地承包经营权调查表.xls";
                    switch (TableType)
                    {
                        case 2:
                            fileName = "土地承包经营权公示表.xls";
                            break;
                        case 3:
                            fileName = "土地承包经营权签字表.xls";
                            break;
                        case 4:
                            fileName = "土地承包经营权村组公示表.xls";
                            break;
                        case 5:
                            fileName = "土地承包经营权单户确认表.xls";
                            break;
                        default:
                            break;
                    }
                    fileName = UnitName;
                    SaveFilePath = Path.Combine(AgricultureSetting.SystemDefaultDirectory, fileName);
                }
                if (File.Exists(SaveFilePath))
                {
                    File.SetAttributes(SaveFilePath, FileAttributes.Normal);
                    File.Delete(SaveFilePath);
                }
                SaveAs(SaveFilePath);
                toolProgress.DynamicProgress();
                //if (!NotShow && TableType == 5)
                //{
                //    System.Diagnostics.Process.Start(SaveFilePath);
                //}
            }
            catch (System.Exception e)
            {
                //DB.CloseConnection();
                result = false;
                PostErrorInfo(e.Message.ToString());
                Dispose();
                //if (e is TaskStopException)
                //    throw e;
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        private bool InitalizeValue()
        {
            persons = new PersonCollection();
            PersonCount = 0;
            landCount = 0;
            onlyAwareAreaCount = 0.0;
            onlyMotorizeLandAreaCount = 0.0;
            onlyTotalTableAreaCount = 0.0;
            ActualAreaCount = 0.0;
            packageCount = 0;
            // PostProgress(19);
            if (familys == null)
                return false;
            if (TableType != 5)
            {
                string staticsFamily = ToolConfiguration.GetSpecialAppSettingValue("StaticsInformationByLandFamily", "true");
                bool yes = false;
                Boolean.TryParse(staticsFamily, out yes);
                string value = ToolConfiguration.GetSpecialAppSettingValue("ExportAgriLandTableByFamilyNumber", "true");
                Boolean.TryParse(value, out exportByFamilyNumber);
                if (exportByFamilyNumber)
                {
                    familys.Sort((a, b) =>
                    {
                        int numa = 0;
                        int numb = 0;
                        Int32.TryParse(a.FamilyNumber, out numa);
                        Int32.TryParse(b.FamilyNumber, out numb);
                        return numa.CompareTo(numb);
                    });
                }
                if (yes)
                {
                    if (SettingDefine.DisplayCollectUsingCBdata)
                    {
                        //List<VirtualPerson> vpList = familys.FindAll(fm => !string.IsNullOrEmpty(fm.Name) && (fm.Name.IndexOf("机动地") >= 0 || fm.Name.IndexOf("集体") >= 0));
                        //if (vpList != null && vpList.Count > 0)
                        //{
                        //    foreach (VirtualPerson vpn in vpList)
                        //    {
                        //        familys.Remove(vpn);
                        //    }
                        //    vpList.Clear();
                        //}
                        //List<ContractLand> landArraysList = LandArrays.FindAll(c => !string.IsNullOrEmpty(c.Name) && (c.Name.IndexOf("机动地") >= 0 || c.Name.IndexOf("集体") >= 0));
                        //if (landArraysList != null && landArraysList.Count > 0)
                        //{
                        //    foreach (ContractLand landArraysitem in landArraysList)
                        //    {
                        //        LandArrays.Remove(landArraysitem);
                        //    }
                        //    landArraysList.Clear();
                        //}

                        familys.RemoveAll(c => c.FamilyExpand.ContractorType != eContractorType.Farmer);


                    }
                }
                //PostProgress(25);
                //PostProgress(28);
            }
            else
            {
                //familys = new List<VirtualPerson>();
                if (Contractor != null)
                {
                    familys.Add(Contractor);
                }
            }
            return true;
        }

        #endregion

        #region 开始往Excel中添加值

        /// <summary>
        /// 书写内容
        /// </summary>
        private void WriteContent()
        {
            toolProgress.InitializationPercent(familys.Count, Percent, CurrentPercent);
            high = 0;//得到每个户中的最大条数
            int concordHigh = 0;//合同高度
            int number = 1;//编号

            bool numberByFamilyNumber;
            string value = ToolConfiguration.GetSpecialAppSettingValue("ExportAgriLandTableNumberByFamilyNumber", "true");
            Boolean.TryParse(value, out numberByFamilyNumber);
            //bool OnlyFamilyInformtion = ToolConfiguration.GetSpecialAppSettingValue("StaticsInformationByLandFamily", "true").ToLower() == "true";

            #region 户信息

            //ContractConcordCollection concordCollection = (contractLandOutputSurveyDefine.ConcordValue || contractLandOutputSurveyDefine.RegeditBookValue) ? DB.ContractConcord.GetByZoneCode(currentZone.FullCode) : new ContractConcordCollection();
            //根据户读取其家庭成员及承包地
            foreach (VirtualPerson item in familys)
            {
                columnIndex = 2;
                //if (OnlyFamilyInformtion && (item.Name.IndexOf("集体") >= 0 || item.Name.IndexOf("机动地") >= 0))
                //{
                //    continue;
                //}
                VirtualPerson tablevp = null;
                if ((contractLandOutputSurveyDefine.IsContainTableValue || contractLandOutputSurveyDefine.IsContainTablelandValue) && TableFamilys != null)
                {
                    tablevp = tablefamilys.Find(t => t.ID == item.ID);
                }
                if (tablevp != null && string.IsNullOrEmpty(tablevp.SharePerson))
                {
                    tablevp = null;
                }
                item.Name = InitalizeFamilyName(item.Name, SystemSet.KeepRepeatFlag);
                List<Person> sharePersons = SortSharePerson(item.SharePersonList, item.Name);               
                List<Person> tablePersons = tablevp != null ? SortSharePerson(tablevp.SharePersonList, tablevp.Name) : new PersonCollection();
                if (SystemSet.PersonTable)
                {
                    sharePersons = sharePersons.FindAll(c => c.IsSharedLand.Equals("是"));
                    tablePersons = tablePersons.FindAll(c => c.IsSharedLand.Equals("是"));
                    //sharePersons.Remove(sharePersons.Find(c => c.IsSharedLand.Equals("否")));
                    //tablePersons.Remove(tablePersons.Find(c => c.IsSharedLand.Equals("否")));
                }
                PersonCount += sharePersons.Count;
                //判断是否存在合同
                List<ContractConcord> concords = (contractLandOutputSurveyDefine.ConcordValue || contractLandOutputSurveyDefine.RegeditBookValue) ? ConcordCollection.FindAll(cd => (cd.ContracterId != null && cd.ContracterId.HasValue) ? cd.ContracterId.Value == item.ID : false) : new List<ContractConcord>();
                if (concords.Any(cd => string.IsNullOrEmpty(cd.ConcordNumber)))
                {
                    concords = new List<ContractConcord>();
                }
                List<ContractLand> cs = LandArrays.FindAll(ld => (ld.OwnerId != null && ld.OwnerId.HasValue) ? ld.OwnerId.Value == item.ID : false);
                if (concords != null && concords.Count == 1)
                {
                    cs = cs.FindAll(ld => (ld.ConcordId != null && ld.ConcordId.HasValue) ? ld.ConcordId.Value == concords[0].ID : false);
                }
                landCount += cs.Count;
                List<SecondTableLand> tablelandList = contractLandOutputSurveyDefine.IsContainTablelandValue ? (TableLandArrays == null ? new List<SecondTableLand>() : TableLandArrays).FindAll(t => t.OwnerId == (tablevp == null ? item.ID : tablevp.ID)) : new List<SecondTableLand>();
                secondLandCount += tablelandList.Count;
                if (contractLandOutputSurveyDefine.NumberAgeValue || contractLandOutputSurveyDefine.NumberGenderValue || contractLandOutputSurveyDefine.IsSharedLandValue
                    || contractLandOutputSurveyDefine.NumberNameValue || contractLandOutputSurveyDefine.NumberIcnValue || contractLandOutputSurveyDefine.NumberRelatioinValue || contractLandOutputSurveyDefine.FamilyCommentValue)
                {
                    high = sharePersons.Count > cs.Count ? sharePersons.Count : cs.Count;
                }
                else
                {
                    high = cs.Count;
                }
                if (contractLandOutputSurveyDefine.IsContainTableValue)
                {
                    int personCount = ComparePersonValue(sharePersons, tablePersons);
                    high = high > personCount ? high : personCount;//获取单户最大值
                }
                if (contractLandOutputSurveyDefine.IsContainTablelandValue)
                {
                    high = high > tablelandList.Count ? high : tablelandList.Count;
                }
                if (high == 0)
                {
                    high = 1;
                }
                //输出户信息
                if (numberByFamilyNumber && !string.IsNullOrEmpty(item.FamilyNumber))
                {
                    Int32.TryParse(item.FamilyNumber, out number);
                }

                InitalizeContractorInformation(high, number.ToString(), item.Name, sharePersons.Count.ToString(), item);
                //输出人口数据
                int familyIndex = WritePerson(sharePersons, tablePersons.Clone() as PersonCollection, item, tablevp, high);
                //输出二轮承包方信息
                WriteTableNumber(high, number.ToString(), tablevp != null ? tablevp.Name : item.Name, tablePersons.Count == 0 ? "1" : tablePersons.Count.ToString(), familyIndex);
                //书写调查信息
                WriteFamilyExpandInformation(high, item, item.FamilyExpand);
                //输出承包地信息
                cs = SortLandCollection(cs);//对承包地块排序
                concordHigh = high;
                int telephoneIndex = -1;
                int curIndex = index;
                if (concords == null || concords.Count <= 1)
                {
                    concord = (concords == null || concords.Count == 0) ? new ContractConcord() { ID = Guid.Empty } : concords[0];
                    regeditBook = (concords == null || concords.Count == 0) ? new ContractRegeditBook() { ID = Guid.Empty } : BookColletion.Find(t => t.ID == concords[0].ID);
                    telephoneIndex = WriteContractLand(cs, high,index, item.Telephone);//填写地块信息
                }
                if (contractLandOutputSurveyDefine.ConcordValue || contractLandOutputSurveyDefine.RegeditBookValue)
                {
                    if (concords != null && concords.Count > 1)
                    {
                        int curCloumnIndex = columnIndex;
                        //high -= cs.Count;
                        int countLand = 0;
                        int cindexx = index;
                        foreach (ContractConcord conrd in concords)
                        {
                            columnIndex = curCloumnIndex;
                            List<ContractLand> landCollection = cs.FindAll(ld => (ld.ConcordId != null && ld.ConcordId.HasValue && ld.ConcordId.Value == conrd.ID));
                            int height = landCollection != null ? landCollection.Count : 0;
                            concord = conrd;
                            if (BookColletion != null)
                            {
                                regeditBook = BookColletion.Find(t => t.ID == concord.ID);
                            }
                            if (landCollection != null)
                            {
                                List<ContractLand> landArray = new List<ContractLand>();
                                foreach (ContractLand land in landCollection)
                                {
                                    landArray.Add(land);
                                }
                                telephoneIndex = WriteContractLand(landArray, height, cindexx, item.Telephone);//填写地块信息
                                landArray.Clear();
                            }
                            cindexx += height;
                            countLand += height;
                        }
                        //high += cs.Count - countLand;
                    }
                    WriteContract(cs, concordHigh, concords);//书写合同信息
                }
                WriteLandExpandInformation(cs, cs.Count);
                if (contractLandOutputSurveyDefine.IsContainTablelandValue)
                {
                    tablelandList = SortTableLandCollection(tablelandList);
                    WriteSecondTableLand(tablelandList, high, tablevp == null ? "" : tablevp.TotalArea);
                }
                WriteLandSurveyInformation(cs, cs.Count);
                number++;
                index += high;
                toolProgress.DynamicProgress(ExcelName + item.Name);
                tablevp = null;
                sharePersons = null;
                tablePersons = null;
                cs = null;
                tablelandList = null;
                concords = null;
            }
            LandArrays = null;
            ConcordCollection = null;
            familys.Clear();
            GC.Collect();
            #endregion

            if (TableType == 5)
            {
                for (int i = 0; i < 20 - high; i++)
                {
                    InsertRowCell(5 + high);
                    index++;
                }
            }
            WriteCount();
            if (TableType == 5)
            {
                WriteLastInformation();
            }
            SetLineType("A1", PublicityConfirmDefine.GetColumnValue((TableType != 3 && TableType != 5) ? contractLandOutputSurveyDefine.ColumnCount : contractLandOutputSurveyDefine.ColumnCount + 1) + index);
        }

        /// <summary>
        /// 宗地排序
        /// </summary>
        /// <param name="lands"></param>
        /// <returns></returns>
        private List<ContractLand> SortLandCollection(List<ContractLand> lands)
        {
            if (lands == null || lands.Count == 0)
            {
                return new List<ContractLand>();
            }
            var orderdVps = lands.OrderBy(ld =>
            {
                int num = 0;
                string landNumber = ContractLand.GetLandNumber(ld.CadastralNumber);
                int index = landNumber.IndexOf("J");
                if (index < 0)
                {
                    index = landNumber.IndexOf("Q");
                }
                if (index > 0)
                {
                    landNumber = landNumber.Substring(index + 1);
                }
                Int32.TryParse(landNumber, out num);
                if (num == 0)
                {
                    num = 10000;
                }
                return num;
            });
            List<ContractLand> landCollection = new List<ContractLand>();
            foreach (var land in orderdVps)
            {
                if (land.ID == null || !land.OwnerId.HasValue)
                {
                    continue;
                }
                landCollection.Add(land);
            }
            lands.Clear();
            return landCollection;
        }

        /// <summary>
        /// 宗地排序
        /// </summary>
        /// <param name="lands"></param>
        /// <returns></returns>
        private List<SecondTableLand> SortTableLandCollection(List<SecondTableLand> lands)
        {
            if (lands == null || lands.Count == 0)
            {
                return new List<SecondTableLand>();
            }
            var orderdVps = lands.OrderBy(ld =>
            {
                int num = 0;
                string landNumber = ContractLand.GetLandNumber(ld.CadastralNumber);
                int index = landNumber.IndexOf("J");
                if (index < 0)
                {
                    index = landNumber.IndexOf("Q");
                }
                if (index > 0)
                {
                    landNumber = landNumber.Substring(index + 1);
                }
                Int32.TryParse(landNumber, out num);
                if (num == 0)
                {
                    num = 10000;
                }
                return num;
            });
            List<SecondTableLand> landCollection = new List<SecondTableLand>();
            foreach (var land in orderdVps)
            {
                landCollection.Add(land);
            }
            lands.Clear();
            return landCollection;
        }

        /// <summary>
        /// 初始化合同编号
        /// </summary>
        /// <returns></returns>
        private ArrayList InitalizeConcordNumber(VirtualPerson vp)
        {
            string concordNumber = string.Empty;
            string bookNumber = string.Empty;
            ArrayList list = new ArrayList();
            if (ConcordCollection != null)
            {
                foreach (ContractConcord concord in ConcordCollection)
                {
                    concordNumber += concord.ConcordNumber;
                    concordNumber += "、";
                    if (BookColletion != null)
                    {
                        ContractRegeditBook book = BookColletion.Find(t => t.ID == concord.ID);

                        if (book == null)
                        {
                            continue;
                        }
                        bookNumber += book.RegeditNumber;
                        bookNumber += "、";
                        book = null;
                    }
                }
                if (!string.IsNullOrEmpty(concordNumber))
                {
                    concordNumber.Substring(0, concordNumber.Length - 1);
                }
                if (!string.IsNullOrEmpty(bookNumber))
                {
                    bookNumber.Substring(0, bookNumber.Length - 1);
                }
                list.Add(concordNumber);
                list.Add(bookNumber);
            }
            return list;
        }

        /// <summary>
        /// 书写最后一栏信息
        /// </summary>
        private void WriteLastInformation()
        {
            index++;
            int count = (contractLandOutputSurveyDefine.ColumnCount + 1) / 3;
            SetRange("A" + index, PublicityConfirmDefine.GetColumnValue(count) + index, 19.75, 9, false, -1, 0, "填表人(签字):");
            SetRange(PublicityConfirmDefine.GetColumnValue(count + 1) + index, PublicityConfirmDefine.GetColumnValue(2 * count) + index, 19.75, 9, false, -1, 0, "村民小组长(签字):");
            SetRange(PublicityConfirmDefine.GetColumnValue(2 * count + 1) + index, PublicityConfirmDefine.GetColumnValue(contractLandOutputSurveyDefine.ColumnCount + 1) + index, 19.75, 9, false, -1, 0, "承包户主(签字):");
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
