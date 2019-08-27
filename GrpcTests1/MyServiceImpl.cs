using System;
using System.Threading.Tasks;
using Grpc.Core;
using static GrpcTests.Logger;

namespace GrpcTests
{
    internal class MyServiceImpl : MyService.MyServiceBase
    {
        public override Task<EchoResponse> Echo(EchoRequest request, ServerCallContext context)
        {
            Log($"Echo: {request.Message}");
            return Task.FromResult(new EchoResponse { Message = Logger.Id + request.Message });
        }

        private static readonly Task<PingResponse> EmptyPingResponse = Task.FromResult(new PingResponse());
        public override Task<PingResponse> Ping(PingRequest request, ServerCallContext context)
        {
            Log($"Ping: {request.Name}");
            return EmptyPingResponse;
        }
    }
}
