using System;
using System.Net.Sockets;

namespace ChatServer
{
    public class Client
    {
        public Socket Socket { get; set; }
        public string Nom { get; set; }
        public Guid Id { get; set; }
    }
}