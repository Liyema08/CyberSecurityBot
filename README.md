# YEMA-CYBER Security Bot - Complete POE

A comprehensive cybersecurity awareness chatbot with task management, quiz, NLP simulation, sentiment detection, and activity logging.

---

## Project Overview

YEMA-CYBER Security Bot is a complete cybersecurity awareness application developed for the PROG6221 module. It started as a console application and evolved into a full WPF desktop application with MySQL database integration. The chatbot helps users learn about cybersecurity while managing tasks, testing knowledge through quizzes, and tracking activity.

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
- **Task Assistant** with MySQL database integration
  - Add tasks with descriptions
  - View all tasks with status (pending/complete)
  - Mark tasks as complete
  - Delete tasks
  - Tasks stored persistently in MySQL database

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

## Database Setup

### Step 1: Install MySQL

1. Download MySQL Installer from https://dev.mysql.com/downloads/installer/
2. Run the installer and select **Developer Default**
3. Set root password during installation (remember it!)

### Step 2: Create Database

Open MySQL Workbench and run:

CREATE DATABASE cybersecuritybot;
USE cybersecuritybot;

CREATE TABLE tasks (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    reminder_date DATETIME,
    is_completed BOOLEAN DEFAULT FALSE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE activity_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    action VARCHAR(255) NOT NULL,
    details TEXT,
    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
);

### Step 3: Update Connection String

In ChatbotEngine.cs, find line 14 and update the password:

private string connectionString = "Server=localhost;Database=cybersecuritybot;Uid=root;Pwd=yourpassword;";

---

## How to Run

### Prerequisites
- Windows operating system
- .NET 6.0 SDK or later
- Visual Studio 2026
- MySQL Server 8.0 (for database features)

### Steps

1. Clone the repository:
   git clone https://github.com/Liyema08/CyberSecurityBot.git

2. Open CyberSecurityBot.sln in Visual Studio 2022

3. Install required NuGet packages:
   - Right-click Dependencies → Manage NuGet Packages
   - Install MySql.Data (for database)
   - Install System.Windows.Extensions (for audio)

4. Set up MySQL database (see instructions above)

5. Build the project: Ctrl + Shift + B

6. Run the project: F5

---

## Commands

### Task Management

| Command | Description |
|---------|-------------|
| Add task: [title] | Create a new task |
| Show tasks | View all tasks |
| Complete task [id] | Mark task as complete |
| Delete task [id] | Remove a task |

### Quiz

| Command | Description |
|---------|-------------|
| Start quiz | Begin the cybersecurity quiz |

### Activity

| Command | Description |
|---------|-------------|
| Activity log | View recent actions |
| Help | Show all available commands |

### Cybersecurity Topics

| Command | Description |
|---------|-------------|
| Tell me about passwords | Password safety tips |
| What is phishing? | Phishing prevention tips |
| Privacy tips | Online privacy advice |
| Safe browsing | Browsing safety tips |

---

## Project Structure

CyberSecurityBot/
├── .github/
│   └── workflows/
│       └── dotnet.yml          # CI/CD pipeline
├── CyberSecurityBot/
│   ├── MainWindow.xaml          # GUI design
│   ├── MainWindow.xaml.cs       # GUI code-behind
│   ├── ChatbotEngine.cs         # Core logic (tasks, quiz, NLP, database)
│   ├── greeting.wav             # Voice greeting file
│   ├── App.xaml                 # Application configuration
│   └── CyberSecurityBot.csproj  # Project file
├── .gitattributes
├── .gitignore
├── README.md
└── CyberSecurityBot.sln

---

## Technologies Used

| Technology | Purpose |
|------------|---------|
| C# .NET 6.0 | Programming language and framework |
| WPF | Graphical User Interface |
| MySQL | Database for task storage |
| GitHub Actions | CI/CD automation |
| NuGet | Package management |
| XAML | GUI design language |

---

## GitHub Actions CI Status

The project uses GitHub Actions for continuous integration. Every push is automatically built and tested.

---

## Releases

| Version | Description |
|---------|-------------|
| v1.0 | Part 1: Console Chatbot |
| v2.0 | Part 2: WPF Chatbot |
| v3.0 | Part 3/POE: Complete Application |

---

## Example Conversation

You: My name is Liyema
Bot: Nice to meet you, Liyema! I'm your cybersecurity assistant. Type 'Help' to see all commands.

You: Add task: Enable two-factor authentication
Bot: Task 'Enable two-factor authentication' added! Type 'Show tasks' to view all tasks.

You: Show tasks
Bot: YOUR TASKS
[1] Enable two-factor authentication - PENDING

You: Start quiz
Bot: Question 1/12
What is phishing?
1. A type of fishing
2. A scam to steal personal info
3. A computer virus
4. A social media app

You: 2
Bot: Correct! Phishing is a scam where attackers pretend to be trusted organizations...

You: Activity log
Bot: ACTIVITY LOG (Recent Actions)
[20:15:30] Task added: Enable two-factor authentication
[20:16:45] User viewed tasks
[20:17:10] Quiz: Correct answer

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

- GitHub Repository: https://github.com/Liyema08/CyberSecurityBot
