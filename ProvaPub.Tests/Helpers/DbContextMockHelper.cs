using Microsoft.EntityFrameworkCore;
using ProvaPub.Helpers;
using ProvaPub.Repository;

namespace ProvaPub.Tests
{
    public static class DbContextHelper
    {
        public static TestDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new TestDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }
        public class FixedDateTimeProvider : IDateTimeProvider
        {
            private readonly DateTime _dateTime;
            public FixedDateTimeProvider(DateTime dateTime) => _dateTime = dateTime;
            public DateTime UtcNow => _dateTime;
        }
    }
}
