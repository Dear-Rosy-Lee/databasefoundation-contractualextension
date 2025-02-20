/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 系统管理--系统设置信息
    /// </summary>
    public class  SystemSetDefineWork : NotifyCDObject
    {
        #region Fields

        private bool tempTableHead;
        private bool cityTableHead;
        private bool countTableHead;
        private bool townTableHead;
        private bool countryTableHead;
        private bool groupTableHead;

        private bool sortTable;
        private bool numberTill;
        private bool personTable;

        private bool areaIsZero;
        private bool landGroup;
        private bool nergionbourSet;
        private bool nergionbourSortSet;

        private bool statisticSet;
        private bool upDownSet;
        private bool villageInlitialSet;
        private bool landNumericFormatSet;
        private int landNumericFormatValueSet;
        private string defaultPath;
        private bool keepRepeatFlag;

        #endregion

        #region Properties

        #region 表头设置
        /// <summary>
        /// 保持模板表头名称设置
        /// </summary>
        public bool TempTableHead
        {
            get { return tempTableHead; }
            set { tempTableHead = value; NotifyPropertyChanged("TempTableHead"); }
        }

        /// <summary>
        /// 表头中使用市级名称设置
        /// </summary>
        public bool CityTableHead
        {
            get { return cityTableHead; }
            set { cityTableHead = value; NotifyPropertyChanged("CityTableHead");}
        }

        /// <summary>
        /// 表头中使用县级名称设置
        /// </summary>
        public bool CountTableHead
        {
            get { return countTableHead; }
            set { countTableHead = value; NotifyPropertyChanged("CountTableHead"); }
        }

        /// <summary>
        /// 表头中使用乡级名称设置
        /// </summary>
        public bool TownTableHead
        {
            get { return townTableHead; }
            set { townTableHead = value; NotifyPropertyChanged("TownTableHead"); }
        }

        /// <summary>
        /// 表头中使用村级名称设置
        /// </summary>
        public bool CountryTableHead
        {
            get { return countryTableHead; }
            set { countryTableHead = value; NotifyPropertyChanged("CountryTableHead"); }
        }

        /// <summary>
        /// 表头中使用组级名称设置
        /// </summary>
        public bool GroupTableHead
        {
            get { return groupTableHead; }
            set { groupTableHead = value; NotifyPropertyChanged("GroupTableHead"); }
        }

        #endregion

        #region 报表设置

        /// <summary>
        /// 按户号排序导出报表
        /// </summary>
        public bool SortTable
        {
            get { return sortTable; }
            set { sortTable = value; NotifyPropertyChanged("SortTable"); }
        }

        /// <summary>
        /// 报表中编号填写户号
        /// </summary>
        public bool NumberTill
        {
            get { return numberTill; }
            set { numberTill = value; NotifyPropertyChanged("NumberTill"); }
        }

        /// <summary>
        /// 报表中共有人设置
        /// </summary>
        public bool PersonTable
        {
            get { return personTable; }
            set { personTable = value; NotifyPropertyChanged("PersonTable"); }
        }

        #endregion

        #region 地块设置

        /// <summary>
        /// 面积为0时设置
        /// </summary>
        public bool AreaIsZero
        {
            get { return areaIsZero; }
            set { areaIsZero = value; NotifyPropertyChanged("AreaIsZero"); }
        }

        /// <summary>
        /// 地力等级描述设置
        /// </summary>
        public bool LandGroup
        {
            get { return landGroup; }
            set { landGroup = value; NotifyPropertyChanged("LandGroup"); }
        }

        /// <summary>
        /// 四至填充设置
        /// </summary>
        public bool NergionbourSet
        {
            get { return nergionbourSet; }
            set { nergionbourSet = value; NotifyPropertyChanged("NergionbourSet"); }
        }

        /// <summary>
        /// 四至填充设置-选择填充四至，并且为东南西北显示
        /// </summary>
        public bool NergionbourSortSet
        {
            get { return nergionbourSortSet; }
            set { nergionbourSortSet = value; NotifyPropertyChanged("NergionbourSortSet"); }
        }

        #endregion

        #region 其他设置

        /// <summary>
        /// 统计数据
        /// </summary>
        public bool StatisticSet
        {
            get { return statisticSet; }
            set { statisticSet = value; NotifyPropertyChanged("StatisticSet"); }
        }

        /// <summary>
        /// 组级下功能使用设置
        /// </summary>
        public bool UpDownSet
        {
            get { return upDownSet; }
            set { upDownSet = value; NotifyPropertyChanged("UpDownSet"); }
        }


        /// <summary>
        /// 按村级进行户号、地块编码统一初始及签订
        /// </summary>
        public bool VillageInlitialSet
        {
            get { return villageInlitialSet; }
            set { villageInlitialSet = value; NotifyPropertyChanged("VillageInlitialSet"); }
        }

        /// <summary>
        /// 截取地块编码长度
        /// </summary>
        public bool LandNumericFormatSet
        {
            get { return landNumericFormatSet; }
            set { landNumericFormatSet = value; NotifyPropertyChanged("LandNumericFormatSet"); }
        }

        /// <summary>
        /// 导出所有报表时姓名保留重复标识
        /// </summary>
        public bool KeepRepeatFlag
        {
            get { return keepRepeatFlag; }
            set { keepRepeatFlag = value; NotifyPropertyChanged("KeepRepeatFlag"); }
        }

        /// <summary>
        /// 截取地块编码长度值
        /// </summary>
        public int LandNumericFormatValueSet
        {
            get { return landNumericFormatValueSet; }
            set { landNumericFormatValueSet = value; NotifyPropertyChanged("LandNumericFormatValueSet"); }
        } 

        /// <summary>
        /// 组级下功能使用设置
        /// </summary>
        public string DefaultPath
        {
            get { return defaultPath; }
            set { defaultPath = value; NotifyPropertyChanged("DefaultPath"); }
        }

        #endregion

    
        #endregion

        #region Ctor

        public SystemSetDefineWork()
        {
            tempTableHead = true;
            cityTableHead = true;
            countTableHead = false;
            townTableHead = false;
            countryTableHead = true;
            groupTableHead = false;

            sortTable = false;
            numberTill = true;
            personTable = false;

            areaIsZero = false;
            landGroup = true;
            nergionbourSet = false;
            nergionbourSortSet = false;

            statisticSet = true;
            upDownSet = false;
            villageInlitialSet = false;
            keepRepeatFlag = false;

            //陈泽林 20161010 更改为用户默认临时文件夹
            //defaultPath = "E:/";
            defaultPath = System.IO.Path.GetTempPath();
        }

        #endregion

    }
}
