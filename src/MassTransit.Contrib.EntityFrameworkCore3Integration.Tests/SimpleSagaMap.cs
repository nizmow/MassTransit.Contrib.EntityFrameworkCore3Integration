using MassTransit.Contrib.EntityFrameworkCore3Integration.Mappings;
using MassTransit.Contrib.EntityFrameworkCore3Integration.Tests.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests
{
    class SimpleSagaMap : SagaClassMap<SimpleSaga>
    {
        protected override void Configure(EntityTypeBuilder<SimpleSaga> entityTypeBuilder, ModelBuilder modelBuilder)
        {
            entityTypeBuilder.Property(x => x.Name).HasMaxLength(40);
            entityTypeBuilder.Property(x => x.Initiated);
            entityTypeBuilder.Property(x => x.Observed);
            entityTypeBuilder.Property(x => x.Completed);

            entityTypeBuilder.ToTable("EfCoreSimpleSagas");
        }
    }
}
