GlaDOS v1.0
Overview

GlaDOS v1.0 is an advanced voice-driven personal assistant application that integrates multiple services to simulate an AI with voice, speech-to-text, emotional recognition, and LLM (Large Language Model) capabilities. The system allows interaction through voice commands, recognizes emotions from camera input, and responds with synthesized speech. The application offers dynamic features such as real-time speech streaming, system diagnostics, and personalized interactions based on user preferences.

Features

Voice Interaction:

Uses Text-to-Speech (TTS) services to respond in a natural-sounding voice, with customizable voice options (e.g., Glados, Microsoft).

Speech-to-Text (STT) service allows the system to listen and interpret spoken commands.

Emotional Recognition:

Integrates with a camera module to analyze the user's emotional state, allowing GlaDOS to adapt its responses accordingly.

Large Language Model (LLM):

Powered by LLamaService, GlaDOS interacts using a pre-configured personality, answering questions, processing commands, and maintaining conversation history.

System Diagnostics:

At startup, GlaDOS runs diagnostic checks to ensure all services (voice, ears, eyes, and personality) are properly initialized. Any issues are logged and reported.

Streaming Responses:

Delivers responses in chunks, with real-time updates on the user interface. It ensures smooth communication with a visible overlay of ongoing responses.

Personalized Interaction:

GlaDOS is configured with personalized settings (e.g., TTS voice, personality type) to provide a customized experience.

Workflow

The system goes through several phases during startup:

Initialization:

Loads application settings.

Initializes the TTS and STT services, as well as the camera and emotion recognition modules.

Personality Injection:

Loads the desired personality model (e.g., Glados, Microsoft) for the LLM to ensure the responses are consistent with the character.

System Diagnostics:

Checks if all services are initialized correctly and logs any issues.

Finalizing:

All systems are set, and GlaDOS starts listening for commands and interacting with the user.

Key Classes and Methods

MainWindow: The main WPF window where all interactions take place.

Service_TTS: Handles Text-to-Speech functionality with Microsoft and Glados voices.

SpeechToTextService: Converts speech input into text that GlaDOS can process.

LLamaService: A service for interacting with the LLM model to generate answers based on user queries.

CameraWatcher: Manages camera input to recognize and analyze emotions based on facial expressions.

ChatHistoryService: Tracks and stores the conversation history to improve the context of interactions.

JsonLogger: Logs messages and errors to a JSON file for diagnostics and troubleshooting.

EmotionHandler: Processes emotions detected from the camera and triggers relevant responses.

How It Works
1. System Boot-Up

The MainWindow class is responsible for managing the entire boot process. Upon loading:

The system begins with an initialization phase, where settings are loaded from the AppSettings.

The voice (TTS) module is loaded based on user preferences (e.g., Glados or Microsoft voice).

If enabled, the system starts the Speech-to-Text (STT) service and prepares the camera module for emotion recognition.

2. Phase Execution

Each phase in the startup is narrated, and progress is displayed with a progress bar. These phases include:

Initiating System: Loading the configuration and settings.

Loading Voice, Ears, and Eyes Modules: Initializing TTS, STT, and camera services.

Injecting Personality: Loading the chosen LLM personality (e.g., Glados).

Running System Diagnostics: Ensuring all services are properly initialized.

Finalizing Startup: Preparing the system for interaction and enabling live speech processing.

3. Speech Interaction

Once the system is ready, the user can interact through voice commands:

Wake Word Detection: GlaDOS listens for specific wake words (e.g., "system" or "sistema") to activate.

Processing Commands: After the wake word is detected, GlaDOS processes the following speech input and responds accordingly, either by asking questions or providing information.

Emotion Responses: If the system detects emotions (via camera), it customizes its responses, such as saying, "I feel happy today."

4. Real-Time Message Streaming

GlaDOS uses a DisplayStream method to show real-time responses to the user. As the model generates responses in chunks, the UI is updated incrementally, and the system ensures smooth interaction by adjusting the delay based on message length.

5. UI Updates

The UI updates in real-time to show system status:

Uptime: Displays the time since the system was started.

Personality: Shows the current active personality model.

Voice Synthesizer: Displays the selected TTS voice model.

Key Considerations

Thread Safety: The UI updates, speech synthesis, and speech recognition all ensure thread-safe access to the UI elements using the Dispatcher.Invoke method.

Error Handling: All operations are wrapped in try-catch blocks to handle exceptions and ensure a graceful shutdown if any critical service fails.

Real-Time Speech: The system allows for streaming responses, which improves the user experience by showing progress as the system generates a full answer.

Requirements

.NET Framework: This application is built using WPF (Windows Presentation Foundation) with .NET 5 or higher.

Libraries:

Newtonsoft.Json for JSON parsing and logging.

OpenCVSharp for camera and emotion recognition.

LLamaService for Large Language Model (LLM) interactions.

Setup

Clone the Repository: Clone this repository to your local machine.

Install Dependencies: Use NuGet to restore all necessary packages.

Configure Settings: Adjust the AppSettings file to configure the TTS, STT, and LLM models.

Run the Application: Build and run the project using Visual Studio or your preferred .NET IDE.

Troubleshooting

Diagnostic Logs: If the system fails at any phase, check the JsonLogger logs for detailed error messages.

Camera Module: Ensure that the camera is correctly connected and accessible for emotion recognition.

Speech Issues: If the TTS or STT services are not working, verify the configuration settings in AppSettings.

License

This project is licensed under the MIT License - see the LICENSE file for details.