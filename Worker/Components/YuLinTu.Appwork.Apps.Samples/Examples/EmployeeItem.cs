using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Appwork.Apps.Samples
{
    public class EmployeeItem : NotifyCDObject
    {
        #region Properties

        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged("Name"); }
        }
        private string _Name;

        public byte[] Photo
        {
            get { return _Photo; }
            set { _Photo = value; NotifyPropertyChanged(() => Photo); }
        }
        private byte[] _Photo;

        public double Freight
        {
            get { return _Freight; }
            set { _Freight = value; NotifyPropertyChanged(() => Freight); }
        }
        private double _Freight;

        public int OrderID
        {
            get { return _OrderID; }
            set { _OrderID = value; NotifyPropertyChanged(() => OrderID); }
        }
        private int _OrderID;

        public string ShipCity
        {
            get { return _ShipCity; }
            set { _ShipCity = value; NotifyPropertyChanged(() => ShipCity); }
        }
        private string _ShipCity;

        public string ShipCountry
        {
            get { return _ShipCountry; }
            set { _ShipCountry = value; NotifyPropertyChanged(() => ShipCountry); }
        }
        private string _ShipCountry;

        public string ShipAddress
        {
            get { return _ShipAddress; }
            set { _ShipAddress = value; NotifyPropertyChanged(() => ShipAddress); }
        }
        private string _ShipAddress;

        public DateTime OrderDate
        {
            get { return _OrderDate; }
            set { _OrderDate = value; NotifyPropertyChanged(() => OrderDate); }
        }
        private DateTime _OrderDate;

        #endregion

        #region Ctor

        public EmployeeItem()
        {
        }

        #endregion
    }
}
