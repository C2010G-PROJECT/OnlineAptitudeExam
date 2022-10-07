using System;
using System.Collections.Generic;
using System.Data;
namespace OnlineAptitudeExam.Models
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            AccountsTested = new List<Account>();
            ExamsTested = new List<Exam>();
        }

        public List<Account> AccountsTested { get; set; }

        public List<Exam> ExamsTested { get; set; }
    }
}