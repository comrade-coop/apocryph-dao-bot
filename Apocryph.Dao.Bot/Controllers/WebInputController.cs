using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Inputs;

namespace Apocryph.Dao.Bot.Controllers
{
    [ApiController]
    [Route("api/webinput")]
    [Produces("application/json")]
    public class WebInputController : Controller
    {
        private readonly Channel<(string, string)> _channel;

        public WebInputController(Channel<(string, string)> channel)
        {
            _channel = channel;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WebInput input)
        {
            await _channel.Writer.WriteAsync((input.Session, input.Message));
            
            return Accepted(string.Empty);
        }
    }
}
