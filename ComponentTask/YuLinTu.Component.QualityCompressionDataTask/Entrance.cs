using System;
using System.IO;
using System.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using YuLinTu.Windows;

namespace YuLinTu.Component.QualityCompressionDataTask
{
    /// <summary>
    /// 应用程序上下文
    /// </summary>
    public class Entrance : EntranceBase
    {
        #region Methods

        /// <summary>
        /// 重写注册工作空间方法
        /// </summary>
        protected override void OnConnect()
        {
            RegisterWorkspaceContext<WorkspaceContext>();
            //LanguageAttribute.AddLanguage(YuLinTu.Business.TaskBasic.Properties.Resources.FolderChs);
            //X9ECParameters sm2Param = ECNamedCurveTable.GetByName("sm2p256v1");
            //ECDomainParameters domainParams = new ECDomainParameters(sm2Param.Curve, sm2Param.G, sm2Param.N, sm2Param.H);

            //// 2. 生成密钥对
            //ECKeyPairGenerator keyPairGenerator = new ECKeyPairGenerator();
            //ECKeyGenerationParameters keyGenParam = new ECKeyGenerationParameters(domainParams, new SecureRandom());
            //keyPairGenerator.Init(keyGenParam);
            //LoadKeysFromPem();
            //AsymmetricCipherKeyPair keyPair = LoadKeysFromPem();
            //SaveKeysToPem(keyPair);

            //todo:后续再进行加密

            // 获取公钥和私钥
            // 读取私钥
            //ECPrivateKeyParameters privateKey = KeyReader.ReadPrivateKeyFromPem("privateKey.pem");
            //// 读取公钥
            //ECPublicKeyParameters publicKey = KeyReader.ReadPublicKeyFromPem("publicKey.pem");

            //// 3. 待加密数据
            //byte[] dataToEncrypt = System.Text.Encoding.UTF8.GetBytes("SM2test");

            //// 4. 加密数据
            //SM2Engine sm2Engine = new SM2Engine();
            //sm2Engine.Init(true, new ParametersWithRandom(publicKey, new SecureRandom()));
            //byte[] encryptedData = sm2Engine.ProcessBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            


            //Console.WriteLine("Encrypted Data: " + BitConverter.ToString(encryptedData));
            //Parameters.PassWord = BitConverter.ToString(encryptedData);

            //var encryptedData2 = StringToByteArray(Parameters.PassWord);
            //// 5. 解密数据
            //sm2Engine.Init(false, privateKey);
            //byte[] decryptedData = sm2Engine.ProcessBlock(encryptedData2, 0, encryptedData.Length);
            //var res = System.Text.Encoding.UTF8.GetString(decryptedData);
            //Console.WriteLine("Decrypted Data: " + System.Text.Encoding.UTF8.GetString(decryptedData));

        }

        public static byte[] StringToByteArray(string hex)
        {
            // 移除分隔符 "-"
            hex = hex.Replace("-", "");

            // 转换为 byte[]
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static AsymmetricCipherKeyPair LoadKeysFromPem()
        {
            using (TextReader privateKeyTextReader = new StreamReader("privateKey.pem"))
            using (TextReader publicKeyTextReader = new StreamReader("publicKey.pem"))
            {
                PemReader privateKeyPemReader = new PemReader(privateKeyTextReader);
                PemReader publicKeyPemReader = new PemReader(publicKeyTextReader);

                ECPrivateKeyParameters privateKey = (ECPrivateKeyParameters)privateKeyPemReader.ReadObject();
                ECPublicKeyParameters publicKey = (ECPublicKeyParameters)publicKeyPemReader.ReadObject();

                return new AsymmetricCipherKeyPair(publicKey, privateKey);
            }
        }

        public static void SaveKeysToPem(AsymmetricCipherKeyPair keyPair)
        {
            // 保存公钥
            using (TextWriter publicKeyTextWriter = new StreamWriter("publicKey.pem"))
            {
                PemWriter pemWriter = new PemWriter(publicKeyTextWriter);
                pemWriter.WriteObject(keyPair.Public);
            }

            // 保存私钥
            using (TextWriter privateKeyTextWriter = new StreamWriter("privateKey.pem"))
            {
                PemWriter pemWriter = new PemWriter(privateKeyTextWriter);
                pemWriter.WriteObject(keyPair.Private);
            }
        }
        #endregion Methods
    }
    public class KeyReader
    {
        public static ECPublicKeyParameters ReadPublicKeyFromPem(string publicKeyPath)
        {
            using (TextReader reader = new StreamReader(publicKeyPath))
            {
                PemReader pemReader = new PemReader(reader);
                return (ECPublicKeyParameters)pemReader.ReadObject();
            }
        }
        public static ECPrivateKeyParameters ReadPrivateKeyFromPem(string privateKeyPath)
        {
            using (TextReader reader = new StreamReader(privateKeyPath))
            {
                PemReader pemReader = new PemReader(reader);
                object obj = pemReader.ReadObject();

                // 如果读取到的是密钥对
                if (obj is AsymmetricCipherKeyPair keyPair)
                {
                    return (ECPrivateKeyParameters)keyPair.Private;
                }

                // 如果读取到的是单个私钥
                if (obj is ECPrivateKeyParameters privateKey)
                {
                    return privateKey;
                }

                throw new InvalidCastException("The provided PEM file does not contain a valid EC private key.");
            }
        }

        // 读取密钥对
        public AsymmetricCipherKeyPair ReadKeyPairFromPem(string privateKeyPath)
        {
            using (TextReader reader = new StreamReader(privateKeyPath))
            {
                PemReader pemReader = new PemReader(reader);
                return (AsymmetricCipherKeyPair)pemReader.ReadObject();
            }
        }
    }
}