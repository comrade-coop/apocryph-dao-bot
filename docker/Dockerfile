#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs
WORKDIR /src
COPY ["src/Apocryph.Dao.Bot/Apocryph.Dao.Bot.csproj", "src/Apocryph.Dao.Bot/"]
COPY ["perper/agent/dotnet/src/Perper/Perper.csproj", "perper/agent/dotnet/src/Perper/"]
RUN dotnet restore "src/Apocryph.Dao.Bot/Apocryph.Dao.Bot.csproj"
COPY . .
WORKDIR "/src/src/Apocryph.Dao.Bot"
RUN dotnet build "Apocryph.Dao.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Apocryph.Dao.Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Apocryph.Dao.Bot.dll"]