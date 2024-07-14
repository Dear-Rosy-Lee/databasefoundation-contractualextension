using MediaDevices;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;

namespace YuLinTu.Component.PadDataHandle
{

    /// <summary>
    /// 设备帮助类
    /// </summary>
    public class DeviceFolder
    {
        public string SoftFrendlyName { get; set; }
        public string FolderName { get; set; }
        public string FolderPath { get; set; }

        public MediaDirectoryInfo DirectoryInfo { get; set; }
    }
}
