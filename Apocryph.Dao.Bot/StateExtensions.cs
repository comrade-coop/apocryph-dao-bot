using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Perper.Model;

namespace Apocryph.Dao.Bot
{
    public static class StateExtensions
    {
        public static async Task RegisterAddress(this IState state, string userName, string address)
        {
            await state.SetAsync($"address-to-user:{address.ToLower()}",                               userName.ToLower());
            await state.SetAsync($"user-by-username:{userName.ToLower()}",                               address.ToLower());
            await state.SetAsync($"user-by-username-address:{userName.ToLower()}:{address.ToLower()}",   false);
        }
        
        public static async Task<bool> IsAddressAvailable(this IState state, string userName, string address)
        {
            var user = await state.TryGetAsync<string>($"address-to-user:{address.ToLower()}");
            
            if (user.Item1)
            {
                if (user.Item2.Equals(userName, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false; // address taken by someone else
            }
            
            return true;
        }
        
        public static async Task<bool> IsAddressSigned(this IState state, string userName, string address)
        {
            var result = await state.TryGetAsync<bool>($"user-by-username-address:{userName.ToLower()}:{address.ToLower()}");
            return result.Item1 && result.Item2;
        }
        
        public static async Task SignAddress(this IState state, string userName, string address)
        {
            await state.SetAsync($"user-by-username-address:{userName.ToLower()}:{address.ToLower()}", true);
        }

        public static async Task CreateSession(this IState state, string session, string userName, ulong userId)
        {
            await state.SetAsync($"web-{session}", new WebSessionData(userName, userId));
        }
        
        public static async Task<WebSessionData> GetSession(this IState state, string session)
        {
            var result = await state.TryGetAsync<WebSessionData>($"web-{session}");
            return result.Item2;
        }
        
        public static async Task<bool> IsValidSession(this IState state, string session)
        {
            var result = await state.TryGetAsync<WebSessionData>($"web-{session}");
            return result.Item1;
        }
    }
    
    public record WebSessionData(string UserName, ulong UserId);

    public record UserAddress(string Address, string UserName, bool IsConfirmed);
}
