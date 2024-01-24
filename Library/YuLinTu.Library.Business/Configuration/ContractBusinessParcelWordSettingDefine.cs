/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包台账地块示意图设置实体类
    /// </summary>
    public partial class ContractBusinessParcelWordSettingDefine : NotifyCDObject
    {
        #region Fields

        private bool showdxmzdwHandle;
        private bool showlandneighborLabel;
        private bool showAlllandneighborLabel;
        private bool isLandNumberStart;
        private bool saveParcelPCAsPDF;
        private double landNumberIndex;
        private string cartographer;
        private DateTime? cartographyDate;
        private string cartographyUnit;
        private string checkPerson;
        private DateTime? checkDate;
        private bool isVacuateDotRing;
        private double vacuateDotRingSetIndex;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 设置处理地块示意图时是否缩略图是否显示点、线、面状地物
        /// </summary>
        public bool ShowdxmzdwHandle
        {
            get { return showdxmzdwHandle; }
            set { showdxmzdwHandle = value; NotifyPropertyChanged("ShowdxmzdwHandle"); }
        }

        /// <summary>
        /// 临宗地块，如果没有相邻地块，是否显示本宗的调查四至标注
        /// </summary>
        public bool ShowlandneighborLabel
        {
            get { return showlandneighborLabel; }
            set { showlandneighborLabel = value; NotifyPropertyChanged("ShowlandneighborLabel"); }
        }

        /// <summary>
        /// 临宗标注直接使用调查四至
        /// </summary>
        public bool ShowAlllandneighborLabel
        {
            get { return showAlllandneighborLabel; }
            set { showAlllandneighborLabel = value; NotifyPropertyChanged("ShowAlllandneighborLabel"); }
        }

        /// <summary>
        /// 导出图片时另存为PDF
        /// </summary>
        public bool SaveParcelPCAsPDF
        {
            get { return saveParcelPCAsPDF; }
            set { saveParcelPCAsPDF = value; NotifyPropertyChanged("SaveParcelPCAsPDF"); }
        }

        /// <summary>
        ///
        /// 设置临宗地块查找缓冲
        /// </summary>
        public double Neighborlandbufferdistence
        {
            get { return neighborlandbufferdistence; }
            set { neighborlandbufferdistence = value; NotifyPropertyChanged("Neighborlandbufferdistence"); }
        }

        /// <summary>
        /// 设置临宗点线面状查找缓冲
        /// </summary>
        public double Neighbordxmzdwbufferdistence
        {
            get { return neighbordxmzdwbufferdistence; }
            set { neighbordxmzdwbufferdistence = value; NotifyPropertyChanged("Neighbordxmzdwbufferdistence"); }
        }

        /// <summary>
        /// 是否从指定的地块索引开始显示地块示意图
        /// </summary>
        public bool IsLandNumberStart
        {
            get { return isLandNumberStart; }
            set { isLandNumberStart = value; NotifyPropertyChanged("IsLandNumberStart"); }
        }

        /// <summary>
        /// 地块示意图开始显示索引号
        /// </summary>
        public double LandNumberIndex
        {
            get { return landNumberIndex; }
            set { landNumberIndex = value; NotifyPropertyChanged("LandNumberIndex"); }
        }

        /// <summary>
        /// 是否在没有初始化界址点线下抽稀地块界址圈
        /// </summary>
        public bool IsVacuateDotRing
        {
            get { return isVacuateDotRing; }
            set { isVacuateDotRing = value; NotifyPropertyChanged("IsVacuateDotRing"); }
        }

        /// <summary>
        /// 在没有初始化界址点线下抽稀地块界址圈缓冲距离
        /// </summary>
        public double VacuateDotRingSetIndex
        {
            get { return vacuateDotRingSetIndex; }
            set { vacuateDotRingSetIndex = value; NotifyPropertyChanged("VacuateDotRingSetIndex"); }
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
        /// 地块示意图制图公司
        /// </summary>
        public string CartographyUnit
        {
            get { return cartographyUnit; }
            set { cartographyUnit = value; NotifyPropertyChanged("CartographyUnit"); }
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

        #endregion Properties

        #region Ctor

        public ContractBusinessParcelWordSettingDefine()
        {
            SaveParcelPCAsPDF = true;
            ShowdxmzdwHandle = false;
            IsLandNumberStart = false;
            showlandneighborLabel = false;
            showAlllandneighborLabel = false;
            LandNumberIndex = 13;
            IsVacuateDotRing = false;
            VacuateDotRingSetIndex = 4;//默认为三米的缓冲抽稀
            Neighborlandbufferdistence = 1;
            Neighbordxmzdwbufferdistence = 10;
            CheckDate = DateTime.Now;
            CartographyDate = DateTime.Now;

            //地块设置
            IsUseLandTableArea = false;
            IsUseLandActualArea = true;
            IsUseLandAwareArea = false;
            IsUseLandNumber = true;
            GetLandMiniNumber = 14;
            IsUseLandName = false;

            IsShowNeighborLandGeo = true;
            IsShowJZDNumber = false;
            IsShowJZDGeo = false;
            IsFixedLandGeoWordExtend = true;
            LandGeoWordWidth = 140;
            LandGeoWordHeight = 140;
            OwnerLandColor = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
            NeighborLandColor = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
            OwnerLandBorderThickness = 1;
            NeighborLandBorderThickness = 1;

            ExportAbandonedLandType = true;
            ExportCollectiveLandType = true;
            ExportContractLandType = true;
            ExportPrivateLandType = true;
            ExportMotorizeLandType = true;
            ExportWasteLandType = true;
            ExportFeedLandType = true;
            ExportEncollecLandType = true;

            IsLandTypeSort = false;

            UseLandLabelFontSet = "宋体";
            UseLandLabelFontSize = 13;
            UseLandLabelFontColor = System.Windows.Media.Colors.Black;

            NeighborLandLabelFontSet = "宋体";
            NeighborLandLabelFontSize = 13;
            NeighborLandLabelFontColor = System.Windows.Media.Colors.Black;

            //全域设置
            ViewOfAllMultiParcelWitdh = 240;
            ViewOfAllMultiParcelHeight = 260;
            IsShowVPallLands = true;
            ViewOfAllvpLandColor = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
            ViewOfAllvpLandStrokeColor = System.Windows.Media.Color.FromArgb(255, 255, 0, 0);
            ViewOfAllvpLandBorderWidth = 1;

            IsShowOtherVPallLands = true;
            ViewOfAllOthervpLandColor = System.Windows.Media.Color.FromArgb(255, 0, 255, 64);
            ViewOfAllOthervpLandBorderWidth = 1;
            IsShowViewOfAllLabel = false;
            ViewOfAllLabelFontColor = System.Windows.Media.Colors.Black;
            ViewOfAllLabelFontSet = "宋体";
            ViewOfAllLabelFontSize = 12;
            IsShowVillageZoneBoundary = true;
            IsShowGroupZoneBoundary = true;
            GroupZoneBoundaryBorderWidth = 1;
            groupBoundaryBorderColor = System.Windows.Media.Color.FromArgb(255, 0, 255, 64);
            VillageZoneBoundaryBorderWidth = 1;
            villageBoundaryBorderColor = System.Windows.Media.Color.FromArgb(255, 0, 255, 64);

            IsShowViewOfAllScale = true;
            ViewOfAllScaleWH = 5000;

            OwnerLandBufferType = "外边缓冲";
            IsFixedViewOfAllLandGeoWordExtend = true;

            NeighborlandLabelisJTuseLandName = false;
            NeighborlandLabeluseDLTGGQname = false;
            NeighborlandSearchUseUserAlgorithm = false;

            SetNeighborLandWestEastLabelVertical = false;

            //模板设置
            HorizontalVersion = false;
            RowCount1 = 2;
            ColCount1 = 4;
            RowCount2 = 2;
            ColCount2 = 5;

            MaxLandNum = 54;
            ExtendRowCount = 2;
            ExtendColCount = 5;
            IsFixedExtendLandGeoWord = true;
            ExtendLandGeoWordWidth = 115;
            ExtendLandGeoWordHeight = 180;
        }

        #endregion Ctor

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static ContractBusinessParcelWordSettingDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<ContractBusinessParcelWordSettingDefine>();
            var section = profile.GetSection<ContractBusinessParcelWordSettingDefine>();
            return section.Settings;
        }
    }
}