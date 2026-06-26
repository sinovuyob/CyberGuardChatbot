using System;
using System.Collections.Generic;

namespace CyberGuardChatbot
{
    public class QuizManager
    {
        private List<QuizQuestion> questions;
        private int currentQuestionIndex;
        private int score;
        private bool quizActive;
        private ActivityLogger logger; 

        public QuizManager()
        {
            InitializeQuestions();
            currentQuestionIndex = 0;
            score = 0;
            quizActive = false;
            logger = new ActivityLogger();  
        }

        private void InitializeQuestions()
        {
            questions = new List<QuizQuestion>
            {
                // Question 1 - Multiple Choice
                new QuizQuestion(
                    "What should you do if you receive an email asking for your password?",
                    new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    2,  // Index 2 = "Report the email as phishing"
                    "Reporting phishing emails helps prevent scams and protects others from falling victim."
                ),

                // Question 2 - Multiple Choice
                new QuizQuestion(
                    "What makes a strong password?",
                    new List<string> { "Your birthday", "8 characters", "12+ characters with numbers, symbols, and mixed case", "Your pet's name" },
                    2,  // Index 2 = "12+ characters with numbers, symbols, and mixed case"
                    "Strong passwords use a combination of uppercase, lowercase, numbers, and symbols with at least 12 characters."
                ),

                // Question 3 - True/False
                new QuizQuestion(
                    "Is it safe to use public Wi-Fi without a VPN?",
                    new List<string> { "True", "False" },
                    1,  // Index 1 = "False"
                    "Public Wi-Fi networks are not secure. Using a VPN encrypts your data and protects you from hackers."
                ),

                // Question 4 - Multiple Choice
                new QuizQuestion(
                    "What is Two-Factor Authentication (2FA)?",
                    new List<string> { "A type of password", "An extra security layer", "A computer virus", "A type of scam" },
                    1,  // Index 1 = "An extra security layer"
                    "2FA adds an extra layer of security by requiring a second verification step, like a code from your phone."
                ),

                // Question 5 - True/False
                new QuizQuestion(
                    "Should you reuse passwords across different websites?",
                    new List<string> { "True", "False" },
                    1,  // Index 1 = "False"
                    "Reusing passwords across sites is dangerous. If one site is compromised, all your accounts with that password are at risk."
                ),

                // Question 6 - Multiple Choice
                new QuizQuestion(
                    "What is phishing?",
                    new List<string> { "A sport", "A hacking method", "Fake emails or messages to steal personal information", "A type of software" },
                    2,  // Index 2 = "Fake emails or messages to steal personal information"
                    "Phishing is a cyber attack where criminals send fraudulent messages to trick you into revealing sensitive information."
                ),

                // Question 7 - Multiple Choice
                new QuizQuestion(
                    "How often should you update your passwords?",
                    new List<string> { "Never", "Every 3-6 months", "Once a year", "Only when forced" },
                    1,  // Index 1 = "Every 3-6 months"
                    "Regular password updates (every 3-6 months) help protect your accounts from unauthorized access."
                ),

                // Question 8 - True/False
                new QuizQuestion(
                    "Is it safe to click links in suspicious emails?",
                    new List<string> { "True", "False" },
                    1,  // Index 1 = "False"
                    "Never click links in suspicious emails. Always hover over links to check the actual URL before clicking."
                ),

                // Question 9 - Multiple Choice
                new QuizQuestion(
                    "What is malware?",
                    new List<string> { "A type of software", "Malicious software designed to harm your computer", "A computer virus", "A web browser" },
                    1,  // Index 1 = "Malicious software designed to harm your computer"
                    "Malware is any software designed to damage, disrupt, or gain unauthorized access to your computer system."
                ),

                // Question 10 - Multiple Choice
                new QuizQuestion(
                    "What should you do if your password is leaked in a data breach?",
                    new List<string> { "Ignore it", "Change your password immediately", "Tell your friends", "Do nothing" },
                    1,  // Index 1 = "Change your password immediately"
                    "If your password is leaked, change it immediately on all accounts where you used it. Also enable 2FA if available."
                ),

                // Question 11 - Multiple Choice (Bonus)
                new QuizQuestion(
                    "What is social engineering?",
                    new List<string> { "Building social networks", "Manipulating people to reveal confidential information", "A type of software", "A coding language" },
                    1,  // Index 1 = "Manipulating people to reveal confidential information"
                    "Social engineering is the psychological manipulation of people to trick them into revealing sensitive information."
                ),

                // Question 12 - True/False
                new QuizQuestion(
                    "Should you use the same password for your bank and social media accounts?",
                    new List<string> { "True", "False" },
                    1,  // Index 1 = "False"
                    "Never use the same password for financial accounts and social media. Financial accounts need the highest level of security."
                )
            };
        }

