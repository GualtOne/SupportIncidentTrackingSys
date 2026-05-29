using System.IO;
using Dapper;
using Microsoft.Data.Sqlite;
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
        public static readonly string tablenameUS = "Users";
        public static readonly string tablenameA = "Attachments";

        private static readonly string createQuery = @"
                        CREATE TABLE IF NOT EXISTS 'Incidents' (
	                        'Id' INTEGER NOT NULL UNIQUE,
	                        'Author' TEXT NOT NULL,
	                        'Subdivision'	TEXT NOT NULL,
	                        'Category'	TEXT,
	                        'Description'	TEXT,
	                        'Priority'	TEXT NOT NULL,
	                        'RegistrationDate'	TEXT NOT NULL,
	                        'ResponseTime'	TEXT,
	                        'DecisionDeadline'	TEXT,
	                        'Responsible'	TEXT,
	                        'Status'	TEXT NOT NULL,
	                        'ResponsibleId'	INTEGER,
	                        'CreatedById'	INTEGER,
	                        PRIMARY KEY('Id' AUTOINCREMENT)
                        );";

        private static readonly string createQueryCategories = @"
                CREATE TABLE IF NOT EXISTS 'Categories' (
	            'Id'	INTEGER NOT NULL UNIQUE,
	            'Name'	TEXT NOT NULL,
	            PRIMARY KEY('Id' AUTOINCREMENT)
                );";

        private static readonly string createQueryCh = @"
                CREATE TABLE IF NOT EXISTS CommentsHistory (
	            Id	INTEGER NOT NULL UNIQUE,
	            IncidentID	INTEGER NOT NULL,
	            Comment	TEXT,
	            ActionType	TEXT NOT NULL,
	            Timestamp	TEXT NOT NULL,
	            PRIMARY KEY(Id AUTOINCREMENT));";

        private static readonly string createQueryS = @"CREATE TABLE IF NOT EXISTS 'Statuses' (
	            'IdStatus'	INTEGER NOT NULL UNIQUE,
	            'StatusName'	TEXT NOT NULL,
	            PRIMARY KEY('IdStatus' AUTOINCREMENT));";

        private static readonly string createQuerySUB = @"CREATE TABLE IF NOT EXISTS 'Subdivision' (
	            'Id'	INTEGER NOT NULL UNIQUE,
	            'Name'	TEXT NOT NULL,
	            PRIMARY KEY('Id' AUTOINCREMENT)
            );";

        private static readonly string createQueryP = @"CREATE TABLE IF NOT EXISTS 'Priorities' (
	            'ID'	INTEGER NOT NULL UNIQUE,
	            'Name'	TEXT NOT NULL,
	            PRIMARY KEY('ID' AUTOINCREMENT));";

        private static readonly string createQueryU= @"CREATE TABLE IF NOT EXISTS 'Users' (
	            'Id'	INTEGER NOT NULL UNIQUE,
	            'Login'	TEXT NOT NULL UNIQUE,
	            'PasswordHash'	TEXT NOT NULL,
	            'FullName'	TEXT NOT NULL,
	            'Role'	TEXT NOT NULL,
	            'IsActive'	BOOLEAN DEFAULT 1,
	            PRIMARY KEY('Id' AUTOINCREMENT)
            );";

        private static readonly string createQueryA = "CREATE TABLE IF NOT EXISTS \"Attachments\" (\r\n\t\"Id\"\tINTEGER,\r\n\t\"IncidentId\"\tINTEGER NOT NULL,\r\n\t\"FileName\"\tTEXT NOT NULL,\r\n\t\"FilePath\"\tTEXT NOT NULL,\r\n\t\"UploadedAt\"\tTEXT NOT NULL,\r\n\t\"UploadedBy\"\tINTEGER,\r\n\tPRIMARY KEY(\"Id\" AUTOINCREMENT)\r\n);";

        private static readonly string insertIntoPrio = @"
                INSERT INTO Priorities (Name) VALUES('Низкий');
                INSERT INTO Priorities (Name) VALUES('Средний');
                INSERT INTO Priorities (Name) VALUES('Высокий');
                INSERT INTO Priorities(Name) VALUES('Критический');";

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
                INSERT INTO Categories (Name) VALUES('Доступ и учетные записи');
                INSERT INTO Categories (Name) VALUES('Сеть и связь');
                INSERT INTO Categories (Name) VALUES('Электронная почта');
                INSERT INTO Categories (Name) VALUES('Безопасность');
                INSERT INTO Categories (Name) VALUES('Запрос на обслуживание');
                INSERT INTO Categories (Name) VALUES('Групповые политики и конфигурации');
                INSERT INTO Categories (Name) VALUES('Инфраструктура');";

        private static readonly string insertIntoSubdivison = @"
                INSERT INTO Subdivision (Name) VALUES 
                ('Руководство'),
                ('Бухгалтерия'),
                ('Отдел продаж'),
                ('ИТ-отдел'),
                ('Отдел кадров'),
                ('Юридический отдел');";

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
                    if (!TableExists(connection, tablenameSUB))
                        CreateTableWithIn(connection, createQuerySUB, insertIntoSubdivison);
                    if (!TableExists(connection, tablenameUS))
                        CreateTable(connection, createQueryU);
                    if(!TableExists(connection, tablenameA))
                        CreateTable(connection, createQueryA);
                }
                else
                {
                    File.Create(filepath).Close();
                    using var connection = new SqliteConnection(connectionString);
                    CreateAll(connection);
                }
            }
            catch (SqliteException ex)
            {
                Messageb.ShowError($"Ошибка при при подсоединение к SQL серверу {ex.Message}");
            }
        }

        public static void CreateAll(SqliteConnection connection)
        {
            connection.Open();
            CreateTable(connection, createQuery);
            CreateTableWithIn(connection, createQueryS, insertIntoStatus);
            CreateTableWithIn(connection, createQueryS, insertIntoStatus);
            CreateTableWithIn(connection, createQueryP, insertIntoPrio);
            CreateTableWithIn(connection, createQueryCategories, insertIntoCategories);
            CreateTableWithIn(connection, createQuerySUB, insertIntoSubdivison);
            CreateTable(connection, createQueryU);
            CreateTable(connection, createQueryCh);
            CreateTable(connection, createQueryA);
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

        public static IEnumerable<CommentsHistory> GetCommentsByIncidentId(int incidentId)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                const string sql = "SELECT * FROM CommentsHistory WHERE IncidentId = @id ORDER BY Timestamp";
                return connection.Query<CommentsHistory>(sql, new { id = incidentId });
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

        public static User GetUserByLogin(string login)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                const string query = "SELECT * FROM Users WHERE Login = @Login";

                return connection.QueryFirstOrDefault<User>(query, new { Login = login }) ?? new User();
            }
            catch (SqliteException ex)
            {
                Messageb.ShowError(ex.Message);
                return new User();
            }
            catch (Exception ex)
            {
                Messageb.ShowError(ex.Message);
                return new User();
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

        public static void DeleteCommentHistory(int Id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = "DELETE FROM CommentsHistory WHERE IncidentId = @id;";
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

        public static void DeleteCommentHistoryA(int Id)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = "DELETE FROM CommentsHistory WHERE Id = @id;";
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

        public static int Add(Incident incident)
        {
            ArgumentNullException.ThrowIfNull(incident);
            if (string.IsNullOrWhiteSpace(incident.Author))
                throw new InvalidOperationException("Поле Author не может быть пустым");
            if (string.IsNullOrWhiteSpace(incident.Subdivision))
                throw new InvalidOperationException("Поле Subdivision не может быть пустым");
            if (string.IsNullOrWhiteSpace(incident.Priority))
                throw new InvalidOperationException("Поле Priority не может быть пустым");
            if (string.IsNullOrWhiteSpace(incident.Status))
                throw new InvalidOperationException("Поле Status не может быть пустым");

            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = @"
            INSERT INTO Incidents 
            (Author, Subdivision, Category, Description, Priority, RegistrationDate, Status, CreatedById) 
            VALUES 
            (@author, @subdivision, @category, @description, @priority, @registrationDate, @status, @createdById);
            SELECT last_insert_rowid();";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@author", incident.Author);
                command.Parameters.AddWithValue("@subdivision", incident.Subdivision);
                command.Parameters.AddWithValue("@category", ToDbValue(incident.Category));
                command.Parameters.AddWithValue("@description", ToDbValue(incident.Description));
                command.Parameters.AddWithValue("@priority", incident.Priority);
                command.Parameters.AddWithValue("@registrationDate", incident.RegistrationDate?.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@status", incident.Status);
                command.Parameters.AddWithValue("@createdById", ToDbValue(incident.CreatedById));

                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (SqliteException ex)
            {
                Messageb.ShowError(ex.Message);
                return -1;
            }
            catch (Exception ex)
            {
                Messageb.ShowError(ex.Message);
                return -1;
            }
        }

        public static int AddCommentHistory(CommentsHistory comment)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = @"INSERT INTO CommentsHistory (IncidentId, Comment, ActionType, Timestamp) VALUES 
                                (@incidentid, @comment, @actiontype, @timestamp); 
                                SELECT last_insert_rowid();";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@incidentid", comment.IncidentId);
                command.Parameters.AddWithValue("@comment", comment.Comment);
                command.Parameters.AddWithValue("@actiontype", comment.ActionType);
                command.Parameters.AddWithValue("@timestamp", comment.Timestamp);
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (SqliteException ex)
            {
                Messageb.ShowError(ex.Message);
                return -1;
            }
            catch (Exception ex)
            {
                Messageb.ShowError(ex.Message);
                return -1;
            }
        }

        public static void AddUser(User user)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = "INSERT INTO Users (Login, PasswordHash, FullName, Role, IsActive) VALUES (@login, @passwordhash, @fullname, @role, @isactive)";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@login", user.Login);
                command.Parameters.AddWithValue("@passwordhash", user.PasswordHash);
                command.Parameters.AddWithValue("@fullname", user.FullName);
                command.Parameters.AddWithValue("@role", user.Role);
                command.Parameters.AddWithValue("@isactive", user.IsActive);
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
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = @"
                UPDATE Incidents SET 
                Author = @author,
                Subdivision = @subdivision,
                Category = @category,
                Description = @description,
                Priority = @priority,
                RegistrationDate = @registrationDate,
                Status = @status,
                Responsible = @responsible,
                ResponseTime = @responseTime,
                DecisionDeadline = @decisionDeadline,
                ResponsibleId = @responsibleId,
                CreatedById = @createdById
                WHERE Id = @id";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", incident.Id);
                command.Parameters.AddWithValue("@author", incident.Author);
                command.Parameters.AddWithValue("@subdivision", incident.Subdivision);
                command.Parameters.AddWithValue("@category", incident.Category);
                command.Parameters.AddWithValue("@description", incident.Description);
                command.Parameters.AddWithValue("@priority", incident.Priority);
                command.Parameters.AddWithValue("@registrationDate", incident.RegistrationDate);
                command.Parameters.AddWithValue("@status", incident.Status);
                command.Parameters.AddWithValue("@responseTime", ToDbValue(incident.ResponseTime));
                command.Parameters.AddWithValue("@decisionDeadline", ToDbValue(incident.DecisionDeadline));
                command.Parameters.AddWithValue("@responsible", ToDbValue(incident.Responsible));
                command.Parameters.AddWithValue("@responsibleId", ToDbValue(incident.ResponsibleId));
                command.Parameters.AddWithValue("@createdById", incident.CreatedById);

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

        public static void UpdateCommentHistory(CommentsHistory commentsHistory)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = @"
                UPDATE CommentsHistory SET 
                Comment = @comment,
                Timestamp = @timestamp
                WHERE Id = @id";

                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", commentsHistory.Id);
                command.Parameters.AddWithValue("@comment", commentsHistory.Comment);
                command.Parameters.AddWithValue("@timestamp", commentsHistory.Timestamp);
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

        public static void ChangeStatus(Incident incident)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = @"
                UPDATE Incidents SET 
                Status = @status
                WHERE Id = @id";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", incident.Id);
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

        public static void ChangeResponsible(Incident incident)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = @"
                UPDATE Incidents SET 
                Responsible = @responsible
                WHERE Id = @id";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", incident.Id);
                command.Parameters.AddWithValue("@responsible", incident.Responsible);
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

        public static void UpdateResposnseTime(Incident incident)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = @"
                UPDATE Incidents SET 
                ResponseTime = @responseTime
                WHERE Id = @id";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", incident.Id);
                command.Parameters.AddWithValue("@responseTime", incident.ResponseTime);
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

        public static void UpdateDisionDeadline(Incident incident)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = @"
                UPDATE Incidents SET 
                DecisionDeadline = @decisionDeadline
                WHERE Id = @id";
                using var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", incident.Id);
                command.Parameters.AddWithValue("@decisionDeadline", incident.DecisionDeadline);
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

        public static int AddAttachment(Attachment attachment)
        {
            using var connection = new SqliteConnection(connectionString);
            string query = @"
            INSERT INTO Attachments (IncidentId, FileName, FilePath, UploadedAt, UploadedBy)
            VALUES (@incidentId, @fileName, @filePath, @uploadedAt, @uploadedBy);
            SELECT last_insert_rowid();";
            var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@incidentId", attachment.IncidentId);
            command.Parameters.AddWithValue("@fileName", attachment.FileName);
            command.Parameters.AddWithValue("@filePath", attachment.FilePath);
            command.Parameters.AddWithValue("@uploadedAt", attachment.UploadedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@uploadedBy", ToDbValue(attachment.UploadedBy));
            connection.Open();
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public static IEnumerable<Attachment> GetAttachmentsByIncidentId(int incidentId)
        {
            using var connection = new SqliteConnection(connectionString);
            const string sql = "SELECT * FROM Attachments WHERE IncidentId = @id ORDER BY Id";
            return connection.Query<Attachment>(sql, new { id = incidentId });
        }

        public static void DeleteAttachment(int attachmentId)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Execute("DELETE FROM Attachments WHERE Id = @id", new { id = attachmentId });
        }

        public static Attachment GetAttachmentIncidentId(int incidentId)
        {
            try
            {
            using var connection = new SqliteConnection(connectionString);
            string query = "SELECT * FROM Attachments WHERE IncidentId = @incidentId";
            return connection.QueryFirstOrDefault<Attachment>(query, new { IncidentId = incidentId }) ?? new Attachment();
            }
            catch (SqliteException ex)
            {
                Messageb.ShowError(ex.Message);
                return new Attachment();
            }
            catch (Exception ex)
            {
                Messageb.ShowError(ex.Message);
                return new Attachment();
            }
        }


        public static object ToDbValue(object? value) => value ?? DBNull.Value;

    }
}
