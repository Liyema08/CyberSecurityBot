using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityBot
{
    public class ChatbotEngine
    {
        // ===============================================
        // MEMORY & STATE
        // ===============================================
        private string userName = "";
        private string userInterest = "";
        private string lastTopic = "";
        private Random random = new Random();
        private ActivityLogger _logger = new ActivityLogger();

        // ===============================================
        // TASK MANAGER (JSON Storage)
        // ===============================================
        private TaskManager _taskManager = new TaskManager();

        // ===============================================
        // QUIZ - 12 Questions
        // ===============================================
        private QuizManager _quizManager = new QuizManager();
        private bool isQuizActive = false;

        // ===============================================
        // SENTIMENT WORD BANKS
        // ===============================================
        private string[] worriedWords = { "worried", "scared", "fear", "concerned", "nervous", "anxious", "afraid", "unsafe" };
        private string[] frustratedWords = { "frustrated", "annoyed", "difficult", "hard", "confused", "angry", "upset" };
        private string[] curiousWords = { "curious", "interested", "tell me", "explain", "learn", "what is", "how does" };
        private string[] happyWords = { "happy", "great", "awesome", "good", "excellent", "fantastic", "thanks" };

        // ===============================================
        // RESPONSE DATABASES
        // ===============================================
        private Dictionary<string, List<string>> responseMap = null!;
        private Dictionary<string, List<string>> followUpMap = null!;

        // ===============================================
        // CONSTRUCTOR
        // ===============================================
        public ChatbotEngine()
        {
            InitializeResponses();
            InitializeFollowUps();
            _logger.Log("Bot initialized");
        }

        // ===============================================
        // RESPONSE SYSTEMS
        // ===============================================
        private void InitializeResponses()
        {
            responseMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            responseMap["password"] = new List<string>
            {
                "Use strong passwords with at least 12 characters. Mix uppercase, lowercase, numbers, and symbols!",
                "Never reuse passwords across different accounts. Use a password manager like Bitwarden!",
                "Enable Two-Factor Authentication (2FA) whenever possible for extra security.",
                "Avoid using personal information like birthdays or pet names in your passwords.",
                "Change your passwords immediately if you suspect a data breach."
            };

            responseMap["phish"] = new List<string>
            {
                "Watch for urgent language, spelling errors, and suspicious links in emails.",
                "Never click links in unsolicited emails. Hover to see the real URL first.",
                "Legitimate companies never ask for passwords via email. Be very suspicious!",
                "Check the sender's email address carefully - scammers use fake addresses.",
                "If an offer seems too good to be true, it's probably a phishing scam."
            };
            responseMap["scam"] = responseMap["phish"];

            responseMap["privacy"] = new List<string>
            {
                "Review your privacy settings on social media regularly. Limit what you share publicly.",
                "Use a VPN on public Wi-Fi to protect your personal information from hackers.",
                "Be careful what personal information you share online - it can be used for identity theft.",
                "Check if your data has been breached using HaveIBeenPwned.com.",
                "Use encrypted messaging apps like Signal or WhatsApp for private conversations."
            };

            responseMap["brows"] = new List<string>
            {
                "Look for 'https://' and the padlock icon in your browser address bar.",
                "Keep your browser and extensions updated for the latest security patches.",
                "Use ad-blockers and avoid clicking on pop-up advertisements.",
                "Clear your browser cache and cookies regularly to remove tracking data.",
                "Consider using privacy-focused browsers like Firefox or Brave."
            };

            responseMap["default"] = new List<string>
            {
                "I'm not sure I understand. Try: 'Help' to see commands, or ask about passwords, phishing, or privacy.",
                "Could you rephrase that? I can help with cybersecurity topics or tasks.",
                "I don't recognize that. Try 'Help' for commands, or ask about cybersecurity topics.",
                "Hmm, I didn't catch that. Try 'Add task:', 'Start quiz', or 'Activity log'"
            };
        }

        private void InitializeFollowUps()
        {
            followUpMap = new Dictionary<string, List<string>>();
            followUpMap["password"] = new List<string>
            {
                "Another password tip: Use a passphrase - a sequence of random words!",
                "Did you know? Password managers can generate and store strong passwords automatically.",
                "Also important: Don't write passwords on sticky notes attached to your monitor!"
            };
            followUpMap["phish"] = new List<string>
            {
                "Another phishing tip: Check the URL carefully. Scammers use similar spellings!",
                "Did you know? Scammers often create fake urgency to make you act without thinking.",
                "Also important: If you're unsure about an email, don't click anything."
            };
            followUpMap["general"] = new List<string>
            {
                "Always keep your software updated to protect against known vulnerabilities.",
                "Back up your important files regularly to an external drive or cloud service.",
                "Use unique passwords for every account - don't reuse them across different sites.",
                "Enable two-factor authentication on all accounts that offer it."
            };
        }

        private string GetKeywordResponse(string input)
        {
            if (input.Contains("another tip") || input.Contains("tell me more") || input.Contains("explain more"))
            {
                if (!string.IsNullOrEmpty(lastTopic) && followUpMap.ContainsKey(lastTopic))
                    return followUpMap[lastTopic][random.Next(followUpMap[lastTopic].Count)];
                return followUpMap["general"][random.Next(followUpMap["general"].Count)];
            }

            foreach (var key in responseMap.Keys)
            {
                if (input.Contains(key))
                {
                    lastTopic = key;
                    return responseMap[key][random.Next(responseMap[key].Count)];
                }
            }

            lastTopic = "";
            return responseMap["default"][random.Next(responseMap["default"].Count)];
        }

        private string CapitalizeName(string name)
        {
            name = name.Trim();
            if (string.IsNullOrEmpty(name)) return "Friend";
            if (name.Contains(" ")) name = name.Split(' ')[0];
            name = new string(name.Where(c => char.IsLetter(c)).ToArray());
            if (string.IsNullOrEmpty(name)) return "Friend";
            return char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }

        // ===============================================
        // NLP INTENT DETECTION
        // ===============================================
        private string ProcessNLPIntent(string input, string lowerInput)
        {
            // TASK INTENTS
            if (lowerInput.Contains("add task") || lowerInput.Contains("add a task") || 
                lowerInput.Contains("create task") || lowerInput.Contains("new task"))
            {
                string title = input;
                if (lowerInput.Contains("add task"))
                    title = input.Substring(input.ToLower().IndexOf("task") + 4).Trim();
                else if (lowerInput.Contains("add a task"))
                    title = input.Substring(input.ToLower().IndexOf("task") + 4).Trim();
                else if (lowerInput.Contains("create task"))
                    title = input.Substring(input.ToLower().IndexOf("task") + 4).Trim();

                if (string.IsNullOrEmpty(title))
                    return "Please provide a task description. Example: 'Add task: Review privacy settings'";

                _logger.Log($"NLP: Task intent detected - '{title}'");
                return _taskManager.AddTask(title, "", "");
            }

            if (lowerInput.Contains("show tasks") || lowerInput.Contains("view tasks") || lowerInput.Contains("my tasks"))
            {
                var tasks = _taskManager.GetAllTasks();
                if (tasks.Count == 0)
                    return "You have no tasks. Add one with: 'Add task: [title]'";

                string result = "📋 YOUR TASKS\n════════════════════════════════════════\n";
                foreach (var task in tasks)
                {
                    string status = task.IsComplete ? "✅ COMPLETE" : "⏳ PENDING";
                    result += $"[{task.Id}] {task.Title} - {status}\n";
                    if (!string.IsNullOrEmpty(task.Description))
                        result += $"   📝 {task.Description}\n";
                }
                result += "\nCommands: Complete task [id] | Delete task [id]";
                _logger.Log("User viewed tasks");
                return result;
            }

            if (lowerInput.Contains("complete task"))
            {
                var parts = input.Split(' ');
                if (int.TryParse(parts[parts.Length - 1], out int id))
                    return _taskManager.MarkAsComplete(id);
                return "Please specify the task ID. Example: 'Complete task 1'";
            }

            if (lowerInput.Contains("delete task"))
            {
                var parts = input.Split(' ');
                if (int.TryParse(parts[parts.Length - 1], out int id))
                    return _taskManager.DeleteTask(id);
                return "Please specify the task ID. Example: 'Delete task 1'";
            }

            // REMINDER INTENT
            if (lowerInput.Contains("remind me") || lowerInput.Contains("set a reminder"))
            {
                string reminderText = input;
                if (lowerInput.Contains("remind me"))
                    reminderText = input.Substring(input.ToLower().IndexOf("remind me") + 9).Trim();
                else if (lowerInput.Contains("set a reminder"))
                    reminderText = input.Substring(input.ToLower().IndexOf("reminder") + 8).Trim();

                if (string.IsNullOrEmpty(reminderText))
                    return "What would you like me to remind you about?";

                _logger.Log($"Reminder set: '{reminderText}'");
                return $"✅ Reminder set for '{reminderText}'!";
            }

            // QUIZ INTENT
            if (lowerInput.Contains("start quiz") || lowerInput.Contains("take quiz") || 
                lowerInput.Contains("quiz me") || lowerInput.Contains("play quiz"))
            {
                _logger.Log("User started the cybersecurity quiz");
                return StartQuiz();
            }

            // QUIZ ANSWER
            if (isQuizActive && int.TryParse(input, out int answer))
            {
                return AnswerQuiz(input);
            }

            // ACTIVITY LOG INTENT
            if (lowerInput.Contains("activity log") || lowerInput.Contains("what have you done") ||
                lowerInput.Contains("what did you do") || lowerInput.Contains("show log"))
            {
                _logger.Log("User viewed activity log");
                return _logger.GetRecentLog();
            }

            // SHOW MORE LOG
            if (lowerInput.Contains("show more"))
            {
                return _logger.GetFullLog();
            }

            // HELP
            if (lowerInput.Contains("help"))
            {
                return @"📖 AVAILABLE COMMANDS
════════════════════════════════════════
1. Add task: [title] - Create a new task
2. Show tasks - View all tasks
3. Complete task [id] - Mark task as done
4. Delete task [id] - Remove a task
5. Start quiz - Begin cybersecurity quiz
6. Activity log - View recent actions
7. Remind me [something] - Set a reminder
8. Tell me about passwords - Cybersecurity tips";
            }

            return null;
        }

        // ===============================================
        // QUIZ METHODS
        // ===============================================
        public string StartQuiz()
        {
            _quizManager.Reset();
            isQuizActive = true;
            _logger.Log("Quiz started");
            return _quizManager.GetCurrentQuestionText();
        }

        public string AnswerQuiz(string input)
        {
            if (!isQuizActive)
                return "The quiz is not active. Type 'Start quiz' to begin!";

            if (int.TryParse(input, out int answerIndex) && answerIndex >= 1 && answerIndex <= 4)
            {
                string feedback = _quizManager.SubmitAnswer(answerIndex - 1);
                
                if (_quizManager.IsFinished())
                {
                    isQuizActive = false;
                    return feedback + "\n\n" + _quizManager.GetFinalScore();
                }
                else
                {
                    return feedback + "\n\n" + _quizManager.GetCurrentQuestionText();
                }
            }
            return "Please enter a valid number (1-4) for your answer.";
        }

        // ===============================================
        // MAIN GET RESPONSE METHOD
        // ===============================================
        public string GetResponse(string userInput)
        {
            string lowerInput = userInput.ToLower().Trim();

            // Check NLP intents first
            string nlpResult = ProcessNLPIntent(userInput, lowerInput);
            if (nlpResult != null) return nlpResult;

            // Extract name
            if (string.IsNullOrEmpty(userName))
            {
                if (lowerInput.Contains("my name is"))
                {
                    int index = lowerInput.IndexOf("my name is") + 10;
                    string name = lowerInput.Substring(index).Trim();
                    if (!string.IsNullOrEmpty(name))
                    {
                        userName = CapitalizeName(name);
                        _logger.Log($"User identified as {userName}");
                        return $"Nice to meet you, {userName}! I'm your cybersecurity assistant. Type 'Help' to see all commands.";
                    }
                }

                if (lowerInput.Contains("i am ") || lowerInput.Contains("i'm "))
                {
                    string name = "";
                    if (lowerInput.Contains("i am "))
                        name = lowerInput.Substring(lowerInput.IndexOf("i am ") + 5).Trim();
                    else if (lowerInput.Contains("i'm "))
                        name = lowerInput.Substring(lowerInput.IndexOf("i'm ") + 4).Trim();

                    if (!string.IsNullOrEmpty(name) && name.Length < 30 && !name.Contains(" "))
                    {
                        userName = CapitalizeName(name);
                        _logger.Log($"User identified as {userName}");
                        return $"Nice to meet you, {userName}! I'm your cybersecurity assistant. Type 'Help' to see all commands.";
                    }
                }

                // Single word name
                bool isSingleWord = !lowerInput.Contains(" ") && lowerInput.Length >= 2 && lowerInput.Length <= 25;
                bool isNotCommand = !lowerInput.Contains("password") && !lowerInput.Contains("phish") &&
                                    !lowerInput.Contains("privacy") && !lowerInput.Contains("help") &&
                                    !lowerInput.Contains("how") && !lowerInput.Contains("what") &&
                                    !lowerInput.Contains("tell") && !lowerInput.Contains("scam");
                if (isSingleWord && isNotCommand)
                {
                    userName = CapitalizeName(userInput);
                    _logger.Log($"User identified as {userName}");
                    return $"Nice to meet you, {userName}! I'm your cybersecurity assistant. Type 'Help' to see all commands.";
                }
            }

            // Extract interest
            if (lowerInput.Contains("interested in"))
            {
                int index = lowerInput.IndexOf("interested in") + 13;
                userInterest = lowerInput.Substring(index).Trim();
                if (userInterest.Length > 25) userInterest = userInterest.Substring(0, 25);
            }

            // Sentiment detection
            string sentiment = "neutral";
            if (happyWords.Any(w => lowerInput.Contains(w))) sentiment = "happy";
            else if (worriedWords.Any(w => lowerInput.Contains(w))) sentiment = "worried";
            else if (frustratedWords.Any(w => lowerInput.Contains(w))) sentiment = "frustrated";
            else if (curiousWords.Any(w => lowerInput.Contains(w))) sentiment = "curious";

            // Get response
            string response = GetKeywordResponse(lowerInput);

            if (!string.IsNullOrEmpty(userName))
                response = $"{userName}, {char.ToLower(response[0]) + response.Substring(1)}";

            switch (sentiment)
            {
                case "worried": response = "It's completely understandable to feel concerned. " + response; break;
                case "frustrated": response = "I understand cybersecurity can be frustrating. Let me simplify. " + response; break;
                case "curious": response = "That's an excellent question! " + response; break;
                case "happy": response = "I'm glad to hear that! " + response; break;
            }

            return response;
        }

        public string GetWelcomeMessage()
        {
            return string.IsNullOrEmpty(userName)
                ? "Hello! Welcome to YEMA-CYBER Security Bot. What's your name?"
                : $"Welcome back, {userName}! Type 'Help' to see all commands.";
        }

        public string GetASCIIArt()
        {
            return @"
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║                     YEMA-CYBER SECURITY BOT v3.0              ║
║                                                               ║
║             Your Personal Cybersecurity Assistant             ║
║                                                               ║
║     Features: Tasks | Quiz | NLP | Activity Log | Sentiment   ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝";
        }
    }
}
