using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.ServiceProcess;
using Microsoft.Win32;

namespace YuLinTu.Library.Basic
{
    /// <summary>
    /// 服务工具
    /// </summary>
    public class ToolServices
    {
        /// <summary>
        /// 启动服务
        /// </summary>
        public static bool StartServices(string serviceName)
        {
            try
            {
                string servicePath = @"SYSTEM\CurrentControlSet\Services\" + serviceName;//服务名路径
                RegistryKey key = Registry.LocalMachine.OpenSubKey(servicePath, false);
                if (key == null)
                {
                    return false;
                }
                ServiceController serviceConstrol = new ServiceController(Environment.MachineName);
                serviceConstrol.ServiceName = serviceName;//服务名称
                if (serviceConstrol.Status != ServiceControllerStatus.Running)//判断服务是否正在运行
                {
                    serviceConstrol.Start();
                }
                return true;
            }catch (SystemException ex)
            {
                Trace.WriteLine(new Log() { Grade = eLogGrade.Error, Description = ex.ToString(), EventID = 20110, Source = typeof(ToolServices).FullName });
            }
            return false;
        }

        /// <summary>
        /// 暂停服务
        /// </summary>
        public static bool PauseServices(string serviceName)
        {
            try
            {
                string servicePath = @"SYSTEM\CurrentControlSet\Services\" + serviceName;//服务名路径
                RegistryKey key = Registry.LocalMachine.OpenSubKey(servicePath, false);
                if (key == null)
                {
                    return false;
                }
                ServiceController serviceConstrol = new ServiceController(Environment.MachineName);
                serviceConstrol.ServiceName = serviceName;//服务名称
                if (serviceConstrol.Status != ServiceControllerStatus.Paused)//判断服务是否已暂停
                {
                    serviceConstrol.Pause();
                }
                return true;
            }catch (SystemException ex)
            {
                Trace.WriteLine(new Log() { Grade = eLogGrade.Error, Description = ex.ToString(), EventID = 20110, Source = typeof(ToolServices).FullName });
            }
            return false;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public static bool StopServices(string serviceName)
        {
            try
            {
                string servicePath = @"SYSTEM\CurrentControlSet\Services\" + serviceName;//服务名路径
                RegistryKey key = Registry.LocalMachine.OpenSubKey(servicePath, false);
                if (key == null)
                {
                    return false;
                }
                ServiceController serviceConstrol = new ServiceController(Environment.MachineName);
                serviceConstrol.ServiceName = serviceName;//服务名称
                if (serviceConstrol.Status != ServiceControllerStatus.Stopped)//判断服务是否已停止
                {
                    serviceConstrol.Stop();
                }
                return true;
            }catch(SystemException ex) {
                Trace.WriteLine(new Log() { Grade = eLogGrade.Error, Description = ex.ToString(), EventID = 20110, Source = typeof(ToolServices).FullName });
            }
            return false;
        }

        /// <summary>
        /// 启动ArcGis服务
        /// </summary>
        /// <param name="connectionString">链接字符串</param>
        public static bool StartArcGisService(string connectionString)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            try
            {
                string[] array = new string[] { "SERVER=", ";DATABASE=", ";INSTANCE=", ";USER=", ";PASSWORD=", ";VERSION=" };
                if (array.Length < 5)
                {
                    return false;
                }
                int startIndex = connectionString.IndexOf(array[2]) + array[2].Length;
                int endIndex = connectionString.IndexOf(array[3]);
                string instance = connectionString.Substring(startIndex, endIndex - startIndex);
                string instanceName = GetInstanceName(instance);
                if (!string.IsNullOrEmpty(instanceName))
                {
                    StartServices(instanceName);
                    Thread.Sleep(2000);
                }
                return true;
            }catch(SystemException ex) {
                Trace.WriteLine(new Log() { Grade = eLogGrade.Error, Description = ex.ToString(), EventID = 20110, Source = typeof(ToolServices).FullName });
            }
            return false;
        }

        /// <summary>
        /// 启动ArcGisLinceseManager服务
        /// </summary>
        /// <param name="connectionString">链接字符串</param>
        public static bool StartLicenseManager()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            try
            {
                string instanceName = "ArcGIS License Manager";
                bool success = StartServices(instanceName);
                if (success)
                {
                    Thread.Sleep(2000);
                }
                return true;
            }
            catch (SystemException ex)
            {
                Trace.WriteLine(new Log() { Grade = eLogGrade.Error, Description = ex.ToString(), EventID = 20110, Source = typeof(ToolServices).FullName });
            }
            return false;
        }

        #region Helper-Methods

        /// <summary>
        /// 获取实例名称
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private static string GetInstanceName(string instance)
        {
            string filePath = Environment.SystemDirectory + @"\drivers\etc\services";
            string instanceName = string.Empty;
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                while (true)
                {
                    string lineData = reader.ReadLine();
                    if (lineData.IndexOf(instance) >= 0)
                    {
                        instanceName = DealWithServiceName(lineData);
                        break;
                    }
                }
            }
            return instanceName;
        }

        /// <summary>
        /// 处理服务名称
        /// </summary>
        /// <param name="servicesName"></param>
        /// <returns></returns>
        private static string DealWithServiceName(string servicesName)
        {
            string[] array = servicesName.Split('\t');
            return array[0];
        }
        
        #endregion

    }
}
