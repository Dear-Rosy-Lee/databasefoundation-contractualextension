/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 压缩文件操作
    /// </summary>
    public class SharpZipOperation
    {
        private static string pwdPre = "{9E783670-76A5-4EAF-B510-08E2AECC49D0}";
        private static string pwdLandPre = "{9E783670-76A5-4EAF-B510-08E2AECC49D0}";

        /// <summary>
        /// 打包文件
        /// </summary>
        public static bool Compress(string path, string name, ArrayList fileList, string password)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(name)
                || fileList == null || fileList.Count == 0)
            {
                return false;
            }
            System.IO.DirectoryInfo dirInfo = new DirectoryInfo(path);
            dirInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
            ZipOutputStream zipStream = new ZipOutputStream(File.Create(path + "\\" + name + ".zip"));            //新建压缩文件流 “ZipOutputStream”
            zipStream.Password = pwdPre + password;
            foreach(string fileName in fileList)
            {
                AddZipEntry(zipStream, fileName); //向压缩文件流加入内容
            }
            zipStream.Finish(); // 结束压缩
            zipStream.Close();
            return true;
        }

        /// <summary>
        /// 打包文件
        /// </summary>
        public static bool CompressContractLand(string path, string name, ArrayList fileList, string password)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(name)
                || fileList == null || fileList.Count == 0)
            {
                return false;
            }
            System.IO.DirectoryInfo dirInfo = new DirectoryInfo(path);
            dirInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
            ZipOutputStream zipStream = new ZipOutputStream(File.Create(path + "\\" + name + ".zip"));            //新建压缩文件流 “ZipOutputStream”
            zipStream.Password = pwdLandPre + password;
            foreach (string fileName in fileList)
            {
                AddZipEntry(zipStream, fileName); //向压缩文件流加入内容
            }
            zipStream.Finish(); // 结束压缩
            zipStream.Close();
            return true;
        }

        /// <summary>
        /// 添加压缩实体
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        private static void AddZipEntry(ZipOutputStream stream, string fileName)
        {
            if (!File.Exists(fileName))  //文件的处理
            {
                return;
            }
            stream.SetLevel(9);      //压缩等级
            FileStream fstream = File.OpenRead(fileName);
            byte[] b = new byte[fstream.Length];
            fstream.Read(b, 0, b.Length);          //将文件流加入缓冲字节中
            ZipEntry entry = new ZipEntry(Path.GetFileName(fileName));
            stream.PutNextEntry(entry);             //为压缩文件流提供一个容器
            stream.Write(b, 0, b.Length);  //写入字节
            fstream.Close();
        }

        /// <summary>
        /// 打包文件
        /// </summary>
        public static bool Compress(string fileName, ArrayList fileList, string password)
        {
            if (string.IsNullOrEmpty(fileName) || fileList == null || fileList.Count == 0)
            {
                return false;
            }
            string path = Path.GetDirectoryName(fileName);
            string name = Path.GetFileName(fileName);
            bool success = Compress(path, name, fileList, password);
            return success;
        }
    }
}
