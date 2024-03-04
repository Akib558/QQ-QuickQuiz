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
        private readonly string _connectionString = "Server=(localdb)\\QuickQuiz; Database=QuickQuiz; Trusted_Connection=True;Encrypt=false;";

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

    }
}