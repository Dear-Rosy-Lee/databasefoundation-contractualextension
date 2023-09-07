using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public delegate void DirectoryOperationEventHandler(object sender, DirectoryOperationEventArgs e);
    public delegate void DirectoryOperationExceptionEventHandler(object sender, DirectoryOperationExceptionEventArgs e);

    public class DirectoryOperationEventArgs : EventArgs
    {
        #region Properties

        public string Source { get; set; }
        public string Destination { get; set; }
        public eDirectoryContentType ContentType { get; set; }

        #endregion

        #region Ctor

        public DirectoryOperationEventArgs(string source, string dest, eDirectoryContentType typeContent)
        {
            Source = source;
            Destination = dest;
            ContentType = typeContent;
        }

        #endregion
    }

    public class DirectoryOperationExceptionEventArgs : DirectoryOperationEventArgs
    {
        #region Properties

        public Exception Exception { get; set; }
        public bool IsCancel { get; set; }

        #endregion

        #region Ctor

        public DirectoryOperationExceptionEventArgs(string source, string dest,
            eDirectoryContentType typeContent, Exception ex)
            : base(source, dest, typeContent)
        {
            Exception = ex;
            IsCancel = false;
        }

        #endregion
    }
}
