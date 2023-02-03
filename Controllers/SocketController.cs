using Microsoft.AspNetCore.Mvc;
using DevSim.Interfaces;
using DevSim.Enums;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace DevSim.Controllers
{
    [ApiController]
    [Route("Socket")]
    public class SocketController : ControllerBase
    {
        private readonly IKeyboardMouseInput _key;
        private readonly IGamepadInput _gamepad;
        private readonly Random _rand;
        public SocketController( IGamepadInput gamepad,
                                IKeyboardMouseInput key) {
            _key = key;
            _gamepad = gamepad;
            _rand = new Random();
        }

        [HttpGet]
        public async Task Get(string? token)
        {
            var context = ControllerContext.HttpContext;
            if (context.WebSockets.IsWebSocketRequest)
            {
                int random = _rand.Next();
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Task.Run(async () => {
                    try { while (webSocket.State == WebSocketState.Open) {
                            await this.SendMessage(webSocket,"ping");
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                        }
                    } catch (Exception e) { }
                });
                await Handle(random,webSocket);
            }
        }

        private async Task Handle(int id, WebSocket ws)
        {
            var connectedKeyboard = new List<string>();
            try
            {
                var pinged = true;
                Task.Run(async () => {
                    try { while (ws.State == WebSocketState.Open) {
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                        if (!pinged) {
                            await ws.CloseAsync(WebSocketCloseStatus.Empty,"ping timeout",CancellationToken.None);
                            return;
                        }
                        pinged = false;
                    }} catch{}
                });

                do
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var message = ReceiveMessage(ws, memoryStream).Result;
                        if (message.Count > 0) {
                            var receivedMessage = Encoding.UTF8.GetString(memoryStream.ToArray());

                            var arr = receivedMessage.Split("|");
                            switch (arr[0])
                            {
                                case "mmr":
                                    await _key.ToggleRelativeMouse(true);
                                    _key.SendMouseMove(Single.Parse(arr[1]),Single.Parse(arr[2]));
                                    break;
                                case "mma":
                                    await _key.ToggleRelativeMouse(false);
                                    _key.SendMouseMove(Single.Parse(arr[1]),Single.Parse(arr[2]));
                                    break;
                                case "mw":
                                    _key.SendMouseWheel(Int32.Parse(arr[1]));
                                    break;
                                case "mu":
                                    _key.SendMouseButtonAction((ButtonCode)Int32.Parse(arr[1]),ButtonAction.Up);
                                    break;
                                case "md":
                                    _key.SendMouseButtonAction((ButtonCode)Int32.Parse(arr[1]),ButtonAction.Down);
                                    break;

                                case "kd":
                                    _key.SendKeyDown(arr[1]);
                                    break;
                                case "ku":
                                    _key.SendKeyUp(arr[1]);
                                    break;
                                case "kr":
                                    _key.SetKeyStatesUp();
                                    break;

                                case "gcon":
                                    var gp = $"{id}.{arr[1]}";
                                    _gamepad.Connect(gp, (object sender,Xbox360FeedbackReceivedEventArgs arg) => {
                                        int LargeMotor  = (int)arg.LargeMotor;
                                        int SmallMotor  = (int)arg.SmallMotor;
                                        int LedNumber  = (int)arg.LedNumber;

                                        SendMessage(ws,$"grum|{arr[1]}|{LargeMotor}|{SmallMotor}|{LedNumber}");
                                    });
                                    connectedKeyboard.Add(gp);
                                    break;
                                case "gdis":
                                    var disgp = $"{id}.{arr[1]}";
                                    _gamepad.DisConnect(disgp);
                                    connectedKeyboard.RemoveAll(x => x == disgp);
                                    break;
                                case "gs":
                                    _gamepad.pressSlider($"{id}.{arr[1]}",Int32.Parse(arr[2]),Single.Parse(arr[3]));
                                    break;
                                case "ga":
                                    _gamepad.pressAxis($"{id}.{arr[1]}",Int32.Parse(arr[2]),Single.Parse(arr[3]));
                                    break;
                                case "gb":
                                    _gamepad.pressButton($"{id}.{arr[1]}",Int32.Parse(arr[2]),arr[3] == "1");
                                    break;

                                case "ping":
                                    pinged = true;
                                    break;
                                default:
                                break;
                            }
                        }
                    }
                } while (ws.State == WebSocketState.Open);
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.Message);
            }
            connectedKeyboard.ForEach(x => _gamepad.DisConnect(x));
            Console.WriteLine("Connection closed");
        }

        private async Task<WebSocketReceiveResult> ReceiveMessage(WebSocket ws, Stream memoryStream)
        {
            var readBuffer = new ArraySegment<byte>(new byte[4 * 1024]);
            WebSocketReceiveResult result;

            do
            {
                result = await ws.ReceiveAsync(readBuffer, CancellationToken.None);
                await memoryStream.WriteAsync(readBuffer.Array, readBuffer.Offset, result.Count,
                    CancellationToken.None);
            } while (!result.EndOfMessage);
            return result;
        }

        private async Task SendMessage(WebSocket ws, string msg)
        {
            var bytes = Encoding.UTF8.GetBytes(msg);
            var buffer = new ArraySegment<byte>(bytes);

            try
            {
                await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            } catch { Console.WriteLine("Fail to send websocket to client"); }
        }
    }
}