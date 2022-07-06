# grpcLearning

gRPC for .Net 6.0 sample created using link https://docs.microsoft.com/en-us/aspnet/core/tutorials/grpc/grpc-start?view=aspnetcore-6.0&tabs=visual-studio

Server project 'GrpcGreeter' created with template ''ASP.NET Core gRPC Service'' with .Net 6.0

Client project created with console app and added below nuget packages
Install-Package Grpc.Net.Client
Install-Package Google.Protobuf
Install-Package Grpc.Tools

The GrpcGreeterClient types are generated automatically by the build process.
GrpcGreeterClient\obj\Debug\[TARGET_FRAMEWORK]\Protos\Greet.cs: The protocol buffer code which populates, serializes and retrieves the request and response message types.
GrpcGreeterClient\obj\Debug\[TARGET_FRAMEWORK]\Protos\GreetGrpc.cs: Contains the the generated client classes.

How to run
In the Greeter service, press Ctrl+F5 to start the server without the debugger.
In the GrpcGreeterClient project, press Ctrl+F5 to start the client without the debugger.