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
		}

		private const string _discordAuthTokenFileName = "token.auth";
		public string DiscordBotToken;

		public bool LocalFilesReady => GetRequiredInitalizations() == RequiredInitalizations.None;


		//-------DEBUG----------
		private const string _fakeTokenStateFileName = "state.fake";
		public FakeTestingToken FakeTokenState = new FakeTestingToken();
		//----------------------


		public LocalFiles()
		{
			CreateFilesIfNotExist();
			LoadFileContents();
		}

		private void CreateFilesIfNotExist ()
		{
			if (!Exists(_discordAuthTokenFileName))
			{
				WriteTextToFile(_discordAuthTokenFileName, string.Empty);
			}

			//-------DEBUG----------
			if (!Exists(_fakeTokenStateFileName))
			{
				WriteTextToFile(_fakeTokenStateFileName, string.Empty);
			}
			//----------------------
		}

		private void LoadFileContents()
		{
			ReadTextFromFile(_discordAuthTokenFileName, out DiscordBotToken);

			//-------DEBUG----------
			string tokenStateRaw;
			ReadTextFromFile(_fakeTokenStateFileName, out tokenStateRaw);
			FakeTokenState = ((FakeTestingToken) Newtonsoft.Json.JsonConvert.DeserializeObject(tokenStateRaw)) ?? new FakeTestingToken();
			//----------------------
		}

		public void SaveFileContents ()
		{
			//-------DEBUG----------
			WriteTextToFile(_fakeTokenStateFileName, Newtonsoft.Json.JsonConvert.SerializeObject(FakeTokenState));
			//----------------------
		}

		public RequiredInitalizations GetRequiredInitalizations()
		{
			var output = RequiredInitalizations.None;

			if (string.IsNullOrEmpty(DiscordBotToken))
			{
				output |= RequiredInitalizations.DiscordToken;
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