/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Office;
using YuLinTu.Library.Entity;
using System.Windows.Forms;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 户主代表声明书
    /// </summary>
    public class ExportApplyBook : AgricultureWordBook
    {
        #region Fields

        private VirtualPerson currentFamily;
        private List<Person> persons;
        private SystemSetDefine sysset = SystemSetDefine.GetIntence();
        #endregion

        #region Propertys

        /// <summary>
        /// 打印模式（不输出承包方，让现实承包方手填）
        /// </summary>
        public bool IsPrint { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 权属名称
        /// </summary>
        public string RightName { get; set; }

        /// <summary>
        /// 地域名称
        /// </summary>
        public string ZoneName { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造方法
        /// </summary>
        public ExportApplyBook(VirtualPerson family)
        {
            if (family == null || family == currentFamily)
            {
                return;
            }
            currentFamily = family.Clone() as VirtualPerson;
            persons = SortSharePerson(currentFamily.SharePersonList, currentFamily.Name);//排序共有人，并返回人口集合
            currentFamily.SharePersonList = persons;
            RightName = "农村土地承包经营权";
            base.TemplateName = "户主声明书";
            base.Tags = new object[1];
            base.Tags[0] = family;
        }

        #endregion

        #region Methods

        #region Priavte

        private string GetHouseHolderName()
        {
            string tempName = InitalizeFamilyName(currentFamily.Name);
            for (int i = 0; i < 3 - currentFamily.Name.Length; i++)
            {
                tempName.Insert(0, " ");
            }
            return tempName;
        }

        private string GetGender()
        {
            string value = EnumNameAttribute.GetDescription(persons[0].Gender);
            string sex = value == EnumNameAttribute.GetDescription(eGender.Unknow) ? "   " : value;
            return sex;
        }

        /// <summary>
        /// 获取身份证长度
        /// </summary>
        private string GetICN(int length)
        {
            string ICN = currentFamily.Number;
            if (string.IsNullOrEmpty(ICN))
            {
                return "                   。";
            }
            ICN = AddEmpty(ICN, length);
            return ICN;
        }

        /// <summary>
        /// 设置共有人
        /// </summary>
        private void SetPersons(int length)
        {
            string personNames = string.Empty;
            int personcount = 0;
            foreach (Person item in persons)
            {
                if (sysset.StatisticsDeadPersonInfo == false && item.Comment.IsNullOrEmpty() == false && item.Comment.Contains("去世"))
                {
                    continue;
                }
                personNames += item.Name == currentFamily.Name ? InitalizeFamilyName(item.Name) : item.Name;
                personNames += "、";
                personcount++;
            }
            personNames = personNames.Substring(0, personNames.Length - 1);
            personNames = AddEmpty(personNames, length);
            SetBookmarkValue("Persons", personNames);
            SetBookmarkValue("PersonCount", AddEmpty(personcount.ToString(), 3));
        }

        /// <summary>
        /// 添加空格
        /// </summary>
        private string AddEmpty(string str, int Emptycount)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            for (int i = 0; i < Emptycount - str.Length; i++)
            {
                if (i % 2 == 0)
                    str = "" + str;
                else
                    str = str + "";
            }
            return str;
        }

        #endregion

        #region Override

        protected override bool OnSetParamValue(object data)
        {
            if (data == null)
            {
                return false;
            }
            //DateValue = DateTime.Now;
            DateValue = Date;
            base.OnSetParamValue(currentFamily);
            SetBookmarkValue("ShortZoneName",$"{ZoneList.Where(x => x.Level == eZoneLevel.Province).FirstOrDefault().Name}" +
                                             $"{ZoneList.Where(x => x.Level == eZoneLevel.City).FirstOrDefault().Name}" +
                                             $"{ZoneList.Where(x => x.Level == eZoneLevel.County).FirstOrDefault().Name}");
            SetBookmarkValue("ZoneName", sysset.ExportAddressToTown ? AddressExporthHelper.GetNewAddressToTown(ZoneName) : ZoneName);
            SetBookmarkValue("RightType", RightName);

            //SetBookmarkValue("Date", (Date != null && Date.HasValue) ? string.Format("{0: yyyy 年 MM 月 dd 日}", Date) : "    年    月    日");

            int value = (Date != null && Date.HasValue) ? Date.Value.Year : 0;
            string valueString = value != 0 ? value.ToString() : "    ";
            SetBookmarkValue("bmYear", valueString);
            value = (Date != null && Date.HasValue) ? Date.Value.Month : 0;
            valueString = value != 0 ? value.ToString() : "    ";
            SetBookmarkValue("bmMonth", valueString);
            value = (Date != null && Date.HasValue) ? Date.Value.Day : 0;
            valueString = value != 0 ? value.ToString() : "    ";
            SetBookmarkValue("bmDay", valueString);

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

        #endregion

        #endregion
    }
}
