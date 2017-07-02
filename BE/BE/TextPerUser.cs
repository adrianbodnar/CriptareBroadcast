using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace BE
{
    [Serializable]
    public  class TextPerUser
    {
        byte[] M { get; set; }
        RSAParameters VK { get; set; }

        public TextPerUser(byte[] M, RSAParameters Vk) {
            this.M = M;
            this.VK = Vk;
        }
    }
}
