using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
    public class AdjustLand : INotifyPropertyChanged
    {
        public Guid Id { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        [Description("地块编码")]
        public string DKBM { get; set; }

        [Description("地块名称")]
        public string DKMC { get; set; }

        [Description("地块类别")]
        public string DKLB { get; set; }

        [Description("合同面积")]
        public double HTMJ { get; set; }

        [Description("实测面积")]
        public double SCMJ { get; set; }

        [Description("承包方名称")]
        public string CBFMC { get; set; }

        public Guid CBFId { get; set; }

        public string YCBFMC { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
