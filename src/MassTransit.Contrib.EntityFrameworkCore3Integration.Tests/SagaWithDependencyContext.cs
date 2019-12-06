using System.Collections.Generic;
using MassTransit.Contrib.EntityFrameworkCore3Integration.Mappings;
using Microsoft.EntityFrameworkCore;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests
{
    public class SagaWithDependencyContext : SagaDbContext
    {
        public SagaWithDependencyContext(DbContextOptions<SagaWithDependencyContext> options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new SagaWithDependencyMap(); }
        }
    }
}
