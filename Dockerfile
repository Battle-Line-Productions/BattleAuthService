#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BattleAuth.Api/BattleAuth.Api.csproj", "BattleAuth.Api/"]
RUN dotnet restore "BattleAuth.Api/BattleAuth.Api.csproj"
COPY . .
WORKDIR "/src/BattleAuth.Api"
RUN dotnet build "BattleAuth.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BattleAuth.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BattleAuth.Api.dll"]