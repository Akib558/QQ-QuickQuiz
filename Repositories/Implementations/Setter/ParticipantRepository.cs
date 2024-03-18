using System.Data.SqlClient;
using Dapper;
using QuickQuiz.Models;
using QuickQuiz.Models.RequestModels;
using QuickQuiz.Models.Room;
using QuickQuiz.Repositories.Interfaces.ISetter;

namespace QuickQuiz.Repositories.Implementations.Setter;

public class ParticipantRepository : IParticipantRepository
{
    private string _connectionString;

    public ParticipantRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<ParticipantsModel>> AllParticipants()
    {
        List<ParticipantsModel> participants = new List<ParticipantsModel>();
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = "SELECT UserID, Username FROM Users WHERE UserType = 1 AND IsDeleted = 0";
                participants = (await connection.QueryAsync<ParticipantsModel>(query)).ToList();

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex);
        }

        return participants;
    }
    
    public async Task<List<int>> RoomParticipants(int roomID)
    {
        List<int> participants = new List<int>();
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var query = "SELECT UserID FROM RoomParticipant WHERE RoomID = @RoomID";
                    participants = (await connection.QueryAsync<int>(query, new { RoomID = roomID }, transaction)).ToList();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Exception: " + ex);
                }
            }
        }
        return await Task.FromResult(participants);
    }

    
       public async Task<GetParticipantsAnswerByIDModel> GetParticipantsAnswerByID(int participantsID, int roomID)
        {
            GetParticipantsAnswerByIDModel partipantsAnswer = new GetParticipantsAnswerByIDModel();
            List<QuestionAnswer> questionAnswers = new List<QuestionAnswer>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = @"select 
                                        qq.QuestionID, qq.Content, qq.CorrectOption, ua.Answer  
                                    from 
                                        UserAnswer ua 
                                        Join Question qq on qq.QuestionID = ua.QuestionID 
                                    Where 
                                        ua.RoomID = @RoomID AND ua.UserID = @UserID AND qq.IsDeleted = 0";
                        using (var command = new SqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@RoomID", roomID);
                            command.Parameters.AddWithValue("@UserID", participantsID);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    QuestionAnswer questionAnswer = new QuestionAnswer();
                                    questionAnswer.QuestionID = reader.GetInt32(0);
                                    questionAnswer.Question = reader.GetString(1);
                                    questionAnswer.CorrectOption = reader.GetInt32(2);
                                    questionAnswer.UserAnswer = Convert.ToInt32(reader.GetString(3));
                                    // questionAnswer.UserAnswer = reader.GetInt32(3);
                                    var options = new List<String>();
                                    questionAnswer.OptionsList = options;
                                    var query2 = "Select Options from QuestionOptions where QuestionID = @QuestionID";
                                    using (var command2 = new SqlCommand(query2, connection, transaction))
                                    {
                                        command2.Parameters.AddWithValue("@QuestionID", questionAnswer.QuestionID);
                                        using (var reader2 = await command2.ExecuteReaderAsync())
                                        {
                                            while (await reader2.ReadAsync())
                                            {
                                                options.Add(reader2.GetString(0));
                                            }
                                        }
                                    }
                                    questionAnswers.Add(questionAnswer);
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
            partipantsAnswer.UseID = participantsID;
            partipantsAnswer.RoomID = roomID;
            partipantsAnswer.Questions = questionAnswers;
            return await Task.FromResult(partipantsAnswer);
        }

        public async Task<List<GetParticipantsAnswerByIDModel>> GetParticipantsAnswerByRoom(int roomID)
        {
            List<GetParticipantsAnswerByIDModel> participantsAnswers = new List<GetParticipantsAnswerByIDModel>();
            List<int> participants = await RoomParticipants(roomID);
            foreach (var participant in participants)
            {
                participantsAnswers.Add(await GetParticipantsAnswerByID(participant, roomID));
            }

            return await Task.FromResult(participantsAnswers);
        }
        
        
        public async Task<int> AddParticipants(AddParticipants addParticipants)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var participant in addParticipants.Participants)
                            {
                                var query = "INSERT INTO RoomParticipant(RoomID, UserID) VALUES (@RoomID, @UserID)";
                                using (var command = new SqlCommand(query, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@RoomID", addParticipants.RoomID);
                                    command.Parameters.AddWithValue("@UserID", participant);
                                    await command.ExecuteNonQueryAsync();
                                }
                            }
                            transaction.Commit();
                            return 1;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine("Exception Message: " + ex);
                            return 2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Message: " + ex);
                return 3;
            }
        }
    
        
        public async Task<bool> RoomParticipantDelete(int roomID, int ParticiapntsID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var query2 = "Update RoomParticipant Set IsDeleted = 1 WHERE UserID = @UserID AND RoomID = @RoomID";
                        using (var command2 = new SqlCommand(query2, connection, transaction))
                        {
                            command2.Parameters.AddWithValue("@RoomID", roomID);
                            command2.Parameters.AddWithValue("@UserID", ParticiapntsID);
                            command2.ExecuteNonQuery();
                        }
                        // var query3 = "DELETE FROM QuizResult WHERE UserID = @UserID AND QuizID = @RoomID";
                        // using (var command3 = new SqlCommand(query3, connection, transaction))
                        // {
                        //     command3.Parameters.AddWithValue("@RoomID", roomID);
                        //     command3.Parameters.AddWithValue("@UserID", ParticiapntsID);
                        //     command3.ExecuteNonQuery();
                        // }
                        // var query4 = "DELETE FROM RoomParticipant WHERE RoomID = @RoomID AND UserID = @UserID";
                        // using (var command4 = new SqlCommand(query4, connection, transaction))
                        // {
                        //     command4.Parameters.AddWithValue("@RoomID", roomID);
                        //     command4.Parameters.AddWithValue("@UserID", ParticiapntsID);
                        //     command4.ExecuteNonQuery();
                        // }
                        transaction.Commit();
                    }

                }
                return await Task.FromResult(true);
            }
            catch (System.Exception)
            {
                return await Task.FromResult(false);
            }
        }

        public async Task<Participant> GetParticipantInfoByID(int roomID, int participantID)
        {
            Participant participant = new Participant();
            participant.UserID = participantID;
            participant.RoomID = roomID;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "SELECT Marks FROM QuizResult WHERE UserID = @UserID AND QuizID = @RoomID";
                        using (var command = new SqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@UserID", participantID);
                            command.Parameters.AddWithValue("@RoomID", roomID);
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    participant.Marks = reader.GetInt32(0);
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
            participant.Questions = GetParticipantsAnswerByID(participantID, roomID).Result.Questions;
            return await Task.FromResult(participant);
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