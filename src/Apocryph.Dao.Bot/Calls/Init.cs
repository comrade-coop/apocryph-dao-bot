using Perper.Model;
using System;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Calls
{
    public class Init
    {
        private readonly IContext _context;
        public Init(IContext context)
        {
            _context = context;
        }
 
        public async Task RunAsync()
        {
             await _context.CallActionAsync(nameof(BlockchainEvents), Array.Empty<object>()).ConfigureAwait(false);
             await _context.CallActionAsync(nameof(UserInteractions), Array.Empty<object>()).ConfigureAwait(false);
        }
    }
}