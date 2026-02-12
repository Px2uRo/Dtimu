using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Dtimu
{
    internal class CertUtil
    {

        internal static string Sha256Fromb64(string b64)
        {
            byte[] bytes = Convert.FromBase64String(b64);
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(bytes);
                // 将哈希字节数组转换为十六进制字符串
                StringBuilder hashString = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    hashString.Append(b.ToString("x2")); // 转为小写十六进制
                }
                return hashString.ToString();
            }
        }
        internal static string Sha256(string fullName)
        {
            return ComputeSha256Hash(fullName);
        }

        private static string ComputeSha256Hash(string fullName)
        {
            using (FileStream stream = File.OpenRead(fullName))
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(stream);

                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("x2"));
                    }
                    return sb.ToString();
                }
            }
        }
    }
}