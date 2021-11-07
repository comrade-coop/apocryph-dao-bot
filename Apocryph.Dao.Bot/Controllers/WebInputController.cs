using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebInputController : ControllerBase
    {
        private readonly ILogger<WebInputController> _logger;
        private readonly Channel<(string, string)> _channel;

        public WebInputController(ILogger<WebInputController> logger, Channel<(string, string)> channel)
        {
            _logger = logger;
            _channel = channel;
        }

        [HttpPost]
        public async Task<StatusCodeResult> Post([FromQuery] string session, [FromBody] string message)
        {
            await _channel.Writer.WriteAsync((session, message));
            return StatusCode(StatusCodes.Status202Accepted);
        }
    }
}
