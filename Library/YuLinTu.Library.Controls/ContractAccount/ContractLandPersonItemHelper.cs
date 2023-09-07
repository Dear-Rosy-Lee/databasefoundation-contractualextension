/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using YuLinTu.Library.Entity;
using YuLinTu.Library.Business;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 承包台账绑定实体(承包方、台账地块)转换类
    /// </summary>
    public class ContractLandPersonItemHelper
    {
        #region Method

        /// <summary>
        /// 转换承包方、台账地块实体为绑定实体
        /// </summary>
        public static ContractLandPersonItem ConvertToItem(VirtualPerson tableVp, List<ContractLand> accountLandList, List<Dictionary> listDKLB, List<Dictionary> listDLDJ)
        {
            if (tableVp == null)
            {
                return null;
            }
            ContractLandPersonItem item = tableVp.ConvertTo<ContractLandPersonItem>();
            item.Tag = tableVp;
            List<ContractLand> list = accountLandList.FindAll(t => t.OwnerId == tableVp.ID);
            foreach (var land in list)
            {
                ContractLandBinding cb = new ContractLandBinding(land);
                ConvertCodeToName(cb, listDKLB, listDLDJ);
                item.Children.Add(cb);
            }
            if (item.Children != null && item.Children.Count() > 0)
            {
                item.Visibility = Visibility.Visible;
            }
            item.Name = CreateItemName(tableVp, item.Children.Count);
            item.LandNumber = CreateItemNumber(tableVp);
            item.ActualAreaUI = SumActualArea(item);
            item.AwareAreaUI = SumAwareArea(item);
            item.TableAreaUI = SumTableArea(item);
            item.Img = item.Tag.Status == eVirtualPersonStatus.Lock ? 3 : 0;
            return item;
        }

        /// <summary>
        /// 将编码转换为名称
        /// </summary>
        /// <param name="landBinding">承包地块(界面实体)</param>
        /// <param name="dicts">数据字典集合</param>
        public static void ConvertCodeToName(ContractLandBinding landBinding, List<Dictionary> listDKLB, List<Dictionary> listDLDJ)
        {
            var category = listDKLB.Find(t => (!string.IsNullOrEmpty(t.Code)) && t.Code == landBinding.Tag.LandCategory);
            string landCategory = category == null ? "" : category.Name;
            landBinding.LandCategoryUI = string.IsNullOrEmpty(landCategory) ? "" : landCategory;
            var level = listDLDJ.Find(t => (!string.IsNullOrEmpty(t.Code)) && t.Code == landBinding.Tag.LandLevel);
            string landLevel = level == null ? "" : level.Name;
            landBinding.LandLevelUI = string.IsNullOrEmpty(landLevel) ? "" : landLevel;
            if (landBinding.Tag.IsFarmerLand == null)
            {
                landBinding.IsFarmerLandUI = "";
            }
            else
            {
                landBinding.IsFarmerLandUI = (bool)landBinding.Tag.IsFarmerLand ? "是" : "否";
            }
        }

        /// <summary>
        /// 承包方名称界面显示的样式
        /// </summary>
        /// <param name="name">承包方名称</param>
        /// <param name="count">选中承包方下的所有地块集合数量</param>
        public static string CreateItemName(VirtualPerson vp, int count)
        {
            string familyName = "";
            if (vp == null)
            {
                return familyName;
            }
            else
            {
                string strDesc = vp.Status == eVirtualPersonStatus.Lock ? "(已锁定)" : "";
                familyName = vp.Name + string.Format("(共有地块:{0})" + strDesc, count);
                return familyName;
            }
        }

        /// <summary>
        /// 户号 界面显示样式
        /// </summary>
        public static string CreateItemNumber(VirtualPerson vp)
        {
            string familyNumber = "";
            if (vp == null)
            {
                return familyNumber;
            }
            else
            {
                string number = vp.FamilyNumber.PadLeft(4, '0');
                familyNumber = string.Format("(户号: {0})", number);
                return familyNumber;
            }
        }

        /// <summary>
        /// 单户实测面积总和
        /// </summary>
        public static string SumActualArea(ContractLandPersonItem clpItem)
        {
            double sum = 0;
            if (clpItem == null) return "";
            foreach (var land in clpItem.Children)
            {
                sum += land.Tag.ActualArea;
            }
            return sum == 0 ? "" : sum.ToString();
        }

        /// <summary>
        /// 单户确权总面积和
        /// </summary>
        public static string SumAwareArea(ContractLandPersonItem clpItem)
        {
            if (clpItem == null) return "";
            double sum = 0;
            if (clpItem == null)
            {
                return "";
            }
            foreach (var land in clpItem.Children)
            {
                sum += land.Tag.AwareArea;
            }
            return sum == 0 ? "" : sum.ToString();
        }

        /// <summary>
        /// 单户二轮合同总面积和
        /// </summary>
        public static string SumTableArea(ContractLandPersonItem clpItem)
        {
            double? sum = 0;
            if (clpItem == null) return "";
            foreach (var land in clpItem.Children)
            {
                sum += land.Tag.TableArea == null ? 0 : land.Tag.TableArea;
            }
            return sum == 0 ? "" : sum.ToString();
        }

        #endregion
    }
}
