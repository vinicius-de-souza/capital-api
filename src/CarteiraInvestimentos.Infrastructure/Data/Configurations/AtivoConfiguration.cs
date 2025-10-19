using CarteiraInvestimentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarteiraInvestimentos.Infrastructure.Data.Configurations;

public class AtivoConfiguration : IEntityTypeConfiguration<Ativo>
{
    public void Configure(EntityTypeBuilder<Ativo> builder)
    {
        builder.HasKey(a => a.Codigo);

        builder.Property(a => a.Codigo).IsRequired().HasMaxLength(10);
        builder.Property(a => a.PrecoMedioCompra).HasColumnType("decimal(18,2)");

        builder.HasMany(a => a.Transacoes)
               .WithOne()
               .HasForeignKey("AtivoCodigo");

        var navigation = builder.Metadata.FindNavigation(nameof(Ativo.Transacoes));

        if (navigation == null)
        {
            throw new InvalidOperationException(
                $"A propriedade de navegação '{nameof(Ativo.Transacoes)}' não foi encontrada na entidade '{nameof(Ativo)}'. Verifique a configuração.");
        }

        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}