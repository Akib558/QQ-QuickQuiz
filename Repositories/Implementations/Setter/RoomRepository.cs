using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
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
                var query = "SELECT UserID, Username FROM Users WHERE UserType = 1";
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
                    var query = "INSERT INTO Room(RoomName, SetterID, StartTime, RoomTypeID) VALUES (@RoomName, @SetterId, GETDATE(), 1)";
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

        public async Task<List<GetQuestionModel>> GetQuetions(int roomID)
        {
            List<GetQuestionModel> questions = new List<GetQuestionModel>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Question WHERE RoomID = @RoomID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomID);
                    await connection.OpenAsync();
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
                            using (var command2 = new SqlCommand(query2, connection))
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

                            // Assign fetched options to questionModel
                            questionModel.Options = options;

                            Console.WriteLine($"Question: {questionModel.Question}, Options Count: {options.Count}");


                            // Add question to the list
                            questions.Add(questionModel);
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
                var query = @"select 
                                qq.QuestionID, qq.Content, qo.Options, qq.CorrectOption, ua.Answer as UserAnswers 
                            from 
                                UserAnswer ua 
                                Join Question qq on qq.QuestionID = ua.QuestionID 
                                join QuestionOptions qo on qo.QuestionID = qq.QuestionID
                            Where 
                                ua.RoomID = @RoomID AND ua.UserID = @UserID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RoomID", roomID);
                command.Parameters.AddWithValue("@UserID", participantsID);
                connection.Open();
                var reader = await command.ExecuteReaderAsync();
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
                    using (var command2 = new SqlCommand(query2, connection))
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
                var query = @"SELECT 
                                ua.UserID, COUNT(CASE WHEN ua.Answer = q.CorrectOption THEN 1 ELSE NULL END) AS Score 
                            FROM 
                                UserAnswer ua JOIN Question q ON ua.QuestionID = q.QuestionID 
                            Where 
                                q.RoomID = @RoomID GROUP BY ua.UserID";

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
                    var query = "DELETE FROM Room WHERE RoomID = @RoomID";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RoomID", roomID);
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

        public async Task<bool> RoomUpdate(RoomUpdateModel roomModel)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
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
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RoomID", roomModel.RoomID);
                        command.Parameters.AddWithValue("@RoomName", roomModel.RoomName);
                        command.Parameters.AddWithValue("@SetterID", roomModel.SetterID);
                        command.Parameters.AddWithValue("@StartTime", roomModel.StartTime);
                        command.Parameters.AddWithValue("@RoomTypeID", roomModel.RoomTypeID);
                        command.Parameters.AddWithValue("@RoomStatus", roomModel.RoomStatus);
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

        public async Task<bool> QuestionDelete(int questionID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = @"
                    DELETE FROM UserAnswer WHERE QuestionID = @QuestionID;
                    DELETE FROM QuestionOptions WHERE QuestionID = @QuestionID;
                    DELETE FROM Question WHERE QuestionID = @QuestionID";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@QuestionID", questionID);
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
                    var query = "DELETE FROM RoomParticipant WHERE RoomID = @RoomID AND UserID = @UserID";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@RoomID", roomID);
                        command.Parameters.AddWithValue("@UserID", ParticiapntsID);
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

        public async Task<Participant> GetParticipantInfoByID(int roomID, int participantID)
        {
            Participant participant = new Participant();
            participant.UserID = participantID;
            participant.RoomID = roomID;
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Marks FROM QuizResult WHERE UserID = @UserID AND QuizID = @RoomID";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", participantID);
                    command.Parameters.AddWithValue("@RoomID", roomID);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            participant.Marks = reader.GetInt32(0);
                        }
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