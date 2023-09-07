using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLinTu.Library.Basic
{
    public class DirectoryCutter : DirectoryOperator
    {
        #region Methods - Override

        protected override bool OnGetPriorityChildValue()
        {
            return true;
        }

        protected override void OnApplyContent(string source, string dest, eDirectoryContentType typeContent)
        {
            if (typeContent == eDirectoryContentType.Directory)
            {
                Directory.CreateDirectory(dest);
                Directory.Delete(source, true);
            }

            else if (typeContent == eDirectoryContentType.File)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                File.Move(source, dest);
            }
        }

        #endregion
    }
}
