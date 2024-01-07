FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Mavanmanen.Discord.ArnhemBot/Mavanmanen.Discord.ArnhemBot.csproj", "Mavanmanen.Discord.ArnhemBot/"]
RUN dotnet restore "Mavanmanen.Discord.ArnhemBot/Mavanmanen.Discord.ArnhemBot.csproj"
COPY . .
WORKDIR "/src/Mavanmanen.Discord.ArnhemBot"
RUN dotnet build "Mavanmanen.Discord.ArnhemBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Mavanmanen.Discord.ArnhemBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mavanmanen.Discord.ArnhemBot.dll"]
