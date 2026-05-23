using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Threading;

namespace CybersecurityChatbotWPF
{
    public class Chatbot
    {
        // Properties to store user information
        public string UserName { get; private set; } = string.Empty;
        public string UserInterest { get; private set; } = string.Empty;

        // Track the last topic discussed for follow-up questions
        private string LastTopic { get; set; } = string.Empty;

        // Random generator for varied responses
        private Random random = new Random();

        // Constructor
        public Chatbot()
        {
        }

        // Plays the voice greeting when application starts
        public void PlayVoiceGreeting()
        {
            try
            {
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

                if (File.Exists(audioPath))
                {
                    SoundPlayer player = new SoundPlayer(audioPath);
                    player.PlaySync();
                }
            }
            catch (Exception ex)
            {
                // Silently fail if audio is not available - program still works
                Console.WriteLine($"Audio error: {ex.Message}");
            }
        }

        // Sets the user's name after they enter it
        public void SetUserName(string name)
        {
            UserName = name;
        }

        // Main method that processes user input and returns a response
        public string GetResponse(string userInput)
        {
            // Check for empty input
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "I did not catch that. Please say something.";
            }

            string lowerInput = userInput.ToLower().Trim();

            // =====================================================
            // SENTIMENT DETECTION - Detects user emotions
            // =====================================================

            if (lowerInput.Contains("worried") || lowerInput.Contains("scared") || lowerInput.Contains("nervous"))
            {
                LastTopic = "worried";
                string[] responses = {
                    "It is normal to feel worried. Cyber threats can be intimidating. Let me share some simple tips to help you feel safer.",
                    "I understand your concern. Many South Africans feel the same way. Start with small steps like using strong passwords.",
                    "Your feelings are valid. Online safety is important. Here is a practical tip to help put your mind at ease."
                };
                return responses[random.Next(responses.Length)] + " Use unique passwords for each account and enable two-factor authentication where possible.";
            }

            if (lowerInput.Contains("curious") || lowerInput.Contains("interested") || lowerInput.Contains("want to learn"))
            {
                LastTopic = "curious";
                string[] responses = {
                    "That is great that you are curious. Learning about cybersecurity is the first step to staying safe online.",
                    "I like your enthusiasm for cybersecurity. Let me share something interesting with you.",
                    "Curiosity is excellent for learning. Here is a useful cybersecurity fact for you."
                };
                return responses[random.Next(responses.Length)] + " Did you know that most cyber attacks start with a simple email scam?";
            }

            if (lowerInput.Contains("frustrated") || lowerInput.Contains("annoyed") || lowerInput.Contains("tired"))
            {
                LastTopic = "frustrated";
                string[] responses = {
                    "I understand your frustration. Cybersecurity can feel overwhelming, but let us take it step by step.",
                    "I hear you. Many people feel frustrated by online threats. Small changes can make a big difference.",
                    "Your frustration is valid. The good news is that you are already taking the right step by learning about safety."
                };
                return responses[random.Next(responses.Length)] + " Start with just one change this week - update your important passwords.";
            }

            // =====================================================
            // MEMORY FEATURE - Remembers user interests
            // =====================================================

            if (lowerInput.Contains("interested in") || lowerInput.Contains("i like"))
            {
                if (lowerInput.Contains("privacy"))
                {
                    UserInterest = "privacy";
                    return $"I will remember that you are interested in privacy. This is very important for staying safe online. I will use this information in our future conversations.";
                }
                else if (lowerInput.Contains("password"))
                {
                    UserInterest = "passwords";
                    return $"I will remember that you want to learn about passwords. Strong passwords are the foundation of online security.";
                }
                else if (lowerInput.Contains("phishing"))
                {
                    UserInterest = "phishing";
                    return $"I will remember that you are interested in phishing. This knowledge will help you spot scams and fake emails.";
                }
                else
                {
                    UserInterest = "cybersecurity";
                    return $"I will remember that you are interested in cybersecurity. This is a very good topic to explore.";
                }
            }

            // Recall user interest when they ask for a tip
            if (!string.IsNullOrEmpty(UserInterest) && (lowerInput.Contains("tip") || lowerInput.Contains("advice")))
            {
                if (UserInterest == "privacy")
                {
                    string[] tips = {
                        $"Since you care about privacy, {UserName}, you should review your social media privacy settings regularly.",
                        $"For someone interested in privacy, {UserName}, consider using a VPN when connecting to public Wi-Fi at coffee shops or airports.",
                        $"As a privacy-focused user, {UserName}, try using a browser like Brave or Firefox with enhanced protection enabled."
                    };
                    return tips[random.Next(tips.Length)];
                }
                else if (UserInterest == "passwords")
                {
                    string[] tips = {
                        $"Since you are interested in passwords, {UserName}, consider using a password manager like Bitwarden or LastPass to store your passwords securely.",
                        $"For someone who cares about password security, {UserName}, enable two-factor authentication on all accounts that offer it.",
                        $"As a password security enthusiast, {UserName}, never reuse the same password across different websites or applications."
                    };
                    return tips[random.Next(tips.Length)];
                }
            }

