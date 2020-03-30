using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bischino.Base.Service;
using Bischino.Bischino;
using Bischino.Controllers.Extensions;
using Bischino.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Bischino.Controllers
{
    public class WebSocketHandler
    {
        private readonly RequestDelegate _next;
        public static event EventHandler<ConnectedSocketEventArgs> Connected;

        public WebSocketHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            var socketFinishedTcs = new TaskCompletionSource<object>();
            var client = await context.WebSockets.AcceptWebSocketAsync();

            Connected?.Invoke(this, new ConnectedSocketEventArgs(client, socketFinishedTcs));
            await socketFinishedTcs.Task;
        }
    }

    public class ConnectedSocketEventArgs
    {
        public WebSocket Socket { get; }
        public TaskCompletionSource<object> TaskCompletionSource { get; }

        public ConnectedSocketEventArgs(WebSocket socket, TaskCompletionSource<object> taskCompletionSource)
        {
            (Socket, TaskCompletionSource) = (socket, taskCompletionSource);
        }
    }
}
