###############################################################################
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY . .

RUN dotnet build -c Release -o /app/build
RUN dotnet test

###############################################################################
FROM build AS publish
RUN dotnet publish BattleShips.Runner/BattleShips.Runner.csproj --self-contained -r linux-x64 -c Release -o /app/publish

###############################################################################
FROM amazonlinux:2 AS make-image
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["./BattleShips.Runner"]