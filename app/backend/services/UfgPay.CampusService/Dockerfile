FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY proto/ ./proto/
COPY app/backend/services/UfgPay.CampusService/ ./app/backend/services/UfgPay.CampusService/

WORKDIR /src/app/backend/services/UfgPay.CampusService
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5003
EXPOSE 5003
ENTRYPOINT ["dotnet", "UfgPay.CampusService.dll"]
