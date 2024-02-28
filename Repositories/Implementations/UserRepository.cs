using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Repositories.Interfaces;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Identity.Data;
using QuickQuiz.Models;

namespace QuickQuiz.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {

        private readonly string _connectionString = "Server=(localdb)\\QuickQuiz; Database=QuickQuiz; Trusted_Connection=True;Encrypt=false;";


        public bool Authenticate(LoginRequestModel loginRequest){

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", loginRequest.Username);
                    command.Parameters.AddWithValue("@PasswordHash", HashPassword(loginRequest.Password));
                    return (int)command.ExecuteScalar() > 0;
                }
            }

        }

        public bool Register(RegistrationRequest request){
          
           try{
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = "INSERT INTO Users (Username, PasswordHash) VALUES (@Username, @PasswordHash)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", request.Username);
                        command.Parameters.AddWithValue("@PasswordHash", HashPassword(request.Password));
                        connection.Open();  
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch(Exception ex){
                return false;
            }
            // return Task.CompletedTask;
        }


        public string HashPassword(string password)
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