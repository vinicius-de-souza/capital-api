FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/CarteiraInvestimentos.Api/CarteiraInvestimentos.Api.csproj src/CarteiraInvestimentos.Api/
COPY src/CarteiraInvestimentos.Application/CarteiraInvestimentos.Application.csproj src/CarteiraInvestimentos.Application/
COPY src/CarteiraInvestimentos.Domain/CarteiraInvestimentos.Domain.csproj src/CarteiraInvestimentos.Domain/
COPY src/CarteiraInvestimentos.Infrastructure/CarteiraInvestimentos.Infrastructure.csproj src/CarteiraInvestimentos.Infrastructure/

RUN dotnet restore "src/CarteiraInvestimentos.Api/CarteiraInvestimentos.Api.csproj"

COPY . .

RUN dotnet publish "src/CarteiraInvestimentos.Api/CarteiraInvestimentos.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CarteiraInvestimentos.Api.dll"]