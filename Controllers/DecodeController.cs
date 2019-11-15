using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MareSueEncoder.Models;
using MareSueEncoder.Lib;
using System.Text;

namespace MareSueEncoder.Controllers
{
    [Route("api/[controller]")]
    public class DecodeController : Controller
    {
        private ILogger _logger;
        private const int _decodeMaxLength = 100000;

        public DecodeController(ILogger<DecodeController> logger)
        {
            _logger = logger;
        }

        [HttpPost()]
        public IActionResult Post([FromBody]DecodeParam param)
        {
            if (param == null || string.IsNullOrWhiteSpace(param.Code))
            {
                _logger.LogError("No param in decoding.");
                return BadRequest("no param");
            }

            var code = param.Code.Trim();
            if (code.Length > _decodeMaxLength)
            {
                _logger.LogError("Decoding param too long: {0}", code.Length);
                return BadRequest("param too long");
            }

            var remoteIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {
                var sourceAes = EncodeTool.StringToByteArray(code);
                var source = AESTool.Decrypt(sourceAes);
                var sourceStr = Encoding.UTF8.GetString(source);

                _logger.LogInformation($"(IP:{remoteIP})Decoding successfully.\nCode: {code} \nSource:  {sourceStr}");
                return new JsonResult(new DecodeResult() { Source = sourceStr });
            }
            catch (Exception ex)
            {
                _logger.LogError($"(IP:{remoteIP})Decoding error for string:\n {code} \nError: {ex.Message}");
                return BadRequest("decode error");
            }
        }
    }
}
