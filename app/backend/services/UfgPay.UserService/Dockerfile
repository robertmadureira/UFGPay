FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY proto/ ./proto/
COPY app/backend/services/UfgPay.UserService/ ./app/backend/services/UfgPay.UserService/

WORKDIR /src/app/backend/services/UfgPay.UserService
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001
ENTRYPOINT ["dotnet", "UfgPay.UserService.dll"]
