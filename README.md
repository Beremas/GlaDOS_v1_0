# GlaDOS v1.0



> **⚠️ Personal Project Notice:**  
> This project is for personal use only. It **cannot be used for production or commercial purposes**.  

------------------------------------

> **⚠️ Important Model Note:** 
> GlaDOS requires the **LLama-2-7b.Q5_K_M.gguf** or any other LLM (.gguf) file to function.
Download `llama-2-7b.Q5_K_M.gguf` (or any other LLMs .gguf) and place it into ~\GlaDOS_v1_0\Service_LLM

------------------------------------

## Overview
**GlaDOS v1.0** is an advanced voice-driven personal assistant application that integrates multiple services to simulate an AI with voice, speech-to-text, emotional recognition, and LLM (Large Language Model) capabilities. The system allows interaction through voice commands, recognizes emotions from camera input, and responds with synthesized speech.  

The application offers dynamic features such as real-time speech streaming, system diagnostics, and personalized interactions based on user preferences.

---

## Features

### Voice Interaction
- Uses Text-to-Speech (TTS) services to respond in a natural-sounding voice, with customizable options (e.g., Glados, Microsoft).  
- Speech-to-Text (STT) service allows the system to listen and interpret spoken commands.

### Emotional Recognition
- Integrates with a camera module to analyze the user's emotional state.  
- Adapts responses dynamically based on detected emotions.

### Large Language Model (LLM)
- Powered by **LLamaService**, GlaDOS interacts using a pre-configured personality.  
- Answers questions, processes commands, and maintains conversation history.

### System Diagnostics
- Performs checks at startup to ensure all services (voice, ears, eyes, personality) are properly initialized.  
- Logs and reports any detected issues.

### Streaming Responses
- Delivers responses in chunks with real-time updates on the UI.  
- Provides smooth communication with a visible overlay of ongoing responses.

### Personalized Interaction
- Configured with personalized settings such as TTS voice and personality type.  

---

## Workflow

### 1. Initialization
- Loads application settings.  
- Initializes TTS, STT, and camera/emotion recognition modules.

### 2. Personality Injection
- Loads the chosen personality model (e.g., Glados, Microsoft).  

### 3. System Diagnostics
- Ensures all services are initialized correctly.  
- Logs any issues for troubleshooting.

### 4. Finalizing
- All systems are ready, and GlaDOS starts listening for commands.  

---

## Key Classes and Methods

- **MainWindow** – Main WPF window where all interactions occur.  
- **Service_TTS** – Handles Text-to-Speech functionality.  
- **SpeechToTextService** – Converts spoken input into text.  
- **LLamaService** – Interacts with the LLM for responses.  
- **CameraWatcher** – Detects emotions from the camera.  
- **ChatHistoryService** – Tracks conversation history.  
- **JsonLogger** – Logs messages and errors in JSON format.  
- **EmotionHandler** – Processes detected emotions to trigger responses.  

---

## How It Works

### System Boot-Up
- **MainWindow** manages the boot process.  
- Initialization loads settings, TTS, STT, and the camera module.  

### Phase Execution
- Startup phases are narrated with progress bars:  
  1. Initiating System  
  2. Loading Voice, Ears, and Eyes Modules  
  3. Injecting Personality  
  4. Running System Diagnostics  
  5. Finalizing Startup  

### Speech Interaction
- Wake Word Detection: Listens for "system" or "sistema".  
- Processes Commands: Responds to spoken input after wake word detection.  
- Emotion Responses: Customizes replies based on camera-detected emotions.  

### Real-Time Message Streaming
- Uses `DisplayStream` to show responses incrementally in the UI.  

### UI Updates
- Displays **uptime**, **active personality**, and **selected TTS voice**.  

---

## Key Considerations

- **Thread Safety:** UI updates, speech synthesis, and recognition ensure thread-safe access via `Dispatcher.Invoke`.  
- **Error Handling:** All operations are wrapped in try-catch blocks for graceful shutdown on failures.  
- **Real-Time Speech:** Streaming responses improve user experience by showing progress as answers are generated.  

---

## Requirements

- **.NET Framework:** WPF with .NET 5 or higher.  
- **Libraries:**  
  - `Newtonsoft.Json` for JSON parsing and logging  
  - `OpenCVSharp` for camera and emotion recognition  
  - `LLamaService` for LLM interactions  

---

## Setup

1. **Clone the Repository:**  
   ```bash
   git clone https://github.com/<your-username>/<your-repo>.git
