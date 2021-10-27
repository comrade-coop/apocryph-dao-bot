using System.Collections.Generic;

namespace Apocryph.Dao.Bot.Discord.Model
{
    record Result(bool IsValid, string Message);
    record PayResult(bool IsValid, string Message) : Result(IsValid, Message);
    record BalanceResult(decimal Amount, bool IsValid, string Message);
    
    public class LocalToken
    {
        public readonly Dictionary<string, decimal> UserBalances = new();
        private readonly Dictionary<ulong, string> _userIdToUser = new();
        private static readonly object Locker = new();
 
        internal PayResult Pay(string fromUser, string toUser, decimal amount)
        {
            lock (Locker)
            {
                // both users must exist
                if (UserBalances.ContainsKey(fromUser) && UserBalances.ContainsKey(toUser))
                {
                    // sender must have money
                    if (UserBalances[fromUser] >= amount)
                    {
                        UserBalances[fromUser] -= amount;
                        UserBalances[toUser] += amount;

                        return new(true, $"Transferred {amount} from {fromUser} to {toUser}");
                    }
					
                    return new(false, "Insufficient balance");
                }

                return new(false, $"User {toUser} does not exist");
            }
        }

        internal BalanceResult Balance(string username)
        {
            lock (Locker)
            {
                if (UserBalances.ContainsKey(username))
                {
                    var value = UserBalances[username];
                    return new (value, true, string.Empty);
                }

                return new(0, false, $"User {username} not found");
            }
        }

        internal Result Add(string username, ulong userId, decimal balance)
        {
            lock (Locker)
            {
                if (!UserBalances.ContainsKey(username) && !_userIdToUser.ContainsKey(userId))
                {
                    _userIdToUser.Add(userId, username);
                    UserBalances.Add(username, balance);

                    return new(true, string.Empty);
                }

                return new(false, string.Empty);
            }
        }
    }
}