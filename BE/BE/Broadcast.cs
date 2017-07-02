using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace BE
{
    public class Broadcast
    {
        public List<RSAParameters> MSK = new List<RSAParameters>();
        public List<RSAParameters> MPK = new List<RSAParameters>();
    
        
        public void Setup(int n)
        {
            for (int i = 0; i < n; i++)
            {
                using (var RSA = new RSACryptoServiceProvider())
                {
                    var x = RSA.ExportParameters(false);
                    var y = RSA.ExportParameters(true);
                    MPK.Add(x);
                    MSK.Add(y);
                }
            }
        }

        public RSAParameters KeyGen(int i)
        {
            return MSK[i];
        }

        public byte[] Encrypt(byte[] M, List<int> S)
        {
            RSAParameters SK, VK;
            Cryptotext cryptotext = new Cryptotext();
            List<byte[]> cry = new List<byte[]>();
            using (var RSA = new RSACryptoServiceProvider())
            {
                VK = RSA.ExportParameters(false);
                SK = RSA.ExportParameters(true);
            }
            for (int i = 1; i < S.Count; i++)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, new TextPerUser(M, VK));
                    using (var RSA = new RSACryptoServiceProvider())
                    {
                        RSA.ImportParameters(MPK[i]);
                        cry[i] = RSA.Encrypt(ms.ToArray(), false);
                    }

                }

            }
            cryptotext.VK = VK;
            cryptotext.C = cry;
            using (var RSA = new RSACryptoServiceProvider())
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    RSA.ImportParameters(SK);
                    bf.Serialize(ms, cry);
                    cryptotext.signedData = RSA.SignData(ms.ToArray(), new SHA1CryptoServiceProvider());
                }
                
            }

            byte[] rezult=null;
            BinaryFormatter bf1 = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf1.Serialize(ms, cryptotext);
                rezult = ms.ToArray();
            }
            return rezult;
        }
    }
}
