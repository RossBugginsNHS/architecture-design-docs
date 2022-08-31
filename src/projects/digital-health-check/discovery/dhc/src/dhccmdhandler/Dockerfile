FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app


# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["dhcapi/dhcapi.csproj", "./dhcapi/"]
COPY ["bmiclient/bmiclient.csproj", "./bmiclient/"]
COPY ["bpclient/bpclient.csproj", "./bpclient/"]
COPY ["dhc/dhc.csproj", "./dhc/"]
COPY ["dhccmdhandler/dhccmdhandler.csproj", "./dhccmdhandler/"]
RUN dotnet restore "dhccmdhandler/dhccmdhandler.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "dhccmdhandler/dhccmdhandler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dhccmdhandler/dhccmdhandler.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dhccmdhandler.dll"]
