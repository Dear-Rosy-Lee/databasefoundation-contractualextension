/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
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
    /// 界址点实体装换辅助类
    /// </summary>
    public static class ConstructionLandDotHelper
    {
        #region Felds

        #endregion

        #region Methods

        /// <summary>
        /// 底层实体转换为界面实体
        /// </summary>
        public static ConstructionLandDotItem ConvertToItem(this BuildLandBoundaryAddressDot dot, KeyValueList<string, string> dictJBLX, KeyValueList<string, string> dictJZDLX)
        {
            ConstructionLandDotItem item = new ConstructionLandDotItem();
            if (dot == null)
                return item;
            item.IsValidUI = dot.IsValid;
            item.DotNumberUI = dot.DotNumber;
            item.UnityNumberUI = dot.UniteDotNumber;
            string xCoordinate = string.Empty;
            string yCoordinate = string.Empty;
            if (dot.Shape != null)
            {
                var cdt = dot.Shape.Instance.Coordinate;
                if (cdt != null)
                {
                    xCoordinate = Convert.ToDouble(cdt.X).ToString("0.000");
                    yCoordinate = Convert.ToDouble(cdt.Y).ToString("0.000");
                }
            }
            item.XCoordinateUI = xCoordinate;
            item.YCoordinateUI = yCoordinate;
            item.DotMarkType = GetDictName(dictJBLX, dot.LandMarkType);
            item.DotType = GetDictName(dictJZDLX, dot.DotType);
            item.Visibility = Visibility.Visible;
            item.Img = 1;
            item.Entity = dot;
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
