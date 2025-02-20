using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuLinTu.Library.Controls
{
    /// <summary>
    /// 检查项
    /// </summary>
    [Serializable]
    public class TermParamCondition : CDObject, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// 序号
        /// </summary>
        private double index;

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
        private string aliseName;

        /// <summary>
        /// 是否可用
        /// </summary>
        private bool isEnable;

        private string img;

        #endregion

        #region Properties

        /// <summary>
        /// 序号
        /// </summary>
        public double Index
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
        public string AliseName
        {
            get { return aliseName; }
            set { aliseName = value; RaisePropertyChanged("AliseName"); }
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
        /// 是否可用
        /// </summary>
        public bool IsEnable
        {
            get { return isEnable; }
            set { isEnable = value; RaisePropertyChanged("IsEnable"); }
        }

        /// <summary>
        /// 图片
        /// </summary>
        public string Img
        {
            get { return img; }
            set { img = value; RaisePropertyChanged("Img"); }
        }

        public bool ExcuteInOtherProcess { get; set; }

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

        public TermParamCondition()
        {
            this.index = 100;
            this.IsEnable = true;
            this.ErrorCount = -1;
            this.ExcuteInOtherProcess = false;
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

        public override object Clone()
        {
            var tpc = new TermParamCondition();
            tpc.CopyPropertiesFrom(this);
            return tpc;
        }
        #endregion
    }
}
