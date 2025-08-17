using GlaDOS_v1_0.Enums;
using GlaDOS_v1_0.Storage.DTOs;
using GlaDOS_v1_0.Storage.Repositories;
using LLama.Common;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;


namespace GlaDOS_v1_0.Storage.Services
{
	public class ChatHistoryService
	{
		private static ChatHistoryService? _instance;
		public static ChatHistoryService Instance => _instance ?? throw new InvalidOperationException("Call Initialize() first.");
		private ChatHistoryDto _chatHistoryDto;
		private ChatHistoryRepository _chatHistoryRepository;

		private ChatHistoryService(Personality personality)
		{
			_chatHistoryRepository = new ChatHistoryRepository(personality);
			_chatHistoryDto = _chatHistoryRepository.Load() ?? new ChatHistoryDto();
		}

		public static void Initialize(Personality personality)
		{
			if (_instance != null)
				return;

			_instance = new ChatHistoryService(personality);
		}

		public void AddMessage(ChatMessageDto message)
		{
			_chatHistoryDto.Messages.Add(message);
			Save(); 
		}

		public IReadOnlyList<ChatMessageDto> GetMessages()
		{
			return _chatHistoryDto.Messages;
		}

		public void SaveChat(string question, string? answer)
		{
			AddMessage(new ChatMessageDto
			{
				Role = AuthorRole.User,
				Content = question,
				Timestamp = DateTime.Now,
			});
			AddMessage(new ChatMessageDto
			{
				Role = AuthorRole.Assistant,
				Content = answer,
				Timestamp = DateTime.Now,
			});
		}

		private void Save()
		{
			_chatHistoryRepository.Save(_chatHistoryDto);
		}

		public void SaveAll() => Save();
	}
}
