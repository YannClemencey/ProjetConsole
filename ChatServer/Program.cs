using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.CompilerServices;
using ChatServer;

Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 2345);

List<Client> clients = new List<Client>();

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
    while (true)
    {
        var clientSocket = socket.Accept();
        if (clientSocket.RemoteEndPoint is not null)
        {
            System.Console.WriteLine("Client connecté depuis " + clientSocket.RemoteEndPoint.ToString());
            var client = new Client
            {
                Socket = clientSocket,
                Id = Guid.NewGuid()
            };
            clients.Add(client);
            Thread t = new Thread(EcouterClient);
            t.IsBackground = true;
            t.Start(client);

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

void EcouterClient(object? obj)
{
    if (obj is Client client)
    {
        try
        {
            while (string.IsNullOrWhiteSpace(client.Nom))
            {
                var message = "Veuillez saisir votre nom";
                byte[] buff = Encoding.UTF8.GetBytes(message);
                client.Socket.Send(buff);
                byte[] nomBuff = new byte[128];
                int read = client.Socket.Receive(nomBuff);
                client.Nom = Encoding.UTF8.GetString(nomBuff, 0, read);
            }
            while (true)
            {
                byte[] buffer = new byte[4096];
                int nb = client.Socket.Receive(buffer);
                var message = Encoding.UTF8.GetString(buffer, 0, nb);
                System.Console.WriteLine($"MESSAGE REÇU de {client.Nom} : {message}");

                byte[] sendBuffer = new byte[8192];
                sendBuffer = Encoding.UTF8.GetBytes($"{client.Nom} : {message}");
                foreach (var c in clients)
                {
                    try
                    {
                        if (c.Id != client.Id)
                        {
                            c.Socket.Send(sendBuffer);
                        }
                    }
                    catch
                    {
                        System.Console.WriteLine($"Impossible d'envoyer un message à {c.Nom}");
                    }

                }
            }
        }
        catch
        {
            System.Console.WriteLine($"Le client {client.Nom} s'est déconnecté");
        }
        finally
        {
            if (client.Socket.Connected)
            {
                client.Socket.Shutdown(SocketShutdown.Both);
            }
            client.Socket.Close();
            clients.Remove(client);
        }
    }
}