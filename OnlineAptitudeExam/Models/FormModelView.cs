namespace OnlineAptitudeExam.Models
{
    public class FormModelView
    {
        public class Test
        {
            public string Name { get; set; }
        }


        public class Question
        {
            public int testId { get; set; }

            public int type { get; set; }

            public string question { get; set; }

            public string answers { get; set; }

            public string correctAnswers { get; set; }

            public float score { get; set; }
        }

        public class Account
        {
            public string fullname { get; set; }
            public string username { get; set; }
            public string password { get; set; }

        }
    }
}