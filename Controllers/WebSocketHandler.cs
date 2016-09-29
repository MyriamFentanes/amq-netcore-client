using System.Net.WebSockets;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebSockets.Server;
using System.Threading.Tasks;
using System;
namespace ws_angular
{
  
public class WebSocketHandler{

    public const int bufferSize= 4096;
    WebSocket webSocket;

    WebSocketHandler(WebSocket websocket)
    {
        this.webSocket = websocket;
    }

    async Task EchoLoop(){
     var buffer = new byte[bufferSize];
     var seg = new ArraySegment<byte>(buffer);
     while (this.webSocket.State == WebSocketState.Open){
            var incoming = await this.webSocket.ReceiveAsync(seg,CancellationToken.None);
            var outgoing = new ArraySegment<byte>(buffer,0,incoming.Count);
            await this.webSocket.SendAsync(outgoing,WebSocketMessageType.Text,true,CancellationToken.None);
    }
   }



    static async Task Acceptor(HttpContext hc,Func<Task>n)
    {
      if (!hc.WebSockets.IsWebSocketRequest)
          return;
      var socket = await hc.WebSockets.AcceptWebSocketAsync();
      var h = new WebSocketHandler(socket);
      await h.EchoLoop();
    }
    public static void Map(IApplicationBuilder app)
    {
      app.UseWebSockets();
      app.Use(WebSocketHandler.Acceptor);
    }
}
}