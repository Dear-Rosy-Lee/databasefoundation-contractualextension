/*
 * (C) 2015  鱼鳞图公司版权所有,保留所有权利 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Web;
using YuLinTu.PropertyRight.Services.Client;

namespace YuLinTu.Library.Business
{
    /// <summary>
    /// 服务帮助类
    /// </summary>
    public class ServiceHelper
    {
        /// <summary>
        /// 初始化服务器数据
        /// </summary>
        /// <returns></returns>
        public static ContractLandRegistrationServiceClient InitazlieServerData(string userName, string sessionCode, bool isEncrypt, string businessDataService)
        {
            ContractLandRegistrationServiceClient landService = null;
            if (isEncrypt)
            {
                var binding = new System.ServiceModel.WSHttpBinding();
                binding.Security.Mode = System.ServiceModel.SecurityMode.Message;
                binding.Security.Message.ClientCredentialType = System.ServiceModel.MessageCredentialType.UserName;
                binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.None;
                binding.SendTimeout = TimeSpan.FromMinutes(30.0);
                binding.MaxBufferPoolSize = 104857600;
                binding.MaxReceivedMessageSize = 104857600;
                binding.MaxBufferPoolSize = 104857600;
                binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas();
                binding.ReaderQuotas.MaxDepth = 32;
                binding.ReaderQuotas.MaxStringContentLength = 104857600;
                binding.ReaderQuotas.MaxArrayLength = 104857600;
                binding.ReaderQuotas.MaxBytesPerRead = 4096;
                binding.ReaderQuotas.MaxNameTableCharCount = 16384;

                var uri = new Uri(businessDataService);
                var identity = System.ServiceModel.EndpointIdentity.CreateDnsIdentity("PRServer");
                var addr = new System.ServiceModel.EndpointAddress(uri, identity);

                landService = new ContractLandRegistrationServiceClient(binding, addr);
                var clientCredentials = landService.Endpoint.Behaviors.Find<System.ServiceModel.Description.ClientCredentials>();
                clientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
                clientCredentials.ServiceCertificate.Authentication.CustomCertificateValidator = new YuLinTu.PropertyRight.Services.CustomX509Validator();
                landService.ClientCredentials.UserName.UserName = userName;
                // sessionCode = InitalizeLoginInformation(userName, password, businessDataService);
                landService.ClientCredentials.UserName.Password = sessionCode;
            }
            else
            {
                var binding = new System.ServiceModel.BasicHttpBinding();
                binding.SendTimeout = TimeSpan.FromMinutes(30.0);
                binding.MaxBufferPoolSize = 104857600;
                binding.MaxReceivedMessageSize = 104857600;
                binding.MaxBufferPoolSize = 104857600;
                binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas();
                binding.ReaderQuotas.MaxDepth = 32;
                binding.ReaderQuotas.MaxStringContentLength = 104857600;
                binding.ReaderQuotas.MaxArrayLength = 104857600;
                binding.ReaderQuotas.MaxBytesPerRead = 4096;
                binding.ReaderQuotas.MaxNameTableCharCount = 16384;
                var addr = new System.ServiceModel.EndpointAddress(businessDataService);
                landService = new ContractLandRegistrationServiceClient(binding, addr);
                landService.ClientCredentials.UserName.UserName = userName;
                //string sessionCode = InitalizeLoginInformation(userName, password, businessDataService);
                landService.ClientCredentials.UserName.Password = sessionCode;
            }
            var serializerBehaviorType = typeof(BasicHttpBinding).Assembly.GetTypes().First(t => t.Name == "DataContractSerializerServiceBehavior");
            var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance;
            var serializerBehavior = serializerBehaviorType.GetConstructor(flags, null, new Type[] { typeof(bool), typeof(int) }, null)
                .Invoke(new object[] { false, 2147483647 });
            landService.Endpoint.Behaviors.Add(serializerBehavior as IEndpointBehavior);
            return landService;
        }

        /// <summary>
        /// 初始化登录信息
        /// </summary>
        public static string InitalizeLoginInformation(string userName, string password, string businessDataService)
        {
            try
            {
                string secruityUrl = businessDataService;
                if (string.IsNullOrEmpty(secruityUrl))
                {
                    return string.Empty;
                }
                secruityUrl = secruityUrl.Replace("amp;", "");
                secruityUrl += userName;
                secruityUrl += "&pwd=";
                secruityUrl += EncryptDES(password);
                secruityUrl = secruityUrl.Replace("+", "%2b");
                WebRequest tokenRequest = System.Net.WebRequest.Create(secruityUrl);
                tokenRequest.ContentType = "application/x-www-form-urlencoded";
                WebResponse tokenResponse = tokenRequest.GetResponse();
                Stream responseStream = tokenResponse.GetResponseStream();
                var readStream = new StreamReader(responseStream);
                string token = readStream.ReadToEnd();
                readStream.Close();
                if (string.IsNullOrEmpty(token))
                {
                    return token;
                }
                token = token.Replace("\"", "");
                Guid id = new Guid(token);
                if (id != null && id != Guid.Empty)
                {
                    return token;
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return string.Empty;
        }

        /// <summary>
        /// 加密
        /// </summary>
        public static string EncryptDES(string content, string container = "_default_")
        {
            byte[] key = System.Text.Encoding.UTF8.GetBytes(container.Substring(0, 8));

            byte[] buffer;
            var desCSP = new DESCryptoServiceProvider();
            byte[] desKey = { };
            byte[] desIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
            MemoryStream ms = new MemoryStream();
            CryptoStream cryStream = new CryptoStream(ms, desCSP.CreateEncryptor(key, desIV), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cryStream);
            sw.WriteLine(content);
            sw.Close();
            cryStream.Close();
            buffer = ms.ToArray();

            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="pToDecrypt">加密的字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns>解密后的字符串</returns>
        private static string Decrypt(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            return HttpContext.Current.Server.UrlDecode(System.Text.Encoding.Default.GetString(ms.ToArray()));
        }
    }
}
