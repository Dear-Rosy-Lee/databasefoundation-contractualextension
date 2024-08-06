using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuLinTu.Component.StockRightShuShan.Annotations;

namespace YuLinTu.Component.StockRightShuShan.Control
{
    /// <summary>
    /// LandStatistics.xaml 的交互逻辑
    /// </summary>
    public partial class LandStatistics : UserControl, INotifyPropertyChanged
    {
        private int _landCount;
        private double _actualAreaTotal;
        private double _stockAreaTotal;
        private string _landName;
        private bool _isBatch;//是否批量
        /// <summary>
        /// 是否批处理
        /// </summary>
        public bool IsBatch
        {
            get
            {
                return _isBatch;
            }

            set
            {
                _isBatch = value;
                OnPropertyChanged("IsBatch");
            }
        }
        /// <summary>
        /// 地块总数
        /// </summary>
        public int LandCount
        {
            get { return _landCount; }
            set
            {
                _landCount = value;
                OnPropertyChanged(nameof(LandCount));
            }
        }



        /// <summary>
        /// 实测总面积
        /// </summary>
        public double ActuralAreaTotal
        {
            get
            {
                return _actualAreaTotal;
            }
            set
            {
                _actualAreaTotal = value;
                OnPropertyChanged(nameof(ActuralAreaTotal));
            }
        }

        /// <summary>
        /// 确股总面积
        /// </summary>
        public double StockAreaTotal
        {
            get
            {
                return _stockAreaTotal;
            }
            set
            {
                _stockAreaTotal = value;
                OnPropertyChanged(nameof(StockAreaTotal));
            }
        }

        /// <summary>
        /// 地域名称
        /// </summary>
        public string LandName
        {
            get { return _landName; }
            set
            {
                if (value == _landName) return;
                _landName = value;
                OnPropertyChanged(nameof(LandName));
            }
        }


        public LandStatistics()
        {
            InitializeComponent();
            DataContext = this;
        }


        public void GetData(int landCount, double actualAreaTotal, double stockAreaTotal, string landName)
        {
            LandCount = landCount;
            ActuralAreaTotal = actualAreaTotal;
            StockAreaTotal = stockAreaTotal;
            LandName = landName;
        }

        /// <summary>
        /// 是否批量
        /// </summary>
        private void caIsbatch_Click(object sender, RoutedEventArgs e)
        {
            IsBatch = (bool)caIsbatch.IsChecked;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
