using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace YuLinTu.Library.Basic
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class ImageAttribute : Attribute
    {
        #region Properties

        public string ImageName { get; set; }

        #endregion

        #region Ctor

        public ImageAttribute(string imageName)
        {
            ImageName = imageName;
        }

        #endregion
    }
}
