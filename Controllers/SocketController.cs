using Microsoft.AspNetCore.Mvc;
using DevSim.Interfaces;
using DevSim.Enums;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
<<<<<<< HEAD
using System.Globalization;
=======
using DevSim.Utilities;
>>>>>>> 758c2f3bd29e0c61162a9eeb12b0d173394aeb6e

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
            await WS.Handle<string>(HttpContext,(val,ws) => {
                HandleKey(id,ws,val,connectedGamepad).Start();
            });
        }



        private async Task HandleKey(int id, WebSocket ws, string receivedMessage, List<string> connectedGamepad) {
            var arr = receivedMessage.Split("|");
            try {
                switch (arr[0])
                {
                case "mmr":
                    await _key.ToggleRelativeMouse(true);
                    await _key.SendMouseMove(Single.Parse(arr[1]),Single.Parse(arr[2]));
                    break;
                case "mma":
                    await _key.ToggleRelativeMouse(false);
                    await _key.SendMouseMove(Single.Parse(arr[1]),Single.Parse(arr[2]));
                    break;
                case "mw":
                    await _key.SendMouseWheel(Int32.Parse(arr[1]));
                    break;
                case "mu":
                    await _key.SendMouseButtonAction((ButtonCode)Int32.Parse(arr[1]),ButtonAction.Up);
                    break;
                case "md":
                    await _key.SendMouseButtonAction((ButtonCode)Int32.Parse(arr[1]),ButtonAction.Down);
                    break;

                case "kd":
                    await _key.SendKeyDown(arr[1]);
                    break;
                case "ku":
                    await _key.SendKeyUp(arr[1]);
                    break;
                case "kr":
                    await _key.SetKeyStatesUp();
                    break;

                case "cs":
                    await _clipboard.Set(Base64.Base64Decode(arr[1]));
                    break;
                case "cp":
                    await _clipboard.Paste();
                    break;

                case "ping":
                    await WS.SendMessage(ws,"ping");
                    break;
                }

                if (this._gamepad.failed) 
                    return;
                


                switch (arr[0])
                {
                case "gcon":
                    var gp = $"{id}.{arr[1]}";
                    _gamepad.Connect(gp, async (object sender,Xbox360FeedbackReceivedEventArgs arg) => {
                        int LargeMotor  = (int)arg.LargeMotor;
                        int SmallMotor  = (int)arg.SmallMotor;
                        int LedNumber  = (int)arg.LedNumber;
                        await WS.SendMessage(ws,$"grum|{arr[1]}|{LargeMotor}|{SmallMotor}|{LedNumber}");
                    });
                    connectedGamepad.Add(gp);
                    break;
                case "gdis":
                    var disgp = $"{id}.{arr[1]}";
                    _gamepad.DisConnect(disgp);
                    connectedGamepad.RemoveAll(x => x == disgp);
                    break;
                case "gs":
                    await _gamepad.pressSlider($"{id}.{arr[1]}",Int32.Parse(arr[2], CultureInfo.InvariantCulture),Single.Parse(arr[3], CultureInfo.InvariantCulture));
                    break;
                case "ga":
                    await _gamepad.pressAxis($"{id}.{arr[1]}",Int32.Parse(arr[2]),Single.Parse(arr[3], CultureInfo.InvariantCulture));
                    break;
                case "gb":
                    await _gamepad.pressButton($"{id}.{arr[1]}",Int32.Parse(arr[2]),arr[3] == "1");
                    break;

                default:
                break;
                }
            } catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}