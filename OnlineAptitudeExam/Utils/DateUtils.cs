using System;

namespace OnlineAptitudeExam.Utils
{
    public class DateUtils
    {
        public static string toDateString(long? timestame)
        {
            if (timestame == null)
            {
                return "";
            }
            else
            {
                return new DateTime(timestame.Value).ToString("HH:mm dd-MM-yyyy");
            }
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }
    }
}