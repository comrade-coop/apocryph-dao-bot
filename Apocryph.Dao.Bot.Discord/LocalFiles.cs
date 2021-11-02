using System;
using System.IO;

namespace Apocryph.Dao.Bot.Discord
{
	/// <summary>
	/// Handle sensitive data
	/// </summary>
	internal sealed class LocalFiles
	{
		public enum RequiredInitalizations
		{
			None = 0,
			DiscordToken = 1,
			OutputChannelId = 2,
		}

		private const string _discordAuthTokenFileName = "token.auth";
		public string DiscordBotToken { get; private set; }

		private const string _discordOutputChannelFileName = "output_channel.txt";
		private const ulong _defaultDiscordChannelId_botCommands = 888705313807675432;
		public ulong DiscordOutputChannelId { get; private set; }

		private DiscordBot.LogAction _log;
		public bool LocalFilesReady => GetRequiredInitalizations() == RequiredInitalizations.None;


		public LocalFiles(DiscordBot.LogAction log)
		{
			this._log = log;
			CreateFilesIfNotExist();
			LoadFileContents();
		}

		private void CreateFilesIfNotExist()
		{
			if (!Exists(_discordAuthTokenFileName))
			{
				WriteTextToFile(_discordAuthTokenFileName, string.Empty);
			}

			if (!Exists(_discordOutputChannelFileName))
			{
				WriteTextToFile(_discordOutputChannelFileName, _defaultDiscordChannelId_botCommands.ToString());
			}
		}

		private void LoadFileContents()
		{
			string discordBotToken;
			if (!ReadTextFromFile(_discordAuthTokenFileName, out discordBotToken))
			{
				_log?.Invoke($"Missing or invalid file {_discordAuthTokenFileName}", true);
			}
			DiscordBotToken = discordBotToken;

			string channelIdStr;
			if (!ReadTextFromFile(_discordOutputChannelFileName, out channelIdStr))
			{
				_log?.Invoke($"Missing or invalid file {_discordOutputChannelFileName}", true);
			}

			ulong discordOutputChannelId;
			if(!ulong.TryParse(channelIdStr, out discordOutputChannelId))
			{
				_log?.Invoke($"Invalid channel id in file {_discordOutputChannelFileName}", true);
			}

			DiscordOutputChannelId = discordOutputChannelId;
		}

		public void SaveFileContents()
		{
		}

		public RequiredInitalizations GetRequiredInitalizations()
		{
			var output = RequiredInitalizations.None;

			if (string.IsNullOrEmpty(DiscordBotToken))
			{
				output |= RequiredInitalizations.DiscordToken;
			}

			if (DiscordOutputChannelId == 0)
			{
				output |= RequiredInitalizations.OutputChannelId;
			}

			return output;
		}

		private static string ParseToAbsolutePath(string path)
		{
			return Path.GetFullPath(path);
		}

		private static void WriteTextToFile(string path, string content)
		{
			string _filePath = ParseToAbsolutePath(path);
			FileInfo _fileInfo = new FileInfo(_filePath);

			_fileInfo.Directory.Create();
			StreamWriter _writer = File.CreateText(_filePath);
			_writer.Close();
			File.WriteAllText(_filePath, content);
		}

		/// <summary>
		/// Outputs a string result and returns true if it was successful
		/// </summary>
		private static bool ReadTextFromFile(string path, out string content)
		{
			string _filePath = ParseToAbsolutePath(path);
			content = string.Empty;

			if (File.Exists(_filePath))
			{
				content = File.ReadAllText(_filePath);
				return true; //read success
			}

			return false; //read failed
		}

		private static bool Exists(string path)
		{
			string _filePath = ParseToAbsolutePath(path);
			return File.Exists(_filePath);
		}

	}
}