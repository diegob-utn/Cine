# Build stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Cine.Api/Cine.Api.csproj", "Cine.Api/"]
COPY ["Cine.Modelos/Cine.Modelos.csproj", "Cine.Modelos/"]
RUN dotnet restore "Cine.Api/Cine.Api.csproj"
COPY . .
WORKDIR "/src/Cine.Api"
RUN dotnet build "Cine.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cine.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cine.Api.dll"]