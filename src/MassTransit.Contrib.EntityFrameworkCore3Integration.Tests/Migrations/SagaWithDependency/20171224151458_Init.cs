﻿// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MassTransit.EntityFrameworkCore3Integration.Tests.Migrations.SagaWithDependency
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SagaInnerDependency",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SagaInnerDependency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SagaDependency",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SagaInnerDependencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SagaDependency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SagaDependency_SagaInnerDependency_SagaInnerDependencyId",
                        column: x => x.SagaInnerDependencyId,
                        principalTable: "SagaInnerDependency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EfCoreSagasWithDepencies",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    DependencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Initiated = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfCoreSagasWithDepencies", x => x.CorrelationId);
                    table.ForeignKey(
                        name: "FK_EfCoreSagasWithDepencies_SagaDependency_DependencyId",
                        column: x => x.DependencyId,
                        principalTable: "SagaDependency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EfCoreSagasWithDepencies_DependencyId",
                table: "EfCoreSagasWithDepencies",
                column: "DependencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SagaDependency_SagaInnerDependencyId",
                table: "SagaDependency",
                column: "SagaInnerDependencyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EfCoreSagasWithDepencies");

            migrationBuilder.DropTable(
                name: "SagaDependency");

            migrationBuilder.DropTable(
                name: "SagaInnerDependency");
        }
    }
}
