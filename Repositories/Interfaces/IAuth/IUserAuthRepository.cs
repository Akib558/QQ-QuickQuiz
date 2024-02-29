using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models;
// using QuickQuiz.Models.Auth;

namespace QuickQuiz.Repositories.Interfaces
{
    public interface IUserAuthRepository
    {
        Task<bool> Authenticate(LoginRequestModel loginRequest);
        Task<bool> Register(RegistrationRequestModel request); 
        Task <string> HashPassword(string password);
        Task<bool> Logout(string tokenString);
    }
}