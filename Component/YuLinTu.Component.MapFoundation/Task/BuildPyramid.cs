using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace YuLinTu.Component.MapFoundation
{
    public class BuildPyramid
    {
        public delegate void OnDescribleFileCreated(string str);
        public delegate void OnFileCreated(int zoom, int row, int col, string fileName);

        [DllImport("tGISCLib.dll", EntryPoint = "CalcSplitCount", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CalcSplitCount(string srcFileName);

        [DllImport("tGISCLib.dll", EntryPoint = "Convert", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Convert(string srcFileName, OnDescribleFileCreated callback1, OnFileCreated callback2);
    }
}
