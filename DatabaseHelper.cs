using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows;

namespace CybersecurityChatbotWPF
{
    public class DatabaseHelper
    {
        // Connection string for master database (to create our database)
        private string masterConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;";

        // Connection string for our database
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CyberBotTasks;Integrated Security=True;";

        public void InitializeDatabase()
        {
            try
            {
                // Step 1: Connect to master database and create our database if it doesn't exist
                using (SqlConnection masterConn = new SqlConnection(masterConnectionString))
                {
                    masterConn.Open();

                    string createDbQuery = @"
                        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CyberBotTasks')
                        BEGIN
                            CREATE DATABASE CyberBotTasks
                        END";

                    SqlCommand cmdCreateDb = new SqlCommand(createDbQuery, masterConn);
                    cmdCreateDb.ExecuteNonQuery();
                    masterConn.Close();
                }

                // Step 2: Now connect to our database and create tables
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Create Tasks table
                    string createTasks = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tasks' AND xtype='U')
                        CREATE TABLE Tasks (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Title NVARCHAR(200) NOT NULL,
                            Description NVARCHAR(500),
                            DueDate DATETIME,
                            IsCompleted BIT DEFAULT 0,
                            CreatedAt DATETIME DEFAULT GETDATE()
                        )";
                    SqlCommand cmdTasks = new SqlCommand(createTasks, conn);
                    cmdTasks.ExecuteNonQuery();

                    // Create ActivityLog table
                    string createLog = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ActivityLog' AND xtype='U')
                        CREATE TABLE ActivityLog (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Action NVARCHAR(200) NOT NULL,
                            Timestamp DATETIME DEFAULT GETDATE(),
                            Username NVARCHAR(100),
                            Details NVARCHAR(500)
                        )";
                    SqlCommand cmdLog = new SqlCommand(createLog, conn);
                    cmdLog.ExecuteNonQuery();

                    // Create QuizQuestions table
                    string createQuiz = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='QuizQuestions' AND xtype='U')
                        CREATE TABLE QuizQuestions (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            Question NVARCHAR(500) NOT NULL,
                            OptionA NVARCHAR(200) NOT NULL,
                            OptionB NVARCHAR(200) NOT NULL,
                            OptionC NVARCHAR(200) NOT NULL,
                            OptionD NVARCHAR(200) NOT NULL,
                            CorrectAnswer CHAR(1) NOT NULL,
                            Category NVARCHAR(50)
                        )";
                    SqlCommand cmdQuiz = new SqlCommand(createQuiz, conn);
                    cmdQuiz.ExecuteNonQuery();

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database init error: {ex.Message}");
                MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool AddTask(string title, string description, DateTime? dueDate)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Tasks (Title, Description, DueDate) VALUES (@Title, @Description, @DueDate)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", (object)description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DueDate", (object)dueDate ?? DBNull.Value);

                    int result = cmd.ExecuteNonQuery();
                    conn.Close();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddTask error: {ex.Message}");
                return false;
            }
        }

        public DataTable GetIncompleteTasks()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Title, Description, DueDate, IsCompleted, CreatedAt FROM Tasks WHERE IsCompleted = 0 ORDER BY CreatedAt DESC";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetIncompleteTasks error: {ex.Message}");
            }
            return dt;
        }

        public bool CompleteTask(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Tasks SET IsCompleted = 1 WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    int result = cmd.ExecuteNonQuery();
                    conn.Close();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CompleteTask error: {ex.Message}");
                return false;
            }
        }

        public bool DeleteTask(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Tasks WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    int result = cmd.ExecuteNonQuery();
                    conn.Close();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteTask error: {ex.Message}");
                return false;
            }
        }

        public bool LogActivity(string action, string userName, string details)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO ActivityLog (Action, Username, Details) VALUES (@Action, @Username, @Details)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@Username", string.IsNullOrEmpty(userName) ? "System" : userName);
                    cmd.Parameters.AddWithValue("@Details", string.IsNullOrEmpty(details) ? "" : details);

                    int result = cmd.ExecuteNonQuery();
                    conn.Close();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LogActivity error: {ex.Message}");
                return false;
            }
        }

        public DataTable GetActivityLog()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT TOP 10 Id, Action, Timestamp, Username, Details FROM ActivityLog ORDER BY Timestamp DESC";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetActivityLog error: {ex.Message}");
            }
            return dt;
        }

        public DataTable GetQuizQuestions()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM QuizQuestions";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetQuizQuestions error: {ex.Message}");
            }
            return dt;
        }

        public void SeedQuizData()
        {
            try
            {
                DataTable dt = GetQuizQuestions();
                if (dt.Rows.Count > 0) return;

                string[] questions = {
                    "What is the most common password used worldwide?|123456|password|admin|letmein|B|Passwords",
                    "What does HTTPS stand for?|Hyper Text Transfer Protocol Secure|High Tech Transfer Protocol|Hyper Transfer Text Protocol|None of the above|A|Browsing",
                    "What is phishing?|A type of fishing|A scam to steal personal information|A computer virus|A type of hacker|B|Phishing",
                    "How often should you change your passwords?|Never|Every 3-6 months|Every year|Only when hacked|B|Passwords",
                    "What is two-factor authentication?|Two passwords|A second verification step|Two users|A type of encryption|B|Passwords",
                    "What should you do if you receive a suspicious email?|Reply to it|Click the link|Report it as phishing|Forward it to friends|C|Phishing",
                    "What is a VPN used for?|Streaming videos|Encrypting internet traffic|Downloading files|Gaming|B|Privacy",
                    "What is social engineering?|Building websites|A type of hacking using human interaction|Creating software|Network setup|B|Phishing",
                    "What is a strong password?|Your name|A mix of letters, numbers, symbols|123456|Your birthday|B|Passwords",
                    "What does the padlock icon mean in a browser?|The site is safe|The site is dangerous|The site is slow|The site is old|A|Browsing",
                    "What is malware?|A type of software|Software designed to harm|A type of game|A type of app|B|Malware",
                    "What is ransomware?|Software that steals passwords|Software that locks your files|A type of game|A type of browser|B|Malware"
                };

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    foreach (string q in questions)
                    {
                        string[] parts = q.Split('|');
                        string query = "INSERT INTO QuizQuestions (Question, OptionA, OptionB, OptionC, OptionD, CorrectAnswer, Category) " +
                                       "VALUES (@Question, @OptionA, @OptionB, @OptionC, @OptionD, @CorrectAnswer, @Category)";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Question", parts[0]);
                        cmd.Parameters.AddWithValue("@OptionA", parts[1]);
                        cmd.Parameters.AddWithValue("@OptionB", parts[2]);
                        cmd.Parameters.AddWithValue("@OptionC", parts[3]);
                        cmd.Parameters.AddWithValue("@OptionD", parts[4]);
                        cmd.Parameters.AddWithValue("@CorrectAnswer", parts[5]);
                        cmd.Parameters.AddWithValue("@Category", parts[6]);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SeedQuizData error: {ex.Message}");
            }
        }
    }
}