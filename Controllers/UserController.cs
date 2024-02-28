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

        public UserController(IUserAuthService userAuthService){
            _userAuthService = userAuthService;
        }

        [Authorize]
        [HttpGet]
        [Route("getdata")]
        public IActionResult GetData(){

                List<User> todos = new List<User>();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "SELECT Id, Username, PasswordHash FROM Users";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            User todo = new User
                            {
                                Id =Convert.ToInt32(reader["Id"]),
                                Username = Convert.ToString(reader["Username"]),
                                PasswordHash = Convert.ToString(reader["PasswordHash"])

                            };
                            todos.Add(todo);
                        }
                    }
                }

                return Ok(todos);
          
            
        }

        [HttpGet]
        [Route("details")]

        public string Details(){
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