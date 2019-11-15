using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MareSueEncoder.Models;
using System.Text;
using MareSueEncoder.Lib;
using Microsoft.Extensions.Logging;

namespace MareSueEncoder.Controllers
{
    [Route("api/[controller]")]
    public class EncodeController : Controller
    {
        private ILogger _logger;
        private const int _encodeMaxLength = 20000;

        public EncodeController(ILogger<EncodeController> logger)
        {
            _logger = logger;
        }

        [HttpPost()]
        public IActionResult Post([FromBody]EncodeParam param)
        {
            _logger.LogDebug("Start Encoding.");
            if (param == null || string.IsNullOrWhiteSpace(param.Source))
            {
                _logger.LogError("No param in encoding.");
                return BadRequest("no param");
            }

            var source = param.Source.Trim();
            if (source.Length> _encodeMaxLength)
            {
                _logger.LogError($"Encoding param too long: {source.Length}");
                return BadRequest("param too long");
            }

            var remoteIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            try
            {
                var sourceAes = AESTool.Encrypt(Encoding.UTF8.GetBytes(source));
                var code = EncodeTool.ByteArrayToString(sourceAes);

                //var testaes = EncodeTool.StringToByteArray(code);
                //if (testaes.Length != sourceAes.Length)
                //{
                //    for (var i = 0; i < testaes.Length; i++)
                //    {
                //        if (testaes[i] != sourceAes[i])
                //        {
                //            _logger.LogError($"Encoding error in {i} of {sourceAes.Length}");
                //            break;
                //        }
                //    }
                //}

                _logger.LogInformation($"(IP:{remoteIP})Encoding successfully.\nSource:  {source} \nCode: {code}");
                return new JsonResult(new EncodeResult() { Code = code });
            }
            catch( Exception ex)
            {
                _logger.LogError($"(IP:{remoteIP})Encoding error for string: \n{source} \nError:{ex.Message}");
                return BadRequest("encode error");
            }
        }
    }
}
