using YuLinTu.Library.Entity;

namespace YuLinTu.Library.Controls
{
    public class ZoneSelectInfo : NotifyCDObject
    {
        #region Fields

        private string name;

        private string fullCode;

        private string fullName;

        #endregion

        #region Properties

        /// <summary>
        /// 区域名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }

        /// <summary>
        /// 区域全名
        /// </summary>
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; NotifyPropertyChanged("FullName"); }
        }

        /// <summary>
        /// 区域全编码
        /// </summary>
        public string FullCode
        {
            get { return fullCode; }
            set { fullCode = value; NotifyPropertyChanged("FullCode"); }
        }

        /// <summary>
        /// 区域等级
        /// </summary>
        public eZoneLevel Level { get; set; }

        /// <summary>
        /// 地域实体
        /// </summary>
        public Zone Entity { get; set; }

        /// <summary>
        /// 是否选择
        /// </summary>
        public bool IsSelect { get; set; }

        /// <summary>
        /// 子区域集合
        /// </summary>
        public System.Collections.ObjectModel.ObservableCollection<ZoneSelectInfo> Children { get; set; }

        #endregion

        #region Ctor

        /// <summary>
        /// 构造函数—用于初始化子文件集合对象
        /// </summary>
        public ZoneSelectInfo()
        {
            Children = new System.Collections.ObjectModel.ObservableCollection<ZoneSelectInfo>();
        }

        #endregion
    }
}
