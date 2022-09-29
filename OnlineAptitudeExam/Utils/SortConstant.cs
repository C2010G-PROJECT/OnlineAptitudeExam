
namespace OnlineAptitudeExam.Utils
{
    public class SortHelper
    {
        public const string ASC = "asc";
        public const string DESC = "desc";

        public const string STT = "stt";
        public const string NAME = "name";
        public const string DATE = "date";

        public static string ValueOrNull(string value, object test)
        {
            return test == null ? null : value;
        }

        public static string GetOrder(string sort, string currentSort, string currentOrder)
        {
            return Equals(sort, currentSort) ? GetNextOrder(currentOrder) : DESC;
        }

        public static string GetNextOrder(string order)
        {
            return string.IsNullOrEmpty(order) ? DESC : (order == DESC) ? ASC : null;
        }

        public static bool IsAsc(string s)
        {
            return ASC.Equals(s);
        }
    }
}