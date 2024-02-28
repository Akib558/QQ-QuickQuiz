using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using Microsoft.AspNetCore.Identity.Data;
using QuickQuiz.Models;
using Microsoft.AspNetCore.Mvc;
using QuickQuiz.Repositories.Implementations;
using QuickQuiz.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using QuickQuiz.Repositories.Interfaces;
using QuickQuiz.Services.Implementations;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace QuickQuiz.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _configuration;
        private IUserAuthService _userAuthService;
        public AuthController(IConfiguration configuration, IUserAuthService userAuthService){
            _configuration = configuration;
            _userAuthService = userAuthService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginRequestModel usr){
            IActionResult response = Unauthorized();
            var user = _userAuthService.Login(usr);
            
            if(user != null){
                // var token = GenerateToken(user);
                response = Ok(new {token = user});
            }

            return response;
        }

        [Authorize]
        [HttpPost]
        [Route("register")]
        public IActionResult AddUser(RegistrationRequest request){
            IActionResult response = Unauthorized();
            var user = _userAuthService.AddUser(request);
            
            if(user != null){
                // var token = GenerateToken(user);
                response = Ok(new {token = user});
            }

            return response;
        }




    }
}