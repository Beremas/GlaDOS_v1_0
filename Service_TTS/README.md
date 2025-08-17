🗣️ TextToSpeechService Class
Provides basic Text-to-Speech (TTS) capabilities using the eSpeak speech synthesizer, and audio playback support via NAudio.

📦 Namespace

GlaDOS_v1_0.TTS


📘 Summary
The TextToSpeechService class allows you to:

🔊 Speak text aloud using the eSpeak engine.
🎧 Play audio files (.wav) using NAudio.

🧱 Constructor

public TextToSpeechService(string path)

path: The full path to the espeak.exe executable. 
Stores the path for invoking eSpeak commands.

🗨️ Methods
void SpeakOutLoud(string text)
Uses eSpeak to convert the given text to speech and play it out loud.
Accepts parameters like:
	-ven+f3 → English voice, female variant 3.
	-s145 → Speed (words per minute).
	-p60 → Pitch.
	-a160 → Amplitude (volume).


tts.SpeakOutLoud("Hello, world!");
💡 Automatically escapes problematic characters (like " quotes) in the input text.

void ReproduceAudio(string wavPath)
Plays back a .wav file using NAudio.

tts.ReproduceAudio("speech.wav");

Internally:
- Initializes WaveOutEvent as the output device.
- Loads audio from AudioFileReader.
- Blocks until playback completes using a loop with Thread.Sleep().

🔧 Example Usage

var tts = new TextToSpeechService("C:\\Tools\\espeak.exe");
tts.SpeakOutLoud("GLaDOS is active.");
tts.ReproduceAudio("C:\\Audio\\hello.wav");


📎 Dependencies
NAudio for audio playback.

eSpeak for TTS generation.