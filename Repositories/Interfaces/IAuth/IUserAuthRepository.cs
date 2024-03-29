using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuickQuiz.Models;
// using QuickQuiz.Models.Auth;

namespace QuickQuiz.Repositories.Interfaces
{
    public interface IUserAuthRepository
    {
        Task<int> Authenticate(LoginRequestModel loginRequest);
        
        Task<bool> SetActive(int userID);
        
        Task<bool> IsActive(int userID);
        Task<bool> Register(RegistrationRequestModel request); 
        Task <string> HashPassword(string password);
        Task<bool> Logout(string tokenString, int userID);
        // Task<object> RefreshToken(RefreshTokenRequestModel refreshTokenRequest);
    }
}