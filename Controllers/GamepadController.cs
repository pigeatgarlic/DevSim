using Microsoft.AspNetCore.Mvc;
using DevSim.Interfaces;
using DevSim.Enums;

namespace DevSim.Controllers
{
    [ApiController]
    [Route("GamePad")]
    public class GamepadController : ControllerBase
    {
        private readonly ILogger<MouseController> _logger;
        
        private readonly IGamepadInput _key;

        public GamepadController(ILogger<MouseController> logger,
                               IGamepadInput key)
        {
            _logger = logger;
            _key = key;
        }


        [HttpPost("Slider")]
        public void Slider(int index, float val)
        {
            _key.pressSlider(index,val);
        }
        [HttpPost("Axis")]
        public void Axis(int index, float val)
        {
            _key.pressAxis(index,val);
        }
        [HttpPost("Button")]
        public void Button(int index, bool val)
        {
            _key.pressButton(index,val);
        }

        [HttpPost("Status")]
        public void Status(bool val)
        {
            if (val == true) {
                _key.Connect();
            } else {
                _key.DisConnect();
            }
        }

        [HttpGet("Status")]
        public bool GetStatus()
        {
            return _key.Status();
        }
        [HttpGet("Feedback")]
        public IActionResult getFeedback()
        {
            var fb = _key.getFeedback();
            if (fb == null) {
                return NotFound();
            }

            var dict = new Dictionary<string,string>();
            dict["largeMotor"] = fb.LargeMotor.ToString();
            dict["smallMotor"] = fb.SmallMotor.ToString();
            dict["ledNumber"] = fb.LedNumber.ToString();
            return Ok(dict); 
        }
    }
}