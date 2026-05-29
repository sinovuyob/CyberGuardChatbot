# CyberGuardChatbot

CyberGuard Chatbot is a desktop application built with WPF (Windows Presentation Foundation) that serves as a cybersecurity awareness assistant. The chatbot engages users in conversations about online safety, providing educational tips and guidance on four main topics: password security, scam/phishing protection, privacy protection, and virus/malware prevention. The application features a graphical user interface with a warm brown and nude color scheme, audio greeting playback, memory functionality that remembers conversation context, and basic sentiment detection.

*Main Window class* - This class controls the user interface and handles what happens when users type messages or click buttons. It displays messages in the chat area, calls the chatbot to get responses, and updates the status bar and current topic display. It also clears the chat when the CLEAR button is clicked and sends messages when Enter is pressed.

*ChatMessage class* - This class stores all the information for a single message in the conversation. It holds the sender name, message text, timestamp, alignment (left or right), and bubble color. When a message is created, it automatically sets the alignment and color based on whether it's a user message or bot message.

*Audiomanager class* - This class plays the greeting sound when the application starts. It locates the greeting.wav file in the Audio folder and plays it using Windows SoundPlayer. If the file is missing, it fails silently without crashing.

*ChatEngine class* - This class is the brain that generates responses based on what the user types. It checks for keywords like "password", "scam", "privacy", or "virus" and returns relevant cybersecurity tips. It also remembers the last topic discussed so when the user says "another", it provides a follow-up tip on the same subject.


