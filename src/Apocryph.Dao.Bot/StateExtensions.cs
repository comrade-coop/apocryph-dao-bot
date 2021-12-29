using System.Collections.Generic;
using System.Threading.Tasks;
using Perper.Model;

namespace Apocryph.Dao.Bot
{
    public static class StateExtensions
    {
        public static string AddressToUser(string address) => $"address-to-user:{address.ToLower()}";
        public static string UserByUserId(ulong userId) => $"user-by-userId:{userId}";
        public static string UserByUserIdAddress(ulong userId, string address) => $"user-by-userId-address:{userId}:{address.ToLower()}";
        
        public static string UserAirdrop(ulong userId, string airdropType) => $"user-airdrop-{userId}:{airdropType}";
        
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
        
        
        // UserAirdrop
        public static async Task AirdropUser(this IState state, ulong userId, string airdropType)
        {
            await state.SetAsync(UserAirdrop(userId, airdropType), true);
        }
        
        public static async Task<bool> GetUserAirdrop(this IState state, ulong userId, string airdropType)
        {
            var result = await state.TryGetAsync<bool>(UserAirdrop(userId, airdropType));
            return result.Item1 && result.Item2;
        }

        public static async Task<BlockData<TEvent>> GetLatestBlockData<TEvent>(this IState state, string contractAddress)
        {
            var result = await state.TryGetAsync<BlockData<TEvent>>($"{contractAddress}-{typeof(TEvent).Name.ToLower()}-block-data");
            return result.Item2;
        }
        
        public static async Task AppendDataToLatestBlock<TEvent>(this IState state, string contractAddress, ulong blockNumber, TEvent item)
        {
            var key = $"{contractAddress}-{typeof(TEvent).Name.ToLower()}-block-data";
            
            var result = await state.TryGetAsync<BlockData<TEvent>>(key);
            
            if (result.Item1)
            {
                var blockState = result.Item2;
                if (blockState.BlockNumber == blockNumber)
                {
                    if (!blockState.Items.Contains(item))
                    {
                        blockState.Items.Add(item);
                        await state.SetAsync(key, blockState);
                    }
                }
                else
                {
                    await state.SetAsync(key, new BlockData<TEvent>(blockNumber, new List<TEvent> { item }));
                }
            }
            else
            {
                await state.SetAsync(key, new BlockData<TEvent>(blockNumber, new List<TEvent> { item }));
            }
        }
    }
    
    public record WebSessionData(string UserName, ulong UserId);
    
    // public record ProposalBlockData(long BlockNumber, List<ulong> VoteIds);
    public record BlockData<TData>(ulong BlockNumber, List<TData> Items);
}
