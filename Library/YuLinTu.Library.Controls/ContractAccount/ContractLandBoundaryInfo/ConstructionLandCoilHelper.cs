/*
 * (C) 2016  鱼鳞图公司版权所有,保留所有权利 
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
    /// 界址线实体装换辅助类
    /// </summary>
    public static class ConstructionLandCoilHelper
    {
        #region Felds

        #endregion

        #region Methods

        /// <summary>
        /// 底层实体转换为界面实体
        /// </summary>
        public static ConstructionLandCoilItem ConvertToItem(this BuildLandBoundaryAddressCoil coil, List<BuildLandBoundaryAddressDot> lstDots,
            KeyValueList<string, string> dictJXXZ, KeyValueList<string, string> dictJXLB, KeyValueList<string, string> dictJZXWZ)
        {
            ConstructionLandCoilItem item = new ConstructionLandCoilItem();
            if (coil == null)
                return item;
            item.CoilNumberUI = (coil.OrderID).ToString();           
            item.StartPointUI = coil.StartNumber.IsNullOrEmpty()? "":coil.StartNumber;
            item.EndPointUI = coil.EndNumber.IsNullOrEmpty() ? "":coil.EndNumber;
            item.CoilLengthUI = coil.CoilLength.ToString("0.00");
            item.CoilPropertyUI = GetDictName(dictJXXZ, coil.LineType);
            item.CoilTypeUI = GetDictName(dictJXLB, coil.CoilType);
            item.CoilLocatioonUI = GetDictName(dictJZXWZ, coil.Position);
            item.NeighborObligeeUI = coil.NeighborPerson;
            item.NeighborReferorUI = coil.NeighborFefer;
            item.Visibility = Visibility.Visible;
            item.Img = 1;
            item.Entity = coil;
            return item;
        }

        #endregion

        #region Method - Helper

        /// <summary>
        /// 根据数据字典编码获取名称
        /// </summary>
        private static string GetDictName(KeyValueList<string, string> lstDictContent, string code)
        {
            string name = string.Empty;
            if (lstDictContent == null || lstDictContent.Count == 0 || code.IsNullOrBlank())
                return name;
            var content = lstDictContent.Find(c => c.Key == code);
            if (content == null)
                return name;
            return content.Value;
        }

        #endregion
    }
}
