FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PaymentPlatform.sln", "."]
COPY ["src/PaymentPlatform.Api/PaymentPlatform.Api.csproj", "src/PaymentPlatform.Api/"]
COPY ["src/PaymentPlatform.Application/PaymentPlatform.Application.csproj", "src/PaymentPlatform.Application/"]
COPY ["src/PaymentPlatform.Contracts/PaymentPlatform.Contracts.csproj", "src/PaymentPlatform.Contracts/"]
COPY ["src/PaymentPlatform.Domain/PaymentPlatform.Domain.csproj", "src/PaymentPlatform.Domain/"]
COPY ["src/PaymentPlatform.Infrastructure/PaymentPlatform.Infrastructure.csproj", "src/PaymentPlatform.Infrastructure/"]
COPY ["src/PaymentPlatform.BackgroundWorkers/PaymentPlatform.BackgroundWorkers.csproj", "src/PaymentPlatform.BackgroundWorkers/"]
RUN dotnet restore "src/PaymentPlatform.Api/PaymentPlatform.Api.csproj"
COPY . .
WORKDIR "/src/src/PaymentPlatform.Api"
RUN dotnet publish "PaymentPlatform.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaymentPlatform.Api.dll"]
