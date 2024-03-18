using System.Data.SqlClient;
using QuickQuiz.Models.RequestModels;
using QuickQuiz.Repositories.Interfaces.ISetter;

namespace QuickQuiz.Repositories.Implementations.Setter;

public class QuestionRepository : IQuestionRepository
{
    private string _connectionString;

    public QuestionRepository(string connectionString)
    {
        _connectionString = connectionString;
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
                        var query = "SELECT * FROM Question WHERE RoomID = @RoomID AND IsDeleted = 0";
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

                                    var options = new List<OptionModel>();
                                    var query2 = "SELECT OptionID, Options FROM QuestionOptions WHERE QuestionID = @QuestionID ";
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
                                var query2 = "Select QuestionID from Question where Content = @Content and RoomID = @RoomID and IsDeleted = 0";
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
        
                
        public async Task<bool> DeleteOption(int questionID, int optionID)
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
                            var query = "Update QuestionOptions Set IsDeleted = 1 WHERE OptionID = @OptionID and QuestionID = @QuestionID";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@OptionID", optionID);
                                command.Parameters.AddWithValue("@QuestionID", questionID);
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                            return await Task.FromResult(true);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return await Task.FromResult(false);
                        }
                    }
                }
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
                            var query = "Update Question Set IsDeleted = 1 WHERE QuestionID = @QuestionID";
                            using (var command = new SqlCommand(query, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@QuestionID", questionID);
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