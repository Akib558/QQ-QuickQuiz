using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using QuickQuiz.Models;
using QuickQuiz.Services.Interfaces;

namespace QuickQuiz.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly string _connectionString = "Server=(localdb)\\QuickQuiz; Database=QuickQuiz; Trusted_Connection=True;Encrypt=false;";

        private IUserAuthService _userAuthService;

        public UserController(IUserAuthService userAuthService)
        {
            _userAuthService = userAuthService;
        }

        [Authorize]
        [HttpGet]
        [Route("getdata")]
        public IActionResult GetData()
        {

            List<User> todos = new List<User>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT UserID, Username, PasswordHash, UserType FROM Users";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        User todo = new User
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            Username = Convert.ToString(reader["Username"]),
                            Password = Convert.ToString(reader["PasswordHash"]),
                            UserType = Convert.ToInt32(reader["UserType"])

                        };
                        todos.Add(todo);
                    }
                }
            }

            return Ok(todos);

            // string token = HttpContext.Request.Headers["Authorization"];

            // // You may need to parse the token string to extract just the token value
            // if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
            // {
            //     token = token.Substring("Bearer ".Length).Trim();
            // }
            // return Ok(token);


        }

        [HttpGet]
        [Route("details")]

        public string Details()
        {
            return "With JWT details";
        }






        private string HashPassword(string password)
        {
            // Your password hashing logic here
            string hashedPassword = HashFunction(password); // Example hashing function

            // Return the hashed password
            return hashedPassword;
        }

        private string HashFunction(string password)
        {
            // Your actual hashing function implementation
            return "hashed_" + password;
        }
    }
}