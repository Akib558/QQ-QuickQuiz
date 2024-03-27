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
    
    public class LoginReposneModel
    {
        public string token { get; set; }
        public string tefreshToken { get; set; }
        public int userID { get; set; }
        
    }
    
    public class RefreshTokenRequestModel
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
        public int UserID { get; set; }
    }
}