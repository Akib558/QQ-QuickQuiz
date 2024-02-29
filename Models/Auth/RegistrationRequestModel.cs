using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickQuiz.Models
{
    public class RegistrationRequestModel
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string UserType { get; set; }

    }


}