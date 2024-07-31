using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightBase.Entity
{
    [Serializable]
    public class LandEx
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Number { get; set; }


        /// <summary>
        /// 当前地块
        /// </summary>
        public ContractLand ContractLand { get; set; } = new ContractLand();

        public double PersonCount { get; set; }

        /// <summary>
        /// 量化户面积
        /// </summary>
        public double QuanficationArea { get; set; }

    }
}
