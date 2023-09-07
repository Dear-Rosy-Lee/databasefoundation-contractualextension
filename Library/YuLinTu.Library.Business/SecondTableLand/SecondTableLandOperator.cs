/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Data;
using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 二轮地块处理类
    /// </summary>
    public class SecondTableLandOperator
    {
        #region Ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        public SecondTableLandOperator()
        {
        }

        #endregion

        #region Method

        /// <summary>
        /// 初始化地块
        /// </summary>
        public static void InitiallizeLand(ContractLand land, List<Dictionary> dicList)
        {
            land.LandCategory = dicList.Find(c => c.GroupCode == "C7" && c.Code == "10").Name;
            land.LandLevel = dicList.Find(c => c.GroupCode == "C8" && c.Code == "900").Name;
            land.PlantType = dicList.Find(c => c.GroupCode == "C20" && c.Code == "3").Name;
            land.LandScopeLevel = dicList.Find(c => c.GroupCode == "C21" && c.Code == "9").Name;
            land.OwnRightType = dicList.Find(c => c.GroupCode == "C6" && c.Code == "30").Name;
            land.Purpose = dicList.Find(c => c.GroupCode == "C9" && c.Code == "1").Name;
            land.ManagementType = dicList.Find(c => c.GroupCode == "C22" && c.Code == "1").Name;
            land.TransferType = dicList.Find(c => c.GroupCode == "C23" && c.Code == "0").Name;
            land.PlatType = dicList.Find(c => c.GroupCode == "C25" && c.Code == "0").Name;
            land.ConstructMode = dicList.Find(c => c.GroupCode == "C10" && c.Code == "110").Name;
            land.Status = VirtualPersonOperator.IsAgricultureBusinessRegister ? dicList.Find(c => c.GroupCode == "C24" && c.Code == "80").Name : dicList.Find(c => c.GroupCode == "C24" && c.Code == "10").Name;
        }

        #endregion

    }
}
