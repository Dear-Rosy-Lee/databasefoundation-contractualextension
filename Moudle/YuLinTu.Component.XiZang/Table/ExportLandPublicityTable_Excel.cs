using System;
using System.Collections.Generic;
using System.Linq;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.XiZang
{
    /// <summary>
    /// 导出公示结果归户表(excel)
    /// </summary>
    [Serializable]
    public class ExportLandPublicityTable_Excel : WordBase
    {
        #region 字段
        private string fileName = string.Empty;//文件名称
        private string errorInformation = string.Empty;//错误信息       
        private GetDictionary dic;//藏文字典
        private int currentIndex;//当前行

        #endregion

        #region 属性
        public VirtualPerson VirtualPerson { get; set; }//承包方
        public Zone CurrentZone { get; set; }//当前地域

        public List<ContractLand> Lands { get; set; }//承包地块


        public ContractConcord Concord { get; set; }//承包合同

        public CollectivityTissue Tissue { get; set; }



        #endregion

        #region Ctor
        /// <summary>
        /// 默认构造方法
        /// </summary>
        public ExportLandPublicityTable_Excel(string fileName, string dictoryName)
        {
            this.fileName = fileName;
            this.currentIndex = 20;
            dic = new GetDictionary(dictoryName);
            dic.Read();
        }

        #endregion

        #region 方法
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

                BeginWrite();
            }
            catch (Exception ex)
            {
                errorInformation = ex.Message;
                return;
            }
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
            if (VirtualPerson == null)
            {
                errorInformation = "承包方为空";
                return false;
            }

            //检查地域数据
            if (CurrentZone == null)
            {
                errorInformation = "行政地域获取失败";
                return false;
            }
            Lands = SortLandCollection(Lands);
            return true;
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
                landCollection.Add(land);
            }
            lands.Clear();
            return landCollection;
        }
        #endregion

        #region 开始生成Excel

        private bool BeginWrite()
        {

            try
            {
                WriteTitleInformation();
                WriteLandInformation();
                WritePersonInformation();
                WritePublicyInformation();
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 写表头信息
        /// </summary>
        private void WriteTitleInformation()
        {
            int familyNumber = 0;
            Int32.TryParse(VirtualPerson.FamilyNumber, out familyNumber);
            List<ContractLand> cLandList = Lands.FindAll(l => l.LandCategory == ((int)eLandCategoryType.ContractLand).ToString());
            double cArea = 0;
            cLandList.ForEach(l => cArea += l.ActualArea);
            double tArea = 0.0;
            cLandList.ForEach(l => tArea += (l.TableArea != null && l.TableArea.HasValue) ? l.TableArea.Value : 0.0);
            double otherArea = 0.0;
            double otherTablearea = 0.0;
            List<ContractLand> otherLandList = Lands.FindAll(l => l.LandCategory != ((int)eLandCategoryType.ContractLand).ToString());

            otherLandList.ForEach(l => otherArea += l.ActualArea);
            otherLandList.ForEach(l => otherTablearea += (l.TableArea != null && l.TableArea.HasValue) ? l.TableArea.Value : 0.0);


            // QA377 - 合同/权证编码、承包起止日期和承包方式都应从承包合同获取

            //string warrantNumber = VirtualPerson != null ? (!string.IsNullOrEmpty(VirtualPerson.FamilyExpand.WarrantNumber) ? VirtualPerson.FamilyExpand.WarrantNumber : "") : "";
            //// 建民拍板：合同权证编码只从承包方的二轮承包信息读取
            //string number = string.IsNullOrEmpty(warrantNumber) ? "/" : warrantNumber;

            InitalizeRangeValue("C3", "G3", ConvertNull(Tissue.Name));//发包方名称          
            ;
            InitalizeRangeValue("L3", "L3", ConvertNull(Tissue.LawyerName));//发包方负责人

            string concordNumber = Concord.ConcordNumber;
            InitalizeRangeValue("C4", "G5", ConvertNull(concordNumber));//合同权证编码

            string concordStartDate = string.Empty;
            string concordEndDate = string.Empty;
            concordStartDate = Concord.ArableLandStartTime == null ? "" : ((DateTime)Concord.ArableLandStartTime).ToLongDateString();
            concordEndDate = Concord.ArableLandEndTime == null ? "" : ((DateTime)Concord.ArableLandEndTime).ToLongDateString();

            if (!string.IsNullOrEmpty(concordStartDate))
                InitalizeRangeValue("C7", "G7", concordStartDate + "至");//承包起始日期
            if (!string.IsNullOrEmpty(concordEndDate))
                InitalizeRangeValue("C9", "G9", concordEndDate);//承包终止日期


            #region 设置承包方式

            int consturctNumber = 0;
            Int32.TryParse(Concord.ArableLandType, out consturctNumber);
            eConstructMode constructMode = (eConstructMode)consturctNumber;
            object temp = (EnumNameAttribute.GetDescription(constructMode) == "") ? "家庭承包" : EnumNameAttribute.GetDescription(constructMode);

            string originalStr = GetRangeToValue("I4", "L9") == null ? "" : GetRangeToValue("I4", "L9").ToString(); ;
            string newStr = ConvertStr(originalStr, temp == null ? "" : temp.ToString());
            InitalizeRangeValue("I4", "L9", newStr);    // 承包方式

            #endregion

            InitalizeRangeValue("D11", "G11", ConvertNull(VirtualPerson.Name));//承包方代表姓名
            InitalizeRangeValue("K10", "L11", ConvertNull(VirtualPerson.Telephone));//联系电话
            temp = EnumNameAttribute.GetDescription(VirtualPerson.CardType);
            originalStr = GetRangeToValue("C12", "G13") == null ? "" : GetRangeToValue("C12", "G13").ToString();
            newStr = ConvertStr(originalStr, temp == null ? "" : temp.ToString());
            InitalizeRangeValue("C12", "G13", newStr);//证件类型
            InitalizeRangeValue("I12", "L13", ConvertNull(VirtualPerson.Number));//证件类型
            InitalizeRangeValue("C14", "I15", ConvertNull(VirtualPerson.Address));//承包方地址
            InitalizeRangeValue("L14", "L15", ConvertNull(VirtualPerson.PostalNumber));//邮政编码
            InitalizeRangeValue("C17", "L17", "共" + ConvertNull(cLandList.Count.ToString()) + "块 " + ConvertNull(cArea.ToString("f2")) + "亩");//承包地块数
        }
        /// <summary>
        /// 写地块信息
        /// </summary>
        private void WriteLandInformation()
        {
            // 只输出承包地块的信息，排除“非承包地块”信息
            List<ContractLand> cLandList = Lands.FindAll(l => l.LandCategory == ((int)eLandCategoryType.ContractLand).ToString());
            InsertRowCell(currentIndex, cLandList.Count > 1 ? cLandList.Count - 1 : 0);
            for (int i = 0; i < cLandList.Count; i++)
            {
                string landNumber = cLandList[i].LandNumber;
                var systemSetting = SystemSetDefine.GetIntence();
                if (systemSetting.LandNumericFormatSet && landNumber.Length > systemSetting.LandNumericFormatValueSet)
                {
                    landNumber = landNumber.Substring(systemSetting.LandNumericFormatValueSet);
                }
                InitalizeRangeValue("A" + currentIndex, "B" + currentIndex, ConvertNull(cLandList[i].Name) + "(" + ConvertNull(landNumber) + ")");//地块名称和地块编码

                InitalizeRangeValue("C" + currentIndex, "D" + currentIndex, string.Format("绊半︽\n东:{0}\n栋︽\n南:{1}\n乘搬︽\n西:{2}\n锤爸︽\n北:{3}",
                                    ConvertNull(cLandList[i].NeighborWest),
                                    ConvertNull(cLandList[i].NeighborSouth),
                                    ConvertNull(cLandList[i].NeighborEast),
                                    ConvertNull(cLandList[i].NeighborNorth)));//地块四至
                InitalizeRangeValue("E" + currentIndex, "E" + currentIndex, ConvertNull(cLandList[i].TableArea.Value.ToString("f2")));//合同面积
                InitalizeRangeValue("F" + currentIndex, "F" + currentIndex, ConvertNull(cLandList[i].ActualArea.ToString("f2")));//实测面积
                InitalizeRangeValue("G" + currentIndex, "G" + currentIndex, ConvertNull(GetEnumDesp<eLandPurposeType>(cLandList[i].Purpose)));//土地用途
                InitalizeRangeValue("H" + currentIndex, "I" + currentIndex, ConvertNull(GetEnumDesp<eContractLandLevel>(cLandList[i].LandLevel)));//地力等级
                InitalizeRangeValue("J" + currentIndex, "K" + currentIndex, ConvertNull(cLandList[i].LandName));//土地利用类型
                InitalizeRangeValue("L" + currentIndex, "L" + currentIndex, ConvertNull(cLandList[i].Comment));//备注
                currentIndex++;
            }
        }
        /// <summary>
        /// 写家庭成员
        /// </summary>
        private void WritePersonInformation()
        {
            int index = currentIndex;
            if (VirtualPerson.FamilyExpand != null && ((int)VirtualPerson.FamilyExpand.ConstructMode).ToString() != "110")
            {
                currentIndex += 6;
                return;

            }
            List<Person> persons = SortSharePerson(VirtualPerson.SharePersonList, VirtualPerson.Name);
            currentIndex++;
            InitalizeRangeValue("L" + currentIndex, "L" + currentIndex, "共" + persons.Count + "人");//共多少人
            currentIndex = currentIndex + 3;
            if (persons.Count > 2)
                InsertRowCell(currentIndex, persons.Count - 2);
            for (int i = 0; i < persons.Count; i++)
            {
                Person person = persons[i];
                InitalizeRangeValue("A" + currentIndex, "B" + currentIndex, ConvertNull(person.Name));//姓名
                InitalizeRangeValue("C" + currentIndex, "E" + currentIndex, ConvertNull(person.Relationship));//家庭关系 
                InitalizeRangeValue("F" + currentIndex, "J" + currentIndex, ConvertNull(person.ICN));//身份证号码 
                InitalizeRangeValue("K" + currentIndex, "L" + currentIndex, ConvertNull(person.Comment));//备注 
                currentIndex++;
            }
            if (persons.Count < 2)
                currentIndex += (2 - persons.Count);
        }
        /// <summary>
        /// 填写其他信息
        /// </summary>
        private void WritePublicyInformation()
        {
            if (Concord == null)
            {
                return;
            }
            string surveyDate = (Concord.PublicityResultDate != null && Concord.PublicityResultDate.HasValue) ? ToolDateTime.GetLongDateString(Concord.PublicityResultDate.Value) : "";
            string checkDate = (Concord.PublicityCheckDate != null && Concord.PublicityCheckDate.HasValue) ? ToolDateTime.GetLongDateString(Concord.PublicityCheckDate.Value) : "";
            string publicDate = (Concord.PublicityDate != null && Concord.PublicityDate.HasValue) ? ToolDateTime.GetLongDateString(Concord.PublicityDate.Value) : "";
            InitalizeRangeValue("B" + currentIndex, "L" + currentIndex, ConvertNull(Concord.PublicityChronicle));//公示记事
            currentIndex += 2;
            InitalizeRangeValue("C" + currentIndex, "F" + currentIndex, ConvertNull(Concord.PublicityChroniclePerson));//调查员
            InitalizeRangeValue("I" + currentIndex, "L" + currentIndex, ConvertNull(publicDate));//调查日期
            currentIndex++;
            InitalizeRangeValue("B" + currentIndex, "L" + currentIndex, ConvertNull(Concord.PublicityResult));//承包方 (代表)对公示结果的意见
            currentIndex += 2;
            InitalizeRangeValue("D" + currentIndex, "F" + currentIndex, ConvertNull(Concord.PublicityContractor));//承包方代表
            InitalizeRangeValue("I" + currentIndex, "L" + currentIndex, ConvertNull(surveyDate));//调查日期
            currentIndex++;
            InitalizeRangeValue("B" + currentIndex, "L" + currentIndex, ConvertNull(Concord.PublicityCheckOpinion));//公示结  果审核  意  见
            currentIndex += 2;
            InitalizeRangeValue("C" + currentIndex, "F" + currentIndex, ConvertNull(Concord.PublicityCheckPerson));//审核人
            InitalizeRangeValue("I" + currentIndex, "L" + currentIndex, ConvertNull(checkDate));//审核日期
        }

        /// <summary>
        /// 对共有人进行排序
        /// </summary>
        /// <param name="personCollection"></param>
        /// <returns></returns>
        private List<Person> SortSharePerson(List<Person> personCollection, string familyName)
        {
            List<Person> sharePersonCollection = new List<Person>();
            foreach (Person person in personCollection)
            {
                if (person.Name == familyName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            foreach (Person person in personCollection)
            {
                if (person.Name != familyName)
                {
                    sharePersonCollection.Add(person);
                }
            }
            return sharePersonCollection;
        }
        #endregion

        #region Common


        /// <summary>
        /// 勾选
        /// </summary>
        /// <param name="originalStr">原始的字符串</param>
        /// <param name="name">承包方式</param>
        /// <returns></returns>
        private string ConvertStr(string originalStr, string name)
        {
            string str = string.Empty;
            string[] china = originalStr.Split('□');
            if (name == "")
            {
                str = originalStr;
                return str;
            }
            for (int i = 0; i < china.Count(); i++)
            {
                if (china[i] == "")
                    continue;
                if (china[i].Contains(name) || name.Contains(china[i].Trim()))
                {
                    str += "√" + china[i];
                }
                else
                {
                    if (i == 0)
                    {
                        str += china[i];
                    }
                    else
                    {
                        str += "□" + china[i];
                    }
                }
            };
            return str;
        }

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

        private string ConvertNull(string originalStr)
        {
            return String.IsNullOrEmpty(originalStr) ? "/" : originalStr;
        }

        #endregion
    }
}
