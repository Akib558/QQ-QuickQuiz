using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Models
{

    public class LoginRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class LogoutRequestModel
    {
        // public string TokenString { get; set; }
        public int UserID { get; set; }
    }
}