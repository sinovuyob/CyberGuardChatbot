# CyberGuardChatbot

A conversational AI-powered chatbot designed to educate users about cybersecurity best practices, password safety, phishing prevention, safe browsing, and data protection. Built with C# and featuring a colorful console UI with audio welcome messages.

Main Window class - This class controls the user interface and handles what happens when users type messages or click buttons. It displays messages in the chat area, calls the chatbot to get responses, and updates the status bar and current topic display. It also clears the chat when the CLEAR button is clicked and sends messages when Enter is pressed.
ChatMessage class - This class stores all the information for a single message in the conversation. It holds the sender name, message text, timestamp, alignment (left or right), and bubble color. When a message is created, it automatically sets the alignment and color based on whether it's a user message or bot message.
