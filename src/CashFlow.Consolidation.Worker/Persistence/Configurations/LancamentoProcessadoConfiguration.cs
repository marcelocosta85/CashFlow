using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CashFlow.Consolidation.Worker.Persistence.Configurations;

public class LancamentoProcessadoConfiguration : IEntityTypeConfiguration<LancamentoProcessado>
{
    public void Configure(EntityTypeBuilder<LancamentoProcessado> builder)
    {
        builder.ToTable("lancamentos_processados");

        builder.HasKey(l => l.LancamentoId);

        builder.Property(l => l.LancamentoId)
            .ValueGeneratedNever();

        builder.Property(l => l.ProcessadoEm)
            .IsRequired();
    }
}
