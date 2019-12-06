using System.Collections.Generic;
using MassTransit.Contrib.EntityFrameworkCore3Integration.Mappings;
using Microsoft.EntityFrameworkCore;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests
{
    public class SimpleSagaDbContext : SagaDbContext
    {
        public SimpleSagaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new SimpleSagaMap(); }
        }
    }
}
