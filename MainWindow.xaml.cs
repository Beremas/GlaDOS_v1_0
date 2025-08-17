using System.Windows;
using System.Windows.Media.Animation;
using System.ComponentModel.DataAnnotations;
using System.Windows.Threading;
using System.Text;
using GlaDOS_v1_0.Service_LLM;
using GlaDOS_v1_0.Storage.Repositories;
using GlaDOS_v1_0.Storage.Services;
using Newtonsoft.Json;
using GlaDOS_v1_0.Service_STT.Vosk_NAudio;
using System.Windows.Input;
using GlaDOS_v1_0.Logs;
using GlaDOS_v1_0.Service_Camera;
using GlaDOS_v1_0.Service_Camera.EmotionRecognition;
using GlaDOS_v1_0.CustomException;

namespace GlaDOS_v1_0
{
	public partial class MainWindow : Window
	{
		[Required]
		private Service_TTS.Microsoft.TextToSpeechService	_ttsMicrosoftService;
		[Required]
		private Service_TTS.Glados.TextToSpeechService		_ttsGladosService;
		[Required]
		private SpeechToTextService							_sttService;
		[Required]
		private LLamaService								_llamaService;
		[Required]
		private ChatHistoryService							_chatHistoryService;
		private bool										_isAwaitingCommandAfterWakeWord = false;
		private CancellationTokenSource?					_streamingCts;

		private  DispatcherTimer							_uptimeTimer;
		private  DateTimeOffset								_startTime;
		private	 CameraWatcher								_cameraWatcher;

		private List<string>								_diagnosticIssues;


		public MainWindow()
		{
			InitializeComponent();

			JsonLogger.LogMessage($"System started at: {DateTime.UtcNow}");

			Loaded += OnLoaded;
		}

		private async void OnLoaded(object sender, RoutedEventArgs e)
		{
			
			StartTimer();

			await Task.Run(async () =>
			{
				_diagnosticIssues = new();
				// Phase 1
				await NarrateStepAsync("Initiating system..", async () => 
				{
					try
					{
						ShowProgressBar(true);

						AppSettings.Load();
					}
					catch (Exception ex)
					{
						JsonLogger.LogException(ex, "Phase 1 failure: ");
						Application.Current.Shutdown();
					}
					
				});

				// Phase 2
				await NarrateStepAsync("Loading voice, ears and eyes modules...", async () =>
				{
					try
					{
						//voice
						if (AppSettings.TTS != null && AppSettings.TTS.enabled)
						{
							if (AppSettings.TTS.synth_voice == Enums.SynthVoice.Glados)
							{
								_ttsGladosService = new Service_TTS.Glados.TextToSpeechService();
							}
							if (AppSettings.TTS.synth_voice == Enums.SynthVoice.Microsoft)
							{
								_ttsMicrosoftService = new Service_TTS.Microsoft.TextToSpeechService(AppSettings.TTS.culture);
							}
							if (AppSettings.TTS.synth_voice == Enums.SynthVoice.Chappy)
							{
								//todo
							}
						}

						//ear
						if (AppSettings.STT != null && AppSettings.STT.enabled)
						{
							_sttService = new SpeechToTextService(saveToFile: false);
						}

						//eyes
						if (AppSettings.Camera != null && AppSettings.Camera.enabled)
						{
							Dispatcher.Invoke(() =>
							{
								_cameraWatcher = new CameraWatcher();
							});
						}
					}
					catch(Exception ex)
					{
						JsonLogger.LogException(ex, "Phase 2 failure: ");
						Application.Current.Shutdown();
					}
						
				});

				// Phase 3
				await NarrateStepAsync($"Injecting '{AppSettings.LLS.personality}' personality...", async () =>
				{
					try
					{
						ChatHistoryService.Initialize(AppSettings.LLS.personality);
						_chatHistoryService = ChatHistoryService.Instance;

						_llamaService = new LLamaService(AppSettings.LLS.model, _chatHistoryService);
						_llamaService.AskForFullResponseAsync("Hello").GetAwaiter().GetResult();
					} 
					catch (Exception ex)
					{
						JsonLogger.LogException(ex, "Phase 3 failure: ");
						Application.Current.Shutdown();
					}
				});

				// Phase 4
				await NarrateStepAsync("Running system diagnostics...", async () =>
				{

					if (AppSettings.TTS.enabled && _ttsMicrosoftService == null && _ttsGladosService == null)
						_diagnosticIssues.Add("TTS service not initialized.");

					if (AppSettings.STT.enabled && _sttService == null)
						_diagnosticIssues.Add("STT service not initialized.");

					if (AppSettings.Camera.enabled && _cameraWatcher == null)
						_diagnosticIssues.Add("Camera service not initialized.");

					if (_llamaService == null)
						_diagnosticIssues.Add("LLama service not initialized.");

					if (_chatHistoryService == null || _chatHistoryService.GetMessages().Count == 0)
						_diagnosticIssues.Add("Chat history service not initialized or no history fed up.");
				});

				if(_diagnosticIssues != null &&  _diagnosticIssues.Count > 0)
				{
					var message = $"Diagnostics completed with issues:\n- {string.Join("\n- ", _diagnosticIssues)}";
					JsonLogger.LogException(new SystemDiagnosticsException(message));
					Application.Current.Shutdown();
				}

				// Phase 5
				await NarrateStepAsync("Finalizing system startup. All systems operating at full capacity.", async () =>
				{
					try
					{
						ShowProgressBar(false);
						OpenEyeAndLookAround();

						ShowQuestionTextBox();
						_sttService?.Start(HandleSpeechResult);
						//_cameraWatcher?.Start(HandleRecognitonResult);
					}
					catch(Exception ex)
					{
						JsonLogger.LogException(ex, "Phase 5 failure: ");
						Application.Current.Shutdown();
					}
				});
				JsonLogger.LogMessage($"System completed initiation at: {DateTime.UtcNow} and fully operating.");
			});
		}

