using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pi.Dns.Common.Authorization;
using Pi.Dns.Common.Models;
using Pi.Dns.Server.Api.Settings;
using Pi.Dns.Server.Api.Utilities;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pi.Dns.Server.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UnboundControlStatsController : ControllerBase
    {
        // Settings
        private readonly BashSettings _bashSettings;

        // Dependency injection
        private readonly IBashUtil _bashUtil;
        private readonly ILogger _logger;

        public UnboundControlStatsController(
            IOptions<BashSettings> bashSettings,
            IBashUtil bashUtil)
        {
            _bashSettings = bashSettings.Value;
            _bashUtil = bashUtil;

            _logger = Log.ForContext("SourceContext", nameof(UnboundControlStatsController));
        }

        [HttpGet]
        [ApiKeyAuthorization]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var unboundControlStatsOutput = await _bashUtil.ExecuteBash(_bashSettings.UnboundControlCmd);
                var domainsOnBlocklistOutput = await _bashUtil.ExecuteBash(_bashSettings.DomainsOnBlocklistCmd);
                var result = unboundControlStatsOutput.Concat(domainsOnBlocklistOutput);
                var unboundControlStats = new UnboundControlStats(result);

                return new OkObjectResult(unboundControlStats);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Got an exception while executing {Controller}", nameof(UnboundControlStatsController));
                return new StatusCodeResult(500); // Always respond with HTTP status code 500 for now
            }
        }
    }
}
