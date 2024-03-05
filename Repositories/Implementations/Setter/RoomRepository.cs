using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models.Room;
using QuickQuiz.Repositories.Interfaces.ISetter;

namespace QuickQuiz.Repositories.Implementations.Setter
{
    public class RoomRepository : IRoomRepository
    {
        private readonly string _connectionString = "Server=(localdb)\\QuickQuiz; Database=QuickQuiz; Trusted_Connection=True;Encrypt=false; MultipleActiveResultSets=true";

        public async Task<List<ParticipantsModel>> AllParticipants()
        {
            List<ParticipantsModel> participants = new List<ParticipantsModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT UserID, Username FROM Users WHERE UserType = 'part'";
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            participants.Add(new ParticipantsModel
                            {
                                UserID = reader.GetInt32(0),
                                Username = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            return await Task.FromResult(participants);
        }

        public async Task<bool> createRoom(RoomModel roomModel)
        {

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = "INSERT INTO Room(RoomID, RoomName, SetterID) VALUES (@RoomID, @RoomName, @SetterId)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RoomID", roomModel.RoomID);
                        command.Parameters.AddWithValue("@RoomName", roomModel.RoomName);
                        command.Parameters.AddWithValue("@SetterId", roomModel.SetterID);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                return await Task.FromResult(true);
            }
            catch (System.Exception)
            {
                return await Task.FromResult(false);
            }

        }

        public async Task<List<int>> RoomParticipants(int roomID)
        {
            List<int> participants = new List<int>();
            using (var connection = new SqlConnection(_connectionString))
            {
                // var query = "SELECT u.UserID FROM RoomParticipant rp JOIN Users u ON rp.UserID = u.UserID WHERE rp.RoomID = @RoomID AND u.UserType = 'part'";
                var query = "SELECT UserID FROM RoomParticipant WHERE RoomID = @RoomID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomID);
                    connection.Open();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            participants.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return await Task.FromResult(participants);
        }

        public async Task<List<QuestionModel>> GetQuetions(int roomID)
        {
            if (isRoomActive(roomID) == false)
            {
                return null;
            }
            List<QuestionModel> questions = new List<QuestionModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Question WHERE RoomID = @RoomID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomID);
                    connection.Open();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            questions.Add(new QuestionModel
                            {
                                QuestionID = reader.GetInt32(0),
                                Question = reader.GetString(1),
                                Options = [.. reader.GetString(2).Split(',')],
                                Answer = reader.GetInt32(3),
                                RoomID = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }
            return await Task.FromResult(questions);
        }


        public async Task<GetParticipantsAnswerByIDModel> GetParticipantsAnswerByID(int participantsID, int roomID)
        {
            GetParticipantsAnswerByIDModel partipantsAnswer = new GetParticipantsAnswerByIDModel();
            List<QuestionAnswer> questionAnswers = new List<QuestionAnswer>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "select qq.QuestionID, qq.Content, qq.Options, qq.CorrectOption, ua.Answer from UserAnswer ua Join Question qq on qq.QuestionID = ua.QuestionID Where ua.RoomID = @RoomID AND ua.UserID = @UserID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RoomID", roomID);
                command.Parameters.AddWithValue("@UserID", participantsID);
                connection.Open();
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    questionAnswers.Add(new QuestionAnswer
                    {
                        QuestionID = reader.GetInt32(0),
                        Question = reader.GetString(1),
                        OptionsList = reader.GetString(2).Split(',').ToList(),
                        CorrectOption = reader.GetInt32(3),
                        UserAnswer = Convert.ToInt32(reader.GetString(4))
                    });
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

        public async Task<List<RoomResultModel>> GetRoomResult(int roomID)
        {
            List<RoomResultModel> roomResults = new List<RoomResultModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT ua.UserID, COUNT(CASE WHEN ua.Answer = q.CorrectOption THEN 1 ELSE NULL END) AS Score FROM UserAnswer ua JOIN Question q ON ua.QuestionID = q.QuestionID Where ua.RoomID = @RoomID GROUP BY ua.UserID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomID);
                    connection.Open();
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
            }

            return await Task.FromResult(roomResults);


        }

