using WebSocket;



public partial class Program
{

    static WebSocketServer ws = new WebSocketServer("http://localhost:3000/");

    public static void Main(string[] args)
    {
        Thread listen_thread = new Thread(ListenThread);

        listen_thread.Start();

        Thread broadcast_thread = new Thread(BroadcastThread);

        broadcast_thread.Start();

        Console.WriteLine("done");
    }

    public static async void BroadcastThread()
    {
        while (true)
        {
            Console.WriteLine("broadcasting");
            System.Threading.Thread.Sleep(10000);
            await ws.BroadcastAsync("deeeez nuts");
        }
    }

    public static async void ListenThread()
    {
        Console.WriteLine("listening");
        await ws.StartAsync();
    }
}

