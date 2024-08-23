using MediaDevices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using YuLinTu.Library.Business;

namespace YuLinTu.Component.PadDataHandle
{

    /// <summary>
    /// 设备帮助类
    /// </summary>
    public class DeviceHelper
    {
        private DispatcherTimer _timer;

        public ObservableCollection<MediaDevice> MediaDevices { get; private set; }

        public Action DeviceChage { get; set; }

        public DeviceHelper()
        {
            MediaDevices = new ObservableCollection<MediaDevice>();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }


        public void DisPose()
        {
            _timer.Stop();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            RefreshDeviceList();
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
                    var devices = MediaDevice.GetDevices();
                    if (MediaDevices.Count == devices.Count())
                    {
                        return;
                    }
                    else if (devices.Count() == 0)
                    {
                        MediaDevices.Clear();
                    }
                    else
                    {
                        var relist = new List<MediaDevice>();
                        foreach (var item in MediaDevices)
                        {
                            if (!devices.Any(w => w.FriendlyName == item.FriendlyName))
                            {
                                relist.Add(item);
                            }
                        }
                        foreach (var item in relist)
                        {
                            MediaDevices.Remove(item);
                        }

                        foreach (var item in devices)
                        {
                            if (!MediaDevices.Any(w => w.FriendlyName == item.FriendlyName))
                                MediaDevices.Add(item);
                        }
                        if (DeviceChage != null)
                            DeviceChage();
                    }
                }, null);
            });
        }

        /// <summary>
        /// 获取软件列表
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
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
            if (enumfolders.Count() > 1)
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
                            deviceFolder.SoftFrendlyName = "专业版移动数据管理应用";
                            list.Add(deviceFolder);
                        }
                        if (ep2.Name == "tPadLite")
                        {
                            deviceFolder.SoftFrendlyName = "大众版移动数据管理应用";
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

        /// <summary>
        /// 获取数据包列表
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="device"></param>
        /// <returns></returns>
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
                deviceFolder.FolderDate = part.LastWriteTime == null ? "" : part.LastWriteTime.Value.ToString("yyyy/MM/dd");
                deviceFolder.DataSize = $"{ToolMath.RoundNumericFormat(GetFileSize(part) / 1048576, 2)}MB";
                list.Add(deviceFolder);
            }
            device.Disconnect();
            return list;
        }

        private ulong GetFileSize(MediaDirectoryInfo mediaDirectoryInfo)
        {
            ulong fileSize = 1;
            foreach (var file in mediaDirectoryInfo.EnumerateFiles())
            {
                fileSize += file.Length;
            }
            foreach (var part in mediaDirectoryInfo.EnumerateDirectories())
            {
                fileSize += GetFileSize(part);
            }
            return fileSize;
        }
    }
}
