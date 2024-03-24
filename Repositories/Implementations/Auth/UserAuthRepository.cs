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


        public async Task<int> Authenticate(LoginRequestModel loginRequest)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT UserID FROM Users WHERE Username = @Username AND Password = @PasswordHash";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", loginRequest.Username);
                    command.Parameters.AddWithValue("@PasswordHash", await HashPassword(loginRequest.Password));
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }

            return 0;
        }
        
        public async Task<bool> SetActive(int userID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE Users SET ActiveStatus = 1 WHERE UserID = @UserID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    var result = await command.ExecuteNonQueryAsync();

                    return result > 0;
                }
            }
        }
        
        
        public async Task<bool> IsActive(int userID)
        {
            Console.WriteLine(userID);
            Console.WriteLine("Enter isActive Function");
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT ActiveStatus FROM Users WHERE UserID = @UserID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    var result = await command.ExecuteScalarAsync();

                    int count = Convert.ToInt32(result);
                    return count > 0;
                }
            }
        }


        public async Task<bool> Logout(String tokenString, int userID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = @"INSERT INTO BlackTokenString (tokenString) VALUES (@tokenString)
                                UPDATE Users SET ActiveStatus = 0 WHERE UserID = @UserID";
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

            return password;
            // return "hashed_" + password;
        }

    }
}