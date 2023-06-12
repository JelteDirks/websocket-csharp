using System.Net;
using System.Net.WebSockets;

namespace WebSocket
{
    public class WebSocketServer
    {
        private HttpListener http_listener;
        private List<Client> clients;

        public WebSocketServer(string uriPrefix)
        {
            http_listener = new HttpListener();
            http_listener.Prefixes.Add(uriPrefix);
            clients = new List<Client>();
        }

        public async Task BroadcastAsync(string message)
        {
            var buffer = System.Text.Encoding.UTF8.GetBytes(message);
            foreach (var client in clients)
            {
                if (client.WebSocket.State == WebSocketState.Open)
                {
                    await client.WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public async Task StartAsync()
        {
            http_listener.Start();
            while (true)
            {
                var context = await http_listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    // Add your own logic here before accepting the connection
                    var wsContext = await context.AcceptWebSocketAsync(null);
                    var webSocket = wsContext.WebSocket;
                    var client = new Client(webSocket);
                    clients.Add(client);
                    _ = HandleConnectionAsync(webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private async Task HandleConnectionAsync(System.Net.WebSockets.WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var client = clients.Find(c => c.WebSocket == webSocket);

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    break;
                }
                else
                {
                    if (client!.Validated == false)
                    {
                        Console.WriteLine("validating client");
                        client.Validated = true;
                        // Perform session ID validation here
                        // ...
                        Console.WriteLine("client validated");
                    }

                    await webSocket.SendAsync(new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes("ooh that tickles")),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }

            Console.WriteLine("client removed");

            clients.Remove(client!);
        }
    }

    public class Client
    {
        public System.Net.WebSockets.WebSocket WebSocket;
        public bool Validated;

        public Client(System.Net.WebSockets.WebSocket ws)
        {
            WebSocket = ws;
            Validated = false;
        }
    }

}
