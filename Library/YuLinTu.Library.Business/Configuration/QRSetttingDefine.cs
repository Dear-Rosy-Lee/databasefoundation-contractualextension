using YuLinTu.Library.Entity;
using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 二维码设置
    /// </summary>
    public class QRSetttingDefine : NotifyCDObject
    {
        #region Fields

        private eQRType _QRType;

        #endregion

        #region Properties

        public eQRType QRType
        {
            get { return _QRType; }
            set { _QRType = value; NotifyPropertyChanged(() => QRType); }
        }
        
        #endregion

        #region Ctor

        public QRSetttingDefine()
        {
            QRType = eQRType.UrlQR;
        }

        #endregion

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static QRSetttingDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<QRSetttingDefine>();
            var section = profile.GetSection<QRSetttingDefine>();
            return section.Settings;
        }
    }
}
