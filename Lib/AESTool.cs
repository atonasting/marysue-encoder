using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MareSueEncoder.Lib
{
    /// <summary>
    /// AES加密工具
    /// </summary>
    public class AESTool
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Source">原文</param>
        /// <returns>Key+密文</returns>
        public static byte[] Encrypt(byte[] Source)
        {
            var aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.GenerateKey();
            aes.KeySize = 128;
            var encryptor = aes.CreateEncryptor();
            var output = encryptor.TransformFinalBlock(Source, 0, Source.Length);
            return aes.Key.Concat(output).ToArray();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Source">Key+密文</param>
        /// <returns>原文</returns>
        public static byte[] Decrypt(byte[] Code)
        {
            var aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.KeySize = 128;
            var keyStringLength = aes.KeySize / 8;
            if (Code.Length <= keyStringLength)
                throw new ArgumentNullException("code too short");

            aes.Key = Code.Take(keyStringLength).ToArray();
            var decryptor = aes.CreateDecryptor();

            var realCode = Code.Skip(keyStringLength).ToArray();
            var output = decryptor.TransformFinalBlock(realCode, 0, realCode.Length);
            return output;
        }
    }
}
