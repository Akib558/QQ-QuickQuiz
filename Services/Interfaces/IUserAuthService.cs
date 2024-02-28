using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models;

namespace QuickQuiz.Services.Interfaces
{
    public interface IUserAuthService
    {
        public string Login(LoginRequestModel loginRequest);

        public string AddUser(RegistrationRequest request);
    }
}