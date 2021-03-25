using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventSourceAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class sseController : ControllerBase
    {
        private readonly ILogger<sseController> _logger;

        public sseController(ILogger<sseController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task BindAsync()
        {
            HttpContext.Response.Headers["cache-control"] = "no-cache";
            HttpContext.Response.Headers["content-type"] = "text/event-stream";

            int id = 1;

            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var data = JsonSerializer.Serialize(new
                {
                    id,
                    time = now.ToString("HH:mm:ss.fff"),
                    payload = $"anything else (e.g. ${new Random().NextDouble() * 10000:N2})"
                });

                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("event: append");
                stringBuilder.AppendLine($"data: {data}");
                stringBuilder.AppendLine($"id: {id++}\n");
                await HttpContext.Response.WriteAsync(stringBuilder.ToString(), HttpContext.RequestAborted);

                if (HttpContext.Connection.RemoteIpAddress == null)
                    _logger.LogInformation($"Wait for 3 seconds and send data again... connection {HttpContext.Connection.Id}");
                else
                    _logger.LogInformation($"Wait for 3 seconds and send data again... connection {HttpContext.Connection.RemoteIpAddress.MapToIPv4()}:{HttpContext.Connection.RemotePort}");

                System.Threading.Thread.Sleep(3000);
            }

            if (HttpContext.Connection.RemoteIpAddress == null)
                _logger.LogInformation($"Connection {HttpContext.Connection.Id} completed");
            else
                _logger.LogInformation($"Connection {HttpContext.Connection.RemoteIpAddress.MapToIPv4()}:{HttpContext.Connection.RemotePort} completed");

        }
    }
}
