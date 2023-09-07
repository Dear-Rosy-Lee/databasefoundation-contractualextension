using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public interface IAddSystemEntries
    {
        #region Methods

        void AddFileName(string fileName);
        void AddFileNames(string[] fileNames);

        void AddFolderName(string folderName);
        void AddFolderNames(string[] folderNames);

        #endregion
    }
}
