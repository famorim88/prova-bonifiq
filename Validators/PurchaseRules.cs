using Microsoft.EntityFrameworkCore;
using ProvaPub.Helpers;
using ProvaPub.Repository;

namespace ProvaPub.Validators
{
    public interface IPurchaseRule
    {
        Task<bool> Validate(int customerId, decimal purchaseValue);
    }

    public class PurchaseRules : IPurchaseRule
    {
        private readonly TestDbContext _ctx;
        public PurchaseRules(TestDbContext ctx) => _ctx = ctx;

        public async Task<bool> Validate(int customerId, decimal purchaseValue)
        {
            var customer = await _ctx.Customers.FindAsync(customerId);
            if (customer == null)
                throw new InvalidOperationException($"Customer Id {customerId} does not exist");
            return true;
        }
    }
    public class CustomerExistsRule : IPurchaseRule
    {
        private readonly TestDbContext _ctx;
        public CustomerExistsRule(TestDbContext ctx) => _ctx = ctx;

        public async Task<bool> Validate(int customerId, decimal purchaseValue)
        {
            var customer = await _ctx.Customers.FindAsync(customerId);
            if (customer == null)
                throw new InvalidOperationException($"Customer Id {customerId} does not exist");
            return true;
        }
    }

    public class OnePurchasePerMonthRule : IPurchaseRule
    {
        private readonly TestDbContext _ctx;
        private readonly IDateTimeProvider _dateTime;
        public OnePurchasePerMonthRule(TestDbContext ctx, IDateTimeProvider dateTime)
        {
            _ctx = ctx;
            _dateTime = dateTime;
        }

        public async Task<bool> Validate(int customerId, decimal purchaseValue)
        {
            var baseDate = _dateTime.UtcNow.AddMonths(-1);
            var count = await _ctx.Orders.CountAsync(s => s.CustomerId == customerId && s.OrderDate >= baseDate);
            return count == 0;
        }
    }

    public class FirstPurchaseMaxValueRule : IPurchaseRule
    {
        private readonly TestDbContext _ctx;
        public FirstPurchaseMaxValueRule(TestDbContext ctx) => _ctx = ctx;

        public async Task<bool> Validate(int customerId, decimal purchaseValue)
        {
            var haveBoughtBefore = await _ctx.Customers
                .CountAsync(s => s.Id == customerId && s.Orders.Any());

            if (haveBoughtBefore == 0 && purchaseValue > 100)
                return false;

            return true;
        }
    }

    public class BusinessHoursRule : IPurchaseRule
    {
        private readonly IDateTimeProvider _dateTime;
        public BusinessHoursRule(IDateTimeProvider dateTime) => _dateTime = dateTime;

        public Task<bool> Validate(int customerId, decimal purchaseValue)
        {
            var now = _dateTime.UtcNow;
            if (now.Hour < 8 || now.Hour > 18 || now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
                return Task.FromResult(false);

            return Task.FromResult(true);
        }
    }

}
