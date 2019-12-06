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
using Microsoft.EntityFrameworkCore.Migrations;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests.Migrations.Audit
{
    [DbContext(typeof(AuditDbContext))]
    [Migration("20170710143716_audit_init")]
    partial class audit_init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.Audit.AuditRecord", b =>
                {
                    b.Property<int>("AuditRecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContextType");

                    b.Property<Guid?>("ConversationId");

                    b.Property<Guid?>("CorrelationId");

                    b.Property<string>("DestinationAddress");

                    b.Property<string>("FaultAddress");

                    b.Property<string>("InputAddress");

                    b.Property<Guid?>("InitiatorId");

                    b.Property<Guid?>("MessageId");

                    b.Property<string>("MessageType");

                    b.Property<Guid?>("RequestId");

                    b.Property<string>("ResponseAddress");

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
        }
    }
}