		private void StartTimer()
		{
			_startTime = DateTime.Now;

			_uptimeTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(1)
			};
			_uptimeTimer.Tick += UpdateSystemInformation;
			_uptimeTimer.Start();
		}


		#region Dialoge Engine

		private void ShowQuestionTextBox()
		{
			Dispatcher.Invoke(() =>
			{
				VoiceInputTextBox.Visibility = Visibility.Visible;
				VoiceInputTextBox.Width = SystemParameters.PrimaryScreenWidth;
				VoiceInputTextBox.Focus();
			});
			
		}

		private void VoiceInputTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				string input = VoiceInputTextBox.Text.Trim();
				if (!string.IsNullOrEmpty(input))
				{
					VoiceInputTextBox.Clear();

					AskAndDisplayStream(input);
				}
			}
		}

	

		private static bool CheckForWakeWorld(string text)
		{
			return (text.ToLower() == "system" || text.ToLower() == "sistema");
		}


		private void HandleRecognitonResult(string emotion, OpenCvSharp.Rect face)
		{
			if (EmotionHandler.ShouldSendEmotion(emotion))
			{
				EmotionHandler.TimeoutEmotion(emotion);
				var emotionBuiltMessage = $"I feel {emotion} emotion today.";
				AskAndDisplayStream(emotionBuiltMessage);
			}
		}


		private void HandleSpeechResult(string question)
		{
			if (!string.IsNullOrWhiteSpace(question))
			{
				// Wake word check
				bool isWakeWorld = CheckForWakeWorld(question);
				if (isWakeWorld)
				{
					_isAwaitingCommandAfterWakeWord = true;
					ShowAndHideAfterwardSpeechMessage("You summoned me. Speak.");
					return;
				}
				// If no wake word was used before, ignore the input
				if (!_isAwaitingCommandAfterWakeWord)
					return;
				// Reset flag so next command requires wake word again
				_isAwaitingCommandAfterWakeWord = false;


				AskAndDisplayStream(question);
			}
		}

		private void AskAndDisplayStream(string question)
		{
			DisplayStream(async chunkCallback =>
			{
				string fullAnswer = await _llamaService.AskAndStreamChunksAsync(question, chunkCallback);
				await NarrateStepAsync(fullAnswer);
				_chatHistoryService.SaveChat(question, fullAnswer);
			});
		}

		#endregion


		#region UI interation
		public void UpdateSystemInformation(object sender, EventArgs e)
		{
			Dispatcher.Invoke(() =>
			{
				var uptime = DateTime.Now - _startTime;
				string formatted = string.Format("🟢 Uptime: {0:%h}h {0:%m}m {0:%s}s", uptime);
				UptimeText.Text = formatted;

				string personality = string.Format("🧠 Personality: {0}", AppSettings.LLS.personality.ToString());
				PersonalityText.Text = personality;

				string synthVoice = string.Format("🔊 Voice synth: {0}", AppSettings.TTS.synth_voice.ToString());
				VoiceSynthText.Text = synthVoice;

			});
		}

		private void OpenEyeAndLookAround()
		{
			Dispatcher.Invoke(() =>
			{
				var openEye = (Storyboard)this.Resources["OpenEyeAnimation"];
				openEye.Begin();
				Storyboard irisLook = (Storyboard)this.Resources["IrisLookAround"];
				irisLook.Begin();
			});
		}

		private void ShowProgressBar(bool show)
		{
			Dispatcher.Invoke(() =>
			{
				LoadingProgressBar.Visibility = show == true ? Visibility.Visible : Visibility.Collapsed;
			});
		}

		/// <summary>
		/// Streams and displays a message in the WPF UI text area in real time, updating it chunk by chunk.
		/// This method ensures thread-safe UI access using the dispatcher and handles cancellation of previous streams.
		/// After the full message is received, it remains visible for a duration based on its length, optionally extended by an additional delay.
		/// </summary>
		/// <param name="streamFunc"> A delegate that accepts a callback to receive message chunks as they arrive asynchronously. </param>
		/// <param name="forcedDelayMs"> Optional additional delay in milliseconds to extend the time the message remains visible after streaming completes.</param>
		public void DisplayStream(Func<Action<string>, Task> streamFunc, int? forcedDelayMs = null)
		{
			_streamingCts?.Cancel();
			_streamingCts = new CancellationTokenSource();
			var token = _streamingCts.Token;

			Dispatcher.Invoke(() =>
			{
				SpeechOverlayText.Text = "";
				((Storyboard)FindResource("ShowSpeech")).Begin();
			});

			StringBuilder fullText = new();

			Task.Run(async () =>
			{
				try
				{
					DateTime lastUpdate = DateTime.UtcNow;

					await streamFunc(chunk =>
					{
						// Append in local thread
						fullText.Append(chunk);
						string currentText = fullText.ToString();

						Dispatcher.Invoke(() =>
						{
							SpeechOverlayText.Text = currentText;
						});

						lastUpdate = DateTime.UtcNow;
					});

					// Calculate delay
					int wordCount = fullText.ToString()
						.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

					int delayMs = Math.Clamp(wordCount * 500, 3000, 15000);
					if (forcedDelayMs != null)
					{
						delayMs += forcedDelayMs.Value;
					}

					var elapsed = (DateTime.UtcNow - lastUpdate).TotalMilliseconds;
					var remaining = Math.Max(0, delayMs - (int)elapsed);

					await Task.Delay(remaining, token);

					Dispatcher.Invoke(() =>
					{
						((Storyboard)FindResource("HideSpeech")).Begin();
					});
				}
				catch (Exception ex)
				{
					JsonLogger.LogException(ex);
				}
			});
		}

		/// <summary>
		/// Displays a message in the WPF UI text area in a thread-safe way by invoking on the main thread.
		/// The message remains visible for a calculated duration based on its length, with an optional additional delay before it is hidden.
		/// <param name="message">Message to be displayed onto the UI</param>
		/// <param name="forcedDelayMs">Optional additional delay in milliseconds to extend the time the message remains visible after streaming completes</param>
		private void ShowAndHideAfterwardSpeechMessage(string? message, int? forcedDelayMs = null)
		{
			Dispatcher.Invoke(() =>
			{
				SpeechOverlayText.Text = message;
				((Storyboard)FindResource("ShowSpeech")).Begin();
			});

			// Estimate delay time based on message length
			int wordCount = message?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length ?? 0;
			int delayMs = Math.Clamp(wordCount * 500, 3000, 15000);
			int totalDelay = forcedDelayMs != null ? delayMs + forcedDelayMs.Value : delayMs;

			// Blocking delay (not recommended on UI thread)
			Task.Delay(totalDelay).Wait();

			Dispatcher.Invoke(() =>
			{
				((Storyboard)FindResource("HideSpeech")).Begin();
			});
		}

		private void ShowSpeechMessage(string? message)
		{
			Dispatcher.Invoke(() =>
			{
				SpeechOverlayText.Text = message;
				((Storyboard)FindResource("ShowSpeech")).Begin();
			});
		}

		private void HideSpeechMessage()
		{
			Dispatcher.Invoke(() =>
			{
				((Storyboard)FindResource("HideSpeech")).Begin();
			});
		}

		private async Task NarrateStepAsync(string message, Func<Task>? asyncAction = null)
		{
			ShowSpeechMessage(message);
			if (AppSettings.TTS != null && AppSettings.TTS.enabled)
			{
				_ttsMicrosoftService?.SpeakAsync(message);
			}

			await Task.Yield();

			if (asyncAction != null)
				await asyncAction();

			await Task.Delay(4000);

			HideSpeechMessage();
		}

		#endregion


	}
}
