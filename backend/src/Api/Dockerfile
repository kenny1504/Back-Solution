# ---- build ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos TODO el backend porque Api referencia Application/Infrastructure/Domain
COPY ./ ./

# Restore + publish (sin AppHost para imágenes más chicas)
RUN dotnet restore ./src/Api/Api.csproj
RUN dotnet publish ./src/Api/Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8088

ENTRYPOINT ["dotnet", "Api.dll"]
