using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Net.NetworkInformation;

namespace YuLinTu.Library.Basic
{
    public class ToolNetwork
    {
        private const int INTERNET_CONNECTION_MODEM = 1;
        private const int INTERNET_CONNECTION_LAN = 2;
        [System.Runtime.InteropServices.DllImport("winInet.dll")]
        public static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);

        #region Methods - Network

        private static string macAddress;

        public static string GetMacAddress()
        {
            if (!string.IsNullOrEmpty(macAddress))
                return macAddress;

            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"])
                    macAddress = mo["MacAddress"].ToString();
                mo.Dispose();
            }

            return macAddress;
        }

        public static string GetPhysicalAddress()
        {
            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
            if (nis == null || nis.Length == 0)
                return null;

            return nis[0].GetPhysicalAddress().ToString();
        }

        public static System.Collections.ArrayList GetAllPhysicalAddress()
        {
            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
            if (nis == null || nis.Length == 0)
                return null;
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            for (int i = 0; i < nis.Length; i++)
            {
                string key = nis[i].GetPhysicalAddress().ToString();
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }
                if (list.Contains(key))
                {
                    continue;
                }
                list.Add(key);
            }
            return list;
        }

        /// <summary>
        /// 检查网络连接状态0:未连接1：采用调治解调器上网2：采用网卡上网3：其它方式上网
        /// </summary>
        /// <returns></returns>
        public static int CheckNetWorkStatus()
        {
            System.Int32 flag = new int();
            bool status = InternetGetConnectedState(ref flag, 0);
            if (!status)
            {
                return 0;
            }
            else if ((flag & INTERNET_CONNECTION_MODEM) != 0)
            {
                return 1;
            }
            else if ((flag & INTERNET_CONNECTION_LAN) != 0)
            {
                return 2;
            }
            return 3;
        }

        /// <summary>
        /// 判断网络连接是否有效
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool NetWorkStarusValide(string address)
        {
            int status = CheckNetWorkStatus();
            if (status == 0)
            {
                return false;
            }
            bool success = ConnectNetAddress(address, 500);
            return success;
        }

        /// <summary>
        /// ping 具体的网址看能否ping通
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool ConnectNetAddress(string address, int time)
        {
            bool flag = false;
            Ping ping = new Ping();
            try
            {
                PingReply pr = ping.Send(address, time);
                if (pr.Status != IPStatus.Success)
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return flag;
        }

        #endregion
    }
}
