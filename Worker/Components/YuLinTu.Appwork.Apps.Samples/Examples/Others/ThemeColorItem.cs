using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf.Metro;

namespace YuLinTu.Appwork.Apps.Samples
{

    public class ThemeColorItem : NotifyCDObject
    {
        #region Properties

        public string ColorName
        {
            get { return _ColorName; }
            set { _ColorName = value; NotifyPropertyChanged(() => ColorName); }
        }
        private string _ColorName;

        public string ColorKey
        {
            get { return _ColorKey; }
            set { _ColorKey = value; NotifyPropertyChanged(() => ColorKey); }
        }
        private string _ColorKey;

        #endregion

        #region Fields

        #endregion

        #region Events

        private static readonly Dictionary<string, ThemeColorItem> dicThemeColors = null;

        #endregion

        #region Ctor

        static ThemeColorItem()
        {
            dicThemeColors = new Dictionary<string, ThemeColorItem>();

            var fis = typeof(ResourceKeys).GetFields(
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.Public);

            foreach (var fi in fis)
            {
                if (!fi.Name.EndsWith("Key"))
                    continue;
                if (fi.FieldType != typeof(string))
                    continue;

                dicThemeColors[fi.Name] = new ThemeColorItem(fi.Name, (string)fi.GetValue(null));
            }
        }

        public ThemeColorItem(string name, string key)
        {
            _ColorName = name;
            _ColorKey = key;
        }

        #endregion

        #region Methods

        #region Methods - Public

        public static ThemeColorItem GetByName(string name)
        {
            return dicThemeColors[name];
        }

        public static ThemeColorItem[] GetAll()
        {
            return dicThemeColors.Select(c => c.Value).ToArray();
        }

        #endregion

        #region Methods - Override

        #endregion

        #region Methods - Events

        #endregion

        #region Methods - Private

        #endregion

        #endregion
    }
}
