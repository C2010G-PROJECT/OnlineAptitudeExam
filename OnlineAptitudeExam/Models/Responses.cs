using System;

namespace OnlineAptitudeExam.Models
{
    public class Responses
    {
        public bool success { get; set; }
        public string msgType { get; set; }

        public string message { get; set; }

        public object data { get; set; }

        public static Responses Success(object data, string message = "success", MessageType msgType = MessageType.SUCCESS)
        {
            Responses response = new Responses();
            response.success = true;
            response.msgType = msgType.ToString().ToLower();
            response.message = message;
            response.data = data;
            return response;
        }
        public static Responses Error(string message = "error", MessageType msgType = MessageType.ERROR, object data = null)
        {
            Responses response = new Responses();
            response.success = false;
            response.msgType = msgType.ToString().ToLower();
            response.message = message;
            response.data = data;
            return response;
        }

        public enum MessageType 
        {
            SUCCESS, INFO, WARNING, ERROR   
        }        
        
    }               
}                 