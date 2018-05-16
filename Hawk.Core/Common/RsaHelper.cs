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
            return rsa.VerifyData(buf, SHA1.Create(), signature);
        }

        public static bool VerifyHash(byte[] buf, byte[] signature, string pubKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(pubKey);
            return rsa.VerifyHash(buf, "", signature);
        }

        /// <summary>
        /// 解析PKCS8格式(java适用)密钥为.net RSA对象
        /// </summary>
        /// <param name="pkcs8"></param>
        /// <returns></returns>
        public static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
        {

            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];

            MemoryStream mem = new MemoryStream(pkcs8);
            int lenstream = (int)mem.Length;
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;


                bt = binr.ReadByte();
                if (bt != 0x02)
                    return null;

                twobytes = binr.ReadUInt16();

                if (twobytes != 0x0001)
                    return null;

                seq = binr.ReadBytes(15);		//read the Sequence OID
                if (!CompareBytearrays(seq, SeqOID))	//make sure Sequence for OID is correct
                    return null;

                bt = binr.ReadByte();
                if (bt != 0x04)	//expect an Octet string 
                    return null;

                bt = binr.ReadByte();		//read next byte, or next 2 bytes is  0x81 or 0x82; otherwise bt is the byte count
                if (bt == 0x81)
                    binr.ReadByte();
                else
                    if (bt == 0x82)
                    binr.ReadUInt16();
                //------ at this stage, the remaining sequence should be the RSA private key

                byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));

                RSACryptoServiceProvider rsacsp = DecodeRSAPrivateKey(rsaprivkey);

                return rsacsp;
            }

            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }
        }


        /// <summary>
        ///  解析PKCS1格式密钥为.net RSA对象
        /// </summary>
        /// <param name="priKey"></param>
        /// <param name="bitLen">密钥长度</param>
        /// <returns></returns>
        public static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] priKey, int bitLen=1024)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            //var strKey = Convert.FromBase64String(privkey);

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(priKey);
            BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------ all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);


                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                CspParameters CspParameters = new CspParameters();
                CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;

                //int bitLen = 1024;
                //if ("RSA2".Equals(signType))
                //{
                //    bitLen = 2048;
                //}

                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(bitLen, CspParameters);
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch //(Exception ex)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }
         static bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)		//expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();	// data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }
            while (binr.ReadByte() == 0x00)
            {	//remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);		//last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }
    }
}
