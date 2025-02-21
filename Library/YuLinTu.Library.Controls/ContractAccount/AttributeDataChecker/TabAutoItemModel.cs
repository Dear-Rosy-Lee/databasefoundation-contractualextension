using System;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 定义TabItem控件
    /// </summary>
    public class TabAutoItemModel : NotifyCDObject
    {
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged("Name"); }
        }
        private string _Name;

        private string count;

        public string Count
        {
            get { return count; }
            set { count = value; NotifyPropertyChanged("Count"); }
        }


        private string img;

        public string Img
        {
            get { return img; }
            set { img = value; NotifyPropertyChanged("Img"); }
        }

        private bool enable;

        public bool Enable
        {
            get { return enable; }
            set { enable = value; NotifyPropertyChanged("Enable"); }
        }

        private string category;

        public string Category
        {
            get { return category; }
            set { category = value; NotifyPropertyChanged("Category"); }
        }

        private string group;

        public string Group
        {
            get { return group; }
            set { group = value; NotifyPropertyChanged("Group"); }
        }

        public bool? IsChecked
        {
            get
            {
                return GetIsChecked();
            }
            set
            {
                SetIsChecked(value);
            }
        }

        private void SetIsChecked(bool? value)
        {
            foreach (var item in Children)
                item.Value = value == null ? false : value.Value;

            NotifyPropertyChanged("IsChecked");
        }

        private bool? GetIsChecked()
        {
            bool? valAnd = null;
            bool? valOr = null;
            foreach (var item in Children)
            {
                valAnd = valAnd != null ? item.Value && valAnd.Value : item.Value;
                valOr = valOr != null ? item.Value || valOr.Value : item.Value;
            }

            if (valOr == null)
                return false;
            else if (!valOr.Value)
                return false;
            else if (valAnd.Value)
                return true;
            else
                return null;
        }


        public System.Collections.ObjectModel.ObservableCollection<TermParamCondition> Children { get; private set; }


        public TabAutoItemModel()
        {
            Enable = true;
            Children = new System.Collections.ObjectModel.ObservableCollection<TermParamCondition>();
            Children.CollectionChanged += Children_CollectionChanged;
        }

        void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Value")
                return;

            NotifyPropertyChanged("IsChecked");
        }

        void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    Children_CollectionChangedAdd(e);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    Children_CollectionChangedRemove(e);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                default:
                    throw new NotSupportedException();
            }
        }

        private void Children_CollectionChangedRemove(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (TermParamCondition item in e.OldItems)
                item.PropertyChanged -= item_PropertyChanged;
        }

        private void Children_CollectionChangedAdd(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (TermParamCondition item in e.NewItems)
                item.PropertyChanged += item_PropertyChanged;
        }
    }
}
