using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CyberGuardChatbot
{
    public partial class MainWindow : Window
    {
        private Chatbot bot;
        private List<ChatMessage> messages;

        public MainWindow()
        {
            InitializeComponent();
            bot = new Chatbot();
            messages = new List<ChatMessage>();
            ChatList.ItemsSource = messages;

            // Play greeting
            try { AudioManager.PlayGreeting(); } catch { }

            // Welcome message
            AddBotMessage("WELCOME TO CYBERGUARD AI!\n\nI'm your personal cybersecurity assistant. Let's have a conversation!\n\n" +
                         "Try asking me:\n" +
                         "• 'Give me password tips'\n" +
                         "• 'How do I avoid scams?'\n" +
                         "• 'Privacy protection tips'\n" +
                         "• 'Virus prevention advice'\n\n" +
                         "You can also say 'tell me more' or 'another tip' to continue the conversation!");
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
                StatusBar.Text = "Please type a message";
                return;
            }

            AddUserMessage(input);
            InputBox.Clear();
            StatusBar.Text = "Thinking...";

            string response = bot.GetResponse(input);
            AddBotMessage(response);
            StatusBar.Text = "Reply sent! Ask for more tips!";
        }

        private void ClearChat()
        {
            messages.Clear();
            AddBotMessage("Chat cleared! Let's continue our conversation about cybersecurity.\n\nWhat would you like to learn about?");
            StatusBar.Text = "Chat cleared";
        }

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
    }

    // Chat Message Class
    public class ChatMessage
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
                Color = Brushes.SaddleBrown;
            }
            else
            {
                Alignment = HorizontalAlignment.Left;
                Color = Brushes.Tan;
            }
        }
    }

    // Audio Manager
    public class AudioManager
    {
        public static void PlayGreeting()
        {
            try
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Audio", "greeting.wav");
                if (System.IO.File.Exists(path))
                {
                    using (var player = new System.Media.SoundPlayer(path))
                        player.PlaySync();
                }
            }
            catch { }
        }
    }

    // Chatbot Engine class
    public class Chatbot
    {
        private string currentTopic = "General";
        private string lastCategory = "";
        private List<string> conversationHistory = new List<string>();

        public string GetResponse(string userInput)
        {
            string input = userInput.ToLower();

            // Store conversation history
            conversationHistory.Add(userInput);

            // ========== PASSWORD CONVERSATION ==========
            if (input.Contains("password") || input.Contains("passwords"))
            {
                lastCategory = "password";
                currentTopic = "PASSWORD SECURITY";
                return "Let's talk about password security!\n\n" + GetPasswordTip() + "\n\nWant more password tips? Just say 'tell me more' or 'another password tip'!";
            }

            // ========== SCAM CONVERSATION ==========
            if (input.Contains("scam") || input.Contains("phish") || input.Contains("phishing"))
            {
                lastCategory = "scam";
                currentTopic = "SCAM PROTECTION";
                return "Great topic! Let me help you avoid scams.\n\n" + GetScamTip() + "\n\nSay 'another scam tip' for more protection advice!";
            }

            // ========== PRIVACY CONVERSATION ==========
            if (input.Contains("privacy"))
            {
                lastCategory = "privacy";
                currentTopic = "PRIVACY TIPS";
                return "Privacy is very important! Here's how to protect yourself:\n\n" + GetPrivacyTip() + "\n\nAsk for 'another privacy tip' to learn more!";
            }

            // ========== VIRUS CONVERSATION ==========
            if (input.Contains("virus") || input.Contains("malware"))
            {
                lastCategory = "virus";
                currentTopic = "VIRUS PROTECTION";
                return "Let me help you protect against viruses!\n\n" + GetVirusTip() + "\n\nSay 'another virus tip' for more prevention advice!";
            }

            // ========== SECURITY CONVERSATION ==========
            if (input.Contains("security"))
            {
                lastCategory = "security";
                currentTopic = "SECURITY TIPS";
                return "Here's an important security tip:\n\n" + GetSecurityTip() + "\n\nWant more security advice? Just ask for 'another security tip'!";
            }

            // ========== FOLLOW-UP CONVERSATION ("tell me more", "another tip") ==========
            if (input.Contains("more") || input.Contains("another") || input.Contains("tell me more") || input.Contains("another tip"))
            {
                if (lastCategory == "password")
                {
                    return "Here's another password security tip!\n\n" + GetPasswordTip() + "\n\nKeep asking for 'more tips' to learn more!";
                }
                if (lastCategory == "scam")
                {
                    return "Here's another scam protection tip!\n\n" + GetScamTip() + "\n\nSay 'another scam tip' for more!";
                }
                if (lastCategory == "privacy")
                {
                    return "Here's another privacy tip!\n\n" + GetPrivacyTip() + "\n\nAsk for 'more privacy tips' to continue!";
                }
                if (lastCategory == "virus")
                {
                    return "Here's another virus prevention tip!\n\n" + GetVirusTip() + "\n\nSay 'another virus tip' for more protection advice!";
                }
                if (lastCategory == "security")
                {
                    return "Here's another security tip!\n\n" + GetSecurityTip() + "\n\nWant even more? Just ask again!";
                }

                // Default follow-up
                return "Here's a helpful cybersecurity tip:\n\n" + GetRandomTip() + "\n\nAsk me about passwords, scams, privacy, or viruses for specific advice!";
            }

            // ========== WORRIED/CONCERNED CONVERSATION ==========
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("nervous") || input.Contains("concerned") || input.Contains("afraid"))
            {
                currentTopic = "SUPPORT";
                return "I understand you're concerned about cybersecurity. That's completely normal!\n\n" +
                       "Let me give you some reassurance:\n\n" +
                       GetComfortingTip() + "\n\n" +
                       "Remember: Taking small steps makes a big difference. Want to learn about a specific topic?";
            }

            // ========== GRATITUDE CONVERSATION ==========
            if (input.Contains("thank") || input.Contains("thanks"))
            {
                currentTopic = "GRATITUDE";
                return "You're very welcome! I'm glad I could help you stay safe online.\n\n" +
                       "Would you like more tips on passwords, scams, privacy, or viruses?";
            }

            // ========== GREETING CONVERSATION ==========
            if (input.Contains("hello") || input.Contains("hi") || input.Contains("hey") || input.Contains("greetings"))
            {
                currentTopic = "GREETING";
                return "Hello there! Welcome to CyberGuard AI!\n\n" +
                       "I'm here to have a conversation about cybersecurity. What would you like to discuss?\n\n" +
                       "Try asking:\n" +
                       "• 'Give me password tips'\n" +
                       "• 'How to avoid scams?'\n" +
                       "• 'Privacy protection tips'\n" +
                       "• 'Virus prevention advice'";
            }

            // ========== HELP CONVERSATION ==========
            if (input.Contains("help") || input.Contains("what can you do") || input.Contains("how to use"))
            {
                currentTopic = "HELP";
                return "I'm your cybersecurity conversation partner!\n\n" +
                       "Here's how to talk with me:\n\n" +
                       "• Ask for 'password tips' - I'll give you password advice\n" +
                       "• Ask for 'scam tips' - I'll teach you about phishing\n" +
                       "• Ask for 'privacy tips' - I'll share privacy protection\n" +
                       "• Ask for 'virus tips' - I'll help with malware prevention\n" +
                       "• Say 'tell me more' or 'another tip' - I'll continue the conversation!\n\n" +
                       "• Say 'thank you' - I'll appreciate it!\n\n" +
                       "What would you like to learn about today?";
            }

            // ========== GOODBYE CONVERSATION ==========
            if (input.Contains("bye") || input.Contains("goodbye") || input.Contains("see you"))
            {
                currentTopic = "GOODBYE";
                return "Goodbye! Stay safe online!\n\n" +
                       "Remember:\n" +
                       "• Use strong passwords\n" +
                       "• Watch out for scams\n" +
                       "• Protect your privacy\n" +
                       "• Keep software updated\n\n" +
                       "Come back anytime for more cybersecurity tips!";
            }

            // ========== DEFAULT CONVERSATION ==========
            currentTopic = "CONVERSATION";
            return "I'm here to talk about cybersecurity!\n\n" +
                   "Let's have a conversation. Try asking me:\n\n" +
                   "• 'Give me password tips'\n" +
                   "• 'How do I avoid scams?'\n" +
                   "• 'Privacy protection tips'\n" +
                   "• 'Virus prevention advice'\n\n" +
                   "• Or just say 'tell me more' and I'll continue!\n\n" +
                   "What would you like to discuss about online safety?";
        }

        private string GetPasswordTip()
        {
            Random rand = new Random();
            int choice = rand.Next(1, 6);

            switch (choice)
            {
                case 1:
                    return "Use passwords with at least 12 characters including uppercase, lowercase, numbers, and symbols. Example: 'MyDogLikesTacos!2024'";
                case 2:
                    return "Never reuse passwords across websites. Use a password manager like Bitwarden or LastPass to store unique passwords!";
                case 3:
                    return "Enable Two-Factor Authentication (2FA) on all accounts. This adds an extra layer of security even if your password is stolen!";
                case 4:
                    return "Avoid using personal information like birthdays, pet names, or addresses in passwords. Hackers can easily find this online!";
                case 5:
                    return "Change default passwords on all smart devices (routers, cameras, smart TVs) immediately. Default passwords are easy for hackers to guess!";
                default:
                    return "Use a passphrase like 'Correct-Horse-Battery-Staple' - it's long, memorable, and hard to crack!";
            }
        }

        private string GetScamTip()
        {
            Random rand = new Random();
            int choice = rand.Next(1, 6);

            switch (choice)
            {
                case 1:
                    return "Never click links in unsolicited emails or texts. Hover over links to see the actual URL before clicking!";
                case 2:
                    return "Scammers create urgency ('Your account will be closed!'). Always verify through official channels by calling the company directly!";
                case 3:
                    return "Check sender email addresses carefully. Scammers use slight misspellings like 'arnazon.com' instead of 'amazon.com'!";
                case 4:
                    return "Never share personal information (passwords, credit cards, SSN) over the phone unless YOU initiated the call!";
                case 5:
                    return "If something sounds too good to be true (free prizes, lottery winnings), it's almost always a scam!";
                default:
                    return "Legitimate companies never ask for passwords via email. Report phishing attempts to the real company!";
            }
        }

        private string GetPrivacyTip()
        {
            Random rand = new Random();
            int choice = rand.Next(1, 6);

            switch (choice)
            {
                case 1:
                    return "Review app permissions on your phone. Many apps request access to contacts, location, and camera when they don't need it!";
                case 2:
                    return "Use encrypted messaging apps like Signal or WhatsApp for sensitive conversations. Regular SMS texts are not encrypted!";
                case 3:
                    return "Cover your webcam with a sliding cover when not in use. Hackers can potentially access your camera without your knowledge!";
                case 4:
                    return "Use privacy-focused search engines like DuckDuckGo instead of Google. They don't track your searches!";
                case 5:
                    return "Disable location tracking for apps that don't need it (games, calculators, flashlights). Check your phone's privacy settings!";
                default:
                    return "Regularly clear your browser cookies and cache. Use private/incognito mode for sensitive browsing!";
            }
        }

        private string GetVirusTip()
        {
            Random rand = new Random();
            int choice = rand.Next(1, 6);

            switch (choice)
            {
                case 1:
                    return "Keep Windows Defender (or your antivirus) updated and run regular scans. Windows Defender is excellent and free!";
                case 2:
                    return "Don't download software from untrusted websites. Only download from official sources like the developer's website!";
                case 3:
                    return "Be extremely careful with email attachments, even from people you know. Their account might be compromised!";
                case 4:
                    return "Signs your computer has malware:\n• Slow performance\n• Pop-up ads everywhere\n• Browser redirects\n• Unexplained file changes";
                case 5:
                    return "Enable ransomware protection in Windows Security. This prevents unauthorized apps from modifying your important files!";
                default:
                    return "Never plug unknown USB drives into your computer. They could contain malware that auto-executes when inserted!";
            }
        }

        private string GetSecurityTip()
        {
            Random rand = new Random();
            int choice = rand.Next(1, 6);

            switch (choice)
            {
                case 1:
                    return "Keep your operating system, browsers, and apps updated. Security patches fix vulnerabilities that hackers exploit!";
                case 2:
                    return "Use a VPN when connecting to public Wi-Fi (coffee shops, airports, hotels). Public networks are easy targets for hackers!";
                case 3:
                    return "Backup your important files regularly using the 3-2-1 rule: 3 copies, 2 different media types, 1 offsite backup!";
                case 4:
                    return "Lock your phone and computer when you step away - even for a minute. This prevents unauthorized access!";
                case 5:
                    return "Review your social media privacy settings. Limit what personal information is publicly visible to strangers!";
                default:
                    return "Use a firewall and reputable antivirus software to block malicious traffic and malware!";
            }
        }

        private string GetComfortingTip()
        {
            Random rand = new Random();
            int choice = rand.Next(1, 4);

            switch (choice)
            {
                case 1:
                    return "Most cyber attacks can be prevented with basic security habits. You're already taking the first step by learning about cybersecurity!";
                case 2:
                    return "Millions of people face these same concerns. Following basic security practices makes you much safer than the average person!";
                case 3:
                    return "Start with one small change: enable 2FA on your email account. Small steps build strong protection over time!";
                default:
                    return "Remember: No one is 100% safe online, but good habits reduce your risk significantly. You've got this!";
            }
        }

        private string GetRandomTip()
        {
            Random rand = new Random();
            int choice = rand.Next(1, 11);

            switch (choice)
            {
                case 1: return "Use unique passwords for every online account!";
                case 2: return "Enable Two-Factor Authentication wherever possible!";
                case 3: return "Never click suspicious links in emails or texts!";
                case 4: return "Keep your software updated for security patches!";
                case 5: return "Use a VPN on public Wi-Fi networks!";
                case 6: return "Back up your important files regularly!";
                case 7: return "Review app permissions on your phone!";
                case 8: return "Lock your devices when stepping away!";
                case 9: return "Be cautious of urgent requests for personal info!";
                case 10: return "Use encrypted messaging for sensitive conversations!";
                default: return "Stay informed about common cybersecurity threats!";
            }
        }

        public string GetCurrentTopic()
        {
            return currentTopic;
        }
    }
}