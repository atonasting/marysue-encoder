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
            try
            {
                var sourceAes = AESTool.Encrypt(Encoding.UTF8.GetBytes(source));
                var code = EncodeTool.ByteArrayToString(sourceAes);

                var testaes = EncodeTool.StringToByteArray(code);
                if (testaes.Length != sourceAes.Length)
                {
                    for (var i = 0; i < testaes.Length; i++)
                    {
                        if (testaes[i] != sourceAes[i])
                        {
                            _logger.LogError("Encoding error in {0} of {1}", i, sourceAes.Length);
                            break;
                        }
                    }
                }

                _logger.LogInformation("Encoding successfully.\nSource:  {0} \nCode: {1}", source, code);
                return new JsonResult(new EncodeResult() { Code = code });
            }
            catch( Exception ex)
            {
                _logger.LogError("Encoding error for string: \n{0} \nError:{1}", source, ex.Message);
                return BadRequest("encode error");
            }
        }
    }
}
