using System;
using System.Collections.Generic;

namespace Apocryph.Dao.Bot.Discord
{
	/// <summary>
	/// Note: This class is just for testing bot commands
	/// </summary>
	class FakeTestingToken
	{
		public Dictionary<string, int> data = new Dictionary<string, int>();
		public Dictionary<string, string> IdToUser = new Dictionary<string, string>();

		public bool Pay(string fromUser, string toUserId, int amount, out string msg)
		{
			try
			{
				data[fromUser] -= amount;
				data[IdToUser[toUserId]] += amount;
				msg = $"Transfered {amount} from {fromUser} to {toUserId}";
				return true;
			}
			catch (Exception e)
			{
				msg = e.ToString();
			}

			return false;
		}

		public float Bal(string user, out bool error, out string msg)
		{
			try
			{
				var value = data[user];
				error = false;
				msg = string.Empty;
				return value;
			}
			catch (Exception e)
			{
				msg = e.ToString();
			}

			error = true;
			return 0;
		}

		internal bool Add(string username, int bal, out string userId)
		{
			if (data.ContainsKey(username))
			{
				userId = string.Empty;
				return false;
			}

			userId = data.Count.ToString();
			data.Add(username, bal);
			IdToUser.Add(userId, username);
			return true;
		}
	}
}
