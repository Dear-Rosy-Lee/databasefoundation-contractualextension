using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YuLinTu.Component.StockRightBase.Entity;
using YuLinTu.Component.StockRightBase.Enum;
using YuLinTu.Library.Entity;

namespace YuLinTu.Component.StockRightBase.Model
{
    public class StockDistributionModel : NotifyInfoCDObject
    {
        private double _stockTotality;
        private double _collectiveRemainStock;
        private double _unitStock;
        private double _stockAreaAll;
        private bool _isEnable;
        private bool _isPersonWay=true;
        private bool _isHouseWay;
        private bool _isUserDefine;
        private int _houseNum;//总户数
        private int _personNum;//总人数
        private StockRightBussinessObject _bussinessObject;
        private ObservableCollection<string> _selectItem = new ObservableCollection<string>();
        private string _currentSelectItem="按人";
        private string _stockNum;
        private StockDistributionWays _stockDistributionWays;
        private double _collectRemainStock;
        private double _unitStockNum;

        public StockRightBussinessObject BussinessObject
        {
            get { return _bussinessObject; }
            set
            {
                _bussinessObject = value;
                _houseNum = BussinessObject.VirtualPersons.Count;
                _personNum = BussinessObject.VirtualPersons.Sum(o => o.SharePersonList.Count);
            }
        }

        /// <summary>
        /// 按人
        /// </summary>
        public bool IsPersonWay
        {
            get { return _isPersonWay; }
            set
            {
                _isPersonWay = value;
                if (IsPersonWay)
                {
                    IsEnable = false;
                    AreaAll = BussinessObject.ContractLands.Sum(o => o.ActualArea);
                    CollectiveRemainStock = BussinessObject.ContractLands.Sum(o => o.ObligateArea);
                    StockTotality = BussinessObject.VirtualPersons.Sum(o => o.SharePersonList.Count);
                    StockDistributionWays = StockDistributionWays.按人;
                }
                NotifyPropertyChanged(nameof(IsPersonWay));
            }
        }

        /// <summary>
        /// 按户
        /// </summary>
        public bool IsHouseWay
        {
            get { return _isHouseWay; }
            set
            {
                _isHouseWay = value;
                if (IsHouseWay)
                {
                    IsEnable = false;
                    AreaAll = BussinessObject.ContractLands.Sum(o => o.ActualArea);
                    CollectiveRemainStock = BussinessObject.ContractLands.Sum(o => o.ObligateArea);
                    StockTotality = BussinessObject.VirtualPersons.Count;
                    StockDistributionWays = StockDistributionWays.按户;
                }
                NotifyPropertyChanged(nameof(IsHouseWay));
            }
        }

        /// <summary>
        /// 自定义
        /// </summary>
        public bool IsUserWay
        {
            get { return _isUserDefine; }
            set
            {
                _isUserDefine = value;
                if (IsUserWay)
                {
                    IsEnable = true;
                    if (CurrentSelectItem == "按人")
                        StockDistributionWays = StockDistributionWays.按人;
                    else
                        StockDistributionWays = StockDistributionWays.按户;
                }
                NotifyPropertyChanged(nameof(IsUserWay));
            }
        }


        public StockDistributionWays StockDistributionWays
        {
            get { return _stockDistributionWays; }
            set
            {
                _stockDistributionWays = value;
                if (StockDistributionWays == StockDistributionWays.按人)
                {
                    UnitStockArea = (StockTotality / (double)_personNum) / StockTotality * (AreaAll-CollectiveRemainStock);
                    StockNum =_personNum+"人";
                    CollectRemainStockNum = CollectiveRemainStock / AreaAll * StockTotality;
                    UnitStockNum = (StockTotality - CollectRemainStockNum) / _personNum;
                }
                if (StockDistributionWays == StockDistributionWays.按户)
                {
                    UnitStockArea = (StockTotality / (double)_houseNum) / StockTotality * (AreaAll- CollectiveRemainStock);
                    StockNum = _houseNum+"户";
                    CollectRemainStockNum = CollectiveRemainStock / AreaAll * StockTotality;
                    UnitStockNum = (StockTotality - CollectRemainStockNum) / _houseNum;
                }
            }
        }


        /// <summary>
        /// 设置自定义时的控件可用性
        /// </summary>
        public bool IsEnable
        {
            get { return _isEnable; }
            set { _isEnable = value; NotifyPropertyChanged(nameof(IsEnable)); }
        }


