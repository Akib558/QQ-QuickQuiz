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
            if (QuizEnded(roomAnswerSubmitModel.RoomID))
            {
                roomAnswerSubmitModel.RoomID = -1;
                return roomAnswerSubmitModel;
            }
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var item in roomAnswerSubmitModel.Answers)
                            {
                                var query = "INSERT INTO UserAnswer(RoomID, UserID, QuestionID, Answer) VALUES (@RoomID, @UserID, @QuestionID, @Answer)";
                                using (var command = new SqlCommand(query, connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@RoomID", roomAnswerSubmitModel.RoomID);
                                    command.Parameters.AddWithValue("@UserID", roomAnswerSubmitModel.UserID);
                                    command.Parameters.AddWithValue("@QuestionID", item[0]);
                                    command.Parameters.AddWithValue("@Answer", item[1]);
                                    await command.ExecuteNonQueryAsync();
                                }
                            }
                            transaction.Commit();
                            return roomAnswerSubmitModel;
                        }
                        catch (System.Exception)
                        {
                            transaction.Rollback();
                            roomAnswerSubmitModel.RoomID = -1;
                            return roomAnswerSubmitModel;
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                roomAnswerSubmitModel.RoomID = -1;
                return roomAnswerSubmitModel;
            }
        }

        private bool QuizEnded(int roomID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("SELECT RoomStatus FROM Room WHERE RoomID = @RoomID", connection))
                {
                    command.Parameters.AddWithValue("@RoomID", roomID);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        return (int)reader["RoomStatus"] == 1;
                    }
                    return true;
                }
            }
        }

    }
}