using YuLinTu.Data;

namespace YuLinTu.Library.CompareResult
{
    public class ProviderResultFolder : Provider, IProviderResultFolder
    {
        #region Properties

        public ResultDataRead Reader { get; private set; }

        #endregion

        #region Fields

        #endregion

        #region Ctor

        static ProviderResultFolder()
        {
            //LanguageAttribute.AddLanguage(YuLinTu.Agriculture.DataManagement.ResultBusiness.DataAccess.Properties.Resources.folderchs);
        }

        public ProviderResultFolder(string cntString)
                : base(cntString)
        {
        }

        #endregion

        #region Methods

        #region Methods - Virtual

        #endregion

        #region Methods - Override

        protected override void OnInitializeComponent(string cntString)
        {
            base.OnInitializeComponent(cntString);

            var b = new ResultFolderConnectionStringBuilder(cntString);
            var path = b.Path;

            Reader = new ResultDataRead() { FilePath = path };
            Reader.InitiallPath();
        }

        public override void Dispose()
        {
            Reader = null;
            base.Dispose();
        }

        public override object Clone()
        {
            var provider = new ProviderResultFolder(ConnectionString);
            provider.Name = Name;

            return provider;
        }

        #endregion

        #endregion
    }
}
