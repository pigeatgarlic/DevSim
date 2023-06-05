using Microsoft.AspNetCore.Mvc;
using DevSim.Interfaces;
using DevSim.Enums;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using System.Globalization;
using DevSim.Utilities;

namespace DevSim.Controllers
{
    [ApiController]
    [Route("Socket")]
    public class SocketController : ControllerBase
    {
        private readonly IKeyboardMouseInput _key;
        private readonly IGamepadInput _gamepad;
        private readonly IClipboardService _clipboard;
        private readonly Random _rand;
        public SocketController(IGamepadInput gamepad,
                                IClipboardService clipboard,
                                IKeyboardMouseInput key) {
            _key = key;
            _gamepad = gamepad;
            _clipboard = clipboard;
            _rand = new Random();
        }

        [HttpGet]
        public async Task Get(string? token)
        {
            int id = _rand.Next();
            var connectedGamepad = new List<string>();
            await WS.Handle(HttpContext,async (val,ws) => {
                try { await HandleKey(id,ws,val,connectedGamepad); }
                catch(Exception e) {Console.WriteLine(e.Message);}
            });
            connectedGamepad.ForEach(x => _gamepad.Disconnect(x));
            Console.WriteLine("Connection closed");
        }



        private async Task HandleKey(int id, WebSocket ws, string receivedMessage, List<string> connectedGamepad) {
            var arr = receivedMessage.Split("|");
            switch (arr[0])
            {
            case "mmr":
                await _key.ToggleRelativeMouse(true);
                await _key.SendMouseMove(Single.Parse(arr[1]),Single.Parse(arr[2]));
                break;
            case "mma":
                await _key.ToggleRelativeMouse(false);
                await _key.SendMouseMove(Single.Parse(arr[1]),Single.Parse(arr[2]));
                return;
            case "mw":
                await _key.SendMouseWheel(Int32.Parse(arr[1]));
                return;
            case "mu":
                await _key.SendMouseButtonAction((ButtonCode)Int32.Parse(arr[1]),ButtonAction.Up);
                return;
            case "md":
                await _key.SendMouseButtonAction((ButtonCode)Int32.Parse(arr[1]),ButtonAction.Down);
                return;

            case "kd":
                await _key.SendKeyDown(arr[1]);
                return;
            case "ku":
                await _key.SendKeyUp(arr[1]);
                return;
            case "kr":
                await _key.SetKeyStatesUp();
                return;

            case "cs":
                await _clipboard.Set(Base64.Base64Decode(arr[1]));
                return;
            case "cp":
                await _clipboard.Paste();
                return;
            case "ping":
                return;
            }

            if (this._gamepad.failed) 
                return;
            


            switch (arr[0])
            {
            case "gcon":
                var gp = $"{id}.{arr[1]}";
                _gamepad.Connect(gp, (object sender,Xbox360FeedbackReceivedEventArgs arg) => {
                    int LargeMotor  = (int)arg.LargeMotor;
                    int SmallMotor  = (int)arg.SmallMotor;
                    int LedNumber  = (int)arg.LedNumber;
                    WS.SendMessage(ws,$"grum|{arr[1]}|{LargeMotor}|{SmallMotor}|{LedNumber}");
                });
                connectedGamepad.Add(gp);
                return;
            case "gdis":
                var disgp = $"{id}.{arr[1]}";
                _gamepad.Disconnect(disgp);
                connectedGamepad.RemoveAll(x => x == disgp);
                return;
            case "gs":
                await _gamepad.pressSlider($"{id}.{arr[1]}",Int32.Parse(arr[2], CultureInfo.InvariantCulture),Single.Parse(arr[3], CultureInfo.InvariantCulture));
                return;
            case "ga":
                await _gamepad.pressAxis($"{id}.{arr[1]}",Int32.Parse(arr[2]),Single.Parse(arr[3], CultureInfo.InvariantCulture));
                return;
            case "gb":
                await _gamepad.pressButton($"{id}.{arr[1]}",Int32.Parse(arr[2]),arr[3] == "1");
                return;
            }
        }
    }
}