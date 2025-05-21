/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;
using YuLinTu.Data;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 配置
    /// </summary>
    public class AgricultureSetting
    {
        #region Helper

        /// <summary>
        /// 检查配置文件是否正确
        /// </summary>
        /// <returns></returns>
        public static bool CheckConfiguration()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config == null || !System.IO.File.Exists(config.FilePath))
            {
                return false;
            }
            return true;
        }

        private static SystemSetDefine SystemSet = SystemSetDefine.GetIntence();

        #endregion Helper

        #region 系统设置

        public const string ISUSESHAREPERSONVALUE = "IsUseSharePersonValue";//是否只使用共有人
        public const string ISUPDATEARCZONESPOT = "IsUpdateArcZoneSpot";//是否更新空间地域图斑
        public const string USETABLECITYTITLE = "UseTableCityTitle";//导出表格使用市
        public const string USETABLECOUNTRYTITLE = "UseTableCountryTitle";//导出表格使用区县
        public const string USETABLETOWNTITLE = "UseTableTownTitle";//导出表格使用乡镇
        public const string USETABLEVILLAGETITLE = "UseTableVillageTitle";//导出表格使用村
        public const string USETABLEGROUPTITLE = "UseTableGroupTitle";//导出表格使用组
        public const string USETABLESOURCETITLE = "UseTableSourceTitle";//导出表格使用源表头
        public const string USETEMPLATETITLE = "UseTemplateTitle";//使用模版中表头
        public const string LIMITEVILLAGEENABLE = "LimiteVillageEnable";//限制村级使用
        public const string SYSTEMDEFAULTDIRECTORY = "SystemDefaultDirectory";//系统默认目录
        public const string SYSTEMDEFAULTAREAFILLMODE = "SystemDefaultAreaFillMode";//系统默认面积填充模式
        public const string USESYSTEMLANDLEVELDESCRIPTION = "UseSystemLandLevelDescription";//使用系统默认地力等级描述
        public const string SYSTEMTABLEADDSUFFIX = "SystemTableAddSuffix";//系统默认表中添加后缀
        public const string SYSTEMTABLELANDNEIGHBORCONTENTS = "SystemTableLandNeighborContents";//系统默认表中使用四至内容
        public const string SYSTEMTABLELANDNEIGHBORDIRECTORY = "SystemTableLandNeighborDirectory";//系统默认表中四至内容填写按东、南、西、北填写
        public const string SYSTEMBOOKMARKNUMBER = "SystemBookMarkNumber";//系统默认书签数
        public const string SYSTEMVIRTUALPERSONSYNCHRONOUS = "SystemVirtualPersonSynchronous";//系统承包方同步

        /// <summary>
        /// 是否只使用共有人
        /// </summary>
        public static bool IsUseSharePersonValue
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(ISUSESHAREPERSONVALUE, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(ISUSESHAREPERSONVALUE, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(ISUSESHAREPERSONVALUE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(ISUSESHAREPERSONVALUE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 是否更新空间地域图斑
        /// </summary>
        public static bool IsUpdateArcZoneSpot
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(ISUPDATEARCZONESPOT, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(ISUPDATEARCZONESPOT, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(ISUPDATEARCZONESPOT, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(ISUPDATEARCZONESPOT, value.ToString());
                }
            }
        }

        /// <summary>
        /// 导出表格使用市
        /// </summary>
        public static bool UseTableCityTitle
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(USETABLECITYTITLE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(USETABLECITYTITLE, "true");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(USETABLECITYTITLE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(USETABLECITYTITLE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 导出表格是否使用区县
        /// </summary>
        public static bool UseTableCountryTitle
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(USETABLECOUNTRYTITLE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(USETABLECOUNTRYTITLE, "true");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(USETABLECOUNTRYTITLE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(USETABLECOUNTRYTITLE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 导出表格使用乡镇
        /// </summary>
        public static bool UseTableTownTitle
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(USETABLETOWNTITLE, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(USETABLETOWNTITLE, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(USETABLETOWNTITLE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(USETABLETOWNTITLE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 导出表格使用村
        /// </summary>
        public static bool UseTableVillageTitle
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(USETABLEVILLAGETITLE, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(USETABLEVILLAGETITLE, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(USETABLEVILLAGETITLE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(USETABLEVILLAGETITLE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 导出表格使用组
        /// </summary>
        public static bool UseTableGroupTitle
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(USETABLEGROUPTITLE, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(USETABLEGROUPTITLE, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(USETABLEGROUPTITLE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(USETABLEGROUPTITLE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 导出表格使用源表头
        /// </summary>
        public static bool UseTableSourceTitle
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(USETABLESOURCETITLE, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(USETABLESOURCETITLE, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(USETABLESOURCETITLE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(USETABLESOURCETITLE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        private static IDbContext CreateDb()
        {
            return DataBaseSource.GetDataBaseSource();
        }

        /// <summary>
        /// 初始化表头
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="zone">地域</param>
        /// <returns></returns>
        public static string InitalizeTitle(Zone zone)
        {
            if (AgricultureSetting.UseTableSourceTitle)
            {
                return "";
            }
            string zoneName = "";
            //Zone city = zone.FullCode.Length >= Zone.ZONE_CITY_LENGTH ? db.Zone.Get(zone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH)) : null;
            //Zone county = zone.FullCode.Length >= Zone.ZONE_COUNTY_LENGTH ? db.Zone.Get(zone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH)) : null;
            //Zone town = zone.FullCode.Length >= Zone.ZONE_TOWN_LENGTH ? db.Zone.Get(zone.FullCode.Substring(0, Zone.ZONE_TOWN_LENGTH)) : null;
            //Zone village = zone.FullCode.Length >= Zone.ZONE_VILLAGE_LENGTH ? db.Zone.Get(zone.FullCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH)) : null;
            AccountLandBusiness alb = new AccountLandBusiness(CreateDb());
            Zone group = new Zone();
            Zone village = new Zone();
            Zone town = new Zone();
            Zone county = new Zone();
            Zone city = new Zone();

            if (zone.Level == eZoneLevel.Group)
            {
                group = zone;
                village = alb.GetParent(group);
                town = alb.GetParent(village);
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (zone.Level == eZoneLevel.Village)
            {
                village = zone;
                town = alb.GetParent(village);
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }
            if (zone.Level == eZoneLevel.Town)
            {
                town = zone;
                county = alb.GetParent(town);
                city = alb.GetParent(county);
            }

            if (AgricultureSetting.UseTableCityTitle)
            {
                zoneName += city != null ? city.Name : "";
            }
            if (AgricultureSetting.UseTableCountryTitle)
            {
                zoneName += county != null ? county.Name : "";
            }
            if (AgricultureSetting.UseTableTownTitle)
            {
                zoneName += town != null ? town.Name : "";
            }
            if (AgricultureSetting.UseTableVillageTitle)
            {
                zoneName += village != null ? village.Name : "";
            }
            if (AgricultureSetting.UseTableGroupTitle)
            {
                zoneName += group != null ? group.Name : "";
            }
            city = null;
            county = null;
            town = null;
            village = null;
            group = null;

            return zoneName;
        }

        /// <summary>
        /// 使用模版中表头
        /// </summary>
        public static bool UseTemplateTitle
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(USETEMPLATETITLE, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(USETEMPLATETITLE, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(USETEMPLATETITLE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(USETEMPLATETITLE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 限制村级使用
        /// </summary>
        public static bool LimiteVillageEnable
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(LIMITEVILLAGEENABLE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(LIMITEVILLAGEENABLE, "true");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(LIMITEVILLAGEENABLE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(LIMITEVILLAGEENABLE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 系统默认面积填充模式
        /// </summary>
        public static bool SystemDefaultAreaFillMode
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(SYSTEMDEFAULTAREAFILLMODE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(SYSTEMDEFAULTAREAFILLMODE, "true");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(SYSTEMDEFAULTAREAFILLMODE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(SYSTEMDEFAULTAREAFILLMODE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 使用系统默认地力等级描述
        /// </summary>
        public static bool UseSystemLandLevelDescription
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(USESYSTEMLANDLEVELDESCRIPTION, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(USESYSTEMLANDLEVELDESCRIPTION, "true");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(USESYSTEMLANDLEVELDESCRIPTION, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(USESYSTEMLANDLEVELDESCRIPTION, value.ToString());
                }
            }
        }

        /// <summary>
        /// 系统默认表中添加后缀
        /// </summary>
        public static bool SystemTableAddSuffix
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(SYSTEMTABLEADDSUFFIX, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(SYSTEMTABLEADDSUFFIX, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(SYSTEMTABLEADDSUFFIX, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(SYSTEMTABLEADDSUFFIX, value.ToString());
                }
            }
        }

        /// <summary>
        /// 系统默认表中使用四至内容
        /// </summary>
        public static bool SystemTableLandNeighborContents
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(SYSTEMTABLELANDNEIGHBORCONTENTS, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(SYSTEMTABLELANDNEIGHBORCONTENTS, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(SYSTEMTABLELANDNEIGHBORCONTENTS, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(SYSTEMTABLELANDNEIGHBORCONTENTS, value.ToString());
                }
            }
        }

        /// <summary>
        /// 系统默认表中四至内容填写按东、南、西、北填写
        /// </summary>
        public static bool SystemTableLandNeighborDirectory
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(SYSTEMTABLELANDNEIGHBORDIRECTORY, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(SYSTEMTABLELANDNEIGHBORDIRECTORY, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(SYSTEMTABLELANDNEIGHBORDIRECTORY, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(SYSTEMTABLELANDNEIGHBORDIRECTORY, value.ToString());
                }
            }
        }

        /// <summary>
        /// 系统默认目录
        /// </summary>
        public static string SystemDefaultDirectory
        {
            get
            {
                bool config = CheckConfiguration();
                string filePath = System.IO.Path.GetTempPath();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(SYSTEMDEFAULTDIRECTORY, filePath) : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(SYSTEMDEFAULTDIRECTORY, filePath);
                if (string.IsNullOrEmpty(value) || !System.IO.Directory.Exists(value))
                {
                    value = System.IO.Path.GetTempPath();
                }
                return value;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(SYSTEMDEFAULTDIRECTORY, value);
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(SYSTEMDEFAULTDIRECTORY, value);
                }
            }
        }

        /// <summary>
        /// 系统默认书签数
        /// </summary>
        public static int SystemBookMarkNumber
        {
            get
            {
                int median = 0;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(SYSTEMBOOKMARKNUMBER, "5") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(SYSTEMBOOKMARKNUMBER, "5");
                Int32.TryParse(value, out median);
                return median <= 0 ? 5: median;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(SYSTEMBOOKMARKNUMBER, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(SYSTEMBOOKMARKNUMBER, value.ToString());
                }
            }
        }

        /// <summary>
        /// 系统承包方同步
        /// </summary>
        public static bool SystemVirtualPersonSynchronous
        {
            get
            {
                bool success = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(SYSTEMVIRTUALPERSONSYNCHRONOUS, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(SYSTEMVIRTUALPERSONSYNCHRONOUS, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(SYSTEMVIRTUALPERSONSYNCHRONOUS, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(SYSTEMVIRTUALPERSONSYNCHRONOUS, value.ToString());
                }
            }
        }

        /// <summary>
        /// 初始化表头
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="zone">地域</param>
        /// <returns></returns>
        //public static string InitalizeTitle(IDbContext db, Zone zone)
        //{
        //    if (AgricultureSetting.UseTableSourceTitle)
        //    {
        //        return "";
        //    }
        //    string zoneName = "";
        //    Zone city = zone.FullCode.Length >= Zone.ZONE_CITY_LENGTH ? db.Zone.Get(zone.FullCode.Substring(0, Zone.ZONE_CITY_LENGTH)) : null;
        //    Zone county = zone.FullCode.Length >= Zone.ZONE_COUNTY_LENGTH ? db.Zone.Get(zone.FullCode.Substring(0, Zone.ZONE_COUNTY_LENGTH)) : null;
        //    Zone town = zone.FullCode.Length >= Zone.ZONE_TOWN_LENGTH ? db.Zone.Get(zone.FullCode.Substring(0, Zone.ZONE_TOWN_LENGTH)) : null;
        //    Zone village = zone.FullCode.Length >= Zone.ZONE_VILLAGE_LENGTH ? db.Zone.Get(zone.FullCode.Substring(0, Zone.ZONE_VILLAGE_LENGTH)) : null;
        //    Zone group = zone.FullCode.Length >= Zone.ZONE_GROUP_LENGTH ? db.Zone.Get(zone.FullCode.Substring(0, Zone.ZONE_GROUP_LENGTH)) : null;
        //    if (AgricultureSetting.UseTableCityTitle)
        //    {
        //        zoneName += city != null ? city.Name : "";
        //    }
        //    if (AgricultureSetting.UseTableCountryTitle)
        //    {
        //        zoneName += county != null ? county.Name : "";
        //    }
        //    if (AgricultureSetting.UseTableTownTitle)
        //    {
        //        zoneName += town != null ? town.Name : "";
        //    }
        //    if (AgricultureSetting.UseTableVillageTitle)
        //    {
        //        zoneName += village != null ? village.Name : "";
        //    }
        //    if (AgricultureSetting.UseTableGroupTitle)
        //    {
        //        zoneName += group != null ? group.Name : "";
        //    }
        //    city = null;
        //    county = null;
        //    town = null;
        //    village = null;
        //    group = null;
        //    return zoneName;
        //}

        /// <summary>
        /// 初始化面积字符串
        /// </summary>
        /// <returns></returns>
        public static string InitalizeAreaString(int number = 2, bool isSprit = true)
        {
            if (isSprit)
                return SystemSet?.EmptyReplacement;
            switch (number)
            {
                case 1:
                    return SystemDefaultAreaFillMode ? " " : "0.0";

                case 2:
                    return SystemDefaultAreaFillMode ? " " : "0.00";

                case 3:
                    return SystemDefaultAreaFillMode ? " " : "0.000";

                case 4:
                    return SystemDefaultAreaFillMode ? " " : "0.0000";

                default:
                    return SystemDefaultAreaFillMode ? " " : "0.00";
            }
        }

        /// <summary>
        /// 初始化数量字符串
        /// </summary>
        /// <returns></returns>
        public static string InitalizeCountString()
        {
            return SystemDefaultAreaFillMode ? " " : "0";
        }

        #endregion 系统设置

        #region 编码设置

        public const string AGRICULTURELANDENCODINGRULE = "AgricultureLandEncodingRule";//农业部编码规则

        /// <summary>
        /// 农业部编码规则
        /// </summary>
        public static int AgricultureLandEncodingRule
        {
            get
            {
                int median = 0;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDENCODINGRULE, "1") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(AGRICULTURELANDENCODINGRULE, "1");
                Int32.TryParse(value, out median);
                return median <= 0 ? 0 : median;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDENCODINGRULE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(AGRICULTURELANDENCODINGRULE, value.ToString());
                }
            }
        }

        #endregion 编码设置

        #region 承包方

        public const string CONTRACTORCHECKTELEPHONE = "ContractorCheckTelephone";//承包方检查电话号码
        public const string CONTRACTORCHECKCELLTELEPHONE = "ContractorCheckCellTelephone";//承包方检查手机电话号码
        public const string CONTRACTORCHECKRELATION = "ContractorCheckRelation";//承包方检查家庭关系
        public const string CONTRACTORCHECKCARDNUMBER = "ContractorCheckCardNumber";//承包方检查证件号码

        /// <summary>
        /// 承包方检查电话号码
        /// </summary>
        public static bool ContractorCheckTelephone
        {
            get
            {
                bool success = false;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(CONTRACTORCHECKTELEPHONE, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(CONTRACTORCHECKTELEPHONE, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(CONTRACTORCHECKTELEPHONE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(CONTRACTORCHECKTELEPHONE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 承包方检查手机电话号码
        /// </summary>
        public static bool ContractorCheckCellTelephone
        {
            get
            {
                bool success = false;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(CONTRACTORCHECKCELLTELEPHONE, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(CONTRACTORCHECKCELLTELEPHONE, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(CONTRACTORCHECKCELLTELEPHONE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(CONTRACTORCHECKCELLTELEPHONE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 承包方检查家庭关系
        /// </summary>
        public static bool ContractorCheckRelation
        {
            get
            {
                bool success = false;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(CONTRACTORCHECKRELATION, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(CONTRACTORCHECKRELATION, "true");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(CONTRACTORCHECKRELATION, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(CONTRACTORCHECKRELATION, value.ToString());
                }
            }
        }

        /// <summary>
        /// 承包方检查证件号码
        /// </summary>
        public static bool ContractorCheckCardNumber
        {
            get
            {
                bool success = false;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(CONTRACTORCHECKCARDNUMBER, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(CONTRACTORCHECKCARDNUMBER, "true");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(CONTRACTORCHECKCARDNUMBER, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(CONTRACTORCHECKCARDNUMBER, value.ToString());
                }
            }
        }

        #endregion 承包方

        #region 二轮台帐

        public const string SHOWSECONDTABLEDATANEIGHBORWIDTHDIRECTION = "ShowSecondTableDataNeighborWithDirection";//设置二轮台帐表格输出四至显示东、南、西、北

        /// <summary>
        /// 设置二轮台帐表格输出四至显示东
        /// </summary>
        public static bool ShowSecondTableDataNeighborWithDirection
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(SHOWSECONDTABLEDATANEIGHBORWIDTHDIRECTION, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(SHOWSECONDTABLEDATANEIGHBORWIDTHDIRECTION, "true");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(SHOWSECONDTABLEDATANEIGHBORWIDTHDIRECTION, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(SHOWSECONDTABLEDATANEIGHBORWIDTHDIRECTION, value.ToString());
                }
            }
        }

        #endregion 二轮台帐

        #region 户籍调查表设置

        public const string ALLOWIDENTIFYNUMBERREPEAT = "AllowIdentifyNumberRepeat";//允许身份证号码重复

        /// <summary>
        /// 允许身份证号码重复
        /// </summary>
        public static bool AllowIdentifyNumberRepeat
        {
            get
            {
                bool success = false;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(ALLOWIDENTIFYNUMBERREPEAT, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(ALLOWIDENTIFYNUMBERREPEAT, "false");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(ALLOWIDENTIFYNUMBERREPEAT, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(ALLOWIDENTIFYNUMBERREPEAT, value.ToString());
                }
            }
        }

        #endregion 户籍调查表设置

        #region 地域设置

        #region 编码设置

        public const string USEZONENAMEFORSENDERNAME = "UseZoneNameForSenderName";//使用现在区域名称作为发包方名称
        public const string ISSUPPORTAGRICULTUREZONECODE = "IsSupportAgricultureCode";//是否支持农业部编码

        /// <summary>
        /// 是否支持农业部编码
        /// </summary>
        public static bool IsSupportAgricultureCode
        {
            get
            {
                bool success = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(ISSUPPORTAGRICULTUREZONECODE, "true");
                Boolean.TryParse(value, out success);
                return success;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(ISSUPPORTAGRICULTUREZONECODE, value.ToString());
            }
        }

        #endregion 编码设置

        #endregion 地域设置

        #region 公共设置

        public const string AGRICUTLTURELANDNUMBERMEDIAN = "AgricultureLandNumberMedian";//设置地块编码导出位数
        public const string AGRICULTURESURVEYBOUNDARYDESCRIPTION = "AgricultureSurveyBoundaryDescription";//地块调查表中界址线说明是否使用长度填充
        public const string AGRICULTURESURVEYLANDNEIGHBOR = "AgricultureSurveyLandNeighbor";//地块调查表按毗邻承包方处理

        /// <summary>
        /// 地块调查表按毗邻承包方处理
        /// </summary>
        public static bool AgricultureSurveyLandNeighbor
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURESURVEYLANDNEIGHBOR, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(AGRICULTURESURVEYLANDNEIGHBOR, "false");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURESURVEYLANDNEIGHBOR, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(AGRICULTURESURVEYLANDNEIGHBOR, value.ToString());
                }
            }
        }

        /// <summary>
        /// 设置地块编码导出位数
        /// </summary>
        public static int AgricultureLandNumberMedian
        {
            get
            {
                int median = 0;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(AGRICUTLTURELANDNUMBERMEDIAN, "0") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(AGRICUTLTURELANDNUMBERMEDIAN, "0");
                Int32.TryParse(value, out median);
                return median < 0 ? 0 : median;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(AGRICUTLTURELANDNUMBERMEDIAN, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(AGRICUTLTURELANDNUMBERMEDIAN, value.ToString());
                }
            }
        }

        /// <summary>
        /// 地块调查表中界址线说明是否使用长度填充
        /// </summary>
        public static bool AgricultureSurveyBoundaryDescription
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURESURVEYBOUNDARYDESCRIPTION, "false") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(AGRICULTURESURVEYBOUNDARYDESCRIPTION, "false");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURESURVEYBOUNDARYDESCRIPTION, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(AGRICULTURESURVEYBOUNDARYDESCRIPTION, value.ToString());
                }
            }
        }

        #endregion 公共设置

        #region 导出Word

        public const string AGRICULTURELANDWORDFAMILYNUMBER = "AgricultureLandWordFamilyNumber";//是否显示承包方编码全编码

        /// <summary>
        /// 是否显示承包方编码全编码
        /// </summary>
        public static bool AgricultureLandWordFamilyNumber
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDFAMILYNUMBER, "false");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDFAMILYNUMBER, value.ToString());
            }
        }

        public const string AutoLoadVirtualPerson = "AutoLoadVirtualPerson";

        public const string StaticsInformationByFamily = "StaticsInformationByFamily";

        public const string SetHouselderStatementDate = "SetHouselderStatementDate";

        public const string SetProxyStatementDate = "SetProxyStatementDate";

        public const string SetPulicliyStatementDate = "SetPulicliyStatementDate";

        public const string SetMeasureRequireDate = "SetMeasureRequireDate";

        public const string PulicliyFamilyInformation = "PulicliyFamilyInformation";

        public const string ExportTableByFamilyNumber = "ExportFamilyTableByFamilyNumber";

        public const string ExportTableNumberByFamilyNumber = "ExportFamilyTableNumberByFamilyNumber";

        #endregion 导出Word

        #region 单户调查表设置

        public const string EXPORTFAMILYSECONDLANDTYPESURVEYTBALE = "ShowFamilySecondLandTypeSurveyTable";//是否显示二轮地类
        public const string EXPORTFAMILYSECONDLANDNAMESURVEYTBALE = "ShowFamilySecondLandNameSurveyTable";//是否显示二轮地块名称
        public const string EXPORTFAMILYSECONDLANDNEIGHBORSURVEYTBALE = "ShowFamilySecondLandNeighborSurveyTable";//是否显示二轮四至

        public const string EXPORTFAMILYLANDTYPESURVEYTBALE = "ShowFamilyLandTypeSurveyTable";//是否显示地类
        public const string EXPORTFAMILYLANDNEIGHBORSURVEYTBALE = "ShowFamilyLandNeighborSurveyTable";//是否显示四至
        public const string EXPORTFAMILYLANDCONTRACTMODESURVEYTBALE = "ShowFamilyLandContractModeSurveyTable";//是否显示承包方式
        public const string EXPORTFAMILYISTRANSFERSURVEYTBALE = "ShowFamilyIsTransferSurveyTable";//是否显示是否流转
        public const string EXPORTFAMILYTRANSFERMODESURVEYTBALE = "ShowFamilyTransferModeSurveyTable";//是否显示流转方式
        public const string EXPORTFAMILYTRANSFERTERMSURVEYTBALE = "ShowFamilyTransferTermSurveyTable";//是否显示流转期限
        public const string EXPORTFAMILYLANDPLATTYPESURVEYTBALE = "ShowFamilyLandPlatTypeSurveyTable";//是否显示种植类型
        public const string EXPORTFAMILYLANDCOMMENTSURVEYTBALE = "ShowFamilyLandCommentSurveyTable";//是否显示备注

        /// <summary>
        /// 是否显示地类
        /// </summary>
        public static bool ShowFamilySecondLandTypeSurveyTable
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(EXPORTFAMILYSECONDLANDTYPESURVEYTBALE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(EXPORTFAMILYSECONDLANDTYPESURVEYTBALE, "true");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(EXPORTFAMILYSECONDLANDTYPESURVEYTBALE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(EXPORTFAMILYSECONDLANDTYPESURVEYTBALE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 是否显示二轮地块名称
        /// </summary>
        public static bool ShowFamilySecondLandNameSurveyTable
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(EXPORTFAMILYSECONDLANDNAMESURVEYTBALE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(EXPORTFAMILYSECONDLANDNAMESURVEYTBALE, "true");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(EXPORTFAMILYSECONDLANDNAMESURVEYTBALE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(EXPORTFAMILYSECONDLANDNAMESURVEYTBALE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 是否显示二轮四至
        /// </summary>
        public static bool ShowFamilySecondLandNeighborSurveyTable
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(EXPORTFAMILYSECONDLANDNEIGHBORSURVEYTBALE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(EXPORTFAMILYSECONDLANDNEIGHBORSURVEYTBALE, "true");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(EXPORTFAMILYSECONDLANDNEIGHBORSURVEYTBALE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(EXPORTFAMILYSECONDLANDNEIGHBORSURVEYTBALE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 是否显示地类
        /// </summary>
        public static bool ShowFamilyLandTypeSurveyTable
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(EXPORTFAMILYLANDTYPESURVEYTBALE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(EXPORTFAMILYLANDTYPESURVEYTBALE, "true");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(EXPORTFAMILYLANDTYPESURVEYTBALE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(EXPORTFAMILYLANDTYPESURVEYTBALE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 是否显示四至
        /// </summary>
        public static bool ShowFamilyLandNeighborSurveyTable
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(EXPORTFAMILYLANDNEIGHBORSURVEYTBALE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(EXPORTFAMILYLANDNEIGHBORSURVEYTBALE, "true");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(EXPORTFAMILYLANDNEIGHBORSURVEYTBALE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(EXPORTFAMILYLANDNEIGHBORSURVEYTBALE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 是否显示承包方式
        /// </summary>
        public static bool ShowFamilyLandContractModeSurveyTable
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(EXPORTFAMILYLANDCONTRACTMODESURVEYTBALE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(EXPORTFAMILYLANDCONTRACTMODESURVEYTBALE, "true");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(EXPORTFAMILYLANDCONTRACTMODESURVEYTBALE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(EXPORTFAMILYLANDCONTRACTMODESURVEYTBALE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 显示是否流转
        /// </summary>
        public static bool ShowFamilyIsTransferSurveyTable
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(EXPORTFAMILYISTRANSFERSURVEYTBALE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(EXPORTFAMILYISTRANSFERSURVEYTBALE, "true");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(EXPORTFAMILYISTRANSFERSURVEYTBALE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(EXPORTFAMILYISTRANSFERSURVEYTBALE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 是否显示流转方式
        /// </summary>
        public static bool ShowFamilyTransferModeSurveyTable
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(EXPORTFAMILYTRANSFERMODESURVEYTBALE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(EXPORTFAMILYTRANSFERMODESURVEYTBALE, "true");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(EXPORTFAMILYTRANSFERMODESURVEYTBALE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(EXPORTFAMILYTRANSFERMODESURVEYTBALE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 是否显示流转期限
        /// </summary>
        public static bool ShowFamilyTransferTermSurveyTable
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(EXPORTFAMILYTRANSFERTERMSURVEYTBALE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(EXPORTFAMILYTRANSFERTERMSURVEYTBALE, "true");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(EXPORTFAMILYTRANSFERTERMSURVEYTBALE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(EXPORTFAMILYTRANSFERTERMSURVEYTBALE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 是否显示种植类型
        /// </summary>
        public static bool ShowFamilyLandPlatTypeSurveyTable
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(EXPORTFAMILYLANDPLATTYPESURVEYTBALE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(EXPORTFAMILYLANDPLATTYPESURVEYTBALE, "true");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(EXPORTFAMILYLANDPLATTYPESURVEYTBALE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(EXPORTFAMILYLANDPLATTYPESURVEYTBALE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 是否显示流转期限
        /// </summary>
        public static bool ShowFamilyLandCommentSurveyTable
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(EXPORTFAMILYLANDCOMMENTSURVEYTBALE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(EXPORTFAMILYLANDCOMMENTSURVEYTBALE, "true");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(EXPORTFAMILYLANDCOMMENTSURVEYTBALE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(EXPORTFAMILYLANDCOMMENTSURVEYTBALE, value.ToString());
                }
            }
        }

        #endregion 单户调查表设置

        #region 集体建设用地使用权相关设置

        /// <summary>
        /// 宗地信息绘图员
        /// </summary>
        public const string DRAWPERSON = "DrawPerson";

        /// <summary>
        /// 宗地信息审核员
        /// </summary>
        public const string VERIFYPERSON = "VerifyPerson";

        #endregion 集体建设用地使用权相关设置

        #region 地块分布图

        public const string AGRICULTURELANDWORDLANDSORTCATALOG = "AgricultureLandWordLandSortCatalog";//是否按地块类别排序
        public const string AGRICULTURELANDWORDLANDSORTTYPE = "AgricultureLandWordLandSortType";//是否按地块属性排序 1.地块编码 2.地块名称 3.地块面积
        public const string AGRICULTURELANDWORDUSEBOOKMARK = "AgricultureLandWordUseBookMark";//空间形状位置使用书签定位
        public const string USEAGRICULTURELANDWORDPARCELSTYLE = "UseAgricultureLandWordParcelStyle";//使用农业部要求地块示意图
        public const string AGRICULTURELANDWORDUSELONGITUDINALTEMPLATE = "AgricultureLandWordUseLongitudinalTemplate";//使用纵向模版
        public const string AGRICULTURELANDWORDLANDROWCOUNT = "AgricultureLandWordLandRowCount";//首页地块显示行数
        public const string AGRICULTURELANDWORDLANDPAGENUMBER = "AgricultureLandWordLandPageNumber";//单页显示地块行数
        public const string AGRICULTURELANDWORDLANDWIDTH = "AgricultureLandWordLandWidth";//显示地块宽度
        public const string AGRICULTURELANDWORDLANDHEIGHT = "AgricultureLandWordLandHeight";//显示地块高度
        public const string AGRICULTURELANDWORDLANDSCAPEFIEXED = "AgricultureLandWordLandScapeFiexed";//固定地块范围
        public const string AGRICULTURELANDWORDSHOWLANDNUMBER = "AgricultureLandWordShowLandNumber";//显示地块编码
        public const string AGRICULTURELANDWORDLANDLABEL = "AgricultureLandWordLandLabel";//是否显示承包地标注
        public const string AGRICULTURELANDWORDLANDNUMBERMEDIAN = "AgricultureLandWordLandNumberMedian";//地块编码位数
        public const string AGRICULTURELANDWORDSHOWLANDNAME = "AgricultureLandWordShowLandName";//显示地块名称
        public const string AGRICULTURELANDWORDSHOWTABLEAREA = "AgricultureLandWordShowTableArea";//显示合同面积
        public const string AGRICULTURELANDWORDTABLEAREAALISENAME = "AgricultureLandWordTableAreaAliseName";//二轮合同面积别名
        public const string AGRICULTURELANDWORDSHOWACTUALAREA = "AgricultureLandWordShowActualArea";//显示实测面积
        public const string AGRICULTURELANDWORDSHOWAWAREAREA = "AgricultureLandWordShowAwareArea";//显示确权面积
        public const string AGRICULTURELANDWORDLANDNEIGHBOR = "AgricultureLandWordLandNeighbor";//是否显示承包地四至
        public const string AGRICULTURELANDWORDLANDCATALOG = "AgricultureLandWordLandCatalog";//是否显示承包地类别
        public const string AGRICULTURELANDWORDLANDNEIGHBORMODE = "AgricultureLandWordLandNeighborMode";//土地四至显示模式
        public const string AGRICULTURELANDWORDALLDATAWIDTH = "AgricultureLandWordLandAllDataWidth";//显示所有数据宽度
        public const string AGRICULTURELANDWORDALLDATAHEIGHT = "AgricultureLandWordLandAllDataHeight";//显示所有数据高度
        public const string AGRICULTURELANDWORDENGLESCAPEFIEXED = "AgricultureLandWordEngleScapeFiexed";//固定鹰眼图范围
        public const string AGRICULTURELANDWORDNEIGHBORBUFFER = "AgricultureLandWordNeighborBuffer";//邻宗地缓冲距离

        /// <summary>
        /// 邻宗地缓冲距离
        /// </summary>
        public static double AgricultureLandWordNeighborBuffer
        {
            get
            {
                double median = 0;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDNEIGHBORBUFFER, "0.0");
                double.TryParse(value, out median);
                return median;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDNEIGHBORBUFFER, value.ToString());
            }
        }

        /// <summary>
        /// 固定鹰眼图范围
        /// </summary>
        public static bool AgricultureLandWordEngleScapeFiexed
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDENGLESCAPEFIEXED, "false");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDENGLESCAPEFIEXED, value.ToString());
            }
        }

        /// <summary>
        /// 显示所有数据高度
        /// </summary>
        public static int AgricultureLandWordLandAllDataHeight
        {
            get
            {
                int median = 0;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDALLDATAHEIGHT, "380");
                Int32.TryParse(value, out median);
                return median;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDALLDATAHEIGHT, value.ToString());
            }
        }

        /// <summary>
        /// 显示所有数据宽度
        /// </summary>
        public static int AgricultureLandWordLandAllDataWidth
        {
            get
            {
                int median = 0;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDALLDATAWIDTH, "370");
                Int32.TryParse(value, out median);
                return median;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDALLDATAWIDTH, value.ToString());
            }
        }

        /// <summary>
        /// 土地四至显示模式
        /// </summary>
        public static bool AgricultureLandWordLandNeighborMode
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDLANDNEIGHBORMODE, "false");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDLANDNEIGHBORMODE, value.ToString());
            }
        }

        /// <summary>
        /// 是否显示承包地类别
        /// </summary>
        public static bool AgricultureLandWordLandCatalog
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDLANDCATALOG, "false");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDLANDCATALOG, value.ToString());
            }
        }

        /// <summary>
        /// 是否按地块类别排序
        /// </summary>
        public static bool AgricultureLandWordLandSortCatalog
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDLANDSORTCATALOG, "false");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDLANDSORTCATALOG, value.ToString());
            }
        }

        /// <summary>
        /// 是否按地块属性排序1.地块编码 2.地块名称 3.地块面积
        /// </summary>
        public static int AgricultureLandWordLandSortType
        {
            get
            {
                int median = 1;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDLANDSORTTYPE, "1");
                Int32.TryParse(value, out median);
                return median == 0 ? 1 : median;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDLANDSORTTYPE, value.ToString());
            }
        }

        /// <summary>
        /// 空间形状位置使用书签定位
        /// </summary>
        public static bool AgricultureLandWordUseBookMark
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDUSEBOOKMARK, "false");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDUSEBOOKMARK, value.ToString());
            }
        }

        /// <summary>
        /// 使用农业部要求地块示意图
        /// </summary>
        public static bool UseAgricultureLandWordParcelStyle
        {
            get
            {
                bool isShow = true;
                bool config = CheckConfiguration();
                string value = config ? ToolConfiguration.GetSpecialAppSettingValue(USEAGRICULTURELANDWORDPARCELSTYLE, "true") : ToolAssemblyInfoConfig.GetSpecialAppSettingValue(USEAGRICULTURELANDWORDPARCELSTYLE, "true");
                Boolean.TryParse(value, out isShow);
                return isShow;
            }
            set
            {
                bool config = CheckConfiguration();
                if (config)
                {
                    ToolConfiguration.SetSpecialAppSettingValue(USEAGRICULTURELANDWORDPARCELSTYLE, value.ToString());
                }
                else
                {
                    ToolAssemblyInfoConfig.SetSpecialAppSettingValue(USEAGRICULTURELANDWORDPARCELSTYLE, value.ToString());
                }
            }
        }

        /// <summary>
        /// 使用纵向模版
        /// </summary>
        public static bool AgricultureLandWordUseLongitudinalTemplate
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDUSELONGITUDINALTEMPLATE, "true");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDUSELONGITUDINALTEMPLATE, value.ToString());
            }
        }

        /// <summary>
        /// 首页地块显示行数
        /// </summary>
        public static int AgricultureLandWordLandRowCount
        {
            get
            {
                int median = 0;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDLANDROWCOUNT, "2");
                Int32.TryParse(value, out median);
                return median;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDLANDROWCOUNT, value.ToString());
            }
        }

        /// <summary>
        /// 单页显示地块行数
        /// </summary>
        public static int AgricultureLandWordLandPageNumber
        {
            get
            {
                int median = 0;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDLANDPAGENUMBER, "6");
                Int32.TryParse(value, out median);
                return median;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDLANDPAGENUMBER, value.ToString());
            }
        }

        /// <summary>
        /// 显示地块宽度
        /// </summary>
        public static int AgricultureLandWordLandWidth
        {
            get
            {
                int median = 0;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDLANDWIDTH, "150");
                Int32.TryParse(value, out median);
                return median;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDLANDWIDTH, value.ToString());
            }
        }

        /// <summary>
        /// 显示地块高度
        /// </summary>
        public static int AgricultureLandWordLandHeight
        {
            get
            {
                int median = 0;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDLANDHEIGHT, "145");
                Int32.TryParse(value, out median);
                return median;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDLANDHEIGHT, value.ToString());
            }
        }

        /// <summary>
        /// 固定地块范围
        /// </summary>
        public static bool AgricultureLandWordLandScapeFiexed
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDLANDSCAPEFIEXED, "false");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDLANDSCAPEFIEXED, value.ToString());
            }
        }

        /// <summary>
        /// 显示地块编码
        /// </summary>
        public static bool AgricultureLandWordShowLandNumber
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDSHOWLANDNUMBER, "true");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDSHOWLANDNUMBER, value.ToString());
            }
        }

        /// <summary>
        /// 是否显示承包地标注
        /// </summary>
        public static bool AgricultureLandWordLandLabel
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDLANDLABEL, "true");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDLANDLABEL, value.ToString());
            }
        }

        /// <summary>
        /// 地块编码位数
        /// </summary>
        public static int AgricultureLandWordLandNumberMedian
        {
            get
            {
                int median = 0;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDLANDNUMBERMEDIAN, "14");
                Int32.TryParse(value, out median);
                return median;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDLANDNUMBERMEDIAN, value.ToString());
            }
        }

        /// <summary>
        /// 显示地块名称
        /// </summary>
        public static bool AgricultureLandWordShowLandName
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDSHOWLANDNAME, "true");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDSHOWLANDNAME, value.ToString());
            }
        }

        /// <summary>
        /// 显示合同面积
        /// </summary>
        public static bool AgricultureLandWordShowTableArea
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDSHOWTABLEAREA, "false");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDSHOWTABLEAREA, value.ToString());
            }
        }

        /// <summary>
        /// 二轮合同面积别名
        /// </summary>
        public static string AgricultureLandWordTableAreaAliseName
        {
            get
            {
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDTABLEAREAALISENAME, "");
                if (!string.IsNullOrEmpty(value))
                {
                    value = value.Replace(",", "");
                }
                return value;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDTABLEAREAALISENAME, value);
            }
        }

        /// <summary>
        /// 显示实测面积
        /// </summary>
        public static bool AgricultureLandWordShowActualArea
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDSHOWACTUALAREA, "true");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDSHOWACTUALAREA, value.ToString());
            }
        }

        /// <summary>
        /// 显示确权面积
        /// </summary>
        public static bool AgricultureLandWordShowAwareArea
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDSHOWAWAREAREA, "false");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDSHOWAWAREAREA, value.ToString());
            }
        }

        /// <summary>
        /// 是否显示承包地四至
        /// </summary>
        public static bool AgricultureLandWordLandNeighbor
        {
            get
            {
                bool canShow = true;
                string value = ToolConfiguration.GetSpecialAppSettingValue(AGRICULTURELANDWORDLANDNEIGHBOR, "true");
                Boolean.TryParse(value, out canShow);
                return canShow;
            }
            set
            {
                ToolConfiguration.SetSpecialAppSettingValue(AGRICULTURELANDWORDLANDNEIGHBOR, value.ToString());
            }
        }

        #endregion 地块分布图
    }
}