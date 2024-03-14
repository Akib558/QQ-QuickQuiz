using System;
using System.Data.SqlClient;

public interface ICustomLogger
{
    void Log(LogLevel logLevel, string message);
    void Log(LogLevel logLevel, Exception exception, string message);
}

public enum LogLevel
{
    Information,
    Warning,
    Error,
    Debug
}

public class CustomLogger : ICustomLogger
{
    private readonly string _connectionString;


    public CustomLogger(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Log(LogLevel logLevel, string message)
    {
        LogMessage(logLevel, message);
    }

    public void Log(LogLevel logLevel, Exception exception, string message)
    {
        LogMessage(logLevel, $"{message} Error: {exception.Message}");
    }

    private void LogMessage(LogLevel logLevel, string message)
    {
        string logLevelString = Enum.GetName(typeof(LogLevel), logLevel).ToUpper();
        string formattedMessage = $"{message}";

        // Save log message to the database
        SaveLogToDatabase(logLevelString, formattedMessage);
    }

    private void SaveLogToDatabase(string loglevel, string message)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = "INSERT INTO LogTable (LogLevel, LogMessage) VALUES (@LogLevel, @LogMessage)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LogLevel", loglevel);
                    command.Parameters.AddWithValue("@LogMessage", message);
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exception, e.g., log it to console
            Console.WriteLine($"Error saving log message to database: {ex.Message}");
        }
    }
}
