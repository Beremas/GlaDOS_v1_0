🗣️ SpeechToTextService - Vosk Speech Recognition for C#
A lightweight speech-to-text wrapper around the Vosk recognition engine using NAudio for microphone input. It supports real-time speech recognition with optional audio saving and supports multiple languages (currently English and Italian).

✅ Features
🎙️ Real-time microphone speech recognition
🌐 Multi-language support (English, Italian)
💾 Optional recording to .wav files
🧠 Easy callback-based result handling
🔁 Offline recognition from saved audio files

📦 Namespace

using GlaDOS_v1_0.STT;

🧪 Example

var sttService = new SpeechToTextService(SpokenLanguage.en_us, saveToFile: false);
sttService.Start(text => Console.WriteLine("Recognized: " + text));

// Later...
sttService.Stop();

🧾 Enum: SpokenLanguage

public enum SpokenLanguage
{
    en_us, // English (US)
    it     // Italian
}
Used to specify the Vosk model language.

🧠 Class: SpeechToTextService

🔧 Constructor

public SpeechToTextService(SpokenLanguage speechCulture, bool saveToFile = false, int deviceNumber = 0)
- Loads the appropriate language model from disk.
- Sets up the microphone input and Vosk recognizer.
- Prepares to record audio if saveToFile is true.

▶️ Start(...)

public void Start(Action<string>? onFinalResultCallback = null)
Begins recording from the microphone. Optionally registers a callback for recognized text.

⏹️ Stop()

public void Stop()
Stops the recording session and releases resources.

📁 RecognizeFromFile(...)

public async Task<string?> RecognizeFromFile(string wavPath)
Runs recognition on a .wav audio file. Returns the transcribed text as a string.

🧩 Events

public event Action<string>? OnFinalResult;
Fired each time a final recognition result is produced.

🔍 Internal Helpers

private static string ExtractText(string json)
Extracts the "text" property from Vosk’s JSON result.

📂 Model Directory Structure

English (US):
STT\Model\vosk-model-small-en-us-0.15

Italian:
STT\Model\vosk-model-small-it-0.22

🧼 Cleanup
Internally disposes WaveInEvent and WaveFileWriter. Provides error logging if recording stops unexpectedly.

🚫 Error Handling
Throws DirectoryNotFoundException if the model path is missing.

Displays a MessageBox to inform users of missing Vosk models.