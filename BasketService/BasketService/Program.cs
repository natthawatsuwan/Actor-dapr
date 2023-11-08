using BasketLibrary;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddActors(options =>
{
    var dapr_http_port = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT")?? "3500";
    options.HttpEndpoint = $"http://localhost:{dapr_http_port}";
    options.Actors.RegisterActor<BasketActor>();
    options.Actors.RegisterActor<PaymentActor>();

});
var app = builder.Build();

app.MapActorsHandlers();
app.MapGet("/", () => "Hello World!");

app.Run();

//dapr run --app-id actor-service --dapr-http-port 3500 --app-port 5006