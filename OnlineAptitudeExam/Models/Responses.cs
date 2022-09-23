namespace OnlineAptitudeExam.Models
{
    public class Responses
    {
        public bool success { get; set; }

        public string message { get; set; }

        public object data { get; set; }

        public static Responses Success(object data, string message = "success")
        {
            Responses response = new Responses();
            response.success = true;
            response.message = message;
            response.data = data;
            return response;
        }
        public static Responses Error(string message = "error", object data = null)
        {
            Responses response = new Responses();
            response.success = false;
            response.message = message;
            response.data = data;
            return response;
        }
    }
}