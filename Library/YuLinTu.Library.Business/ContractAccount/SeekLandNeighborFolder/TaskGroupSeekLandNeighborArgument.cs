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
   public class TaskGroupSeekLandNeighborArgument : TaskArgument
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// 数据库
        /// </summary>
        public IDbContext Database { get; set; }

        /// <summary>
        /// 当前地域
        /// </summary>
        public Zone CurrentZone { get; set; }

        /// <summary>
        /// 当前地域下的含空间数据地块集合
        /// </summary>
        public List<ContractLand> CurrentZoneLandList { get; set; }

        /// <summary>
        /// 查找四至
        /// </summary>
        public SeekLandNeighborSetting seekLandNeighborSet { get; set; }

        /// <summary>
        /// 数据字典
        /// </summary>
        public List<Dictionary> DictList { get; set; }

        /// <summary>
        /// 子级地域集合(包括当前地域)
        /// </summary>
        public List<Zone> AllZones { get; set; }

        #endregion


    }
}
