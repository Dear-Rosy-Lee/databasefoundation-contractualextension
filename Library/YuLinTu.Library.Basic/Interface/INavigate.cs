using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public interface INavigatable
    {
        void Previous();
        void Next();

        void Open();
    }
}
