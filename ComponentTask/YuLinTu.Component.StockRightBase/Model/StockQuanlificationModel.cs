using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Component.StockRightBase.Model
{
    /// <summary>
    /// 股权量化界面绑定实体
    /// </summary>
    public class StockQuanlificationModel:NotifyInfoCDObject
    {
        private double _stockTotality;
        private double _areaTotality;
        private double _singleStockArea;


        /// <summary>
        /// 总股数
        /// </summary>
        public double StockTotality
        {
            get { return _stockTotality; }
            set
            {
                _stockTotality = value;
                NotifyPropertyChanged(nameof(StockTotality));
            }
        }

        /// <summary>
        /// 总面积
        /// </summary>
        public double AreaTotality
        {
            get { return _areaTotality; }
            set
            {
                _areaTotality = value;
                NotifyPropertyChanged(nameof(AreaTotality));
            }
        }

        /// <summary>
        /// 单股面积
        /// </summary>
        public double SingleStockArea
        {
            get { return _singleStockArea; }
            set
            {
                _singleStockArea = value; 
                NotifyPropertyChanged(nameof(SingleStockArea));
            }
        }




    }
}
 