using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.CrossChainTransfers.MsSqlRepositories.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cross_chain_transfers");

            migrationBuilder.CreateTable(
                name: "deduplication_log",
                schema: "cross_chain_transfers",
                columns: table => new
                {
                    deduplication_id = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deduplication_log", x => x.deduplication_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deduplication_log",
                schema: "cross_chain_transfers");
        }
    }
}
