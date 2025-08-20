/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 承包台账地块示意图设置实体类-本宗及邻宗地块设置
    /// </summary>
    public partial class ContractBusinessParcelWordSettingDefine : NotifyCDObject
    {
        #region Fields
        //本宗标注设置  
        private bool isUseLandTableArea;
        private bool isUseLandActualArea;
        private bool isUseLandAwareArea;
        private bool isUseLandNumber;
        private int getLandMiniNumber;
        private bool isUseLandName;      
        private bool isShowJZDNumber;
        private bool isUniteJZDNumber;
        private bool isShowJZDGeo;
        private bool isFixedLandGeoWordExtend;
        private double landGeoWordWidth;
        private double landGeoWordHeight;
        private System.Windows.Media.Color ownerLandColor;
        private System.Windows.Media.Color boundaryCircleColor;
        private double ownerLandBorderThickness; 

        private bool exportContractLandType;
        private bool exportPrivateLandType;
        private bool exportMotorizeLandType;
        private bool exportWasteLandType;
        private bool exportCollectiveLandType;
        private bool exportEncollecLandType;
        private bool exportFeedLandType;
        private bool exportAbandonedLandType;

        private bool isLandTypeSort;      
            
        private string useLandLabelFontSet;
        private double useLandLabelFontSize;
        private System.Windows.Media.Color useLandLabelFontColor;
        private bool useLandLabelBold;
        private bool useLandLabelUnderLine;
        private bool useLandLabelStrikeLine;//删除线
        private bool useLandLabeltiltLine;//倾斜线

        //临宗标注设置       
        private double neighborlandbufferdistence;
        private double neighbordxmzdwbufferdistence;
        private bool isShowNeighborLandGeo;
        private System.Windows.Media.Color neighborLandColor;
        private double neighborLandBorderThickness;
        private string neighborLandLabelFontSet;
        private double neighborLandLabelFontSize;
        private System.Windows.Media.Color neighborLandLabelFontColor;
        private bool neighborLandLabelBold;
        private bool neighborLandLabelUnderLine;
        private bool neighborLandLabelStrikeLine;//删除线
        private bool neighborLandLabeltiltLine;//倾斜线

        private bool neighborlandLabelisJTuseLandName;//如果临宗为集体地则改为二类地块名称
        private bool neighborlandLabeluseDLTGGQname;//如果临宗调查四至为道路  沟渠  田埂则直接打印
        private bool neighborlandSearchUseUserAlgorithm;

        private bool setNeighborLandWestEastLabelVertical; //若设置了 直接勾选调查四至，或无邻宗地时显示调查四至，东西向的四至需要竖排显示。

        #endregion

        #region Properties

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
        /// 本宗是否显示地块名称
        /// </summary>
        public bool IsUseLandName
        {
            get { return isUseLandName; }
            set { isUseLandName = value; NotifyPropertyChanged("IsUseLandName"); }
        }
             

        /// <summary>
        /// 是否显示邻宗地图形
        /// </summary>
        public bool IsShowNeighborLandGeo
        {
            get { return isShowNeighborLandGeo; }
            set { isShowNeighborLandGeo = value; NotifyPropertyChanged("IsShowNeighborLandGeo"); }
        }
        
        /// <summary>
        /// 是否显示界址点号
        /// </summary>
        public bool IsShowJZDNumber
        {
            get { return isShowJZDNumber; }
            set { isShowJZDNumber = value; NotifyPropertyChanged("IsShowJZDNumber"); }
        }

        public bool IsUniteJZDNumber
        {
            get { return isUniteJZDNumber; }
            set { isUniteJZDNumber = value; NotifyPropertyChanged("IsUniteJZDNumber"); }
        }

        /// <summary>
        /// 是否显示界址圈
        /// </summary>
        public bool IsShowJZDGeo
        {
            get { return isShowJZDGeo; }
            set { isShowJZDGeo = value; NotifyPropertyChanged("IsShowJZDGeo"); }
        }

        /// <summary>
        /// 设置是否固定地块的显示范围,固定则是模板的里面的固定值，否则是用户输入设置的
        /// </summary>
        public bool IsFixedLandGeoWordExtend
        {
            get { return isFixedLandGeoWordExtend; }
            set { isFixedLandGeoWordExtend = value; NotifyPropertyChanged("IsFixedLandGeoWordExtend"); }
        }       

        /// <summary>
        /// 地块宽度,主要整个图片在word中显示的宽度
        /// </summary>
        public double LandGeoWordWidth
        {
            get { return landGeoWordWidth; }
            set { landGeoWordWidth = value; NotifyPropertyChanged("LandGeoWordWidth"); }
        }

        /// <summary>
        /// 地块高度,高度主要整个图片在word中显示的高度
        /// </summary>
        public double LandGeoWordHeight
        {
            get { return landGeoWordHeight; }
            set { landGeoWordHeight = value; NotifyPropertyChanged("LandGeoWordHeight"); }
        }

        /// <summary>
        /// 本宗颜色
        /// </summary>
        public System.Windows.Media.Color OwnerLandColor
        {
            get { return ownerLandColor; }
            set { ownerLandColor = value; NotifyPropertyChanged("OwnerLandColor"); }
        }

        /// <summary>
        /// 邻宗颜色
        /// </summary>
        public System.Windows.Media.Color NeighborLandColor
        {
            get { return neighborLandColor; }
            set { neighborLandColor = value; NotifyPropertyChanged("NeighborLandColor"); }
        }

        /// <summary>
        /// 本宗线宽
        /// </summary>
        public double OwnerLandBorderThickness
        {
            get { return ownerLandBorderThickness; }
            set { ownerLandBorderThickness = value; NotifyPropertyChanged("OwnerLandBorderThickness"); }
        }

        /// <summary>
        /// 邻宗线宽
        /// </summary>
        public double NeighborLandBorderThickness
        {
            get { return neighborLandBorderThickness; }
            set { neighborLandBorderThickness = value; NotifyPropertyChanged("NeighborLandBorderThickness"); }
        }
            
        /// <summary>
        /// 选择承包地块导出
        /// </summary>
        public bool ExportContractLandType
        {
            get { return exportContractLandType; }
            set { exportContractLandType = value; NotifyPropertyChanged("ExportContractLandType"); }
        }
        /// <summary>
        /// 选择自留地导出
        /// </summary>
        public bool ExportPrivateLandType
        {
            get { return exportPrivateLandType; }
            set { exportPrivateLandType = value; NotifyPropertyChanged("ExportPrivateLandType"); }
        }
        /// <summary>
        /// 选择机动地导出
        /// </summary>
        public bool ExportMotorizeLandType
        {
            get { return exportMotorizeLandType; }
            set { exportMotorizeLandType = value; NotifyPropertyChanged("ExportMotorizeLandType"); }
        }

        /// <summary>
        /// 选择开荒地导出
        /// </summary>
        public bool ExportWasteLandType
        {
            get { return exportWasteLandType; }
            set { exportWasteLandType = value; NotifyPropertyChanged("ExportWasteLandType"); }
        }

        /// <summary>
        /// 选择其他集体土地导出
        /// </summary>
        public bool ExportCollectiveLandType
        {
            get { return exportCollectiveLandType; }
            set { exportCollectiveLandType = value; NotifyPropertyChanged("ExportCollectiveLandType"); }
        }
        /// <summary>
        /// 选择经济地导出
        /// </summary>
        public bool ExportEncollecLandType
        {
            get { return exportEncollecLandType; }
            set { exportEncollecLandType = value; NotifyPropertyChanged("ExportEncollecLandType"); }
        }
        /// <summary>
        /// 选择饲料地导出
        /// </summary>
        public bool ExportFeedLandType
        {
            get { return exportFeedLandType; }
            set { exportFeedLandType = value; NotifyPropertyChanged("ExportFeedLandType"); }
        }

        /// <summary>
        /// 选择撂荒地导出
        /// </summary>
        public bool ExportAbandonedLandType
        {
            get { return exportAbandonedLandType; }
            set { exportAbandonedLandType = value; NotifyPropertyChanged("ExportAbandonedLandType"); }
        }

        /// <summary>
        /// 本宗地块编码排序
        /// </summary>
        public bool IsLandTypeSort
        {
            get { return isLandTypeSort; }
            set { isLandTypeSort = value; NotifyPropertyChanged("IsLandTypeSort"); }
        }

      
        /// <summary>
        /// 地块标注字体
        /// </summary>
        public string UseLandLabelFontSet
        {
            get { return useLandLabelFontSet; }
            set { useLandLabelFontSet = value; NotifyPropertyChanged("UseLandLabelFontSet"); }
        }

        /// <summary>
        /// 地块标注颜色
        /// </summary>
        public System.Windows.Media.Color UseLandLabelFontColor
        {
            get { return useLandLabelFontColor; }
            set { useLandLabelFontColor = value; NotifyPropertyChanged("UseLandLabelFontColor"); }
        }

        /// <summary>
        /// 地块标注大小
        /// </summary>
        public double UseLandLabelFontSize
        {
            get { return useLandLabelFontSize; }
            set { useLandLabelFontSize = value; NotifyPropertyChanged("UseLandLabelFontSize"); }
        }
        
        /// <summary>
        /// 地块标注加粗
        /// </summary>
        public bool UseLandLabelBold
        {
            get { return useLandLabelBold; }
            set { useLandLabelBold = value; NotifyPropertyChanged("UseLandLabelBold"); }
        }

        /// <summary>
        /// 地块标注下划线
        /// </summary>
        public bool UseLandLabelUnderLine
        {
            get { return useLandLabelUnderLine; }
            set { useLandLabelUnderLine = value; NotifyPropertyChanged("UseLandLabelUnderLine"); }
        }

        /// <summary>
        /// 地块标注删除线
        /// </summary>
        public bool UseLandLabelStrikeLine
        {
            get { return useLandLabelStrikeLine; }
            set { useLandLabelStrikeLine = value; NotifyPropertyChanged("UseLandLabelStrikeLine"); }
        }

        /// <summary>
        /// 地块标注倾斜
        /// </summary>
        public bool UseLandLabeltiltLine
        {
            get { return useLandLabeltiltLine; }
            set { useLandLabeltiltLine = value; NotifyPropertyChanged("UseLandLabeltiltLine"); }
        }
        
        /// <summary>
        /// 邻宗地块标注字体
        /// </summary>
        public string NeighborLandLabelFontSet
        {
            get { return neighborLandLabelFontSet; }
            set { neighborLandLabelFontSet = value; NotifyPropertyChanged("NeighborLandLabelFontSet"); }
        }

        /// <summary>
        /// 邻宗地块标注颜色
        /// </summary>
        public System.Windows.Media.Color NeighborLandLabelFontColor
        {
            get { return neighborLandLabelFontColor; }
            set { neighborLandLabelFontColor = value; NotifyPropertyChanged("NeighborLandLabelFontColor"); }
        }

        /// <summary>
        /// 邻宗地块标注大小
        /// </summary>
        public double NeighborLandLabelFontSize
        {
            get { return neighborLandLabelFontSize; }
            set { neighborLandLabelFontSize = value; NotifyPropertyChanged("NeighborLandLabelFontSize"); }
        }

        /// <summary>
        /// 邻宗地块标注加粗
        /// </summary>
        public bool NeighborLandLabelBold
        {
            get { return neighborLandLabelBold; }
            set { neighborLandLabelBold = value; NotifyPropertyChanged("NeighborLandLabelBold"); }
        }

        /// <summary>
        /// 邻宗地块标注下划线
        /// </summary>
        public bool NeighborLandLabelUnderLine
        {
            get { return neighborLandLabelUnderLine; }
            set { neighborLandLabelUnderLine = value; NotifyPropertyChanged("NeighborLandLabelUnderLine"); }
        }

        /// <summary>
        /// 邻宗地块标注删除线
        /// </summary>
        public bool NeighborLandLabelStrikeLine
        {
            get { return neighborLandLabelStrikeLine; }
            set { neighborLandLabelStrikeLine = value; NotifyPropertyChanged("NeighborLandLabelStrikeLine"); }
        }

        /// <summary>
        /// 邻宗地块标注倾斜
        /// </summary>
        public bool NeighborLandLabeltiltLine
        {
            get { return neighborLandLabeltiltLine; }
            set { neighborLandLabeltiltLine = value; NotifyPropertyChanged("NeighborLandLabeltiltLine"); }
        }


        /// <summary>
        /// 如果临宗为集体地则改为二类地块名称
        /// </summary>
        public bool NeighborlandLabelisJTuseLandName
        {
            get { return neighborlandLabelisJTuseLandName; }
            set { neighborlandLabelisJTuseLandName = value; NotifyPropertyChanged("NeighborlandLabelisJTuseLandName"); }
        }


        /// <summary>
        /// 如果临宗调查四至为道路  沟渠  田埂则直接打印
        /// </summary>
        public bool NeighborlandLabeluseDLTGGQname
        {
            get { return neighborlandLabeluseDLTGGQname; }
            set { neighborlandLabeluseDLTGGQname = value; NotifyPropertyChanged("NeighborlandLabeluseDLTGGQname"); }
        }

        /// <summary>
        /// 临宗地块查询使用默认算法
        /// </summary>
        public bool NeighborlandSearchUseUserAlgorithm
        {
            get { return neighborlandSearchUseUserAlgorithm; }
            set { neighborlandSearchUseUserAlgorithm = value; NotifyPropertyChanged("NeighborlandSearchUseUserAlgorithm"); }
        }

        /// <summary>
        /// 若设置了 直接勾选调查四至，或无邻宗地时显示调查四至，东西向的四至需要竖排显示。
        /// </summary>
        public bool SetNeighborLandWestEastLabelVertical
        {
            get { return setNeighborLandWestEastLabelVertical; }
            set { setNeighborLandWestEastLabelVertical = value; NotifyPropertyChanged("SetNeighborLandWestEastLabelVertical"); }
        }

        #endregion

    }
}
