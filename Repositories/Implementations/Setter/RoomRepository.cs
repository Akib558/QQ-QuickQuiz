using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using QuickQuiz.Models;
using QuickQuiz.Models.Room;
using QuickQuiz.Repositories.Interfaces.ISetter;

namespace QuickQuiz.Repositories.Implementations.Setter
{
    public class RoomRepository : IRoomRepository
    {
        private readonly string _connectionString = "Server=(localdb)\\QuickQuiz; Database=QQ; Trusted_Connection=True;Encrypt=false; MultipleActiveResultSets=true";

        public async Task<List<ParticipantsModel>> AllParticipants()
        {
            List<ParticipantsModel> participants = new List<ParticipantsModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {

                        var query = "SELECT UserID, Username FROM Users WHERE UserType = 1";
                        using (var command = new SqlCommand(query, connection, transaction))
                        {
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
                                    command.Parameters.AddWithValue("@SetterId", roomModel.SetterID);
                                    var roomId = await command.ExecuteScalarAsync();
                                    roomModel.RoomID = Convert.ToInt32(roomId);
                                    Console.WriteLine("Room ID: " + roomModel.RoomID);
                                    if (roomModel.Participants.Count > 0)
                                    {

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
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine("Exception: " + ex);
                            return await Task.FromResult(false);
                        }


                }
                return await Task.FromResult(true);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
                return await Task.FromResult(false);
            }

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
                        using (var command = new SqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@RoomID", roomID);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    participants.Add(reader.GetInt32(0));
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
            return await Task.FromResult(participants);
        }

        public async Task<List<GetQuestionModel>> GetQuetions(int roomID)
        {
            List<GetQuestionModel> questions = new List<GetQuestionModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "SELECT * FROM Question WHERE RoomID = @RoomID";
                        using (var command = new SqlCommand(query, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@RoomID", roomID);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    GetQuestionModel questionModel = new GetQuestionModel();
                                    questionModel.QuestionID = reader.GetInt32(0);
                                    questionModel.Question = reader.GetString(1);
                                    questionModel.Answer = reader.GetInt32(2);
                                    questionModel.RoomID = reader.GetInt32(3);

                                    // Fetch options for this question
                                    // var options = new List<(int, string)>();
                                    var options = new List<OptionModel>();
                                    var query2 = "SELECT OptionID, Options FROM QuestionOptions WHERE QuestionID = @QuestionID";
                                    using (var command2 = new SqlCommand(query2, connection, transaction))
                                    {
                                        command2.Parameters.AddWithValue("@QuestionID", questionModel.QuestionID);
                                        using (var reader2 = await command2.ExecuteReaderAsync())
                                        {
                                            while (await reader2.ReadAsync())
                                            {
                                                Console.WriteLine($"Option ID: {reader2.GetInt32(0)}, Option: {reader2.GetString(1)}");
                                                options.Add(new OptionModel
                                                {
                                                    OptionID = reader2.GetInt32(0),
                                                    Option = reader2.GetString(1)
                                                });
                                            }
                                        }
                                    }
                                    questionModel.Options = options;

                                    Console.WriteLine($"Question: {questionModel.Question}, Options Count: {options.Count}");
                                    questions.Add(questionModel);
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
            return await Task.FromResult(questions);
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
                                qq.QuestionID, qq.Content, qo.Options, qq.CorrectOption, ua.Answer as UserAnswers 
                            from 
                                UserAnswer ua 
                                Join Question qq on qq.QuestionID = ua.QuestionID 
                                join QuestionOptions qo on qo.QuestionID = qq.QuestionID
                            Where 
                                ua.RoomID = @RoomID AND ua.UserID = @UserID";
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
                                    questionAnswer.CorrectOption = reader.GetInt32(3);
                                    questionAnswer.UserAnswer = Convert.ToInt32(reader.GetString(4));
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
                        var query = @"SELECT 
                                ua.UserID, COUNT(CASE WHEN ua.Answer = q.CorrectOption THEN 1 ELSE NULL END) AS Score 
                            FROM 
                                UserAnswer ua JOIN Question q ON ua.QuestionID = q.QuestionID 
                            WHERE 
                                q.RoomID = @RoomID GROUP BY ua.UserID";

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


        public async Task<int> AddQuestions(AddQuestion addQuestion)
        {
            // if (isRoomAuthorized(addQuestion.RoomID, addQuestion.SetterID) == false)
            // {
            //     return await Task.FromResult(0);
            // }
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var query = "INSERT INTO Question(Content, CorrectOption, RoomID) VALUES (@Content, @CorrectOption, @RoomID)";
                            foreach (var question in addQuestion.Questions)
                            {
                                using (var command = new SqlCommand(query, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@Content", question.Question);
                                    command.Parameters.AddWithValue("@CorrectOption", question.Answer);
                                    command.Parameters.AddWithValue("@RoomID", addQuestion.RoomID);
                                    command.ExecuteNonQuery();
                                    command.Parameters.Clear();
                                }
                                var query2 = "Select QuestionID from Question where Content = @Content and RoomID = @RoomID";
                                using (var command2 = new SqlCommand(query2, connection, transaction))
                                {
                                    command2.Parameters.AddWithValue("@Content", question.Question);
                                    command2.Parameters.AddWithValue("@RoomID", addQuestion.RoomID);
                                    using (var reader = command2.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            var questionID = reader.GetInt32(0);
                                            var query3 = "INSERT INTO QuestionOptions(QuestionID, Options) VALUES (@QuestionID, @Options)";
                                            foreach (var option in question.Options)
                                            {
                                                using (var command3 = new SqlCommand(query3, connection, transaction))
                                                {
                                                    command3.Parameters.AddWithValue("@QuestionID", questionID);
                                                    command3.Parameters.AddWithValue("@Options", option);
                                                    command3.ExecuteNonQuery();
                                                    command3.Parameters.Clear();
                                                }
                                            }
                                        }
                                    }
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
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand(query, connection, transaction))
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


        public async Task<int> StartQuiz(int roomID)
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
                            var query = "UPDATE Room SET RoomStatus = 1 WHERE RoomID = @RoomID";
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
                            return await Task.FromResult(2);
                        }
                    }
                }
                return 1;
            }
            catch (Exception)
            {
                return 2;
            }
        }

        public async Task<int> PauseQuiz(int roomID)
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
                            var query = "UPDATE Room SET RoomStatus = 2 WHERE RoomID = @RoomID";
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
                            return await Task.FromResult(2);
                        }
                    }
                }
                return 1;
            }
            catch (Exception)
            {
                return 2;
            }
        }

        public async Task<int> StopQuiz(int roomID)
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
                                        RoomID = @RoomID;
                                        
                                   INSERT INTO QuizResult (UserID, QuizID, Marks)
                                    SELECT rp.UserID, @RoomID, COUNT(CASE WHEN ua.Answer = q.CorrectOption THEN 1 ELSE NULL END) AS Score
                                    FROM RoomParticipant rp
                                    JOIN UserAnswer ua ON rp.UserID = ua.UserID
                                    JOIN Question q ON ua.QuestionID = q.QuestionID
                                    WHERE q.RoomID = $RoomID
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
                            return await Task.FromResult(2);
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            return await Task.FromResult(1);

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
                            var query = "DELETE FROM RoomParticipant WHERE RoomID = @RoomID";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@RoomID", roomID);
                                command.ExecuteNonQuery();
                            }
                            var query2 = "DELETE FROM UserAnswer WHERE RoomID = @RoomID";
                            using (var command2 = new SqlCommand(query2, connection, transaction))
                            {
                                command2.Parameters.AddWithValue("@RoomID", roomID);
                                command2.ExecuteNonQuery();
                            }
                            var query3 = "DELETE FROM QuestionOptions WHERE QuestionID IN (SELECT QuestionID FROM Question WHERE RoomID = @RoomID)";
                            using (var command3 = new SqlCommand(query3, connection, transaction))
                            {
                                command3.Parameters.AddWithValue("@RoomID", roomID);
                                command3.ExecuteNonQuery();
                            }
                            var query4 = "DELETE FROM Question WHERE RoomID = @RoomID";
                            using (var command4 = new SqlCommand(query4, connection, transaction))
                            {
                                command4.Parameters.AddWithValue("@RoomID", roomID);
                                command4.ExecuteNonQuery();
                            }
                            var query5 = "DELETE FROM Room WHERE RoomID = @RoomID";
                            using (var command5 = new SqlCommand(query5, connection, transaction))
                            {
                                command5.Parameters.AddWithValue("@RoomID", roomID);
                                command5.ExecuteNonQuery();
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
                                command.Parameters.AddWithValue("@SetterID", roomModel.SetterID);
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

        public async Task<bool> QuestionDelete(int questionID)
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
                            var query = "DELETE FROM UserAnswer WHERE QuestionID = @QuestionID";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@QuestionID", questionID);
                                command.ExecuteNonQuery();
                            }
                            var query2 = "DELETE FROM QuestionOptions WHERE QuestionID = @QuestionID";
                            using (var command2 = new SqlCommand(query2, connection, transaction))
                            {
                                command2.Parameters.AddWithValue("@QuestionID", questionID);
                                command2.ExecuteNonQuery();
                            }
                            var query3 = "DELETE FROM Question WHERE QuestionID = @QuestionID";
                            using (var command3 = new SqlCommand(query3, connection, transaction))
                            {
                                command3.Parameters.AddWithValue("@QuestionID", questionID);
                                command3.ExecuteNonQuery();
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

        public async Task<bool> QuestionUpdate(UpdateQuestionModel questionModel)
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
                            var query = "UPDATE Question SET Content = @Content, CorrectOption = @CorrectOption, RoomID = @RoomID WHERE QuestionID = @QuestionID";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@QuestionID", questionModel.QuestionID);
                                command.Parameters.AddWithValue("@Content", questionModel.Question);
                                command.Parameters.AddWithValue("@CorrectOption", questionModel.Answer);
                                command.Parameters.AddWithValue("@RoomID", questionModel.RoomID);
                                command.ExecuteNonQuery();
                            }

                            var query2 = "UPDATE QuestionOptions Set QuestionID = @QuestioNID, Options = @Options Where OptionID = @OptionID";
                            foreach (var option in questionModel.Options)
                            {
                                using (var command3 = new SqlCommand(query2, connection, transaction))
                                {
                                    command3.Parameters.AddWithValue("@OptionID", option.OptionID);
                                    command3.Parameters.AddWithValue("@QuestionID", questionModel.QuestionID);
                                    command3.Parameters.AddWithValue("@Options", option.Option);
                                    command3.ExecuteNonQuery();
                                    Console.WriteLine(option.Option);
                                    command3.Parameters.Clear();
                                }
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


        public async Task<bool> RoomParticipantDelete(int roomID, int ParticiapntsID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        var query2 = "DELETE FROM UserAnswer WHERE UserID = @UserID AND RoomID = @RoomID";
                        using (var command2 = new SqlCommand(query2, connection, transaction))
                        {
                            command2.Parameters.AddWithValue("@RoomID", roomID);
                            command2.Parameters.AddWithValue("@UserID", ParticiapntsID);
                            command2.ExecuteNonQuery();
                        }
                        var query3 = "DELETE FROM QuizResult WHERE UserID = @UserID AND QuizID = @RoomID";
                        using (var command3 = new SqlCommand(query3, connection, transaction))
                        {
                            command3.Parameters.AddWithValue("@RoomID", roomID);
                            command3.Parameters.AddWithValue("@UserID", ParticiapntsID);
                            command3.ExecuteNonQuery();
                        }
                        var query4 = "DELETE FROM RoomParticipant WHERE RoomID = @RoomID AND UserID = @UserID";
                        using (var command4 = new SqlCommand(query4, connection, transaction))
                        {
                            command4.Parameters.AddWithValue("@RoomID", roomID);
                            command4.Parameters.AddWithValue("@UserID", ParticiapntsID);
                            command4.ExecuteNonQuery();
                        }
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

        private bool isSetter(int userID)
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