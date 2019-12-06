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

namespace MassTransit.EntityFrameworkCore3Integration.Tests.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EfCoreSimpleSagas",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(nullable: false),
                    Completed = table.Column<bool>(nullable: false),
                    Initiated = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 40, nullable: true),
                    Observed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfCoreSimpleSagas", x => x.CorrelationId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EfCoreSimpleSagas");
        }
    }
}
