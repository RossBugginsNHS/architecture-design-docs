using RabbitMQ.Stream.Client;
using System.Net;

public static class SuperDefaults
{
    public static StreamSystemConfig DefaultConfig()
    {
        var config = new StreamSystemConfig
        {
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/",
            Endpoints = new List<EndPoint>() { new IPEndPoint(IPAddress.Loopback, 5552) }
        };
        return config;
    }
}
