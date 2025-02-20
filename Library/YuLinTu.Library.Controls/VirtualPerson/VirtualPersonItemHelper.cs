/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包方数据转换类
    /// </summary>
    public class VirtualPersonItemHelper
    {
        #region Methods

        /// <summary>
        /// 转换承包方为绑定实体
        /// </summary>
        public static VirtualPersonItem ConvertToItem(VirtualPerson vp, List<BindPerson> blist = null, bool isExpand = false, bool isAddChildren = true)
        {
            if (vp == null)
                return null;
            VirtualPersonItem item = new VirtualPersonItem();
            item.ID = vp.ID;
            item.ICN = vp.Number;
            item.Comment = vp.Comment;
            item.OldVirtualCode = vp.OldVirtualCode;
            item.Tag = vp;
            item.FamilyNumber = vp.FamilyNumber;
            item.Status = vp.Status;
            item.EquityArea = 0;
            item.EquityValue = 0;
            item.ZoneCode = vp.ZoneCode;
            item.HouseHolderName = vp.Name;
            item.Visibility = Visibility.Visible;
            if (isExpand)
            {
                //item.ContractorNumber = vp.FamilyExpand.ContractorNumber;
                item.ContractorNumber = vp.PersonCount;
            }
            item.TotalTableArea = vp.TotalTableArea;
            List<BindPerson> list = GetPersonList(vp);
            if (blist != null)
            {
                list.ForEach(t => blist.Add(t));
            }
            var num = vp.FamilyNumber == null ? "" : vp.FamilyNumber.PadLeft(4, '0');
            item.Name = vp.Name + "(共有人数:" + list.Count + ")" + "(户号:" + num + ")";//CreateItemName(vp.Name, vp.SharePersonList.Count, vp.FamilyNumber, vp.Status);
            if (isAddChildren)
                list.ForEach(t => item.Children.Add(t));
            item.Img = item.Status == eVirtualPersonStatus.Lock ? eImage.Lock : eImage.Family;
            return item;
        }

        /// <summary>
        /// 创建绑定项名称
        /// </summary>
        public static string CreateItemName(string name, int count, string familyNum, eVirtualPersonStatus status = eVirtualPersonStatus.Right)
        {
            string statusString = status == eVirtualPersonStatus.Right ? "" : "(已锁定)";
            return name + string.Format("(共有人数:{0})(户号:{1})" + statusString, count, familyNum.PadLeft(4, '0'));
        }

        /// <summary>
        /// 成员
        /// </summary>
        public static List<BindPerson> GetPersonList(VirtualPerson vp)
        {
            List<BindPerson> list = new List<BindPerson>();
            List<Person> personList = vp.SharePersonList;
            if (vp == null || personList == null || personList.Count == 0)
            {
                return list;
            }
            try
            {
                foreach (var item in personList)
                {
                    BindPerson bp = new BindPerson(item);
                    list.Add(bp);
                }
            }
            catch (Exception ex)
            {
                YuLinTu.Library.Log.Log.WriteException(personList, "获取成员GetPersonList", ex.Message + ex.StackTrace);
            }
            return list;
        }

        /// <summary>
        /// 创建承包方
        /// </summary>
        public static VirtualPerson CreateVirtualPerson(Zone zone, eContractorType type)
        {
            VirtualPerson p = new VirtualPerson();
            p.ZoneCode = zone.FullCode;
            p.VirtualType = eVirtualPersonType.Family;
            p.CardType = eCredentialsType.IdentifyCard;
            p.Founder = "Admin";
            p.Address = zone.FullName;
            p.FamilyExpand = new VirtualPersonExpand() { ContractorType = type };
            p.SharePersonList = new List<Person>();
            return p;
        }

        /// <summary>
        /// 创建承包方
        /// </summary>
        public static VirtualPerson CreateVirtualPerson(Person person)
        {
            VirtualPerson p = new VirtualPerson();
            p.ZoneCode = person.ZoneCode;
            p.VirtualType = eVirtualPersonType.Family;
            p.CardType = eCredentialsType.IdentifyCard;
            p.Founder = "Admin";
            p.Address = person.Address;
            p.Name = person.Name;
            p.Number = person.ICN;
            p.PersonCount = "1";
            p.SharePersonList = new List<Person>() { person };
            return p;
        }

        /// <summary>
        /// 成员
        /// </summary>
        public static Person CreatePerson(VirtualPerson vp)
        {
            Person p = new Person();
            p.ZoneCode = vp.ZoneCode;
            p.FamilyID = vp.ID;
            p.FamilyNumber = vp.FamilyNumber;
            p.Nation = eNation.Han;
            p.CardType = eCredentialsType.IdentifyCard;
            p.Gender = eGender.Unknow;
            return p;
        }

        /// <summary>
        /// 成员
        /// </summary>
        public static eImage ChangeByGender(eGender gender)
        {
            eImage img = eImage.Unknown;
            switch (gender)
            {
                case eGender.Male:
                    img = eImage.Man;
                    break;
                case eGender.Female:
                    img = eImage.Woman;
                    break;
                case eGender.Unknow:
                    img = eImage.Unknown;
                    break;
            }
            return img;
        }

        #endregion
    }

    /// <summary>
    /// 查询数据
    /// </summary>
    public class SearchNumber
    {
        /// <summary>
        /// 数据量
        /// </summary>
        public int DataCount { get; set; }

        /// <summary>
        /// 当前位置
        /// </summary>
        public int CurrentIndex { get; set; }
    }
}
