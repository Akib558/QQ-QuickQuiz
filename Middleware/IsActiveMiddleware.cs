using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace YourNamespace
{
    public class IsActiveMiddleware
    {
        private readonly RequestDelegate _next;

        public IsActiveMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string token = context.Request.Headers.Authorization;

            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            bool isActive = CheckIsActive(token);

            PathString path = context.Request.Path;

            if (path.Value == "/api/auth/login" || path.Value == "/api/auth/login")
            {
                await _next(context);
                return;
            }
            if (!isActive)
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("User is not active.");
                return;
            }

            // if (path.StartsWithSegments("/api/participants"))
            // {
            //     // Extract participant ID from the path
            //     string[] segments = path.Value.Split('/');
            //     if (segments.Length >= 3)
            //     {
            //         string userId = segments[2];
            //         s // Assuming participant ID is the third segment
            //         // Check if the user has access to the participant ID
            //         int userId = int.Parse(context.Request.Form["UserID"]);
            //         bool hasAccess = CheckParticipantAccess(userId, roomId);
            //         if (!hasAccess)
            //         {
            //             // Redirect to other page or URL
            //             context.Response.Redirect("/otherpage");
            //             return;
            //         }
            //     }
            // }


            await _next(context);
        }

        private bool CheckIsActive(string tokenString)
        {
            string _connectionString = "Server=(localdb)\\QuickQuiz; Database=QuickQuiz; Trusted_Connection=True;Encrypt=false;";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT * FROM BlackTokenString WHERE tokenString = @tokenString", connection))
                {
                    command.Parameters.AddWithValue("@tokenString", tokenString);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    return !reader.HasRows;
                }
            }
        }

        private bool CheckParticipantAccess(int userID, string roomID)
        {
            string _connectionString = "Server=(localdb)\\QuickQuiz; Database=QuickQuiz; Trusted_Connection=True;Encrypt=false;";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT * FROM RoomParticipant WHERE UserID = @userID and RoomID = @roomID", connection))
                {
                    command.Parameters.AddWithValue("@userID", userID);
                    command.Parameters.AddWithValue("@roomID", roomID);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    return !reader.HasRows;
                }
            }
        }
    }
}
