# YEMA-CYBER Security Bot

A professional WPF cybersecurity awareness chatbot that educates users about online safety through interactive conversation.

## Project Overview

This chatbot helps users learn about cybersecurity topics including password safety, phishing prevention, online privacy, safe browsing, malware protection, social engineering awareness, and software updates. The application features a modern graphical user interface with chat bubbles, voice greeting, and ASCII art logo.

## Features

### Core Functionality
- **Voice Greeting** - Plays a welcome message when the application starts
- **ASCII Art Logo** - Custom visual branding displayed in the header
- **7 Cybersecurity Topics** - Covers passwords, phishing, privacy, safe browsing, malware, social engineering, and software updates
- **Random Responses** - 6 different responses per topic for natural conversation
- **Name Memory** - Remembers the user's name throughout the conversation
- **Interest Memory** - Recalls topics the user wants to learn about
- **Sentiment Detection** - Detects worried, curious, frustrated, and happy emotions
- **Conversation Flow** - Handles follow-up questions like "tell me more"
- **Error Handling** - Gracefully manages unrecognized inputs

### Technical Features
- WPF GUI with custom chat bubble styles
- Keyboard shortcuts (Enter key to send)
- Automatic scrolling to latest message
- Professional color scheme and layout

## How to Run

### Prerequisites
- Windows operating system
- .NET 6.0 SDK or later
- Visual Studio 2022

### Steps
1. Clone the repository
2. Open `CyberSecurityBot.sln` in Visual Studio 2022
3. Build the solution (Ctrl + Shift + B)
4. Run the application (F5)

## Example Conversations

| You Type | Bot Response |
|----------|--------------|
| `My name is John` | "Nice to meet you, John! I'm your cybersecurity assistant..." |
| `Tell me about passwords` | Random password safety tip (uses your name) |
| `What is phishing?` | Random phishing prevention tip |
| `I'm interested in privacy` | Remembers your interest for future responses |
| `tell me more` | Provides additional tips on the last topic |
| `I'm worried about scams` | Empathetic response with helpful information |

## Project Structure

- MainWindow.xaml - GUI design
- MainWindow.xaml.cs - GUI code-behind
- ChatbotEngine.cs - Chatbot logic
- greeting.wav - Voice greeting file

## Technologies

- C# .NET 6.0
- WPF (Windows Presentation Foundation)
- GitHub Actions CI

## Author

Liyema

## GitHub Repository

https://github.com/Liyema08/CyberSecurityBot

