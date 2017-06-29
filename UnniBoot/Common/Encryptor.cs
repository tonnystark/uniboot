using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace UnniBoot.Common
{
    public static class Encryptor
    {
        /// <summary>
        /// Mã hóa MD5 cho password
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            // compute hast
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash
            byte[] result = md5.Hash;

            StringBuilder strBuild = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                // change it into 2 hexa digit
                strBuild.Append(result[i].ToString("x2"));
            }

            return strBuild.ToString();
        }
    }
}