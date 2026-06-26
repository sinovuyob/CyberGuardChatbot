using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace CyberGuardChatbot
{
    public class DatabaseHelper
    {
        // For Windows Authentication (Recommended)
        private string connectionString = @"Server=LabVM2049939\SQLEXPRESS;Database=chatbotdb;Integrated Security=True;TrustServerCertificate=True;";
        private string masterConnectionString = @"Server=LabVM2049939\SQLEXPRESS;Database=master;Integrated Security=True;TrustServerCertificate=True;";

        public DatabaseHelper()
        {
            CreateDatabaseIfNotExists();
            CreateTableIfNotExists();
        }

        private void CreateDatabaseIfNotExists()
        {
            try
            {
                using (var conn = new SqlConnection(masterConnectionString))
                {
                    conn.Open();
                    string createDbQuery = @"
                        IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'chatbotdb')
                        BEGIN
                            CREATE DATABASE chatbotdb;
                        END";
                    using (var cmd = new SqlCommand(createDbQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database creation error: " + ex.Message);
            }
        }

        private void CreateTableIfNotExists()
        {
            try
            {
                string query = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tasks')
                    BEGIN
                        CREATE TABLE tasks (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Title NVARCHAR(255) NOT NULL,
                            Description NVARCHAR(MAX),
                            ReminderDate DATETIME,
                            IsCompleted BIT DEFAULT 0,
                            CreatedAt DATETIME DEFAULT GETDATE()
                        )
                    END";

                ExecuteNonQuery(query);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Table creation error: " + ex.Message);
            }
        }

        public void AddTask(string title, string description, DateTime? reminderDate = null)
        {
            string query = @"
                INSERT INTO tasks (Title, Description, ReminderDate) 
                VALUES (@Title, @Description, @ReminderDate)";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@ReminderDate", (object)reminderDate ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Task> GetTasks(bool includeCompleted = false)
        {
            var tasks = new List<Task>();
            string query = includeCompleted ?
                "SELECT * FROM tasks ORDER BY CreatedAt DESC" :
                "SELECT * FROM tasks WHERE IsCompleted = 0 ORDER BY CreatedAt DESC";

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new Task
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString(reader.GetOrdinal("Description")),
                            ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ReminderDate")),
                            IsCompleted = reader.GetBoolean(reader.GetOrdinal("IsCompleted")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                        });
                    }
                }
            }
            return tasks;
        }

        public void CompleteTask(int taskId)
        {
            string query = "UPDATE tasks SET IsCompleted = 1 WHERE Id = @Id";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", taskId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteTask(int taskId)
        {
            string query = "DELETE FROM tasks WHERE Id = @Id";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", taskId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void ExecuteNonQuery(string query)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ExecuteNonQuery error: " + ex.Message);
                throw;
            }
        }
    }

    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public string DisplayText
        {
            get
            {
                string status = IsCompleted ? "[✓ DONE]" : "[ ]";
                string reminder = ReminderDate.HasValue ? $" (Reminder: {ReminderDate.Value:yyyy-MM-dd})" : "";
                return $"{status} {Title}{reminder}";
            }
        }
    }
}