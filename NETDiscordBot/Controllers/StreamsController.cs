namespace NETDiscordBot.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Services.DataAccess;

    [ApiController]
    [Route("[controller]")]
    public class StreamsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IStreamDataAccessService _streamDataAccessService;

        public StreamsController(
            IConfiguration configuration,
            IStreamDataAccessService streamDataAccessService)
        {
            this._configuration = configuration;
            this._streamDataAccessService = streamDataAccessService;
        }
        
        [HttpGet]
        public async Task<IActionResult> FetchStreams(
            [FromQuery]string userName = null)
        {
            return Ok(await this._streamDataAccessService.FetchStreams(userName));
        }
    }
}