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
    }
}