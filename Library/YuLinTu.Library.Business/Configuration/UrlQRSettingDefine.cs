using YuLinTu.Windows;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 网址二维码设置
    /// </summary>
    public class UrlQRSettingDefine : NotifyCDObject
    {
        #region Fields

        private string _Url;
        private string _ImageSize;
        private string _ImagePixel;
        private string _Version;
        private bool _DrawQuietZones;
        private string _SizeInWord;

        #endregion

        #region Properties

        /// <summary>
        /// URL
        /// </summary>
        public string Url
        {
            get { return _Url; }
            set { _Url = value; NotifyPropertyChanged(() => Url); }
        }

        /// <summary>
        /// 二维码图片尺寸
        /// </summary>
        public string ImageSize
        {
            get { return _ImageSize; }
            set { _ImageSize = value; NotifyPropertyChanged(() => ImageSize); }
        }



        /// <summary>
        /// 二维码像素清晰度
        /// </summary>
        public string ImagePixel
        {
            get { return _ImagePixel; }
            set { _ImagePixel = value; NotifyPropertyChanged(() => ImagePixel); }
        }

        /// <summary>
        /// 二维码像素密集度
        /// </summary>
        public string Version
        {
            get { return _Version; }
            set { _Version = value; NotifyPropertyChanged(() => Version); }
        }

        /// <summary>
        /// 是否添加边框
        /// </summary>
        public bool DrawQuietZones
        {
            get { return _DrawQuietZones; }
            set { _DrawQuietZones = value; NotifyPropertyChanged(() => DrawQuietZones); }
        }

        /// <summary>
        /// 在Word中的尺寸大小
        /// </summary>
        public string SizeInWord
        {
            get { return _SizeInWord; }
            set { _SizeInWord = value; NotifyPropertyChanged(() => SizeInWord); }
        }
        


        #endregion

        #region Ctor

        public UrlQRSettingDefine()
        {
            Url = "http://nyncw.cq.gov.cn/cbjyq/qzcx";
            ImageSize = "200";
            ImagePixel = "10";
            Version = "4";
            DrawQuietZones = false;
            SizeInWord = "70";
        }

        #endregion

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        public static UrlQRSettingDefine GetIntence()
        {
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();  //系统配置
            var profile = systemCenter.GetProfile<UrlQRSettingDefine>();
            var section = profile.GetSection<UrlQRSettingDefine>();
            return section.Settings;
        }
    }
}
