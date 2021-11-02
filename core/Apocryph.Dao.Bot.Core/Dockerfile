#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["core/Apocryph.Dao.Bot.Core/Apocryph.Dao.Bot.Core.csproj", "core/Apocryph.Dao.Bot.Core/"]
RUN dotnet restore "core/Apocryph.Dao.Bot.Core/Apocryph.Dao.Bot.Core.csproj"
COPY . .
WORKDIR "/src/core/Apocryph.Dao.Bot.Core"
RUN dotnet build "Apocryph.Dao.Bot.Core.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Apocryph.Dao.Bot.Core.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Apocryph.Dao.Bot.Core.dll"]