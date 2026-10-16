FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY proto/ ./proto/
COPY app/backend/services/UfgPay.WalletService/ ./app/backend/services/UfgPay.WalletService/

WORKDIR /src/app/backend/services/UfgPay.WalletService
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5002
EXPOSE 5002
ENTRYPOINT ["dotnet", "UfgPay.WalletService.dll"]
