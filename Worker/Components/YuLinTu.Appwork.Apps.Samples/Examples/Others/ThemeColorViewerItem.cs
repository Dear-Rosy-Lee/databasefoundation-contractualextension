using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples
{

    public class ThemeColorViewerItem : HandleableNotifyCDObject
    {
        #region Properties


        [DisplayLanguage("边框")]
        [PropertyDescriptor(Catalog = "颜色",
            Builder = typeof(PropertyDescriptorBuilderThemeColorEditor),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/PictureBorderColorPickerClassic.png")]
        public ThemeColorItem BorderBrushKey
        {
            get { return _BorderBrushKey; }
            set { _BorderBrushKey = value; NotifyPropertyChanged(() => BorderBrushKey); }
        }
        private ThemeColorItem _BorderBrushKey;

        [DisplayLanguage("背景")]
        [PropertyDescriptor(Catalog = "颜色",
            Builder = typeof(PropertyDescriptorBuilderThemeColorEditor),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/FontFillBackColorPicker.png")]
        public ThemeColorItem BackgroundKey
        {
            get { return _BackgroundKey; }
            set { _BackgroundKey = value; NotifyPropertyChanged(() => BackgroundKey); }
        }
        private ThemeColorItem _BackgroundKey;

        [DisplayLanguage("文本")]
        [PropertyDescriptor(Catalog = "颜色",
            Builder = typeof(PropertyDescriptorBuilderThemeColorEditor),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/GroupBasicText.png")]
        public ThemeColorItem ForegroundKey
        {
            get { return _ForegroundKey; }
            set { _ForegroundKey = value; NotifyPropertyChanged(() => ForegroundKey); }
        }
        private ThemeColorItem _ForegroundKey;

        [DisplayLanguage("边框宽度")]
        [PropertyDescriptor(Catalog = "布局",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/BorderStyle.png")]
        public double BorderWidth
        {
            get { return _BorderWidth; }
            set { _BorderWidth = value; NotifyPropertyChanged(() => BorderWidth); }
        }
        private double _BorderWidth = 1;

        [DisplayLanguage("文本大小")]
        [PropertyDescriptor(Catalog = "布局")]
        public double FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; NotifyPropertyChanged(() => FontSize); }
        }
        private double _FontSize = 13;

        [DisplayLanguage("文本")]
        [PropertyDescriptor(Catalog = "其他",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/ChangeCase.png")]
        public string Text
        {
            get { return _Text; }
            set { _Text = value; NotifyPropertyChanged(() => Text); }
        }
        private string _Text = "Text";



        [Enabled(false)]
        public string PreviewXaml
        {
            get { return _PreviewXaml; }
            set { _PreviewXaml = value; NotifyPropertyChanged(() => PreviewXaml); }
        }
        private string _PreviewXaml;

        #endregion

        #region Fields

        #endregion

        #region Events

        #endregion

        #region Ctor

        public ThemeColorViewerItem()
        {
        }

        #endregion

        #region Methods

        #region Methods - Public

        #endregion

        #region Methods - Override

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(sender, e);

            if (e.PropertyName == nameof(PreviewXaml))
                return;

            RefreshPreview();
        }

        private void RefreshPreview()
        {
            PreviewXaml = string.Format(
                Properties.Resources.ThemeColorElementTemplate,
                BorderWidth,
                BorderBrushKey?.ColorName,
                BackgroundKey?.ColorName,
                ForegroundKey?.ColorName,
                Text);
        }

        #endregion

        #region Methods - Events

        #endregion

        #region Methods - Private

        #endregion

        #endregion
    }
}
