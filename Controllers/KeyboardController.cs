using Microsoft.AspNetCore.Mvc;
using DevSim.Interfaces;
using DevSim.Enums;

namespace DevSim.Controllers
{
    [ApiController]
    [Route("Keyboard")]
    public class KeyboardController : ControllerBase
    {
        private readonly ILogger<KeyboardController> _logger;
        
        private readonly IKeyboardMouseInput _key;

        public KeyboardController(ILogger<KeyboardController> logger,
                                         IKeyboardMouseInput key)
        {
            _logger = logger;
            _key = key;
        }


        [HttpPost("Up")]
        public void PostKeyUp(string key)
        {
            _key.SendKeyUp(key);
        }
        [HttpPost("Press")]
        public async Task PostKeyPress(string key)
        {
            _key.SendKeyDown(key);
            await Task.Delay(1);
            _key.SendKeyUp(key);
        }
        [HttpPost("Down")]
        public void PostKeyDown(string key)
        {
            _key.SendKeyDown(key);
        }
    }
}