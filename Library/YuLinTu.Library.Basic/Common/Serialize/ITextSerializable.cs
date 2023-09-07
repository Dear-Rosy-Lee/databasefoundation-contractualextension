using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YuLinTu.Library.Basic
{
    public interface ITextSerializable
    {
        #region Methods

        void Serialize(TextWriter writer);

        #endregion
    }
}
