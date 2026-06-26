using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace CyberSecurityBot
{
    public class ChatbotEngine
    {
        // ===============================================
        // DATABASE CONNECTION - CHANGE PASSWORD HERE!
        // ===============================================
        private string connectionString = "Server=localhost;Database=cybersecuritybot;Uid=root;Pwd=passwod123@;";

        // ===============================================
        // MEMORY & STATE
        // ===============================================
        private string userName = "";
        private string userInterest = "";
        private string lastTopic = "";
        private Random random = new Random();
        private List<string> activityLog = new List<string>();

        // ===============================================
        // QUIZ - 12 Questions
        // ===============================================
        private List<QuizQuestion> quizQuestions = new List<QuizQuestion>();
        private int currentQuizIndex = 0;
        private int quizScore = 0;
        private bool isQuizActive = false;

        // ===============================================
        // SENTIMENT WORD BANKS
        // ===============================================
        private string[] worriedWords = { "worried", "scared", "fear", "concerned", "nervous", "anxious", "afraid", "unsafe", "terrified", "panicked" };
        private string[] frustratedWords = { "frustrated", "annoyed", "difficult", "hard", "confused", "angry", "upset", "stuck", "complicated", "tired" };
        private string[] curiousWords = { "curious", "interested", "tell me", "explain", "learn", "what is", "how does", "teach me", "want to know", "tell me about" };
        private string[] happyWords = { "happy", "great", "awesome", "good", "excellent", "fantastic", "wonderful", "thanks", "thank you", "amazing", "love" };

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
            InitializeQuizQuestions();
            InitializeDatabase();
            LogActivity("Bot initialized");
        }

        // ===============================================
        // DATABASE CONNECTION TEST
        // ===============================================
        private void InitializeDatabase()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    LogActivity("Database connected successfully");
                }
            }
            catch (Exception ex)
            {
                LogActivity($"Database connection failed: {ex.Message}");
            }
        }

        // ===============================================
        // TASK MANAGEMENT (CRUD Operations)
        // ===============================================
        public string AddTask(string title, string description, string reminderDate = null)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO tasks (title, description, reminder_date) 
                                    VALUES (@title, @description, @reminderDate)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@description", description ?? "");

                        if (DateTime.TryParse(reminderDate, out DateTime reminder))
                        {
                            cmd.Parameters.AddWithValue("@reminderDate", reminder);
                            cmd.ExecuteNonQuery();
                            LogActivity($"Task added: {title} with reminder");
                            return $"✅ Task '{title}' added with reminder on {reminderDate}!";
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@reminderDate", DBNull.Value);
                            cmd.ExecuteNonQuery();
                            LogActivity($"Task added: {title}");
                            return $"✅ Task '{title}' added! Type 'Show tasks' to view all tasks.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error adding task: {ex.Message}";
            }
        }

        public string GetTasks()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT id, title, description, reminder_date, is_completed FROM tasks ORDER BY created_at DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        string result = "📋 YOUR TASKS\n";
                        result += "════════════════════════════════════════\n";

                        bool hasTasks = false;
                        while (reader.Read())
                        {
                            hasTasks = true;
                            int id = reader.GetInt32("id");
                            string title = reader.GetString("title");
                            string desc = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString("description");
                            bool completed = reader.GetBoolean("is_completed");
                            string status = completed ? "✅ COMPLETE" : "⏳ PENDING";

                            result += $"[{id}] {title} - {status}\n";
                            if (!string.IsNullOrEmpty(desc))
                                result += $"   📝 {desc}\n";
                        }

                        if (!hasTasks)
                            return "You have no tasks. Add one with: 'Add task: [title]'";

                        result += "\nCommands: Complete task [id] | Delete task [id]";
                        LogActivity("User viewed tasks");
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error getting tasks: {ex.Message}";
            }
        }

        public string CompleteTask(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE tasks SET is_completed = TRUE WHERE id = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            LogActivity($"Task {id} completed");
                            return $"✅ Task {id} marked as complete! Great job! 🎉";
                        }
                        return $"❌ Task {id} not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error completing task: {ex.Message}";
            }
        }

        public string DeleteTask(int id)
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM tasks WHERE id = @id";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            LogActivity($"Task {id} deleted");
                            return $"✅ Task {id} has been deleted.";
                        }
                        return $"❌ Task {id} not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error deleting task: {ex.Message}";
            }
        }

        // ===============================================
        // QUIZ SYSTEM
        // ===============================================
        private void InitializeQuizQuestions()
        {
            quizQuestions = new List<QuizQuestion>
            {
                new QuizQuestion(
                    "What is phishing?",
                    new List<string> { "A type of fishing", "A scam to steal personal info", "A computer virus", "A social media app" },
                    1,
                    "Phishing is a scam where attackers pretend to be trusted organizations to steal your personal information."
                ),
                new QuizQuestion(
                    "Which password is the safest?",
                    new List<string> { "123456", "password", "MyCat123!", "Tr0ub4dor&3" },
                    3,
                    "A strong password has at least 12 characters with uppercase, lowercase, numbers, and symbols."
                ),
                new QuizQuestion(
                    "What does HTTPS stand for?",
                    new List<string> { "Hyper Text Transfer Protocol Secure", "High Tech Transfer System", "Hyper Transfer System", "None of the above" },
                    0,
                    "HTTPS stands for Hyper Text Transfer Protocol Secure - it encrypts data between your browser and the website."
                ),
                new QuizQuestion(
                    "True or False: You should use the same password for all accounts.",
                    new List<string> { "True", "False" },
                    1,
                    "Using the same password for all accounts is dangerous. If one is compromised, all are compromised!"
                ),
                new QuizQuestion(
                    "What is two-factor authentication (2FA)?",
                    new List<string> { "A password manager", "A security measure requiring two verification methods", "A type of malware", "A VPN service" },
                    1,
                    "2FA adds a second layer of security, like a code from your phone, in addition to your password."
                ),
                new QuizQuestion(
                    "True or False: Emails from unknown senders are always safe to open.",
                    new List<string> { "True", "False" },
                    1,
                    "Never open emails from unknown senders. They may contain phishing links or malware attachments."
                ),
                new QuizQuestion(
                    "What is social engineering?",
                    new List<string> { "Building social networks", "Manipulating people to reveal information", "Engineering software", "A type of hacking tool" },
                    1,
                    "Social engineering manipulates people into revealing confidential information through psychological tricks."
                ),
                new QuizQuestion(
                    "Which URL is safer to use?",
                    new List<string> { "http://example.com", "https://example.com", "ftp://example.com", "http://example.org" },
                    1,
                    "https:// is secure because it encrypts data between your browser and the website."
                ),
                new QuizQuestion(
                    "True or False: Public Wi-Fi is completely safe for online banking.",
                    new List<string> { "True", "False" },
                    1,
                    "Public Wi-Fi is unsafe for banking. Use a VPN or your mobile data instead."
                ),
                new QuizQuestion(
                    "What should you do if you receive a suspicious email?",
                    new List<string> { "Click the link", "Reply with your info", "Delete it and report it", "Forward it to friends" },
                    2,
                    "Delete suspicious emails immediately and report them to the company being impersonated."
                ),
                new QuizQuestion(
                    "What is malware?",
                    new List<string> { "Hardware problem", "Malicious software designed to harm your device", "A type of firewall", "A password generator" },
                    1,
                    "Malware is malicious software designed to damage, disrupt, or gain unauthorized access to your device."
                ),
                new QuizQuestion(
                    "True or False: Updates are important for security.",
                    new List<string> { "True", "False" },
                    0,
                    "Software updates fix security vulnerabilities. Always install updates promptly!"
                )
            };
        }

        public string StartQuiz()
        {
            currentQuizIndex = 0;
            quizScore = 0;
            isQuizActive = true;
            LogActivity("User started the cybersecurity quiz");
            return GetNextQuizQuestion();
        }

        public string GetNextQuizQuestion()
        {
            if (currentQuizIndex >= quizQuestions.Count)
            {
                isQuizActive = false;
                string result = $"🏆 QUIZ COMPLETE!\nScore: {quizScore}/{quizQuestions.Count}\n";
                if (quizScore >= 10) result += "🌟 Excellent! You're a cybersecurity expert!";
                else if (quizScore >= 7) result += "👍 Good job! Keep learning!";
                else result += "📚 Keep learning! Cybersecurity is important for everyone.";
                LogActivity($"Quiz completed with score {quizScore}/{quizQuestions.Count}");
                return result;
            }

            var q = quizQuestions[currentQuizIndex];
            string questionText = $"📝 Question {currentQuizIndex + 1}/{quizQuestions.Count}\n";
            questionText += $"{q.Question}\n\n";
            for (int i = 0; i < q.Options.Count; i++)
                questionText += $"{i + 1}. {q.Options[i]}\n";
            questionText += "\nType the number of your answer:";
            return questionText;
        }

        public string AnswerQuiz(string input)
        {
            if (!isQuizActive) return "The quiz is not active. Type 'Start quiz' to begin!";
            if (int.TryParse(input, out int answerIndex) && answerIndex >= 1 && answerIndex <= 4)
            {
                var q = quizQuestions[currentQuizIndex];
                bool isCorrect = (answerIndex - 1) == q.CorrectIndex;
                if (isCorrect)
                {
                    quizScore++;
                    currentQuizIndex++;
                    LogActivity($"Quiz: Correct answer");
                    return $"✅ Correct! {q.Explanation}\n\n" + GetNextQuizQuestion();
                }
                else
                {
                    currentQuizIndex++;
                    LogActivity($"Quiz: Incorrect answer");
                    return $"❌ Wrong. {q.Explanation}\n\n" + GetNextQuizQuestion();
                }
            }
            return "Please enter a valid number (1-4) for your answer.";
        }

        // ===============================================
        // ACTIVITY LOG
        // ===============================================
        private void LogActivity(string action)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string entry = $"[{timestamp}] {action}";
            activityLog.Add(entry);
            if (activityLog.Count > 50) activityLog.RemoveAt(0);

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO activity_log (action, details) VALUES (@action, @details)";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@action", action);
                        cmd.Parameters.AddWithValue("@details", timestamp);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception) { }
        }

        public string GetActivityLog()
        {
            LogActivity("User viewed activity log");
            if (activityLog.Count == 0) return "No activity logged yet.";
            string result = "📋 ACTIVITY LOG (Recent Actions)\n";
            result += "════════════════════════════════════════\n";
            int start = Math.Max(0, activityLog.Count - 10);
            for (int i = activityLog.Count - 1; i >= start; i--)
                result += $"{activityLog[i]}\n";
            return result;
        }

        // ===============================================
        // NLP SIMULATION
        // ===============================================
        private string ProcessNLPCommand(string input, string lowerInput)
        {
            // ADD TASK
            if (lowerInput.Contains("add task") || lowerInput.Contains("new task"))
            {
                string title = input;
                if (lowerInput.Contains("add task"))
                    title = input.Substring(input.IndexOf("task") + 4).Trim();
                else if (lowerInput.Contains("new task"))
                    title = input.Substring(input.IndexOf("task") + 4).Trim();

                if (string.IsNullOrEmpty(title))
                    return "Please provide a task description. Example: 'Add task: Review privacy settings'";
                return AddTask(title, "", "");
            }

            // SHOW TASKS
            if (lowerInput.Contains("show tasks") || lowerInput.Contains("view tasks") || lowerInput.Contains("my tasks"))
                return GetTasks();

            // COMPLETE TASK
            if (lowerInput.Contains("complete task"))
            {
                var parts = input.Split(' ');
                if (int.TryParse(parts[parts.Length - 1], out int id))
                    return CompleteTask(id);
                return "Please specify the task ID. Example: 'Complete task 1'";
            }

            // DELETE TASK
            if (lowerInput.Contains("delete task"))
            {
                var parts = input.Split(' ');
                if (int.TryParse(parts[parts.Length - 1], out int id))
                    return DeleteTask(id);
                return "Please specify the task ID. Example: 'Delete task 1'";
            }

            // START QUIZ
            if (lowerInput.Contains("start quiz") || lowerInput.Contains("play quiz") || lowerInput.Contains("take quiz"))
                return StartQuiz();

            // QUIZ ANSWER
            if (isQuizActive && int.TryParse(input, out int answer))
                return AnswerQuiz(input);

            // ACTIVITY LOG
            if (lowerInput.Contains("activity log") || lowerInput.Contains("what have you done"))
                return GetActivityLog();

            // HELP
            if (lowerInput.Contains("help"))
                return @"📖 AVAILABLE COMMANDS:
1. Add task: [title] - Create a new task
2. Show tasks - View all tasks
3. Complete task [id] - Mark task as done
4. Delete task [id] - Remove a task
5. Start quiz - Begin cybersecurity quiz
6. Activity log - View recent actions
7. Tell me about passwords - Cybersecurity tips";

            return null;
        }

        // ===============================================
        // MAIN GET RESPONSE
        // ===============================================
        public string GetResponse(string userInput)
        {
            string lowerInput = userInput.ToLower().Trim();

            // Check NLP commands first
            string nlpResult = ProcessNLPCommand(userInput, lowerInput);
            if (nlpResult != null) return nlpResult;

            // Extract name
            if (string.IsNullOrEmpty(userName))
            {
                if (lowerInput.Contains("my name is"))
                {
                    int index = lowerInput.IndexOf("my name is") + 10;
                    string name = lowerInput.Substring(index).Trim();
                    if (!string.IsNullOrEmpty(name) && name.Length < 30)
                    {
                        userName = CapitalizeName(name);
                        LogActivity($"User identified as {userName}");
                        return $"Nice to meet you, {userName}! I'm your cybersecurity assistant. Type 'Help' to see all commands.";
                    }
                }

                if (lowerInput.Contains("i am ") || lowerInput.Contains("i'm "))
                {
                    string name = "";
                    if (lowerInput.Contains("i am "))
                    {
                        int index = lowerInput.IndexOf("i am ") + 5;
                        name = lowerInput.Substring(index).Trim();
                    }
                    else if (lowerInput.Contains("i'm "))
                    {
                        int index = lowerInput.IndexOf("i'm ") + 4;
                        name = lowerInput.Substring(index).Trim();
                    }
                    if (!string.IsNullOrEmpty(name) && name.Length < 30 && !name.Contains(" "))
                    {
                        userName = CapitalizeName(name);
                        LogActivity($"User identified as {userName}");
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
                    LogActivity($"User identified as {userName}");
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

            responseMap["malware"] = new List<string>
            {
                "Install reputable antivirus software and keep it updated regularly.",
                "Don't download software from untrusted websites or torrents.",
                "Be careful with email attachments - scan them before opening.",
                "Enable Windows Defender or use trusted antivirus like Bitdefender.",
                "Regularly scan your computer for malware and remove suspicious files."
            };

            responseMap["social"] = new List<string>
            {
                "Never give personal information to unsolicited callers or emails.",
                "Verify the identity of anyone asking for sensitive data.",
                "Be aware of manipulation tactics like urgency, authority, or fake familiarity.",
                "If something feels wrong, trust your instincts and verify before acting.",
                "Train yourself to recognize social engineering red flags."
            };

            responseMap["update"] = new List<string>
            {
                "Enable automatic updates for your operating system and all software.",
                "Security patches fix known vulnerabilities - don't delay installing them.",
                "Outdated software is one of the main ways hackers gain access to devices.",
                "Set reminders to check for updates if automatic updates aren't available.",
                "Update your router firmware as well - it's often overlooked but critical."
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

    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int CorrectIndex { get; set; }
        public string Explanation { get; set; }

        public QuizQuestion(string question, List<string> options, int correctIndex, string explanation)
        {
            Question = question;
            Options = options;
            CorrectIndex = correctIndex;
            Explanation = explanation;
        }
    }
}