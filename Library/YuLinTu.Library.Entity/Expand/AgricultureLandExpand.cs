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
        /// 农业部编码
        /// </summary>
        public string AgricultureNumber
        {
            get { return agricultureNumber; }
            set { agricultureNumber = value; NotifyPropertyChanged("AgricultureNumber"); }
        }
       
        /// <summary>
        /// 利用情况
        /// </summary>
        public string UseSituation
        {
            get { return useSituation; }
            set { useSituation = value; NotifyPropertyChanged("UseSituation"); }
        }

        /// <summary>
        /// 产量
        /// </summary>
        public string Yield
        {
            get { return yield; }
            set { yield = value; NotifyPropertyChanged("Yield"); }
        }

        /// <summary>
        /// 产值
        /// </summary>
        public string OutputValue
        {
            get { return outputValue; }
            set { outputValue = value; NotifyPropertyChanged("OutputValue"); }
        }

        /// <summary>
        /// 收益情况
        /// </summary>
        public string IncomeSituation
        {
            get { return incomeSituation; }
            set { incomeSituation = value; NotifyPropertyChanged("IncomeSituation"); }
        }

        /// <summary>
        /// 二轮地块编码
        /// </summary>
        public string SecondLandNumber
        {
            get { return secondLandNumber; }
            set { secondLandNumber = value; NotifyPropertyChanged("SecondLandNumber"); }
        }

        /// <summary>
        /// 二轮土地类型
        /// </summary>
        public string SecondLandType
        {
            get { return secondLandType; }
            set { secondLandType = value; NotifyPropertyChanged("SecondLandType"); }
        }

        /// <summary>
        /// 二轮土地用途
        /// </summary>
        public string SecondLandPurpose
        {
            get { return secondLandPurpose; }
            set { secondLandPurpose = value; NotifyPropertyChanged("SecondLandPurpose"); }
        }

        /// <summary>
        /// 二轮是否基本农田
        /// </summary>
        public string SecondIsFarmerLand
        {
            get { return secondIsFarmerLand; }
            set { secondIsFarmerLand = value; NotifyPropertyChanged("SecondIsFarmerLand"); }
        }

        /// <summary>
        /// 二轮土地等级
        /// </summary>
        public string SecondLandLevel
        {
            get { return secondLandLevel; }
            set { secondLandLevel = value; NotifyPropertyChanged("SecondLandLevel"); }
        }

        /// <summary>
        /// 高程信息
        /// </summary>
        public double? Elevation
        {
            get { return elevation; }
            set { elevation = value; NotifyPropertyChanged("Elevation"); }
        }

        /// <summary>
        /// 土壤类型。
        /// </summary>
        public string SoilTexture
        {
            get { return soilTexture; }
            set { soilTexture = value; NotifyPropertyChanged("SoilTexture"); }
        }

        /// <summary>
        /// 调查人。
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
        /// 制图人。
        /// </summary>
        public string Cartographer
        {
            get { return cartographer; }
            set { cartographer = value; NotifyPropertyChanged("Cartographer"); }
        }

        /// <summary>
        /// 制图日期。
        /// </summary>
        public DateTime? CartographyDate
        {
            get { return cartographyDate; }
            set { cartographyDate = value; NotifyPropertyChanged("CartographyDate"); }
        }

        /// <summary>
        /// 年降雨量。
        /// </summary>
        public double? AnnualRainfall
        {
            get { return annualRainfall; }
            set { annualRainfall = value; NotifyPropertyChanged("AnnualRainfall"); }
        }

        /// <summary>
        /// 日照。
        /// </summary>
        public double? Sunshine
        {
            get { return sunshine; }
            set { sunshine = value; NotifyPropertyChanged("Sunshine"); }
        }

        /// <summary>
        /// 积温。
        /// </summary>
        public double? TotalTemperature
        {
            get { return totalTemperature; }
            set { totalTemperature = value; NotifyPropertyChanged("TotalTemperature"); }
        }

        /// <summary>
        /// 经营方式
        /// </summary>
        public string ManagerMode
        {
            get { return managerMode; }
            set { managerMode = value; NotifyPropertyChanged("managerMode"); }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; NotifyPropertyChanged("Description"); }
        }

        /// <summary>
        /// 图幅编号
        /// </summary>
        public string ImageNumber
        {
            get { return imageNumber; }
            set { imageNumber = value; NotifyPropertyChanged("ImageNumber"); }
        }

        /// <summary>
        /// 指界人
        /// </summary>
        public string ReferPerson
        {
            get { return referPerson; }
            set { referPerson = value; NotifyPropertyChanged("ReferPerson"); }
        }

        /// <summary>
        /// 量算面积
        /// </summary>
        public double MeasureArea
        {
            get { return measureArea; }
            set { measureArea = value; NotifyPropertyChanged("MeasureArea"); }
        }

        /// <summary>
        /// 公示备注
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