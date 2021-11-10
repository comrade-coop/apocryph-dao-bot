using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perper.Model;

namespace Apocryph.Dao.Bot
{
    public static class StateExtensions
    {
        public static async Task ConfirmUser(this IState state, string userName)
        {
            await state.SetAsync("user-confirmed", true);
        }
        
        public static async Task<bool> IsUserConfirmed(this IState state, string userName)
        {
            var result =  await state.TryGetAsync<bool>("user-confirmed");
            return result.Item1 && result.Item2;
        }

        public static async Task<bool> IsValidSession(this IState state, string session)
        {
            var result = await state.TryGetAsync<WebSessionData>($"web-{session}");
            return result.Item1;
        }
        
        public static async Task<string> CreateSession(this IState state, string session, string userName, ulong userId)
        {
            await state.SetAsync($"web-{session}", new WebSessionData(userName, userId));
            return session;
        }
    }
    
    public record WebSessionData(string UserName, ulong UserId);
    
}
