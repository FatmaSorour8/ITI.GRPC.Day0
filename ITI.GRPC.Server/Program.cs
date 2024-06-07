using ITI.GRPC.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register gRPC services
builder.Services.AddGrpc(); 
builder.Services.AddGrpcReflection();



var app = builder.Build();

// Configure the HTTP request pipeline
app.UseHttpsRedirection();


app.MapGrpcService<ProductTrackingService>();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.Run();
