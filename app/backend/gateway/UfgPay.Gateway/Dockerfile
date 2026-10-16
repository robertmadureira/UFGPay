FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY proto/ ./proto/
COPY app/backend/gateway/UfgPay.Gateway/ ./app/backend/gateway/UfgPay.Gateway/

WORKDIR /src/app/backend/gateway/UfgPay.Gateway
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "UfgPay.Gateway.dll"]
