/*
 * (C) 2025  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace YuLinTu.Library.WorkStation
{
    /// <summary>
    /// 编码
    /// </summary>
    public class ToolCoding
    {
        #region Methods - Encode

        public static string EncodeToMD5(string txt)
        {
            if (txt == null)
                txt = string.Empty;

            string str = string.Empty;
            MD5 md = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(txt);
            byte[] buffer2 = md.ComputeHash(bytes);
            md.Clear();
            for (int i = 0; i < (buffer2.Length - 1); i++)
            {
                str = str + buffer2[i].ToString("x").PadLeft(2, '0');
            }
            return str;
        }

        /// <summary>
        /// 生成加密文件
        /// </summary>
        public static string InitalizeTokenFile(string tokenCode)
        {
            string fullCode = ToolCoding.EncodeToMD5(tokenCode);
            fullCode += ToolCoding.EncodeToMD5("yulintu");
            fullCode += ToolCoding.EncodeToMD5("songsong");
            string tokenString = string.Empty;
            for (int i = 0; i < tokenCode.Length; i++)
            {
                string key = tokenCode.Substring(i, 1);
                tokenString += key;
                tokenString += fullCode.Substring(i * 4, 4);
            }
            return tokenString;
        }

        /// <summary>
        /// 生成加密文件
        /// </summary>
        public static void InitalizeTokenFile(string fileName, string tokenCode)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                return;
            }
            string fullCode = ToolCoding.EncodeToMD5(tokenCode);
            fullCode += ToolCoding.EncodeToMD5("yulintu");
            fullCode += ToolCoding.EncodeToMD5("songsong");
            string tokenString = string.Empty;
            for (int i = 0; i < tokenCode.Length; i++)
            {
                string key = tokenCode.Substring(i, 1);
                int val = 0;
                Int32.TryParse(key, out val);
                string prefiex = InitalizeColumnValue(val);
                tokenString += prefiex;
                tokenString += fullCode.Substring(i * 4, 4);
            }
            string tokenstring = ToolString.SplitIn(tokenString, '-', 5).ToUpper();
            FileStream stream = File.Create(fileName);
            BinaryFormatter binFmt = new BinaryFormatter();
            binFmt.Serialize(stream, tokenstring);
            stream.Close();
        }

        /// <summary>
        /// 获取列值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string InitalizeColumnValue(int value)
        {
            string columnName = string.Empty;
            switch (value)
            {
                case 3:
                    columnName = "A";
                    break;
                case 2:
                    columnName = "9";
                    break;
                case 1:
                    columnName = "C";
                    break;
                case 4:
                    columnName = "6";
                    break;
                case 5:
                    columnName = "E";
                    break;
                case 8:
                    columnName = "F";
                    break;
                case 7:
                    columnName = "G";
                    break;
                case 6:
                    columnName = "5";
                    break;
                case 9:
                    columnName = "I";
                    break;
                default:
                    columnName = "2";
                    break;
            }
            return columnName;
        }

        /// <summary>
        /// 获取列值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int DeserializeColumnValue(string value)
        {
            int columnName = 0;
            switch (value)
            {
                case "A":
                    columnName = 3;
                    break;
                case "9":
                    columnName = 2;
                    break;
                case "C":
                    columnName = 1;
                    break;
                case "6":
                    columnName = 4;
                    break;
                case "E":
                    columnName = 5;
                    break;
                case "F":
                    columnName = 8;
                    break;
                case "G":
                    columnName = 7;
                    break;
                case "5":
                    columnName = 6;
                    break;
                case "I":
                    columnName = 9;
                    break;
                case "2":
                    columnName = 0;
                    break;
                default:
                    columnName = 0;
                    break;
            }
            return columnName;
        }

        /// <summary>
        /// 生成加密文件
        /// </summary>
        public static string DeserializeTokenFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return string.Empty;
            }
            object obj = null;
            try
            {
                FileStream stream = File.OpenRead(fileName);
                BinaryFormatter binFmt = new BinaryFormatter();
                obj = binFmt.Deserialize(stream);
                stream.Close();
                if (obj == null)
                {
                    return string.Empty;
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            if (obj == null)
            {
                return string.Empty;
            }
            string[] value = obj.ToString().Split('-');
            if (value == null || value.Length == 0)
            {
                return string.Empty;
            }
            string fullCode = string.Empty;
            for (int i = 0; i < value.Length; i++)
            {
                string key = value[i].Substring(0, 1);
                fullCode += DeserializeColumnValue(key);
            }
            return fullCode;
        }

        /// <summary>
        /// 生成加密文件
        /// </summary>
        public static string DeserializeTokenInformation(string tokenString)
        {
            string token = string.Empty;
            int length = tokenString.Length / 5;
            for (int i = 0; i < length; i++)
            {
                string key = tokenString.Substring(i * 5, 1);
                token += key;
            }
            return token;
        }

        #endregion
    }
}
