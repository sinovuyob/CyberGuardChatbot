using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace CyberGuardChatbot
{
    class DatabaseHelper
    {
        // Connection string - Update with YOUR password
        private string connectionString = "Server=localhost;Database=chatbotdb;User ID=root;Password=2019Imbu$";

        public DatabaseHelper()
        {
            CreateTableIfNotExists();
        }

        private void CreateTableIfNotExists()
        {
            string query = @"
                CREATE TABLE IF NOT EXISTS tasks (
                    Id INT PRIMARY KEY AUTO_INCREMENT,
                    Title VARCHAR(255) NOT NULL,
                    Description TEXT,
                    ReminderDate DATETIME,
                    IsCompleted BOOLEAN DEFAULT FALSE,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                )";

            ExecuteNonQuery(query);
        }

        public void AddTask(string title, string description, DateTime? reminderDate = null)
        {
            string query = @"
                INSERT INTO tasks (Title, Description, ReminderDate) 
                VALUES (@Title, @Description, @ReminderDate)";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(query, conn))
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
                "SELECT * FROM tasks WHERE IsCompleted = FALSE ORDER BY CreatedAt DESC";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new Task
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title"),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description"),
                            ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate")) ? (DateTime?)null : reader.GetDateTime("ReminderDate"),
                            IsCompleted = reader.GetBoolean("IsCompleted"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        });
                    }
                }
            }
            return tasks;
        }

        public void CompleteTask(int taskId)
        {
            string query = "UPDATE tasks SET IsCompleted = TRUE WHERE Id = @Id";
            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", taskId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteTask(int taskId)
        {
            string query = "DELETE FROM tasks WHERE Id = @Id";
            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", taskId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void ExecuteNonQuery(string query)
        {
            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }

    // Task Model Class
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
    

