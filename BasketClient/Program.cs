// See https://aka.ms/new-console-template for more information
using BasketLibrary;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;

var http_port = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3501";
var daprClient = new DaprClientBuilder()
    .UseHttpEndpoint($"http://localhost:{http_port}").Build();

while(!await daprClient.CheckHealthAsync(CancellationToken.None))
{
    Console.WriteLine("Waittin for Dapr to ready port:" + http_port);
    await Task.Delay(TimeSpan.FromSeconds(1));
}
Console.WriteLine("Create Actor! 1");
var actor1 = CreateBasketActor("1");
Console.WriteLine("Adding apple");
await actor1.AddProductAsync("apple", 1);
//await actor1.AddProductAsync("apple", 2);
await actor1.AddProductAsync("banana", 2);
var status1 = await actor1.GetStatus();
Console.WriteLine($"Actor 1 status:{status1}");
//var result = await actor1.CheckOutAsync();
//Console.WriteLine(result);
await actor1.SetAutoCheckout(true);
for (int i = 0; i < 10; i++)
{
    await Task.Delay(TimeSpan.FromSeconds(5));
    var output = await actor1.GetStatus();
    Console.WriteLine(output);
}
Console.ReadLine();

IBasketActor CreateBasketActor(string id)
{
    var http_port = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3501";
    var newActor = ActorProxy.Create<IBasketActor>(new ActorId(id), "BasketActor", new ActorProxyOptions()
    {
        HttpEndpoint = $"http://localhost:{http_port}",
    });
    return newActor;
}
//dapr run --app-id basket-client --dapr-http-port 3501 --resources-path .