        /// <summary>
        /// 总面积
        /// </summary>
        public double AreaAll
        {
            get { return _stockAreaAll; }
            set
            {
                _stockAreaAll = value;
                if (StockDistributionWays == StockDistributionWays.按人)
                {
                    UnitStockArea = (AreaAll - CollectiveRemainStock) / StockTotality;
                    UnitStockNum = (StockTotality - CollectRemainStockNum) / _personNum;
                }
                else
                {
                    UnitStockArea = (AreaAll - CollectiveRemainStock) / StockTotality;
                    UnitStockNum = (StockTotality - CollectRemainStockNum) / _houseNum;
                }
                _stockAreaAll = Math.Round(AreaAll, 2);
                NotifyPropertyChanged(nameof(AreaAll));

            }
        }

        /// <summary>
        /// 总股数
        /// </summary>
        public double StockTotality
        {
            get { return _stockTotality; }
            set
            {
                _stockTotality = value;
                if (StockDistributionWays == StockDistributionWays.按人)
                {
                    UnitStockArea = (AreaAll-CollectiveRemainStock) / StockTotality;
                    UnitStockNum = (StockTotality-(double)CollectRemainStockNum) / _personNum;
                }
                else
                {
                    UnitStockArea = (AreaAll -(double)CollectiveRemainStock)/ StockTotality;
                    UnitStockNum = (StockTotality-CollectRemainStockNum) / _houseNum;
                }
                _stockTotality = Math.Round(StockTotality, 2);
                NotifyPropertyChanged(nameof(StockTotality));
            }
        }


        /// <summary>
        /// 集体预留面积
        /// </summary>
        public double CollectiveRemainStock
        {
            get { return _collectiveRemainStock; }
            set
            {
                _collectiveRemainStock = value;
                _collectiveRemainStock = Math.Round(CollectiveRemainStock, 2);
                NotifyPropertyChanged(nameof(CollectiveRemainStock));
            }
        }

        /// <summary>
        /// 单股面积
        /// </summary>
        public double UnitStockArea
        {
            get { return _unitStock; }
            set
            {
                _unitStock = value;
                _unitStock = Math.Round(UnitStockArea,2);
                NotifyPropertyChanged(nameof(UnitStockArea));
            }
        }



        /// <summary>
        /// 当前选择项
        /// </summary>
        public string CurrentSelectItem
        {
            get { return _currentSelectItem; }
            set
            {
                _currentSelectItem = value;
                if (CurrentSelectItem == "按人")
                {
                  
                    UnitStockArea = (StockTotality / (double)_personNum) / StockTotality * AreaAll;
                    StockDistributionWays = StockDistributionWays.按人;
                }
                if (CurrentSelectItem == "按户")
                {
                    UnitStockArea = (StockTotality / (double)_houseNum) / StockTotality * AreaAll;
                    StockDistributionWays = StockDistributionWays.按户;
                }
                NotifyPropertyChanged(nameof(CurrentSelectItem));
            }
        }


        /// <summary>
        /// 当前选择是多少人或者多少户
        /// </summary>
        public string StockNum
        {
            get
            {
                return _stockNum;
            }
            set
            {
                _stockNum = value;
                NotifyPropertyChanged(nameof(StockNum));
            }
        }


        /// <summary>
        /// 集体预留股数
        /// </summary>
        public double CollectRemainStockNum
        {
            get
            {
                return _collectRemainStock;
            }
            set
            {
                _collectRemainStock = value;
                CollectiveRemainStock = (CollectRemainStockNum / StockTotality)*AreaAll;
                if (StockDistributionWays == StockDistributionWays.按人)
                {
                    UnitStockArea = (AreaAll - CollectiveRemainStock) / StockTotality;
                    UnitStockNum = (StockTotality - CollectRemainStockNum) / _personNum;
                }
                else
                {
                    UnitStockArea = (AreaAll - CollectiveRemainStock) / StockTotality;
                    UnitStockNum = (StockTotality - CollectRemainStockNum) / _houseNum;
                }
                _collectRemainStock = Math.Round(CollectRemainStockNum, 2);
                NotifyPropertyChanged(nameof(CollectRemainStockNum));
            }
        }

        /// <summary>
        /// 单位股数
        /// </summary>
        public double UnitStockNum
        {
            get { return _unitStockNum; }
            set
            {
                _unitStockNum = value;
                _unitStockNum = Math.Round(UnitStockNum,2);
                NotifyPropertyChanged(nameof(UnitStockNum));
            }
        }


        public ObservableCollection<string> SelectItems
        {
            get
            {
                _selectItem.Add("按人");
                _selectItem.Add("按户");
                return _selectItem;
            }
            set
            {
                _selectItem = value;
                NotifyPropertyChanged(nameof(SelectItems));
            }
        }


    }
}
