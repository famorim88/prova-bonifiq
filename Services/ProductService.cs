using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services
{
	public class ProductService : IProductService
    {
		TestDbContext _ctx;

		public ProductService(TestDbContext ctx)
		{
			_ctx = ctx;
		}

		public Paginated<Product>  ListProducts(int page)
		{
            const int pageSize = 10;

            var totalCount = _ctx.Products.Count();

            var products = _ctx.Products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new Paginated<Product>
            {
                HasNext = page * pageSize < totalCount,
                TotalCount = totalCount,
                Data = products
            };
        }

	}
}
