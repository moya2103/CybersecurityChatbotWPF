using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;

namespace CybersecurityChatbotWPF
{
    public partial class MainWindow : Window
    {
        // Create an instance of the chatbot
        private Chatbot _chatbot = new Chatbot();

        // Track if the user has entered their name yet
        private bool _nameEntered = false;

        public MainWindow()
        {
            InitializeComponent();

            // Play voice greeting when application starts
            _chatbot.PlayVoiceGreeting();

            // Display welcome message and ASCII art
            DisplayAsciiArt();
            AddMessageToChat("Bot: Welcome to CyberBot. Please enter your name to begin.", System.Windows.Media.Brushes.Magenta);
        }

        // Displays the ASCII art logo in the chat window
        private void DisplayAsciiArt()
        {
            string art = @"  ░██████             ░██                            ░████████                 ░██    
 ░██   ░██            ░██                            ░██    ░██                ░██    
░██        ░██    ░██ ░████████   ░███████  ░██░████ ░██    ░██   ░███████  ░████████ 
░██        ░██    ░██ ░██    ░██ ░██    ░██ ░███     ░████████   ░██    ░██    ░██    
░██        ░██    ░██ ░██    ░██ ░█████████ ░██      ░██     ░██ ░██    ░██    ░██    
 ░██   ░██ ░██   ░███ ░███   ░██ ░██        ░██      ░██     ░██ ░██    ░██    ░██    
  ░██████   ░█████░██ ░██░█████   ░███████  ░██      ░█████████   ░███████      ░████ 
                  ░██                                                                 
            ░███████                                                                  
                                                                                      
                        CYBERSECURITY AWARENESS BOT                                    ";

            Run run = new Run(art);
            run.Foreground = System.Windows.Media.Brushes.Cyan;
            Paragraph paragraph = new Paragraph(run);
            paragraph.Margin = new Thickness(0);
            txtChatHistory.Document.Blocks.Add(paragraph);
        }

        // Adds a message to the chat history with specified colour
        private void AddMessageToChat(string message, System.Windows.Media.Brush colour)
        {
            Run run = new Run(message + "\n");
            run.Foreground = colour;

            Paragraph paragraph = new Paragraph(run);
            paragraph.Margin = new Thickness(0);

            txtChatHistory.Document.Blocks.Add(paragraph);

            // Auto-scroll to the bottom
            txtChatHistory.ScrollToEnd();
        }

        // Processes and sends the user's message
        private void SendUserMessage(string userInput)
        {
            // Do nothing if input is empty
            if (string.IsNullOrWhiteSpace(userInput))
                return;

            // Display what the user typed
            AddMessageToChat($"You: {userInput}", System.Windows.Media.Brushes.White);

            // Check if this is the first message (name entry)
            if (!_nameEntered)
            {
                // Store the user's name
                _chatbot.SetUserName(userInput.Trim());
                _nameEntered = true;

                // Display welcome message with their name
                AddMessageToChat($"Bot: Hello, {userInput.Trim()}! Welcome to CyberBot.", System.Windows.Media.Brushes.Yellow);
                AddMessageToChat("Bot: Type 'help' to see a list of available commands.", System.Windows.Media.Brushes.Magenta);

                // Update status bar
                txtStatus.Text = $"Current user: {userInput.Trim()}";
            }
            else
            {
                // Get response from the chatbot
                string response = _chatbot.GetResponse(userInput);
                AddMessageToChat($"Bot: {response}", System.Windows.Media.Brushes.Cyan);
            }

            // Clear the input box for the next message
            txtUserInput.Clear();
        }

        // Event handler for the Send button
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendUserMessage(txtUserInput.Text);
        }

        // Event handler for the Exit button
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Allows the user to press Enter to send a message
        private void txtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendUserMessage(txtUserInput.Text);
            }
        }
    }
}