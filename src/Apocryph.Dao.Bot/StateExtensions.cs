﻿using System.Threading.Tasks;
using Perper.Model;

namespace Apocryph.Dao.Bot
{
    public static class StateExtensions
    {
        public static string AddressToUser(string address) => $"address-to-user:{address.ToLower()}";
        public static string UserByUserId(ulong userId) => $"user-by-userId:{userId}";
        public static string UserByUserIdAddress(ulong userId, string address) => $"user-by-userId-address:{userId}:{address.ToLower()}";
        
        public static async Task RegisterAddress(this IState state, ulong userId, string address)
        {
            await state.SetAsync(AddressToUser(address), userId);
            await state.SetAsync(UserByUserId(userId), address.ToLower());
            await state.SetAsync(UserByUserIdAddress(userId, address), false);
        }
        
        public static async Task<string> GetAddress(this IState state, ulong userId)
        {
            var result = await state.TryGetAsync<string>(UserByUserId(userId));
            return result.Item2;
        }
        
        public static async Task<bool> IsAddressAvailable(this IState state, ulong userId, string address)
        {
            var user = await state.TryGetAsync<ulong>(AddressToUser(address));
            
            if (user.Item1)
            {
                if (user.Item2 == userId)
                    return true;

                return false; // address taken by someone else
            }
            
            return true;
        }
        
        public static async Task<bool> IsAddressRegistered(this IState state, ulong userId)
        {
            var result = await state.TryGetAsync<string>(UserByUserId(userId));
            return result.Item1;
        }
        
        public static async Task<bool> IsAddressSigned(this IState state, ulong userId, string address)
        {
            var result = await state.TryGetAsync<bool>(UserByUserIdAddress(userId, address));
            return result.Item1 && result.Item2;
        }
        
        public static async Task SignAddress(this IState state, ulong userId, string address)
        {
            await state.SetAsync(UserByUserIdAddress(userId, address), true);
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
}