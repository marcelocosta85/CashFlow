using CashFlow.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Consolidation.Worker.Persistence.Configurations;

public class SaldoDiarioConfiguration : IEntityTypeConfiguration<SaldoDiario>
{
    public void Configure(EntityTypeBuilder<SaldoDiario> builder)
    {
        builder.ToTable("saldos_diarios");

        builder.HasKey(s => s.Data);

        builder.Property(s => s.Data)
            .HasColumnType("date")
            .ValueGeneratedNever();

        builder.Property(s => s.TotalCreditos)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(s => s.TotalDebitos)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Ignore(s => s.SaldoConsolidado);
    }
}
