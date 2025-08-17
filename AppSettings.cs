using GlaDOS_v1_0.Enums;
using GlaDOS_v1_0.Logs;
using GlaDOS_v1_0.Storage.Repositories;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace GlaDOS_v1_0
{
	public static class AppSettings
	{
		public static LLSConfig LLS { get; private set; }
		public static STTConfig STT { get; private set; }
		public static TTSConfig TTS { get; private set; }
		public static Camera Camera { get; private set; }
		
		private static bool _isLoaded = false;

		private class AppSettingsInstance
		{
			public LLSConfig LLS { get; set; }
			public STTConfig STT { get; set; }
			public TTSConfig TTS { get; set; }
			public Camera Camera { get; set; }
		}

		public static void Load()
		{
			if (_isLoaded) return;

			string configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

			if (!File.Exists(configPath))
			{
				return;
			}
			
			var json = File.ReadAllText(configPath);
			var settings = JsonConvert.DeserializeObject<AppSettingsInstance>(json);

			// Validate and assign
			LLS = settings?.LLS ?? throw new InvalidDataException("LLS section missing in config.");
			STT = settings?.STT ?? throw new InvalidDataException("STT section missing in config.");
			TTS = settings?.TTS ?? throw new InvalidDataException("TTS section missing in config.");
			Camera = settings?.Camera ?? throw new InvalidDataException("Camera section missing in config.");

			_isLoaded = true;
		}
	}

	public class LLSConfig
	{
		public Personality personality { get; set; }
		public string model { get; set; }
	}

	public class STTConfig
	{
		public bool enabled { get; set; }
		public string model { get; set; }
	}

	public class TTSConfig
	{
		public bool enabled { get; set; }
		public SynthVoice synth_voice { get; set; }

		public SpeechCulture culture { get; set; }
	}

	public class Camera
	{
		public bool enabled  { get; set; }
	}
}
