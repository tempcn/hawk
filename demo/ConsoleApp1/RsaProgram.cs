using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hawk;
using Hawk.Common;
using System.Security.Cryptography;
using System.IO;

using Org.BouncyCastle;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Utilities.Encoders;

namespace ConsoleApp1
{
    class RsaProgram
    {
        static void Main()
        {
            //RSA密钥对的构造器  
            RsaKeyPairGenerator pGen = new RsaKeyPairGenerator();

            //RSA密钥构造器的参数  
            RsaKeyGenerationParameters param = new RsaKeyGenerationParameters(
               BigInteger.ValueOf(0x11),
                new SecureRandom(),
                1024,   //密钥长度  
                100);
            //用参数初始化密钥构造器  
            pGen.Init(param);

           // RsaKeyParameters pubKey = null;

            //产生密钥对  
            AsymmetricCipherKeyPair keyPair = pGen.GenerateKeyPair();
            //获取公钥和密钥  
            // AsymmetricKeyParameter publicKey = keyPair.Public;
            //AsymmetricKeyParameter privateKey = keyPair.Private;
            RsaKeyParameters publicKey = (RsaKeyParameters)keyPair.Public;
            RsaKeyParameters privateKey = (RsaKeyParameters)keyPair.Private;

            if (publicKey.Modulus.BitLength < 1024)
            {
                Console.WriteLine("failed key generation (1024) length test");
            }

            //一个测试…………………… 
            //输入，十六进制的字符串，解码为byte[] 
            //string input = "4e6f77206973207468652074696d6520666f7220616c6c20676f6f64206d656e"; 
            //byte[] testData = Org.BouncyCastle.Utilities.Encoders.Hex.Decode(input);   
            
      
            string input = "我是RSA测试";
            byte[] testData = Encoding.UTF8.GetBytes(input);
            
            Console.WriteLine("明文:" + input);

            IAsymmetricBlockCipher engine = new RsaEngine();
            //公钥加密
            engine.Init(true, publicKey);
            //engine.Init(true, privateKey);
            try
            {
                testData = engine.ProcessBlock(testData, 0, testData.Length);
                Console.WriteLine("密文（base64编码）:" + Convert.ToBase64String(testData) + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed - exception " + Environment.NewLine + ex.ToString());
            }

            //私钥解密 
            engine.Init(false, privateKey);
           // engine.Init(false, publicKey);
            try
            {
                testData = engine.ProcessBlock(testData, 0, testData.Length);

            }
            catch (Exception e)
            {
                Console.WriteLine("failed - exception " + e.ToString());
            }
            if (input.Equals(Encoding.UTF8.GetString(testData)))
            {
                Console.WriteLine("解密成功");
            }else
            {
                Console.WriteLine("结果不同");
            }
            Console.Read();
        }

        static void Main2(string[] args)
        {
            var s = "中华人民共和国";

            var signType = "RSA";

            var res = "MIICXQIBAAKBgQDxtuDEwnLNHXailcEc9jqx+5iQx7C4ZBpzES18Yg4BU/5Pl6jUGHhTx265tKupz42JmYX8lJRtCx6gePKYU2JTKFe+v3ZUVLOjZWFaPsputqZZHOwhfYKpTm0WQZyNbzQe/pVKRtT4P+5oNBXyABKSROg1lNee//cPPtRUORjObQIDAQABAoGAJpUXabDUHFOQpUEcMyBGnDRZ1PpbBgPMiQN77DfGnoWmuVOu+jPxuQXDcdcZ86ASqp0b2wZobsNwnxLPPmtI7Ue/2RbwEQImXfD7itWgtEJlufW9tlX7Klw05sLtLkpIU5ymRFYcX64UIlKdAHLMHmdJjNOnkt5V6K92F9GWi0ECQQD6lbpxhEeiq9y6iTXGUgBKJMOgrJp9Jq/kSC0DmW4EgLPql9QJTD+H6oe5Fy8xl/9NOpWDOZdYj619avF3WbXxAkEA9vATp1AYoeTtB9mXPUjJlDluoxfOwqdKTzf/+xVGiTPAUGn+r6dEfa3GbJLD42NrNxxCe8QxdD0YD8zn6nu0PQJBAIklu8Z3bLGmuIdLo6fop4ns9zkQXvmSXABoVGK87c7/FfmWoZF5LuhXv3LZMpZFJ5EAOGZ69c+dy4lyJ7h33DECQQDtAmR6tB/QU19Fp4zHn3MKt0z/cLxcjCCAhGlG3pbC3U76X6G5ijvsvLu0PfGR8DxZut/81sP4oyLTF4KIxo6pAkA4NUZJ1q6BEX2ibfDBe+24lqm8CxSSpEJOMiceeiYOtHEwtOIlze/2ZJ1q6JRPJUcNdy1NXlMoaYHEK+e24DKy";
            RSACryptoServiceProvider rsa = DecodeRSAPrivateKey(res, signType);

            byte[] cipherbytes;
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(s), false);

            var en2 = Convert.ToBase64String(cipherbytes);
            Console.WriteLine(en2);

            cipherbytes = rsa.Decrypt(Convert.FromBase64String(en2), false);

            Console.WriteLine("=======================");
            Console.WriteLine(Encoding.UTF8.GetString(cipherbytes));

            RSAParameters sp = rsa.ExportParameters(true);

            var p1 = rsa.ToXmlString(true);
            // var p2 = rsa.ToXmlString(false);

            Console.WriteLine(p1);
            // Console.WriteLine(p1);

            Console.WriteLine("=======================");
            rsa = new RSACryptoServiceProvider();

            var pp = rsa.ToXmlString(true);
            Console.WriteLine(pp);

        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="publickey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSAEncrypt(string publickey, string content)
        {
            publickey = @"<RSAKeyValue>
<Modulus>5m9m14XH3oqLJ8bNGw9e4rGpXpcktv9MSkHSVFVMjHbfv+SJ5v0ubqQxa5YjLN4vc49z7SVju8s0X4gZ6AzZTn06jzWOgyPRV54Q4I0DCYadWW4Ze3e+BOtwgVU1Og3qHKn8vygoj40J6U85Z/PTJu3hN1m75Zr195ju7g9v4Hk=</Modulus>
<Exponent>AQAB</Exponent>
</RSAKeyValue>";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publickey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);


            //  RSAParameters pa2 = rsa.ExportParameters(false);           

            return Convert.ToBase64String(cipherbytes);
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="privatekey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RSADecrypt(string privatekey, string content)
        {
            privatekey = @"<RSAKeyValue>
<Modulus>5m9m14XH3oqLJ8bNGw9e4rGpXpcktv9MSkHSVFVMjHbfv+SJ5v0ubqQxa5YjLN4vc49z7SVju8s0X4gZ6AzZTn06jzWOgyPRV54Q4I0DCYadWW4Ze3e+BOtwgVU1Og3qHKn8vygoj40J6U85Z/PTJu3hN1m75Zr195ju7g9v4Hk=</Modulus><Exponent>AQAB</Exponent><P>/hf2dnK7rNfl3lbqghWcpFdu778hUpIEBixCDL5WiBtpkZdpSw90aERmHJYaW2RGvGRi6zSftLh00KHsPcNUMw==</P><Q>6Cn/jOLrPapDTEp1Fkq+uz++1Do0eeX7HYqi9rY29CqShzCeI7LEYOoSwYuAJ3xA/DuCdQENPSoJ9KFbO4Wsow==</Q><DP>ga1rHIJro8e/yhxjrKYo/nqc5ICQGhrpMNlPkD9n3CjZVPOISkWF7FzUHEzDANeJfkZhcZa21z24aG3rKo5Qnw==</DP><DQ>MNGsCB8rYlMsRZ2ek2pyQwO7h/sZT8y5ilO9wu08Dwnot/7UMiOEQfDWstY3w5XQQHnvC9WFyCfP4h4QBissyw==</DQ><InverseQ>EG02S7SADhH1EVT9DD0Z62Y0uY7gIYvxX/uq+IzKSCwB8M2G7Qv9xgZQaQlLpCaeKbux3Y59hHM+KpamGL19Kg==</InverseQ><D>vmaYHEbPAgOJvaEXQl+t8DQKFT1fudEysTy31LTyXjGu6XiltXXHUuZaa2IPyHgBz0Nd7znwsW/S44iql0Fen1kzKioEL3svANui63O3o5xdDeExVM6zOf1wUUh/oldovPweChyoAdMtUzgvCbJk1sYDJf++Nr0FeNW1RB1XG30=</D></RSAKeyValue>";
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privatekey);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(content), false);

            // RSAParameters pa = rsa.ExportParameters(true);

            return Encoding.UTF8.GetString(cipherbytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="privkey"></param>
        /// <param name="signType">RSA(1024),RSA2(2048)</param>
        /// <returns></returns>
        private static RSACryptoServiceProvider DecodeRSAPrivateKey(string privkey, string signType)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            var strKey = Convert.FromBase64String(privkey);

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(strKey);
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

                int bitLen = 1024;
                if ("RSA2".Equals(signType))
                {
                    bitLen = 2048;
                }

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
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)     //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte
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
            {   //remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);       //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }
    }
}
