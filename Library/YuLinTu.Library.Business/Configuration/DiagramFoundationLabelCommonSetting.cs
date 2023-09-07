using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;
namespace YuLinTu.Library.Business
{
    public class DiagramFoundationLabelCommonSetting : NotifyCDObject
    {
        #region Fields

        private bool isUseContractorName;
        private bool isUseLandNumber;
        private int getLandMiniNumber;
        private bool isUseLandSurveyNumber;
        private bool isUseLandName;
        private bool isUseLandTableArea;
        private bool isUseLandActualArea;
        private bool isUseLandAwareArea;
        private bool isUseLandAreaUnitMu;

        private string cartographer;
        private DateTime? cartographyDate;
        private string checkPerson;
        private DateTime? checkDate;
        private string cartographyUnit;

        private System.Windows.Media.Color landSeparateLineColor;

        private bool isShowViewOfAllScale;
        private double viewOfAllScaleWH;

        private string useLayoutModel;
        private string useLandLabelFontSet;//标注设置
        #endregion

        #region Properties

        /// <summary>
        /// 地块标注字体
        /// </summary>
        public string UseLandLabelFontSet
        {
            get { return useLandLabelFontSet; }
            set { useLandLabelFontSet = value; NotifyPropertyChanged("UseLandLabelFontSet"); }
        }

        /// <summary>
        /// 是否显示户主名称
        /// </summary>
        public bool IsUseContractorName
        {
            get { return isUseContractorName; }
            set { isUseContractorName = value; NotifyPropertyChanged("IsUseContractorName"); }
        }

        /// <summary>
        /// 是否显示地块编码
        /// </summary>
        public bool IsUseLandNumber
        {
            get { return isUseLandNumber; }
            set { isUseLandNumber = value; NotifyPropertyChanged("IsUseLandNumber"); }
        }

        /// <summary>
        /// 地块编码开始截取位数
        /// </summary>
        public int GetLandMiniNumber
        {
            get { return getLandMiniNumber; }
            set { getLandMiniNumber = value; NotifyPropertyChanged("GetLandMiniNumber"); }
        }

        /// <summary>
        /// 是否显示调查编码
        /// </summary>
        public bool IsUseLandSurveyNumber
        {
            get { return isUseLandSurveyNumber; }
            set { isUseLandSurveyNumber = value; NotifyPropertyChanged("IsUseLandSurveyNumber"); }
        }

        /// <summary>
        /// 本宗是否显示地块名称
        /// </summary>
        public bool IsUseLandName
        {
            get { return isUseLandName; }
            set { isUseLandName = value; NotifyPropertyChanged("IsUseLandName"); }
        }

        /// <summary>
        /// 设置标注中是否显示二轮合同面积
        /// </summary>
        public bool IsUseLandTableArea
        {
            get { return isUseLandTableArea; }
            set { isUseLandTableArea = value; NotifyPropertyChanged("IsUseLandTableArea"); }
        }

        /// <summary>
        /// 设置标注中是否显示实测面积
        /// </summary>
        public bool IsUseLandActualArea
        {
            get { return isUseLandActualArea; }
            set { isUseLandActualArea = value; NotifyPropertyChanged("IsUseLandActualArea"); }
        }

        /// <summary>
        /// 设置标注中是否显示确权面积
        /// </summary>
        public bool IsUseLandAwareArea
        {
            get { return isUseLandAwareArea; }
            set { isUseLandAwareArea = value; NotifyPropertyChanged("IsUseLandAwareArea"); }
        }

        /// <summary>
        /// 是否添加单位亩,面积后面是否显示单位亩
        /// </summary>
        public bool IsUseLandAreaUnitMu
        {
            get { return isUseLandAreaUnitMu; }
            set { isUseLandAreaUnitMu = value; NotifyPropertyChanged("IsUseLandAreaUnitMu"); }
        }

        /// <summary>
        /// 地块示意图制图者
        /// </summary>
        public string Cartographer
        {
            get { return cartographer; }
            set { cartographer = value; NotifyPropertyChanged("Cartographer"); }
        }
        /// <summary>
        /// 地块示意图制图时间
        /// </summary>
        public DateTime? CartographyDate
        {
            get { return cartographyDate; }
            set { cartographyDate = value; NotifyPropertyChanged("CartographyDate"); }
        }

        /// <summary>
        /// 地块示意图审核者
        /// </summary>
        public string CheckPerson
        {
            get { return checkPerson; }
            set { checkPerson = value; NotifyPropertyChanged("CheckPerson"); }
        }
        /// <summary>
        /// 地块示意图审核日期
        /// </summary>
        public DateTime? CheckDate
        {
            get { return checkDate; }
            set { checkDate = value; NotifyPropertyChanged("CheckDate"); }
        }

        /// <summary>
        /// 地块示意图制图公司
        /// </summary>
        public string CartographyUnit
        {
            get { return cartographyUnit; }
            set { cartographyUnit = value; NotifyPropertyChanged("CartographyUnit"); }
        }

        /// <summary>
        /// 设置分隔线颜色
        /// </summary>
        public System.Windows.Media.Color LandSeparateLineColor
        {
            get { return landSeparateLineColor; }
            set { landSeparateLineColor = value; NotifyPropertyChanged("LandSeparateLineColor"); }
        }

        /// <summary>
        /// 是否显示比例尺
        /// </summary>
        public bool IsShowViewOfAllScale
        {
            get { return isShowViewOfAllScale; }
            set { isShowViewOfAllScale = value; NotifyPropertyChanged("IsShowViewOfAllScale"); }
        }

        /// <summary>
        /// 比例尺大小分母设置
        /// </summary>
        public double ViewOfAllScaleWH
        {
            get { return viewOfAllScaleWH; }
            set { viewOfAllScaleWH = value; NotifyPropertyChanged("ViewOfAllScaleWH"); }
        }
        
        /// <summary>
        /// 设置公示图使用的默认模板
        /// </summary>
        public string UseLayoutModel
        {
            get { return useLayoutModel; }
            set { useLayoutModel = value; NotifyPropertyChanged("UseLayoutModel"); }
        }

        #endregion


        public DiagramFoundationLabelCommonSetting()
        {
            isUseContractorName = false;
            isUseLandNumber = false;
            getLandMiniNumber = 0;
            isUseLandSurveyNumber = false;
            isUseLandName = false;
            isUseLandTableArea = false;
            isUseLandActualArea = false;
            isUseLandAwareArea = true;
            isUseLandAreaUnitMu = false;
            cartographyDate = DateTime.Now;
            checkDate = DateTime.Now;
            landSeparateLineColor = System.Windows.Media.Colors.Black;
            isShowViewOfAllScale = false;
            viewOfAllScaleWH = 5000;
            useLayoutModel = "A0_横版";
            UseLandLabelFontSet = "微软雅黑";
        }


        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static DiagramFoundationLabelCommonSetting GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<DiagramFoundationLabelCommonSetting>();
            var section = profile.GetSection<DiagramFoundationLabelCommonSetting>();
            return section.Settings;
        }
    }
}
