using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perper.Model;

namespace Apocryph.Dao.Bot.Core
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
    }
}
