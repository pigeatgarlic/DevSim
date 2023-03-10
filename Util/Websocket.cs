using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DevSim.Utilities
{
    public class WS {

        static async Task<WebSocketReceiveResult> ReceiveMessage(WebSocket ws, Stream memoryStream)
        {
            var readBuffer = new ArraySegment<byte>(new byte[4 * 1024]);
            WebSocketReceiveResult result;

            do {
                result = await ws.ReceiveAsync(readBuffer, CancellationToken.None);
                await memoryStream.WriteAsync(readBuffer.Array, readBuffer.Offset, result.Count, CancellationToken.None);
            } while (!result.EndOfMessage);
            return result;
        }

        public static async Task SendMessage(WebSocket ws, string msg)
        {
            var bytes = Encoding.UTF8.GetBytes(msg);
            var buffer = new ArraySegment<byte>(bytes);

            try {
                await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            } catch { 
                Console.WriteLine("Fail to send websocket to client"); 
                await ws.CloseAsync(WebSocketCloseStatus.Empty,"ping timeout",CancellationToken.None);
            }
        }

        public static async Task Handle<T>(HttpContext context,Action<T,WebSocket> action)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var ws = await context.WebSockets.AcceptWebSocketAsync();
            
            try {
                do
                {
                    var memoryStream = new MemoryStream();
                    var message = await WS.ReceiveMessage(ws, memoryStream);
                    if (message.Count == 0) 
                        continue;

                    var receivedMessage = Encoding.UTF8.GetString(memoryStream.ToArray());
                    var msg = JsonConvert.DeserializeObject<T>(receivedMessage);
                    action(msg);
                } while (ws.State == WebSocketState.Open);
            } catch (Exception e) {
            }
        }
    }
}