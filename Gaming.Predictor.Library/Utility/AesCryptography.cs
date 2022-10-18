using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Gaming.Predictor.Library.Utility
{
    class AesCryptography
    {
        private static readonly string VIKey = "gOkUop94+OCp/OYluffYmg==";//Cannot contain symbols or special characters
        private static readonly string Key = "HykIcHiGo3s7nATSuuiobA==";//Cannot contain symbols or special characters

        public static string AesDecrypt(string vCryptoText)
        {
            string decodedUserGuid = Decode(vCryptoText);
            byte[] cipherText = null;
            byte[] plainText = null;
            string mPlainText = string.Empty;
            try
            {
                cipherText = Convert.FromBase64String(decodedUserGuid);
                using (Aes aes = new AesManaged())
                {
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = Convert.FromBase64String(Key);
                    aes.IV = Convert.FromBase64String(VIKey);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherText, 0, cipherText.Length);
                        }
                        plainText = ms.ToArray();
                    }
                }
                mPlainText = System.Text.Encoding.Unicode.GetString(plainText);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cipherText = null;
                plainText = null;
            }
            return mPlainText;
        }

        public static string Decode(string str)
        {
            byte[] decbuff = Convert.FromBase64String(str);
            return System.Text.Encoding.UTF8.GetString(decbuff);
        }
    }
}
