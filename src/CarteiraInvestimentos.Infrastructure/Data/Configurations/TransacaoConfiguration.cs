using CarteiraInvestimentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarteiraInvestimentos.Infrastructure.Data.Configurations;

public class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
{
    public void Configure(EntityTypeBuilder<Transacao> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasQueryFilter(t => true);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.Property(t => t.CodigoAtivo).IsRequired().HasMaxLength(10);
        builder.Property(t => t.PrecoUnitario).HasColumnType("decimal(18,2)");
    }
}