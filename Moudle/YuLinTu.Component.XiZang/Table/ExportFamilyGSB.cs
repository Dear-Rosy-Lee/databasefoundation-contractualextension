using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.XiZangLZ
{
    /// <summary>
    /// 农村土地承包经营权基础信息公示表
    /// </summary>
    public class ExportFamilyGSB : ExcelBase
    {
        #region Fields
        private string errorInformation = string.Empty;//错误信息   
        private string fileName = string.Empty;//文件名称    
        private GetDictionary dic;//字典
        private int currentIndex;

        #endregion

        #region Properties
        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }
        /// <summary>
        /// 承包合同
        /// </summary>
        public ContractConcord Concord { get; set; }
        /// <summary>
        /// 承包地集合
        /// </summary>
        public List<ContractLand> Lands { get; set; }
        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson VirtualPerson { get; set; }
        /// <summary>
        /// 系统设置
        /// </summary>
        public SystemSetDefine SystemSet { get; set; }

        #endregion

        #region Ctor

        public ExportFamilyGSB(string fileName, string dictoryname)
        {
            this.fileName = fileName;
            dic = new GetDictionary(dictoryname);
            dic.Read();
            currentIndex = 8;
        }

        #endregion

        #region Methods

        #region Override
        public override void Read()
        {

        }
        /// <summary>
        /// 写方法
        /// </summary>
        public override void Write()
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || !OpenFamilyFile(fileName) || !CheckDataInformation())
                {
                    return;
                }
                WriteTitleInformation();
                WriteConcordInformation();
                Disponse();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                errorInformation = ex.ToString();
                return;
            }
            return;
        }
        /// <summary>
        /// 打开文件
        /// </summary>
        private bool OpenFamilyFile(string fname)
        {
            try
            {
                Open(fname, 0);//打开文件
                return true;
            }
            catch
            {
                errorInformation = "打开文件失败";
                return false;
            }
        }
        /// <summary>
        /// 检查数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool CheckDataInformation()
        {
            //检查合同数据
            if (VirtualPerson == null)
            {
                errorInformation = "请选择承包方";
                return false;
            }
            Lands = SortLandCollection(Lands);
            if (CurrentZone == null)
            {
                errorInformation = "行政地域为空";
                return false;
            }
            if (dic == null)
            {
                errorInformation = "请检查字典，字典为空";
                return false;
            }
            return true;
        }

        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            VirtualPerson = null;
            CurrentZone = null;
            Lands = null;
            GC.Collect();
        }

        #endregion

        #region Family

        /// <summary>
        /// 设置共有人信息
        /// </summary>
        /// <param name="dt"></param>
        private void WriteSharePersonValue()
        {

            if (VirtualPerson.FamilyExpand != null &&VirtualPerson.FamilyExpand.ConstructMode != eConstructMode.Family)
            {
                currentIndex += 4;
                return;

            }
            List<Person> spersons = VirtualPerson.SharePersonList;//得到户对应的共有人
            //object obj = GetRangeToValue("A" + currentIndex, "F" + currentIndex);
            //if(obj!=null)
            //{
            //    string[] percount = obj.ToString().Split('（');
            //    string value = "";
            //    if(percount.Count()<2)
            //    {
            //        percount= obj.ToString().Split('(');
            //        value = percount[0] + "(" + spersons.Count + percount[1];
            //    }
            //    else
            //        value= percount[0] + "（" + spersons.Count + percount[1];
            //    InitalizeRangeValue("A" + currentIndex, "F" + currentIndex,value);
            //}
            //currentIndex++;
            //InitalizeRangeValue("A" + currentIndex, "F" + currentIndex, "共(" + spersons.Count + ")人");//共多少人

            List<Person> sharePersons = SortSharePerson(spersons, VirtualPerson.Name);//排序共有人，并返回人口集合

            if (sharePersons.Count > 1)
                InsertRowCell(currentIndex, sharePersons.Count - 1);
            foreach (Person person in sharePersons)
            {
                InitalizeRangeValue("A" + currentIndex, "D" + currentIndex, person.Relationship);//家庭关系 
                InitalizeRangeValue("E" + currentIndex, "H" + currentIndex, person.Name);//姓名
                string birthDay = "";

                if (!string.IsNullOrEmpty(person.ICN))
                {
                    DateTime dt = Library.Business.ToolICN.GetBirthdayInNotCheck(person.ICN);
                    if (dt.Year > 1850)
                    {
                        birthDay = Library.Business.ToolDateTime.GetShortDateString(dt);
                    }
                }
                else
                {
                    birthDay = "";
                }
                InitalizeRangeValue("I" + currentIndex, "I" + currentIndex, birthDay);//出身年月
                InitalizeRangeValue("J" + currentIndex, "L" + currentIndex, person.ICN);//身份证号码 
                if (person.Name == VirtualPerson.Name && person.ICN == VirtualPerson.Number)
                    InitalizeRangeValue("M" + currentIndex, "O" + currentIndex, VirtualPerson.Telephone);//联系电话
                else
                    InitalizeRangeValue("M" + currentIndex, "O" + currentIndex, "");//联系电话
                InitalizeRangeValue("P" + currentIndex, "Q" + currentIndex, person.Comment);//备注 
                currentIndex++;
            }
            currentIndex = currentIndex + (sharePersons.Count < 1 ? 4 : 3);
        }

        /// <summary>
        /// 对共有人排序
        /// </summary>
        /// <param name="personCollection"></param>
        /// <param name="houseName"></param>
        /// <returns></returns>
        private List<Person> SortSharePerson(List<Person> personCollection, string houseName)
        {
            List<Person> sharePersonCollection = new List<Person>();
            foreach (Person person in personCollection)
            {
                if (person.Name == houseName)
                {
                    sharePersonCollection.Add(person);
                }
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

        #endregion

        #region Contractland

        /// <summary>
        /// 填写承包地块信息
        /// </summary>
        private void WriteLandInformation()
        {
            if (Lands == null || Lands.Count < 1)
            {
                return;
            }
            if (Lands.Count > 1)
                InsertRowCell(currentIndex, Lands.Count - 1);
            int i = 1;
            foreach (var item in Lands)
            {
                //string landNumber = item.CadastralNumber;
                //if (item.CadastralNumber.Length > AgricultureSetting.AgricultureLandNumberMedian&&SystemSet.LandNumericFormatSet)
                //{
                //    landNumber = item.CadastralNumber.Substring(SystemSet.LandNumericFormatValueSet);
                //}

                string landNumber = ContractLand.GetLandNumber(item.CadastralNumber);
                if (landNumber.Length > AgricultureSetting.AgricultureLandNumberMedian)
                {
                    landNumber = landNumber.Substring(AgricultureSetting.AgricultureLandNumberMedian);
                }
                InitalizeRangeValue("A" + currentIndex, "A" + currentIndex, i.ToString());//序号
                InitalizeRangeValue("B" + currentIndex, "B" + currentIndex, item.Name);//地块名称
                InitalizeRangeValue("C" + currentIndex, "E" + currentIndex, landNumber);//地块编号
                InitalizeRangeValue("F" + currentIndex, "F" + currentIndex, GetEnumDesp<eLandCategoryType>(item.LandCategory));//地类
                InitalizeRangeValue("H" + currentIndex, "H" + currentIndex, item.ActualArea.ToString("f2"));//实测面积
                InitalizeRangeValue("I" + currentIndex, "J" + currentIndex, item.NeighborEast);//东
                InitalizeRangeValue("K" + currentIndex, "L" + currentIndex, item.NeighborSouth);//南
                InitalizeRangeValue("M" + currentIndex, "N" + currentIndex, item.NeighborWest);//西
                InitalizeRangeValue("O" + currentIndex, "Q" + currentIndex, item.NeighborNorth);//北

                currentIndex += 1;
                i++;
            }
            double tablearea = 0.0, aware = 0.0, actularea = 0.0;
            Lands.ForEach(c => tablearea += c.TableArea.Value);
            Lands.ForEach(c => aware += c.AwareArea);
            Lands.ForEach(c => actularea += c.ActualArea);

            InitalizeRangeValue("B" + currentIndex, "B" + currentIndex, "地块数量：" + Lands.Count);
            InitalizeRangeValue("H" + currentIndex, "H" + currentIndex, actularea.ToString("f2"));
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
                var constructMode = new eConstructMode();
                Enum.TryParse<eConstructMode>(land.ConstructMode, out constructMode);
                if (constructMode != eConstructMode.Family)
                {
                    continue;
                }
                landCollection.Add(land);
            }
            lands.Clear();
            return landCollection;
        }

        #endregion

        #region Concord

        /// <summary>
        /// 填写合同信息
        /// </summary>
        private void WriteConcordInformation()
        {
            WriteSharePersonValue();

            WriteLandInformation();
        }

        #endregion

        #region OtherInformation

        /// <summary>
        /// 写表头信息
        /// </summary>
        private void WriteTitleInformation()
        {
            //InitalizeRangeValue("B3", "D3", concord.SenderName);//发包方名称
            //InitalizeRangeValue("B2", "D2", dic.translante(concord.SenderName));//发包方名称
            int familyNumber = 0;
            Int32.TryParse(VirtualPerson.FamilyNumber, out familyNumber);
            string number = string.Format("{0:D4}", familyNumber);
            InitalizeRangeValue("O5", "Q5", number);//承包方编码
            InitalizeRangeValue("B5", "F5", VirtualPerson.Name);//承包方姓名
            string vpName = VirtualPerson.Name;
            if (vpName == null || vpName == "")
                vpName = dic.translante(VirtualPerson.Name);
            InitalizeRangeValue("B4", "F4", vpName);//承包方姓名

        }

        #endregion

        #region other
        /// <summary>
        /// 根据枚举值获取枚举描述
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="enumValueStr">枚举值字符串</param>
        /// <returns></returns>
        private string GetEnumDesp<TEnum>(string enumValueStr) where TEnum : struct
        {
            TEnum tEnum = new TEnum();
            Enum.TryParse<TEnum>(enumValueStr, out tEnum);

            return EnumNameAttribute.GetDescription(tEnum);
        }
        #endregion

        #endregion
    }
}
