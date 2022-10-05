using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineAptitudeExam.Models
{
    public class ChoiceAnswer
    {
        public int IsSelected { get; set; }

        public List<Question> question { get; set; }
    }

   
}