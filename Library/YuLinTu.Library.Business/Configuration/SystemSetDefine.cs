/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;
using YuLinTu.Windows;
using YuLinTu.Windows.Wpf;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 系统管理--系统设置信息
    /// </summary>
    public class SystemSetDefine : NotifyCDObject
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
        private bool exportAddressToTown;
        private bool statisticSet;
        private bool upDownSet;
        private bool exportVPTableCountContainsDiedPerson;
        private bool exportTableSenderDesToVillage;
        private bool villageInlitialSet;
        private bool keepRepeatFlag;
        private bool landNumericFormatSet;
        private int landNumericFormatValueSet;
        private string defaultPath;
        private int versionNumber;
        private string backUpPath;
        private int backDay;
        private DateTime backUperDate;
        private int chooseArea;
        private string emptyReplacement;
        private bool statisticsDeadPersonInfo;

        private string dataExchangeDirectory;

        #endregion Fields

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
            set { cityTableHead = value; NotifyPropertyChanged("CityTableHead"); }
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

        #endregion 表头设置

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

        #endregion 报表设置

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
            set
            {
                nergionbourSet = value; NotifyPropertyChanged("NergionbourSet");
                //按苍松哥需求修改 by sunjie
                if (!value) { NergionbourSortSet = value; }
            }
        }

        /// <summary>
        /// 四至填充设置-选择填充四至，并且为东南西北显示
        /// </summary>
        public bool NergionbourSortSet
        {
            get { return nergionbourSortSet; }
            set { nergionbourSortSet = value; NotifyPropertyChanged("NergionbourSortSet"); }
        }

        /// <summary>
        /// 导出地址到镇处理
        /// </summary>
        public bool ExportAddressToTown
        {
            get { return exportAddressToTown; }
            set { exportAddressToTown = value; NotifyPropertyChanged("ExportAddressToTown"); }
        }

        #endregion 地块设置

        #region 其他设置

        /// <summary>
        /// 承包方调查表统计家庭成员总数包括已故人员-按照备注里面有【已故】两个字进行识别
        /// </summary>
        public bool ExportVPTableCountContainsDiedPerson
        {
            get { return exportVPTableCountContainsDiedPerson; }
            set { exportVPTableCountContainsDiedPerson = value; NotifyPropertyChanged("ExportVPTableCountContainsDiedPerson"); }
        }

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
        /// 导出表发包方名称到村
        /// </summary>
        public bool ExportTableSenderDesToVillage
        {
            get { return exportTableSenderDesToVillage; }
            set { exportTableSenderDesToVillage = value; NotifyPropertyChanged("ExportTableSenderDesToVillage"); }
        }

        /// <summary>
        /// 按村级进行户号、地块编码统一初始及签订
        /// </summary>
        public bool VillageInlitialSet
        {
            get { return villageInlitialSet; }
            set { villageInlitialSet = value; NotifyPropertyChanged("VillageInlitialSet"); }
        }

        public bool StatisticsDeadPersonInfo
        {
            get { return statisticsDeadPersonInfo; }
            set { statisticsDeadPersonInfo = value; NotifyPropertyChanged("StatisticsDeadPersonInfo"); }
        }

        public int rewritingStartNumber;
        public int RewritingStartNumber
        {
            get { return rewritingStartNumber; }
            set { rewritingStartNumber = value; NotifyPropertyChanged("RewritingStartNumber"); }
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
        /// 面积选择
        /// </summary>
        public int ChooseArea
        {
            get { return chooseArea; }
            set { chooseArea = value; NotifyPropertyChanged("ChooseArea"); }
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

        /// <summary>
        /// 版本号
        /// </summary>
        public int VersionNumber
        {
            get { return versionNumber; }
            set { versionNumber = value; NotifyPropertyChanged("VersionNumber"); }
        }

        /// <summary>
        /// 备份路径
        /// </summary>
        public string BackUpPath
        {
            get { return backUpPath; }
            set { backUpPath = value; NotifyPropertyChanged("BackUpPath"); }
        }

        /// <summary>
        /// 备份时的间隔天数
        /// </summary>
        public int BackDay
        {
            get { return backDay; }
            set { backDay = value; NotifyPropertyChanged("BackDay"); }
        }

        /// <summary>
        /// 上次备份时间
        /// </summary>
        public DateTime BackUperDate
        {
            get { return backUperDate; }
            set { backUperDate = value; NotifyPropertyChanged("BackUperDate"); }
        }

        /// <summary>
        /// 空数据的替代字符串
        /// </summary>
        public string EmptyReplacement
        {
            get { return emptyReplacement; }
            set { emptyReplacement = value; NotifyPropertyChanged("EmptyReplacement"); }
        }

        /// <summary>
        /// 数据缓存目录
        /// </summary>
        public string DataExchangeDirectory
        {
            get { return dataExchangeDirectory; }
            set { dataExchangeDirectory = value; NotifyPropertyChanged("dataExchangeDirectory"); }
        }

        /// <summary>
        /// 陈泽林 20161010 获取模板表头名称
        /// </summary>
        /// <returns></returns>
        public string GetTableHeaderStr(Zone currentZone)
        {
            string Name = "";
            if (tempTableHead)
                return Name;
            var dbContext = DataSource.Create<IDbContext>(TheBns.Current.GetDataSourceName());
            var zoneStation = dbContext.CreateZoneWorkStation();
            var zonelist = zoneStation.GetParentsToProvince(currentZone);
            if (currentZone.Level == eZoneLevel.Group)
            {
                if (groupTableHead)
                    Name = currentZone.Name + Name;
                currentZone = zonelist.Find(f => f.FullCode == currentZone.UpLevelCode);
            }
            if (currentZone.Level == eZoneLevel.Village)
            {
                if (countryTableHead)
                    Name = currentZone.Name + Name;
                currentZone = zonelist.Find(f => f.FullCode == currentZone.UpLevelCode);
            }
            if (currentZone.Level == eZoneLevel.Town)
            {
                if (townTableHead)
                    Name = currentZone.Name + Name;
                currentZone = zonelist.Find(f => f.FullCode == currentZone.UpLevelCode);
            }
            if (currentZone.Level == eZoneLevel.County)
            {
                if (countTableHead)
                    Name = currentZone.Name + Name;
                currentZone = zonelist.Find(f => f.FullCode == currentZone.UpLevelCode);
            }
            if (currentZone.Level == eZoneLevel.City)
            {
                if (cityTableHead)
                    Name = currentZone.Name + Name;
                currentZone = zonelist.Find(f => f.FullCode == currentZone.UpLevelCode);
            }
            return Name;
        }

        /// <summary>
        /// 陈泽林 20161011 获取填报单位名称取三级地域
        /// </summary>
        /// <returns></returns>
        public string GetTBDWStr(Zone currentZone)
        {
            var dbContext = DataSource.Create<IDbContext>(TheBns.Current.GetDataSourceName());
            var zoneStation = dbContext.CreateZoneWorkStation();
            Zone z1 = zoneStation.Get(currentZone.UpLevelCode);
            Zone z2 = zoneStation.Get(z1.UpLevelCode);
            Zone z3 = zoneStation.Get(z2.UpLevelCode);
            string str = "";
            //导出发包方到村,并且传入的地域是组级时
            if (currentZone.Level == eZoneLevel.Group && ExportTableSenderDesToVillage)
            {
                str = z3.Name + z2.Name + z1.Name;
            }
            else
                str = z2.Name + z1.Name + currentZone.Name;
            return str;
        }

        public string InitalizeAreaString()
        {
            return AreaIsZero ? " " : "0.00";
        }

        #endregion 其他设置

        #endregion Properties

        #region Ctor

        public SystemSetDefine()
        {
            tempTableHead = true;
            cityTableHead = true;
            countTableHead = false;
            townTableHead = false;
            countryTableHead = true;
            groupTableHead = false;
            statisticsDeadPersonInfo = true;
            sortTable = false;
            numberTill = true;
            personTable = false;

            areaIsZero = false;
            landGroup = true;
            nergionbourSet = false;
            nergionbourSortSet = false;
            exportAddressToTown = false;

            exportVPTableCountContainsDiedPerson = true;
            exportTableSenderDesToVillage = false;
            statisticSet = true;
            upDownSet = false;
            villageInlitialSet = false;
            keepRepeatFlag = false;
            defaultPath = System.IO.Path.GetTempPath();
            versionNumber = 820;
            backDay = 1;
            chooseArea = 1;//默认选择实测面积
            backUpPath = Path.Combine(TheApp.Current.GetDataPath(), "Backup");
            backUperDate = DateTime.Now;
            emptyReplacement = "/";
            RewritingStartNumber = 1;
        }

        private static SystemSetDefine _familyOtherDefine;

        /// <summary>
        /// 单例获取配置
        /// </summary>
        /// <returns></returns>
        public static SystemSetDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<SystemSetDefine>();
            var section = profile.GetSection<SystemSetDefine>();
            return _familyOtherDefine = section.Settings;
        }

        #endregion Ctor
    }
}