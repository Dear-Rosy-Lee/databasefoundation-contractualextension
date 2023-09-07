using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [Serializable()]
    public class YltEntityName : YltEntity
    {
        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #endregion

        #region Fields

        private string name = string.Empty;

        #endregion
    }
}
