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

    
        public async Task<string> Login(LoginRequestModel loginRequest)
        {
            if (await _userRepository.Authenticate(loginRequest))
            {
                return await Task.FromResult(GenerateToken(loginRequest));
            }
            return null;
        }

        public async Task<bool> Logout(String tokenString){
            if(await _userRepository.Logout(tokenString)){
                return await Task.FromResult(true);
            }
            return false;
        }


        public async Task<string> AddUser(RegistrationRequestModel request)
        {
            if (await _userRepository.Register(request))
            {
                return await Task.FromResult(GenerateToken(request));
            }
            return null;
        }




        private string GenerateToken(LoginRequestModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null,
                expires: DateTime.Now.AddMinutes(30),
                
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string GenerateToken(RegistrationRequestModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}