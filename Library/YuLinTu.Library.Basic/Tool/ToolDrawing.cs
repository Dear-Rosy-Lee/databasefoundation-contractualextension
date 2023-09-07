using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace YuLinTu.Library.Basic
{
    public class ToolDrawing
    {
        #region Methods

        public static Rectangle ReviseRectangle(Rectangle largeRect, Rectangle smallRect)
        {
            if (smallRect.Top < largeRect.Top)
                smallRect.Y = largeRect.Y;
            if (smallRect.Left < largeRect.Left)
                smallRect.X = largeRect.X;

            if (smallRect.Bottom > largeRect.Bottom)
                smallRect.Y = largeRect.Bottom - smallRect.Height;
            if (smallRect.Right > largeRect.Right)
                smallRect.X = largeRect.Right - smallRect.Width;

            return smallRect;
        }

        #endregion
    }
}
