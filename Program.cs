using System; //Imports basic C# functions
using NAudio.Wave;  //Makes it possible to play audio files
using System.Collections.Generic; //Enables use of list and dictionaries
using System.Threading; //Used for typing effect and audio playback
using System.IO;
using System.Reflection.Metadata.Ecma335; //Used for ASCII art and checking audio files

class Chatbot //The class name is Chatbot, it contains all chatbot functions
{
    static void Main(string[] args) //This is where the execution begins, It's the main function 
    {
        Console.Title = "Top Bot"; //The console window title is set to "Top Bot"
        Console.OutputEncoding = System.Text.Encoding.UTF8; //Ensures emojis display properly

        PlayAudio("new short intro.wav"); //The PlayAudio functions plays the selected .wav sound file
        DisplayAsciiArt(); //This function shows the ASCII art

        Console.ForegroundColor = ConsoleColor.Magenta; //Changes the text colour to Magenta
        string userName = GetValidUserName(); //Asks the user for their name
        Console.ResetColor(); //Resets the colour to the default colour

        TypingEffect($"\n🤖 Welcome, {userName}! Feel free to ask questions.", ConsoleColor.Cyan); //Displays the welcome message with typing effecct
        Console.WriteLine(); //Skips a space
        Console.ForegroundColor = ConsoleColor.DarkGreen; //Changes the text colour to Dark Green
        Console.WriteLine($"-How are you?     -What's your purpose? -What can I ask about you?"); //Displays all Top Bot's available questions
        Console.WriteLine($"-Password safety  -Phishing             -Safe browsing");
        Console.WriteLine($"-How do I create a strong password?     -What are the signs of a phishing attack?");
        Console.WriteLine($"-How can I tell if a website is safe?   -How do I stay safe on social media?");
        Console.ResetColor(); //Resets the colour to the default colour
        ChatLoop(userName); //Enables the conversation loop

    }

    static void PlayAudio(string fileName) //The function to play an audio file
    {
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
    static void DisplayAsciiArt() //Function to display the ASCII art
    {
        string filePath1 = "gengar.txt"; //First ASCII art file
        string filePath2 = "top bot logo.txt"; //Second ASCII art file

        Console.ForegroundColor = ConsoleColor.Cyan; //This changes the text colour to cyan

        if (File.Exists(filePath1)) //Checks if the first file exists
        {
            Console.WriteLine(File.ReadAllText(filePath1)); //Reads and prints the file contents
        }
        else //If the file is missing, will display an error message 
        {
            Console.ForegroundColor = ConsoleColor.Red; //This changes the text colour to Red
            Console.WriteLine("ERROR first ASCII art file is not found!!!"); //Displays an error message
            Console.ResetColor(); //Resets the text colour back to cyan
        }

        Console.WriteLine(); //Skips a space for better readability 

        if (File.Exists(filePath2)) //Checks if the second file exists 
        {
            Console.WriteLine(File.ReadAllText(filePath2)); //Reads and prints the second ASCII art
        }
        else //If the file is missing it will display an error message 
        {
            Console.ForegroundColor = ConsoleColor.Red; //Changes the text colour to red
            Console.WriteLine("ERROR second ASCII art file is not found!!!"); //Displays an error message
            Console.ResetColor(); //Resets the text colour to default
        }

        Console.ResetColor(); //Resets the text colour to default
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

    static string TopBotResponse(string input, string userName, out string audioFile) //Function to get a response based on user input
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
    }

    static void ChatLoop(string userName) //Function that handles the chatbot conversation
    {
        while (true) //Infinite loop to keep the conversation going
        {
            Console.ForegroundColor = ConsoleColor.Magenta; //Sets the text colour to magenta 
            Console.Write("\n👤 You: "); //Displays "You: " prompt for user input
            Console.ResetColor(); //Resets text colour
            string input = Console.ReadLine()?.ToLower().Trim(); //Reads user input, converts it to lower case and removes extra spaces

            if (input == "exit" || input.Equals("e", StringComparison.OrdinalIgnoreCase)) //If user types "exit", this exits the loop & ends the chat
            {
                TypingEffect($"\nGoodbye {userName}! Have a great day further!", ConsoleColor.Green); //Prints an exit message
                break; //Exit the loop
            }

            string audioFile; // Declares a string variable to store the name of the audio file that will be played as a response
            string response = TopBotResponse(input, userName, out audioFile); //Get chatbot response

            TypingEffect($"\n🤖 Top Bot: {response}\n", ConsoleColor.Cyan); //Display chatbot's response

            PlayAudio(audioFile); //Plays corresponding audio response

            Console.ForegroundColor= ConsoleColor.Yellow;
            Console.WriteLine("Press 'e' to exit or type a new question: ");
            Console.ResetColor();
        }
    }
    static bool IsValidName(string name)
    {
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
}
