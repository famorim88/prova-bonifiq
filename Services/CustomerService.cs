using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services.Interfaces;
using ProvaPub.Validators;

namespace ProvaPub.Services
{
    public class CustomerService : ICustomerService
    {
        TestDbContext _ctx;
        private readonly IEnumerable<IPurchaseRule> _rules;


        public CustomerService(TestDbContext ctx, IEnumerable<IPurchaseRule> rules)
        {
            _ctx = ctx;
            _rules = rules;
        }

        public Paginated<Customer> ListCustomers(int page)
        {
            const int pageSize = 10;

            var totalCount = _ctx.Products.Count();

            var customers = _ctx.Customers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new Paginated<Customer>
            {
                HasNext = page * pageSize < totalCount,
                TotalCount = totalCount,
                Data = customers
            };
        }

        public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
        {
            if (customerId <= 0)
                throw new ArgumentOutOfRangeException(nameof(customerId));

            if (purchaseValue <= 0)
                throw new ArgumentOutOfRangeException(nameof(purchaseValue));

            foreach (var rule in _rules)
            {
                if (!await rule.Validate(customerId, purchaseValue))
                    return false;
            }
            return true;
        }

    }
}
