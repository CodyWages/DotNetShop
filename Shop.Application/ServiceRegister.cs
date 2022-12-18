using Microsoft.Extensions.DependencyInjection;
using Shop.Application.UsersAdmin;
using Shop.Application.OrdersAdmin;
using Shop.Application.Cart;

namespace Microsoft.Extension.DependencyInjection
{
    public static class ServiceRegister
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection @this)
        {
            @this.AddTransient<AddCustomerInformation>();
            @this.AddTransient<AddToCart>();
            @this.AddTransient<GetCart>();
            @this.AddTransient<GetCustomerInformation>();
            @this.AddTransient<Shop.Application.Cart.GetOrder>();
            @this.AddTransient<RemoveFromCart>();

            @this.AddTransient<Shop.Application.OrdersAdmin.GetOrder>();
            @this.AddTransient<GetOrders>();
            @this.AddTransient<UpdateOrder>();

            @this.AddTransient<CreateUser>();

            return @this;
        }
    }
}
