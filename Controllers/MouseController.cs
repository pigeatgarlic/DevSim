using Microsoft.AspNetCore.Mvc;
using DevSim.Interfaces;
using DevSim.Enums;

namespace DevSim.Controllers
{
    [ApiController]
    [Route("Mouse")]
    public class MouseController : ControllerBase
    {
        private readonly ILogger<MouseController> _logger;
        
        private readonly IKeyboardMouseInput _key;

        public MouseController(ILogger<MouseController> logger,
                               IKeyboardMouseInput key)
        {
            _logger = logger;
            _key = key;
        }

        [HttpPost("Move")]
        public void PostMouseMove([FromBody]Dictionary<string,float> data)
        {
            _key.SendMouseMove(data["X"],data["Y"]);
        }
        [HttpPost("Wheel")]
        public void PostMouseWheel([FromBody]int deltaY)
        {
            _key.SendMouseWheel(deltaY);
        }

        [HttpPost("Up")]
        public void PostMouseUp([FromBody]ButtonCode button)
        {
            _key.SendMouseButtonAction(button,ButtonAction.Up);
        }
        [HttpPost("Down")]
        public void PostMouseDown([FromBody]ButtonCode button)
        {
            _key.SendMouseButtonAction(button,ButtonAction.Down);
        }
    }
}