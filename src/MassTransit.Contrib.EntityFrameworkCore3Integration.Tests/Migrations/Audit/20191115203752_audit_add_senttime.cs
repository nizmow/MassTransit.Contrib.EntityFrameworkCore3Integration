namespace MassTransit.EntityFrameworkCore3Integration.Tests.Migrations.Audit
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class audit_add_senttime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SentTime",
                table: "EfCoreAudit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SentTime",
                table: "EfCoreAudit");
        }
    }
}
