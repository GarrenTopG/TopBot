using System.Text.RegularExpressions; //Used for pattern matching in user inputs
using NAudio.Wave;  //Makes it possible to play audio files

class Chatbot //The class name is Chatbot, it contains all chatbot functions
{
    static string userName = ""; //Stores the user's name
    static string userInterest = "";  //Stores the user's stated interest
    const string interestFile = "interest.txt";  //Filename to store user interest persistently

    static void Main(string[] args)
    {
        // Set up the application
        Console.Title = "Top Bot"; // Sets console window title
        Console.OutputEncoding = System.Text.Encoding.UTF8; // Supports emoji and symbols

        // Intro with sound and ASCII art
        PlayAudio("new short intro.wav");
        DisplayAsciiArt();

        Console.ForegroundColor = ConsoleColor.Magenta;
        userName = GetValidUserName(); // Prompt for and validate user's name
        Console.ResetColor();

        TypingEffect($"\n🤖 Welcome, {userName}! I'm here to help you with cybersecurity.", ConsoleColor.Cyan);
        DisplayHelp(); // Show the help menu

        ChatLoop(); // Start the chatbot conversation loop
    }

    static void PlayAudio(string fileName) //The function to play an audio file
    {
        // This function safely plays an audio file using NAudio
        try
        {
            if (File.Exists(fileName))  //Checks if the file exists
            {
                using (var audioFile = new AudioFileReader(fileName))  //Loads the audio file
                using (var outputDevice = new WaveOutEvent()) //Creates an audio playback device
                {
                    outputDevice.Init(audioFile); //Intializes the playback
                    outputDevice.Play(); //Plays the audio

                    AutoResetEvent audioFinishedPlaying = new AutoResetEvent(false);  //Creates an event that will be used to signal when the audio has finished playing. 
                                                                                      //"false" means it starts in an unsignaled state.
                    outputDevice.PlaybackStopped += (sender, args) => audioFinishedPlaying.Set();  //Attaches an event handler to detect when the audio playback stops. 
                                                                                                   //When the event is triggered, it signals `audioFinishedPlaying` to resume execution.
                    audioFinishedPlaying.WaitOne(); //Waits for the audio playback to complete.
                }
            }
            else  //This displays the error message if the file is missing
            {
                Console.ForegroundColor = ConsoleColor.Red; //Changes the text colour to Red
                Console.WriteLine($"ERROR Audio file {fileName} not found!!!"); //Displays the error message
                Console.ResetColor(); //Resets text colour back to default
            }
        }
        catch (Exception ex) //Catches all audio playback errors that occurs
        {
            Console.WriteLine($"ERROR Could not play audio: {ex.Message}"); //Displays the error message
        }
    }
    static void DisplayAsciiArt()
    {
        // Display branding using ASCII art from file
        string[] artFiles = { "top bot logo.txt" };
        Console.ForegroundColor = ConsoleColor.Cyan;

        foreach (var file in artFiles)
        {
            if (File.Exists(file))
            {
                Console.WriteLine(File.ReadAllText(file));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: {file} not found!");
            }
            Console.WriteLine();
        }

        Console.ResetColor();
    }

    static void TypingEffect(string text, ConsoleColor color) //Function that prints text with a typing effect
    {
        Console.ForegroundColor = color; //Sets the text colour

        foreach (char c in text) //Loops through each character in the text
        {
            Console.Write(c); //Prints the character without a new line
            Thread.Sleep(20); //Pauses for 20 milliseconds to simulate typing
        }

        Console.ResetColor(); //Resets the text colour
        Console.WriteLine(); //Moves to the next line after finishing
    }

