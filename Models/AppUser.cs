using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
    }

    public class RegistrationRequest
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string UserType { get; set; }

    }

    public class LoginRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}