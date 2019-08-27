pskill GrpcTests
pskill PowerShell

copy \\tsclient\d\lab\GrpcTests\GrpcTests1\bin\debug\net462\GrpcTests*.* %~dp0..\1.22.0\
copy \\tsclient\d\lab\GrpcTests\GrpcTests1\bin\debug\net462\*.bat %~dp0..\1.22.0\
copy \\tsclient\d\lab\GrpcTests\GrpcTests1\bin\debug\net462\*.ps1 %~dp0..\1.22.0\

copy \\tsclient\d\lab\GrpcTests\GrpcTests2\bin\debug\net462\GrpcTests*.* %~dp0..\2.23.0\
copy \\tsclient\d\lab\GrpcTests\GrpcTests2\bin\debug\net462\*.bat %~dp0..\2.23.0\
copy \\tsclient\d\lab\GrpcTests\GrpcTests2\bin\debug\net462\*.ps1 %~dp0..\2.23.0\

pause
