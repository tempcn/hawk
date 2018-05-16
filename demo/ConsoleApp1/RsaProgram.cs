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

using Bc_BigInteger = Org.BouncyCastle.Math.BigInteger;
using BigInt = System.Numerics.BigInteger;
using System.Xml;

namespace ConsoleApp1
{
    class RsaProgram
    {
        static void Main99()
        {
            string xmlPubKey = @"<RSAKeyValue><Modulus>0iQ8qYXhw4NOBPHNOjd10aOJ7ZC8yyhe8RYJlKrosfnShHdtZI5ch97QR/QbyUc6F6F7thhNiXaF
u/bNHX/j8mn4gEG6X4Mqe9Iv2cdc3kMj5lHy2++no753AFmeFLjgMbpKJNBGoADZBC/1DB72D87w
85iGJCyTHQGpVAMkAUk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            string xmlPriKey = @"<RSAKeyValue><Modulus>0iQ8qYXhw4NOBPHNOjd10aOJ7ZC8yyhe8RYJlKrosfnShHdtZI5ch97QR/QbyUc6F6F7thhNiXaF
u/bNHX/j8mn4gEG6X4Mqe9Iv2cdc3kMj5lHy2++no753AFmeFLjgMbpKJNBGoADZBC/1DB72D87w
85iGJCyTHQGpVAMkAUk=</Modulus><Exponent>AQAB</Exponent><P>7gUHuEHxIGIHWvTDmEaO1xtZ7FP+vkDFGCNKNM6VdbKXrrSsRUnJ7TxmiXQe4Y8OZLHXF+amj2XR
REVfqIaamQ==</P><Q>4gQVQwhuUd87VxP2epei6nTdSzSXzRPNbvn+VjUt6lULfqfcbPNGOEWT3/B5jIbF2gd0QjUU/0xw
Xstp0P/6MQ==</Q><DP>13dIQL25CWaUX/tZIP0mi4WgBrcW0aWShkJUB6/HTu+oLigyFtswZ4kZDW9IEUpObksreuB6gS9b
nGDssoN/aQ==</DP><DQ>lLQZBRyT8PwNv9IbljcUcmvneWamBcDkpgKHS1L73bSMto1c4rYA2l801t8SKdo2bKgA3tqr8Pjq
6gOtoFv60Q==</DQ><InverseQ>x3prKFvX5kMXEbhtPiFRU+oBu50v4i8q9yB8j0kyoCEvBkR62GMK5lfYEE5NuCjvSuJXKUIIpTq7
o8zoiB7G+Q==</InverseQ><D>oW3W7/96PEBdKe0609MQ/jecWFRMw+BCdv+P4pYcZcRdVQeNkKbQLEwdQnki8091L/wMVgl7XvMe
rxNb5KJ/TzBhtqjAlzxmfUzW4UM94HU2J1iqdJ4qNdqIYGwlHgKbcX6PddOZUtqTlV3lUAp7ZyZV
qzgvdCh8OOkFU799oIE=</D></RSAKeyValue>";

            string s = "中华人民共和国,hello";
            Console.WriteLine("原加密字符串：\n{0}\n", s);
            Console.WriteLine("BASE64字符串是:{0}", Convert.ToBase64String(Encoding.UTF8.GetBytes(s)));
            var priKey = "MIICXQIBAAKBgQCh7hhp9QWrFdzl3JvnOGOTJs5wAXH9o2r7bN34xf4ANMHjogGy7jhCsT5O4XNV9CK4aq38uLvcs5FkVqYOHdWClZ9Y6SOnsJqb9vNpTkfcMyvVhwRCLiehAnfl0uKrHX9QQoIfM6lQLJyPHBRXkkJs0Nd/GoqnvohCPyuMNd3vkQIDAQABAoGAMIbh2h8Lq9HJeTk7n3dOn/KpOvez6YbnsKFXhA4RqI4m7MjIKY8qXRIw0gLORZv8K7gMnAfghKtrjt5ItUJLkXSrP4PmA/FQYSQbFSV0uCODsYz7exzm/3OAlrdoSPQW0OGsOQFjOU3e13zjmjKVtnXPjs3frsa1rK1Z4ipC1IECQQDN0zT7FMlbjAj/tqo07LKEYGgQ3watGgevIsCZ2HQaCTPDhSbZaE639WdQzELv/qQ7ogckC7hgLbz40ZZ1sKKtAkEAyWeQE4aUnf6Wc+XDXgzh1TkPmnj3Mpv8CxFw0tyJcInBP1ALQgGsqgp2KWdFdYf3kNrlYgZvO6z1pV3TE6NA9QJAL4Et2z2C8+QwN4TbZ/exhCgUHMC887rsRxnIRjnVIiU5k0jqHIeck4zKWbWrRWUKVtEgwMsLtPcZhHwrM+KeyQJBALPNrSaBY3SZsJ/PY9c3EPZWdsOlcqrP7vfCnkLheCHsGYX4Y9SNwiQcKtlTxQLkW/QKN/aHYA6ansL/PE2v1XECQQCypsY3hBMrhYeji5FN7HnL3NmKUMp0fMKxXyTx1pwuSdsxX6NDJF3ut78BKkMg/7dkbaSZNBNjAsAYF0v4ZQ1t";

            //net 2048位
            // priKey = "MIIEpAIBAAKCAQEAtuHlnO70eARmRI/VYKSuFJzOuMf7pTpk53L1hxAcG5AxnonPH7HSuQ2vyJDKMxGcwn3bHD9AWbn6hJUmfFscHYe8aEdmeqKd8vvWbHx+3gYizGyiccW5R3p1gRiSOiqQGG/e6ftaWhJAFAvZ961Vzk05yxJ4nO5/ZdiWFuUNuebJeDS9APr0MnxXbptdOXtYcYDYovSGWbhtmnWPfnCBquRvUfchu9aOVTqt8QRZ6IxBvy9wdahC1umR1mwZg1fpVtauNNWy/jeiOiNh3bs7AO8jnYMyo2TjB5a+tbCv7/HB6T0QHwywfMQcbuunUmz45DgqMNFj8IS7rjW0FJFuWQIDAQABAoIBAQCisWeRKsr1EgTgrYxHg3kSAUWuAMqPfNlTRWPDmcNHigl6XmKScaFi2xgsNxKKR/rK0yffgy1+JQMGe6FXM75ZTu1/XzV9l7kn9n4U2NQMNC006tfAmwNu4TQzemZrtH2oH62RPfhs9JtoufpYai1RcEYfr/j4svtG4Vz0VSTW4UYBLRoOjJDBVzlQOI9drNpa0yoRe/Nd6mYQIC+Ar5M7Z3KRUMNaleIsNd7DJyxyy0T/lDZfZwubgoABFVMWQjjb7aQfq1c2CRRBHHBHnj3mnB8qNyXpZ6wLfO0P7vCyxNpLu1MgpxDvjQ9m7TnRrli3BEQKL0pcxxaQVzwmxmoBAoGBANx9XCeBDLpqQfylkJEOszUv+CF1mHyBRYTvcY6srQW8s5/tXIl5q3TcK6tXOVTcxqFebT7pJ1jxyQ3DD5JbSRCoIddZTS8ldhm/XOjF6AUmUg8kHM7VhTeO7FpCBDIVjO2es9jgR4X3zfqF30V1MBYP6kvUVwPpWs7qNZ0EIxW5AoGBANRWA7KPeMM6ObFwORapoEJYXnZLQL2empFdgX9jdW5pLXkgG7qkbpNz146mPLqer1kKCEpiuNdEKfDku5LCVzr6Ji/AJR45y1UYmY51hnNDAyKM0LYWeaUC94MkA3bhr0204tA3bX1kHgh16BnklCiPYTybLvZ+/qIYkg9cyW2hAoGAX6zuDh5LfaCaHZ1iS++LB+tWyn4SuwQFPJgCOJzpP3IQp7cBzo3DPqRDNshUkmRytJca5I+biVbxnU0lNqbx3451kNKpUWn6A1YsZL1r3sAwH23WKlIwylj0an33AByl2H5jIBrCLnnHIYxxw9wED668RkdPstzRkLPEq+udpdECgYBexlW4KZm5ag++R1zz5JqHgnIHUud+u4A6SgY3Gemcco29drtpv2MrpZUdMs4AbjvN/lBdA1uFmgMuZqnig6PzyxuoTJdEun8raGOB1qtAXzTTAw9VdwqswHpBqp1xPqLEiGyEi3jvcvSEyjvi8se+ouC/8HQxydVV/KmU+dFegQKBgQDHFyylw3m7s89SPIRCKBUqbfde3xM1Ll+P+ZHta+SG1hKgMcpRghIooQEcfJ4tN9PMjchNBmNDQ86YEhAfP3Iq4WehfKAEW25pFb+jF+Gje85+PQBIeigaLbQn/T0wpnfoCDPmXevgVQ5pe7FyID1KMYk+7VYnr2LNpBhWqWPJcw==";

            //java 私钥
            priKey = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBALtOu+jFMNqQfbgaD6NqJa7+rdHbUwOvLOEkwrAun3ojMtzsYn6pk4Xce7fMS23BU/jj+XHqjpty5r8ue/PKJjcyTlUJ5QecJd+rQ8HHr3URiX5h3gcjzn52A6UGbq7rWI9eMpRM2EFYn0jSCIsgC29Qtxoxdnqd8FHkpklGY5R5AgMBAAECgYBCLZBArqMTJef1SufpkdBcosjhE3+iFpthtK5At0hLS/JlkfM+00U3D256wuiHj73OeyWf9QKLs64JMDBFM1AKpxYcJI2f8U2XdIEm3+F7DGuXJxgovEH5z6Xog2CKCTJblalU/lrLQGNV9X8IyOrVCfBqun4WhdKVlshYnF5Y7QJBAO9q9bBf0ED9tWhOCw+ZAOnr7hpQsi0JZHdH/XE9RjU414SbWLvc5TV3xdqxYY/iR2YpxxRWTuGw9sBbWfog6asCQQDIR9L3GmNtAZ2hDNPXDSrjYd3GI5V1qXjWQqgh5rXq+ln++MWaYbspN7GRmpm0NRMfX0qc3i2HAxMWE1SEn75rAkA7GdpAicZs5LRNZUaRuSFinV0Pne/98h2c3GaR96BHLxr0nyyOY38pbcGntLXywNcDPzjnqk6apAalgWd6uXk1AkEAretpMPnyY0Om7ablAvfFSaW/34MhFAcyDuXdeWsOiNoUcsz3U+QQkm9xvJ5DYXFBhNnMQkLRyE+MMHxixbSutwJBAOhXvmK+vyu2Yjwq9+eVZ5WadmGqcAj+6/XaoP98vMsNfpIHmriOrZpyZDYz2POVX6r4ouk6IAMJrlEK8dxhfYU=";
          
            RSACryptoServiceProvider rsa;

            //rsa = new RSACryptoServiceProvider();
            //string[] genKey = RsaHelper.GenerateKey();
            //rsa.FromXmlString(genKey[0]);
            //rsa.FromXmlString("<RSAKeyValue><Modulus>u0676MUw2pB9uBoPo2olrv6t0dtTA68s4STCsC6feiMy3OxifqmThdx7t8xLbcFT+OP5ceqOm3Lmvy5788omNzJOVQnlB5wl36tDwcevdRGJfmHeByPOfnYDpQZurutYj14ylEzYQVifSNIIiyALb1C3GjF2ep3wUeSmSUZjlHk=</Modulus><Exponent>AQAB</Exponent><P>72r1sF/QQP21aE4LD5kA6evuGlCyLQlkd0f9cT1GNTjXhJtYu9zlNXfF2rFhj+JHZinHFFZO4bD2wFtZ+iDpqw==</P><Q>yEfS9xpjbQGdoQzT1w0q42HdxiOVdal41kKoIea16vpZ/vjFmmG7KTexkZqZtDUTH19KnN4thwMTFhNUhJ++aw==</Q><DP>OxnaQInGbOS0TWVGkbkhYp1dD53v/fIdnNxmkfegRy8a9J8sjmN/KW3Bp7S18sDXAz8456pOmqQGpYFnerl5NQ==</DP><DQ>retpMPnyY0Om7ablAvfFSaW/34MhFAcyDuXdeWsOiNoUcsz3U+QQkm9xvJ5DYXFBhNnMQkLRyE+MMHxixbSutw==</DQ><InverseQ>6Fe+Yr6/K7ZiPCr355VnlZp2YapwCP7r9dqg/3y8yw1+kgeauI6tmnJkNjPY85Vfqvii6TogAwmuUQrx3GF9hQ==</InverseQ><D>Qi2QQK6jEyXn9Urn6ZHQXKLI4RN/ohabYbSuQLdIS0vyZZHzPtNFNw9uesLoh4+9znsln/UCi7OuCTAwRTNQCqcWHCSNn/FNl3SBJt/hewxrlycYKLxB+c+l6INgigkyW5WpVP5ay0BjVfV/CMjq1Qnwarp+FoXSlZbIWJxeWO0=</D></RSAKeyValue>");

            var pkcs8 = Convert.FromBase64String(priKey);
            rsa = RsaHelper.DecodePrivateKeyInfo(pkcs8);

            //string signType = "RSA";// "RSA";//"RSA2";
            //rsa =  DecodeRSAPrivateKey(priKey, signType);

            //有错误
            //RSAParameters param = RSA.ConvertFromPrivateKey(priKey);

            //RSAParameters param2 = rsa.ExportParameters(true);

            //Console.WriteLine("参数结果:{0}", param.Equals(param2));
            //Console.WriteLine("参数结果:{0}", param.Exponent == param2.Exponent);


            byte[] cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(s), true);

