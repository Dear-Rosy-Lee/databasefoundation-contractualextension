/*
 * (C) 2024  鱼鳞图公司版权所有,保留所有权利
 */

using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YuLinTu.Library.Business;
using YuLinTu.Windows;

namespace YuLinTu.Component.PadDataHandle
{
    /// <summary>
    /// 任务包管理界面
    /// </summary>
    public partial class NetdiskManagerPanel : UserControl
    {
        #region Fields
        #region static
        private readonly string CACHEDIR = ".cache";                     // 压缩包存放的缓存目录
        private readonly string PASSWORD = "JeF8U9wHFOMfs2Y8";           // 压缩密钥
        private readonly string FILEEXT = ".yltpkg";                     // 压缩文件新后缀

        private readonly string REMOTEPATH = "package";                  // 数据堡存数据的路径
        private readonly string BASEURL = "https://www.yizhangtu.com";   // 数据堡域名
        private readonly string REMOTEFILEURL = "/api/file/children";    // 获取云盘目录URL
        private readonly string CREATEDIRURL = "/api/file/create/dir";   // 创建云盘目录URL
        private readonly string UPLOADFILEURL = "/api/file/upload";      // 上传文件URL
        private readonly string DOWNLOADFILEURL = "/api/file/download";  // 下载文件URL
        # endregion static
        private ObservableCollection<ExtendedDirectoryInfo> localFileList;
        private ObservableCollection<RemoteFileInfo> remoteFileList;
        private string dataExchangeDirectory;
        private HttpClient httpClient;

        // 测试数据
        #region test
        private string token = "d19bf846-659e-4331-9f55-9a5fd516d32f";
        #endregion test

        #endregion Fields


        #region Properties

        #endregion Properties


        #region Ctor

        /// <summary>
        /// 进行初始化操作
        /// </summary>
        public NetdiskManagerPanel()
        {
            InitializeComponent();
            Init();
        }

        #endregion Ctor

        #region Methods

        #region Init
        /// <summary>
        /// 初始化组件
        /// </summary>
        private void Init()
        {
            InitHttpClient();
            InitLocalView();
            InitRemoteView();
            InitCacheDir();
        }

        /// <summary>
        /// 刷新页面数据
        /// </summary>
        private void Refresh()
        {
            InitLocalView();
            InitRemoteView();
        }

        /// <summary>
        /// 初始化HttpClient对象
        /// </summary>
        private void InitHttpClient()
        {
            var handler = new HttpClientHandler() { UseCookies = false };
            httpClient = new HttpClient(handler);

            httpClient.BaseAddress = new Uri(BASEURL);
            httpClient.Timeout = TimeSpan.FromMinutes(30); // 30min的超时时间
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            string Cookie = $"_s={token}";
            httpClient.DefaultRequestHeaders.Add("cookie", Cookie);
        }

        /// <summary>
        /// 初始化 .cache 缓存目录
        /// </summary>
        private void InitCacheDir()
        {
            if (dataExchangeDirectory is null) return;
            string CachePath = Path.Combine(dataExchangeDirectory, CACHEDIR);
            // 检查目录是否存在
            if (!Directory.Exists(CachePath))
            {
                // 创建目录
                Directory.CreateDirectory(CachePath);
            }
            // 设置目录为隐藏
            DirectoryInfo dirInfo = new DirectoryInfo(CachePath);
            if ((dirInfo.Attributes & FileAttributes.Hidden) == 0)
            {
                dirInfo.Attributes |= FileAttributes.Hidden;
            }
        }

        /// <summary>
        /// 读取本地文件并展示
        /// </summary>
        private void InitLocalView()
        {
            var fileList = GetLocalFileData();
            if (fileList != null)
            {
                List<ExtendedDirectoryInfo> extFileList = fileList
                .Select(dir => new ExtendedDirectoryInfo(dir))
                .ToList();
                localFileList = new ObservableCollection<ExtendedDirectoryInfo>(extFileList);
                // 使用增强的目录类展示
                localview.ItemsSource = localFileList;
            }
            else
            {
                /*
                    TODO 此处做提示设置缓存目录操作
                 */
                // 执行上传或其他相关逻辑
                Console.WriteLine("没找到目录");
            }
        }

