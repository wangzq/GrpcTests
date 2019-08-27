using System;
using System.Collections.Generic;
using System.IO;
using Grpc.Core;
using Grpc.Core.Logging;

namespace GrpcTests
{
    public static class GrpcHelper
    {
        public static Server StartServer(IList<ServerServiceDefinition> services, ServerCredentials serverCredentials, string host, int basePort, int count, out int port)
        {
            UseConsoleVerboseLogger();

            var options = new List<ChannelOption>
            {
                new ChannelOption(ChannelOptions.MaxSendMessageLength, int.MaxValue),
                new ChannelOption(ChannelOptions.MaxReceiveMessageLength, int.MaxValue),
            };

            for (var i = 0; i < count; i++)
            {
                var server = new Server(options);
                foreach (var service in services)
                {
                    server.Services.Add(service);
                }

                server.Ports.Add(host, basePort + i, serverCredentials);
                try
                {
                    server.Start();
                    port = basePort + i;
                    return server;
                }
                catch (IOException ex) when (i < count - 1 &&
                                             ex.Message.IndexOf("Failed to bind port",
                                                 StringComparison.OrdinalIgnoreCase) >= 0)
                {
                }
            }

            throw new InvalidOperationException($"Failed to start server at {host} using port range [{basePort}-{basePort+count})");
        }

        public static void UseConsoleVerboseLogger()
        {
            var logger = new LogLevelFilterLogger(new ConsoleLogger(), LogLevel.Debug, false);
            GrpcEnvironment.SetLogger(logger);
        }
    }
}
