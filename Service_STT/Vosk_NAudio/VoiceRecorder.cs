using System;
using System.IO;
using NAudio.Wave;

namespace GlaDOS_v1_0.Service_STT.Vosk_NAudio
{
	public class VoiceRecorder
	{
		private WaveInEvent _waveIn;
		private WaveFileWriter? _writer;
		public string? OutputFilePath { get; private set; }

		private readonly bool _saveToFile;
		private readonly Action<byte[], int>? _onDataReceived;

		public VoiceRecorder(bool saveToFile = true, Action<byte[], int>? onDataReceived = null, int deviceNumber = 0)
		{
			if (WaveIn.DeviceCount == 0)
				throw new InvalidOperationException("No recording devices found.");

			_saveToFile = saveToFile;
			_onDataReceived = onDataReceived;

			var deviceInfo = WaveIn.GetCapabilities(deviceNumber);
			Console.WriteLine($"Using input device: {deviceNumber} - {deviceInfo.ProductName}");

			if (_saveToFile)
				OutputFilePath = Path.Combine(Path.GetTempPath(), $"voice_{Guid.NewGuid()}.wav");

			_waveIn = new WaveInEvent
			{
				DeviceNumber = deviceNumber,
				WaveFormat = new WaveFormat(16000, 1)
			};

			_waveIn.DataAvailable += OnDataAvailable;
			_waveIn.RecordingStopped += OnRecordingStopped;
		}

		public void StartRecording()
		{
			if (_saveToFile && OutputFilePath != null)
			{
				_writer = new WaveFileWriter(OutputFilePath, _waveIn.WaveFormat);
				Console.WriteLine($"Recording started. Saving to: {OutputFilePath}");
			}
			else
			{
				Console.WriteLine("Recording started (live mode, not saving to file).");
			}

			_waveIn.StartRecording();
		}

		public void StopRecording()
		{
			_waveIn?.StopRecording();
		}

		private void OnDataAvailable(object sender, WaveInEventArgs e)
		{
			if (_saveToFile && _writer != null)
			{
				_writer.Write(e.Buffer, 0, e.BytesRecorded);
				_writer.Flush();
			}

			// Invoke callback for real-time processing (e.g., live transcription)
			_onDataReceived?.Invoke(e.Buffer, e.BytesRecorded);
		}

		private void OnRecordingStopped(object sender, StoppedEventArgs e)
		{
			_writer?.Dispose();
			_writer = null;

			_waveIn.Dispose();
			_waveIn = null;

			if (_saveToFile && OutputFilePath != null)
			{
				var fileInfo = new FileInfo(OutputFilePath);
				Console.WriteLine($"Recording stopped. File size: {fileInfo.Length} bytes");
			}

			if (e.Exception != null)
			{
				Console.WriteLine("Recording stopped due to an error: " + e.Exception.Message);
			}
		}
	}
}
