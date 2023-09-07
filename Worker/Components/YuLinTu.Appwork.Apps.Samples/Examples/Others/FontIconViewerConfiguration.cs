using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows.Media;
using YuLinTu;
using YuLinTu.Windows.Wpf.Metro.Components;

namespace YuLinTu.Appwork.Apps.Samples.Examples
{
    public class FontIconViewerConfiguration : HandleableNotifyCDObject
    {
        #region Properties

        [DisplayLanguage("字体")]
        [PropertyDescriptor(Catalog = "图标集",
            Builder = typeof(PropertyDescriptorBuilderFontEditor),
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/FontDialog.png")]
        public string FontFamily
        {
            get { return _FontFamily; }
            set { _FontFamily = value; NotifyPropertyChanged(() => FontFamily); }
        }
        private string _FontFamily;

        [DisplayLanguage("代号")]
        [PropertyDescriptor(Catalog = "字体",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/FunctionWizard.png")]
        public string CharacterHex
        {
            get { return _CharacterHex; }
            set { _CharacterHex = value; NotifyPropertyChanged(() => CharacterHex); }
        }
        private string _CharacterHex = string.Empty;

        [DisplayLanguage("大小")]
        [PropertyDescriptor(Catalog = "预览",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/SizeToFit.png")]
        [Range(10, 50)]
        public double FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; NotifyPropertyChanged(() => FontSize); }
        }
        private double _FontSize = 50;

        [DisplayLanguage("颜色")]
        [PropertyDescriptor(Catalog = "预览",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/GroupBasicText.png")]
        public Brush Foreground
        {
            get { return _Foreground; }
            set { _Foreground = value; NotifyPropertyChanged("Foreground"); }
        }
        private Brush _Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 32, 32, 32));

        [DisplayLanguage("文本")]
        [PropertyDescriptor(Catalog = "预览",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/HeaderInsertGallery.png")]
        public string Text
        {
            get { return _Text; }
            set { _Text = value; NotifyPropertyChanged(() => Text); }
        }
        private string _Text = "Font Icon";



        [DisplayLanguage("序号")]
        [PropertyDescriptor(Catalog = "字体",
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/SlideMasterTextPlaceholderInsert.png")]
        public int CharacterIndex
        {
            get { return _CharacterIndex; }
            set { if (value == _CharacterIndex) return; _CharacterIndex = value; NotifyPropertyChanged(() => CharacterIndex); }
        }
        private int _CharacterIndex;

        [DisplayLanguage("字符")]
        [PropertyDescriptor(Catalog = "字体", Editable = false,
            UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/ChangeCase.png")]
        public string Character
        {
            get { return _Character; }
            set { _Character = value; NotifyPropertyChanged(() => Character); }
        }
        private string _Character;





        [Enabled(false)]
        public string PreviewXaml
        {
            get { return _PreviewXaml; }
            set { _PreviewXaml = value; NotifyPropertyChanged(() => PreviewXaml); }
        }
        private string _PreviewXaml;

        [Enabled(false)]
        public FontCharacterMetadata[] FontCharacters
        {
            get { return _FontCharacters; }
            set { _FontCharacters = value; NotifyPropertyChanged(() => FontCharacters); }
        }
        private FontCharacterMetadata[] _FontCharacters;

        #endregion

        #region Ctor

        public FontIconViewerConfiguration()
        {
            FontFamily = "pack://application:,,,/YuLinTu.Resources;component/Fonts/#FontAwesome";
            CharacterIndex = 61440;
        }

        #endregion

        #region Methods

        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(sender, e);

            if (e.PropertyName == nameof(PreviewXaml))
                return;

            RefreshPreview();
        }

        [PropertyChangedHandler("FontFamily")]
        protected void OnFontFamilyChanged(string name, object value)
        {
            if (FontFamily.IsNullOrBlank())
            {
                FontCharacters = new FontCharacterMetadata[0];
                return;
            }

            try
            {
                var font = YuLinTu.Resources.Fonts.SymbolFonts.
                    FirstOrDefault(c => c.Item1.EndsWith(FontFamily));
                if (font == null)
                    font = new Tuple<string, System.Windows.Media.FontFamily>(
                        FontFamily, new FontFamily(FontFamily));

                List<FontCharacterMetadata> list = new List<FontCharacterMetadata>();

                var typefaces = font.Item2.GetTypefaces();
                foreach (Typeface typeface in typefaces)
                {
                    GlyphTypeface glyph;
                    typeface.TryGetGlyphTypeface(out glyph);
                    if (glyph == null)
                        continue;

                    IDictionary<int, ushort> characterMap = glyph.CharacterToGlyphMap;
                    foreach (var kvp in characterMap)
                    {
                        byte[] bs = new byte[2];
                        bs[0] = (byte)kvp.Key;
                        bs[1] = (byte)(kvp.Key >> 8);
                        var txt = Encoding.Unicode.GetString(bs);
                        list.Add(new FontCharacterMetadata() { Index = kvp.Key, Character = txt });
                    }

                    break;
                }

                FontCharacters = list.ToArray();
            }
            catch
            {
                FontCharacters = new FontCharacterMetadata[0];
            }
        }

        [PropertyChangedHandler("CharacterIndex")]
        protected void OnCharacterIndexChanged(string name, object value)
        {
            Character = ((char)CharacterIndex).ToString();

            byte[] bs = new byte[2];
            bs[0] = (byte)CharacterIndex;
            bs[1] = (byte)(CharacterIndex >> 8);

            StringBuilder sb = new StringBuilder();

            for (int i = 1; i >= 0; i--)
                sb.Append(bs[i].ToString("X2"));

            CharacterHex = sb.ToString();
        }

        [PropertyChangedHandler("CharacterHex")]
        protected void OnCharacterHexChanged(string name, object value)
        {
            CharacterIndex = Convert.ToInt32(CharacterHex, 16);
        }

        private void RefreshPreview()
        {
            PreviewXaml = string.Format(
                Properties.Resources.FontIconTemplate,
                CharacterHex.ToLower(),
                FontSize,
                Foreground,
                FontFamily,
                Text);
        }

        #endregion
    }
}
