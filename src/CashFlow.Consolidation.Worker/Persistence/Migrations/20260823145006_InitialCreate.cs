using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CashFlow.Consolidation.Worker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "consolidation");

            migrationBuilder.CreateTable(
                name: "saldos_diarios",
                schema: "consolidation",
                columns: table => new
                {
                    Data = table.Column<DateTime>(type: "date", nullable: false),
                    TotalCreditos = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalDebitos = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_saldos_diarios", x => x.Data);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "saldos_diarios",
                schema: "consolidation");
        }
    }
}
