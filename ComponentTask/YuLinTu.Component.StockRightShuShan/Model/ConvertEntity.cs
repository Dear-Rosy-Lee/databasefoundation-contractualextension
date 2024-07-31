using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightShuShan.Model
{
    /// <summary>
    /// 读取Excel后转化的中间实体
    /// </summary>
    public class ConvertEntity
    {
        /// <summary>
        /// 承包方编号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson Contractor { get; set; }
        /// <summary>
        /// 承包方对应的承包地
        /// </summary>
        public List<ContractLand> LandList { get; set; }

        public ConvertEntity()
        {
            Contractor = new VirtualPerson();
            LandList = new List<ContractLand>();
        }
    }
}
