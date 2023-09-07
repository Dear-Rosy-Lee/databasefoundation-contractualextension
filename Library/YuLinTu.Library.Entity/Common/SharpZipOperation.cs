//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Collections;
//using System.IO;
//using ICSharpCode.SharpZipLib.Zip;

//namespace YuLinTu.Library.Entity
//{
//    /// <summary>
//    /// 压缩文件操作
//    /// </summary>
//    public class SharpZipOperation
//    {
//        private static string pwdPre = "{9E783670-76A5-4EAF-B510-08E2AECC88D0}";

//        /// <summary>
//        /// 打包文件
//        /// </summary>
//        public static bool Compress(string path, string name, ArrayList fileList, string password)
//        {
//            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(name)
//                || fileList == null || fileList.Count == 0)
//            {
//                return false;
//            }
//            System.IO.DirectoryInfo dirInfo = new DirectoryInfo(path);
//            dirInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
//            using (ZipFile zip = new ZipFile(path))
//            {
//                zip.Password = pwdPre + password;
//                foreach (string file in fileList)
//                {
//                    zip.Add(file);
//                }
//                //zip.cr(name + ".zip");
//            }
//            return true;
//        }

//        /// <summary>
//        /// 打包文件
//        /// </summary>
//        public static bool Compress(string fileName, ArrayList fileList, string password)
//        {
//            if (string.IsNullOrEmpty(fileName) || fileList == null || fileList.Count == 0)
//            {
//                return false;
//            }
//            string path = Path.GetDirectoryName(fileName);
//            string name = Path.GetFileName(fileName);
//            bool success = Compress(path, name, fileList, password);
//            return success;
//        }

//        /// <summary>
//        /// 打包文件
//        /// </summary>
//        public static bool Extract(string fileName, string password)
//        {
//            //if (string.IsNullOrEmpty(fileName) || !ZipFile.IsZipFile(fileName))
//            //{
//            //    return false;
//            //}
//            //using (ZipFile zip = ZipFile.Read(fileName))
//            //{
//            //    zip.Password = pwdPre + password;
//            //    zip.ExtractAll(Path.GetDirectoryName(fileName));
//            //}
//            return true;
//        }

//        public static string ExtractZip(string fileName, string password)
//        {
//            //using (ZipFile zip = ZipFile.Read(fileName))
//            //{
//            //    string dirName = Path.Combine(
//            //        Path.GetDirectoryName(fileName),
//            //        Path.GetFileNameWithoutExtension(fileName));

//            //    zip.Password = pwdPre + password;
//            //    zip.ExtractAll(dirName, ExtractExistingFileAction.OverwriteSilently);

//            //    return Directory.GetFiles(dirName)[0];
//            //}
//            return "";
//        }

//        public static void CompressZip(string zipFileName, string[] fileNames, string password)
//        {
//            //using (ZipFile zip = new ZipFile())
//            //{
//            //    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
//            //    zip.Password = pwdPre + password;
//            //    zip.AddFiles(fileNames, ".");
//            //    zip.Save(zipFileName);
//            //}
//        }
//    }
//}
