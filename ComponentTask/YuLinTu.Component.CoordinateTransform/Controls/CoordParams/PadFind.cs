using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Threading;

namespace YuLinTu.Component.CoordinateTransformTask
{
    public class PadFind
    {
        public PadFind()
        {
            EnumerateUSBDevices();
            ScanDisk();
        }

        public void EnumerateUSBDevices()
        {
            try
            {
                // 构造 WMI 查询语句
                //  string query = "SELECT * FROM Win32_PnPEntity WHERE Caption LIKE '%libusb-win32%'";

                // 构造 WMI 查询语句
                string query = "SELECT * FROM Win32_USBControllerDevice";


                // 创建 ManagementObjectSearcher 对象并执行查询
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection collection = searcher.Get();

                // 遍历查询结果并输出设备信息
                foreach (ManagementObject obj in collection)
                {
                    string dependent = obj["Dependent"].ToString();
                    string antecedent = obj["Antecedent"].ToString();

                    // 获取相关设备的详细信息
                    string[] dependentArray = dependent.Split('=');
                    string[] antecedentArray = antecedent.Split('=');

                    string dependentDeviceId = dependentArray[1].Trim('"');
                    string antecedentDeviceId = antecedentArray[1].Trim('"');

                    // 查询相关设备的详细信息
                    ManagementObjectSearcher searcherDependent = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE DeviceID = '" + dependentDeviceId + "'");
                    ManagementObjectSearcher searcherAntecedent = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE DeviceID = '" + antecedentDeviceId + "'");

                    ManagementObjectCollection collectionDependent = searcherDependent.Get();
                    ManagementObjectCollection collectionAntecedent = searcherAntecedent.Get();

                    // 输出设备信息
                    foreach (ManagementObject dependentObj in collectionDependent)
                    {
                        Console.WriteLine("Dependent Device Name: {0}", dependentObj["Caption"]);
                        Console.WriteLine("Dependent Device ID: {0}", dependentObj["DeviceID"]);
                        Console.WriteLine("-----------------------------------");
                    }

                    foreach (ManagementObject antecedentObj in collectionAntecedent)
                    {
                        Console.WriteLine("Antecedent Device Name: {0}", antecedentObj["Caption"]);
                        Console.WriteLine("Antecedent Device ID: {0}", antecedentObj["DeviceID"]);
                        Console.WriteLine("-----------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void ScanDisk()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                // 可移动存储设备，且不是A盘
                if ((drive.DriveType == DriveType.Removable) && false == drive.Name.Substring(0, 1).Equals("A"))
                {
                    Console.WriteLine("找到一个U盘：" + drive.Name);
                }
            }
        }

        public static List<string> GetListDisk()
        {
            List<string> lstDisk = new List<string>();
            ManagementClass mgtCls = new ManagementClass("Win32_DiskDrive");
            var disks = mgtCls.GetInstances();
            foreach (ManagementObject mo in disks)
            {
                //if (mo.Properties["InterfaceType"].Value.ToString() != "SCSI" 
                //  && mo.Properties["InterfaceType"].Value.ToString() != "USB"
                //  )
                //  continue;

                if (mo.Properties["MediaType"].Value == null ||
                  mo.Properties["MediaType"].Value.ToString() != "External hard disk media")
                {
                    continue;
                }

                //foreach (var prop in mo.Properties)
                //{
                //  Console.WriteLine(prop.Name + "\t" + prop.Value);
                //}

                foreach (ManagementObject diskPartition in mo.GetRelated("Win32_DiskPartition"))
                {
                    foreach (ManagementBaseObject disk in diskPartition.GetRelated("Win32_LogicalDisk"))
                    {
                        lstDisk.Add(disk.Properties["Name"].Value.ToString());
                    }
                }


            }
            return lstDisk;
        }
    }
}
