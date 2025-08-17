using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using GlaDOS_v1_0.Logs;
using NAudio.Wave;
using Vosk;

namespace GlaDOS_v1_0.Service_STT.Vosk_NAudio
{
	public enum SpokenLanguage
	{
		en_us,
		it
	}

	public class SpeechToTextService
	{
		private readonly Model				 _model;
		private VoskRecognizer				 _recognizer;
		private WaveInEvent					 _waveIn;
		private WaveFileWriter?				 _writer;
		private readonly StringBuilder		 _finalResult;
		private Func<string, Task>?			 _onFinalResultCallback;
		private readonly bool				 _saveToFile;
		private readonly string				 _en_us_path ;
		private readonly string				 _it_path;

		public event Action<string>?		 onFinalResult;
		public string?						 OutputFilePath { get; private set; }



		public SpeechToTextService(bool saveToFile = false, int deviceNumber = 0)
		{
			try
			{
				SpokenLanguage speechCulture = SpokenLanguage.en_us;
				Vosk.Vosk.SetLogLevel(0);
				_finalResult = new StringBuilder();
				_saveToFile = saveToFile;
				_en_us_path = "Service_STT\\Model\\vosk-model-small-en-us-0.15";
				_it_path = "Service_STT\\Model\\vosk-model-small-it-0.22";

				if (AppSettings.STT.model == "en_US")
				{
					speechCulture = SpokenLanguage.en_us;
				}
				else if (AppSettings.STT.model == "it")
				{
					speechCulture = SpokenLanguage.it;
				}

				string modelPath = speechCulture switch
				{
					SpokenLanguage.en_us => Path.Combine(AppContext.BaseDirectory, _en_us_path),
					SpokenLanguage.it => Path.Combine(AppContext.BaseDirectory, _it_path),
					_ => Path.Combine(AppContext.BaseDirectory, _en_us_path),
				};
				if (!Directory.Exists(modelPath))
				{
					MessageBox.Show("Vosk model not found at:\n" + modelPath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					throw new DirectoryNotFoundException("Vosk model directory not found.");
				}

				//This code sets up the Vosk speech recognition engine by loading the appropriate language model and initializing the recognizer.
				_model = new Model(modelPath);
				_recognizer = new VoskRecognizer(_model, 16000.0f);

				//This block of code sets up audio recording from a microphone input device using the NAudio library.
				_waveIn = new WaveInEvent
				{
					DeviceNumber = deviceNumber,
					WaveFormat = new WaveFormat(16000, 1)
				};

				_waveIn.DataAvailable += OnDataAvailable;
				_waveIn.RecordingStopped += OnRecordingStopped;

				if (_saveToFile)
				{
					OutputFilePath = Path.Combine(Path.GetTempPath(), $"voice_{Guid.NewGuid()}.wav");
				}
			}
			catch (Exception ex)
			{
				JsonLogger.LogException(ex);
			}
		}

		public void Start(Action<string>? onFinalResultCallback = null)
		{
			try
			{
				if (_waveIn == null) throw new InvalidOperationException("WaveIn is not initialized.");

				if (_saveToFile && OutputFilePath != null)
				{
					_writer = new WaveFileWriter(OutputFilePath, _waveIn.WaveFormat);
				}

				_recognizer.Reset(); // Reset Vosk internal state
				_finalResult.Clear(); // Clear previous results

				if (onFinalResultCallback != null)
					onFinalResult += onFinalResultCallback;

				_waveIn.StartRecording();
			}
			catch (Exception ex)
			{
				JsonLogger.LogException(ex);
			}
			
		}

		public void Stop()
		{
			_waveIn?.StopRecording();
		}

		private void OnDataAvailable(object? sender, WaveInEventArgs e)
		{
			try
			{
				if (_saveToFile && _writer != null)
				{
					_writer.Write(e.Buffer, 0, e.BytesRecorded);
					_writer.Flush();
				}

				if (_recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
				{
					var result = _recognizer.Result();
					var text = ExtractText(result);
					_finalResult.AppendLine(text);
					onFinalResult?.Invoke(text);
				}
			}
			catch (Exception ex)
			{
				JsonLogger.LogException(ex);
			}
			
		}

		private void OnRecordingStopped(object? sender, StoppedEventArgs e)
		{
			try
			{
				_writer?.Dispose();
				_writer = null;

				_waveIn.Dispose();
				_waveIn = null;

				if (e.Exception != null)
				{
					Console.WriteLine("Recording stopped due to an error: " + e.Exception.Message);
				}
			}
			catch(Exception ex)
			{
				JsonLogger.LogException(ex);
			}
			
		}

		private static string ExtractText(string json)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(json)) return string.Empty;
				using var doc = JsonDocument.Parse(json);
				return doc.RootElement.GetProperty("text").GetString() ?? string.Empty;
			}
			catch (Exception ex) 
			{ 
				JsonLogger.LogException(ex);
				return "";
			}
			
		}

		public async Task<string?> RecognizeFromFile(string wavPath)
		{
			try
			{
				if (!File.Exists(wavPath)) return null;

				await Task.Delay(300); // Allow for final file flush if just written

				using var audioStream = File.OpenRead(wavPath);
				var fileRecognizer = new VoskRecognizer(_model, 16000.0f);
				byte[] buffer = new byte[4096];

				while (audioStream.Read(buffer, 0, buffer.Length) > 0)
				{
					fileRecognizer.AcceptWaveform(buffer, buffer.Length);
				}

				return ExtractText(fileRecognizer.FinalResult());
			}
			catch (Exception ex)
			{
				JsonLogger.LogException(ex);
				return null;
			}
			
		}
	}
}
