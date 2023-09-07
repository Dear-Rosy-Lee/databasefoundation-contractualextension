using System.Data.Common;

namespace YuLinTu.Library.CompareResult
{
    public class ResultFolderConnectionStringBuilder : DbConnectionStringBuilder
    {
        #region Properties

        public string Path
        {
            get { return ContainsKey(stringDirectoryName) ? (string)this[stringDirectoryName] : string.Empty; }
            set { this[stringDirectoryName] = value; }
        }

        #endregion

        #region Fields

        public const string ProviderType = "CompareResult.Result";
        private const string stringDirectoryName = "Path";

        #endregion

        #region Ctor

        public ResultFolderConnectionStringBuilder()
        {
        }

        public ResultFolderConnectionStringBuilder(string cntString)
        {
            ConnectionString = cntString;
        }

        #endregion

        #region Methods

        #region Methods - Static

        #endregion

        #region Methods - Private

        #endregion

        #endregion
    }
}
