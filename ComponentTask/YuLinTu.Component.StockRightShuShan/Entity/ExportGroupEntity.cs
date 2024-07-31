using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightShuShan
{
    /// <summary>
    /// 一户人的数据
    /// </summary>
    public class ExportGroupEntity
    {
        /// <summary>
        /// 承包方
        /// </summary>
        public VirtualPerson Contractor { get; set; }

        /// <summary>
        /// 发包方
        /// </summary>
        public CollectivityTissue Tissue { get; set; }


        /// <summary>
        /// 地块集合
        /// </summary>
        public List<ContractLand> LandCollection { get; set; }


        public string ShareAreaSum
        {
            get
            {
                if (LandCollection == null || LandCollection.Count == 0)
                    return "";
                return LandCollection.FirstOrDefault().ShareArea;
            }
        }

        public string ConcordAreaSum
        {
            get
            {
                if (LandCollection == null || LandCollection.Count == 0)
                    return "";
                return LandCollection.Sum(o => Convert.ToDouble(o.ConcordArea)).ToString();
            }
        }

        //public string QulitityHouseAreaSum
        //{
        //    get
        //    {
        //        if (LandCollection == null || LandCollection.Count == 0)
        //            return "";
        //        return LandCollection.Sum(o => Convert.ToDouble(o.QuantificatAreaByLand)).ToString();
        //    }
        //}


        public string StockQulititySum
        {
            get
            {
                if (LandCollection == null || LandCollection.Count == 0)
                    return "";
                return LandCollection.FirstOrDefault().StockQuantity.ToString();
            }
        }

        public int LandCountSum
        {
            get
            {
                if (LandCollection == null || LandCollection.Count == 0)
                    return 0;
                return LandCollection.Count;
            }
        }

        public string StockQulitityAdvSum
        {
            get
            {
                if (LandCollection == null || LandCollection.Count == 0)
                    return "";
                return LandCollection.FirstOrDefault().StockQuantityAdv;
            }
        }


    }
}
