using OnlineAptitudeExam.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

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

        public static Pair GetScoreFromExam(Exam exam)
        {
            Pair pair = new Pair();
        
            var examDetails = exam.ExamDetails.OrderBy(ex => ex.Question.type).ToList();
            var questions = exam.Test.Questions.OrderBy(ex => ex.type).ToList();

            double totalScore = 0, examScore = 0;
            for (int i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                var examDetail = examDetails.Count > i ? examDetails[i] : null;
                examScore += question.score.Value;
                if (examDetail != null && examDetail.selected_question == question.correct_answers)
                {
                    totalScore += question.score.Value;
                }
            }
            pair.First = totalScore;
            pair.Second = examScore;
            
            return pair;
        }
    }


}