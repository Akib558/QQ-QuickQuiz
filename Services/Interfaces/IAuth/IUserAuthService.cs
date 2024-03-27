using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models;

namespace QuickQuiz.Services.Interfaces
{
    public interface IUserAuthService
    {
        Task<object> Login(LoginRequestModel loginRequest);

        Task<string> AddUser(RegistrationRequestModel request);
        
        Task<bool> IsActive(int userID);
        Task<bool> SetActive(int userID);
    
        Task<object> RefreshToken(RefreshTokenRequestModel refreshTokenRequest);
        Task<bool> Logout(string tokenString, int userID);
    }
}