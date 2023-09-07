using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public interface IExpandable
    {
        void ExpandAll();
        void Expand();
        void Collapse();
        void CollapseAll();
    }
}
