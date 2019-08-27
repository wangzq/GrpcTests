using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using static GrpcTests.Logger;

namespace GrpcTests
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                await MainInternal(args);
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                Console.ReadLine();
                return 1;
            }
        }

        public static async Task MainInternal(string[] args)
        {
            var host = args.Length > 0 ? args[0] : "localhost";
            var count = args.Length > 1 ? int.Parse(args[1]) : 5;
            var random = new Random();

            var BasePort = int.Parse(ConfigurationManager.AppSettings["BasePort"]);

            var impl = new MyServiceImpl();
            var services = new[]
            {
                MyService.BindService(impl),
            };
            var server = GrpcHelper.StartServer(services, ServerCredentials.Insecure, host, BasePort, count * 2,
                out var port);
            var id = $"{host}:{port}";
            Logger.Id = id;
            Console.Title = id;
            Log($"Listening on {host}:{port}");

            var ports = Enumerable.Range(BasePort, count).Take(count).Where(p => p != port).ToArray();
            var clients = ports.ToDictionary(p => p, p => CreateClient(host, p));
            while (true)
            {
                var tasks = clients.Select(p => TestAsync(id, $"{host}:{p.Key}", p.Value)).ToArray();
                await Task.WhenAll(tasks);
                await Task.Delay(random.Next(5000, 10000));
            }
        }

        private static async Task TestAsync(string from, string endpoint, MyService.MyServiceClient client)
        {
            try
            {
                Log($"Sending Ping to {endpoint}");
                var deadline = DateTime.UtcNow.AddMilliseconds(500);
                await client.PingAsync(new PingRequest { Name = from }, deadline: deadline);
            }
            catch (Exception ex)
            {
                Log($"Failed to Ping {endpoint}: {ex.Message}");
            }

            try
            {
                Log($"Sending Echo to {endpoint}");
                var deadline = DateTime.UtcNow.AddMilliseconds(500);
                var resp = await client.EchoAsync(new EchoRequest { Message = from }, deadline: deadline);
                Log($"Echo response from {endpoint}: {resp.Message}");
            }
            catch (Exception ex)
            {
                Log($"Failed to Echo {endpoint}: {ex.Message}");
            }
        }

        private static MyService.MyServiceClient CreateClient(string host, int port)
        {
            var channel = new Channel($"{host}:{port}", ChannelCredentials.Insecure);
            return new MyService.MyServiceClient(channel);
        }
    }
}
