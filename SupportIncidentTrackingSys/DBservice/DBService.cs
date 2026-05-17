using System.IO;
using Dapper;
using Microsoft.Data.Sqlite;
using SupportIncidentTrackingSys.Interfaces;
using SupportIncidentTrackingSys.Models;

namespace SupportIncidentTrackingSys.DBservice
{
    public class DBService
    {
        private static readonly string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        static readonly string folder = "Supportsys";
        private static readonly string file = "Incidents.db";

        private static readonly string folderpath = Path.Combine(docPath, folder);
        private static readonly string filepath = Path.Combine(folderpath, file);

        private static readonly string connectionString = $"Data Source={filepath}";
        public static readonly string tablename = "Incidents";
        public static readonly string tablenameC = "Categories";
        public static readonly string tablenameP = "Priorities";
        public static readonly string tablenameCH = "CommentsHistory";
        public static readonly string tablenameS = "Statuses";
        public static readonly string tablenameSUB = "Subdivision";

        private static readonly string createQuery = @"
                CREATE TABLE IF NOT EXISTS Incidents (
	            Id INTEGER NOT NULL UNIQUE,
	            Author TEXT NOT NULL,
	            Subdivision	TEXT NOT NULL,
	            Category TEXT,
	            Description TEXT,
	            Priority TEXT NOT NULL,
	            RegistrationDate TEXT NOT NULL,
	            ResponseTime TEXT,
	            DecisionDeadline TEXT,
	            Responsible TEXT,
	            Status INTEGER NOT NULL,
	            PRIMARY KEY(Id AUTOINCREMENT));";

        private static readonly string createQueryCategories = @"
                CREATE TABLE IF NOT EXISTS CommentsHistory (
	            Id	INTEGER NOT NULL UNIQUE,
	            IncidentID	INTEGER NOT NULL,
	            Comment	TEXT,
	            ActionType	TEXT NOT NULL,
	            Timestamp	TEXT NOT NULL,
	            PRIMARY KEY(Id AUTOINCREMENT));";

        private static readonly string createQueryCh = @"
                CREATE TABLE IF NOT EXISTS CommentsHistory (
	            Id	INTEGER NOT NULL UNIQUE,
	            IncidentID	INTEGER NOT NULL,
	            Comment	TEXT,
	            ActionType	TEXT NOT NULL,
	            Timestamp	TEXT NOT NULL,
	            PRIMARY KEY(Id AUTOINCREMENT));";

        private static readonly string createQueryS = @"CREATE TABLE 'Statuses' (
	            'IdStatus'	INTEGER NOT NULL UNIQUE,
	            'StatusName'	TEXT NOT NULL,
	            PRIMARY KEY('IdStatus' AUTOINCREMENT));";

