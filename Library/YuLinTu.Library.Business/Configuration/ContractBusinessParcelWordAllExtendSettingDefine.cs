/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 承包台账地块示意图设置实体类-全域设置
    /// </summary>
    public partial class ContractBusinessParcelWordSettingDefine : NotifyCDObject
    {
        #region Fields
        private double viewOfAllMultiParcelWitdh;
        private double viewOfAllMultiParcelHeight;
        private bool isShowVPallLands;
        private System.Windows.Media.Color viewOfAllvpLandColor;
        private System.Windows.Media.Color viewOfAllvpLandStrokeColor;
        private double viewOfAllvpLandBorderWidth;

        private bool isShowOtherVPallLands;
        private System.Windows.Media.Color viewOfAllOthervpLandColor;
        private double viewOfAllOthervpLandBorderWidth;

        private bool isShowVillageZoneBoundary;
        private bool isShowGroupZoneBoundary;
        private double groupBoundaryBorderWidth;
        private double villageBoundaryBorderWidth;
        private System.Windows.Media.Color groupBoundaryBorderColor;
        private System.Windows.Media.Color villageBoundaryBorderColor;

        private bool isShowViewOfAllScale;
        private double viewOfAllScaleWH;
        private bool containsOtherZoneLand;
        private string ownerLandBufferType;
        private bool isFixedViewOfAllLandGeoWordExtend;

        //全域标注设置      
        private bool isShowViewOfAllLabel;
        private string viewOfAllLabelFontSet;
        private double viewOfAllLabelFontSize;
        private System.Windows.Media.Color viewOfAllLabelFontColor;
        private bool viewOfAllLabelBold;
        private bool viewOfAllLabelUnderLine;
        private bool viewOfAllLabelStrikeLine;//删除线
        private bool viewOfAllLabeltiltLine;//倾斜线

        private bool isShowVPBufferBoundary;
        private double viewOfAllMultiParcelBuffer;
        #endregion

        #region Properties

        /// <summary>
        /// 全域高度主要整个图片在word中显示的宽度
        /// </summary>
        public double ViewOfAllMultiParcelWitdh
        {
            get { return viewOfAllMultiParcelWitdh; }
            set { viewOfAllMultiParcelWitdh = value; NotifyPropertyChanged("ViewOfAllMultiParcelWitdh"); }
        }

        /// <summary>
        /// 全域高度主要整个图片在word中显示的高度
        /// </summary>
        public double ViewOfAllMultiParcelHeight
        {
            get { return viewOfAllMultiParcelHeight; }
            set { viewOfAllMultiParcelHeight = value; NotifyPropertyChanged("ViewOfAllMultiParcelHeight"); }
        }

        /// <summary>
        /// 全域显示本户下所有地块
        /// </summary>
        public bool IsShowVPallLands
        {
            get { return isShowVPallLands; }
            set { isShowVPallLands = value; NotifyPropertyChanged("IsShowVPallLands"); }
        }

        /// <summary>
        /// 全域本户下地块颜色
        /// </summary>
        public System.Windows.Media.Color ViewOfAllvpLandColor
        {
            get { return viewOfAllvpLandColor; }
            set { viewOfAllvpLandColor = value; NotifyPropertyChanged("ViewOfAllvpLandColor"); }
        }

        /// <summary>
        /// 全域本户下地块边框颜色
        /// </summary>
        public System.Windows.Media.Color ViewOfAllvpLandStrokeColor
        {
            get { return viewOfAllvpLandStrokeColor; }
            set { viewOfAllvpLandStrokeColor = value; NotifyPropertyChanged("ViewOfAllvpLandStrokeColor"); }
        }

        /// <summary>
        /// 全域本户下地块线宽
        /// </summary>
        public double ViewOfAllvpLandBorderWidth
        {
            get { return viewOfAllvpLandBorderWidth; }
            set { viewOfAllvpLandBorderWidth = value; NotifyPropertyChanged("ViewOfAllvpLandBorderWidth"); }
        }

        /// <summary>
        /// 全域显示非本户下所有地块
        /// </summary>
        public bool IsShowOtherVPallLands
        {
            get { return isShowOtherVPallLands; }
            set { isShowOtherVPallLands = value; NotifyPropertyChanged("IsShowOtherVPallLands"); }
        }

        /// <summary>
        /// 全域非本户下地块颜色
        /// </summary>
        public System.Windows.Media.Color ViewOfAllOthervpLandColor
        {
            get { return viewOfAllOthervpLandColor; }
            set { viewOfAllOthervpLandColor = value; NotifyPropertyChanged("ViewOfAllOthervpLandColor"); }
        }

        /// <summary>
        /// 全域非本户下地块线宽
        /// </summary>
        public double ViewOfAllOthervpLandBorderWidth
        {
            get { return viewOfAllOthervpLandBorderWidth; }
            set { viewOfAllOthervpLandBorderWidth = value; NotifyPropertyChanged("ViewOfAllOthervpLandBorderWidth"); }
        }


        /// <summary>
        /// 显示村级区域边界
        /// </summary>
        public bool IsShowVillageZoneBoundary
        {
            get { return isShowVillageZoneBoundary; }
            set { isShowVillageZoneBoundary = value; NotifyPropertyChanged("IsShowVillageZoneBoundary"); }
        }

        /// <summary>
        /// 显示村民小组边界
        /// </summary>
        public bool IsShowGroupZoneBoundary
        {
            get { return isShowGroupZoneBoundary; }
            set { isShowGroupZoneBoundary = value; NotifyPropertyChanged("IsShowGroupZoneBoundary"); }
        }

        /// <summary>
        /// 组级行政区域边界线宽
        /// </summary>
        public double GroupZoneBoundaryBorderWidth
        {
            get { return groupBoundaryBorderWidth; }
            set { groupBoundaryBorderWidth = value; NotifyPropertyChanged("GroupZoneBoundaryBorderWidth"); }
        }

        /// <summary>
        /// 村级行政区域边界线宽
        /// </summary>
        public double VillageZoneBoundaryBorderWidth
        {
            get { return villageBoundaryBorderWidth; }
            set { villageBoundaryBorderWidth = value; NotifyPropertyChanged("VillageZoneBoundaryBorderWidth"); }
        }

        /// <summary>
        /// 组级行政区域边界线颜色
        /// </summary>
        public System.Windows.Media.Color GroupBoundaryBorderColor
        {
            get { return groupBoundaryBorderColor; }
            set { groupBoundaryBorderColor = value; NotifyPropertyChanged("GroupBoundaryBorderColor"); }
        }

        /// <summary>
        /// 村级行政区域边界线颜色
        /// </summary>
        public System.Windows.Media.Color VillageZoneBoundaryBorderColor
        {
            get { return villageBoundaryBorderColor; }
            set { villageBoundaryBorderColor = value; NotifyPropertyChanged("VillageZoneBoundaryBorderColor"); }
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
        /// 比例尺大小分母设置
        /// </summary>
        public bool ContainsOtherZoneLand
        {
            get { return containsOtherZoneLand; }
            set { containsOtherZoneLand = value; NotifyPropertyChanged("ContainsOtherZoneLand"); }
        }

        /// <summary>
        /// 本宗地矩形框向外缓冲方式
        /// </summary>
        public string OwnerLandBufferType
        {
            get { return ownerLandBufferType; }
            set { ownerLandBufferType = value; NotifyPropertyChanged("OwnerLandBufferType"); }
        }

        /// <summary>
        /// 固定鹰眼显示范围
        /// </summary>
        public bool IsFixedViewOfAllLandGeoWordExtend
        {
            get { return isFixedViewOfAllLandGeoWordExtend; }
            set { isFixedViewOfAllLandGeoWordExtend = value; NotifyPropertyChanged("IsFixedViewOfAllLandGeoWordExtend"); }
        }

        /// <summary>
        /// 全域是否显示本宗地块编码标注
        /// </summary>
        public bool IsShowViewOfAllLabel
        {
            get { return isShowViewOfAllLabel; }
            set { isShowViewOfAllLabel = value; NotifyPropertyChanged("IsShowViewOfAllLabel"); }
        }
        /// <summary>
        /// 全域地块标注字体
        /// </summary>
        public string ViewOfAllLabelFontSet
        {
            get { return viewOfAllLabelFontSet; }
            set { viewOfAllLabelFontSet = value; NotifyPropertyChanged("ViewOfAllLabelFontSet"); }
        }

        /// <summary>
        /// 全域地块标注颜色
        /// </summary>
        public System.Windows.Media.Color ViewOfAllLabelFontColor
        {
            get { return viewOfAllLabelFontColor; }
            set { viewOfAllLabelFontColor = value; NotifyPropertyChanged("ViewOfAllLabelFontColor"); }
        }

        /// <summary>
        /// 全域地块标注大小
        /// </summary>
        public double ViewOfAllLabelFontSize
        {
            get { return viewOfAllLabelFontSize; }
            set { viewOfAllLabelFontSize = value; NotifyPropertyChanged("ViewOfAllLabelFontSize"); }
        }

        /// <summary>
        /// 全域地块标注加粗
        /// </summary>
        public bool ViewOfAllLabelBold
        {
            get { return viewOfAllLabelBold; }
            set { viewOfAllLabelBold = value; NotifyPropertyChanged("ViewOfAllLabelBold"); }
        }

        /// <summary>
        /// 全域地块标注下划线
        /// </summary>
        public bool ViewOfAllLabelUnderLine
        {
            get { return viewOfAllLabelUnderLine; }
            set { viewOfAllLabelUnderLine = value; NotifyPropertyChanged("ViewOfAllLabelUnderLine"); }
        }

        /// <summary>
        /// 全域地块标注删除线
        /// </summary>
        public bool ViewOfAllLabelStrikeLine
        {
            get { return viewOfAllLabelStrikeLine; }
            set { viewOfAllLabelStrikeLine = value; NotifyPropertyChanged("ViewOfAllLabelStrikeLine"); }
        }

        /// <summary>
        /// 全域地块标注倾斜
        /// </summary>
        public bool ViewOfAllLabeltiltLine
        {
            get { return viewOfAllLabeltiltLine; }
            set { viewOfAllLabeltiltLine = value; NotifyPropertyChanged("ViewOfAllLabeltiltLine"); }
        }

        /// <summary>
        /// 显示本户地块缓冲区域
        /// </summary>
        public bool IsShowVPBufferBoundary
        {
            get { return isShowVPBufferBoundary; }
            set { isShowVPBufferBoundary = value; NotifyPropertyChanged("IsShowVPBufferBoundary"); }
        }

        /// <summary>
        /// 本户地块缓冲区范围
        /// </summary>
        public double ViewOfAllMultiParcelBuffer
        {
            get { return viewOfAllMultiParcelBuffer; }
            set { viewOfAllMultiParcelBuffer = value; NotifyPropertyChanged("ViewOfAllMultiParcelBuffer"); }
        }

        #endregion

    }
}