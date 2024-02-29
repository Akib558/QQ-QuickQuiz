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


        public async Task<bool> Authenticate(LoginRequestModel loginRequest)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", loginRequest.Username);
                    command.Parameters.AddWithValue("@PasswordHash", await HashPassword(loginRequest.Password));
                    var result = await command.ExecuteScalarAsync();

                    int count = Convert.ToInt32(result);
                    return count > 0;
                }
            }
        }


        public async Task<bool> Logout(String tokenString){
            try{
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = "INSERT INTO BlackTokenString (tokenString) VALUES (@tokenString)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@tokenString", tokenString);
                        connection.Open();  
                        command.ExecuteNonQuery();
                    }
                }
                return await Task.FromResult(true);
            }
            catch(Exception ex){
                return await Task.FromResult(false);
            }
        }


        public async Task<bool> Register(RegistrationRequest request){
          
           try{
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = "INSERT INTO Users (UserID, Username, PasswordHash, UserType) VALUES (@UserID ,@Username, @PasswordHash, @UserType)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", request.UserID);
                        command.Parameters.AddWithValue("@Username", request.Username);
                        command.Parameters.AddWithValue("@PasswordHash", await HashPassword(request.Password));
                        command.Parameters.AddWithValue("@UserType", request.UserType);
                        // connection.Open();  
                        await connection.OpenAsync();
                        var result = await command.ExecuteNonQueryAsync();
                    }
                }
                return await Task.FromResult(true);
            }
            catch(Exception ex){
                return await Task.FromResult(false);
            }
        }


        public async Task<string> HashPassword(string password)
        {
            string hashedPassword = HashFunction(password);
            return await Task.FromResult(hashedPassword);
        }

        private string HashFunction(string password){

            return "hashed_" + password;
        }

    }
}