using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BE
{
    [Serializable]
    public class Cryptotext
    {
        public RSAParameters VK { get; set; }
        public List<byte[]> Ci = new List<byte[]>();
        public byte[] signedData{ get; set; }
    }
}
