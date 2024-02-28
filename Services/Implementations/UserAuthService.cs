using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using QuickQuiz.Models;
using QuickQuiz.Repositories.Interfaces;


namespace QuickQuiz.Services.Implementations
{
    public class UserAuthService : IUserAuthService
    {
        private IConfiguration _configuration;
        private IUserRepository _userRepository;
        public UserAuthService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }


        public string Login(LoginRequestModel loginRequest)
        {
            if (_userRepository.Authenticate(loginRequest))
            {
                return GenerateToken(loginRequest);
            }
            return null;
        }


        public string AddUser(RegistrationRequest request)
        {
            if (_userRepository.Register(request))
            {
                return GenerateToken(request);
            }
            return null;
        }




        private string GenerateToken(LoginRequestModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null,
                expires: DateTime.Now.AddSeconds(30),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string GenerateToken(RegistrationRequest user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], null,
                expires: DateTime.Now.AddSeconds(30),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}