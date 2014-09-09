using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace RS_dash.Utils
{
    public class Security
    {
        private static string salt = "%gRk*!34XM0";

        public static string encrypt(string str)
        {
            string pass = str + salt;
            byte[] bytes = Encoding.UTF8.GetBytes(pass);
            Array.Reverse(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string decrypt(string str)
        {
            byte[] bytes = Convert.FromBase64String(str);
            Array.Reverse(bytes);
            string pass = ASCIIEncoding.ASCII.GetString(bytes);
            return pass.Replace(salt, "");
        }

    }
}
