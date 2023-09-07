/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
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
    /// 权证数据转换类
    /// </summary>
    public class ContractRegeditBookItemHelper
    {
        #region Methods

        /// <summary>
        /// 转换承包方为绑定实体
        /// </summary>
        public static ContractRegeditBookItem ConvertToItem(VirtualPerson vp, List<ContractRegeditBook> blist, List<ContractConcord> clist, List<ContractLand> cllist)
        {
            if (vp == null || blist == null || blist.Count == 0)
            {
                return null;
            }
            //添加人
            ContractRegeditBookItem item = new ContractRegeditBookItem();
            item.ID = vp.ID;
            item.Name = CreateItemName(vp);
            item.Tag = vp;
            item.Img = vp.Status == eVirtualPersonStatus.Lock ? 0 : 1;

            //添加权证
            //获取权证对应的合同
            List<ContractConcord> ccuse = new List<ContractConcord>();

            foreach (var cb in blist)
            {
                ContractConcord ccitem = clist.Find(t => t.ID == cb.ID);
                ccuse.Add(ccitem);
                List<ContractLand> cbLandlist = cllist.FindAll(t => t.ConcordId == ccitem.ID);

                BindContractRegeditBook bc = new BindContractRegeditBook(cb, ccitem, cbLandlist.Count);
                item.Children.Add(bc);
            }
            return item;
        }

        #endregion

        #region Private

        /// <summary>
        /// 承包方名称界面显示的样式
        /// </summary>
        /// <param name="name">承包方名称</param>
        /// <param name="count">选中承包方下的所有地块集合数量</param>
        public static string CreateItemName(VirtualPerson vp)
        {
            string familyName = "";
            if (vp == null)
            {
                return familyName;
            }
            else
            {
                string strDesc = vp.Status == eVirtualPersonStatus.Lock ? "(已锁定)" : "";
                familyName = vp.Name + strDesc;
                return familyName;
            }
        }

        #endregion
    }
}
