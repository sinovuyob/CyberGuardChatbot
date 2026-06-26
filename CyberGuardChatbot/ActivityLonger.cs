using System;
using System.Collections.Generic;
using System.Text;

namespace CyberGuardChatbot
{
    public class ActivityLogger
    {
        private List<LogEntry> logEntries;
        private int maxLogEntries;

        public ActivityLogger()
        {
            logEntries = new List<LogEntry>();
            maxLogEntries = 50; // Keep last 50 entries
        }

        // Add a log entry
        public void AddLog(string action, string details)
        {
            LogEntry entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Action = action,
                Details = details
            };

            logEntries.Add(entry);

            // Keep only the last maxLogEntries entries
            if (logEntries.Count > maxLogEntries)
            {
                logEntries.RemoveAt(0);
            }
        }

        // Get last 5-10 log entries as formatted string
        public string GetRecentLogs(int count = 10)
        {
            if (logEntries.Count == 0)
            {
                return "No activities logged yet.";
            }

            // Get the most recent entries
            int startIndex = Math.Max(0, logEntries.Count - count);
            int entriesToShow = Math.Min(count, logEntries.Count - startIndex);

            StringBuilder result = new StringBuilder();
            result.AppendLine("ACTIVITY LOG");
            result.AppendLine("============");
            result.AppendLine("");

            for (int i = logEntries.Count - 1; i >= startIndex; i--)
            {
                var entry = logEntries[i];
                int logNumber = (logEntries.Count - i);
                result.AppendLine(logNumber + ". " + entry.Timestamp.ToString("yyyy-MM-dd HH:mm") + " - " + entry.Action);
                result.AppendLine("   " + entry.Details);
                result.AppendLine("");
            }

            if (logEntries.Count > count)
            {
                result.AppendLine("Showing last " + entriesToShow + " of " + logEntries.Count + " entries.");
                result.AppendLine("Type 'show full log' to see all entries.");
            }
            else
            {
                result.AppendLine("Total entries: " + logEntries.Count);
            }

            return result.ToString();
        }

        // Get all log entries
        public string GetFullLog()
        {
            if (logEntries.Count == 0)
            {
                return "No activities logged yet.";
            }

            StringBuilder result = new StringBuilder();
            result.AppendLine("FULL ACTIVITY LOG");
            result.AppendLine("=================");
            result.AppendLine("");

            int logNumber = 1;
            foreach (var entry in logEntries)
            {
                result.AppendLine(logNumber + ". " + entry.Timestamp.ToString("yyyy-MM-dd HH:mm") + " - " + entry.Action);
                result.AppendLine("   " + entry.Details);
                result.AppendLine("");
                logNumber++;
            }

            result.AppendLine("Total entries: " + logEntries.Count);
            return result.ToString();
        }

        // Clear all logs
        public void ClearLogs()
        {
            logEntries.Clear();
            AddLog("LOG CLEARED", "All activity logs were cleared by user");
        }

        // Helper methods for logging specific actions
        public void LogTaskAdded(string taskTitle, string description, bool hasReminder)
        {
            string reminderInfo = hasReminder ? " (with reminder set)" : " (no reminder)";
            AddLog("TASK ADDED", "Task: '" + taskTitle + "' - " + description + reminderInfo);
        }

        public void LogTaskCompleted(string taskTitle)
        {
            AddLog("TASK COMPLETED", "Task '" + taskTitle + "' marked as completed");
        }

        public void LogTaskDeleted(string taskTitle)
        {
            AddLog("TASK DELETED", "Task '" + taskTitle + "' was deleted");
        }

        public void LogReminderSet(string taskTitle, DateTime reminderDate)
        {
            AddLog("REMINDER SET", "Reminder for '" + taskTitle + "' set for " + reminderDate.ToString("yyyy-MM-dd HH:mm"));
        }

        public void LogQuizStarted()
        {
            AddLog("QUIZ STARTED", "User started the cybersecurity quiz");
        }

        public void LogQuizCompleted(int score, int totalQuestions)
        {
            double percentage = (double)score / totalQuestions * 100;
            AddLog("QUIZ COMPLETED", "Score: " + score + "/" + totalQuestions + " (" + percentage.ToString("F1") + "%)");
        }

        public void LogNLPInterpretation(string userInput, string interpretedAction)
        {
            AddLog("NLP DETECTED", "User: '" + userInput + "' -> Interpreted as: " + interpretedAction);
        }

        public void LogMessage(string action, string details)
        {
            AddLog(action, details);
        }
    }

    // Log Entry Class
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
    }
}