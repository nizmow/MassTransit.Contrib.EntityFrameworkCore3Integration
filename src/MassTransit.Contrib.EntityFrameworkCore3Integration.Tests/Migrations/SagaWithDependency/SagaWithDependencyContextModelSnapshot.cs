// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
// Modifications copyright 2019 Neil Houghton.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests.Migrations.SagaWithDependency
{
    [DbContext(typeof(SagaWithDependencyContext))]
    partial class SagaWithDependencyContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.Tests.SagaDependency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("SagaInnerDependencyId");

                    b.HasKey("Id");

                    b.HasIndex("SagaInnerDependencyId");

                    b.ToTable("SagaDependency");
                });

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.Tests.SagaInnerDependency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("SagaInnerDependency");
                });

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.Tests.SagaWithDependency", b =>
                {
                    b.Property<Guid>("CorrelationId");

                    b.Property<bool>("Completed");

                    b.Property<Guid>("DependencyId");

                    b.Property<bool>("Initiated");

                    b.Property<string>("Name")
                        .HasMaxLength(40);

                    b.HasKey("CorrelationId");

                    b.HasIndex("DependencyId");

                    b.ToTable("EfCoreSagasWithDepencies");
                });

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.Tests.SagaDependency", b =>
                {
                    b.HasOne("MassTransit.EntityFrameworkCoreIntegration.Tests.SagaInnerDependency", "SagaInnerDependency")
                        .WithMany()
                        .HasForeignKey("SagaInnerDependencyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.Tests.SagaWithDependency", b =>
                {
                    b.HasOne("MassTransit.EntityFrameworkCoreIntegration.Tests.SagaDependency", "Dependency")
                        .WithMany()
                        .HasForeignKey("DependencyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
