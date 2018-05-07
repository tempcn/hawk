using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Hawk.Common
{
    public static class Safety
    {
        public static string HashCrc32(this string s, Encoding encode = null, int length = 1048576, bool isFile = false, bool upper = false)
        {
            HashAlgorithm crc32 = new Crc32();

            // Crc32 crc32 = new Crc32();
            string result = string.Empty;
            byte[] hashByte = null;
            if (isFile)
            {
                if (File.Exists(s))
                {
                    using (FileStream stream = new FileStream(s, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        byte[] bufferSize = new byte[length];
                        long offset = 0;
                        while (offset < stream.Length)
                        {
                            int readSize = length;
                            if (offset + readSize > stream.Length)
                                readSize = (int)(stream.Length - offset);
                            stream.Read(bufferSize, 0, readSize);
                            if (offset + readSize < stream.Length) // 不是最后一块                 
                                crc32.TransformBlock(bufferSize, 0, readSize, bufferSize, 0);
                            else // 最后一块
                                crc32.TransformFinalBlock(bufferSize, 0, readSize);
                            offset += length;

                            //  backgroundWorkerHash.ReportProgress((int)(offset * 100 / fs.Length));
                        }
                        return ByteToString(crc32.Hash, upper);
                    }
                }
            }
            else
            {
                byte[] data = (encode != null ? encode : Encoding.UTF8).GetBytes(s);
                hashByte = crc32.ComputeHash(data);
            }
            if (hashByte != null)
                result = ByteToString(hashByte, upper);

            return result;
        }

        #region Hash散列

        public static string Hash(this string s, HashName hashName = HashName.MD5, Encoding encode = null, bool upper = false)
        {
            char[] c = s.ToCharArray(0, s.Length);
            byte[] inputBuffer = (encode != null ? encode : Encoding.UTF8).GetBytes(c, 0, c.Length);
            byte[] hashBytes = HashAlgorithm.Create(Enum.GetName(typeof(HashName), hashName)).ComputeHash(inputBuffer, 0, inputBuffer.Length);
            return ByteToString(hashBytes, upper);
            // return BitConverter.ToString(hashBytes, 0, hashBytes.Length).Replace("-", "");            
        }

        //public static string Hash(string s, HashName hashName = HashName.MD5, bool upper = false)
        //{
        //    return Hash(s, Encoding.UTF8, hashName, upper);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        /// <param name="hashName"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static string HashFile(this Stream stream, int length = 1048576, HashName hashName = HashName.MD5, bool upper = false)
        {
            if (stream != null)
            {
                //    byte[] hashBytes = HashAlgorithm.Create(Enum.GetName(typeof(HashName), hashName)).ComputeHash(stream);
                //    return ByteToString(hashBytes, upper);           

                HashAlgorithm hash = HashAlgorithm.Create(Enum.GetName(typeof(HashName), hashName));
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
                return ByteToString(hash.Hash, upper);
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">文件的全路径文件名</param>
        /// <param name="length"></param>
        /// <param name="hashName"></param>
        /// <returns></returns>
        public static string HashFile(this string fileName, int length = 1048576, HashName hashName = HashName.MD5, bool upper = false)
        {
            if (File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    return HashFile(fs, length, hashName, upper);
            }
            return string.Empty;
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

            return ByteToString(resultBytes, upper);
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
            string result = string.Empty;

            byte[] sourceBytes = HexToBytes(s);
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

        public static int[] ValidKeyLength(Algorithm algName)
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

        #region Obsolete

        ///// <summary>
        ///// 检查密钥的长度.如果大于指定的长度,截取为相等的长度;
        ///// 等于指定的长度,返回;
        ///// 如果小于指定的长度,用默认密钥的字节补充.
        ///// </summary>
        ///// <param name="byteArray">要检查的数组</param>
        ///// <param name="len">要检查数组指定的长度</param>
        ///// <returns>返回一个与指定数组长度相等的数组</returns>
        //[Obsolete]
        //static byte[] CheckByteLength(byte[] byteArray, int len)
        //{
        //    if (byteArray != null && byteArray.Length > 0)
        //    {
        //        int length = byteArray.Length;
        //        if (length >= len)
        //        {
        //            if (length == len)
        //                return byteArray;
        //            else
        //                return CopyBytes(byteArray, len);

        //            //byte[] resultByte = new byte[len];
        //            //Array.Copy(byteArray, 0, resultByte, 0, len);
        //            //return resultByte;                    
        //        }
        //        else
        //        {
        //            byte[] resultByte2 = new byte[len];
        //            for (int j = 0; j < len; j++)
        //                if (j == length)
        //                {
        //                    for (int k = length; k < len; k++)
        //                        resultByte2[k] = Keys[k];
        //                    break;
        //                }
        //                else
        //                    resultByte2[j] = byteArray[j];
        //            return resultByte2;
        //        }
        //    }
        //    else
        //    {
        //        if (len >= Keys.Length)
        //            return Keys;
        //        if (len >= IV.Length)
        //            return IV;
        //        return CopyBytes(Keys, len);
        //    }
        //}
        #endregion
        #endregion

        #region 实现Base64编码转换
        /// <summary>
        /// 字节数组加密为Base64编码的字符串
        /// </summary>
        /// <param name="inByte"></param>
        /// <returns></returns>
        public static string BytesToBase64(byte[] inByte)
        {
            try
            {
                return Convert.ToBase64String(inByte, 0, inByte.Length, Base64FormattingOptions.None);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 将字符串转换成Base64编码的字符串
        /// </summary>
        /// <param name="s">要转换的字符串</param>
        /// <param name="encode">编码</param>
        /// <returns></returns>
        public static string StringToBase64(string s, Encoding encode)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                byte[] b = encode.GetBytes(s);
                return BytesToBase64(b);
            }
            return string.Empty;
        }

        public static string StringToBase64(string s)
        {
            return StringToBase64(s, Encoding.UTF8);
        }

        /// <summary>
        /// 将Base64编码的字符串转换为字节数组
        /// </summary>
        /// <param name="s">要转换的Base64编码的字符串</param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static byte[] Base64ToBytes(string s)
        {
            byte[] resultByte = null;// encoding.GetBytes("字符串格式无效");
            if ((!string.IsNullOrWhiteSpace(s)) && s.Length % 4 == 0 && CheckBase64(s))
            {
                //               try
                //                {
                resultByte = Convert.FromBase64String(s);
                //                }
                //#if DEBUG
                //                catch (Exception ex)
                //                {
                //                    resultByte = Encoding.UTF8.GetBytes(ex.Message);
                //                }
                //#else
                //                catch { }
                //#endif
            }
            return resultByte;
        }

        /// <summary>
        /// 将Base64编码的字符串转换成其它编码的字符串
        /// </summary>
        /// <param name="s">要转换的Base64编码的字符串</param>
        /// <param name="encode">编码,要与Base64编码格式一致</param>
        /// <returns></returns>
        public static string Base64ToString(string s, Encoding encode)
        {
            byte[] outByte = Base64ToBytes(s);

            if (outByte != null)
                return encode.GetString(outByte, 0, outByte.Length);
            return "字符串格式无效";//string.Empty;
        }

        public static string Base64ToString(string s)
        {
            return Base64ToString(s, Encoding.UTF8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckBase64(string str)
        {
            string pattern = "^[a-zA-Z0-9+/]+[=]{0,2}$";
            //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.None);
            //return regex.IsMatch(str, 0);
            return Regex.IsMatch(str, pattern, RegexOptions.None);
        }
        #endregion

        #region 密钥和初始向量
        /// <summary>
        /// 默认字节密钥
        /// </summary>
        public static readonly byte[] Keys = { 0x10, 0x13, 0x29, 0x33, 0x48, 0x58, 0x64, 0x73, 0x85, 0x9A, 0xB0, 0xC6, 0xD3, 0xE9, 0xFB, 0x25, 0x22, 0x39, 0x45, 0x55, 0x66, 0x72, 0x88, 0x99, 0xA5, 0xBA, 0xC6, 0xD7, 0xEF, 0xFF, 0x96, 0x8F };

        /// <summary>
        /// 初始化密钥向量
        /// </summary>
        public static readonly byte[] IV = { 0x78, 0x9A, 0x3F, 0x52, 0x42, 0x5C, 0x67, 0x7F, 0x89, 0x9D, 0x0A, 0xAF, 0xBC, 0xCD, 0xD5, 0xEF };

        #endregion

        #region Hash,Encrypt 枚举

        public enum HashName
        {
            MD5, SHA1, SHA256, SHA384, SHA512
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
        public static string ByteToString(byte[] bytes, bool upper = false)
        {
            string format = upper ? "{0:X2}" : "{0:x2}";
            StringBuilder str = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                str.AppendFormat(format, bytes[i]);
            return str.ToString();
        }

        /// <summary>
        /// 将16进制字符串转化为字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] HexToBytes(string value)
        {
            if (Regular.IsHex(value))
            {
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
            return null;
        }

        ///// <summary>
        ///// 判断字符串是否符合十六进数制格式
        ///// </summary>
        ///// <param name="s"></param>
        ///// <returns></returns>
        //public static bool CheckHex(string s)
        //{
        //    string pattern = "^[a-fA-F0-9]+$";
        //    //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.None);
        //    //return regex.IsMatch(str, 0);
        //    return Regex.IsMatch(s, pattern, RegexOptions.None);
        //}
        #endregion
    }
}

//using System;
//using System.Security.Cryptography;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.IO;

//namespace Hawk.Common
//{
//    /// <summary>
//    ///加密解密类
//    /// </summary>
//    public class Safety
//    {
//        /// <summary>
//        /// 默认字节密钥
//        /// </summary>
//        static readonly byte[] Keys = { 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xA0, 0xB0, 0xC0, 0xD0, 0xE0, 0xF0, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x41, 0x61 };

//        /// <summary>
//        /// 初始化密钥向量
//        /// </summary>
//        static readonly byte[] IV = { 0x01, 0x12, 0x23, 0x34, 0x45, 0x56, 0x67, 0x78, 0x89, 0x90, 0x0A, 0xAB, 0xBC, 0xCD, 0xDE, 0xEF };

//        static readonly string[] _hashName = { "SHA", "SHA1", "System.Security.Cryptography.SHA1", "System.Security.Cryptography.HashAlgorithm", "MD5", "System.Security.Cryptography.MD5", "SHA256", "SHA-256", "System.Security.Cryptography.SHA256", "SHA384", "SHA-384", "System.Security.Cryptography.SHA384", "SHA512", "SHA-512", "System.Security.Cryptography.SHA512" };

//        /// <summary>
//        /// System.Security.Cryptography.HashAlgorithm
//        /// 类的指定实现的实例名
//        /// </summary>
//        public static string[] HashName
//        {
//            get { return _hashName; }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="hashName"></param>
//        /// <returns></returns>
//        static bool ExistsHashName(string hashName)
//        {
//            bool exists = false;
//            for (int i = 0; i < HashName.Length; i++)
//                if (string.Compare(hashName, HashName[i], true) == 0)
//                {
//                    exists = true; break;
//                }
//            return exists;
//        }

//        /// <summary>
//        /// Byte字节数组转换为十六进制的字符串
//        /// </summary>
//        /// <param name="bytes"></param>
//        /// <returns></returns>
//        public static string BytesToStr(byte[] bytes)
//        {
//            StringBuilder str = new StringBuilder(bytes.Length * 2);
//            for (int i = 0; i < bytes.Length; i++)
//                str.AppendFormat("{0:x2}", bytes[i]);
//            return str.ToString();
//        }

//        /// <summary>
//        /// 将16进制字符串转化为字节数组
//        /// </summary>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public static byte[] HexToBytes(string value)
//        {
//            if (CheckHex(value))
//            {
//                int len = value.Length / 2;
//                byte[] ret = new byte[len];
//                for (int i = 0; i < len; i++)
//                {
//                    //int k;
//                    //System.Globalization.CultureInfo provider = new System.Globalization.CultureInfo("zh-cn");
//                    //if (Int32.TryParse(value.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier, provider, out k))
//                    //    ret[i] = (byte)k;

//                    ret[i] = (byte)(Convert.ToInt32(value.Substring(i * 2, 2), 16));
//                }
//                return ret;
//            }
//            return null;
//        }

//        #region Hash散列
//        /// <summary>
//        /// 使用当前系统ANSI代码页的编码对字符串哈希
//        /// </summary>
//        /// <param name="message">加密字符串</param>
//        /// <returns>返回MD5</returns>
//        public static string Hash(string message)
//        {
//            return Hash(HashName[4], message, Encoding.Default);
//        }

//        /// <summary>
//        /// 使用当前系统ANSI代码页的编码对字符串哈希
//        /// </summary>
//        /// <param name="algorithm">散列方式MD5, SHA1, SHA256, SHA384, SHA512</param>
//        /// <param name="message">加密字符串</param>
//        /// <returns></returns>
//        public static string Hash(string algorithm, string message)
//        {
//            return Hash(algorithm, message, Encoding.Default);
//        }

//        /// <summary>
//        /// 使用指定的散列方式和字符编码对字符串哈希
//        /// </summary>
//        /// <param name="algorithm">散列方式MD5, SHA1, SHA256, SHA384, SHA512</param>
//        /// <param name="message">加密字符串</param>
//        /// <returns></returns>
//        public static string Hash(string algorithm, string message, Encoding encoding)
//        {
//            string result = "";
//            if (ExistsHashName(algorithm))
//            {
//                byte[] data = encoding.GetBytes(message);
//                byte[] hashBytes = HashAlgorithm.Create(algorithm).ComputeHash(data);
//                result = BytesToStr(hashBytes);
//            } return result;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="str"></param>
//        /// <param name="isStr"></param>
//        /// <param name="encoding"></param>
//        /// <returns></returns>
//        public static string HashCrc32(string str, bool isStr, Encoding encoding)
//        {
//            Crc32 crc32 = new Crc32();
//            string result = string.Empty;
//            byte[] hashByte = null;
//            if (isStr)
//            {
//                byte[] data = encoding.GetBytes(str);
//                hashByte = crc32.ComputeHash(data);
//            }
//            else if (File.Exists(str))
//            {
//                using (FileStream fs = new FileStream(str, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
//                {
//                    hashByte = crc32.ComputeHash(fs);
//                }
//            }
//            if (hashByte != null)
//            {
//                result = BytesToStr(hashByte);
//            }
//            return result;
//        }

//        ///// <summary>
//        ///// 计算指定字符串的MD5哈希值
//        ///// </summary>
//        ///// <param name="message">要进行哈希计算的字符串</param>
//        ///// <returns></returns>
//        //public static string HashMD5(string message)
//        //{
//        //    return HashMD5(message, Encoding.Default);
//        //}

//        ///// <summary>
//        ///// 计算指定字符串的MD5哈希值
//        ///// </summary>
//        ///// <param name="message">要进行哈希计算的字符串</param>
//        ///// <param name="encoding">选择加密的编码</param>
//        ///// <returns></returns>
//        //public static string HashMD5(string message, Encoding encoding)
//        //{
//        //    if (message.Length > 0)
//        //    {
//        //        MD5 md5 = MD5.Create();
//        //        byte[] source = encoding.GetBytes(message);
//        //        byte[] encrypt = md5.ComputeHash(source);
//        //        return BitConverter.ToString(encrypt).Replace("-", "");
//        //    }
//        //    else
//        //    {
//        //        return "";
//        //    }
//        //    //StringBuilder result = new StringBuilder();
//        //    //if (message.Length > 0)
//        //    //{
//        //    //    MD5 md5 = MD5.Create();
//        //    //    byte[] source = encoding.GetBytes(message);
//        //    //    byte[] encrypt = md5.ComputeHash(source);
//        //    //    for (int i = 0; i < encrypt.Length; i++)
//        //    //        result.Append(encrypt[i].ToString("X2"));
//        //    //}
//        //    //return result.ToString();
//        //}
//        /// <summary>
//        /// 计算文件的MD5哈希值
//        /// </summary>
//        /// <param name="Stream">文件流</param>
//        /// <returns></returns>
//        public static string HashFile(Stream stream)
//        {
//            string result = "";
//            if (ExistsHashName(HashName[4]))
//            {
//                byte[] hashBytes = HashAlgorithm.Create(HashName[4]).ComputeHash(stream);
//                result = BytesToStr(hashBytes);
//            }
//            return result;
//        }

//        /// <summary>
//        /// 计算文件的哈希值
//        /// </summary>
//        /// <param name="algorithm">加密方式MD5, SHA1, SHA256, SHA384, SHA512</param>
//        /// <param name="Stream">文件流</param>
//        /// <returns></returns>
//        public static string HashFile(string algorithm, Stream stream)
//        {
//            string result = "";
//            if (ExistsHashName(algorithm))
//            {
//                byte[] hashBytes = HashAlgorithm.Create(algorithm).ComputeHash(stream);
//                result = BytesToStr(hashBytes);
//            }
//            return result;
//        }
//        /// <summary>
//        /// 计算文件的MD5哈希值
//        /// </summary>
//        /// <param name="fileName">文件的全路径文件名</param>
//        /// <returns></returns>
//        public static string HashFile(string fileName)
//        {
//            return HashFile(HashName[4], fileName);
//        }
//        /// <summary>
//        /// 计算文件的哈希值
//        /// </summary>
//        /// <param name="algorithm">加密方式MD5, SHA1, SHA256, SHA384, SHA512</param>
//        /// <param name="fileName">文件的全路径文件名</param>
//        /// <returns></returns>
//        public static string HashFile(string algorithm, string fileName)
//        {
//            string result = "";
//            if (File.Exists(fileName))
//                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
//                    result = HashFile(algorithm, fs);
//            return result;
//        }
//        #endregion

//        #region 对称加密
//        /// <summary>
//        /// 对称加密算法
//        /// </summary>
//        enum AlgName
//        {
//            /// <summary>
//            /// Rijndael算法
//            /// </summary>
//            Rijndael,
//            /// <summary>
//            /// 
//            /// </summary>
//            AES,
//            /// <summary>
//            /// 
//            /// </summary>
//            DES,
//            /// <summary>
//            /// 
//            /// </summary>
//            TripleDES,
//            /// <summary>
//            /// 
//            /// </summary>
//            RC2
//        }
//        /* 
//         * algName: 
//         * Rijndael     - RijndaelManaged 
//         * AES          -   AesManaged
//         * DES          - DESCryptoServiceProvider 
//         * RC2          - RC2CryptoServiceProvider 
//         * TripleDES    - TripleDESCryptoServiceProvider 
//         */
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="algName"></param>
//        /// <param name="index"></param>
//        /// <returns></returns>
//        static bool ExistsAlgName(string algName, out int index)
//        {
//            index = 0; bool existsName = false;
//            string[] strAlgName = Enum.GetNames(typeof(AlgName));//{ "Rijndael", "DES", "TripleDES", "RC2",  };
//            for (int i = 0; i < strAlgName.Length; i++)
//            {
//                if (string.Compare(algName, strAlgName[i], true) == 0)
//                {
//                    index = i; existsName = true; break;
//                }
//            }
//            return existsName;
//        }

//        #region 加密
//        /// <summary>
//        /// 用指定的密钥和初始化向量加密字节数组.
//        /// </summary>
//        /// <param name="sourceBytes">要加密的字节数组</param>
//        /// <param name="rgbKey">密钥。AES：32，Rijndael：32，DES：8，TripleDES：24，RC2：16</param>
//        /// <param name="rgbIV">初始化向量。AES：16，Rijndael：16，DES：8，TripleDES：8，RC2：8</param>
//        /// <param name="algName">对称加密算法Rijndael,DES,TripleDES,RC2</param>
//        /// <returns></returns>
//        public static byte[] EncryptBytes(byte[] sourceBytes, byte[] rgbKey, byte[] rgbIV, string algName)
//        {
//            byte[] buffer = null;
//            if (sourceBytes != null)
//            {
//                int index = 0; bool existsName = ExistsAlgName(algName, out index);
//                if (existsName)
//                {
//                    SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create(algName);
//                    ICryptoTransform iCryptoTransform;
//                    switch (index)
//                    {
//                        #region 加密实现
//                        case (int)AlgName.AES:
//                            {
//                                //AES密钥长度与DES密钥长度不同
//                                rgbKey = CheckByteLength(rgbKey, 32);
//                                rgbIV = CheckByteLength(rgbIV, 16);
//                                AesManaged aes = new AesManaged();//net framework4.0支持
//                                //aes.Mode = CipherMode.CBC;
//                                //aes.Padding = PaddingMode.Zeros;
//                                iCryptoTransform = aes.CreateEncryptor(rgbKey, rgbIV);
//                            }
//                            break;
//                        case (int)AlgName.Rijndael:
//                            {
//                                //AES密钥长度与DES密钥长度不同
//                                //检查密钥数组长度是否是32位
//                                rgbKey = CheckByteLength(rgbKey, 32);
//                                rgbIV = CheckByteLength(rgbIV, 16);
//                                RijndaelManaged rijndael = new RijndaelManaged();
//                                iCryptoTransform = rijndael.CreateEncryptor(rgbKey, rgbIV);
//                            } break;
//                        case (int)AlgName.DES://DES
//                            {
//                                //检查密钥数组长度是否为8位
//                                rgbKey = CheckByteLength(rgbKey, 8);
//                                rgbIV = CheckByteLength(rgbIV, 8);
//                                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
//                                iCryptoTransform = des.CreateEncryptor(rgbKey, rgbIV);
//                            } break;
//                        case (int)AlgName.TripleDES:
//                            {
//                                rgbKey = CheckByteLength(rgbKey, 24);
//                                rgbIV = CheckByteLength(rgbIV, 8);
//                                TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider();
//                                iCryptoTransform = triple.CreateEncryptor(rgbKey, rgbIV);
//                            } break;
//                        case (int)AlgName.RC2:
//                            {
//                                rgbKey = CheckByteLength(rgbKey, 16);
//                                rgbIV = CheckByteLength(rgbIV, 8);

//                                RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();
//                                iCryptoTransform = rc2.CreateEncryptor(rgbKey, rgbIV);
//                            } break;
//                        default:
//                            iCryptoTransform = null;
//                            break;
//                        #endregion
//                    }
//                    if (iCryptoTransform != null)
//                    {
//                        ////实例化内存流MemoryStream
//                        //MemoryStream mStream = new MemoryStream();
//                        ////实例化CryptoStream
//                        //CryptoStream cStream = new CryptoStream(mStream, iCryptoTransform, CryptoStreamMode.Write);
//                        //cStream.Write(sourceBytes, 0, sourceBytes.Length);
//                        //cStream.FlushFinalBlock();
//                        ////将内存流转换成字节数组
//                        //buffer = mStream.ToArray();
//                        //mStream.Close();//关闭流
//                        //cStream.Close();//关闭流

//                        buffer = iCryptoTransform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length);
//                    }
//                }
//            }
//            return buffer;
//        }

//        /// <summary>
//        /// 用指定的密钥和初始化向量加密字符串.
//        /// </summary>
//        /// <param name="encryptString">加密的字符串</param>
//        /// <param name="rgbKey">密钥</param>
//        /// <param name="rgbIV">初始化向量</param>
//        /// <param name="algName">对称加密算法AES,Rijndael,DES,TripleDES,RC2</param>
//        /// <param name="encoding"></param>
//        /// <returns></returns>
//        public static string Encrypt(string encryptString, byte[] rgbKey, byte[] rgbIV, string algName, Encoding encoding)
//        {
//            string result = string.Empty;
//            if (encryptString.Length > 0)
//            {
//                char[] chars = encryptString.ToCharArray(0, encryptString.Length);

//                byte[] buffer = encoding.GetBytes(chars, 0, chars.Length); //Encoding.UTF8.GetBytes(encryptString);
//                byte[] resultByte = EncryptBytes(buffer, rgbKey, rgbIV, algName);
//                if (resultByte != null)
//                    result = BytesToStr(resultByte); //Convert.ToBase64String(resultByte, 0, resultByte.Length, Base64FormattingOptions.None);
//            } return result;
//        }

//        /// <summary>
//        /// 用指定的密钥和初始化向量加密字符串
//        /// </summary>
//        /// <param name="encryptString">加密的字符串</param>
//        /// <param name="key">加密密钥</param>
//        /// <param name="algName">对称加密算法AES,Rijndael,DES,TripleDES,RC2</param>
//        /// <param name="encoding"></param>
//        /// <returns></returns>
//        public static string Encrypt(string encryptString, string key, string algName, Encoding encoding)
//        {
//            byte[] rgbKey = null;
//            if (key.Length > 0)
//                rgbKey = encoding.GetBytes(key);
//            return Encrypt(encryptString, rgbKey, null, algName, encoding);
//        }

//        /// <summary>
//        /// 用指定的密钥和初始化向量以Rijndael加密字符串
//        /// </summary>
//        /// <param name="encryptString">加密的字符串</param>
//        /// <param name="key">加密密钥</param>
//        /// <returns>返回AES加密的字符串</returns>
//        public static string Encrypt(string encryptString, string key)
//        {
//            string algName = Enum.GetNames(typeof(AlgName))[(int)AlgName.AES];
//            return Encrypt(encryptString, key, algName, Encoding.UTF8);
//        }

//        /// <summary>
//        /// 用指定的密钥和初始化向量以Rijndael加密字符串
//        /// </summary>
//        /// <param name="encryptString">加密的字符串</param>
//        /// <returns>返回AES加密的字符串</returns>
//        public static string Encrypt(string encryptString)
//        {
//            return Encrypt(encryptString, string.Empty);
//        }
//        #endregion

//        #region 解密
//        /// <summary>
//        /// 用指定的密钥和初始化向量解密字节数组.
//        /// </summary>
//        /// <param name="sourceBytes">要解密的字节数组</param>
//        /// <param name="rgbKey">密钥</param>
//        /// <param name="rgbIV">初始化向量</param>
//        /// <param name="algName">对称加密算法AES,Rijndael,DES,TripleDES,RC2</param>
//        /// <returns></returns>
//        public static byte[] DecryptBytes(byte[] sourceBytes, byte[] rgbKey, byte[] rgbIV, string algName)
//        {
//            byte[] buffer = null;
//            if (sourceBytes != null)
//            {
//                int index; bool existsName = ExistsAlgName(algName, out index);
//                if (existsName)
//                {
//                    SymmetricAlgorithm symmetricAlgorithm = SymmetricAlgorithm.Create(algName);
//                    ICryptoTransform iCryptoTransform;
//                    switch (index)
//                    {
//                        #region 解密实现
//                        case (int)AlgName.AES:
//                            {
//                                //AES密钥长度与DES密钥长度不同
//                                //检查密钥数组长度是否是32位
//                                rgbKey = CheckByteLength(rgbKey, 32);
//                                rgbIV = CheckByteLength(rgbIV, 16);
//                                AesManaged aes = new AesManaged();



//                                //aes.Mode = CipherMode.CBC;
//                                //aes.Padding = PaddingMode.Zeros;
//                                iCryptoTransform = aes.CreateDecryptor(rgbKey, rgbIV);
//                            } break;
//                        case (int)AlgName.Rijndael:
//                            {
//                                //AES密钥长度与DES密钥长度不同
//                                //检查密钥数组长度是否是32位
//                                rgbKey = CheckByteLength(rgbKey, 32);
//                                rgbIV = CheckByteLength(rgbIV, 16);
//                                RijndaelManaged rijndael = new RijndaelManaged();
//                                iCryptoTransform = rijndael.CreateDecryptor(rgbKey, rgbIV);
//                            } break;
//                        case (int)AlgName.DES://DES 
//                            {
//                                //检查密钥数组长度是否为8位
//                                rgbKey = CheckByteLength(rgbKey, 8);
//                                rgbIV = CheckByteLength(rgbIV, 8);
//                                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
//                                iCryptoTransform = des.CreateDecryptor(rgbKey, rgbIV);
//                            } break;
//                        case (int)AlgName.TripleDES:
//                            {
//                                rgbKey = CheckByteLength(rgbKey, 24);
//                                rgbIV = CheckByteLength(rgbIV, 8);
//                                TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider();
//                                iCryptoTransform = triple.CreateDecryptor(rgbKey, rgbIV);
//                            } break;
//                        case (int)AlgName.RC2:
//                            {
//                                rgbKey = CheckByteLength(rgbKey, 16);
//                                rgbIV = CheckByteLength(rgbIV, 8);
//                                RC2CryptoServiceProvider rc2 = new RC2CryptoServiceProvider();
//                                iCryptoTransform = rc2.CreateDecryptor(rgbKey, rgbIV);
//                            } break;
//                        default:
//                            iCryptoTransform = null;
//                            break;
//                        #endregion
//                    }
//                    if (iCryptoTransform != null)
//                    {
//                        ////实例化内存流MemoryStream
//                        //MemoryStream mStream = new MemoryStream();
//                        ////实例化CryptoStream
//                        //CryptoStream cStream = new CryptoStream(mStream, iCryptoTransform, CryptoStreamMode.Write);
//                        //try
//                        //{
//                        //    cStream.Write(sourceBytes, 0, sourceBytes.Length);
//                        //    cStream.FlushFinalBlock();
//                        //    //将内存流转换成字节数组
//                        //    buffer = mStream.ToArray();
//                        //    cStream.Close();
//                        //}
//                        //catch (Exception ex)
//                        //{
//                        //    buffer = Encoding.UTF8.GetBytes(ex.Message);
//                        //}
//                        //mStream.Close();//关闭流  
//                        buffer = iCryptoTransform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length);
//                    }
//                }
//            }
//            return buffer;
//        }

//        /// <summary>
//        /// 用指定的密钥和初始化向量解密字符串.
//        /// </summary>
//        /// <param name="decryptString">解密的字符串</param>
//        /// <param name="rgbKey">密钥</param>
//        /// <param name="rgbIV">初始化向量</param>
//        /// <param name="algName">对称加密算法AES,Rijndael,DES,TripleDES,RC2</param>
//        /// <param name="encoding"></param>
//        /// <returns></returns>
//        public static string Decrypt(string decryptString, byte[] rgbKey, byte[] rgbIV, string algName, Encoding encoding)
//        {
//            string result = string.Empty;

//            byte[] buffer = HexToBytes(decryptString); //BytesFromBase64(decryptString, Encoding.UTF8); //StringToBase64(decryptString, Encoding.UTF8);
//            if (buffer != null)
//            {
//                byte[] resultByte = null;
//                try
//                {
//                    resultByte = DecryptBytes(buffer, rgbKey, rgbIV, algName);
//                    if (resultByte != null)
//                        result = encoding.GetStringData(resultByte, 0, resultByte.Length);
//                }
//                catch { }
//            }
//            return result;
//        }

//        /// <summary>
//        /// 用指定的密钥和初始化向量解密字符串
//        /// </summary>
//        /// <param name="decryptString">解密的字符串</param>
//        /// <param name="key">解密密钥</param>
//        /// <param name="rgbIV">算法的初始化向量</param>
//        /// <param name="algName">对称加密算法AES,Rijndael,DES,RC2,TripleDES</param>
//        /// <param name="encoding"></param>
//        /// <returns></returns>
//        public static string Decrypt(string decryptString, string key, string algName, Encoding encoding)
//        {
//            string result = string.Empty;
//            byte[] buffer = HexToBytes(decryptString); //BytesFromBase64(decryptString, Encoding.UTF8);
//            if (buffer != null)
//            {
//                byte[] rgbKey = null;
//                if (key.Length > 0)
//                    rgbKey = encoding.GetBytes(key);

//                result = Decrypt(decryptString, rgbKey, null, algName, encoding);
//            }
//            return result;
//        }

//        /// <summary>
//        /// 用指定的密钥和初始化向量以Rijndael解密字符串
//        /// </summary>
//        /// <param name="decryptString">解密的字符串</param>
//        /// <param name="key">解密密钥</param>
//        /// <returns>返回AES解密的字符串</returns>
//        public static string Decrypt(string decryptString, string key)
//        {
//            string algName = Enum.GetNames(typeof(AlgName))[(int)AlgName.AES];
//            return Decrypt(decryptString, key, algName, Encoding.UTF8);
//        }

//        /// <summary>
//        /// 用指定的密钥和初始化向量以Rijndael解密字符串
//        /// </summary>
//        /// <param name="decryptString">解密的字符串</param>
//        /// <returns>返回AES解密的字符串</returns>
//        public static string Decrypt(string decryptString)
//        {
//            return Decrypt(decryptString, string.Empty);
//        }
//        #endregion
//        #endregion

//        #region 加密算法KEY，IV验证
//        /// <summary>
//        /// 检查密钥的长度.如果大于指定的长度,截取为相等的长度;
//        /// 等于指定的长度,返回;
//        /// 如果小于指定的长度,用默认密钥的字节补充.
//        /// </summary>
//        /// <param name="byteArray">要检查的数组</param>
//        /// <param name="len">要检查数组指定的长度</param>
//        /// <returns>返回一个与指定数组长度相等的数组</returns>
//        static byte[] CheckByteLength(byte[] byteArray, int len)
//        {
//            if (byteArray != null && byteArray.Length > 0)
//            {
//                int length = byteArray.Length;
//                if (length >= len)
//                {
//                    if (length == len)
//                        return byteArray;
//                    else
//                        return CopyArray(byteArray, len);

//                    //byte[] resultByte = new byte[len];
//                    //Array.Copy(byteArray, 0, resultByte, 0, len);
//                    //return resultByte;                    
//                }
//                else
//                {
//                    byte[] resultByte2 = new byte[len];
//                    for (int j = 0; j < len; j++)
//                        if (j == length)
//                        {
//                            for (int k = length; k < len; k++)
//                                resultByte2[k] = Keys[k];
//                            break;
//                        }
//                        else
//                            resultByte2[j] = byteArray[j];
//                    return resultByte2;
//                }
//            }
//            else
//            {
//                if (len >= Keys.Length)
//                    return Keys;
//                if (len >= IV.Length)
//                    return IV;
//                return CopyArray(Keys, len);
//            }
//        }

//        /// <summary>
//        /// 截取一个byte[]数组指定个数到另一个byte[]数组,要求截取个数小于原数组个数
//        /// </summary>
//        /// <param name="sourceByte">原byte[]数组</param>
//        /// <param name="len">截取个数</param>
//        /// <returns></returns>
//        static byte[] CopyArray(byte[] sourceByte, int len)
//        {
//            byte[] resultByte = new byte[len];
//            for (int i = 0; i < len; i++)
//                resultByte[i] = sourceByte[i];

//            //Array.Copy(sourceByte, 0, resultByte, 0, len);

//            return resultByte;
//        }

//        /// <summary>
//        /// 判断字符串是否符合十六进数制格式
//        /// </summary>
//        /// <param name="source"></param>
//        /// <returns></returns>
//        static bool CheckHex(string source)
//        {
//            string pattern = "^[a-fA-F0-9]+$";
//            //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.None);
//            //return regex.IsMatch(str, 0);
//            return Regex.IsMatch(source, pattern, RegexOptions.None);
//        }

//        #endregion

//        #region 实现Base64编码转换
//        /// <summary>
//        /// 字节数组加密为Base64编码的字符串
//        /// </summary>
//        /// <param name="buffer"></param>
//        /// <returns></returns>
//        public static string BytesToBase64(byte[] buffer)
//        {
//            string result = string.Empty;
//            if (buffer.Length > 0)
//                result = Convert.ToBase64String(buffer, 0, buffer.Length, Base64FormattingOptions.None);
//            return result;

//        }
//        /// <summary>
//        /// 将字符串转换成Base64编码的字符串
//        /// </summary>
//        /// <param name="source">要转换的字符串</param>
//        /// <param name="encoding">编码</param>
//        /// <returns></returns>
//        public static string StringToBase64(string source, Encoding encoding)
//        {
//            byte[] buffer = null;
//            if (source != null && source.Length > 0)
//                buffer = encoding.GetBytes(source);
//            return BytesToBase64(buffer);
//        }
//        /// <summary>
//        /// 将字符串转换成Base64编码的字符串
//        /// </summary>
//        /// <param name="source">要转换的字符串</param>
//        /// <returns></returns>
//        public static string StringToBase64(string source)
//        {
//            return StringToBase64(source, Encoding.Default);
//        }
//        /// <summary>
//        /// 将Base64编码的字符串转换为字节数组
//        /// </summary>
//        /// <param name="str">要转换的Base64编码的字符串</param>
//        /// <param name="encoding"></param>
//        /// <returns></returns>
//        public static byte[] Base64ToBytes(string str, Encoding encoding)
//        {
//            byte[] resultByte = null;// encoding.GetBytes("字符串格式无效");
//            if (str.Length % 4 == 0 && CheckBase64(str))
//            {
//                try
//                {
//                    resultByte = Convert.FromBase64String(str);
//                }
//                catch (Exception ex)
//                {
//                    resultByte = encoding.GetBytes(ex.Message);
//                }
//            }
//            return resultByte;
//        }

//        /// <summary>
//        /// 将Base64编码的字符串转换为字节数组
//        /// </summary>
//        /// <param name="str">要转换的Base64编码的字符串</param>
//        /// <returns></returns>
//        public static byte[] Base64ToBytes(string str)
//        {
//            return Base64ToBytes(str, Encoding.Default);
//        }
//        /// <summary>
//        /// 将Base64编码的字符串转换成其它编码的字符串
//        /// </summary>
//        /// <param name="str">要转换的Base64编码的字符串</param>
//        /// <param name="encoding">编码,要与Base64编码格式一致</param>
//        /// <returns></returns>
//        public static string Base64ToString(string str, Encoding encoding)
//        {
//            string result = string.Empty;
//            byte[] buffer = Base64ToBytes(str, encoding);
//            if (buffer != null)
//                result = encoding.GetStringData(buffer);
//            return result;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="str"></param>
//        /// <returns></returns>
//        public static bool CheckBase64(string str)
//        {
//            string pattern = "^[a-zA-Z0-9+/]+[=]{0,2}$";
//            //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.None);
//            //return regex.IsMatch(str, 0);
//            return System.Text.RegularExpressions.Regex.IsMatch(str, pattern, System.Text.RegularExpressions.RegexOptions.None);
//        }
//        #endregion
//    }
//}