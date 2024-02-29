using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models;

namespace QuickQuiz.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> Authenticate(LoginRequestModel loginRequest);
        Task<bool> Register(RegistrationRequest request); 
        Task <string> HashPassword(string password);
        Task<bool> Logout(string tokenString);
    }
}