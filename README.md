# YEMA-CYBER Security Bot - Complete POE

A comprehensive cybersecurity awareness chatbot with task management, quiz, NLP simulation, sentiment detection, and activity logging.

---

## Project Overview

YEMA-CYBER Security Bot is a complete cybersecurity awareness application developed for the PROG6221 module. It started as a console application and evolved into a full WPF desktop application with JSON file storage. The chatbot helps users learn about cybersecurity while managing tasks, testing knowledge through quizzes, and tracking activity.

---

## Features

### Part 1: Console Chatbot
- Voice greeting on application startup
- ASCII art logo display
- Cybersecurity topic responses (passwords, phishing, privacy, safe browsing, malware, social engineering, updates)
- Personalized interaction using user's name
- Console UI enhancements (colors, typing effects)

### Part 2: WPF GUI Chatbot
- Professional Windows Presentation Foundation (WPF) graphical user interface
- Chat bubble design for user and bot messages
- Memory system (remembers user name and interests)
- Sentiment detection (worried, curious, frustrated, happy)
- Random responses (6 variations per topic)
- Conversation flow (handles "tell me more" follow-ups)
- Keyboard support (Enter key to send messages)
- Automatic scrolling to latest message

### Part 3: POE Advanced Features
- **Task Assistant** with JSON file storage
  - Add tasks with descriptions
  - View all tasks with status (pending/complete)
  - Mark tasks as complete
  - Delete tasks
  - Tasks stored in `tasks.json` file (no database needed!)

- **Cybersecurity Quiz (12 Questions)**
  - Multiple choice and true/false questions
  - Immediate feedback with explanations
  - Final score tracking with motivational feedback
  - Covers phishing, passwords, safe browsing, social engineering, 2FA, malware

- **NLP Simulation**
  - Flexible command recognition
  - Understands different phrasings (e.g., "Show tasks", "View my tasks", "What tasks do I have?")
  - Handles variations in user input

- **Activity Log**
  - Tracks all user actions with timestamps
  - Displays recent actions (last 5-10 entries)
  - Logs tasks, quiz attempts, and interactions

---

## How to Run

### Prerequisites
- Windows operating system
- .NET 6.0 SDK or later
- Visual Studio 2022

### Steps

1. Clone the repository:
git clone https://github.com/Liyema08/CyberSecurityBot.git

text

2. Open `CyberSecurityBot.sln` in Visual Studio 2022

3. Build the project: `Ctrl + Shift + B`

4. Run the project: `F5`

### No Database Setup Required!

All tasks are stored in a `tasks.json` file that is automatically created in the same folder as the application when you add your first task. No MySQL or any other database is needed!

---

## Commands

### Task Management

| Command | Description |
|---------|-------------|
| `Add task: [title]` | Create a new task |
| `Show tasks` | View all tasks |
| `Complete task [id]` | Mark task as complete |
| `Delete task [id]` | Remove a task |

### Quiz

| Command | Description |
|---------|-------------|
| `Start quiz` | Begin the cybersecurity quiz |

### Activity

| Command | Description |
|---------|-------------|
| `Activity log` | View recent actions |
| `Help` | Show all available commands |

### Cybersecurity Topics

| Command | Description |
|---------|-------------|
| `Tell me about passwords` | Password safety tips |
| `What is phishing?` | Phishing prevention tips |
| `Privacy tips` | Online privacy advice |
| `Safe browsing` | Browsing safety tips |

---

## Project Structure
CyberSecurityBot/
├── .github/
│ └── workflows/
│ └── dotnet.yml # CI/CD pipeline
├── CyberSecurityBot/
│ ├── ActivityLogger.cs # Activity log system
│ ├── App.xaml # Application configuration
│ ├── App.xaml.cs
│ ├── AssemblyInfo.cs
│ ├── ChatbotEngine.cs # Core logic (tasks, quiz, NLP)
│ ├── CyberSecurityBot.csproj # Project file
│ ├── CyberTask.cs # Task model class
│ ├── greeting.wav # Voice greeting file
│ ├── MainWindow.xaml # GUI design
│ ├── MainWindow.xaml.cs # GUI code-behind
│ ├── QuizManager.cs # Quiz logic with 12 questions
│ ├── TaskManager.cs # Task business logic
│ └── TaskStorageHelper.cs # JSON file storage
├── .gitattributes
├── .gitignore
├── README.md
└── CyberSecurityBot.sln

text

---

## Technologies Used

| Technology | Purpose |
|------------|---------|
| C# .NET 6.0 | Programming language and framework |
| WPF | Graphical User Interface |
| JSON | Task storage (tasks.json) |
| Newtonsoft.Json | JSON serialization/deserialization |
| GitHub Actions | CI/CD automation |
| NuGet | Package management |
| XAML | GUI design language |

---

## GitHub Actions CI Status

![CI Status](https://github.com/Liyema08/CyberSecurityBot/actions/workflows/dotnet.yml/badge.svg)

The project uses GitHub Actions for continuous integration. Every push is automatically built and tested.

---

## Releases

| Version | Description |
|---------|-------------|
| [v1.0](https://github.com/Liyema08/CyberSecurityBot/releases/tag/v1.0) | Part 1: Console Chatbot |
| [v2.0](https://github.com/Liyema08/CyberSecurityBot/releases/tag/v2.0) | Part 2: WPF Chatbot |
| [v3.0](https://github.com/Liyema08/CyberSecurityBot/releases/tag/v3.0) | Part 3/POE: Complete Application |

---

## Example Conversation
You: My name is Liyema
Bot: Nice to meet you, Liyema! I'm your cybersecurity assistant. Type 'Help' to see all commands.

You: Add task: Enable two-factor authentication
Bot: ✅ Task 'Enable two-factor authentication' added successfully!

You: Show tasks
Bot: 📋 YOUR TASKS
[1] Enable two-factor authentication - ⏳ PENDING

You: Start quiz
Bot: 📝 Question 1/12
What is phishing?

A type of fishing

A scam to steal personal info

A computer virus

A social media app

You: 2
Bot: ✅ Correct! Phishing is a scam where attackers pretend to be trusted organizations...

You: Activity log
Bot: 📋 ACTIVITY LOG (Recent Actions)

[20:15] Task added: 'Enable two-factor authentication'

[20:16] User viewed tasks

[20:17] Quiz: Correct answer

text

---

## Author

**Liyema**

- Student Number: ST10495510
- Module: PROG6221 - Programming 2A

---

## License

Educational Project - All Rights Reserved

---

## Links

- [GitHub Repository](https://github.com/Liyema08/CyberSecurityBot)
