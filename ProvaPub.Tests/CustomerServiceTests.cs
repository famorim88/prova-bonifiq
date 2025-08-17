using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;
using ProvaPub.Validators;
using Xunit;
using static ProvaPub.Tests.DbContextHelper;

namespace ProvaPub.Tests
{
    public class CustomerServiceTests
    {
        private CustomerService BuildService(TestDbContext ctx, DateTime dateTime)
        {
            var dateTimeProvider = new FixedDateTimeProvider(dateTime);
            var rules = new IPurchaseRule[]
            {
                new CustomerExistsRule(ctx),
                new OnePurchasePerMonthRule(ctx, dateTimeProvider),
                new FirstPurchaseMaxValueRule(ctx),
                new BusinessHoursRule(dateTimeProvider)
            };

            return new CustomerService(ctx, rules);
        }

        private void purgeCustomers(TestDbContext context )
        {
            context.Customers.RemoveRange(context.Customers);
            context.SaveChanges();
        }

        [Fact]
        public async Task CanPurchase_ShouldThrow_WhenCustomerDoesNotExist()
        {
            var ctx = CreateInMemoryDb();
            purgeCustomers(ctx);
            var service = BuildService(ctx, new DateTime(2025, 01, 15, 10, 0, 0));

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CanPurchase(999, 50));
        }

        [Fact]
        public async Task CanPurchase_ShouldReturnFalse_WhenCustomerPurchasedLastMonth()
        {
            var ctx = CreateInMemoryDb();
            purgeCustomers(ctx);

            var customer = new Customer { Id = 1, Name = "Test" };
            ctx.Customers.Add(customer);
            ctx.Orders.Add(new Order
            {
                CustomerId = 1,
                OrderDate = DateTime.UtcNow.AddDays(-5)
            });
            await ctx.SaveChangesAsync();

            var service = BuildService(ctx, DateTime.UtcNow);

            var result = await service.CanPurchase(1, 50);

            Assert.False(result);
        }

        [Fact]
        public async Task CanPurchase_ShouldReturnFalse_WhenFirstPurchaseAbove100()
        {
            var ctx = CreateInMemoryDb();
            purgeCustomers(ctx);

            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });
            await ctx.SaveChangesAsync();

            var service = BuildService(ctx, new DateTime(2025, 01, 15, 10, 0, 0));

            var result = await service.CanPurchase(1, 200);

            Assert.False(result);
        }

        [Fact]
        public async Task CanPurchase_ShouldReturnFalse_WhenOutsideBusinessHours()
        {
            var ctx = CreateInMemoryDb();
            purgeCustomers(ctx);

            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });
            await ctx.SaveChangesAsync();

            // Domingo às 10h
            var service = BuildService(ctx, new DateTime(2025, 01, 19, 10, 0, 0));

            var result = await service.CanPurchase(1, 50);

            Assert.False(result);
        }

        [Fact]
        public async Task CanPurchase_ShouldReturnTrue_WhenAllRulesPass()
        {
            var ctx = CreateInMemoryDb();
            purgeCustomers(ctx);

            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });
            await ctx.SaveChangesAsync();

            // Quarta-feira às 10h
            var service = BuildService(ctx, new DateTime(2025, 01, 15, 10, 0, 0));

            var result = await service.CanPurchase(1, 50);

            Assert.True(result);
        }
    }
}
