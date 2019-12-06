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
using MassTransit.Contrib.EntityFrameworkCore3Integration.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests.Migrations.Audit
{
    [DbContext(typeof(AuditDbContext))]
    partial class AuditDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.Audit.AuditRecord", b =>
                {
                    b.Property<int>("AuditRecordId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ContextType");

                    b.Property<Guid?>("ConversationId");

                    b.Property<Guid?>("CorrelationId");

                    b.Property<string>("DestinationAddress");

                    b.Property<string>("FaultAddress");

                    b.Property<Guid?>("InitiatorId");

                    b.Property<string>("InputAddress");

                    b.Property<Guid?>("MessageId");

                    b.Property<string>("MessageType");

                    b.Property<Guid?>("RequestId");

                    b.Property<string>("ResponseAddress");

                    b.Property<DateTime?>("SentTime");

                    b.Property<string>("SourceAddress");

                    b.Property<string>("_custom")
                        .HasColumnName("Custom");

                    b.Property<string>("_headers")
                        .HasColumnName("Headers");

                    b.Property<string>("_message")
                        .HasColumnName("Message");

                    b.HasKey("AuditRecordId");

                    b.ToTable("EfCoreAudit");
                });
#pragma warning restore 612, 618
        }
    }
}
