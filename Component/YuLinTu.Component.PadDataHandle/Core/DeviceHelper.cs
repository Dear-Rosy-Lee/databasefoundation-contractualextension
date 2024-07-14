using MediaDevices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Threading;

namespace YuLinTu.Component.PadDataHandle
{

    /// <summary>
    /// 设备帮助类
    /// </summary>
    public class DeviceHelper
    {
        private Timer _timer;

        public ObservableCollection<MediaDevice> MediaDevices { get; private set; }

        public DeviceHelper()
        {
            MediaDevices = new ObservableCollection<MediaDevice>();
            _timer = new Timer((o) =>
            {
                RefreshDeviceList();
            }, new object(), 5000, Timeout.Infinite);
        }


        public void DisPose()
        {
            _timer.Dispose();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public void RefreshDeviceList()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                SynchronizationContext.SetSynchronizationContext(new
                    DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                SynchronizationContext.Current.Post(pl =>
                {
                    MediaDevices.Clear();
                    var devices = MediaDevice.GetDevices();
                    foreach (var item in devices)
                    {
                        MediaDevices.Add(item);
                    }
                }, null);
            });
        }

        public List<DeviceFolder> GetFolderList(MediaDevice device)
        {
            var list = new List<DeviceFolder>();
            if (device == null)
                return list;
            device.Connect();
            string folderPath = @"内部存储设备\SysSetting";
            //创建目录对象
            MediaDirectoryInfo currentFolder = device.GetRootDirectory();
            //拆分路径、分步进入
            string[] pathParts = folderPath.Split('\\');
            var enumfolders = currentFolder.EnumerateDirectories();
            if (enumfolders.Count() == 1)
                enumfolders = enumfolders.ToList()[0].EnumerateDirectories();
            foreach (var part in enumfolders)
            {
                if (part.Name.StartsWith("."))
                    continue;
                if (part.Name == "pad") { }
                if (part.Name == "YuLinTu")
                {
                    var ep = part.EnumerateDirectories();
                    foreach (var ep2 in ep)
                    {
                        DeviceFolder deviceFolder = new DeviceFolder();
                        deviceFolder.FolderName = ep2.Name;
                        deviceFolder.FolderPath = ep2.FullName;
                        deviceFolder.DirectoryInfo = ep2;
                        if (ep2.Name == "tPadCommon")
                        {
                            deviceFolder.SoftFrendlyName = "tPadCommon调查平板";
                            list.Add(deviceFolder);
                        }
                        else if (ep2.Name == "tPad_NYDPC")
                        {
                            deviceFolder.SoftFrendlyName = "农用地现状普查平板";
                            list.Add(deviceFolder);
                        }
                    }
                }
            }
            device.Disconnect();
            return list;
        }

        public List<DeviceFolder> GetDataList(DeviceFolder folder, MediaDevice device)
        {
            var list = new List<DeviceFolder>();
            if (device == null || folder.DirectoryInfo == null)
                return list;
            device.Connect();

            var datalist = folder.DirectoryInfo.EnumerateDirectories();
            var dataDirectory = datalist.FirstOrDefault(t => t.Name == "database");
            if (dataDirectory == null)
                return list;

            foreach (var part in dataDirectory.EnumerateDirectories())
            {
                if (part.Name == "photo" || part.Name == "photo")
                    continue;
                DeviceFolder deviceFolder = new DeviceFolder();
                deviceFolder.FolderName = part.Name;
                deviceFolder.FolderPath = part.FullName;
                deviceFolder.DirectoryInfo = part;
                list.Add(deviceFolder);
            }
            device.Disconnect();
            return list;
            dataDirectory.EnumerateDirectories();
            string folderPath = folder.FolderPath;
            //创建目录对象
            MediaDirectoryInfo currentFolder = device.GetRootDirectory();
            //拆分路径、分步进入
            string[] pathParts = folderPath.Split('\\');
            foreach (string part in pathParts)
            {
                if (!string.IsNullOrEmpty(part))
                {
                    currentFolder = currentFolder.EnumerateDirectories()
                        .FirstOrDefault(dir => dir.Name.Equals(part, StringComparison.OrdinalIgnoreCase));
                }
            }
            return list;
        }
    }
}
