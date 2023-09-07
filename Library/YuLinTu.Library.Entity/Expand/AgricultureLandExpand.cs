using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using YuLinTu.Data;
using YuLinTu;

namespace YuLinTu.Library.Entity
{
    [Serializable]
    public class AgricultureLandExpand : NotifyCDObject
    {
        #region Fileds
        
        private string name;
        private string houseHolderName;
        private string agricultureNumber;
        private string useSituation;
        private string yield;
        private string outputValue;
        private string incomeSituation;
        private string secondLandNumber;
        private string secondLandType;
        private string secondLandPurpose;
        private string secondIsFarmerLand;
        private string secondLandLevel;
        private double? elevation;
        private string soilTexture;
        private string surveyPerson;
        private DateTime? surveyDate;
        private string surveyChronicle;
        private string checkPerson;
        private DateTime? checkDate;
        private string checkOpinion;
        private string cartographer;
        private DateTime? cartographyDate;
        private double? annualRainfall;
        private double? sunshine;
        private double? totalTemperature;
        private string managerMode;
        private string description;
        private string imageNumber;
        private string referPerson;
        private double measureArea;
        private string publicityComment;

        #endregion

        #region Properties

        public Guid ID
        {
            get;
            set;
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
        /// ũҵ������
        /// </summary>
        public string AgricultureNumber
        {
            get { return agricultureNumber; }
            set { agricultureNumber = value; NotifyPropertyChanged("AgricultureNumber"); }
        }
       
        /// <summary>
        /// �������
        /// </summary>
        public string UseSituation
        {
            get { return useSituation; }
            set { useSituation = value; NotifyPropertyChanged("UseSituation"); }
        }

        /// <summary>
        /// ����
        /// </summary>
        public string Yield
        {
            get { return yield; }
            set { yield = value; NotifyPropertyChanged("Yield"); }
        }

        /// <summary>
        /// ��ֵ
        /// </summary>
        public string OutputValue
        {
            get { return outputValue; }
            set { outputValue = value; NotifyPropertyChanged("OutputValue"); }
        }

        /// <summary>
        /// �������
        /// </summary>
        public string IncomeSituation
        {
            get { return incomeSituation; }
            set { incomeSituation = value; NotifyPropertyChanged("IncomeSituation"); }
        }

        /// <summary>
        /// ���ֵؿ����
        /// </summary>
        public string SecondLandNumber
        {
            get { return secondLandNumber; }
            set { secondLandNumber = value; NotifyPropertyChanged("SecondLandNumber"); }
        }

        /// <summary>
        /// ������������
        /// </summary>
        public string SecondLandType
        {
            get { return secondLandType; }
            set { secondLandType = value; NotifyPropertyChanged("SecondLandType"); }
        }

        /// <summary>
        /// ����������;
        /// </summary>
        public string SecondLandPurpose
        {
            get { return secondLandPurpose; }
            set { secondLandPurpose = value; NotifyPropertyChanged("SecondLandPurpose"); }
        }

        /// <summary>
        /// �����Ƿ����ũ��
        /// </summary>
        public string SecondIsFarmerLand
        {
            get { return secondIsFarmerLand; }
            set { secondIsFarmerLand = value; NotifyPropertyChanged("SecondIsFarmerLand"); }
        }

        /// <summary>
        /// �������صȼ�
        /// </summary>
        public string SecondLandLevel
        {
            get { return secondLandLevel; }
            set { secondLandLevel = value; NotifyPropertyChanged("SecondLandLevel"); }
        }

        /// <summary>
        /// �߳���Ϣ
        /// </summary>
        public double? Elevation
        {
            get { return elevation; }
            set { elevation = value; NotifyPropertyChanged("Elevation"); }
        }

        /// <summary>
        /// �������͡�
        /// </summary>
        public string SoilTexture
        {
            get { return soilTexture; }
            set { soilTexture = value; NotifyPropertyChanged("SoilTexture"); }
        }

        /// <summary>
        /// �����ˡ�
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
        /// ��ͼ�ˡ�
        /// </summary>
        public string Cartographer
        {
            get { return cartographer; }
            set { cartographer = value; NotifyPropertyChanged("Cartographer"); }
        }

        /// <summary>
        /// ��ͼ���ڡ�
        /// </summary>
        public DateTime? CartographyDate
        {
            get { return cartographyDate; }
            set { cartographyDate = value; NotifyPropertyChanged("CartographyDate"); }
        }

        /// <summary>
        /// �꽵������
        /// </summary>
        public double? AnnualRainfall
        {
            get { return annualRainfall; }
            set { annualRainfall = value; NotifyPropertyChanged("AnnualRainfall"); }
        }

        /// <summary>
        /// ���ա�
        /// </summary>
        public double? Sunshine
        {
            get { return sunshine; }
            set { sunshine = value; NotifyPropertyChanged("Sunshine"); }
        }

        /// <summary>
        /// ���¡�
        /// </summary>
        public double? TotalTemperature
        {
            get { return totalTemperature; }
            set { totalTemperature = value; NotifyPropertyChanged("TotalTemperature"); }
        }

        /// <summary>
        /// ��Ӫ��ʽ
        /// </summary>
        public string ManagerMode
        {
            get { return managerMode; }
            set { managerMode = value; NotifyPropertyChanged("managerMode"); }
        }

        /// <summary>
        /// ����
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; NotifyPropertyChanged("Description"); }
        }

        /// <summary>
        /// ͼ�����
        /// </summary>
        public string ImageNumber
        {
            get { return imageNumber; }
            set { imageNumber = value; NotifyPropertyChanged("ImageNumber"); }
        }

        /// <summary>
        /// ָ����
        /// </summary>
        public string ReferPerson
        {
            get { return referPerson; }
            set { referPerson = value; NotifyPropertyChanged("ReferPerson"); }
        }

        /// <summary>
        /// �������
        /// </summary>
        public double MeasureArea
        {
            get { return measureArea; }
            set { measureArea = value; NotifyPropertyChanged("MeasureArea"); }
        }

        /// <summary>
        /// ��ʾ��ע
        /// </summary>
        public string PublicityComment 
        {
            get { return publicityComment; }
            set { publicityComment = value; NotifyPropertyChanged("PublicityComment"); }
        }

        #endregion

        #region Ctor

        public AgricultureLandExpand()
        {
        }

        #endregion
    }
}