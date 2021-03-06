using System;
using System.Security.Cryptography;
using System.Text;

namespace ChloesBeauty.API.Helpers
{
    public static class Functions
    {
        #region Public Methods

        public static string Decrypt(string toDecrypt)
        {
            byte[] results;
            var hasProvider = new MD5CryptoServiceProvider();
            byte[] TDESkey = hasProvider.ComputeHash(UTF8Encoding.UTF8.GetBytes(Constants.ENCRYPTIONPASSWORD));

            var alg = new TripleDESCryptoServiceProvider
            {
                Key = TDESkey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            byte[] dataToDecrypt = Convert.FromBase64String(toDecrypt);

            try
            {
                ICryptoTransform decryptor = alg.CreateDecryptor();
                results = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            }
            finally
            {
                alg.Clear();
                hasProvider.Clear();
            }

            return UTF8Encoding.UTF8.GetString(results);
        }

        public static string Encrypt(string toEncrypt)
        {
            byte[] results;
            var hasProvider = new MD5CryptoServiceProvider();
            byte[] TDESkey = hasProvider.ComputeHash(UTF8Encoding.UTF8.GetBytes(Constants.ENCRYPTIONPASSWORD));

            var alg = new TripleDESCryptoServiceProvider
            {
                Key = TDESkey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            byte[] dataToEncrypt = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            try
            {
                ICryptoTransform encryptor = alg.CreateEncryptor();
                results = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            finally
            {
                alg.Clear();
                hasProvider.Clear();
            }

            return Convert.ToBase64String(results);
        }

        #endregion Public Methods
    }
}