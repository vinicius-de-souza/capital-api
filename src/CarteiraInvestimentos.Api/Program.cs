using CarteiraInvestimentos.Api.Middleware;
using CarteiraInvestimentos.Application.Commands.ComprarAtivo;
using CarteiraInvestimentos.Domain.Interfaces;
using CarteiraInvestimentos.Domain.Interfaces.Repositories;
using CarteiraInvestimentos.Infrastructure.Data;
using CarteiraInvestimentos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CarteiraInvestimentosContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ComprarAtivoCommand).Assembly));

builder.Services.AddScoped<IRepositorioAtivo, RepositorioAtivo>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

// Swagger habilitado em todos os ambientes para fins de showcase
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

// Aplicar migrações pendentes do banco de dados ao iniciar o aplicativo
try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<CarteiraInvestimentosContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Verificando e aplicando migrações do banco de dados...");
        context.Database.Migrate();

        logger.LogInformation("Migrações do banco de dados aplicadas com sucesso.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Ocorreu um erro ao aplicar as migrações do banco de dados.");
}

app.Run();