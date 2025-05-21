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
    /// 合同数据转换类
    /// </summary>
    public class ConcordItemHelper
    {
        #region Methods

        /// <summary>
        /// 转换承包方为绑定实体
        /// </summary>
        public static ConcordItem ConvertToItem(VirtualPerson vp, List<ContractConcord> blist, List<Dictionary> contractWayList, List<Dictionary> landPurposeList)
        {
            if (vp == null)
            {
                return null;
            }
            ConcordItem item = new ConcordItem();
            item.ID = vp.ID;
            item.Name = CreateItemName(vp);
            item.Tag = vp;
            item.Img = vp.Status == eVirtualPersonStatus.Lock ? 0 : 1;
            if (blist != null)
            {
                blist.ForEach(b =>
                {
                    BindConcord bc = new BindConcord(b, contractWayList, landPurposeList);
                    bc.Name = vp.FamilyNumber.PadLeft(4, '0');
                    item.Children.Add(bc);
                });
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
