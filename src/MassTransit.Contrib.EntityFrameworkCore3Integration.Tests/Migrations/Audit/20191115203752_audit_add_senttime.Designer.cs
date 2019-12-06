﻿// <auto-generated />
using System;
using MassTransit.Contrib.EntityFrameworkCore3Integration.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MassTransit.EntityFrameworkCore3Integration.Tests.Migrations.Audit
{
    [DbContext(typeof(AuditDbContext))]
    [Migration("20191115203752_audit_add_senttime")]
    partial class audit_add_senttime
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