    /* static string TopBotResponse(string input, string userName, out string audioFile) //Function to get a response based on user input
    {
        var responses = new Dictionary<string, (string Text, string Audio)> //Stores responses and the corresponding audio files
        { {"whats your purpose", ($"My sole mission is to make sure you stay secure online. " +
        $"Whether it’s password safety, phishing defence, safe browsing, or social media security, " +
        $"I provide the knowledge you need to stay untouchable in the digital world", "purpose.wav")},
            {$"what can i ask about you", ($"The questions i am programmed to answer are as follows: ", "question list.wav")},
            {$"password safety", ($"Your password is your first line of defense. " +
            $"A weak password is an invitation for hackers.", "password safety.wav") },
            {$"phishing", ($"Phishing is when cybercriminals try to trick you into giving" +
            $" up sensitive information—passwords, banking details, or personal data.", "phishing.wav") },
            { $"safe browsing", ($"Most hacks happen because people visit the wrong websites. Stay smart.",
            "safe browsing.wav")},
            {$"how do i create a strong password",($"A strong password follows these rules:", "strong password.wav") },
            {$"what are the signs of a phishing attack", ($"Hackers use deception to trick you. Look out for:",
            "signs of phishing.wav") },
            {$"how can i tell if a website is safe", ($"Before you enter personal data, check:","safe website.wav") },
            {$"how do i stay safe on social media", ("Social media is a goldmine for cybercriminals. Stay smart","safe on sm.wav") },
            {$"how are you", ("I'm running at peak efficiency as always","how are you.wav") }
        };

        input = input.ToLower().Replace("'", "").Replace("?", "").Trim(); //Converts input to lowercase and removes unnecessary characters

        foreach (var key in responses.Keys.OrderByDescending(k => k.Length)) //Loops through stored questions, prioritizes the longer ones
        {
            if (input.Contains(key.ToLower())) //If user input contains a known question
            {
                var response = responses[key]; //Gets the corresponding response
                audioFile = response.Audio; //Gets the audio file name
                return response.Text; //Returns the response text
            }
        }

        audioFile = "invalid input.wav"; //If no match is found , return the default audio file 
        return $"Hmm I'm not sure I understand that, {userName}. Please rephrase the question"; //The default response when the input is recognized
    } */

    static void ChatLoop()
    {
        // Keeps the chatbot running until user types "exit"
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("\n👤 You: ");
            Console.ResetColor();
            string input = Console.ReadLine()?.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(input)) continue;

            if (input == "exit" || input == "e")
            {
                TypingEffect($"\nGoodbye {userName}! Have a great day!", ConsoleColor.Green);
                break;
            }

