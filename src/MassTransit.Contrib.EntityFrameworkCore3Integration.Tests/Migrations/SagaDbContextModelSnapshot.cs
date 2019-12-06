using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests.Migrations
{
    [DbContext(typeof(SimpleSagaDbContext))]
    partial class SagaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MassTransit.Tests.Saga.SimpleSaga", b =>
                {
                    b.Property<Guid>("CorrelationId");

                    b.Property<bool>("Completed");

                    b.Property<bool>("Initiated");

                    b.Property<string>("Name")
                        .HasMaxLength(40);

                    b.Property<bool>("Observed");

                    b.HasKey("CorrelationId");

                    b.ToTable("EfCoreSimpleSagas");
                });
        }
    }
}
