using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using YuLinTu.Data;
using YuLinTu;
using System.Xml.Linq;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 承包方扩展
    /// </summary>
    [Serializable]
    public class VirtualPersonExpand : NotifyCDObject  //(修改前)YltEntityIDName
    {
        #region Filds

        private Guid id;
        private string name;
        private string houseHolderName;
        private string laborNumber;
        private string isSourceContractor;
        private string contractorNumber;
        private string farmerNature;
        private string moveFormerlyLandType;
        private string moveFormerlyLandArea;
        private string firstContractorPersonNumber;
        private string firstContractArea;
        private string secondContractorPersonNumber;
        private string secondExtensionPackArea;
        private string foodCropArea;
        private string allocationPerson;
        private eBusinessStatus businessStatus;
        private eContractorType contractorType;
        private string description;
        private string checkInformation;
        private bool? ischecked;
        private string surveyPerson;
        private DateTime? surveyDate;
        private string surveyChronicle;
        private string checkPerson;
        private DateTime? checkDate;
        private string checkOpinion;
        private string publicityChronicle;
        private string publicityChroniclePerson;
        private DateTime? publicityDate;
        private string publicityCheckPerson;
        private string concordNumber;
        private string warrantNumber;
        private DateTime? concordStartTime;
        private DateTime? concordEndTime;
        private eConstructMode constructType;
        private string equityNumber;
        private double equityValue;
        private double equityArea;
        private string collectivityName;
        private DateTime? quantifyTime;
        private DateTime? equityStartTime;
        private DateTime? equityEndTime;
        private string issueUnit;
        private DateTime? issueTime;
        private string registrationAuthority;
        private DateTime? registrationTime;
        private string comment;
        //青海用
        private double secondConcordTotalArea;
        private int secondConcordTotalLandCount;
        //扩展字段
        private string extendName;
        #endregion

        #region Properties

        /// <summary>
        /// ID
        /// </summary>
        public Guid ID
        {
            get { return id; }
            set { id = value; NotifyPropertyChanged("ID"); }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }

        /// <summary>
        ///承包方名称
        /// </summary>
        public string HouseHolderName
        {
            get { return houseHolderName; }
            set { houseHolderName = value; NotifyPropertyChanged("HouseHolderName"); }
        }

        /// <summary>
        /// 总劳力数
        /// </summary>
        public string LaborNumber
        {
            get { return laborNumber; }
            set { laborNumber = value; NotifyPropertyChanged("LaborNumber"); }
        }

        /// <summary>
        /// 是否是原承包户
        /// </summary>
        public string IsSourceContractor
        {
            get { return isSourceContractor; }
            set { isSourceContractor = value; NotifyPropertyChanged("IsSourceContractor"); }
        }

        /// <summary>
        /// 现承包人数
        /// </summary>
        public string ContractorNumber
        {
            get { return contractorNumber; }
            set { contractorNumber = value; NotifyPropertyChanged("ContractorNumber"); }
        }

        /// <summary>
        /// 农户性质
        /// </summary>
        public string FarmerNature
        {
            get { return farmerNature; }
            set { farmerNature = value; NotifyPropertyChanged("FarmerNature"); }
        }

        /// <summary>
        /// 迁入前土地类型
        /// </summary>
        public string MoveFormerlyLandType
        {
            get { return moveFormerlyLandType; }
            set { moveFormerlyLandType = value; NotifyPropertyChanged("MoveFormerlyLandType"); }
        }

        /// <summary>
        /// 迁入前土地面积
        /// </summary>
        public string MoveFormerlyLandArea
        {
            get { return moveFormerlyLandArea; }
            set { moveFormerlyLandArea = value; NotifyPropertyChanged("MoveFormerlyLandArea"); }
        }

        /// <summary>
        /// 一轮承包人数
        /// </summary>
        public string FirstContractorPersonNumber
        {
            get { return firstContractorPersonNumber; }
            set { firstContractorPersonNumber = value; NotifyPropertyChanged("FirstContractorPersonNumber"); }
        }

        /// <summary>
        /// 一轮承包面积
        /// </summary>
        public string FirstContractArea
        {
            get { return firstContractArea; }
            set { firstContractArea = value; NotifyPropertyChanged("FirstContractArea"); }
        }

        /// <summary>
        /// 二轮承包人数
        /// </summary>
        public string SecondContractorPersonNumber
        {
            get { return secondContractorPersonNumber; }
            set { secondContractorPersonNumber = value; NotifyPropertyChanged("SecondContractorPersonNumber"); }
        }

        /// <summary>
        /// 二轮延包面积
        /// </summary>
        public string SecondExtensionPackArea
        {
            get { return secondExtensionPackArea; }
            set { secondExtensionPackArea = value; NotifyPropertyChanged("SecondExtensionPackArea"); }
        }

        /// <summary>
        /// 粮食种植面积
        /// </summary>
        public string FoodCropArea
        {
            get { return foodCropArea; }
            set { foodCropArea = value; NotifyPropertyChanged("FoodCropArea"); }
        }

        /// <summary>
        /// 实际分配人数
        /// </summary>
        public string AllocationPerson
        {
            get { return allocationPerson; }
            set { allocationPerson = value; NotifyPropertyChanged("AllocationPerson"); }
        }

        /// <summary>
        /// 业务状态
        /// </summary>
        public eBusinessStatus BusinessStatus
        {
            get { return businessStatus; }
            set { businessStatus = value; NotifyPropertyChanged("BusinessStatus"); }
        }

        /// <summary>
        /// 承包方类型
        /// </summary>
        public eContractorType ContractorType
        {
            get { return contractorType; }
            set { contractorType = value; NotifyPropertyChanged("ContractorType"); }
        }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; NotifyPropertyChanged("Description"); }
        }

        /// <summary>
        /// 审核信息
        /// </summary>
        public string CheckInformation
        {
            get { return checkInformation; }
            set { checkInformation = value; NotifyPropertyChanged("CheckInformation"); }
        }

        /// <summary>
        /// 审核
        /// </summary>
        public bool? Checked
        {
            get { return ischecked; }
            set { ischecked = value; NotifyPropertyChanged("Checked"); }
        }

        /// <summary>
        /// 调查员
        /// </summary>
        public string SurveyPerson
        {
            get { return surveyPerson; }
            set { surveyPerson = value; NotifyPropertyChanged("SurveyPerson"); }
        }

        /// <summary>
        /// 调查日期
        /// </summary>
        public DateTime? SurveyDate
        {
            get { return surveyDate; }
            set { surveyDate = value; NotifyPropertyChanged("SurveyDate"); }
        }

        /// <summary>
        /// 调查记事
        /// </summary>
        public string SurveyChronicle
        {
            get { return surveyChronicle; }
            set { surveyChronicle = value; NotifyPropertyChanged("SurveyChronicle"); }
        }

        /// <summary>
        /// 审核人
        /// </summary>
        public string CheckPerson
        {
            get { return checkPerson; }
            set { checkPerson = value; NotifyPropertyChanged("CheckPerson"); }
        }

        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime? CheckDate
        {
            get { return checkDate; }
            set { checkDate = value; NotifyPropertyChanged("CheckDate"); }
        }

        /// <summary>
        /// 审核意见
        /// </summary>
        public string CheckOpinion
        {
            get { return checkOpinion; }
            set { checkOpinion = value; NotifyPropertyChanged("CheckOpinion"); }
        }

        /// <summary>
        /// 公示记事
        /// </summary>
        public string PublicityChronicle
        {
            get { return publicityChronicle; }
            set { publicityChronicle = value; NotifyPropertyChanged("PublicityChronicle"); }
        }

        /// <summary>
        /// 公示记事人
        /// </summary>
        public string PublicityChroniclePerson
        {
            get { return publicityChroniclePerson; }
            set { publicityChroniclePerson = value; NotifyPropertyChanged("PublicityChroniclePerson"); }
        }

        /// <summary>
        /// 公示日期
        /// </summary>
        public DateTime? PublicityDate
        {
            get { return publicityDate; }
            set { publicityDate = value; NotifyPropertyChanged("PublicityDate"); }
        }

        /// <summary>
        /// 公示审核人
        /// </summary>
        public string PublicityCheckPerson
        {
            get { return publicityCheckPerson; }
            set { publicityCheckPerson = value; NotifyPropertyChanged("PublicityCheckPerson"); }
        }

        /// <summary>
        /// 二轮承包合同编号
        /// </summary>
        public string ConcordNumber
        {
            get { return concordNumber; }
            set { concordNumber = value; NotifyPropertyChanged("ConcordNumber"); }
        }

        /// <summary>
        /// 二轮经营权证编号
        /// </summary>
        public string WarrantNumber
        {
            get { return warrantNumber; }
            set { warrantNumber = value; NotifyPropertyChanged("WarrantNumber"); }
        }

        /// <summary>
        /// 二轮承包开始日期
        /// </summary>
        public DateTime? ConcordStartTime
        {
            get { return concordStartTime; }
            set { concordStartTime = value; NotifyPropertyChanged("ConcordStartTime"); }
        }

        /// <summary>
        /// 二轮承包结束日期
        /// </summary>
        public DateTime? ConcordEndTime
        {
            get { return concordEndTime; }
            set { concordEndTime = value; NotifyPropertyChanged("ConcordEndTime"); }
        }

        /// <summary>
        /// 承包方式
        /// </summary>
        public eConstructMode ConstructMode
        {
            get { return constructType; }
            set { constructType = value; NotifyPropertyChanged("ConstructMode"); }
        }

        /// <summary>
        /// 股权证号
        /// </summary>
        public string EquityNumber
        {
            get { return equityNumber; }
            set { equityNumber = value; NotifyPropertyChanged("EquityNumber"); }
        }

        /// <summary>
        /// 入股份数
        /// </summary>
        public double EquityValue
        {
            get { return equityValue; }
            set { equityValue = value; NotifyPropertyChanged("EquityValue"); }
        }

        /// <summary>
        /// 入股面积
        /// </summary>
        public double EquityArea
        {
            get { return equityArea; }
            set { equityArea = value; NotifyPropertyChanged("EquityArea"); }
        }

        /// <summary>
        /// 集体经济组织名称
        /// </summary>
        public string CollectivityName
        {
            get { return collectivityName; }
            set { collectivityName = value; NotifyPropertyChanged("CollectivityName"); }
        }

        /// <summary>
        /// 量化时间
        /// </summary>
        public DateTime? QuantifyTime
        {
            get { return quantifyTime; }
            set { quantifyTime = value; NotifyPropertyChanged("QuantifyTime"); }
        }

        /// <summary>
        /// 股权开始时间
        /// </summary>
        public DateTime? EquityStartTime
        {
            get { return equityStartTime; }
            set { equityStartTime = value; NotifyPropertyChanged("EquityStartTime"); }
        }

        /// <summary>
        /// 股权结束时间
        /// </summary>
        public DateTime? EquityEndTime
        {
            get { return equityEndTime; }
            set { equityEndTime = value; NotifyPropertyChanged("EquityEndTime"); }
        }

        /// <summary>
        /// 发证单位
        /// </summary>
        public string IssueUnit
        {
            get { return issueUnit; }
            set { issueUnit = value; NotifyPropertyChanged("IssueUnit"); }
        }

        /// <summary>
        /// 发证时间
        /// </summary>
        public DateTime? IssueTime
        {
            get { return issueTime; }
            set { issueTime = value; NotifyPropertyChanged("IssueTime"); }
        }

        /// <summary>
        /// 登记机关
        /// </summary>
        public string RegistrationAuthority
        {
            get { return registrationAuthority; }
            set { registrationAuthority = value; NotifyPropertyChanged("RegistrationAuthority"); }
        }

        /// <summary>
        /// 登记时间
        /// </summary>
        public DateTime? RegistrationTime
        {
            get { return registrationTime; }
            set { registrationTime = value; NotifyPropertyChanged("RegistrationTime"); }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment
        {
            get { return comment; }
            set { comment = value; NotifyPropertyChanged("Comment"); }
        }

        /// <summary>
        /// 二轮承包合同总面积-青海用
        /// </summary>
        public double SecondConcordTotalArea
        {
            get { return secondConcordTotalArea; }
            set { secondConcordTotalArea = value; NotifyPropertyChanged("SecondConcordTotalArea"); }
        }

        /// <summary>
        /// 二轮承包合同地块总数-青海用
        /// </summary>
        public int SecondConcordTotalLandCount
        {
            get { return secondConcordTotalLandCount; }
            set { secondConcordTotalLandCount = value; NotifyPropertyChanged("SecondConcordTotalLandCount"); }
        }

        /// <summary>
        /// 扩展字段
        /// </summary>
        public string ExtendName
        {
            get { return extendName; }
            set { extendName = value; NotifyPropertyChanged("ExtendName"); }
        }

        /// <summary>
        /// 变化情况
        /// </summary>
        public string ChangeComment
        {
            get { return changeComment; }
            set { changeComment = value; NotifyPropertyChanged("ChangeComment"); }
        }
        private string changeComment;

        #endregion

        #region Ctor

        public VirtualPersonExpand()
        {
            BusinessStatus = eBusinessStatus.UnKnown;
            ContractorType = eContractorType.Farmer;  //农户
            constructType = eConstructMode.Family; //家庭承包
        }

        #endregion

        #region Method

        /// <summary>
        /// 创建拓展信息
        /// </summary>
        public static VirtualPersonExpand CreateExpandByXml(string xml)
        {
            if (xml == null || xml.Length < 0)
            {
                return null;
            }
            //2011-04-28 16:11:07 郑建(Roc Zheng) 修改因文本编码类型导致的XML读取失败的BUG，但是存在效率的遗留问题，期望有更好的解决办法
            XElement root = null;
            try
            {
                if (xml.ToCharArray()[0] == 65279)
                {
                    xml = xml.Remove(0, 1);
                }
                root = XElement.Parse(xml);
            }
            catch (System.Xml.XmlException)
            {
                root = InitalizeRootInformtaion(xml);
            }
            return GetLandFromXElement(root);
        }

        /// <summary>
        /// 初始化元素信息
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static XElement InitalizeRootInformtaion(string xml)
        {
            XElement root = null;
            try
            {
                //使用内存流对编码类型进行重新设置，转换为正常可使用的编码(默认)
                using (System.IO.Stream stream = new System.IO.MemoryStream())
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(stream))
                    {
                        writer.Write(xml);

                        stream.Position = 0;    //重置流的位置
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                        {
                            root = XElement.Load(reader);
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return root;
        }

        /// <summary>
        /// 从Xml元素中获取地块信息
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        private static VirtualPersonExpand GetLandFromXElement(XElement el)
        {
            if (el == null)
            {
                return null;
            }
            VirtualPersonExpand land = new VirtualPersonExpand();
            XAttribute attr = el.Attribute("HouseHolderName");
            land.HouseHolderName = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("BusinessStatus");//业务状态
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                try
                {
                    object obj = Enum.Parse(typeof(eBusinessStatus), attr.Value);
                    land.BusinessStatus = obj == null ? eBusinessStatus.UnKnown : (eBusinessStatus)obj;
                }
                catch (SystemException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }

            attr = el.Attribute("LaborNumber");//总劳力数
            land.LaborNumber = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("AllocationPerson");//实际分配人数
            land.AllocationPerson = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("FarmerNature");//农户性质
            land.FarmerNature = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("IsSourceContractor");
            land.IsSourceContractor = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("ContractorNumber");
            land.ContractorNumber = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("MoveFormerlyLandType");
            land.MoveFormerlyLandType = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("MoveFormerlyLandArea");
            land.MoveFormerlyLandArea = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("FirstContractorPersonNumber");
            land.FirstContractorPersonNumber = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("FirstContractArea");
            land.FirstContractArea = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("SecondContractorPersonNumber");
            land.SecondContractorPersonNumber = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("SecondExtensionPackArea");
            land.SecondExtensionPackArea = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("FoodCropArea");
            land.FoodCropArea = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("Description");
            land.Description = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("CheckInformation");
            land.CheckInformation = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("SurveyPerson");
            land.SurveyPerson = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("SurveyDate");
            DateTime date = DateTime.Now;
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                DateTime.TryParse(attr.Value, out date);
                land.SurveyDate = date;
            }

            attr = el.Attribute("SurveyChronicle");
            land.SurveyChronicle = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("CheckPerson");
            land.CheckPerson = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("CheckDate");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                DateTime.TryParse(attr.Value, out date);
                land.CheckDate = date;
            }

            attr = el.Attribute("CheckOpinion");
            land.CheckOpinion = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("PublicityChronicle");
            land.PublicityChronicle = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("PublicityChroniclePerson");
            land.PublicityChroniclePerson = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("PublicityCheckPerson");
            land.PublicityCheckPerson = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("PublicityDate");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                DateTime.TryParse(attr.Value, out date);
                land.PublicityDate = date;
            }

            attr = el.Attribute("ContractorType");
            object objValue = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : Enum.Parse(typeof(eBusinessStatus), attr.Value);
            land.ContractorType = (attr == null || String.IsNullOrEmpty(attr.Value)) ? eContractorType.Farmer : (eContractorType)(objValue);

            attr = el.Attribute("Checked");
            if (attr == null || string.IsNullOrEmpty(attr.Value))
            {
                land.Checked = null;
            }
            else
            {
                bool check = false;
                Boolean.TryParse(attr.Value, out check);
                land.Checked = check;
            }

            attr = el.Attribute("ConcordNumber");
            land.ConcordNumber = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("WarrantNumber");
            land.WarrantNumber = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("ConstructType");
            object typeobj = EnumNameAttribute.GetValue(typeof(eConstructMode), ((attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value));
            land.ConstructMode = typeobj == null ? eConstructMode.Family : (eConstructMode)typeobj;

            attr = el.Attribute("ConcordStartTime");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                DateTime.TryParse(attr.Value, out date);
                land.ConcordStartTime = date;
            }

            attr = el.Attribute("ConcordEndTime");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                DateTime.TryParse(attr.Value, out date);
                land.ConcordEndTime = date;
            }

            attr = el.Attribute("EquityNumber");
            land.EquityNumber = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("CollectivityName");
            land.CollectivityName = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("EquityValue");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                double equityValue = 0.0;
                double.TryParse(attr.Value, out equityValue);
                land.EquityValue = equityValue;
            }

            attr = el.Attribute("EquityArea");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                double equityArea = 0.0;
                double.TryParse(attr.Value, out equityArea);
                land.EquityArea = equityArea;
            }

            attr = el.Attribute("QuantifyTime");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                DateTime.TryParse(attr.Value, out date);
                land.QuantifyTime = date;
            }

            attr = el.Attribute("EquityStartTime");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                DateTime.TryParse(attr.Value, out date);
                land.EquityStartTime = date;
            }

            attr = el.Attribute("EquityEndTime");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                DateTime.TryParse(attr.Value, out date);
                land.EquityEndTime = date;
            }

            attr = el.Attribute("IssueUnit");
            land.IssueUnit = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("IssueTime");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                DateTime.TryParse(attr.Value, out date);
                land.IssueTime = date;
            }

            attr = el.Attribute("RegistrationAuthority");
            land.RegistrationAuthority = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("RegistrationTime");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                DateTime.TryParse(attr.Value, out date);
                land.RegistrationTime = date;
            }

            attr = el.Attribute("Comment");
            land.Comment = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("ExtendName");
            land.ExtendName = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;
            return land;
        }

        #endregion
    }
}