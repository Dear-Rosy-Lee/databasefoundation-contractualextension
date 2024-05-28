/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 委托代理声明书
    /// </summary>
    public class ExportDelegateBook : AgricultureWordBook
    {
        #region Fields

        private VirtualPerson currentFamily;
        private List<Person> persons;

        #endregion

        #region Property
        
        /// <summary>
        /// 权属名称
        /// </summary>
        public string RightName { get; set; }

        /// <summary>
        /// 政府名称
        /// </summary>
        public string UnitName { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ExportDelegateBook(VirtualPerson family)
        {
            if (family == null || family == currentFamily)
            { return; }
            currentFamily = family;
            persons = SortSharePerson(family.SharePersonList, currentFamily.Name);//排序共有人，并返回人口集合
        }

        #endregion

        #region Methods

        #region Priavte

        #endregion

        #region Override

        protected override bool OnSetParamValue(object data)
        {
            if (data == null)
            {
                return false;
            }
            base.OnSetParamValue(currentFamily);
            SetBookmarkValue("UnitName", UnitName);
            base.Destroyed();
            Disponse();
            return true;
        }

        /// <summary>
        /// 注销
        /// </summary>
        private void Disponse()
        {
            persons.Clear();
            GC.Collect();
        }

        ///// <summary>
        ///// 写地域扩展书签
        ///// </summary>
        //private void WriteZoneExpressBookMark()
        //{
        //    Zone currentZone = DB.Zone.Get(currentFamily.SenderCode);
        //    if (currentZone == null)
        //    {
        //        return;
        //    }
        //    string zoneName = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.County);
        //    SetBookmarkValue("bmCountryName", zoneName);
        //    for (int i = 0; i < 6; i++)
        //    {
        //        SetBookmarkValue("County" + (i == 0 ? "" : i.ToString()), zoneName);
        //        SetBookmarkValue("SmallCounty" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));

        //    }
        //    zoneName = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.Province);
        //    for (int i = 0; i < 6; i++)
        //    {
        //        SetBookmarkValue("Province" + (i == 0 ? "" : i.ToString()), zoneName);
        //        SetBookmarkValue("SmallProvince" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));

        //    }
        //    zoneName = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.City);
        //    for (int i = 0; i < 6; i++)
        //    {
        //        SetBookmarkValue("City" + (i == 0 ? "" : i.ToString()), zoneName);
        //        SetBookmarkValue("SmallCity" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));
        //    }
        //    zoneName = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.Town);
        //    for (int i = 0; i < 6; i++)
        //    {
        //        SetBookmarkValue("Town" + (i == 0 ? "" : i.ToString()), zoneName);
        //        SetBookmarkValue("SmallTown" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));
        //    }
        //    if (currentZone.Level >= eZoneLevel.Group)
        //    {
        //        zoneName = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.Village);
        //        for (int i = 0; i < 6; i++)
        //        {
        //            SetBookmarkValue("Village" + (i == 0 ? "" : i.ToString()), zoneName);
        //            SetBookmarkValue("SmallVillage" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1).Replace("社区", "").Replace("街道办事处", ""));
        //        }
        //    }
        //    if (currentZone.Level == eZoneLevel.Group)
        //    {
        //        zoneName = GetZoneNameByLevel(currentZone.FullCode, eZoneLevel.Group);
        //        for (int i = 0; i < 6; i++)
        //        {
        //            SetBookmarkValue("Group" + (i == 0 ? "" : i.ToString()), zoneName);
        //            string number = ToolString.GetLeftNumberWithInString(zoneName);
        //            string groupName = string.IsNullOrEmpty(number) ? zoneName : ToolMath.GetChineseLowNumber(number);
        //            SetBookmarkValue("ZoneName" + (i == 0 ? "" : i.ToString()), currentZone.FullName);//座落
        //            groupName = string.IsNullOrEmpty(number) ? zoneName : zoneName.Replace(number, ToolMath.GetChineseLowNumber(number));
        //            SetBookmarkValue("GroupName" + (i == 0 ? "" : i.ToString()), groupName);
        //            SetBookmarkValue("SmallGroup" + (i == 0 ? "" : i.ToString()), zoneName.Substring(0, zoneName.Length - 1));
        //        }
        //    }
        //    currentZone = null;
        //}

        ///// <summary>
        ///// 填写信息
        ///// </summary>
        //private void WriteFamilyInformation()
        //{
        //    for (int i = 0; i < 6; i++)
        //    {
        //        SetBookmarkValue("FamilyName" + (i == 0 ? "" : i.ToString()), VirtualPerson.InitalizeFamilyName(currentFamily.Name));
        //        SetBookmarkValue("ContractorTelephone" + (i == 0 ? "" : i.ToString()), string.IsNullOrEmpty(currentFamily.Telephone) ? "            " : currentFamily.Telephone);
        //        string gender = GetGender();
        //        SetBookmarkValue("Gender" + (i == 0 ? "" : i.ToString()), gender);
        //        string age = GetAge();
        //        SetBookmarkValue("Age" + (i == 0 ? "" : i.ToString()), age);
        //        SetBookmarkValue("IdentifyNumber" + (i == 0 ? "" : i.ToString()), currentFamily.Number);
        //        SetBookmarkValue("Comment" + (i == 0 ? "" : i.ToString()), currentFamily.Comment);
        //        string year = (Date != null && Date.HasValue) ? Date.Value.Year.ToString() : "   ";
        //        SetBookmarkValue("Year" + (i == 0 ? "" : i.ToString()), year);
        //        string month = (Date != null && Date.HasValue) ? Date.Value.Month.ToString() : "   ";
        //        SetBookmarkValue("Month" + (i == 0 ? "" : i.ToString()), month);
        //        string day = (Date != null && Date.HasValue) ? Date.Value.Day.ToString() : "   ";
        //        SetBookmarkValue("Day" + (i == 0 ? "" : i.ToString()), day);
        //        string fullDate = (Date != null && Date.HasValue) ? year + "年" + month + "月" + day + "日" : "   ";
        //        SetBookmarkValue("FullDate" + (i == 0 ? "" : i.ToString()), fullDate);
        //        year = (Date != null && Date.HasValue) ? ToolMath.GetChineseLowNimeric(Date.Value.Year.ToString()) : "   ";
        //        SetBookmarkValue("ChineseYear" + (i == 0 ? "" : i.ToString()), year);
        //        month = (Date != null && Date.HasValue) ? ToolMath.GetChineseLowNumber(Date.Value.Month.ToString()) : "   ";
        //        SetBookmarkValue("ChineseMonth" + (i == 0 ? "" : i.ToString()), month);
        //        day = (Date != null && Date.HasValue) ? ToolMath.GetChineseLowNumber(Date.Value.Day.ToString()) : "   ";
        //        SetBookmarkValue("ChineseDay" + (i == 0 ? "" : i.ToString()), day);
        //        fullDate = (Date != null && Date.HasValue) ? year + "年" + month + "月" + day + "日" : "   ";
        //        SetBookmarkValue("FullChineseDate" + (i == 0 ? "" : i.ToString()), fullDate);
        //    }
        //}

        //private string GetTownName(string fullCode)
        //{
        //    Zone town = DB.Zone.Get(fullCode.Substring(0, Zone.ZONE_TOWN_LENGTH));
        //    string townName = town != null ? town.Name : "";
        //    town = null;
        //    return townName;
        //}

        //private string GetZoneNameByLevel(string zoneCode, eZoneLevel level)
        //{
        //    Zone temp = DB.Zone.Get(zoneCode);
        //    if (temp == null)
        //        return " ";
        //    if (temp.Level == level)
        //        return temp.Name;
        //    else
        //        return GetZoneNameByLevel(temp.UpLevelCode, level);
        //}

        #endregion

        #endregion
    }
}
