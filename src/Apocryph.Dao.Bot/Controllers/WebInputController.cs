using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;
using System.Threading.Tasks;
using Apocryph.Dao.Bot.Message;
using Serilog;

namespace Apocryph.Dao.Bot.Controllers
{
    [ApiController]
    [Route("api/webinput")]
    [Produces("application/json")]
    public class WebInputController : Controller
    {
        private readonly Channel<IWebInboundMessage> _channel;

        public WebInputController(Channel<IWebInboundMessage> channel)
        {
            _channel = channel;
        }

        [HttpPost("intro-attempt")]
        public async Task<IActionResult> Post([FromBody] IntroAttemptMessage input) => await HandleWebInboundMessage(input);
 
        private async Task<IActionResult> HandleWebInboundMessage(IWebInboundMessage input)
        {
            var sessionLog = Log.ForContext("Session", input.Session);
            
            await _channel.Writer.WriteAsync(input);
            
            sessionLog.Information("Accepted request {@Input}", input);
            
            return Accepted(string.Empty);
        }
    }
}