            string response = GetSentimentAdjustedResponse(input);
            TypingEffect($"\n🤖 Top Bot: {response}", ConsoleColor.Cyan);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nAsk another question or type 'exit' to leave.");
            Console.ResetColor();
        }
    }
    static bool IsValidName(string name)
    {
        // Check that name contains only letters or spaces
        foreach (char c in name)
        {
            if (!char.IsLetter(c) && c != ' ')
                return false;
        }
        return true;
    }
    static string GetValidUserName()
    {
        string userName;
        do
        {
            Console.Write("\n👤 Enter your name: ");
            userName = (Console.ReadLine() ?? "").Trim();

            if (string.IsNullOrEmpty(userName) || !IsValidName(userName))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid name! Please enter only letters (no numbers or special characters).\n");
                Console.ResetColor();
            }
        } while (string.IsNullOrEmpty(userName) || !IsValidName(userName));
        return userName;
    }

    static Dictionary<string, List<string>> keywordResponses = new Dictionary<string, List<string>>
    {
        { "phishing", new List<string>
            {
                "Phishing scams often look like messages from trusted sources. Always double-check the sender's email and avoid clicking links unless you're sure they're safe.",
                "Attackers use fake websites to steal your credentials. Before logging in anywhere, inspect the URL closely for odd characters or misspellings.",
                "Be cautious of urgent messages that pressure you to act quickly. Real companies don’t demand sensitive information over email or text."
            }
        },
        { "password", new List<string>
            {
                "Use passwords that are long, random, and unique. A mix of uppercase letters, symbols, and numbers greatly boosts your account’s security.",
                "Avoid using personal information in your passwords. Things like your name or birthdate are easy for attackers to guess or find online.",
                "Consider using a password manager to generate and store secure passwords. This reduces reuse and keeps your accounts safer."
            }
        },
        { "privacy", new List<string>
            {
                "Think twice before posting personal details online. Once shared, your info can be stored, sold, or misused without your knowledge.",
                "Review your app permissions regularly. Many apps request access they don’t need, which can expose more data than you intended.",
                "Use privacy-focused tools like encrypted messaging apps and search engines that don’t track you. They help limit your digital footprint."
            }
        }
    };

    static string GetResponseFromKeyword(string input)
    {
        // Match input keywords to a list of responses and pick one randomly
        foreach (var entry in keywordResponses)
        {
            if (input.Contains(entry.Key))
            {
                string selected = entry.Value[new Random().Next(entry.Value.Count)];
                return selected;
            }
        }

        // Default fallback message
        return "I'm not yet programmed to answer that. In a future update, I'll be able to help with even more topics!";
    }

    static string GetSentimentAdjustedResponse(string input)
    {
        /* string audioFile;
        string responseFromTopBot = TopBotResponse(input, userName, out audioFile);

        if (!responseFromTopBot.StartsWith("Hmm I'm not sure")) // Meaning: we got a match
        {
            PlayAudio(audioFile); // Play the associated audio
            return responseFromTopBot;
        } */

        // Emotion detection
        if ((input.Contains("worried") || input.Contains("scared")) && input.Contains("scam"))
        {
            return "It's completely understandable to feel that way. Scammers can be very convincing. Let me share some tips to help you stay safe.";
        }
        if (input.Contains("worried") || input.Contains("scared"))
        {
            return "It's okay to feel worried. Cyber threats are real, but I'm here to help you navigate them.";
        }
        if (input.Contains("frustrated") || input.Contains("angry"))
        {
            return "Take a deep breath. Cybersecurity can be frustrating, but we’ll work through it together.";
        }
        if (input.Contains("curious"))
        {
            return "Curiosity is great! Ask me about anything from phishing to password safety to privacy tips.";
        }

        // Detect and save interest
        if (input.StartsWith("i'm interested in ") || input.StartsWith("im interested in "))
        {
            int startIndex = input.IndexOf("interested in ") + "interested in ".Length;
            userInterest = input.Substring(startIndex).Trim('.', ' ', '?');

            File.WriteAllText(interestFile, userInterest);
            return $"Great! I'll remember that you're interested in {userInterest}. It's a key area of cybersecurity.";
        }

        // Handle "remind me" with persistence
        if (Regex.IsMatch(input, @"\b(remind me|what did i say|what was my interest)\b"))
        {
            if (File.Exists(interestFile))
            {
                string savedInterest = File.ReadAllText(interestFile).Trim();
                if (!string.IsNullOrEmpty(savedInterest))
                {
                    return $"You told me you're interested in {savedInterest}. Be sure to keep learning about it regularly.";
                }
            }
            return "I don't have any saved interest from you yet. Try saying 'I'm interested in privacy' or another topic.";
        }

        // Keyword detection
        return GetResponseFromKeyword(input);
    }

    static void DisplayHelp()
    {
        // Shows available commands to the user
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n📌 You can ask me about these cybersecurity topics:");
        Console.WriteLine(" - Phishing\n - Password Safety\n - Online Privacy");
        Console.WriteLine("\n📌 I'm trained to respond to emotions like:");
        Console.WriteLine(" - Worried\n - Frustrated\n - Curious");
        Console.WriteLine("\n📌 Example prompts:");
        Console.WriteLine(" - I'm worried about online scams.");
        Console.WriteLine(" - Give me a phishing tip.");
        Console.WriteLine(" - I'm interested in privacy.");
        Console.WriteLine(" - Remind me what I liked.");
        Console.WriteLine(" - How can I make a strong password?");
        Console.ResetColor();
    }
}
