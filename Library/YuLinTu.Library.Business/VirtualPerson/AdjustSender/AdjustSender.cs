using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Business
{
    public class AdjustSender : INotifyPropertyChanged
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

        [Description("户号")]
        public string HH { get; set; }


        [Description("名称")]
        public string MC { get; set; }


        [Description("证件号码")]
        public string ZJHM { get; set; }


        [Description("成员数量")]
        public string CYSL { get; set; }


        [Description("地块数量")]
        public int DKSL { get; set; }


        [Description("发包方名称")]
        public string FBFMC { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
