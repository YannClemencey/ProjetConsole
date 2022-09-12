using System.Net;
using System.Net.Sockets;
using System.Text;

var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
var endpoint = new IPEndPoint(IPAddress.Loopback, 2345);

try
{
    socket.Connect(endpoint);

    Thread t = new Thread(LireMessages);
    t.IsBackground = true;
    t.Start(socket);

    while (true)
    {
        string? message = Console.ReadLine();
        if (message == "q")
        {
            break;
        }
        if (!string.IsNullOrEmpty(message))
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            socket.Send(buffer);
        }

    }
}
catch
{
    System.Console.WriteLine("Le serveur est injoignable");
}
finally
{
    if (socket.Connected)
    {
        socket.Shutdown(SocketShutdown.Both);

    }

    socket.Close();
}
void LireMessages(object? obj)
{
        if(obj is Socket socket)
    {
        while (true)
        {
            byte[] buffer = new byte[4096];
            int read = socket.Receive(buffer);
            if (read > 0)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, read);
                Console.WriteLine(message);
            }
        }
    }
}
