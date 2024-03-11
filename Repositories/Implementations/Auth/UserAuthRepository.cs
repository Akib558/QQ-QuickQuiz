using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Repositories.Interfaces;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Identity.Data;
using QuickQuiz.Models;
// using QuickQuiz.Models.Auth;
// using QuickQuiz.Models.Auth;
// using QuickQuiz.Models.Auth;

namespace QuickQuiz.Repositories.Implementations
{
    public class UserAuthRepository : IUserAuthRepository
    {

        private readonly string _connectionString = "Server=(localdb)\\QuickQuiz; Database=QQ; Trusted_Connection=True;Encrypt=false;";


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


        public async Task<bool> Logout(String tokenString)
        {
            try
            {
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
            catch (Exception ex)
            {
                return await Task.FromResult(false);
            }
        }


        public async Task<bool> Register(RegistrationRequestModel request)
        {
            Console.WriteLine(1);

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = @"INSERT INTO 
                            Users (Username, [Email], [Password], [UserType], [ActiveStatus]) 
                        VALUES 
                            (@Username, @Email, @PasswordHash, @UserType, @ActiveStatus)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", request.Username);
                        command.Parameters.AddWithValue("@Email", request.Email);
                        command.Parameters.AddWithValue("@PasswordHash", request.Password);
                        command.Parameters.AddWithValue("@UserType", request.UserType);
                        command.Parameters.AddWithValue("@ActiveStatus", request.ActiveStatus);

                        await connection.OpenAsync();
                        Console.WriteLine(2);
                        var result = await command.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine("Error occurred during registration: " + ex.Message);
                return false;
            }
        }



        public async Task<string> HashPassword(string password)
        {
            string hashedPassword = HashFunction(password);
            return await Task.FromResult(hashedPassword);
        }

        private string HashFunction(string password)
        {

            return "hashed_" + password;
        }

    }
}