        private static readonly string createQueryP = @"CREATE TABLE 'Priorities' (
	            'ID'	INTEGER NOT NULL UNIQUE,
	            'Name'	TEXT NOT NULL,
	            PRIMARY KEY('ID' AUTOINCREMENT));";

        private static readonly string insertIntoPrio = @"
                INSERT INTO Priorities (Name) VALUES('Низкий');
                INSERT INTO Priorities (Name) VALUES('Средний');
                INSERT INTO Priorities (Name) VALUES('Высокий');";

        private static readonly string insertIntoStatus = @"
                INSERT INTO Statuses (StatusName) VALUES('новый');
                INSERT INTO Statuses (StatusName) VALUES('в работе');
                INSERT INTO Statuses (StatusName) VALUES('ожидает информации');
                INSERT INTO Statuses (StatusName) VALUES('решен');
                INSERT INTO Statuses (StatusName) VALUES('закрыт');
                INSERT INTO Statuses (StatusName) VALUES('отклонен');";

        private static readonly string insertIntoCategories = @"
                INSERT INTO Categories (Name) VALUES('Аппаратное обеспечение');
                INSERT INTO Categories (Name) VALUES('Программное обеспечение');
                INSERT INTO Categories (Name) VALUES('Доступ и учетные записи ');
                INSERT INTO Categories (Name) VALUES('Сеть и связь');
                INSERT INTO Categories (Name) VALUES('Электронная почта');
                INSERT INTO Categories (Name) VALUES('Безопасность');
                INSERT INTO Categories (Name) VALUES('Запрос на обслуживание');
                INSERT INTO Categories (Name) VALUES('Групповые политики и конфигурации');
                INSERT INTO Categories (Name) VALUES('Инфраструктура');";

        static DBService()
        {
            if (!Directory.Exists(folderpath))
                Directory.CreateDirectory(folderpath);
        }
        public static void Connection()
        {
            try
            {
                if (!Directory.Exists(folderpath))
                    Directory.CreateDirectory(folderpath);

                if (DataBaseExists())
                {
                    using SqliteConnection connection = new(connectionString);
                    connection.Open();
                    if (!TableExists(connection, tablename))
                        CreateTable(connection, createQuery);
                    if (!TableExists(connection, tablenameS))
                        CreateTableWithIn(connection, createQueryS, insertIntoStatus);
                    if(!TableExists(connection, tablenameP))
                        CreateTableWithIn(connection, createQueryP, insertIntoPrio);
                    if(!TableExists(connection, tablenameC))
                        CreateTableWithIn(connection, createQueryCategories, insertIntoCategories);
                    if (!TableExists(connection, tablenameCH))
                        CreateTable(connection, createQueryCh);
                }
                else
                {

                    File.Create(filepath).Close();
                    using var connection = new SqliteConnection(connectionString);
                    connection.Open();
                    CreateTable(connection, createQuery);
                }
            }
            catch (SqliteException ex)
            {
                Messageb.ShowError($"Ошибка при при подсоединение к SQL серверу {ex.Message}");
            }
        }

        public static bool DataBaseExists()
        {
            if (File.Exists(filepath)) return true;
            else return false;
        }

        public static void CreateTable(SqliteConnection connection, string createQuery)
        {
            using var createCmd = new SqliteCommand(createQuery, connection);
            createCmd.ExecuteNonQuery();
        }

        public static void CreateTableWithIn(SqliteConnection connection, string createQuery, string insertQuery)
        {
            using var createCmd = new SqliteCommand(createQuery, connection);
            createCmd.ExecuteNonQuery();

            using var Insert = new SqliteCommand(insertQuery, connection);
            Insert.ExecuteNonQuery();
        }

        public static bool TableExists(SqliteConnection connection, string tablename)
        {
            string query = "SELECT name FROM sqlite_master WHERE type='table' AND name=@name";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@name", tablename);
            var result = command.ExecuteScalar();
            return result != null;
        }

        public static IEnumerable<T> GetAllFrom<T>(string table)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                return connection.Query<T>($"SELECT * FROM {table}");
            }
            catch (SqliteException ex)
            {
                Messageb.ShowError(ex.Message);
                return [];
            }
            catch (Exception ex)
            {
                Messageb.ShowError(ex.Message);
                return [];
            }
        }


        public static void Delete(int Id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = "DELETE FROM Incidents WHERE Id = @id;";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", Id);
                command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Messageb.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Messageb.ShowError(ex.Message);
            }
        }

        public static void Add(Incident incident)
        {
            ArgumentNullException.ThrowIfNull(incident);
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = "INSERT INTO Incidents (Author, Subdivision, Category, Description, Priority, RegistrationDate, ResponseTime, " +
                    "DecisionDeadline, Responsible, Status) " +
                    "VALUES (@author, @subdivision, @category, @description, @priority, @registrationDate, @responseTime, @decisionDeadline, @responsible, @status)";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@author", incident.Author);
                command.Parameters.AddWithValue("@subdivision", incident.Subdivison);
                command.Parameters.AddWithValue("@category", incident.Category);
                command.Parameters.AddWithValue("@description", incident.Description);
                command.Parameters.AddWithValue("@priority", incident.Priority);
                command.Parameters.AddWithValue("@registrationDate", incident.Regdate);
                command.Parameters.AddWithValue("@responseTime", incident.ResponseTime);
                command.Parameters.AddWithValue("@decisionDeadline", incident.DecisionDeadline);
                command.Parameters.AddWithValue("@responsible", incident.Responsibale);
                command.Parameters.AddWithValue("@status", incident.Status);
                command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Messageb.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Messageb.ShowError(ex.Message);
            }
        }

        public static void Update(Incident incident)
        {
            ArgumentNullException.ThrowIfNull(incident);
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = $"UPDATE Incidents SET Author = @author, Subdivision = @subdivision, Category = @category, Description = @description, " +
                    $"Priority = @priority, RegistrationDate = @registrationDate, ResponseTime = @responseTime, DecisionDeadline = @decisionDeadline, " +
                    $"Responsible = @responsible, Status = @status" +
                    $" WHERE Id = @id";
                using var command = new SqliteCommand( query, connection);
                command.Parameters.AddWithValue("@id", incident.Id);
                command.Parameters.AddWithValue("@author", incident.Author);
                command.Parameters.AddWithValue("@subdivision", incident.Subdivison);
                command.Parameters.AddWithValue("@category", incident.Category);
                command.Parameters.AddWithValue("@description", incident.Description);
                command.Parameters.AddWithValue("@priority", incident.Priority);
                command.Parameters.AddWithValue("@registrationDate", incident.Regdate);
                command.Parameters.AddWithValue("@responseTime", incident.ResponseTime);
                command.Parameters.AddWithValue("@decisionDeadline", incident.DecisionDeadline);
                command.Parameters.AddWithValue("@responsible", incident.Responsibale);
                command.Parameters.AddWithValue("@status", incident.Status);
                command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Messageb.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                Messageb.ShowError(ex.Message);
            }
        }
    }
}