            var en2 = Convert.ToBase64String(cipherbytes);
            Console.WriteLine("加密结果：\n{0}\n", en2);

            en2 = "BLeM47f7ZYlDJzR0rEGZ5/o6mvPD3hG/C3NWf9zzYw2DLEiwYaantOikYycENhcvjxZ41GfVOizPL3UuNUkZTTa1WyNkneP8Aw1ji209kOGKC5qD8G6WMtuclkXXG0F5jJ/AMYNkKbjpq7hoHtyZOCunH1r2meZApQCzeXeQXr0=";

            cipherbytes = rsa.Decrypt(Convert.FromBase64String(en2), true);

            Console.WriteLine("=======================");
            Console.WriteLine("解密结果：\n{0}\n", Encoding.UTF8.GetString(cipherbytes));

            Console.WriteLine("=======================");

            RSAParameters sp = rsa.ExportParameters(true);

            var p1 = rsa.ToXmlString(true);
            var p2 = rsa.ToXmlString(false);
            Console.WriteLine("私钥：\n{0}\n", p1);
            Console.WriteLine("公钥：\n{0}\n", p2);

            Console.Read();
        }

        static void Main()
        {
            int p = 61; int q = 53;//两个不相等的质数
            int n = p * q;//3233
            int e = 17;// 65537;//1<e<φ(n),e与φ(n)互质
       
            int d = 2753;  //模反元素d,e*d=1(modφ(n));

            int[] pubKey = { n, e };
            int[] priKey = { n, d };

            byte[] m = Encoding.Default.GetBytes("加密");//原信息

            //int encrypt =  Math.Pow(s, e) % n;

            BigInt x = new BigInt(m);
            BigInt ng = new BigInt(n);

            BigInt c = BigInt.Pow(x, e) % ng;//m^e=c(mod n);取c

            string encrypt = c.ToString();//加密后的字符串

            Console.WriteLine("原信息是:{0}", Encoding.Default.GetString(m));

            Console.WriteLine("加密后信息是:{0}", c);

            BigInt de = BigInt.Pow(c, d) % ng;//c^d = m(mod n);取m


            Console.WriteLine("解密后信息是:{0}", de.ToString());
            
        }

