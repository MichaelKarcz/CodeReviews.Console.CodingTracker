using System.Configuration;
using Dapper;
using Microsoft.Data.Sqlite;

namespace CodingTracker
{
    internal static class SQLHelper
    {
        private static readonly string CONNECTION_STRING = ConfigurationManager.AppSettings.Get("connectionString");
        private static readonly string TABLENAME = "codingSessions";

        public static void CreateDatabaseIfNotExists()
        {
            try
            {
                var connection = new SqliteConnection(CONNECTION_STRING);

                string commandText = $@"CREATE TABLE IF NOT EXISTS {TABLENAME} (
                                              Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                              StartTime TEXT,
                                              EndTime TEXT,
                                              Duration TEXT
                                              )";

                connection.Execute(commandText);
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"\n\nThere was an error trying to instantiate the database. Error message: {e.Message}\n\n");
            }
        }

        public static List<CodingSession> GetAllSessions()
        {

            string commandText = $@"SELECT * FROM {TABLENAME}";

            List<CodingSession> allEntries = PerformReadOperation(commandText);
            
            return allEntries;
        }

        public static List<CodingSession> GetUnfinishedSessions()
        {
            string commandText = $@"Select * FROM {TABLENAME}
                                    WHERE EndTime IS NULL OR EndTime=''";

            List<CodingSession> unfinishedSessions = PerformReadOperation(commandText);

            return unfinishedSessions;
        }

        public static bool InsertSingleSession(CodingSession habit)
        {
            string commandText = $@"INSERT INTO {TABLENAME} (StartTime, EndTime, Duration)
                                 VALUES (@StartTime, @EndTime, @Duration);";

            object[] parameters = { new {StartTime=habit.StartTime, EndTime=habit.EndTime, Duration=habit.Duration }};

            return PerformCUDOperation(commandText, parameters);

        }

        public static bool DeleteSession(int id)
        {
            string commandText = $@"DELETE FROM {TABLENAME}
                                 WHERE Id=@Id;";

            object[] parameters = { new { Id = id } };

            return PerformCUDOperation(commandText, parameters);
        }

        public static bool UpdateSession(int id, CodingSession habit)
        {
            string commandText = $@"UPDATE {TABLENAME}
                                 SET StartTime=@StartTime, EndTime=@EndTime, Duration=@Duration
                                 WHERE Id=@Id;";

            object[] parameters = { new { Id = id, StartTime = habit.StartTime, EndTime = habit.EndTime, Duration = habit.Duration } };

            return PerformCUDOperation(commandText, parameters);
        }

        private static List<CodingSession> PerformReadOperation(string commandText)
        {
            List<CodingSession> retrievedSessions = new List<CodingSession>();

            try
            {
                var connection = new SqliteConnection(CONNECTION_STRING);
                retrievedSessions = connection.Query<CodingSession>(commandText).ToList<CodingSession>();
                
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"\n\nThere was an error trying to retrieve the database records. Error message: {e.Message}\n\n");
                return new List<CodingSession>();
            }
            return retrievedSessions;
        }

        private static bool PerformCUDOperation(string commandText, object[] parameters)
        {
            bool commandSuccessful = false;

            try
            {
                var connection = new SqliteConnection(CONNECTION_STRING);

                int rowsUpdated = connection.Execute(commandText, parameters);

                commandSuccessful = rowsUpdated > 0 ? true : false;
            }
            catch (SqliteException e)
            {
                Console.WriteLine($"\n\n{e.Message}\n\n");
                commandSuccessful = false;
            }

            return commandSuccessful;
        }

    }
}
