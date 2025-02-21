using System;
using System.ComponentModel;


namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 检查项
    /// </summary>
    [Serializable]
    public class TermCondition : CDObject, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// 序号
        /// </summary>
        private int index;

        /// <summary>
        /// 名称
        /// </summary>
        private string name;

        /// <summary>
        /// 信息
        /// </summary>
        private string comment;

        /// <summary>
        /// 值
        /// </summary>
        private bool isCheck;

        /// <summary>
        /// 描述
        /// </summary>
        private string description;

        /// <summary>
        /// 百分比
        /// </summary>
        private int percentValue;

        /// <summary>
        /// 是否可用
        /// </summary>
        private bool isEnable;

        #endregion

        #region Properties

        /// <summary>
        /// 序号
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; RaisePropertyChanged("Index"); }
        }

        public string category;
        /// <summary>
        /// 分类
        /// </summary>
        public string Category
        {
            get { return category; }
            set { category = value; RaisePropertyChanged("Category"); }
        }

        public string group;

        /// <summary>
        /// 分组
        /// </summary>
        public string Group
        {
            get { return group; }
            set { group = value; RaisePropertyChanged("Group"); }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged("Name"); }
        }

        /// <summary>
        /// 值
        /// </summary>
        public bool Value
        {
            get { return isCheck; }
            set { isCheck = value; RaisePropertyChanged("Value"); }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; RaisePropertyChanged("Description"); }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Comment
        {
            get { return comment; }
            set { comment = value; RaisePropertyChanged("Comment"); }
        }

        /// <summary>
        /// 参数值
        /// </summary>
        public int PercentValue
        {
            get { return percentValue; }
            set { percentValue = value; RaisePropertyChanged("PercentValue"); }
        }

        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsEnable
        {
            get { return isEnable; }
            set { isEnable = value; RaisePropertyChanged("IsEnable"); }
        }

        /// <summary>
        /// 错误数量
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// 警告数量
        /// </summary>
        public int WarnCount { get; set; }

        #endregion

        #region Ctor

        public TermCondition()
        {
            this.index = 100;
            this.percentValue = 1;
            this.IsEnable = true;
            this.ErrorCount = -1;
        }

        #endregion

        #region Method

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
