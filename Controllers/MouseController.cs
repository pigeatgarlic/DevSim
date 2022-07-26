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
        public void PostMouseMove(int X, int Y)
        {
            _key.SendMouseMove((double)X,(double)Y);
        }

        [HttpPost("Up")]
        public void PostMouseUp(int X, int Y,ButtonCode button)
        {
            _key.SendMouseButtonAction(button,ButtonAction.Up,(double)X,(double)Y);
        }
        [HttpPost("Down")]
        public void PostMouseDown(int X, int Y,ButtonCode button)
        {
            _key.SendMouseButtonAction(button,ButtonAction.Down,(double)X,(double)Y);
        }
    }
}