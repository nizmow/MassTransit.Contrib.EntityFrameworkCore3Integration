using System;
using MassTransit.Contrib.EntityFrameworkCore3Integration.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MassTransit.EntityFrameworkCore3Integration.Tests.Migrations
{
    using EntityFrameworkCore3Integration.Tests;


    [DbContext(typeof(SimpleSagaDbContext))]
    [Migration("20170710150441_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
