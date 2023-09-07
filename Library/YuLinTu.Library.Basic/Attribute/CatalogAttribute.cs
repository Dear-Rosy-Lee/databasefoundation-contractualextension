using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace YuLinTu.Library.Basic
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class CatalogAttribute : Attribute
    {
        #region Properties

        public string Catalog { get; set; }

        #endregion

        #region Ctor

        public CatalogAttribute(string catalog)
        {
            Catalog = catalog;
        }

        #endregion
    }
}
