using System;
using System.Collections.Generic;
using System.Text;

namespace CyberGuardChatbot
{
    public class TaskManager
    {
        private DatabaseHelper dbHelper;
        private ActivityLogger logger; 

        public TaskManager()
        {
            dbHelper = new DatabaseHelper();
            logger = new ActivityLogger(); 
        }

        // Add a new task
        public string AddTask(string title, string description, DateTime? reminderDate = null)
        {
            try
            {
                dbHelper.AddTask(title, description, reminderDate);

                // ===== ADD LOGGING HERE =====
                logger.LogTaskAdded(title, description, reminderDate.HasValue);
                if (reminderDate.HasValue)
                {
                    logger.LogReminderSet(title, reminderDate.Value);
                }
                // ===== END LOGGING =====

                string result = "Task added successfully!\n";
                result += "Title: " + title + "\n";
                result += "Description: " + description + "\n";

                if (reminderDate.HasValue)
                {
                    result += "Reminder set for: " + reminderDate.Value.ToString("yyyy-MM-dd HH:mm");
                }
                else
                {
                    result += "No reminder set";
                }

                return result;
            }
            catch (Exception ex)
            {
                return "Error adding task: " + ex.Message;
            }
        }

        // Get all tasks as formatted string
        public string GetTasks(bool includeCompleted = false)
        {
            var tasks = dbHelper.GetTasks(includeCompleted);

            if (tasks.Count == 0)
            {
                return "You have no tasks. Add a task using: 'Add task: [title] - [description]'";
            }

            StringBuilder result = new StringBuilder();
            result.AppendLine("Your Tasks:");
            result.AppendLine("-----------");

            foreach (var task in tasks)
            {
                string status = task.IsCompleted ? "[COMPLETED]" : "[PENDING]";
                string reminder = task.ReminderDate.HasValue ?
                    " Reminder: " + task.ReminderDate.Value.ToString("yyyy-MM-dd") : "";

                result.AppendLine(task.Id + ". " + status + " " + task.Title + reminder);
                result.AppendLine("   " + task.Description);
                result.AppendLine("");
            }

            return result.ToString();
        }

        // Get pending tasks only
        public string GetPendingTasks()
        {
            return GetTasks(false);
        }

        // Get all tasks including completed
        public string GetAllTasks()
        {
            return GetTasks(true);
        }

        // Complete a task by ID
        public string CompleteTask(string taskIdInput)
        {
            try
            {
                if (!int.TryParse(taskIdInput, out int taskId))
                {
                    return "Invalid task ID. Please provide a valid number.";
                }

                var tasks = dbHelper.GetTasks(true);
                var task = tasks.Find(t => t.Id == taskId);

                if (task == null)
                {
                    return "Task with ID " + taskId + " not found.";
                }

                if (task.IsCompleted)
                {
                    return "Task '" + task.Title + "' is already completed.";
                }

                dbHelper.CompleteTask(taskId);

                // ===== ADD LOGGING HERE =====
                logger.LogTaskCompleted(task.Title);
                // ===== END LOGGING =====

                return "Task '" + task.Title + "' marked as completed! Good job!";
            }
            catch (Exception ex)
            {
                return "Error completing task: " + ex.Message;
            }
        }

        // Delete a task by ID
        public string DeleteTask(string taskIdInput)
        {
            try
            {
                if (!int.TryParse(taskIdInput, out int taskId))
                {
                    return "Invalid task ID. Please provide a valid number.";
                }

                var tasks = dbHelper.GetTasks(true);
                var task = tasks.Find(t => t.Id == taskId);

                if (task == null)
                {
                    return "Task with ID " + taskId + " not found.";
                }

                dbHelper.DeleteTask(taskId);

                // ===== ADD LOGGING HERE =====
                logger.LogTaskDeleted(task.Title);
                // ===== END LOGGING =====

                return "Task '" + task.Title + "' has been deleted.";
            }
            catch (Exception ex)
            {
                return "Error deleting task: " + ex.Message;
            }
        }

        // Extract task details from user input
        public TaskInfo ExtractTaskInfo(string userInput)
        {
            TaskInfo info = new TaskInfo();
            string input = userInput.ToLower();

            // Try to extract title and description
            // Format: "Add task: [title] - [description]"
            // Or: "Add task: [title]"

            if (userInput.Contains(":"))
            {
                string afterColon = userInput.Substring(userInput.IndexOf(':') + 1).Trim();

                // Check for description separator
                if (afterColon.Contains(" - "))
                {
                    int separatorIndex = afterColon.IndexOf(" - ");
                    info.Title = afterColon.Substring(0, separatorIndex).Trim();
                    info.Description = afterColon.Substring(separatorIndex + 3).Trim();
                }
                else
                {
                    info.Title = afterColon;
                    info.Description = "No description provided";
                }
            }
            else
            {
                // If no colon, use the whole input as title
                info.Title = userInput;
                info.Description = "No description provided";
            }

            // Check for reminder
            if (input.Contains("remind") || input.Contains("in") || input.Contains("days"))
            {
                info.ReminderDate = ExtractReminderDate(userInput);
            }

            return info;
        }

        // Extract reminder date from user input
        private DateTime? ExtractReminderDate(string userInput)
        {
            string input = userInput.ToLower();

            // Check for "in X days"
            if (input.Contains("in") && input.Contains("day"))
            {
                try
                {
                    // Find the number
                    string[] words = input.Split(' ');
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i] == "in" && i + 1 < words.Length)
                        {
                            if (int.TryParse(words[i + 1], out int days))
                            {
                                return DateTime.Now.AddDays(days);
                            }
                        }
                    }
                }
                catch { }
            }

            // Check for "tomorrow"
            if (input.Contains("tomorrow"))
            {
                return DateTime.Now.AddDays(1);
            }

            // Check for "next week"
            if (input.Contains("next week"))
            {
                return DateTime.Now.AddDays(7);
            }

            return null;
        }
    }

    // Task Info helper class
    public class TaskInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
    }
}