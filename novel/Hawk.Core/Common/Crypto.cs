using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Hawk.Common
{
    public static class Crypto
    {
        #region Hash散列

        static byte[] HashByte(string s, HashAlgorithm algorithm, Encoding encode = null)
        {    
            char[] c = s.ToCharArray(0, s.Length);
            byte[] inputBuffer = (encode != null ? encode : Encoding.UTF8).GetBytes(c, 0, c.Length);
            return algorithm.ComputeHash(inputBuffer, 0, inputBuffer.Length);
        }

        static string Hash(string s, HashAlgorithm algorithm, Encoding encode = null, bool upper = false)
        {           
            byte[] hashByte = HashByte(s, algorithm, encode);
            return ToHex(hashByte, upper);
        }
        static HashAlgorithm GetAlgorithm(HashName hashName)
        {
            if (hashName == HashName.CRC32) return new Crc32();
            else return HashAlgorithm.Create(Enum.GetName(typeof(HashName), hashName));
        }

        public static byte[] HashByteEx(this string s, HashName hashName = HashName.MD5, Encoding encode = null)
        {
            if (string.IsNullOrEmpty(s)) return null;
            HashAlgorithm algorithm = GetAlgorithm(hashName);
            return HashByte(s, algorithm, encode);
        }    

        public static string HashEx(this string s, HashName hashName = HashName.MD5, Encoding encode = null, bool upper = false)
        {
            if (string.IsNullOrEmpty(s)) return null;
            HashAlgorithm algorithm = GetAlgorithm(hashName);
            return Hash(s, algorithm, encode, upper);
        }

        /// <summary>
        /// 获取文件的散列值
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="hashName"></param>
        /// <param name="upper"></param>
        /// <param name="length">每次读取的长度</param>
        /// <returns></returns>
        public static byte[] HashFileByte(this Stream stream, HashName hashName = HashName.MD5, bool upper = false, int length = 1048576)
        {
            if (stream == null) return null;
            HashAlgorithm hash = GetAlgorithm(hashName);
            byte[] bufferSize = new byte[length];
            long offset = 0;
            while (offset < stream.Length)
            {
                int readSize = length;
                if (offset + readSize > stream.Length)
                    readSize = (int)(stream.Length - offset);
                stream.Read(bufferSize, 0, readSize);
                if (offset + readSize < stream.Length) // 不是最后一块                 
                    hash.TransformBlock(bufferSize, 0, readSize, bufferSize, 0);
                else // 最后一块
                    hash.TransformFinalBlock(bufferSize, 0, readSize);
                offset += length;
                //  backgroundWorkerHash.ReportProgress((int)(offset * 100 / fs.Length));
            }
            return hash.Hash;
        }

        /// <summary>
        /// 获取文件的散列值
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="hashName"></param>
        /// <param name="upper"></param>
        /// <param name="length">每次读取的长度</param>
        /// <returns></returns>
        public static string HashFile(this Stream stream, HashName hashName = HashName.MD5, bool upper = false, int length = 1048576)
        {
            if (stream == null) return null;
            var b = HashFileByte(stream, hashName, upper, length);
            return ToHex(b, upper);
        }

        #endregion

        #region 对称加密
        #region 加密

        public static byte[] EncryptBytes(this byte[] sByte, byte[] rgbKey = null, byte[] rgbIV = null, CipherMode cm = CipherMode.CBC, PaddingMode pm = PaddingMode.PKCS7, bool padding = false, Algorithm algName = Algorithm.AES)
        {
            SymmetricAlgorithm sa = SymmetricAlgorithm.Create(Enum.GetName(typeof(Algorithm), algName));
            int[] len = ValidKeyLength(algName);

            byte[] key = ValidKeySize(rgbKey, Keys, len[0], padding);
            byte[] iv = ValidKeySize(rgbIV, IV, len[1], padding);
            //填充方式共用java,php
            sa.Mode = cm;// CipherMode.CBC;
            sa.Padding = pm;// PaddingMode.Zeros;
            ICryptoTransform encryptor = sa.CreateEncryptor(key, iv);

            return encryptor.TransformFinalBlock(sByte, 0, sByte.Length);

            #region   CryptoStream实现
            #region Write实现
            //using (MemoryStream ms = new MemoryStream())
            //using (CryptoStream cStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            //{
            //    cStream.Write(sourceBytes, 0, sourceBytes.Length);
            //    cStream.FlushFinalBlock();

            //    //using (StreamWriter sw = new StreamWriter(cStream))
            //    //    sw.Write();

            //    return  ms.ToArray();
            //}
            #endregion
            #region  Read实现
            //using (MemoryStream ms=new MemoryStream())
            //using (CryptoStream cStream = new CryptoStream(ms,encryptor, CryptoStreamMode.Read))
            //using (MemoryStream destMs = new MemoryStream())
            //{
            //    byte[] buffer = new byte[100];
            //    int readLen;
            //    while ((readLen = cStream.Read(buffer, 0, 100)) > 0)
            //        destMs.Write(buffer, 0, readLen);
            //    return destMs.ToArray();
            //}
            #endregion
            #endregion
        }

        public static string Encryption(this string s, Encoding encode = null, byte[] rgbKey = null, byte[] rgbIV = null, CipherMode cm = CipherMode.CBC, PaddingMode pm = PaddingMode.PKCS7, bool padding = false, Algorithm algName = Algorithm.AES, bool upper = false)
        {
            char[] chars = s.ToCharArray(0, s.Length);
            byte[] sourceBytes = (encode != null ? encode : Encoding.UTF8).GetBytes(chars, 0, chars.Length);
            byte[] resultBytes = EncryptBytes(sourceBytes, rgbKey, rgbIV, cm, pm, padding, algName);

            return ToHex(resultBytes, upper);
        }

        //public static string Encryption(string s, byte[] rgbKey = null, byte[] rgbIV = null, CipherMode cm = CipherMode.CBC, PaddingMode pm = PaddingMode.PKCS7, bool padding = false, Algorithm algName = Algorithm.AES, bool upper = false)
        //{
        //    return Encryption(s, Encoding.UTF8, rgbKey, rgbIV, cm, pm, padding, algName, upper);
        //}

        public static string Encrypt(this string s, Encoding encode = null, string key = null, string iv = null, CipherMode cm = CipherMode.CBC, PaddingMode pm = PaddingMode.PKCS7, bool padding = false, Algorithm algName = Algorithm.AES, bool upper = false)
        {
            encode = encode != null ? encode : Encoding.UTF8;
            byte[] rgbKey = null, rgbIV = null;
            if (!string.IsNullOrWhiteSpace(key))
                rgbKey = encode.GetBytes(key);
            if (!string.IsNullOrWhiteSpace(iv))
                rgbIV = encode.GetBytes(iv);
            return Encryption(s, encode, rgbKey, rgbIV, cm, pm, padding, algName, upper);
        }

        //public static string Encrypt(string s, string key = null, string iv = null, CipherMode cm = CipherMode.CBC, PaddingMode pm = PaddingMode.PKCS7, bool padding = false, Algorithm algName = Algorithm.AES, bool upper = false)
        //{
        //    return Encrypt(s, Encoding.UTF8, key, iv, cm, pm, padding, algName, upper);
        //}

        #endregion

        #region 解密
        public static byte[] DecryptBytes(this byte[] sByte, byte[] rgbKey, byte[] rgbIV, CipherMode cm = CipherMode.CBC, PaddingMode pm = PaddingMode.PKCS7, bool padding = false, Algorithm algName = Algorithm.AES)
        {
            SymmetricAlgorithm sa = SymmetricAlgorithm.Create(Enum.GetName(typeof(Algorithm), algName));
            int[] len = ValidKeyLength(algName);

            byte[] key = ValidKeySize(rgbKey, Keys, len[0], padding);
            byte[] iv = ValidKeySize(rgbIV, IV, len[1], padding);

            //sa.Mode = CipherMode.CBC;
            //sa.Padding = PaddingMode.PKCS7;

            //填充方式共用java,php
            sa.Mode = cm; //CipherMode.OFB;
            sa.Padding = pm;// PaddingMode.ISO10126;

            ICryptoTransform decryptor = sa.CreateDecryptor(key, iv);

            return decryptor.TransformFinalBlock(sByte, 0, sByte.Length);


            #region   CryptoStream实现
            //using (MemoryStream ms = new MemoryStream())
            //using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
            //{
            //    cs.Write(sourceBytes, 0, sourceBytes.Length);
            //    cs.FlushFinalBlock();

            //    //using (StreamWriter sw = new StreamWriter(cs))
            //    //    sw.Write();

            //    return ms.ToArray();
            //}
            #endregion
        }

        public static string Decryption(this string s, Encoding encode = null, byte[] rgbKey = null, byte[] rgbIV = null, CipherMode cm = CipherMode.CBC, PaddingMode pm = PaddingMode.PKCS7, bool padding = false, Algorithm algName = Algorithm.AES)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            string result = string.Empty;
            byte[] sourceBytes = Hex(s);
            if (sourceBytes != null)
            {
                try
                {
                    byte[] resultByte = DecryptBytes(sourceBytes, rgbKey, rgbIV, cm, pm, padding, algName);
                    if (resultByte != null)
                        result = (encode != null ? encode : Encoding.UTF8).GetString(resultByte, 0, resultByte.Length);
                }
#if DEBUG
                catch (CryptographicException ex)
                {
                    result = ex.Message;
                }
                catch (Exception ex)
                {
                    result = ex.ToString();
                }
#else
                catch
                {
                    result = string.Empty;
                }
#endif
            }
            return result;
        }

        //public static string Decryption(string s, byte[] rgbKey = null, byte[] rgbIV = null, CipherMode cm = CipherMode.CBC, PaddingMode pm = PaddingMode.PKCS7, bool padding = false, Algorithm algName = Algorithm.AES)
        //{
        //    return Decryption(s, Encoding.UTF8, rgbKey, rgbIV, cm, pm, padding, algName);
        //}

        public static string Decrypt(this string s, Encoding encode = null, string key = null, string iv = null, CipherMode cm = CipherMode.CBC, PaddingMode pm = PaddingMode.PKCS7, bool padding = false, Algorithm algName = Algorithm.AES)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            encode = (encode != null ? encode : Encoding.UTF8);
            byte[] rgbKey = null, rgbIV = null;
            if (!string.IsNullOrWhiteSpace(key))
                rgbKey = encode.GetBytes(key);
            if (!string.IsNullOrWhiteSpace(iv))
                rgbIV = encode.GetBytes(iv);
            return Decryption(s, encode, rgbKey, rgbIV, cm, pm, padding, algName);
        }

        //public static string Decrypt(string s, string key = null, string iv = null, CipherMode cm = CipherMode.CBC, PaddingMode pm = PaddingMode.PKCS7, bool padding = false, Algorithm algName = Algorithm.AES)
        //{
        //    return Decrypt(s, Encoding.UTF8, key, iv, cm, pm, padding, algName);
        //}
        #endregion
        #endregion

        #region 加密算法KEY，IV验证

         static int[] ValidKeyLength(Algorithm algName)
        {
            int[] len = new int[2];
            switch (algName)
            {
                case Algorithm.DES:
                    len[0] = 8;//max:64,min:64,skip:0
                    len[1] = 8;//max:64,min:64,skip:0
                    break;
                case Algorithm.RC2:
                    len[0] = 16;//max:128,min:40,skip:8
                    len[1] = 8;//max:64,min:64,skip:0
                    break;
                case Algorithm.TripleDES:
                    len[0] = 24;//max:192,min:128,skip:64
                    len[1] = 8;//max:64,min:64,skip:0
                    break;
                default:
                    len[0] = 32;//max:256,min:128,skip:64
                    len[1] = 16;//max:128,min:128,skip:0
                    break;
            }
            return len;
        }

        /// <summary>
        /// 检查密钥的长度.如果大于指定的长度,截取为相等的长度;
        /// 等于指定的长度,返回;
        /// 如果小于指定的长度,用默认密钥的字节补充.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="padBytes"></param>
        /// <param name="length"></param>
        /// <param name="padding">是否填充密钥</param>
        /// <returns></returns>
        static byte[] ValidKeySize(byte[] keys, byte[] padBytes, int length, bool padding = false)
        {
            if (!padding && keys != null)
                return keys;

            if (keys != null && keys.Length > 0)
            {
                int len = keys.Length;
                if (len >= length)
                {
                    if (length == len)
                        return keys;
                    else
                        return CopyBytes(keys, length);
                }
                else
                    return CopyPadBytes(keys, padBytes, length);
            }
            return CopyBytes(padBytes, length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceByte"></param>
        /// <param name="padBytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        static byte[] CopyPadBytes(byte[] sourceByte, byte[] padBytes, int length)
        {
            byte[] resultByte = new byte[length];

            for (int i = 0; i < length; i++)
            {
                if (i == length)
                {
                    for (int j = length; j < sourceByte.Length; j++)
                        resultByte[j] = padBytes[j];
                    break;
                }
                else
                    resultByte[i] = padBytes[i];
            }
            return resultByte;
        }

        /// <summary>
        /// 截取一个byte[]数组指定个数到另一个byte[]数组,要求截取个数小于原数组个数
        /// </summary>
        /// <param name="sourceByte">原byte[]数组</param>
        /// <param name="len">截取个数</param>
        /// <returns></returns>
        static byte[] CopyBytes(byte[] sourceByte, int length)
        {
            byte[] resultByte = new byte[length];
            for (int i = 0; i < length; i++)
                resultByte[i] = sourceByte[i];
            // Array.Copy(sourceByte, 0, resultByte, 0, length);
            return resultByte;
        }        
        #endregion        

        #region 密钥和初始向量
        /// <summary>
        /// 默认字节密钥
        /// </summary>
        internal static readonly byte[] Keys = { 0x10, 0x13, 0x29, 0x33, 0x48, 0x58, 0x64, 0x73, 0x85, 0x9A, 0xB0, 0xC6, 0xD3, 0xE9, 0xFB, 0x25, 0x22, 0x39, 0x45, 0x55, 0x66, 0x72, 0x88, 0x99, 0xA5, 0xBA, 0xC6, 0xD7, 0xEF, 0xFF, 0x96, 0x8F };

        /// <summary>
        /// 初始化密钥向量
        /// </summary>
        internal static readonly byte[] IV = { 0x78, 0x9A, 0x3F, 0x52, 0x42, 0x5C, 0x67, 0x7F, 0x89, 0x9D, 0x0A, 0xAF, 0xBC, 0xCD, 0xD5, 0xEF };

        #endregion

        #region Hash,Encrypt 枚举

        public enum HashName
        {
            CRC32, MD5, SHA1, SHA256, SHA384, SHA512
        }

        /// <summary>
        /// 对称加密算法
        /// </summary>
        public enum Algorithm
        {
            AES, Rijndael, TripleDES, DES, RC2
        }
        #endregion

        #region 公共方法

        /// <summary>
        /// Byte字节数组转换为十六进制的字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHex(this byte[] bytes, bool upper = false)
        {
            string format = upper ? "{0:X2}" : "{0:x2}";
            var s = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                s.AppendFormat(format, bytes[i]);
            return s.ToString();
        }

        /// <summary>
        /// 将16进制字符串转化为字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Hex(this string value)
        {
            if (!value.IsHex()) return null;

            int len = value.Length / 2;
            byte[] ret = new byte[len];
            for (int i = 0; i < len; i++)
            {
                //int k;
                //System.Globalization.CultureInfo provider = new System.Globalization.CultureInfo("zh-cn");
                //if (Int32.TryParse(value.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier, provider, out k))
                //    ret[i] = (byte)k;
                ret[i] = (byte)(Convert.ToInt32(value.Substring(i * 2, 2), 16));
            }
            return ret;
        }
        #endregion
    }
}