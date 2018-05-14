using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Hawk.Common
{
    public class RsaHelper
    {
        /// <summary>产生非对称密钥对</summary>
        /// <remarks>
        /// RSAParameters的各个字段采用大端字节序，转为BigInteger的之前一定要倒序。
        /// RSA加密后密文最小长度就是密钥长度，所以1024密钥最小密文长度是128字节。
        /// </remarks>
        /// <param name="keySize">密钥长度，默认1024位强密钥</param>
        /// <returns></returns>
        public static string[] GenerateKey(int keySize = 1024)
        {
            var rsa = new RSACryptoServiceProvider(keySize);
            var ss = new string[2];
            ss[0] = rsa.ToXmlString(true);
            ss[1] = rsa.ToXmlString(false);
            return ss;
        }

        public static byte[] Encrypt(byte[] buf, string pubKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(pubKey);
            return rsa.Encrypt(buf, true);
        }

        public static byte[] Decrypt(byte[] buf, string priKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(priKey);
            return rsa.Decrypt(buf, true);
        }

        public static byte[] Sign(byte[] buf, string priKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(priKey);
            return rsa.SignData(buf, SHA1.Create());
        }

        public static bool Verify(byte[] buf, byte[] signature, string pubKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(pubKey);
            return rsa.VerifyData(buf, "", signature);
        }

        public static bool VerifyHash(byte[] buf, byte[] signature, string pubKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(pubKey);
            return rsa.VerifyHash(buf, "", signature);
        }
    }
}