        /// <summary>
        /// 获取云端数据
        /// </summary>
        private async void InitRemoteView()
        {
            try
            {
                string getRemoteFileUrl = REMOTEFILEURL + $"?path={REMOTEPATH}";
                HttpResponseMessage response = await httpClient.GetAsync(getRemoteFileUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    ResponseData responseData = serializer.Deserialize<ResponseData>(responseBody);
                    if (responseData.status == 200)
                    {
                        if (responseData != null && responseData.data != null)
                        {
                            List<RemoteFileInfo> fileList = responseData.data;
                            fileList = fileList.Where(remoteFile => CheckRemoteFile(remoteFile.name)).ToList();
                            remoteFileList = new ObservableCollection<RemoteFileInfo>(fileList);
                            netview.ItemsSource = remoteFileList;
                        }
                        else
                        {
                            // 如果云端没有package目录 尝试创建package目录
                            string createRemotePathUrl = CREATEDIRURL + $"?path=/&name={REMOTEPATH}";
                            response = await httpClient.PostAsync(createRemotePathUrl, null);
                            if (response.IsSuccessStatusCode)
                            {
                                remoteFileList = new ObservableCollection<RemoteFileInfo>();
                                netview.ItemsSource = remoteFileList;
                            }
                        }
                    }
                    else
                    {
                        // TODO 处理get远程目录出错
                    }
                }
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine($"任务被取消:{e.Message}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"请求异常: {e.Message}");
            }
        }

        #endregion Init

        /// <summary>
        /// 从环境中获取DataExchangeDirectory路径再遍历其下的所有文件
        /// </summary>
        private List<DirectoryInfo> GetLocalFileData()
        {
            // TODO 从TheApp.Current不能直接拿到DataExchangeDirectory后续可以优化。
            var systemCenter = TheApp.Current.GetSystemSettingsProfileCenter();
            var profile = systemCenter.GetProfile<SystemSetDefine>();
            var section = profile.GetSection<SystemSetDefine>();
            var config = (section.Settings);
            dataExchangeDirectory = config.DataExchangeDirectory;
            // 你的初始化逻辑，比如加载目录和文件列表
            Console.WriteLine($"配置的目录是{dataExchangeDirectory}");
            if (dataExchangeDirectory != null && Directory.Exists(dataExchangeDirectory))
            {
                List<DirectoryInfo> fileInfoList = new List<DirectoryInfo>();
                //string[] files = Directory.GetFiles(dataExchangeDirectory);
                string[] files = Directory.GetDirectories(dataExchangeDirectory);
                foreach (string file in files)
                {
                    if (!CheckLocalFile(file)) continue; // 筛选文件
                    var fileInfo = new DirectoryInfo(file);
                    fileInfoList.Add(fileInfo);
                }
                return fileInfoList;

            }
            return null;
        }

        /// <summary>
        /// 用来筛选本地文件进行展示
        /// </summary>
        private bool CheckLocalFile(string filePath)
        { 
            if (filePath.EndsWith(CACHEDIR)) return false;  // 不展示cache目录
            return true;
        }

        /// <summary>
        /// 筛选云盘文件进行展示
        /// </summary>
        private bool CheckRemoteFile(string filename)
        {
            if (filename.EndsWith(FILEEXT)) return true;    // 只展示 .yltpkg后缀的文件
            return false;
        }

        /// <summary>
        /// 压缩文件到.cache目录
        /// </summary>
        /// <returns>压缩包文件FileInfo对象</returns>
        private FileInfo CompressDirectory(DirectoryInfo source)
        {
            // 创建压缩后的 ZIP 文件路径
            string zipFilePath = Path.Combine(dataExchangeDirectory, CACHEDIR, Path.GetFileNameWithoutExtension(source.Name) + FILEEXT);
            using (ZipFile zip = new ZipFile())
            {
                zip.Password = PASSWORD;                            // 设置加密密码
                zip.Encryption = EncryptionAlgorithm.WinZipAes256;  // 使用 AES 256 加密
                zip.AlternateEncoding = Encoding.GetEncoding("GBK");// 设置编码为 GBK，以支持汉字
                zip.AlternateEncodingUsage = ZipOption.Always;
                zip.AddDirectory(source.FullName, source.Name);
                zip.Save(zipFilePath);
            }

            return new FileInfo(zipFilePath);
        }

        /// <summary>
        /// 解压文件到 dataExchangeDirectory 目录
        /// </summary>
        private void DecompressFile(string zipFilePath)
        {
            if (!File.Exists(zipFilePath))
            {
                throw new FileNotFoundException("压缩文件未找到", zipFilePath);
            }
            if (!Directory.Exists(dataExchangeDirectory))
            {
                Directory.CreateDirectory(dataExchangeDirectory);
            }
            using (ZipFile zip = ZipFile.Read(zipFilePath))
            {
                // 设置编码为 GBK，以支持汉字
                zip.AlternateEncoding = Encoding.GetEncoding("GBK");
                zip.AlternateEncodingUsage = ZipOption.Always;
                if (!string.IsNullOrEmpty(PASSWORD)) { zip.Password = PASSWORD; }
                foreach (ZipEntry entry in zip)
                {
                    entry.Extract(dataExchangeDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        /// <summary>
        /// 上传文件到网盘/{REMOTEPATH}目录下
        /// </summary>
        /// <exception cref="HttpRequestException">上传请求状态码非200</exception>
        /// <exception cref="Exception">其他异常</exception>
        private void UploadFile(FileInfo uploadFile)
        {
            MultipartFormDataContent formData = new MultipartFormDataContent();
            byte[] fileBytes = File.ReadAllBytes(uploadFile.FullName);
            ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            formData.Add(fileContent, "file", uploadFile.Name);
            formData.Add(new StringContent($"/{REMOTEPATH}"), "path");
            try
            {
                HttpResponseMessage response = httpClient.PostAsync(UPLOADFILEURL, formData).Result;
                if (!response.IsSuccessStatusCode)
                {
                    // 抛出异常，包含状态码和原因
                    throw new HttpRequestException($"上传失败，状态码: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"上传文件失败:{ex.Message}");
            }
        }

        /// <summary>
        /// 下载文件到 CACHE 目录下并解压文件到 dataExchangeDirectory 目录
        /// </summary>
        /// <exception cref="HttpRequestException">下载请求状态码非200</exception>
        private void DownloadFile(string filename)
        {
            string filePath = Path.Combine("/package/", filename);
            string pathParma = $"path={filePath}";
            string url = DOWNLOADFILEURL +"?"+ pathParma;
            try
            {
                HttpResponseMessage response = httpClient.GetAsync(url).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    // 读取响应内容并保存到文件
                    string tempFilePath = Path.Combine(dataExchangeDirectory, CACHEDIR, filename);
                    using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        response.Content.CopyToAsync(fileStream).Wait();
                    }
                    DecompressFile(tempFilePath);
                }
                else
                {
                    // 抛出异常，包含状态码和原因
                    throw new HttpRequestException($"下载失败，状态码: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // 将异常传播给调用者
                throw new Exception($"下载文件时出错: {ex.Message}", ex);
            }
        }

        #endregion Methods

        #region Event

        /// <summary>
        /// 上传数据触发事件
        /// </summary>
        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string dirPath = button.Tag as string;
            if (button != null && dirPath != null)
            {
                var task = new Task();

                task.Go += (s, args) =>
                {
                    Dispatcher.Invoke(new Action(() => { button.IsEnabled = false; }));
                    ExtendedDirectoryInfo selectedDirectory = localFileList
                    .Where(item => item.DirectoryInfo.FullName.Equals(dirPath, StringComparison.Ordinal))
                    .FirstOrDefault();
                    FileInfo compression = CompressDirectory(selectedDirectory.DirectoryInfo);
                    UploadFile(compression);
                    compression.Delete();
                };
                task.Completed += (s, args) =>
                {
                    // UI更新，表示任务完成  
                    Dispatcher.Invoke(new Action(() =>
                    {
                        InitRemoteView();
                        button.IsEnabled = true;
                        MessageBox.Show("所有文件处理完成");
                    }));
                };

                task.Terminated += (s, args) =>
                {
                    // 处理异常
                    Dispatcher.Invoke(new Action(() =>
                    {
                        button.IsEnabled = true;
                        MessageBox.Show($"处理失败: {args.Exception.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                };
                // 启动任务
                task.StartAsync();
            }
        }

        /// <summary>
        /// 下载数据包
        /// </summary>
        private void Download_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string name = button.Tag as string;
            if (button != null && name != null)
            {
                var task = new Task();
                task.Go += (s, args) =>
                {
                    Dispatcher.Invoke(() => { button.IsEnabled = false; });
                    DownloadFile(name);
                };
                task.Completed += (s, args) =>  
                {
                    Dispatcher.Invoke(() =>
                    {
                        Dispatcher.Invoke(() => { InitLocalView(); });
                        button.IsEnabled = true;
                        MessageBox.Show($"{name}下载成功");
                    });
                };
                // 任务失败时处理异常
                task.Terminated += (s, args) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        button.IsEnabled = true;
                        MessageBox.Show($"下载失败: {args.Exception.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                };
                // 启动任务
                task.StartAsync();
            }
        }
        
        /// <summary>
        /// 删除本地数据包事件
        /// </summary>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string fullName = button.Tag as string;
            if (button != null && fullName != null)
            {
                // 提示用户确认删除
                var result = MessageBox.Show($"确定要删除目录 '{fullName}' 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        Dispatcher.Invoke(() => { button.IsEnabled = false; });
                        ExtendedDirectoryInfo directoryInfo  = localFileList
                            .Where(file => file.DirectoryInfo.FullName.Equals(fullName, StringComparison.Ordinal))
                            .FirstOrDefault(); 
                        //DirectoryInfo directoryInfo = selectedDirectory.DirectoryInfo;
                        if (directoryInfo.DirectoryInfo.Exists)
                        {
                            directoryInfo.DirectoryInfo.Delete(true);
                            localFileList.Remove(directoryInfo);
                            //localview.Items.Remove(selectedDirectory);
                            //localview.Items.Clear();
                            //localview.ItemsSource = localFileList;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("删除过程中发生错误。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        Dispatcher.Invoke(() => { button.IsEnabled = true; });
                    }
                }

            }
        }

        #endregion Event

    }

    #region otherClass

    /// <summary>
    /// DirectoryInfo的功能扩展，添加获取目录大小的参数
    /// </summary>
    public class ExtendedDirectoryInfo
    {
        public DirectoryInfo DirectoryInfo { get; }
        public long DirectorySize => GetDirectorySize();
        public string Name => DirectoryInfo.Name;
        public string FullPath => DirectoryInfo.FullName;
        public DateTime LastWriteTime => DirectoryInfo.LastWriteTime;
        public ExtendedDirectoryInfo(DirectoryInfo directoryInfo)
        {
            DirectoryInfo = directoryInfo;
        }

        private long GetDirectorySize()
        {
            FileInfo[] files = DirectoryInfo.GetFiles("*", SearchOption.AllDirectories);
            long totalSize = 0;
            foreach (FileInfo file in files)
            {
                totalSize += file.Length;
            }
            return totalSize;
        }
    }

    /// <summary>
    /// 云端文件信息
    /// </summary>
    class RemoteFileInfo
    {
        public string name { get; set; }
        public string type { get; set; }
        public long size { get; set; }
        public string absPath { get; set; }
        public bool canRead { get; set; }
        public bool canWrite { get; set; }
        public long lastModified { get; set; }

    }

    class ResponseData
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<RemoteFileInfo> data { get; set; }
    }

    /// <summary>
    /// 将文件大小转换为更适合展示的单位
    /// </summary>
    public class ByteToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long byteSize)
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                double len = byteSize;
                int order = 0;

                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len /= 1024;
                }

                return $"{len:F2} {sizes[order]}"; // 格式化为两位小数，并附带合适的单位
            }
            return "0 B";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 将时间戳转换为事件字符串
    /// </summary>
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long timestamp)
            {
                // Unix 时间戳是自1970年1月1日以来的秒数
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                dateTime = dateTime.AddMilliseconds(timestamp).ToLocalTime();
                // 格式化为字符串 yyyy/MM/dd HH:mm:ss
                return dateTime.ToString("yyyy/MM/dd HH:mm:ss");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}