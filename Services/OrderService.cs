using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services.Interfaces;
using ProvaPub.Services.Payment;

namespace ProvaPub.Services
{
	public class OrderService : IOrderService
    {
        TestDbContext _ctx;
        private readonly PaymentStrategyFactory _factory;


        public OrderService(TestDbContext ctx, PaymentStrategyFactory factory)
        {
            _ctx = ctx;
            _factory = factory;

        }

        public async Task<Order> PayOrder(string paymentMethod, decimal paymentValue, int customerId)
		{
            var strategy = _factory.GetStrategy(paymentMethod);
            await strategy.ProcessPayment(paymentValue, customerId);

            var order = new Order
            {
                Value = paymentValue,
                OrderDate = DateTime.UtcNow,
                CustomerId = customerId,
            };

            return await InsertOrder(order);    


        }

		public async Task<Order> InsertOrder(Order order)
        {
            var entity = (await _ctx.Orders.AddAsync(order)).Entity;
            await _ctx.SaveChangesAsync();
            return entity;
        }
	}
}
