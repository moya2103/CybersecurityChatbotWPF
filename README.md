# Cybersecurity Awareness Chatbot

## Project Overview
A cybersecurity awareness chatbot developed for Programming 2A POE. The chatbot educates users about cybersecurity topics including password safety, phishing awareness, and safe browsing practices.

The project consists of three parts:
- **Part 1:** Console-based application
- **Part 2:** WPF GUI application with keyword recognition, memory, and sentiment detection
- **Part 3:** WPF application with task assistant, quiz game, NLP simulation, and activity log

## Features

### Part 1 - Console Application
- Voice greeting on application startup
- Custom ASCII art logo with header
- Personalized conversation using user's name
- Input validation and error handling
- Cybersecurity tips for password safety, phishing, and safe browsing
- Interactive help menu
- Typing effect for natural conversation flow
- Color-coded user interface

### Part 2 - WPF GUI Application
- Graphical user interface with chat display
- Keyword recognition (password, phishing, privacy)
- Random responses for natural conversation
- Conversation flow with follow-up questions
- Memory and recall of user interests
- Sentiment detection (worried, curious, frustrated)

### Part 3 - Advanced WPF Application
- Task assistant with database integration
  - Add, complete, and delete tasks
  - Optional due dates
  - SQL Server LocalDB storage
- Cybersecurity quiz game
  - 12 multiple choice questions
  - Instant feedback and score tracking
- Natural Language Processing simulation
  - Understands different phrasing for commands
- Activity log
  - Tracks all user actions
  - Shows last 10 actions

## Technologies Used

- **C# .NET 8.0** - Programming language
- **WPF (Windows Presentation Foundation)** - GUI framework for Parts 2 and 3
- **SQL Server LocalDB** - Database for task storage (Part 3)
- **GitHub Actions** - Continuous Integration
- **System.Media** - Audio playback for voice greeting

## How to Run

### Prerequisites
- Windows 10 or 11
- Visual Studio 2022
- .NET 8.0 SDK
- SQL Server LocalDB (installed with Visual Studio) - Required for Part 3

### Part 1 - Console Application
1. Clone this repository
2. Open the solution in Visual Studio
3. Ensure `greeting.wav` is in the project output directory
4. Press F5 to run
5. Enter your name when prompted
6. Type commands to interact with CyberBot

### Part 2 & 3 - WPF Application
1. Open the WPF solution (`CybersecurityChatbotWPF.sln`)
2. Ensure `greeting.wav` is in the project output directory
3. Press F5 to run
4. Enter your name when prompted
5. Use the tabs to access different features

## Available Commands

| Command | Description |
|---------|-------------|
| `password` | Get password safety tips |
| `phishing` | Learn to spot phishing scams |
| `privacy` | Get privacy protection tips |
| `safe browsing` | Learn to browse the web safely |
| `tip` | Get random cybersecurity advice |
| `joke` | Hear a cybersecurity joke |
| `fun fact` | Learn interesting cybersecurity facts |
| `I am interested in...` | Bot remembers your interest |
| `tell me more` | Follow up on a topic |
| `add task` | Add a new task (Part 3) |
| `show tasks` | View your tasks (Part 3) |
| `start quiz` | Start the cybersecurity quiz (Part 3) |
| `show activity log` | View activity log (Part 3) |
| `help` | Show all available commands |
| `exit` or `bye` | Close the application |

## Database Setup (Part 3)

The application uses SQL Server LocalDB which is installed with Visual Studio.

### Automatic Database Creation
The database is created automatically when the application runs for the first time. No manual setup is required.

**Database Tables:**
- Tasks - Stores user tasks with title, description, due date, and completion status
- ActivityLog - Tracks user actions with timestamps
- QuizQuestions - Stores 12 cybersecurity quiz questions

## Project Structure
CybersecurityChatbot/ # Part 1 - Console Application
├── Program.cs # Entry point
├── Chatbot.cs # Chatbot logic
└── greeting.wav # Voice greeting audio

CybersecurityChatbotWPF/ # Parts 2 & 3 - WPF Application
├── MainWindow.xaml # WPF GUI design
├── MainWindow.xaml.cs # GUI code-behind
├── Chatbot.cs # Chatbot logic
├── DatabaseHelper.cs # Database operations
├── greeting.wav # Voice greeting audio
└── .github/
└── workflows/
└── dotnet.yml # GitHub Actions CI

text

## GitHub Actions CI Status

### Part 1
[![.NET Build](https://github.com/moya2103/CybersecurityChatbot/actions/workflows/dotnet.yml/badge.svg)](https://github.com/moya2103/CybersecurityChatbot/actions/workflows/dotnet.yml)

### Part 2 & 3
[![.NET Build WPF](https://github.com/moya2103/CybersecurityChatbotWPF/actions/workflows/dotnet.yml/badge.svg)](https://github.com/moya2103/CybersecurityChatbotWPF/actions/workflows/dotnet.yml)

## Releases

### Part 1
- v1.0 - Initial release with console application

### Parts 2 & 3
- v1.0 - WPF GUI with keyword recognition and memory features
- v2.0 - Added task assistant and quiz game
- v3.0 - Complete with NLP simulation and activity log

## Author
Tsholofelo Molomo
ST10481285

## Date
June 2026