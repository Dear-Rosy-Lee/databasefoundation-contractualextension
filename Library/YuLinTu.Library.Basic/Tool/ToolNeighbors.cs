using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YuLinTu.Library.Basic
{
    public class ToolNeighbors
    {
        public static string[] GetLandNeighbors(string text)
        {
            string[] names = text.Split(new char[] { '\r' });

            return names;
        }
    }
}
