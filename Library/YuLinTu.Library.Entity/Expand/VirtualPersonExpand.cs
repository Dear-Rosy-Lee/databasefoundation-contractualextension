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
    /// �а�����չ
    /// </summary>
    [Serializable]
    public class VirtualPersonExpand : NotifyCDObject  //(�޸�ǰ)YltEntityIDName
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
        //�ຣ��
        private double secondConcordTotalArea;
        private int secondConcordTotalLandCount;
        //��չ�ֶ�
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
        /// ����
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }

        /// <summary>
        ///�а�������
        /// </summary>
        public string HouseHolderName
        {
            get { return houseHolderName; }
            set { houseHolderName = value; NotifyPropertyChanged("HouseHolderName"); }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public string LaborNumber
        {
            get { return laborNumber; }
            set { laborNumber = value; NotifyPropertyChanged("LaborNumber"); }
        }

        /// <summary>
        /// �Ƿ���ԭ�а���
        /// </summary>
        public string IsSourceContractor
        {
            get { return isSourceContractor; }
            set { isSourceContractor = value; NotifyPropertyChanged("IsSourceContractor"); }
        }

        /// <summary>
        /// �ֳа�����
        /// </summary>
        public string ContractorNumber
        {
            get { return contractorNumber; }
            set { contractorNumber = value; NotifyPropertyChanged("ContractorNumber"); }
        }

        /// <summary>
        /// ũ������
        /// </summary>
        public string FarmerNature
        {
            get { return farmerNature; }
            set { farmerNature = value; NotifyPropertyChanged("FarmerNature"); }
        }

        /// <summary>
        /// Ǩ��ǰ��������
        /// </summary>
        public string MoveFormerlyLandType
        {
            get { return moveFormerlyLandType; }
            set { moveFormerlyLandType = value; NotifyPropertyChanged("MoveFormerlyLandType"); }
        }

        /// <summary>
        /// Ǩ��ǰ�������
        /// </summary>
        public string MoveFormerlyLandArea
        {
            get { return moveFormerlyLandArea; }
            set { moveFormerlyLandArea = value; NotifyPropertyChanged("MoveFormerlyLandArea"); }
        }

        /// <summary>
        /// һ�ֳа�����
        /// </summary>
        public string FirstContractorPersonNumber
        {
            get { return firstContractorPersonNumber; }
            set { firstContractorPersonNumber = value; NotifyPropertyChanged("FirstContractorPersonNumber"); }
        }

        /// <summary>
        /// һ�ֳа����
        /// </summary>
        public string FirstContractArea
        {
            get { return firstContractArea; }
            set { firstContractArea = value; NotifyPropertyChanged("FirstContractArea"); }
        }

        /// <summary>
        /// ���ֳа�����
        /// </summary>
        public string SecondContractorPersonNumber
        {
            get { return secondContractorPersonNumber; }
            set { secondContractorPersonNumber = value; NotifyPropertyChanged("SecondContractorPersonNumber"); }
        }

        /// <summary>
        /// �����Ӱ����
        /// </summary>
        public string SecondExtensionPackArea
        {
            get { return secondExtensionPackArea; }
            set { secondExtensionPackArea = value; NotifyPropertyChanged("SecondExtensionPackArea"); }
        }

        /// <summary>
        /// ��ʳ��ֲ���
        /// </summary>
        public string FoodCropArea
        {
            get { return foodCropArea; }
            set { foodCropArea = value; NotifyPropertyChanged("FoodCropArea"); }
        }

        /// <summary>
        /// ʵ�ʷ�������
        /// </summary>
        public string AllocationPerson
        {
            get { return allocationPerson; }
            set { allocationPerson = value; NotifyPropertyChanged("AllocationPerson"); }
        }

        /// <summary>
        /// ҵ��״̬
        /// </summary>
        public eBusinessStatus BusinessStatus
        {
            get { return businessStatus; }
            set { businessStatus = value; NotifyPropertyChanged("BusinessStatus"); }
        }

        /// <summary>
        /// �а�������
        /// </summary>
        public eContractorType ContractorType
        {
            get { return contractorType; }
            set { contractorType = value; NotifyPropertyChanged("ContractorType"); }
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; NotifyPropertyChanged("Description"); }
        }

        /// <summary>
        /// �����Ϣ
        /// </summary>
        public string CheckInformation
        {
            get { return checkInformation; }
            set { checkInformation = value; NotifyPropertyChanged("CheckInformation"); }
        }

        /// <summary>
        /// ���
        /// </summary>
        public bool? Checked
        {
            get { return ischecked; }
            set { ischecked = value; NotifyPropertyChanged("Checked"); }
        }

        /// <summary>
        /// ����Ա
        /// </summary>
        public string SurveyPerson
        {
            get { return surveyPerson; }
            set { surveyPerson = value; NotifyPropertyChanged("SurveyPerson"); }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public DateTime? SurveyDate
        {
            get { return surveyDate; }
            set { surveyDate = value; NotifyPropertyChanged("SurveyDate"); }
        }

        /// <summary>
        /// �������
        /// </summary>
        public string SurveyChronicle
        {
            get { return surveyChronicle; }
            set { surveyChronicle = value; NotifyPropertyChanged("SurveyChronicle"); }
        }

        /// <summary>
        /// �����
        /// </summary>
        public string CheckPerson
        {
            get { return checkPerson; }
            set { checkPerson = value; NotifyPropertyChanged("CheckPerson"); }
        }

        /// <summary>
        /// �������
        /// </summary>
        public DateTime? CheckDate
        {
            get { return checkDate; }
            set { checkDate = value; NotifyPropertyChanged("CheckDate"); }
        }

        /// <summary>
        /// ������
        /// </summary>
        public string CheckOpinion
        {
            get { return checkOpinion; }
            set { checkOpinion = value; NotifyPropertyChanged("CheckOpinion"); }
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        public string PublicityChronicle
        {
            get { return publicityChronicle; }
            set { publicityChronicle = value; NotifyPropertyChanged("PublicityChronicle"); }
        }

        /// <summary>
        /// ��ʾ������
        /// </summary>
        public string PublicityChroniclePerson
        {
            get { return publicityChroniclePerson; }
            set { publicityChroniclePerson = value; NotifyPropertyChanged("PublicityChroniclePerson"); }
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        public DateTime? PublicityDate
        {
            get { return publicityDate; }
            set { publicityDate = value; NotifyPropertyChanged("PublicityDate"); }
        }

        /// <summary>
        /// ��ʾ�����
        /// </summary>
        public string PublicityCheckPerson
        {
            get { return publicityCheckPerson; }
            set { publicityCheckPerson = value; NotifyPropertyChanged("PublicityCheckPerson"); }
        }

        /// <summary>
        /// ���ֳа���ͬ���
        /// </summary>
        public string ConcordNumber
        {
            get { return concordNumber; }
            set { concordNumber = value; NotifyPropertyChanged("ConcordNumber"); }
        }

        /// <summary>
        /// ���־�ӪȨ֤���
        /// </summary>
        public string WarrantNumber
        {
            get { return warrantNumber; }
            set { warrantNumber = value; NotifyPropertyChanged("WarrantNumber"); }
        }

        /// <summary>
        /// ���ֳа���ʼ����
        /// </summary>
        public DateTime? ConcordStartTime
        {
            get { return concordStartTime; }
            set { concordStartTime = value; NotifyPropertyChanged("ConcordStartTime"); }
        }

        /// <summary>
        /// ���ֳа���������
        /// </summary>
        public DateTime? ConcordEndTime
        {
            get { return concordEndTime; }
            set { concordEndTime = value; NotifyPropertyChanged("ConcordEndTime"); }
        }

        /// <summary>
        /// �а���ʽ
        /// </summary>
        public eConstructMode ConstructMode
        {
            get { return constructType; }
            set { constructType = value; NotifyPropertyChanged("ConstructMode"); }
        }

        /// <summary>
        /// ��Ȩ֤��
        /// </summary>
        public string EquityNumber
        {
            get { return equityNumber; }
            set { equityNumber = value; NotifyPropertyChanged("EquityNumber"); }
        }

        /// <summary>
        /// ��ɷ���
        /// </summary>
        public double EquityValue
        {
            get { return equityValue; }
            set { equityValue = value; NotifyPropertyChanged("EquityValue"); }
        }

        /// <summary>
        /// ������
        /// </summary>
        public double EquityArea
        {
            get { return equityArea; }
            set { equityArea = value; NotifyPropertyChanged("EquityArea"); }
        }

        /// <summary>
        /// ���徭����֯����
        /// </summary>
        public string CollectivityName
        {
            get { return collectivityName; }
            set { collectivityName = value; NotifyPropertyChanged("CollectivityName"); }
        }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime? QuantifyTime
        {
            get { return quantifyTime; }
            set { quantifyTime = value; NotifyPropertyChanged("QuantifyTime"); }
        }

        /// <summary>
        /// ��Ȩ��ʼʱ��
        /// </summary>
        public DateTime? EquityStartTime
        {
            get { return equityStartTime; }
            set { equityStartTime = value; NotifyPropertyChanged("EquityStartTime"); }
        }

        /// <summary>
        /// ��Ȩ����ʱ��
        /// </summary>
        public DateTime? EquityEndTime
        {
            get { return equityEndTime; }
            set { equityEndTime = value; NotifyPropertyChanged("EquityEndTime"); }
        }

        /// <summary>
        /// ��֤��λ
        /// </summary>
        public string IssueUnit
        {
            get { return issueUnit; }
            set { issueUnit = value; NotifyPropertyChanged("IssueUnit"); }
        }

        /// <summary>
        /// ��֤ʱ��
        /// </summary>
        public DateTime? IssueTime
        {
            get { return issueTime; }
            set { issueTime = value; NotifyPropertyChanged("IssueTime"); }
        }

        /// <summary>
        /// �Ǽǻ���
        /// </summary>
        public string RegistrationAuthority
        {
            get { return registrationAuthority; }
            set { registrationAuthority = value; NotifyPropertyChanged("RegistrationAuthority"); }
        }

        /// <summary>
        /// �Ǽ�ʱ��
        /// </summary>
        public DateTime? RegistrationTime
        {
            get { return registrationTime; }
            set { registrationTime = value; NotifyPropertyChanged("RegistrationTime"); }
        }

        /// <summary>
        /// ��ע
        /// </summary>
        public string Comment
        {
            get { return comment; }
            set { comment = value; NotifyPropertyChanged("Comment"); }
        }

        /// <summary>
        /// ���ֳа���ͬ�����-�ຣ��
        /// </summary>
        public double SecondConcordTotalArea
        {
            get { return secondConcordTotalArea; }
            set { secondConcordTotalArea = value; NotifyPropertyChanged("SecondConcordTotalArea"); }
        }

        /// <summary>
        /// ���ֳа���ͬ�ؿ�����-�ຣ��
        /// </summary>
        public int SecondConcordTotalLandCount
        {
            get { return secondConcordTotalLandCount; }
            set { secondConcordTotalLandCount = value; NotifyPropertyChanged("SecondConcordTotalLandCount"); }
        }

        /// <summary>
        /// ��չ�ֶ�
        /// </summary>
        public string ExtendName
        {
            get { return extendName; }
            set { extendName = value; NotifyPropertyChanged("ExtendName"); }
        }

        /// <summary>
        /// �仯���
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
            ContractorType = eContractorType.Farmer;  //ũ��
            constructType = eConstructMode.Family; //��ͥ�а�
        }

        #endregion

        #region Method

        /// <summary>
        /// ������չ��Ϣ
        /// </summary>
        public static VirtualPersonExpand CreateExpandByXml(string xml)
        {
            if (xml == null || xml.Length < 0)
            {
                return null;
            }
            //2011-04-28 16:11:07 ֣��(Roc Zheng) �޸����ı��������͵��µ�XML��ȡʧ�ܵ�BUG�����Ǵ���Ч�ʵ��������⣬�����и��õĽ���취
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
        /// ��ʼ��Ԫ����Ϣ
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static XElement InitalizeRootInformtaion(string xml)
        {
            XElement root = null;
            try
            {
                //ʹ���ڴ����Ա������ͽ����������ã�ת��Ϊ������ʹ�õı���(Ĭ��)
                using (System.IO.Stream stream = new System.IO.MemoryStream())
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(stream))
                    {
                        writer.Write(xml);

                        stream.Position = 0;    //��������λ��
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
        /// ��XmlԪ���л�ȡ�ؿ���Ϣ
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

            attr = el.Attribute("BusinessStatus");//ҵ��״̬
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

            attr = el.Attribute("LaborNumber");//��������
            land.LaborNumber = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("AllocationPerson");//ʵ�ʷ�������
            land.AllocationPerson = (attr == null || String.IsNullOrEmpty(attr.Value)) ? "" : attr.Value;

            attr = el.Attribute("FarmerNature");//ũ������
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