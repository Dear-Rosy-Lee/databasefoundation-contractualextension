using System;
using System.IO;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

public static class EncrypterSM
{
    // 加密方法
    // SM4密钥长度为16字节(128位)
    private const int KeySize = 16;
    // SM4初始化向量长度为16字节(128位)
    private const int IvSize = 16;

    private static byte[] IV = new byte[] { 121, 117, 108, 105, 110, 116, 117, 50, 48, 50, 53, 48, 54, 49, 51, 45 };
    /// <summary>
    /// SM4加密
    /// </summary>
    /// <param name="plainText">明文</param>
    /// <param name="key">密钥(16字节)</param>
    /// <param name="iv">初始化向量(16字节，ECB模式下可为null)</param>
    /// <param name="mode">加密模式：ECB/CBC</param>
    /// <param name="padding">填充方式：PKCS7/None</param>
    /// <returns>Base64编码的密文</returns>
    public static string EncryptSM4(string plainText, string keyText, byte[] iv = null)
    {
        if (string.IsNullOrEmpty(plainText))
            return null;
        if (iv == null) iv = IV;
        byte[] data = Encoding.UTF8.GetBytes(plainText);
        byte[] key = Encoding.UTF8.GetBytes(keyText);
        byte[] encrypted = Encrypt(data, key, iv);
        return Convert.ToBase64String(encrypted);
    }


    public static string DecryptSM4(string encryptedText, string keyText, byte[] iv = null)
    {
        if (string.IsNullOrEmpty(encryptedText))
            return null;
        if (iv == null) iv = IV;
        byte[] data = Convert.FromBase64String(encryptedText); //Encoding.UTF8.GetBytes(encryptedText);
        byte[] key = Encoding.UTF8.GetBytes(keyText);
        byte[] encrypted = Decrypt(data, key, iv);
        return Encoding.UTF8.GetString(encrypted);
    }

    // 加密方法
    public static byte[] Encrypt(byte[] plainText, byte[] key, byte[] iv)
    {
        // 参数校验
        if (key == null || key.Length != KeySize)
            throw new ArgumentException("Key must be 16 bytes (128 bits)");
        if (iv == null || iv.Length != IvSize)
            throw new ArgumentException("IV must be 16 bytes (128 bits)");

        // 创建加密引擎（CBC模式 + PKCS7填充）
        BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(
            new CbcBlockCipher(new SM4Engine()), new Pkcs7Padding());

        // 初始化加密参数
        cipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));

        // 处理加密
        byte[] output = new byte[cipher.GetOutputSize(plainText.Length)];
        int bytesProcessed = cipher.ProcessBytes(plainText, 0, plainText.Length, output, 0);
        bytesProcessed += cipher.DoFinal(output, bytesProcessed);

        // 返回正确长度的密文
        byte[] result = new byte[bytesProcessed];
        Array.Copy(output, result, bytesProcessed);
        return result;
    }

    // 解密方法
    public static byte[] Decrypt(byte[] cipherText, byte[] key, byte[] iv)
    {
        // 参数校验
        if (key == null || key.Length != KeySize)
            throw new ArgumentException("Key must be 16 bytes (128 bits)");
        if (iv == null || iv.Length != IvSize)
            throw new ArgumentException("IV must be 16 bytes (128 bits)");
        // 创建解密引擎（CBC模式 + PKCS7填充）
        BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(
            new CbcBlockCipher(new SM4Engine()), new Pkcs7Padding());

        // 初始化解密参数
        cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));

        // 处理解密
        byte[] output = new byte[cipher.GetOutputSize(cipherText.Length)];
        int bytesProcessed = cipher.ProcessBytes(cipherText, 0, cipherText.Length, output, 0);
        bytesProcessed += cipher.DoFinal(output, bytesProcessed);

        // 返回正确长度的明文
        byte[] result = new byte[bytesProcessed];
        Array.Copy(output, result, bytesProcessed);
        return result;
    }
}
