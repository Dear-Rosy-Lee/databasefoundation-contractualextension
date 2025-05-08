/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using YuLinTu.Library.Business;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 装换承包方到界面实体
    /// </summary>
    public static class ConvertContractor
    {
        public static ContractLandPersonItem ConvertItem
            (this VirtualPerson tableVp, List<ContractLand> accountLandList,
            List<Dictionary> listDKLB, List<Dictionary> listDLDJ,
            bool isStockLand, List<BelongRelation> ralationList,
            bool isAddChildren = true)
        {
            ContractLandPersonItem item = new ContractLandPersonItem() { ID = tableVp.ID };
            item.Tag = tableVp;
            // 先获取非股地，再获取股地
            List<ContractLand> list = accountLandList.FindAll(t => t.OwnerId == tableVp.ID && !t.IsStockLand).OrderBy(s => s.LandNumber).ToList();
            if (isStockLand && ralationList.Count > 0)
            {
                // 股地里，“权属关系”表里必有对应信息
                var ralations = ralationList.FindAll(o => o.VirtualPersonID == tableVp.ID);
                var geoLandOfFamily = new List<ContractLand>();
                ralations.ForEach(r =>
                {
                    var land = accountLandList.Find(g => g.ID == r.LandID);
                    if (land != null)
                    {
                        land = land.Clone() as ContractLand;
                        if (!list.Any(c => c.ID.Equals(land.ID)))
                        {
                            land.AwareArea = r.QuanficationArea;
                            list.Add(land);
                        }
                    }
                });
            }
            foreach (var land in list)
            {
                ContractLandBinding cb = new ContractLandBinding(land);
                ConvertCodeToName(cb, listDKLB, listDLDJ);
                if (isAddChildren)
                    item.Children.Add(cb);
            }
            if (list.Count > 0)
                item.Visibility = Visibility.Visible;
            item.Name = CreateItemName(tableVp, list.Count) + CreateItemNumber(tableVp);
            item.ActualAreaUI = list.Sum(o => o.ActualArea).AreaFormat(2);
            item.AwareAreaUI = list.Sum(o => o.AwareArea).AreaFormat(2);
            item.TableAreaUI = list.Sum(o => o.TableArea).AreaFormat(2);
            item.ContractDelayAreaUI = list.Sum(o => o.ContractDelayArea).AreaFormat(2);
            item.Img = item.Tag.Status == eVirtualPersonStatus.Lock ? 3 : 0;
            return item;
        }


        public static ContractLandPersonItem ConvertItem
            (this VirtualPerson tableVp, List<ContractLand> accountLandList,
            Dictionary<string, Dictionary> listDKLB, Dictionary<string, Dictionary> listDLDJ,
            bool isStockLand, List<BelongRelation> ralationList,
            bool isAddChildren = true, int smallNum = 2)
        {
            ContractLandPersonItem item = new ContractLandPersonItem() { ID = tableVp.ID };
            item.Tag = tableVp;
            // 先获取非股地，再获取股地
            List<ContractLand> list = accountLandList.FindAll(t => t.OwnerId == tableVp.ID && !t.IsStockLand).OrderBy(s => s.LandNumber).ToList();
            if (isStockLand && ralationList.Count > 0)
            {
                // 股地里，“权属关系”表里必有对应信息
                var ralations = ralationList.FindAll(o => o.VirtualPersonID == tableVp.ID);
                var geoLandOfFamily = new List<ContractLand>();
                ralations.ForEach(r =>
                {
                    var land = accountLandList.Find(g => g.ID == r.LandID);
                    if (land != null)
                    {
                        land = land.Clone() as ContractLand;
                        if (!list.Any(c => c.ID.Equals(land.ID)))
                        {
                            land.AwareArea = r.QuanficationArea;
                            list.Add(land);
                        }
                    }
                });
            }
            foreach (var land in list)
            {
                ContractLandBinding cb = new ContractLandBinding(land);
                ConvertCodeToName(cb, listDKLB, listDLDJ);
                if (isAddChildren)
                    item.Children.Add(cb);
            }
            if (list.Count > 0)
                item.Visibility = Visibility.Visible;
            item.Name = CreateItemName(tableVp, list.Count) + CreateItemNumber(tableVp);
            item.ActualAreaUI = list.Sum(o => o.ActualArea).AreaFormat(smallNum);
            item.AwareAreaUI = list.Sum(o => o.AwareArea).AreaFormat(smallNum);
            item.TableAreaUI = list.Sum(o => o.TableArea).AreaFormat(smallNum);
            item.ContractDelayAreaUI = list.Sum(o => o.ContractDelayArea).AreaFormat(smallNum);
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
            var category = listDKLB.Find(t => t.Code == landBinding.Tag.LandCategory);
            landBinding.LandCategoryUI = category == null ? "" : category.Name;
            var level = listDLDJ.Find(t => t.Code == landBinding.Tag.LandLevel);
            landBinding.LandLevelUI = level == null ? "" : level.Name;
            if (landBinding.Tag.IsFarmerLand != null)
                landBinding.IsFarmerLandUI = Convert.ToBoolean(landBinding.Tag.IsFarmerLand) ? "是" : "否";
        }

        /// <summary>
        /// 将编码转换为名称
        /// </summary>
        /// <param name="landBinding">承包地块(界面实体)</param>
        /// <param name="dicts">数据字典集合</param>
        public static void ConvertCodeToName(ContractLandBinding landBinding, Dictionary<string, Dictionary> listDKLB, Dictionary<string, Dictionary> listDLDJ)
        {
            Dictionary category = null;
            if (listDKLB.TryGetValue(landBinding.Tag.LandCategory, out category))
                landBinding.LandCategoryUI = category.Name;

            Dictionary level = null;
            if (listDLDJ.TryGetValue(landBinding.Tag.LandLevel, out level))
            {
                landBinding.LandLevelUI = level.Name;
            }
            if (landBinding.Tag.IsFarmerLand != null)
                landBinding.IsFarmerLandUI = Convert.ToBoolean(landBinding.Tag.IsFarmerLand) ? "是" : "否";
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
                familyName = vp.Name + string.Format("(地块数:{0})" + strDesc, count);
                return familyName;
            }
        }

        /// <summary>
        /// 单户实测面积总和
        /// </summary>
        public static string SumActualArea(ContractLandPersonItem clpItem)
        {
            double sum = 0;
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
            double sum = 0;
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
            foreach (var land in clpItem.Children)
            {
                sum += land.Tag.TableArea == null ? 0 : land.Tag.TableArea;
            }
            return sum == 0 ? "" : sum.ToString();
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
                string number = vp.FamilyNumber == null ? "" : vp.FamilyNumber.PadLeft(4, '0');
                familyNumber = string.Format("(户号: {0})", number);
                return familyNumber;
            }
        }
    }
}
