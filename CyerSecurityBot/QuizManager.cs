using System;
using System.Collections.Generic;

namespace CyberSecurityBot
{
    public class QuizManager
    {
        private List<QuizQuestion> _questions = new List<QuizQuestion>();
        private int _currentIndex = 0;
        private int _score = 0;
        private ActivityLogger _logger = new ActivityLogger();

        public QuizManager()
        {
            InitializeQuestions();
        }

        private void InitializeQuestions()
        {
            _questions.Add(new QuizQuestion(
                "What is phishing?",
                new List<string> { "A type of fishing", "A scam to steal personal info", "A computer virus", "A social media app" },
                1,
                "Phishing is a scam where attackers pretend to be trusted organizations to steal your personal information."
            ));

            _questions.Add(new QuizQuestion(
                "Which password is the safest?",
                new List<string> { "123456", "password", "MyCat123!", "Tr0ub4dor&3" },
                3,
                "A strong password has at least 12 characters with uppercase, lowercase, numbers, and symbols."
            ));

            _questions.Add(new QuizQuestion(
                "What does HTTPS stand for?",
                new List<string> { "Hyper Text Transfer Protocol Secure", "High Tech Transfer System", "Hyper Transfer System", "None of the above" },
                0,
                "HTTPS stands for Hyper Text Transfer Protocol Secure - it encrypts data between your browser and the website."
            ));

            _questions.Add(new QuizQuestion(
                "True or False: You should use the same password for all accounts.",
                new List<string> { "True", "False" },
                1,
                "Using the same password for all accounts is dangerous. If one is compromised, all are compromised!"
            ));

            _questions.Add(new QuizQuestion(
                "What is two-factor authentication (2FA)?",
                new List<string> { "A password manager", "A security measure requiring two verification methods", "A type of malware", "A VPN service" },
                1,
                "2FA adds a second layer of security, like a code from your phone, in addition to your password."
            ));

            _questions.Add(new QuizQuestion(
                "True or False: Emails from unknown senders are always safe to open.",
                new List<string> { "True", "False" },
                1,
                "Never open emails from unknown senders. They may contain phishing links or malware attachments."
            ));

            _questions.Add(new QuizQuestion(
                "What is social engineering?",
                new List<string> { "Building social networks", "Manipulating people to reveal information", "Engineering software", "A type of hacking tool" },
                1,
                "Social engineering manipulates people into revealing confidential information through psychological tricks."
            ));

            _questions.Add(new QuizQuestion(
                "Which URL is safer to use?",
                new List<string> { "http://example.com", "https://example.com", "ftp://example.com", "http://example.org" },
                1,
                "https:// is secure because it encrypts data between your browser and the website."
            ));

            _questions.Add(new QuizQuestion(
                "True or False: Public Wi-Fi is completely safe for online banking.",
                new List<string> { "True", "False" },
                1,
                "Public Wi-Fi is unsafe for banking. Use a VPN or your mobile data instead."
            ));

            _questions.Add(new QuizQuestion(
                "What should you do if you receive a suspicious email?",
                new List<string> { "Click the link", "Reply with your info", "Delete it and report it", "Forward it to friends" },
                2,
                "Delete suspicious emails immediately and report them to the company being impersonated."
            ));

            _questions.Add(new QuizQuestion(
                "What is malware?",
                new List<string> { "Hardware problem", "Malicious software designed to harm your device", "A type of firewall", "A password generator" },
                1,
                "Malware is malicious software designed to damage, disrupt, or gain unauthorized access to your device."
            ));

            _questions.Add(new QuizQuestion(
                "True or False: Updates are important for security.",
                new List<string> { "True", "False" },
                0,
                "Software updates fix security vulnerabilities. Always install updates promptly!"
            ));
        }

        public QuizQuestion GetCurrentQuestion()
        {
            if (_currentIndex < _questions.Count)
                return _questions[_currentIndex];
            return null;
        }

        public string SubmitAnswer(int answerIndex)
        {
            if (_currentIndex >= _questions.Count)
                return "Quiz is already finished!";

            var q = _questions[_currentIndex];
            bool correct = answerIndex == q.CorrectIndex;
            if (correct) _score++;

            _currentIndex++;
            _logger.Log($"Quiz: {(correct ? "Correct" : "Incorrect")} answer to Q{_currentIndex}");

            if (correct)
                return $"✅ Correct! {q.Explanation}";
            else
                return $"❌ Wrong. {q.Explanation}";
        }

        public bool IsFinished()
        {
            return _currentIndex >= _questions.Count;
        }

        public string GetFinalScore()
        {
            _logger.Log($"Quiz completed - score: {_score}/{_questions.Count}");
            string result = $"🏆 QUIZ COMPLETE!\nScore: {_score}/{_questions.Count}\n";

            if (_score >= 10)
                result += "🌟 Excellent! You're a cybersecurity expert!";
            else if (_score >= 7)
                result += "👍 Good job! Keep learning!";
            else
                result += "📚 Keep learning! Cybersecurity is important for everyone.";

            return result;
        }

        public string GetCurrentQuestionText()
        {
            var q = GetCurrentQuestion();
            if (q == null) return "No questions left!";

            string result = $"📝 Question {_currentIndex + 1}/{_questions.Count}\n";
            result += $"{q.Question}\n\n";

            for (int i = 0; i < q.Options.Count; i++)
            {
                result += $"{i + 1}. {q.Options[i]}\n";
            }

            result += "\nType the number of your answer:";
            return result;
        }

        public void Reset()
        {
            _currentIndex = 0;
            _score = 0;
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
