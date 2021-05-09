
FROM public.ecr.aws/lambda/dotnet:5.0 AS base

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim as build
WORKDIR /src
COPY ["BattleAuth.Api/BattleAuth.Api.csproj", "BattleAuth.Api/"]
RUN dotnet restore "BattleAuth.Api/BattleAuth.Api.csproj"

COPY . .
WORKDIR "/src/BattleAuth.Api"
RUN dotnet build "BattleAuth.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BattleAuth.Api.csproj" -c Release --runtime linux-x64 --self-contained false -p:PublishReadyToRun=true -o /app/publish

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .