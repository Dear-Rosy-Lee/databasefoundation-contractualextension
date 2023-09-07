using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public interface IEditable
    {
        void Add(object argument);
        void Insert(object argument);
        void Delete(object argument);
        void DeleteAll(object argument);
    }
}
