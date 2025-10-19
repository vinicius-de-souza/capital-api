using CarteiraInvestimentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CarteiraInvestimentos.Infrastructure.Data;

public class CarteiraInvestimentosContext : DbContext
{
    public CarteiraInvestimentosContext(DbContextOptions<CarteiraInvestimentosContext> options) : base(options)
    {
    }

    public DbSet<Ativo> Ativos { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}