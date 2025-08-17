using ProvaPub.Models;

namespace ProvaPub.Services.Interfaces
{
    public interface IProductService
    {
        Paginated<Product> ListProducts(int page);
    }
}