        public int Euler(int n)
        {
            if (n == 1) return 1;


            return 0;
        }

        static void Main000()
        {
            var content = "中华人民共和国,w$*";
            Type type = typeof(Crypto.HashName);

            var names = Enum.GetNames(type);
            foreach (var item in names)
            {
                var name = (Crypto.HashName)Enum.Parse(type, item);
                Console.WriteLine("散列方式:{0},结果:{1}", item, content.HashEx(name));
            }
            Console.WriteLine("===============对称加解密 ===============");
            Type t = typeof(Crypto.Algorithm);

            names = Enum.GetNames(t);
            foreach (var item in names)
            {
                var name = (Crypto.Algorithm)Enum.Parse(t, item);
            
                var cs = content.Encrypt(algName: name, upper: true);
                Console.WriteLine("加密方式:{0},结果:{1}", item, cs);
                var ds = cs.Decrypt(algName: name);
                Console.WriteLine("解密方式:{0},结果:{1}", item, ds);
            }
        }

        static void Main4()
        {    
            //RSA密钥对的构造器  
            RsaKeyPairGenerator pGen = new RsaKeyPairGenerator();

            //RSA密钥构造器的参数  
            RsaKeyGenerationParameters param = new RsaKeyGenerationParameters(
             Org.BouncyCastle.Math.BigInteger.ValueOf(0x11),
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

            TextWriter textWriter = new StringWriter();
            PemWriter pemWriter = new PemWriter(textWriter);
            pemWriter.WriteObject(privateKey);
            pemWriter.Writer.Flush();

            string pemKey = textWriter.ToString();
            Console.WriteLine(pemKey);

            CspParameters csp = new CspParameters();            

            RSACryptoServiceProvider rSA = new RSACryptoServiceProvider(1024, csp);
            string xmlPrivateKey= @"<RSAKeyValue><Modulus>5m9m14XH3oqLJ8bNGw9e4rGpXpcktv9MSkHSVFVMjHbfv+SJ5v0ubqQxa5YjLN4vc49z7SVju8s0X4gZ6AzZTn06jzWOgyPRV54Q4I0DCYadWW4Ze3e+BOtwgVU1Og3qHKn8vygoj40J6U85Z/PTJu3hN1m75Zr195ju7g9v4Hk=</Modulus><Exponent>AQAB</Exponent><P>/hf2dnK7rNfl3lbqghWcpFdu778hUpIEBixCDL5WiBtpkZdpSw90aERmHJYaW2RGvGRi6zSftLh00KHsPcNUMw==</P><Q>6Cn/jOLrPapDTEp1Fkq+uz++1Do0eeX7HYqi9rY29CqShzCeI7LEYOoSwYuAJ3xA/DuCdQENPSoJ9KFbO4Wsow==</Q><DP>ga1rHIJro8e/yhxjrKYo/nqc5ICQGhrpMNlPkD9n3CjZVPOISkWF7FzUHEzDANeJfkZhcZa21z24aG3rKo5Qnw==</DP><DQ>MNGsCB8rYlMsRZ2ek2pyQwO7h/sZT8y5ilO9wu08Dwnot/7UMiOEQfDWstY3w5XQQHnvC9WFyCfP4h4QBissyw==</DQ><InverseQ>EG02S7SADhH1EVT9DD0Z62Y0uY7gIYvxX/uq+IzKSCwB8M2G7Qv9xgZQaQlLpCaeKbux3Y59hHM+KpamGL19Kg==</InverseQ><D>vmaYHEbPAgOJvaEXQl+t8DQKFT1fudEysTy31LTyXjGu6XiltXXHUuZaa2IPyHgBz0Nd7znwsW/S44iql0Fen1kzKioEL3svANui63O3o5xdDeExVM6zOf1wUUh/oldovPweChyoAdMtUzgvCbJk1sYDJf++Nr0FeNW1RB1XG30=</D></RSAKeyValue>";
            rSA.FromXmlString(xmlPrivateKey);

            var p = rSA.ExportParameters(true);

            var key = new RsaPrivateCrtKeyParameters(
                new Bc_BigInteger(1, p.Modulus), new Bc_BigInteger(1, p.Exponent),
               new Bc_BigInteger(1, p.D), new Bc_BigInteger(1, p.P),
                new Bc_BigInteger(1, p.Q), new Bc_BigInteger(1, p.DP), new Bc_BigInteger(1, p.DQ),
                new Bc_BigInteger(1, p.InverseQ));
            
            

            Console.WriteLine("===============");
            Console.WriteLine(privateKey.Modulus);
            Console.WriteLine(privateKey.Modulus.ToString());
            Console.WriteLine(privateKey.Modulus.ToString().Length);
            Console.WriteLine("===============");


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
                Console.WriteLine("密文是:{0}", testData.ToHex());
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
    }
}
