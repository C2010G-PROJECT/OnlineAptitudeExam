using System.Collections.Generic;

namespace OnlineAptitudeExam.Models
{
    public class CustomModel
    {
        public struct TestReport
        {
            public int id;
            public string name;
            public int totalUser;

            public TestReport(int id, string name, int totalUser)
            {
                this.id = id;
                this.name = name;
                this.totalUser = totalUser;
            }

            public override bool Equals(object obj)
            {
                return obj is TestReport other &&
                       id == other.id &&
                       name == other.name &&
                       totalUser == other.totalUser;
            }

            public override int GetHashCode()
            {
                int hashCode = -3333083;
                hashCode = hashCode * -1521134295 + id.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
                hashCode = hashCode * -1521134295 + totalUser.GetHashCode();
                return hashCode;
            }

            public void Deconstruct(out int id, out string name, out int item3)
            {
                id = this.id;
                name = this.name;
                item3 = totalUser;
            }

            public static implicit operator (int id, string name, int totalUser)(TestReport value)
            {
                return (value.id, value.name, value.totalUser);
            }

            public static implicit operator TestReport((int id, string name, int totalUser) value)
            {
                return new TestReport(value.id, value.name, value.totalUser);
            }
        }


    }
}