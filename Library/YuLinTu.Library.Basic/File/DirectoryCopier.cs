using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLinTu.Library.Basic
{
    public class DirectoryCopier : DirectoryOperator
    {
        #region Methods - Override

        protected override void OnApplyContent(string source, string dest, eDirectoryContentType typeContent)
        {
            if (typeContent == eDirectoryContentType.Directory)
                Directory.CreateDirectory(dest);

            else if (typeContent == eDirectoryContentType.File)
                File.Copy(source, dest);
        }

        #endregion
    }
}
