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

        public async Task<List<int>> RoomParticpants(int roomID)
        {
            List<int> participants = new List<int>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT u.UserID FROM RoomParticipant rp JOIN Users u ON rp.UserID = u.UserID WHERE rp.RoomID = @RoomID AND u.UserType = 'part'";
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


    }
}