            // =====================================================
            // CONVERSATION FLOW - Handles follow-up questions
            // =====================================================

            if (lowerInput.Contains("tell me more") || lowerInput.Contains("more") || lowerInput.Contains("explain more"))
            {
                if (LastTopic == "password")
                {
                    return "More about passwords: A strong password should be at least 12 characters long and include uppercase letters, lowercase letters, numbers, and symbols. Never use personal information like your birthday or your pet's name. Would you like another tip?";
                }
                else if (LastTopic == "phishing")
                {
                    return "More about phishing: Scammers often create fake websites that look exactly like real ones. Always check the website address carefully. Look for the padlock icon in your browser and be careful of messages that say 'urgent' or 'your account will be closed'.";
                }
                else if (LastTopic == "browsing")
                {
                    return "More safe browsing tips: Use ad-blockers and privacy extensions in your browser. Clear your cookies and browsing history regularly. Do not save your passwords in your browser - use a password manager instead.";
                }
                else if (LastTopic == "worried")
                {
                    return "To help with your concerns, here are three simple steps to follow: First, use unique passwords for each account. Second, enable two-factor authentication. Third, never click on suspicious links in emails or text messages. These three habits will protect you from most common online threats.";
                }
                else
                {
                    return "What would you like to know more about? You can ask me about passwords, phishing, safe browsing, or privacy. Just type the topic and I will give you more information.";
                }
            }

            if (lowerInput.Contains("another tip") || lowerInput.Contains("another one"))
            {
                if (LastTopic == "password")
                {
                    return "Another password tip: Use a passphrase instead of a single word. For example, 'BlueElephantRunsFast' is strong and easy to remember.";
                }
                else if (LastTopic == "phishing")
                {
                    return "Another phishing tip: Hover your mouse over any link before clicking to see where it really goes. If the web address looks strange or has spelling mistakes, do not click on it.";
                }
                else
                {
                    return GetResponse("tip");
                }
            }

            // =====================================================
            // BASIC RESPONSES - Greetings and purpose
            // =====================================================

            if (lowerInput.Contains("how are you"))
            {
                string[] greetings = {
                    "I am doing well, thank you for asking. How can I help you with cybersecurity today?",
                    "I am functioning properly and ready to help you stay safe online.",
                    "All systems are working well. What would you like to learn about today?"
                };
                return greetings[random.Next(greetings.Length)];
            }

            if (lowerInput.Contains("purpose") || lowerInput.Contains("what do you do"))
            {
                string[] purposes = {
                    "My purpose is to educate you about cybersecurity. You can ask me about passwords, phishing scams, privacy protection, or safe browsing habits.",
                    "I am here to help you stay safe online. Ask me about passwords, phishing, privacy, or safe browsing and I will give you useful tips.",
                    "Think of me as your personal cybersecurity assistant. I provide tips and information to help you navigate the internet safely."
                };
                return purposes[random.Next(purposes.Length)];
            }

            if (lowerInput.Contains("what can i ask") || lowerInput.Contains("help"))
            {
                return "Here is what I can help you with:\n\n- password : Get password safety tips\n- phishing : Learn to spot fake emails and scams\n- privacy : Get privacy protection tips\n- safe browsing : Learn to browse the web safely\n- tip : Get random cybersecurity advice\n- joke : Hear a cybersecurity joke\n- I am interested in... : I will remember your interest\n- tell me more : Follow up on a topic\n\nJust type a command to get started.";
            }

            // =====================================================
            // KEYWORD RECOGNITION - Main cybersecurity topics
            // =====================================================

            if (lowerInput.Contains("privacy"))
            {
                LastTopic = "privacy";
                string[] responses = {
                    "Privacy tip: Review your social media privacy settings regularly. Limit what information is visible to the public.",
                    "Privacy tip: Use a VPN when connecting to public Wi-Fi networks to encrypt your internet traffic.",
                    "Privacy tip: Check which apps have access to your location, camera, and microphone on your phone. Remove permissions for apps that do not need them.",
                    "Privacy tip: Use privacy-focused browsers like Brave or Firefox with enhanced tracking protection turned on.",
                    "Privacy tip: Clear your cookies and browser history regularly to prevent companies from tracking your online activity."
                };
                return responses[random.Next(responses.Length)];
            }

