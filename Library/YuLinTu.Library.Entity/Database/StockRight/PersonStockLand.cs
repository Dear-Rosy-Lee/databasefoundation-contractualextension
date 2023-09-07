using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Entity
{
   
    /// <summary>
    /// 建立人和确股地关系数据结构
    /// </summary>
    public class PersonStockLand
    {
        private VirtualPerson _contractor=new VirtualPerson();
        public VirtualPerson Contractor 
        {
            get
            {
                return _contractor;
            }
            set
            {
                _contractor = value;
            }
        }

        private List<ContractLand> _stockLands = new List<ContractLand>();
        public List<ContractLand> StockLands 
        {
            get { return _stockLands; }
            set 
            {
                _stockLands = value;
            }
        }
    }
}
