using System.Net.Sockets;
using System.Net;
using System.Text;

Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 2345);
try
{
    socket.Bind(endpoint);
    socket.Listen();
}
catch
{
    System.Console.WriteLine("Impossible de démarrer le serveur");
    Environment.Exit(-1);
}
try
{
    var clientSocket = socket.Accept();
    if (clientSocket.RemoteEndPoint is not null)
    {
        System.Console.WriteLine("Client connecté depuis " + clientSocket.RemoteEndPoint.ToString());
        while (true)
        {
            byte[] buffer = new byte[128];
            int nb = clientSocket.Receive(buffer);
            System.Console.WriteLine("MESSAGE REÇU :");
            System.Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, nb));
        }

    }
}
catch
{
    System.Console.WriteLine("Communication impossible avec le client");
}
finally
{
    if (socket.Connected)
    {
        socket.Shutdown(SocketShutdown.Both);
    }
    socket.Close();
}