        public async Task<int> AddParticipants(AddParticipants addParticipants)
        {
            try
            {
                if (isSetter(addParticipants.SetterID) == false && isRoomAuthorized(addParticipants.RoomID, addParticipants.SetterID) == false)
                {
                    return await Task.FromResult(0);
                }
                foreach (var participant in addParticipants.Participants)
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {

                        var query = "INSERT INTO RoomParticipant(RoomID, UserID) VALUES (@RoomID, @UserID)";
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@RoomID", addParticipants.RoomID);
                            command.Parameters.AddWithValue("@UserID", participant);
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
                return await Task.FromResult(1);
            }
            catch (System.Exception)
            {
                return await Task.FromResult(2);
            }
        }

        public async Task<int> AddQuestions(AddQuestion addQuestion)
        {
            if (isRoomAuthorized(addQuestion.RoomID, addQuestion.SetterID) == false)
            {
                return await Task.FromResult(0);
            }
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var query = "INSERT INTO Question(QuestionID, Content, Options, CorrectOption, RoomID) VALUES (@QuestionID, @Content, @Options, @CorrectOption, @RoomID)";
                            foreach (var question in addQuestion.Questions)
                            {
                                using (var command = new SqlCommand(query, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@QuestionID", question.QuestionID); // Assuming QuestionID is already generated as an identity column
                                    command.Parameters.AddWithValue("@Content", question.Question);
                                    command.Parameters.AddWithValue("@Options", string.Join(",", question.Options));
                                    command.Parameters.AddWithValue("@CorrectOption", question.Answer);
                                    command.Parameters.AddWithValue("@RoomID", addQuestion.RoomID);
                                    command.ExecuteNonQuery();
                                    command.Parameters.Clear();
                                }
                            }
                            transaction.Commit();
                            return await Task.FromResult(1);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            // Handle exception
                            return await Task.FromResult(-1);
                        }
                    }
                }

            }
            catch (System.Exception)
            {
                return await Task.FromResult(2);
            }
        }

        public async Task<List<RoomModel>> RoomList(GetRoomListRequest getRoomListRequest)
        {
            List<RoomModel> rooms = new List<RoomModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Room WHERE SetterID = @SetterID";
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SetterID", getRoomListRequest.SetterID);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            RoomModel roomModel = new RoomModel();
                            roomModel.RoomID = reader.GetInt32(0);
                            roomModel.RoomName = reader.GetString(1);
                            roomModel.SetterID = reader.GetInt32(2);
                            var participants = new List<int>();
                            roomModel.Participants = participants;

                            var query2 = "SELECT UserID FROM RoomParticipant WHERE RoomID = @RoomID";
                            using (var command2 = new SqlCommand(query2, connection))
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
            }
            return rooms;
        }


        public async Task<int> StartQuiz(int roomID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query1 = "UPDATE Room SET RoomStatus = 1 WHERE RoomID = @RoomID";
                    // var query2 = "UPDATE Room SET RoomStatus = 0 WHERE RoomID = @RoomID";
                    using (var command1 = new SqlCommand(query1, connection))
                    {
                        command1.Parameters.AddWithValue("@RoomID", roomID);
                        connection.Open();
                        await command1.ExecuteNonQueryAsync();
                    }
                }
                return 1; // Success
            }
            catch (Exception)
            {
                return 2; // Error
            }
        }

        public async Task<int> PauseQuiz(int roomID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query1 = "UPDATE Room SET RoomStatus = 2 WHERE RoomID = @RoomID";
                    // var query2 = "UPDATE Room SET RoomStatus = 0 WHERE RoomID = @RoomID";
                    using (var command1 = new SqlCommand(query1, connection))
                    {
                        command1.Parameters.AddWithValue("@RoomID", roomID);
                        connection.Open();
                        await command1.ExecuteNonQueryAsync();
                    }
                }
                return 1; // Success
            }
            catch (Exception)
            {
                return 2; // Error
            }
        }

        public async Task<int> StopQuiz(int roomID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query1 = "UPDATE Room SET RoomStatus = 0 WHERE RoomID = @RoomID";
                    // var query2 = "UPDATE Room SET RoomStatus = 0 WHERE RoomID = @RoomID";
                    using (var command1 = new SqlCommand(query1, connection))
                    {
                        command1.Parameters.AddWithValue("@RoomID", roomID);
                        connection.Open();
                        await command1.ExecuteNonQueryAsync();
                    }
                }
                return 1; // Success
            }
            catch (Exception)
            {
                return 2; // Error
            }
        }


        private bool isSetter(int userID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Users WHERE UserID = @UserID AND UserType = 'setter'";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool isRoomActive(int roomID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Room WHERE RoomID = @RoomID AND RoomStatus = 1";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool isRoomAuthorized(int roomID, int userID)
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
                            return true;
                        }
                    }
                }
            }
            return false;
        }

    }
}