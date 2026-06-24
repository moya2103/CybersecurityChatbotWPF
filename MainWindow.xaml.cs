using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace CybersecurityChatbotWPF
{
    public partial class MainWindow : Window
    {
        // Core components
        private Chatbot _chatbot = new Chatbot();
        private DatabaseHelper _db = new DatabaseHelper();
        private bool _nameEntered = false;
        private string _userName = "";

        // Quiz variables
        private DataTable _quizQuestions;
        private int _currentQuestionIndex = 0;
        private int _quizScore = 0;
        private bool _quizActive = false;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize database
            _db.InitializeDatabase();
            _db.SeedQuizData();

            // Play voice greeting
            _chatbot.PlayVoiceGreeting();

            // Display welcome
            DisplayAsciiArt();
            AddMessageToChat("Bot: Welcome to CyberBot. Please enter your name to begin.", System.Windows.Media.Brushes.Magenta);

            // Load tasks and activity log
            LoadTasks();
            LoadActivityLog();

            // Log application start
            _db.LogActivity("Application started", "System", "CyberBot launched");
            LoadActivityLog();
        }

        // ============================================================
        // CHAT FUNCTIONS
        // ============================================================

        private void DisplayAsciiArt()
        {
            string art = @"
  ░██████             ░██                            ░████████                 ░██    
 ░██   ░██            ░██                            ░██    ░██                ░██    
░██        ░██    ░██ ░████████   ░███████  ░██░████ ░██    ░██   ░███████  ░████████ 
░██        ░██    ░██ ░██    ░██ ░██    ░██ ░███     ░████████   ░██    ░██    ░██    
░██        ░██    ░██ ░██    ░██ ░█████████ ░██      ░██     ░██ ░██    ░██    ░██    
 ░██   ░██ ░██   ░███ ░███   ░██ ░██        ░██      ░██     ░██ ░██    ░██    ░██    
  ░██████   ░█████░██ ░██░█████   ░███████  ░██      ░█████████   ░███████      ░████ 
                  ░██                                                                 
            ░███████                                                                  
                                                                                      
                        CYBERSECURITY AWARENESS BOT                 
";
            Run run = new Run(art);
            run.Foreground = System.Windows.Media.Brushes.Cyan;
            Paragraph paragraph = new Paragraph(run);
            paragraph.Margin = new Thickness(0);
            txtChatHistory.Document.Blocks.Add(paragraph);
        }

        private void AddMessageToChat(string message, System.Windows.Media.Brush color)
        {
            Run run = new Run(message + "\n");
            run.Foreground = color;
            Paragraph paragraph = new Paragraph(run);
            paragraph.Margin = new Thickness(0);
            txtChatHistory.Document.Blocks.Add(paragraph);
            txtChatHistory.ScrollToEnd();
        }

        private void SendUserMessage(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput)) return;

            AddMessageToChat($"You: {userInput}", System.Windows.Media.Brushes.White);

            // Check for exit/bye commands FIRST (before name check)
            string lowerInput = userInput.ToLower().Trim();
            if (lowerInput.Contains("exit") || lowerInput.Contains("bye"))
            {
                AddMessageToChat($"Bot: Goodbye, {_userName}! Stay safe online!", System.Windows.Media.Brushes.Red);
                _db.LogActivity("Session ended", _userName, "User exited application");
                LoadActivityLog();

                // Close after a short delay so user sees goodbye
                System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(2);
                timer.Tick += (s, e) => { timer.Stop(); Application.Current.Shutdown(); };
                timer.Start();
                txtUserInput.Clear();
                return;
            }

            if (!_nameEntered)
            {
                _userName = userInput.Trim();
                _chatbot.SetUserName(_userName);
                _nameEntered = true;
                AddMessageToChat($"Bot: Hello, {_userName}! Welcome to CyberBot.", System.Windows.Media.Brushes.Yellow);
                AddMessageToChat("Bot: Type 'help' to see commands, or use the tabs above for tasks and quiz.", System.Windows.Media.Brushes.Magenta);
                txtStatus.Text = $"Logged in as: {_userName}";

                // Log user login
                _db.LogActivity("User logged in", _userName, $"User {_userName} started session");
                LoadActivityLog();
            }
            else
            {
                string response = _chatbot.GetResponse(userInput);
                AddMessageToChat($"Bot: {response}", System.Windows.Media.Brushes.Cyan);

                // Log chat activity
                _db.LogActivity("Chat message", _userName, userInput);
                LoadActivityLog();

                // Check for activity log command
                if (lowerInput.Contains("show activity log") || lowerInput.Contains("what have you done"))
                {
                    LoadActivityLog();
                    AddMessageToChat("Bot: Activity log has been updated. Check the Activity Log tab.", System.Windows.Media.Brushes.Yellow);
                }
            }

            txtUserInput.Clear();
        }

        // ============================================================
        // TASK FUNCTIONS
        // ============================================================

        private void LoadTasks()
        {
            DataTable dt = _db.GetIncompleteTasks();
            lstTasks.Items.Clear();

            foreach (DataRow row in dt.Rows)
            {
                var task = new
                {
                    Id = row["Id"],
                    Title = row["Title"],
                    Description = row["Description"] ?? "",
                    DueDate = row["DueDate"] != DBNull.Value ? ((DateTime)row["DueDate"]).ToShortDateString() : "No date",
                    Status = (bool)row["IsCompleted"] ? "Complete" : "Pending"
                };
                lstTasks.Items.Add(task);
            }
        }

        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTaskTitle.Text.Trim();
            string description = txtTaskDescription.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                txtStatus.Text = "Please enter a task title.";
                return;
            }

            DateTime? dueDate = dpDueDate.SelectedDate;

            if (_db.AddTask(title, description, dueDate))
            {
                txtStatus.Text = "Task added successfully!";

                // Log the activity
                bool logged = _db.LogActivity("Task added", _userName, $"Title: {title}");
                System.Diagnostics.Debug.WriteLine($"Log result: {logged}");

                // Refresh activity log
                LoadActivityLog();

                txtTaskTitle.Clear();
                txtTaskDescription.Clear();
                dpDueDate.SelectedDate = null;
                LoadTasks();

                AddMessageToChat($"Bot: Task '{title}' has been added to your list.", System.Windows.Media.Brushes.Yellow);
            }
            else
            {
                txtStatus.Text = "Error adding task. Please try again.";
            }
        }

        private void btnCompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (lstTasks.SelectedItem == null)
            {
                txtStatus.Text = "Please select a task to complete.";
                return;
            }

            dynamic selected = lstTasks.SelectedItem;
            int id = selected.Id;

            if (_db.CompleteTask(id))
            {
                txtStatus.Text = "Task marked as complete!";
                _db.LogActivity("Task completed", _userName, $"Task ID: {id}");
                LoadActivityLog();
                LoadTasks();
                AddMessageToChat($"Bot: Task '{selected.Title}' has been completed. Well done!", System.Windows.Media.Brushes.Yellow);
            }
            else
            {
                txtStatus.Text = "Error completing task.";
            }
        }

        private void btnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (lstTasks.SelectedItem == null)
            {
                txtStatus.Text = "Please select a task to delete.";
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete this task?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                dynamic selected = lstTasks.SelectedItem;
                int id = selected.Id;

                if (_db.DeleteTask(id))
                {
                    txtStatus.Text = "Task deleted!";
                    _db.LogActivity("Task deleted", _userName, $"Task ID: {id}");
                    LoadActivityLog();
                    LoadTasks();
                    AddMessageToChat($"Bot: Task '{selected.Title}' has been deleted.", System.Windows.Media.Brushes.Red);
                }
                else
                {
                    txtStatus.Text = "Error deleting task.";
                }
            }
        }

        private void btnRefreshTasks_Click(object sender, RoutedEventArgs e)
        {
            LoadTasks();
            txtStatus.Text = "Tasks refreshed.";
        }

        // ============================================================
        // QUIZ FUNCTIONS
        // ============================================================

        private void LoadQuizQuestions()
        {
            _quizQuestions = _db.GetQuizQuestions();
        }

        private void DisplayQuestion()
        {
            if (_quizQuestions == null || _quizQuestions.Rows.Count == 0)
            {
                txtQuizQuestion.Text = "No questions available. Please try again later.";
                return;
            }

            if (_currentQuestionIndex >= _quizQuestions.Rows.Count)
            {
                FinishQuiz();
                return;
            }

            DataRow row = _quizQuestions.Rows[_currentQuestionIndex];
            txtQuizQuestion.Text = row["Question"].ToString();
            rbOptionA.Content = "A. " + row["OptionA"].ToString();
            rbOptionB.Content = "B. " + row["OptionB"].ToString();
            rbOptionC.Content = "C. " + row["OptionC"].ToString();
            rbOptionD.Content = "D. " + row["OptionD"].ToString();

            rbOptionA.IsChecked = false;
            rbOptionB.IsChecked = false;
            rbOptionC.IsChecked = false;
            rbOptionD.IsChecked = false;

            txtQuizProgress.Text = $"Question {_currentQuestionIndex + 1} of {_quizQuestions.Rows.Count}";
            txtQuizFeedback.Text = "";
            txtQuizScore.Text = $"Score: {_quizScore}";

            btnSubmitAnswer.IsEnabled = true;
            btnNextQuestion.IsEnabled = false;
        }

        private void FinishQuiz()
        {
            _quizActive = false;
            txtQuizQuestion.Text = $"Quiz Complete! You scored {_quizScore} out of {_quizQuestions.Rows.Count}.";
            txtQuizFeedback.Text = GetQuizFeedback(_quizScore, _quizQuestions.Rows.Count);
            btnSubmitAnswer.IsEnabled = false;
            btnNextQuestion.IsEnabled = false;
            btnStartQuiz.IsEnabled = true;

            _db.LogActivity("Quiz completed", _userName, $"Score: {_quizScore}/{_quizQuestions.Rows.Count}");
            LoadActivityLog();
        }

        private string GetQuizFeedback(int score, int total)
        {
            double percentage = (double)score / total * 100;
            if (percentage >= 80) return "Excellent! You're a cybersecurity pro! Keep up the great work.";
            if (percentage >= 60) return "Good job! You have a solid understanding. Keep learning to improve.";
            if (percentage >= 40) return "Not bad! Review the topics and try again to improve your score.";
            return "Keep learning! Cybersecurity is important. Review the tips and try again.";
        }

        private void btnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            LoadQuizQuestions();
            if (_quizQuestions == null || _quizQuestions.Rows.Count == 0)
            {
                txtQuizQuestion.Text = "No quiz questions available. Please check the database.";
                return;
            }

            _currentQuestionIndex = 0;
            _quizScore = 0;
            _quizActive = true;
            btnStartQuiz.IsEnabled = false;
            txtQuizScore.Text = "Score: 0";

            _db.LogActivity("Quiz started", _userName, $"{_quizQuestions.Rows.Count} questions");
            LoadActivityLog();
            DisplayQuestion();
        }

        private void btnSubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (!_quizActive || _quizQuestions == null) return;

            RadioButton selected = null;
            if (rbOptionA.IsChecked == true) selected = rbOptionA;
            else if (rbOptionB.IsChecked == true) selected = rbOptionB;
            else if (rbOptionC.IsChecked == true) selected = rbOptionC;
            else if (rbOptionD.IsChecked == true) selected = rbOptionD;

            if (selected == null)
            {
                txtQuizFeedback.Text = "Please select an answer.";
                return;
            }

            DataRow row = _quizQuestions.Rows[_currentQuestionIndex];
            string correctAnswer = row["CorrectAnswer"].ToString();
            string selectedLetter = selected.Content.ToString().Substring(0, 1);

            if (selectedLetter == correctAnswer)
            {
                _quizScore++;
                txtQuizFeedback.Text = "Correct! Well done!";
                txtQuizFeedback.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                txtQuizFeedback.Text = $"Incorrect. The correct answer was {correctAnswer}.";
                txtQuizFeedback.Foreground = System.Windows.Media.Brushes.Red;
            }

            txtQuizScore.Text = $"Score: {_quizScore}";
            btnSubmitAnswer.IsEnabled = false;
            btnNextQuestion.IsEnabled = true;
        }

        private void btnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            _currentQuestionIndex++;
            DisplayQuestion();
        }

        // ============================================================
        // ACTIVITY LOG FUNCTIONS
        // ============================================================

        private void LoadActivityLog()
        {
            DataTable dt = _db.GetActivityLog();
            lstActivityLog.Items.Clear();

            foreach (DataRow row in dt.Rows)
            {
                var log = new
                {
                    Timestamp = row["Timestamp"] != DBNull.Value ? ((DateTime)row["Timestamp"]).ToString("yyyy-MM-dd HH:mm") : "",
                    Action = row["Action"] ?? "",
                    Username = row["Username"] ?? "",
                    Details = row["Details"] ?? ""
                };
                lstActivityLog.Items.Add(log);
            }
        }

        private void btnRefreshLog_Click(object sender, RoutedEventArgs e)
        {
            LoadActivityLog();
            txtStatus.Text = "Activity log refreshed.";
            _db.LogActivity("Log viewed", _userName, "User viewed activity log");
            LoadActivityLog();
        }

        // ============================================================
        // EVENT HANDLERS
        // ============================================================

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendUserMessage(txtUserInput.Text);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            _db.LogActivity("Session ended", _userName, "User clicked Exit button");
            LoadActivityLog();
            Application.Current.Shutdown();
        }

        private void txtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendUserMessage(txtUserInput.Text);
            }
        }
    }
}