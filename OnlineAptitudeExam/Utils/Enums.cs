
namespace OnlineAptitudeExam.Utils
{

    public class Constants
    {
        public const int TOTAL_QUESTION_IN_TEST = 15;
        public const int TOTAL_QUESTION_IN_CATEGORY = 5;
    }

    public class Enums
    {
        public enum Status : ushort
        {
            UNLOCK = 0,
            LOCK = 1,
            PRIVATE = 0,
            PUBLIC = 1,
        }

        public enum Type : ushort
        {
            ADMIN = 0,
            USER = 1,

            GENERAL_KNOWLEDGE = 0,
            MATHEMATICS = 1,
            COMPUTER_TECHNOLOGY = 2
        }

        public static byte GetOpposite(byte val)
        {
            return (byte)(val == 0 ? 1 : 0);
        }

        public static string GetQuestionType(int val)
        {
            return val == 0 ? "General Knowledge" :
                val == 1 ? "Mathematics" : "Computer Technology";
        }
    }
}