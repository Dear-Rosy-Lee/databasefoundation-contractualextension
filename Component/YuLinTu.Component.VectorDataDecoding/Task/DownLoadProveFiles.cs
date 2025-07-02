using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YuLinTu;
using YuLinTu.Component.VectorDataDecoding.Core;
using YuLinTu.Component.VectorDataDecoding.JsonEntity;
using YuLinTu.DF;

namespace YuLinTu.Component.VectorDataDecoding.Task
{
    [TaskDescriptor(TypeArgument = typeof(DownLoadProveFilesArgument),
        Name = "下载证明材料", Gallery = @"Gallery1\Gallery2",
        UriImage16 = "pack://application:,,,/YuLinTu.Resources;component/Images/16/store.png",
        UriImage24 = "pack://application:,,,/YuLinTu.Resources;component/Images/24/store.png")]
    public class DownLoadProveFiles :YuLinTu. Task
    {
        #region Properties
        internal IVectorService vectorService { get; set; }
        #endregion

        #region Fields

        #endregion

        #region Ctor

        public DownLoadProveFiles()
        {
            Name = "下载证明材料";
            Description = "下载证明材料";
        }

        #endregion

        #region Methods

        #region Methods - Override

        protected override void OnGo()
        {
            this.ReportProgress(0, "任务开始执行");
            this.ReportInfomation("任务开始执行");
            vectorService = new VectorService();
            var args = Argument as DownLoadProveFilesArgument;
            if (args == null)
            {
                this.ReportError("参数不能为空");
                return;
            }
            int pageIndex = 1;int pageSize = 30;int progress = 0;
            while (true)
            {
                var fileInfos = vectorService.DownLoadProveFile(args.ZoneCode, pageIndex, pageSize);
                if (fileInfos.Count == 0) break;
                int count = 0;
                 foreach ( var fileInfo in fileInfos )

                {
                    count++;
                    progress += (50 / fileInfos.Count);
                    if (progress > 90) progress = 90;
                    var msg= ConvertToFile(fileInfo, args,out bool sucess);
                    if(!sucess)
                    {
                        this.ReportError(msg);
                    }
                    else
                    {
                        this.ReportInfomation(msg);
                    }
                    this.ReportProgress(progress);
                }
                pageIndex++;
                this.ReportProgress(100, "完成");

            }
            // TODO : 任务的逻辑实现


            this.ReportProgress(100, "完成");
            this.ReportInfomation("完成");
        }

        private string ConvertToFile(ProveFileEn fileInfo, DownLoadProveFilesArgument args, out bool sucess)
        {
            sucess = false;
            string message = string.Empty;
            if (string.IsNullOrWhiteSpace(fileInfo.base_str))
            {
                message = "错误：Base64字符串为空";
                return message;   
            }
            if (fileInfo.base_str.Length % 4 != 0 || !Regex.IsMatch(fileInfo.base_str, @"^[a-zA-Z0-9\+/]*={0,3}$"))
            {
                message = "错误：无效的Base64格式:"+ fileInfo.base_str;
                return message;
            }
            string mimeType = null;
            if (fileInfo.base_str.StartsWith("data:"))
            {
                int commaIndex = fileInfo.base_str.IndexOf(',');
                if (commaIndex != -1)
                {
                    mimeType = fileInfo.base_str.Substring(5, commaIndex - 5);
                    fileInfo.base_str = fileInfo.base_str.Substring(commaIndex + 1);
                }
            }
            // 转换为字节数组
            byte[] fileBytes = Convert.FromBase64String(fileInfo.base_str);
            string defaultExt = ".pdf";
            if (!string.IsNullOrEmpty(mimeType))
            {
                defaultExt = MimeTypeToExtension(mimeType) ?? defaultExt;
            }
            else
            {
                string detectedType = DetectFileType(fileBytes);
                defaultExt = FileTypeToExtension(detectedType) ?? defaultExt;
            }

            // saveFileDialog.DefaultExt = defaultExt;
            //saveFileDialog.FileName = $"restored_file{defaultExt}";
            var ext = Path.GetExtension(fileInfo.file_name);
            var dateTimeTAG = DateTime.Parse(fileInfo.upload_time);
            var timeTag= dateTimeTAG.ToString("yyyy-MM-dd-HHmmss");
            fileInfo.file_name = Path.Combine(args.ResultFilePath, $"{args.ZoneCode}{args.ZoneName}_{timeTag}{ext})");
            File.WriteAllBytes(fileInfo.file_name, fileBytes);
            message = $"成功下载文件：{fileInfo.file_name}";
            sucess = true;
            return message;
        }

        private string MimeTypeToExtension(string mimeType)
        {
            // 简化版的MIME类型到扩展名映射
            switch (mimeType.Split(';')[0].ToLower())
            {
                case "image/jpeg": return ".jpg";
                case "image/png": return ".png";
                case "image/gif": return ".gif";
                case "application/pdf": return ".pdf";
                case "application/zip": return ".zip";
                case "application/msword": return ".doc";
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document": return ".docx";
                case "text/plain": return ".txt";
                case "audio/mpeg": return ".mp3";
                case "audio/wav": return ".wav";
                case "video/mp4": return ".mp4";
                default: return null;
            }
        }

        private string FileTypeToExtension(string fileType)
        {
            // 检测到的文件类型到扩展名映射
            if (fileType.Contains("JPEG")) return ".jpg";
            if (fileType.Contains("PNG")) return ".png";
            if (fileType.Contains("GIF")) return ".gif";
            if (fileType.Contains("PDF")) return ".pdf";
            if (fileType.Contains("ZIP")) return ".zip";
            if (fileType.Contains("Office")) return ".docx";
            if (fileType.Contains("文本")) return ".txt";
            if (fileType.Contains("MP3")) return ".mp3";
            if (fileType.Contains("WAV")) return ".wav";
            if (fileType.Contains("MPEG")) return ".mp4";
            return null;
        }

        private string DetectFileType(byte[] bytes)
        {
            if (bytes.Length < 4) return "未知类型";

            // 常见文件类型的魔数签名
            if (bytes[0] == 0xFF && bytes[1] == 0xD8) return "JPEG 图像";
            if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47) return "PNG 图像";
            if (bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46) return "GIF 图像";
            if (bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46) return "PDF 文档";
            if (bytes[0] == 0x50 && bytes[1] == 0x4B && bytes[2] == 0x03 && bytes[3] == 0x04) return "ZIP 压缩文件";
            if (bytes[0] == 0xD0 && bytes[1] == 0xCF && bytes[2] == 0x11 && bytes[3] == 0xE0) return "Microsoft Office 文档";
            if (bytes[0] == 0x49 && bytes[1] == 0x44 && bytes[2] == 0x33) return "MP3 音频";
            if (bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46) return "WAV 音频";
            if (bytes[0] == 0x00 && bytes[1] == 0x00 && bytes[2] == 0x01 && bytes[3] == 0xBA) return "MPEG 视频";

            // 纯文本检测
            bool isText = true;
            for (int i = 0; i < Math.Min(100, bytes.Length); i++)
            {
                if (bytes[i] < 32 && bytes[i] != 9 && bytes[i] != 10 && bytes[i] != 13)
                {
                    isText = false;
                    break;
                }
            }

            if (isText) return "文本文件";

            return "二进制文件";
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
        #endregion

        #endregion
    }
}
