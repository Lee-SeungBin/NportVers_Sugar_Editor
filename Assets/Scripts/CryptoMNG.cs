using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class CryptoMNG : MonoBehaviour
{
    private static string Enckey = "7rMG6jG3UWwXBZS3sRWLZ7jjalkTsudw";
    private static readonly string EncIv = "0123456789012345";

    //AES 암호화

    public static string AESEncrypt256(string input)
    {
        try
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(Enckey);
            aes.IV = Encoding.UTF8.GetBytes(EncIv);
            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] buf = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(input);
                    cs.Write(xXml, 0, xXml.Length);
                }
                buf = ms.ToArray();
            }
            string Output = Convert.ToBase64String(buf);
            return Output;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return ex.Message;
        }
    }

    //AES 복호화
    public static string AESDecrypt256(string input)
    {
        try
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(Enckey);
            aes.IV = Encoding.UTF8.GetBytes(EncIv);
            var decrypt = aes.CreateDecryptor();
            byte[] buf = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Convert.FromBase64String(input);
                    cs.Write(xXml, 0, xXml.Length);
                }
                buf = ms.ToArray();
            }
            string Output = Encoding.UTF8.GetString(buf);
            return Output;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return string.Empty;
        }
    }


    public string DecrStage(string stageDataStr)
    {
        return AESDecrypt256(stageDataStr);
    }

    public string EncrStage(string stageDataStr)
    {
        return AESEncrypt256(stageDataStr);
    }
}