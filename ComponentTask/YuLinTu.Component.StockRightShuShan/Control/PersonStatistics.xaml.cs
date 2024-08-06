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
    /// PersonStatistics.xaml 的交互逻辑
    /// </summary>
    public partial class PersonStatistics : INotifyPropertyChanged
    {

        private int _personCount;

        private int _houseCount;

        /// <summary>
        /// 总人数
        /// </summary>
        public int PersonCount
        {
            get { return _personCount; }
            set
            {
                _personCount = value;
                OnPropertyChanged(nameof(PersonCount));
            }
        }

        /// <summary>
        /// 总户数
        /// </summary>
        public int HouseCount
        {
            get { return _houseCount; }
            set
            {
                _houseCount = value;
                OnPropertyChanged(nameof(HouseCount));
            }
        }

        public PersonStatistics()
        {
            InitializeComponent();
            bool designTime =
               (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
            DataContext = this;

        }

        public void GetData(int personCount, int houseCount)
        {
            PersonCount = personCount;
            HouseCount = houseCount;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
