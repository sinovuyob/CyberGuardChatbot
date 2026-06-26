using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CyberGuardChatbot
{
    public partial class MainWindow : Window
    {
        private ChatbotService bot;
        private List<ChatMessage> messages;
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();

            bot = new ChatbotService();
            messages = new List<ChatMessage>();
            ChatList.ItemsSource = messages;

            // Welcome message
            AddBotMessage("WELCOME TO CYBERGUARD CHATBOT!\n\n" +
                         "I am your personal cybersecurity guardian.\n\n" +
                         "Current Topics:\n" +
                         "  [PASSWORD SAFETY]  [SCAM DETECTION]  [PRIVACY TIPS]  [PHISHING]\n\n" +
                         "What is your name?");
        }

        private void AddUserMessage(string text)
        {
            messages.Add(new ChatMessage("YOU", text, true));
            ChatList.ScrollIntoView(messages[messages.Count - 1]);
        }

        private void AddBotMessage(string text)
        {
            messages.Add(new ChatMessage("CYBERGUARD", text, false));
            ChatList.ScrollIntoView(messages[messages.Count - 1]);
            TopicText.Text = bot.GetCurrentTopic();
        }

        private void SendMessage()
        {
            string input = InputBox.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                StatusBar.Text = "⚠️ Please type a message";
                return;
            }

            AddUserMessage(input);
            InputBox.Clear();
            StatusBar.Text = "🤔 Thinking...";

            string response = bot.GetResponse(input);
            AddBotMessage(response);
            StatusBar.Text = "✅ Reply sent!";
        }

        private void ClearChat()
        {
            messages.Clear();
            bot.ResetMemory();
            AddBotMessage("Chat cleared! Memory reset.\n\nWhat is your name?");
            StatusBar.Text = "🗑️ Chat cleared";
        }

        // ===== EVENT HANDLERS =====
        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearChat();
        }

        // ===== NEW BUTTON CLICK HANDLERS =====

        private void StartQuizBtn_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Text = "start quiz";
            SendMessage();
        }

        private void ViewTasksBtn_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Text = "show my tasks";
            SendMessage();
        }

        private void AddTaskBtn_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Text = "add task: ";
            InputBox.Focus();
            InputBox.SelectionStart = InputBox.Text.Length;
            StatusBar.Text = "📝 Type your task after 'add task: '";
        }

        private void ShowLogBtn_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Text = "show log";
            SendMessage();
        }

        private void HelpBtn_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Text = "help";
            SendMessage();
        }
    }

    // Chat Message Class
    public class ChatMessage : INotifyPropertyChanged
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
        public HorizontalAlignment Alignment { get; set; }
        public Brush Color { get; set; }

        public ChatMessage(string sender, string message, bool isUser)
        {
            Sender = sender;
            Message = message;
            Time = DateTime.Now.ToString("HH:mm");

            if (isUser)
            {
                Alignment = HorizontalAlignment.Right;
                Color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(233, 69, 96));
            }
            else
            {
                Alignment = HorizontalAlignment.Left;
                Color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(15, 52, 96));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // Chatbot Service
    public class ChatbotService
    {
        // ===== PRIVATE FIELDS =====
        private string userName = "";
        private string favoriteTopic = "";
        private string currentTopic = "General";
        private string currentSentiment = "neutral";
        private string lastCategory = "";
        private Random random = new Random();

        // ===== NEW MANAGERS =====
        private TaskManager taskManager;
        private QuizManager quizManager;
        private ActivityLogger activityLogger;

        private Dictionary<string, List<string>> responses;
        private Dictionary<string, List<string>> sentimentResponses;

        // ===== CONSTRUCTOR =====
        public ChatbotService()
        {
            InitializeResponses();
            taskManager = new TaskManager();
            quizManager = new QuizManager();
            activityLogger = new ActivityLogger();

            // Log application start
            activityLogger.AddLog("APP STARTED", "CyberGuard Chatbot initialized");
        }

        private void InitializeResponses()
        {
            responses = new Dictionary<string, List<string>>
            {
                ["password"] = new List<string>
                {
                    "Use strong passwords with at least 12 characters including uppercase, lowercase, numbers, and symbols.",
                    "Never reuse passwords across accounts. Use a password manager like Bitwarden!",
                    "Enable Two-Factor Authentication (2FA) on all accounts for extra security.",
                    "Avoid using personal information like birthdays or pet names in your passwords."
                },
                ["scam"] = new List<string>
                {
                    "Never click suspicious links in emails or text messages.",
                    "Legitimate companies will never ask for your password via email.",
                    "If something sounds too good to be true, it is probably a scam.",
                    "Scammers create urgency. Always verify through official channels."
                },
                ["privacy"] = new List<string>
                {
                    "Review your privacy settings on social media regularly.",
                    "Use a VPN when connecting to public Wi-Fi networks.",
                    "Clear your browser cookies and cache regularly to protect your privacy.",
                    "Check which apps have access to your location, camera, and microphone."
                },
                ["phishing"] = new List<string>
                {
                    "Phishing attacks try to trick you into giving away personal information.",
                    "Hover over links before clicking to see the actual URL destination.",
                    "Check sender email addresses carefully for misspellings.",
                    "Never enter personal information on websites reached through email links."
                }
            };

            sentimentResponses = new Dictionary<string, List<string>>
            {
                ["worried"] = new List<string>
                {
                    "I understand your concern. It is completely normal to feel worried about online security.",
                    "Your safety matters. Don't worry - I am here to help you stay protected.",
                    "Feeling worried is understandable. Let me share some reassuring tips."
                },
                ["curious"] = new List<string>
                {
                    "Great question! I am glad you are curious about cybersecurity.",
                    "That is an excellent topic to explore. Here is what you should know.",
                    "I am glad you asked. Curiosity is the first step to better online safety."
                },
                ["frustrated"] = new List<string>
                {
                    "I hear your frustration. Cybersecurity can feel overwhelming. Let me simplify this.",
                    "Take a deep breath. I will break this down into simple steps.",
                    "I understand it can be frustrating. Let me help you understand this better."
                }
            };
        }

        // ===== MAIN RESPONSE METHOD =====
        public string GetResponse(string userInput)
        {
            string input = userInput.ToLower();

            // ===== TASK COMMANDS =====
            if (input.Contains("add task") || input.Contains("create task") || input.Contains("new task") ||
                input.Contains("show tasks") || input.Contains("my tasks") || input.Contains("list tasks") ||
                input.Contains("complete task") || input.Contains("done task") ||
                input.Contains("delete task") || input.Contains("remove task") ||
                input.Contains("all tasks"))
            {
                string taskResponse = HandleTaskCommand(input);
                if (taskResponse != null)
                {
                    currentTopic = "TASK MANAGER";
                    activityLogger.LogNLPInterpretation(userInput, "Task command");
                    return taskResponse;
                }
            }

            // ===== QUIZ COMMANDS =====
            if (input.Contains("start quiz") || input.Contains("take quiz") || input.Contains("begin quiz") ||
                input.Contains("quiz result") || input.Contains("show result") ||
                (quizManager.IsQuizActive() && (input == "a" || input == "b" || input == "c" || input == "d")))
            {
                string quizResponse = HandleQuizCommand(userInput);
                if (quizResponse != null)
                {
                    if (quizManager.IsQuizActive())
                    {
                        currentTopic = "QUIZ - QUESTION " + quizManager.GetCurrentQuestionIndex();
                    }
                    else
                    {
                        currentTopic = "QUIZ";
                    }
                    activityLogger.LogNLPInterpretation(userInput, "Quiz command");
                    return quizResponse;
                }
            }

            // ===== LOG COMMANDS =====
            if (input.Contains("show log") || input.Contains("activity log") ||
                input.Contains("what have you done") || input.Contains("show activity") ||
                input.Contains("full log") || input.Contains("clear log"))
            {
                string logResponse = HandleLogCommand(userInput);
                if (logResponse != null)
                {
                    currentTopic = "ACTIVITY LOG";
                    return logResponse;
                }
            }

            // ===== GET USER NAME =====
            if (string.IsNullOrEmpty(userName))
            {
                userName = userInput;
                currentTopic = "GREETING";
                activityLogger.LogMessage("USER GREETED", "User introduced themselves as: " + userName);
                return "Nice to meet you, " + userName + "!\n\nWhat cybersecurity topic would you like to learn about today?\n\n[PASSWORD] [SCAM] [PRIVACY] [PHISHING]\n\nOr try:\n[START QUIZ] to test your knowledge!\n[ADD TASK] to manage your cybersecurity tasks!\n[SHOW LOG] to see what I've done!";
            }

            // ===== MEMORY RECALL =====
            if (input.Contains("remember") || input.Contains("what do i like"))
            {
                if (!string.IsNullOrEmpty(favoriteTopic))
                {
                    return "You mentioned you are interested in " + favoriteTopic + ". Would you like to learn more?";
                }
                return "You haven't told me your favorite topic yet. Try asking about passwords, scams, privacy, or phishing!";
            }

            // ===== STORE FAVORITE TOPIC =====
            if (input.Contains("password")) favoriteTopic = "passwords";
            else if (input.Contains("scam")) favoriteTopic = "scams";
            else if (input.Contains("privacy")) favoriteTopic = "privacy";
            else if (input.Contains("phishing")) favoriteTopic = "phishing";

            // ===== SENTIMENT DETECTION =====
            DetectSentiment(input);
            string sentimentPrefix = GetSentimentResponse();

            // ===== CHECK KEYWORDS =====
            foreach (var keyword in responses.Keys)
            {
                if (input.Contains(keyword))
                {
                    lastCategory = keyword;
                    currentTopic = keyword.ToUpper() + " SECURITY";
                    var responseList = responses[keyword];
                    string response = responseList[random.Next(responseList.Count)];

                    if (currentSentiment != "neutral")
                    {
                        response = sentimentPrefix + "\n\n" + response;
                    }

                    return response + "\n\nSay 'tell me more' for another tip!";
                }
            }

            // ===== FOLLOW-UP =====
            if (input.Contains("more") || input.Contains("another"))
            {
                if (!string.IsNullOrEmpty(lastCategory) && responses.ContainsKey(lastCategory))
                {
                    var responseList = responses[lastCategory];
                    return responseList[random.Next(responseList.Count)] + "\n\nSay 'tell me more' for another tip!";
                }
                return "Ask me about passwords, scams, privacy, or phishing for specific advice!";
            }

            // ===== GOODBYE =====
            if (input.Contains("bye") || input.Contains("goodbye") || input.Contains("exit"))
            {
                currentTopic = "GOODBYE";
                activityLogger.LogMessage("USER GOODBYE", "User said goodbye");
                return "Goodbye, " + userName + "! Stay safe online!";
            }

            // ===== THANK YOU =====
            if (input.Contains("thank"))
            {
                currentTopic = "GRATITUDE";
                return "You are welcome, " + userName + "!";
            }

            // ===== HELP =====
            if (input.Contains("help"))
            {
                currentTopic = "HELP";
                activityLogger.LogMessage("HELP REQUESTED", "User asked for help");
                return "CYBERSECURITY TOPICS:\n[PASSWORD SAFETY] [SCAM DETECTION] [PRIVACY TIPS] [PHISHING]\n\n" +
                       "TASK MANAGEMENT:\n'Add task: [title] - [description]'\n'Show my tasks'\n'Complete task [ID]'\n'Delete task [ID]'\n\n" +
                       "CYBERSECURITY QUIZ:\n'Start quiz' - Test your knowledge!\n'A', 'B', 'C', or 'D' - Answer questions\n\n" +
                       "ACTIVITY LOG:\n'Show log' - View recent activities\n'Full log' - View all activities\n'Clear log' - Clear activity history";
            }

            // ===== DEFAULT =====
            currentTopic = "CONVERSATION";
            string[] defaultResponses = {
                "I am not sure I understand, " + userName + ". Try asking about passwords, scams, privacy, or phishing.",
                "Could you ask about specific topics like passwords, scams, privacy, or phishing?"
            };
            return defaultResponses[random.Next(defaultResponses.Length)];
        }

        // ===== TASK COMMAND HANDLER =====
        private string HandleTaskCommand(string input)
        {
            string lowerInput = input.ToLower();

            // Add task
            if (lowerInput.Contains("add task") || lowerInput.Contains("create task") || lowerInput.Contains("new task"))
            {
                TaskInfo info = taskManager.ExtractTaskInfo(input);

                // Log the NLP interpretation
                activityLogger.LogNLPInterpretation(input, "Add task: " + info.Title);

                string result = taskManager.AddTask(info.Title, info.Description, info.ReminderDate);

                if (info.ReminderDate.HasValue)
                {
                    return "Task added with reminder!\n\n" + result;
                }

                return result + "\n\nWould you like to set a reminder? Say 'remind me in X days'";
            }

            // Show tasks
            if (lowerInput.Contains("show tasks") || lowerInput.Contains("my tasks") || lowerInput.Contains("list tasks"))
            {
                activityLogger.LogNLPInterpretation(input, "Show tasks");
                return taskManager.GetPendingTasks();
            }

            // Show all tasks including completed
            if (lowerInput.Contains("all tasks") || lowerInput.Contains("show all"))
            {
                activityLogger.LogNLPInterpretation(input, "Show all tasks");
                return taskManager.GetAllTasks();
            }

            // Complete task
            if (lowerInput.Contains("complete task") || lowerInput.Contains("done task"))
            {
                string taskId = ExtractTaskId(input);
                if (!string.IsNullOrEmpty(taskId))
                {
                    activityLogger.LogNLPInterpretation(input, "Complete task ID: " + taskId);
                    return taskManager.CompleteTask(taskId);
                }
                return "Please specify which task to complete. Example: 'complete task 1'";
            }

            // Delete task
            if (lowerInput.Contains("delete task") || lowerInput.Contains("remove task"))
            {
                string taskId = ExtractTaskId(input);
                if (!string.IsNullOrEmpty(taskId))
                {
                    activityLogger.LogNLPInterpretation(input, "Delete task ID: " + taskId);
                    return taskManager.DeleteTask(taskId);
                }
                return "Please specify which task to delete. Example: 'delete task 1'";
            }

            return null;
        }

        // ===== QUIZ COMMAND HANDLER =====
        private string HandleQuizCommand(string input)
        {
            string lowerInput = input.ToLower();

            // Start quiz
            if (lowerInput.Contains("start quiz") || lowerInput.Contains("take quiz") ||
                lowerInput.Contains("begin quiz") || lowerInput.Contains("quiz"))
            {
                activityLogger.LogNLPInterpretation(input, "Start quiz");
                return quizManager.StartQuiz();
            }

            // Submit answer (A, B, C, D)
            if (quizManager.IsQuizActive())
            {
                string answer = input.Trim().ToUpper();
                if (answer == "A" || answer == "B" || answer == "C" || answer == "D" ||
                    answer == "A)" || answer == "B)" || answer == "C)" || answer == "D)" ||
                    input.Trim().Length == 1)
                {
                    // Clean up the answer
                    if (answer.EndsWith(")"))
                    {
                        answer = answer.Replace(")", "");
                    }
                    return quizManager.SubmitAnswer(answer);
                }
            }

            // Check if user is in quiz mode but didn't give a valid answer
            if (quizManager.IsQuizActive())
            {
                return "Please answer with A, B, C, or D.\n\n" + quizManager.GetCurrentQuestion();
            }

            // Show quiz result
            if (lowerInput.Contains("quiz result") || lowerInput.Contains("show result"))
            {
                return quizManager.GetQuizResult();
            }

            return null;
        }

        // ===== LOG COMMAND HANDLER =====
        private string HandleLogCommand(string input)
        {
            string lowerInput = input.ToLower();

            // Show recent logs
            if (lowerInput.Contains("show log") || lowerInput.Contains("activity log") ||
                lowerInput.Contains("what have you done") || lowerInput.Contains("show activity"))
            {
                activityLogger.AddLog("LOG VIEWED", "User requested activity log");
                return activityLogger.GetRecentLogs(10);
            }

            // Show full log
            if (lowerInput.Contains("full log") || lowerInput.Contains("show all"))
            {
                return activityLogger.GetFullLog();
            }

            // Clear log
            if (lowerInput.Contains("clear log") || lowerInput.Contains("delete log"))
            {
                activityLogger.ClearLogs();
                return "Activity log has been cleared.";
            }

            return null;
        }

        // ===== EXTRACT TASK ID FROM USER INPUT =====
        private string ExtractTaskId(string input)
        {
            string[] words = input.Split(' ');
            foreach (string word in words)
            {
                if (int.TryParse(word, out int id))
                {
                    return word;
                }
            }
            return null;
        }

        // ===== SENTIMENT DETECTION =====
        private void DetectSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("nervous"))
                currentSentiment = "worried";
            else if (input.Contains("curious") || input.Contains("interested") || input.Contains("wonder"))
                currentSentiment = "curious";
            else if (input.Contains("frustrated") || input.Contains("confused") || input.Contains("hard"))
                currentSentiment = "frustrated";
            else
                currentSentiment = "neutral";
        }

        // ===== GET SENTIMENT RESPONSE =====
        private string GetSentimentResponse()
        {
            if (currentSentiment != "neutral" && sentimentResponses.ContainsKey(currentSentiment))
            {
                var responses = sentimentResponses[currentSentiment];
                return responses[random.Next(responses.Count)];
            }
            return "";
        }

        // ===== PUBLIC GETTERS =====
        public string GetCurrentTopic()
        {
            return currentTopic;
        }

        public string GetCurrentSentiment()
        {
            return currentSentiment;
        }

        public void ResetMemory()
        {
            userName = "";
            favoriteTopic = "";
            lastCategory = "";
            currentSentiment = "neutral";
            currentTopic = "General";
            activityLogger.AddLog("MEMORY RESET", "User memory was reset");
        }
    }
}