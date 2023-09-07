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

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 二轮台账绑定实体(二轮承包方、二轮地块)转换类
    /// </summary>
    public class SecondVirtualPersonItemHelper
    {
        #region Method

        /// <summary>
        /// 转换二轮承包方、二轮地块实体为绑定实体
        /// </summary>
        public static SecondVirtualPersonItem ConvertToItem(VirtualPerson tableVp, List<SecondTableLand> listSecondLd)
        {
            if (tableVp == null)
            {
                return null;
            }
            SecondVirtualPersonItem item = tableVp.ConvertTo<SecondVirtualPersonItem>();
            //item.Name = CreateItemName(tableVp,listSecondLd);
            item.Tag = tableVp;
            item.TableArea = tableVp.TotalTableArea;
            item.Tag = tableVp;
            item.Visibility = Visibility.Visible;
            List<SecondTableLand> list = listSecondLd.FindAll(t => t.OwnerId == tableVp.ID);
            if (list != null)
            {
                list.ForEach(c => item.Children.Add(c.ConvertTo<SecondLandBinding>()));
            }
            if (item.Children.Count > 0)
                item.Visibility = Visibility.Visible;
            item.Name = CreateItemName(tableVp, item.Children.Count);
            item.Img = true;
            return item;
        }

        /// <summary>
        /// 创建承包方绑定项名称
        /// </summary>
        /// <param name="name">承包方名称</param>
        /// <param name="count">选中地域下的所有地块集合</param>
        public static string CreateItemName(VirtualPerson virtualPerson, int count)
        {
            return virtualPerson.Name + string.Format("(共有地块:{0})", count);
        }

        /// <summary>
        /// 改变界面显示承包方名称
        /// </summary>
        /// <param name="currentVirtualPerson">当前选择的承包方</param>
        /// <param name="listSecondLd">当前地域下的所有地块集合</param>
        public static SecondVirtualPersonItem ChangeTableName(VirtualPerson currentVirtualPerson, List<SecondTableLand> listSecondLd)
        {
            if (currentVirtualPerson == null)
            {
                return null;
            }
            SecondVirtualPersonItem item = currentVirtualPerson.ConvertTo<SecondVirtualPersonItem>();
            item.Tag = currentVirtualPerson;
            item.TableArea = currentVirtualPerson.TotalTableArea;
            item.Tag = currentVirtualPerson;
            item.Visibility = Visibility.Visible;
            List<SecondTableLand> list = listSecondLd.FindAll(t => t.OwnerId == currentVirtualPerson.ID);
            list.ForEach(c => item.Children.Add(c.ConvertTo<SecondLandBinding>()));
            item.Name = CreateItemName(currentVirtualPerson, item.Children.Count);
            item.Img = true;
            return item;
        }

        #endregion
    }
}
