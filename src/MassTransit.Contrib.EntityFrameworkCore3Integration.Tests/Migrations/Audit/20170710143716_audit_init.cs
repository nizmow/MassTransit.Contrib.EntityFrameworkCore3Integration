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
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MassTransit.Contrib.EntityFrameworkCore3Integration.Tests.Migrations.Audit
{
    public partial class audit_init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EfCoreAudit",
                columns: table => new
                {
                    AuditRecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContextType = table.Column<string>(nullable: true),
                    ConversationId = table.Column<Guid>(nullable: true),
                    CorrelationId = table.Column<Guid>(nullable: true),
                    DestinationAddress = table.Column<string>(nullable: true),
                    FaultAddress = table.Column<string>(nullable: true),
                    InputAddress = table.Column<string>(nullable: true),
                    InitiatorId = table.Column<Guid>(nullable: true),
                    MessageId = table.Column<Guid>(nullable: true),
                    MessageType = table.Column<string>(nullable: true),
                    RequestId = table.Column<Guid>(nullable: true),
                    ResponseAddress = table.Column<string>(nullable: true),
                    SourceAddress = table.Column<string>(nullable: true),
                    Custom = table.Column<string>(nullable: true),
                    Headers = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfCoreAudit", x => x.AuditRecordId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EfCoreAudit");
        }
    }
}
