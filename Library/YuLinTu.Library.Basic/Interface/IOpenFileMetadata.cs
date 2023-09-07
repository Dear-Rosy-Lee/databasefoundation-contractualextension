using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public interface IOpenSystemEntriesMetadata
    {
        #region Properties

        string Title { get; }
        string Filter { get; }
        string InitialDirectory { get; }
        bool Multiselect { get; }
        bool ShowNewFolderButton { get; }

        #endregion
    }
}
