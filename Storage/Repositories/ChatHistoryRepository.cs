using GlaDOS_v1_0.Enums;
using GlaDOS_v1_0.Storage.DTOs;
using Newtonsoft.Json;
using System;
using System.IO;

namespace GlaDOS_v1_0.Storage.Repositories
{


	public class ChatHistoryRepository : IChatHistoryRepository
	{
		private string _ukNeutralChatPath			= Path.Combine(AppContext.BaseDirectory, "Storage", "JSON", "Personalities", "uk_neutral_chat.json");
		private string _ukGladosChatPath			= Path.Combine(AppContext.BaseDirectory, "Storage", "JSON", "Personalities", "uk_glados_chat.json");
		private string _ukChappyChatPath			= Path.Combine(AppContext.BaseDirectory, "Storage", "JSON", "Personalities", "uk_chappy_chat.json");
		private string currentChatPath;

		public ChatHistoryRepository(Personality personality) 
		{
			currentChatPath = personality switch
			{
				Personality.Neutral => _ukNeutralChatPath,
				Personality.Glados => _ukGladosChatPath,
				Personality.Chappy => _ukChappyChatPath,
				_ => _ukNeutralChatPath,
			};
		}

		public ChatHistoryDto Load()
		{
			if (!File.Exists(currentChatPath))
				return new ChatHistoryDto();

			string json = File.ReadAllText(currentChatPath);
			return JsonConvert.DeserializeObject<ChatHistoryDto>(json) ?? new ChatHistoryDto();
		}

		public void Save(ChatHistoryDto history)
		{
			var json = JsonConvert.SerializeObject(history, Formatting.Indented);
			File.WriteAllText(currentChatPath, json);
		}
	}
}
