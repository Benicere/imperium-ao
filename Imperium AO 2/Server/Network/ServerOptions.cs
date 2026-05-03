using System;
namespace ImperiumAO.Server.Network;

public class ServerOptions
{
    public int Port { get; set; } = 7666;
    public int MaxConnections { get; set; } = 500;
}

