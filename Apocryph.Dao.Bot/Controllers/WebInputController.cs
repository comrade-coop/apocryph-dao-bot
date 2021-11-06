using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apocryph.Dao.Bot.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebInputController : ControllerBase
    {
        private readonly ILogger<WebInputController> _logger;

        public WebInputController(ILogger<WebInputController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task Post()
        {
            
        }
    }
}
