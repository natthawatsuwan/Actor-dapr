using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;

namespace BasketLibrary
{
    public interface IPaymentActor : IActor
    {
        Task<int> PayAsync(int amount);
    }

    public class PaymentActor : Actor, IPaymentActor
    {
        public PaymentActor(ActorHost host) : base(host)
        {
        }

        public Task<int> PayAsync(int itemCount)
        {
            Console.WriteLine($"{DateTime.Now} Paying for {itemCount}");

            return Task.FromResult(itemCount * 10);
        }

        public static IPaymentActor Create(string id)
        {
            var http_port = Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500";
            var newActor = ActorProxy.Create<IPaymentActor>(new ActorId(id), "PaymentActor", new ActorProxyOptions()
            {
                HttpEndpoint = $"http://localhost:{http_port}",
            });
            return newActor;
        }
    }
}
