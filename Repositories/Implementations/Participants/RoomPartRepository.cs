using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using QuickQuiz.Models.Room;
using QuickQuiz.Repositories.Interfaces.IParticipants;

namespace QuickQuiz.Repositories.Implementations.Participants
{
    public class RoomPartRepository : IRoomPartRepository
    {
        private readonly string _connectionString = "Server=(localdb)\\QuickQuiz; Database=QuickQuiz; Trusted_Connection=True;Encrypt=false;";

        public async Task<List<QuestionModelParticipant>> GetQuestions(int roomID)
        {
            List<QuestionModelParticipant> questions = new List<QuestionModelParticipant>();
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
                            questions.Add(new QuestionModelParticipant
                            {
                                QuestionID = reader.GetInt32(0),
                                Question = reader.GetString(1),
                                Options = [.. reader.GetString(2).Split(',')],
                                // Answer = reader.GetInt32(3),
                                RoomID = reader.GetInt32(4)
                            });
                        }
                    }
                }
            }
            return await Task.FromResult(questions);
        }

        public async Task<RoomAnswerSubmitModel> SubmitAnswer(RoomAnswerSubmitModel roomAnswerSubmitModel)
        {
            try
            {
                foreach (var item in roomAnswerSubmitModel.Answers)
                {
                    using (var connection = new SqlConnection(_connectionString))
                    {
                        var query = "INSERT INTO UserAnswer(RoomID, UserID, QuestionID, Answer) VALUES (@RoomID, @UserID, @QuestionID, @Answer)";
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@RoomID", roomAnswerSubmitModel.RoomID);
                            command.Parameters.AddWithValue("@UserID", roomAnswerSubmitModel.UserID);
                            command.Parameters.AddWithValue("@QuestionID", item[0]);
                            command.Parameters.AddWithValue("@Answer", item[1]);
                            connection.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
                return await Task.FromResult(roomAnswerSubmitModel);
            }
            catch (System.Exception)
            {

                roomAnswerSubmitModel.RoomID = -1;
                return roomAnswerSubmitModel;
            }

        }

    }
}