        public string StartQuiz()
        {
            currentQuestionIndex = 0;
            score = 0;
            quizActive = true;

            // ===== LOG THE ACTION =====
            logger.LogQuizStarted();

            return "QUIZ STARTED!\n\n" + GetCurrentQuestion() + "\n\nType the letter of your answer (A, B, C, or D)";
        }

        public string GetCurrentQuestion()
        {
            if (!quizActive || currentQuestionIndex >= questions.Count)
            {
                return "Quiz not active. Type 'start quiz' to begin!";
            }

            var question = questions[currentQuestionIndex];
            return question.GetDisplayText();
        }

        public string SubmitAnswer(string userInput)
        {
            if (!quizActive)
            {
                return "Quiz not active. Type 'start quiz' to begin!";
            }

            if (currentQuestionIndex >= questions.Count)
            {
                return GetQuizResult();
            }

            string input = userInput.ToUpper().Trim();
            int selectedIndex = -1;

            // Convert A, B, C, D to index
            if (input == "A") selectedIndex = 0;
            else if (input == "B") selectedIndex = 1;
            else if (input == "C") selectedIndex = 2;
            else if (input == "D") selectedIndex = 3;

            // Also check if user typed the full answer
            if (selectedIndex == -1)
            {
                var question = questions[currentQuestionIndex];
                for (int i = 0; i < question.Options.Count; i++)
                {
                    if (question.Options[i].ToLower() == userInput.ToLower())
                    {
                        selectedIndex = i;
                        break;
                    }
                }
            }

            if (selectedIndex == -1)
            {
                return "Invalid input. Please type A, B, C, or D.";
            }

            var currentQuestion = questions[currentQuestionIndex];
            bool isCorrect = (selectedIndex == currentQuestion.CorrectAnswerIndex);

            string feedback = "";
            if (isCorrect)
            {
                score++;
                feedback = "CORRECT! " + currentQuestion.Explanation;
            }
            else
            {
                string correctAnswer = currentQuestion.Options[currentQuestion.CorrectAnswerIndex];
                feedback = "INCORRECT. The correct answer was: " + correctAnswer + "\n\n" + currentQuestion.Explanation;
            }

            currentQuestionIndex++;

            // Check if quiz is complete
            if (currentQuestionIndex >= questions.Count)
            {
                quizActive = false;

                // ===== LOG THE COMPLETION =====
                logger.LogQuizCompleted(score, questions.Count);

                return feedback + "\n\n" + GetQuizResult();
            }

            return feedback + "\n\nNEXT QUESTION:\n" + GetCurrentQuestion();
        }

        public string GetQuizResult()
        {
            quizActive = false;
            int totalQuestions = questions.Count;
            double percentage = (double)score / totalQuestions * 100;

            string result = "QUIZ COMPLETE!\n";
            result += "Score: " + score + " out of " + totalQuestions + " (" + percentage.ToString("F1") + "%)\n\n";

            if (percentage >= 80)
            {
                result += "EXCELLENT JOB! You are a cybersecurity pro! Keep up the great work staying safe online!";
            }
            else if (percentage >= 60)
            {
                result += "GOOD EFFORT! You have a solid understanding of cybersecurity basics. Keep learning to become even safer!";
            }
            else if (percentage >= 40)
            {
                result += "NOT BAD! Review the topics you missed and try again. Cybersecurity is an important skill to develop!";
            }
            else
            {
                result += "KEEP LEARNING! Cybersecurity is an important topic. Try studying the topics you missed and take the quiz again!";
            }

            return result;
        }

        public bool IsQuizActive()
        {
            return quizActive;
        }

        public int GetCurrentQuestionIndex()
        {
            return currentQuestionIndex + 1;
        }

        public int GetTotalQuestions()
        {
            return questions.Count;
        }
    }

    // Quiz Question Class
    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }

        public QuizQuestion(string question, List<string> options, int correctAnswerIndex, string explanation)
        {
            Question = question;
            Options = options;
            CorrectAnswerIndex = correctAnswerIndex;
            Explanation = explanation;
        }

        public string GetDisplayText()
        {
            string result = "Question " + (GetQuestionNumber()) + ": " + Question + "\n\n";
            string[] letters = { "A", "B", "C", "D" };

            for (int i = 0; i < Options.Count; i++)
            {
                result += "  " + letters[i] + ") " + Options[i] + "\n";
            }

            return result;
        }

        private int GetQuestionNumber()
        {
            // This is a helper - the actual question number is tracked by QuizManager
            return 0;
        }
    }
}