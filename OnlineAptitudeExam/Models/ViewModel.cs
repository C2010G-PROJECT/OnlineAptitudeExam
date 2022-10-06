using System;
using System.Collections.Generic;
using System.Data;
namespace OnlineAptitudeExam.Models
{
    public class HomeViewModel
    {
        public List<Account> AccountsTested { get; set; }

        public List<Exam> ExamsTested { get; set; }
    }
}