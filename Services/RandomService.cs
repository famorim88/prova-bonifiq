using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services
{
	public class RandomService : IRandomService
    {
        TestDbContext _ctx;
		public RandomService(TestDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<int> GetRandom()
		{
            var rnd = new Random();
            int number;
            do
            {
                number = rnd.Next(100);
            }
            while (_ctx.Numbers.Any(n => n.Number == number));
            _ctx.Numbers.Add(new RandomNumber { Number = number });
            await _ctx.SaveChangesAsync();
            return number;
        }

	}
}
