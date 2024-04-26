using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using YuLinTu.tGIS.Client;

namespace YuLinTu.Component.MapFoundation
{
    public class SplitItem : INotifyPropertyChanged
    {
        #region Fields

        private bool _isChecked;
        private string _surveyNumber;

        #endregion Fields

        #region Properties

        public Graphic Graphic { get; set; }

        public string Text { get; set; }

        public string SurveyNumber
        {
            get { return _surveyNumber; }
            set
            {
                if (_surveyNumber != value) // 只有当值发生变化时才触发事件
                {
                    _surveyNumber = value;
                    OnPropertyChanged(nameof(SurveyNumber)); // 触发事件
                }
            }
        }

        public string OldNumber { get; set; }

        public string NewNumber { get; set; }

        public Visibility Flag { get; set; }

        // IsChecked 属性
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value) // 只有当值发生变化时才触发事件
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked)); // 触发事件
                }
            }
        }

        public string DKMJ { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Properties

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}