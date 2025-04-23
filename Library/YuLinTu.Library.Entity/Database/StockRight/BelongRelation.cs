using System;
using YuLinTu.Data;

namespace YuLinTu.Library.Entity
{
    /// <summary>
    /// 农村土地承包地块
    /// </summary>
    [Serializable]
    [DataTable("QSGX")]
    public class BelongRelation : NotifyCDObject
    {
        private Guid id;

        /// <summary>
        ///标识码
        /// </summary>
        [DataColumn("ID", Nullable = false, PrimaryKey = false)]
        public Guid ID
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("ID");
            }
        }

        private Guid landId;
        /// <summary>
        ///地块标识码
        /// </summary>
        [DataColumn("DKID", Nullable = false, PrimaryKey = true)]
        public Guid LandID
        {
            get { return landId; }
            set
            {
                landId = value;
                NotifyPropertyChanged("LandID");
            }
        }

        private Guid virtualPersonID;
        /// <summary>
        ///承包方标识码
        /// </summary>
        [DataColumn("CBFID", Nullable = false, PrimaryKey = true)]
        public Guid VirtualPersonID
        {
            get { return virtualPersonID; }
            set
            {
                virtualPersonID = value;
                NotifyPropertyChanged("VirtualPersonID");
            }
        }

        private string zoneCode;
        /// <summary>
        /// 所在地域
        /// </summary>
        [DataColumn("SZDY")]
        public string ZoneCode
        {
            get { return zoneCode; }
            set
            {
                zoneCode = value;
                NotifyPropertyChanged("ZoneCode");
            }
        }

        private double _quanficationArea;
        /// <summary>
        /// 量化户面积
        /// </summary>
        [DataColumn("LHHMJ")]
        public double QuanficationArea
        {
            get { return _quanficationArea; }
            set
            {
                _quanficationArea = value;
                NotifyPropertyChanged("QuanficationArea");
            }
        }

        private double _tableArea;
        /// <summary>
        /// 台账面积
        /// </summary>
        [DataColumn("TZMJ")]
        public double TableArea
        {
            get { return _tableArea; }
            set
            {
                _tableArea = value;
                NotifyPropertyChanged("TableArea");
            }
        }

        private double _quotiety;
        /// <summary>
        /// 系数
        /// </summary>
        [DataColumn("XS")]
        public double Quotiety
        {
            get { return _quotiety; }
            set
            {
                _quotiety = value;
                NotifyPropertyChanged("Quotiety");
            }
        }

        public BelongRelation()
        {
            id = Guid.NewGuid();
        }
    }
}
