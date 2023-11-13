using Dapr.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketLibrary
{
    public interface IBasketActor : IActor
    {
        Task AddProductAsync(string productId, int quantity);
        Task<string> GetStatus();
        Task<string> CheckOutAsync();

        Task SetAutoCheckout(bool autoCheckout);
    }
}
