using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using System.IO;

using System.Collections;
using YuLinTu.Data;
using YuLinTu.Library.WorkStation;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.FuSui
{
    public class ExportContractInformationExcelFuSui : ExportExcelBase
    {
        #region Fields

        private bool result = true;
        private Zone currentZone;//当前地域
        private int serial;
        private int cindex;
        private int familyIndex;
        private List<VirtualPerson> familys;//承包方集合
        private List<VirtualPerson> tablefamilys;//承包方集合
        private string templaePath;//模版文件
        private int index;//索引值
        private int high;//单元格合并数量
        private Library.Business.ToolProgress toolProgress;//进度

        #endregion Fields

        #region Properties

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
        public List<Library.Entity.ContractRegeditBook> BookColletion { get; set; }

        /// <summary>
        /// 进度百分比
        /// </summary>
        public double Percent { get; set; }

        /// <summary>
        /// 当前百分比
        /// </summary>
        public double CurrentPercent { get; set; }

        public IDbContext DbContext { get; set; }

        public IContractLandWorkStation LandStation { get; set; }
        public IZoneWorkStation ZoneStation { get; set; }

        #endregion Properties

        public ExportContractInformationExcelFuSui(IDbContext dbContext)
        {
            SaveFilePath = string.Empty;
            LandArrays = new List<ContractLand>();
            DictionList = new List<Dictionary>();
            TableLandArrays = new List<SecondTableLand>();
            ConcordCollection = new List<ContractConcord>();
            BookColletion = new List<Library.Entity.ContractRegeditBook>();
            toolProgress = new Library.Business.ToolProgress();
            DbContext = dbContext;
            LandStation = dbContext.CreateContractLandWorkstation();
            ZoneStation = dbContext.CreateZoneWorkStation();
            toolProgress.OnPostProgress += new Library.Business.ToolProgress.PostProgressDelegate(toolProgress_OnPostProgress);
            TemplateName = "合同信息调查表";
        }

        /// <summary>
        /// 进度提示
        /// </summary>
        private void toolProgress_OnPostProgress(int progress, string info = "")
        {
            PostProgress(progress, info);
        }

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
            this.templaePath = templaePath;

            Write();//写数据
            return result;
        }

        public override void Write()
        {
            try
            {
                //PostProgress(5);
                Open(templaePath);

                // PostProgress(30);
                BeginWrite();

                string fileName = "扶绥土地承包经营权合同信息表.xlt";

                SaveFilePath = Path.Combine(AgricultureSetting.SystemDefaultDirectory, fileName);

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

        private void BeginWrite()
        {
            try
            {
                WriteContent();//开始写内容
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private void WriteContent()
        {
            index = 4;
            toolProgress.InitializationPercent(familys.Count, Percent, CurrentPercent);
            high = 0;//得到每个户中的最大条数
            int concordHigh = 0;//合同高度
            cindex = 4;
            familyIndex = 4;
            serial = 1;
            //根据户读取其家庭成员及承包地
            foreach (VirtualPerson item in familys)
            {
                VirtualPerson tablevp = null;

                if (tablevp != null && string.IsNullOrEmpty(tablevp.SharePerson))
                {
                    tablevp = null;
                }
                List<Person> sharePersons = SortSharePerson(item.SharePersonList, item.Name);
                List<Person> tablePersons = tablevp != null ? SortSharePerson(tablevp.SharePersonList, tablevp.Name) : new PersonCollection();

                //判断是否存在合同
                List<ContractConcord> concords = ConcordCollection.FindAll(cd => (cd.ContracterId != null && cd.ContracterId.HasValue) ? cd.ContracterId.Value == item.ID : false);
                if (concords.Any(cd => string.IsNullOrEmpty(cd.ConcordNumber)))
                {
                    concords = new List<ContractConcord>();
                }
                List<ContractLand> cs = LandArrays.FindAll(ld => (ld.OwnerId != null && ld.OwnerId.HasValue) ? ld.OwnerId.Value == item.ID : false);
                if (concords != null && concords.Count == 1)
                {
                    cs = cs.FindAll(ld => (ld.ConcordId != null && ld.ConcordId.HasValue) ? ld.ConcordId.Value == concords[0].ID : false);
                }
                List<SecondTableLand> tablelandList = new List<SecondTableLand>();
                tablelandList = TableLandArrays.FindAll(t => t.OwnerId == (tablevp == null ? item.ID : tablevp.ID));
                int personCount = ComparePersonValue(sharePersons, tablePersons);

                if (cs.Count >= personCount)
                {
                    high = cs.Count;
                }
                else
                {
                    high = personCount;
                }
                //输出户信息

                var partents = ZoneStation.GetParentsToProvince(currentZone);
                var contractCode = concords.FirstOrDefault().ConcordNumber;

                var listTissue = LandStation.GetTissuesByConcord(currentZone);
                CollectivityTissue tissue = listTissue.Find(tu => tu.ID == concords.FirstOrDefault().SenderId);
                InitalizeZoneInformation(high, partents, contractCode, item, tissue, concords.FirstOrDefault());
                cs = SortLandCollection(cs);//对承包地块排序
                concordHigh = high;
                int curIndex = index;

                WriteContractLand(cs, cs.Count, sharePersons.Count);//填写地块信息
                WriteSharePerson(sharePersons, cs.Count, sharePersons.Count);
            }
            SetLineType("X" + (cindex-1).ToString());

        }

        public void WriteSharePerson(List<Person> vp, int csCount, int spCount)
        {
            for (int i = 0; i < vp.Count; i++)
            {
                if (i == vp.Count - 1)
                {
                    SetRange("R" + familyIndex, "R" + familyIndex, vp[i].Name);
                    SetRange("S" + familyIndex, "S" + familyIndex, vp[i].Relationship);
                    SetRange("T" + familyIndex, "T" + familyIndex, vp[i].ICN);
                    SetRange("U" + familyIndex, "U" + familyIndex, vp[i].Comment);
                    familyIndex = familyIndex + high - vp.Count + 1;
                }
                else
                {
                    SetRange("R" + familyIndex, "R" + familyIndex, vp[i].Name);
                    SetRange("S" + familyIndex, "S" + familyIndex, vp[i].Relationship);
                    SetRange("T" + familyIndex, "T" + familyIndex, vp[i].ICN);
                    SetRange("U" + familyIndex, "U" + familyIndex, vp[i].Comment);
                    familyIndex++;
                }
            }
        }

        public void WriteContractLand(List<ContractLand> cs, int csCount, int spCount)
        {
            foreach (ContractLand land in cs)
            {
                SetRange("N" + cindex, "N" + cindex, land.Name);
                SetRange("O" + cindex, "O" + cindex, land.LandNumber);
                SetRange("P" + cindex, "P" + cindex, land.AwareArea.ToString());
                SetRange("Q" + cindex, "Q" + cindex, land.Comment);
                cindex++;
            }
        }

        private void InitalizeZoneInformation(int high, List<Zone> address, string contractCode, VirtualPerson item,
                                              CollectivityTissue tissue, ContractConcord concord)
        {
            try
            {
              
                var provinceName = address[4].Name;
                var cityName = address[3].Name;
                var countyName = address[2].Name;
                var townName = address[1].Name;
                var villageName = address[0].Name;
                SetRange("A" + index, "A" + index, serial.ToString());//行政区域
                serial++;
                SetRange("B" + index, "B" + index, provinceName);//行政区域
                SetRange("C" + index, "C" + index, cityName);//行政区域
                SetRange("D" + index, "D" + index, countyName);//行政区域
                SetRange("E" + index, "E" + index, townName);//行政区域
                SetRange("F" + index, "F" + index, villageName);//行政区域
                SetRange("G" + index, "G" + index, contractCode);
                SetRange("H" + index, "H" + index, tissue.Code);
                SetRange("I" + index, "I" + index, tissue.Name);
                int familyNumber = int.Parse(item.FamilyNumber);
                string number = string.Format("{0:D4}", familyNumber);
                SetRange("J" + index, "J" + index, $"{item.ZoneCode}{number}");
                SetRange("K" + index, "K" + index, item.Name);
                SetRange("L" + index, "L" + index, item.Number);
                SetRange("M" + index, "M" + index, item.Telephone);
                var arableLandStartTime = (DateTime)concord.ArableLandStartTime;
                SetRange("V" + index, "V" + index, arableLandStartTime.ToString("yyyy-MM-dd"));
                var arableLandEndTime = (DateTime)concord.ArableLandEndTime;
                SetRange("W" + index, "W" + index, arableLandEndTime.ToString("yyyy-MM-dd"));
                var senderDate = (DateTime)concord.SenderDate;
                SetRange("X" + index, "X" + index, senderDate.ToString("yyyy-MM-dd"));
                index = index + high;
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private List<Person> SortSharePerson(List<Person> personCollection, string houseName)
        {
            if (personCollection == null || personCollection.Count == 0)
            {
                return new List<Person>();
            }
            List<Person> sharePersonCollection = new List<Person>();
            foreach (var person in personCollection)
            {
                if (person.Name == houseName)
                {
                    sharePersonCollection.Add(person);
                    break;
                }
            }
            foreach (var person in personCollection)
            {
                if (person.Name != houseName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            return sharePersonCollection;
        }

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
    }
}