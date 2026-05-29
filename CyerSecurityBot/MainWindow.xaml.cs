using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CyberSecurityBot
{
    /// <summary>
    /// MainWindow.xaml.cs - Code-behind for the GUI
    /// Handles:
    /// - Voice greeting playback
    /// - Displaying ASCII art
    /// - Sending user messages to the chatbot
    /// - Displaying bot responses
    /// - Error handling for empty inputs
    /// - Scrolling chat to bottom automatically
    /// </summary>
    public partial class MainWindow : Window
    {
        // Create an instance of our chatbot engine
        private ChatbotEngine chatbot;

        public MainWindow()
        {
            // This calls the InitializeComponent() method which loads the XAML design
            InitializeComponent();

            // Set up the chatbot
            InitializeChatbot();
        }

        /// <summary>
        /// Sets up the chatbot, displays ASCII art, plays voice greeting, shows welcome message
        /// </summary>
        private void InitializeChatbot()
        {
            // Create new chatbot engine instance
            chatbot = new ChatbotEngine();

            // Display the ASCII art in the header
            AsciiArtBlock.Text = chatbot.GetASCIIArt();

            // Play the voice greeting
            try
            {
                // SoundPlayer plays WAV files
                using (SoundPlayer player = new SoundPlayer("greeting.wav"))
                {
                    player.Play(); // Play asynchronously (doesn't block the UI)
                }
            }
            catch (Exception)
            {
                // If the file isn't found or there's an error, just continue without audio
                // This is good error handling - the program still works
            }

            // Show the welcome message in the chat
            AddBotMessage(chatbot.GetWelcomeMessage());
        }

        /// <summary>
        /// Called when the Send button is clicked
        /// </summary>
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        /// <summary>
        /// Called when the user presses a key in the text box
        /// Allows Enter key to send message
        /// </summary>
        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        /// <summary>
        /// Main method to send user message and get bot response
        /// </summary>
        private void SendMessage()
        {
            // Get the text the user typed and remove extra spaces
            string input = UserInput.Text.Trim();

            // ERROR HANDLING: Check if input is empty
            if (string.IsNullOrWhiteSpace(input))
            {
                AddSystemMessage("Please enter a message.");
                return;
            }

            // Add user's message to the chat display
            AddUserMessage(input);

            // Get the bot's response
            string response = chatbot.GetResponse(input);

            // Add bot's response to the chat display
            AddBotMessage(response);

            // Clear the input text box for the next message
            UserInput.Clear();

            // Put focus back on the text box so user can type immediately
            UserInput.Focus();
        }

        /// <summary>
        /// Adds a user message to the chat display with the user's bubble style
        /// </summary>
        private void AddUserMessage(string message)
        {
            ChatDisplay.Items.Add(new ChatMessage
            {
                Message = $"You: {message}",
                Style = (Style)FindResource("UserBubble")
            });
            ScrollToBottom();
        }

        /// <summary>
        /// Adds a bot message to the chat display with the bot's bubble style
        /// </summary>
        private void AddBotMessage(string message)
        {
            ChatDisplay.Items.Add(new ChatMessage
            {
                Message = $"Bot: {message}",
                Style = (Style)FindResource("BotBubble")
            });
            ScrollToBottom();
        }

        /// <summary>
        /// Adds a system message (for errors or notifications)
        /// </summary>
        private void AddSystemMessage(string message)
        {
            ChatDisplay.Items.Add(new ChatMessage
            {
                Message = $"System: {message}",
                Style = null
            });
            ScrollToBottom();
        }

        /// <summary>
        /// Automatically scrolls the chat display to show the most recent message
        /// </summary>
        private void ScrollToBottom()
        {
            if (ChatDisplay.Items.Count > 0)
            {
                ChatDisplay.ScrollIntoView(ChatDisplay.Items[ChatDisplay.Items.Count - 1]);
            }
        }
    }

    /// <summary>
    /// ChatMessage class - stores a single chat message with its style
    /// Used for the ListBox in MainWindow.xaml
    /// </summary>
    public class ChatMessage
    {
        public string Message { get; set; }
        public Style Style { get; set; }
    }
}