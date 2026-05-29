using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityBot
{
    public class ChatbotEngine
    {
        // Memory storage
        private string userName = "";
        private string userInterest = "";
        private string lastTopic = "";
        private Random random = new Random();

        // Sentiment detection word banks
        private string[] worriedWords = { "worried", "scared", "fear", "concerned", "nervous", "anxious", "afraid", "unsafe", "terrified", "panicked" };
        private string[] frustratedWords = { "frustrated", "annoyed", "difficult", "hard", "confused", "angry", "upset", "stuck", "complicated", "tired" };
        private string[] curiousWords = { "curious", "interested", "tell me", "explain", "learn", "what is", "how does", "teach me", "want to know", "tell me about" };
        private string[] happyWords = { "happy", "great", "awesome", "good", "excellent", "fantastic", "wonderful", "thanks", "thank you", "amazing", "love" };

        // Response databases
        private Dictionary<string, List<string>> responseMap = null!;
        private Dictionary<string, List<string>> followUpMap = null!;

        public ChatbotEngine()
        {
            InitializeResponses();
            InitializeFollowUps();
        }

        private void InitializeResponses()
        {
            responseMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            // Password Safety - 6 responses
            responseMap["password"] = new List<string>
            {
                "Use strong passwords with at least 12 characters. Mix uppercase letters, lowercase letters, numbers, and special symbols like !@#$%.",
                "Never reuse passwords across different accounts. If one account gets hacked, all your accounts become vulnerable. Use a password manager like Bitwarden or LastPass!",
                "Enable Two-Factor Authentication (2FA) whenever possible. This adds an extra layer of security by requiring a code from your phone.",
                "Avoid using personal information like your birthday, pet's name, or family members' names in your passwords. Hackers can easily find this information.",
                "Change your passwords immediately if you suspect a data breach has occurred. Websites like HaveIBeenPwned can tell you if your accounts are compromised.",
                "A good technique is to use a passphrase - a sequence of random words like 'correct-horse-battery-staple' - it's long, easy to remember, and hard to crack!"
            };

            // Phishing - 6 responses
            responseMap["phish"] = new List<string>
            {
                "Watch for urgent language like 'Act now!' or 'Your account will be closed!' Scammers create fake urgency to make you act without thinking.",
                "Never click links in unsolicited emails. Instead, hover your mouse over the link to see the real URL. If it looks suspicious, don't click!",
                "Legitimate companies like banks and PayPal NEVER ask for your password via email. If an email asks for personal information, it's almost certainly a scam.",
                "Check the sender's email address carefully. Scammers use fake addresses like 'support@arnazon.com' instead of 'support@amazon.com'.",
                "If an offer seems too good to be true, it probably is. Scammers promise free money, prizes, or incredible deals to trick you.",
                "Be wary of emails with spelling mistakes and bad grammar. Legitimate companies proofread their communications."
            };
            responseMap["scam"] = responseMap["phish"];

            // Privacy - 6 responses
            responseMap["privacy"] = new List<string>
            {
                "Review your privacy settings on social media regularly. Set your profiles to private and limit what information is visible to the public.",
                "Use a VPN (Virtual Private Network) when connecting to public Wi-Fi at cafes, airports, or hotels. This encrypts your internet traffic.",
                "Be careful what personal information you share online. Your full name, address, phone number, and birthday can be used for identity theft.",
                "Check if your data has been breached using websites like HaveIBeenPwned.com. Enter your email to see if your accounts have been compromised.",
                "Use encrypted messaging apps like Signal or WhatsApp for private conversations. These apps use end-to-end encryption.",
                "Regularly check what permissions your phone apps have. Revoke permissions that aren't necessary."
            };

            // Safe Browsing - 6 responses
            responseMap["brows"] = new List<string>
            {
                "Always look for 'https://' and the padlock icon in your browser's address bar before entering any personal information.",
                "Keep your browser and extensions updated. Outdated software can have security vulnerabilities that hackers exploit.",
                "Use ad-blockers to block malicious advertisements. Some ads contain malware that can infect your computer.",
                "Clear your browser cache, cookies, and history regularly. This removes tracking data that websites use to monitor you.",
                "Consider using privacy-focused browsers like Firefox or Brave instead of Chrome or Edge.",
                "Avoid downloading files from untrusted websites. Only download software from official sources."
            };

            // Malware - 6 responses
            responseMap["malware"] = new List<string>
            {
                "Install reputable antivirus software and keep it updated. Windows Defender is built into Windows and works well.",
                "Don't download software from torrent websites or untrusted sources. Pirated software often contains malware.",
                "Be extremely careful with email attachments. Even attachments from people you know could be infected.",
                "Enable Windows Defender or use trusted antivirus like Bitdefender. Run regular full system scans.",
                "Be cautious of pop-up ads that say 'Your computer is infected!' These are scams designed to trick you.",
                "Back up your important files regularly to an external hard drive or cloud storage."
            };

            // Social Engineering - 6 responses
            responseMap["social"] = new List<string>
            {
                "Never give personal information to unsolicited callers, even if they claim to be from your bank.",
                "Verify the identity of anyone asking for sensitive information. Call back using official contact information.",
                "Be aware of manipulation tactics like creating urgency, pretending to be an authority figure, or acting familiar.",
                "If something feels wrong or suspicious, trust your instincts. Take a moment to verify before taking action.",
                "Train yourself to recognize social engineering red flags: pressure to act quickly, requests for personal information.",
                "Never share one-time passwords or verification codes with anyone. Legitimate companies never ask for these."
            };

            // Software Updates - 6 responses
            responseMap["update"] = new List<string>
            {
                "Enable automatic updates for your operating system and all software. Security patches fix known vulnerabilities.",
                "Security patches fix known vulnerabilities. Don't delay installing updates - each day you wait is a risk.",
                "Outdated software is one of the main ways hackers gain access to devices. They look for known security holes.",
                "Set reminders to check for updates if automatic updates aren't available. Make it a weekly habit.",
                "Update your router's firmware as well. Routers are often overlooked but are critical for network security.",
                "Don't ignore update notifications. While they can be annoying, updates often contain important security fixes."
            };

            // Greetings
            responseMap["hello"] = new List<string>
            {
                "Hello! Welcome to YEMA-CYBER Security Bot. How can I help you stay safe online today?",
                "Hi there! I'm your personal cybersecurity assistant. Ask me about passwords, phishing, privacy, or safe browsing!",
                "Greetings! Ready to learn about online safety? What cybersecurity topic would you like to explore today?",
                "Hey! Glad to see you're taking cybersecurity seriously. What can I help you learn about today?",
                "Welcome back! I'm here to help you stay secure online. What questions do you have about cybersecurity?"
            };
            responseMap["hi"] = responseMap["hello"];
            responseMap["hey"] = responseMap["hello"];

            // How are you
            responseMap["how are you"] = new List<string>
            {
                "I'm doing great, thanks for asking! I'm ready to help you learn about cybersecurity. What would you like to know?",
                "I'm fantastic! Always excited to talk about online safety. It's so important in today's digital world.",
                "Doing well, thank you! I'm here and ready to answer your cybersecurity questions.",
                "I'm operating at 100%! Ready to share cybersecurity tips and tricks.",
                "I'm wonderful, thanks for checking in! Let's talk about keeping your digital life secure."
            };

            // Purpose
            responseMap["purpose"] = new List<string>
            {
                "My purpose is to educate and raise awareness about cybersecurity! I help people understand online threats and how to protect themselves.",
                "I'm here to help you stay safe online. Cybersecurity can be complicated, so I give simple, practical tips you can use right away.",
                "Think of me as your personal cybersecurity assistant. I provide information about common threats and best practices.",
                "My mission is to make cybersecurity knowledge accessible to everyone. No technical jargon - just practical advice."
            };

            // Default responses (Error Handling)
            responseMap["default"] = new List<string>
            {
                "I'm not sure I understand. Try asking about: passwords, phishing, privacy, safe browsing, malware, social engineering, or software updates.",
                "Could you rephrase that? I can help with cybersecurity topics like password safety, phishing scams, or online privacy.",
                "I don't recognize that topic. I specialize in cybersecurity awareness. Try asking me about passwords, phishing, or privacy!",
                "That's outside my knowledge base. Would you like to learn about password safety, phishing prevention, or online privacy?",
                "Hmm, I didn't quite catch that. Try asking me 'Tell me about passwords' or 'What is phishing?' and I'll give you helpful information!",
                "I'm still learning about cybersecurity topics. Could you ask me something about passwords, phishing attacks, or protecting your privacy online?"
            };
        }

        private void InitializeFollowUps()
        {
            followUpMap = new Dictionary<string, List<string>>();

            followUpMap["password"] = new List<string>
            {
                "Another password tip: Use a passphrase - a sequence of random words like 'correct-horse-battery-staple'!",
                "Did you know? Password managers can generate and store strong passwords for you automatically.",
                "Also important: Don't write passwords on sticky notes attached to your monitor! Use a password manager instead.",
                "Consider using biometric authentication like fingerprint or face recognition where available.",
                "One more thing: Check if your passwords have been exposed in data breaches using HaveIBeenPwned.com."
            };

            followUpMap["phish"] = new List<string>
            {
                "Another phishing tip: Check the URL carefully. Scammers use similar spellings like 'arnazon.com' instead of 'amazon.com'.",
                "Did you know? Scammers often create fake urgency to make you act without thinking.",
                "Also important: If you're unsure about an email, don't click anything. Type the company's official website manually.",
                "Report suspicious emails to the company being impersonated AND to authorities.",
                "Remember: Legitimate companies will NEVER ask you to verify your password via email or text."
            };

            followUpMap["privacy"] = new List<string>
            {
                "Another privacy tip: Use different email addresses for different purposes - one for banking, one for social media.",
                "Did you know? Your data is valuable - companies collect and sell your browsing history and personal information.",
                "Also important: Regularly check what permissions apps have on your phone. Revoke any that seem unnecessary.",
                "Consider using privacy-focused search engines like DuckDuckGo instead of Google."
            };

            followUpMap["brows"] = new List<string>
            {
                "Another safe browsing tip: Use private or incognito mode when using public or shared computers.",
                "Did you know? Browser extensions can track your activity across websites. Only install trusted extensions.",
                "Also important: Log out of websites when you're done, especially on shared devices.",
                "Consider using a password manager's autofill feature. It only fills passwords on legitimate websites."
            };

            followUpMap["general"] = new List<string>
            {
                "Always keep your software updated to protect against known vulnerabilities.",
                "Back up your important files regularly to an external drive or cloud service.",
                "Use unique passwords for every account - don't reuse them across different sites.",
                "Be skeptical of unsolicited messages asking for personal information, even if they look official.",
                "Enable two-factor authentication on all accounts that offer it.",
                "Monitor your bank statements regularly for unauthorized transactions."
            };
        }

        public string GetResponse(string userInput)
        {
            string lowerInput = userInput.ToLower().Trim();

            // Extract user's name
            if (string.IsNullOrEmpty(userName))
            {
                // Pattern 1: "my name is X"
                if (lowerInput.Contains("my name is"))
                {
                    int index = lowerInput.IndexOf("my name is") + 10;
                    string name = lowerInput.Substring(index).Trim();
                    if (!string.IsNullOrEmpty(name) && name.Length < 30)
                    {
                        userName = CapitalizeName(name);
                        return $"Nice to meet you, {userName}! I'm your cybersecurity assistant. What would you like to learn about today?";
                    }
                }

                // Pattern 2: "i am X" or "i'm X"
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
                        return $"Nice to meet you, {userName}! I'm your cybersecurity assistant. What would you like to learn about today?";
                    }
                }

                // Pattern 3: User just types their name alone (e.g., "Liyema")
                bool isSingleWord = !lowerInput.Contains(" ") && lowerInput.Length >= 2 && lowerInput.Length <= 25;
                bool isNotCommand = !lowerInput.Contains("password") && !lowerInput.Contains("phish") && !lowerInput.Contains("privacy") &&
                                    !lowerInput.Contains("help") && !lowerInput.Contains("how") && !lowerInput.Contains("what") &&
                                    !lowerInput.Contains("tell") && !lowerInput.Contains("scam") && !lowerInput.Contains("brows") &&
                                    !lowerInput.Contains("hello") && !lowerInput.Contains("hi") && !lowerInput.Contains("hey");

                if (isSingleWord && isNotCommand)
                {
                    userName = CapitalizeName(userInput);
                    return $"Nice to meet you, {userName}! I'm your cybersecurity assistant. What would you like to learn about today?";
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
            if (happyWords.Any(w => lowerInput.Contains(w)))
                sentiment = "happy";
            else if (worriedWords.Any(w => lowerInput.Contains(w)))
                sentiment = "worried";
            else if (frustratedWords.Any(w => lowerInput.Contains(w)))
                sentiment = "frustrated";
            else if (curiousWords.Any(w => lowerInput.Contains(w)))
                sentiment = "curious";

            // Get response
            string response = GetKeywordResponse(lowerInput);

            // Personalize with name
            if (!string.IsNullOrEmpty(userName))
            {
                response = $"{userName}, {char.ToLower(response[0]) + response.Substring(1)}";
            }

            // Add sentiment adjustment
            switch (sentiment)
            {
                case "worried":
                    response = "It's completely understandable to feel concerned. " + response;
                    break;
                case "frustrated":
                    response = "I understand cybersecurity can be frustrating. Let me simplify. " + response;
                    break;
                case "curious":
                    response = "That's an excellent question! " + response;
                    break;
                case "happy":
                    response = "I'm glad to hear that! " + response;
                    break;
            }

            return response;
        }

        private string GetKeywordResponse(string input)
        {
            // Handle follow-up questions
            if (input.Contains("another tip") || input.Contains("tell me more") || input.Contains("explain more"))
            {
                if (!string.IsNullOrEmpty(lastTopic) && followUpMap.ContainsKey(lastTopic))
                {
                    var tips = followUpMap[lastTopic];
                    return tips[random.Next(tips.Count)];
                }
                var generalTips = followUpMap["general"];
                return generalTips[random.Next(generalTips.Count)];
            }

            // Check for keywords
            foreach (var key in responseMap.Keys)
            {
                if (input.Contains(key))
                {
                    lastTopic = key;
                    var responses = responseMap[key];
                    return responses[random.Next(responses.Count)];
                }
            }

            // Default
            lastTopic = "";
            var defaults = responseMap["default"];
            return defaults[random.Next(defaults.Count)];
        }

        private string CapitalizeName(string name)
        {
            name = name.Trim();
            if (string.IsNullOrEmpty(name)) return "Friend";

            if (name.Contains(" "))
            {
                name = name.Split(' ')[0];
            }

            name = new string(name.Where(c => char.IsLetter(c)).ToArray());
            if (string.IsNullOrEmpty(name)) return "Friend";

            return char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }

        public string GetWelcomeMessage()
        {
            if (string.IsNullOrEmpty(userName))
                return "Hello! Welcome to the YEMA-CYBER Security Bot. What's your name?";
            return $"Welcome back, {userName}! How can I help you stay safe online today?";
        }

        public string GetASCIIArt()
        {
            return @"
╔══════════════════════════════════════════════════════════════════════════════════════════════╗
║                                                                                              ║
║                              [ YEMA-CYBER SECURITY BOT v2.0 ]                                ║
║                                                                                              ║
║                         Your Personal Cybersecurity Assistant                                ║
║                                                                                              ║
╚══════════════════════════════════════════════════════════════════════════════════════════════╝";
        }
    }
}