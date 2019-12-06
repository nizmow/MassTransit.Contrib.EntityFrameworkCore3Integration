using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests
{
    public class ContextFactory : IDesignTimeDbContextFactory<SimpleSagaDbContext>
    {
        public SimpleSagaDbContext CreateDbContext(string[] args)
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<SimpleSagaDbContext>();

            dbContextOptionsBuilder.UseSqlServer(LocalDbConnectionStringProvider.GetLocalDbConnectionString(),
                m =>
                {
                    var executingAssembly = typeof(ContextFactory).GetTypeInfo().Assembly;
                    m.MigrationsAssembly(executingAssembly.GetName().Name);
                });

            return new SimpleSagaDbContext(dbContextOptionsBuilder.Options);
        }
    }
}