            if (lowerInput.Contains("password"))
            {
                LastTopic = "password";
                string[] responses = {
                    "Password safety: Use at least 12 characters with a mix of uppercase letters, lowercase letters, numbers, and symbols.",
                    "Password safety: Never reuse the same password across different accounts. Use a password manager to store your passwords securely.",
                    "Password safety: Enable two-factor authentication on all accounts that offer this feature. It adds an extra layer of security.",
                    "Password safety: Avoid using personal information like your birthday, your pet's name, or your address in your passwords.",
                    "Password safety: Change your important passwords every 3 to 6 months, especially for banking and email accounts."
                };
                return responses[random.Next(responses.Length)];
            }

            if (lowerInput.Contains("phishing"))
            {
                LastTopic = "phishing";
                string[] responses = {
                    "Phishing awareness: Never click on links in emails or text messages that you were not expecting. Hover over the link first to see where it really goes.",
                    "Phishing awareness: Real companies will never ask for your password or personal information via email. If someone asks, it is probably a scam.",
                    "Phishing awareness: Check the sender's email address carefully. Scammers often use addresses that look similar to real ones but have small differences.",
                    "Phishing awareness: Look for spelling and grammar mistakes in messages. These are common signs of phishing attempts.",
                    "Phishing awareness: If you are not sure about a message, go directly to the company's website instead of clicking any links in the message."
                };
                return responses[random.Next(responses.Length)];
            }

            if (lowerInput.Contains("safe browsing") || lowerInput.Contains("browsing") || lowerInput.Contains("browser"))
            {
                LastTopic = "browsing";
                string[] responses = {
                    "Safe browsing: Always look for 'https://' and the padlock icon in the address bar before entering any personal information on a website.",
                    "Safe browsing: Avoid using public Wi-Fi for banking or other sensitive activities. If you must use public Wi-Fi, use a VPN.",
                    "Safe browsing: Keep your browser updated to the latest version. Updates often include important security fixes.",
                    "Safe browsing: Use ad-blockers and privacy extensions to block malicious advertisements and trackers.",
                    "Safe browsing: Clear your cookies and browser cache regularly to remove tracking data that websites have stored about you."
                };
                return responses[random.Next(responses.Length)];
            }

            // Entertainment responses
            if (lowerInput.Contains("joke"))
            {
                string[] jokes = {
                    "Why did the hacker go to the bank? To get his account hacked.",
                    "What is a computer's favourite type of music? Encryption.",
                    "Why was the computer cold? It left its Windows open.",
                    "What do you call a hacker who flies planes? A cyber-pilot.",
                    "Why do hackers wear leather jackets? To keep their phish dry."
                };
                return jokes[random.Next(jokes.Length)];
            }

            if (lowerInput.Contains("fun fact") || lowerInput.Contains("fact"))
            {
                string[] facts = {
                    "Did you know? The first computer virus was created in 1983 and was called 'Elk Cloner'.",
                    "Fact: '123456' is still the most common password used worldwide. Please do not use this password.",
                    "Did you know? Most cybersecurity breaches are caused by human error, not technical problems.",
                    "Fact: The world's first hacker was a woman named Susan Headley in the 1970s.",
                    "Did you know? The average person has over 100 passwords to remember. This is why password managers are useful."
                };
                return facts[random.Next(facts.Length)];
            }

            // General tip
            if (lowerInput.Contains("tip") || lowerInput.Contains("advice"))
            {
                string[] tips = {
                    "Enable two-factor authentication on all accounts that offer this feature. It is one of the best ways to protect yourself.",
                    "Keep your software and operating system updated. Updates fix security problems that hackers could use against you.",
                    "Back up your important files to an external hard drive or cloud storage regularly. This protects you from data loss.",
                    "Be careful of email attachments, even from people you know. Their account might have been hacked.",
                    "Use a VPN when connecting to public Wi-Fi networks at coffee shops, airports, or hotels.",
                    "Create unique passwords for every account. Do not reuse the same password on different websites.",
                    "Review your social media privacy settings at least once every few months.",
                    "If an online offer sounds too good to be true, it is probably a scam. Be very careful.",
                    "Never share your verification codes or one-time passwords with anyone, even if they claim to be from a company you trust.",
                    "Use a password manager to generate and store strong passwords so you do not have to remember them all."
                };
                return $"Cybersecurity tip:\n\n{tips[random.Next(tips.Length)]}";
            }

            // Default response for unrecognized input
            return "I did not understand that. Could you please rephrase?\n\nTry asking about:\n- password for safety tips\n- phishing to spot scams\n- privacy for privacy tips\n- safe browsing for web safety\n- tip for cybersecurity advice\n- joke for a laugh\n- fun fact to learn something new\n- help to see all options";
        }
    }
}