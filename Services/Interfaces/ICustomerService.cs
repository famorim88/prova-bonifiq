using ProvaPub.Models;

namespace ProvaPub.Services.Interfaces
{
    public interface ICustomerService
    {
        Paginated<Customer> ListCustomers(int page);
        Task<bool> CanPurchase(int customerId, decimal purchaseValue);
    }
}
