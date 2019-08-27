# GrpcTests

For reproducing the issue in https://github.com/grpc/grpc/issues/18461

 * GrpcTests1: using version 1.22.0: reproduced
 * GrpcTests2: using version 2.23.0: reproduced
 * GrpcTests3: using version 1.18.0: pending
 * GrpcTests4: using version 1.19.0: pending
 
After building, copy the binary to Windows Server 2016, make sure you have `procdump.exe` and `pskill.exe` from [Sysinternals](https://docs.microsoft.com/en-us/sysinternals/) (run them once to accept EULA) in your PATH, then run `_RunTests.bat`, which will start 10 instances of the test program to connect to each other peridiocally. The script will assign a random lifetime to each instance between 1 and 180 minutes, when the time is up, it will kill the instance and start a new one. If during the tests any instance crashes with `System.AccessViolationException`, it will create a dump in the `dumps` subfolder created by the script.

In my test, after running for 2~3 hours, it created 2 crashes which has following stack trace:

```
>	grpc_csharp_ext.x64.dll!grpc_core::SockToPolledFdMap::CloseSocket(unsigned __int64 s, void * user_data) Line 822	C++
 	grpc_csharp_ext.x64.dll!ares__close_sockets(ares_channeldata * channel, server_state * server) Line 59	C
 	grpc_csharp_ext.x64.dll!ares__destroy_servers_state(ares_channeldata * channel) Line 106	C
 	grpc_csharp_ext.x64.dll!ares_destroy(ares_channeldata * channel) Line 76	C
 	grpc_csharp_ext.x64.dll!grpc_ares_ev_driver_unref(grpc_ares_ev_driver * ev_driver) Line 112	C++
 	grpc_csharp_ext.x64.dll!grpc_combiner_continue_exec_ctx() Line 269	C++
 	grpc_csharp_ext.x64.dll!grpc_core::ExecCtx::Flush() Line 151	C++
 	grpc_csharp_ext.x64.dll!pollset_work(grpc_pollset * pollset, grpc_pollset_worker * * worker_hdl, __int64 deadline) Line 132	C++
 	grpc_csharp_ext.x64.dll!cq_next(grpc_completion_queue * cq, gpr_timespec deadline, void * reserved) Line 1042	C++
 	grpc_csharp_ext.x64.dll!grpc_completion_queue_next(grpc_completion_queue * cq, gpr_timespec deadline, void * reserved) Line 1118	C++
 	grpc_csharp_ext.x64.dll!grpcsharp_completion_queue_next(grpc_completion_queue * cq) Line 398	C
```
