using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using QuickQuiz.Models;
using QuickQuiz.Models.RequestModels;
using QuickQuiz.Models.Room;
using QuickQuiz.Repositories.Interfaces.ISetter;
using Dapper;

namespace QuickQuiz.Repositories.Implementations.Setter
{
    public class RoomRepository : IRoomRepository
    {

        private readonly string _connectionString; // = "Server=(localdb)\\QuickQuiz; Database=QQ; Trusted_Connection=True;Encrypt=false; MultipleActiveResultSets=true";

        public RoomRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task<bool> createRoom(RoomModel roomModel)
        {

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                        try
                        {
                            {
                                var query = @"
                                    INSERT INTO 
                                        Room(RoomName, SetterID, StartTime, RoomTypeID) 
                                    VALUES 
                                        (@RoomName, @SetterId, GETDATE(), 1)
                                    SELECT SCOPE_IDENTITY() AS NewUserID;    
                                    ";
                                using (var command = new SqlCommand(query, connection, transaction))
                                {
                                    // command.Parameters.AddWithValue("@RoomID", roomModel.RoomID);
                                    command.Parameters.AddWithValue("@RoomName", roomModel.RoomName);
                                    command.Parameters.AddWithValue("@SetterId", roomModel.UserID);
                                    // Log("Command Executing Started for createRoom");
                                    var roomId = await command.ExecuteScalarAsync();
                                    // Log("Command Executing for createRoom");
                                    roomModel.RoomID = Convert.ToInt32(roomId);
                                    // Console.WriteLine("Room ID: " + roomModel.RoomID);
                                    if (roomModel.Participants.Count > 0)
                                    {
                                        // Log("RoomModel Participants Count: " + roomModel.Participants.Count);
                                        foreach (var participant in roomModel.Participants)
                                        {
                                            var query2 = "INSERT INTO RoomParticipant(RoomID, UserID) VALUES (@RoomID, @UserID)";
                                            using (var command2 = new SqlCommand(query2, connection, transaction))
                                            {
                                                command2.Parameters.AddWithValue("@RoomID", roomModel.RoomID); // Use command2 here
                                                command2.Parameters.AddWithValue("@UserID", participant);

                                                await command2.ExecuteNonQueryAsync();

                                            }
                                        }
                                    }

                                }
                                transaction.Commit();
                                // Log("Transaction Committed for createRoom");
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log("Exception: " + ex);
                            transaction.Rollback();
                            // Log("Transaction Rollback for createRoom" + ex);
                            // Console.WriteLine("Exception: " + ex);
                            return await Task.FromResult(false);
                        }


                }
                return await Task.FromResult(true);
            }
            catch (System.Exception ex)
            {
                // Log("Exception: " + ex);
                // Console.WriteLine("Exception: " + ex);
                return await Task.FromResult(false);
            }

        }





     
        public async Task<List<RoomResultModel>> GetRoomResult(int roomID)
        {
            List<RoomResultModel> roomResults = new List<RoomResultModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(); // Asynchronously open the connection

                using (var transaction = connection.BeginTransaction()) // Begin a transaction
                {
                    try
                    {
                        var query = @"
                                    Select
                                        ua.UserID,
                                        Count(ua.UserID) as Score
                                    From
                                        UserAnswer ua
                                        Join Question qu on qu.QuestionID = ua.QuestionID
                                        Join RoomParticipant rp on ua.UserID = rp.UserID 
                                    Where
                                        ua.RoomID = 1
                                        And ua.Answer = qu.CorrectOption
                                        AND qu.IsDeleted = 0
                                        AND rp.IsDeleted = 0
                                    Group BY
                                        ua.UserID";

                        using (var command = new SqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@RoomID", roomID);

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    roomResults.Add(new RoomResultModel
                                    {
                                        UserID = reader.GetInt32(0),
                                        Marks = reader.GetInt32(1)
                                    });
                                }
                            }
                        }

                        transaction.Commit(); // Commit the transaction if everything succeeds
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); // Rollback the transaction if an exception occurs
                        Console.WriteLine("Exception: " + ex);
                        // Handle the exception appropriately, e.g., logging
                    }
                }
            }

            return roomResults;
        }





            

        
        
        public async Task<List<RoomModel>> RoomList(GetRoomListRequest getRoomListRequest)
        {
            List<RoomModel> rooms = new List<RoomModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Room WHERE SetterID = @SetterID and IsDeleted = 0";
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@SetterID", getRoomListRequest.UserID);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    RoomModel roomModel = new RoomModel();
                                    roomModel.RoomID = reader.GetInt32(0);
                                    roomModel.RoomName = reader.GetString(1);
                                    roomModel.UserID = reader.GetInt32(2);
                                    roomModel.StartTime = reader.GetDateTime(3);
                                    // roomModel.RoomTypeID = reader.GetInt32(4);
                                    roomModel.RoomStatus = reader.GetInt32(5);
                                    
                                    
                                    var participants = new List<int>();
                                    roomModel.Participants = participants;

                                    var query2 = "SELECT UserID FROM RoomParticipant WHERE RoomID = @RoomID";
                                    using (var command2 = new SqlCommand(query2, connection, transaction))
                                    {
                                        command2.Parameters.AddWithValue("@RoomID", roomModel.RoomID);
                                        using (var reader2 = await command2.ExecuteReaderAsync())
                                        {
                                            while (await reader2.ReadAsync())
                                            {
                                                participants.Add(reader2.GetInt32(0));
                                            }
                                        }
                                    }
                                    rooms.Add(roomModel);
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Exception: " + ex);
                    }
                }
            }
            return rooms;
        }


        public async Task<bool> StartQuiz(int roomID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var query = "UPDATE Room SET RoomStatus = 1 WHERE RoomID = @RoomID and IsDeleted = 0";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@RoomID", roomID);
                                await command.ExecuteNonQueryAsync();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return await Task.FromResult(false);
                        }
                    }
                }
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> PauseQuiz(int roomID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var query = "UPDATE Room SET RoomStatus = 2 WHERE RoomID = @RoomID and IsDeleted = 0";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@RoomID", roomID);
                                await command.ExecuteNonQueryAsync();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return await Task.FromResult(false);
                        }
                    }
                }
                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> StopQuiz(int roomID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var query = @"UPDATE 
                                        Room 
                                    SET 
                                        RoomStatus = 0 
                                    WHERE 
                                        RoomID = @RoomID and IsDeleted = 0;
                                        
                                   INSERT INTO QuizResult (UserID, QuizID, Marks)
                                    SELECT rp.UserID, @RoomID, COUNT(CASE WHEN ua.Answer = q.CorrectOption THEN 1 ELSE NULL END) AS Score
                                    FROM RoomParticipant rp
                                    JOIN UserAnswer ua ON rp.UserID = ua.UserID
                                    JOIN Question q ON ua.QuestionID = q.QuestionID
                                    WHERE q.RoomID = $RoomID and q.IsDeleted = 0
                                    GROUP BY rp.UserID;

                                        ";
                            using (var command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@RoomID", roomID);
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }

                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return await Task.FromResult(true);
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            return await Task.FromResult(true);

        }



        public async Task<bool> RoomDelete(int roomID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var query = "Update ROOM SET IsDeleted = 1 WHERE RoomID = @RoomID";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@RoomID", roomID);
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return await Task.FromResult(false);
                        }
                    }
                }
                return await Task.FromResult(true);
            }
            catch (System.Exception)
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<bool> RoomUpdate(RoomUpdateModel roomModel)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var query = @"UPDATE 
                                    Room 
                                SET 
                                    RoomName = @RoomName, 
                                    SetterID = @SetterID, 
                                    StartTime = @StartTime, 
                                    RoomTypeID = @RoomTypeID, 
                                    RoomStatus = @RoomStatus 
                                WHERE 
                                    RoomID = @RoomID";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@RoomID", roomModel.RoomID);
                                command.Parameters.AddWithValue("@RoomName", roomModel.RoomName);
                                command.Parameters.AddWithValue("@SetterID", roomModel.UserID);
                                command.Parameters.AddWithValue("@StartTime", roomModel.StartTime);
                                command.Parameters.AddWithValue("@RoomTypeID", roomModel.RoomTypeID);
                                command.Parameters.AddWithValue("@RoomStatus", roomModel.RoomStatus);
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return await Task.FromResult(false);
                        }
                    }
                }
                return await Task.FromResult(true);
            }
            catch (System.Exception)
            {
                return await Task.FromResult(false);
            }
        }
        
                public Task<bool> isSetter(int userID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Users WHERE UserID = @UserID AND UserType = 0";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return Task.FromResult(true);
                        }
                    }
                }
            }
            return Task.FromResult(false);
        }

        public Task<bool> isRoomSetter(int roomID, int setterID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Room WHERE SetterID = @UserID AND RoomID = @RoomID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", setterID);
                    command.Parameters.AddWithValue("@RoomID", roomID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return Task.FromResult(true);
                        }
                    }
                }
            }
            return Task.FromResult(false);
        }

        public Task<bool> isRoomActive(int roomID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                // _logger.commit("Checking if room is active");
                var query = "SELECT * FROM Room WHERE RoomID = @RoomID AND RoomStatus = 1";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return Task.FromResult(true);
                        }
                    }
                }
            }
            return Task.FromResult(false);
        }

        public Task<bool> isMember(int roomID, int userID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM RoomParticipant WHERE RoomID = @RoomID AND UserID = @UserID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomID);
                    command.Parameters.AddWithValue("@UserID", userID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return Task.FromResult(true);
                        }
                    }
                }
            }
            return Task.FromResult(false);
        }

        public Task<bool> isRoomAuthorized(int roomID, int userID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Room WHERE RoomID = @RoomID AND SetterID = @UserID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomID);
                    command.Parameters.AddWithValue("@UserID", userID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return Task.FromResult(true);
                        }
                    }
                }
            }
            return Task.FromResult(false);
        }

    }
}

