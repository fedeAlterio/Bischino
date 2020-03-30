using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Bischino.Controllers.Extensions
{
    public static class WebSocketExtensions
    {
        public static async Task<T> GetAsync<T>(this WebSocket me, CancellationToken cancellationToken = default)
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            string json = null;
            await using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    result = await me.ReceiveAsync(buffer, cancellationToken);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);

                if (result.MessageType != WebSocketMessageType.Text)
                    throw new Exception("Message type is not text");

                // Encoding UTF8: https://tools.ietf.org/html/rfc6455#section-5.6
                using var reader = new StreamReader(ms, Encoding.UTF8);
                json = await reader.ReadToEndAsync();
            }

            var ret = JsonConvert.DeserializeObject<T>(json);
            return ret;
        }

        public static async Task SendAsync<T>(this WebSocket me, T obj, CancellationToken cancellationToken = default)
        {
            var jsonString = JsonConvert.SerializeObject(obj);
            var bytes = Encoding.UTF8.GetBytes(jsonString);
            var array = new ArraySegment<byte>(bytes);
            await me.SendAsync(array, WebSocketMessageType.Text, true, cancellationToken);
        }

    }
}
