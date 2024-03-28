using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
// using QuickQuiz.Models.Auth;
using QuickQuiz.Models;

using QuickQuiz.Repositories.Interfaces;


namespace QuickQuiz.Services.Implementations
{
    public class UserAuthService : IUserAuthService
    {
        private IConfiguration _configuration;
        private IUserAuthRepository _userRepository;
        public UserAuthService(IConfiguration configuration, IUserAuthRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }


        public async Task<object> Login(LoginRequestModel loginRequest)
        {
            var userId = await _userRepository.Authenticate(loginRequest);
            if (userId != 0)
            {
                SetActive(Convert.ToInt32(userId));
                var token= await Task.FromResult(GenerateToken(loginRequest, userId));
                var acessToken = await Task.FromResult(GenerateToken(loginRequest, userId));
                
                
                return new
                {
                    token = token,
                    userID = userId,
                    acessToken = acessToken
                };
            }
            return null;
        }
            
        
        public async Task<object> RefreshToken(RefreshTokenRequestModel refreshTokenRequest)
        {
            var status = await IsActive(refreshTokenRequest.UserID);
            if (status)
            {
                var token = await Task.FromResult(GenerateRefreshToken(refreshTokenRequest,
                    refreshTokenRequest.UserID));
                var refreshToken = await Task.FromResult(GenerateRefreshToken(refreshTokenRequest, refreshTokenRequest.UserID));
                
                return new
                {   
                    refreshToken = token,
                    token = token,
                    userID = refreshTokenRequest.UserID
                };
            }

            return new
            {
                refreshToken = "",
                token = "",
                userID = refreshTokenRequest.UserID
            
            };
        }
        
        
        public async Task<bool> Logout(String tokenString,int userID)
        {
            if (await _userRepository.Logout(tokenString, userID))
            {
                return await Task.FromResult(true);
            }
            return false;
        }


        public async Task<string> AddUser(RegistrationRequestModel request)
        {
            if (await _userRepository.Register(request))
            {
                Console.WriteLine(4);
                return await Task.FromResult(GenerateToken(request));
            }
            Console.WriteLine(5);
            return null;
        }

        public async Task<bool> IsActive(int userID)
        {
            if (await _userRepository.IsActive(userID))
            {
                return await Task.FromResult(true);
            }
            return false;
        }
        
        public async Task<bool> SetActive(int userID)
        {
            if (await _userRepository.SetActive(userID))
            {
                return await Task.FromResult(true);
            }
            return false;
        }

        private string GenerateRefreshToken(RefreshTokenRequestModel user, int userID)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddHours(30), // Set token expiration
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = credential
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
        
        
        private string GenerateToken(LoginRequestModel user, int userID)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddHours(30), // Set token expiration
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = credential
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
        private string GenerateToken(RegistrationRequestModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}