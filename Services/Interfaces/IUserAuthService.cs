using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models;

namespace QuickQuiz.Services.Interfaces
{
    public interface IUserAuthService
    {
        Task<string> Login(LoginRequestModel loginRequest);

        Task<string> AddUser(RegistrationRequest request);

        Task<bool> Logout(string tokenString);
    }
}