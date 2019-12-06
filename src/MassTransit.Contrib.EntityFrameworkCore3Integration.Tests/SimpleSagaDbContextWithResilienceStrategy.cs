using Microsoft.EntityFrameworkCore;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests
{
    public class SimpleSagaDbContextWithResilienceStrategy : SimpleSagaDbContext
    {
        public SimpleSagaDbContextWithResilienceStrategy(DbContextOptions options)
            : base(options)
        {

        }
    }
}
