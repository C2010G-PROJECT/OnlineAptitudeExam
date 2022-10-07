using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

namespace OnlineAptitudeExam.Utils
{
    public class Helper
    {
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");
            }
            return byte2String;
        }
        public static string GenerateAlphaNumericPwd(int length = 24)
        {
            string numbers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            Random objrandom = new Random();
            string strrandom = string.Empty;
            for (int i = 0; i < length; i++)
            {
                int temp = objrandom.Next(0, numbers.Length);
                string passwordString = numbers.ToCharArray()[temp].ToString();
                strrandom += passwordString;
            }
            return strrandom;
        }
    }

  
}