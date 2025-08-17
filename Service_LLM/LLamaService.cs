using LLama.Common;
using LLama;
using GlaDOS_v1_0.Storage.Services;
using System;
using System.Text;
using Newtonsoft.Json.Serialization;
using GlaDOS_v1_0.Logs;

namespace GlaDOS_v1_0.Service_LLM
{
	public class LLamaService
	{
		private ModelParams				_modelParams;
		private InteractiveExecutor		_executor;
		private LLamaContext			_context;
		private ChatSession				_chatSession;
		private InferenceParams			_inferenceParams;
		private ChatHistory				_chatHistory;
		private string					_modelPath;
		private ChatHistoryService		_chatHistoryService;

		public LLamaService(string llamaModelPath, ChatHistoryService chatHistoryService)
		{
			_modelPath = llamaModelPath;
			_chatHistoryService = chatHistoryService;
			_modelParams = new ModelParams(_modelPath)
			{
				ContextSize = 4096,
				GpuLayerCount = 5
			};
			using var model = LLamaWeights.LoadFromFile(_modelParams);
			_context = model.CreateContext(_modelParams);

			_chatHistory = new();
			LoadHistory();

			_executor = new InteractiveExecutor(_context);
			_chatSession = new ChatSession(_executor, _chatHistory);
			_inferenceParams = new()
			{
				MaxTokens = 1024,
				AntiPrompts = new List<string> { "User:" },
			};
		}

		private void LoadHistory()
		{

			var messages = _chatHistoryService.GetMessages();

			foreach (var message in messages)
			{
				_chatHistory.AddMessage(message.Role, message.Content);
			}
		}

		/// <summary>
		/// Sends a question to the chat model and streams the response chunk by chunk via a callback.
		/// Removes "Assistant:" from the start (if present) and trims "User:" from the end, even if split across multiple chunks.
		/// Returns the full cleaned response after streaming completes.
		/// </summary>
		/// <param name="question">The user's input question.</param>
		/// <param name="onChunk">A callback invoked with each streamed chunk of the assistant's response.</param>
		/// <returns>The full assistant response with cleaned start and end.</returns>
		public async Task<string> AskAndStreamChunksAsync(string question, Action<string> onChunk)
		{
			try
			{
				var responseStream = _chatSession.ChatAsync(new ChatHistory.Message(AuthorRole.User, question), _inferenceParams);

				StringBuilder answerBuilder = new();
				bool isFirstChunk = true;
				StringBuilder prefixBuffer = new();
				StringBuilder trailingBuffer = new();
				Queue<char> rollingCharBuffer = new();
				int maxTrailingLength = 8; // room for "\nUser:"

				void FlushRollingBuffer()
				{
					if (rollingCharBuffer.Count == 0) return;

					string output = new(rollingCharBuffer.ToArray());
					onChunk?.Invoke(output);
					answerBuilder.Append(output);
					rollingCharBuffer.Clear();
				}

				await foreach (var chunk in responseStream)
				{
					if (isFirstChunk)
					{
						prefixBuffer.Append(chunk);
						string buffered = prefixBuffer.ToString();

						if (buffered.Length >= "Assistant:".Length || !("Assistant:".StartsWith(buffered, StringComparison.OrdinalIgnoreCase)))
						{
							if (buffered.StartsWith("Assistant:", StringComparison.OrdinalIgnoreCase))
							{
								string cleaned = buffered["Assistant:".Length..].TrimStart();
								foreach (var c in cleaned)
									rollingCharBuffer.Enqueue(c);
							}
							else
							{
								foreach (var c in buffered)
									rollingCharBuffer.Enqueue(c);
							}
							isFirstChunk = false;
						}
					}
					else
					{
						foreach (var c in chunk)
							rollingCharBuffer.Enqueue(c);
					}

					// Keep rolling buffer within bounds
					while (rollingCharBuffer.Count > maxTrailingLength)
					{
						char c = rollingCharBuffer.Dequeue();
						onChunk?.Invoke(c.ToString());
						answerBuilder.Append(c);
					}
				}

				// At the end: check if remaining rolling buffer ends with "User:" (with optional newline)
				string remaining = new string(rollingCharBuffer.ToArray()).TrimEnd();

				if (remaining.EndsWith("User:", StringComparison.OrdinalIgnoreCase))
				{
					// Discard
				}
				else
				{
					FlushRollingBuffer();
				}

				return answerBuilder.ToString();
			}
			catch (Exception ex)
			{
				JsonLogger.LogException(ex);
				return "";
			}
		}

		public async Task<string> AskForFullResponseAsync(string question)
		{
			try
			{
				var responseStream = _chatSession.ChatAsync(new ChatHistory.Message(AuthorRole.User, question), _inferenceParams);

				string answer = "";
				await foreach (var chunk in responseStream)
				{
					answer += chunk;
				}

				// Trim assistant prefix & anti-prompt suffix
				if (answer.StartsWith("Assistant:", StringComparison.OrdinalIgnoreCase))
				{
					answer = answer["Assistant:".Length..].TrimStart();
				}
				if (answer.EndsWith("User:", StringComparison.OrdinalIgnoreCase))
				{
					answer = answer[..^"User:".Length].TrimEnd();
				}

				return answer;
			}
			catch (Exception ex)
			{
				JsonLogger.LogException(ex);
				return "";
			}

		}


	}
}
