using Dapr.Actors;
using Dapr.Actors.Runtime;
using System.Text.Json;

namespace BasketLibrary
{
    public interface IBasketActor : IActor
    {
        Task AddProductAsync(string productId,int  quantity);
        Task<string> GetStatus();
        Task<string> CheckOutAsync();
        Task SetAutoCheckout(bool autoCheckout);
    }

    public class BasketActor : Actor, IBasketActor, IRemindable
    {
        public BasketActor(ActorHost host) : base(host)
        {
        }

        public async Task AddProductAsync(string productId, int quantity)
        {
           var basket = await StateManager.TryGetStateAsync<Dictionary<string,int>> ("Basket");
            if(basket.HasValue)
            {
                if(basket.Value.ContainsKey(productId))
                {
                    basket.Value[productId] += quantity;
                }
                else
                {
                    basket.Value[productId] = quantity;
                }
                await StateManager.SetStateAsync("Basket", basket.Value);
            }
            else
            {
                var newBasket = new Dictionary<string, int>()
                {
                    {productId, quantity}
                };
                await StateManager.SetStateAsync("Basket", newBasket);
            }
        }

        public async Task<string> CheckOutAsync()
        {
            var basket = await StateManager.TryGetStateAsync<Dictionary<string, int>>("Basket");
            if (!basket.HasValue || basket.Value.Count == 0)
            {
                return "Basket is empty";
            }

            var paymentActor = PaymentActor.Create(Id.GetId());
            var total = await paymentActor.PayAsync(basket.Value.Values.Sum());
            await StateManager.SetStateAsync("Basket", new Dictionary<string, int>());
            var output =   $"Checkount successfully for {total} baht.";
            Console.WriteLine(output);
            return output;
        }

        public async Task<string> GetStatus()
        {
            var basket = await StateManager.TryGetStateAsync<Dictionary<string, int>>("Basket");
            if(!basket.HasValue || basket.Value.Count == 0)
            {
                return "Basket is empty";
            }
            else
            {
                return $"{DateTime.Now} Current basket status: {JsonSerializer.Serialize(basket.Value)}";
            }
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if(reminderName == "AutoCheckoutReminder")
            {
                await SetAutoCheckout(false);
                await CheckOutAsync();
            }
        }

        public async Task SetAutoCheckout(bool autoCheckout)
        {
            if (autoCheckout)
            {
                await RegisterReminderAsync("AutoCheckoutReminder", null, TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(0));
            }
            else
            {
                await UnregisterReminderAsync("AutoCheckoutReminder");
            }
        }
    }
}
