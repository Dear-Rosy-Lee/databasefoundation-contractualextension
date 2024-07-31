using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightBase.Entity
{
    /// <summary>
    /// 导入确股数据实体类 一块地对应多个承包方
    /// </summary>
    public class LandContractor
    {

        /// <summary>
        /// 当前地块
        /// </summary>
        public ContractLand ContractLand { get; set; } = new ContractLand();

        /// <summary>
        /// 当前承包方
        /// </summary>
        public VirtualPerson VirtualPerson { get; set; } = new VirtualPerson();
    
        /// <summary>
        /// 量化户面积
        /// </summary>
        public double QuanficationArea { get; set; }


        ///// <summary>
        ///// 当前地块下的人
        ///// </summary>
        //public List<VirtualPerson> VirtualPersons { get; set; } = new List<VirtualPerson>();


    }
}
