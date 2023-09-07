using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuLinTu.Data;

namespace YuLinTu.Library.CompareResult
{ 
    public interface IResultFolderContext : IDataSource
    {
        #region Properties

        IProviderResultFolder DataSource { get; }
        ResultDataRead Reader { get; }

        Dictionary<string, int> DKBMToFIDs { get; }

        Dictionary<string, Dictionary<string, int>> ShpFileDKBMToFIDs { get; }

        #endregion

        #region Methods

        #region Methods - Public

        void InitializeDKBMToFIDsDictionary();

        #endregion

        #region Methods - Cache

        #endregion

        #endregion
    }
}
