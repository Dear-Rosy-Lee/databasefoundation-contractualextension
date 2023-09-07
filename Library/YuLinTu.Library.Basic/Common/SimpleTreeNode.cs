using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    [Serializable]
    public class SimpleTreeNode
    {
        #region Properties

        public string Name { get; set; }
        public string Text { get; set; }
        public object Tag { get; set; }

        public SimpleTreeNodeCollection Nodes { get; set; }

        #endregion

        #region Ctor

        public SimpleTreeNode()
        {
            Nodes = new SimpleTreeNodeCollection();
        }

        #endregion
    }
}
