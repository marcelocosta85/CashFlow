using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CashFlow.Consolidation.Worker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLancamentosProcessados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lancamentos_processados",
                schema: "consolidation",
                columns: table => new
                {
                    LancamentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lancamentos_processados", x => x.LancamentoId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lancamentos_processados",
                schema: "consolidation");
        }
    }
}
