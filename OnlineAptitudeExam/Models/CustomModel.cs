using System.Collections.Generic;

namespace OnlineAptitudeExam.Models
{
    public class CustomModel
    {
        public class TestReport
        {
            public int? Id { get; }
            public string Name { get; }
            public List<Exam> Exams { get; }

            public TestReport(int? id, string name, List<Exam> exams)
            {
                Id = id;
                Name = name;
                Exams = exams;
            }

            public override bool Equals(object obj)
            {
                return obj is TestReport other &&
                       Id == other.Id &&
                       Name == other.Name &&
                       EqualityComparer<List<Exam>>.Default.Equals(Exams, other.Exams);
            }

            public override int GetHashCode()
            {
                int hashCode = 1536209737;
                hashCode = hashCode * -1521134295 + Id.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
                hashCode = hashCode * -1521134295 + EqualityComparer<List<Exam>>.Default.GetHashCode(Exams);
                return hashCode;
            }
        }
      
    }
}