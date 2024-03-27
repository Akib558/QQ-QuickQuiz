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
using Microsoft.AspNetCore.Authentication;

// using QuickQuiz.Models;

namespace QuickQuiz.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _configuration;
        private IUserAuthService _userAuthService;

        public AuthController(IConfiguration configuration, IUserAuthService userAuthService)
        {
            _configuration = configuration;
            _userAuthService = userAuthService;
        }

        [Authorize]
        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout(LogoutRequestModel logoutRequestModel)
        {
            // HttpContext.SignOutAsync();
            // return Ok(true);
            int userID = logoutRequestModel.UserID;
            string token = HttpContext.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            // return Ok(token)
            IActionResult response = Unauthorized();
            bool user = await _userAuthService.Logout(token, userID);

            if (user)
            {
                response = Ok(new { token = user });
            }

            return response;
        }
        
        [Authorize]
        [HttpPost]
        [Route("active/{userID}")]
        public async Task<IActionResult> IsActive(int userID)
        {
            Console.WriteLine("isactive controller");
            IActionResult response = Unauthorized();
            bool user = await _userAuthService.IsActive(userID);

            return Ok(
                new
                {
                    status = user
                }
                );
        }
        
        
        [AllowAnonymous]
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestModel refreshTokenRequestModel)
        {
            // HttpContext.SignOutAsync();
            // return Ok(true);
            int userID = refreshTokenRequestModel.UserID;
            string token = HttpContext.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            return Ok(await _userAuthService.RefreshToken(refreshTokenRequestModel));
        }
        
        
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequestModel usr)
        {
            // string _connectionString = "Server=(localdb)\\QuickQuiz; Database=QuickQuiz; Trusted_Connection=True;Encrypt=false;";

            // try
            // {
            //     using (var connection = new SqlConnection(_connectionString))
            //     {
            //         await connection.OpenAsync();
            //         var query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";
            //         using (var command = new SqlCommand(query, connection))
            //         {
            //             command.Parameters.AddWithValue("@Username", usr.Username);
            //             command.Parameters.AddWithValue("@PasswordHash", usr.Password);

            //             // Execute the query asynchronously
            //             var result = await command.ExecuteScalarAsync();

            //             // Convert the result to a boolean value
            //             int count = Convert.ToInt32(result);
            //             return Ok(count > 0);
            //         }
            //     }

            // }
            // catch (Exception ex)
            // {
            //     return Ok(ex);
            //     // throw;
            // }


            // // return Ok(HttpContext.Request.Path);
            IActionResult response = Unauthorized();
            var user = await _userAuthService.Login(usr);

            if (user != null)
            {
                response = Ok(user);
                // var token = GenerateToken(user);
            }

            return response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> AddUser(RegistrationRequestModel request)
        {
            IActionResult response = Ok();
            var user = await _userAuthService.AddUser(request);

            if (user != null)
            {
                // var token = GenerateToken(user);
                response = Ok(new { token = user });
            }

            return response;
        }
    }
}